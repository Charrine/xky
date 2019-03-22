﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using Newtonsoft.Json.Linq;
using Xky.Core;
using Xky.Core.Model;
using Xky.Platform.UserControl;
using Xky.Platform.UserControl.Pages;
using Color = System.Windows.Media.Color;
using FontFamily = System.Windows.Media.FontFamily;

namespace Xky.Platform
{
    /// <summary>
    ///     MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var wc = new WindowChrome
            {
                CaptionHeight = 30,
                GlassFrameThickness = new Thickness(0),
                CornerRadius = new CornerRadius(0),
                UseAeroCaptionButtons = false,
                ResizeBorderThickness = new Thickness(25)
            };
            WindowChrome.SetWindowChrome(this, wc);
            Activated += MainWindow_Activated;
            Deactivated += MainWindow_Deactivated;
            SizeChanged += MainWindow_OnSizeChanged;


            try
            {
                using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
                {
                    if (Math.Abs(graphics.DpiX - 96) > 0)
                    {
                        TextOptions.SetTextFormattingMode(this, TextFormattingMode.Ideal);
                        FontFamily = new FontFamily("Microsoft Yahei");
                    }
                    else
                    {
                        TextOptions.SetTextFormattingMode(this, TextFormattingMode.Display);
                        FontFamily = new FontFamily("SimSun");
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }

            _buttonStatus.Baseurl = "Resources/Icon/ControlBox/drak/";
            DataContext = _buttonStatus;
            //初始化页面加载
            MyTabItem_OnOnClickEvent(null, "Login", true);
            Common.MainWindow = this;
        }

        #region 基础属性

        //缓存内部页面提高加载效率
        private readonly Dictionary<string, System.Windows.Controls.UserControl> _userControlDic =
            new Dictionary<string, System.Windows.Controls.UserControl>();

        //记录窗口按钮状态
        private readonly ButtonStatusModel _buttonStatus = new ButtonStatusModel();

        #endregion

        #region 界面UI事件和属性



        private void MainWindow_Deactivated(object sender, EventArgs e)
        {
            DropShadowEffect.Color = Color.FromArgb(204, 200, 200, 200);
        }

        private void MainWindow_Activated(object sender, EventArgs e)
        {
            DropShadowEffect.Color = Color.FromArgb(204, 0, 0, 0);
        }

        private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                var top = (e.NewSize.Height - SystemParameters.PrimaryScreenHeight) / 2 + SystemParameters.WorkArea.Top;
                var left = (e.NewSize.Width - SystemParameters.WorkArea.Width) / 2 + SystemParameters.WorkArea.Left;
                var right = (e.NewSize.Width - SystemParameters.WorkArea.Width) / 2 +
                            SystemParameters.PrimaryScreenWidth - SystemParameters.WorkArea.Right;
                var bottom = (e.NewSize.Height - SystemParameters.PrimaryScreenHeight) / 2 +
                             SystemParameters.PrimaryScreenHeight - SystemParameters.WorkArea.Bottom;
                MainGrid.Margin = new Thickness(left, top, right, bottom);
                BtnRestore.Visibility = Visibility.Visible;
                BtnMax.Visibility = Visibility.Collapsed;
                var wc = new WindowChrome
                {
                    CaptionHeight = 30,
                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0),
                    UseAeroCaptionButtons = false,
                    ResizeBorderThickness = new Thickness(0)
                };
                WindowChrome.SetWindowChrome(this, wc);
            }
            else
            {
                MainGrid.Margin = new Thickness(20);
                BtnRestore.Visibility = Visibility.Collapsed;
                BtnMax.Visibility = Visibility.Visible;
                var wc = new WindowChrome
                {
                    CaptionHeight = 30,
                    GlassFrameThickness = new Thickness(0),
                    CornerRadius = new CornerRadius(0),
                    UseAeroCaptionButtons = false,
                    ResizeBorderThickness = new Thickness(25)
                };
                WindowChrome.SetWindowChrome(this, wc);
            }
        }

        private void Btn_close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Btn_max(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState != WindowState.Maximized ? WindowState.Maximized : WindowState.Normal;
        }

        private void Btn_min(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        #endregion

        #region 点击事件

        private void MyTabItem_OnOnClickEvent(MyTabItem sender, string pagename, bool dark)
        {
            _buttonStatus.Baseurl = dark ? "Resources/Icon/ControlBox/dark/" : "Resources/Icon/ControlBox/default/";
            if (pagename == null)
                return;
            if (_userControlDic.ContainsKey(pagename))
            {
                MainContent.Content = _userControlDic[pagename];
            }
            else
            {
                switch (pagename)
                {
                    case "Login":
                    {
                        var page = new MyLogin();
                        _userControlDic.Add(pagename, page);

                        MainContent.Content = page;
                        break;
                    }
                    case "MainControl":
                    {
                        var page = new MyMainControl();
                        _userControlDic.Add(pagename, page);
                        MainContent.Content = page;
                        break;
                    }
                }
            }
        }

        #endregion
    }

    #region 窗体按钮状态转换模型

    public class ButtonStatus : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value + "" + parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public class ButtonStatusModel : INotifyPropertyChanged
    {
        private string _baseurl;

        public string Baseurl
        {
            get => _baseurl;
            set
            {
                _baseurl = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Baseurl"));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    #endregion
}