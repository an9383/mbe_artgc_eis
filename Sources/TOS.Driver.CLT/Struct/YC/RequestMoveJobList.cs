using JsonServices;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOS.Driver.CLT.Struct.Common;

namespace TOS.Driver.CLT.Struct.YC
{
    public partial class YCMethod
    {
        /// <summary>
        /// Send move order to AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendMoveJob
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        public class RequestMoveJobList : EventArgs, IList<RequestMoveJob>, IReturn<RequestMoveJobResponse>
        {
            private List<RequestMoveJob> list = new List<RequestMoveJob>();

            public RequestMoveJob this[int index]
            {
                get => list[index];
                set => list[index] = value;
            }

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(RequestMoveJob item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(RequestMoveJob item)
            {
                return list.Contains(item);
            }

            public void CopyTo(RequestMoveJob[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<RequestMoveJob> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(RequestMoveJob item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, RequestMoveJob item)
            {
                list.Insert(index, item);
            }

            public bool Remove(RequestMoveJob item)
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
