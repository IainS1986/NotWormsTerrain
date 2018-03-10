using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using TinyIoC;
using Terrain.Utility.Services;

namespace Terrain.Debugging
{
    public enum Brush
    {
        NONE = -1,
        AIR = 0,
        EARTH = 1,
        STONE = 2,
    };

    public class DebugBrush : MonoBehaviour
    {
        private ILoggingService m_logging;

        public Brush m_brush = Brush.NONE;

        [SerializeField]
        private int m_size = 2;

        [SerializeField]
        private DebugMain m_main;

        private Material m_lineMaterial;

        private IEnumerable<Brush> m_brushTypes;

        public void Start()
        {
            m_lineMaterial = new Material(Shader.Find("Particles/Alpha Blended"));
            m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

            m_brushTypes = Enum.GetValues(typeof(Brush)).Cast<Brush>();
            m_logging = TinyIoCContainer.Current.Resolve<ILoggingService>();
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
                if (change)
                    m_logging.Log(string.Format("ReBuild took {0} ms", tspan.TotalMilliseconds));
            }
        }

        void OnGUI()
        {
            Rect boundary = DebugButton.GetBrushWidgetRect();
            GUI.Box(boundary, "BRUSH");

            int i = 1;
            foreach(var brush in m_brushTypes)
            {
                DebugButton.AddButton(boundary, i++, (m_brush == brush) ? brush.ToString().ToUpper() : brush.ToString().ToLower(), () => m_brush = brush);
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
                GL.Vertex3(xx - s, yy - s, 0);
                GL.Vertex3(xx + s, yy - s, 0);
                GL.Vertex3(xx + s, yy + s, 0);
                GL.Vertex3(xx - s, yy + s, 0);
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
}
