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
        public open()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            Process.Start(System.IO.Path.GetDirectoryName(this.Context.Parameters["AssemblyPath"]) + @"\Liberators.exe");

            
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            System.Diagnostics.Process.GetCurrentProcess().Kill();
            //string projectDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liberators";
            var projectDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Liberators";
            if (Directory.Exists(projectDir) == true)  Directory.Delete(projectDir, true);
        }
    }
}
