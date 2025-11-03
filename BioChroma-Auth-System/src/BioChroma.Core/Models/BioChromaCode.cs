using System;
using System.Collections.Generic;

namespace BioChroma.Core.Models
{
    /// <summary>
    /// Represents a complete BioChroma code with particles and topology
    /// </summary>
    public class BioChromaCode
    {
        /// <summary>
        /// Version of the encoding format
        /// </summary>
        public string Version { get; set; } = "1.0";

        /// <summary>
        /// Unix timestamp when the code was generated
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Collection of 3D particles
        /// </summary>
        public List<Particle3D> Particles { get; set; } = new List<Particle3D>();

        /// <summary>
        /// Edges representing connections between particles (topology)
        /// Each tuple contains (particleId1, particleId2)
        /// </summary>
        public List<(int, int)> Edges { get; set; } = new List<(int, int)>();

        /// <summary>
        /// Hash of the biometric features used to generate this code
        /// </summary>
        public string? BiometricHash { get; set; }

        /// <summary>
        /// Random nonce for uniqueness
        /// </summary>
        public string? Nonce { get; set; }

        /// <summary>
        /// User identifier (encrypted)
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Overall rotation speed multiplier
        /// </summary>
        public float GlobalRotationSpeed { get; set; } = 1.0f;

        /// <summary>
        /// Overall breathing effect frequency (Hz)
        /// </summary>
        public float BreathingFrequency { get; set; } = 1.0f;

        public int ParticleCount => Particles.Count;

        public int EdgeCount => Edges.Count;

        /// <summary>
        /// Calculate the bounding box of all particles
        /// </summary>
        public (float minX, float minY, float minZ, float maxX, float maxY, float maxZ) GetBoundingBox()
        {
            if (Particles.Count == 0)
                return (0, 0, 0, 0, 0, 0);

            float minX = float.MaxValue, minY = float.MaxValue, minZ = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue, maxZ = float.MinValue;

            foreach (var particle in Particles)
            {
                if (particle.X < minX) minX = particle.X;
                if (particle.Y < minY) minY = particle.Y;
                if (particle.Z < minZ) minZ = particle.Z;
                if (particle.X > maxX) maxX = particle.X;
                if (particle.Y > maxY) maxY = particle.Y;
                if (particle.Z > maxZ) maxZ = particle.Z;
            }

            return (minX, minY, minZ, maxX, maxY, maxZ);
        }

        public override string ToString()
        {
            return $"BioChromaCode(v{Version}, {ParticleCount} particles, {EdgeCount} edges, ts={Timestamp})";
        }
    }
}
