using System;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Struct;

namespace KR.MBE.UI.ControlUtil
{

    public partial class ErrorMessageBox : Form
    {
        ReturnData retData = new ReturnData();

        const int g_iShortHeight = 185;
        const int g_iDetailHeight = 400;

        public ErrorMessageBox( ReturnData returnData )
        {
            InitializeComponent();

            retData = returnData;
        }


        private void btnClose_Click( object sender, EventArgs e )
        {
            this.Close();
        }

        private void ErrorMessageBox_Load( object sender, EventArgs e )
        {
            this.Height = g_iShortHeight;

            lblErrorCode.Text = "[ " + retData.returncode + " ] Error";
            txtErrorMessage.Text = retData.returnmessage;
            txtErrorDetailMessage.Text = retData.returndetailmessage;

        }

        private void btnDetail_Click( object sender, EventArgs e )
        {
            if( btnDetail.Text == "자세히보기" )
            {
                this.Height = g_iDetailHeight;
                btnDetail.Text = "감추기";
            }
            else
            {
                this.Height = g_iShortHeight;
                btnDetail.Text = "자세히보기";
            }
        }

    }
}
