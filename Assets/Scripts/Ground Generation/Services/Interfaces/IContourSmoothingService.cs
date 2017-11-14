using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContourSmoothingService
{
    void SmoothContours(Ground ground);
    void RemoveVertices(Ground ground);

    void SmoothContours(List<GroundChunk> chunks);
    void RemoveVertices(List<GroundChunk> chunks);
}
