using IoCContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugins
{
    [Serializable]
    class FirstPlugin : IPlugin
    {
        public int Test { get; set; }
        public int Identify(int[] args)
        {
            return args.Length > 0 ? args.Sum() : -1;
        }
    }
}
