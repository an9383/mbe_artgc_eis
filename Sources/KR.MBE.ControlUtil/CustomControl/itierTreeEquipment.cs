using KR.MBE.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Handler;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class itierTreeEquipment : TreeViewBound
    {
        private String m_sSiteID = String.Empty;
        private String m_sSiteName = String.Empty;
        private DataTable m_dtData = null;

        public itierTreeEquipment()
        {
            InitializeComponent();

        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void getEquipmentTreeData(string siteID, string siteName)
        {
            getEquipmentTreeData(siteID, siteName, "");
        }
        public void getEquipmentTreeData(string siteID, string siteName, string equipmenttype)
        {

            m_sSiteID = siteID;
            m_sSiteName = siteName;

            try
            {
                Hashtable htParameter = new Hashtable();
                htParameter.Add("SITEID", m_sSiteID);
                htParameter.Add("EQUIPMENTTYPE", equipmenttype);
                DataSet dsResult = MessageHandler.getCustomQuery(m_sSiteID, "GetEquipmentDefinition", "00001", Constant.LanguageCode.LC_KOREAN, htParameter);
                // TreeView Bind
                m_dtData = dsResult.Tables["_REPLYDATA"].Copy();
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.ToString());
            }

            if (m_dtData.Rows.Count == 0)
            {
                this.Clear();
                TreeNodeBound treeNode = new TreeNodeBound(m_sSiteName);
                treeNode.Value = m_sSiteID;
                this.Nodes.Add(treeNode);
            }
            else
            {
                DataRow drSite = m_dtData.NewRow();
                drSite["SITEID"] = m_sSiteID;
                drSite["EQUIPMENTID"] = m_sSiteID;
                drSite["PARENTEQUIPMENTID"] = "";
                drSite["EQUIPMENTNAME"] = m_sSiteName;
                drSite["EQUIPMENTTYPE"] = "Site";
                m_dtData.Rows.InsertAt(drSite, 0);
                m_dtData.AcceptChanges();

                this.initTreeView(m_dtData, "PARENTEQUIPMENTID", "EQUIPMENTID", "EQUIPMENTNAME", "EQUIPMENTID", "EQUIPMENTTYPE");
            }

            treeViewSetImage(this.Nodes[0]);
            this.ExpandAll();
        }



        private void treeViewSetImage(TreeNode oNode)
        {
            // 이미지
            this.ImageList = imageListEquipment;
            TreeNodeBound treeNode = ((TreeNodeBound)(oNode));

            oNode.ImageKey = "Icon" + treeNode.LevelValue + ".png";
            oNode.SelectedImageKey = "Icon" + treeNode.LevelValue + ".png";

            if (oNode.Nodes.Count > 0)
            {
                foreach (TreeNode oChildNode in oNode.Nodes)
                {
                    treeViewSetImage(oChildNode);
                }
            }


        }



    }
}
