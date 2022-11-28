using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebView2HttpServer.Http
{
    //GET /epub30-test-0100-20160116/EPUB/xhtml/front.xhtml HTTP/1.1\r\n
    //Referer: http://127.0.0.1:8080/readium-js/dev/index_RequireJS_single-bundle.html\r\n
    //Accept: text/html, */*; q=0.01\r\n
    //Accept-Language: en-US,en;q=0.8,it-IT;q=0.6,it;q=0.4,ru;q=0.2\r\n
    //X-Requested-With: XMLHttpRequest\r\n
    //Accept-Encoding: gzip, deflate\r\n
    //User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; WebView/3.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36 Edge/18.18362\r\n
    //Host: 127.0.0.1:8080\r\n
    //Connection: Keep-Alive\r\n
    //Cache-Control: no-cache\r\n
    //\r\n
    //{messageBody}
    public class Request
    {
        public string Method { get; set; }
        public string Uri { get; set; }
        public string HttpVersion { get; set; }

        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

        public string MessageBody { get; set; }

        public string BaseUri { get; set; }
        public string RelativeUri { get; set; }
        public string FileExtensions { get; set; } = "";

        public Request(string request)
        {
            Parse(request);
        }

        private void Parse(string request)
        {
            var requestArray = request.Split(Environment.NewLine);

            ParseRequestLine(requestArray[0]);
            if (requestArray.Length < 2) return;

            ParseHeaders(requestArray.Skip(1));

            //ParseMessageBody(requestArray.Skip(1));
        }

        private void ParseRequestLine(string requestLine)
        {
            var firstLineArr = requestLine.Split(' ');
            if (firstLineArr.Length > 0)
                Method = firstLineArr[0].Trim().ToUpper();
            if (firstLineArr.Length > 1)
                Uri = firstLineArr[1].Trim();
            if (firstLineArr.Length > 2)
                HttpVersion = firstLineArr[2].Trim();
            var rLen = Uri.Length;
            if (Uri.Substring(rLen - 1) == "?")
                Uri = Uri.Substring(0, rLen - 1);

            Uri = System.Uri.UnescapeDataString(Uri);
            RelativeUri = Uri.Substring(1);

            var requestUriArray = Uri.Split('/');
            if (requestUriArray.Length > 1)
                BaseUri = requestUriArray[1];

            var requestFileArray = Uri.Split('.');
            if (requestFileArray.Length > 1)
                FileExtensions = $".{requestFileArray.Last()}";
        }

        private void ParseHeaders(IEnumerable<string> requestArray)
        {
            foreach (var header in requestArray)
            {
                var headerArray = header.Split(':');
                if (headerArray.Length < 2) continue;

                var key = headerArray[0];
                var value = headerArray[1].Trim();
                Headers.Add(key, value);
            }
        }

        private void ParseMessageBody(IEnumerable<string> requestArray)
        {
            int index = 0;
            for (int i = 0; i < requestArray.Count(); i++)
            {
                var text = requestArray.ElementAt(i).Trim();
                if (string.IsNullOrEmpty(text))
                {
                    index = i;
                    break;
                }
            }

            if (index < 1) return;

            var bodyBuilder = new StringBuilder();
            foreach (var messageBody in requestArray.Skip(index + 1))
            {
                bodyBuilder.Append(messageBody.Trim() + Environment.NewLine);
            }
            MessageBody = System.Uri.UnescapeDataString(bodyBuilder.ToString());
        }
    }
}
