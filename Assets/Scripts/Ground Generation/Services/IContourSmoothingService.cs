using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    /// <summary>
    /// Terrain Subservice used to smooth contours
    /// </summary>
    public interface IContourSmoothingService
    {
        /// <summary>
        /// Smooths all contours in the provided Ground terrain
        /// This will smooth both contours and holes.
        /// </summary>
        /// <param name="ground">The ground terrain we want to smooth all contours in</param>
        void SmoothContours(Ground ground);

        /// <summary>
        /// Removes vertices not needed from all contours in the provided ground terrain.
        /// This will aim to remove vertices that produce straight lines
        /// This will remove vertices in both contours and holes.
        /// </summary>
        /// <param name="ground">The ground terrain we want to remove vertices from all contours in</param>
        void RemoveVertices(Ground ground);

        /// <summary>
        /// Smooths all contours in the provided list of ground chunks
        /// This will smooth both contours and holes
        /// </summary>
        /// <param name="chunks">The ground chunks we want to smooth all contours in</param>
        void SmoothContours(Dictionary<int, GroundChunk> chunks);

        /// <summary>
        /// Removes vertices not needed from all contours in the provided ground chunks.
        /// This will aim to remove vertices that produce straight lines
        /// This will remove vertices in both contours and holes
        /// </summary>
        /// <param name="chunks">The ground chunks we want to remove vertices from all contours in</param>
        void RemoveVertices(Dictionary<int, GroundChunk> chunks);
    }
}
