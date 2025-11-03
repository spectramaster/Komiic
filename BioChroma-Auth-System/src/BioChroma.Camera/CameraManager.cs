using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioChroma.Core.Interfaces;
using BioChroma.Core.Models;

namespace BioChroma.Camera
{
    /// <summary>
    /// High-level camera manager that coordinates camera access and feature extraction
    /// </summary>
    public class CameraManager
    {
        private readonly ICameraProvider _cameraProvider;
        private readonly FeatureExtractor _featureExtractor;

        public CameraManager(ICameraProvider cameraProvider)
        {
            _cameraProvider = cameraProvider ?? throw new ArgumentNullException(nameof(cameraProvider));
            _featureExtractor = new FeatureExtractor();
        }

        public bool IsCapturing => _cameraProvider.IsCapturing;

        /// <summary>
        /// Initialize camera and request permissions
        /// </summary>
        public async Task<bool> InitializeAsync()
        {
            return await _cameraProvider.RequestPermissionAsync();
        }

        /// <summary>
        /// Get available cameras
        /// </summary>
        public async Task<IEnumerable<CameraDevice>> GetCamerasAsync()
        {
            return await _cameraProvider.GetDevicesAsync();
        }

        /// <summary>
        /// Start capturing from a camera
        /// </summary>
        public async Task StartCaptureAsync(string deviceId, int width = 640, int height = 480)
        {
            await _cameraProvider.StartCaptureAsync(deviceId, width, height);
        }

        /// <summary>
        /// Capture a frame and extract features
        /// </summary>
        public async Task<BiometricFeatures?> CaptureAndExtractFeaturesAsync()
        {
            var frameData = await _cameraProvider.CaptureFrameAsync();

            if (frameData == null || frameData.Length == 0)
                return null;

            // Assume 640x480 for now - in production, get actual dimensions
            int width = 640;
            int height = 480;

            return _featureExtractor.ExtractFeatures(frameData, width, height);
        }

        /// <summary>
        /// Stop capturing
        /// </summary>
        public async Task StopCaptureAsync()
        {
            await _cameraProvider.StopCaptureAsync();
        }
    }
}
