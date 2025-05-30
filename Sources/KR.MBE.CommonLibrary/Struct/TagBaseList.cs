using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KR.MBE.CommonLibrary.Interface;

namespace KR.MBE.CommonLibrary.Struct
{
    public class TagBaseList : List<CTagBase>
    {
        public TagBaseList()
        {
        }

        public new void Add(CTagBase newItem)
        {
            base.Add(newItem);
        }

        public new void Remove(CTagBase tobeRemoved)
        {
            base.Remove(tobeRemoved);
        }

        public CTagBase FindByTagId(string tagId)
        {
            foreach (var item in this)
            {
                if (item.TAGID == tagId)
                {
                    return item;
                }
            }

            return null;
        }

        public CTagBase FindByDescription(string description)
        {
            foreach (var item in this)
            {
                if (item.DESCRIPTION == description)
                {
                    return item;
                }
            }

            return null;
        }
    }
}
