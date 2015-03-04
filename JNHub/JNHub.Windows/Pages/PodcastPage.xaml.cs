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
using Windows.Media;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
    public sealed partial class PodcastPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

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
        private WaitViewProvider waitView;

        public PodcastPage()
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
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            waitView = new WaitViewProvider(this.rootGrid, "");

            musicPlayer.MediaOpened += musicPlayer_MediaOpened;
            jnItem = (JNItem)e.NavigationParameter;

            this.pageTitle.Text = jnItem.Title;
            this.description.Text = jnItem.Description;

            headerImage.ImageSource = new BitmapImage(new Uri(jnItem.ImageURL, UriKind.Absolute));

            Uri uri = new Uri(jnItem.Podcast.FileURL, UriKind.Absolute);
            musicPlayer.Source = uri;

            waitView.Show();

            InitializeTransportControls();
        }

        void musicPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            double absvalue = Math.Round(
                 musicPlayer.NaturalDuration.TimeSpan.TotalSeconds,
                MidpointRounding.AwayFromZero);
         

            waitView.Remove();

        }

        SystemMediaTransportControls systemControls;

        void InitializeTransportControls()
        {
            // Hook up app to system transport controls.
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += SystemControls_ButtonPressed;

            // Register to handle the following system transpot control buttons.
            systemControls.IsPlayEnabled = true;
            systemControls.IsPauseEnabled = true;
        }


        void SystemControls_ButtonPressed(SystemMediaTransportControls sender,
    SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayMedia();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PauseMedia();
                    break;
                default:
                    break;
            }
        }

        async void PlayMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                musicPlayer.Play();
            });
        }

        async void PauseMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                musicPlayer.Pause();
            });
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



        void musicPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
           

           switch (musicPlayer.CurrentState)
            {
                case MediaElementState.Playing:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Paused:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Stopped:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaElementState.Closed:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                default:
                    break;
            }
        }

        private async void musicPlayer_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            waitView.Remove();
            var dialog = new MessageDialog("Não foi possível carregar este vídeo. Verifique sua conexão.");
            await dialog.ShowAsync();

            this.Frame.GoBack();
        }
    }
}
