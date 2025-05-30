using System.Collections;
using System.Data;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Handler;

namespace KR.MBE.UI.ControlUtil
{
    public class DataBind
    {

        public DataBind()
        {
        }

        public static DataTable getEnumValue( string _siteid, string _languagecode, string _enumname )
        {
            Hashtable htBindValue = new Hashtable();
            htBindValue.Add( "SITEID", _siteid );
            DataSet dsData = MessageHandler.getCustomQuery( _siteid, "GetEnumValueList", "00001", _languagecode, htBindValue );
            return dsData.Tables["_REPLYDATA"];
        }

        public static string[] getDataTableValueList( DataTable _dtData, string _columnname, string _columnvalue, string _getcolumnname )
        {
            DataRow[] drDataList = _dtData.Select( _columnname + " = '" + _columnvalue + "'" );
            string[] arrayReturn = new string[drDataList.Length];

            if( drDataList != null )
            {
                for( int i = 0; i < drDataList.Length; i++ )
                {
                    arrayReturn[i] = drDataList[i][_getcolumnname].ToString();

                }
            }
            return arrayReturn;
        }

        public static void ComboBoxBind( System.Windows.Forms.ComboBox _combobox, DataTable _dt, string _valuemember, string _displaymember, bool _addall )
        {
            ComboBoxBind( _combobox, _dt, _valuemember, _displaymember, _addall, "전체" );
        }

        public static void ComboBoxBind( System.Windows.Forms.ComboBox _combobox, DataTable _dt, string _valuemember, string _displaymember, bool _addall, string _allDataName )
        {
            _combobox.DataSource = null;

            if( _dt != null && _dt.Rows.Count != 0 )
            {
                if( _addall )
                {
                    DataRow drAll = _dt.NewRow();
                    drAll[_valuemember] = "";
                    drAll[_displaymember] = _allDataName;
                    _dt.Rows.InsertAt( drAll, 0 );
                }

                _combobox.DataSource = _dt;
                if( _dt.Columns.Contains( _displaymember ) )
                {
                    _combobox.DisplayMember = _displaymember;
                }

                if( _dt.Columns.Contains( _valuemember ) )
                {
                    _combobox.ValueMember = _valuemember;
                }
                _combobox.DropDownStyle = ComboBoxStyle.DropDownList;
                _combobox.FlatStyle = FlatStyle.Popup;
            }
        }

        //2020.03.26 강성묵 추가.
        public static void ComboBoxBind( System.Windows.Forms.ComboBox _combobox, DataTable _dt, string selectColName, string selectValue, string _valuemember, string _displaymember, bool _addall )
        {
            DataTable dtSubSection = _dt.Clone();
            DataRow[] arrDataRow = _dt.Select( selectColName + "=\'" + selectValue + "\'" );
            foreach( DataRow dr in arrDataRow )
            {
                dtSubSection.ImportRow( dr );
            }
            ComboBoxBind( _combobox, dtSubSection, _valuemember, _displaymember, _addall );
        }

        public static void ComboBoxBind( System.Windows.Forms.ComboBox _combobox, string _siteid, string _queryid, string _queryversion, string _languagecode, Hashtable _htbindvalue, string _valuemember, string _displaymember, bool _addall )
        {
            DataSet dsData = MessageHandler.getCustomQuery( _siteid, _queryid, _queryversion, _languagecode, _htbindvalue );
            if ( (dsData != null) && (dsData.Tables.Count > 0))
            {
                ComboBoxBind(_combobox, dsData.Tables[0], _valuemember, _displaymember, _addall);
            }
        }

        //2021.10.06 강성묵 추가.
        public static void ComboBoxBind( System.Windows.Forms.ComboBox _combobox, string _siteid, string _queryid, string _queryversion, string _languagecode, Hashtable _htbindvalue, string _valuemember, string _displaymember, bool _addall, string _allDataName )
        {
            DataSet dsData = MessageHandler.getCustomQuery( _siteid, _queryid, _queryversion, _languagecode, _htbindvalue );
            ComboBoxBind( _combobox, dsData.Tables[0], _valuemember, _displaymember, _addall, _allDataName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_combobox"></param>
        /// <param name="_dtenumvalue"></param>
        /// <param name="_enumid"></param>
        /// <param name="_isfullname"></param>
        public static void ComboBoxBindEnumValue( System.Windows.Forms.ComboBox _combobox, DataTable _dtenumvalue, string _enumid, bool _isfullname, bool _isAddAllItem )
        {
            //DataRow[] drDataList = _dtenumvalue.Select( "ENUMID = '" + _enumid + "'", "POSITION" );
            DataRow[] drDataList = _dtenumvalue.Select( "ENUMID = '" + _enumid + "'" );
            DataTable dtEnumValue = _dtenumvalue.Clone();
            int iDefaultSelectIndex = 0;

            // 전체 목록 추가
            if( _isAddAllItem )
            {
                DataRow drAll = dtEnumValue.NewRow();
                drAll["ENUMVALUEFULLNAME"] = "전체";
                drAll["ENUMVALUENAME"] = "전체";
                drAll["ENUMVALUE"] = "";
                dtEnumValue.Rows.InsertAt( drAll, 0 );
            }

            if( drDataList != null )
            {
                for( int i = 0; i < drDataList.Length; i++ )
                {
                    dtEnumValue.ImportRow( drDataList[i] );
                }
            }

            _combobox.DataSource = null;
            _combobox.DataSource = dtEnumValue;
            if( _isfullname )
            {
                _combobox.DisplayMember = "ENUMVALUEFULLNAME";
            }
            else
            {
                _combobox.DisplayMember = "ENUMVALUENAME";
            }
            _combobox.ValueMember = "ENUMVALUE";

            // 기본값 설정
            for( int i = 0; i < dtEnumValue.Rows.Count; i++ )
            {
                if( dtEnumValue.Rows[i]["DEFAULTFLAG"].ToString() == "Yes" )
                {
                    iDefaultSelectIndex = i;
                    break;
                }
            }

            if (dtEnumValue.Rows.Count > 0)
            {
                _combobox.SelectedIndex = iDefaultSelectIndex;
                _combobox.DropDownStyle = ComboBoxStyle.DropDownList;
                _combobox.FlatStyle = FlatStyle.Popup;
            }

        }


        public static void ComboBoxEnumValueAddItem( System.Windows.Forms.ComboBox _combobox, int _insertrow, string _name, string _value, string _fullname )
        {
            DataTable dtEnumValue = ( DataTable )_combobox.DataSource;
            DataRow drAddItem = dtEnumValue.NewRow();
            drAddItem["ENUMVALUEFULLNAME"] = _fullname;
            drAddItem["ENUMVALUENAME"] = _name;
            drAddItem["ENUMVALUE"] = _value;
            dtEnumValue.Rows.InsertAt( drAddItem, _insertrow );
        }

        public static void SetAutoCompleteQuery( System.Windows.Forms.TextBox _textbox, string _siteid, string _queryid, string _queryversion, string _languagecode, Hashtable _htbindvalue, string _columnname )
        {
            DataSet dsResult = MessageHandler.getCustomQuery( _siteid, _queryid, _queryversion, _languagecode, _htbindvalue );
            if( dsResult.Tables[0] != null )
            {
                AutoCompleteStringCollection dcData = new AutoCompleteStringCollection();
                if( dsResult.Tables[0].Columns.Contains( _columnname ) )
                {
                    for( int i = 0; i < dsResult.Tables[0].Rows.Count; i++ )
                    {
                        DataRow drData = dsResult.Tables[0].Rows[i];
                        dcData.Add( drData[_columnname].ToString() );
                    }

                    _textbox.AutoCompleteCustomSource = dcData;
                    _textbox.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    _textbox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                }
            }
        }

        public static void setDataGridView( System.Windows.Forms.DataGridView _datagrid, DataTable _dt, bool _autowidthfit )
        {
            _datagrid.Rows.Clear();

            if( _datagrid.Columns.Count > 0 )
            {
                for( int iRow = 0; iRow < _dt.Rows.Count; iRow++ )
                {
                    _datagrid.Rows.Add();
                    DataGridViewRow drData = _datagrid.Rows[iRow];

                    for( int iCol = 0; iCol < _datagrid.Columns.Count; iCol++ )
                    {
                        string sColumnName = _datagrid.Columns[iCol].Name;
                        if( _dt.Columns.Contains( sColumnName ) )
                        {
                            drData.Cells[sColumnName].Value = _dt.Rows[iRow][sColumnName];
                        }
                    }
                }
            }
            else
            {
                _datagrid.DataSource = _dt;
            }

            if( _autowidthfit )
            {
                _datagrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                /*
                for (int i = 0; i < _datagrid.Columns.Count - 1; i++)
                {
                    _datagrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                _datagrid.Columns[_datagrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

                for (int i = 0; i < _datagrid.Columns.Count; i++)
                {
                    int colw = _datagrid.Columns[i].Width;
                    _datagrid.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                    _datagrid.Columns[i].Width = colw;
                }
                */
            }


        }



    }
}
