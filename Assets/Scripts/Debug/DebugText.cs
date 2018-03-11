using System.Collections;
using System.Collections.Generic;
using Terrain.Utility.Services;
using TinyIoC;
using UnityEngine;

namespace Terrain.Debugging
{
    /// <summary>
    /// Helper class to interact with the ILoggingService as an alternative to the 
    /// Unity Debug.Log console as that is painfully slow and actually slowing down
    /// the terrain algorithm
    /// </summary>
    public class DebugText : MonoBehaviour
    {
        /// <summary>
        /// Logging service used to display log data to the screen instead
        /// of Unity's console window as thats painfully slow. Use this
        /// instead of Debug.Log
        /// </summary>
        private ILoggingService m_loggingService;

        /// <summary>
        /// The current string to display in the DebugText
        /// </summary>
        private string m_logMessage;

        /// <summary>
        /// The current string to display in the DebugText
        /// </summary>
        public string LogMessage
        {
            get { return m_logMessage; }
            set { m_logMessage = value; }
        }

        /// <summary>
        /// Unity function called when the gameobject instantiates
        /// </summary>
        private void Awake()
        {
            m_loggingService = TinyIoCContainer.Current.Resolve<ILoggingService>();
            m_loggingService.OnLogEvent += OnLogEvent;
        }

        /// <summary>
        /// Unity function called when the gameobject is destroyed
        /// </summary>
        private void OnDestroy()
        {
            m_loggingService.OnLogEvent -= OnLogEvent;
        }

        /// <summary>
        /// Callback triggered when a string is logged in ILoggingService
        /// </summary>
        /// <param name="s"></param>
        private void OnLogEvent(string s)
        {
            LogMessage = s;
        }

        /// <summary>
        /// Unity OnGUI function called every frame and renders the current log message
        /// to a text field at the bottom of the screen.
        /// </summary>
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
