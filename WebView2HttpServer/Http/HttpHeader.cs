using System;
using System.Collections.Generic;
using System.Text;

namespace WebView2HttpServer.Http
{
    public class HttpHeader
    {
        private readonly Dictionary<string, string> _headersDictionary;

        public HttpHeader(long contentLength, string contentType)
        {
            _headersDictionary = new Dictionary<string, string>
            {
                {"", "HTTP/1.1 200 OK"},
                {"Cache-control", "no-cache"},
                {"Date", $"{DateTime.Now:R}"},
                {"Server", "MeoGoEmbedded/1.0"},
                {"Content-Length", $"{contentLength}"},
                {"Content-Type", $"{contentType}"},
                {"Connection", "close"}
            };
        }

        public string Get()
        {
            StringBuilder headers = new StringBuilder();
            foreach (var header in _headersDictionary)
            {
                var key = string.IsNullOrEmpty(header.Key) ? "" : $"{header.Key}: ";
                headers.Append($"{key}{header.Value}\r\n");
            }
            headers.Append("\r\n");

            return headers.ToString();
        }
    }
}
