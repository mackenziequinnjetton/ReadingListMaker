using Newtonsoft.Json.Linq;

namespace ReadingListMaker
{
    // Holds book information
    public class Book
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