using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMeshService
{
    void BuildMesh(Ground ground);
    void BuildMesh(Dictionary<int, GroundChunk> chunks);
}
