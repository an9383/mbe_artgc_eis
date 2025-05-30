using KR.MBE.Data;
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Handler;

namespace KR.MBE.UI.ControlUtil
{
    public class GridColumns
    {

        public static DataGridViewCheckBoxColumn GetYesNoCustomCheckBox()
        {
            DataGridViewCheckBoxColumn oYesNoCheck = new DataGridViewCheckBoxColumn();
            oYesNoCheck.TrueValue = "Yes";
            oYesNoCheck.FalseValue = "No";
            return oYesNoCheck;
        }

        public static DataGridViewCheckBoxColumn GetCustomCheckBox( string strTrueText, string strFalseText )
        {
            DataGridViewCheckBoxColumn oCheck = new DataGridViewCheckBoxColumn();
            oCheck.TrueValue = strTrueText;
            oCheck.FalseValue = strFalseText;
            return oCheck;
        }

        public static DataGridViewTextBoxColumn GetNumber( int iDecimalPlaces )
        {
            DataGridViewTextBoxColumn oNumber = new DataGridViewTextBoxColumn();
            CultureInfo ci = CultureInfo.CreateSpecificCulture( "en-US" );
            oNumber.ValueType = typeof( decimal );
            oNumber.DefaultCellStyle.FormatProvider = ci;
            if( iDecimalPlaces > 0 )
            {
                oNumber.DefaultCellStyle.Format = "N" + iDecimalPlaces.ToString();
            }
            else
            {
                oNumber.DefaultCellStyle.Format = "N0";
            }

            return oNumber;
        }

        // 2020-03-04 강성묵 추가
        //CustomControl.testGrid 에서 사용 중
        //public static DataGridViewitierTextBoxColumn GetNumber( int iDecimalPlaces )
        //{
        //    DataGridViewitierTextBoxColumn oNumber = new DataGridViewitierTextBoxColumn();
        //    oNumber.ValueType = typeof( decimal );
        //    oNumber.DefaultCellStyle.Format = "N" + iDecimalPlaces.ToString();

        //    switch( iDecimalPlaces )
        //    {
        //        case 0:
        //        {
        //            oNumber.MaskType = CustomControl.Mask.Integer;
        //            oNumber.DecimalPoint = 0;
        //            break;
        //        }
        //        default:
        //        {
        //            oNumber.MaskType = CustomControl.Mask.Decimal;
        //            oNumber.DecimalPoint = iDecimalPlaces;
        //            break;
        //        }
        //    }
        //    return oNumber;
        //}

        public static DataGridViewTextBoxColumn GetText()
        {
            DataGridViewTextBoxColumn oText = new DataGridViewTextBoxColumn();
            return oText;
        }

        public static DataGridViewTextBoxColumn GetRichText()
        {
            DataGridViewTextBoxColumn oRichText = new DataGridViewTextBoxColumn();
            oRichText.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            return oRichText;
        }

        public static MaskedTextColumn GetMaskedText( String maskString )
        {
            MaskedTextColumn oMask = new MaskedTextColumn();
            oMask.Mask = maskString;
            return oMask;
        }

        public static MultiColumnComboColumn GetMultiColumnCombo()
        {
            MultiColumnComboColumn oMultiColumn = new MultiColumnComboColumn();
            return oMultiColumn;
        }

        public static CalendarColumn GetCalendarDate()
        {
            CalendarColumn oDate = new CalendarColumn();
            return oDate;
        }

        public static DataGridViewTextBoxColumn GetDateTime()
        {
            DataGridViewTextBoxColumn oDateTime = new DataGridViewTextBoxColumn();
            oDateTime.ValueType = typeof( DateTime );
            oDateTime.DefaultCellStyle.Format = "yyyy-MM-dd HH:mm:ss";
            return oDateTime;
        }

        // 2020-03-04 강성묵 추가
        //public static DataGridViewMaskedTextBoxColumn GetDateTime()
        //{
        //    DataGridViewMaskedTextBoxColumn oDateTime = new DataGridViewMaskedTextBoxColumn();
        //    oDateTime.Mask = "####-##-## ##:##:##";
        //    return oDateTime;
        //}

        public static DataGridViewButtonColumn GetButton( string strText )
        {
            DataGridViewButtonColumn oButton = new DataGridViewButtonColumn();
            oButton.FlatStyle = FlatStyle.Standard;
            oButton.UseColumnTextForButtonValue = true;
            oButton.Text = strText;
            return oButton;
        }

        public static DataGridViewComboBoxColumn GetCustomComboBox( DataTable dtEnumList, string enumID, string editFlag, bool blankFlag )
        {
            DataGridViewComboBoxColumn cmbCol = new DataGridViewComboBoxColumn();

            if( enumID.Length > 0 )
            {
                DataTable dtList = dtEnumList.Clone();
                DataRow[] dtRowList = dtEnumList.Select( "ENUMID ='" + enumID + "'" );
                foreach( DataRow Copy in dtRowList )
                {
                    dtList.ImportRow( Copy );
                }
                if( blankFlag )
                {
                    // 2020-05-25 Empty Row Insert by SH.Jung
                    DataRow drEmpty = dtList.NewRow();
                    dtList.Rows.InsertAt( drEmpty, 0 );
                }
                cmbCol.DataSource = dtList;
                cmbCol.ValueMember = "ENUMVALUE";
                cmbCol.DisplayMember = "ENUMVALUENAME";
            }
            // 콤보박스 컬럼 스타일 설정.
            if( editFlag == Constant.FlagYesOrNo.Yes )
            {
                cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            }
            else
            {
                cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
            cmbCol.FlatStyle = FlatStyle.Popup;
            return cmbCol;
        }

        public static DataGridViewComboBoxColumn GetCustomComboBox( string siteID, string enumID, string editFlag )
        {
            DataGridViewComboBoxColumn cmbCol = new DataGridViewComboBoxColumn();

            if( enumID.Length > 0 )
            {
                // db에서 enum정보를 받아옴.
                Hashtable htParameter = new Hashtable();
                htParameter.Add( "SITEID", siteID );
                htParameter.Add( "ENUMID", enumID );
                DataSet dsEnum = MessageHandler.getCustomQuery( siteID, "GetEnumValue", "00001", Constant.LanguageCode.LC_KOREAN, htParameter );

                // 2020.04.17 강성묵 수정.
                if( dsEnum.Tables.Contains( "_ERROR" ) )
                {
                    string sReturnCode = dsEnum.Tables["_ERROR"].Rows[0]["returncode"].ToString();
                    MessageBox.Show( sReturnCode );
                    return null;
                }
                else
                {
                    DataTable dtList = dsEnum.Tables["_REPLYDATA"];

                    if( dsEnum.Tables["_REPLYDATA"].Rows.Count == 0 )
                    {
                        //MessageBox.Show( enumID + "에 대한 데이터가 없습니다." );
                        MessageBox.Show("No data for enumID" + enumID); // USER-508

                        return null;
                    }
                    // 2020-05-25 Empty Row Insert by SH.Jung
                    DataRow drEmpty = dtList.NewRow();
                    dtList.Rows.InsertAt( drEmpty, 0 );

                    cmbCol.DataSource = dtList;
                    cmbCol.ValueMember = "ENUMVALUE";
                    cmbCol.DisplayMember = "ENUMVALUENAME";
                }
            }
            // 콤보박스 컬럼 스타일 설정.
            if( editFlag == Constant.FlagYesOrNo.Yes )
            {
                cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            }
            else
            {
                cmbCol.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
            cmbCol.FlatStyle = FlatStyle.Popup;

            return cmbCol;
        }

        public static DataGridViewProgressColumn GetPercent()
        {
            DataGridViewProgressColumn oPercent = new DataGridViewProgressColumn();
            return oPercent;
        }

        public static DataGridViewTextBoxColumn GetEncrypt()
        {
            DataGridViewEncryptColumn oText = new DataGridViewEncryptColumn();
            return oText;
        }

        #region Celltype에 맞게 셀 생성.
        public static DataGridViewCheckBoxCell GetCustomCheckBoxCell( string trueVal, string falseVal )
        {
            DataGridViewCheckBoxCell chkCell = new DataGridViewCheckBoxCell();
            chkCell.TrueValue = trueVal;
            chkCell.FalseValue = falseVal;
            return chkCell;
        }

        public static DataGridViewButtonCell GetCustomButtonCell( string btnText )
        {
            DataGridViewButtonCell btnCell = new DataGridViewButtonCell();
            btnCell.UseColumnTextForButtonValue = true;
            btnCell.FlatStyle = FlatStyle.Standard;
            btnCell.Value = btnText;
            btnCell.ReadOnly = false;
            return btnCell;
        }

        public static DataGridViewComboBoxCell GetCustomComboBoxCell( string siteID, string enumID )
        {
            // db에서 enum정보를 받아옴.
            Hashtable htParameter = new Hashtable();
            htParameter.Add( "SITEID", siteID );
            htParameter.Add( "ENUMID", enumID );
            DataSet dsEnum = MessageHandler.getCustomQuery( siteID, "GetEnumValue", "00001", Constant.LanguageCode.LC_KOREAN, htParameter );

            // 셀에 적용.
            DataGridViewComboBoxCell cmbCell = new DataGridViewComboBoxCell();
            cmbCell.DataSource = dsEnum.Tables[0];
            cmbCell.ValueMember = "ENUMVALUE";
            cmbCell.DisplayMember = "ENUMVALUENAME";

            // 콤보박스 스타일 설정.
            cmbCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            cmbCell.FlatStyle = FlatStyle.Popup;

            return cmbCell;
        }

        public static DataGridViewTextBoxCell GetCustomNumberCell( int iDecimalPlaces )
        {
            DataGridViewTextBoxCell numCell = GetCustomTextCell();
            CultureInfo ci = CultureInfo.CreateSpecificCulture( "en-US" );
            numCell.ValueType = typeof( decimal );
            numCell.Style.FormatProvider = ci;
            if( iDecimalPlaces > 0 )
            {

                numCell.Style.Format = "N" + iDecimalPlaces.ToString();
            }
            else
            {
                numCell.Style.Format = "N0";
            }
            return numCell;
        }

        public static DataGridViewTextBoxCell GetCustomDateTimeCell()
        {
            DataGridViewTextBoxCell dateCell = GetCustomTextCell();
            dateCell.ValueType = typeof( DateTime );
            dateCell.Style.Format = "yyyy-MM-dd HH:mm:ss";
            return dateCell;
        }

        public static DataGridViewTextBoxCell GetCustomRichTextCell()
        {
            DataGridViewTextBoxCell rtCell = GetCustomTextCell();
            rtCell.Style.WrapMode = DataGridViewTriState.True;
            return rtCell;
        }

        public static DataGridViewTextBoxCell GetCustomTextCell( bool readOnly )
        {
            DataGridViewTextBoxCell txtCell = new DataGridViewTextBoxCell();
            txtCell.ReadOnly = readOnly;
            return txtCell;
        }
        public static DataGridViewTextBoxCell GetCustomTextCell()
        {
            DataGridViewTextBoxCell txtCell = new DataGridViewTextBoxCell();
            return txtCell;
        }

        #endregion

        // 2020-03-04 강성묵 추가
        //#region DataGridViewMaskedTextBoxColumn
        //public class DataGridViewMaskedTextBoxCell : DataGridViewTextBoxCell
        //{
        //    #region Fields
        //    private static Type cellType = typeof( DataGridViewMaskedTextBoxCell );
        //    private static Type valueType = typeof( decimal );
        //    private static Type editorType = typeof( DataGridViewMaskedTextBoxEditingControl );
        //    #endregion

        //    #region Constructor, Clone, ToString
        //    public DataGridViewMaskedTextBoxCell()
        //        : base()
        //    {
        //        Mask = string.Empty;
        //    }

        //    public override object Clone()
        //    {
        //        DataGridViewMaskedTextBoxCell cell = base.Clone() as DataGridViewMaskedTextBoxCell;
        //        cell.Mask = this.Mask;
        //        return cell;
        //    }

        //    public override string ToString()
        //    {
        //        StringBuilder builder = new StringBuilder( 0x40 );
        //        builder.Append( "DataGridViewMaskedTextCell { ColumnIndex=" );
        //        builder.Append( base.ColumnIndex.ToString() );
        //        builder.Append( ", RowIndex=" );
        //        builder.Append( base.RowIndex.ToString() );
        //        builder.Append( " }" );
        //        return builder.ToString();
        //    }
        //    #endregion

        //    #region Derived methods
        //    public override void DetachEditingControl()
        //    {
        //        DataGridView dataGridView = base.DataGridView;
        //        if( ( dataGridView == null ) || ( dataGridView.EditingControl == null ) )
        //        {
        //            throw new InvalidOperationException();
        //        }
        //        MaskedTextBox editingControl = dataGridView.EditingControl as MaskedTextBox;
        //        if( editingControl != null )
        //        {
        //            editingControl.ClearUndo();
        //        }
        //        base.DetachEditingControl();
        //    }

        //    public override void InitializeEditingControl( int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle )
        //    {
        //        base.InitializeEditingControl( rowIndex, initialFormattedValue, dataGridViewCellStyle );
        //        DataGridViewMaskedTextBoxEditingControl editingControl = base.DataGridView.EditingControl as DataGridViewMaskedTextBoxEditingControl;
        //        if( editingControl != null )
        //        {
        //            if( Value == null || Value is DBNull )
        //            {
        //                editingControl.Text = ( string )DefaultNewRowValue;
        //            }
        //            else
        //            {
        //                //editingControl.Text = (string)Value;
        //                string sValue = Value.ToString();
        //                editingControl.Text = String.Format("{0:#,##0}", sValue);
        //            }
        //        }
        //    }
        //    #endregion

        //    #region Derived properties
        //    public override Type EditType
        //    {
        //        get
        //        {
        //            return editorType;
        //        }
        //    }

        //    public override Type ValueType
        //    {
        //        get
        //        {
        //            return valueType;
        //        }
        //    }
        //    #endregion

        //    #region Additional Mask property 
        //    private string mask;
        //    public string Mask
        //    {
        //        get
        //        {
        //            return mask == null ? string.Empty : mask;
        //        }
        //        set
        //        {
        //            mask = value;
        //        }
        //    }
        //    #endregion
        //}

        //public class DataGridViewMaskedTextBoxEditingControl : MaskedTextBox, IDataGridViewEditingControl
        //{
        //    #region Fields
        //    private DataGridView dataGridView;
        //    private bool valueChanged;
        //    private int rowIndex;
        //    #endregion

        //    #region Constructor
        //    public DataGridViewMaskedTextBoxEditingControl()
        //    {
        //        Mask = string.Empty;
        //    }
        //    #endregion

        //    #region Interface's properties
        //    public DataGridView EditingControlDataGridView
        //    {
        //        get
        //        {
        //            return dataGridView;
        //        }
        //        set
        //        {
        //            dataGridView = value;
        //        }
        //    }

        //    public object EditingControlFormattedValue
        //    {
        //        get
        //        {
        //            return Text;
        //        }
        //        set
        //        {
        //            if( value is string )
        //            {
        //                Text = ( string )value;
        //            }
        //        }
        //    }

        //    public int EditingControlRowIndex
        //    {
        //        get
        //        {
        //            return rowIndex;
        //        }
        //        set
        //        {
        //            rowIndex = value;
        //        }
        //    }

        //    public bool EditingControlValueChanged
        //    {
        //        get
        //        {
        //            return valueChanged;
        //        }
        //        set
        //        {
        //            valueChanged = value;
        //        }
        //    }

        //    public Cursor EditingPanelCursor
        //    {
        //        get
        //        {
        //            return base.Cursor;
        //        }
        //    }

        //    public bool RepositionEditingControlOnValueChange
        //    {
        //        get
        //        {
        //            return false;
        //        }
        //    }
        //    #endregion

        //    #region Interface's methods
        //    public void ApplyCellStyleToEditingControl( DataGridViewCellStyle dataGridViewCellStyle )
        //    {
        //        Font = dataGridViewCellStyle.Font;
        //        DataGridViewMaskedTextBoxCell cell = dataGridView.CurrentCell as DataGridViewMaskedTextBoxCell;
        //        if( cell != null )
        //        {
        //            Mask = cell.Mask;
        //        }
        //    }

        //    public bool EditingControlWantsInputKey( Keys key, bool dataGridViewWantsInputKey )
        //    {
        //        switch( key & Keys.KeyCode )
        //        {
        //            case Keys.Left:
        //            case Keys.Right:
        //            case Keys.Home:
        //            case Keys.End:
        //                return true;
        //            default:
        //                return false;
        //        }
        //    }

        //    public object GetEditingControlFormattedValue( DataGridViewDataErrorContexts context )
        //    {
        //        return EditingControlFormattedValue;
        //    }

        //    public void PrepareEditingControlForEdit( bool selectAll )
        //    {
        //        if( selectAll )
        //        {
        //            SelectAll();
        //        }
        //        else
        //        {
        //            SelectionStart = 0;
        //            SelectionLength = 0;
        //        }
        //    }
        //    #endregion

        //    #region MaskedTextBox event
        //    protected override void OnTextChanged( System.EventArgs e )
        //    {
        //        base.OnTextChanged( e );
        //        EditingControlValueChanged = true;
        //        if( EditingControlDataGridView != null )
        //        {
        //            try
        //            {
        //                string sText = Text;
        //                EditingControlDataGridView.CurrentCell.Value = sText.Replace(",", "");
        //            }
        //            catch { }

        //        }
        //    }
        //    #endregion
        //}

        //public class DataGridViewMaskedTextBoxColumn : DataGridViewColumn
        //{
        //    #region Fields
        //    private static Type columnType = typeof( DataGridViewMaskedTextBoxColumn );
        //    #endregion

        //    #region Constructors
        //    public DataGridViewMaskedTextBoxColumn()
        //        : this( String.Empty )
        //    {
        //    }

        //    public DataGridViewMaskedTextBoxColumn( string maskString ) : base( new DataGridViewMaskedTextBoxCell() )
        //    {
        //        SortMode = DataGridViewColumnSortMode.Automatic;
        //        Mask = maskString;
        //    }
        //    #endregion

        //    #region Methods
        //    public override string ToString()
        //    {
        //        StringBuilder builder = new StringBuilder( 0x40 );
        //        builder.Append( "DataGridViewMaskedTextColumn { Name=" );
        //        builder.Append( base.Name );
        //        builder.Append( ", Index=" );
        //        builder.Append( base.Index.ToString() );
        //        builder.Append( " }" );
        //        return builder.ToString();
        //    }

        //    public override object Clone()
        //    {
        //        DataGridViewMaskedTextBoxColumn col = ( DataGridViewMaskedTextBoxColumn )base.Clone();
        //        col.Mask = Mask;
        //        col.CellTemplate = ( DataGridViewMaskedTextBoxCell )this.CellTemplate.Clone();
        //        return col;
        //    }
        //    #endregion

        //    #region Derived properties
        //    public override DataGridViewCell CellTemplate
        //    {
        //        get
        //        {
        //            return base.CellTemplate;
        //        }
        //        set
        //        {
        //            if( ( value != null ) && !( value is DataGridViewMaskedTextBoxCell ) )
        //            {
        //                throw new InvalidCastException( "DataGridView: WrongCellTemplateType, must be DataGridViewMaskedTextCell" );
        //            }
        //            base.CellTemplate = value;
        //        }
        //    }

        //    [DefaultValue( 1 )]
        //    public new DataGridViewColumnSortMode SortMode
        //    {
        //        get
        //        {
        //            return base.SortMode;
        //        }
        //        set
        //        {
        //            base.SortMode = value;
        //        }
        //    }

        //    private DataGridViewMaskedTextBoxCell MaskedTextCellTemplate
        //    {
        //        get
        //        {
        //            return ( DataGridViewMaskedTextBoxCell )CellTemplate;
        //        }
        //    }
        //    #endregion

        //    #region Mask property
        //    public string Mask
        //    {
        //        get
        //        {
        //            if( MaskedTextCellTemplate == null )
        //            {
        //                throw new InvalidOperationException( "DataGridViewColumn: CellTemplate required" );
        //            }
        //            return MaskedTextCellTemplate.Mask;
        //        }
        //        set
        //        {
        //            if( Mask != value )
        //            {
        //                MaskedTextCellTemplate.Mask = value;
        //                if( base.DataGridView != null )
        //                {
        //                    DataGridViewRowCollection rows = base.DataGridView.Rows;
        //                    int count = rows.Count;
        //                    for( int i = 0; i < count; i++ )
        //                    {
        //                        DataGridViewMaskedTextBoxCell cell = rows.SharedRow( i ).Cells[base.Index] as DataGridViewMaskedTextBoxCell;
        //                        if( cell != null )
        //                        {
        //                            cell.Mask = value;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        //#region DataGridViewitierTextBoxColumn
        //public class DataGridViewitierTextBoxCell : DataGridViewTextBoxCell
        //{
        //    #region Fields
        //    private static Type cellType = typeof( DataGridViewitierTextBoxCell );
        //    private static Type valueType = typeof( decimal );
        //    private static Type editorType = typeof( DataGridViewitierTextBoxEditingControl );
        //    #endregion

        //    #region Constructor, Clone, ToString
        //    public DataGridViewitierTextBoxCell()
        //        : base()
        //    {
        //        MaskType = Mask.None;
        //        DecimalPoint = 0;
        //    }

        //    public override object Clone()
        //    {
        //        DataGridViewitierTextBoxCell cell = base.Clone() as DataGridViewitierTextBoxCell;
        //        cell.MaskType = this.MaskType;
        //        cell.DecimalPoint = this.DecimalPoint;
        //        return cell;
        //    }

        //    public override string ToString()
        //    {
        //        StringBuilder builder = new StringBuilder( 0x40 );
        //        builder.Append( "DataGridViewitierTextBoxCell { ColumnIndex=" );
        //        builder.Append( base.ColumnIndex.ToString() );
        //        builder.Append( ", RowIndex=" );
        //        builder.Append( base.RowIndex.ToString() );
        //        builder.Append( " }" );
        //        return builder.ToString();
        //    }
        //    #endregion

        //    #region Derived methods
        //    public override void DetachEditingControl()
        //    {
        //        DataGridView dataGridView = base.DataGridView;
        //        if( ( dataGridView == null ) || ( dataGridView.EditingControl == null ) )
        //        {
        //            throw new InvalidOperationException();
        //        }
        //        itierTextBox editingControl = dataGridView.EditingControl as itierTextBox;
        //        if( editingControl != null )
        //        {
        //            editingControl.ClearUndo();
        //        }
        //        base.DetachEditingControl();
        //    }

        //    public override void InitializeEditingControl( int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle )
        //    {
        //        base.InitializeEditingControl( rowIndex, initialFormattedValue, dataGridViewCellStyle );
        //        DataGridViewitierTextBoxEditingControl editingControl = base.DataGridView.EditingControl as DataGridViewitierTextBoxEditingControl;
        //        if( editingControl != null )
        //        {
        //            if( Value == null || Value is DBNull )
        //            {
        //                editingControl.Text = (string)DefaultNewRowValue;
        //            }
        //            else
        //            {
        //                try
        //                {
        //                    editingControl.Text = (string)Value;
        //                }
        //                catch
        //                {
        //                    editingControl.Text = Value.ToString();
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    #region Derived properties
        //    public override Type EditType
        //    {
        //        get
        //        {
        //            return editorType;
        //        }
        //    }

        //    public override Type ValueType
        //    {
        //        get
        //        {
        //            return valueType;
        //        }
        //    }
        //    #endregion

        //    #region Additional property 
        //    private Mask mask;
        //    public Mask MaskType
        //    {
        //        get
        //        {
        //            return mask;
        //        }
        //        set
        //        {
        //            mask = value;
        //        }
        //    }

        //    public int DecimalPoint
        //    {
        //        get; set;
        //    }
        //    #endregion
        //}

        //public class DataGridViewitierTextBoxEditingControl : itierTextBox, IDataGridViewEditingControl
        //{
        //    #region Fields
        //    private DataGridView dataGridView;
        //    private bool valueChanged;
        //    private int rowIndex;
        //    #endregion

        //    #region Constructor
        //    public DataGridViewitierTextBoxEditingControl()
        //    {
        //        MaskType = Mask.None;
        //        DecimalPoint = 0;
        //    }
        //    #endregion

        //    #region Interface's properties
        //    public DataGridView EditingControlDataGridView
        //    {
        //        get
        //        {
        //            return dataGridView;
        //        }
        //        set
        //        {
        //            dataGridView = value;
        //        }
        //    }

        //    public object EditingControlFormattedValue
        //    {
        //        get
        //        {
        //            return Text;
        //        }
        //        set
        //        {
        //            if( value is string )
        //            {
        //                Text = ( string )value;
        //            }
        //        }
        //    }

        //    public int EditingControlRowIndex
        //    {
        //        get
        //        {
        //            return rowIndex;
        //        }
        //        set
        //        {
        //            rowIndex = value;
        //        }
        //    }

        //    public bool EditingControlValueChanged
        //    {
        //        get
        //        {
        //            return valueChanged;
        //        }
        //        set
        //        {
        //            valueChanged = value;
        //        }
        //    }

        //    public Cursor EditingPanelCursor
        //    {
        //        get
        //        {
        //            return base.Cursor;
        //        }
        //    }

        //    public bool RepositionEditingControlOnValueChange
        //    {
        //        get
        //        {
        //            return false;
        //        }
        //    }
        //    #endregion

        //    #region Interface's methods
        //    public void ApplyCellStyleToEditingControl( DataGridViewCellStyle dataGridViewCellStyle )
        //    {
        //        Font = dataGridViewCellStyle.Font;
        //        //	get the current cell to use the specific mask string
        //        DataGridViewitierTextBoxCell cell = dataGridView.CurrentCell as DataGridViewitierTextBoxCell;
        //        if( cell != null )
        //        {
        //            MaskType = cell.MaskType;
        //            DecimalPoint = cell.DecimalPoint;
        //        }
        //    }

        //    public bool EditingControlWantsInputKey( Keys key, bool dataGridViewWantsInputKey )
        //    {
        //        switch( key & Keys.KeyCode )
        //        {
        //            case Keys.Left:
        //            case Keys.Right:
        //            case Keys.Home:
        //            case Keys.End:
        //                return true;
        //            default:
        //                return false;
        //        }
        //    }

        //    public object GetEditingControlFormattedValue( DataGridViewDataErrorContexts context )
        //    {
        //        return EditingControlFormattedValue;
        //    }

        //    public void PrepareEditingControlForEdit( bool selectAll )
        //    {
        //        if( selectAll )
        //        {
        //            SelectAll();
        //        }
        //        else
        //        {
        //            SelectionStart = 0;
        //            SelectionLength = 0;
        //        }
        //    }
        //    #endregion

        //    #region itierTextBox event
        //    protected override void OnTextChanged( System.EventArgs e )
        //    {
        //        //if( EditingControlDataGridView.CurrentCell.Value.ToString() == "" )
        //        //{
        //        //    return;
        //        //}

        //        base.OnTextChanged( e );
        //        EditingControlValueChanged = false;
        //        if( EditingControlDataGridView != null )
        //        {
        //            try
        //            {
        //                string sText = Text;
        //                EditingControlDataGridView.CurrentCell.Value = sText.Replace(",", "");
        //            }
        //            catch { }
        //        }
        //        //EditingControlValueChanged = true;
        //    }
        //    #endregion
        //}

        //public class DataGridViewitierTextBoxColumn : DataGridViewColumn
        //{
        //    #region Fields
        //    private static Type columnType = typeof( DataGridViewitierTextBoxColumn );
        //    #endregion

        //    #region Constructors
        //    public DataGridViewitierTextBoxColumn()
        //        : this( Mask.None )
        //    {
        //    }

        //    public DataGridViewitierTextBoxColumn( Mask maskString ) : base( new DataGridViewitierTextBoxCell() )
        //    {
        //        SortMode = DataGridViewColumnSortMode.Automatic;
        //        MaskType = maskString;
        //    }
        //    #endregion

        //    #region Methods
        //    public override string ToString()
        //    {
        //        StringBuilder builder = new StringBuilder( 0x40 );
        //        builder.Append( "DataGridViewitierTextColumn { Name=" );
        //        builder.Append( base.Name );
        //        builder.Append( ", Index=" );
        //        builder.Append( base.Index.ToString() );
        //        builder.Append( " }" );
        //        return builder.ToString();
        //    }

        //    public override object Clone()
        //    {
        //        DataGridViewitierTextBoxColumn col = ( DataGridViewitierTextBoxColumn )base.Clone();
        //        col.MaskType = this.MaskType;
        //        col.DecimalPoint = this.DecimalPoint;
        //        col.CellTemplate = ( DataGridViewitierTextBoxCell )this.CellTemplate.Clone();
        //        return col;
        //    }
        //    #endregion

        //    #region Derived properties
        //    public override DataGridViewCell CellTemplate
        //    {
        //        get
        //        {
        //            return base.CellTemplate;
        //        }
        //        set
        //        {
        //            if( ( value != null ) && !( value is DataGridViewitierTextBoxCell ) )
        //            {
        //                throw new InvalidCastException( "DataGridView: WrongCellTemplateType, must be DataGridViewitierTextCell" );
        //            }
        //            base.CellTemplate = value;
        //        }
        //    }

        //    [DefaultValue( 1 )]
        //    public new DataGridViewColumnSortMode SortMode
        //    {
        //        get
        //        {
        //            return base.SortMode;
        //        }
        //        set
        //        {
        //            base.SortMode = value;
        //        }
        //    }

        //    private DataGridViewitierTextBoxCell itierTextCellTemplate
        //    {
        //        get
        //        {
        //            return ( DataGridViewitierTextBoxCell )CellTemplate;
        //        }
        //    }
        //    #endregion

        //    #region property
        //    public int DecimalPoint
        //    {
        //        get
        //        {
        //            if( itierTextCellTemplate == null )
        //            {
        //                throw new InvalidOperationException( "DataGridViewColumn: CellTemplate required" );
        //            }
        //            return itierTextCellTemplate.DecimalPoint;
        //        }
        //        set
        //        {
        //            if( DecimalPoint != value )
        //            {
        //                /// If the mask is changed, the cell template has to be changed,
        //                /// and each cell of the column has to be adapted.
        //                itierTextCellTemplate.DecimalPoint = value;
        //                if( base.DataGridView != null )
        //                {
        //                    DataGridViewRowCollection rows = base.DataGridView.Rows;
        //                    int count = rows.Count;
        //                    for( int i = 0; i < count; i++ )
        //                    {
        //                        DataGridViewitierTextBoxCell cell = rows.SharedRow( i ).Cells[base.Index] as DataGridViewitierTextBoxCell;
        //                        if( cell != null )
        //                        {
        //                            cell.DecimalPoint = value;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    public Mask MaskType
        //    {
        //        get
        //        {
        //            if( itierTextCellTemplate == null )
        //            {
        //                throw new InvalidOperationException( "DataGridViewColumn: CellTemplate required" );
        //            }
        //            return itierTextCellTemplate.MaskType;
        //        }
        //        set
        //        {
        //            if( MaskType != value )
        //            {
        //                /// If the mask is changed, the cell template has to be changed,
        //                /// and each cell of the column has to be adapted.
        //                itierTextCellTemplate.MaskType = value;
        //                if( base.DataGridView != null )
        //                {
        //                    DataGridViewRowCollection rows = base.DataGridView.Rows;
        //                    int count = rows.Count;
        //                    for( int i = 0; i < count; i++ )
        //                    {
        //                        DataGridViewitierTextBoxCell cell = rows.SharedRow( i ).Cells[base.Index] as DataGridViewitierTextBoxCell;
        //                        if( cell != null )
        //                        {
        //                            cell.MaskType = value;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    #endregion
        //}
        //#endregion

        // 2021.05.27 강성묵 추가
        #region DataGridViewProgressColumn Class
        public class DataGridViewProgressColumn : DataGridViewImageColumn
        {
            public DataGridViewProgressColumn()
            {
                CellTemplate = new DataGridViewProgressCell();
            }

            public DataGridViewProgressColumn( string columnName )
            {
                CellTemplate = new DataGridViewProgressCell();
                Name = columnName;
            }
        }

        public class DataGridViewProgressCell : DataGridViewImageCell
        {
            static Image emptyImage;
            static DataGridViewProgressCell()
            {
                emptyImage = new Bitmap( 1, 1, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
            }
            public DataGridViewProgressCell()
            {
                this.ValueType = typeof( int );
            }

            protected override object GetFormattedValue( object value, int rowIndex, ref DataGridViewCellStyle cellStyle, TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context )
            {
                return emptyImage;
            }

            protected override void Paint( System.Drawing.Graphics g, System.Drawing.Rectangle clipBounds, System.Drawing.Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts )
            {
                double progress = ( value == null ) ? 0.0f : Convert.ToDouble( value );
                double percentage = progress / 100.0f; // Need to convert to float before division; otherwise C# returns int which is 0 for anything but 100%.
                string progressVal = string.Format( "{0:0}", progress );
                Brush backColorBrush = new SolidBrush( cellStyle.BackColor );
                Brush foreColorBrush = new SolidBrush( cellStyle.ForeColor );
                // Draws the cell grid
                base.Paint( g, clipBounds, cellBounds,
                 rowIndex, cellState, value, formattedValue, errorText,
                 cellStyle, advancedBorderStyle, ( paintParts & ~DataGridViewPaintParts.ContentForeground ) );

                // 0% ~ 50%
                if( percentage >= 0.0 && percentage < 0.5 )
                {
                    g.FillRectangle( new SolidBrush( Color.FromArgb( 255, 51, 51 ) ), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32( ( percentage * cellBounds.Width - 4 ) ), cellBounds.Height - 4 );
                    g.DrawString( progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + ( cellBounds.Width / 2 ) - 9, cellBounds.Y + 2 );

                }
                // 50% ~ 100% 
                else if( percentage >= 0.5 && percentage < 1.0 )
                {
                    g.FillRectangle( new SolidBrush( Color.FromArgb( 51, 51, 255 ) ), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32( ( percentage * cellBounds.Width - 4 ) ), cellBounds.Height - 4 );
                    g.DrawString( progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + ( cellBounds.Width / 2 ) - 9, cellBounds.Y + 2 );

                }
                // 100% ~
                else if( percentage >= 1.0 )
                {
                    percentage = 1.0f;
                    g.FillRectangle( new SolidBrush( Color.FromArgb( 105, 252, 105 ) ), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32( ( percentage * cellBounds.Width - 4 ) ), cellBounds.Height - 4 );
                    g.DrawString( progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + ( cellBounds.Width / 2 ) - 9, cellBounds.Y + 2 );
                }
                // -100% ~ 0%
                else if( percentage < 0.0 && percentage > -1.0 )
                {
                    percentage *= -1;
                    g.FillRectangle( new SolidBrush( Color.FromArgb( 204, 204, 204 ) ), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32( ( percentage * cellBounds.Width - 4 ) ), cellBounds.Height - 4 );
                    g.DrawString( progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + ( cellBounds.Width / 2 ) - 15, cellBounds.Y + 2 );
                }
                // ~ -100%
                else
                {
                    percentage = 1.0f;
                    g.FillRectangle( new SolidBrush( Color.FromArgb( 204, 204, 204 ) ), cellBounds.X + 2, cellBounds.Y + 2, Convert.ToInt32( ( percentage * cellBounds.Width - 4 ) ), cellBounds.Height - 4 );
                    g.DrawString( progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + ( cellBounds.Width / 2 ) - 15, cellBounds.Y + 2 );
                }
            }
        }
        #endregion

        //2021.05.31 강성묵 추가
        #region DataGridViewEncryptColumn Class
        public class DataGridViewEncryptColumn : DataGridViewTextBoxColumn
        {
            public DataGridViewEncryptColumn()
            {
                CellTemplate = base.CellTemplate;
            }
        }
        #endregion
    }
}
