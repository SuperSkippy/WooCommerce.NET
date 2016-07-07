using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WooCommerceNET
{
    public class RestAPI
    {
        private string wc_url = string.Empty;
        private string wc_key = "";
        private string wc_secret = "";
        //private bool wc_Proxy = false;

        public RestAPI(string url, string key, string secret)//, bool useProxy = false)
        {
            wc_url = url;
            wc_key = key;
            if (url.ToLower().Contains("wc-api/v3") && !wc_url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                wc_secret = secret + "&";
            else
                wc_secret = secret;

            //wc_Proxy = useProxy;
        }

        /// <summary>
        /// Make Restful calls
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <param name="method">HEAD, GET, POST, PUT, PATCH, DELETE</param>
        /// <param name="requestBody">If your call doesn't have a body, please pass string.Empty, not null.</param>
        /// <param name="parms"></param>
        /// <returns>json string</returns>
        public async Task<string> SendHttpClientRequest<T>(string endpoint, HttpMethod method, T requestBody, Dictionary<string, string> parms = null)
        {
            HttpRequestMessage httpWebRequest = new HttpRequestMessage();
            try
            {
                HttpClient hc=new HttpClient();
                httpWebRequest.RequestUri = new Uri(wc_url + GetOAuthEndPoint(method.ToString(), endpoint, parms));

               
                if (wc_url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
                {
                    string credentials=Convert.ToBase64String(Encoding.UTF8.GetBytes(string.Format("{0}:{1}",wc_key , wc_secret)));

                    httpWebRequest.Headers.Authorization=new AuthenticationHeaderValue("Basic", credentials);
                }

                // start the stream immediately
                httpWebRequest.Method = method;
                
                //httpWebRequest.AllowReadStreamBuffering = false;
                //if (wc_Proxy)
                //    httpWebRequest.Proxy.Credentials = CredentialCache.DefaultCredentials;
                //else
                //    httpWebRequest.Proxy = null;
                
                if (requestBody.GetType() != typeof(string))
                {
                    var buffer = UTF8Encoding.UTF8.GetBytes(SerializeJSon(requestBody));
                    httpWebRequest.Content = new ByteArrayContent(buffer,0,buffer.Length);
                    httpWebRequest.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                }

                HttpResponseMessage response = await hc.SendAsync(httpWebRequest);

                // asynchronously get a response

                return await response.Content.ReadAsStringAsync();

            }
            /*catch (WebException we)
            {
                if (httpWebRequest != null && httpWebRequest.HaveResponse)
                    if (we.Response != null)
                        throw new Exception(await GetStreamContent(we.Response.GetResponseStream(), we.Response.ContentType.Split('=')[1]));
                    else
                        throw we;
                else
                    throw we;
            }*/
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> GetRestful(string endpoint, Dictionary<string, string> parms = null)
        {
            return await SendHttpClientRequest(endpoint, HttpMethod.Get, string.Empty, parms);
        }

        public async Task<string> PostRestful(string endpoint, object jsonObject, Dictionary<string, string> parms = null)
        {
            return await SendHttpClientRequest(endpoint, HttpMethod.Post, jsonObject, parms);
        }

        public async Task<string> PutRestful(string endpoint, object jsonObject, Dictionary<string, string> parms = null)
        {
            return await SendHttpClientRequest(endpoint, HttpMethod.Put, jsonObject, parms);
        }

        public async Task<string> DeleteRestful(string endpoint, Dictionary<string, string> parms = null)
        {
            return await SendHttpClientRequest(endpoint, HttpMethod.Delete, string.Empty, parms);
        }

        private string GetOAuthEndPoint(string method, string endpoint, Dictionary<string, string> parms = null)
        {
            if (wc_url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                if (parms == null)
                    return endpoint;
                else
                {
                    string requestParms = string.Empty;
                    foreach (var parm in parms)
                        requestParms += parm.Key + "=" + parm.Value + "&";

                    return endpoint + "?" + requestParms.TrimEnd('&');
                }
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("oauth_consumer_key", wc_key);
            dic.Add("oauth_nonce", Common.GetSHA1(Common.GetUnixTime(true)));
            dic.Add("oauth_signature_method", "HMAC-SHA256");
            dic.Add("oauth_timestamp", Common.GetUnixTime(false));

            if (parms != null)
                foreach (var p in parms)
                    dic.Add(p.Key, p.Value);

            string base_request_uri = System.Uri.EscapeDataString(wc_url + endpoint).Replace("%2f", "%2F").Replace("%3a", "%3A");
            string stringToSign = string.Empty;

            foreach (var parm in dic.OrderBy(x => x.Key))
                stringToSign += parm.Key + "%3D" + parm.Value + "%26";

            base_request_uri = method.ToUpper() + "&" + base_request_uri + "&" + stringToSign.Substring(0, stringToSign.Length - 3);

            base_request_uri = base_request_uri.Replace(",", "%252C").Replace("[", "%255B").Replace("]", "%255D");

            Common.DebugInfo.Append(base_request_uri);

            stringToSign += "oauth_signature%3D" + Common.GetSHA256(wc_secret, base_request_uri).Replace(",", "%252C").Replace("[", "%255B").Replace("]", "%255D").Replace("=", "%3D");

            dic.Add("oauth_signature", Common.GetSHA256(wc_secret, base_request_uri).Replace(",", "%252C").Replace("[", "%255B").Replace("]", "%255D").Replace("=", "%3D"));

            string parmstr = string.Empty;
            foreach (var parm in dic)
                parmstr += parm.Key + "=" + System.Uri.EscapeDataString(parm.Value) + "&";


            return endpoint + "?" + parmstr.TrimEnd('&');

        }

        public static string SerializeJSon<T>(T t)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings()
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ssZ"),
                UseSimpleDictionaryFormat = true
            };
            MemoryStream stream = new MemoryStream();
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T), settings);
            ds.WriteObject(stream, t);
            byte[] data = stream.ToArray();
            string jsonString = Encoding.UTF8.GetString(data, 0, data.Length);
            if (typeof(T).IsArray)
                jsonString = "{\"" + typeof(T).Name.ToLower().Replace("[]", "s") + "\":" + jsonString + "}";
            else
                jsonString = "{\"" + typeof(T).Name.ToLower() + "\":" + jsonString + "}";

            stream.Dispose();

            return jsonString;
        }

        public static T DeserializeJSon<T>(string jsonString)
        {
            DataContractJsonSerializerSettings settings = new DataContractJsonSerializerSettings()
            {
                DateTimeFormat = new DateTimeFormat("yyyy-MM-ddTHH:mm:ssZ"),
                UseSimpleDictionaryFormat = true
            };
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T), settings);
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(stream);
            stream.Dispose();

            return obj;
        }
    }

    public enum RequestMethod
    {
        HEAD = 1,
        GET = 2,
        POST = 3,
        PUT = 4,
        PATCH = 5,
        DELETE = 6
    }

    public static class HttpWebRequestExtensions
    {
        public static Task<Stream> GetRequestStreamAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<Stream>();

            try
            {
                request.BeginGetRequestStream(iar =>
               {
                   try
                   {
                       var response = request.EndGetRequestStream(iar);
                       tcs.SetResult(response);
                   }
                   catch (Exception exc)
                   {
                       tcs.SetException(exc);
                   }
               }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }

        public static Task<HttpWebResponse> GetResponseAsync(this HttpWebRequest request)
        {
            var tcs = new TaskCompletionSource<HttpWebResponse>();

            try
            {
                request.BeginGetResponse(iar =>
                {
                    try
                    {
                        var response = (HttpWebResponse)request.EndGetResponse(iar);
                        tcs.SetResult(response);
                    }
                    catch (Exception exc)
                    {
                        tcs.SetException(exc);
                    }
                }, null);
            }
            catch (Exception exc)
            {
                tcs.SetException(exc);
            }

            return tcs.Task;
        }
    }
}
