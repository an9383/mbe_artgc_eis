using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBE.Driver.LSElectric
{
    public class WriteQueue<T> : Queue<T>
    {
        public object syncLock = new object();

        public WriteQueue(int capacity) : base(capacity) { }
        
        public WriteQueue(IEnumerable<T> collection) : base(collection) { }

        public WriteQueue() : base() { }

        public int Count
        {
            get
            {
                lock (syncLock)
                {
                    return base.Count;
                }
            }
        }

        public void Enqueue(T item)
        {
            lock (syncLock)
            {
                base.Enqueue(item);
            }
        }

        public T Dequeue()
        {
            lock (syncLock)
            {
                return base.Dequeue();
            }
        }
    }
}
