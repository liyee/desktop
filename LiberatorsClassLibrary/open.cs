using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace LiberatorsClassLibrary
{
    [RunInstaller(true)]
    public partial class open : System.Configuration.Install.Installer
    {
        Process proc = null;
        public open()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            //Process.Start(System.IO.Path.GetDirectoryName(this.Context.Parameters["AssemblyPath"]) + @"\Liberators.exe");
            //Process.Start("liberators://");

        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            //System.Diagnostics.Process.Start(System.IO.Path.GetDirectoryName(Me.Context.Parameters("AssemblyPath")) + "\MyApplication.exe")

            string targetDir = string.Format(Path.GetDirectoryName(this.Context.Parameters["AssemblyPath"]));//this is where testChange.bat lies
            proc = new Process();
            proc.StartInfo.WorkingDirectory = targetDir;
            proc.StartInfo.FileName = "start.bat";
            proc.StartInfo.Arguments = string.Format("10");//this is argument
                                                           
            proc.Start();
            proc.WaitForExit();
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            string projectDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liberators";
            if(Directory.Exists(projectDir) == true)  Directory.Delete(projectDir, true);
        }
    }
}
