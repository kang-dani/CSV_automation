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
                // 백엔드 초기화
                Backend.Initialize(ref stream);

                // 요청 본문 파싱
                string requestBody;
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) requestBody = requestBody = reader.ReadToEnd();

                // JSON 파싱
                JsonData data = JsonMapper.ToObject(requestBody);
                string tableName = data["table"].ToString();
                JsonData rowList = data["rows"];

                int successCount = 0;
                int failCount = 0;

                foreach (JsonData row in rowList)
                {
                    Param param = new Param();

                    foreach (string key in row.Keys)
                    {
                        param.Add(key, row[key]);
                    }

                    var result = Backend.GameData.Insert(tableName, param);

                    if (result.IsSuccess()) successCount++;
                    else failCount++;
                }

                var responseJson = new JsonData();
                responseJson["SuccessCount"] = successCount;
                responseJson["failCount"] = failCount;

                return ToStream(responseJson.ToJson());

            }
            catch (Exception e)
            {
                var errorJson = new JsonData();
                errorJson["error"] = e.ToString();
                return ToStream(errorJson.ToJson());
            }

        }

        // 응답 문자열 Stream 변환 함수
        private Stream ToStream(string s)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(s));
        }
    }
}
