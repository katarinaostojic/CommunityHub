using System;
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

            // 🔥 pokreće flow
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
            await Task.Delay(3000); // 3 sekunde loading

            var service = new NeighborhoodAccessRequestService();

            bool granted = service.RequestAccess(_user, _neighborhood);

            this.Close();

            if (granted)
            {
                var successDialog = new NeighborhoodAccessGrantedDialog(_user, _neighborhood);
                successDialog.ShowDialog();
            }
            else
            {
                var requestDialog = new NeighborhoodRequestCreatedDialog(_user, _neighborhood);
                requestDialog.ShowDialog();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}