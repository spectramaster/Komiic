using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioChroma.Core.Interfaces;

#if ANDROID
using Android.Hardware;
using Android.Graphics;
using Android.Runtime;
using Android.Content;
using AndroidX.Core.Content;
using AndroidX.Core.App;
#endif

namespace BioChroma.Camera.Platform.Android
{
    /// <summary>
    /// Android camera provider using Camera2 API
    /// Requires Android 5.0 (API 21) or higher
    /// </summary>
    public class AndroidCameraProvider : ICameraProvider
    {
#if ANDROID
        private bool _isCapturing = false;
        private global::Android.Hardware.Camera? _camera;
        private byte[]? _lastFrame;
        private readonly object _frameLock = new object();

        public bool IsCapturing => _isCapturing;

        public async Task<bool> RequestPermissionAsync()
        {
            // Camera permission should be requested in MainActivity
            // This method just checks if we have permission
            await Task.Delay(10);

            var context = global::Android.App.Application.Context;
            var result = ContextCompat.CheckSelfPermission(context, global::Android.Manifest.Permission.Camera);

            return result == global::Android.Content.PM.Permission.Granted;
        }

        public async Task<IEnumerable<CameraDevice>> GetDevicesAsync()
        {
            return await Task.Run(() =>
            {
                var devices = new List<CameraDevice>();
                int cameraCount = global::Android.Hardware.Camera.NumberOfCameras;

                for (int i = 0; i < cameraCount; i++)
                {
                    var info = new global::Android.Hardware.Camera.CameraInfo();
                    global::Android.Hardware.Camera.GetCameraInfo(i, info);

                    string name = info.Facing == CameraFacing.Front ? "Front Camera" : "Back Camera";

                    devices.Add(new CameraDevice
                    {
                        Id = i.ToString(),
                        Name = name,
                        IsAvailable = true
                    });
                }

                return devices;
            });
        }

        public async Task StartCaptureAsync(string deviceId, int width = 640, int height = 480)
        {
            await Task.Run(() =>
            {
                if (!int.TryParse(deviceId, out int cameraId))
                {
                    cameraId = 0;
                }

                try
                {
                    _camera = global::Android.Hardware.Camera.Open(cameraId);

                    if (_camera == null)
                    {
                        throw new InvalidOperationException($"Failed to open camera {cameraId}");
                    }

                    // Set parameters
                    var parameters = _camera.GetParameters();
                    parameters?.SetPreviewSize(width, height);
                    _camera.SetParameters(parameters);

                    // Set preview callback
                    _camera.SetPreviewCallback(new CameraPreviewCallback(this));

                    // Start preview
                    _camera.StartPreview();

                    _isCapturing = true;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to start camera: {ex.Message}", ex);
                }
            });
        }

        public async Task<byte[]?> CaptureFrameAsync()
        {
            if (!_isCapturing)
                return null;

            return await Task.Run(() =>
            {
                lock (_frameLock)
                {
                    return _lastFrame;
                }
            });
        }

        public async Task StopCaptureAsync()
        {
            await Task.Run(() =>
            {
                if (_camera != null)
                {
                    _camera.StopPreview();
                    _camera.SetPreviewCallback(null);
                    _camera.Release();
                    _camera = null;
                }

                _isCapturing = false;
                _lastFrame = null;
            });
        }

        private void OnPreviewFrame(byte[] data, global::Android.Hardware.Camera camera)
        {
            lock (_frameLock)
            {
                // Convert YUV to RGB
                var parameters = camera.GetParameters();
                int width = parameters?.PreviewSize?.Width ?? 640;
                int height = parameters?.PreviewSize?.Height ?? 480;

                _lastFrame = ConvertYUVToRGB(data, width, height);
            }
        }

        private byte[] ConvertYUVToRGB(byte[] yuv, int width, int height)
        {
            // Simplified YUV420 to RGB conversion
            int frameSize = width * height;
            byte[] rgb = new byte[frameSize * 3];

            for (int j = 0, yp = 0; j < height; j++)
            {
                int uvp = frameSize + (j >> 1) * width;
                int u = 0, v = 0;

                for (int i = 0; i < width; i++, yp++)
                {
                    int y = (0xff & yuv[yp]) - 16;
                    if (y < 0) y = 0;

                    if ((i & 1) == 0)
                    {
                        v = (0xff & yuv[uvp++]) - 128;
                        u = (0xff & yuv[uvp++]) - 128;
                    }

                    int y1192 = 1192 * y;
                    int r = (y1192 + 1634 * v);
                    int g = (y1192 - 833 * v - 400 * u);
                    int b = (y1192 + 2066 * u);

                    r = Math.Max(0, Math.Min(r, 262143));
                    g = Math.Max(0, Math.Min(g, 262143));
                    b = Math.Max(0, Math.Min(b, 262143));

                    int index = yp * 3;
                    rgb[index] = (byte)(r >> 10);
                    rgb[index + 1] = (byte)(g >> 10);
                    rgb[index + 2] = (byte)(b >> 10);
                }
            }

            return rgb;
        }

        private class CameraPreviewCallback : Java.Lang.Object, global::Android.Hardware.Camera.IPreviewCallback
        {
            private readonly AndroidCameraProvider _provider;

            public CameraPreviewCallback(AndroidCameraProvider provider)
            {
                _provider = provider;
            }

            public void OnPreviewFrame(byte[]? data, global::Android.Hardware.Camera? camera)
            {
                if (data != null && camera != null)
                {
                    _provider.OnPreviewFrame(data, camera);
                }
            }
        }
#else
        public bool IsCapturing => false;

        public Task<bool> RequestPermissionAsync() => Task.FromResult(false);

        public Task<IEnumerable<CameraDevice>> GetDevicesAsync() =>
            Task.FromResult<IEnumerable<CameraDevice>>(new List<CameraDevice>());

        public Task StartCaptureAsync(string deviceId, int width = 640, int height = 480) =>
            Task.CompletedTask;

        public Task<byte[]?> CaptureFrameAsync() => Task.FromResult<byte[]?>(null);

        public Task StopCaptureAsync() => Task.CompletedTask;
#endif
    }
}
