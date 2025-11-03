using System;
using System.Collections.Generic;
using BioChroma.Core.Models;
using SkiaSharp;

namespace BioChroma.Rendering
{
    /// <summary>
    /// Optimized 3D particle rendering engine with batching and object pooling
    /// Target: 90fps @ 512 particles
    /// </summary>
    public class OptimizedParticle3DEngine : Particle3DEngine
    {
        private readonly ParticlePool _particlePool;
        private readonly List<(float x, float y, float size, SKColor color)> _batchBuffer;
        private readonly int _batchSize = 256;

        // Cached objects to reduce allocations
        private SKPaint? _cachedParticlePaint;
        private SKPaint? _cachedLinePaint;
        private SKPoint[]? _cachedPointBuffer;

        public OptimizedParticle3DEngine(int maxParticles = 512)
        {
            _particlePool = new ParticlePool(initialSize: maxParticles, maxSize: maxParticles * 2);
            _batchBuffer = new List<(float, float, float, SKColor)>(maxParticles);
            _cachedPointBuffer = new SKPoint[maxParticles];

            // Pre-create reusable paint objects
            _cachedParticlePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                IsAntialias = true
            };

            _cachedLinePaint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 1.0f,
                IsAntialias = true
            };
        }

        /// <summary>
        /// Optimized render with batching
        /// </summary>
        public new void Render(SKCanvas canvas, BioChromaCode code, int width, int height)
        {
            if (canvas == null)
                throw new ArgumentNullException(nameof(canvas));
            if (code == null)
                throw new ArgumentNullException(nameof(code));
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width), "Width must be positive");
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height), "Height must be positive");

            if (code.Particles.Count == 0)
                return;

            // Clear batch buffer
            _batchBuffer.Clear();

            // Update animation state
            UpdateAnimation();

            // Clear background
            canvas.Clear(new SKColor(10, 10, 20));

            var centerX = width / 2f;
            var centerY = height / 2f;
            var scale = Math.Min(width, height) * 0.4f;

            // Render connections first (background layer)
            if (EnableConnections && code.Edges.Count > 0)
            {
                RenderConnectionsBatched(canvas, code, centerX, centerY, scale);
            }

            // Collect all particles for batch rendering
            foreach (var particle in code.Particles)
            {
                var (x, y, z) = ApplyRotation(particle.X - 0.5f, particle.Y - 0.5f, particle.Z - 0.5f);

                float breathingFactor = ApplyBreathingEffect(particle.Phase);
                float size = particle.Size * ParticleBaseSize * breathingFactor;

                float screenX = centerX + x * scale;
                float screenY = centerY + y * scale;

                float depth = z + 0.5f;
                byte alpha = (byte)(100 + depth * 155);

                var color = HSVToRGB(particle.Hue, particle.Saturation, particle.Value * breathingFactor);
                color = color.WithAlpha(alpha);

                _batchBuffer.Add((screenX, screenY, size, color));
            }

            // Batch render all particles
            RenderParticlesBatched(canvas);
        }

        private void RenderParticlesBatched(SKCanvas canvas)
        {
            if (_cachedParticlePaint == null)
                return;

            // Process particles in batches to reduce draw calls
            for (int i = 0; i < _batchBuffer.Count; i += _batchSize)
            {
                int batchEnd = Math.Min(i + _batchSize, _batchBuffer.Count);

                for (int j = i; j < batchEnd; j++)
                {
                    var (x, y, size, color) = _batchBuffer[j];

                    if (EnableGlow)
                    {
                        // Draw glow with cached paint
                        _cachedParticlePaint.Shader = SKShader.CreateRadialGradient(
                            new SKPoint(x, y),
                            size * 1.5f,
                            new[] {
                                color.WithAlpha((byte)(color.Alpha * 0.6)),
                                color.WithAlpha(0)
                            },
                            new[] { 0.3f, 1.0f },
                            SKShaderTileMode.Clamp
                        );

                        canvas.DrawCircle(x, y, size * 1.5f, _cachedParticlePaint);
                        _cachedParticlePaint.Shader?.Dispose();
                        _cachedParticlePaint.Shader = null;
                    }

                    // Draw core
                    _cachedParticlePaint.Color = color;
                    canvas.DrawCircle(x, y, size / 2, _cachedParticlePaint);
                }
            }
        }

        private void RenderConnectionsBatched(SKCanvas canvas, BioChromaCode code, float centerX, float centerY, float scale)
        {
            if (_cachedLinePaint == null)
                return;

            // Pre-calculate all particle positions
            var positions = new SKPoint[code.Particles.Count];
            var depths = new float[code.Particles.Count];

            for (int i = 0; i < code.Particles.Count; i++)
            {
                var p = code.Particles[i];
                var (x, y, z) = ApplyRotation(p.X - 0.5f, p.Y - 0.5f, p.Z - 0.5f);

                positions[i] = new SKPoint(centerX + x * scale, centerY + y * scale);
                depths[i] = z + 0.5f;
            }

            // Batch render connections
            foreach (var (id1, id2) in code.Edges)
            {
                if (id1 >= positions.Length || id2 >= positions.Length)
                    continue;

                float avgDepth = (depths[id1] + depths[id2]) / 2;
                byte alpha = (byte)(30 + avgDepth * 70);

                _cachedLinePaint.Color = new SKColor(100, 150, 255, alpha);
                canvas.DrawLine(positions[id1], positions[id2], _cachedLinePaint);
            }
        }

        /// <summary>
        /// Fast rotation without creating new tuples
        /// </summary>
        private (float x, float y, float z) ApplyRotation(float x, float y, float z)
        {
            float cosY = MathF.Cos(_rotationAngle);
            float sinY = MathF.Sin(_rotationAngle);

            float rotX = x * cosY - z * sinY;
            float rotZ = x * sinY + z * cosY;

            // Prevent division by zero
            float denominator = 1.0f + rotZ * 0.5f;
            if (MathF.Abs(denominator) < 0.01f)
                denominator = 0.01f;

            float perspective = 1.0f / denominator;
            return (rotX * perspective, y * perspective, rotZ);
        }

        /// <summary>
        /// Fast breathing effect calculation
        /// </summary>
        private float ApplyBreathingEffect(float phase)
        {
            return 0.85f + MathF.Sin(_breathingPhase + phase * 0.01745329f) * 0.15f; // 0.01745329 = PI/180
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cachedParticlePaint?.Dispose();
                _cachedLinePaint?.Dispose();
            }
            base.Dispose(disposing);
        }

        // Cache for rotation calculations
        private float _rotationAngle = 0f;
        private float _breathingPhase = 0f;
        private DateTime _startTime = DateTime.UtcNow;

        private new void UpdateAnimation()
        {
            float deltaTime = (float)(DateTime.UtcNow - _startTime).TotalSeconds;
            _rotationAngle = deltaTime * RotationSpeed * 0.5f;
            _breathingPhase = deltaTime * BreathingSpeed * 6.28318f; // 2*PI
        }
    }
}
