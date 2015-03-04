using JNHub.JN;
using JNHub.Shared;
using JNHub.Shared.RSSReader;
using JNHub.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
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
    public sealed partial class MainPage : Page
    {

        private WaitViewProvider waitView;

        public MainPage()
        {
            this.InitializeComponent();

            Window.Current.SizeChanged += Current_SizeChanged;
            load();

        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            double width = e.Size.Width;
            double height = e.Size.Height;

            // Obtain the orientation - Landscape or Portait
            string CurrentViewState = ApplicationView.GetForCurrentView().Orientation.ToString();

            /*
            // Test to see if the app is fullscreen or not
            if (ApplicationView.GetForCurrentView().IsFullScreen)
            {
                // It is, so handle Landscape or Portrait visual states as before
                VisualStateManager.GoToState(this, CurrentViewState, false);
                return;
            }*/

            // At this point, the screen is split, so decide if it's narrow enough
            // to require a new visual state

            load();
        }

        private async void load()
        {
            waitView = new WaitViewProvider(this.rootGrid, "");

            waitView.Show();
            try
            {

                var bounds = Window.Current.Bounds;
                int height = (int)bounds.Height;

                int maxItemsWidth = 3;
                int limit = (((int)Math.Floor((double)height / 156) - 1) * maxItemsWidth) - maxItemsWidth;


                var nerdcasts = (await JNRSSReader.GetMainNerdcasts());
                int shouldGetLength = limit >= nerdcasts.Count ? nerdcasts.Count : limit;
                NerdcastSection.DataContext = new HubBasicSectionContent("Nerdcast", nerdcasts.GetRange(0, shouldGetLength));

                var nerdoffices = (await JNRSSReader.GetMainNerdOffices());
                shouldGetLength = limit >= nerdoffices.Count ? nerdoffices.Count : limit;
                NerdOfficeSection.DataContext = new HubBasicSectionContent("NerdOffice", nerdoffices.GetRange(0, shouldGetLength));

                var nerdplayers = (await JNRSSReader.GetMainNerdPlayers());
                shouldGetLength = limit >= nerdplayers.Count ? nerdplayers.Count : limit;
                NerdPlayerSection.DataContext = new HubBasicSectionContent("NerdPlayer", nerdplayers.GetRange(0, shouldGetLength));

                var mrgs = (await JNRSSReader.GetMainMRGs());
                shouldGetLength = limit >= mrgs.Count ? mrgs.Count : limit;
                MRGSection.DataContext = new HubBasicSectionContent("Matando Robôs Gigantes", mrgs.GetRange(0, shouldGetLength));

                var mrgsShow = (await JNRSSReader.GetMainMRGShows());
                shouldGetLength = limit >= mrgsShow.Count ? mrgsShow.Count : limit;
                MRGShowSection.DataContext = new HubBasicSectionContent("MRG Show", mrgsShow.GetRange(0, shouldGetLength));

            }
            catch
            {
                var messageDialog = new MessageDialog("Não foi possível atualizar seu feed. Clique em \"OK\" para tentar novamente.");
                messageDialog.ShowAsync();
            }
            waitView.Remove();


        }

        private void VariableSizeGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            SectionItemData sectionItemData = (SectionItemData)e.ClickedItem;
            if (sectionItemData.JNItem.isPodcast)
            {
                this.Frame.Navigate(typeof(Pages.PodcastPage), sectionItemData.JNItem);
            }
            else if (sectionItemData.JNItem.isVideo)
            {
                this.Frame.Navigate(typeof(Pages.VideoPage), sectionItemData.JNItem);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HubBasicSectionContent clickedHub = (HubBasicSectionContent)((Button)sender).Tag;
            this.Frame.Navigate(typeof(Pages.CategoryPage), clickedHub.CategoryName);
        }

        private void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubBasicSectionContent clickedHub = (HubBasicSectionContent)e.Section.DataContext;
            this.Frame.Navigate(typeof(Pages.CategoryPage), clickedHub.CategoryName);
        }

        private void GoToTermsOfUse(object sender, RoutedEventArgs e)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.adheus.com/jnhub/terms.html", UriKind.Absolute));
        }

        private void GoToSite(object sender, RoutedEventArgs e)
        {

            Windows.System.Launcher.LaunchUriAsync(new Uri("http://www.adheus.com", UriKind.Absolute));
        }

    }
}
