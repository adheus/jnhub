using JNHub.JN;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Web.Http;
using Windows.Web.Syndication;

namespace JNHub.Shared.RSSReader
{
    class JNRSSReader
    {

        private const string MAIN_FEED_URL = @"http://jovemnerd.com.br/feed/";
        private const string NERDCAST_FEED_URL = @"http://jovemnerd.com.br/categoria/nerdcast/feed/";
        private const string NERDOFFICE_FEED_URL = @"http://jovemnerd.com.br/categoria/nerdoffice/feed/";
        private const string NERDPLAYER_FEED_URL = @"http://jovemnerd.com.br/categoria/nerdplayer/feed/";
        private const string NERDOLOGIA_FEED_URL = @"http://gdata.youtube.com/feeds/api/users/nerdologia/uploads";
        private const string MRG_FEED_URL = @"http://jovemnerd.com.br/categoria/matando-robos-gigantes/feed/";
        private const string MRG_SHOW_FEED_URL = @"http://gdata.youtube.com/feeds/api/users/matandorobosgigantes/uploads";

        private static JNRSSReader instance;

        private HttpClient httpClient;

        private const string NERDCAST_FEED_PREFIX = "http://jovemnerd.com.br/nerdcast";
        private const string NERDOFFICE_FEED_PREFIX = "http://jovemnerd.com.br/nerdoffice";
        private const string MRG_FEED_PREFIX = "http://jovemnerd.com.br/matando-robos-gigantes";
        private const string MRG_SHOW_TITLE_STRING = @"MRG Show";

        private List<JNItem> lastFetchedMainJNItems = new List<JNItem>();

        private List<JNItem> lastFetchedNerdcastJNItems = new List<JNItem>();
        private List<JNItem> lastFetchedNerdOfficeJNItems = new List<JNItem>();
        private List<JNItem> lastFetchedNerdPlayerJNItems = new List<JNItem>();
        private List<JNItem> lastFetchedNerdologiaJNItems = new List<JNItem>();
        private List<JNItem> lastFetchedMRGJNItems = new List<JNItem>();
        private List<JNItem> lastFetchedMRGShowJNItems = new List<JNItem>();


        private JNRSSReader()
        {
            httpClient = new HttpClient();
        }

        private static JNRSSReader getInstance()
        {
            if (instance == null)
                instance = new JNRSSReader();

            return instance;
        }

        public static async Task<List<JNItem>> GetMainNerdcasts()
        {
            return (await GetMainFeed()).Where(i => i.Categories.Contains("Nerdcast")).ToList();
        }

        public static async Task<List<JNItem>> GetMainNerdOffices()
        {
            return (await GetMainFeed()).Where(i => i.Categories.Contains("NerdOffice")).ToList();
        }

        public static async Task<List<JNItem>> GetMainNerdPlayers()
        {
            return (await GetMainFeed()).Where(i => i.Categories.Contains("NerdPlayer")).ToList();
        }

        public static async Task<List<JNItem>> GetMainMRGs()
        {
            return (await GetMainFeed()).Where(i => i.Categories.Contains("Matando Robôs Gigantes")).ToList();
        }

        public static async Task<List<JNItem>> GetMainMRGShows()
        {
            return (await GetMainFeed()).Where(i => i.Categories.Contains("Matando Robôs Gigantes") && i.Categories.Contains("Show")).ToList();
        }

        public async static Task<List<JNItem>> GetMainFeed()
        {
            if (getInstance().lastFetchedMainJNItems.Count == 0)
                 getInstance().lastFetchedMainJNItems = await getInstance().update(MAIN_FEED_URL, getInstance().lastFetchedMainJNItems);
            
            return getInstance().lastFetchedMainJNItems;
        }

        public async static Task<List<JNItem>> GetNerdcastFeed()
        {
            if (getInstance().lastFetchedNerdcastJNItems.Count == 0)
                getInstance().lastFetchedNerdcastJNItems = await getInstance().update(NERDCAST_FEED_URL, getInstance().lastFetchedNerdcastJNItems);
            
            return getInstance().lastFetchedNerdcastJNItems;
        }

        public async static Task<List<JNItem>> GetNerdOfficeFeed()
        {
            if (getInstance().lastFetchedNerdOfficeJNItems.Count == 0)
                getInstance().lastFetchedNerdOfficeJNItems = await getInstance().update(NERDOFFICE_FEED_URL, getInstance().lastFetchedNerdOfficeJNItems);
            
            return getInstance().lastFetchedNerdOfficeJNItems;
        }

        public async static Task<List<JNItem>> GetNerdPlayerFeed()
        {
            if (getInstance().lastFetchedNerdPlayerJNItems.Count == 0)
                getInstance().lastFetchedNerdPlayerJNItems = await getInstance().update(NERDPLAYER_FEED_URL, getInstance().lastFetchedNerdPlayerJNItems);
            
            return getInstance().lastFetchedNerdPlayerJNItems;
        }

        public async static Task<List<JNItem>> GetNerdologiaFeed()
        {
            if (getInstance().lastFetchedNerdologiaJNItems.Count == 0)
                getInstance().lastFetchedNerdologiaJNItems = (await getInstance().update(NERDOLOGIA_FEED_URL, getInstance().lastFetchedNerdologiaJNItems));

            return getInstance().lastFetchedNerdologiaJNItems;
        }
        
        public async static Task<List<JNItem>> GetMRGsFeed()
        {
            if (getInstance().lastFetchedMRGJNItems.Count == 0)
                getInstance().lastFetchedMRGJNItems = (await getInstance().update(MRG_FEED_URL, getInstance().lastFetchedMRGJNItems));

            return getInstance().lastFetchedMRGJNItems;
        }

        public async static Task<List<JNItem>> GetMRGShowsFeed()
        {
            if (getInstance().lastFetchedMRGShowJNItems.Count == 0)
                getInstance().lastFetchedMRGShowJNItems = (await getInstance().update(MRG_SHOW_FEED_URL, getInstance().lastFetchedMRGShowJNItems));

            return getInstance().lastFetchedMRGShowJNItems.ToList(); ;
        }


        private async Task<List<JNItem>> update(String url, List<JNItem> jnItems)
        {
            Uri feedUri = new Uri(url);

            try
            {
                var httpResponse = await httpClient.GetAsync(feedUri);

                var buffer = await httpResponse.Content.ReadAsBufferAsync();

                var xmlString = Encoding.UTF8.GetString(buffer.ToArray(), 0, (int)(buffer.Length - 1));
                xmlString = xmlString.Replace((char)0x1B, ' ');
                SyndicationFeed feed = new SyndicationFeed();
                feed.Load(xmlString);
                
                XElement rss = XElement.Parse(xmlString);
                var channel = rss.Element("channel");
                IEnumerable<XElement> xmlItems = null;
                if(channel != null)
                     xmlItems = channel.Elements("item");

                jnItems.Clear();

                int index = 0;
                foreach (var item in feed.Items)
                {
                    JNItem jnItem = new JNItem();
                    jnItem.Title = item.Title.Text;
                    if (item.Summary != null)
                        jnItem.HTMLDescription = item.Summary.Text;
                    else if (item.Content != null)
                    {
                        jnItem.HTMLDescription = item.Content.Text;
                        jnItem.setCustomDescription(item.Content.Text);
                    }
                    

                    if (xmlItems != null)
                    {
                        var xmlItem = xmlItems.ElementAt(index);
                        index++;
                        XNamespace content = "http://purl.org/rss/1.0/modules/content/";
                        var contentEncoded = xmlItem.Descendants(content + "encoded");
                        if (contentEncoded.FirstOrDefault() != null)
                            jnItem.ContentEncoded = contentEncoded.FirstOrDefault().Value;
                    }

                    var enclosure = item.Links.Where(l => l.Relationship.Equals("enclosure", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    var alternate = item.Links.Where(l => l.Relationship.Equals("alternate", StringComparison.OrdinalIgnoreCase)).FirstOrDefault();

                    if(alternate != null)
                    {
                        var videoURL = alternate.Uri.AbsoluteUri;
                        jnItem.setCustomVideoURL(videoURL);
                    }

                    if (enclosure != null)
                    {
                        var podcastItem = new PodcastItem();
                        podcastItem.FileURL = enclosure.Uri.AbsoluteUri;
                        podcastItem.FileLength = enclosure.Length;
                        podcastItem.MediaType = enclosure.MediaType;

                        jnItem.Podcast = podcastItem;
                    }

                    jnItem.PubDate = item.PublishedDate;
                    jnItem.Link = item.Id;
                    foreach (var category in item.Categories)
                        jnItem.Categories.Add(category.NodeValue);

                    jnItems.Add(jnItem);
                }
            }
            catch(Exception e) 
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }
            return jnItems;
        }
    }
}
