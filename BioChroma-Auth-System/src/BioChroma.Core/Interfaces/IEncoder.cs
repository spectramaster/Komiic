using System.Threading.Tasks;
using BioChroma.Core.Models;

namespace BioChroma.Core.Interfaces
{
    /// <summary>
    /// Interface for BioChroma code encoding
    /// </summary>
    public interface IEncoder
    {
        /// <summary>
        /// Generate a BioChroma code from biometric features
        /// </summary>
        /// <param name="features">Biometric features extracted from camera</param>
        /// <param name="userId">User identifier to encode</param>
        /// <returns>Generated BioChroma code</returns>
        Task<BioChromaCode> EncodeAsync(BiometricFeatures features, string userId);

        /// <summary>
        /// Encode raw data into a BioChroma code
        /// </summary>
        /// <param name="data">Raw byte data to encode</param>
        /// <param name="features">Optional biometric features for personalization</param>
        /// <returns>Generated BioChroma code</returns>
        Task<BioChromaCode> EncodeDataAsync(byte[] data, BiometricFeatures? features = null);
    }
}
