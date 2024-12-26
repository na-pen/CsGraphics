namespace CsGraphics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class ObjectList<T>
        where T : Object.Object
    {
        private List<T> items = new List<T>();

        internal void Add(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.items.Add(item);
        }

        internal bool Remove(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.items.Remove(item);
        }

        internal void Clear()
        {
            this.items.Clear();
        }

        internal T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                return this.items[index];
            }

            set
            {
                if (index < 0 || index >= this.items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }

                this.items[index] = value;
            }
        }

        // 要素数を取得
        internal int Count => this.items.Count;

        // すべての要素を取得
        internal IEnumerable<T> GetAllItems() => items;

        internal IEnumerable<U> GetItemsOfType<U>()
            where U : T
        {
            return this.items.OfType<U>();
        }
    }
}
