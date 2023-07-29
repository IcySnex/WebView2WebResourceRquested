﻿using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace WebView2WebResourceRquestedSample
{
    public sealed partial class MainPage : Page
    {
        private static string baseUrl = "appassets.html.example";
        private static string baseHttpUrl = $"http://{baseUrl}";

        public MainPage()
        {
            this.InitializeComponent();

            WebView2.CoreWebView2Initialized += OnCoreWebView2Initialized;
            WebView2.Source = new Uri($"http://{baseUrl}/index.html");
        }

        private void OnCoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            WebView2.CoreWebView2.SetVirtualHostNameToFolderMapping(baseUrl, "html", CoreWebView2HostResourceAccessKind.Allow);
            WebView2.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
            WebView2.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
        }

        private async void OnWebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            var deferral = e.GetDeferral();

            if (e.Request.Uri == "https://httpbin.org/custom")
            {
                e.Response = await CustomASync();
            }
            else if (e.Request.Uri == "https://httpbin.org/get")
            {
                e.Response = await RedirectAsync(e.Request);
            }

            deferral.Complete();
        }

        private async Task<CoreWebView2WebResourceResponse> CustomASync()
        {
            var randomMemoryStream = new InMemoryRandomAccessStream();
            await randomMemoryStream.WriteAsync(Encoding.UTF8.GetBytes("Moby").AsBuffer());
            var cwv2Response = WebView2.CoreWebView2.Environment.CreateWebResourceResponse(randomMemoryStream, 200, "OK", "Content-Type: text/plain");
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
            var randomMemoryStream = new InMemoryRandomAccessStream();
            await RandomAccessStream.CopyAsync(stream.AsInputStream(), randomMemoryStream);
            var cwv2Response = WebView2.CoreWebView2.Environment.CreateWebResourceResponse(randomMemoryStream, (int)response.StatusCode, "OK", "");

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
