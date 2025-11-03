using System;
using System.Threading.Tasks;
using Xunit;
using BioChroma.Core.Models;
using BioChroma.Core.Encoding;

namespace BioChroma.Core.Tests.Encoding
{
    public class BioChromaEncoderTests
    {
        private readonly BioChromaEncoder _encoder;

        public BioChromaEncoderTests()
        {
            _encoder = new BioChromaEncoder();
        }

        [Fact]
        public async Task EncodeAsync_WithValidFeatures_ReturnsCode()
        {
            // Arrange
            var features = new BiometricFeatures
            {
                FeatureVector = new float[] { 0.1f, 0.2f, 0.3f },
                DominantColors = new System.Collections.Generic.List<(float, float, float)>
                {
                    (180f, 0.8f, 0.9f)
                }
            };

            // Act
            var code = await _encoder.EncodeAsync(features, "testUser");

            // Assert
            Assert.NotNull(code);
            Assert.Equal("1.0", code.Version);
            Assert.True(code.ParticleCount > 0);
            Assert.NotNull(code.Nonce);
        }

        [Fact]
        public async Task EncodeDataAsync_GeneratesParticles()
        {
            // Arrange
            var data = new byte[] { 0xA5, 0x5A, 0xFF, 0x00 };

            // Act
            var code = await _encoder.EncodeDataAsync(data);

            // Assert
            Assert.NotNull(code);
            Assert.True(code.ParticleCount >= data.Length);
            Assert.All(code.Particles, p =>
            {
                Assert.InRange(p.X, 0f, 1f);
                Assert.InRange(p.Y, 0f, 1f);
                Assert.InRange(p.Z, 0f, 1f);
                Assert.InRange(p.Hue, 0f, 360f);
                Assert.InRange(p.Saturation, 0f, 1f);
                Assert.InRange(p.Value, 0f, 1f);
            });
        }

        [Fact]
        public async Task EncodeDataAsync_GeneratesTopology()
        {
            // Arrange
            var data = new byte[100]; // 足够的数据生成一些连线
            new Random().NextBytes(data);

            // Act
            var code = await _encoder.EncodeDataAsync(data);

            // Assert
            Assert.NotNull(code.Edges);
            // 应该有一些连线，但不是所有粒子都连接
            Assert.True(code.EdgeCount > 0);
        }

        [Theory]
        [InlineData(0xA5)] // 10100101
        [InlineData(0x5A)] // 01011010
        [InlineData(0xFF)] // 11111111
        [InlineData(0x00)] // 00000000
        public async Task EncodeDataAsync_DifferentBytes_GenerateDifferentParticles(byte testByte)
        {
            // Arrange
            var data = new byte[] { testByte };

            // Act
            var code = await _encoder.EncodeDataAsync(data);

            // Assert
            var particle = code.Particles[0];

            // X坐标应该对应低4位
            float expectedXRange = (testByte & 0x0F) / 15.0f;
            Assert.InRange(particle.X, expectedXRange * 0.8f, expectedXRange * 0.8f + 0.3f);

            // Y坐标应该对应高4位
            float expectedYRange = (testByte >> 4) / 15.0f;
            Assert.InRange(particle.Y, expectedYRange * 0.8f, expectedYRange * 0.8f + 0.3f);

            // 色相应该是字节值的函数
            float expectedHue = (testByte * 1.5f) % 360;
            Assert.InRange(particle.Hue, expectedHue - 1, expectedHue + 1);
        }

        [Fact]
        public async Task EncodeAsync_AppliesBiometricInfluence()
        {
            // Arrange
            var features = new BiometricFeatures
            {
                FeatureVector = new float[] { 0.5f },
                DominantColors = new System.Collections.Generic.List<(float, float, float)>
                {
                    (120f, 0.9f, 0.8f) // 绿色主导
                },
                Quality = 0.9f
            };

            // Act
            var code = await _encoder.EncodeAsync(features, "user123");

            // Assert
            Assert.NotNull(code.BiometricHash);
            Assert.NotEmpty(code.BiometricHash);

            // 全局速度应该受质量影响
            Assert.InRange(code.GlobalRotationSpeed, 0.8f, 1.2f);
        }

        [Fact]
        public async Task EncodeDataAsync_EmptyData_HandlesGracefully()
        {
            // Arrange
            var data = new byte[0];

            // Act
            var code = await _encoder.EncodeDataAsync(data);

            // Assert
            Assert.NotNull(code);
            // 即使是空数据，也应该生成默认数量的粒子
            Assert.True(code.ParticleCount > 0);
        }

        [Fact]
        public async Task EncodeDataAsync_LargeData_HandlesCorrectly()
        {
            // Arrange
            var data = new byte[1000];
            new Random(42).NextBytes(data);

            // Act
            var code = await _encoder.EncodeDataAsync(data);

            // Assert
            Assert.NotNull(code);
            Assert.True(code.ParticleCount >= data.Length);
        }

        [Fact]
        public async Task EncodeAsync_MultipleCalls_GeneratesDifferentNonces()
        {
            // Arrange
            var features = new BiometricFeatures();

            // Act
            var code1 = await _encoder.EncodeAsync(features, "user");
            var code2 = await _encoder.EncodeAsync(features, "user");

            // Assert
            Assert.NotEqual(code1.Nonce, code2.Nonce);
        }
    }
}
