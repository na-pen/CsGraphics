using System.Runtime.CompilerServices;

namespace CsGraphics
{
    public class ObjectManager
    {
        private Object.Object[] objectList = [];
        private ushort idMax = 0;
        private Dictionary<int,int> dictIdIndex = new ();
        private Dictionary<string,int> dictNameId = new ();

        internal int Add(Object.Object @object)
        {
            idMax++;
            Array.Resize(ref objectList, objectList.Length + 1);
            @object.ID = idMax;
            objectList[objectList.Length - 1] = @object;

            this.dictIdIndex.Add(idMax, objectList.Length-1);
            this.dictNameId.Add(@object.Name, idMax);

            return idMax;
        }

        internal bool Remove(int id)
        {
            int index = this.dictIdIndex[id];
            Object.Object @object = this.objectList[index];

            dictIdIndex.Remove(id);
            dictNameId.Remove(@object.Name);

            this.objectList = this.objectList.Where((_, i) => i != index).ToArray();

            foreach (var key in this.dictIdIndex.Keys)
            {
                if (this.dictIdIndex[key] > index)
                {
                    this.dictIdIndex[key] -= 1;
                }
            }

            return true;
        }

        internal bool Remove(string name)
        {
            return Remove(dictNameId[name]);
        }

        internal bool RemoveAll()
        {
            this.objectList = [];
            this.dictIdIndex = new ();
            this.dictNameId = new ();

            return true;
        }

        public Object.Object Get(int id)
        {
            return this.objectList[this.dictIdIndex[id]];
        }

        public Object.Object Get(string name)
        {
            return this.Get(this.dictNameId[name]);
        }

        public T Get<T>(int id) where T : Object.Object
        {
            return (T)objectList[this.dictIdIndex[id]];
        }
        public T Get<T>(string name) where T : Object.Object
        {
            return (T)objectList[this.dictIdIndex[this.dictNameId[name]]];
        }

        internal Object.Object this[int index]
        {
            get { return this.objectList[index]; }
        }

        internal IEnumerable<T> GetObjectsOfType<T>()
            where T : Object.Object
        {
            return this.objectList.OfType<T>();
        }

        internal int Count()
        {
            return this.objectList.Length;
        }
    }
}
