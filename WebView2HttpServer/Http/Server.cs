using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace WebView2HttpServer.Http
{
    public class Server : IDisposable
    {
        private static string _baseUri = "http://127.0.0.1";
        private static string _basePort = "8800";
        private static string _address => $"{_baseUri}:{Port}";
        private static string _webFolder = "Web";
        private static string _indexUri = $"{_webFolder}/index.html";

        private const int Port = 8080;
        private readonly StreamSocketListener _listener;
        //private readonly IReadiumFileResolver _readiumFileResolver;

        public Server()
        {
            //_readiumFileResolver = readiumFileResolver;
            _listener = new StreamSocketListener();
            _listener.ConnectionReceived += async (s, e) => await ProcessRequestAsync(e.Socket);
        }

        public static Uri HomeUrl => new Uri($"{_address}/{_indexUri}");

        public async Task Start() => await _listener.BindServiceNameAsync(Port.ToString());

        public void Dispose()
        {
            _listener.Dispose();
        }

        private async Task ProcessRequestAsync(StreamSocket socket)
        {
            // this works for text only
            var requestString = await StreamHelper.StreamToString(socket.InputStream);
            var request = new Request(requestString);

            //using (IOutputStream output = socket.OutputStream)
            //{
            if (request.Method == HttpMethod.Get.Method)
            {
                await ProcessRequest(request, socket.OutputStream);
            }
            else if (request.Method == HttpMethod.Head.Method)
            {
                await ProcessRequest(request, socket.OutputStream);
            }
            else
            {
                throw new InvalidDataException($"HTTP method not supported: {request.Method}");
            }
            //}
            // TODO: close all the streams
        }

        private async Task ProcessRequest(Request request, IOutputStream outputStream)
        {
            if (request.BaseUri == _webFolder)
            {
                await ProcessInternalRequest(request, outputStream);
            }
            else
            {
                await ProcessExternalRequest(request, outputStream);
            }
        }

        private async Task ProcessInternalRequest(Request request, IOutputStream outputStream)
        {
            try
            {
                var uri = WebHelper.ConvertUriToBackslash(request.Uri);
                var fileOrFolder = await Package.Current.InstalledLocation.TryGetItemAsync(uri);

                if (fileOrFolder is StorageFile file)
                {
                    await WriteResponseAsync(await file.OpenStreamForReadAsync(), file.FileType, outputStream);
                }
                else
                {
                    await WriteEmptyResponseAsync(request.FileExtensions, outputStream);
                }
            }
            catch
            {
                await WriteEmptyResponseAsync(request.FileExtensions, outputStream);
            }
        }

        private async Task ProcessExternalRequest(Request request, IOutputStream outputStream)
        {
            try
            {
                var stream = StreamHelper.StringToStream("Bello");
                await WriteResponseAsync(stream, request.FileExtensions, outputStream);
            }
            catch
            {
                await WriteEmptyResponseAsync(request.FileExtensions, outputStream);
            }
        }

        private async Task WriteResponseAsync(Stream inputStream, string fileExtension, IOutputStream outputStream)
        {
            using (var writeOutputStream = outputStream.AsStreamForWrite())
            {
                await WriteResponseHeaderAsync(fileExtension, inputStream.Length, writeOutputStream);
                await StreamHelper.WriteStreamToStream(inputStream, writeOutputStream);
            }
        }

        private async Task WriteEmptyResponseAsync(string fileExtension, IOutputStream outputStream)
        {
            IRandomAccessStream inputStream = new InMemoryRandomAccessStream();
            using (var writeOutputStream = outputStream.AsStreamForWrite())
            {
                await WriteResponseHeaderAsync(fileExtension, (long)inputStream.Size, writeOutputStream);
                if (!string.IsNullOrEmpty(fileExtension))
                {
                    await StreamHelper.WriteStreamToStream(inputStream, writeOutputStream);
                }
            }
        }

        private async Task WriteResponseHeaderAsync(string fileExtension, long contentLength, Stream outputStream)
        {
            var contentType = MimeType.Mappings.TryGetValue(fileExtension, out var mime) ? mime : "unknown/unknown";
            string httpHeader = new HttpHeader(contentLength, contentType).Get();
            await StreamHelper.WriteStringToStream(httpHeader, outputStream);
        }
    }
}