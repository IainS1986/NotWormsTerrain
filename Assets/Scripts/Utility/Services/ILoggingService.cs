using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terrain.Utility.Services
{
    interface ILoggingService
    {
        Action<string> OnLogEvent { get; set; }

        void Log(string s);
    }
}
