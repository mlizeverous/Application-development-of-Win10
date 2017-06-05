using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Todos.Models;


namespace Todos.ViewModels
{
    class EditViewModels
    {
        private string title;
        public string Title { get { return title;  } set { title = value; } }

        private string details;
        public string Details { get { return details;  } set { details = value;  } }

        private DateTimeOffset date;
        public DateTimeOffset Date { get { return date;  } set { date = value;  } }

        private ImageSource image;
        public ImageSource Image { get { return image;  } set { image = value;  } }

        private bool? item1;
        public bool? Item1 { get { return item1; } set { item1 = value; } }

        private bool? item2;
        public bool? Item2 { get { return item2; } set { item2 = value; } }

        public void LoadData()
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey("TheData"))
            {
                MyDataItem data = JsonConvert.DeserializeObject<MyDataItem>(
                    (string)ApplicationData.Current.RoamingSettings.Values["TheData"]);
                
            }
            else
            {
                // New start, initialize the data
                
            }
        }

        public void SaveData()
        {
            MyDataItem data = new MyDataItem {
            date = this.Date,
            details = this.Details,
            title = this.Title,
            image = this.Image,
            item1 = this.Item1,
            item2 = this.Item2

        };
            ApplicationData.Current.RoamingSettings.Values["TheData"] =
                JsonConvert.SerializeObject(data);
        }
    }
}
