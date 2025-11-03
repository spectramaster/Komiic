using System;

namespace BioChroma.Core.Models
{
    /// <summary>
    /// Result of a BioChroma code verification
    /// </summary>
    public class VerificationResult
    {
        /// <summary>
        /// Whether the verification was successful
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Confidence score (0.0 to 1.0)
        /// </summary>
        public float Confidence { get; set; }

        /// <summary>
        /// Decoded user identifier
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Timestamp from the code
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Biometric hash from the code
        /// </summary>
        public string? BiometricHash { get; set; }

        /// <summary>
        /// Error message if verification failed
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Number of particles successfully decoded
        /// </summary>
        public int ParticlesDecoded { get; set; }

        /// <summary>
        /// Total number of particles expected
        /// </summary>
        public int ParticlesExpected { get; set; }

        /// <summary>
        /// Time taken for verification (milliseconds)
        /// </summary>
        public long VerificationTimeMs { get; set; }

        public static VerificationResult Success(string userId, long timestamp, float confidence = 1.0f)
        {
            return new VerificationResult
            {
                IsValid = true,
                UserId = userId,
                Timestamp = timestamp,
                Confidence = confidence
            };
        }

        public static VerificationResult Failure(string errorMessage)
        {
            return new VerificationResult
            {
                IsValid = false,
                ErrorMessage = errorMessage,
                Confidence = 0.0f
            };
        }

        public override string ToString()
        {
            if (IsValid)
                return $"✅ Valid (User: {UserId}, Confidence: {Confidence:P0}, {VerificationTimeMs}ms)";
            else
                return $"❌ Invalid ({ErrorMessage})";
        }
    }
}
