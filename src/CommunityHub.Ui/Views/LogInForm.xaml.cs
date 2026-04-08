using System.Windows;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

namespace CommunityHub.Ui.Views;

using CommunityHub.Ui.Views.TenantViews;
using CommunityHub.Ui.Views.ManagerViews;

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

        switch (user.Role)
        {
            case "tenant":
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                mainWindow.NavigateTo(new BrowseBuildingsPage(user));
                break;
            case "manager":
                ManagerMainWindow managerWindow = new ManagerMainWindow(user);
                managerWindow.Show();
                break;
            case "coordinator":

                CoordinatorViews.CoordinatorMainWindow coordinatorWindow = new CoordinatorViews.CoordinatorMainWindow(user.Id);
                coordinatorWindow.Show();
                break;
            case "citizen":
                //CitizenWindow citizenWindow = new CitizenWindow(user);
                // citizenWindow.Show();
                break;
            default:
                ErrorMessageTextBlock.Text = "Unknown user role.";
                ErrorMessageTextBlock.Visibility = Visibility.Visible;
                return;
        }

        this.Hide();
    }
}
