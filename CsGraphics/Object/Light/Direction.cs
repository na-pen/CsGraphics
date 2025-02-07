using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics.Object.Light
{
    internal class Direction : Object
    {
        internal Direction(string name, int id = -1)
            : base(name)
        {
        }

        internal float Brightness = 1.0f;
    }
}
