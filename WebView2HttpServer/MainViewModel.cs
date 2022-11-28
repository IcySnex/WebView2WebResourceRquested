using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Threading.Tasks;

namespace WebView2HttpServer
{
    public class MainViewModel : ObservableObject
    {
        private Uri _webViewUri = new Uri("about:blank");

        public Uri WebViewUri
        {
            get => _webViewUri;
            set => SetProperty(ref _webViewUri, value);
        }

        public async Task Initialize()
        {
            //WebViewUri = GetAbsoluteUri();
        }
    }
}