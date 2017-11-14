using UnityEngine;
using System.Collections;
using System;

public class DebugBrush : MonoBehaviour {

    public enum Brush
    {
        NONE = -1,
        AIR = 0,
        EARTH = 1,
        STONE = 2,
    };


    public Brush m_brush = Brush.NONE;

    [SerializeField]
    private int m_size = 2;

    [SerializeField]
    private Main m_main;

    private Material m_lineMaterial;

    public void Start()
    {
        m_lineMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
        m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
    }

    void Update()
    {
        if (m_brush != Brush.NONE && Input.GetMouseButton(0))
        {
            Vector2 p = MousePositionInWorld();
            int xx = UnityEngine.Mathf.FloorToInt(p.x);
            int yy = UnityEngine.Mathf.FloorToInt(p.y);
            DateTime now = DateTime.Now;
            bool change = m_main.TerrainService.GroundChangeSelectiveRebuild(xx, yy, m_size, (int)m_brush);
            TimeSpan tspan = DateTime.Now.Subtract(now);
            if(change)
                Debug.Log(string.Format("ReBuild took {0} ms", tspan.TotalMilliseconds));
        }
    }

    void OnGUI()
    {
        int b = 10;
        int w = 120;
        int h = 20;

        int num_buttons = 8;
        Rect boundary = new Rect(b / 2, b / 2, w + b, num_buttons * (b + h) + h);
        GUI.Box(boundary, "BRUSH");

        Rect r = new Rect(b, b * 1 + h * 1, w, h);
        if (GUI.Button(r, (m_brush == Brush.NONE) ? "NONE" : "none"))
        {
            m_brush = Brush.NONE;
        }

        r = new Rect(b, b * 2 + h * 2, w, h);
        if (GUI.Button(r, (m_brush == Brush.AIR) ? "AIR" : "air"))
        {
            m_brush = Brush.AIR;
        }

        r = new Rect(b, b * 3 + h * 3, w, h);
        if (GUI.Button(r, (m_brush == Brush.EARTH) ? "EARTH" : "earth"))
        {
            m_brush = Brush.EARTH;
        }

        r = new Rect(b, b * 4 + h * 4, w, h);
        if (GUI.Button(r, (m_brush == Brush.STONE) ? "STONE" : "stone"))
        {
            m_brush = Brush.STONE;
        }
    }

    public void OnPostRender()
    {
        // set the current material
        m_lineMaterial.SetPass(0);

        RenderBrush();
    }

    private void RenderBrush()
    {
        if (m_brush != Brush.NONE)
        {
            Color col = Color.black;
            if (m_brush == Brush.EARTH) col = m_main.EARTH_COL;
            if (m_brush == Brush.STONE) col = m_main.STONE_COL;
            col.a = 0.7f;

            Vector2 pos = MousePositionInWorld();

            //Render brush
            GL.Begin(GL.QUADS);
            GL.Color(col);
            int xx = UnityEngine.Mathf.FloorToInt(pos.x);
            int yy = UnityEngine.Mathf.FloorToInt(pos.y);
            float s = m_size;
            GL.Vertex3(xx, yy, 0);
            GL.Vertex3(xx + s, yy, 0);
            GL.Vertex3(xx + s, yy + s, 0);
            GL.Vertex3(xx, yy + s, 0);
            GL.End();
        }
    }

    private Vector2 MousePositionInWorld()
    {
        Vector2 p = new Vector2(-1, -1);
        Vector3 mousepos = Input.mousePosition;
        mousepos.z = -Camera.main.gameObject.transform.position.z;
        Vector3 worldpos = Camera.main.ScreenToWorldPoint(mousepos);
        return worldpos;
    }
}
