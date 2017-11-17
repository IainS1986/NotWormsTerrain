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

    public Mesh Mesh { get; set; }

    public GameObject GameObject { get; set; }

    public List<GameObject> Lips { get; set; }

    public List<Mesh> LipMeshes { get; set; }

    public int GroundType { get; set; }

    public GroundChunk()
    {
        Holes = new List<VertexSequence>();
        Lips = new List<GameObject>();
        LipMeshes = new List<Mesh>();
	}

    public void Dispose()
    {
        DisposeMesh();
        DisposeLips();
    }

    public void DisposeMesh()
    {
        if(Mesh!= null)
        {
            MonoBehaviour.Destroy(Mesh);
        }

        if(GameObject!= null)
        {
            MonoBehaviour.Destroy(GameObject);
        }
    }

    public void DisposeLips()
    {
        if(Lips!= null)
        {
            foreach(var lip in Lips)
            {
                MonoBehaviour.Destroy(lip);
            }
            Lips.Clear();
        }

        if(LipMeshes!= null)
        {
            foreach(var lip in LipMeshes)
            {
                MonoBehaviour.Destroy(lip);
            }
            LipMeshes.Clear();
        }
    }
}
