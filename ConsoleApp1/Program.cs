using System;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    public class University
    {
        private const string Root = "{\"domains\":[\"marywood.edu\"],\"country\":\"United States\",\"alpha_two_code\":\"US\",\"state-province\":null,\"web_pages\":[\"http://www.marywood.edu\"],\"name\":\"Marywood University\"}";

        [JsonProperty("domains")]
        public string[] Domains { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("alpha_two_code")]
        public string AlphaTwoCode { get; set; }

        [JsonProperty("state-province")]
        public string StateProvince { get; set; }

        [JsonProperty("web_pages")]
        public string[] WebPages { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            string currentdatetime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string LogFolder = @"D:\Files\Logs\";

            try
            {
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var json = "";
                using (WebClient wc = new WebClient())
                {
                    json = wc.DownloadString("http://universities.hipolabs.com/search?country=United+States");
                }

                University[] universities = JsonConvert.DeserializeObject<University[]>(json);

                foreach (University university in universities)
                {
                   
                    string connectionString = @"Data Source = DESKTOP-0BKAGA1; Initial Catalog = JSON_DATA; Integrated Security = True; ";
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        // Open the connection
                        connection.Open();

                        // Create the insert statement
                        string insertStatement = "INSERT INTO JSON_DATA(Name, Country, AlphaTwoCode) " +
    $"VALUES('{university.Name.Replace("'", "''")}', '{university.Country.Replace("'", "''")}', '{university.AlphaTwoCode}')";

                        // Create a SqlCommand object with the insert statement and the SqlConnection object
                        SqlCommand command = new SqlCommand(insertStatement, connection);

                        // Execute the insert statement
                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }

            catch (Exception exception)
            {
                using (StreamWriter sw = File.CreateText(LogFolder + "\\" + "ErrorLog_" + currentdatetime + ".log"))
                {
                    sw.WriteLine(exception.ToString());
                }
            }
        }
    }
}
