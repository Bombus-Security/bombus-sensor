using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;

namespace HoneybeeService
{
    [RunInstaller(true)]
    public partial class HoneybeeProjectInstaller : System.Configuration.Install.Installer
    {
        public HoneybeeProjectInstaller()
        {
            InitializeComponent();
        }
    }
}
