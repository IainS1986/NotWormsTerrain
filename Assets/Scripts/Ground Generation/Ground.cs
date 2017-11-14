using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground
{
    public int[,] Dots { get; set; }
    public int[,] DotToChunk { get; set; }
    public Dictionary<int, GroundChunk> IDToChunk { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public List<GroundChunk> Chunks { get; set; }
    public GroundStage CurrentStage { get; set; }

    public Ground(int _w, int _h)
    {
        Width = _w;
        Height = _h;

        Dots = new int[Height, Width];
        DotToChunk = new int[Height, Width];
        IDToChunk = new Dictionary<int, GroundChunk>();
        CurrentStage = GroundStage.NONE;
        Chunks = new List<GroundChunk>();
    }
}

public enum GroundStage
{
    NONE = 0,
    DOTS,
    MARCHING,
    SMOOTHED,
    VERTEX_REMOVAL,
    DECOMP,
    MESH,
    LIPS,
};
