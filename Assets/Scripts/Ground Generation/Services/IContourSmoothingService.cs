﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services
{
    public interface IContourSmoothingService
    {
        void SmoothContours(Ground ground);
        void RemoveVertices(Ground ground);

        void SmoothContours(Dictionary<int, GroundChunk> chunks);
        void RemoveVertices(Dictionary<int, GroundChunk> chunks);
    }
}