using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BioChroma.Core.Interfaces;
using OpenCvSharp;

namespace BioChroma.Camera.Platform
{
    /// <summary>
    /// OpenCV-based cross-platform camera provider
    /// Works on Windows, macOS, and Linux
    /// </summary>
    public class OpenCVCameraProvider : ICameraProvider, IDisposable
    {
        private VideoCapture? _capture;
        private bool _isCapturing = false;
        private int _currentDeviceIndex = -1;
        private int _width = 640;
        private int _height = 480;

        public bool IsCapturing => _isCapturing;

        public async Task<bool> RequestPermissionAsync()
        {
            // OpenCV doesn't have explicit permission request
            // Permission is handled by the OS when we try to open the camera
            await Task.Delay(10);
            return true;
        }

        public async Task<IEnumerable<CameraDevice>> GetDevicesAsync()
        {
            return await Task.Run(() =>
            {
                var devices = new List<CameraDevice>();

                // Try to enumerate cameras (typically 0-5 is enough)
                for (int i = 0; i < 6; i++)
                {
                    using (var test = new VideoCapture(i))
                    {
                        if (test.IsOpened())
                        {
                            devices.Add(new CameraDevice
                            {
                                Id = i.ToString(),
                                Name = $"Camera {i}",
                                IsAvailable = true
                            });
                            test.Release();
                        }
                    }
                }

                // If no cameras found, add a default one
                if (devices.Count == 0)
                {
                    devices.Add(new CameraDevice
                    {
                        Id = "0",
                        Name = "Default Camera",
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
                // Parse device ID
                if (!int.TryParse(deviceId, out int index))
                {
                    index = 0;
                }

                _width = width;
                _height = height;
                _currentDeviceIndex = index;

                // Create video capture
                _capture = new VideoCapture(index);

                if (!_capture.IsOpened())
                {
                    throw new InvalidOperationException($"Failed to open camera {index}");
                }

                // Set resolution
                _capture.Set(VideoCaptureProperties.FrameWidth, width);
                _capture.Set(VideoCaptureProperties.FrameHeight, height);

                // Set FPS to 30
                _capture.Set(VideoCaptureProperties.Fps, 30);

                _isCapturing = true;
            });
        }

        public async Task<byte[]?> CaptureFrameAsync()
        {
            if (!_isCapturing || _capture == null || !_capture.IsOpened())
            {
                return null;
            }

            return await Task.Run(() =>
            {
                try
                {
                    using (var frame = new Mat())
                    {
                        // Read frame from camera
                        if (!_capture.Read(frame) || frame.Empty())
                        {
                            return null;
                        }

                        // Resize if necessary
                        if (frame.Width != _width || frame.Height != _height)
                        {
                            using (var resized = new Mat())
                            {
                                Cv2.Resize(frame, resized, new Size(_width, _height));
                                return ConvertMatToRGBBytes(resized);
                            }
                        }

                        return ConvertMatToRGBBytes(frame);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error capturing frame: {ex.Message}");
                    return null;
                }
            });
        }

        public async Task StopCaptureAsync()
        {
            await Task.Run(() =>
            {
                if (_capture != null)
                {
                    _capture.Release();
                    _capture.Dispose();
                    _capture = null;
                }

                _isCapturing = false;
                _currentDeviceIndex = -1;
            });
        }

        private byte[] ConvertMatToRGBBytes(Mat frame)
        {
            // Ensure frame is in BGR format (OpenCV default)
            Mat bgrFrame;
            if (frame.Channels() == 4)
            {
                bgrFrame = new Mat();
                Cv2.CvtColor(frame, bgrFrame, ColorConversionCodes.BGRA2BGR);
            }
            else if (frame.Channels() == 1)
            {
                bgrFrame = new Mat();
                Cv2.CvtColor(frame, bgrFrame, ColorConversionCodes.GRAY2BGR);
            }
            else
            {
                bgrFrame = frame;
            }

            // Convert BGR to RGB
            using (var rgbFrame = new Mat())
            {
                Cv2.CvtColor(bgrFrame, rgbFrame, ColorConversionCodes.BGR2RGB);

                // Convert to byte array
                int width = rgbFrame.Width;
                int height = rgbFrame.Height;
                byte[] data = new byte[width * height * 3];

                // Copy pixel data
                unsafe
                {
                    byte* ptr = (byte*)rgbFrame.Data;
                    for (int i = 0; i < data.Length; i++)
                    {
                        data[i] = ptr[i];
                    }
                }

                if (bgrFrame != frame)
                {
                    bgrFrame.Dispose();
                }

                return data;
            }
        }

        public void Dispose()
        {
            StopCaptureAsync().Wait();
        }
    }
}
