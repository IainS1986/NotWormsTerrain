using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    /// <summary>
    /// Terrain Generation Subservice that decomposites ground chunks ready for mesh generation
    /// and physics simulation
    /// </summary>
    public interface IDecompService
    {
        /// <summary>
        /// Decomposites all ground chunks in the provided ground terrain object.
        /// </summary>
        /// <param name="ground">The ground terrain to decomposite all chunks in</param>
        void Decomp(Ground ground);

        /// <summary>
        /// Decomposites all ground chunks passed in
        /// </summary>
        /// <param name="chunks">The sublist of all chunks we want to decomposite</param>
        void Decomp(Dictionary<int, GroundChunk> chunks);
    }
}
