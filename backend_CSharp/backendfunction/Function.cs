using Amazon.Lambda.Core;
using BackEnd;
using LitJson;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace BackendFunction
{
    public class BFunc
    {
        //public async Task<Stream> Function(Stream stream, ILambdaContext context) // Asynchronous type function declaration possible
                                                                                    // Use this when want to do asynchronous processing.
        public Stream Function(Stream stream, ILambdaContext context)
        {
            try
            {
                // Initialize BackendFunction API
                Backend.Initialize(ref stream);
            }
            catch (Exception e)
            {
                //Return the reason if Initializing BackendFunction API was failed
                return ReturnErrorObject("initialize " + e.ToString());
            }

            // TODO
            
            //Example
            Console.WriteLine(Backend.Content["value"].ToString()); // Write "Hello world" in degubConfig.json
            //await Task.Delay(100);                                // Optional - wait 100ms if asynchronous processing

            // If return the value by Stream type, the value will send to Backend SDK 
            return Backend.StringToStream("BackendFunction");
        }

        static Stream ReturnErrorObject(string err)
        {
            JObject error = new JObject();
            error.Add("error", err);

            return Backend.JsonToStream(error.ToString());
        }
    }
}
