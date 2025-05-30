using KR.MBE.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Handler;
using static KR.MBE.UI.ControlUtil.GridColumns;
using KR.MBE.CommonLibrary.Utils;

namespace KR.MBE.UI.ControlUtil.CustomControl
{
    public partial class itierGrid : DataGridView
    {
        #region Variables
        string m_strSiteID;            // SiteID
        int m_currentRow = -1;
        int m_preCurrentRow = -1;
        string m_currentRowPK = "";
        Hashtable m_htDefaultValue;     // Grid Column의 DefaultValue 정보
        ArrayList m_arrListPK;          // 해당 테이블의 PK Column 정보
        ArrayList m_arrListNotNull;
        DataTable m_dtGridDetail = null;       // GridID로 정의된 Grid Deatil 정보
        DataTable m_dtEnumList = null;         // EnumList 정보
        DataGridView m_dgvEditGrid = null;     // EditGrid 연계
        bool m_isDataBinding = false;

        CheckBox chkAll = new CheckBox();

        #endregion

        #region Constants
        static Color SELECTIONBACKCOLOR = Color.Teal;
        #endregion

        #region PROPERTIES

        [Category( "CustomData" ), Description( "엔터키 Next Cell Focus 이동 여부" )]
        public bool EnterKeyNextCell { set; get; } = false;
        public int PrevRow
        {
            get
            {
                return (m_preCurrentRow < 0 || m_preCurrentRow >= this.Rows.Count) ? 0 : m_preCurrentRow;
            }
        }

        #endregion

        public itierGrid()
        {
            InitializeComponent();
            base.DoubleBuffered = true;
        }

        public void InitGrid( string sSiteID, DataTable dtGridData, DataTable dtEnumData )
        {
            m_dtGridDetail = dtGridData;
            m_dtEnumList = dtEnumData;
            InitGrid(sSiteID, this.Name);
        }

        private void InitGrid( string sSiteID, string sGridID )
        {
            m_strSiteID = sSiteID;
            InitDefaultGridStyle();

            m_dtGridDetail = GetGridInformation(sSiteID, sGridID);
            m_htDefaultValue = GetDefaultValue();
            m_arrListPK = GetPrimaryKeyList();
            m_arrListNotNull = GetNotNullList();

            if( m_dtGridDetail?.Rows?.Count <= 0 )
            {
                MessageBox.Show( "Not Found GridDetail Information !", "Warning" ); // USER-535
            }
            else
            {
                InitGridColumn();
            }
        }

        /// <summary>
        /// Grid 기준정보 가져오기
        /// </summary>
        /// <param name="sSiteID"></param>
        /// <param name="sGridID"></param>
        /// <returns></returns>
        private DataTable GetGridInformation( String sSiteID, String sGridID )
        {
            DataTable dtReturn = m_dtGridDetail;
            if( m_dtGridDetail == null )
            {
                Hashtable htParameter = new Hashtable();
                htParameter.Add( "SITEID", sSiteID);
                htParameter.Add( "GRIDID", sGridID );
                DataSet dsReturn = MessageHandler.getCustomQuery( m_strSiteID, "GetGridInit", "00001", Constant.LanguageCode.LC_KOREAN, htParameter );
                dtReturn = dsReturn.Tables["_REPLYDATA"];
            }
            return dtReturn;
        }

        private Hashtable GetDefaultValue()
        {
            Hashtable htReturn = new Hashtable();
            if( m_dtGridDetail != null )
            {
                for( int i = 0; i < m_dtGridDetail.Rows.Count; i++ )
                {
                    string sDefaultValue = m_dtGridDetail.Rows[i]["DEFAULTVALUE"].ToString();
                    switch( sDefaultValue )
                    {
                        case "[SITEID]":
                            htReturn.Add( m_dtGridDetail.Rows[i]["GRIDCOLUMNID"], m_strSiteID );
                            break;
                        default:
                            htReturn.Add( m_dtGridDetail.Rows[i]["GRIDCOLUMNID"], sDefaultValue );
                            break;
                    }

                }
            }
            return htReturn;
        }
        private ArrayList GetPrimaryKeyList()
        {
            ArrayList arrListReturn = new ArrayList();
            if( m_dtGridDetail != null )
            {
                for( int i = 0; i < m_dtGridDetail.Rows.Count; i++ )
                {
                    string sPK = m_dtGridDetail.Rows[i]["PRIMARYKEYFLAG"].ToString();
                    if( sPK == "Yes" )
                    {
                        arrListReturn.Add( m_dtGridDetail.Rows[i]["GRIDCOLUMNID"] );
                    }
                }
            }
            return arrListReturn;
        }
        private ArrayList GetNotNullList()
        {
            ArrayList arrListReturn = new ArrayList();
            if( m_dtGridDetail != null && m_dtGridDetail.Columns.Contains( "NOTNULLFLAG" ) )
            {
                for( int i = 0; i < m_dtGridDetail.Rows.Count; i++ )
                {
                    string sPK = m_dtGridDetail.Rows[i]["NOTNULLFLAG"].ToString();
                    if( sPK == "Yes" )
                    {
                        arrListReturn.Add( m_dtGridDetail.Rows[i]["GRIDCOLUMNID"] );
                    }
                }
            }
            return arrListReturn;
        }

        /// <summary>
        /// Grid 의 기본 속성 설정
        /// </summary>
        public void InitDefaultGridStyle()
        {

            this.AutoGenerateColumns = false;                                   // Column 자동 생성 불가
            this.AllowUserToAddRows = false;                                    // 추가행 표시 안함.
            this.RowHeadersVisible = true;                                      // RowHeader 숨김
            this.AllowUserToResizeRows = false;                                 // RowHeight Resize 불가
            this.EnableHeadersVisualStyles = false;                             // 헤더셀 스타일 변경 가능.
            this.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;    // 컬럼의 헤더텍스트 가운데정렬.

            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;

            this.SelectionMode = DataGridViewSelectionMode.CellSelect;       // 셀을 선택하면 하나의 셀이 선택됨.

            this.BackgroundColor = Color.White;
            this.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb( 255, 211, 220, 233 );
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            /*
            int lColumnHeadersHeight = ConvertUtil.ObjectToInt(m_dtGridDetail.Rows[0]["HEADERCOLUMNSIZE"], 25);
            if (lColumnHeadersHeight == 0) lColumnHeadersHeight = 25;

            this.ColumnHeadersHeight = lColumnHeadersHeight;
            */
            this.ColumnHeadersHeight = 25;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            this.EditMode = DataGridViewEditMode.EditOnEnter;
            //this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 2021.12.21 By SH Jung DataSource = dtSheet 오래 걸리는 버그로 인해 아래 속성 주석처리
            // - DataGridViewAutoSizeRowsMode.None 이여야 버그 없음)
            //this.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            this.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler( Grid_EditingControlShowing );
            this.DataError += new DataGridViewDataErrorEventHandler( Grid_DataError );
            this.RowPostPaint += new DataGridViewRowPostPaintEventHandler( Grid_RowPostPaint );
            this.CurrentCellDirtyStateChanged += new EventHandler( Grid_CurrentCellDirtyStateChanged );
            this.CellValueChanged += new DataGridViewCellEventHandler( Grid_CellValueChanged );
            this.CellPainting += new DataGridViewCellPaintingEventHandler( Grid_CellPainting );
            this.CellBeginEdit += new DataGridViewCellCancelEventHandler( Grid_CellBeginEdit );
            this.MouseDown += new MouseEventHandler( Grid_MouseDown );
            //this.RowEnter += new DataGridViewCellEventHandler( Grid_RowEnter );  // OnRowEnter 로 처리

            //this.CellMouseClick += new DataGridViewCellMouseEventHandler( Grid_CellMouseClick );// 2021.10.14 smkang
            //this.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler( Grid_ColumnHeaderMouseClick ); // 2021.11.04 smkang

            #region DataGridView Row Indicator 표시 안하기
            this.RowPrePaint += new DataGridViewRowPrePaintEventHandler( Grid_RowPrePaint );
            //this.CellFormatting += new DataGridViewCellFormattingEventHandler( Grid_CellFormatting );
            #endregion

            this.MultiSelect = false;                                           // Single Row 선택

            //this.ReadOnly = true;                                               // 읽기 전용으로 함.
            //this.ClearSelection();                                              // 초기화할 때 선택된 셀을 없게 함.
            //this.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb( 222, 134, 244 );  // 헤더셀 색상을 보라색으로.
            //this.DataError += new DataGridViewDataErrorEventHandler(Event_DataError);
            this.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb( 255, 237, 244, 254 );

            // RowHeader Width AutoSize
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToFirstHeader;

            this.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.LightBlue;

        }

        private void Grid_CellMouseClick( object sender, DataGridViewCellMouseEventArgs e )
        {
            if( this.CurrentRow == null || this.CurrentRow.Index < 0 )
            {
                return;
            }

            if( e.Button == MouseButtons.Left && ( ModifierKeys & Keys.Control ) == Keys.Control )
            {
                if( this.Columns.Contains( Grid.ColumnName._CHK ) )
                {
                    if( this[Grid.ColumnName._CHK, this.CurrentRow.Index].Value.ToString() == Constant.FlagYesOrNo.Yes )
                    {
                        this[Grid.ColumnName._CHK, this.CurrentRow.Index].Value = Constant.FlagYesOrNo.No;
                    }
                    else
                    {
                        this[Grid.ColumnName._CHK, this.CurrentRow.Index].Value = Constant.FlagYesOrNo.Yes;
                    }
                }
            }
        }

        private void ItierGrid_CellBeginEdit( object sender, DataGridViewCellCancelEventArgs e )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitGridColumn()
        {
            if( m_dtGridDetail == null )
            {
                MessageBox.Show( "Not Found GridDetail Information !", "Warning" ); //USER-535
            }
            else
            {
                #region Grid Header 및 공통부분
                if( m_dtGridDetail.Rows[0]["CHECKBOX"].ToString() == "Yes" )
                {
                    if( m_dtGridDetail.Columns.Contains( Grid.ColumnName._CHK ) == false )
                    {
                        DataGridViewCheckBoxColumn dgvColumn = GridColumns.GetYesNoCustomCheckBox();
                        dgvColumn.HeaderText = "V";
                        dgvColumn.Name = Grid.ColumnName._CHK;
                        dgvColumn.Tag = Grid.ColumnName._CHK;
                        dgvColumn.DataPropertyName = Grid.ColumnName._CHK;
                        dgvColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvColumn.Width = 30;
                        //dgvColumn.DisplayIndex = 1;
                        int iCol = this.Columns.Add( dgvColumn );
                        this.Columns[iCol].Frozen = true;
                    }
                }
                if( m_dtGridDetail.Rows[0]["ROWSTATUS"].ToString() == "Yes" )
                {
                    // _ROWSTATUS
                    if( this.Columns.Contains( Grid.ColumnName._ROWSTATUS ) == false )
                    {
                        DataGridViewTextBoxColumn dgvColumn = GridColumns.GetText();
                        dgvColumn.Name = Grid.ColumnName._ROWSTATUS;
                        //dgvColumn.HeaderText = Grid.ColumnName._ROWSTATUS;
                        dgvColumn.Tag = Grid.ColumnName._ROWSTATUS;
                        dgvColumn.DataPropertyName = Grid.ColumnName._ROWSTATUS;
                        dgvColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvColumn.Visible = false;
                        dgvColumn.ReadOnly = true;
                        dgvColumn.Width = 30;
                        //dgvColumn.DisplayIndex = 2;
                        int iCol = this.Columns.Add( dgvColumn );
                        this.Columns[iCol].Frozen = true;
                    }
                    // _ROWSTATUSIMAGE
                    if( this.Columns.Contains( Grid.ColumnName._ROWSTATUSIMAGE ) == false )
                    {
                        DataGridViewImageColumn dgvColumn = new DataGridViewImageColumn();
                        dgvColumn.Name = Grid.ColumnName._ROWSTATUSIMAGE;
                        dgvColumn.HeaderText = "";
                        dgvColumn.Tag = Grid.ColumnName._ROWSTATUSIMAGE;
                        dgvColumn.DataPropertyName = Grid.ColumnName._ROWSTATUSIMAGE;
                        dgvColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        dgvColumn.DefaultCellStyle.NullValue = null;
                        dgvColumn.Visible = true;
                        dgvColumn.ReadOnly = true;
                        dgvColumn.ImageLayout = DataGridViewImageCellLayout.Normal;
                        //dgvColumn.ValuesAreIcons = true;
                        dgvColumn.ValueType = typeof( System.Drawing.Image );
                        dgvColumn.Width = 35;
                        //dgvColumn.DisplayIndex = 3;
                        int iCol = this.Columns.Add( dgvColumn );
                        this.Columns[iCol].Frozen = true;
                    }

                }
                #endregion


                for( int i = 0; i < m_dtGridDetail.Rows.Count; i++ )
                {
                    DataRow drData = m_dtGridDetail.Rows[i];    // 받아온 데이터테이블의 로우.

                    // 예약된 컬럼 항목은 GridDetail에 입력을 해도 처리 안되도록 함. 
                    if( ( drData["GRIDCOLUMNID"].ToString().Equals( Grid.ColumnName._CHK ) ) ||
                        ( drData["GRIDCOLUMNID"].ToString().Equals( Grid.ColumnName._ROWSTATUS ) ) ||
                        ( drData["GRIDCOLUMNID"].ToString().Equals( Grid.ColumnName._ROWSTATUSIMAGE ) ) )
                    {
                        continue;
                    }

                    DataGridViewColumn dgvAddColumn = null;     // Grid에 추가할 컬럼.

                    #region CELLTYPE
                    switch( drData["CELLTYPE"].ToString() )      // CELLTYPE
                    {
                        case "ActiveState":
                            dgvAddColumn = GridColumns.GetCustomCheckBox( "Active", "Inactive" );
                            break;
                        case "Button":
                            dgvAddColumn = GridColumns.GetButton( drData["GRIDCOLUMNNAME"].ToString() );
                            break;
                        case "TrueFalse":
                            dgvAddColumn = GridColumns.GetCustomCheckBox( "True", "False" );
                            break;
                        case "YesNo":
                            dgvAddColumn = GridColumns.GetYesNoCustomCheckBox();
                            break;
                        case "ComboBox":
                            dgvAddColumn = GridColumns.GetCustomComboBox( m_dtEnumList, "", drData["EDITFLAG"].ToString(), true );
                            break;
                        case "EnumComboBox":
                            dgvAddColumn = GridColumns.GetCustomComboBox( m_dtEnumList, drData["COMBOENUMID"].ToString(), drData["EDITFLAG"].ToString(), true );
                            break;
                        case "EnumComboBoxNoBlank":
                            dgvAddColumn = GridColumns.GetCustomComboBox( m_dtEnumList, drData["COMBOENUMID"].ToString(), drData["EDITFLAG"].ToString(), false );
                            break;
                        case "MultiComboBox":
                            dgvAddColumn = GridColumns.GetMultiColumnCombo();
                            break;
                        case "Number":
                            dgvAddColumn = GridColumns.GetNumber( 0 );
                            break;
                        case "Double":
                            dgvAddColumn = GridColumns.GetNumber( 1 );
                            break;
                        case "Double2":
                            dgvAddColumn = GridColumns.GetNumber( 2 );
                            break;
                        case "Double3":
                            dgvAddColumn = GridColumns.GetNumber( 3 );
                            break;
                        case "Date":
                            dgvAddColumn = GridColumns.GetCalendarDate();
                            break;
                        case "DateTime":
                            //dgvAddColumn = GridColumns.GetMaskedText("0000-00-00 00:00:00");
                            dgvAddColumn = GridColumns.GetDateTime();
                            break;
                        case "RichText":
                            dgvAddColumn = GridColumns.GetRichText();
                            break;
                        case "Percent":
                            dgvAddColumn = GridColumns.GetPercent();
                            break;
                        case "Encrypt":
                            dgvAddColumn = GridColumns.GetEncrypt();
                            break;
                        default:
                            dgvAddColumn = GridColumns.GetText();
                            break;
                    }
                    #endregion

                    dgvAddColumn.Name = drData["GRIDCOLUMNID"].ToString();                              // GRIDCOLUMNID
                    dgvAddColumn.HeaderText = drData["GRIDCOLUMNNAME"].ToString();                      // GRIDCOLUMNNAME
                    dgvAddColumn.ToolTipText = drData["DESCRIPTION"].ToString();                        // DESCRIPTION
                    dgvAddColumn.Width = Convert.ToInt32( drData["GRIDCOLUMNSIZE"].ToString() );        // GRIDCOLUMNSIZE
                    dgvAddColumn.Visible = drData["VISIBLEFLAG"].ToString() == "Yes" ? true : false;    // VISIBLE
                    //dgvAddColumn.ReadOnly = drData["EDITFLAG"].ToString() == "Yes" ? false : true;      // EDITIBLE
                    if (Constant.FlagYesOrNo.Yes.Equals(drData["EDITFLAG"].ToString()))                 // EDITIBLE
                    {
                        dgvAddColumn.ReadOnly = false;
                        //dgvAddColumn.DefaultCellStyle.ForeColor = Color.Black;
                    }
                    else
                    {
                        dgvAddColumn.ReadOnly = true;
                        //dgvAddColumn.DefaultCellStyle.ForeColor = Color.Gray;
                    }

                    //dgvAddColumn.DisplayIndex = Convert.ToInt32(drData["POSITION"].ToString());         // POSITION
                    dgvAddColumn.DataPropertyName = drData["GRIDCOLUMNID"].ToString();
                    //dgvAddColumn.ValueType 설정

                    #region ALIGNMENT 
                    switch ( drData["ALIGN"].ToString() )  //ALIGNMENT 
                    {
                        case "Right":
                            dgvAddColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                            break;
                        case "Center":
                            dgvAddColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                            break;
                        default:
                            dgvAddColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                            break;
                    }
                    #endregion

                    this.Columns.Add( dgvAddColumn );

                }

                // DataSource 값이 null 아니도록 처리한다.
                SetDataTable();

                // 2021.10.12 sortflag 추가 
                // Column Header 클릭시 정렬될때 수동으로 만들어준 Grid Style 이 깨짐. _CHK (CheckBox) 항목 초기화됨.
                for( int iCol = 0; iCol < this.Columns.Count; iCol++ )
                {
                    string colName = this.Columns[iCol].Name;
                    this.Columns[iCol].SortMode = DataGridViewColumnSortMode.NotSortable;

                    if( m_dtGridDetail != null &&
                        colName != Grid.ColumnName._CHK &&
                        colName != Grid.ColumnName._ROWSTATUS &&
                        colName != Grid.ColumnName._ROWSTATUSIMAGE )
                    {
                        string sortFlag = m_dtGridDetail.Select( $"GRIDCOLUMNID = \'{colName}\'" ).CopyToDataTable<DataRow>().Rows[0]["SORTFLAG"].ToString();
                        if( sortFlag == Constant.FlagYesOrNo.Yes )
                        {
                            this.Columns[iCol].SortMode = DataGridViewColumnSortMode.Automatic;
                            this.Columns[iCol].HeaderCell.Style.Font = new Font( this.Font, FontStyle.Underline );
                        }
                    }
                }

                // 2021.11.04
                //for (int iCol = 0; iCol < this.Columns.Count; iCol++)
                //{
                //    string colName = this.Columns[iCol].Name;
                //    this.Columns[iCol].SortMode = DataGridViewColumnSortMode.NotSortable;
                //
                //    if (m_dtGridDetail != null &&
                //        colName != Grid.ColumnName._CHK &&
                //        colName != Grid.ColumnName._ROWSTATUS &&
                //        colName != Grid.ColumnName._ROWSTATUSIMAGE)
                //    {
                //        string sortFlag = m_dtGridDetail.Select($"GRIDCOLUMNID = \'{colName}\'").CopyToDataTable<DataRow>().Rows[0]["SORTFLAG"].ToString();
                //        if (sortFlag == Constant.FlagYesOrNo.Yes)
                //        {
                //            this.Columns[iCol].HeaderCell.Style.Font = new Font(this.Font, FontStyle.Underline);
                //        }
                //    }
                //}
            }
        }

        public void SetDataTable()
        {
            DataTable dt = new DataTable(); // 담을 객체
            for( int iCol = 0; iCol < this.Columns.Count; iCol++ )
            {
                if( this.Columns[iCol].ValueType == null )
                {
                    dt.Columns.Add( this.Columns[iCol].Name );
                }
                else
                {
                    dt.Columns.Add( this.Columns[iCol].Name, this.Columns[iCol].ValueType );
                }
            }//컬럼 생성

            for( int iRow = 0; iRow < this.Rows.Count; iRow++ )
            {
                DataRow dr = dt.NewRow();
                for( int iCol = 0; iCol < dt.Columns.Count; iCol++ )
                {
                    dr[iCol] = this.Rows[iRow].Cells[iCol].Value;
                }
                dt.Rows.Add( dr );

            } //데이터 삽입
            this.DataSource = dt;

            // p_ViewGrid Column 에 DataField 값을 재 설정한다.
            //for (int iCol = 0; iCol < this.Columns.Count; iCol++)
            //{
            //    this.Columns[iCol].DataPropertyName = this.Columns[iCol].Name;
            //}

        }

        /// <summary>
        /// 조회를 한 후 현재까지 편집된 행의 개수입니다.
        /// </summary>
        /// <returns></returns>
        public int GetUpdateRowCount()
        {
            int count = 0;
            for( int i = 0; i < this.Rows.Count; ++i )
            {
                if( !string.IsNullOrEmpty( this["_ROWSTATUS", i].Value.ToString() ) )
                {
                    count++;
                }
            }

            return count;
        }

        /// <summary>
        /// 기본키 문자열을 가져오는 함수입니다.
        /// </summary>
        /// <returns> DB에서 PrimaryKey = "Yes"인 컬럼ID를 문자열로 반환 </returns>
        public string GetPKString()
        {
            string strRet = "";
            if( m_arrListPK != null )
            {
                for( int i = 0; i < m_arrListPK.Count; ++i )
                {
                    strRet += m_arrListPK[i].ToString();
                    strRet += ",";
                }
                if( m_arrListPK.Count > 0 )
                {
                    strRet = strRet.Remove( strRet.Length - 1 );
                }
            }

            return strRet;
        }

        public string GetNotNullString()
        {
            string strRet = "";
            if( m_arrListNotNull != null )
            {
                for( int i = 0; i < m_arrListNotNull.Count; ++i )
                {
                    strRet += m_arrListNotNull[i].ToString();
                    strRet += ",";
                }
                if( m_arrListNotNull.Count > 0 )
                {
                    strRet = strRet.Remove( strRet.Length - 1 );
                }
            }

            return strRet;
        }

        #region Event Handler


        protected override void OnRowEnter(DataGridViewCellEventArgs e)
        {

            if (m_isDataBinding)
            {
                // DataBinding 중일때는 RowEnter Event 발생안되도록한다.
                return;
            }
            else
            {
                /*
                if( this.m_dgvEditGrid != null )
                {
                    for( int iEditRow = 0; iEditRow < m_dgvEditGrid.Rows.Count; iEditRow++ )
                    {
                        String sColumnID = m_dgvEditGrid.Rows[iEditRow].Cells[Grid.EditColumnName.COLUMNID].Value.ToString();
                        if( this.Columns.Contains( sColumnID ) )
                        {
                            m_dgvEditGrid.Rows[iEditRow].Cells[Grid.EditColumnName.COLUMNVALUE].Value = this.Rows[e.RowIndex].Cells[sColumnID].Value;
                        }
                    }
                }
                */
                m_currentRowPK = "";
                for (int i = 0; i < this.Columns.Count; ++i)
                {
                    if (m_arrListPK != null)
                    {
                        if (m_arrListPK.Contains(this.Columns[i].Name))
                        {
                            m_currentRowPK += this[i, e.RowIndex].Value.ToString();
                        }
                    }
                }

                this.m_preCurrentRow = this.m_currentRow;
                this.m_currentRow = e.RowIndex;
                if (!m_preCurrentRow.Equals(this.m_currentRow))
                {
                    this.OnChangeRowIndex();
                }

                //this.m_currentRow = e.RowIndex;
                //Console.WriteLine( " m_currentRow : " + m_currentRow );

                base.OnRowEnter(e);

            }

        }

/*
        private void Grid_RowEnter( object sender, DataGridViewCellEventArgs e )
        {
            //if( this.m_dgvEditGrid != null )
            //{
            //    for( int iEditRow = 0; iEditRow < m_dgvEditGrid.Rows.Count; iEditRow++ )
            //    {
            //        String sColumnID = m_dgvEditGrid.Rows[iEditRow].Cells[Grid.EditColumnName.COLUMNID].Value.ToString();
            //        if( this.Columns.Contains( sColumnID ) )
            //        {
            //            m_dgvEditGrid.Rows[iEditRow].Cells[Grid.EditColumnName.COLUMNVALUE].Value = this.Rows[e.RowIndex].Cells[sColumnID].Value;
            //        }
            //    }
            //}
            m_currentRowPK = "";
            for( int i = 0; i < this.Columns.Count; ++i )
            {
                if( m_arrListPK != null )
                {
                    if( m_arrListPK.Contains( this.Columns[i].Name ) )
                    {
                        m_currentRowPK += this[i, e.RowIndex].Value.ToString();
                    }
                }
            }

            if (!e.RowIndex.Equals(this.m_currentRow))
            {
                this.m_preCurrentRow = this.m_currentRow;
            }
            this.m_currentRow = e.RowIndex;
            //Console.WriteLine( " m_currentRow : " + m_currentRow );
        }
*/

        private static void Event_DataError( object sender, DataGridViewDataErrorEventArgs e )
        {
            ;
        }

        private void Grid_EditingControlShowing( object sender, DataGridViewEditingControlShowingEventArgs e )
        {

            /*
            if( e.Control.GetType() == typeof( DataGridViewComboBoxEditingControl ) )
            {
                ComboBox oCombo = ( ComboBox )e.Control;
                //oCombo.DropDownStyle = ComboBoxStyle.DropDown;
                //oCombo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
            if( e.Control.GetType() == typeof( MultiColumnComboEditingControl ) )
            {
                ComboBox oCombo = ( ComboBox )e.Control;
                oCombo.DropDownStyle = ComboBoxStyle.DropDown;
                oCombo.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            }
            */

            this.EditingControl.ImeMode = GetIMEMode( this.CurrentCell.OwningColumn.Name );

            e.Control.KeyPress -= new KeyPressEventHandler( Grid_KeyPress );
            e.Control.KeyPress += new KeyPressEventHandler( Grid_KeyPress );
        }

        private void Grid_DataError( object sender, DataGridViewDataErrorEventArgs e )
        {
            /*
            DataGridView Grid = (DataGridView)sender;
            DataGridViewComboBoxEditingControl ctl = Grid.EditingControl as DataGridViewComboBoxEditingControl;

            DataGridViewCell oCell = Grid.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (oCell is DataGridViewComboBoxCell)
            {
                if (ctl.SelectedIndex == -1)
                {
                    MessageBox.Show("목록에 존재하지 않는 값입니다!\r\nComboBox에서 선택해 주시기 바랍니다.");
                }
            }
            */
            //e.ThrowException = true;
        }

        private void Grid_RowPostPaint( object sender, DataGridViewRowPostPaintEventArgs e )
        {
            if( this.RowHeadersVisible )
            {
                //Console.WriteLine( "Grid_RowPostPaint " );

                // RowPointPaint 이벤트 핸들러
                // 행헤더 열영역에 행번호를 보여주기 위해 장방형으로 처리
                Rectangle rect = new Rectangle( e.RowBounds.Location.X, e.RowBounds.Location.Y, this.RowHeadersWidth - 4, e.RowBounds.Height );
                //위에서 생성된 장방형내에 행번호를 보여주고 폰트색상 및 배경을 설정
                TextRenderer.DrawText( e.Graphics,
                    ( e.RowIndex + 1 ).ToString(),
                    this.RowHeadersDefaultCellStyle.Font,
                    rect,
                    this.RowHeadersDefaultCellStyle.ForeColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.Right );
            }
        }

        private void Grid_CurrentCellDirtyStateChanged( object sender, EventArgs e )
        {

            if( this.IsCurrentCellDirty )
            {
                this.CommitEdit( DataGridViewDataErrorContexts.Commit );
                this.EndEdit();

                // 2020.04.24 강성묵 추가.
                if( this.EditingControl is TextBox )
                {
                    TextBox temp = ( TextBox )this.EditingControl;
                    if( temp.SelectionLength != 0 )
                    {
                        temp.SelectionStart = this.CurrentCell.Value.ToString().Length + 1;
                        temp.SelectionLength = 0;
                    }

                }
                
            }
        }


        // 2021.09.29 smkang 생성
        // 2021.10.13 smkang 수정
        // 체크박스 선택시 로우색상 변경
        private Dictionary<int, List<Color>> prevBackColor = new Dictionary<int, List<Color>>();
        private Dictionary<int, List<Color>> prevForeColor = new Dictionary<int, List<Color>>();
        private void DisplaySelectedRow( int rowIndex, int chkIndex )
        {

            if( this.GetCellValue( rowIndex, chkIndex ) == Constant.FlagYesOrNo.Yes )
            {
                if( !prevBackColor.ContainsKey( rowIndex ) )
                {
                    prevBackColor.Add( rowIndex, new List<Color>() );
                }
                if( !prevForeColor.ContainsKey( rowIndex ) )
                {
                    prevForeColor.Add( rowIndex, new List<Color>() );
                }
                for( int i = 0; i < this.Columns.Count; i++ )
                {
                    prevBackColor[rowIndex].Add( this.Rows[rowIndex].Cells[i].Style.BackColor );
                    prevForeColor[rowIndex].Add( this.Rows[rowIndex].Cells[i].Style.ForeColor );
                    //this.Rows[rowIndex].Cells[i].Style.BackColor = this.DefaultCellStyle.SelectionBackColor;
                    this.Rows[rowIndex].Cells[i].Style.BackColor = SELECTIONBACKCOLOR;
                    this.Rows[rowIndex].Cells[i].Style.ForeColor = this.DefaultCellStyle.SelectionForeColor;
                }
            }
            else
            {
                if( !prevBackColor.ContainsKey( rowIndex ) || !prevForeColor.ContainsKey( rowIndex ) )
                {
                    return;
                }
                for( int i = 0; i < this.Columns.Count; i++ )
                {
                    this.Rows[rowIndex].Cells[i].Style.BackColor = prevBackColor[rowIndex][i];
                    this.Rows[rowIndex].Cells[i].Style.ForeColor = prevForeColor[rowIndex][i];
                }
                prevBackColor.Remove( rowIndex );
                prevForeColor.Remove( rowIndex );
            }
        }

        private void Grid_CellValueChanged( object sender, DataGridViewCellEventArgs e )
        {
            var dgv = sender as itierGrid;
            if( dgv.Columns.Contains( Grid.ColumnName._ROWSTATUS ) &&
                ( e.ColumnIndex == dgv.Columns[Grid.ColumnName._ROWSTATUS].Index || e.ColumnIndex == dgv.Columns[Grid.ColumnName._ROWSTATUSIMAGE].Index ) )
            {
                return;
            }

            // 2021.09.30 smkang
            // 2021.10.28 shjung
            // RowPrePaint 이벤트로 이동
            //if( this.Columns.Contains( Grid.ColumnName._CHK ) && e.ColumnIndex == this.Columns[Grid.ColumnName._CHK].Index )
            //{
            //    DisplaySelectedRow( e.RowIndex, e.ColumnIndex );
            //}

            CheckRowStatus( e.RowIndex );

            // 현재 Value값이 Number Type에 맞으면 수정 안하고 맞는경우만 수정함.

            if( this.CurrentCell == null )
            {
                return;
            }

            if( m_dtGridDetail != null )
            {
                string sColumnID = this.CurrentCell.OwningColumn.Name;
                DataRow[] drData = m_dtGridDetail.Select( "GRIDCOLUMNID='" + sColumnID + "'" );

                if( drData != null && drData.Length > 0 )
                {
                    switch( drData[0]["CELLTYPE"].ToString() )
                    {
                        case "PhoneNumber":
                            this.EditingControl.Text = Util.changePhoneNumber( this.CurrentCell.Value.ToString() );
                            ( this.EditingControl as TextBox ).SelectionStart = this.EditingControl.Text.Length;
                            ( this.EditingControl as TextBox ).ScrollToCaret();
                            if( this.CurrentCell.Value.ToString() != this.EditingControl.Text )
                            {
                                this.CurrentCell.Value = this.EditingControl.Text;
                            }
                            /*
                            if( this.CurrentCell.Value.ToString() != this.EditingControl.Text.Replace( "-", "" ) )
                            {
                                this.CurrentCell.Value = this.EditingControl.Text.Replace( "-", "" );
                            }
                            */
                            break;
                        case "Number":
                            MakeDecimal( 0 );
                            break;
                        case "Double":
                            MakeDecimal( 1 );
                            break;
                        case "Double2":
                            MakeDecimal( 2 );
                            break;
                        case "Double3":
                            MakeDecimal( 3 );
                            break;
                        default:
                            break;
                    }
                }
            }
        }


        // 2020-03-11 강성묵 추가.
        private void MakeDecimal( int decimalPoint )
        {
            if( EditingControl == null ) return;
            string numText = this.CurrentCell.Value.ToString();

            //string numText = this.EditingControl.Text.ToString();
            if( numText.Length > 0 )
            {
                if( numText.Substring( numText.Length - 1 ) != "." )
                {
                    if( numText.IndexOf( "." ) > 0 )
                    {
                        String sDecimalPointFormat = String.Empty;
                        String sDP = String.Empty;
                        int iCurrentDecimalPoint = numText.Split( '.' )[1].Length;
                        if( iCurrentDecimalPoint > 0 )
                        {
                            if( iCurrentDecimalPoint > decimalPoint )
                            {
                                // 마지막 한자리 짜르기 (5누르면 반올림됨.)
                                numText = numText.Substring( 0, numText.IndexOf( "." ) + 1 + decimalPoint );
                                for( int i = 0; i < decimalPoint; i++ )
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
                            this.EditingControl.Text = String.Format( sDecimalPointFormat, Convert.ToDouble( numText ) );
                        }
                        else
                        {
                            sDecimalPointFormat = "{0:#,##0}";
                            this.EditingControl.Text = String.Format( sDecimalPointFormat, Convert.ToDouble( numText ) );
                        }
                    }
                    else
                    {
                        this.EditingControl.Text = String.Format( "{0:#,##0}", Convert.ToDouble( numText ) );
                    }

                    if( this.EditingControl is TextBox )
                    {
                        ( this.EditingControl as TextBox ).SelectionStart = this.EditingControl.Text.Length;
                        ( this.EditingControl as TextBox ).ScrollToCaret();
                    }
                    if( this.CurrentCell.Value.ToString() != this.EditingControl.Text.Replace( ",", "" ) )
                    {
                        this.CurrentCell.Value = this.EditingControl.Text.Replace( ",", "" );
                    }
                }
            }
        }


        // 2020-03-11 강성묵 추가.
        private void Grid_KeyPress( object sender, KeyPressEventArgs e )
        {
            string sColumnID = this.CurrentCell.OwningColumn.Name;
            if( m_dtGridDetail == null )
            {
                return;
            }
            DataRow[] drData = m_dtGridDetail.Select( "GRIDCOLUMNID='" + sColumnID + "'" );

            if( drData != null && drData.Length > 0 )
            {
                switch( drData[0]["CELLTYPE"].ToString() )
                {
                    case "PhoneNumber":
                        TypingOnlyNumber( sender, e, false, false );
                        break;
                    case "Number":
                        TypingOnlyNumber( sender, e, false, true );
                        break;
                    case "Double":
                        TypingOnlyNumber( sender, e, true, true, 1 );
                        break;
                    case "Double2":
                        TypingOnlyNumber( sender, e, true, true, 2 );
                        break;
                    case "Double3":
                        TypingOnlyNumber( sender, e, true, true, 3 );
                        break;
                    // 2020-03-17 강성묵 추가.
                    case "EnumComboBox":
                        e.Handled = true;
                        break;
                    default:
                        break;
                }
            }
        }

        // 2021-06-10 By SH Jung  숫자입력 Cell에 한글입력을 막기 위해 추가
        private ImeMode GetIMEMode( String sColumnID )
        {
            ImeMode returnImeMode = ImeMode.NoControl;
            if( m_dtGridDetail == null )
            {
                return returnImeMode;
            }
            DataRow[] drData = m_dtGridDetail.Select( "GRIDCOLUMNID='" + sColumnID + "'" );
            if( drData != null && drData.Length > 0 )
            {
                String sCellType = drData[0]["CELLTYPE"].ToString();
                if( sCellType == "Number" || sCellType == "Double" || sCellType == "Double2" || sCellType == "Double3" )
                {
                    returnImeMode = ImeMode.Disable;
                }
            }
            return returnImeMode;
        }


        private void Grid_CellPainting( object sender, DataGridViewCellPaintingEventArgs e )
        {
            if( this.Columns.Contains( Grid.ColumnName._CHK ) )
            {
                if( e.ColumnIndex == 0 && e.RowIndex == -1 )
                {
                    e.PaintBackground( e.ClipBounds, false );

                    Point pt = e.CellBounds.Location;  // where you want the bitmap in the cell

                    int nChkBoxWidth = 15;
                    int nChkBoxHeight = 15;
                    int offsetx = ( e.CellBounds.Width - nChkBoxWidth ) / 2;
                    int offsety = ( e.CellBounds.Height - nChkBoxHeight ) / 2;

                    pt.X += offsetx;
                    pt.Y += offsety;

                    // chkAll 이 이미 만들어져 있으면 위치만 변경함.
                    foreach (Control oControl in this.Controls)
                    {
                        if (oControl.Name == "chkAll")
                        {
                            chkAll = (CheckBox)oControl;
                            break;
                        }
                    }
                    chkAll.Size = new Size( nChkBoxWidth, nChkBoxHeight );
                    chkAll.Location = pt;
                    chkAll.Name = "chkAll";
                    // 2020.10.26 강성묵 수정
                    try
                    {
                        chkAll.CheckedChanged -= AllCheckBox_CheckedChanged;
                    }
                    finally
                    {
                        chkAll.CheckedChanged += new EventHandler( AllCheckBox_CheckedChanged );
                    }
                    this.Controls.Add( chkAll);

                    e.Handled = true;
                }
            }

        }

        private void Grid_CellBeginEdit( object sender, DataGridViewCellCancelEventArgs e )
        {
            if( m_dtGridDetail == null )
            {
                return;
            }
            bool bCancel = false;
            if( m_dtGridDetail.Rows.Count > 0 )
            {
                // 해당 Cell의 Ready Only 처리
                string sColumnID = this.Columns[e.ColumnIndex].DataPropertyName;
                DataRow[] drData = m_dtGridDetail.Select( "GRIDCOLUMNID='" + sColumnID + "'" );
                if( drData != null && drData.Length > 0 )
                {
                    bCancel = drData[0]["EDITFLAG"].ToString() == "Yes" ? false : true;

                    if( drData[0]["PRIMARYKEYFLAG"].ToString() == "Yes" )
                    {
                        bCancel = true;
                        if( this.Columns.Contains( Grid.ColumnName._ROWSTATUS ) )
                        {
                            if( this.Rows[e.RowIndex].Cells[Grid.ColumnName._ROWSTATUS].Value.ToString() == "C" )
                            {
                                bCancel = false;
                            }
                        }
                    }

                }
            }
            e.Cancel = bCancel;
        }

        private void Grid_MouseDown( object sender, MouseEventArgs e )
        {
            if( e.Button == MouseButtons.Right )
            {
                // Use HitTest to resolve the row under the cursor
                HitTestInfo oHitTest = this.HitTest( e.X, e.Y );
                if( ( oHitTest.RowIndex >= 0 ) && ( oHitTest.ColumnIndex >= 0 ) )
                {
                    this.CurrentCell = this[oHitTest.ColumnIndex, oHitTest.RowIndex];
                    this.Rows[oHitTest.RowIndex].Cells[oHitTest.ColumnIndex].Selected = true;
                }
            }
        }

        #region DataGridView Row Indicator 표시 안하기 
        private void Grid_RowPrePaint( object sender, DataGridViewRowPrePaintEventArgs e )
        {
            // 2021.09.30 smkang
            // 2021.10.28 shjung
            // RowPrePaint 에서 처리해야 정렬되어도 같이 처리됨.
            if( this.Columns.Contains( "ORDERTYPE" ) )
            {
                if( this["ORDERTYPE", e.RowIndex].Value.ToString() == "Additional" )
                {
                    for( int j = 0; j < this.Columns.Count; j++ )
                    {
                        if( this[j, e.RowIndex].Style.BackColor != SELECTIONBACKCOLOR )
                        {
                            this[j, e.RowIndex].Style.BackColor = Color.Yellow;
                        }
                    }
                }
            }

            if( this.Columns.Contains( Grid.ColumnName._CHK ) )
            {
                DisplaySelectedRow( e.RowIndex, this.Columns[Grid.ColumnName._CHK].Index );
            }
            /*
            // Row Indeicator 보이지 않기.
            e.PaintCells( e.ClipBounds, DataGridViewPaintParts.All );
            e.PaintHeader( DataGridViewPaintParts.Background
                | DataGridViewPaintParts.Border
                | DataGridViewPaintParts.Focus
                | DataGridViewPaintParts.SelectionBackground
                | DataGridViewPaintParts.ContentForeground );
            e.Handled = true;
            */
        }

        private void Grid_CellFormatting( object sender, DataGridViewCellFormattingEventArgs e )
        {
            //this.Rows[e.RowIndex].HeaderCell.Value = e.RowIndex.ToString();
            //this.Rows[e.RowIndex].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        }
        #endregion

        private void AllCheckBox_CheckedChanged( object sender, EventArgs e )
        {
            if( ( this.CurrentCell != null ) && ( this.Rows.Count > 0 ) && ( this.Columns.Count > 0 ) )
            {
                int iRow = this.CurrentCell.RowIndex;
                int iCol = this.CurrentCell.ColumnIndex;
                this.CurrentCell = null;
                if( this.Columns.Contains( Grid.ColumnName._CHK ) )
                {
                    foreach( DataGridViewRow dgvRow in this.Rows )
                    {
                        if( ( ( CheckBox )sender ).Checked )
                        {
                            dgvRow.Cells[Grid.ColumnName._CHK].Value = ( ( DataGridViewCheckBoxCell )dgvRow.Cells[Grid.ColumnName._CHK] ).TrueValue;
                        }
                        else
                        {
                            dgvRow.Cells[Grid.ColumnName._CHK].Value = ( ( DataGridViewCheckBoxCell )dgvRow.Cells[Grid.ColumnName._CHK] ).FalseValue;
                        }

                        int colIndex = dgvRow.Cells[Grid.ColumnName._CHK].ColumnIndex;
                    }
                }
                //this.GetDataTable().AcceptChanges();
                this.CurrentCell = this[iCol, iRow];
            }
        }
        #endregion

        #region Custom Public Method

        public void InitGridMultiColumnCombo( string columnID, string queryID, string queryVersion, string p_ValueColumn, string p_DisplayColumn, Hashtable htParameter )
        {
            DataSet dsStateObjectList = MessageHandler.getCustomQuery( m_strSiteID, queryID, queryVersion, Constant.LanguageCode.LC_KOREAN, htParameter );
            DataTable dtList = dsStateObjectList.Tables["_REPLYDATA"];

            if( dtList.Rows.Count > 0 )
            {
                InitGridMultiColumnCombo( columnID, dtList, p_ValueColumn, p_DisplayColumn );
            }
        }

        /// <summary>
        /// Grid 특정 Cell 에 Multi Column Combo 설정 (DataTable)   dgvPortDefinition.InitGridCombo("STATECHANGEPOLICY", dt, "STATEOBJECT", "STATEOBJECT");
        // <param name="columnID"></param>
        /// <param name="p_dt"></param>
        /// <param name="p_ValueColumn"></param>
        /// <param name="p_DisplayColumn"></param>
        public void InitGridMultiColumnCombo( string columnID, DataTable p_dt, string p_ValueColumn, string p_DisplayColumn )
        {
            if( p_dt.Rows.Count == 0 )
            {
                DataRow drAll = p_dt.NewRow();
                p_dt.Rows.InsertAt( drAll, 0 );
            }
            else
            {
                // 빈 목록 추가
                DataRow drAll = p_dt.NewRow();
                drAll[p_ValueColumn] = "";
                drAll[p_DisplayColumn] = "";
                p_dt.Rows.InsertAt( drAll, 0 );
            }

            if( this.Columns[columnID].CellType.Name == "MultiColumnComboCell" )
            {
                MultiColumnComboColumn dgvCombo = ( MultiColumnComboColumn )this.Columns[columnID];
                dgvCombo.DataSource = p_dt;
                dgvCombo.ValueMember = p_ValueColumn;
                dgvCombo.DisplayMember = p_DisplayColumn;

                dgvCombo.AutoComplete = true;
                dgvCombo.FlatStyle = FlatStyle.Popup;
                dgvCombo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
        }


        /// <summary>
        /// Grid 특정 Cell 에 Combo 설정 (DataTable)   dgvPortDefinition.InitGridCombo("STATECHANGEPOLICY", "GetStateObjectList", "00001", "STATEOBJECT", "STATEOBJECT", htParameter);
        /// </summary>
        /// <param name="columnID"></param>
        /// <param name="queryID"></param>
        /// <param name="queryVersion"></param>
        /// <param name="p_ValueColumn"></param>
        /// <param name="p_DisplayColumn"></param>
        /// <param name="htParameter"></param>
        public void InitGridComboCell( int rowIndex, string columnID, string queryID, string queryVersion, string p_ValueColumn, string p_DisplayColumn, Hashtable htParameter )
        {
            DataSet dsStateObjectList = MessageHandler.getCustomQuery( m_strSiteID, queryID, queryVersion, Constant.LanguageCode.LC_KOREAN, htParameter );
            DataTable dtList = dsStateObjectList.Tables["_REPLYDATA"];

            //if( dtList.Rows.Count > 0 )
            //{
                InitGridComboCell( rowIndex, columnID, dtList, p_ValueColumn, p_DisplayColumn );
            //}
        }


        /// <summary>
        /// Grid 특정 Column 에 Combo 설정 (DataTable)   dgvPortDefinition.InitGridCombo("STATECHANGEPOLICY", "GetStateObjectList", "00001", "STATEOBJECT", "STATEOBJECT", htParameter);
        /// </summary>
        /// <param name="columnID"></param>
        /// <param name="queryID"></param>
        /// <param name="queryVersion"></param>
        /// <param name="p_ValueColumn"></param>
        /// <param name="p_DisplayColumn"></param>
        /// <param name="htParameter"></param>
        public void InitGridCombo( string columnID, string queryID, string queryVersion, string p_ValueColumn, string p_DisplayColumn, Hashtable htParameter )
        {
            DataSet dsReturn = MessageHandler.getCustomQuery( m_strSiteID, queryID, queryVersion, Constant.LanguageCode.LC_KOREAN, htParameter );

            DataTable dtList = dsReturn.Tables["_REPLYDATA"];

            if (dtList != null)
            {
                if (dtList.Rows.Count > 0)
                {
                    InitGridCombo(columnID, dtList, p_ValueColumn, p_DisplayColumn);
                }
            }
        }


        /// <summary>
        /// Grid 특정 Cell 에 Combo 설정 (DataTable)   dgvPortDefinition.InitGridCombo("STATECHANGEPOLICY", dt, "STATEOBJECT", "STATEOBJECT");
        // <param name="columnID"></param>
        /// <param name="p_dt"></param>
        /// <param name="p_ValueColumn"></param>
        /// <param name="p_DisplayColumn"></param>
        public void InitGridCombo( string columnID, DataTable p_dt, string p_ValueColumn, string p_DisplayColumn )
        {
            if( p_dt.Rows.Count == 0 )
            {
                DataRow drAll = p_dt.NewRow();
                p_dt.Rows.InsertAt( drAll, 0 );
            }
            else
            {
                // 빈 목록 추가
                DataRow drAll = p_dt.NewRow();
                drAll[p_ValueColumn] = "";
                drAll[p_DisplayColumn] = "";
                p_dt.Rows.InsertAt( drAll, 0 );
            }

            if( this.Columns[columnID].CellType.Name == "DataGridViewComboBoxCell" )
            {
                DataGridViewComboBoxColumn dgvCombo = ( DataGridViewComboBoxColumn )this.Columns[columnID];
                dgvCombo.DataSource = p_dt;
                dgvCombo.ValueMember = p_ValueColumn;
                dgvCombo.DisplayMember = p_DisplayColumn;

                dgvCombo.AutoComplete = true;
                dgvCombo.FlatStyle = FlatStyle.Popup;

                if( this.Columns[columnID].ReadOnly )
                {
                    dgvCombo.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                }
                else
                {
                    dgvCombo.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
                }
            }
        }

        /// <summary>
        /// Grid 특정 Cell 에 Combo 설정 (DataTable)   dgvPortDefinition.InitGridCombo("STATECHANGEPOLICY", dt, "STATEOBJECT", "STATEOBJECT");
        // <param name="columnID"></param>
        /// <param name="p_dt"></param>
        /// <param name="p_ValueColumn"></param>
        /// <param name="p_DisplayColumn"></param>
        public void InitGridComboCell( int rowIndex, string columnID, DataTable p_dt, string p_ValueColumn, string p_DisplayColumn )
        {
            DataTable dtList = p_dt.Copy();
            
            // 
            if (dtList.Columns.Contains(p_ValueColumn) == false)
            {
                dtList.Columns.Add(p_ValueColumn);
            }
            if (dtList.Columns.Contains(p_DisplayColumn) == false)
            {
                dtList.Columns.Add(p_DisplayColumn);
            }

            // 빈 목록 추가
            DataRow drAll = dtList.NewRow();
            drAll[p_ValueColumn] = "";
            drAll[p_DisplayColumn] = "";
            dtList.Rows.InsertAt( drAll, 0 );

            DataGridViewComboBoxCell dgvComboCell = new DataGridViewComboBoxCell();
            dgvComboCell.DataSource = null;
            dgvComboCell.DataSource = dtList;
            dgvComboCell.ValueMember = p_ValueColumn;
            dgvComboCell.DisplayMember = p_DisplayColumn;

            dgvComboCell.AutoComplete = true;
            dgvComboCell.FlatStyle = FlatStyle.Popup;

            if( this.Columns[columnID].ReadOnly )
            {
                dgvComboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
            }
            else
            {
                dgvComboCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.ComboBox;
            }

            this.Rows[rowIndex].Cells[columnID] = dgvComboCell;
        }



        public DataTable GetDataTable( bool hasEncryptColumn )
        {
            // 수정된것 반영후 복사된 DataTable을 가지고 CheckBox Value 변환 작업을 한다.
            //( ( DataTable )this.DataSource ).AcceptChanges();
            //DataTable dtReturn = ( ( DataTable )this.DataSource ).Copy();

            //for( int iCol = 0; iCol < this.Columns.Count; iCol++ )
            //{
            //    if( this.Columns[iCol].Tag.ToString() == Grid.ColumnName._CHK )
            //    {
            //        continue;
            //    }

            //    if( this.Columns[iCol] is DataGridViewCheckBoxColumn )
            //    {
            //        string sTrueValue = ( ( DataGridViewCheckBoxColumn )( this.Columns[iCol] ) ).TrueValue.ToString();
            //        string sFalseValue = ( ( DataGridViewCheckBoxColumn )( this.Columns[iCol] ) ).FalseValue.ToString();

            //        if( sTrueValue.Length > 0 )
            //        {
            //            for( int iRow = 0; iRow < this.Rows.Count; iRow++ )
            //            {
            //                string sValue = this.Rows[iRow].Cells[iCol].Value.ToString();
            //                if( sValue == "True" )
            //                {
            //                    dtReturn.Rows[iRow][iCol] = sTrueValue;
            //                }
            //                if( sValue == "False" )
            //                {
            //                    dtReturn.Rows[iRow][iCol] = sFalseValue;
            //                }
            //            }
            //        }
            //    }
            //}
            //return dtReturn;

            // 2021.10.28 shjung
            // 정렬된 DataTable로 처리하기 위해 변경
            DataTable dtSource = ( DataTable )this.DataSource;
            //DataTable dtSource = GetDataGridViewAsDataTable();   // 문제는 없어 보이는데 좀더 검증후 반영예정 
            //DataTable dtReturn = dtSource.Copy();     // Copy() 할때 마지막 수정된 값이 복사가 안되는 버그 존재함.
            DataTable dtReturn = dtSource;    

            // DataGridViewEncryptColumn 존재할경우 해당 Column 처리
            if ( hasEncryptColumn )
            {
                dtReturn.AcceptChanges();
                //dtSource.AcceptChanges();
                //dtReturn = dtSource.Copy();
                for ( int iRow = 0; iRow < this.Rows.Count; iRow++ )
                {
                    for( int iCol = 0; iCol < this.Columns.Count; iCol++ )
                    {
                        if( this.Columns[iCol] is DataGridViewEncryptColumn )
                        {
                            string sValue = this[iCol, iRow].Value.ToString();
                            sValue = AES.getEncryptString( sValue );
                            dtReturn.Rows[iRow][iCol] = sValue;
                        }
                    }
                }
            }

            return dtReturn;
        }

        public DataTable GetDataTable()
        {
            return GetDataTable( false );
        }

        public DataTable GetUpdateDataTable()
        {
            DataTable dtReturn = this.GetDataTable().Clone();
            DataTable dtTransactionDataTable = this.GetDataTable().Copy();

            if (dtTransactionDataTable.Rows.Count > 0)
            {
                if (dtTransactionDataTable.Columns.Contains("_ROWSTATUS"))
                {
                    DataRow[] dataRowArray = dtTransactionDataTable.Select("_ROWSTATUS <> ''");
                    if (dataRowArray.Length > 0)
                    {
                        dtReturn = dataRowArray.CopyToDataTable();
                    }
                }

            }
            return dtReturn;
        }


        /// <summary>
        /// Grid정렬된 DataTable 가져오기     // 2021.10.28 shjung
        /// </summary>
        /// <returns></returns>
        private DataTable GetDataGridViewAsDataTable()
        {
            try
            {
                if( this.ColumnCount == 0 )
                    return null;
                DataTable dtSource = new DataTable();

                //////create columns
                foreach( DataGridViewColumn col in this.Columns )
                {
                    if( col.ValueType == null )
                    {
                        dtSource.Columns.Add( col.Name, typeof( string ) );
                    }
                    else
                    {
                        dtSource.Columns.Add( col.Name, col.ValueType );
                    }
                    dtSource.Columns[col.Name].Caption = col.HeaderText;
                }
                ///////insert row data
                foreach( DataGridViewRow row in this.Rows )
                {
                    DataRow drNewRow = dtSource.NewRow();
                    foreach( DataColumn col in dtSource.Columns )
                    {
                        drNewRow[col.ColumnName] = row.Cells[col.ColumnName].Value;
                    }
                    dtSource.Rows.Add( drNewRow );
                }
                return dtSource;
            }
            catch
            {
                return null;
            }
        }

        public void InsertRowByDataTable()
        {
            int iInsertRow = 0;
            if (this.CurrentRow != null)
            {
                iInsertRow = this.CurrentRow.Index;
            }
            InsertRowByDataTable(iInsertRow, m_htDefaultValue);
        }

        /// <summary>
        /// Grid의 DataTable 기준으로 특정 Row 의 Row 삽입
        /// </summary>
        /// <param name="_insertRow"></param>
        public void InsertRowByDataTable( int iInsertRow )
        {
            InsertRowByDataTable( iInsertRow, m_htDefaultValue );
        }

        public void InsertRowByDataTable( int iInsertRow, Hashtable htDefaultData )
        {
            DataTable dtData = ( DataTable )this.DataSource;
            if( dtData != null )
            {
                DataRow drData = dtData.NewRow();
                if( dtData.Columns.Contains( Grid.ColumnName._ROWSTATUS ) )
                {
                    drData[Grid.ColumnName._ROWSTATUS] = "C";
                }

                if( htDefaultData != null )
                {
                    for( int iCol = 0; iCol < this.Columns.Count; iCol++ )
                    {
                        if( htDefaultData.Contains( this.Columns[iCol].Name ) &&
                            !string.IsNullOrEmpty( htDefaultData[this.Columns[iCol].Name].ToString() ) /* smkang */ )
                        {
                            drData[this.Columns[iCol].Name] = htDefaultData[this.Columns[iCol].Name].ToString();
                        }
                    }

                }
                dtData.Rows.InsertAt( drData, iInsertRow );

                if( dtData.Rows.Count > 0 )
                {
                    SetRowStatus( iInsertRow );
                }
                if( this.Rows.Count > 0 )
                {
                    //this.CurrentCell = this.Rows[iInsertRow].Cells[Columns.GetFirstColumn( DataGridViewElementStates.Displayed ).Index];
                    for( int i = 0; i < m_arrListPK.Count; i++ )
                    {
                        this[m_arrListPK[i].ToString(), iInsertRow].ReadOnly = false;
                    }
                    // 2021-08-05 By SHJung
                    // InsertRow 후에 Edit가능한 Column 에 Focus가도록 처리 
                    // CurrentCell 마지막에 수행하도록, this.focus 추가
                    this.CurrentCell = this.Rows[iInsertRow].Cells[this.GetFirstEditColumnIndex()];
                    this.Focus();
                }
            }
        }

        /// <summary>
        /// 2021-08-05 By SHJung
        /// InsertRow 후에 Edit가능한 Column 에 Focus가도록 처리 
        /// CurrentCell 마지막에 수행하도록, this.focus 추가
        /// </summary>
        /// <returns>Edit 가능한 첫번째 Column Index</returns>
        private int GetFirstEditColumnIndex()
        {
            //DataGridViewColumn returnColumn = Columns.GetFirstColumn(DataGridViewElementStates.Displayed, DataGridViewElementStates.Frozen);
            int iReturn = 0;
            for( int i = 0; i < this.Columns.Count; i++ )
            {
                // 기본 컬럼은 무시
                if( this.Columns[i].Name.Equals( Grid.ColumnName._CHK ) )
                    continue;
                if( this.Columns[i].Name.Equals( Grid.ColumnName._ROWSTATUS ) )
                    continue;
                if( this.Columns[i].Name.Equals( Grid.ColumnName._ROWSTATUSIMAGE ) )
                    continue;

                // Visible 이면서 Edit가능한 Column 
                if ( this.Columns[i].Visible ) 
                {
                    iReturn = i;
                    if (!this.Columns[i].ReadOnly)
                    {
                        iReturn = i;
                        break;
                    }
                }
            }
            return iReturn;
        }

        public bool deleteRowByDataTable( string displayColumn )
        {
            string sSelectRow = string.Empty;
            int iSelectCount = 0;

            DataTable dtData = ( DataTable )this.DataSource;

            // "_CHK 가 존재하는 경우에만 처리함.
            if( dtData.Columns.Contains( Grid.ColumnName._CHK ) )
            {
                // Multi Select
                for( int i = 0; i < this.Rows.Count; i++ )
                {
                    string sChk = this.Rows[i].Cells[Grid.ColumnName._CHK].Value.ToString();

                    if( sChk == Constant.FlagYesOrNo.Yes )
                    {
                        if( sSelectRow != string.Empty )
                        {
                            sSelectRow += ", ";
                        }
                        if( ( displayColumn != null ) && ( displayColumn.Length > 0 ) )
                        {
                            sSelectRow += this.Rows[i].Cells[displayColumn].Value.ToString();
                        }
                        iSelectCount++;
                    }
                }
            }
            else
            {
                // 2020-03-04 강성묵 추가
                if( this.CurrentRow != null )
                {
                    // Single Select
                    if( this.CurrentRow.Index >= 0 )
                    {
                        sSelectRow += this.CurrentRow.Cells[displayColumn].Value.ToString();
                        iSelectCount++;
                    }
                }
            }
            if( iSelectCount == 0 )
            {
                MessageBox.Show("No data selected. Please select the data you want to delete"); // USER-504
                return false;
            }
            else
            {
                string sMessage = "선택한 데이터를 삭제하시겠습니까?";
                if( sSelectRow != string.Empty )
                {
                    sMessage = "선택한 데이터(" + sSelectRow + ") 를 삭제하시겠습니까?";
                }

                if( MessageBox.Show( sMessage, "Info" ,MessageBoxButtons.YesNo ) == DialogResult.Yes )
                {
                    if( dtData.Columns.Contains( Grid.ColumnName._CHK ) )
                    {
                        // 삭제 처리
                        for( int i = this.Rows.Count - 1; i >= 0; i-- )
                        {
                            string sChk = this.Rows[i].Cells[Grid.ColumnName._CHK].Value.ToString();
                            if( sChk == Constant.FlagYesOrNo.Yes )
                            {
                                deleteRowByDataTable( i );
                            }
                        }

                        // Check All Check Box Clear
                        try
                        {
                            chkAll.CheckedChanged -= AllCheckBox_CheckedChanged;
                            chkAll.Checked = false;
                        }
                        finally
                        {
                            chkAll.CheckedChanged += new EventHandler(AllCheckBox_CheckedChanged);
                        }

                    }
                    else
                    {
                        // 2020-03-04 강성묵 추가
                        if( this.CurrentRow != null )
                        {
                            if( this.CurrentRow.Index >= 0 )
                            {
                                deleteRowByDataTable( this.CurrentRow.Index );
                            }
                        }
                    }

                }
                else
                {
                    return false;
                }
            }

            return true;

        }

        /// <summary>
        /// Grid의 DataTable 기준으로 특정 Row 삭제 상태로 변경
        /// </summary>
        /// <param name="deleteRow"></param>
        public void deleteRowByDataTable( int deleteRow )
        {
            DataTable dtData = ( DataTable )this.DataSource;
            if( dtData != null )
            {
                if( dtData.Columns.Contains( Grid.ColumnName._ROWSTATUS ) )
                {
                    string sRowStatus = this.Rows[deleteRow].Cells[Grid.ColumnName._ROWSTATUS].Value.ToString();
                    if( sRowStatus == "C" )
                    {
                        this.Rows.RemoveAt( deleteRow );
                    }
                    else
                    {
                        /* this.GetDataTable()의 Copy()시 _ROWSTATUS 값을 가져오지 못함.
                         * 삭제처리후 다른 셀 이동시 반영되지만 삭제후 바로 저장시 해당 DataTable.Copy()시 "D"값이 사라짐...
                         * DataGridView Cell 값을 변경하면 DataTable에 바로 반영이 안되는듯...
                        this.Rows[deleteRow].Cells[Grid.ColumnName._ROWSTATUS].Value = "D";
                        SetRowStatus( deleteRow );
                        */
                        DataRow drData = ((DataRowView)this.Rows[deleteRow].DataBoundItem).Row;
                        drData[Grid.ColumnName._ROWSTATUS] = "D";
                        SetRowStatus( deleteRow );

                        // 삭제 처리후 UnCheck 처리
                        if( dtData.Columns.Contains( Grid.ColumnName._CHK ) )
                        {
                            if( this.Rows[deleteRow].Cells[Grid.ColumnName._CHK].Value.ToString() == Constant.FlagYesOrNo.Yes )
                            {
                                this.Rows[deleteRow].Cells[Grid.ColumnName._CHK].Value = Constant.FlagYesOrNo.No;
                            }
                        }
                    }
                }

            }
        }

        /// <summary>
        /// Grid의 DataTable 기준으로 Currnet Row 의 데이터는 원래 값으로 원복
        /// </summary>
        public void undoRowByDataTable()
        {
            if( this.CurrentRow != null )        // UndoFlag dgjang
            {
                undoRowByDataTable( this.CurrentRow.Index );
            }
        }

        /// <summary>
        /// Grid의 DataTable 기준으로 특정 Row 의 데이터는 원래 값으로 원복
        /// </summary>
        /// <param name="undoRow"></param>
        public void undoRowByDataTable( int undoRow )
        {
            DataTable dtData = ( DataTable )this.DataSource;
            if( dtData != null )
            {
                if( dtData.Columns.Contains( Grid.ColumnName._ROWSTATUS ) )
                {
                    if( undoRow >= 0 )
                    {
                        string sCurrentRowStatus = this.Rows[undoRow].Cells[Grid.ColumnName._ROWSTATUS].Value.ToString();

                        // 2023.02.16 shjung
                        // 정렬된 itierGrid인 경우 DataSource의 DataTable가 불일치된다.
                        //DataRow drData = dtData.Rows[undoRow];
                        DataRow drData = ((DataRowView)this.Rows[undoRow].DataBoundItem).Row;
                        drData.RejectChanges();
                        if( sCurrentRowStatus == "C" )
                        {
                            // 신규 Row 인경우는 삭제되므로 더이상할 로직이 없음.
                        }
                        else
                        {
                            SetRowStatus( undoRow );
                        }
                    }
                }
            }
        }

        // 2020-04-09 강성묵 오버로딩.
        public void SetRowStatus()
        {
            for( int i = 0; i < this.Rows.Count; ++i )
            {
                SetRowStatus( i );
            }
        }

        public void SetRowStatus( int iRow )
        {
            DataTable dtData = ( DataTable )this.DataSource;
            // Grid 에 _ROWSTATUS , _ROWSTATUSIMAGE 가 존재해야 가능함.
            if( ( dtData.Columns.Contains( Grid.ColumnName._ROWSTATUS ) ) && ( dtData.Columns.Contains( Grid.ColumnName._ROWSTATUSIMAGE ) ) )
            {
                if( iRow >= 0 )
                {
                    string sRowStatus = this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUS].Value.ToString();
                    switch( sRowStatus )
                    {
                        case "C":
                            this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUSIMAGE].Value = Properties.Resources._24X20_신규;
                            break;
                        case "U":
                            this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUSIMAGE].Value = Properties.Resources._24X20_수정;
                            break;
                        case "D":
                            this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUSIMAGE].Value = Properties.Resources._24X20_삭제;
                            break;
                        case "F":
                            this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUSIMAGE].Value = Properties.Resources._24X20_완료; // dgjang
                            break;
                        case "W":
                            this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUSIMAGE].Value = Properties.Resources._24X20_대기; // dgjang
                            break;
                        default:
                            this.Rows[iRow].Cells[Grid.ColumnName._ROWSTATUSIMAGE].Value = null;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// BindData : DataTable 값 바인딩 처리 
        /// </summary>
        /// <param name="_dt"></param>
        public void BindData( DataTable dt )
        {
            m_isDataBinding = true;
            try
            {
                if( dt == null )
                {
                    return;
                }
                else
                {
                    ResetSorted();

                    DataTable dtCopy = dt.Copy();
                    if( this.DataSource == null )
                    {
                        // Column 정의 등 초기화가 되어 있지 않으면 _dt 기준으로 Bind한다.
                        this.DataSource = dtCopy;

                        for( int i = 0; i < dtCopy.Columns.Count; i++ )
                        {
                            if( this.Columns.Count < i + 1 )
                            {
                                this.Columns.Add( dtCopy.Columns[i].ColumnName, dtCopy.Columns[i].ColumnName );
                            }
                            this.Columns[i].Tag = dtCopy.Columns[i].ColumnName;
                            this.Columns[i].DataPropertyName = dtCopy.Columns[i].ColumnName;
                        }
                    }
                    else
                    {
                        // Sheet 에 필드 설정및 초기화가 되어 있으면 초기화된 기준으로 Bind 한다.
                        DataTable dtSheet = ( ( DataTable )this.DataSource ).Clone();
                        for( int i = 0; i < dtCopy.Rows.Count; i++ )
                        {
                            // dtSheet.ImportRow(dt.Rows[i]);
                            MyImportRow( dtSheet, dtCopy.Rows[i] ); // smkang

                            // _CHK 컬럼 초기값 
                            if( dtSheet.Columns.Contains( Grid.ColumnName._CHK ) )
                            {
                                dtSheet.Rows[i][Grid.ColumnName._CHK] = false;
                            }
                            // DataTable의 값을 CellType이 CheckBox 인 경우에 Spread에 맞게 변경한다. 
                            for( int iCol = 0; iCol < this.Columns.Count; iCol++ )
                            {
                                if( this.Columns[iCol].DataPropertyName == Grid.ColumnName._CHK )
                                {
                                    continue;
                                }
                            }

                        }

                        // 2021.05.31 강성묵 추가
                        for( int iRow = 0; iRow < dtSheet.Rows.Count; iRow++ )
                        {
                            for( int iCol = 0; iCol < dtSheet.Columns.Count; iCol++ )
                            {
                                if( this.Columns[iCol] is DataGridViewEncryptColumn )
                                {
                                    if( !string.IsNullOrEmpty( dtSheet.Rows[iRow][iCol].ToString() ) )
                                    {
                                        dtSheet.Rows[iRow][iCol] = AES.getDecryptString( dtSheet.Rows[iRow][iCol].ToString() );
                                    }
                                }
                            }
                        }

                        // 현재 Data Bind
                        dtSheet.AcceptChanges();
                        this.DataSource = dtSheet;

                        prevBackColor.Clear();
                        prevForeColor.Clear();

                        // GridDetail 정보를 가지고 DataGridViewColumn의 ReadOnly 처리해준다.
                        // 추가입력시 PK Column 입력받을때 해당 Column의 ReadOnly가 변경되기 때문이다.
                        RefreshReadOnly();
                    }

                    #region 2020-07-28
                    // BindData 이전의 PK 기준으로 해당 Data가 선택되도록 처리
                    for ( int i = 0; i < this.Rows.Count; ++i )
                    {
                        if( m_currentRowPK == "" )
                        {
                            break;
                        }
                        string strPK = "";
                        for( int j = 0; j < m_arrListPK.Count; ++j )
                        {
                            strPK += this[m_arrListPK[j].ToString(), i].Value.ToString();
                        }
                        if( strPK == m_currentRowPK )
                        {
                            m_currentRow = i;
                            break;
                        }
                    }
                    if( m_currentRow != -1 && this.Rows.Count > 0 )
                    {
                        int iLastRowIndex = m_currentRow;
                        if( m_currentRow > this.Rows.Count - 1 )
                        {
                            iLastRowIndex = this.Rows.Count - 1;
                        }
                        this.CurrentCell = this[this.FirstDisplayedScrollingColumnIndex, iLastRowIndex];
                    }
                    #endregion
                }
            }
            catch( Exception ex )
            {
                throw new Exception( "BindData : " + ex.Message.ToString() );
            }
            finally
            {
                m_isDataBinding = false;

                // 2023-08-16 Added By SHJUNG (조회시 Bind Data 후 현재 Row Enter Event 발생 , ChangeRowIndex Event 발생 하도록 추가)
                /*   hyperMES 테스트 후 Open 예정
                if ( (this.CurrentCell != null) && (this.CurrentCell.RowIndex >= 0) )
                {
                    this.m_currentRow = -1;
                    DataGridViewCellEventArgs arg = new DataGridViewCellEventArgs(this.CurrentCell.ColumnIndex, this.CurrentCell.RowIndex);
                    this.OnRowEnter(arg);
                }
                */
            }


        }


        public void CheckRowStatus( int rowIndex )
        {
            if( ( this.Columns.Contains( Grid.ColumnName._ROWSTATUS ) && ( this.Rows[rowIndex].Cells[Grid.ColumnName._ROWSTATUS].Value != null ) ) )
            {
                string sCurrentRowStatus = this.Rows[rowIndex].Cells[Grid.ColumnName._ROWSTATUS].Value.ToString();
                this.CellValueChanged -= Grid_CellValueChanged;
                //if( ( sCurrentRowStatus != "C" ) && ( sCurrentRowStatus != "D" ) && (sCurrentRowStatus != "F") && (sCurrentRowStatus != "W"))
                //if( ( sCurrentRowStatus != "C" ) && ( sCurrentRowStatus != "D" ))
                if( ( sCurrentRowStatus == "U" ) || ( sCurrentRowStatus == "" ) )
                {
                    DataTable dtData = ( ( DataTable )this.DataSource );

                    // 2021.10.28 shjung
                    // 정렬된 itierGrid인 경우 DataSource의 DataTable가 불일치된다.
                    //DataRow drData = dtData.Rows[rowIndex];
                    DataRow drData = ( ( DataRowView )this.Rows[rowIndex].DataBoundItem ).Row;


                    this.Rows[rowIndex].Cells["_ROWSTATUS"].Value = "";
                    for( int i = 0; i < dtData.Columns.Count; i++ )
                    {
                        DataColumn dcData = dtData.Columns[i];
                        if( ( dcData.ColumnName != Grid.ColumnName._CHK ) && ( dcData.ColumnName != "_ROWSTATUS" ) && ( dcData.ColumnName != "_ROWSTATUSIMAGE" ) )
                        {
                            string sOriginal = drData[dcData, DataRowVersion.Original].ToString();
                            string sCurrent = this.Rows[rowIndex].Cells[dcData.ColumnName].Value.ToString();

                            if( sOriginal != sCurrent )
                            {
                                //Console.WriteLine( "ORIGINAL = |" + sOriginal + "| , Current = |" + sCurrent + "|" );

                                if( this.Columns[dcData.ColumnName] is DataGridViewCheckBoxColumn )
                                {
                                    string sFalseValue = ( ( DataGridViewCheckBoxColumn )this.Columns[dcData.ColumnName] ).FalseValue.ToString();
                                    // CheckBox일때는 Original이 빈값이면 False 처리 
                                    if( ( sOriginal == string.Empty ) && ( sCurrent == sFalseValue ) )
                                    {
                                        continue;
                                    }
                                }
                                else if( this.Columns[dcData.ColumnName] is DataGridViewButtonColumn )
                                {
                                    // Button 일때는 Skip
                                    continue;
                                }
                                this.Rows[rowIndex].Cells["_ROWSTATUS"].Value = "U";
                            }
                        }
                    }
                    SetRowStatus( rowIndex );
                }
                this.CellValueChanged += Grid_CellValueChanged;
            }
        }

        private void RefreshReadOnly()
        {
            if( m_dtGridDetail == null )
            {
                return;
            }

            for( int i = 0; i < m_dtGridDetail.Rows.Count; i++ )
            {
                DataRow drData = m_dtGridDetail.Rows[i];
                this.Columns[drData["GRIDCOLUMNID"].ToString()].ReadOnly =
                    ( drData["EDITFLAG"].ToString() != "Yes" || drData["PRIMARYKEYFLAG"].ToString() == "Yes" ); // smkang
            }
        }

        // 2020-03-11 강성묵 추가.
        private void TypingOnlyNumber( object sender, KeyPressEventArgs e, bool includePoint, bool includeMinus, int iDecimalPoint = 0 )
        {
            bool isValidInput = false;
            string currText = this.EditingControl.Text.Replace( ",", "" );

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


            //if (!char.IsControl(e.KeyChar) && ( includePoint == true ))
            if( includePoint == true )
            {
                int iCurrentDecimalPoint = 0;
                if( currText.IndexOf( "." ) > 0 )
                {
                    iCurrentDecimalPoint = currText.Split( '.' )[1].Length;
                }

                if( ( ( e.KeyChar == '.' ) && ( iDecimalPoint == 0 ) ) || ( iCurrentDecimalPoint > iDecimalPoint ) )
                {
                    e.Handled = true;
                }

                if( e.KeyChar == '.' && ( string.IsNullOrEmpty( currText.Trim() ) || currText.IndexOf( '.' ) > -1 ) )
                {
                    e.Handled = true;
                }

            }
            if( includeMinus == true )
            {
                if( e.KeyChar == '-' && ( !string.IsNullOrEmpty( currText.Trim() ) || currText.IndexOf( '-' ) > -1 ) )
                    e.Handled = true;
            }
        }
        #endregion

        // smkang
        private void MyImportRow( DataTable dt, DataRow dr )
        {
            DataRow drAdd = dt.NewRow();

            for( int i = 0; i < dt.Columns.Count; i++ )
            {
                string ithColName = dt.Columns[i].ColumnName;

                if( dr.Table.Columns.Contains( ithColName ) && !string.IsNullOrEmpty( dr[ithColName].ToString() ) )
                {
                    drAdd[ithColName] = dr[ithColName];
                }
            }
            dt.Rows.Add( drAdd );
        }

        /// <summary>
        /// Grid에서 선택된 Row 를 한칸위로 올린다.
        /// </summary>
        /// <param name="iSelectRowIndex"></param>
        public void RowMoveUp( int iSelectRowIndex )
        {
            DataTable dtCurrent = ( DataTable )this.DataSource;

            dtCurrent.AcceptChanges();

            int iInsertAtIndex = iSelectRowIndex - 1;

            // Sheet 에 2 Row 이상일때만 처리한다.
            if( this.Rows.Count > 1 )
            {
                if( iInsertAtIndex < 0 )
                {
                    iInsertAtIndex = 0;
                }

                DataRow drSrouce = dtCurrent.Rows[iSelectRowIndex];
                DataRow drAdd = dtCurrent.NewRow();

                var array = new object[drSrouce.ItemArray.Length];
                Array.Copy( drSrouce.ItemArray, 0, array, 0, drSrouce.ItemArray.Length );
                drAdd.ItemArray = array;

                // 위로 이동이기 때문에 이동전에 현재 선택된 데이터는 삭제한 후에 
                dtCurrent.Rows.RemoveAt( iSelectRowIndex );

                dtCurrent.Rows.InsertAt( drAdd, iInsertAtIndex );
                this.DataSource = dtCurrent;

                if( iInsertAtIndex == 0 )
                {
                    this.CurrentCell = this[this.FirstDisplayedScrollingColumnIndex, 0];
                }
                else
                {
                    this.CurrentCell = this[this.FirstDisplayedScrollingColumnIndex, iInsertAtIndex];
                }
            }
        }

        /// <summary>
        /// Grid에서 선택된 Row 를 한칸 아래로 내린다.
        /// </summary>
        /// <param name="iSelectRowIndex"></param>
        public void RowMoveDown( int iSelectRowIndex )
        {
            DataTable dtCurrent = ( DataTable )this.DataSource;

            dtCurrent.AcceptChanges();

            int iInsertAtIndex = iSelectRowIndex + 2;

            // Sheet 에 2 Row 이상일때만 처리한다.
            if( this.Rows.Count > 1 )
            {
                DataRow drSrouce = dtCurrent.Rows[iSelectRowIndex];
                DataRow drAdd = dtCurrent.NewRow();

                var array = new object[drSrouce.ItemArray.Length];
                Array.Copy( drSrouce.ItemArray, 0, array, 0, drSrouce.ItemArray.Length );
                drAdd.ItemArray = array;

                if( iInsertAtIndex >= this.Rows.Count )
                {
                    dtCurrent.Rows.Add( drAdd );
                }
                else
                {
                    dtCurrent.Rows.InsertAt( drAdd, iInsertAtIndex );
                }
                dtCurrent.Rows.RemoveAt( iSelectRowIndex );
                this.DataSource = dtCurrent;

                if( iInsertAtIndex >= this.Rows.Count )
                {
                    this.CurrentCell = this[this.FirstDisplayedScrollingColumnIndex, this.Rows.Count - 1];
                }
                else
                {
                    this.CurrentCell = this[this.FirstDisplayedScrollingColumnIndex, iInsertAtIndex - 1];
                }
            }
        }

        /// <summary>
        /// 그리드에 표시된 첫번째 셀에 focus를 설정합니다.
        /// </summary>
        public void SetViewPort()
        {
            this.CurrentCell = this.FirstDisplayedCell;
        }

        /// <summary>
        /// Grid의 DataTable 기준으로 특정 Row 의 Row 추가
        /// </summary>
        /// <param name="_addRow"></param>
        /// <param name="htDefaultData"></param>
        public void AddRowByDataTable( int _addRow, Hashtable htDefaultData )
        {
            DataTable dtData = ( DataTable )this.DataSource;

            if( dtData != null )
            {
                DataRow drData = dtData.NewRow();

                IDictionaryEnumerator deDefaultData = htDefaultData.GetEnumerator();

                for( int i = 0; i < htDefaultData.Count; i++ )
                {
                    deDefaultData.MoveNext();
                    string sColumnName = deDefaultData.Key.ToString();
                    string sValue = deDefaultData.Value.ToString();

                    if( this.Columns[sColumnName].CellType.Name == "DataGridViewCheckBoxCell" )
                    {
                        string sTrueValue = ( ( DataGridViewCheckBoxColumn )this.Columns[sColumnName] ).TrueValue.ToString();
                        string sFalseValue = ( ( DataGridViewCheckBoxColumn )this.Columns[sColumnName] ).FalseValue.ToString();
                        if( sTrueValue.Length > 0 )
                        {
                            if( sValue == sTrueValue )
                            {
                                drData[sColumnName] = true;
                            }
                            else
                            {
                                drData[sColumnName] = false;
                            }
                        }
                    }
                    else
                    {
                        drData[sColumnName] = sValue;
                    }
                }

                if( _addRow < dtData.Rows.Count )
                {
                    dtData.Rows.InsertAt( drData, _addRow );
                }
                else
                {
                    dtData.Rows.InsertAt( drData, _addRow + 1 );
                }
                this.SetRowStatus( _addRow );
            }
        }



        public void SaveNextDefaultAction()
        {
            // 저장후 초기화 처리
            DataTable dtData = ( DataTable )this.DataSource;

            if( dtData.Columns.Contains( "_ROWSTATUS" ) )
            {
                for( int i = dtData.Rows.Count - 1; i >= 0; i-- )
                {
                    DataRow drData = dtData.Rows[i];
                    string sRowStatus = drData["_ROWSTATUS"].ToString();

                    if( sRowStatus == "D" )
                    {
                        DeleteRow( i );
                    }
                    else
                    {
                        this["_ROWSTATUS", i].Value = "";
                        SetRowStatus( i );
                    }
                }
                dtData.AcceptChanges();
            }
        }

        public void DeleteRow( int _row )
        {
            this.Rows.RemoveAt( _row );
        }



        /// <summary>
        /// ActiveRow(CurrentRow)의 특정 Column 의 Value를 가져온다. 
        /// </summary>
        /// <param name="_sheet"></param>
        /// <param name="_row"></param>
        /// <param name="_columnname"></param>
        /// <returns></returns>
        public string GetActiveRowCellValue( string _columnname )
        {
            string sReturn = string.Empty;
            int iActiveRow = this.CurrentRow.Index;
            if( iActiveRow >= 0 )
            {
                sReturn = GetCellValue( iActiveRow, _columnname );
            }
            return sReturn;
        }

        /// <summary>
        /// 특정 Cell의 Value를 가져온다.
        /// </summary>
        /// <param name="_sheet"></param>
        /// <param name="_row"></param>
        /// <param name="_columnname"></param>
        /// <returns></returns>
        public string GetCellValue( int _row, string _columnname )
        {
            string sReturn = string.Empty;
            if( this.Columns.Contains( _columnname ) )
            {
                if( this[_columnname, _row].Value != null )
                {
                    sReturn = this[_columnname, _row].Value.ToString();
                }
            }
            return sReturn;
        }

        // 2020.05.19 강성묵 추가.
        /// <summary>
        /// 특정 Cell의 Value를 가져온다.
        /// </summary>
        /// <param name="_row"></param>
        /// <param name="_columnname"></param>
        /// <returns></returns>
        public string GetCellValue( int _row, int _col )
        {
            string sReturn = string.Empty;
            if( this[_col, _row].Value != null )
            {
                sReturn = this[_col, _row].Value.ToString();
            }
            return sReturn;
        }

        /// <summary>
        /// Grid Value Find String 
        /// 현재위치에서 마지막까지 검색후 처음부터 현재위치까지 검색
        /// </summary>
        /// <param name="findText"></param>
        public void GetFindRowByText(string findText)
        {
            GetFindRowByText(findText, true);
        }

        /// <summary>
        /// Grid Value Find String 
        /// </summary>
        /// <param name="findText"></param>
        /// <param name="isFindAll">현재위치에서 마지막까지 검색후 처음부터 현재위치까지 검색 여부</param>
        public void GetFindRowByText(string findText, bool isFindAll)
        {
            //현재 위치에서 마지막까지
            int currentRow = this.CurrentCell.RowIndex;
            int currentCol = this.CurrentCell.ColumnIndex;
            for (int iRow = currentRow; iRow < this.Rows.Count; iRow++)
            {
                int findCol = 0;
                if (iRow == currentRow) findCol = currentCol + 1;
                for (int iCol = findCol; iCol < this.Columns.Count; iCol++)
                {
                    if (this[iCol, iRow].Visible)
                    {
                        if (this[iCol, iRow].Value.ToString().Contains(findText))
                        {
                            this.CurrentCell = this[iCol, iRow];
                            return;
                        }

                    }
                }
            }

            // 처음부터 재 검색
            if (isFindAll)
            {
                for (int iRow = 0; iRow <= currentRow; iRow++)
                {
                    int findCol = this.Columns.Count;
                    if (iRow == currentRow) findCol = currentCol - 1;
                    for (int iCol = 0; iCol < findCol; iCol++)
                    {
                        if (this[iCol, iRow].Visible)
                        {
                            if (this[iCol, iRow].Value.ToString().Contains(findText))
                            {
                                this.CurrentCell = this[iCol, iRow];
                                return;
                            }

                        }
                    }
                }
            }

        }


        public void RowVisible(int rowIndex, bool isVisible)
        {
            if (!isVisible)
            {
                //if (this.CurrentRow == null) return;
                //if (this.CurrentCell == null) return;
                //int colIndex = this.CurrentCell.ColumnIndex;
                int colIndex = GetFirstEditColumnIndex();
                if (this.CurrentCell != null)
                {
                    colIndex = this.CurrentCell.ColumnIndex;
                }

                int changeRow = rowIndex;
                
                if ((this.CurrentRow != null) && (rowIndex == this.CurrentRow.Index) )
                {
                    int nextShowRow = -1;
                    for (int i = rowIndex + 1; i<this.Rows.Count; i++)
                    {
                        if (this.Rows[i].Visible)
                        {
                            nextShowRow = i;
                            break;
                        }
                    }

                    if (nextShowRow >= 0)  // Hidden 처리할 Row 다음으로 보여지는 Row가 존재하는 경우
                    {
                        changeRow = nextShowRow;
                    }
                    else  // Hidden 처리할 Row 다음으로 보여지는 Row가 존재하지 않는 경우 이전 Visible Row를 찾아 현재로 변경한다.
                    {
                        int preShowRow = -1;
                        for (int i = rowIndex - 1; i >= 0; i--)
                        {
                            if (this.Rows[i].Visible)
                            {
                                preShowRow = i;
                                break;
                            }
                        }
                        if (preShowRow >= 0)  // Hidden 처리할 Row 다음으로 보여지는 Row가 존재하는 경우
                        {
                            changeRow = preShowRow;
                        }
                    }
                }

                // 현재 Row Hidden 처리시 선택된 Row 는 처리가 안되므로 현재 Row 를 변경한다.
                if ((changeRow >= 0) && (changeRow < this.RowCount))
                {
                    //if (this[colIndex, changeRow].Visible)
                    //{
                    //    this.CurrentCell = this[colIndex, changeRow];
                    //}
                    if (changeRow == rowIndex)
                    {
                        this.CurrentCell = null;
                    }
                    else 
                    {
                        this.CurrentCell = this[colIndex, changeRow];
                    }
                }
                else
                {
                    this.CurrentCell = null;
                }
            }

            if ((rowIndex >= 0) && (rowIndex < this.RowCount))
            {
                if (this.Rows[rowIndex].Visible != isVisible)
                {
                    this.Rows[rowIndex].Visible = isVisible;
                }
            }


        }


        /// <summary>
        /// Grid 내용 클리어 처리
        /// </summary>
        /// <param name="_sheet"></param>
        public void Clear()
        {
            //this.RowCount = 0;
            if( this.DataSource != null && ( this.DataSource as DataTable ).Rows.Count > 0 )
            {
                ( ( DataTable )this.DataSource ).Clear();
            }
        }

        protected override bool ProcessDialogKey( Keys keyData ) // Fired when key is press in edit mode
        {
            //Console.WriteLine( "ProcessDialogKey: " + keyData.ToString() );
            if( keyData == Keys.Enter )
            {
                if( EnterKeyNextCell )
                {
                    MoveToRightCell();
                }
                else
                {
                    MoveToDownCell();
                }
                return true;
            }

            return base.ProcessDialogKey( keyData );
        }
        protected override void OnKeyDown( KeyEventArgs e ) // Fired when key is press in non-edit mode
        {
            //Console.WriteLine( "OnKeyDown: " + e.KeyData.ToString() );
            if( e.KeyData == Keys.Enter )
            {
                if( EnterKeyNextCell )
                {
                    MoveToRightCell();
                    e.Handled = true;
                    return;
                }
            }

            base.OnKeyDown( e );
        }

        private void MoveToRightCell()
        {
            int oldCol = this.CurrentCell.ColumnIndex;
            int newCol = oldCol;
            int row = this.CurrentCell.RowIndex;
            if( oldCol < this.ColumnCount - 1 )
            {
                for( int i = oldCol + 1; i < this.ColumnCount; i++ )
                {
                    if( this[i, row].Visible )
                    {
                        newCol = i;
                        break;
                    }
                }
            }
            if( oldCol == newCol )
            {
                for( int i = 0; i < this.ColumnCount; ++i )
                {
                    if( this[i, row].Visible )
                    {
                        newCol = i;
                        break;
                    }
                }
                row++;
            }

            if( row < this.Rows.Count )
            {
                this.CurrentCell = this[newCol, row];
            }

        }

        private void MoveToDownCell()
        {
            int oldRow = this.CurrentCell.RowIndex;
            int newRow = oldRow;
            int col = this.CurrentCell.ColumnIndex;
            if( oldRow < this.RowCount - 1 )
            {
                for( int i = oldRow + 1; i < this.RowCount; i++ )
                {
                    if( this[col, i].Visible )
                    {
                        newRow = i;
                        break;
                    }
                }
            }

            if( newRow != oldRow )
            {
                this.CurrentCell = this[col, newRow];
            }

        }



        #region # 엑셀 저장, 업로드 추가 (dgjang)

        [Category( "CustomData" ), Description( "업로드 한 엑셀 데이터 저장 버튼 활성화" )]
        public bool IsExcelSaveFlag
        {
            get; set;
        }

        public void ExcelFileDownLoad()
        {
            ExcelFileDownLoad( "제목없음" );
        }

        /// <summary>
        /// Excel Export (DownLoad) 파일명으로 사용할 작업명 입력
        /// </summary>
        /// <param name="sWorkProcessName"></param> 
        public void ExcelFileDownLoad( string sWorkProcessName )
        {
            /*bool isExport = false;

            if( this.Rows.Count == 0 )
            {
                MessageBox.Show("No data exists to store"); // USER-012
                return;
            }

            SaveFileDialog savefileDialog = ExcelReportSave( sWorkProcessName );
            if( savefileDialog.ShowDialog() == DialogResult.OK )
            {
                using( Util.BeginWaitCursorBlock() )
                {
                    // Excel 오브젝트 생성
                    Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add( Type.Missing );
                    Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                    try
                    {
                        worksheet = ( Microsoft.Office.Interop.Excel._Worksheet )workbook.ActiveSheet;
                        worksheet.Name = sWorkProcessName;

                        int iCellRowIndex = 1;
                        int iCellColumnIndex = 1;

                        // 컬럼 헤더 데이터
                        for( int col = 0; col < this.Columns.Count; col++ )
                        {
                            if( this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                                || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                                || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                            {
                                continue;
                            }

                            if( iCellRowIndex == 1 )
                            {
                                worksheet.Cells[iCellRowIndex, iCellColumnIndex] = this.Columns[col].HeaderText;
                            }

                            iCellColumnIndex++;
                        }
                        iCellColumnIndex = 1;
                        iCellRowIndex++;

                        // Row 데이터
                        for( int row = 0; row < this.Rows.Count; row++ )
                        {
                            for( int col = 0; col < this.Columns.Count; col++ )
                            {
                                if( this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                                    || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                                    || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                                {
                                    continue;
                                }
                                if( this.Rows[row].Cells[col].Value == null )
                                {
                                    iCellColumnIndex++;
                                    continue;
                                }

                                //string colName = this.Columns[col].Name;
                                //string cellType = m_dtGridDetail.Select( $"GRIDCOLUMNID = \'{colName}\'" ).CopyToDataTable().Rows[0]["CELLTYPE"].ToString();

                                //if( cellType == "PhoneNumber" )
                                //{
                                //    worksheet.Cells[iCellRowIndex, iCellColumnIndex] = this.Rows[row].Cells[col].Value.ToString();
                                //}

                                worksheet.Cells[iCellRowIndex, iCellColumnIndex] = this.Rows[row].Cells[col].Value;
                                iCellColumnIndex++;
                            }

                            iCellColumnIndex = 1;
                            iCellRowIndex++;
                        }

                        worksheet.Cells.Font.Name = "Tahoma";
                        worksheet.Cells.Font.Size = 10;
                        worksheet.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                        #region # visible filtering ver.
                        //int iAvailableCount = 0;

                        //for( int col = 0; col < this.Columns.Count; col++ )
                        //{
                        //    if( this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                        //        || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                        //        || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                        //    {
                        //        continue;
                        //    }
                        //    if( iCellRowIndex == 1 && this.Columns[col].Visible == true )
                        //    {
                        //        worksheet.Cells[iCellRowIndex, iCellColumnIndex] = this.Columns[iAvailableCount].HeaderText;
                        //        iCellColumnIndex++;
                        //    }

                        //    iAvailableCount++;
                        //}

                        //iCellColumnIndex = 1;
                        //iCellRowIndex++;

                        //for( int row = 0; row < this.Rows.Count; row++ )
                        //{
                        //    iAvailableCount = 0;

                        //    for( int col = 0; col < this.Columns.Count; col++ )
                        //    {
                        //        if( this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                        //            || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                        //            || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                        //        {
                        //            continue;
                        //        }
                        //        if( this.Columns[col].Visible == true )
                        //        {
                        //            worksheet.Cells[iCellRowIndex, iCellColumnIndex] = this.Rows[row].Cells[iAvailableCount].Value.ToString();

                        //            iCellColumnIndex++;
                        //        }

                        //        iAvailableCount++;
                        //    }

                        //    iCellColumnIndex = 1;
                        //    iCellRowIndex++;
                        //}
                        #endregion

                        workbook.SaveAs( savefileDialog.FileName );
                        //MessageBox.Show("엑셀을 성공적으로 저장했습니다.");
                        isExport = true;
                    }
                    catch( Exception ex )
                    {
                        MessageBox.Show( ex.Message );
                    }
                    finally
                    {
                        workbook.Close( false );
                        excel.Quit();
                        workbook = null;
                        excel = null;
                        if( isExport )
                        {
                            Process.Start( savefileDialog.FileName );
                        }
                    }
                }
            }*/
        }

        private SaveFileDialog ExcelReportSave( string sWorkProcessName )
        {
            SaveFileDialog savefileDialog = new SaveFileDialog();
            savefileDialog.CheckPathExists = true;
            savefileDialog.AddExtension = true;
            savefileDialog.ValidateNames = true;
            savefileDialog.InitialDirectory = Environment.GetFolderPath( Environment.SpecialFolder.Desktop );

            savefileDialog.DefaultExt = ".xlsx";
            //savefileDialog.Filter = "Microsoft Excel Workbook (*.xls)|*.xlsx";
            savefileDialog.Filter = "Microsoft Excel Workbook (*.xls)|*.xlsx|All Files (*.*)|*.*";
            //savefileDialog.Filter = "Excel File (*.xlsx)|*.xlsx|Excel File 97~2003 (*.xls)|*.xls|All Files (*.*)|*.*";
            savefileDialog.FileName = String.Format( @"{0:yyyy-MM-dd}_{1}", DateTime.Now, sWorkProcessName );
            //savefileDialog.FileName = String.Format( @"{0:yyyy-MM-dd}_재고실사 현황리스트", DateTime.Now );

            return savefileDialog;
        }

        /// <summary>
        /// Excel Import (UpLoad)
        /// </summary>
        /*public void ExcelFileUpLoad()
        {
            IsExcelSaveFlag = false;

            // Excel 오브젝트 생성
            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel._Workbook workbook = null;
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Microsoft Excel Workbook (*.xls)|*.xlsx|All Files (*.*)|*.*";

            if( openFileDialog.ShowDialog() == DialogResult.OK )
            {
                using (Util.BeginWaitCursorBlock())
                {

                    try
                    {
                        // dgView 초기화
                        this.Clear();

                        // 엑셀데이터 담을 DataTalbe 선언 (※ Clone)
                        DataTable dtExcel = ((DataTable)this.DataSource).Clone();

                        // 엑셀 변수 초기화
                        workbook = excel.Workbooks.Open(openFileDialog.FileName);
                        worksheet = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Worksheets.get_Item(1);

                        // 시트 범위 따로 지정하려면 Worksheet.Range[Worksheet.Cells[시작 행, 시작 열], Worksheet.Cells[끝 행, 끝 열]]
                        Microsoft.Office.Interop.Excel.Range range = worksheet.UsedRange;

                        // 배열에 Excel 데이터 매핑
                        object[,] objArrData = (object[,])range.Value;

                        #region # visible filtering ver.
                        //for (int row=1; row< range.Rows.Count; row++)
                        //{
                        //    for (int col=0; col<dgvMaterialStock.Columns.Count; col++)
                        //    {
                        //        if (dgvMaterialStock.Columns[col].Visible == false)
                        //        {
                        //            objArrData[row, col] = string.Empty;
                        //            continue;
                        //        }
                        //        else
                        //        {
                        //            objArrData[row, col] = range[row][col];
                        //        }
                        //    }
                        //}

                        //// 데이터테이블 컬럼 추가
                        //for (int i=1; i<=dgvMaterialStock.Columns.Count; i++)
                        //{
                        //    dtExcel.Columns.Add(i.ToString(), typeof(string));
                        //}

                        //// 데이터테이블에 2차원 배열에 담은 엑셀데이터 추가
                        //for (int row=1; row<range.Rows.Count; row++)
                        //{
                        //    DataRow drExcel = dtExcel.Rows.Add();

                        //    for (int col=0; col<dgvMaterialStock.Columns.Count; col++)
                        //    {
                        //        if (dgvMaterialStock.Columns[col].Visible == true)
                        //        {
                        //            drExcel[col - 1] = objArrData[row + 1, col];
                        //        }
                        //    }
                        //}
                        #endregion

                        // DataTable에 데이터 매핑
                        for (int row = 2; row < range.Rows.Count; row++)
                        {
                            DataRow drExcel = dtExcel.Rows.Add();

                            for (int col = 0; col < range.Columns.Count; col++)
                            {
                                /*
                                if (this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                                    || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                                    || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE)
                                {
                                    continue;
                                }
                                */
                                /*String sColumnName = objArrData[2, col + 1].ToString();
                                if (objArrData[row + 1, col + 1] == null)
                                {
                                    drExcel[sColumnName] = DBNull.Value;
                                }
                                else
                                {
                                    drExcel[sColumnName] = objArrData[row + 1, col + 1].ToString();
                                }
                            }
                            drExcel[Grid.ColumnName._ROWSTATUS] = "C";
                        }
                        dtExcel.AcceptChanges();

                        // dgView에 dataTable 데이터 매핑
                        this.DataSource = dtExcel;

                        this.CurrentCell = this.FirstDisplayedCell;
                        //this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                        //this.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

                        this.SetRowStatus();
                        
                        MessageBox.Show("Excel upload successful"); //USER-506


                        IsExcelSaveFlag = true;
                    }
                    catch (Exception ex)
                    {
                        IsExcelSaveFlag = false;
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        // 객체 해제
                        workbook.Close(true);
                        excel.Quit();
                        workbook = null;
                        excel = null;
                    }
                }

            }

        }*/

        /*public void ExcelFormatFileDownLoad(string sWorkProcessName)
        {
            bool isExport = false;

            SaveFileDialog savefileDialog = ExcelReportSave(sWorkProcessName);
            if (savefileDialog.ShowDialog() == DialogResult.OK)
            {
                using (Util.BeginWaitCursorBlock())
                {
                    // Excel 오브젝트 생성
                    Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();
                    Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);
                    Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

                    try
                    {
                        worksheet = (Microsoft.Office.Interop.Excel._Worksheet)workbook.ActiveSheet;
                        worksheet.Name = sWorkProcessName;

                        int iCellRowIndex = 1;
                        int iCellColumnIndex = 1;

                        // 컬럼 헤더 데이터
                        for (int col = 0; col < this.Columns.Count; col++)
                        {
                            if (this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                                || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                                || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE)
                            {
                                continue;
                            }

                            if (iCellRowIndex == 1)
                            {
                                worksheet.Cells[1, iCellColumnIndex] = this.Columns[col].HeaderText;
                                worksheet.Cells[2, iCellColumnIndex] = this.Columns[col].DataPropertyName;
                            }
                            iCellColumnIndex++;
                        }

                        worksheet.Cells.Font.Name = "Tahoma";
                        worksheet.Cells.Font.Size = 10;
                        worksheet.Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;

                        #region # visible filtering ver.
                        //int iAvailableCount = 0;

                        //for( int col = 0; col < this.Columns.Count; col++ )
                        //{
                        //    if( this.Columns[col].DataPropertyName == Grid.ColumnName._CHK
                        //        || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUS
                        //        || this.Columns[col].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                        //    {
                        //        continue;
                        //    }
                        //    if( iCellRowIndex == 1 && this.Columns[col].Visible == true )
                        //    {
                        //        worksheet.Cells[iCellRowIndex, iCellColumnIndex] = this.Columns[iAvailableCount].HeaderText;
                        //        iCellColumnIndex++;
                        //    }

                        //    iAvailableCount++;
                        //}

                        //iCellColumnIndex = 1;
                        //iCellRowIndex++;

                        #endregion

                        workbook.SaveAs(savefileDialog.FileName);
                        isExport = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                    finally
                    {
                        workbook.Close(false);
                        excel.Quit();
                        workbook = null;
                        excel = null;
                        if (isExport)
                        {
                            Process.Start(savefileDialog.FileName);
                        }
                    }
                }
            }
        }*/


        #endregion

        // 2020.04.22 강성묵 추가
        /// <summary>
        /// Grid에서 Select한 데이터를 데이터테이블로 반환.(ex. itierGrid.Select( "SITEID", "ITIER" ))
        /// </summary>
        public DataTable Select( string colName, string value )
        {
            string expression = colName + " =  '" + value + "'";
            return Select( expression );
        }

        // 2020.04.22 강성묵 추가
        /// <summary>
        /// Grid에서 Select한 데이터를 데이터테이블로 반환.(ex. itierGrid.Select( "SITEID = 'ITIER'" ))
        /// </summary>
        public DataTable Select( string expression )
        {
            if( ( DataTable )this.DataSource == null )
            {
                MessageBox.Show("Data table not bound to grid"); // USER-507
                return null;
            }

            DataTable dtResult = ( ( DataTable )this.DataSource ).Clone();
            DataRow[] drResultList = ( ( DataTable )this.DataSource ).Select( expression );

            try
            {
                int count = 0;
                foreach( DataRow dr in drResultList )
                {
                    dtResult.ImportRow( dr );
                    count++;
                }
                //Console.WriteLine( "# of row selected :" + count.ToString() );
            }

            catch( Exception e )
            {
                MessageBox.Show( e.Message );
            }

            return dtResult;
        }

        // 2020.06.15 강성묵 추가
        public void SetColumnSecondToCustomTime( int colIndex )
        {
            DataTable dtSrc = this.GetDataTable();
            if( dtSrc == null || dtSrc.Rows.Count == 0 )
            {
                return;
            }
            for( int i = 0; i < dtSrc.Rows.Count; ++i )
            {
                if( !( dtSrc.Rows[i][colIndex] is DBNull ) )
                {
                    dtSrc.Rows[i][colIndex] = Common.ConvertSecToCustomTime( Convert.ToInt32( dtSrc.Rows[i][colIndex] ) );
                }
            }
        }

        public void SetColumnSecondToCustomTime( string colName )
        {
            int colIndex = this.Columns[colName].Index;
            SetColumnSecondToCustomTime( colIndex );
        }

        public void RemovePKColumn( string colName )
        {
            if( m_arrListPK.Contains( colName ) )
            {
                m_arrListPK.Remove( colName );
            }
        }

        // 2021.06.03 강성묵 추가
        #region Print Function
        private PrintDialog printDialog = new PrintDialog();
        private PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
        private PrintDocument printDocument = new PrintDocument();
        int m_cnt = 0;      // 행 개수
        int m_pageNo = 1;   // 페이지 번호

        /// <summary>
        /// 해당 그리드를 인쇄합니다.
        /// </summary>
        public void PrintGrid()
        {
            PrintGrid( "제목 없음" );
        }

        /// <summary>
        /// 해당 그리드를 인쇄합니다.
        /// </summary>
        /// <param name="title">타이틀</param>
        public void PrintGrid( string title )
        {
            PrintGrid( title, true, true );
        }

        /// <summary>
        /// 해당 그리드를 인쇄합니다.
        /// </summary>
        /// <param name="title">타이틀</param>
        /// <param name="checkDate">인쇄날짜 표시할지 여부</param>
        /// <param name="checkPageNo">쪽수 표시할지 여부</param>
        public void PrintGrid( string title, bool checkDate, bool checkPageNo )
        {
            printDialog.Document = printDocument;
            printDocument.PrintPage += ( sender, e ) => printDocument_PrintPage( sender, e, title, checkDate, checkPageNo );
            printDocument.BeginPrint += printDocument_BeginPrint;
            if( printDialog.ShowDialog() == DialogResult.OK )
            {
                printDocument.Print();
            }
            printDocument = new PrintDocument();
            /*
            printDocument.PrintPage -= ( sender, e ) => printDocument_PrintPage( sender, e, title, checkDate, checkPageNo );
            printDocument.BeginPrint -= printDocument_BeginPrint;
            */
        }

        private void printDocument_PrintPage( object sender, PrintPageEventArgs e, string title, bool checkDate, bool checkPageNo )
        {
            StringFormat format = new StringFormat() // 컬럼 안에있는 값 가운데로 정렬하기 위한 포맷.
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            int width_a4 = 827;                     // a4용지 가로길이
            int higth_a4 = 1170;                    // a4용지 세로길이
            int startXPos = 10;                     // gridView 시작 x좌표
            int startYPos = 140;                    // gridView 시작 y좌표
            int avgHeight = this.Rows[0].Height;    // gridView 컬럼 하나의 높이
            int start;                              // gridView 컬럼 시작위치
            int width;                              // gridView 컬럼 가로길이
            int temp = 0;                           // 현재 페이지의 행 수

            e.Graphics.DrawString( title, new Font( "Arial", 20, FontStyle.Bold ), Brushes.Black, width_a4 / 2, 40, format );
            if( checkDate )
            {
                e.Graphics.DrawString( "인쇄일 : " + DateTime.Now.ToString( "yyyy/MM/dd" ), new Font( "Arial", 8 ), Brushes.Black, width_a4 / 2, higth_a4 - 60, format );
            }
            if( checkPageNo )
            {
                e.Graphics.DrawString( "페이지번호 : " + m_pageNo, new Font( "Arial", 8 ), Brushes.Black, width_a4 / 2, higth_a4 - 40, format );
            }

            int columnOrder = 0;
            for( int i = 0; i < this.Columns.Count; i++ )
            {
                // _chk, _rowstatusimage 컬럼 건너뛰기
                if( this.Columns[i].DataPropertyName == Grid.ColumnName._CHK
                    || this.Columns[i].DataPropertyName == Grid.ColumnName._ROWSTATUS
                    || this.Columns[i].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                {
                    continue;
                }
                // hidden 컬럼 건너뛰기
                if( this.Columns[i].Visible == false )
                {
                    continue;
                }

                if( columnOrder == 0 )
                {
                    start = 0;
                    width = this.Columns[i].Width;
                }
                else
                {
                    start = this.Columns[columnOrder].Width;
                    width = this.Columns[i].Width;
                }

                RectangleF drawRect = new RectangleF( ( float )( startXPos + start ), ( float )startYPos, ( float )width, avgHeight );
                e.Graphics.DrawRectangle( Pens.Black, ( float )( startXPos + start ), ( float )startYPos, ( float )width, avgHeight );
                e.Graphics.DrawString( this.Columns[i].HeaderText, new Font( "Arial", 8, FontStyle.Bold ), Brushes.Black, drawRect, format );
                startXPos += start;
                columnOrder = i;
            }
            startYPos += avgHeight;
            columnOrder = 0;

            for( int i = m_cnt; i < this.RowCount; i++ )
            {
                startXPos = 10; // 다시 초기화
                for( int j = 0; j < this.ColumnCount; j++ )
                {
                    if( this.Columns[j].DataPropertyName == Grid.ColumnName._CHK
                        || this.Columns[j].DataPropertyName == Grid.ColumnName._ROWSTATUS
                        || this.Columns[j].DataPropertyName == Grid.ColumnName._ROWSTATUSIMAGE )
                    {
                        continue;
                    }
                    if( this.Columns[j].Visible == false )
                    {
                        continue;
                    }

                    if( columnOrder == 0 )
                    {
                        start = 0;
                        width = this.Columns[j].Width;
                    }
                    else
                    {
                        start = this.Columns[columnOrder].Width;
                        width = this.Columns[j].Width;
                    }

                    Rectangle drawRect = new Rectangle( startXPos + start, startYPos, width, avgHeight );
                    string sValue = this.Rows[i].Cells[j].FormattedValue.ToString();
                    switch( this.Columns[j].DefaultCellStyle.Alignment )
                    {
                        case DataGridViewContentAlignment.TopCenter:
                        case DataGridViewContentAlignment.MiddleCenter:
                        case DataGridViewContentAlignment.BottomCenter:
                            format.Alignment = StringAlignment.Center;
                            break;
                        case DataGridViewContentAlignment.TopRight:
                        case DataGridViewContentAlignment.MiddleRight:
                        case DataGridViewContentAlignment.BottomRight:
                            format.Alignment = StringAlignment.Far;
                            break;
                        default:
                            format.Alignment = StringAlignment.Near;
                            break;
                    }
                    e.Graphics.DrawRectangle( Pens.Black, drawRect );
                    e.Graphics.DrawString( sValue, new Font( "Arial", 8 ), Brushes.Black, drawRect, format );
                    startXPos += start;
                    columnOrder = j;
                }
                startYPos += avgHeight;
                temp++;
                m_cnt++;
                columnOrder = 0;

                // 한 페이지당 최대 40줄씩 인쇄
                if( temp % 40 == 0 && i != this.RowCount - 1 )
                {
                    e.HasMorePages = true;
                    m_pageNo++;
                    return;
                }
            }
        }

        public void PrintGridPreview()
        {
            PrintGridPreview( "제목 없음" );
        }

        public void PrintGridPreview( string title )
        {
            PrintGridPreview( title, true, true );
        }

        public void PrintGridPreview( string title, bool checkDate, bool checkPageNo )
        {
            printPreviewDialog.Document = printDocument;
            printDocument.PrintPage += ( sender, e ) => printDocument_PrintPage( sender, e, title, checkDate, checkPageNo );
            printDocument.BeginPrint += printDocument_BeginPrint;
            printPreviewDialog.ShowDialog();
            printDocument.PrintPage -= ( sender, e ) => printDocument_PrintPage( sender, e, title, checkDate, checkPageNo );
            printDocument.BeginPrint -= printDocument_BeginPrint;
        }

        private void printDocument_BeginPrint( object sender, CancelEventArgs e )
        {
            m_cnt = 0;
            m_pageNo = 1;
        }
        #endregion

        string m_sortedColumnName = string.Empty;
        bool m_asc = true;

        // 2021.11.04
        private void Grid_ColumnHeaderMouseClick( object sender, DataGridViewCellMouseEventArgs e )
        {
            prevBackColor.Clear();
            prevForeColor.Clear();

            //if( this.Rows.Count > 0 && e.Button == MouseButtons.Left )
            //{
            //    DataTable dt = GetDataTable();
            //    int iCol = e.ColumnIndex;
            //    string colName = this.Columns[iCol].Name;
            //    if( m_dtGridDetail != null &&
            //        colName != Grid.ColumnName._CHK &&
            //        colName != Grid.ColumnName._ROWSTATUS &&
            //        colName != Grid.ColumnName._ROWSTATUSIMAGE )
            //    {
            //        string sortFlag = m_dtGridDetail.Select( $"GRIDCOLUMNID = \'{colName}\'" ).CopyToDataTable<DataRow>().Rows[0]["SORTFLAG"].ToString();
            //        if( sortFlag == Constant.FlagYesOrNo.Yes )
            //        {
            //            m_asc = m_sortedColumnName == colName ? !m_asc : true;
            //            string asc = m_asc ? "ASC" : "DESC";

            //            dt = dt.Select( "", $"{colName} {asc}" ).CopyToDataTable();
            //            this.DataSource = dt;
            //            m_sortedColumnName = colName;
            //        }
            //    }
            //}
        }

        private void ResetSorted()
        {
            m_sortedColumnName = string.Empty;
            m_asc = true;
        }

        public Hashtable CurrentRowToHashtable()
        {
            //DataTable dtData = (DataTable)this.DataSource;
            //DataRow drRow = dtData.Rows[this.CurrentRow.Index];
            
            // 정렬된 RowIndex인경우 처리를 위해
            DataRow drRow = ((DataRowView)this.Rows[this.CurrentRow.Index].DataBoundItem).Row;
            Hashtable ht = DataRowToHashtable(drRow);
            return ht;
        }

        public Hashtable DataRowToHashtable(DataRow drRow)
        {
            Hashtable ht = new Hashtable();

            foreach (DataColumn dc in drRow.Table.Columns)
            {
                if ((dc.ColumnName.Equals(Grid.ColumnName._CHK)) ||
                    (dc.ColumnName.Equals(Grid.ColumnName._ROWSTATUS)) ||
                    (dc.ColumnName.Equals(Grid.ColumnName._ROWSTATUSIMAGE)))
                {
                    continue;
                }
                if (drRow[dc.ColumnName] != DBNull.Value)
                {
                    if (drRow[dc.ColumnName] != DBNull.Value)
                        ht[dc.ColumnName] = drRow[dc.ColumnName];
                }
            }
            return ht;
        }


        public class DataGridViewChangeRowEventArgs : EventArgs
        {
            public int PreRowIndex { get; set; }
            public int CurrentRowIndex { get; set; }
        }

        public delegate void DataGridViewChangeRowHandler(object sender, DataGridViewChangeRowEventArgs e);

        private DataGridViewChangeRowHandler _onChangeRowIndexHandler;
        public event DataGridViewChangeRowHandler ChangeRowIndex
        {
            add
            {
                _onChangeRowIndexHandler += value;
            }
            remove
            {
                _onChangeRowIndexHandler -= value;
            }
        }
        public void OnChangeRowIndex()
        {
            if (_onChangeRowIndexHandler != null)
            {
                DataGridViewChangeRowEventArgs arg = new DataGridViewChangeRowEventArgs();
                arg.PreRowIndex = this.m_preCurrentRow;
                arg.CurrentRowIndex = this.m_currentRow;

                _onChangeRowIndexHandler(this, arg);
            }
        }






    }



}
