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
        /// Send abort order to AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendAbortJob
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        [DataContract]
        public class RequestAbortJobList : EventArgs, IList<RequestAbortJob>, IReturn<RequestAbortJobResponse>
        {
            private List<RequestAbortJob> list = new List<RequestAbortJob>();

            public RequestAbortJob this[int index]
            {
                get => list[index];
                set => list[index] = value;
            }

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(RequestAbortJob item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(RequestAbortJob item)
            {
                return list.Contains(item);
            }

            public void CopyTo(RequestAbortJob[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<RequestAbortJob> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(RequestAbortJob item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, RequestAbortJob item)
            {
                list.Insert(index, item);
            }

            public bool Remove(RequestAbortJob item)
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
