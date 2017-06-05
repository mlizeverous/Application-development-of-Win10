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
using SQLitePCL;
using System.Text;



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
                Models.TodoItem nowPicked = ViewModel.AllItems[ViewModel.AllItems.Count - 1];

                var db = App.conn;
                using (var todoitem = db.Prepare("INSERT INTO TodoItem (Id, Title, Context, Date) VALUES (?, ?, ?, ?);"))
                {
                    todoitem.Bind(1, nowPicked.id);
                    todoitem.Bind(2, title.Text);
                    todoitem.Bind(3, details.Text);
                    todoitem.Bind(4, date.Date.ToString());
                    todoitem.Step();
                }
                
                Frame.Navigate(typeof(MainPage), ViewModel);
            }
            else
            {
                var db = App.conn;
                using (var todoitem = db.Prepare("UPDATE TodoItem SET Title = ?, Context = ?, Date = ? WHERE Id = ?"))
                {
                    todoitem.Bind(4, ViewModel.SelectedItem.id);
                    todoitem.Bind(1, title.Text);
                    todoitem.Bind(2, details.Text);
                    todoitem.Bind(3, date.Date.ToString());
                    todoitem.Step();
                }
                ViewModel.UpdateTodoItem(ViewModel.SelectedItem.id, title.Text, details.Text, date.Date, image.Source);
                Frame.Navigate(typeof(MainPage), ViewModel);
            }

        }
        private void DeleteButton_Clicked(object sender, RoutedEventArgs e)
        {
            if (ViewModel.SelectedItem != null)
            {
                var db = App.conn;
                
                using (var statement = db.Prepare("DELETE FROM TodoItem WHERE Id = ?"))
                {
                    statement.Bind(1, ViewModel.SelectedItem.id);
                    statement.Step();
                }
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

        private class Todoitem
        {
            public string Id { get; set; }
            public string Title { get; set; } 
            public string Context { get; set; }
            public string Date { get; set; }
            //To do
        }

        private async void BtnGetAll_Click(object sender, RoutedEventArgs e)
        {
            Todoitem tt = null;
            string str = "";
            var db = App.conn;
            using (var statement = db.Prepare("SELECT Id, Title, Context, Date FROM TodoItem WHERE Title Like ? or Context Like ? or Date Like ?"))
            {
                string temp = Query.Text;
                statement.Bind(1, "%" + temp + "%");
                statement.Bind(2, "%" + temp + "%");
                statement.Bind(3, "%" + temp + "%");

                while (SQLiteResult.ROW == statement.Step())
                {
                    tt = new Todoitem()
                    {
                        Id = (string)statement[0],
                        Title = (string)statement[1],
                        Context = (string)statement[2],
                        Date = (string)statement[3]
                    };
                    //str = "";
                    //str += "The query result: \n";
                    str += "Id : " + (string)statement[0] + ";  ";
                    str += "Title : " + (string)statement[1] + ";  ";
                    str += "Context : " + (string)statement[2] + ";  ";
                    str += "Date : " + (string)statement[3] + "; \n";
                }
            }
            if(tt != null)
            {
                var messageDialog = new MessageDialog(str);
                await messageDialog.ShowAsync();
            }
            else
            {
                var messageDialog = new MessageDialog("Nothing Found");
                await messageDialog.ShowAsync();
            }
        }
    }
}
