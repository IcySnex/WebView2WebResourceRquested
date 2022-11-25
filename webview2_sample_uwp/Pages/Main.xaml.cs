using ClassLibrary1;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace WebView2_UWP
{
    public sealed partial class MainPage : Page
    {
        private WebView2Controller WebView2Controller;

        private Dictionary<string, string> codeSnippets = new Dictionary<string, string>()
        {
            {"Update header", "document.getElementById(\"header\").innerHTML = \"New Header\";" },
            {"Remove the body background", "list = document.getElementsByTagName(\"body\")[0].classList;\nlist.remove(\"webviewhero_bg\");" },
            {"Add the body background", "list = document.getElementsByTagName(\"body\")[0].classList;\nlist.add(\"webviewhero_bg\");" },
            {"Show an alert", "window.alert(\"Alert message\");" },
            {"Retrieve text from the content area", "document.getElementById(\"content\").innerHTML;" },
            {"Call a function and get the return value", "Add(9, 3);" },
        };

        public MainPage()
        {
            this.InitializeComponent();
            WebView2Controller = new WebView2Controller(WebView2Container);
        }

        private void OnCodeSnippetChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CodeSnippetsCombo.SelectedItem != null)
            {
                var snippet = ((KeyValuePair<string, string>)CodeSnippetsCombo.SelectedItem).Value;
                CodeSnippetTextBox.Text = snippet;
            }
        }

        private async void OnExecuteJavascriptButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string snippet = CodeSnippetTextBox.Text;
            string scriptResult = await WebView2Controller.ExecuteScriptAsync(snippet);

            if (scriptResult != null)
            {
                ReturnedValueTextBox.Text = scriptResult;
            }
        }
    }
}