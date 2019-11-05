using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace EpubThing
{
    static class XmlServices
    {

        public static List<EpubChapter> GetChapters(XDocument ncxDoc)
        {
            ncxDoc = RemoveNamespaces(ncxDoc);
            XElement navMap = ncxDoc.Descendants("navMap").FirstOrDefault();
            IEnumerable<XElement> chapterNodes = navMap.Elements("navPoint");

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
            chapter.ContentPath = contentNode.Attribute("src").Value;

            IEnumerable<XElement> subChapters = navPoint.Elements("navPoint");
            foreach (var x in subChapters)
                chapter.SubChapters.Add(GetNavpointChapters(x));

            return chapter;
        }

        public static string GetNcxFilePath(XDocument content)
        {
            content = RemoveNamespaces(content);

            XElement ncxElement = content.Descendants("item")
                .FirstOrDefault(x => (string)x.Attribute("id") == "ncx");

            return ncxElement.Attribute("href").Value;
        }

        public static string GetContentFilePath(XDocument container)
        {
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
