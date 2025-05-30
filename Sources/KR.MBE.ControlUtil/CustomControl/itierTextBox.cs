using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public enum Mask { None, Date, DateTime, Integer, Decimal };
    public partial class itierTextBox : TextBox
    {

        private Int64 m_maxvalue = Int64.MaxValue;
        private int m_decimalpoint = 2;
        private Mask m_mask = Mask.None;

        private string sTagID = string.Empty;
        [Category("CUSTOM"), Description("Custom")]
        public string TAGID
        {
            get { return sTagID; }
            set { sTagID = value; }
        }

        // 2020.04.20 강성묵 추가.
        public string Value
        {
            get
            {
                return this.Text.Replace( ",", "" );
            }
        }

        public itierTextBox()
        {
            this.MaxLength = 18;
            this.TextChanged += new EventHandler( ItierTextBox_TextChanged );
            this.KeyPress += new KeyPressEventHandler( ItierTextBox_KeyPress );
            //this.KeyDown += new KeyEventHandler( ItierTextBox_KeyDown );
        }

        private void TypingOnlyNumber( object sender, KeyPressEventArgs e, bool includePoint, bool includeMinus )
        {
            bool isValidInput = false;
            if( !char.IsControl( e.KeyChar ) && !char.IsDigit( e.KeyChar ) )
            {
                if( includePoint == true )
                {
                    if( e.KeyChar == '.' )
                        isValidInput = true;
                }
                if( includeMinus == true )
                {
                    if( e.KeyChar == '-' )
                        isValidInput = true;
                }

                if( isValidInput == false )
                    e.Handled = true;
            }

            if( includePoint == true )
            {
                int iCurrentDecimalPoint = 0;
                if( this.Text.IndexOf( "." ) > 0 )
                {
                    iCurrentDecimalPoint = this.Text.Split( '.' )[1].Length;
                }

                if( ( ( e.KeyChar == '.' ) && ( this.DecimalPoint == 0 ) ) || ( iCurrentDecimalPoint > this.DecimalPoint ) )
                {
                    e.Handled = true;
                }

                if( e.KeyChar == '.' && ( string.IsNullOrEmpty( ( sender as TextBox ).Text.Trim() ) || ( sender as TextBox ).Text.IndexOf( '.' ) > -1 ) )
                {
                    e.Handled = true;
                }

            }
            if( includeMinus == true )
            {
                if( e.KeyChar == '-' && ( !string.IsNullOrEmpty( ( sender as TextBox ).Text.Trim() ) || ( sender as TextBox ).Text.IndexOf( '-' ) > -1 ) )
                    e.Handled = true;
            }
        }
        #region Event

        private void ItierTextBox_TextChanged( object sender, EventArgs e )
        {
            if( this.Text == "-" )
                return;
            if( this.Text.Length > 0 )
            {
                switch( MaskType )
                {
                    case Mask.Integer:
                        this.Text = String.Format( "{0:#,##0}", Convert.ToDecimal( this.Text.Replace( ",", "" ) ) );
                        break;
                    case Mask.Decimal:
                        MakeDecimal();
                        break;
                    default:
                        break;
                }
                this.SelectionStart = this.TextLength; //** 캐럿을 맨 뒤로 보낸다...
                this.SelectionLength = 0;
            }
        }

        private void MakeDecimal()
        {
            if( this.Text.Length > 0 )
            {
                if( this.Text.Substring( this.Text.Length - 1 ) != "." )
                {
                    if( this.Text.IndexOf( "." ) > 0 )
                    {
                        String sDecimalPointFormat = String.Empty;
                        String sDP = String.Empty;
                        int iCurrentDecimalPoint = this.Text.Split( '.' )[1].Length;
                        if( iCurrentDecimalPoint > 0 )
                        {
                            if( iCurrentDecimalPoint > this.DecimalPoint )
                            {
                                this.Text = this.Text.Substring( 0, this.Text.IndexOf( "." ) + 1 + this.DecimalPoint );
                                for( int i = 0; i < this.DecimalPoint; i++ )
                                {
                                    sDP += "0";
                                }
                            }
                            else
                            {
                                for( int i = 0; i < iCurrentDecimalPoint; i++ )
                                {
                                    sDP += "0";
                                }
                            }
                        }
                        if( sDP.Length > 0 )
                        {
                            sDecimalPointFormat = "{0:#,##0." + sDP + "}";
                            this.Text = String.Format( sDecimalPointFormat, Convert.ToDouble( this.Text.Replace( ",", "" ) ) );

                        }
                        else
                        {
                            sDecimalPointFormat = "{0:#,##0}";
                            this.Text = String.Format( sDecimalPointFormat, Convert.ToDouble( this.Text.Replace( ",", "" ) ) );
                        }
                    }
                    else
                    {
                        this.Text = String.Format( "{0:#,##0}", Convert.ToDouble( this.Text.Replace( ",", "" ) ) );

                    }
                }
            }
        }

        private void ItierTextBox_KeyPress( object sender, KeyPressEventArgs e )
        {
            switch( MaskType )
            {
                case Mask.Integer:
                    TypingOnlyNumber( sender, e, false, true );
                    break;
                case Mask.Decimal:
                    TypingOnlyNumber( sender, e, true, true );
                    break;
                default:
                    break;
            }
        }
        /*
                private void ItierTextBox_KeyDown( object sender, KeyEventArgs e )
                {
                    switch( MaskType )
                    {
                        case Mask.Integer:
                            TypingOnlyNumberKeyDown( sender, e, false, true );
                            break;
                        case Mask.Decimal:
                            TypingOnlyNumberKeyDown( sender, e, true, true );
                            break;
                        default:
                            break;
                    }

                }
        */
        #endregion

        #region Property

        [Description( "Max of number you needed" )]
        public Int64 MaxValue
        {
            set
            {
                m_maxvalue = value;
            }
            get
            {
                return m_maxvalue;
            }
        }

        [Description( "Decimal Point Value. Default 0" )]
        public int DecimalPoint
        {
            set
            {
                m_decimalpoint = value;
            }
            get
            {
                return m_decimalpoint;
            }
        }

        [Description( "TextBox Mask Type" )]
        public Mask MaskType
        {
            get
            {
                return m_mask;
            }
            set
            {
                m_mask = value;
            }
        }
        #endregion
    }
}
