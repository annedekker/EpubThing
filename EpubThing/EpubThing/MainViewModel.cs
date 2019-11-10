using EpubThing.Model;
using Microsoft.Win32;
using ReactiveUI;
using System;
using System.Linq;
using System.Windows.Input;

namespace EpubThing
{
    class MainViewModel : ReactiveObject
    {
        EpubManager manager;

        private EpubBook book;
        public EpubBook Book
        {
            get => this.book;
            set => this.RaiseAndSetIfChanged(ref this.book, value);
        }

        private EpubChapter selectedChapter;
        public EpubChapter SelectedChapter
        {
            get => this.selectedChapter;
            set => this.RaiseAndSetIfChanged(ref this.selectedChapter, value);
        }

        private string viewSource;
        public string ViewSource
        {
            get => this.viewSource;
            set => this.RaiseAndSetIfChanged(ref this.viewSource, value);
        }

        private string selectPageText;
        public string SelectPageText
        {
            get => this.selectPageText;
            set => this.RaiseAndSetIfChanged(ref this.selectPageText, value);
        }

        public ICommand OpenFileCommand { get; }
        public ICommand SelectPageCommand { get; }

        public MainViewModel()
        {
            manager = new EpubManager();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            this.OpenFileCommand = ReactiveCommand.Create(OpenFile);
            this.SelectPageCommand = ReactiveCommand.Create(SelectPage);

            this.WhenAnyValue(x => x.SelectedChapter).Subscribe(ch => SelectChapter());
        }

        private void OnExit(object sender, EventArgs e)
        {
            manager.DeleteTempFolder();
        }

        private void OpenFile()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = manager.FilePath;
            ofd.Filter = "Epub files (*.epub)|*.epub";
            if (ofd.ShowDialog() == true)
            {
                this.Book = manager.LoadFromFile(ofd.FileName);
            }
        }

        private void SelectChapter()
        {
            if (SelectedChapter == null) return;

            this.ViewSource = manager.GetChapterFilePath(SelectedChapter);
        }

        private void SelectPage()
        {
            int val;
            if (!Int32.TryParse(SelectPageText, out val))
            {
                SelectPageText = "";
                return;
            }
            else if (val < 1)
            {
                val = 1;
                SelectPageText = val.ToString();
            }
            else if (val > this.Book.LastPageNumber)
            {
                val = this.Book.LastPageNumber;
                SelectPageText = val.ToString();
            }

            string contentFile = this.book.ContentPageNumbers.Keys
                .Where(x => this.Book.ContentPageNumbers[x] <= val)
                .OrderByDescending(x => this.Book.ContentPageNumbers[x])
                .FirstOrDefault();

            string contentLink = contentFile + "#page_" + val.ToString();

            this.ViewSource = contentLink;
        }
    }
}
