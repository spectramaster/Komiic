using System;
using System.Diagnostics;
using BenchmarkDotNet.Attributes;
using BioChroma.Core.Models;
using BioChroma.Rendering;
using SkiaSharp;

namespace BioChroma.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 3, iterationCount: 10)]
    public class RenderingBenchmarks
    {
        private Particle3DEngine? _standardEngine;
        private OptimizedParticle3DEngine? _optimizedEngine;
        private BioChromaCode? _code128;
        private BioChromaCode? _code256;
        private BioChromaCode? _code512;
        private SKSurface? _surface;
        private SKCanvas? _canvas;

        [GlobalSetup]
        public void Setup()
        {
            _standardEngine = new Particle3DEngine();
            _optimizedEngine = new OptimizedParticle3DEngine(512);

            _code128 = CreateTestCode(128);
            _code256 = CreateTestCode(256);
            _code512 = CreateTestCode(512);

            var info = new SKImageInfo(800, 600);
            _surface = SKSurface.Create(info);
            _canvas = _surface.Canvas;
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _surface?.Dispose();
        }

        private BioChromaCode CreateTestCode(int particleCount)
        {
            var code = new BioChromaCode();
            var random = new Random(42);

            for (int i = 0; i < particleCount; i++)
            {
                code.Particles.Add(new Particle3D(i,
                    (float)random.NextDouble(),
                    (float)random.NextDouble(),
                    (float)random.NextDouble())
                {
                    Hue = (float)(random.NextDouble() * 360),
                    Saturation = 0.7f + (float)(random.NextDouble() * 0.3f),
                    Value = 0.6f + (float)(random.NextDouble() * 0.4f),
                    RotationSpeed = (float)(random.NextDouble()),
                    Phase = (float)(random.NextDouble() * 360)
                });
            }

            // Add some edges
            for (int i = 0; i < particleCount - 1; i++)
            {
                if (random.NextDouble() < 0.3)
                {
                    code.Edges.Add((i, i + 1));
                }
            }

            return code;
        }

        [Benchmark]
        public void StandardRender_128Particles()
        {
            _standardEngine!.Render(_canvas!, _code128!, 800, 600);
        }

        [Benchmark(Baseline = true)]
        public void StandardRender_256Particles()
        {
            _standardEngine!.Render(_canvas!, _code256!, 800, 600);
        }

        [Benchmark]
        public void StandardRender_512Particles()
        {
            _standardEngine!.Render(_canvas!, _code512!, 800, 600);
        }

        [Benchmark]
        public void OptimizedRender_128Particles()
        {
            _optimizedEngine!.Render(_canvas!, _code128!, 800, 600);
        }

        [Benchmark]
        public void OptimizedRender_256Particles()
        {
            _optimizedEngine!.Render(_canvas!, _code256!, 800, 600);
        }

        [Benchmark]
        public void OptimizedRender_512Particles()
        {
            _optimizedEngine!.Render(_canvas!, _code512!, 800, 600);
        }

        /// <summary>
        /// Measure FPS for continuous rendering
        /// </summary>
        [Benchmark]
        public double MeasureFPS_256Particles()
        {
            const int frameCount = 100;
            var stopwatch = Stopwatch.StartNew();

            for (int i = 0; i < frameCount; i++)
            {
                _optimizedEngine!.Render(_canvas!, _code256!, 800, 600);
            }

            stopwatch.Stop();
            double fps = frameCount / stopwatch.Elapsed.TotalSeconds;
            return fps;
        }
    }
}
