using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using KR.MBE.CommonLibrary.Handler;
using KR.MBE.Data;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class itierEnumComboBox : UserControl
    {
        //////////////////////////////////////////////////////////////////////////////////////////////////// Variable
        string m_siteID;
        string m_enumID;

        //////////////////////////////////////////////////////////////////////////////////////////////////// Property
        /// <summary>
        /// 라벨에 입력될 텍스트.
        /// </summary>
        public string SetLabel
        {
            set
            {
                LbLCmbUC.Text = value;
            }
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////// Method
        ////////////////////////////////////////////////////////////////////////////////////////// UIEvent
        public itierEnumComboBox()
        {
            InitializeComponent();
        }

        ////////////////////////////////////////////////////////////////////////////////////////// Custom
        //////////////////////////////////////////////////////////////////////////////// Public
        /// <summary>
        /// 콤보박스를 초기화합니다.
        /// </summary>
        /// <param name="siteID"> 로그인한 SiteID </param>
        /// <param name="enumID"> db에 저장된 EnumID </param>
        public void InitComboBox( string siteID, string enumID )
        {
            // 멤버변수 세팅.
            SetVariable( siteID, enumID );

            // 콤보박스 초기화.
            Hashtable htParameter = new Hashtable();
            htParameter.Add( "SITEID", m_siteID );
            htParameter.Add( "ENUMID", m_enumID );
            InitCustomComboBox( htParameter, "GetEnumValue", "00001", "ENUMVALUE", "ENUMVALUENAME" );
        }

        /// <summary>
        /// 콤보박스에서 선택한 값을 가져옵니다.
        /// </summary>
        /// <returns></returns>
        public string GetComboValue()
        {
            if( CmbUC.SelectedValue != null )
            {
                return CmbUC.SelectedValue.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        //////////////////////////////////////////////////////////////////////////////// Private
        private void SetVariable( string siteID, string enumID )
        {
            m_siteID = siteID;
            m_enumID = enumID;
        }

        private void InitCustomComboBox( Hashtable htParameter, string strQueryID, string strQueryVersion, string strValueMember, string strDisplayMemeber )
        {
            // 예외 처리.
            if( !htParameter.ContainsKey( "SITEID" ) )
            {
                //MessageBox.Show( "htParameter에 SITEID가 존재하지 않습니다." );
                MessageBox.Show("SITEID does not exist in htParameter"); // USER-501

                return;
            }

            // db에서 받아온 정보로 콤보박스 초기화.
            DataSet dsStateObjectList = MessageHandler.getCustomQuery( htParameter["SITEID"].ToString(), strQueryID, strQueryVersion, Constant.LanguageCode.LC_KOREAN, htParameter );
            dsStateObjectList.Tables[0].Rows.InsertAt( dsStateObjectList.Tables[0].NewRow(), 0 );
            CmbUC.DataSource = dsStateObjectList.Tables[0];
            CmbUC.ValueMember = strValueMember;
            CmbUC.DisplayMember = strDisplayMemeber;

            // 스타일 설정.
            panel1.BackColor = Color.Transparent;
            CmbUC.DropDownStyle = ComboBoxStyle.DropDownList;
            CmbUC.FlatStyle = FlatStyle.Popup;
        }
    }
}
