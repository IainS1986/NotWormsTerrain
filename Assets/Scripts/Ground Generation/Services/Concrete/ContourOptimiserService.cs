using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services.Concrete
{
    /// <summary>
    /// Terrain Generation Subservice used to allow optimisation to be run
    /// on Ground Chunks before they are processed further. Namely this will
    /// have functions to remove ground chunks deemed too small to produce nice
    /// results.
    /// </summary>
    public class ContourOptimiserService : IContourOptimiserService
    {
        /// <summary>
        /// Min amount of vertices a contour must contain to prevent
        /// it from being culled.
        /// </summary>
        private const int cMinVertexCount = 16;

        /// <summary>
        /// Removes all "small" ground chunks in the whole terrain
        /// </summary>
        /// <param name="ground">The Ground terrain object to process</param>
        public void RemoveSmallContours(Ground ground)
        {
            RemoveSmallContours(ground, ground.Chunks);
        }

        /// <summary>
        /// Removes all "small" ground chunks in the terrain from the sublist of chunks
        /// provided
        /// </summary>
        /// <param name="ground">The Ground terrain object to remove the chunks from</param>
        /// <param name="chunks">The sublist of ground chunks to process</param>
        public void RemoveSmallContours(Ground ground, Dictionary<int, GroundChunk> chunks)
        {
            List<int> chunksToRemove = new List<int>();
            foreach (var chunk in chunks)
            {
                if(ContourIsTooSmall(chunk.Value.Edge))
                {
                    //Whole chunk needs removing...
                    chunksToRemove.Add(chunk.Key);
                    continue;
                }

                List<VertexSequence> holesToRemove = new List<VertexSequence>();
                foreach (var hole in chunk.Value.Holes)
                {
                    if(ContourIsTooSmall(hole))
                    {
                        //Fill in the hole
                        holesToRemove.Add(hole);
                    }
                }

                foreach (var hole in holesToRemove)
                    chunk.Value.Holes.Remove(hole);
            }

            //Clear dot chunk links
            for (int a = 0; a < ground.Width; a++)
            {
                for (int b = 0; b < ground.Height; b++)
                {
                    if (ground.Dots[b, a].Chunk != 0 && chunksToRemove.Contains(ground.Dots[b, a].Chunk))
                    {
                        ground.Dots[b, a].Value = 0;
                        ground.Dots[b, a].Chunk = 0;
                    }
                }
            }

            //Clear chunk objects
            foreach (int id in chunksToRemove)
            {
                chunks.Remove(id);
            }
        }

        // TODO would be nice for this to actually review poly surface area....but for now lets keep
        // it super simple and just cull any contours under a certain vertex length...as we can assume they
        // will be too small.
        private bool ContourIsTooSmall(VertexSequence seq)
        {
            return (seq == null) ? true : seq.Count < cMinVertexCount;
        }
    }
}
