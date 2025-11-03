using System;
using System.Collections.Generic;
using System.Linq;
using BioChroma.Core.Models;

namespace BioChroma.Camera
{
    /// <summary>
    /// Extracts biometric features from camera frames
    /// </summary>
    public class FeatureExtractor
    {
        /// <summary>
        /// Extract features from a raw RGB frame
        /// </summary>
        /// <param name="frameData">RGB frame data</param>
        /// <param name="width">Frame width</param>
        /// <param name="height">Frame height</param>
        /// <returns>Extracted biometric features</returns>
        public BiometricFeatures ExtractFeatures(byte[] frameData, int width, int height)
        {
            var features = new BiometricFeatures
            {
                ImageWidth = width,
                ImageHeight = height,
                CaptureTime = DateTime.UtcNow
            };

            // Extract simplified feature vector
            features.FeatureVector = ExtractSimplifiedFeatures(frameData, width, height);

            // Extract dominant colors
            features.DominantColors = ExtractDominantColors(frameData, width, height);

            // Calculate quality score
            features.Quality = CalculateQuality(frameData, width, height);

            return features;
        }

        private float[] ExtractSimplifiedFeatures(byte[] data, int width, int height)
        {
            // Simplified feature extraction
            // In production, use proper face detection (OpenCV, ML.NET, etc.)

            var features = new List<float>();

            // Calculate color histogram
            int[] rHist = new int[16];
            int[] gHist = new int[16];
            int[] bHist = new int[16];

            for (int i = 0; i < data.Length; i += 3)
            {
                if (i + 2 < data.Length)
                {
                    rHist[data[i] / 16]++;
                    gHist[data[i + 1] / 16]++;
                    bHist[data[i + 2] / 16]++;
                }
            }

            // Normalize and add to features
            int totalPixels = width * height;
            for (int i = 0; i < 16; i++)
            {
                features.Add(rHist[i] / (float)totalPixels);
                features.Add(gHist[i] / (float)totalPixels);
                features.Add(bHist[i] / (float)totalPixels);
            }

            // Add image statistics
            features.Add(width / 1000.0f);
            features.Add(height / 1000.0f);

            return features.ToArray();
        }

        private List<(float h, float s, float v)> ExtractDominantColors(byte[] data, int width, int height, int count = 5)
        {
            var colors = new List<(float h, float s, float v)>();

            // Sample pixels in a grid pattern
            int sampleStep = Math.Max(1, (width * height) / 1000);

            var samples = new List<(float h, float s, float v)>();

            for (int i = 0; i < data.Length; i += sampleStep * 3)
            {
                if (i + 2 < data.Length)
                {
                    float r = data[i] / 255.0f;
                    float g = data[i + 1] / 255.0f;
                    float b = data[i + 2] / 255.0f;

                    var (h, s, v) = RGBToHSV(r, g, b);
                    samples.Add((h, s, v));
                }
            }

            // Simple clustering - take samples at regular intervals
            if (samples.Count > 0)
            {
                int clusterSize = samples.Count / count;
                for (int i = 0; i < count && i * clusterSize < samples.Count; i++)
                {
                    colors.Add(samples[i * clusterSize]);
                }
            }

            return colors;
        }

        private float CalculateQuality(byte[] data, int width, int height)
        {
            // Simple quality metric based on variance
            float sum = 0;
            float sumSquared = 0;
            int count = 0;

            for (int i = 0; i < data.Length; i++)
            {
                sum += data[i];
                sumSquared += data[i] * data[i];
                count++;
            }

            float mean = sum / count;
            float variance = (sumSquared / count) - (mean * mean);

            // Normalize variance to 0-1 range
            float quality = Math.Min(1.0f, variance / 10000.0f);

            return quality;
        }

        private (float h, float s, float v) RGBToHSV(float r, float g, float b)
        {
            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float delta = max - min;

            // Hue
            float h = 0;
            if (delta != 0)
            {
                if (max == r)
                    h = 60 * (((g - b) / delta) % 6);
                else if (max == g)
                    h = 60 * (((b - r) / delta) + 2);
                else
                    h = 60 * (((r - g) / delta) + 4);
            }

            if (h < 0) h += 360;

            // Saturation
            float s = max == 0 ? 0 : delta / max;

            // Value
            float v = max;

            return (h, s, v);
        }
    }
}
