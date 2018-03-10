using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface IContourOptimiserService
    {
        void RemoveSmallContours(Ground ground);

        void RemoveSmallContours(Ground ground, Dictionary<int, GroundChunk> chunks);
    }
}
