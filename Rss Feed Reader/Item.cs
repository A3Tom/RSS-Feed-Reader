using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rss_Feed_Reader
{
    class Item
    {
        //This contains the required fields for the item class
        public string Title { get; set; }
        public string Link { get; set; }
        public string Description { get; set; }
        public string PublishDate { get; set; }

        public Item()
        {
            Link = "";
            Title = "";
            Description = "";
            PublishDate = "";
        }
    }
}
