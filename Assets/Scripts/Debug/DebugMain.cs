using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Terrain.Services;
using TinyIoC;

namespace Terrain.Debugging
{
    /// <summary>
    /// The main debug class that handles displaying buttons
    /// that allow the user to trigger the various stages of terrain
    /// generation
    /// </summary>
    public class DebugMain : MonoBehaviour
    {
        /// <summary>
        /// The ITerrainService that contains the ground data the buttons
        /// in this class interact with
        /// </summary>
        private ITerrainService m_terrainService;

        /// <summary>
        /// The width in dots of the terrain object
        /// </summary>
        [SerializeField]
        private int m_width = 512;

        /// <summary>
        /// The height in dots of the terrain object
        /// </summary>
        [SerializeField]
        private int m_height = 128;

        /// <summary>
        /// The colour of the dots for the "earth" material
        /// </summary>
        [SerializeField]
        public Color m_earthColour = new UnityEngine.Color(1,0,0,1f);

        /// <summary>
        /// The colour of the dots for the "stone" material
        /// </summary>
        [SerializeField]
        public Color m_stoneColour = new UnityEngine.Color(0,1,0,1f);

        /// <summary>
        /// Bool to denote if extra debug information should be displayed. This
        /// includes rendering more vertice information and toggling mesh renderers
        /// This is set during the Update function based on the DebugEnabled bool.
        /// The reason for this mapping is to allow the bool to be altered in the editor
        /// at runtime.
        /// </summary>
        [SerializeField]
        private bool m_renderExtraDebug = false;

        /// <summary>
        /// The Material used in the GL rendering of the dots and edges
        /// </summary>
        private Material m_lineMaterial;

        /// <summary>
        /// The ITerrainService that contains the ground data the buttons
        /// in this class interact with
        /// </summary>
        public ITerrainService TerrainService
        {
            get { return m_terrainService; }
        }

        /// <summary>
        /// Bool to denote if extra debug information should be displayed. This
        /// includes rendering more vertice information and toggling mesh renderers
        /// Can be toggled in the editor during runtime.
        /// </summary>
        private bool m_debugEnabled = false;
        public bool DebugEnabled
        {
            get { return m_debugEnabled; }
            set { m_debugEnabled = value; OnDebugToggle(); }
        }

        /// <summary>
        /// Unity Start function called when the gameobject instantiates
        /// </summary>
        public void Start()
        {
            m_lineMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
            m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

            m_terrainService = TinyIoCContainer.Current.Resolve<ITerrainService>();
            m_terrainService.SetDimensions(m_width, m_height);
        }

        /// <summary>
        /// Unity update functino called once per frame. Will toggle extra debug
        /// information based on the bool that can be set in the inspector from
        /// the editor at runtime.
        /// </summary>
        public void Update()
        {
            if (m_renderExtraDebug && !DebugEnabled)
                DebugEnabled = true;
            else if (!m_renderExtraDebug && DebugEnabled)
                DebugEnabled = false;
        }

        /// <summary>
        /// Unity function called after every frame is rendered to allow further GL
        /// rendering calls. Will render, dot, edge, decomp and vertice information
        /// to the screen in debug GL quads and lines.
        /// </summary>
        public void OnPostRender()
        {
            // set the current material
            m_lineMaterial.SetPass( 0 );

            RenderDots();
            RenderEdges();
            RenderDecomp();
            if (DebugEnabled)
                RenderVertices();
        }

        /// <summary>
        /// When extra debug is toggled the mesh renderers will be hidden to
        /// allow for the GL mesh rendering to be visible.
        /// </summary>
        private void OnDebugToggle()
        {
            //Get All Meshrenderers
            var mrs = FindObjectsOfType<MeshRenderer>();
            foreach (var mr in mrs)
                mr.enabled = !DebugEnabled;
        }

        /// <summary>
        /// Unity OnGUI call triggered every frame that will display the 
        /// terrain widget and the buttons to allow various terrain generation steps
        /// to be triggered.
        /// </summary>
        void OnGUI()
        {
            Rect boundary = DebugButton.GetMainWidgetRect();
            GUI.Box(boundary, "GROUND GEN");

            int i = 1;
            DebugButton.AddButton(boundary, i++, "DOTS", () => m_terrainService.Generate());
            DebugButton.AddButton(boundary, i++, "MARCH", () => m_terrainService.March());
            DebugButton.AddButton(boundary, i++, "SMOOTH", () => m_terrainService.SmoothContours());
            DebugButton.AddButton(boundary, i++, "REMOVE", () => m_terrainService.RemoveVertices());
            DebugButton.AddButton(boundary, i++, "DECOMP", () => m_terrainService.Decomp());
            DebugButton.AddButton(boundary, i++, "MESH", () => m_terrainService.Mesh());
            DebugButton.AddButton(boundary, i++, "LIP", () => m_terrainService.Lips());
            DebugButton.AddButton(boundary, i++, "ALL", () =>
            {
                m_terrainService.Generate();
                m_terrainService.March();
                m_terrainService.SmoothContours();
                m_terrainService.RemoveVertices();
                m_terrainService.Decomp();
                m_terrainService.Mesh();
                m_terrainService.Lips();
            });
        }

        /// <summary>
        /// Renders all the dots in the terrain using GL quads.
        /// </summary>
        private void RenderDots()
        {
            if (m_terrainService.Ground == null ||
                m_terrainService.Ground.Dots == null ||
                m_terrainService.Ground.CurrentStage >= GroundStage.MESH)
                return;

            // render dots
            GL.Begin(GL.QUADS);

            float s = 0.1f;
        
            GL.Color(Color.gray);
            GL.Vertex3(-1, -1, 0);  
            GL.Vertex3(-1, m_height + 1, 0);  
            GL.Vertex3(m_width + 1, m_height + 1, 0);  
            GL.Vertex3(m_width + 1, -1, 0);
        
            for(int y=0; y < m_height; y++)
            {
                for(int x=0; x < m_width; x++)
                {
                    int val  = m_terrainService.Ground.Dots[y,x].Value;

                    if(val <= 0)
                        continue;

                    Color col = m_stoneColour;
                    if (val == 1)
                        col = m_earthColour;

                    GL.Color(col);
                    float xx = x;
                    float yy = y;
                    GL.Vertex3(xx - s, yy - s, 0);  
                    GL.Vertex3(xx - s, yy + s, 0);  
                    GL.Vertex3(xx + s, yy + s, 0);  
                    GL.Vertex3(xx + s, yy - s, 0);
                }
            }   
            GL.End();
        }

        /// <summary>
        /// Renders all contour edges in the terrain using GL lines.
        /// </summary>
        private void RenderEdges()
        {
            GL.Begin(GL.LINES);

            if (m_terrainService.Ground != null &&
                m_terrainService.Ground.Chunks!= null &&
                m_terrainService.Ground.CurrentStage < GroundStage.MESH)
            {
                foreach(var chunk in m_terrainService.Ground.Chunks)
                {
                    //Set Colour
                    Color col = m_stoneColour;
                    if (chunk.Value.GroundType == 1)
                        col = m_earthColour;

                    GL.Color(col);
                    for(int i=0; i<chunk.Value.Edge.Count; i++)
                    {
                        GL.Vertex3(chunk.Value.Edge[i].x, chunk.Value.Edge[i].y, 0);
                        GL.Vertex3(chunk.Value.Edge[i + 1].x, chunk.Value.Edge[i + 1].y, 0);

                        //Render point
                        GL.End();
                        GL.Begin(GL.QUADS);
                        GL.Color(Color.black);
                        float s = 0.05f;
                        float xx = chunk.Value.Edge[i].x;
                        float yy = chunk.Value.Edge[i].y;
                        GL.Vertex3(xx - s, yy - s, 0);
                        GL.Vertex3(xx + s, yy - s, 0);
                        GL.Vertex3(xx + s, yy + s, 0);
                        GL.Vertex3(xx - s, yy + s, 0);
                        GL.End();
                        GL.Begin(GL.LINES);
                        GL.Color(col);
                    }

                    foreach(VertexSequence hole in chunk.Value.Holes)
                    {
                        for(int i=0; i<hole.Count; i++)
                        {
                            GL.Color(Color.blue);
                            GL.Vertex3(hole[i].x, hole[i].y, 0);
                            GL.Vertex3(hole[i + 1].x, hole[i + 1].y, 0);

                            //Render Point
                            GL.End();
                            GL.Begin(GL.QUADS);
                            GL.Color(Color.black);
                            float s = 0.05f;
                            float xx = hole[i].x;
                            float yy = hole[i].y;
                            GL.Vertex3(xx - s, yy - s, 0);
                            GL.Vertex3(xx + s, yy - s, 0);
                            GL.Vertex3(xx + s, yy + s, 0);
                            GL.Vertex3(xx - s, yy + s, 0);
                            GL.End();
                            GL.Begin(GL.LINES);
                            GL.Color(col);
                        }
                    }
                }
            }

            GL.End();
        }

        /// <summary>
        /// Renders all the decomp object outlines using GL Lines
        /// </summary>
        private void RenderDecomp()
        {
            GL.Begin(GL.LINES);

            if (m_terrainService.Ground != null &&
                m_terrainService.Ground.Chunks != null &&
                m_terrainService.Ground.CurrentStage < GroundStage.MESH)
            {
                foreach (var chunk in m_terrainService.Ground.Chunks)
                {
                    if (chunk.Value.Poly == null || chunk.Value.Poly.Tris == null)
                        continue;

                    var poly = chunk.Value.Poly;

                    //Set Colour
                    Color col = m_stoneColour;
                    if (chunk.Value.GroundType == 1)
                        col = m_earthColour;

                    List<Vector2> points = poly.Points;
                    int[] tris = poly.Tris;

                    GL.Color(col);
                    for (int i = 0; i < tris.Length / 3; i++)
                    {
                        Vector2 pa = points[tris[i * 3 + 0]];
                        Vector2 pb = points[tris[i * 3 + 1]];
                        Vector2 pc = points[tris[i * 3 + 2]];

                        GL.Vertex3(pa.x, pa.y, 0);
                        GL.Vertex3(pb.x, pb.y, 0);

                        GL.Vertex3(pb.x, pb.y, 0);
                        GL.Vertex3(pc.x, pc.y, 0);

                        GL.Vertex3(pc.x, pc.y, 0);
                        GL.Vertex3(pa.x, pa.y, 0);
                    }
                }
            }

            GL.End();
        }

        /// <summary>
        /// Renders all the mesh and lip vertices as GL quads with lips as GL lines
        /// </summary>
        private void RenderVertices()
        {
            GL.Begin(GL.LINES);

            if (m_terrainService.Ground != null &&
                m_terrainService.Ground.Chunks != null &&
                m_terrainService.Ground.CurrentStage >= GroundStage.MESH)
            {
                GL.Begin(GL.QUADS);
                foreach (var chunk in m_terrainService.Ground.Chunks)
                {
                    if (chunk.Value == null || chunk.Value.LipMeshes == null)
                        return;

                    Color[] cols = new Color[] { Color.red, Color.red, Color.blue };
                    foreach(var lip in chunk.Value.LipMeshes)
                    {
                        var verts = lip.vertices;

                        for(int i=0; i<verts.Length; i++)
                        {
                            Vector3 v = verts[i];

                            var mod = i % 3;
                            GL.Color(cols[mod]);

                            //Render point
                            float s = 0.05f;
                            float xx = v.x;
                            float yy = v.y;
                            float zz = v.z;
                            GL.Vertex3(xx - s, yy - s, zz);
                            GL.Vertex3(xx + s, yy - s, zz);
                            GL.Vertex3(xx + s, yy + s, zz);
                            GL.Vertex3(xx - s, yy + s, zz);
                        }

                        //Render "lip edges"
                        GL.End();
                        GL.Begin(GL.LINES);
                        GL.Color(Color.white);
                        for(int i=0; i<verts.Length; i+=3)
                        {
                            var v1 = verts[i];
                            var v2 = verts[i + 1];
                            var v3 = verts[i + 2];

                            //Render Edge V1-V2 and V3-V1
                            GL.Vertex3(v1.x, v1.y, v1.z);
                            GL.Vertex3(v2.x, v2.y, v2.z);
                            GL.Vertex3(v1.x, v1.y, v1.z);
                            GL.Vertex3(v3.x, v3.y, v3.z);
                        }
                        GL.End();
                        GL.Begin(GL.QUADS);

                    }
                }
                GL.End();
            }
        }
    }
}
