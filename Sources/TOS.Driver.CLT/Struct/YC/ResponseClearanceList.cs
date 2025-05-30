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
        /// If the vehicle is in the right position(CPS 00) under the AYC (Yard Crane) from TOS to EIS
        /// /equipment/ayc-job/sendClearance
        /// OPUS (TOS) -> IoT Platform -> EIS
        /// </summary>
        public class ResponseClearanceList : EventArgs, IList<ResponseClearance>, IReturn<ResponseClearanceResponse>
        {
            private List<ResponseClearance> list = new List<ResponseClearance>();

            public ResponseClearance this[int index]
            {
                get => list[index];
                set => list[index] = value;
            }

            public int Count => list.Count;

            public bool IsReadOnly => false;

            public void Add(ResponseClearance item)
            {
                list.Add(item);
            }

            public void Clear()
            {
                list.Clear();
            }

            public bool Contains(ResponseClearance item)
            {
                return list.Contains(item);
            }

            public void CopyTo(ResponseClearance[] array, int arrayIndex)
            {
                list.CopyTo(array, arrayIndex);
            }

            public IEnumerator<ResponseClearance> GetEnumerator()
            {
                return list.GetEnumerator();
            }

            public int IndexOf(ResponseClearance item)
            {
                return list.IndexOf(item);
            }

            public void Insert(int index, ResponseClearance item)
            {
                list.Insert(index, item);
            }

            public bool Remove(ResponseClearance item)
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
