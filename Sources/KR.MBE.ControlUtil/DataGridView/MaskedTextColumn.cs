using System;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil
{

    [System.Drawing.ToolboxBitmap( typeof( System.Windows.Forms.MaskedTextBox ) )]
    public class MaskedTextColumn : DataGridViewColumn
    {

        #region Fields
        private static Type columnType = typeof( MaskedTextColumn );
        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public MaskedTextColumn()
            : this( String.Empty )
        {
        }

        /// <summary>
        /// Constructor using a Mask string
        /// </summary>
        /// <param name="maskString">Mask string used in the EditingControl</param>
        public MaskedTextColumn( string maskString )
            : base( new DataGridViewMaskedTextCell() )
        {
            SortMode = DataGridViewColumnSortMode.Automatic;
            Mask = maskString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Converting the current MaskedTextColumn instance to a string value.
        /// </summary>
        /// <returns>String value of the instance containing the name 
        /// and column index.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder( 0x40 );
            builder.Append( "DataGridViewMaskedTextColumn { Name=" );
            builder.Append( base.Name );
            builder.Append( ", Index=" );
            builder.Append( base.Index.ToString() );
            builder.Append( " }" );
            return builder.ToString();
        }

        /// <summary>
        /// Creates a copy of a MaskedTextColumn containing the DGV-Column properties.
        /// </summary>
        /// <returns>Instance of a MaskedTextColumn using the Mask string.</returns>
        public override object Clone()
        {
            MaskedTextColumn col = ( MaskedTextColumn )base.Clone();
            col.Mask = Mask;
            col.CellTemplate = ( DataGridViewMaskedTextCell )this.CellTemplate.Clone();
            return col;
        }

        #endregion

        #region Derived properties

        [Browsable( false ),
         DesignerSerializationVisibility( DesignerSerializationVisibility.Hidden )]
        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                if( ( value != null ) && !( value is DataGridViewMaskedTextCell ) )
                {
                    throw new InvalidCastException( "DataGridView: WrongCellTemplateType, must be DataGridViewMaskedTextCell" );
                }
                base.CellTemplate = value;
            }
        }

        [DefaultValue( 1 )]
        public new DataGridViewColumnSortMode SortMode
        {
            get
            {
                return base.SortMode;
            }
            set
            {
                base.SortMode = value;
            }
        }

        private DataGridViewMaskedTextCell MaskedTextCellTemplate
        {
            get
            {
                return ( DataGridViewMaskedTextCell )CellTemplate;
            }
        }

        #endregion

        #region Mask property

        /// <summary>
        /// Input String that rules the possible input values in each cell of the column.
        /// </summary>
        [Category( "Masking" )]
        [ReferencedDescription( typeof( System.Windows.Forms.MaskedTextBox ), "Mask" )]
        public string Mask
        {
            get
            {
                if( MaskedTextCellTemplate == null )
                {
                    throw new InvalidOperationException( "DataGridViewColumn: CellTemplate required" );
                }
                return MaskedTextCellTemplate.Mask;
            }
            set
            {
                if( Mask != value )
                {
                    /// If the mask is changed, the cell template has to be changed,
                    /// and each cell of the column has to be adapted.
                    MaskedTextCellTemplate.Mask = value;
                    if( base.DataGridView != null )
                    {
                        DataGridViewRowCollection rows = base.DataGridView.Rows;
                        int count = rows.Count;
                        for( int i = 0; i < count; i++ )
                        {
                            DataGridViewMaskedTextCell cell = rows.SharedRow( i ).Cells[base.Index] as DataGridViewMaskedTextCell;
                            if( cell != null )
                            {
                                cell.Mask = value;
                            }
                        }
                    }
                }
            }
        }

        #endregion

    }





    /// <summary>
    /// DataGridViewMaskedTextCell is derived from TextBoxCell using all TextBox
    /// properties and containing the Mask property to host a MaskedTextBox.
    /// </summary>
    public class DataGridViewMaskedTextCell : DataGridViewTextBoxCell
    {

        #region Fields
        private static Type cellType = typeof( DataGridViewMaskedTextCell );
        private static Type valueType = typeof( string );
        private static Type editorType = typeof( DataGridViewMaskedTextEditingControl );
        #endregion

        #region Constructor, Clone, ToString

        public DataGridViewMaskedTextCell()
            : base()
        {
            Mask = String.Empty;
        }

        /// <summary>
        /// Creates a copy of a MaskedTextCell containing the DGV-Cell properties.
        /// </summary>
        /// <returns>Instance of a MaskedTextCell using the Mask string.</returns>
        public override object Clone()
        {
            DataGridViewMaskedTextCell cell = base.Clone() as DataGridViewMaskedTextCell;
            cell.Mask = this.Mask;
            return cell;
        }

        /// <summary>
        /// Converting the current MaskedTextCell instance to a string value.
        /// </summary>
        /// <returns>String value of the instance containing the type name, 
        /// column index, and row index.</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder( 0x40 );
            builder.Append( "DataGridViewMaskedTextCell { ColumnIndex=" );
            builder.Append( base.ColumnIndex.ToString() );
            builder.Append( ", RowIndex=" );
            builder.Append( base.RowIndex.ToString() );
            builder.Append( " }" );
            return builder.ToString();
        }

        #endregion

        #region Derived methods

        [EditorBrowsable( EditorBrowsableState.Advanced )]
        public override void DetachEditingControl()
        {
            DataGridView dataGridView = base.DataGridView;
            if( ( dataGridView == null ) || ( dataGridView.EditingControl == null ) )
            {
                throw new InvalidOperationException();
            }
            MaskedTextBox editingControl = dataGridView.EditingControl as MaskedTextBox;
            if( editingControl != null )
            {
                editingControl.ClearUndo();
            }
            base.DetachEditingControl();
        }

        public override void InitializeEditingControl( int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle )
        {
            base.InitializeEditingControl( rowIndex, initialFormattedValue, dataGridViewCellStyle );
            DataGridViewMaskedTextEditingControl editingControl
                = base.DataGridView.EditingControl as DataGridViewMaskedTextEditingControl;
            if( editingControl != null )
            {
                if( Value == null || Value is DBNull )
                    editingControl.Text = ( string )DefaultNewRowValue;
                else
                    editingControl.Text = ( string )Value;
            }
        }

        #endregion

        #region Derived properties

        public override Type EditType
        {
            get
            {
                return editorType;
            }
        }

        public override Type ValueType
        {
            get
            {
                return valueType;
            }
        }

        #endregion

        #region Additional Mask property 

        private string mask;

        /// <summary>
        /// Input String that rules the possible input values in the current cell of the column.
        /// </summary>
        public string Mask
        {
            get
            {
                return mask == null ? String.Empty : mask;
            }
            set
            {
                mask = value;
            }
        }

        #endregion

    }


    /// <summary>
    /// DataGridViewMaskedTextEditingControl is the MaskedTextBox that is hosted
    /// in a DataGridViewMaskedTextColumn.
    /// </summary>
    public class DataGridViewMaskedTextEditingControl : MaskedTextBox, IDataGridViewEditingControl
    {

        #region Fields
        private DataGridView dataGridView;
        private bool valueChanged;
        private int rowIndex;
        #endregion

        #region Constructor

        public DataGridViewMaskedTextEditingControl()
        {
            Mask = String.Empty;
        }

        #endregion

        #region Interface's properties

        public DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        public object EditingControlFormattedValue
        {
            get
            {
                return Text;
            }
            set
            {
                if( value is string )
                    Text = ( string )value;
            }
        }

        public int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        public bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        #endregion

        #region Interface's methods

        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle )
        {
            Font = dataGridViewCellStyle.Font;
            //	get the current cell to use the specific mask string
            DataGridViewMaskedTextCell cell
                = dataGridView.CurrentCell as DataGridViewMaskedTextCell;
            if( cell != null )
            {
                Mask = cell.Mask;
            }
        }

        public bool EditingControlWantsInputKey( Keys key, bool dataGridViewWantsInputKey )
        {
            //  Note: In a DataGridView, one could prefer to change the row using
            //	the up/down keys.
            switch( key & Keys.KeyCode )
            {
                case Keys.Left:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                    return true;
                default:
                    return false;
            }
        }

        public object GetEditingControlFormattedValue( DataGridViewDataErrorContexts context )
        {
            return EditingControlFormattedValue;
        }

        public void PrepareEditingControlForEdit( bool selectAll )
        {
            if( selectAll )
                SelectAll();
            else
            {
                SelectionStart = 0;
                SelectionLength = 0;
            }
        }

        #endregion

        #region MaskedTextBox event

        protected override void OnTextChanged( System.EventArgs e )
        {
            base.OnTextChanged( e );
            EditingControlValueChanged = true;
            if( EditingControlDataGridView != null )
            {
                EditingControlDataGridView.CurrentCell.Value = Text;
            }
        }

        #endregion

    }


    /// <summary>
    /// ReferencedDescriptionAttribute shows the description of a specific property
    /// in an existing class (the "referenced type").
    /// </summary>
    public class ReferencedDescriptionAttribute : DescriptionAttribute
    {

        public ReferencedDescriptionAttribute( Type referencedType, string propertyName )
        {
            //	default description
            string result = "Referenced description not available";

            //	gets the properties of the referenced type
            PropertyDescriptorCollection properties
                = TypeDescriptor.GetProperties( referencedType );

            if( properties != null )
            {

                // gets a PropertyDescriptor to the specific property.
                PropertyDescriptor property = properties[propertyName];
                if( property != null )
                {
                    //  gets the attributes of the required property
                    AttributeCollection attributes = property.Attributes;

                    // Gets the description attribute from the collection.
                    DescriptionAttribute descript
                        = ( DescriptionAttribute )attributes[typeof( DescriptionAttribute )];

                    // register the referenced description
                    if( !String.IsNullOrEmpty( descript.Description ) )
                        result = descript.Description;
                }

            }
            DescriptionValue = result;

        }

    }































}
