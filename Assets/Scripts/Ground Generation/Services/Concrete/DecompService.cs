using Common;
using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Polygon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services.Concrete
{
    /// <summary>
    /// Terrain Generation Subservice that decomposites ground chunks ready for mesh generation
    /// and physics simulation. This interacts with the Poly2Tri libraries from Farseer Physics
    /// which will do the actual Decomposition itself.
    /// </summary>
    public class DecompService : IDecompService
    {
        /// <summary>
        /// Decomposites all ground chunks in the provided ground terrain object.
        /// This will decomposite ground chunks that contain holes too.
        /// </summary>
        /// <param name="ground">The ground terrain to decomposite all chunks in</param>
        public void Decomp(Ground ground)
        {
            Decomp(ground.Chunks);
        }

        /// <summary>
        /// Decomposites all ground chunks passed in
        /// This will decomposite ground chunks that contain holes too.
        /// </summary>
        /// <param name="chunks">The sublist of all chunks we want to decomposite</param>
        public void Decomp(Dictionary<int, GroundChunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                chunk.Value.Dispose();
                Decomp(chunk.Value);
            }
        }

        /// <summary>
        /// This decomposites the provided ground chunk, supporting holes too.
        /// This will convert the ground chunk into a Polygon object that is then
        /// passed to the Poly2Tri library which returns the triangle arrays of 
        /// decomposition.
        /// </summary>
        /// <param name="chunk"></param>
        private void Decomp(GroundChunk chunk)
        {
            List<PolygonPoint> edge = new List<PolygonPoint>();
            for (int i = 0; i < chunk.Edge.Count; i++)
                edge.Add(new PolygonPoint(chunk.Edge[i].x, chunk.Edge[i].y));

            List<Polygon> holes = new List<Polygon>();
            if (chunk.Holes != null)
            {
                for (int i = 0; i < chunk.Holes.Count; i++)
                {
                    List<PolygonPoint> hole = new List<PolygonPoint>();
                    for (int j = 0; j < chunk.Holes[i].Count; j++)
                        hole.Add(new PolygonPoint(chunk.Holes[i][j].x, chunk.Holes[i][j].y));

                    holes.Add(new Polygon(hole));
                }
            }

            Polygon poly = new Polygon(edge);
            for (int i = 0; i < holes.Count; i++)
                poly.AddHole(holes[i]);


            //Triangulate!
            P2T.Triangulate(poly);
            IList<TriangulationPoint> points = poly.Points;
            IList<DelaunayTriangle> tris = poly.Triangles;

            List<Vector2> finalPoints = new List<Vector2>();
            int[] finalTris = new int[tris.Count * 3];
            Dictionary<TriangulationPoint, int> pointToIndex = new Dictionary<TriangulationPoint, int>();
            for (int t = 0; t < tris.Count; t++)
            {
                DelaunayTriangle tri = tris[t];
                for (int i = 0; i < 3; i++)
                {
                    //check if the points are in the dictionary, if not add it with the next index
                    int val = 0;
                    if (pointToIndex.TryGetValue(tri.Points[i], out val))
                    {
                        finalTris[t * 3 + i] = val;
                    }
                    else
                    {
                        finalPoints.Add(new Vector2() { x = tri.Points[i].Xf, y = tri.Points[i].Yf });
                        pointToIndex[tri.Points[i]] = finalPoints.Count - 1;
                        finalTris[t * 3 + i] = finalPoints.Count - 1;
                    }
                }
            }

            Decomp result = new Decomp();
            result.Points = finalPoints;
            result.Tris = finalTris;
            chunk.Poly = result;
        }
    }
}
