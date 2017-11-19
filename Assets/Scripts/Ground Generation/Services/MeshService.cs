using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshService : IMeshService
{
    private static Material[] Materials = new Material[2];
    private static Material[] Materials_Lips = new Material[2];

    private static float sLipDepth = 5;

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
        foreach (var chunk in chunks)
            chunk.Value.DisposeLips();

        foreach(var chunk in chunks)
        {
            BuildLips(chunk.Value);
        }
    }

    private void BuildLips(GroundChunk chunk)
    {
        BuildLipForContour(chunk, chunk.Edge, Materials_Lips[chunk.GroundType]);

        if(chunk.Holes!= null)
        {
            foreach(var hole in chunk.Holes)
            {
                BuildLipForContour(chunk, hole, Materials_Lips[chunk.GroundType]);
            }
        }
    }

    private void BuildLipForContour(GroundChunk chunk, VertexSequence contour, Material material)
    {
        Mesh lip = new Mesh();
        lip.name = "GroundChunkLipMesh";

        GameObject lipObjt = new GameObject("GroundChunkLip");
        MeshFilter mf = lipObjt.AddComponent<MeshFilter>();
        MeshRenderer mr = lipObjt.AddComponent<MeshRenderer>();
        mf.mesh = lip;
        mr.material = Materials_Lips[chunk.GroundType];

        //Each point the contour has 3 verts connected to the next point on the contour. The verts are in an L-Shape
        //It sits over the edge of the main contour mesh
        int totalVerts = contour.Count * 2;

        Vector3[] verts = new Vector3[totalVerts];
        Vector3[] norms = new Vector3[totalVerts];
        Vector2[] uvs = new Vector2[totalVerts];

        //The L-Shape has 4 tris per segment. 2 forming the rectangle on the face, 2 forming the rectangle on the "floor/roof/depth"
        //Everything is multiplied by 3 as its 3 verts per tri...(the verts are index)
        int[] tris = new int[totalVerts* 3];

        int t = 0;
        int v = 0;
        for(int i=0; i<contour.Count; i++)
        {
            Point p = contour[i - 1];
            Point q = contour[i];
            Point r = contour[i + 1];

            //TODO Rotation, use p and r to work out the "normal" direciton of q so we rotate the lip correctly...

            //Add top surface
            verts[v++] = new Vector3(q.X, q.Y, 0);
            verts[v++] = new Vector3(q.X, q.Y, sLipDepth);

            //Add 2 tris to the next indices
            //Note - Last segment wraps round to the first indices...
            if (i == contour.Count - 1)
            {
                tris[t++] = v - 2; tris[t++] = v - 1; tris[t++] = 0;
                tris[t++] = v - 1; tris[t++] = 1; tris[t++] = 0;
            }
            else
            {
                tris[t++] = v-2; tris[t++] = v-1; tris[t++] = v;
                tris[t++] = v-1; tris[t++] = v+1; tris[t++] = v;
            }
        }

        lip.vertices = verts;
        lip.normals = norms;
        lip.triangles = tris;
        lip.uv = uvs;

        lip.RecalculateNormals();

        chunk.Lips.Add(lipObjt);
        chunk.LipMeshes.Add(lip);
    }
    #endregion
}
