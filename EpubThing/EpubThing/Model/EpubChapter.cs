using System.Collections.Generic;

namespace EpubThing.Model
{
    class EpubChapter
    {
        public string Title { get; set; }

        public List<EpubChapter> SubChapters { get; set; }

        public string FilePath { get; set; }

        public int PageNumber { get; set; }

        public EpubChapter()
        {
            this.SubChapters = new List<EpubChapter>();
        }

        public EpubChapter(string title)
        {
            this.Title = title;
            this.SubChapters = new List<EpubChapter>();
        }
    }
}
