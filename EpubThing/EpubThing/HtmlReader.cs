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

        public static int? GetContentFilePageNumber(string filename)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(filename);

            HtmlNode pageNode = doc.DocumentNode.SelectSingleNode("//a[@id]");
            if (pageNode == null) return null;

            string pageNum = pageNode.GetAttributeValue("id", "null");
            pageNum = pageNum.Replace("page_", "");
            return Int32.Parse(pageNum);
        }

        public static int GetLastPageNumber(string filename)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(filename);

            //HtmlNode pageNode = doc.DocumentNode.SelectSingleNode("//a[@id | Last()]");

            HtmlNodeCollection nodes = doc.DocumentNode.SelectNodes("//a[@id]");
            HtmlNode pageNode = nodes.Last();

            while (true)
            {
                pageNode = nodes.Last();
                if (pageNode.GetAttributeValue("id", "null").Contains("page")) break;
                else nodes.Remove(pageNode);
            }

            string pageNum = pageNode.GetAttributeValue("id", "null");
            pageNum = pageNum.Replace("page_", "");
            return Int32.Parse(pageNum);
        }
    }
}
