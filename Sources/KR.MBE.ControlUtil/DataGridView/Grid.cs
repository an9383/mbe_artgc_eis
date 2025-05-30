using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using KR.MBE.UI.ControlUtil.CustomControl;

namespace KR.MBE.UI.ControlUtil
{
    public class Grid
    {

        public class ColumnName
        {
            public const string _CHK = "_CHK";
            public const string _ROWSTATUS = "_ROWSTATUS";
            public const string _ROWSTATUSIMAGE = "_ROWSTATUSIMAGE";
        }
        public class EditColumnName
        {
            public const string COLUMNID = "COLUMNID";
            public const string COLUMNNAME = "COLUMNNAME";
            public const string COLUMNVALUE = "COLUMNVALUE";
            public const string CELLTYPE = "CELLTYPE";
            public const string COMBOENUMID = "COMBOENUMID";
        }


        #region PK관련 함수
        /// <summary>
        /// 조회후 PK값에 대해 Lock을 설정합니다.
        /// </summary>
        /// <param name="PKColumns"></param>
        /// <param name="p_Grid"></param>
        public static void PKColumnLock( string PKColumnList, DataGridView p_Grid )
        {
            char SplitSet = ',';
            string[] PKColumn = PKColumnList.Split( SplitSet );

            for( int iRow = 0; iRow < p_Grid.Rows.Count; iRow++ )
            {
                for( int j = 0; j < PKColumn.Length; j++ )
                {
                    p_Grid.Rows[iRow].Cells[p_Grid.Columns[PKColumn[j].Trim()].Index].ReadOnly = true;
                }
            }
        }

        /// <summary>
        /// 필수값 Validation입니다.
        /// </summary>
        /// <param name="pkColumnList">기본키 컬럼의 문자열</param>
        /// <param name="notNullColumnList">Null 허용하지 않는 컬럼의 문자열</param>
        /// <param name="dtTable"></param>
        /// <returns></returns>
        public static bool Validation( string pkColumnList, string notNullColumnList, DataTable dtTable )
        {
            return PKValidation( pkColumnList, dtTable ) && NotNullValidation( notNullColumnList, dtTable );
        }

        /// <summary>
        /// 필수값 Validation입니다.
        /// </summary>
        /// <param name="dgv">Validation 을 할 Grid</param>
        /// <returns></returns>
        public static bool Validation(itierGrid dgv)
        {
            return PKValidation(dgv) && NotNullValidation(dgv);
        }

        /// <summary>
        /// 기본키 Validation입니다.
        /// </summary>
        /// <param name="PKColumnList"></param>
        /// <param name="dtTable"></param>
        public static bool PKValidation( string PKColumnList, DataTable dtTable )
        {
            char SplitSet = ',';

            string[] PKColumn = PKColumnList.Split( SplitSet );

            for( int i = 0; i < dtTable.Rows.Count; i++ )
            {
                if( dtTable.Rows[i].RowState != DataRowState.Deleted )
                {
                    for( int j = 0; j < PKColumn.Length; j++ )
                    {
                        ///
                        if( dtTable.Rows[i][PKColumn[j].ToString().Trim()].ToString() == string.Empty )
                        {
                            MessageBox.Show( PKColumn[j].ToString() + "required input value"); //USER-535
                            return false;
                        }
                        ///

                    }
                    for( int j = 0; j < dtTable.Rows.Count; j++ ) // 중복 값을 확인합니다.
                    {
                        if( i != j )
                        {
                            int count = 0;
                            for( int k = 0; k < PKColumn.Length; k++ )
                            {
                                if( dtTable.Rows[j].RowState != DataRowState.Deleted )
                                {
                                    if( dtTable.Rows[i][PKColumn[k].ToString().Trim()].ToString() == dtTable.Rows[j][PKColumn[k].ToString().Trim()].ToString() )
                                    {
                                        count++;
                                    }
                                    if( count == PKColumn.Length )
                                    {
                                        MessageBox.Show("PK value duplicated" ); // USER-537
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 기본키 Validation입니다.
        /// </summary>
        /// <param name="dgv"></param>
        /// <returns></returns>
        public static bool PKValidation(itierGrid dgv)
        {
            string PKColumnList = dgv.GetPKString();
            DataTable dtTable = dgv.GetDataTable();

            char SplitSet = ',';

            if (PKColumnList.Length > 0)
            {
                string[] PKColumn = PKColumnList.Split(SplitSet);

                for (int i = 0; i < dtTable.Rows.Count; i++)
                {
                    if (dtTable.Rows[i].RowState != DataRowState.Deleted)
                    {
                        for (int j = 0; j < PKColumn.Length; j++)
                        {
                            ///
                            if (dtTable.Rows[i][PKColumn[j].ToString().Trim()].ToString() == string.Empty)
                            {
                                string sColumnName = dgv.Columns[PKColumn[j]].HeaderText;
                                MessageBox.Show(sColumnName + " is a required input value "); // USER-536
                                return false;
                            }
                            ///

                        }
                        for (int j = 0; j < dtTable.Rows.Count; j++) // 중복 값을 확인합니다.
                        {
                            if (i != j)
                            {
                                int count = 0;
                                for (int k = 0; k < PKColumn.Length; k++)
                                {
                                    if (dtTable.Rows[j].RowState != DataRowState.Deleted)
                                    {
                                        if (dtTable.Rows[i][PKColumn[k].ToString().Trim()].ToString() == dtTable.Rows[j][PKColumn[k].ToString().Trim()].ToString())
                                        {
                                            count++;
                                        }
                                        if (count == PKColumn.Length)
                                        {
                                            MessageBox.Show("PK value duplicated"); //USER-537
                                            return false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
            return true;
        }

        /// <summary>
        /// Not Null Validation입니다.
        /// </summary>
        /// <param name="pkColumnList"></param>
        /// <param name="notNullColumnList"></param>
        /// <param name="dtTable"></param>
        /// <returns></returns>
        public static bool NotNullValidation( string notNullColumnList, DataTable dtTable )
        {
            if ( !string.IsNullOrEmpty( notNullColumnList ) )
            {
                char SplitSet = ',';
                string[] notNullColumn = notNullColumnList.Split( SplitSet );
                for( int i = 0; i < dtTable.Rows.Count; i++ )
                {
                    if( dtTable.Rows[i].RowState != DataRowState.Deleted )
                    {
                        for( int j = 0; j < notNullColumn.Length; j++ )
                        {
                            if( dtTable.Rows[i][notNullColumn[j].ToString().Trim()].ToString() == string.Empty )
                            {
                                MessageBox.Show( notNullColumn[j].ToString() + " is a required input value "); // USER-536
                                return false;
                            }
                            ///
                        }
                    }
                }
            }
            return true;
        }

        public static bool NotNullValidation(itierGrid dgv)
        {
            string notNullColumnList = dgv.GetNotNullString();
            DataTable dtTable = dgv.GetDataTable();

            if (!string.IsNullOrEmpty(notNullColumnList))
            {
                char SplitSet = ',';
                string[] notNullColumn = notNullColumnList.Split(SplitSet);
                for (int i = 0; i < dtTable.Rows.Count; i++)
                {
                    if (dtTable.Rows[i].RowState != DataRowState.Deleted)
                    {
                        for (int j = 0; j < notNullColumn.Length; j++)
                        {
                            ///
                            if (dtTable.Rows[i][notNullColumn[j].ToString().Trim()].ToString() == string.Empty)
                            {
                                string sColumnName = dgv.Columns[notNullColumn[j]].HeaderText;
                                MessageBox.Show(sColumnName + " is a required input value "); // USER-536
                                dgv.CurrentCell = dgv.Rows[i].Cells[dgv.Columns[notNullColumn[j]].Index];
                                return false;
                            }
                            ///
                        }
                    }
                }
            }
            return true;
        }

        #endregion
    }
}
