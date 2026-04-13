using System;
using System.Windows;
using CommunityHub.Application.Domain;
using CommunityHub.Ui.Views.CitizenViews;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs
{
    public partial class NeighborhoodAccessGrantedDialog : Window
    {
        private readonly User _user;
        private readonly Neighborhood _neighborhood;
        private readonly Window _parentWindow;

        public NeighborhoodAccessGrantedDialog(User user, Neighborhood neighborhood, Window parentWindow)
        {
            InitializeComponent();

            _user = user;
            _neighborhood = neighborhood;
            _parentWindow = parentWindow;

            LoadData();

            CloseButton.Click += CloseButton_Click;
            GoToDashboardButton.Click += GoToDashboardButton_Click;
            ViewMyRequestsButton.Click += ViewMyRequestsButton_Click;
        }

        private void LoadData()
        {
            NeighborhoodNameText.Text = _neighborhood.Name;
            CitizenNameText.Text = _user.Username;
            JoinDateText.Text = DateTime.Now.ToString("dd.MM.yyyy.");
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void GoToDashboardButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ViewMyRequestsButton_Click(object sender, RoutedEventArgs e)
        {
            MyRequestsPage myRequestsPage = new MyRequestsPage(_user);
            myRequestsPage.Show();
            _parentWindow.Close();
            Close();
        }
    }
}