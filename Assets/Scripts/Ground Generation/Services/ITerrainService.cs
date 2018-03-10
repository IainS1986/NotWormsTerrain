using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface ITerrainService
    {
        Ground Ground { get; }
        void SetDimensions(int width, int height);
        bool GroundChangeSelectiveRebuild(int x, int y, int s, int type);
        void Generate();
        void March();
        void SmoothContours();
        void RemoveVertices();
        void Decomp();
        void Mesh();
        void Lips();
    }
}
