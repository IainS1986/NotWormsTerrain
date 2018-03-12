using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terrain.Utility.Services;
using TinyIoC;
using UnityEngine;

namespace Terrain.Debugging
{
    /// <summary>
    /// Helper class used to easily render OnGUI Buttons
    /// to the screen.
    /// </summary>
    public class DebugButton
    {
        /// <summary>
        /// Global GUI border size
        /// </summary>
        public static int Border = 10;

        /// <summary>
        /// Global GUI Button width
        /// </summary>
        public static int Width = 150;

        /// <summary>
        /// Global GUI Button height
        /// </summary>
        public static int Height = 20;

        /// <summary>
        /// Logging service used to display log data to the screen instead
        /// of Unity's console window as thats painfully slow. Use this
        /// instead of Debug.Log
        /// </summary>
        private static ILoggingService m_loggingService;

        /// <summary>
        /// Logging service used to display log data to the screen instead
        /// of Unity's console window as thats painfully slow. Use this
        /// instead of Debug.Log
        /// </summary>
        private static ILoggingService Logging
        {
            get
            {
                if (m_loggingService == null)
                    m_loggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();

                return m_loggingService;
            }
        }

        /// <summary>
        /// Cached list of all enum values for Brush to allow iteration
        /// through them
        /// </summary>
        private static IEnumerable<Brush> m_brushTypes;

        /// <summary>
        /// Cached list of all enum values for Brush to allow iteration
        /// through them
        /// </summary>
        private static IEnumerable<Brush> BrushTypes
        {
            get
            {
                if(m_brushTypes == null)
                    m_brushTypes = Enum.GetValues(typeof(Brush)).Cast<Brush>();

                return m_brushTypes;
            }
        }


        /// <summary>
        /// Renders a Button in an OnGUI and triggers the Action passed in
        /// when pressed.
        /// </summary>
        /// <param name="boundary">The boundary the button is inside</param>
        /// <param name="i">The index of the button within the list inside the boundary</param>
        /// <param name="s">The string displayed on the Button</param>
        /// <param name="action">The action triggered when the button is pressed</param>
        public static void AddButton(Rect boundary, int i, string s, Action action)
        {
            Rect r = new Rect(Border, boundary.y + (Border * i + Height * i), Width, Height);
            if (GUI.Button(r, s))
            {
                DateTime before = DateTime.Now;
                if (action != null)
                    action();
                DateTime after = DateTime.Now;

                if (string.IsNullOrEmpty(s) == false)
                    Logging.Log(string.Format("{0} took {1}ms", s, (after - before).TotalMilliseconds));
            }
        }

        /// <summary>
        /// Gets the rect on the screen for the Brush Widget
        /// </summary>
        /// <returns>A Rect in screen space for the OnGUI Brush Widget</returns>
        public static Rect GetBrushWidgetRect()
        {
            int y = DebugButton.Border / 2;

            int num_buttons = BrushTypes.Count() + 1;
            return DebugButton.GetWidgetRect(y, num_buttons);
        }

        /// <summary>
        /// Gets the rect on the screen for the main terrain Widget
        /// </summary>
        /// <returns>A Rect in screen space for the OnGUI terrain widget</returns>
        public static Rect GetMainWidgetRect()
        {
            Rect brushWidget = GetBrushWidgetRect();
            int y = (int)(brushWidget.y + brushWidget.height) + Border;

            return DebugButton.GetWidgetRect(y, 8);
        }
    
        /// <summary>
        /// Gets a rect for a widget at a defined Y position on the screen and a 
        /// set number of buttons tall
        /// </summary>
        /// <param name="y">The screen Y position the OnGUI widget is at</param>
        /// <param name="num_buttons">The number of buttons in the widget</param>
        /// <returns></returns>
        public static Rect GetWidgetRect(int y, int num_buttons)
        {
            return new Rect(DebugButton.Border / 2, y, DebugButton.Width + DebugButton.Border, num_buttons * (DebugButton.Border + DebugButton.Height) + DebugButton.Height + (DebugButton.Border / 2));
        }
    }
}
