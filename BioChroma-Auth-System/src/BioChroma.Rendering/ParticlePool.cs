using System;
using System.Collections.Concurrent;
using BioChroma.Core.Models;

namespace BioChroma.Rendering
{
    /// <summary>
    /// Object pool for Particle3D to reduce GC pressure
    /// </summary>
    public class ParticlePool
    {
        private readonly ConcurrentBag<Particle3D> _pool;
        private readonly int _maxSize;
        private int _currentSize;

        public ParticlePool(int initialSize = 512, int maxSize = 2048)
        {
            _pool = new ConcurrentBag<Particle3D>();
            _maxSize = maxSize;
            _currentSize = 0;

            // Pre-allocate initial particles
            for (int i = 0; i < initialSize; i++)
            {
                _pool.Add(new Particle3D());
                _currentSize++;
            }
        }

        /// <summary>
        /// Rent a particle from the pool
        /// </summary>
        public Particle3D Rent()
        {
            if (_pool.TryTake(out var particle))
            {
                return particle;
            }

            // Pool is empty, create new particle
            _currentSize++;
            return new Particle3D();
        }

        /// <summary>
        /// Return a particle to the pool
        /// </summary>
        public void Return(Particle3D particle)
        {
            if (particle == null)
                return;

            // Reset particle to default state
            particle.X = 0;
            particle.Y = 0;
            particle.Z = 0;
            particle.Hue = 0;
            particle.Saturation = 0;
            particle.Value = 0;
            particle.RotationSpeed = 0;
            particle.Phase = 0;
            particle.Size = 1.0f;

            // Only return to pool if not at max capacity
            if (_currentSize <= _maxSize)
            {
                _pool.Add(particle);
            }
            else
            {
                _currentSize--;
            }
        }

        /// <summary>
        /// Return multiple particles to the pool
        /// </summary>
        public void ReturnRange(Particle3D[] particles)
        {
            foreach (var particle in particles)
            {
                Return(particle);
            }
        }

        public int AvailableCount => _pool.Count;
        public int TotalAllocated => _currentSize;
    }
}
