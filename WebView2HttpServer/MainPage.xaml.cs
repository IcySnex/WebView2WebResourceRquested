using WebView2HttpServer.Http;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WebView2HttpServer
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Initialize();
            
            //WebView.Navigate(Server.HomeUrl);
            WebView2.Source = Server.HomeUrl;

            //await InitializeWebServer();
        }

        public MainViewModel ViewModel { get; } = new MainViewModel();

        //private async Task InitializeWebServer()
        //{
        //    var configuration = new HttpServerConfiguration()
        //        .ListenOnPort(8800)
        //        .RegisterRoute(new StaticFileRouteHandler(@"Web"))
        //        .EnableCors(); // allow cors requests on all origins
        //                       //.EnableCors(x => x.AddAllowedOrigin("http://specificserver:<listen-port>"));

        //    var httpServer = new HttpServer(configuration);
        //    _httpServer = httpServer;

        //    // Don't release deferral, otherwise app will stop
        //}
    }
}
