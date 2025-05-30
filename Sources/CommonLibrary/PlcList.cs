using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLibrary
{
    public class PlcList<T> : List<T>
    {
        public new void Add(T newItem)
        {
            base.Add(newItem);
        }

        public new void Remove(T tobeRemoved)
        {
            base.Remove(tobeRemoved);
        }

        public T FindByEquipmentId(string equipmentId)
        {
            foreach (T item in this)
            {
                if (item.GetType().GetProperty("EQUIPMENTID").GetValue(item).ToString() == equipmentId)
                {
                    return item;
                }
            }

            return default(T);
        }
    }
}
