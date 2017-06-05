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
using Todos.ViewModels;

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

            ViewModel = new EditViewModels();
            DataContext = ViewModel;
            ViewModel.LoadData(); 
        }

        EditViewModels ViewModel { get; set; }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewPage), "");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((App)App.Current).BackRequested += MainPage_BackRequested;
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            }
            else
            {
                if(ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgress"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] as ApplicationDataCompositeValue;
                    checkBox1.IsChecked = (bool?)composite["Item1"];
                    if (checkBox1.IsChecked == true)
                        line1.Visibility = Visibility.Visible;
                    else line1.Visibility = Visibility.Collapsed;
                    checkBox2.IsChecked = (bool?)composite["Item2"];
                    if (checkBox2.IsChecked == true)
                        line2.Visibility = Visibility.Visible;
                    else line2.Visibility = Visibility.Collapsed;

                    ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");


                }
            }
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            ViewModel.SaveData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((App)App.Current).BackRequested -= MainPage_BackRequested;

            bool suspending = ((App)App.Current).IsSuspending;

            var composite = new ApplicationDataCompositeValue();
            composite["Item1"] = checkBox1.IsChecked;
            composite["Item2"] = checkBox2.IsChecked;

            ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;

        }

        private void checkBox2_Checked(object sender, RoutedEventArgs e)
        {
            if (checkBox2.IsChecked == true)
                line2.Visibility = Visibility.Visible;
            else line2.Visibility = Visibility.Collapsed;
        }

        private void checkBox1_Checked(object sender, RoutedEventArgs e)
        {
            if (checkBox1.IsChecked == true)
                line1.Visibility = Visibility.Visible;
            else line1.Visibility = Visibility.Collapsed;
        }
    }
}
