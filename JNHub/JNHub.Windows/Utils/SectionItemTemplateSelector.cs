using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Windows.UI.Xaml.Controls;

namespace JNHub.Utils
{
    class SectionItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate SectionHeadlineItemTemplate { get; set; }
        public DataTemplate SectionItemTemplate { get; set; }


        protected override DataTemplate SelectTemplateCore(object item,
                  DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is SectionItemData)
            {
                SectionItemData sectionItemData = item as SectionItemData;


                switch (sectionItemData.Size)
                {

                    case SectionItemData.HEADLINE:
                        return SectionHeadlineItemTemplate;

                    case SectionItemData.NORMAL:
                        return SectionItemTemplate;
                }

            }

            return null;
        }
    }
}
