using EpubThing.Model;
using System.Windows;
using System.Windows.Controls;

namespace EpubThing
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainViewModel();
        }

        private void Chapters_SelectionChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            (this.DataContext as MainViewModel).SelectedChapter =
                (EpubChapter)(sender as TreeView).SelectedItem;
        }
    }
}
