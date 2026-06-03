using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.DTOs.Neighborhoods;
using CommunityHub.Ui.Converters;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class NeighborhoodDetailsPage : Page
{
    private readonly NeighborhoodDto _neighborhood;

    public NeighborhoodDetailsPage(NeighborhoodDto neighborhood)
    {
        InitializeComponent();
        _neighborhood = neighborhood;

        NeighborhoodNameText.Text = neighborhood.Name;
        NeighborhoodLocationText.Text = neighborhood.Location;
        DescriptionText.Text = neighborhood.Description;
        StreetsControl.ItemsSource = neighborhood.Streets;

        if (neighborhood.ImagePaths.Count > 0)
            NeighborhoodImage.Source = ImagePathConverter.LoadImage(neighborhood.ImagePaths[0]);
    }

    private void RequestsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new ManageRequestsPage(_neighborhood.CoordinatorId, _neighborhood.Name), "Requests");
    }

    private void MembersButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new NeighborhoodMembersPage(_neighborhood), "Members");
    }

    private void MeetingsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new MeetingsPage(_neighborhood.CoordinatorId, _neighborhood.Id), "Meetings");
    }

    private void CityObjectsButton_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        CoordinatorMainWindow.Instance.NavigateTo(
            new CityObjectsPage(_neighborhood.Id), "City Objects");
    }
}