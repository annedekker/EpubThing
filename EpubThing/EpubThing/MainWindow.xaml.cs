using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Windows;
using System.Xml.Linq;

namespace EpubThing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string openFilePath;
        string tempFolderPath;
        string contentFolderPath;

        List<string> chapterNames;
        Dictionary<string, string> chapterContent;

        public MainWindow()
        {
            InitializeComponent();

            tempFolderPath = Path.Combine(Path.GetTempPath(), "EpubThing");
            openFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = openFilePath;
            ofd.Filter = "Epub files (*.epub)|*.epub";
            if (ofd.ShowDialog() == true)
            {
                openFilePath = ofd.FileName;
                LoadEpubArchive();
            }
        }

        private void LoadEpubArchive()
        {
            if (Directory.Exists(tempFolderPath)) Directory.Delete(tempFolderPath, true);

            ZipFile.ExtractToDirectory(openFilePath, tempFolderPath);

            string contentFilePath = XmlServices.GetContentFilePath(
                XDocument.Load(Path.Combine(tempFolderPath, "META-INF", "container.xml")));

            contentFolderPath = Path.GetDirectoryName(
                Path.Combine(tempFolderPath, contentFilePath));

            string ncxFilePath = XmlServices.GetNcxFilePath(
                XDocument.Load(Path.Combine(tempFolderPath, contentFilePath)));

            ncxFilePath = Path.Combine(contentFolderPath, ncxFilePath);

            XDocument ncxDoc = XDocument.Load(ncxFilePath);
            ncxDoc = XmlServices.RemoveNamespaces(ncxDoc);
            IEnumerable<XElement> chapterNodes = ncxDoc.Descendants()
                .Where(x => x.Name == "navPoint");
            chapterNodes.OrderBy(x => Int32.Parse(x.Attribute("playOrder").Value));

            chapterNames = new List<string>();
            chapterContent = new Dictionary<string, string>();
            foreach (var e in chapterNodes)
            {
                string title = e.Descendants().Where(x => x.Name == "text").FirstOrDefault().Value;
                string contentPath = e.Descendants().Where(x => x.Name == "content").FirstOrDefault().Attribute("src").Value;

                chapterNames.Add(title);
                chapterContent[title] = contentPath;
            }

            ChapterView.ItemsSource = chapterNames;
        }

        private void ChapterView_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string filePath = Path.Combine(
                contentFolderPath,
                chapterContent[(string)ChapterView.SelectedItem]);

            BookView.Source = new Uri(filePath);
        }
    }
}
