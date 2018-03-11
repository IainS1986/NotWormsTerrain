using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain.Services.Concrete
{
    /// <summary>
    /// Terrain Subservice used to smooth contours
    /// </summary>
    public class ContourSmoothingService : IContourSmoothingService
    {
        /// <summary>
        /// How many smoothing passes are run on the contour. This allows for additional smoothing to be achieved
        /// with a single call to the service.
        /// </summary>
        public const int cNumSmoothingPasses = 5;

        /// <summary>
        /// Array of weight values to apply to vertices when smoothing. The center
        /// of the array is applied to the vertex being smoothed, with the previous indices
        /// being applied to the previous vertices, and visa versa with the latter indices/vertices
        /// The weight values are multiplied by the vertex values to allow vertices closer to the point
        /// being smoothed to have more of an effect.
        /// </summary>
        public int[] m_smoothWeights = new int[] { 1, 2, 1 };

        /// <summary>
        /// Smooths all contours in the provided Ground terrain
        /// This will smooth both contours and holes.
        /// </summary>
        /// <param name="ground">The ground terrain we want to smooth all contours in</param>
        public void SmoothContours(Ground ground)
        {
            SmoothContours(ground.Chunks);
        }
        
        /// <summary>
        /// Smooths all contours in the provided list of ground chunks
        /// This will smooth both contours and holes
        /// </summary>
        /// <param name="chunks">The ground chunks we want to smooth all contours in</param>
        public void SmoothContours(Dictionary<int, GroundChunk> chunks)
        {
            foreach (var chunk in chunks)
            {
                chunk.Value.Dispose();
                chunk.Value.Poly = null;
                //Smooth Contours
                for (int i = 0; i < cNumSmoothingPasses; i++)
                    SmoothContour(chunk.Value.Edge);

                foreach (var hole in chunk.Value.Holes)
                    for (int i = 0; i < cNumSmoothingPasses; i++)
                        SmoothContour(hole);
            }
        }

        /// <summary>
        /// Smooths the provided vertexsequence. Will iterate round all the vertices in the contour
        /// and will average out vertex positions based on thepositions of its neighbours. This will
        /// smooth out the edges of the contour and remove sharp angles.
        /// </summary>
        /// <param name="vs">The vertex sequence to smooth</param>
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

        /// <summary>
        /// Removes vertices not needed from all contours in the provided ground terrain.
        /// This will aim to remove vertices that produce straight lines
        /// This will remove vertices in both contours and holes.
        /// </summary>
        /// <param name="ground">The ground terrain we want to remove vertices from all contours in</param>
        public void RemoveVertices(Ground ground)
        {
            RemoveVertices(ground.Chunks);
        }

        /// <summary>
        /// Removes vertices not needed from all contours in the provided ground chunks.
        /// This will aim to remove vertices that produce straight lines
        /// This will remove vertices in both contours and holes
        /// </summary>
        /// <param name="chunks">The ground chunks we want to remove vertices from all contours in</param>
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

        /// <summary>
        /// Removes vertices from the provided vertex sequence. This will scan round all vertices in the contour
        /// and if the vector to and from each vertex to its neighbours is the same (i.e. 3 vertices are in a straight
        /// line), then the center vertex is culled. Reducing the amount of vertices in the contour but retaining the shape.
        /// </summary>
        /// <param name="vs">The vertex sequence to cull vertices from.</param>
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
}
