using Prism.Mvvm;

namespace MiniRisViewer.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "ミニRIS監視プログラム";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        public MainWindowViewModel()
        {

        }
    }
}
