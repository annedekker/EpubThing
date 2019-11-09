﻿using System;
using System.Windows;
using System.Windows.Controls;

namespace EpubThing
{
    public static class WebbrowserUtility
    {
        // Stolen from:
        // https://stackoverflow.com/questions/263551/databind-the-source-property-of-the-webbrowser-in-wpf

        public static readonly DependencyProperty BindableSourceProperty =
        DependencyProperty.RegisterAttached("BindableSource", typeof(string), typeof(WebbrowserUtility), new UIPropertyMetadata(null, BindableSourcePropertyChanged));

        public static string GetBindableSource(DependencyObject obj)
        {
            return (string)obj.GetValue(BindableSourceProperty);
        }

        public static void SetBindableSource(DependencyObject obj, string value)
        {
            obj.SetValue(BindableSourceProperty, value);
        }

        public static void BindableSourcePropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = o as WebBrowser;
            if (browser != null)
            {
                string uri = e.NewValue as string;
                browser.Source = !String.IsNullOrEmpty(uri) ? new Uri(uri) : null;
            }
        }
    }
}
