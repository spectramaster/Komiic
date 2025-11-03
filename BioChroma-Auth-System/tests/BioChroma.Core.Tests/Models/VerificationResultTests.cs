using System;
using Xunit;
using BioChroma.Core.Models;

namespace BioChroma.Core.Tests.Models
{
    public class VerificationResultTests
    {
        [Fact]
        public void Success_CreatesValidResult()
        {
            // Act
            var result = VerificationResult.Success("user123", 1698765432, 0.95f);

            // Assert
            Assert.True(result.IsValid);
            Assert.Equal("user123", result.UserId);
            Assert.Equal(1698765432, result.Timestamp);
            Assert.Equal(0.95f, result.Confidence);
            Assert.Null(result.ErrorMessage);
        }

        [Fact]
        public void Failure_CreatesInvalidResult()
        {
            // Act
            var result = VerificationResult.Failure("Timeout");

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal("Timeout", result.ErrorMessage);
            Assert.Equal(0.0f, result.Confidence);
        }

        [Fact]
        public void ToString_ValidResult_ContainsUserInfo()
        {
            // Arrange
            var result = VerificationResult.Success("user123", 1698765432, 0.95f);
            result.VerificationTimeMs = 420;

            // Act
            var output = result.ToString();

            // Assert
            Assert.Contains("✅", output);
            Assert.Contains("user123", output);
            Assert.Contains("95%", output);
            Assert.Contains("420ms", output);
        }

        [Fact]
        public void ToString_InvalidResult_ContainsErrorMessage()
        {
            // Arrange
            var result = VerificationResult.Failure("Invalid timestamp");

            // Act
            var output = result.ToString();

            // Assert
            Assert.Contains("❌", output);
            Assert.Contains("Invalid timestamp", output);
        }

        [Theory]
        [InlineData(0.0f)]
        [InlineData(0.5f)]
        [InlineData(1.0f)]
        public void Confidence_AcceptsValidRange(float confidence)
        {
            // Arrange & Act
            var result = VerificationResult.Success("user", 123, confidence);

            // Assert
            Assert.Equal(confidence, result.Confidence);
        }
    }
}
