using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;

namespace Tema1
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly string configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Tema1", "treestate.txt");
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isFolderExplorerVisible = true;
        private ObservableCollection<TabViewModel> _tabs;
        private FileSystemItem _copiedDirectory;

        private TabViewModel _selectedTab;
        public ObservableCollection<TabViewModel> Tabs
        {
            get => _tabs;
            set => _tabs = value;
        }
        public TabViewModel SelectedTab
        {
            get => _selectedTab;
            set
            {
                if (_selectedTab != value)
                {
                    _selectedTab = value;
                    OnPropertyChanged(nameof(SelectedTab));
                }
            }
        }
        public ObservableCollection<FileSystemItem> Partitions { get; set; }
        public bool IsFolderExplorerVisible
        {
            get => _isFolderExplorerVisible;
            set
            {
                if (_isFolderExplorerVisible != value)
                {
                    _isFolderExplorerVisible = value;
                    OnPropertyChanged(nameof(IsFolderExplorerVisible));
                }
            }
        }

        public RelayCommand NewFileCommand { get; set; }
        public RelayCommand OpenFileCommand { get; set; }
        public RelayCommand SaveFileCommand { get; set; }
        public RelayCommand SaveFileAsCommand { get; set; }
        public RelayCommand CloseFileCommand { get; set; }
        public RelayCommand CloseAllFilesCommand { get; set; }
        public RelayCommand ExitCommand { get; set; }
        public RelayCommand ViewStandardCommand { get; set; }
        public RelayCommand ViewFolderExplorerCommand { get; set; }
        public RelayCommand HelpAboutCommand { get; set; }
        public RelayCommand OpenFromTreeCommand { get; set; }
        public RelayCommand NewFileInDirectoryCommand { get; set; }
        public RelayCommand CopyPathCommand { get; set; }
        public RelayCommand CopyDirectoryCommand { get; set; }
        public RelayCommand PasteDirectoryCommand { get; set; }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            Tabs = new();
            Partitions = new();
            foreach (var drive in DriveInfo.GetDrives())
                Partitions.Add(new FileSystemItem(drive.Name));

            NewFileCommand = new RelayCommand(_ => CreateNewTab());
            OpenFileCommand = new RelayCommand(_ => OpenFile());
            SaveFileCommand = new RelayCommand(_ => SaveFile(SelectedTab));
            SaveFileAsCommand = new RelayCommand(_ => SaveFileAs());
            CloseFileCommand = new RelayCommand(param => CloseFile(param as TabViewModel ?? SelectedTab));
            CloseAllFilesCommand = new RelayCommand(_ => CloseAllFiles());
            ExitCommand = new RelayCommand(_ => Exit());

            ViewStandardCommand = new RelayCommand(_ => IsFolderExplorerVisible = false);
            ViewFolderExplorerCommand = new RelayCommand(_ => IsFolderExplorerVisible = true);

            CopyPathCommand = new RelayCommand(CopyPath);
            CopyDirectoryCommand = new RelayCommand(CopyDirectory);
            PasteDirectoryCommand = new RelayCommand(PasteDirectory);
            NewFileInDirectoryCommand = new RelayCommand(NewFileInDirectory);

            HelpAboutCommand = new RelayCommand(_ => new AboutWindow()
            { Owner = Application.Current.MainWindow }.ShowDialog());
            OpenFromTreeCommand = new RelayCommand(param => OpenFromTree(param as FileSystemItem));
            CreateNewTab();
        }

        private void CreateNewTab()
        {
            TabViewModel tab = new();
            Tabs.Add(tab);
            SelectedTab = tab;
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var existing = Tabs.FirstOrDefault(t => t.Filepath == openFileDialog.FileName);
                if (existing != null)
                {
                    SelectedTab = existing;
                    return;
                }
                string content = File.ReadAllText(openFileDialog.FileName);
                TabViewModel tab = new();
                tab.Content = content;
                tab.Title = Path.GetFileName(openFileDialog.FileName);
                tab.Filepath = openFileDialog.FileName;
                tab.IsModified = false;
                Tabs.Add(tab);
                SelectedTab = tab;
            }
        }
        private void SaveFile(TabViewModel tab)
        {
            if (tab.Filepath == null)
            {
                SaveFileAs();
                return;
            }
            File.WriteAllText(tab.Filepath, tab.Content);
            tab.IsModified = false;
        }
        private void SaveFileAs()
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Text documents (*.txt)|*.txt|All files (*.*)|*.*";
            saveFileDialog.FileName = SelectedTab.CleanTitle;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, SelectedTab.Content);
                SelectedTab.IsModified = false;
                SelectedTab.Title = Path.GetFileName(saveFileDialog.FileName);
                SelectedTab.Filepath = saveFileDialog.FileName;
            }
        }

        private void CloseFile(TabViewModel tab)
        {
            if (tab.IsModified)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Do you want to save the changes to {tab.Title}",
                    "Save file",
                    MessageBoxButton.YesNoCancel,
                    MessageBoxImage.Warning);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        SaveFile(tab);
                        if (Tabs.Count > 1)
                            Tabs.Remove(tab);
                        return;

                    case MessageBoxResult.No:
                        Tabs.Remove(tab);
                        if (!Tabs.Any())
                        {
                            TabViewModel newTab = new();
                            Tabs.Add(newTab);
                            SelectedTab = newTab;
                        }
                        return;

                    case MessageBoxResult.Cancel:
                        return;
                }
            }
            else
            {
                if (Tabs.Count > 1)
                    Tabs.Remove(tab);
            }
        }
        private void CloseAllFiles()
        {
            foreach (var file in Tabs.ToList())
            {
                CloseFile(file);
            }
        }
        private void Exit()
        {
            CloseAllFiles();
            Application.Current.Shutdown();
        }
        private void OpenFromTree(FileSystemItem item)
        {
            if (item == null || item.IsDirectory) return;

            string extension = Path.GetExtension(item.FullPath);
            string[] extensions = { ".log", ".tmp", ".sys" };

            if (extensions.Contains(extension))
            {
                MessageBox.Show($"File with extension {extension} cannot be opened", "Info",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var existing = Tabs.FirstOrDefault(t => t.Filepath == item.FullPath);
            if (existing != null)
            {
                SelectedTab = existing;
                return;
            }

            string content = File.ReadAllText(item.FullPath);
            TabViewModel tab = new();
            tab.Title = item.Name;
            tab.Content = content;
            tab.Filepath = item.FullPath;
            tab.IsModified = false;
            Tabs.Add(tab);
            SelectedTab = tab;
        }
        private void CollectExpandedPaths(IEnumerable<FileSystemItem> items, List<string> paths)
        {
            foreach (var item in items)
            {
                if (item == null) continue;

                if (item.IsExpanded)
                    paths.Add(item.FullPath);

                CollectExpandedPaths(item.Items, paths);
            }
        }
        public void SaveTreeState()
        {
            List<string> expandedNodes = new();
            CollectExpandedPaths(Partitions, expandedNodes);
            Directory.CreateDirectory(Path.GetDirectoryName(configPath));
            File.WriteAllLines(configPath, expandedNodes);

            string visibilityPath = Path.Combine(Path.GetDirectoryName(configPath), "visibility.txt");
            File.WriteAllText(visibilityPath, IsFolderExplorerVisible.ToString());
        }
        private void RestoreExpandedPaths(IEnumerable<FileSystemItem> items, string[] paths)
        {
            foreach (var item in items)
            {
                if (item == null) continue;

                if (paths.Contains(item.FullPath))
                    item.IsExpanded = true;

                RestoreExpandedPaths(item.Items, paths);
            }
        }
        public void LoadTreeState()
        {
            if (!File.Exists(configPath)) return;

            string[] paths = File.ReadAllLines(configPath);
            RestoreExpandedPaths(Partitions, paths);

            string visibilityPath = Path.Combine(Path.GetDirectoryName(configPath), "visibility.txt");
            if (File.Exists(visibilityPath))
                IsFolderExplorerVisible = bool.Parse(File.ReadAllText(visibilityPath));
        }
        private void CopyPath(object parameter)
        {
            var item = (FileSystemItem)parameter;

            if (item == null || !item.IsDirectory) return;

            Clipboard.SetText(item.FullPath);
        }
        private void NewFileInDirectory(object parameter)
        {
            var item = (FileSystemItem)parameter;

            if (item == null || !item.IsDirectory) return;

            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Text documents (*.txt)|*.txt |All files (*.*)|*.*";
            saveFileDialog.InitialDirectory = item.FullPath;
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, "");
                var existing = Tabs.FirstOrDefault(t => t.Filepath == saveFileDialog.FileName);
                if (existing != null)
                {
                    SelectedTab = existing;
                    return;
                }
                if (File.Exists(saveFileDialog.FileName))
                    item.Items.Add(new FileSystemItem(saveFileDialog.FileName));

                TabViewModel tab = new();
                tab.Title = Path.GetFileName(saveFileDialog.FileName);
                tab.Filepath = saveFileDialog.FileName;
                Tabs.Add(tab);
                SelectedTab = tab;
            }
        }
        private void CopyDirectory(object parameter)
        {
            var item = (FileSystemItem)parameter;

            if (item == null || !item.IsDirectory) return;

            _copiedDirectory = item;
        }
        private void PasteDirectory(object parameter)
        {
            var destination = (FileSystemItem)parameter;

            if (destination == null || !destination.IsDirectory || _copiedDirectory == null) return;

            string destinationPath = Path.Combine(destination.FullPath, Path.GetFileName(_copiedDirectory.FullPath));

            if (Directory.Exists(destinationPath))
            {
                MessageBoxResult result = MessageBox.Show("Directory already exists. Overwrite?", "Info",
                    MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.No) return;
            }

            CopyContent(_copiedDirectory.FullPath, destinationPath);
            var existing = destination.Items.FirstOrDefault(item => destinationPath == item.FullPath);
            if (existing == null)
                destination.Items.Add(new FileSystemItem(destinationPath));
        }
        private void CopyContent(string source, string destination)
        {
            Directory.CreateDirectory(destination);

            string[] files = Directory.GetFiles(source);
            foreach (var file in files)
            {
                string path = Path.Combine(destination, Path.GetFileName(file));
                File.Copy(file, path, true);
            }

            string[] directories = Directory.GetDirectories(source);
            foreach (var subdir in directories)
            {
                string newDestination = Path.Combine(destination, Path.GetFileName(subdir));
                CopyContent(subdir, newDestination);
            }
        }
    }
}
