using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terrain.Utility;
using UnityEngine;
using ClipperLib;

namespace Terrain.Services.Concrete
{
    public class ClipperMeshService : MeshService
    {
        protected override void BuildLipForContour(GroundChunk chunk, VertexSequence contour, Material material)
        {
            //Prepare Unity Gameobject data
            Mesh lip = new Mesh();
            lip.name = "GroundChunkLipMesh";

            List<IntPoint> outerLipPath = new List<IntPoint>();
            List<IntPoint> innerLipPath = new List<IntPoint>();

            GameObject lipObjt = new GameObject("GroundChunkLip");
            MeshFilter mf = lipObjt.AddComponent<MeshFilter>();
            MeshRenderer mr = lipObjt.AddComponent<MeshRenderer>();
            mf.mesh = lip;
            mr.material = Materials_Lips[chunk.GroundType];

            //Each point the contour has 3 verts connected to the next point on the contour. The verts are in an L-Shape
            //It sits over the edge of the main contour mesh
            int totalVerts = contour.Count * 3;
            int totalTris = contour.Count * 4;

            Vector3[] verts = new Vector3[totalVerts];
            Vector3[] norms = new Vector3[totalVerts];
            Vector2[] uvs = new Vector2[totalVerts];

            //The L-Shape has 4 tris per segment. 2 forming the rectangle on the face, 2 forming the rectangle on the "floor/roof/depth"
            //Everything is multiplied by 3 as its 3 verts per tri...(the verts are index)
            int[] tris = new int[totalTris * 3];

            int t = 0;
            int v = 0;
            Vector2 previousNorm = new Vector2();
            for (int i = 0; i < contour.Count; i++)
            {
                Vector2 p = contour[i - 1];
                Vector2 q = contour[i];
                Vector2 r = contour[i + 1];

                //TODO Rotation, use p and r to work out the "normal" direciton of q so we rotate the lip correctly...
                Vector2 qp = (p - q).normalized;
                Vector2 qr = (r - q).normalized;

                Vector2 norm = (qp + qr).normalized;
                norm *= sLipSize;

                //Convex/Concave flip
                var cross = Vector2DExtensionMethods.Cross(qp, qr);
                if (cross < 0)
                    norm = -norm;

                //If we have a perfectly straight line, then the normal (or bisect) will be 0,0
                //So as a simple fix for this, we just copy the lip vector from the previous lip
                if (Mathf.Approximately(qp.x, -qr.x) &&
                    Mathf.Approximately(qp.y, -qr.y))
                {
                    norm = previousNorm;
                }

                previousNorm = norm;

                //Add top surface
                verts[v++] = new Vector3(q.x, q.y, sLipOverhang);
                verts[v++] = new Vector3(q.x, q.y, sLipDepth);
                verts[v++] = new Vector3(q.x + norm.x, q.y + norm.y, sLipOverhang);

                //Add outer lip point
                outerLipPath.Add(new IntPoint(q.x, q.y));
                //Add inner lip point
                innerLipPath.Add(new IntPoint(q.x + norm.x, q.y + norm.y));

                //Add tris to the next indices
                //Note - Last segment wraps round to the first indices...
                if (i == contour.Count - 1)
                {
                    //TOP
                    tris[t++] = v - 3; tris[t++] = v - 2; tris[t++] = 0;
                    tris[t++] = v - 2; tris[t++] = 1; tris[t++] = 0;

                    //LIP
                    tris[t++] = v - 3; tris[t++] = 0; tris[t++] = 2;
                    tris[t++] = v - 1; tris[t++] = v - 3; tris[t++] = 2;
                }
                else
                {
                    //TOP
                    tris[t++] = v - 3; tris[t++] = v - 2; tris[t++] = v;
                    tris[t++] = v - 2; tris[t++] = v + 1; tris[t++] = v;

                    //LIP
                    tris[t++] = v - 3; tris[t++] = v; tris[t++] = v + 2;
                    tris[t++] = v - 1; tris[t++] = v - 3; tris[t++] = v + 2;
                }
            }

            //Clipper union on the lip face in the XY-axis
            innerLipPath.Reverse();
            var polys = Clipper.SimplifyPolygons(new List<List<IntPoint>> { outerLipPath, innerLipPath});
            //Triangulate using Poly2Tri

            lip.vertices = verts;
            lip.normals = norms;
            lip.triangles = tris;
            lip.uv = uvs;

            lip.RecalculateNormals();

            chunk.Lips.Add(lipObjt);
            chunk.LipMeshes.Add(lip);
        }
    }
}
