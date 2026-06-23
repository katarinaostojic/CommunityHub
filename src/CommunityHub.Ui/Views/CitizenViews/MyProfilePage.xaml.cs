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

    public MyProfilePage(User user, long neighborhoodId, string neighborhoodName, bool openMenuOnLoad = false)
    {
        InitializeComponent();
        _user = user;
        _neighborhoodId = neighborhoodId;

        TrustRecordService service = Injector.CreateInstance<TrustRecordService>();
        _viewModel = new MyProfileViewModel(service, user, neighborhoodId, neighborhoodName);
        DataContext = _viewModel;

        NavBar.SetTitle(TryFindResource("Profile_Title") as string ?? "Profile");
        LanguageManager.LanguageChanged += () => NavBar.SetTitle(TryFindResource("Profile_Title") as string ?? "Profile");
        NavBar.SetUrl("communityhub://profile");
        NavBar.SetUsername(_user.Username);
        NavBar.UpdateNavButtons();

        NavBar.BurgerClicked += () => CitizenMenu.Visibility = Visibility.Visible;
        NavBar.ProfileClicked += () => { /* vec smo na profilu */ };
        NavBar.BackNavigated += HandleNavBack;
        NavBar.ForwardNavigated += HandleNavForward;
        UsernameText.Text = _user.Username;
        NeighborhoodNameText.Text = neighborhoodName;

        CitizenMenu.CloseRequested += CitizenMenu_CloseRequested;
        CitizenMenu.NavigationRequested += CitizenMenu_NavigationRequested;
        CitizenMenu.LogoutRequested += CitizenMenu_LogoutRequested;

        if (openMenuOnLoad)
            CitizenMenu.Visibility = Visibility.Visible;
        Loaded += (s, e) =>
        {
            var dto = _viewModel.TrustRecord;
            if (dto != null)
                DrawPieChart(dto.EventsOrganized, dto.EventsVolunteered);
        };
    }


    private void CitizenMenu_CloseRequested()
        => CitizenMenu.Visibility = Visibility.Collapsed;

    private void CitizenMenu_NavigationRequested(string destination)
    {
        switch (destination)
        {
            case "Neighborhoods":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                new BrowseNeighborhoodPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "MyRequests":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                new MyRequestsPage(_user, openMenuOnLoad: true).Show(); Close(); break;
            case "Events":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToEvents(_user, this); break;
            case "Citizens":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCitizens(_user, this); break;
            case "Meetings":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToMeetings(_user, this); break;
            case "CityObjects":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCityObjects(_user, this); break;
            case "CoordinatorReviews":
                NavigationHistory.NavigateTo(new NavigationEntry("MyProfile", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToCoordinatorReviews(_user, _neighborhoodId, this); break;
            case "Budget":
                NavigationHistory.NavigateTo(new NavigationEntry("Profile", _user, _neighborhoodId));
                CitizenNavigationHelper.NavigateToBudget(_user, this); break;
            case "Profile": CitizenMenu.Visibility = Visibility.Collapsed; break;
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
        if (nId == null) { MsgHelper.Warn("Msg_NotMember", "Msg_Error"); return null; }
        return nId;
    }

    private void DrawPieChart(int organized, int volunteered)
    {
        PieCanvas.Children.Clear();
        int total = organized + volunteered;
        if (total == 0) return;

        double angle = (organized / (double)total) * 360.0;
        var (x, y) = CalculateArcEndPoint(angle);
        bool isLarge = angle > 180;

        PieCanvas.Children.Add(BuildVolunteeredSlice(x, y, isLarge));
        PieCanvas.Children.Add(BuildOrganizedSlice(x, y, isLarge));
    }

    private static (double x, double y) CalculateArcEndPoint(double angle)
    {
        const double radius = 100, cx = 110, cy = 110;
        double rad = angle * Math.PI / 180.0;
        return (cx + radius * Math.Sin(rad), cy - radius * Math.Cos(rad));
    }

    private static System.Windows.Shapes.Path BuildOrganizedSlice(double x, double y, bool isLarge)
    {
        const double radius = 100, cx = 110, cy = 110;
        return new System.Windows.Shapes.Path
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
    }

    private static System.Windows.Shapes.Path BuildVolunteeredSlice(double x, double y, bool isLarge)
    {
        const double radius = 100, cx = 110, cy = 110;
        return new System.Windows.Shapes.Path
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
    }

    private void ThemeToggle_Click(object sender, RoutedEventArgs e)
    {
        ThemeManager.ToggleTheme();
    }

    private void LanguageToggle_Click(object sender, RoutedEventArgs e)
    {
        LanguageManager.ToggleLanguage();
    }
    private void HandleNavBack()
    {
        var target = NavigationHistory.GoBack(new NavigationEntry("Profile", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }

    private void HandleNavForward()
    {
        var target = NavigationHistory.GoForward(new NavigationEntry("Profile", _user, _neighborhoodId));
        if (target != null) CitizenNavigationHelper.NavigateToEntry(target, this);
        NavBar.UpdateNavButtons();
    }
}