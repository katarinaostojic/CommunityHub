using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CommunityHub.Ui.Views.CitizenViews.Dialogs;

public partial class AddItemDialog : Window
{
    public string ItemName { get; private set; } = string.Empty;

    public AddItemDialog()
    {
        InitializeComponent();
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(ItemNameTextBox.Text))
        {
            MessageBox.Show("Please enter item name.", "Validation",
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        ItemName = ItemNameTextBox.Text.Trim();
        DialogResult = true;
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}