using MiniRisViewer.Dialog;
using MiniRisViewer.Dialog.Service;
using Prism.Commands;
using System.Windows.Input;

namespace MiniRisViewer.Dialog
{
    public class DialogViewModel
    {
        IDialogService dialogService;
        DelegateCommand showMessageCommand;

        /// <summary>
        /// メッセージを表示するコマンド
        /// </summary>
        public ICommand ShowMessageCommand
        {
            get
            {
                if (showMessageCommand is null)
                {
                    showMessageCommand = new DelegateCommand(
                        () =>
                        {
                            dialogService?.ShowMessage("Dialog");
                        });
                }
                return showMessageCommand;
            }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public DialogViewModel(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }
    }
}
