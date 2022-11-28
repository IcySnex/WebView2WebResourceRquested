using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace WebView2HttpServer.Http
{
    public static class StreamHelper
    {
        public static async Task<string> StreamToString(IInputStream inputStream)
        {
            DataReader sr = new DataReader(inputStream) { InputStreamOptions = InputStreamOptions.Partial };

            await sr.LoadAsync(1024);
            return sr.ReadString(sr.UnconsumedBufferLength);
        }

        public static async Task WriteStreamToStream(IRandomAccessStream inputStream, Stream outputStream)
        {
            var input = inputStream.AsStreamForRead();
            await WriteStreamToStream(input, outputStream);
        }

        public static async Task WriteStreamToStream(Stream inputStream, Stream outputStream)
        {
            if (inputStream.Length == 0)
            {
            }
            else if (outputStream.CanWrite)
            {
                await inputStream.CopyToAsync(outputStream);
                await outputStream.FlushAsync();
            }
            else
            {
            }
        }

        public static async Task WriteStringToStream(string text, Stream outputStream)
        {
            var textArray = Encoding.UTF8.GetBytes(text);
            if (outputStream.CanWrite)
            {
                await outputStream.WriteAsync(textArray, 0, textArray.Length);
            }
        }

        public static Stream StringToStream(string src)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes(src);
            return new MemoryStream(byteArray);
        }
    }
}