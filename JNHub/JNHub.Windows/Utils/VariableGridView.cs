using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace JNHub.Utils
{
    class VariableSizeGridView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            var sectionItemData = item as Utils.SectionItemData;

            switch(sectionItemData.Size)
            {
                case Utils.SectionItemData.HEADLINE:
                    element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 2);
                    element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 2);
                    break;
                case Utils.SectionItemData.NORMAL:
                    element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 1);
                    element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 1);
                    break;
            }
           

            base.PrepareContainerForItemOverride(element, item);
        } 
    }
}
