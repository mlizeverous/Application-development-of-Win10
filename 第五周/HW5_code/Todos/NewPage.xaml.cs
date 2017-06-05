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
        public NewPage()
        {
            this.InitializeComponent();
            var viewTitleBar = Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar;
            viewTitleBar.BackgroundColor = Windows.UI.Colors.CornflowerBlue;
            viewTitleBar.ButtonBackgroundColor = Windows.UI.Colors.CornflowerBlue;
            image.Source = new BitmapImage(new Uri(BaseUri, "Assets/background.jpg"));

        }

        private ViewModels.TodoItemViewModel ViewModel;



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = ((ViewModels.TodoItemViewModel)e.Parameter);
            if (ViewModel.SelectedItem == null)
            {
                createButton.Content = "Create";
                title.Text = "";
                details.Text = "";
                image.Source = new BitmapImage(new Uri(BaseUri, "Assets/background.jpg"));
                //var i = new MessageDialog("Welcome!").ShowAsync();
            }
            else
            {
                createButton.Content = "Update";
                title.Text = ViewModel.SelectedItem.title;
                details.Text = ViewModel.SelectedItem.description;
                date.Date = ViewModel.SelectedItem.date;
                image.Source = ViewModel.SelectedItem.image;

                // ...
            }
        }
        private void CreateButton_Clicked(object sender, RoutedEventArgs e)
        {
            // check the textbox and datapicker
            // if ok
            if (title.Text == "" && details.Text == "")
            {
                var i = new MessageDialog("标题和内容均不可为空").ShowAsync();
            }
            else if (date.Date < DateTime.Now)
            {
                var i = new MessageDialog("结束时间不合法").ShowAsync();
            }
            else if(ViewModel.SelectedItem == null)
            {
                ViewModel.AddTodoItem(title.Text, details.Text, date.Date, image.Source);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
            else
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, details.Text, date.Date, image.Source);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
            
        }
        private  void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }


        /*private void UpdateButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                if (title.Text == "" && details.Text == "")
                {
                    var i = new MessageDialog("标题和内容均不可为空").ShowAsync();
                }
                else if (date.Date < DateTime.Now)
                {
                    var i = new MessageDialog("结束时间不合法").ShowAsync();
                }
                else
                {
                    ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, details.Text, date.Date);
                    Frame.Navigate(typeof(MainPage), ViewModel);
                }
                // check then update
            }
        }*/

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(MainPage), ViewModel);
        }

        
        private async void SelectPictureButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;  //设置文件的现实方式，这里选择的是图标
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary; //设置打开时的默认路径，这里选择的是图片库
            picker.FileTypeFilter.Add(".jpg");                 //添加可选择的文件类型，这个必须要设置
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            StorageFile file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    // Set the image source to the selected bitmap 
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.DecodePixelWidth = 180; //match the target Image.Width, not shown
                    await bitmapImage.SetSourceAsync(fileStream);
                    image.Source = bitmapImage;
                }

            }
            else
            {
                return;
            }
        }
    }
}
