using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    [Browsable(true)]
    public partial class itierButton : Button
    {

        #region Variables

        public enum ActionTypeList
        { 
            None = 0 , 
            Search = 1 ,
            Save = 2,
            Insert = 3,
            Delete = 4,
            Undo = 5,
            Excel = 6,
            Print = 7
        };

        #endregion

        private ActionTypeList m_ActionType = ActionTypeList.None;

        #region PROPERTIES
        [Category("CustomData"), Description("Button Action Type")]
        public ActionTypeList ActionType
        {
            get
            {
                return m_ActionType;
            }
            set
            {
                m_ActionType = value;
                switch (value)
                {
                    case ActionTypeList.Search:
                        this.Image = imgList24.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.Blue;
                        break;
                    case ActionTypeList.Save:
                        this.Image = imgList24.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.Blue;
                        break;
                    case ActionTypeList.Insert:
                        this.Image = imgList20.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.OliveGreen;
                        break;
                    case ActionTypeList.Delete:
                        this.Image = imgList20.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.OliveGreen;
                        break;
                    case ActionTypeList.Undo:
                        this.Image = imgList20.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.OliveGreen;
                        break;
                    case ActionTypeList.Excel:
                        this.Image = imgList20.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.OliveGreen;
                        break;
                    case ActionTypeList.Print:
                        this.Image = imgList20.Images["Icon" + value.ToString() + ".png"];
                        //this.Scheme = Schemes.OliveGreen;
                        break;
                    case ActionTypeList.None:
                        this.Image = null;
                        break;
                }
                this.OnTextChanged(null);
                
            }
        }

        #endregion


        public itierButton()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }
    }
}
