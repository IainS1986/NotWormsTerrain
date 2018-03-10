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

    public void SmoothContours(Dictionary<int, GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.Value.Dispose();
            chunk.Value.Poly = null;
            //Smooth Contours
            for (int i = 0; i < m_numSmoothingPasses; i++)
                SmoothContour(chunk.Value.Edge);

            foreach (var hole in chunk.Value.Holes)
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
                x += vs[i + j].x * m_smoothWeights[j + (m_smoothWeights.Length / 2)];
                y += vs[i + j].y * m_smoothWeights[j + (m_smoothWeights.Length / 2)];
                w += m_smoothWeights[j + (m_smoothWeights.Length / 2)];
            }
            x /= w;
            y /= w;
            smooth.Add(new Vector2() { x = x, y = y });
        }

        vs.CopyFrom(smooth);
    }

    public void RemoveVertices(Ground ground)
    {
        RemoveVertices(ground.Chunks);
    }

    public void RemoveVertices(Dictionary<int, GroundChunk> chunks)
    {
        foreach (var chunk in chunks)
        {
            chunk.Value.Dispose();
            chunk.Value.Poly = null;
            //Smooth Contours
            RemoveVertices(chunk.Value.Edge);
            foreach (var hole in chunk.Value.Holes)
                RemoveVertices(hole);
        }
    }

    private void RemoveVertices(VertexSequence vs)
    {
        int count = 0;
        for (int i = 0; i < vs.Count; i++)
        {
            Vector2 a = vs[i - 1];
            Vector2 b = vs[i];
            Vector2 c = vs[i + 1];

            //Remove b if its the same X or same Y as a and c
            bool abX = UnityEngine.Mathf.Approximately(a.x, b.x);
            bool bcX = UnityEngine.Mathf.Approximately(b.x, c.x);
            bool abY = UnityEngine.Mathf.Approximately(a.y, b.y);
            bool bcY = UnityEngine.Mathf.Approximately(b.y, c.y);
            if ((abX && bcX) || (abY && bcY))
            {
                count++;
                vs.Remove(b);
                i--;
            }
        }
    }
}
