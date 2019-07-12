using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Automaton.Winforms
{
    public partial class MainWindow : Form
    {
        public ProgramLogic logic;
        public MainWindow()
        {
            logic = new ProgramLogic();
            logic.SetLogFn(LogLine);
            logic.SetWorkerUpdateFn(UpdateWorkers);
            InitializeComponent();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            
        }

        public void LogLine(string line)
        {
            if (LogListBox != null)
                LogListBox.Invoke((MethodInvoker)delegate
                {

                    LogListBox.Items.Add(line);
                    LogListBox.SelectedIndex = LogListBox.Items.Count - 1;
                    LogListBox.SelectedIndex = -1;
                    this.Text = "Automaton - " + line;
                });
        }

        public void UpdateWorkers(string[] statuses)
        {
            if (QueueStatus != null)
                QueueStatus.Invoke((MethodInvoker)delegate
                {
                    QueueStatus.Lines = statuses;
                });

            if (SetInstallLocationBtn != null)
                SetInstallLocationBtn.Invoke((MethodInvoker)delegate
                {
                    SetInstallLocationBtn.Enabled = logic.InstallerData != null && !logic.Installing;
                });

            if (InstallBtn != null)
                InstallBtn.Invoke((MethodInvoker)delegate
                {
                    InstallBtn.Enabled = logic.InstallerData != null && logic.InstallFolder != null && !logic.Installing;
                });
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.auto|*.auto";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                logic.LoadModPack(ofd.FileName);
            }

            

        }

        private void SetInstallLocationBtn_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.SelectedPath = "c:\\";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                logic.InstallFolder = fbd.SelectedPath;
            }

        }

        private void InstallBtn_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                logic.Install();
            }
            ).Start();
        }

        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
