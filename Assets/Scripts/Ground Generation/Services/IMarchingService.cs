using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface IMarchingService
    {
        void March(int xx, int yy, int ww, int hh, Ground ground, out Dictionary<int, GroundChunk> chunks);
    }
}
