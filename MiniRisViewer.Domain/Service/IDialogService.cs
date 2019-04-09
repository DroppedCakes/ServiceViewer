using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRisViewer.Domain
{
    public interface IDialogService
    {
        void ShowMessage(string message,string title);
    }
}
