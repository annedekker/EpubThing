using System.Collections.Generic;

namespace EpubThing.Model
{
    class EpubChapter
    {
        public string Title { get; set; }

        public List<EpubChapter> SubChapters { get; set; }

        public string FilePath { get; set; }

        public bool HasPageNumber { get; private set; }
        public int PageNumber { get; private set; }

        public EpubChapter()
        {
            this.SubChapters = new List<EpubChapter>();
        }

        public EpubChapter(string title)
        {
            this.Title = title;
            this.SubChapters = new List<EpubChapter>();
        }

        public void SetPageNumber(int number)
        {
            this.HasPageNumber = true;
            this.PageNumber = number;
        }
    }
}
