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

        private int eventId = 1;

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
        }

        protected override void OnStart(string[] args)
        {
            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            hbEventLog.WriteEntry("Honeybee service starting.");

            //We want to phone home every 30 minutes
            Timer phoneHomeTimer = new Timer();
            phoneHomeTimer.Interval = 1800000; // 30 minutes
            phoneHomeTimer.Elapsed += new ElapsedEventHandler(this.TryPhoneHome);
            phoneHomeTimer.Start();

            //We want to check the status of the docker images every 5 minutes
            Timer dockerStatusTimer = new Timer();
            dockerStatusTimer.Interval = 300000; // 5 minutes
            dockerStatusTimer.Elapsed += new ElapsedEventHandler(this.TryDockerStatus);
            dockerStatusTimer.Start();

            // Update the service state to Running.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void TryStartDocker()
        {

        }

        public void TryStopDocker()
        {

        }
        public void TryDockerStatus(object sender, ElapsedEventArgs args)
        {

        }
        public void TryPhoneHome(object sender, ElapsedEventArgs args)
        {
            hbEventLog.WriteEntry("Trying to phone home.", EventLogEntryType.Information, eventId++);
            try
            {
                using (Process myProcess = new Process())
                {
                    myProcess.StartInfo.UseShellExecute = false;
                    // You can start any process, HelloWorld is a do-nothing example.
                    myProcess.StartInfo.FileName = "C:\\HelloWorld.exe";
                    myProcess.StartInfo.CreateNoWindow = true;
                    myProcess.Start();
                }
            }
            catch (Exception e)
            {
                hbEventLog.WriteEntry(e.Message);
            }

            hbEventLog.WriteEntry("Phoned home but no one answered.", EventLogEntryType.Information, eventId++);
        }

        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            hbEventLog.WriteEntry("Honeybee service stopped.");

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }
    }
}
