using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace WebView2HttpServer.Http
{
    public static class WebHelper
    {
        public static string GetContentRef(string url)
        {
            var array = url.Split('/');
            return array.Length > 1 ? array[1] : array[0];
        }

        public static string ConvertUriToBackslash(string uri) => uri.Replace("/", "\\");

        public static string ConvertUriToSlash(string uri) => uri.Replace("\\", "/");

        public static async Task<string> WebViewInvokeScriptAsync(WebView webView, string function, bool canExecute, bool trackError, params string[] parametersArray)
        {
            string result = null;

            if (canExecute)
            {
                var newParametersArray = parametersArray.Select(parameter => $"'{parameter}'").ToList();
                var parameters = newParametersArray.Count < 1 ? "" : string.Join(",", newParametersArray);
                var eval = $"{function}({parameters});";

                try
                {
                    result = await webView.InvokeScriptAsync("eval", new[] { eval });
                }
                catch (Exception exception)
                {
                }
            }

            return result;
        }
    }
}
