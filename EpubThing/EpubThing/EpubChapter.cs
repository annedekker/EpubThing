using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubThing
{
    class EpubChapter
    {
        public Guid Id { get; private set; }
        public string Title { get; set; }
        public string ContentPath { get; set; }
        public List<EpubChapter> SubChapters { get; set; }

        public EpubChapter(string title)
        {
            this.Id = Guid.NewGuid();
            this.Title = title;
            this.SubChapters = new List<EpubChapter>();
        }
    }
}
