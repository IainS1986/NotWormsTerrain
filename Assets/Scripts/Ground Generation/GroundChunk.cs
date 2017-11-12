using Poly2Tri.Triangulation.Polygon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChunk 
{
    private static int s_nextID = 0;
    public static int NextID
    {
        get { return ++s_nextID; }
    }

    public VertexSequence Edge { get; set; }

    public List<VertexSequence> Holes { get; set; }

    public Decomp Poly { get; set; }

    public int GroundType { get; set; }

    public GroundChunk()
    {
        Holes = new List<VertexSequence>();
	}
}
