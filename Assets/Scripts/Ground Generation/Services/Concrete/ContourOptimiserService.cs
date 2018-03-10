using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services.Concrete
{
    public class ContourOptimiserService : IContourOptimiserService
    {
        public void RemoveSmallContours(Ground ground)
        {
            RemoveSmallContours(ground, ground.Chunks);
        }

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

            foreach (int id in chunksToRemove)
                chunks.Remove(id);
        }

        // TODO would be nice for this to actually review poly surface area....but for now lets keep
        // it super simple and just cull any contours under a certain vertex length...as we can assume they
        // will be too small.
        private bool ContourIsTooSmall(VertexSequence seq)
        {
            return (seq == null) ? true : seq.Count < 10;
        }
    }
}
