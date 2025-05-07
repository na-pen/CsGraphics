using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics.Object.Light
{
    internal class Point : Object
    {
        internal Point(string name, int id = -1)
            : base(name)
        {
        }

        internal float Brightness = 1.0f;
    }
}
