using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace EpubThing.Model
{
    class EpubManager
    {
        public EpubBook Book { get; private set; }
        public string TempFolderPath { get; private set; }
        public string FilePath { get; private set; }
        public string ContentFolderPath { get; private set; }

        public EpubManager()
        {
            TempFolderPath = Path.Combine(Path.GetTempPath(), "EpubThing");
            FilePath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        public void DeleteTempFolder()
        {
            if (Directory.Exists(TempFolderPath)) Directory.Delete(TempFolderPath, true);
        }

        public EpubBook LoadFromFile(string filename)
        {
            this.FilePath = filename;

            DeleteTempFolder();
            ZipFile.ExtractToDirectory(FilePath, TempFolderPath);

            string contentFilePath = XmlServices.GetContentFilePath(
                Path.Combine(TempFolderPath, "META-INF", "container.xml"));

            this.ContentFolderPath = Path.GetDirectoryName(
                Path.Combine(TempFolderPath, contentFilePath));

            string ncxFilePath = XmlServices.GetNcxFilePath(
                Path.Combine(TempFolderPath, contentFilePath));

            this.Book = new EpubBook();

            this.Book.Title = XmlServices.GetTitle(
                Path.Combine(TempFolderPath, contentFilePath));

            this.Book.Chapters = XmlServices.GetChapters(
                Path.Combine(ContentFolderPath, ncxFilePath));

            foreach (var ch in this.Book.Chapters)
                HtmlReader.AddChapterPageNumber(ch, this);

            GetContentPageNumbers();

            return this.Book;
        }

        public string GetChapterFilePath(EpubChapter chapter)
        {
            return Path.Combine(ContentFolderPath, chapter.FilePath);
        }

        private void GetContentPageNumbers()
        {
            string[] htmlFiles = Directory.GetFiles(ContentFolderPath, "*.html", SearchOption.AllDirectories);

            foreach (var filename in htmlFiles)
            {
                int? pageNum = HtmlReader.GetContentFilePageNumber(filename);

                if (pageNum != null)
                    this.Book.ContentPageNumbers[filename] = pageNum.Value;
            }

            string lastFile = this.Book.ContentPageNumbers.Keys
                .OrderByDescending(k => this.Book.ContentPageNumbers[k])
                .First();

            this.Book.LastPageNumber = HtmlReader.GetLastPageNumber(lastFile);
        }
    }
}
