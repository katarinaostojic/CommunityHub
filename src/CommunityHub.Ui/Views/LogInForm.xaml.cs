using System.Windows;
using CommunityHub.Application.Database.Repositories.Shared;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Ui.Helpers;
using CommunityHub.Ui.Views.CitizenViews;
using CommunityHub.Ui.Views.ManagerViews;
using CommunityHub.Ui.Views.TenantViews;

namespace CommunityHub.Ui.Views;

public partial class LogInForm : Window
{
    private readonly UserDbRepository _userRepository;

    public LogInForm()
    {
        InitializeComponent();
        _userRepository = new UserDbRepository();
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string username = UsernameTextBox.Text.Trim();
        string password = PasswordBox.Password.Trim();

        User? user = _userRepository.GetByCredentials(username, password);

        if (user == null)
        {
            ErrorMessageTextBlock.Text = "Invalid username or password.";
            ErrorMessageTextBlock.Visibility = Visibility.Visible;
            return;
        }

        ThemeManager.ApplyTheme(user.Role);

        switch (user.Role)
        {
            case UserRole.Tenant:
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                mainWindow.NavigateTo(new BrowseBuildingsPage(user));
                break;

            case UserRole.Manager:
                ManagerMainWindow managerWindow = new ManagerMainWindow(user);
                managerWindow.Show();
                break;

            case UserRole.Coordinator:
                CoordinatorViews.CoordinatorMainWindow coordinatorWindow =
                    new CoordinatorViews.CoordinatorMainWindow(user.Id);
                coordinatorWindow.Show();
                break;

            case UserRole.Citizen:
                BrowseNeighborhoodPage citizenWindow = new BrowseNeighborhoodPage(user);
                citizenWindow.Show();
                break;

            default:
                ErrorMessageTextBlock.Text = "Unknown user role.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
        }

        Hide();
    }
}