using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContourSmoothingService
{
    void SmoothContours(ref List<GroundChunk> chunks);
    void RemoveVertices(ref List<GroundChunk> chunks);
}
