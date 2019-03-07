using System;
using System.ServiceProcess;
using System.Windows.Data;

namespace MiniRisViewer.ServiceStatus.Helper
{
    [ValueConversion(typeof(ServiceControllerStatus), typeof(String))]
    public class StatusConverter : IValueConverter
    {
        public string ConvertStatus;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int target = (int)value;

            switch (target)
            {
                case (1):
                    ConvertStatus = "停止中";
                    break;

                case (2):
                    ConvertStatus = "サービス開始させています";
                    break;

                case (3):
                    ConvertStatus = "停止させています";
                    break;

                case (4):
                    ConvertStatus = "動作中";
                    break;

                case (5):
                    ConvertStatus = "再開させています";
                    break;

                case (6):
                    ConvertStatus = "一時停止させています";
                    break;

                case (7):
                    ConvertStatus = "一時停止中";
                    break;

                default:
                    ConvertStatus = "不明";
                    break;
            }
            return ConvertStatus;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}