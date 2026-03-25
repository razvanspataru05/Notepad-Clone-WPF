using System.ComponentModel;

namespace Tema1
{
    public class TabViewModel : INotifyPropertyChanged
    {
        private static int _counter = 1;
        private string _title;
        private string _content;
        private string? _filepath;
        private bool _isModified = false;
        public event PropertyChangedEventHandler PropertyChanged;

        public string CleanTitle => _title;

        public TabViewModel()
        {
            _title = $"File {_counter++}";
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Title
        {
            get => IsModified ? _title + "*" : _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    IsModified = true;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }
        public string Filepath
        {
            get => _filepath;
            set
            {
                if (_filepath != value)
                {
                    _filepath = value;
                    OnPropertyChanged(nameof(Filepath));
                }
            }
        }

        public bool IsModified
        {
            get => _isModified;
            set
            {
                if (_isModified != value)
                {
                    _isModified = value;
                    OnPropertyChanged(nameof(IsModified));
                    OnPropertyChanged(nameof(Title));
                }   
            }
        }
    }
}
