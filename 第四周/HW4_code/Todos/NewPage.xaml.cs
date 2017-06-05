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
using Windows.UI.Xaml.Media.Imaging;

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
            if (title.Text == "" && details.Text == "")
            {
                var i = new MessageDialog("标题和内容均不可为空").ShowAsync();
            }
            if (date.Date < DateTime.Now)
            {
                var i = new MessageDialog("结束时间不合法").ShowAsync();
            }
            else
            {
                Frame.Navigate(typeof(MainPage), "");
            }
            
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            details.Text = "";
            date.Date = DateTime.Now;
            Frame.Navigate(typeof(MainPage), "");
            //image.Source = new BitmapImage(new Uri(BaseUri, "Assets/background.jpg"));
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgress"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] as ApplicationDataCompositeValue;
                    title.Text = (string)composite["title"];
                    details.Text = (string)composite["details"];
                    date.Date = (DateTimeOffset)composite["date"];
                    // We're done with it, so remove it
                    ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)App.Current).IsSuspending;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["details"] = details.Text;
                composite["date"] = date.Date;
                ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;
            }
        }
    }
}
