using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WebView2WebResourceRquestedTest
{
    public partial class MainWindow : Window
    {
        private static string baseUrl = "appassets.html.example";
        private static string baseHttpUrl = $"http://{baseUrl}";

        public MainWindow()
        {
            this.InitializeComponent();

            WebView2.CoreWebView2InitializationCompleted += Browser_CoreWebView2InitializationCompleted;
            WebView2.Source = new Uri($"http://{baseUrl}/index.html");
        }

        private void Browser_CoreWebView2InitializationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2InitializationCompletedEventArgs e)
        {
            WebView2.CoreWebView2.SetVirtualHostNameToFolderMapping(baseUrl, "html", CoreWebView2HostResourceAccessKind.Allow);
            WebView2.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WebView2.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        }

        private async void OnWebResourceRequested(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            CoreWebView2Deferral deferral = e.GetDeferral();

            if (e.Request.Uri == "https://httpbin.org/custom")
            {
                e.Response = Custom();
            }
            else if (e.Request.Uri == "https://httpbin.org/get")
            {
                e.Response = await RedirectAsync(e.Request);
            }

            deferral.Complete();
        }

        private CoreWebView2WebResourceResponse Custom()
        {
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes("Moby"));
            var cwv2Response = WebView2.CoreWebView2.Environment.CreateWebResourceResponse(memoryStream, 200, "OK", "Content-Type: text/plain");
            cwv2Response.Headers.AppendHeader("Access-Control-Allow-Origin", baseHttpUrl);
            return cwv2Response;
        }

        private async Task<CoreWebView2WebResourceResponse> RedirectAsync(CoreWebView2WebResourceRequest request)
        {
            using (HttpRequestMessage httpreq = ConvertRequest(request))
            using (var client = new HttpClient())
            using (var response = await client.SendAsync(httpreq))
            {
                foreach (var header2 in response.Headers)
                {
                    string headerContent = string.Join(",", header2.Value.ToArray()); ;
                }               
                return await ConvertResponseAsync(response);
            }
        }

        private async Task<CoreWebView2WebResourceResponse> ConvertResponseAsync(HttpResponseMessage response)
        {
            //Set the response content to a class variable and hold it till Downloadstart
            //Stream stream = await response.Content.ReadAsStreamAsync();
            //var cwv2Response = this.WebView2.CoreWebView2.Environment.CreateWebResourceResponse(stream, (int)response.StatusCode, response.ReasonPhrase, "");

            //Copy the stream to a Memory Stream.
            var stream = await response.Content.ReadAsStreamAsync();
            MemoryStream memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            var cwv2Response = WebView2.CoreWebView2.Environment.CreateWebResourceResponse(memoryStream, (int)response.StatusCode, response.ReasonPhrase, "");

            //Default is what I would normally expect.
            //var stream = await response.Content.ReadAsStreamAsync();
            //var cwv2Response = this.WebView2.CoreWebView2.Environment.CreateWebResourceResponse(stream, (int)response.StatusCode, response.ReasonPhrase, "");

            foreach (var header in response.Headers)
            {
                string headerContent = string.Join(",", header.Value.ToArray());
                cwv2Response.Headers.AppendHeader(header.Key, headerContent);
            }
            return cwv2Response;
        }

        private HttpRequestMessage ConvertRequest(CoreWebView2WebResourceRequest request)
        {
            HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get, "https://httpbin.org/html");

            foreach (var header in request.Headers)
            {
                req.Headers.Add(header.Key, header.Value);
            }
            return req;
        }
    }
}
