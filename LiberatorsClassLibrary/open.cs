using System;
using System.Collections;
using System.ComponentModel;
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