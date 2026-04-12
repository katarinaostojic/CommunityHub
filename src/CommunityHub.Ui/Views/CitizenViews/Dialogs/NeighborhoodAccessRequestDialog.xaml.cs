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

        public NeighborhoodAccessRequestDialog(User user, Neighborhood neighborhood)
        {
            InitializeComponent();

            _user = user;
            _neighborhood = neighborhood;

            DataContext = _neighborhood;

            LoadData();

            CloseButton.Click += CloseButton_Click;

            StartRequestFlow();
        }

        private void LoadData()
        {
            NeighborhoodNameText.Text = _neighborhood.Name;
            DescriptionText.Text = _neighborhood.Description;

            if (_neighborhood.Streets != null && _neighborhood.Streets.Any())
            {
                AddressText.Text = "Address: " + string.Join(", ",
                    _neighborhood.Streets.Select(s =>
                        $"{s.StreetName} {s.StartNumber} - {s.EndNumber}"));
            }
            else
            {
                AddressText.Text = "Address: No street information available";
            }

            LocationText.Text = $"Location: {_neighborhood.Location.CityName}, {_neighborhood.Location.CountryName}";
        }

        private async void StartRequestFlow()
        {
            await Task.Delay(3000);

            var service = new NeighborhoodAccessRequestService();
            AccessRequestResult result = service.RequestAccess(_user, _neighborhood);

            Close();

            switch (result)
            {
                case AccessRequestResult.Granted:
                    new NeighborhoodAccessGrantedDialog(_user, _neighborhood).ShowDialog();
                    break;

                case AccessRequestResult.RequestCreated:
                    new NeighborhoodRequestCreatedDialog(_user, _neighborhood).ShowDialog();
                    break;

                case AccessRequestResult.AlreadyPending:
                    MessageBox.Show(
                        "You already have a pending request for this neighborhood.",
                        "Request already exists",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                case AccessRequestResult.AlreadyMember:
                    MessageBox.Show(
                        "You are already a member of a neighborhood.",
                        "Already a member",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}