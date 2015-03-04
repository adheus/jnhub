using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using JNHub.JN;

namespace JNHub.Utils
{
    class SectionItemData
    {
        public const int NORMAL = 1;
        public const int HEADLINE = 2;

        public JNItem JNItem { get; set; }
        
        public BitmapImage Image
        {
            get
            {
                if(JNItem.ImageURL != null)
                    return new BitmapImage(new Uri(JNItem.ImageURL, UriKind.Absolute));
                return null;
            }
        }
        public int Size { get; set; }



        public static List<SectionItemData> FromJNItemList(List<JNItem> jnItems)
        {
            List<SectionItemData> sectionItems = new List<SectionItemData>();
            if (jnItems != null)
            {
                bool first = true;
                foreach (JNItem jnItem in jnItems)
                {
                    if (jnItem != null)
                    {
                        var sectionItemData = new SectionItemData()
                        {
                            JNItem = jnItem
                        };

                        sectionItemData.Size = first ? HEADLINE : NORMAL;
                        first = false;

                        sectionItems.Add(sectionItemData);
                    }
                }
            }
            return sectionItems;
        }
    }
}
