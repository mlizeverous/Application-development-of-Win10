using System;
using System.Windows;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Todos.ViewModels;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

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

            ViewModel = new EditViewModels();
            DataContext = ViewModel;
            ViewModel.LoadData();

            //image.Source = new BitmapImage(new Uri("Assets/background.jpg", UriKind.Relative));
            //BitmapImage bitmapimage = new BitmapImage();
            //bitmapimage.BeginInit();
            //image.Source = new BitmapImage(new Uri(this.BaseUri,"‪C://Users/lushijuan/Pictures/Saved Pictures/scene.jpg"));
            //bitmapimage.EndInit();
        }

        EditViewModels ViewModel { get; set; }

        private void Create_Item(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), "");
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ((App)App.Current).BackRequested += NewPage_BackRequested;
            if (e.NavigationMode == NavigationMode.New)
            {
                ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
            }
            else
            {
                if (ApplicationData.Current.LocalSettings.Values.ContainsKey("TheWorkInProgress"))
                {
                    var composite = ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] as ApplicationDataCompositeValue;

                    title.Text = (string)composite["Title"];
                    details.Text = (string)composite["Details"];
                    date.Date = (DateTimeOffset)composite["Date"];
                    BitmapImage bitmapimage = new BitmapImage();
                    //bitmapimage.BeginInit();
                    //string source = (string)composite["Image"];
                    //bitmapimage.UriSource = new Uri(source, UriKind.Absolute);
                    //bitmapimage.EndInit();
                    //image.Source = bitmapimage;

                   
                    ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");


                }
            }
        }

        private void NewPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            ViewModel.SaveData();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((App)App.Current).BackRequested -= NewPage_BackRequested;

            bool suspending = ((App)App.Current).IsSuspending;

            var composite = new ApplicationDataCompositeValue();
            composite["Title"] = title.Text;
            composite["Details"] = details.Text;
            composite["Date"] = date.Date;
            /*if (imageSource != string.Empty)
            {
                composite["Image"] = imageSource;
                imageSource = string.Empty;
            }*/
           

            ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;

        }

        public string imageSource = string.Empty;

        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".gif");
            StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                using (IRandomAccessStream filestream = await
                    file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelHeight = 180;
                    await bitmapImage.SetSourceAsync(filestream);
                    image.Source = bitmapImage;
                    //imageSource = file.Path;
                    //imageSource = ((BitmapImage)(image.Source)).UriSource.LocalPath;
                }
            }


        }
    }
}
