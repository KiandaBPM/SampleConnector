using SampleConnector.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Kianda;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Web;

namespace SampleConnector.Controllers
{
    public class ConnectorController : ApiController
    {
        [Route("api/test")]
        [HttpPost]
        public ConnectorResponse Test(ConnectorRequest connectorRequest)
        {
            var settings = Kianda.Helper.GetSettings(connectorRequest);
            //Implemention goes here


            return new ConnectorResponse() { queryResult = new QueryResult() { success = true }, signature = Helper.HashWithHMACSHA256(connectorRequest.requestId) };
        }

        [Route("api/metadata")]
        [HttpPost]
        public ConnectorResponse Metadata(ConnectorRequest connectorRequest)
        {
            var settingsPlain = Helper.AESDecrypt(connectorRequest.encryptedSettingsPropertyBag, connectorRequest.iv);

            var settingsObj = JsonConvert.DeserializeObject<JObject>(settingsPlain);
            //Implemention goes here
            // The Tree structure can be created dynamically here 
            // an example of the tree structure is returned from this request
            var sample = Helper.GetSampleTree();

            return new ConnectorResponse() { queryResult = new QueryResult() { success = true, items = sample }, signature = Helper.HashWithHMACSHA256(connectorRequest.requestId) };
        }


        [Route("api/query")]
        [HttpPost]
        public ConnectorResponse Query(ConnectorRequest connectorRequest)
        {
            var settingsPlain = Helper.AESDecrypt(connectorRequest.encryptedSettingsPropertyBag, connectorRequest.iv);

            var settingsObj = JsonConvert.DeserializeObject<JObject>(settingsPlain);
            Query q = connectorRequest.query;

            var text = connectorRequest.query.info.GetValue("text");
            var mappings = q.info.GetValue("mappings"); // Retrieve input and output mappings
            var inputMapping = mappings.Values("inputmappings");
            ConnectorResponse resp = new ConnectorResponse();
            resp.queryResult = new QueryResult();
            resp.queryResult.success = true;
            resp.queryResult.message = "You made a request to  " + text + " with the following input mappings: " + inputMapping;
            List<JObject> resultItems = new List<JObject>();
            resultItems.Add(new JObject { ["SampleExportFieldName"] = "This is your result mapping " }); // use the names of the output fields for mapping results 
            resp.queryResult.items = resultItems;

            resp.signature = Helper.HashWithHMACSHA256(connectorRequest.requestId);

            byte[] IV;
            resp.encryptedSettingsPropertyBag = Helper.AESEncrypt(JsonConvert.SerializeObject(settingsObj), out IV);
            resp.iv = IV;


            return resp;
            //return new ConnectorResponse() { queryResult = new QueryResult() { success = true, data = { "You made a request to  "+ text + " with the following input mappings: " + inputMapping } }, signature = Helper.HashWithHMACSHA256(connectorRequest.requestId) , encryptedSettingsPropertyBag= Helper.AESEncrypt(JsonConvert.SerializeObject(settingsObj), out IV ), iv=IV };
        }
    }
}