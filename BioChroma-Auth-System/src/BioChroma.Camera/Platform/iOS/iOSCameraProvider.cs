using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BioChroma.Core.Interfaces;

#if IOS
using AVFoundation;
using CoreVideo;
using CoreMedia;
using CoreGraphics;
using Foundation;
#endif

namespace BioChroma.Camera.Platform.iOS
{
    /// <summary>
    /// iOS camera provider using AVFoundation
    /// Requires iOS 13.0 or higher
    /// </summary>
    public class iOSCameraProvider : ICameraProvider
    {
#if IOS
        private AVCaptureSession? _captureSession;
        private AVCaptureVideoDataOutput? _videoOutput;
        private byte[]? _lastFrame;
        private readonly object _frameLock = new object();
        private bool _isCapturing = false;

        public bool IsCapturing => _isCapturing;

        public async Task<bool> RequestPermissionAsync()
        {
            var status = AVCaptureDevice.GetAuthorizationStatus(AVAuthorizationMediaType.Video);

            if (status == AVAuthorizationStatus.NotDetermined)
            {
                var granted = await AVCaptureDevice.RequestAccessForMediaTypeAsync(AVAuthorizationMediaType.Video);
                return granted;
            }

            return status == AVAuthorizationStatus.Authorized;
        }

        public async Task<IEnumerable<CameraDevice>> GetDevicesAsync()
        {
            return await Task.Run(() =>
            {
                var devices = new List<CameraDevice>();

                var discoverySession = AVCaptureDeviceDiscoverySession.Create(
                    new[] { AVCaptureDeviceType.BuiltInWideAngleCamera },
                    AVMediaTypes.Video,
                    AVCaptureDevicePosition.Unspecified
                );

                if (discoverySession?.Devices != null)
                {
                    foreach (var device in discoverySession.Devices)
                    {
                        string name = device.Position == AVCaptureDevicePosition.Front
                            ? "Front Camera"
                            : "Back Camera";

                        devices.Add(new CameraDevice
                        {
                            Id = device.UniqueID,
                            Name = name,
                            IsAvailable = true
                        });
                    }
                }

                return devices;
            });
        }

        public async Task StartCaptureAsync(string deviceId, int width = 640, int height = 480)
        {
            await Task.Run(() =>
            {
                try
                {
                    _captureSession = new AVCaptureSession();
                    _captureSession.BeginConfiguration();

                    // Find device
                    AVCaptureDevice? device = null;
                    if (!string.IsNullOrEmpty(deviceId))
                    {
                        device = AVCaptureDevice.DeviceWithUniqueID(deviceId);
                    }

                    device ??= AVCaptureDevice.GetDefaultDevice(AVMediaTypes.Video);

                    if (device == null)
                    {
                        throw new InvalidOperationException("No camera device found");
                    }

                    // Create input
                    var input = AVCaptureDeviceInput.FromDevice(device, out var error);
                    if (error != null || input == null)
                    {
                        throw new InvalidOperationException($"Failed to create input: {error?.LocalizedDescription}");
                    }

                    if (_captureSession.CanAddInput(input))
                    {
                        _captureSession.AddInput(input);
                    }

                    // Create output
                    _videoOutput = new AVCaptureVideoDataOutput();
                    _videoOutput.SetSampleBufferDelegateQueue(new SampleBufferDelegate(this),
                        new global::CoreFoundation.DispatchQueue("VideoQueue"));

                    if (_captureSession.CanAddOutput(_videoOutput))
                    {
                        _captureSession.AddOutput(_videoOutput);
                    }

                    _captureSession.CommitConfiguration();
                    _captureSession.StartRunning();

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
                if (_captureSession != null)
                {
                    _captureSession.StopRunning();
                    _captureSession.Dispose();
                    _captureSession = null;
                }

                _videoOutput?.Dispose();
                _videoOutput = null;

                _isCapturing = false;
                _lastFrame = null;
            });
        }

        private void OnFrameReceived(CMSampleBuffer sampleBuffer)
        {
            using (var pixelBuffer = sampleBuffer.GetImageBuffer() as CVPixelBuffer)
            {
                if (pixelBuffer == null)
                    return;

                pixelBuffer.Lock(CVPixelBufferLock.ReadOnly);

                try
                {
                    int width = (int)pixelBuffer.Width;
                    int height = (int)pixelBuffer.Height;

                    // Convert to RGB
                    byte[] rgb = ConvertToRGB(pixelBuffer, width, height);

                    lock (_frameLock)
                    {
                        _lastFrame = rgb;
                    }
                }
                finally
                {
                    pixelBuffer.Unlock(CVPixelBufferLock.ReadOnly);
                }
            }
        }

        private byte[] ConvertToRGB(CVPixelBuffer pixelBuffer, int width, int height)
        {
            byte[] rgb = new byte[width * height * 3];

            // Get base address
            IntPtr baseAddress = pixelBuffer.BaseAddress;

            // Assuming BGRA format (most common on iOS)
            int bytesPerRow = (int)pixelBuffer.BytesPerRow;

            unsafe
            {
                byte* ptr = (byte*)baseAddress.ToPointer();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int srcIndex = y * bytesPerRow + x * 4;
                        int dstIndex = (y * width + x) * 3;

                        // BGRA to RGB
                        rgb[dstIndex] = ptr[srcIndex + 2];     // R
                        rgb[dstIndex + 1] = ptr[srcIndex + 1]; // G
                        rgb[dstIndex + 2] = ptr[srcIndex];     // B
                    }
                }
            }

            return rgb;
        }

        private class SampleBufferDelegate : AVCaptureVideoDataOutputSampleBufferDelegate
        {
            private readonly iOSCameraProvider _provider;

            public SampleBufferDelegate(iOSCameraProvider provider)
            {
                _provider = provider;
            }

            public override void DidOutputSampleBuffer(AVCaptureOutput captureOutput,
                CMSampleBuffer sampleBuffer, AVCaptureConnection connection)
            {
                _provider.OnFrameReceived(sampleBuffer);
                sampleBuffer.Dispose();
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
