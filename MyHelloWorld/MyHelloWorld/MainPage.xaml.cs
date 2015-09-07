using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace MyHelloWorld
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            HelloText.Text = string.Format("Hello, {0}!  Glad to meet you!", EnteredName.Text);
        }
    }
}
