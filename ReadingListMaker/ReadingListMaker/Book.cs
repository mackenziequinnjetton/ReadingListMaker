using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ReadingListMaker
{
    internal class Book
    {
        public JToken Title { get; set; }
        public JToken Authors { get; set; }
        public JToken Publisher { get; set; }

        public Book(JToken title, JToken author, JToken publisher)
        {
            Title = title;
            Authors = author;
            Publisher = publisher;
        }
    }
}
