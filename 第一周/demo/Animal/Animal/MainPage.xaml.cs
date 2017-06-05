using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace Animal
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private delegate string AnimalSaying(object sender, myEventArgs e);//声明一个委托
        private event AnimalSaying Say;//委托声明一个事件
        public string name = "";

        public MainPage()
        {
            this.InitializeComponent();
        }

        interface Animal
        {
            //方法
            string saying(object sender, myEventArgs e);
            //属性
            int A { get; set; }
        }

        class cat : Animal
        {
            TextBlock word;
            private int a;

            public cat(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                if(e.name == "cat") this.word.Text += "cat: I'm a cat." + "\n";
                else this.word.Text += "I'm a cat." + "\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class dog : Animal
        {
            TextBlock word;
            private int a;

            public dog(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                if (e.name == "dog") this.word.Text += "dog: I'm a dog." + "\n";
                else this.word.Text += "I'm a dog." + "\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        class pig : Animal
        {
            TextBlock word;
            private int a;

            public pig(TextBlock w)
            {
                this.word = w;
            }
            public string saying(object sender, myEventArgs e)
            {
                if (e.name == "pig") this.word.Text += "pig: I'm a pig." + "\n";
                else this.word.Text += "I'm a pig." + "\n";
                return "";
            }
            public int A
            {
                get { return a; }
                set { this.a = value; }
            }
        }

        private cat c;
        private dog d;
        private pig p;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
                c = new cat(words);
                d = new dog(words);
                p = new pig(words);
                //注册事件
            Random ran = new Random();
            int n = ran.Next(0, 3);
            if(n == 0)
            {
                Say += new AnimalSaying(c.saying);
                Say(this, new myEventArgs(name));
                Say -= new AnimalSaying(c.saying);
            }
            if(n == 1)
            {
                Say += new AnimalSaying(d.saying);
                Say(this, new myEventArgs(name));
                Say -= new AnimalSaying(d.saying);
            }
            if(n == 2)
            {
                Say += new AnimalSaying(p.saying);
                Say(this, new myEventArgs(name));
                Say -= new AnimalSaying(p.saying);
            }

            //执行事件
            //Say(this, new myEventArgs(times++));  //事件中传递参数times
        }

        //自定义一个Eventargs传递事件参数
        class myEventArgs : EventArgs
        {
            public string name = "";
            public myEventArgs(string _name)
            {
                this.name = _name;
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string nn = textBox.Text;
            name = nn;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            c = new cat(words);
            d = new dog(words);
            p = new pig(words);
            if (name == "cat")
            {
                Say += new AnimalSaying(c.saying);
                Say(this, new myEventArgs(name));
                Say -= new AnimalSaying(c.saying);
                name = "";
                textBox.Text = "";
            }
            if (name == "dog")
            {
                Say += new AnimalSaying(d.saying);
                Say(this, new myEventArgs(name));
                Say -= new AnimalSaying(d.saying);
                name = "";
                textBox.Text = "";
            }
            if (name == "pig")
            {
                Say += new AnimalSaying(p.saying);
                Say(this, new myEventArgs(name));
                Say -= new AnimalSaying(p.saying);
                name = "";
                textBox.Text = "";
            }
            else textBox.Text = "";
        }
    }
}