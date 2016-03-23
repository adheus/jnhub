using JNHub.Shared.RSSReader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace JNHub
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class JNMainPage : Page
    {
        public JNMainPage()
        {
            this.InitializeComponent();
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = Windows.UI.Colors.DarkCyan;
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = Windows.UI.Colors.DarkCyan;
            
           
            Window.Current.SizeChanged += Current_SizeChanged;
            Current_SizeChanged(null, null);
            setFeed(JNRSSReader.GetCurrentMainFeed());
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().VisibleBounds.Width > 400)
            {
                MySplitView.CompactPaneLength = 48;
            } else
            {
                MySplitView.CompactPaneLength = 0;
            }
        }


        private async void setFeed(System.Threading.Tasks.Task<List<JN.JNItem>> getFeedTask)
        {
            var waitView = new Utils.WaitViewProvider(this.ContentGrid, "");
            waitView.Show();
            this.ContentListView.ItemsSource = null;
            var feed = await getFeedTask;
            this.ContentListView.ItemsSource = feed;
            waitView.Remove();
        }
       
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
        }

        private void ContentListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var jNItem = (JN.JNItem) e.ClickedItem;
            if (jNItem.isPodcast)
            {
                this.Frame.Navigate(typeof(Pages.PodcastPage), jNItem);
            }
            else if (jNItem.isVideo)
            {
                this.Frame.Navigate(typeof(Pages.VideoPage), jNItem);
            } else
            {
                this.Frame.Navigate(typeof(Pages.WebViewPage), jNItem);
            }
        }

        private void NerdcastButton_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryName.Text = "NERDCAST";
            this.MySplitView.IsPaneOpen = false;
            setFeed(JNRSSReader.GetNerdcastFeed());
        }

        private void NerdofficeButton_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryName.Text = "NERDOFFICE";
            this.MySplitView.IsPaneOpen = false;
            setFeed(JNRSSReader.GetNerdOfficeFeed());
        }

        private void NerdplayerButton_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryName.Text = "NERDPLAYER";
            this.MySplitView.IsPaneOpen = false;
            setFeed(JNRSSReader.GetNerdPlayerFeed());
        }

        private void MRGButton_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryName.Text = "MATANDO ROBÔS GIGANTES";
            this.MySplitView.IsPaneOpen = false;
            setFeed(JNRSSReader.GetMRGsFeed());
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            this.CategoryName.Text = "INÍCIO";
            this.MySplitView.IsPaneOpen = false;
            setFeed(JNRSSReader.GetMainFeed());
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            this.MySplitView.IsPaneOpen = false;
            this.Frame.Navigate(typeof(Pages.AboutPage));
        }
    }
}
