using JsonServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TOS.Driver.CLT.Struct.Common;

namespace TOS.Driver.CLT.Struct.YC
{
    public partial class YCMethod
    {
        /// <summary>
        /// Send abort order result to TOS from AYC (Yard Crane) to EIS
        /// /equipment/ayc-job/abortStatus
        /// EIS -> IoT Platform -> OPUS (TOS)
        /// </summary>
        [DataContract]
        public class ResponseAbortJobList : EventArgs, IList<ResponseAbortJob>, IReturn<ResponseAbortJobResponse>
        {
            private List<ResponseAbortJob> list = new List<ResponseAbortJob>();

            public ResponseAbortJob this[int index]
            {
                get => list[index];
                set => list[index] = value;
            }

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(ResponseAbortJob item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(ResponseAbortJob item)
            {
                return list.Contains(item);
            }

            public void CopyTo(ResponseAbortJob[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<ResponseAbortJob> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(ResponseAbortJob item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, ResponseAbortJob item)
            {
                list.Insert(index, item);
            }

            public bool Remove(ResponseAbortJob item)
            {
                return list.Remove(item);
            }

            public void RemoveAt(int index)
            {
                list.RemoveAt(index);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return list.GetEnumerator();
            }
        }
    }
}
