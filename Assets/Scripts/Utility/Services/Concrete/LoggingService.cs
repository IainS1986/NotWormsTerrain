using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terrain.Utility.Services.Concrete
{
    public class LoggingService : ILoggingService
    {
        public Action<string> OnLogEvent
        {
            get;
            set;
        }

        public void Log(string s)
        {
            if (OnLogEvent != null)
                OnLogEvent(s);
        }
    }
}
