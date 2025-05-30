using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARTGC.EIS
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string sMessage = string.Empty;
            sMessage += "EIS Local Application is already running." + System.Environment.NewLine;
            sMessage += "Process in Task Manager to check that please."; // "작업관리자에서 해당 Process 를 확인해 보시기 바랍니다.";

            if (!IsAppAlreadyRunning())
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new frmMain());
            }
            else
            {
                MessageBox.Show(sMessage, "Warning");
                Application.Exit();
            }
        }

        private static bool IsAppAlreadyRunning()
        {
            System.Diagnostics.Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
            return (checkProcess(currentProcess.Id, currentProcess.ProcessName));
        }

        private static bool checkProcess(int ID, string Name)
        {
            bool bAlreadyRunnig = false;

            System.Diagnostics.Process[] processess = System.Diagnostics.Process.GetProcesses();

            foreach (System.Diagnostics.Process process in processess)
            {
                if (ID != process.Id)
                {
                    if (Name == process.ProcessName)
                    {
                        bAlreadyRunnig = true;
                        break;
                    }
                }
            }
            return bAlreadyRunnig;
        }

    }
}
