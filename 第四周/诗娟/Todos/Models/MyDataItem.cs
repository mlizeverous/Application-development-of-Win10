using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;

namespace Todos.Models
{
    class MyDataItem
    {
        public string title { get; set; }
        public string details { get; set; }
        public DateTimeOffset date { get; set; }
        public ImageSource image { get; set; }
        public bool? item1 { get; set; }
        public bool? item2 { get; set; }
    }
}
