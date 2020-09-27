using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
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

        protected override void OnAfterInstall(IDictionary savedState)
        {
            base.OnAfterInstall(savedState);

            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly(); 
            string path = asm.Location.Remove(asm.Location.LastIndexOf("\\"));
            System.Diagnostics.Process.Start(path+@"\Liberators.exe");
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
