using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Timers;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

public enum ServiceState
{
    SERVICE_STOPPED = 0x00000001,
    SERVICE_START_PENDING = 0x00000002,
    SERVICE_STOP_PENDING = 0x00000003,
    SERVICE_RUNNING = 0x00000004,
    SERVICE_CONTINUE_PENDING = 0x00000005,
    SERVICE_PAUSE_PENDING = 0x00000006,
    SERVICE_PAUSED = 0x00000007,
}

[StructLayout(LayoutKind.Sequential)]
public struct ServiceStatus
{
    public int dwServiceType;
    public ServiceState dwCurrentState;
    public int dwControlsAccepted;
    public int dwWin32ExitCode;
    public int dwServiceSpecificExitCode;
    public int dwCheckPoint;
    public int dwWaitHint;
};

namespace HoneybeeService
{
    public partial class HoneybeeService : ServiceBase
    {

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(System.IntPtr handle, ref ServiceStatus serviceStatus);

        UpdateManager updateManager;

        ServiceStatus serviceStatus;

        public HoneybeeService()
        {
            InitializeComponent();
            hbEventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("HoneybeeService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "HoneybeeService", "HoneybeeServiceLog");
            }
            hbEventLog.Source = "HoneybeeService";
            hbEventLog.Log = "HoneybeeServiceLog";

            updateManager = new UpdateManager();

            serviceStatus = new ServiceStatus();
            serviceStatus.dwWaitHint = 100000;
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            hbEventLog.WriteEntry("Honeybee service starting.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            /* Do any start up activities between these two blocks */

            // Update the service state to Running.
            hbEventLog.WriteEntry("Honeybee service started.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            hbEventLog.WriteEntry("Honeybee service stopped.");

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
    }
}
