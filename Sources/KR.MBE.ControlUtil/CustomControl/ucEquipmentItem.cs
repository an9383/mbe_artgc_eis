using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Utils;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class ucEquipmentItem : UserControl
    {
        #region var/const
        string mEquipmentID, mEquipmentName, mColor;
        #endregion

        #region ctor/getter/setter
        public ucEquipmentItem()
        {
            InitializeComponent();
        }

        public ucEquipmentItem( string equipmentID, string equipmentName, string color )
        {
            InitializeComponent();
            mEquipmentID = equipmentID;
            mEquipmentName = equipmentName;
            mColor = color;
        }

        public string getEquipmentID()
        {
            return mEquipmentID;
        }

        public string getColor()
        {
            return mColor;
        }
        #endregion

        #region event
        private void ucEquipmentItem_Load( object sender, EventArgs e )
        {
            lblEquipment.Text = $"{mEquipmentName}({mEquipmentID})";
            lblColor.BackColor = ConvertUtil.StringToColor( mColor );
        }
        #endregion

        #region func
        private void ChangeColor( Color color )
        {
            mColor = ConvertUtil.colorToString( color );
            lblColor.BackColor = color;
        }

        private void ChangeColor( string strColor )
        {
            mColor = strColor;
            lblColor.BackColor = ConvertUtil.StringToColor( strColor );
        }
        #endregion
    }
}
