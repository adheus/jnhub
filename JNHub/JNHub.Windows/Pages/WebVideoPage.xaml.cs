using JNHub.Common;
using JNHub.JN;
using JNHub.Utils;
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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace JNHub.Pages
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class WebVideoPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private WaitViewProvider waitViewProvider;

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

        private JNItem jnItem;
        private DispatcherTimer timer;

        public WebVideoPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;


            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);

            Window.Current.SizeChanged += Current_SizeChanged;
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            loadHTML();
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {

            jnItem = (JNItem)e.NavigationParameter;

            this.pageTitle.Text = jnItem.Title;
            //headerImage.ImageSource =  new BitmapImage(new Uri(jnItem.ImageURL, UriKind.Absolute));
            webView.NavigationCompleted += webView_NavigationCompleted;

            waitViewProvider = new WaitViewProvider(this.rootGrid, "");
            waitViewProvider.Show();
            webView.Visibility = Visibility.Collapsed;
            loadHTML();
        }

        void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            waitViewProvider.Remove();
            webView.Visibility = Visibility.Visible;

            this.pageRoot.PointerMoved += page_PointerMoved;
            startTimer();
        }

        void page_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            titleGrid.Visibility = Visibility.Visible;
            stopTimer();
            startTimer();
        }

        void startTimer()
        {
            timer.Tick += timer_Tick;
            timer.Start();
        }

        void stopTimer()
        {
            timer.Tick -= timer_Tick;
            timer.Stop();
        }

        void timer_Tick(object sender, object e)
        {
            HideTitle();
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
            webView.NavigateToString("");
        }

        #endregion


        private void loadHTML()
        {
            var bounds = Window.Current.Bounds;
            string width = (((int)bounds.Width - 8) + "");
            string height = (((int)bounds.Height - 18) + "");
            string videoURL = jnItem.VideoURL;
            if (!videoURL.Contains("http"))
                videoURL = "http:" + videoURL;
            string iframe = @"<body bgcolor=""#000000""><iframe width=""{{WIDTH}}"" height=""{{HEIGHT}}"" src=""{{VIDEO_URI}}"" frameborder=""0""></iframe></body>";
            var videoHtml = iframe.Replace("{{VIDEO_URI}}", videoURL).Replace("{{WIDTH}}", width).Replace("{{HEIGHT}}", height);

            webView.NavigateToString(videoHtml);
        }

        private void HideTitle()
        {
            titleGrid.Visibility = Visibility.Collapsed;
            stopTimer();
        }
    }
}
