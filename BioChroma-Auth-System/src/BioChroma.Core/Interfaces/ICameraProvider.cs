using System.Collections.Generic;
using System.Threading.Tasks;

namespace BioChroma.Core.Interfaces
{
    /// <summary>
    /// Camera device information
    /// </summary>
    public class CameraDevice
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
    }

    /// <summary>
    /// Cross-platform camera provider interface
    /// </summary>
    public interface ICameraProvider
    {
        /// <summary>
        /// Request camera permission from the system
        /// </summary>
        Task<bool> RequestPermissionAsync();

        /// <summary>
        /// Get list of available camera devices
        /// </summary>
        Task<IEnumerable<CameraDevice>> GetDevicesAsync();

        /// <summary>
        /// Start capturing frames from specified camera
        /// </summary>
        /// <param name="deviceId">Camera device ID</param>
        /// <param name="width">Desired frame width</param>
        /// <param name="height">Desired frame height</param>
        Task StartCaptureAsync(string deviceId, int width = 640, int height = 480);

        /// <summary>
        /// Capture a single frame
        /// </summary>
        /// <returns>Frame data as byte array (RGB)</returns>
        Task<byte[]?> CaptureFrameAsync();

        /// <summary>
        /// Stop capturing
        /// </summary>
        Task StopCaptureAsync();

        /// <summary>
        /// Check if camera is currently capturing
        /// </summary>
        bool IsCapturing { get; }
    }
}
