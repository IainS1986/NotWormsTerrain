using System.Collections;
using System.Collections.Generic;
using Terrain.Utility.Services;
using TinyIoC;
using UnityEngine;

namespace Terrain.Debugging
{
    public class DebugText : MonoBehaviour
    {
        private ILoggingService m_loggingService;

        private string m_logMessage;
        public string LogMessage
        {
            get { return m_logMessage; }
            set { m_logMessage = value; }
        }

        private void Awake()
        {
            m_loggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();
            m_loggingService.OnLogEvent += OnLogEvent;
        }

        private void OnDestroy()
        {
            m_loggingService.OnLogEvent -= OnLogEvent;
        }

        private void OnLogEvent(string s)
        {
            LogMessage = s;
        }

        private void OnGUI()
        {
            int h = 20;
            Rect boundary = new Rect(DebugButton.Border, Screen.height - DebugButton.Border - h, Screen.width - (DebugButton.Border * 2), h);
            GUI.Box(boundary, string.Empty);

            if(string.IsNullOrEmpty(LogMessage) == false)
            {
                Rect label = new Rect(DebugButton.Border * 2, Screen.height - DebugButton.Border - h, Screen.width - (DebugButton.Border * 2), h);
                GUI.Label(label, LogMessage);
            }
        }
    }
}
