using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityHub.Application.Domain;
using CommunityHub.Application.Services;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs
{
    public partial class NeighborhoodAccessRequestDialog : Window
    {
        private readonly User _user;
        private readonly Neighborhood _neighborhood;
        private readonly NeighborhoodAccessRequestService _service;

        public NeighborhoodAccessRequestDialog(User user, Neighborhood neighborhood)
        {
            InitializeComponent();

            _user = user;
            _neighborhood = neighborhood;
            _service = new NeighborhoodAccessRequestService();

            DataContext = _neighborhood;

            LoadData();

            CloseButton.Click += CloseButton_Click;
            StartRequestFlow();
        }

        private void LoadData()
        {
            NeighborhoodNameText.Text = _neighborhood.Name;
            DescriptionText.Text = _neighborhood.Description;
            AddressText.Text = BuildAddressText();
            LocationText.Text = $"Location: {_neighborhood.Location.CityName}, {_neighborhood.Location.CountryName}";
        }

        private string BuildAddressText()
        {
            if (_neighborhood.Streets == null || !_neighborhood.Streets.Any())
                return "Address: No street information available";

            return "Address: " + string.Join(", ",
                _neighborhood.Streets.Select(s => $"{s.StreetName} {s.StartNumber} - {s.EndNumber}"));
        }

        private async void StartRequestFlow()
        {
            await Task.Delay(3000);
            AccessRequestResult result = _service.RequestAccess(_user, _neighborhood);
            Close();
            HandleRequestResult(result);
        }

        private void HandleRequestResult(AccessRequestResult result)
        {
            switch (result)
            {
                case AccessRequestResult.Granted:
                    new NeighborhoodAccessGrantedDialog(_user, _neighborhood, this.Owner).ShowDialog();
                    break;

                case AccessRequestResult.RequestCreated:
                    new NeighborhoodRequestCreatedDialog(_user, _neighborhood).ShowDialog();
                    break;

                case AccessRequestResult.AlreadyPending:
                    ShowInfoMessage("You already have a pending request for this neighborhood.", "Request already exists");
                    break;

                case AccessRequestResult.AlreadyMember:
                    ShowInfoMessage("You are already a member of a neighborhood.", "Already a member");
                    break;
            }
        }

        private void ShowInfoMessage(string message, string title)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}