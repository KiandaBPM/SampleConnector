using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleConnector.Models
{
    public class ConnectorResponse
    {
        public string signature { get; set; }
        public string encryptedSettingsPropertyBag { get; set; }
        public byte[] iv { get; set; }
        public QueryResult queryResult { get; set; }
    }

    public class QueryResult
    {
        public bool success { get; set; }
        public string message { get; set; }
        public JObject data { set; get; }
        public List<JObject> items { get; set; }
        public string resultCount { get; set; }
    }
}