using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Tema1
{
    public partial class FindWindow : Window
    {
        public FindWindow(MainViewModel mainViewModel, TabControl tabControl, bool isReplaceMode)
        {
            InitializeComponent();
            DataContext = new FindViewModel(mainViewModel, isReplaceMode);
            var vm = (FindViewModel)DataContext;
            vm.HighlightResult = (index, length) => SelectText(tabControl, index, length);
        }

        private void SelectText(TabControl tabControl, int index, int length)
        {
            var textBox = FindTextBox(tabControl);
            if (textBox != null)
            {
                textBox.Focus();
                textBox.Select(index, length);
            }
        }

        private TextBox FindTextBox(DependencyObject parent)
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is TextBox tb) return tb;

                var result = FindTextBox(child);

                if (result != null) return result;
            }
            return null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
