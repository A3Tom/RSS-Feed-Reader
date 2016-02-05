using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rss_Feed_Reader
{
    class Feed
    {
        //This class collates header and body together.
        public string title, link, description;
        public IList<Item> items {get; set;}

    }
}
