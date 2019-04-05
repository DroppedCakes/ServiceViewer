using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniRisViewer.Dialog
{
    public interface IDialogService
    {
        /// <summary>
        /// メッセージを表示する
        /// </summary>
        void ShowMessage(String msg);
    }
}
