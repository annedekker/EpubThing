using EpubThing.Model;
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
        EpubManager manager;

        public MainWindow()
        {
            this.manager = new EpubManager();

            InitializeComponent();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);
        }

        private void OnExit(object sender, EventArgs e)
        {
            manager.DeleteTempFolder();
        }

        private void Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = manager.FilePath;
            ofd.Filter = "Epub files (*.epub)|*.epub";
            if (ofd.ShowDialog() == true)
            {
                manager.LoadFromFile(ofd.FileName);
                
            }
        }

        private void ChapterView_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            /*string filePath = ((EpubChapter)ChapterView.SelectedItem).ContentPath;

            filePath = Path.Combine(contentFolderPath, filePath);

            BookView.Source = new Uri(filePath);*/
        }
    }
}
