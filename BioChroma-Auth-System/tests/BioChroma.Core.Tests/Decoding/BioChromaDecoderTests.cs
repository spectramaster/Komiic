using System;
using System.Threading.Tasks;
using Xunit;
using BioChroma.Core.Models;
using BioChroma.Core.Encoding;
using BioChroma.Core.Decoding;

namespace BioChroma.Core.Tests.Decoding
{
    public class BioChromaDecoderTests
    {
        private readonly BioChromaEncoder _encoder;
        private readonly BioChromaDecoder _decoder;

        public BioChromaDecoderTests()
        {
            _encoder = new BioChromaEncoder();
            _decoder = new BioChromaDecoder();
        }

        [Fact]
        public async Task DecodeAsync_ValidCode_ReturnsSuccess()
        {
            // Arrange
            var features = new BiometricFeatures
            {
                FeatureVector = new float[] { 0.5f }
            };
            var code = await _encoder.EncodeAsync(features, "testUser");

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal("testUser", result.UserId);
            Assert.True(result.Confidence > 0);
        }

        [Fact]
        public async Task DecodeAsync_RoundTrip_PreservesData()
        {
            // Arrange
            var originalUserId = "user12345";
            var features = new BiometricFeatures
            {
                FeatureVector = new float[] { 0.1f, 0.2f, 0.3f }
            };

            // Act
            var code = await _encoder.EncodeAsync(features, originalUserId);
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(originalUserId, result.UserId);
        }

        [Fact]
        public async Task DecodeAsync_ExpiredTimestamp_ReturnsFailure()
        {
            // Arrange
            var features = new BiometricFeatures();
            var code = await _encoder.EncodeAsync(features, "user");

            // 修改时间戳为很久以前
            code.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60; // 60秒前

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("Timestamp", result.ErrorMessage);
        }

        [Fact]
        public async Task ExtractDataAsync_PreservesParticleOrder()
        {
            // Arrange
            var originalData = new byte[] { 0xA5, 0x5A, 0xFF, 0x00, 0x42 };
            var code = await _encoder.EncodeDataAsync(originalData);

            // Act
            var extractedData = await _decoder.ExtractDataAsync(code);

            // Assert
            Assert.NotNull(extractedData);
            Assert.True(extractedData.Length >= originalData.Length);

            // 至少前几个字节应该能正确还原
            for (int i = 0; i < Math.Min(5, originalData.Length); i++)
            {
                // 允许小的误差（由于浮点精度）
                Assert.InRange(extractedData[i],
                    (byte)Math.Max(0, originalData[i] - 2),
                    (byte)Math.Min(255, originalData[i] + 2));
            }
        }

        [Fact]
        public async Task DecodeAsync_CalculatesConfidence()
        {
            // Arrange
            var features = new BiometricFeatures();
            var code = await _encoder.EncodeAsync(features, "user");

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.InRange(result.Confidence, 0f, 1f);
        }

        [Fact]
        public async Task DecodeAsync_RecordsVerificationTime()
        {
            // Arrange
            var features = new BiometricFeatures();
            var code = await _encoder.EncodeAsync(features, "user");

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.True(result.VerificationTimeMs > 0);
            Assert.True(result.VerificationTimeMs < 5000); // 应该很快
        }

        [Fact]
        public async Task DecodeAsync_WithBiometricHash_PreservesHash()
        {
            // Arrange
            var features = new BiometricFeatures
            {
                FeatureVector = new float[] { 0.1f, 0.2f, 0.3f }
            };
            var code = await _encoder.EncodeAsync(features, "user");
            var expectedHash = code.BiometricHash;

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.Equal(expectedHash, result.BiometricHash);
        }

        [Fact]
        public async Task DecodeAsync_CorruptedData_HandlesGracefully()
        {
            // Arrange
            var code = new BioChromaCode
            {
                Particles = new System.Collections.Generic.List<Particle3D>
                {
                    new Particle3D(0, 0.5f, 0.5f, 0.5f)
                },
                Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotNull(result.ErrorMessage);
        }

        [Fact]
        public async Task DecodeFromFramesAsync_NotImplemented_ReturnsFailure()
        {
            // Arrange
            var frames = new System.Collections.Generic.List<byte[]>
            {
                new byte[100],
                new byte[100]
            };

            // Act
            var result = await _decoder.DecodeFromFramesAsync(frames);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains("not yet implemented", result.ErrorMessage);
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(256)]
        public async Task DecodeAsync_VariousParticleCounts_Succeeds(int particleCount)
        {
            // Arrange
            var data = new byte[particleCount];
            new Random(42).NextBytes(data);
            var code = await _encoder.EncodeDataAsync(data);

            // Act
            var result = await _decoder.DecodeAsync(code);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal(particleCount, result.ParticlesDecoded);
        }
    }
}
