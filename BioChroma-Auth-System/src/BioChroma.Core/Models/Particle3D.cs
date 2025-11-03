using System;

namespace BioChroma.Core.Models
{
    /// <summary>
    /// Represents a 3D particle in the BioChroma code
    /// </summary>
    public class Particle3D
    {
        /// <summary>
        /// Unique identifier for the particle
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// X coordinate (0.0 to 1.0, normalized)
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y coordinate (0.0 to 1.0, normalized)
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// Z coordinate (0.0 to 1.0, normalized for depth)
        /// </summary>
        public float Z { get; set; }

        /// <summary>
        /// Hue value (0 to 360 degrees)
        /// </summary>
        public float Hue { get; set; }

        /// <summary>
        /// Saturation value (0.0 to 1.0)
        /// </summary>
        public float Saturation { get; set; }

        /// <summary>
        /// Value/Brightness (0.0 to 1.0)
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// Rotation speed for animation (radians per second)
        /// </summary>
        public float RotationSpeed { get; set; }

        /// <summary>
        /// Initial phase for breathing effect (0 to 360 degrees)
        /// </summary>
        public float Phase { get; set; }

        /// <summary>
        /// Particle size multiplier (0.5 to 2.0)
        /// </summary>
        public float Size { get; set; } = 1.0f;

        public Particle3D()
        {
        }

        public Particle3D(int id, float x, float y, float z)
        {
            Id = id;
            X = x;
            Y = y;
            Z = z;
        }

        /// <summary>
        /// Calculate distance to another particle
        /// </summary>
        public float DistanceTo(Particle3D other)
        {
            float dx = X - other.X;
            float dy = Y - other.Y;
            float dz = Z - other.Z;
            return (float)Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public override string ToString()
        {
            return $"Particle({Id}: [{X:F2}, {Y:F2}, {Z:F2}] HSV({Hue:F0}°, {Saturation:F2}, {Value:F2}))";
        }
    }
}
