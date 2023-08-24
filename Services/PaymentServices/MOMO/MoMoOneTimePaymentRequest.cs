using System.Net;

namespace API_Test1.Services.PaymentServices.MOMO
{
    public class MoMoOneTimePaymentRequest
    {
        public MoMoOneTimePaymentRequest()
        {
        }
        public static string sendPaymentRequest(string endpoint, string postJsonString)
        {
            try
            {
                HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create(endpoint);

                var postData = postJsonString;

                var data = Encoding.UTF8.GetBytes(postData);

                httpWReq.ProtocolVersion = HttpVersion.Version11;
                httpWReq.Method = "POST";
                httpWReq.ContentType = "application/json";

                httpWReq.ContentLength = data.Length;
                httpWReq.ReadWriteTimeout = 30000;
                httpWReq.Timeout = 15000;

                // Thêm dòng mã sau để cho phép truy cập từ miền khác
                httpWReq.Headers.Add("Access-Control-Allow-Origin", "http://localhost:5070");

                Stream stream = httpWReq.GetRequestStream();
                stream.Write(data, 0, data.Length);
                stream.Close();

                HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();

                string jsonresponse = "";

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    string temp = null;
                    while ((temp = reader.ReadLine()) != null)
                    {
                        jsonresponse += temp;
                    }
                }

                //todo parse it
                return jsonresponse;
                //return new MomoResponse(mtid, jsonresponse);
            }
            catch (WebException e)
            {
                return e.Message;
            }
        }
    }
}
