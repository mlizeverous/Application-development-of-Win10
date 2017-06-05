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
        private async void GetWeather(string tel)
        {
            try
            {
                // 创建一个HTTP client实例对象
                HttpClient httpClient = new HttpClient();

                // Add a user-agent header to the GET request. 
              
                var headers = httpClient.DefaultRequestHeaders;

                // The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
                // especially if the header value is coming from user input.
                string header = "ie Mozilla/5.0 (Windows NT 6.2; WOW64; rv:25.0) Gecko/20100101 Firefox/25.0";
                if (!headers.UserAgent.TryParseAdd(header))
                {
                    throw new Exception("Invalid header value: " + header);
                }
                

                string getCityCode = "http://apistore.baidu.com/microservice/cityinfo?cityname=" + cityName.Text;

                //发送GET请求
                HttpResponseMessage response = await httpClient.GetAsync(getCityCode);

                // 确保返回值为成功状态
                response.EnsureSuccessStatusCode();

                // 因为返回的字节流中含有中文，传输过程中，所以需要编码后才可以正常显示
                // “\u5e7f\u5dde”表示“广州”，\u表示Unicode
                Byte[] getByte = await response.Content.ReadAsByteArrayAsync();

                // 可以用来测试返回的结果
                //string returnContent = await response.Content.ReadAsStringAsync();

                // UTF-8是Unicode的实现方式之一。这里采用UTF-8进行编码
                Encoding code = Encoding.GetEncoding("UTF-8");
                string result = code.GetString(getByte, 0, getByte.Length);

                JsonTextReader json = new JsonTextReader(new StringReader(result));
                string jsonVal = "", cityCode = "";

                // 先获取城市对应的ID，再去第二个URL中查询
                while (json.Read())
                {
                    jsonVal += json.Value;
                    if (jsonVal.Equals("cityCode"))  // 读到“cityCode”时，取出下一个json token（即城市对应代码）
                    {
                        json.Read();
                        cityCode += json.Value;  // 该对象重载了“+=”,故可与字符串进行连接
                        break;
                    }
                    jsonVal = "";
                }

                string getWeatherCode = "http://apistore.baidu.com/microservice/weather?cityid=" + cityCode;

                //发送GET请求
                HttpResponseMessage response1 = await httpClient.GetAsync(getWeatherCode);

                response1.EnsureSuccessStatusCode();

                Byte[] getByte1 = await response1.Content.ReadAsByteArrayAsync();
                Encoding code1 = Encoding.GetEncoding("UTF-8");
                string result1 = code1.GetString(getByte1, 0, getByte1.Length);
                JsonTextReader json1 = new JsonTextReader(new StringReader(result1));
                string flag = "", tempVal = "";
                while (json1.Read())
                {
                    flag += json1.Value;
                    if (flag.Equals("weather"))
                    {
                        json1.Read();
                        tempVal += json1.Value;
                        weather.Text = tempVal;
                        tempVal = "";
                    }
                    if (flag.Equals("l_tmp"))
                    {
                        json1.Read();
                        tempVal += json1.Value;
                        temperature.Text = tempVal + "℃  到  ";
                        tempVal = "";
                    }
                    if (flag.Equals("h_tmp"))
                    {
                        json1.Read();
                        tempVal += json1.Value;
                        temperature.Text += tempVal + "℃";
                        tempVal = "";
                        break;
                    }
                    flag = "";
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
            weather.Text = "";
            temperature.Text = "";
            GetWeather(cityName.Text);
        }
    }
}
