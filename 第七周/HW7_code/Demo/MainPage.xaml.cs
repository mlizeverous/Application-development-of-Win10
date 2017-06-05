using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Xml;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Demo
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void Getcity(string tel)
        {
            try
            {
                HttpClient httpClient = new HttpClient();  // 创建一个HTTP client实例对象
                string getCityCode = "http://apis.baidu.com/apistore/mobilephoneservice/mobilephone?tel=" + number.Text;
                System.Net.HttpWebRequest request;
                request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(getCityCode);
                request.Method = "GET";

                request.Headers["apikey"] = "730cc78feaef785c56d40c63df801206";

                //发出请求并接收响应数据
                var  response =  await request.GetResponseAsync();
               
                Stream s;
                s = response.GetResponseStream(); //把数据流转换为字符串
                string StrDate = "";
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8); //解码
                while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate + "\r\n";
                }
                JsonTextReader json = new JsonTextReader(new StringReader(strValue));

                // 解析json
                while (json.Read())
                {
                    StrDate += json.Value;

                    if (StrDate.Equals("province"))
                    {
                        json.Read();                      
                        province.Text = json.Value.ToString();
                        strValue = "";
                    }
                    if (StrDate.Equals("carrier"))
                    {
                        json.Read();
                        carrier.Text = json.Value.ToString();
                        strValue = "";
                    }
                    StrDate = "";
                }
            }
            catch (HttpRequestException ex1)
            {
                infor.Text = ex1.ToString();
            }
            catch (Exception ex2)
            {
                infor.Text = ex2.ToString();
            }

            }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            province.Text = "";
            carrier.Text = "";
            Getcity(number.Text);
        }
    }
}
