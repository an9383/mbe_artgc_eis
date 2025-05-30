using System.Windows.Forms;
using KR.MBE.CommonLibrary.Utils;

namespace KR.MBE.UI.ControlUtil
{
    /// <summary>
    /// Summary description for TabPage.
    /// </summary>
    public class TabPageEx : System.Windows.Forms.TabPage
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        public TabPageEx( System.ComponentModel.IContainer container )
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            container.Add( this );
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        public TabPageEx()
        {
            ///
            /// Required for Windows.Forms Class Composition Designer support
            ///
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if( components != null )
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }



        #region Component Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();


        }
        public override string Text
        {
            get
            {
                int iTabWidthLength = 10;
                int iTextLength = ConvertUtil.GetStringLength(base.Text);
                if (iTextLength < iTabWidthLength)
                {
                    iTextLength = iTabWidthLength;
                }
                string sReturn = base.Text.PadRight(iTextLength, ' ') + "          ";
                return sReturn;
                
                //return base.Text + "               ";
            }
            set
            {
                base.Text = value;
            }
        }

        // TODO ContextMenu is no longer supported. Use ContextMenuStrip instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
        private ContextMenuStrip ctxtMenu = null;

        public // TODO ContextMenu is no longer supported. Use ContextMenuStrip instead. For more details see https://docs.microsoft.com/en-us/dotnet/core/compatibility/winforms#removed-controls
            ContextMenuStrip Menu
        {
            get
            {
                return this.ctxtMenu;
            }
            set
            {

                this.ctxtMenu = value;
            }
        }




        #endregion
    }
}
