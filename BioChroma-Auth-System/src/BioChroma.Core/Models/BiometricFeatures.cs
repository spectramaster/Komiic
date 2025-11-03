using System;
using System.Collections.Generic;

namespace BioChroma.Core.Models
{
    /// <summary>
    /// Represents extracted biometric features from camera
    /// </summary>
    public class BiometricFeatures
    {
        /// <summary>
        /// Raw feature vector (simplified, could be facial landmarks, etc.)
        /// </summary>
        public float[] FeatureVector { get; set; } = Array.Empty<float>();

        /// <summary>
        /// Dominant colors extracted from the image
        /// Each tuple contains (Hue, Saturation, Value)
        /// </summary>
        public List<(float h, float s, float v)> DominantColors { get; set; } = new List<(float, float, float)>();

        /// <summary>
        /// Image width used for feature extraction
        /// </summary>
        public int ImageWidth { get; set; }

        /// <summary>
        /// Image height used for feature extraction
        /// </summary>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Timestamp of feature extraction
        /// </summary>
        public DateTime CaptureTime { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Quality score of the capture (0.0 to 1.0)
        /// </summary>
        public float Quality { get; set; } = 1.0f;

        /// <summary>
        /// Calculate a simple hash from the feature vector
        /// </summary>
        public string CalculateHash()
        {
            if (FeatureVector.Length == 0)
                return string.Empty;

            // Simple hash calculation
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = new byte[FeatureVector.Length * 4];
                Buffer.BlockCopy(FeatureVector, 0, bytes, 0, bytes.Length);
                var hash = sha.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }

        public override string ToString()
        {
            return $"BiometricFeatures(Vector: {FeatureVector.Length}, Colors: {DominantColors.Count}, Quality: {Quality:F2})";
        }
    }
}
