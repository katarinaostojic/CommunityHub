using CommunityHub.Application.Domain;
using CommunityHub.Ui.Views.ManagerViews.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.ManagerViews;

public partial class MyBuildingsPage : Page
{
    private readonly User _currentUser;

    public MyBuildingsPage(User user)
    {
        InitializeComponent();
        _currentUser = user;
    }

    private void RegisterBuilding_Click(object sender, RoutedEventArgs e)
    {
        RegisterBuildingDialog dialog = new RegisterBuildingDialog(_currentUser);
        dialog.ShowDialog();
    }
}