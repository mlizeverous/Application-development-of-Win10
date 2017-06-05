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
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;

namespace Todos
{
    public sealed partial class NewPage : Page
    {
        //private bool suspending;

        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
        }

        private void Create_Item(object sender, RoutedEventArgs e)
        {
            if (title.Text == "")
            {
                var i1 = new MessageDialog("Title cannot be empty!").ShowAsync();
            }
            if (detail.Text == "")
            {
                var i2 = new MessageDialog("Details cannot be empty!").ShowAsync();
            }
            if (date.Date < DateTime.Today)
            {
                var i3 = new MessageDialog("Date is invalid!").ShowAsync();
            }
            else
            {
                Frame.Navigate(typeof(MainPage), "");
            }
        }

        private void Cancel_Item(object sender, RoutedEventArgs e)
        {
            title.Text = "";
            detail.Text = "";
            date.Date = DateTime.Today;
        }

        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            ///创建一个WriteableBitmap对象，它的作用是提供了一个可写入并更新的BitmapSource
            WriteableBitmap writeAbleBitmap = new WriteableBitmap(172, 129);
            ///创建一个FileOpenPicker对象
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".png");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".bmp");
            ///file被创建来保存我们选择的图片
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                ///以只读的方式打开选定的文件
                IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.Read);
                ///通过访问流来设置源图像
                await writeAbleBitmap.SetSourceAsync(stream);
                image.Source = writeAbleBitmap;
            }
        }
        //TheViewModel ViewModel { get; set; }

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
                    detail.Text = (string)composite["detail"];
                    date.Date = (DateTimeOffset)composite["date"];
                    // We're done with it, so remove it
                    ApplicationData.Current.LocalSettings.Values.Remove("TheWorkInProgress");
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            bool suspending = ((App)Application.Current).issuspending;
            //((App)Application.Current).BackRequested -= Page2_BackRequested;
            if (suspending)
            {
                // Save volatile state in case we get terminated later on, then
                // we can restore as if we'd never been gone :)
                var composite = new ApplicationDataCompositeValue();
                composite["title"] = title.Text;
                composite["detail"] = detail.Text;
                composite["date"] = date.Date;
                ApplicationData.Current.LocalSettings.Values["TheWorkInProgress"] = composite;
            }
        }
    }
}
