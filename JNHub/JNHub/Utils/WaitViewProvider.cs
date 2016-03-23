using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace JNHub.Utils
{
    class WaitViewProvider
    {
        private Grid rootGrid;
        private ProgressRing wait;
        private TextBlock label;
        private StackPanel fadedBackground;

        public WaitViewProvider(Grid rootGrid, String text)
        {
            this.rootGrid = rootGrid;

            wait = new ProgressRing();
            wait.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 0, 161, 169));
            wait.MinWidth = 80;
            wait.MinHeight = 80;
            label = new TextBlock();
            label.Margin = new Thickness(0, 50, 0, 0);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 50, 50));
            fadedBackground = new StackPanel();
            fadedBackground.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(100, 255, 255, 255));
            fadedBackground.Width = this.rootGrid.Width;
            fadedBackground.Height = this.rootGrid.Height;

            if (text.Trim().Length == 0)
                label.Visibility = Visibility.Collapsed;
            label.Text = text;
        }

        public WaitViewProvider(Grid rootGrid, String text, Windows.UI.Color backgroundColor)
        {
            this.rootGrid = rootGrid;

            wait = new ProgressRing();
            wait.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 50, 50));
            wait.Height = 150;
            label = new TextBlock();
            label.Margin = new Thickness(0, 50, 0, 0);
            label.HorizontalAlignment = HorizontalAlignment.Center;
            label.VerticalAlignment = VerticalAlignment.Center;
            label.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 50, 50, 50));
            fadedBackground = new StackPanel();
            fadedBackground.Background = new SolidColorBrush(backgroundColor);
            fadedBackground.Width = this.rootGrid.Width;
            fadedBackground.Height = this.rootGrid.Height;

            if (text.Trim().Length == 0)
                label.Visibility = Visibility.Collapsed;
            label.Text = text;
        }

        public void Show()
        {
            this.rootGrid.Children.Add(fadedBackground);
            this.rootGrid.Children.Add(wait);
            this.rootGrid.Children.Add(label);

            wait.IsActive = true;
        }
        public void Remove()
        {
            wait.IsActive = false;

            this.rootGrid.Children.Remove(fadedBackground);
            this.rootGrid.Children.Remove(label);
            this.rootGrid.Children.Remove(wait);
        }
    }
}
