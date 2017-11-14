using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground
{
    public Dot[,] Dots { get; set; }
    public Dictionary<int, GroundChunk> Chunks { get; set; }
    public int Height { get; set; }
    public int Width { get; set; }
    public GroundStage CurrentStage { get; set; }

    public Ground(int _w, int _h)
    {
        Width = _w;
        Height = _h;

        Dots = new Dot[Height, Width];
        Chunks = new Dictionary<int, GroundChunk>();
        CurrentStage = GroundStage.NONE;
    }

    public void ResetChunks()
    {
        Chunks = new Dictionary<int, GroundChunk>();
        for(int y = 0; y<Height; y++)
        {
            for(int x = 0; x<Width; x++)
            {
                Dots[y, x].Chunk = 0;
            }
        }
    }
}

public struct Dot
{
    public int Value { get; set; }
    public int Chunk { get; set; }
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
