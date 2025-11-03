using System;
using BioChroma.Core.Models;
using SkiaSharp;

namespace BioChroma.Rendering
{
    /// <summary>
    /// 3D particle rendering engine
    /// Handles transformation, animation, and rendering of particle clouds
    /// </summary>
    public class Particle3DEngine : IDisposable
    {
        private bool _disposed = false;
        private float _rotationAngle = 0f;
        private float _breathingPhase = 0f;
        private DateTime _startTime = DateTime.UtcNow;

        public float RotationSpeed { get; set; } = 1.0f; // radians per second
        public float BreathingSpeed { get; set; } = 1.0f; // Hz
        public float ParticleBaseSize { get; set; } = 6.0f; // pixels
        public bool EnableGlow { get; set; } = true;
        public bool EnableConnections { get; set; } = true;

        /// <summary>
        /// Render a BioChroma code to a canvas
        /// </summary>
        public void Render(SKCanvas canvas, BioChromaCode code, int width, int height)
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(Particle3DEngine));

            if (canvas == null)
                throw new ArgumentNullException(nameof(canvas));

            if (code == null)
                throw new ArgumentNullException(nameof(code));

            if (width <= 0 || height <= 0)
                throw new ArgumentException("Width and height must be positive");

            if (code.Particles.Count == 0)
                return;

            // Update animation state
            UpdateAnimation();

            // Clear background
            canvas.Clear(new SKColor(10, 10, 20)); // Dark blue-black

            // Calculate view transform
            var centerX = width / 2f;
            var centerY = height / 2f;
            var scale = Math.Min(width, height) * 0.4f;

            // Render connection lines first (background layer)
            if (EnableConnections && code.Edges.Count > 0)
            {
                RenderConnections(canvas, code, centerX, centerY, scale);
            }

            // Render particles (foreground layer)
            RenderParticles(canvas, code, centerX, centerY, scale);
        }

        private void RenderParticles(SKCanvas canvas, BioChromaCode code, float centerX, float centerY, float scale)
        {
            foreach (var particle in code.Particles)
            {
                // Apply 3D rotation
                var (x, y, z) = ApplyRotation(particle.X - 0.5f, particle.Y - 0.5f, particle.Z - 0.5f);

                // Apply breathing effect
                float breathingFactor = ApplyBreathingEffect(particle.Phase);
                float size = particle.Size * ParticleBaseSize * breathingFactor;

                // Calculate screen position
                float screenX = centerX + x * scale;
                float screenY = centerY + y * scale;

                // Calculate depth-based alpha (further = more transparent)
                float depth = z + 0.5f; // 0 to 1
                byte alpha = (byte)(100 + depth * 155); // 100-255

                // Convert HSV to RGB
                var color = HSVToRGB(particle.Hue, particle.Saturation, particle.Value * breathingFactor);
                color = color.WithAlpha(alpha);

                // Draw particle with glow
                if (EnableGlow)
                {
                    DrawParticleWithGlow(canvas, screenX, screenY, size, color, depth);
                }
                else
                {
                    DrawParticle(canvas, screenX, screenY, size, color);
                }
            }
        }

        private void RenderConnections(SKCanvas canvas, BioChromaCode code, float centerX, float centerY, float scale)
        {
            using (var paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.0f,
                IsAntialias = true
            })
            {
                foreach (var (id1, id2) in code.Edges)
                {
                    if (id1 >= code.Particles.Count || id2 >= code.Particles.Count)
                        continue;

                    var p1 = code.Particles[id1];
                    var p2 = code.Particles[id2];

                    // Apply rotation to both points
                    var (x1, y1, z1) = ApplyRotation(p1.X - 0.5f, p1.Y - 0.5f, p1.Z - 0.5f);
                    var (x2, y2, z2) = ApplyRotation(p2.X - 0.5f, p2.Y - 0.5f, p2.Z - 0.5f);

                    float screenX1 = centerX + x1 * scale;
                    float screenY1 = centerY + y1 * scale;
                    float screenX2 = centerX + x2 * scale;
                    float screenY2 = centerY + y2 * scale;

                    // Calculate average depth for line alpha
                    float avgDepth = ((z1 + z2) / 2 + 0.5f);
                    byte alpha = (byte)(30 + avgDepth * 70); // 30-100

                    paint.Color = new SKColor(100, 150, 255, alpha);
                    canvas.DrawLine(screenX1, screenY1, screenX2, screenY2, paint);
                }
            }
        }

        private void DrawParticle(SKCanvas canvas, float x, float y, float size, SKColor color)
        {
            using (var paint = new SKPaint
            {
                Color = color,
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            })
            {
                canvas.DrawCircle(x, y, size / 2, paint);
            }
        }

        private void DrawParticleWithGlow(SKCanvas canvas, float x, float y, float size, SKColor color, float depth)
        {
            // Draw outer glow
            using (var glowPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            })
            {
                using (var shader = SKShader.CreateRadialGradient(
                    new SKPoint(x, y),
                    size * 1.5f,
                    new[] {
                        color.WithAlpha((byte)(color.Alpha * 0.6)),
                        color.WithAlpha(0)
                    },
                    new[] { 0.3f, 1.0f },
                    SKShaderTileMode.Clamp
                ))
                {
                    glowPaint.Shader = shader;
                    canvas.DrawCircle(x, y, size * 1.5f, glowPaint);
                }
            }

            // Draw core
            DrawParticle(canvas, x, y, size, color);
        }

        private (float x, float y, float z) ApplyRotation(float x, float y, float z)
        {
            // Rotate around Y axis
            float cosY = (float)Math.Cos(_rotationAngle);
            float sinY = (float)Math.Sin(_rotationAngle);

            float rotX = x * cosY - z * sinY;
            float rotZ = x * sinY + z * cosY;

            // Simple perspective projection with safety check
            float denominator = 1.0f + rotZ * 0.5f;
            if (Math.Abs(denominator) < 0.01f)
                denominator = 0.01f; // Prevent division by zero

            float perspective = 1.0f / denominator;
            return (rotX * perspective, y * perspective, rotZ);
        }

        private float ApplyBreathingEffect(float phase)
        {
            float breathing = (float)Math.Sin(_breathingPhase + phase * Math.PI / 180);
            return 0.85f + breathing * 0.15f; // 0.7 to 1.0
        }

        private void UpdateAnimation()
        {
            float deltaTime = (float)(DateTime.UtcNow - _startTime).TotalSeconds;

            _rotationAngle = deltaTime * RotationSpeed * 0.5f; // Slower rotation
            _breathingPhase = deltaTime * BreathingSpeed * 2 * (float)Math.PI;
        }

        private SKColor HSVToRGB(float h, float s, float v)
        {
            float c = v * s;
            float x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            float m = v - c;

            float r, g, b;

            if (h < 60)
            {
                r = c; g = x; b = 0;
            }
            else if (h < 120)
            {
                r = x; g = c; b = 0;
            }
            else if (h < 180)
            {
                r = 0; g = c; b = x;
            }
            else if (h < 240)
            {
                r = 0; g = x; b = c;
            }
            else if (h < 300)
            {
                r = x; g = 0; b = c;
            }
            else
            {
                r = c; g = 0; b = x;
            }

            return new SKColor(
                (byte)((r + m) * 255),
                (byte)((g + m) * 255),
                (byte)((b + m) * 255)
            );
        }

        /// <summary>
        /// Reset animation to start
        /// </summary>
        public void ResetAnimation()
        {
            _startTime = DateTime.UtcNow;
            _rotationAngle = 0f;
            _breathingPhase = 0f;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    // Currently no managed resources to dispose
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
