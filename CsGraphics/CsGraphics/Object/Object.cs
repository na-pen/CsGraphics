using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics.Object
{
    internal class Object
    {
        public Object() { }

        public int ID { get; set; }

        public string Name { get; set; }

        public bool IsVisible { get; set; }

        private Vertex Vertex { get; set; }
    }
}
