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

        List<EpubChapter> chapters;

        public MainWindow()
        {
            InitializeComponent();

            tempFolderPath = Path.Combine(Path.GetTempPath(), "EpubThing");
            openFilePath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Directory.Delete(tempFolderPath, true);
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

            chapters = XmlServices.GetChapters(ncxDoc);

            ChapterView.ItemsSource = chapters;
        }

        private void ChapterView_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            string filePath = ((EpubChapter)ChapterView.SelectedItem).ContentPath;

            filePath = Path.Combine(contentFolderPath, filePath);

            BookView.Source = new Uri(filePath);
        }
    }
}
