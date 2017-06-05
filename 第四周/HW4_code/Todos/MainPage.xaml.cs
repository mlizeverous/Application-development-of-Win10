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

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Todos
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            if (checkbox1.IsChecked == false) line1.Visibility = Visibility.Collapsed;
            if (checkbox2.IsChecked == false) line2.Visibility = Visibility.Collapsed;
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewPage), "");
        }

        private void checkbox2_Checked(object sender, RoutedEventArgs e)
        {
            line2.Visibility = Visibility.Visible;
        }

        private void checkbox2_Unchecked(object sender, RoutedEventArgs e)
        {
            line2.Visibility = Visibility.Collapsed;
        }

        private void checkbox1_Checked(object sender, RoutedEventArgs e)
        {
            line1.Visibility = Visibility.Visible;
        }

        private void checkbox1_Unchecked(object sender, RoutedEventArgs e)
        {
            line1.Visibility = Visibility.Collapsed;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)Application.Current).IsSuspending;
            //((App)Application.Current).BackRequested -= Page2_BackRequested;
            if (suspending)
            {
                var composite = new ApplicationDataCompositeValue();
                composite["check1"] = checkbox1.IsChecked;
                composite["check2"] = checkbox2.IsChecked;
                ApplicationData.Current.LocalSettings.Values["mainpage"] = composite;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("mainpage");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("mainpage"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["mainpage"] as ApplicationDataCompositeValue;
                    checkbox1.IsChecked = (bool)composite["check1"];
                    checkbox2.IsChecked = (bool)composite["check2"];
                    // We're done with it, so remove it
                    ApplicationData.Current.LocalSettings.Values.Remove("mainpage");
                }
            }
        }
    }
}
