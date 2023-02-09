using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleConnector.Models
{
    public class ConnectorRequest
    {

        public string subscriptionId { get; set; }
        public string userId { get; set; }
        public string requestId { get; set; }
        public string encryptedSettingsPropertyBag { get; set; }
        public byte[] iv { get; set; }
        public Query query { get; set; }
    }

    public class Query
    {
        public Query()
        {
            fields = new List<string>();
            conditions = new List<JObject>();
            mappings = new Dictionary<string, object>();
        }

        public string id { get; set; }
        public string action { get; set; }
        public JObject info { get; set; }
        public string orderBy { get; set; }
        public bool orderAscending { get; set; }
        public string paging { get; set; }
        public int rowLimit { get; set; }
        public List<string> fields { get; set; }
        public List<JObject> conditions { get; set; }
        public Dictionary<string, object> mappings { get; set; }
        public string filter { get; set; }
        public string filterBy { get; set; }
        public string filterMode { get; set; }
    }

}