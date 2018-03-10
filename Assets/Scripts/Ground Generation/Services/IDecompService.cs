using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface IDecompService
    {
        void Decomp(Ground ground);

        void Decomp(Dictionary<int, GroundChunk> chunks);
    }
}
