using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil
{
    public partial class frmWaitDialog : Form
    {
        public frmWaitDialog()
        {
            InitializeComponent();
        }

        public void setMessage( string sMessage )
        {
            lblMessage.Text = sMessage;
        }

    }
}
