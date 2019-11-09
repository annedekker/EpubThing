using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubThing.Model
{
    class EpubBook
    {
        public string Title { get; set; }

        public List<EpubChapter> Chapters { get; set; }

        public int LastPageNumber { get; set; }

        public EpubBook()
        {
            this.Chapters = new List<EpubChapter>();
        }

        public EpubBook(string title)
        {
            this.Title = title;
            this.Chapters = new List<EpubChapter>();
        }
    }
}
