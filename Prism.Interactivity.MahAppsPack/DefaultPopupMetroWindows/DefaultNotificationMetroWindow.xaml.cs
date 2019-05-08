using MahApps.Metro.Controls;
using Prism.Interactivity.InteractionRequest;
using System.Windows;

namespace Prism.Interactivity.MahAppsPack.DefaultPopupMetroWindows
{
    /// <summary>
    /// DefaultNotificationMetroWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DefaultNotificationMetroWindow : MetroWindow
    {
        public DefaultNotificationMetroWindow()
        {
            InitializeComponent();
        }

        public INotification Notification
        {
            get
            {
                return this.DataContext as INotification;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
