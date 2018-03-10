using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DebugMain : MonoBehaviour
{
    private ITerrainService m_terrainService;

    [SerializeField]
    private int m_width = 512;

    [SerializeField]
    private int m_height = 128;

    [SerializeField]
    public Color EARTH_COL = new UnityEngine.Color(1,0,0,1f);

    [SerializeField]
    public Color STONE_COL = new UnityEngine.Color(0,1,0,1f);

    [SerializeField]
    private bool m_renderExtraDebug = false;

    private Material m_lineMaterial;

    public ITerrainService TerrainService
    {
        get { return m_terrainService; }
    }

    private bool m_debugEnabled = false;
    public bool DebugEnabled
    {
        get { return m_debugEnabled; }
        set { m_debugEnabled = value; OnDebugToggle(); }
    }

    public void Start()
    {
        m_lineMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
        m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

        m_terrainService = new TerrainService();
        m_terrainService.SetDimensions(m_width, m_height);
    }

    public void Update()
    {
        if (m_renderExtraDebug && !DebugEnabled)
            DebugEnabled = true;
        else if (!m_renderExtraDebug && DebugEnabled)
            DebugEnabled = false;
    }

    // OpenGL Rendering
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

    private void OnDebugToggle()
    {
        //Get All Meshrenderers
        var mrs = FindObjectsOfType<MeshRenderer>();
        foreach (var mr in mrs)
            mr.enabled = !DebugEnabled;
    }

    void OnGUI()
    {
        BottomPane();
    }

    void BottomPane()
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

                Color col = EARTH_COL;
                if (val == 1)
                    col = EARTH_COL;
                else
                    col = STONE_COL;

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
                Color col = EARTH_COL;
                if (chunk.Value.GroundType == 1)
                    col = EARTH_COL;
                else
                    col = STONE_COL;

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
                Color col = EARTH_COL;
                if (chunk.Value.GroundType == 1)
                    col = EARTH_COL;
                else
                    col = STONE_COL;

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
