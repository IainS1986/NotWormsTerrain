using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    /// <summary>
    /// Terrain Generation Subservice used to allow optimisation to be run
    /// on Ground Chunks before they are processed further. Namely this will
    /// have functions to remove ground chunks deemed too small to produce nice
    /// results.
    /// </summary>
    public interface IContourOptimiserService
    {
        /// <summary>
        /// Removes all "small" ground chunks in the whole terrain
        /// </summary>
        /// <param name="ground">The Ground terrain object to process</param>
        void RemoveSmallContours(Ground ground);

        /// <summary>
        /// Removes all "small" ground chunks in the terrain from the sublist of chunks
        /// provided
        /// </summary>
        /// <param name="ground">The Ground terrain object to remove the chunks from</param>
        /// <param name="chunks">The sublist of ground chunks to process</param>
        void RemoveSmallContours(Ground ground, Dictionary<int, GroundChunk> chunks);
    }
}
