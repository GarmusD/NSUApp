using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;

namespace NSUService
{
    public sealed class NSUSocketTask : IBackgroundTask
    {
        BackgroundTaskDeferral taskDeferral;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            taskDeferral = taskInstance.GetDeferral();

            
        }

        private async void Parse(byte[] buff)
        {

            taskDeferral.Complete();
        }
    }
}
