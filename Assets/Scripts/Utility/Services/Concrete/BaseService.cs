using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyIoC;

namespace Terrain.Utility.Services.Concrete
{
    abstract class BaseService
    {
        private ILoggingService m_loggingService;
        protected ILoggingService Logging
        {
            get { return m_loggingService; }
            private set { m_loggingService = value; }
        }

        public BaseService()
        {
            Logging = TinyIoCContainer.Current.Resolve<ILoggingService>();
        }
    }
}
