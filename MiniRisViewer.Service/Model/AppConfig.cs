using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniRisViewer.Service.Model
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
