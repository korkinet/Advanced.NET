using IoCContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Plugins
{
    [Serializable]
    class FirstPlugin : IPlugin
    {
        Timer timer;
        long cnt = 1;
        List<long> arr = new List<long>();

        public FirstPlugin()
        {
            timer = new Timer(200);
            timer.Elapsed += DoSomeThing;
            timer.Start();
        }

        public int Identify(int[] args)
        {
            return args.Length > 0 ? args.Sum() : -1;
        }

        public void DoSomeThing(object sender, ElapsedEventArgs e)
        {
            for (long i = 0; i < cnt; i++)
            {
                arr.Add(i);
            }
            cnt *= 2;
        }
    }
}
