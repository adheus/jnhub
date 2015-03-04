using JNHub.Common;
using JNHub.JN;
using JNHub.Shared;
using JNHub.Shared.RSSReader;
using JNHub.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace JNHub.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class CategoryPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private List<JNItem> categoryItems;
        private List<SectionItemData> currentList;
        private WaitViewProvider waitView;
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public CategoryPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.pageTitle.Text = (string) e.NavigationParameter;

            waitView = new WaitViewProvider(this.rootGrid, "");
            
            waitView.Show();

            switch(this.pageTitle.Text)
            {
                case "NERDCAST":
                    categoryItems =  await JNRSSReader.GetNerdcastFeed();
                    break;
                case "NERDOFFICE":
                    categoryItems =  await JNRSSReader.GetNerdOfficeFeed();
                    break;
                case "NERDPLAYER":
                    categoryItems =  await JNRSSReader.GetNerdPlayerFeed();
                    break;
                case "NERDOLOGIA":
                    categoryItems =  await JNRSSReader.GetNerdologiaFeed();
                    break;
                case "MATANDO ROBÔS GIGANTES":
                    categoryItems =  await JNRSSReader.GetMRGsFeed();
                    break;
                case "MRG SHOW":
                    categoryItems =  await JNRSSReader.GetMRGShowsFeed();
                    break;



            }
            currentList = SectionItemData.FromJNItemList(categoryItems);
            this.itemsGridView.ItemsSource = currentList;
            waitView.Remove();
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void itemsGridView_ItemClick(object sender, ItemClickEventArgs e)
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

        private void searchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var culture = CultureInfo.CurrentCulture;
            string searchTerm = searchBox.Text;
            if (searchTerm.Length >= 3)
            {
                currentList = SectionItemData.FromJNItemList(categoryItems.Where(i => culture.CompareInfo.IndexOf(i.Title, searchTerm, CompareOptions.IgnoreCase) >= 0 || culture.CompareInfo.IndexOf(i.Description, searchTerm, CompareOptions.IgnoreCase) >= 0).ToList());
                if (currentList.Count == 0)
                {
                    this.noItemsMessage.Visibility = Visibility.Visible;
                    this.itemsGridView.Visibility = Visibility.Collapsed;
                }
                else
                {
                    
                    this.itemsGridView.ItemsSource = currentList;
                    this.noItemsMessage.Visibility = Visibility.Collapsed;
                    this.itemsGridView.Visibility = Visibility.Visible;
                }
            }
            else if (currentList.Count < categoryItems.Count)
            {
                currentList = SectionItemData.FromJNItemList(categoryItems);
                this.itemsGridView.ItemsSource = currentList;
                this.noItemsMessage.Visibility = Visibility.Collapsed;
                this.itemsGridView.Visibility = Visibility.Visible;
            }
        }
    }
}
