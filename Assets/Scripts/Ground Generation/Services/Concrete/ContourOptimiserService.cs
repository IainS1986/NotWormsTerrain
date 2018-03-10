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
            foreach (var chunk in chunks)
            {
                if(ContourIsTooSmall(chunk.Value.Edge))
                {
                    //Whole chunk needs removing...
                }

                foreach (var hole in chunk.Value.Holes)
                {
                    if(ContourIsTooSmall(hole))
                    {
                        //Fill in the hole
                    }
                }
            }
        }

        private bool ContourIsTooSmall(VertexSequence seq)
        {
            return false;
        }
    }
}
