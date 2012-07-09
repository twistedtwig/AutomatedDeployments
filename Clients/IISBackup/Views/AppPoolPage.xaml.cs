using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls;
using IISBackup.ViewModels;

namespace IISBackup.Views
{
    /// <summary>
    /// Interaction logic for AppPoolView.xaml
    /// </summary>
    public partial class AppPoolPage : UserControl
    {
        public AppPoolPage()
        {
            InitializeComponent();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            AppPoolViewModel appPoolViewModel = DataContext as AppPoolViewModel;
            if (appPoolViewModel == null) { return; }

            IList selectedItems = appPoolList.SelectedItems;
            if (selectedItems == null || selectedItems.Count == 0) { return; } 

            IList<string> appPoolNames = new List<string>(selectedItems.Count);

            foreach (AppPoolOverViewViewModel item in selectedItems)
            {
                appPoolNames.Add(item.Name);
            }

            appPoolViewModel.UpdateSelectedAppPoolsCommand.Execute(appPoolNames);
        }
    }
}
