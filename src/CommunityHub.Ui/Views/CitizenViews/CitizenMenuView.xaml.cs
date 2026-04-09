using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CommunityHub.Ui.Views.CitizenViews
{
    public partial class CitizenMenuView : UserControl
    {
        public event Action<string>? NavigationRequested;
        public event Action? CloseRequested;
        public event Action? LogoutRequested;

        public CitizenMenuView()
        {
            InitializeComponent();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is string destination)
            {
                NavigationRequested?.Invoke(destination);
            }
        }

        private void Overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseRequested?.Invoke();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            LogoutRequested?.Invoke();
        }
    }
}