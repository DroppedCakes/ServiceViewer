using Prism.Mvvm;

namespace MiniRisViewer.Domain.Model
{
    public class AppConfig : BindableBase
    {
        private bool hideMpps;

        public bool HideMpps
        {
            get { return hideMpps; }
            set { SetProperty(ref hideMpps, value); }
        }

        public AppConfig()
        {
        }
    }
}