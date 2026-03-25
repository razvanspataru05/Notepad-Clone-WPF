using System.ComponentModel;
using System.Windows;

namespace Tema1
{
    public partial class MainWindow : Window
    {
        private FindWindow _findWindow;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            var vm = (MainViewModel)DataContext;
            vm.LoadTreeState();
        }

        private void FindMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_findWindow == null || !_findWindow.IsLoaded)
            {
                var vm = (MainViewModel)DataContext;
                _findWindow = new FindWindow(vm, MainTabControl, false) { Owner = this };
                _findWindow.Show();
            }
            else
                _findWindow.Focus();
        }
        private void ReplaceMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (_findWindow == null || !_findWindow.IsLoaded)
            {
                var vm = (MainViewModel)DataContext;
                _findWindow = new FindWindow(vm, MainTabControl, true) { Owner = this };
                _findWindow.Show();
            }
            else
                _findWindow.Focus();
        }
        private void TreeView_MouseDoubleClick(object sender, RoutedEventArgs e)
        {
            var vm = (MainViewModel)DataContext;
            var item = (FileSystemItem)FolderExplorer.SelectedItem;
            vm.OpenFromTreeCommand.Execute(item);
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            var vm = (MainViewModel)DataContext;
            vm.SaveTreeState();
        }
    }
}