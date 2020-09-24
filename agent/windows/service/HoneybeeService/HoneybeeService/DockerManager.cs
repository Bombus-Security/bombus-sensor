using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Docker.DotNet;
using Docker.DotNet.Models;

namespace HoneybeeService
{
    class DockerManager
    {
        //The http client is used for calling API endpoints
        HttpClient httpClient;

        DockerClient dockerClient;

        System.Diagnostics.EventLog hbEventLog;

        DockerManager()
        {
            //The http client is used for calling API endpoints
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://hnyb.app/api/")
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            dockerClient = new DockerClientConfiguration().CreateClient();

            //Open or create the eventlog
            hbEventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("HoneybeeService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "HoneybeeService", "HoneybeeServiceLog");
            }
            hbEventLog.Source = "HoneybeeService";
            hbEventLog.Log = "HoneybeeServiceLog";

            hbEventLog.WriteEntry("DockerManager: DockerManager created.");
        }

        /*
         * Returns true if the Docker Service is running.
         */
        public async Task<bool> GetDockerServiceStatus()
        {
        }

        public async Task<bool> JoinSwarm(List<String> remote_addrs, String join_token, String listen_addr, String advertise_addr="", String data_path_addr="")
        {
            await dockerClient.Swarm.JoinSwarmAsync
            (
                new SwarmJoinParameters
                {
                    ListenAddr = listen_addr,
                    RemoteAddrs = remote_addrs,
                    JoinToken = join_token
                }
            );
        }

        public async Task StopDocker()
        {

        }

        public async Task StartDocker(String )
        {

        }
    }
}
