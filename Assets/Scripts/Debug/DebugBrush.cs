using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;
using TinyIoC;
using Terrain.Utility.Services;
using Terrain.Services;

namespace Terrain.Debugging
{
    /// <summary>
    /// Brush types for painting terrain
    /// </summary>
    public enum Brush
    {
        NONE = -1,
        AIR = 0,
        EARTH = 1,
        STONE = 2,
    };

    /// <summary>
    /// Class to encapsulate the GUI element to select
    /// the paint settings the user can use to paint terrain
    /// Also handles triggering the Ground terrain alteration
    /// and regeneration
    /// </summary>
    public class DebugBrush : MonoBehaviour
    {
        /// <summary>
        /// Logging service used to display log data to the screen instead
        /// of Unity's console window as thats painfully slow. Use this
        /// instead of Debug.Log
        /// </summary>
        private ILoggingService m_logging;

        /// <summary>
        /// The TerrainService that stores the ground data that the brush
        /// interacts with on user input.
        /// </summary>
        private ITerrainService m_terrainService;

        /// <summary>
        /// The current select brush in use when painting with the LMB
        /// </summary>
        public Brush m_brush = Brush.NONE;

        /// <summary>
        /// The current brush size used when painting with the LMB
        /// </summary>
        [SerializeField]
        private int m_size = 2;

        /// <summary>
        /// The brush colour for the "earth" material
        /// </summary>
        [SerializeField]
        public Color m_earthBrushColour = new UnityEngine.Color(1, 0, 0, 1f);

        /// <summary>
        /// The brush colour for the "stone" material
        /// </summary>
        [SerializeField]
        public Color m_stoneBrushColour = new UnityEngine.Color(0, 1, 0, 1f);

        /// <summary>
        /// Line renderer material for GL debug rendering
        /// </summary>
        private Material m_lineMaterial;

        /// <summary>
        /// Cached list of all enum values for Brush to allow iteration
        /// through them
        /// </summary>
        private IEnumerable<Brush> m_brushTypes;

        /// <summary>
        /// Unity function called after instantiating the gameobject
        /// </summary>
        public void Start()
        {
            m_lineMaterial = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended"));
            m_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            m_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;

            m_brushTypes = Enum.GetValues(typeof(Brush)).Cast<Brush>();
            m_logging = TinyIoCContainer.Current.Resolve<ILoggingService>();
            m_terrainService = TinyIoCContainer.Current.Resolve<ITerrainService>();
        }

        /// <summary>
        /// Unity function called at every update loop
        /// </summary>
        void Update()
        {
            if (m_brush != Brush.NONE && Input.GetMouseButton(0))
            {
                Vector2 p = MousePositionInWorld();
                int xx = UnityEngine.Mathf.FloorToInt(p.x);
                int yy = UnityEngine.Mathf.FloorToInt(p.y);
                DateTime now = DateTime.Now;
                bool change = m_terrainService.GroundChangeSelectiveRebuild(xx, yy, m_size, (int)m_brush);
                TimeSpan tspan = DateTime.Now.Subtract(now);
                if (change)
                    m_logging.Log(string.Format("ReBuild took {0} ms", tspan.TotalMilliseconds));
            }
        }


        /// <summary>
        /// Unity function called every update for OnGUI rendering
        /// </summary>
        void OnGUI()
        {
            Rect boundary = DebugButton.GetBrushWidgetRect();
            GUI.Box(boundary, "BRUSH");

            //Render +/- size UI
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.white;

            GUI.Label(new Rect(boundary.x, boundary.y + DebugButton.Height + DebugButton.Border, boundary.width, DebugButton.Height), "Size", style);
            if(GUI.Button(new Rect(boundary.x + DebugButton.Border, boundary.y + DebugButton.Height + DebugButton.Border, DebugButton.Height, DebugButton.Height), "-"))
            {
                if (m_brush != Brush.NONE &&
                    m_size > 1)
                    m_size--;
            }

            if (GUI.Button(new Rect(boundary.x + boundary.width - DebugButton.Border - DebugButton.Height, boundary.y + DebugButton.Height + DebugButton.Border, DebugButton.Height, DebugButton.Height), "+"))
            {
                if (m_brush != Brush.NONE)
                    m_size++;
            }


            //Render brush material buttons after +/- size buttons
            int i = 2;
            foreach(var brush in m_brushTypes)
            {
                DebugButton.AddButton(boundary, i++, (m_brush == brush) ? brush.ToString().ToUpper() : brush.ToString().ToLower(), () => m_brush = brush);
            }
        }

        /// <summary>
        /// Unity function called after ever frame rendered.
        /// </summary>
        public void OnPostRender()
        {
            // set the current material
            if (m_lineMaterial != null)
            {
                m_lineMaterial.SetPass(0);
            }

            RenderBrush();
        }

        /// <summary>
        /// Renders a square brush object using simple GL quad
        /// at the mouse position. Colour and size is based off
        /// the current brush type selected and brush size.
        /// </summary>
        private void RenderBrush()
        {
            if (m_brush != Brush.NONE)
            {
                Color col = Color.black;
                if (m_brush == Brush.EARTH) col = m_earthBrushColour;
                if (m_brush == Brush.STONE) col = m_stoneBrushColour;
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

        /// <summary>
        /// Gets the Vector2 world position (in relation to the terrain)
        /// from the screen position of the mouse.
        /// </summary>
        /// <returns></returns>
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
