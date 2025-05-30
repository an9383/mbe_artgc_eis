using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonServices;
using JsonServices.Messages;
using TOS.Driver.CLT.Struct.Common;

namespace TOS.Driver.CLT.Struct.YC
{
    public partial class YCMethod
    {
        /// <summary>
        /// Send work order to AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendAycJob
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        public class RequestReceiveJobList : EventArgs, IList<RequestReceiveJob>, IReturn<RequestReceiveJobResponse>
        {
            private List<RequestReceiveJob> list = new List<RequestReceiveJob>();

            public RequestReceiveJob this[int index]
            {
                get => list[index];
                set => list[index] = value;
            }

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(RequestReceiveJob item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(RequestReceiveJob item)
            { 
                return list.Contains(item);
            }

            public void CopyTo(RequestReceiveJob[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<RequestReceiveJob> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(RequestReceiveJob item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, RequestReceiveJob item)
            {
                list.Insert(index, item);
            }

            public bool Remove(RequestReceiveJob item)
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
