using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Domain.Entities.Shared;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CitizenViewModels.Neighborhoods;
using CommunityHub.Ui.Views;
using CommunityHub.Ui.Helpers.Citizen;
using System.Windows;
using System.Windows.Media;
using CommunityHub.Application.Services.Entities.Neighborhoods.Meetings;

namespace CommunityHub.Ui.Views.CitizenViews;

public partial class MyProfilePage : Window
{
    private readonly User _user;
    private readonly long _neighborhoodId;
    private readonly MyProfileViewModel _viewModel;

    public MyProfilePage(User user, long neighborhoodId, string neighborhoodName)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        TrustRecordService service = Injector.CreateInstance<TrustRecordService>();
        _viewModel = new MyProfileViewModel(service, user, neighborhoodId, neighborhoodName);
        DataContext = _viewModel;

        LoggedInUserTextBlock.Text = _user.Username;
        UsernameText.Text = _user.Username;
        NeighborhoodNameText.Text = neighborhoodName;

        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;
        ThemeToggleButton.IsChecked = ThemeManager.IsDark;
        ThemeToggleButton.IsChecked = ThemeManager.IsDark;
        LanguageToggleButton.IsChecked = !LanguageManager.IsSerbianActive;
        Loaded += (s, e) =>
        {
            var dto = _viewModel.TrustRecord;
            if (dto != null)
                DrawPieChart(dto.EventsOrganized, dto.EventsVolunteered);
        };
    }

    private void BurgerButton_Click(object sender, RoutedEventArgs e)
        => CitizenMenu.Visibility = Visibility.Visible;

    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        switch (destination)
        {
            case "Neighborhoods": new BrowseNeighborhoodPage(_user).Show(); Close(); break;
            case "MyRequests": new MyRequestsPage(_user).Show(); Close(); break;
            case "Events": NavigateToEvents(); break;
            case "Citizens": NavigateToCitizens(); break;
            case "Meetings": NavigateToMeetings(); break;
            case "Profile": break;
            case "CityObjects": CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "Budget": CitizenNavigationHelper.NavigateToBudget(_user, this); break;
        }
    }

    private void CitizenMenu_LogoutRequested()
    {
        CitizenMenu.Visibility = Visibility.Collapsed;
        new LogInForm().Show();
        Close();
    }

    private void NavigateToEvents()
    {
        long? nId = GetMembershipId();
        if (nId == null) return;
        new EventsPage(_user, nId.Value).Show();
        Close();
    }

    private void NavigateToCitizens()
    {
        new NeighborhoodCitizensPage(_user, _neighborhoodId).Show();
        Close();
    }

    private void NavigateToMeetings()
    {
        long? nId = GetMembershipId();
        if (nId == null) return;
        new MeetingsPage(_user, nId.Value).Show();
        Close();
    }

    private long? GetMembershipId()
    {
        NeighborhoodAccessRequestService s = Injector.CreateInstance<NeighborhoodAccessRequestService>();
        long? nId = s.GetMembershipNeighborhoodId(_user.Id);
        if (nId == null) MessageBox.Show("You are not a member of any neighborhood.");
        return nId;
    }
    private void DrawPieChart(int organized, int volunteered)
    {
        PieCanvas.Children.Clear();
        int total = organized + volunteered;
        if (total == 0) return;

        double radius = 100;
        double cx = 110, cy = 110;
        double angle = (organized / (double)total) * 360.0;
        double rad = angle * Math.PI / 180.0;

        double x = cx + radius * Math.Sin(rad);
        double y = cy - radius * Math.Cos(rad);

        bool isLarge = angle > 180;

        var organizedPath = new System.Windows.Shapes.Path
        {
            Fill = new SolidColorBrush(Color.FromRgb(26, 58, 92)),
            Data = new PathGeometry(new[]
            {
            new PathFigure(new Point(cx, cy), new PathSegment[]
            {
                new LineSegment(new Point(cx, cy - radius), true),
                new ArcSegment(new Point(x, y), new Size(radius, radius), 0, isLarge,
                    SweepDirection.Clockwise, true),
                new LineSegment(new Point(cx, cy), true)
            }, true)
        })
        };

        var volunteeredPath = new System.Windows.Shapes.Path
        {
            Fill = new SolidColorBrush(Color.FromRgb(214, 228, 240)),
            Data = new PathGeometry(new[]
            {
            new PathFigure(new Point(cx, cy), new PathSegment[]
            {
                new LineSegment(new Point(x, y), true),
                new ArcSegment(new Point(cx, cy - radius), new Size(radius, radius), 0,
                    !isLarge, SweepDirection.Clockwise, true),
                new LineSegment(new Point(cx, cy), true)
            }, true)
        })
        };

        PieCanvas.Children.Add(volunteeredPath);
        PieCanvas.Children.Add(organizedPath);
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.ToggleTheme();
    }
    private void LanguageToggle_Click(object sender, RoutedEventArgs e)
    {
        LanguageManager.ToggleLanguage();
    }
}