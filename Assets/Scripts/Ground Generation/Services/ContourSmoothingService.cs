using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContourSmoothingService : IContourSmoothingService
{
    public int[] m_smoothWeights = new int[] { 1, 2, 1 };
    public int m_numSmoothingPasses = 5;

    public void SmoothContours(Ground ground)
    {
        SmoothContours(ground.Chunks);
    }

    public void SmoothContours(List<GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.Poly = null;
            //Smooth Contours
            for (int i = 0; i < m_numSmoothingPasses; i++)
                SmoothContour(chunk.Edge);

            foreach (var hole in chunk.Holes)
                for (int i = 0; i < m_numSmoothingPasses; i++)
                    SmoothContour(hole);
        }
    }

    private void SmoothContour(VertexSequence vs)
    {
        VertexSequence smooth = new VertexSequence();
        for (int i = 0; i < vs.Count; i++)
        {
            float x = 0;
            float y = 0;
            int w = 0;
            for (int j = -m_smoothWeights.Length / 2; j <= m_smoothWeights.Length / 2; j++)
            {
                x += vs[i + j].X * m_smoothWeights[j + (m_smoothWeights.Length / 2)];
                y += vs[i + j].Y * m_smoothWeights[j + (m_smoothWeights.Length / 2)];
                w += m_smoothWeights[j + (m_smoothWeights.Length / 2)];
            }
            x /= w;
            y /= w;
            smooth.Add(new Point() { X = x, Y = y });
        }

        vs.CopyFrom(smooth);
    }

    public void RemoveVertices(Ground ground)
    {
        RemoveVertices(ground.Chunks);
    }

    public void RemoveVertices(List<GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.Poly = null;
            //Smooth Contours
            RemoveVertices(chunk.Edge);
            foreach (var hole in chunk.Holes)
                RemoveVertices(hole);
        }
    }

    private void RemoveVertices(VertexSequence vs)
    {
        int count = 0;
        for (int i = 0; i < vs.Count; i++)
        {
            Point a = vs[i - 1];
            Point b = vs[i];
            Point c = vs[i + 1];

            //Remove b if its the same X or same Y as a and c
            bool abX = UnityEngine.Mathf.Approximately(a.X, b.X);
            bool bcX = UnityEngine.Mathf.Approximately(b.X, c.X);
            bool abY = UnityEngine.Mathf.Approximately(a.Y, b.Y);
            bool bcY = UnityEngine.Mathf.Approximately(b.Y, c.Y);
            if ((abX && bcX) || (abY && bcY))
            {
                count++;
                vs.Remove(b);
                i--;
            }
        }
    }
}
