using EpubThing.Model;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpubThing
{
    class HtmlReader
    {
        public static void AddChapterPageNumber(EpubChapter chapter, EpubManager manager)
        {
            if (chapter.FilePath.Contains("#")) return;

            HtmlDocument doc = new HtmlDocument();
            doc.Load(manager.GetChapterFilePath(chapter));

            HtmlNode pageNode = doc.DocumentNode.SelectSingleNode("//a[@id]");
            if (pageNode == null) return;

            string pageNum = pageNode.GetAttributeValue("id", "null");
            pageNum = pageNum.Replace("page_", "");

            chapter.SetPageNumber(Int32.Parse(pageNum));

            foreach (var ch in chapter.SubChapters)
                AddChapterPageNumber(ch, manager);
        }
    }
}
