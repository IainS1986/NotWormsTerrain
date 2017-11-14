using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshService : IMeshService
{
    private static Material[] Materials = new Material[2];

    public MeshService()
    {
        Materials = new Material[]
        {
            null,
            (Material)Resources.Load("Materials/Earth"),
            (Material)Resources.Load("Materials/Stone")
        };
    }

    public void BuildMesh(Ground ground)
    {
        BuildMesh(ground.Chunks);
    }

    public void BuildMesh(Dictionary<int, GroundChunk> chunks)
    {
        foreach(var chunk in chunks)
        {
            BuildMesh(chunk.Value);
        }
    }

    public void BuildMesh(GroundChunk chunk)
    {
        if (chunk.Mesh == null)
        {
            chunk.Mesh = new Mesh();
            chunk.Mesh.name = "GroundChunkMesh";
        }

        if (chunk.GameObject == null)
        {
            chunk.GameObject = new GameObject("GroundChunk");
            MeshFilter mf = chunk.GameObject.AddComponent<MeshFilter>();
            MeshRenderer mr = chunk.GameObject.AddComponent<MeshRenderer>();

            mf.mesh = chunk.Mesh;
        
            mr.material = Materials[chunk.GroundType];
        }

        //Build Verts
        Vector3[] verts = new Vector3[chunk.Poly.Points.Count];
        int[] tris = new int[chunk.Poly.Tris.Length];

        for (int i = 0; i < verts.Length; i++) verts[i] = new Vector3(chunk.Poly.Points[i].X, chunk.Poly.Points[i].Y, 0);
        for (int i = 0; i < tris.Length; i++) tris[i] = chunk.Poly.Tris[i];

        //Tris are backwards
        for (int i = 0; i < tris.Length; i += 3)
        {
            int temp = tris[i];
            tris[i] = tris[i + 2];
            tris[i + 2] = temp;
        }

        //UVs and Normals aren't used yet but here would be where they are calculated
        Vector2[] uvs = new Vector2[verts.Length];
        Vector3[] norms = new Vector3[verts.Length];

        chunk.Mesh.vertices = verts;
        chunk.Mesh.triangles = tris;
        chunk.Mesh.uv = uvs;
        chunk.Mesh.normals = norms;

        //mesh.Optimize();
        chunk.Mesh.RecalculateNormals();
    }
}
