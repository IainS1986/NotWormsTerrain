using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Terrain.Utility.Services;
using TinyIoC;
using UnityEngine;

namespace Terrain.Debugging
{
    public class DebugButton
    {
        public static int Border = 10;

        public static int Width = 150;

        public static int Height = 20;

        private static ILoggingService m_loggingService;
        private static ILoggingService Logging
        {
            get
            {
                if (m_loggingService == null)
                    m_loggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();

                return m_loggingService;
            }
        }

        private static IEnumerable<Brush> m_brushTypes;
        private static IEnumerable<Brush> BrushTypes
        {
            get
            {
                if(m_brushTypes == null)
                    m_brushTypes = Enum.GetValues(typeof(Brush)).Cast<Brush>();

                return m_brushTypes;
            }
        }

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

        public static Rect GetBrushWidgetRect()
        {
            int y = DebugButton.Border / 2;

            int num_buttons = BrushTypes.Count();
            return DebugButton.GetWidgetRect(y, num_buttons);
        }

        public static Rect GetMainWidgetRect()
        {
            Rect brushWidget = GetBrushWidgetRect();
            int y = (int)(brushWidget.y + brushWidget.height) + Border;

            return DebugButton.GetWidgetRect(y, 8);
        }
    
        public static Rect GetWidgetRect(int y, int num_buttons)
        {
            return new Rect(DebugButton.Border / 2, y, DebugButton.Width + DebugButton.Border, num_buttons * (DebugButton.Border + DebugButton.Height) + DebugButton.Height + (DebugButton.Border / 2));
        }
    }
}
