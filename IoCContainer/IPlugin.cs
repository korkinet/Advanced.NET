using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainer
{
    public interface IPlugin
    {
        int Identify(int[] args);
    }
}
