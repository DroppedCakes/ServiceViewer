using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiniRisViewer.Dialog.Service
{

    /// <summary>
    /// IDialogService実装（メッセージ表示のみ）
    /// </summary>
    public class DialogService : IDialogService
    {
        /// <summary>
        /// メッセージを表示する
        /// </summary>
        public void ShowMessage(String msg)
        {
            MessageBox.Show(msg);
        }
    }

    /// <summary>
    /// IDialogService実装(確認ボタンも表示)
    /// </summary>
    public class ConfirmDialogService : IDialogService
    {
        /// <summary>
        /// メッセージを表示する
        /// </summary>
        public void ShowMessage(String msg)
        {
            MessageBox.Show(msg, "確認", MessageBoxButton.YesNo);
        }
    }
}
