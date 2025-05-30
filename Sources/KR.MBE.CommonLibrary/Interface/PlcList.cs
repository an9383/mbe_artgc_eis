using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KR.MBE.CommonLibrary.Interface
{
    /// <summary>
    /// PLC Interface 추상 리스트
    /// </summary>
    /// <typeparam name="PlcInterface">이기종 추상 PLC Interface</typeparam>
    public class PlcList<PlcInterface> : List<PlcInterface>
    {
        /// <summary>
        /// List 추가
        /// </summary>
        /// <param name="newItem">추가할 구조체</param>
        public new void Add(PlcInterface newItem)
        {
            base.Add(newItem);
        }

        /// <summary>
        /// List에서 삭제
        /// </summary>
        /// <param name="tobeRemoved">삭제할 구조체</param>
        public new void Remove(PlcInterface tobeRemoved)
        {
            base.Remove(tobeRemoved);
        }

        /// <summary>
        /// List에서 크래인 EquipmentId로 검색
        /// </summary>
        /// <param name="equipmentId">크레인 고유 ID 값</param>
        /// <returns>찾은 구조체 또는 null</returns>
        public PlcInterface FindByEquipmentId(string equipmentId)
        {
            foreach (PlcInterface item in this)
            {
                if (item.GetType().GetProperty("EQUIPMENTID").GetValue(item).ToString() == equipmentId)
                {
                    return item;
                }
            }

            return default(PlcInterface);
        }
    }
}
