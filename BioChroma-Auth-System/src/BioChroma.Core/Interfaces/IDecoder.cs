using System.Collections.Generic;
using System.Threading.Tasks;
using BioChroma.Core.Models;

namespace BioChroma.Core.Interfaces
{
    /// <summary>
    /// Interface for BioChroma code decoding
    /// </summary>
    public interface IDecoder
    {
        /// <summary>
        /// Decode a BioChroma code from captured frames
        /// </summary>
        /// <param name="code">The BioChroma code to decode</param>
        /// <returns>Verification result</returns>
        Task<VerificationResult> DecodeAsync(BioChromaCode code);

        /// <summary>
        /// Decode from multiple camera frames (for scanning)
        /// </summary>
        /// <param name="frames">Multiple frames captured from camera</param>
        /// <returns>Verification result</returns>
        Task<VerificationResult> DecodeFromFramesAsync(List<byte[]> frames);

        /// <summary>
        /// Extract raw data from a BioChroma code
        /// </summary>
        /// <param name="code">The BioChroma code</param>
        /// <returns>Decoded byte data</returns>
        Task<byte[]> ExtractDataAsync(BioChromaCode code);
    }
}
