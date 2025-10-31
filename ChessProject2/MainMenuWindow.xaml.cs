using System.Windows;

namespace ChessProject2
{
    public partial class MainMenuWindow : Window
    {
        public MainMenuWindow()
        {
            InitializeComponent();
        }

        private void PlayWithFriend_Click(object sender, RoutedEventArgs e)
        {
            // Открываем окно выбора стороны
            SideSelectionWindow sideWindow = new SideSelectionWindow();
            sideWindow.Show();
            this.Close();
        }

        private void PlayWithBot_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Режим игры с ботом будет добавлен в будущем обновлении!",
                          "В разработке",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
