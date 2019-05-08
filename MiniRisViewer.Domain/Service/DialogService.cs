using System.Windows;

namespace MiniRisViewer.Domain
{
    public class DialogService : IDialogService
    {
        public void ShowMessage(string message, string title)
        {
            MessageBox.Show(message, title);
        }
    }
}