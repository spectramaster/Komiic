using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using BioChroma.Core.Interfaces;
using BioChroma.Core.Models;

namespace BioChroma.Core.Encoding
{
    /// <summary>
    /// Core BioChroma encoding engine
    /// Converts biometric features and data into 3D particle codes
    /// </summary>
    public class BioChromaEncoder : IEncoder
    {
        private const int DEFAULT_PARTICLE_COUNT = 256;
        private const float PARTICLE_SPREAD = 0.8f; // Particles use 80% of the space
        private const int MAX_DATA_SIZE = 10 * 1024 * 1024; // 10MB limit to prevent DoS
        private readonly string _encryptionKey;

        /// <summary>
        /// Creates a new BioChromaEncoder with a custom encryption key
        /// </summary>
        /// <param name="encryptionKey">Encryption key for securing data. If null, will attempt to read from environment variable BIOCHROMA_KEY</param>
        public BioChromaEncoder(string? encryptionKey = null)
        {
            _encryptionKey = encryptionKey
                ?? Environment.GetEnvironmentVariable("BIOCHROMA_KEY")
                ?? "BioChromaDefaultKey"; // WARNING: Default key is insecure for production use!

            if (_encryptionKey == "BioChromaDefaultKey")
            {
                // Log warning in production scenarios
                System.Diagnostics.Debug.WriteLine("WARNING: Using default encryption key. Set BIOCHROMA_KEY environment variable or pass custom key for production use.");
            }
        }

        public async Task<BioChromaCode> EncodeAsync(BiometricFeatures features, string userId)
        {
            // Input validation
            if (features == null)
                throw new ArgumentNullException(nameof(features));
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));

            // Create payload
            var payload = new
            {
                userId = userId,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                biometricHash = features.CalculateHash(),
                nonce = GenerateNonce()
            };

            // Serialize and encrypt
            var json = JsonSerializer.Serialize(payload);
            var data = Encoding.UTF8.GetBytes(json);
            var encrypted = await EncryptDataAsync(data);

            // Encode into particles
            return await EncodeDataAsync(encrypted, features);
        }

        public async Task<BioChromaCode> EncodeDataAsync(byte[] data, BiometricFeatures? features = null)
        {
            // Input validation
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.Length == 0)
                throw new ArgumentException("Data cannot be empty", nameof(data));
            if (data.Length > MAX_DATA_SIZE)
                throw new ArgumentException($"Data size exceeds maximum allowed size of {MAX_DATA_SIZE} bytes", nameof(data));

            return await Task.Run(() =>
            {
                var code = new BioChromaCode
                {
                    Version = "1.0",
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Nonce = GenerateNonce()
                };

                // Calculate number of particles needed
                int particleCount = Math.Max(data.Length, DEFAULT_PARTICLE_COUNT);

                // Generate particles from data
                for (int i = 0; i < particleCount; i++)
                {
                    var particle = GenerateParticleFromByte(i, data, features);
                    code.Particles.Add(particle);
                }

                // Generate topology (connection edges)
                code.Edges = GenerateTopology(code.Particles);

                // Apply biometric influence if available
                if (features != null)
                {
                    ApplyBiometricInfluence(code, features);
                }

                return code;
            });
        }

        private Particle3D GenerateParticleFromByte(int index, byte[] data, BiometricFeatures? features)
        {
            byte value = index < data.Length ? data[index] : (byte)0;

            // Position encoding: use byte value to determine X, Y
            float x = (value & 0x0F) / 15.0f * PARTICLE_SPREAD + (1 - PARTICLE_SPREAD) / 2;
            float y = ((value >> 4) & 0x0F) / 15.0f * PARTICLE_SPREAD + (1 - PARTICLE_SPREAD) / 2;
            float z = HashByte(value, index) / 255.0f * PARTICLE_SPREAD + (1 - PARTICLE_SPREAD) / 2;

            // Color encoding: derive from byte value
            float hue = (value * 1.5f) % 360;
            float saturation = 0.7f + (value % 30) / 100.0f;
            float brightness = 0.6f + (value % 40) / 100.0f;

            // Dynamic properties
            float rotationSpeed = (value % 10) / 10.0f;
            float phase = (value % 360);

            var particle = new Particle3D(index, x, y, z)
            {
                Hue = hue,
                Saturation = saturation,
                Value = brightness,
                RotationSpeed = rotationSpeed,
                Phase = phase,
                Size = 0.8f + (value % 50) / 100.0f
            };

            return particle;
        }

        private List<(int, int)> GenerateTopology(List<Particle3D> particles)
        {
            var edges = new List<(int, int)>();

            // Connect nearby particles (simplified Delaunay-like approach)
            const float CONNECTION_THRESHOLD = 0.3f;

            for (int i = 0; i < particles.Count; i++)
            {
                for (int j = i + 1; j < particles.Count; j++)
                {
                    float distance = particles[i].DistanceTo(particles[j]);
                    if (distance < CONNECTION_THRESHOLD)
                    {
                        edges.Add((i, j));
                    }
                }
            }

            return edges;
        }

        private void ApplyBiometricInfluence(BioChromaCode code, BiometricFeatures features)
        {
            // Apply dominant colors from biometric features
            if (features.DominantColors.Count > 0)
            {
                code.GlobalRotationSpeed = 0.8f + features.Quality * 0.4f;
                code.BreathingFrequency = 0.8f + (1.0f - features.Quality) * 0.4f;

                // Shift particle colors based on biometric colors
                for (int i = 0; i < code.Particles.Count && i < features.DominantColors.Count; i++)
                {
                    var (h, s, v) = features.DominantColors[i % features.DominantColors.Count];
                    var particle = code.Particles[i];
                    particle.Hue = (particle.Hue + h) % 360;
                    particle.Saturation = Math.Min(1.0f, particle.Saturation * s);
                    particle.Value = Math.Min(1.0f, particle.Value * v);
                }
            }

            code.BiometricHash = features.CalculateHash();
        }

        private byte HashByte(byte value, int index)
        {
            using (var sha = SHA256.Create())
            {
                var input = new byte[] { value, (byte)(index & 0xFF), (byte)((index >> 8) & 0xFF) };
                var hash = sha.ComputeHash(input);
                return hash[0];
            }
        }

        private string GenerateNonce()
        {
            var bytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Convert.ToBase64String(bytes);
        }

        private async Task<byte[]> EncryptDataAsync(byte[] data)
        {
            return await Task.Run(() =>
            {
                using (var aes = Aes.Create())
                {
                    aes.Key = DeriveKey(_encryptionKey);
                    aes.GenerateIV();

                    using (var encryptor = aes.CreateEncryptor())
                    {
                        var encrypted = encryptor.TransformFinalBlock(data, 0, data.Length);

                        // Prepend IV to encrypted data
                        var result = new byte[aes.IV.Length + encrypted.Length];
                        Buffer.BlockCopy(aes.IV, 0, result, 0, aes.IV.Length);
                        Buffer.BlockCopy(encrypted, 0, result, aes.IV.Length, encrypted.Length);

                        return result;
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
