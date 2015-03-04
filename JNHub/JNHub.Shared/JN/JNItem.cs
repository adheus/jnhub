using System;
using System.Collections.Generic;
using System.Text;
using Windows.Data.Html;

namespace JNHub.JN
{
    class JNItem
    {
        private const string YOUTUBE_DEFAULT_THUMBNAIL_URL = @"http://img.youtube.com/vi/{YOUTUBE_VIDEO_ID}/hqdefault.jpg";

        public string Title { get; set; }
        public string HTMLDescription { get; set; }
        public string ContentEncoded { get; set; }
        public string Link { get; set; }
        public DateTimeOffset PubDate { get; set; }
        public List<string> Categories = new List<string>();

        public PodcastItem Podcast { get; set; }

        private string customDescription;
        private string customVideoURL;

        public bool isPodcast
        {
            get
            {
                return Podcast != null;
            }
        }

        public bool hasImage
        {
            get
            {
                return HTMLDescription.Contains("<img") && HTMLDescription.Contains("src=");
            }
        }

        public bool isVideo
        {
            get
            {
                if (customVideoURL != null)
                    return true;

                if(ContentEncoded != null)
                    return ContentEncoded.Contains("<iframe");

                return false;
            }
        }

        public void setCustomDescription(String customDescription)
        {
            this.customDescription = customDescription;
        }

        public void setCustomVideoURL(String customVideoURL)
        {
            this.customVideoURL = customVideoURL;
        }

        public string VideoURL
        {
            get
            {
                if (customVideoURL == null)
                {
                    if (isVideo)
                    {
                        string iframePrefix = "<iframe";
                        int iframeIndex = ContentEncoded.IndexOf(iframePrefix);
                        string imageSourcePrefix = "src=\"";
                        int index = ContentEncoded.Substring(iframeIndex).IndexOf(imageSourcePrefix) + iframeIndex;
                        int lastIndex = ContentEncoded.Substring(index + imageSourcePrefix.Length).IndexOf("\"");
                        customVideoURL = ContentEncoded.Substring(index + imageSourcePrefix.Length, lastIndex);
                    }
                }
                return customVideoURL;
            }
        }

        public string YoutubeID
        {
            get
            {
                
                if(VideoURL != null)
                {

                    string youTubeEmbedPrefix = "www.youtube.com/embed/";
                    if (!VideoURL.Contains(youTubeEmbedPrefix))
                        youTubeEmbedPrefix = "www.youtube.com/watch?v=";
                        
                    int initIndex = VideoURL.IndexOf(youTubeEmbedPrefix) + youTubeEmbedPrefix.Length;
                    int finishedIndex = VideoURL.Substring(initIndex).IndexOf("?");
                    if (finishedIndex > 0)
                        return VideoURL.Substring(initIndex, finishedIndex);
                    else
                    {
                        finishedIndex = VideoURL.Substring(initIndex).IndexOf("&");
                        if (finishedIndex > 0)
                            return VideoURL.Substring(initIndex, finishedIndex);
                        
                        return VideoURL.Substring(initIndex);
                    }
                }
                return null;
            }
        }

        public string Description
        {
            get
            {
                if(customDescription == null)
                    return HtmlUtilities.ConvertToText(HTMLDescription);
                return customDescription;
            }
        }

        public string FormattedSmallDescription
        {
            get
            {
               return Description.Trim().Substring(0, 200)+"...";
            }
        }

        public string FormattedPubDate
        {
            get
            {
                return PubDate.ToString("dd/MM/yyyy 'ás' HH:mm");
            }
        }

        public string ImageURL
        {
            get
            {
              if (hasImage)
              {
                  string imageSourcePrefix = "src=\""; 
                  int index = HTMLDescription.IndexOf(imageSourcePrefix);
                  int lastIndex = HTMLDescription.Substring(index+imageSourcePrefix.Length).IndexOf("\"");
                  return HTMLDescription.Substring(index + imageSourcePrefix.Length, lastIndex);
              } else if(isVideo)
              {
                  return YOUTUBE_DEFAULT_THUMBNAIL_URL.Replace("{YOUTUBE_VIDEO_ID}", YoutubeID);
              }

              return null;
            }
        }
    }
}
