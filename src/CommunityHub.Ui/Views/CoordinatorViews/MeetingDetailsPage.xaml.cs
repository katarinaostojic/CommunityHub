using CommunityHub.Application.DependencyInjection;
using CommunityHub.Application.Services.Entities.Neighborhoods;
using CommunityHub.Ui.ViewModels.CoordinatorViewModels.Neighborhoods;
using System.Windows.Controls;

namespace CommunityHub.Ui.Views.CoordinatorViews;

public partial class MeetingDetailsPage : Page
{
    public MeetingDetailsPage(long meetingId)
    {
        InitializeComponent();
        MeetingService meetingService = Injector.CreateInstance<MeetingService>();
        DataContext = new MeetingDetailsViewModel(meetingService, meetingId);
    }
}