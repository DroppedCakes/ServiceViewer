using MahApps.Metro.Controls;
using Prism.Interactivity.InteractionRequest;
using System.Windows;

namespace Prism.Interactivity.MahAppsPack.DefaultPopupMetroWindows
{
    /// <summary>
    /// DefaultConfirmationMetroWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class DefaultConfirmationMetroWindow :  MetroWindow
    {
        public DefaultConfirmationMetroWindow()
        {
            InitializeComponent();
        }

        public IConfirmation Confirmation
        {
            get
            {
                return this.DataContext as IConfirmation;
            }
            set
            {
                this.DataContext = value;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Confirmation.Confirmed = false;
            this.Close();
        }
        }
    }
