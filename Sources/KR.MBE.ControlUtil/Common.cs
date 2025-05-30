// Common.cs

// Last Update : 2020. 06. 01
// Update History
/****************************************************************************************************
 * 2020. 06. 01 
 * public static void WaitDialogHide(Form mainForm) 함수 추가 - smkang
 *  폼이 활성화되는 이벤트에서 WaitDialog 사용 시 폼 전체가 맨 뒤로 보내지는 현상을 수정하기 위해 추가.
 *  공정진행, Lot 이력조회, 월간 CycleTime 에서 사용중.
 *  
 * public static Boolean SearchPanelFlag(System.Windows.Forms.Panel SearchPanel) 함수 변경 - smkang
 *  Control이 콤보박스일 때 데이터를 바인딩하지 않아도 함수를 실행하기 위해 SelectedItem 사용.
 * 
****************************************************************************************************/

using System;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using KR.MBE.CommonLibrary.Struct;
using KR.MBE.CommonLibrary.Utils;

namespace KR.MBE.UI.ControlUtil
{
    public class Common
    {
        public static DataTable g_dtColorList = new DataTable();

        public static Color m_cRun = Color.Blue;
        public static Color m_cIdle = Color.Orange;
        public static Color m_cDown = Color.Red;
        public static Color m_cHold = Color.Indigo;
        public static Color m_cPM = Color.Yellow;
        public static Color m_cMaint = Color.LimeGreen;

        private static frmWaitDialog m_frmWaitDialog = new frmWaitDialog();

        public static void SetDoubleBuffered(Control control)
        {
            // set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                null, control, new object[] { true });
        }


        public static Font AutoFontSize(Label label)
        {

            Font ftReturn = label.Font;

            if (label.Text.Length > 0)
            {
                Graphics gp;
                SizeF sz;
                Single Faktor, FaktorX, FaktorY;

                gp = label.CreateGraphics();
                sz = gp.MeasureString(label.Text, label.Font);
                gp.Dispose();

                FaktorX = (label.Width) / sz.Width;
                FaktorY = (label.Height) / sz.Height;

                if (FaktorX > FaktorY)
                    Faktor = FaktorY;
                else
                    Faktor = FaktorX;
                
                ftReturn = new Font(ftReturn.Name, ftReturn.SizeInPoints * (Faktor) - 1);

            }

            return ftReturn;

        }

        public static Boolean DateValidation(DateTimePicker FromDate, DateTimePicker Todate)
        {
            //if( FromDate.Value > Todate.Value )
            //{
            //    MessageBox.Show( "From 날짜가 To날짜보다 큽니다." );
            //    return false;
            //}
            //else
            //{
            //    return true;
            //}
            return DateValidation(FromDate.Value, Todate.Value);
        }

        public static Boolean DateValidation(DateTime FromDate, DateTime Todate)
        {
            if (FromDate > Todate)
            {
                //MessageBox.Show("From 날짜가 To날짜보다 큽니다.");
                MessageBox.Show("From date is greater than To date."); // USER-500

                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FitToForm"></param>
        public static void WaitDialogShow(Form FitToForm, string sMessage)
        {
            /*
            m_frmWaitDialog.setMessage( sMessage );
            m_frmWaitDialog.Show();
            m_frmWaitDialog.Location = FitToForm.Location;
            m_frmWaitDialog.Width = FitToForm.Width;
            m_frmWaitDialog.Height = FitToForm.Height;
            m_frmWaitDialog.Refresh();
            m_frmWaitDialog.BringToFront();
            m_frmWaitDialog.Focus();
            */

            m_frmWaitDialog.setMessage(sMessage);
            m_frmWaitDialog.Show();

            int iY = FitToForm.Location.Y + ((FitToForm.Height - m_frmWaitDialog.Height) / 2);
            Point pLocation = new Point(FitToForm.Location.X + ((FitToForm.Width - m_frmWaitDialog.Width) / 2), iY);
            m_frmWaitDialog.Location = pLocation;
            //m_frmWaitDialog.Location = FitToForm.Location;
            m_frmWaitDialog.Width = FitToForm.Width - 100;
            //m_frmWaitDialog.Height = FitToForm.Height;
            m_frmWaitDialog.TopMost = true;
            m_frmWaitDialog.Refresh();
            m_frmWaitDialog.BringToFront();
            m_frmWaitDialog.Focus();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="HideForm"></param>
        public static void WaitDialogHide()
        {
            m_frmWaitDialog.Hide();
        }

        public static void WaitDialogHide(Form mainForm)
        {
            m_frmWaitDialog.SendToBack();
            m_frmWaitDialog.Hide();
            mainForm.BringToFront();
        }

        /// <summary>
        /// 전체화면이 변경되었을때 Search Panel에 조회조건 값이 있을경우 다시 조회합니다.
        /// </summary>
        /// <param name="sPanel"></param>
        /// <returns></returns>
        public static Boolean SearchPanelFlag(System.Windows.Forms.Panel SearchPanel)
        {
            foreach (Control Control in SearchPanel.Controls)
            {
                if (Control is System.Windows.Forms.TextBox) // Text 항목에 값이 있는지 확인
                {
                    System.Windows.Forms.TextBox TextControl = Control as System.Windows.Forms.TextBox;
                    if (ConvertUtil.GetStringValue(TextControl.Text.Trim()) != "")
                    {
                        return true;
                    }
                }
                else if (Control is System.Windows.Forms.ComboBox) // ComboBox에 선택된 값이 있는지 확인
                {
                    System.Windows.Forms.ComboBox ComboControl = Control as System.Windows.Forms.ComboBox;
                    //if (ConvertUtil.GetStringValue(ComboControl.SelectedValue.ToString()) != "")
                    //{
                    //    return true;
                    //}
                    if (ConvertUtil.GetStringValue(ComboControl.SelectedItem.ToString()) != "")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// object가 null일 경우 error안나게 하기위해. string 변환
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string GetStringValue(object Value)
        {
            string rtnValue = "";
            try
            {
                if (Value == null || Value.ToString() == "")
                {
                    rtnValue = "";
                }
                else
                {
                    rtnValue = Value.ToString();
                }
            }
            catch
            {
                rtnValue = "";
            }
            return rtnValue;
        }

        /// <summary>
        /// 저장 에러시 창을 띄웁니다.
        /// </summary>
        /// <param name="returnData"></param>
        public static void ErrorMessageBoxShow(ReturnData returnData)
        {
            ErrorMessageBox MsgBox = new ErrorMessageBox(returnData);

            MsgBox.WindowState = FormWindowState.Normal;
            MsgBox.StartPosition = FormStartPosition.CenterParent;
            MsgBox.ShowDialog();
        }

        /// <summary>
        /// 쿼리 에러시 창을 띄웁니다.
        /// </summary>
        /// <param name="returnData"></param>
        public static void QueryErrorMessageBoxShow(DataSet returnData)
        {
            ReturnData ErrorData = new ReturnData();

            ErrorData.returncode = returnData.Tables["_REPLYDATA"].Rows[0]["ERRORCODE"].ToString();
            ErrorData.returnmessage = returnData.Tables["_REPLYDATA"].Rows[0]["RETURNMESSAGE"].ToString();
            ErrorData.returndetailmessage = returnData.Tables["_REPLYDATA"].Rows[0]["RETURNDETAILMESSAGE"].ToString();

            ErrorMessageBox MsgBox = new ErrorMessageBox(ErrorData);

            MsgBox.WindowState = FormWindowState.Normal;
            MsgBox.StartPosition = FormStartPosition.CenterParent;
            MsgBox.ShowDialog();

        }

        public static string CalculateSpendingTime(string txnName, DateTime startTime, DateTime endTime)
        {
            double spendingTime = (endTime - startTime).TotalSeconds;
            string strReturn = "'" + txnName + "' 소요시간: " + spendingTime.ToString("0.000000");
            strReturn += " (전송: " + startTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + " 응답: " + endTime.ToString("yyyy-MM-dd HH:mm:ss.ffffff") + ")";

            return strReturn;
        }

        /// <summary>
        /// Decimal 값을 지정된 DecimalPoint까지만 짤라서 String으로 반환한다.
        /// </summary>
        /// <param name="dValue"></param>
        /// <param name="decimalPoint"></param>
        /// <returns></returns>
        public static string ChangeDecimalText(decimal dValue, int decimalPoint)
        {
            string sReturn = dValue.ToString();
            if (sReturn.Contains("."))
            {
                int iIndexofPoint = sReturn.IndexOf(".");
                int iDecimalPoint = sReturn.Length - iIndexofPoint - 1;
                if (iDecimalPoint > decimalPoint)
                {
                    sReturn = sReturn.Substring(0, iIndexofPoint + 2);
                }
            }
            return sReturn;
        }

        public static string ConvertSecToCustomTime(int totalSeconds)
        {
            string sReturn = string.Empty;

            if (totalSeconds >= 0)
            {
                int unitSectoHour = 60 * 60;
                int unitSectoMin = 60;

                int iHour = totalSeconds / unitSectoHour;
                totalSeconds %= unitSectoHour;
                int iMinute = totalSeconds / unitSectoMin;
                totalSeconds %= unitSectoMin;
                int iSecond = totalSeconds;

                if (iHour > 0)
                {
                    sReturn = String.Format("{0:D2}:{1:D2}:{2:D2}", iHour, iMinute, iSecond);
                }
                else
                {
                    sReturn = String.Format("{0:D2}:{1:D2}", iMinute, iSecond);
                }
            }
            return sReturn;
        }

        public static string ConvertMinToCustomTime(int totalMinutes)
        {
            return ConvertSecToCustomTime(totalMinutes * 60);
        }

        #region CenterWinDialog             using (new CenterWinDialog(this)) {   }

        public class CenterWinDialog : IDisposable
        {
            private int mTries = 0;
            private Form mOwner;

            public CenterWinDialog(Form owner)
            {
                mOwner = owner;
                owner.BeginInvoke(new System.Windows.Forms.MethodInvoker(findDialog));
            }

            private void findDialog()
            {
                // Enumerate windows to find the message box
                if (mTries < 0) return;
                EnumThreadWndProc callback = new EnumThreadWndProc(checkWindow);
                if (EnumThreadWindows(GetCurrentThreadId(), callback, IntPtr.Zero))
                {
                    if (++mTries < 10) mOwner.BeginInvoke(new System.Windows.Forms.MethodInvoker(findDialog));
                }
            }

            private bool checkWindow(IntPtr hWnd, IntPtr lp)
            {
                // Checks if <hWnd> is a dialog
                StringBuilder sb = new StringBuilder(260);
                GetClassName(hWnd, sb, sb.Capacity);
                if (sb.ToString() != "#32770") return true;

                // Got it
                Rectangle frmRect = new Rectangle(mOwner.Location, mOwner.Size);
                RECT dlgRect;
                GetWindowRect(hWnd, out dlgRect);
                MoveWindow(hWnd,
                    frmRect.Left + (frmRect.Width - dlgRect.Right + dlgRect.Left) / 2,
                    frmRect.Top + (frmRect.Height - dlgRect.Bottom + dlgRect.Top) / 2,
                    dlgRect.Right - dlgRect.Left,
                    dlgRect.Bottom - dlgRect.Top, true);
                return false;
            }

            public void Dispose()
            {
                mTries = -1;
            }

            // P/Invoke declarations
            private delegate bool EnumThreadWndProc(IntPtr hWnd, IntPtr lp);
            [DllImport("user32.dll")]
            private static extern bool EnumThreadWindows(int tid, EnumThreadWndProc callback, IntPtr lp);
            [DllImport("kernel32.dll")]
            private static extern int GetCurrentThreadId();
            [DllImport("user32.dll")]
            private static extern int GetClassName(IntPtr hWnd, StringBuilder buffer, int buflen);
            [DllImport("user32.dll")]
            private static extern bool GetWindowRect(IntPtr hWnd, out RECT rc);
            [DllImport("user32.dll")]
            private static extern bool MoveWindow(IntPtr hWnd, int x, int y, int w, int h, bool repaint);
            private struct RECT { public int Left; public int Top; public int Right; public int Bottom; }
        }
        #endregion
    }
}
