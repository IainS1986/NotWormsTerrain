﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Main : MonoBehaviour
{
    [SerializeField]
    private int m_width = 512;

    [SerializeField]
    private int m_height = 128;

    [SerializeField]
    public Color EARTH_COL = new UnityEngine.Color(1,0,0,1f);

    [SerializeField]
    public Color STONE_COL = new UnityEngine.Color(0,1,0,1f);

    private GroundGenerator m_groundGenerator;
    public GroundGenerator Ground
    {
        get { return m_groundGenerator; }
        set { m_groundGenerator = value; }
    }

    private Material m_lineMaterial;

    public void Start()
    {
        m_lineMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
        m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

        m_groundGenerator = new GroundGenerator(m_width, m_height);
    }

    // OpenGL Rendering
    public void OnPostRender()
    {
        // set the current material
        m_lineMaterial.SetPass( 0 );

        RenderDots();
        RenderEdges();
        RenderDecomp();
    }

    void OnGUI()
    {
        BottomPane();
    }

    void BottomPane(){
        int b = 10;
        int w = 150;
        int h = 20;
        
        int num_buttons = 7;
        Rect boundary = new Rect(b/2, Screen.height - h - b - (b/2) - h, num_buttons * (b+w), h + b + h);
        GUI.Box(boundary, "GROUND GEN");

        int i = 0;
        AddButton(i++, "DOTS", () => m_groundGenerator.Generate(), b, w, h);
        AddButton(i++, "MARCH", () => m_groundGenerator.March(), b, w, h);
        AddButton(i++, "SMOOTH", () => m_groundGenerator.SmoothContours(), b, w, h);
        AddButton(i++, "REMOVE", () => m_groundGenerator.RemoveVertices(), b, w, h);
        AddButton(i++, "DECOMP", () => m_groundGenerator.Decomp(), b, w, h);
        AddButton(i++, "ALL", () =>
        {
            m_groundGenerator.Generate();
            m_groundGenerator.March();
            m_groundGenerator.SmoothContours();
            m_groundGenerator.RemoveVertices();
            m_groundGenerator.Decomp();
        }, b, w, h);
    }

    private void AddButton(int i, string s, Action action, int b, int w, int h)
    {
        Rect r = new Rect(b * (i+1) + w * i, Screen.height - h - b, w, h);
        if (GUI.Button(r, s))
        {
            DateTime before = DateTime.Now;
            if (action != null)
                action();
            DateTime after = DateTime.Now;
            Debug.Log(string.Format("{0} took {1}ms", s, (after - before).TotalMilliseconds));
        }
    }

    private void RenderDots()
    {
        if (m_groundGenerator.Ground == null ||
            m_groundGenerator.Ground.Dots == null)
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
                int val  = m_groundGenerator.Ground.Dots[y,x];

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

        if (m_groundGenerator.Ground != null &&
            m_groundGenerator.Ground.Chunks!= null)
        {
            foreach(GroundChunk chunk in m_groundGenerator.Ground.Chunks)
            {
                //Set Colour
                Color col = EARTH_COL;
                if (chunk.GroundType == 1)
                    col = EARTH_COL;
                else
                    col = STONE_COL;

                GL.Color(col);
                for(int i=0; i<chunk.Edge.Count; i++)
                {
                    GL.Vertex3(chunk.Edge[i].X, chunk.Edge[i].Y, 0);
                    GL.Vertex3(chunk.Edge[i + 1].X, chunk.Edge[i + 1].Y, 0);

                    //Render point
                    GL.End();
                    GL.Begin(GL.QUADS);
                    GL.Color(Color.black);
                    float s = 0.05f;
                    float xx = chunk.Edge[i].X;
                    float yy = chunk.Edge[i].Y;
                    GL.Vertex3(xx - s, yy - s, 0);
                    GL.Vertex3(xx + s, yy - s, 0);
                    GL.Vertex3(xx + s, yy + s, 0);
                    GL.Vertex3(xx - s, yy + s, 0);
                    GL.End();
                    GL.Begin(GL.LINES);
                    GL.Color(col);
                }

                foreach(VertexSequence hole in chunk.Holes)
                {
                    for(int i=0; i<hole.Count; i++)
                    {
                        GL.Color(Color.blue);
                        GL.Vertex3(hole[i].X, hole[i].Y, 0);
                        GL.Vertex3(hole[i + 1].X, hole[i + 1].Y, 0);

                        //Render Point
                        GL.End();
                        GL.Begin(GL.QUADS);
                        GL.Color(Color.black);
                        float s = 0.05f;
                        float xx = hole[i].X;
                        float yy = hole[i].Y;
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

        if (m_groundGenerator.Ground != null &&
            m_groundGenerator.Ground.Chunks != null)
        {
            foreach (GroundChunk chunk in m_groundGenerator.Ground.Chunks)
            {
                if (chunk.Poly == null || chunk.Poly.Tris == null)
                    continue;

                var poly = chunk.Poly;

                //Set Colour
                Color col = EARTH_COL;
                if (chunk.GroundType == 1)
                    col = EARTH_COL;
                else
                    col = STONE_COL;

                List<Point> points = poly.Points;
                int[] tris = poly.Tris;

                GL.Color(col);
                for (int i = 0; i < tris.Length / 3; i++)
                {
                    Point pa = points[tris[i * 3 + 0]];
                    Point pb = points[tris[i * 3 + 1]];
                    Point pc = points[tris[i * 3 + 2]];

                    GL.Vertex3(pa.X, pa.Y, 0);
                    GL.Vertex3(pb.X, pb.Y, 0);

                    GL.Vertex3(pb.X, pb.Y, 0);
                    GL.Vertex3(pc.X, pc.Y, 0);

                    GL.Vertex3(pc.X, pc.Y, 0);
                    GL.Vertex3(pa.X, pa.Y, 0);
                }
            }
        }

        GL.End();
    }
}
