using Microsoft.UI.Xaml.Controls;
using Microsoft.Web.WebView2.Core;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace ClassLibrary1
{
    public class WebView2Controller
    {
        private WebView2 WebView2 = new WebView2();
        private string baseUrl = "appassets.html.example";

        public WebView2Controller(Border WebView2Container)
        {
            WebView2Container.Child = WebView2;
            WebView2.CoreWebView2Initialized += OnCoreWebView2Initialized;
            WebView2.Source = new Uri($"http://{baseUrl}/index.html");
        }

        private void OnCoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            sender.CoreWebView2.SetVirtualHostNameToFolderMapping(baseUrl, "ClassLibrary1/html", CoreWebView2HostResourceAccessKind.Allow);
            sender.CoreWebView2.AddWebResourceRequestedFilter("*httpbin*", CoreWebView2WebResourceContext.All);
            sender.CoreWebView2.WebResourceRequested += OnWebResourceRequested;
            //sender.CoreWebView2.WebResourceResponseReceived += OnWebResourceResponseReceived;
        }

        public void OnWebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs e)
        {
            if (e.Request.Uri.ToString().Contains("httpbin.org"))
            {
                var def = e.GetDeferral();
                CoreWebView2WebResourceResponse newres = sender.Environment.CreateWebResourceResponse(
                    createStream("Moby"), 200, "OK", "Content-Type: text/plain");
                e.Response = newres;
                def.Complete();
                return;
            }
        }

        InMemoryRandomAccessStream createStream(string str)
        {
            InMemoryRandomAccessStream stream = new InMemoryRandomAccessStream();
            DataWriter writer = new DataWriter(stream);
            writer.WriteString(str);
            writer.StoreAsync().GetAwaiter().GetResult();
            return stream;
        }

        public async Task<string> ExecuteScriptAsync(string snippet)
        {
            return await WebView2.CoreWebView2.ExecuteScriptAsync(snippet);
        }

        //var client = new HttpClient();
        //var response = client.GetAsync(e.Request.Uri).GetAwaiter().GetResult();
        //var responseContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        //responseContent = responseContent.Replace("Moby-Dick", "Moby");

        //private void OnWebResourceResponseReceived(CoreWebView2 sender, CoreWebView2WebResourceResponseReceivedEventArgs args)
        //{
        //    System.Diagnostics.Debug.WriteLine("OnWebResourceResponseReceived");
        //    System.Diagnostics.Debug.WriteLine(args.Request.Uri);
        //    System.Diagnostics.Debug.WriteLine(args.Response.ReasonPhrase);
        //    System.Diagnostics.Debug.WriteLine(args.Response.StatusCode);
        //    foreach (var header in args.Response.Headers)
        //    {
        //        System.Diagnostics.Debug.WriteLine(header.Key);
        //        System.Diagnostics.Debug.WriteLine(header.Value);
        //    }
        //}

        //private async void OnWebResourceRequested(CoreWebView2 sender, CoreWebView2WebResourceRequestedEventArgs args)
        //{
        //    var deferral = args.GetDeferral();
        //    string fname = @"ClassLibrary1\Html\script2.js";
        //    StorageFolder InstallationFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
        //    StorageFile file = await InstallationFolder.GetFileAsync(fname);
        //    if (File.Exists(file.Path))
        //    {
        //        var path = file.Path;
        //        var contents = File.ReadAllBytes(file.Path);
        //        var stream = File.OpenRead(path);
        //        stream.Position = 0;
        //        args.Response = sender.Environment.CreateWebResourceResponse(stream.AsRandomAccessStream(), 200, "OK", "Content-Type: text/html");
        //    }
        //    deferral.Complete();
        //    return;
        //}
    }
}