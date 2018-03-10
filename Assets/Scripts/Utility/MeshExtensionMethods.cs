using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Utility
{
    public static class MeshExtensionMethods
    {
	    public static Mesh Copy(this Mesh m)
        {
            Mesh copy = new Mesh();

            Vector3[] verts_orig = m.vertices;
            Vector3[] norms_orig = m.normals;
            Vector2[] uvs_orig = m.uv;
            int[] tris_orig = m.triangles;

            Vector3[] verts = new Vector3[verts_orig.Length];
            Vector3[] norms = new Vector3[norms_orig.Length];
            Vector2[] uvs = new Vector2[uvs_orig.Length];
            int[] tris = new int[tris_orig.Length];

            for (int i = 0; i < verts.Length; i++) verts[i] = verts_orig[i];
            for (int i = 0; i < norms.Length; i++) norms[i] = norms_orig[i];
            for (int i = 0; i < uvs.Length; i++) uvs[i] = uvs_orig[i];
            for (int i = 0; i < tris.Length; i++) tris[i] = tris_orig[i];

            copy.vertices = verts;
            copy.uv = uvs;
            copy.normals = norms;
            copy.triangles = tris;

            return copy;
        }
    }
}
