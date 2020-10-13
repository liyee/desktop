using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace LiberatorsClassLibrary
{
    [RunInstaller(true)]
    public partial class open : System.Configuration.Install.Installer
    {
        private static tool tool = null;
        public open()
        {
            InitializeComponent();
            if (tool == null) tool = new tool();
        }

        protected override void OnBeforeInstall(IDictionary savedState)
        {
            try
            {
                base.OnBeforeInstall(savedState);
                Process[] pProcess1;
                Process[] pProcess2;
                pProcess1 = Process.GetProcessesByName("Liberators");
                if (pProcess1.Length > 0) pProcess1[0].Kill();
                pProcess2 = Process.GetProcessesByName("CefSharp.BrowserSubprocess");

                if (pProcess2.Length > 0)
                {
                    for (int i = 0; i <= pProcess2.Length - 1; i++)
                    {
                        pProcess2[i].Kill();
                    }
                }
            }
            catch (Exception)
            {

                //throw;
            }
            
        }

        public override void Install(IDictionary stateSaver)
        {
            try
            {
                base.Install(stateSaver);
                tool.log("micend", "", 11);
            }
            catch (Exception)
            {

                //throw;
            }
                       
        }

        public override void Commit(IDictionary savedState)
        {
            base.Commit(savedState);
            //Process.Start(System.IO.Path.GetDirectoryName(this.Context.Parameters["AssemblyPath"]) + @"\Liberators.exe");
        }

        public override void Uninstall(IDictionary savedState)
        {
            base.Uninstall(savedState);
            try
            {
                //string projectDir = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Liberators";
                var projectDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Liberators";
                if (Directory.Exists(projectDir) == true) Directory.Delete(projectDir, true);
                tool.log("micend", projectDir, 12);
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }
}