using System.Windows;

namespace LiveSearchTextBoxControlTest
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly List<string> sourceCollection =
            new List<string>
                {
                    "1",
                    "2",
                    "3",
                    "4",
                    "5",
                    "6",
                    "7",
                    "8",
                    "9",
                    "10",
                    "11",
                    "12",
                    "13",
                    "14",
                    "15",
                    "16",
                    "17",
                    "18",
                    "19",
                    "20",
                    "Hello, world!",
                    "Heello, world!!"
                };

        private Task filterTask;

        private ObservableCollection<string> collection;

        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        public MainWindow()
        {
            FillSourceCollection();
            collection = new ObservableCollection<string>(sourceCollection);
            InitializeComponent();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Collection
        {
            get => collection;
            set
            {
                collection = value;
                OnPropertyChanged();
            }
        }

        public Task FilterTask
        {
            get => filterTask;
            set
            {
                filterTask = value;
                OnPropertyChanged();
            }
        }

        public string FilterText { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void FillSourceCollection()
        {
            Random random = new Random();
            for (int i = 0; i < 1_000_000; i++) sourceCollection.Add(random.Next(1_000_000).ToString());
        }

        private void LiveSearchTextBox_OnFilter(object sender, RoutedEventArgs e)
        {
            FilterTask = FilterTask?.ContinueWith(_ => Filter()) ?? Task.Run(() => Filter());
        }

        private void Filter()
        {
            if (string.IsNullOrEmpty(FilterText))
            {
                Collection = new ObservableCollection<string>(sourceCollection);
                return;
            }

            cancellationTokenSource = new CancellationTokenSource();
            var tempCollection = new ObservableCollection<string>();
            foreach (var str in sourceCollection)
            {
                if (cancellationTokenSource.Token.IsCancellationRequested) return;

                if (str.Contains(FilterText)) tempCollection.Add(str);
            }

            Collection = tempCollection;
        }

        private void LiveSearchTextBox_OnCancelFiltering(object sender, RoutedEventArgs e)
        {
            cancellationTokenSource.Cancel();
        }
    }
}
