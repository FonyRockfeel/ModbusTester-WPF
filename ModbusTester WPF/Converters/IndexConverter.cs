using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Controls;

namespace ModbusTester_WPF.Converters
{
    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, CultureInfo culture)
        {
            ListBoxItem item = (ListBoxItem)value;
            ListBox listView = ItemsControl.ItemsControlFromItemContainer(item) as ListBox;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return index.ToString();
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }        
    }
}
