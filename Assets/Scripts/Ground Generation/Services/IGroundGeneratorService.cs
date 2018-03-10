using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface IGroundGeneratorService
    {
        void Generate(Ground ground);
        void DotRemoval(int xx, int yy, int ww, int hh, Ground ground);
        void RemoveDiagonals(int xx, int yy, int ww, int hh, Ground ground);
        bool SafeGroundFillForGenerator(int x, int y, int r, int type, Ground ground);
    }
}
