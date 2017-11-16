using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshService : IMeshService
{
    private static Mesh sStraightMesh;
    private static int[] sStraightVertexPairings;
    private static int[] sStraightLeftHandVerts;
    private static int[] sStraightRightHandVerts;

    private static Material[] Materials = new Material[2];
    private static Material[] Materials_Lips = new Material[2];

    public MeshService()
    {
        Materials = new Material[]
        {
            null,
            (Material)Resources.Load("Materials/Earth"),
            (Material)Resources.Load("Materials/Stone")
        };

        Materials_Lips = new Material[]
        {
            null,
            (Material)Resources.Load("Materials/Earth_Lip"),
            (Material)Resources.Load("Materials/Stone_Lip")
        };

        //Build the straight mesh we use as a template for lips, placing each template at each vertexsequence segment
        BuildStraightMesh();
    }

    private static void BuildStraightMesh()
    {
        Mesh straight = new Mesh();

        Vector3[] verts = new Vector3[6];
        int[] tris = new int[4 * 3];
        Vector2[] uvs = new Vector2[6];
        Vector3[] norms = new Vector3[6];

        verts[0] = new Vector3(-0.5f, -0.5f, 0f);
        verts[1] = new Vector3(-0.5f, 0, 0f);
        verts[2] = new Vector3(-0.5f, 0, 2.5f);

        verts[3] = new Vector3(0.5f, -0.5f, 0f);
        verts[4] = new Vector3(0.5f, 0, 0f);
        verts[5] = new Vector3(0.5f, 0, 2.5f);

        int t = 0;
        tris[t++] = 0; tris[t++] = 1; tris[t++] = 4;
        tris[t++] = 4; tris[t++] = 3; tris[t++] = 0;
        tris[t++] = 1; tris[t++] = 2; tris[t++] = 5;
        tris[t++] = 5; tris[t++] = 4; tris[t++] = 1;

        sStraightVertexPairings = new int[] { 3, 4, 5, 0, 1, 2 };
        sStraightLeftHandVerts = new int[] { 0, 1, 2 };
        sStraightRightHandVerts = new int[] { 3, 4, 5 };

        straight.vertices = verts;
        straight.uv = uvs;
        straight.triangles = tris;
        straight.normals = norms;

        straight.RecalculateNormals();

        sStraightMesh = straight;
    }

    #region Main Mesh
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
    #endregion

    #region Lips
    public void BuildLips(Ground ground)
    {
        BuildLips(ground.Chunks);
    }

    public void BuildLips(Dictionary<int, GroundChunk> chunks)
    {
        foreach(var chunk in chunks)
        {
            BuildLips(chunk.Value);
        }
    }

    private void BuildLips(GroundChunk chunk)
    {
        BuildLipForContour(chunk.Edge, Materials_Lips[chunk.GroundType]);

        if(chunk.Holes!= null)
        {
            foreach(var hole in chunk.Holes)
            {
                BuildLipForContour(hole, Materials_Lips[chunk.GroundType]);
            }
        }
    }

    private void BuildLipForContour(VertexSequence contour, Material material)
    {
        Mesh lip = new Mesh();

        //Each point the contour has 3 verts connected to the next point on the contour. The verts are in an L-Shape
        //It sits over the edge of the main contour mesh
        int totalVerts = contour.Count * 3;

        Vector3[] verts = new Vector3[totalVerts];
        Vector3[] norms = new Vector3[totalVerts];
        Vector2[] uvs = new Vector2[totalVerts];

        //The L-Shape has 4 tris per segment. 2 forming the rectangle on the face, 2 forming the rectangle on the "floor/roof/depth"
        int[] tris = new int[contour.Count * 4];
    }
    #endregion
}
