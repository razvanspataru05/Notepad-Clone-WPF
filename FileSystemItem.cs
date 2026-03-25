using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;

namespace Tema1
{
    public class FileSystemItem : INotifyPropertyChanged
    {
        public string Name { get; }
        public string FullPath { get; }
        public bool IsDirectory { get; }
        public ObservableCollection<FileSystemItem> Items { get; }
        public event PropertyChangedEventHandler PropertyChanged;

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(nameof(IsExpanded));
                    if (value) LoadChildren();
                }
            }
        }

        public FileSystemItem(string path)
        {
            FullPath = path;
            Name = string.IsNullOrEmpty(Path.GetFileName(path)) ? path : Path.GetFileName(path);
            IsDirectory = Directory.Exists(path);
            Items = new();
            if (IsDirectory) Items.Add(null);
        }
        private void LoadChildren()
        {
            if (!IsDirectory) return;

            if (Items.Count == 1 && Items[0] == null)
                Items.Clear();
            else
                return;
            try
            {
                foreach (var dir in Directory.GetDirectories(FullPath))
                    Items.Add(new FileSystemItem(dir));

                foreach (var file in Directory.GetFiles(FullPath))
                    Items.Add(new FileSystemItem(file));
            }
            catch { }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
