using MahApps.Metro.Controls;
using Prism.Interactivity.InteractionRequest;
using Prism.Interactivity.MahAppsPack.DefaultPopupMetroWindows;
using System.Collections.ObjectModel;
using System.Windows;

namespace Prism.Interactivity.MahAppsPack
{
    public class PopupMetroWindowAction : PopupWindowAction
    {
        #region MetroStyle
        public ResourceDictionary MetroStyle
        {
            get { return (ResourceDictionary)GetValue(MetroStyleProperty); }
            set { SetValue(MetroStyleProperty, value); }
        }
        public static readonly DependencyProperty MetroStyleProperty =
            DependencyProperty.Register("MetroStyle", typeof(ResourceDictionary), typeof(PopupMetroWindowAction), new PropertyMetadata(null));
        #endregion
        #region Accent
        public Accents? Accent
        {
            get { return (Accents?)GetValue(AccentProperty); }
            set { SetValue(AccentProperty, value); }
        }
        public static readonly DependencyProperty AccentProperty =
            DependencyProperty.Register("Accent", typeof(Accents?), typeof(PopupMetroWindowAction), new PropertyMetadata(null));
        #endregion
        #region Theme
        public Themes? Theme
        {
            get { return (Themes?)GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        public static readonly DependencyProperty ThemeProperty =
            DependencyProperty.Register("Theme", typeof(Themes?), typeof(PopupMetroWindowAction), new PropertyMetadata(null));
        #endregion

        protected override Window CreateWindow()
        {
            MetroWindow window = new DefaultMetroWindow();
            this.SetMetroStyle(window);
            return window;
        }
        protected override Window CreateDefaultWindow(INotification notification)
        {
            MetroWindow window = null;
            if (notification is IConfirmation) window = new DefaultConfirmationMetroWindow() { Confirmation = (IConfirmation)notification };
            else window = new DefaultNotificationMetroWindow() { Notification = notification };
            this.SetMetroStyle(window);
            return window;
        }
        /// <summary>
        /// <see cref="MetroWindow"/>にMetroStyle(AccentとTheme)をセットする
        /// <para>MetroStyleが設定されていれば最優先で適用される</para>
        /// <para>MetroStyleが設定されておらず、AccentとThemeが設定されていればこれらを適用する</para>
        /// <para>MetroStyleが設定されておらず、AccentとThemeの両方が設定されていなければOwnerと同じものを適用する</para>
        /// </summary>
        /// <param name="window"></param>
        protected virtual void SetMetroStyle(MetroWindow window)
        {
            if (window == null) return;
            if (this.MetroStyle != null)
            {
                window.Resources.MergedDictionaries.Add(this.MetroStyle);
                return;
            }
            if (this.Accent.HasValue && this.Theme.HasValue)
            {
                MahApps.Metro.ThemeManager.ChangeAppStyle(
                    window,
                    MahApps.Metro.ThemeManager.GetAccent(AccentsExtensions.ToStringFromEnum(this.Accent.Value)),
                    MahApps.Metro.ThemeManager.GetAppTheme(ThemesExtensions.ToStringFromEnum(this.Theme.Value)));
                return;
            }
            window.Owner = Window.GetWindow(this.AssociatedObject);
            if (window.Owner is MetroWindow)
            {
                var ownerStyle = MahApps.Metro.ThemeManager.DetectAppStyle(window.Owner);
                MahApps.Metro.ThemeManager.ChangeAppStyle(window, ownerStyle.Item2, ownerStyle.Item1);
            }
        }
    }
}

