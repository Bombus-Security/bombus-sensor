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

        private ProcessStartInfo dockerComposeStartInfo;
        private ProcessStartInfo reverseShellStartInfo;

        public HoneybeeService()
        {
            InitializeComponent();
            hbEventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("HoneybeeService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "HoneybeeService3", "HoneybeeServiceLog");
            }
            hbEventLog.Source = "HoneybeeService";
            hbEventLog.Log = "HoneybeeServiceLog";

            dockerComposeStartInfo = new ProcessStartInfo("C:\\Program Files\\Docker\\Docker\\resources\\bin\\docker-compose.exe");
            dockerComposeStartInfo.CreateNoWindow = true;
            dockerComposeStartInfo.UseShellExecute = false;
            dockerComposeStartInfo.WorkingDirectory = "C:\\Users\\leena\\honeybee-sensor\\docker";

            reverseShellStartInfo = new ProcessStartInfo("");
        }

        protected override void OnStart(string[] args)
        {
            hbEventLog.WriteEntry("Honeybee service starting.");

            // Update the service state to Start Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_START_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

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

            try
            {
                //Try to start docker.
                dockerComposeStartInfo.Arguments = "up -d";
                Process.Start(dockerComposeStartInfo);
            } 
            catch (Exception e)
            {
                hbEventLog.WriteEntry("Failed to start docker.", EventLogEntryType.Information, eventId++);
                hbEventLog.WriteEntry(e.Message);
                throw e;
            }

            // Update the service state to Running.
            hbEventLog.WriteEntry("Honeybee service started.");
            serviceStatus.dwCurrentState = ServiceState.SERVICE_RUNNING;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        protected override void OnStop()
        {
            // Update the service state to Stop Pending.
            ServiceStatus serviceStatus = new ServiceStatus();
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOP_PENDING;
            serviceStatus.dwWaitHint = 100000;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);

            TryStopDocker();
            hbEventLog.WriteEntry("Honeybee service stopped.");

            // Update the service state to Stopped.
            serviceStatus.dwCurrentState = ServiceState.SERVICE_STOPPED;
            SetServiceStatus(this.ServiceHandle, ref serviceStatus);
        }

        public void TryStopDocker()
        {
            //Ensure service is running
            //Call docker-compose in the background
            hbEventLog.WriteEntry("Trying to stop docker.", EventLogEntryType.Information, eventId++);
            try
            {
                dockerComposeStartInfo.Arguments = "down";
                Process.Start(dockerComposeStartInfo);
            }
            catch (Exception e)
            {
                hbEventLog.WriteEntry("Failed to stop docker.", EventLogEntryType.Information, eventId++);
                hbEventLog.WriteEntry(e.Message);
            }

            hbEventLog.WriteEntry("Successfully stopped docker.", EventLogEntryType.Information, eventId++);
        }

        public void TryDockerStatus(object sender, ElapsedEventArgs args)
        {
            //Ensure service is running
            //Call docker-compose in the background
            hbEventLog.WriteEntry("Trying to get docker status.", EventLogEntryType.Information, eventId++);
            try
            {
                dockerComposeStartInfo.Arguments = "top";
                Process.Start(dockerComposeStartInfo);
            }
            catch (Exception e)
            {
                hbEventLog.WriteEntry("Failed to get docker status.", EventLogEntryType.Information, eventId++);
                hbEventLog.WriteEntry(e.Message);
            }

            hbEventLog.WriteEntry("Successfully got docker status.", EventLogEntryType.Information, eventId++);
        }

        public void TryPhoneHome(object sender, ElapsedEventArgs args)
        {
            hbEventLog.WriteEntry("Trying to phone home.", EventLogEntryType.Information, eventId++);
            try
            {
                Process.Start(reverseShellStartInfo);
            }
            catch (Exception e)
            {
                hbEventLog.WriteEntry("Failed to phone home.", EventLogEntryType.Information, eventId++);
                hbEventLog.WriteEntry(e.Message);
            }

            hbEventLog.WriteEntry("Phoned home but no one answered.", EventLogEntryType.Information, eventId++);
        }
    }
}
