using System;
using System.Globalization;
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
                case 1:
                    ConvertStatus = "停止中";
                    break;

                case 2:
                    ConvertStatus = "サービス開始させています";
                    break;

                case 3:
                    ConvertStatus = "停止させています";
                    break;

                case 4:
                    ConvertStatus = "動作中";
                    break;

                case 5:
                    ConvertStatus = "再開させています";
                    break;

                case 6:
                    ConvertStatus = "一時停止させています";
                    break;

                case 7:
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


    /// <summary>
    //ToggleButton（IsChecked）のため、実行中(4)以外はfalseとする
    /// <summary>
    [ValueConversion(typeof(ServiceControllerStatus), typeof(Boolean))]
    public class SimpleStatus : IValueConverter
    {
        public Boolean  Simple;
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int target = (int)value;

            if (target != 4)
            {
                Simple = false;
                if(target == 2) { 
                 Simple = true;
                }
            }
            else
{
                Simple = true;
            }
            return Simple;
        }

        //ToggleButtonの入力はtrue,falseのため、booleanで返す
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Boolean  target = (Boolean )value;
            Simple = target;
            return Simple;
        }

    }

    /// <summary>
    //ToggleButtonとpase表示の切り替えのため、実行中,停止中以外はToggleButton非表示、pose表示とする
    /// <summary>
    [ValueConversion(typeof(ServiceControllerStatus), typeof(String ))]
    public class DispButton : IValueConverter
    {
        public string DispButtonVisibility;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int target = (int)value;


            if ((target ==1)|| (target == 4))
            {
                DispButtonVisibility = "Visible";

            }
            else
            {
                DispButtonVisibility = "Collapsed";

            }
            return DispButtonVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String target = (string)value;
            DispButtonVisibility = target;
            return DispButtonVisibility;
        }
    }

    /// <summary>
    //ToggleButtonとpase表示の切り替えのため、実行中,停止中以外はToggleButton非表示、pose表示とする
    /// <summary>
    [ValueConversion(typeof(ServiceControllerStatus), typeof(String))]
    public class DispPose : IValueConverter
    {
        public string DispPoseVisibility;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int target = (int)value;


            if ((target == 1) || (target == 4))
            {
                DispPoseVisibility = "Collapsed";

            }
            else
            {
                DispPoseVisibility = "Visible";

            }
            return DispPoseVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            String target = (string)value;
            DispPoseVisibility = target;
            return DispPoseVisibility;
        }
    }

}