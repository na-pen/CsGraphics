using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsGraphics
{
    internal class ObjectList<T>
        where T : Object.Object
    {
            private List<T> items = new List<T>();

            public void Add(T item)
            {
                items.Add(item);
            }

            public T this[int index] => items[index];

            public int Count => items.Count;
        }
    }
}
