using JNHub.JN;
using JNHub.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Media.Imaging;

namespace JNHub.Shared
{
    class HubBasicSectionContent
    {

        public List<SectionItemData> Items { get; set; }
        public string CategoryName { get; set; }
        public HubBasicSectionContent() { }



        public HubBasicSectionContent(String categoryName, List<JNItem> items)
        {
            CategoryName = categoryName;
            Items = SectionItemData.FromJNItemList(items);

        }
    }
}
