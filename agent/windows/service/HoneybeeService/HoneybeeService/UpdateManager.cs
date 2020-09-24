using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Win32;
using System.IO;

namespace HoneybeeService
{
    class UpdateManager
    {
        //The http client is used for calling API endpoints
        HttpClient httpClient;

        //The web client is used for downloading files
        WebClient webClient;

        //The path to the base install directory
        String basePath;

        //The path to the content directory
        String contentPath;

        System.Diagnostics.EventLog hbEventLog;

        public UpdateManager()
        {
            //Retrieve paths to our content and base directories
            basePath = (String)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Honeybee\\Agent", "AppPath", "");
            contentPath = (String)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Honeybee\\Agent", "ContentPath", basePath + "\\content");

            //The http client is used for calling API endpoints
            httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://hnyb.app/api/")
            };
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            //The web client is used for downloading files
            webClient = new WebClient();

            //Open or create the eventlog
            hbEventLog = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("HoneybeeService"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "HoneybeeService", "HoneybeeServiceLog");
            }
            hbEventLog.Source = "HoneybeeService";
            hbEventLog.Log = "HoneybeeServiceLog";

            hbEventLog.WriteEntry("UpdateManager: UpdateManager created.");
        }

        /*
         * Determines whether the current LATEST patch is newer than current content.
         * If it is download it, verify it's signature, and apply it.
         */
        public async Task GetContent()
        {
            try
            {
                hbEventLog.WriteEntry("UpdateManager: Checking for content updates.");
                //Get the timestamp/hash of our previous update
                int local_version = GetLocalContentVersion();

                //Check /api/content/currentContentInfo
                int latest_version = await GetLatestContentVersion();

                //If newer content available download from /content/LATEST
                if (latest_version > local_version)
                {
                    hbEventLog.WriteEntry("UpdateManager: Newer content available. Attempting to download.");

                    String endpoint = await GetContentEndpoint();
                    String path_to_update = await DownloadLatestContent(endpoint);
                }
            }
        }

        /*
         * Retrieves the update time from <content>/contentversion stored as an int32
         */
        public int GetLocalContentVersion()
        {
            return Int32.Parse(File.ReadAllLines(contentPath + "\\contentversion")[0]);
        }

        /*
         * Gets the current latest update version from Honeybee servers
         */
        public async Task<int> GetLatestContentVersion()
        {
            hbEventLog.WriteEntry("UpdateManager: Checking for latest content version.");
  
            int version = 0;
            HttpResponseMessage response = await httpClient.GetAsync("content/currentContentVersion");

            if (response.IsSuccessStatusCode)
            {
                version = await response.Content.ReadAsAsync<int>();
                hbEventLog.WriteEntry("UpdateManager: Got latest content version: " + version);
            }
            else
            {
                hbEventLog.WriteEntry("UpdateManager: Could not retrieve latest content version. Server returned: " + response.StatusCode.ToString() + " " + response.ReasonPhrase);
            }

            return version;
        }

        /*
         * Requests an endpoint from Honeybee servers to download content files from.
         */
        public async Task<String> GetContentEndpoint()
        {
            String endpoint = "";
            HttpResponseMessage response = await httpClient.GetAsync("content/getContentEndpoint"); 
            
            if (response.IsSuccessStatusCode)
            {
                endpoint = await response.Content.ReadAsAsync<String>();
                hbEventLog.WriteEntry("UpdateManager: Content endpoint received: " + endpoint);
            }
            else
            {
                //Throw an error
                hbEventLog.WriteEntry("UpdateManager: Failed to retrieve content endpoint. Server returned: " + response.StatusCode.ToString() + " " + response.ReasonPhrase);
            }

            return endpoint;
        }

        /*
         * Downloads the LATEST content and sig file from the Honeybee endpoint
         */
        public async Task<String> DownloadLatestContent(String endpoint)
        {
            String update_file_path = Path.GetTempFileName();
            String sig_file_path = update_file_path + ".sig";

            await webClient.DownloadFileTaskAsync(endpoint + "latest-windows.zip", update_file_path);
            hbEventLog.WriteEntry("UpdateManager: Downloaded newest content: " + update_file_path);

            //Also get the signature file
            await webClient.DownloadFileTaskAsync(endpoint + "latest-windows.zip.sig", sig_file_path);
            hbEventLog.WriteEntry("UpdateManager: Downloaded newest content signature: " + sig_file_path);

            //If no signature file throw an error

            //Verify signature
            //If fail throw an error
            if (!VerifyContentSignature(sig_file_path))
            {
                hbEventLog.WriteEntry("UpdateManager: Signature failed verification: " + sig_file_path);
            }

            return update_file_path;
        }

        /*
         * Verifies the .sig file downloaded with the LATEST content
         * Returns FALSE on verification failure
         */
        public bool VerifyContentSignature(String sig_path)
        {
            hbEventLog.WriteEntry("UpdateManager: Signature successfully verified: " + sig_path);
            return true;
        }

        /*
         * Uploads any local data to a Honeybee 
         */
        public void UploadData()
        {
            //Check /api/upload/getUploadEndpoint
            //If an endpoint is returned:
            //  Marshall data
            //  Upload data to endpoint
        }
    }
}
