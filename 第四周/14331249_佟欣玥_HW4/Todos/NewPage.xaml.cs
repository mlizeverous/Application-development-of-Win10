using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;


namespace Todos
{
    public sealed partial class NewPage : Page
    {
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }

        private void Create_Item(object sender, RoutedEventArgs e)
        {
            if (title.Text.Trim() == String.Empty)
            {
                var i = new MessageDialog("The title is empty!").ShowAsync();
            }
            if (detail.Text.Trim() == String.Empty)
            {
                var i = new MessageDialog("The detail is empty!").ShowAsync();
            }
            if (datepicker.Date < DateTime.Today) 
            {
                var i = new MessageDialog("Please input a right time!").ShowAsync();
            }
            if (detail.Text.Trim() != String.Empty && title.Text.Trim() != String.Empty && datepicker.Date >= DateTime.Today) {
                Frame.Navigate(typeof(MainPage), "");
            }
        }
        private void Cancle_Item(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            detail.Text = "";
            datepicker.Date = DateTime.Now;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgress"))
            {
                var composite = ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] as ApplicationDataCompositeValue;
                title.Text = (string)composite["title"];
                detail.Text = (string)composite["details"];

                // We're done with it, so remove it
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).IsSuspending;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["details"] = detail.Text;
                ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;
            }
        }

    }
}
