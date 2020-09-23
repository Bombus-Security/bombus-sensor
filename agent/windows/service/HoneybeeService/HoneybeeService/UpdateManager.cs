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
        HttpClient http_client;

        //The web client is used for downloading files
        WebClient web_client;

        String base_path;
        String content_path;

        System.Diagnostics.EventLog hbEventLog;

        public UpdateManager()
        {
            //Retrieve paths to our content and base directories
            base_path = (String)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Honeybee\\Agent", "AppPath", "");
            content_path = (String)Registry.GetValue("HKEY_LOCAL_MACHINE\\SOFTWARE\\Honeybee\\Agent", "ContentPath", base_path + "\\content");

            //The http client is used for calling API endpoints
            http_client = new HttpClient();

            http_client.BaseAddress = new Uri("https://hnyb.app/api/");
            http_client.DefaultRequestHeaders.Accept.Clear();
            http_client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            //The web client is used for downloading files
            web_client = new WebClient();

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
            hbEventLog.WriteEntry("UpdateManager: Checking for content updates.");
            //Get the timestamp/hash of our previous update
            int local_timestamp = GetLocalContentTimestamp();

            //Check /api/content/currentContentInfo
            int latest_timestamp = await GetLatestContentTimestamp();

            //If newer content available download from /content/LATEST
            if (latest_timestamp > local_timestamp)
            {
                hbEventLog.WriteEntry("UpdateManager: Newer content available. Attempting to download.");

                String endpoint = await GetContentEndpoint();
                String path_to_update = await DownloadLatestContent(endpoint);
            }
        }

        /*
         * Retrieves the update time from <content>/lastupdate stored as a unix timestamp
         */
        public int GetLocalContentTimestamp()
        {
            return Int32.Parse(File.ReadAllLines(content_path + "\\lastupdate")[0]);
        }

        /*
         * Gets the current latest update timestamp from Honeybee servers
         */
        public async Task<int> GetLatestContentTimestamp()
        {
            hbEventLog.WriteEntry("UpdateManager: Checking for latest content timestamp.");
  
            int unix_timestamp = 0;
            HttpResponseMessage response = await http_client.GetAsync("content/currentContentInfo");

            if (response.IsSuccessStatusCode)
            {
                unix_timestamp = await response.Content.ReadAsAsync<int>();
                hbEventLog.WriteEntry("UpdateManager: Got latest content timestamp: " + unix_timestamp);
            }
            else
            {
                hbEventLog.WriteEntry("UpdateManager: Could not retrieve latest content timestamp. Server returned: " + response.StatusCode.ToString() + " " + response.ReasonPhrase);
            }

            return unix_timestamp;
        }

        /*
         * Requests an endpoint from Honeybee servers to download content files from.
         */
        public async Task<String> GetContentEndpoint()
        {
            String endpoint = "";
            HttpResponseMessage response = await http_client.GetAsync("content/getContentEndpoint"); 
            
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

            await web_client.DownloadFileTaskAsync(endpoint + "latest-windows.zip", update_file_path);
            hbEventLog.WriteEntry("UpdateManager: Downloaded newest content: " + update_file_path);

            //Also get the signature file
            await web_client.DownloadFileTaskAsync(endpoint + "latest-windows.zip.sig", sig_file_path);
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
