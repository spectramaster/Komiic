using System;
using Xunit;
using BioChroma.Core.Models;

namespace BioChroma.Core.Tests.Models
{
    public class BioChromaCodeTests
    {
        [Fact]
        public void DefaultConstructor_InitializesCollections()
        {
            // Act
            var code = new BioChromaCode();

            // Assert
            Assert.NotNull(code.Particles);
            Assert.NotNull(code.Edges);
            Assert.Empty(code.Particles);
            Assert.Empty(code.Edges);
        }

        [Fact]
        public void Version_DefaultValue_IsOne()
        {
            // Act
            var code = new BioChromaCode();

            // Assert
            Assert.Equal("1.0", code.Version);
        }

        [Fact]
        public void ParticleCount_ReturnsCorrectCount()
        {
            // Arrange
            var code = new BioChromaCode();
            code.Particles.Add(new Particle3D(0, 0.1f, 0.2f, 0.3f));
            code.Particles.Add(new Particle3D(1, 0.4f, 0.5f, 0.6f));

            // Act & Assert
            Assert.Equal(2, code.ParticleCount);
        }

        [Fact]
        public void EdgeCount_ReturnsCorrectCount()
        {
            // Arrange
            var code = new BioChromaCode();
            code.Edges.Add((0, 1));
            code.Edges.Add((1, 2));
            code.Edges.Add((0, 2));

            // Act & Assert
            Assert.Equal(3, code.EdgeCount);
        }

        [Fact]
        public void GetBoundingBox_EmptyParticles_ReturnsZeros()
        {
            // Arrange
            var code = new BioChromaCode();

            // Act
            var (minX, minY, minZ, maxX, maxY, maxZ) = code.GetBoundingBox();

            // Assert
            Assert.Equal(0, minX);
            Assert.Equal(0, maxX);
        }

        [Fact]
        public void GetBoundingBox_MultipleParticles_ReturnsCorrectBounds()
        {
            // Arrange
            var code = new BioChromaCode();
            code.Particles.Add(new Particle3D(0, 0.1f, 0.2f, 0.3f));
            code.Particles.Add(new Particle3D(1, 0.9f, 0.8f, 0.7f));
            code.Particles.Add(new Particle3D(2, 0.5f, 0.5f, 0.5f));

            // Act
            var (minX, minY, minZ, maxX, maxY, maxZ) = code.GetBoundingBox();

            // Assert
            Assert.Equal(0.1f, minX);
            Assert.Equal(0.2f, minY);
            Assert.Equal(0.3f, minZ);
            Assert.Equal(0.9f, maxX);
            Assert.Equal(0.8f, maxY);
            Assert.Equal(0.7f, maxZ);
        }

        [Fact]
        public void GlobalRotationSpeed_DefaultValue_IsOne()
        {
            // Act
            var code = new BioChromaCode();

            // Assert
            Assert.Equal(1.0f, code.GlobalRotationSpeed);
        }

        [Fact]
        public void BreathingFrequency_DefaultValue_IsOne()
        {
            // Act
            var code = new BioChromaCode();

            // Assert
            Assert.Equal(1.0f, code.BreathingFrequency);
        }

        [Fact]
        public void ToString_ContainsVersionAndCounts()
        {
            // Arrange
            var code = new BioChromaCode
            {
                Timestamp = 1698765432
            };
            code.Particles.Add(new Particle3D(0, 0.1f, 0.2f, 0.3f));
            code.Edges.Add((0, 1));

            // Act
            var result = code.ToString();

            // Assert
            Assert.Contains("v1.0", result);
            Assert.Contains("1 particles", result);
            Assert.Contains("1 edges", result);
            Assert.Contains("1698765432", result);
        }
    }
}
