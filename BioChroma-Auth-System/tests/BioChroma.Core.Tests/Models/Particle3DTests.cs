using System;
using System.Threading.Tasks;
using Xunit;
using BioChroma.Core.Models;
using BioChroma.Core.Encoding;

namespace BioChroma.Core.Tests.Models
{
    public class Particle3DTests
    {
        [Fact]
        public void Constructor_SetsBasicProperties()
        {
            // Arrange & Act
            var particle = new Particle3D(1, 0.5f, 0.3f, 0.7f);

            // Assert
            Assert.Equal(1, particle.Id);
            Assert.Equal(0.5f, particle.X);
            Assert.Equal(0.3f, particle.Y);
            Assert.Equal(0.7f, particle.Z);
        }

        [Fact]
        public void DistanceTo_CalculatesCorrectDistance()
        {
            // Arrange
            var particle1 = new Particle3D(1, 0.0f, 0.0f, 0.0f);
            var particle2 = new Particle3D(2, 3.0f, 4.0f, 0.0f);

            // Act
            var distance = particle1.DistanceTo(particle2);

            // Assert
            Assert.Equal(5.0f, distance, 2); // 3-4-5 triangle
        }

        [Theory]
        [InlineData(0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, 1.732f)] // 对角线
        [InlineData(0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.5f, 0.0f)]   // 同一点
        public void DistanceTo_VariousPositions_ReturnsCorrectDistance(
            float x1, float y1, float z1,
            float x2, float y2, float z2,
            float expectedDistance)
        {
            // Arrange
            var particle1 = new Particle3D(1, x1, y1, z1);
            var particle2 = new Particle3D(2, x2, y2, z2);

            // Act
            var distance = particle1.DistanceTo(particle2);

            // Assert
            Assert.Equal(expectedDistance, distance, 2);
        }

        [Fact]
        public void ToString_ReturnsFormattedString()
        {
            // Arrange
            var particle = new Particle3D(5, 0.5f, 0.3f, 0.7f)
            {
                Hue = 180.0f,
                Saturation = 0.8f,
                Value = 0.9f
            };

            // Act
            var result = particle.ToString();

            // Assert
            Assert.Contains("Particle(5", result);
            Assert.Contains("180°", result);
        }

        [Fact]
        public void Size_DefaultValue_IsOne()
        {
            // Arrange & Act
            var particle = new Particle3D();

            // Assert
            Assert.Equal(1.0f, particle.Size);
        }
    }
}
