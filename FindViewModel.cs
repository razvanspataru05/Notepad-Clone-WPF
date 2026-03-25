using System.ComponentModel;
using System.Windows;

namespace Tema1
{
    class FindViewModel : INotifyPropertyChanged
    {
        private string _searchText;
        private string _replaceText;
        private readonly MainViewModel _mainViewModel;
        public event PropertyChangedEventHandler PropertyChanged;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }
        public string ReplaceText
        {
            get => _replaceText;
            set
            {
                if (_replaceText != value)
                {
                    _replaceText = value;
                    OnPropertyChanged(nameof(ReplaceText));
                }
            }
        }
        public string WindowTitle { get; set; }
        public bool IsReplaceMode { get; set; }
        public Action<int, int> HighlightResult { get; set; }
        public RelayCommand FindInCurrentTabCommand { get; set; }
        public RelayCommand FindInAllTabsCommand { get; set; }
        public RelayCommand ReplaceCommand { get; set; }
        public RelayCommand ReplaceAllCommand { get; set; }
        public RelayCommand ReplaceAllInAllTabsCommand { get; set; }

        public FindViewModel(MainViewModel mainViewModel, bool isReplaceMode)
        {
            _mainViewModel = mainViewModel;
            FindInCurrentTabCommand = new RelayCommand(_ => Search(false));
            FindInAllTabsCommand = new RelayCommand(_ => Search(true));
            ReplaceCommand = new RelayCommand(_ => Replace());
            ReplaceAllCommand = new RelayCommand(_ => ReplaceAll(false));
            ReplaceAllInAllTabsCommand = new RelayCommand(_ => ReplaceAll(true));

            IsReplaceMode = isReplaceMode;
            WindowTitle = IsReplaceMode ? "Replace" : "Find";
        }

        private void Search(bool allTabs)
        {
            if (string.IsNullOrEmpty(SearchText)) return;

            if (allTabs)
            {
                foreach (var tab in _mainViewModel.Tabs)
                {
                    if (tab.Content == null) continue;

                    int index = tab.Content?.IndexOf(SearchText) ?? -1;
                    if (index != -1)
                    {
                        _mainViewModel.SelectedTab = tab;
                        HighlightResult?.Invoke(index, SearchText.Length);
                        return;
                    }
                }
                MessageBox.Show("Text not found", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (_mainViewModel.SelectedTab.Content == null)
                {
                    MessageBox.Show("File is empty", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                int index = _mainViewModel.SelectedTab?.Content.IndexOf(SearchText) ?? -1;
                if (index != -1)
                {
                    HighlightResult?.Invoke(index, SearchText.Length);
                }
                else
                    MessageBox.Show("Text not found", "Find", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Replace()
        {
            if (string.IsNullOrEmpty(SearchText)) return;

            var tab = _mainViewModel.SelectedTab;
            if (tab?.Content == null) return;

            int index = tab.Content.IndexOf(SearchText);
            if (index != -1)
            {
                tab.Content = tab.Content.Remove(index, SearchText.Length).Insert(index, ReplaceText ?? "");
                tab.IsModified = true;
            }
            else
                MessageBox.Show("Text not found", "Replace", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void ReplaceAll(bool allTabs)
        {
            if (string.IsNullOrEmpty(SearchText)) return;

            if (allTabs)
            {
                foreach (var tab in _mainViewModel.Tabs)
                {
                    if (tab.Content == null) continue;

                    tab.Content = tab.Content.Replace(SearchText, ReplaceText ?? "");
                    tab.IsModified = true;
                }
            }
            else
            {
                var tab = _mainViewModel.SelectedTab;
                if (tab.Content == null) return;

                tab.Content = tab.Content.Replace(SearchText, ReplaceText ?? "");
                tab.IsModified = true;
            }
        }
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
