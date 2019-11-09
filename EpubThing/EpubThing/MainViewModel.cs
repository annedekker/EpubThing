using EpubThing.Model;
using Microsoft.Win32;
using ReactiveUI;
using System;
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

        public ICommand OpenFileCommand { get; }

        public MainViewModel()
        {
            manager = new EpubManager();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnExit);

            this.OpenFileCommand = ReactiveCommand.Create(OpenFile);

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
    }
}
