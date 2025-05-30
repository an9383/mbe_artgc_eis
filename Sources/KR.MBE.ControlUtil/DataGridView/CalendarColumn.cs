using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace KR.MBE.UI.ControlUtil
{
    public class CalendarColumn : DataGridViewColumn
    {
        public CalendarColumn() : base( new CalendarCell() )
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                // Ensure that the cell used for the template is a CalendarCell.
                if( value != null &&
                    !value.GetType().IsAssignableFrom( typeof( CalendarCell ) ) )
                {
                    throw new InvalidCastException( "Must be a CalendarCell" );
                }
                base.CellTemplate = value;
            }
        }

    }

    public class CalendarCell : DataGridViewTextBoxCell
    {
        public CalendarCell()
            : base()
        {
            // Use the short date format.
            this.Style.Format = "d";
        }

        public override void InitializeEditingControl( int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle )
        {
            // Set the value of the editing control to the current cell value.
            base.InitializeEditingControl( rowIndex, initialFormattedValue, dataGridViewCellStyle );
            CalendarEditingControl editingControl = base.DataGridView.EditingControl as CalendarEditingControl;
            if( editingControl != null )
            {
                if( Value == null || Value is DBNull )
                {
                    editingControl.Value = ( DateTime )this.DefaultNewRowValue;
                }
                else
                {
                    editingControl.Value = Convert.ToDateTime( this.Value );
                }
            }
        }

        public override Type EditType
        {
            get
            {
                // Return the type of the editing control that CalendarCell uses.
                return typeof( CalendarEditingControl );
            }
        }

        public override Type ValueType
        {
            get
            {
                // Return the type of the value that CalendarCell contains.
                return typeof( DateTime );
            }
        }

        public override object DefaultNewRowValue
        {
            get
            {
                // Use the current date and time as the default value.
                return DateTime.Now;
            }
        }
    }

    class CalendarEditingControl : DatePicker, IDataGridViewEditingControl
    {
        DataGridView dataGridView;
        private bool valueChanged = false;
        int rowIndex;

        public CalendarEditingControl()
        {
            this.Format = DateTimePickerFormat.Short;
        }

        // Implements the IDataGridViewEditingControl.EditingControlFormattedValue
        // property.
        public object EditingControlFormattedValue
        {
            get
            {
                return this.ToShortDateString();
            }
            set
            {
                if( value is String )
                {
                    try
                    {
                        // This will throw an exception of the string is
                        // null, empty, or not in the format of a date.
                        this.Value = DateTime.Parse( ( String )value );
                    }
                    catch
                    {
                        // In the case of an exception, just use the
                        // default value so we're not left with a null
                        // value.
                        this.Value = DateTime.Now;
                    }
                }
            }
        }

        // Implements the
        // IDataGridViewEditingControl.GetEditingControlFormattedValue method.
        public object GetEditingControlFormattedValue(
            DataGridViewDataErrorContexts context )
        {
            return EditingControlFormattedValue;
        }

        // Implements the
        // IDataGridViewEditingControl.ApplyCellStyleToEditingControl method.
        public void ApplyCellStyleToEditingControl(
            DataGridViewCellStyle dataGridViewCellStyle )
        {
            this.Font = dataGridViewCellStyle.Font;
            this.CalendarForeColor = dataGridViewCellStyle.ForeColor;
            this.CalendarMonthBackground = dataGridViewCellStyle.BackColor;
        }

        // Implements the IDataGridViewEditingControl.EditingControlRowIndex
        // property.
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

        // Implements the IDataGridViewEditingControl.EditingControlWantsInputKey
        // method.
        public bool EditingControlWantsInputKey(
            Keys key, bool dataGridViewWantsInputKey )
        {
            // Let the DateTimePicker handle the keys listed.
            switch( key & Keys.KeyCode )
            {
                case Keys.Left:
                case Keys.Up:
                case Keys.Down:
                case Keys.Right:
                case Keys.Home:
                case Keys.End:
                case Keys.PageDown:
                case Keys.PageUp:
                    return true;
                default:
                    return !dataGridViewWantsInputKey;
            }
        }

        // Implements the IDataGridViewEditingControl.PrepareEditingControlForEdit
        // method.
        public void PrepareEditingControlForEdit( bool selectAll )
        {
            // No preparation needs to be done.
        }

        // Implements the IDataGridViewEditingControl
        // .RepositionEditingControlOnValueChange property.
        public bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        // Implements the IDataGridViewEditingControl
        // .EditingControlDataGridView property.
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

        // Implements the IDataGridViewEditingControl
        // .EditingControlValueChanged property.
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

        // Implements the IDataGridViewEditingControl
        // .EditingPanelCursor property.
        public Cursor EditingPanelCursor
        {
            get
            {
                return base.Cursor;
            }
        }

        protected override void OnValueChanged( EventArgs eventargs )
        {
            // Notify the DataGridView that the contents of the cell
            // have changed.
            valueChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty( true );
            base.OnValueChanged( eventargs );
        }
    }

    [ToolboxBitmap( typeof( System.Windows.Forms.DateTimePicker ) )] //Show datetimepicker icon
    public class DatePicker : System.Windows.Forms.DateTimePicker
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;
        private bool realDate = true;
        private DateTimePickerFormat oldFormat;

        public DatePicker()
        {
            // This call is required by the Windows.Forms Form Designer.
            InitializeComponent();
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
        #endregion

        //Bindable true: so we can bind to the property
        //Browsable false: if browsable is set to false, the property does not appear in the property panel in the form designer
        //Browsable is by default true (a public property normally appears in the property panel)
        [Bindable( true ), Browsable( false )]
        public new object Value
        {
            get
            {
                if( realDate )
                {
                    return base.Value;
                }
                else
                {
                    return DBNull.Value; //If not a real date, sent DBNull to the bound field
                }
            }
            set
            {
                if( Convert.IsDBNull( value ) )
                {
                    realDate = false;
                    oldFormat = Format; //Store the Format of the datetimepicker
                    Format = DateTimePickerFormat.Custom;
                    CustomFormat = " "; //With this custom format, the datetimepicker is empty
                }
                else
                {
                    realDate = true;
                    base.Value = Convert.ToDateTime( value );
                }
            }
        }

        protected override void OnKeyDown( System.Windows.Forms.KeyEventArgs e )
        {
            if( e.KeyCode == System.Windows.Forms.Keys.Delete )
            {
                Value = MinDate; //Trigger changed
                Value = DateTime.Now; //For drop down
                Value = DBNull.Value; //To DBNull
            }
            else
            {
                if( !realDate )
                {
                    //If no date present and edit started, start at the current date...
                    Format = oldFormat; //Restore the format for a real date
                    CustomFormat = null; //If you don't reset the custom format to 'null' the datetimepicker will stay empty
                    realDate = true;
                    Value = DateTime.Now;
                }
            }
        }

        protected override void OnCloseUp( EventArgs eventargs )
        {
            //This is the magic touch!!!!
            if( Control.MouseButtons == MouseButtons.None )
            {
                if( !realDate )
                {
                    Format = oldFormat; //Restore the format for a real date
                    CustomFormat = null; //If you don't reset the custom format to 'null' the datetimepicker will stay empty
                    realDate = true;
                    DateTime tempdate;
                    tempdate = ( DateTime )Value;
                    Value = MinDate;
                    Value = tempdate;
                }
            }
            base.OnCloseUp( eventargs );
        }

        public string ToShortDateString()
        {
            if( !realDate )
                return String.Empty;
            else
            {
                DateTime dt = ( DateTime )Value;
                return dt.ToShortDateString();
            }
        }
    }


}
