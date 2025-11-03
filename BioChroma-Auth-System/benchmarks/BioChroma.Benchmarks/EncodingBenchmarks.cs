using System;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BioChroma.Core.Models;
using BioChroma.Core.Encoding;

namespace BioChroma.Benchmarks
{
    [MemoryDiagnoser]
    [SimpleJob(warmupCount: 3, iterationCount: 10)]
    public class EncodingBenchmarks
    {
        private BioChromaEncoder? _encoder;
        private BiometricFeatures? _features;
        private byte[]? _smallData;
        private byte[]? _mediumData;
        private byte[]? _largeData;

        [GlobalSetup]
        public void Setup()
        {
            _encoder = new BioChromaEncoder();

            _features = new BiometricFeatures
            {
                FeatureVector = new float[] { 0.1f, 0.2f, 0.3f, 0.4f, 0.5f },
                DominantColors = new System.Collections.Generic.List<(float, float, float)>
                {
                    (120f, 0.8f, 0.9f),
                    (240f, 0.7f, 0.8f)
                },
                Quality = 0.85f
            };

            _smallData = new byte[50];
            _mediumData = new byte[256];
            _largeData = new byte[1000];

            new Random(42).NextBytes(_smallData);
            new Random(42).NextBytes(_mediumData);
            new Random(42).NextBytes(_largeData);
        }

        [Benchmark]
        public async Task<BioChromaCode> EncodeSmallData()
        {
            return await _encoder!.EncodeDataAsync(_smallData!);
        }

        [Benchmark(Baseline = true)]
        public async Task<BioChromaCode> EncodeMediumData()
        {
            return await _encoder!.EncodeDataAsync(_mediumData!);
        }

        [Benchmark]
        public async Task<BioChromaCode> EncodeLargeData()
        {
            return await _encoder!.EncodeDataAsync(_largeData!);
        }

        [Benchmark]
        public async Task<BioChromaCode> EncodeWithBiometrics()
        {
            return await _encoder!.EncodeAsync(_features!, "benchmarkUser");
        }
    }
}
