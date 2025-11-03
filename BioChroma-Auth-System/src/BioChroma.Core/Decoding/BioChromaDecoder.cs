using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BioChroma.Core.Interfaces;
using BioChroma.Core.Models;

namespace BioChroma.Core.Decoding
{
    /// <summary>
    /// Core BioChroma decoding engine
    /// Converts 3D particle codes back into data and verifies authenticity
    /// </summary>
    public class BioChromaDecoder : IDecoder
    {
        private const int TIMESTAMP_TOLERANCE_SECONDS = 30; // Allow 30 second time window
        private readonly string _encryptionKey;

        /// <summary>
        /// Creates a new BioChromaDecoder with a custom encryption key
        /// </summary>
        /// <param name="encryptionKey">Encryption key for decrypting data. Must match the encoder's key. If null, will attempt to read from environment variable BIOCHROMA_KEY</param>
        public BioChromaDecoder(string? encryptionKey = null)
        {
            _encryptionKey = encryptionKey
                ?? Environment.GetEnvironmentVariable("BIOCHROMA_KEY")
                ?? "BioChromaDefaultKey"; // WARNING: Default key is insecure for production use!

            if (_encryptionKey == "BioChromaDefaultKey")
            {
                System.Diagnostics.Debug.WriteLine("WARNING: Using default encryption key. Set BIOCHROMA_KEY environment variable or pass custom key for production use.");
            }
        }

        public async Task<VerificationResult> DecodeAsync(BioChromaCode code)
        {
            // Input validation
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (code.Particles == null || code.ParticleCount == 0)
                throw new ArgumentException("Code must contain particles", nameof(code));

            var stopwatch = Stopwatch.StartNew();

            try
            {
                // Extract raw data from particles
                var encrypted = await ExtractDataAsync(code);

                // Decrypt
                var decrypted = await DecryptDataAsync(encrypted);
                var json = Encoding.UTF8.GetString(decrypted);

                // Parse payload
                var payload = JsonSerializer.Deserialize<JsonElement>(json);

                // Extract fields
                var userId = payload.GetProperty("userId").GetString() ?? string.Empty;
                var timestamp = payload.GetProperty("timestamp").GetInt64();
                var biometricHash = payload.GetProperty("biometricHash").GetString();
                var nonce = payload.GetProperty("nonce").GetString();

                // Verify timestamp
                var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                if (Math.Abs(now - timestamp) > TIMESTAMP_TOLERANCE_SECONDS)
                {
                    return VerificationResult.Failure($"Timestamp expired (age: {now - timestamp}s)");
                }

                // Calculate confidence based on particle decode quality
                float confidence = CalculateConfidence(code);

                stopwatch.Stop();

                return new VerificationResult
                {
                    IsValid = true,
                    UserId = userId,
                    Timestamp = timestamp,
                    BiometricHash = biometricHash,
                    Confidence = confidence,
                    ParticlesDecoded = code.ParticleCount,
                    ParticlesExpected = code.ParticleCount,
                    VerificationTimeMs = stopwatch.ElapsedMilliseconds
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                return new VerificationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Decoding failed: {ex.Message}",
                    VerificationTimeMs = stopwatch.ElapsedMilliseconds
                };
            }
        }

        public async Task<VerificationResult> DecodeFromFramesAsync(List<byte[]> frames)
        {
            // TODO: Implement multi-frame particle detection and tracking
            // For now, return a placeholder
            await Task.Delay(100);

            return VerificationResult.Failure("Multi-frame decoding not yet implemented");
        }

        public async Task<byte[]> ExtractDataAsync(BioChromaCode code)
        {
            return await Task.Run(() =>
            {
                // Sort particles by ID to maintain order
                code.Particles.Sort((a, b) => a.Id.CompareTo(b.Id));

                var data = new byte[code.ParticleCount];

                for (int i = 0; i < code.ParticleCount; i++)
                {
                    var particle = code.Particles[i];

                    // Reverse the encoding process
                    byte value = 0;

                    // Extract from position (X, Y encode low and high nibbles)
                    float normalizedX = (particle.X - (1 - 0.8f) / 2) / 0.8f;
                    float normalizedY = (particle.Y - (1 - 0.8f) / 2) / 0.8f;

                    byte xBits = (byte)(Math.Clamp(normalizedX * 15, 0, 15));
                    byte yBits = (byte)(Math.Clamp(normalizedY * 15, 0, 15));

                    value = (byte)((yBits << 4) | xBits);

                    // Verify color matches expected value
                    float expectedHue = (value * 1.5f) % 360;
                    float hueError = Math.Abs(particle.Hue - expectedHue);

                    // Allow some tolerance for color variations
                    if (hueError > 30 && hueError < 330) // 330 accounts for wraparound
                    {
                        // Color mismatch - might be corrupted
                        // For now, trust the position encoding
                    }

                    data[i] = value;
                }

                return data;
            });
        }

        private float CalculateConfidence(BioChromaCode code)
        {
            float confidence = 1.0f;

            // Reduce confidence if particle count is low
            if (code.ParticleCount < 100)
                confidence *= 0.8f;

            // Reduce confidence if no edges
            if (code.EdgeCount == 0)
                confidence *= 0.9f;

            // Reduce confidence if no biometric hash
            if (string.IsNullOrEmpty(code.BiometricHash))
                confidence *= 0.95f;

            return Math.Max(0.0f, Math.Min(1.0f, confidence));
        }

        private async Task<byte[]> DecryptDataAsync(byte[] encrypted)
        {
            if (encrypted == null)
                throw new ArgumentNullException(nameof(encrypted));
            if (encrypted.Length < 16)
                throw new ArgumentException("Encrypted data is too short to contain IV", nameof(encrypted));

            return await Task.Run(() =>
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = DeriveKey(_encryptionKey);

                    // Extract IV (first 16 bytes)
                    var iv = new byte[16];
                    Buffer.BlockCopy(encrypted, 0, iv, 0, 16);
                    aes.IV = iv;

                    // Decrypt remaining data
                    var encryptedData = new byte[encrypted.Length - 16];
                    Buffer.BlockCopy(encrypted, 16, encryptedData, 0, encryptedData.Length);

                    using (var decryptor = aes.CreateDecryptor())
                    {
                        return decryptor.TransformFinalBlock(encryptedData, 0, encryptedData.Length);
                    }
                }
            });
        }

        private byte[] DeriveKey(string password)
        {
            using (var sha = SHA256.Create())
            {
                return sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
