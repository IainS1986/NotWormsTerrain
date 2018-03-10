using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Utility
{
    public static class Vector2DExtensionMethods
    {
        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
    }
}
