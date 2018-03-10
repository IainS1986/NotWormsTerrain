using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface IMeshService
    {
        void BuildMesh(Ground ground);
        void BuildMesh(Dictionary<int, GroundChunk> chunks);
        void BuildLips(Ground ground);
        void BuildLips(Dictionary<int, GroundChunk> chunks);
    }
}
