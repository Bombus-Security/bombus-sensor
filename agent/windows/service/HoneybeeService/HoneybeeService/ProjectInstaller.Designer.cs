namespace HoneybeeService
{
    partial class HoneybeeProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.honeybeeServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.honeybeeServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // honeybeeServiceProcessInstaller
            // 
            this.honeybeeServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.honeybeeServiceProcessInstaller.Password = null;
            this.honeybeeServiceProcessInstaller.Username = null;
            // 
            // honeybeeServiceInstaller
            // 
            this.honeybeeServiceInstaller.Description = "Manages the Honeybee agent.";
            this.honeybeeServiceInstaller.DisplayName = "Honeybee";
            this.honeybeeServiceInstaller.ServiceName = "Service1";
            this.honeybeeServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // HoneybeeProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.honeybeeServiceProcessInstaller,
            this.honeybeeServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller honeybeeServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller honeybeeServiceInstaller;
    }
}