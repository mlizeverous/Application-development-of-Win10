using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;



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
            this.ViewModel = new ViewModels.TodoItemViewModel();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            //dataTransferManager.DataRequested += OnShareDataRequested;
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            ViewModel.AddTodoItem("test", "testing", DateTime.Now, new BitmapImage(new Uri(BaseUri, "Assets/background.jpg")));
            ViewModel.AddTodoItem("完成作业", "作业好多啊啊啊啊", DateTime.Now, new BitmapImage(new Uri(BaseUri, "Assets/background.jpg")));
            image.Source = new BitmapImage(new Uri(BaseUri, "Assets/background.jpg"));
        }


        ViewModels.TodoItemViewModel ViewModel { get; set; }
        //ImageSource image = new BitmapImage(new Uri(BaseUri, "Assets/background.jpg"));

        //Models.TodoItem nowPicked = ViewModel.AllItems[ViewModel.AllItems.Count - 1];


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter.GetType() == typeof(ViewModels.TodoItemViewModel))
            {
                this.ViewModel = (ViewModels.TodoItemViewModel)(e.Parameter);
            }
        }

        private void TodoItem_ItemClicked(object sender, ItemClickEventArgs e)
        {
            ViewModel.SelectedItem = (Models.TodoItem)(e.ClickedItem);
            if(this.ActualWidth <= 800)
            {
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
            else
            {
                createButton.Content = "Update";
                title.Text = ViewModel.SelectedItem.title;
                details.Text = ViewModel.SelectedItem.description;
                date.Date = ViewModel.SelectedItem.date;
                image.Source = ViewModel.SelectedItem.image;
            }
            
        }

        private void AddAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.ActualWidth <= 800)
            {
                ViewModel.SelectedItem = null;
                Frame.Navigate(typeof(NewPage), ViewModel);
            }
           else
            {
                return;
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
            else if (ViewModel.SelectedItem == null)
            {
                ViewModel.AddTodoItem(title.Text, details.Text, date.Date, image.Source);
                Frame.Navigate(typeof(MainPage), ViewModel);
                //Frame.Navigate(typeof(MainPage), ViewModel);
            }
            else
            {
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, details.Text, date.Date, image.Source);
                //Frame.Navigate(typeof(MainPage), ViewModel);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }

        }
        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                ViewModel.RemoveTodoItem(ViewModel.SelectedItem.id);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = null;
            Frame.Navigate(typeof(MainPage), ViewModel);
        }

        private void OnClick(object sender, RoutedEventArgs e)
        {
            Windows.Data.Xml.Dom.XmlDocument tileXml = new Windows.Data.Xml.Dom.XmlDocument();
            tileXml.LoadXml(File.ReadAllText("tiles.xml"));
            Windows.Data.Xml.Dom.XmlNodeList str = tileXml.GetElementsByTagName("text");
            for (int i = 0; i < str.Count; i++)
            {
                ((Windows.Data.Xml.Dom.XmlElement)str[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count - 1].title;
                i++;
                ((Windows.Data.Xml.Dom.XmlElement)str[i]).InnerText = ViewModel.AllItems[ViewModel.AllItems.Count-1].description;
            }

            TileNotification notifi = new TileNotification(tileXml);
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.Update(notifi);
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            var defl = args.Request.GetDeferral();

            DataPackage dp = new DataPackage();
            dp.Properties.Title = ViewModel.SelectedItem.title;
            dp.Properties.Description = ViewModel.SelectedItem.description;
            //dp.Properties.
            dp.SetText(ViewModel.SelectedItem.description);
            args.Request.Data = dp;
            defl.Complete();
        }
        //async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        //{
        //    var dp = args.Request.Data;
        //    var deferral = args.Request.GetDeferral();

        //    dp.Properties.Title = nowPicked.title;
        //    dp.Properties.Description = nowPicked.description;
        //}

        private void share_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.SelectedItem = ((MenuFlyoutItem)sender).DataContext as Models.TodoItem;
            DataTransferManager.ShowShareUI();
        }
    }
}
