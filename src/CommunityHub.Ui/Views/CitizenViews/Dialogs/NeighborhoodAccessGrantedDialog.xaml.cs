using System;
using System.Windows;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs
{
    public partial class NeighborhoodAccessGrantedDialog : Window
    {
        private readonly User _user;
        private readonly Neighborhood _neighborhood;

        public NeighborhoodAccessGrantedDialog(User user, Neighborhood neighborhood)
        {
            InitializeComponent();

            _user = user;
            _neighborhood = neighborhood;

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
            MessageBox.Show("Open My Requests page.");
            Close();
        }
    }
}