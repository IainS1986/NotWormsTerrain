using Common;
using Poly2Tri.Triangulation;
using Poly2Tri.Triangulation.Delaunay;
using Poly2Tri.Triangulation.Polygon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decomp
{
    public List<Point> Points;
    public int[] Tris;
}

public class DecompService : IDecompService
{
    public void Decomp(ref List<GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            Decomp(chunk);
        }
    }

    //TODO Drop vertex point ect and just use PolygonPoint throughout?
    private void Decomp(GroundChunk chunk)
    {
        List<PolygonPoint> edge = new List<PolygonPoint>();
        for (int i = 0; i < chunk.Edge.Count; i++)
            edge.Add(new PolygonPoint(chunk.Edge[i].X, chunk.Edge[i].Y));

        List<Polygon> holes = new List<Polygon>();
        if (chunk.Holes != null)
        {
            for (int i = 0; i < chunk.Holes.Count; i++)
            {
                List<PolygonPoint> hole = new List<PolygonPoint>();
                for (int j = 0; j < chunk.Holes[i].Count; j++)
                    hole.Add(new PolygonPoint(chunk.Holes[i][j].X, chunk.Holes[i][j].Y));

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

        List<Point> finalPoints = new List<Point>();
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
                    finalPoints.Add(new Point() { X = tri.Points[i].Xf, Y = tri.Points[i].Yf });
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
