using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMarchingService
{
    List<GroundChunk> March(int xx, int yy, int ww, int hh, Ground ground);
}
