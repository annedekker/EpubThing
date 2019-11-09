using EpubThing.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EpubThing
{
    static class XmlServices
    {
        public static string GetTitle(string contentFileName)
        {
            XDocument content = XDocument.Load(contentFileName);
            content = RemoveNamespaces(content);

            XNode titleNode = content.Descendants("title").FirstOrDefault();

            return (titleNode as XElement).Value;
        }

        public static List<EpubChapter> GetChapters(string ncxFileName)
        {
            XDocument ncxDoc = XDocument.Load(ncxFileName);
            ncxDoc = RemoveNamespaces(ncxDoc);

            XElement navMap = ncxDoc.Descendants("navMap").FirstOrDefault();
            IEnumerable<XElement> chapterNodes = navMap.Elements("navPoint");

            chapterNodes.OrderBy(ch => Int32.Parse((ch as XElement).Attribute("playOrder").Value));

            List<EpubChapter> chapters = new List<EpubChapter>();

            foreach (XElement ch in chapterNodes)
                chapters.Add(GetNavpointChapters(ch));

            return chapters;
        }

        static EpubChapter GetNavpointChapters(XElement navPoint)
        {
            XElement labelNode = navPoint.Element("navLabel");
            XElement textNode = labelNode.Element("text");

            EpubChapter chapter = new EpubChapter(textNode.Value);

            XElement contentNode = navPoint.Element("content");
            chapter.FilePath = contentNode.Attribute("src").Value;

            IEnumerable<XElement> subChapters = navPoint.Elements("navPoint");

            subChapters.OrderBy(ch => Int32.Parse((ch as XElement).Attribute("playOrder").Value));

            foreach (var x in subChapters)
                chapter.SubChapters.Add(GetNavpointChapters(x));

            return chapter;
        }

        public static string GetNcxFilePath(string contentFileName)
        {
            XDocument content = XDocument.Load(contentFileName);
            content = RemoveNamespaces(content);

            XElement ncxElement = content.Descendants("item")
                .FirstOrDefault(x => (string)x.Attribute("id") == "ncx");

            return ncxElement.Attribute("href").Value;
        }

        public static string GetContentFilePath(string containerFileName)
        {
            XDocument container = XDocument.Load(containerFileName);
            container = RemoveNamespaces(container);

            XNode rootFileNode = container.Descendants().Where(
                x => x.Name == "rootfile").FirstOrDefault();

            return (rootFileNode as XElement).Attributes().Where(
                x => x.Name == "full-path").FirstOrDefault().Value;
        }

        static XDocument RemoveNamespaces(XDocument xdoc)
        {
            var namespaces = from a in xdoc.Descendants().Attributes()
                             where a.IsNamespaceDeclaration && a.Name != "xsi"
                             select a;
            namespaces.Remove();

            RemoveNamespacePrefix(xdoc.Root);

            return xdoc;
        }

        static void RemoveNamespacePrefix(XElement element)
        {
            //Remove from element
            if (element.Name.Namespace != null)
                element.Name = element.Name.LocalName;

            //Remove from attributes
            var attributes = element.Attributes().ToArray();
            element.RemoveAttributes();
            foreach (var attr in attributes)
            {
                var newAttr = attr;

                if (attr.Name.Namespace != null)
                    newAttr = new XAttribute(attr.Name.LocalName, attr.Value);

                element.Add(newAttr);
            };

            //Remove from children
            foreach (var child in element.Descendants())
                RemoveNamespacePrefix(child);
        }
    }
}
