using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniRisViewer.Domain
{
    public class DialogService:IDialogService 
    {
        public void ShowMessage(string message,string title)
        {
            MessageBox.Show(message,title);
        }
    }
}
