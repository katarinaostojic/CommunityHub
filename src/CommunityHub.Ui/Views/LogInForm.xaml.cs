using System.Windows;
using CommunityHub.Application.Database.Repositories;
using CommunityHub.Application.Domain;

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

        switch (user.Role)
        {
            case "tenant":
                BrowseBuildingsWindow browseBuildingsWindow = new BrowseBuildingsWindow(user);
                browseBuildingsWindow.Show();
                break;
            case "manager":
                // ManagerWindow managerWindow = new ManagerWindow(user);
                // managerWindow.Show();
                break;
            case "coordinator":
                // CoordinatorWindow coordinatorWindow = new CoordinatorWindow(user);
                // coordinatorWindow.Show();
                break;
            case "citizen":
                // CitizenWindow citizenWindow = new CitizenWindow(user);
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
