using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WaccaMyPageScraper.Wpf.Views.Properties
{
    public static class Filter
    {
        public static readonly DependencyProperty ByProperty = DependencyProperty.RegisterAttached("By",
            typeof(Predicate<object>),
            typeof(Filter),
            new PropertyMetadata(default(Predicate<object>), OnByChanged));

        public static void SetBy(ItemsControl element, Predicate<object> value)
        {
            element.SetValue(ByProperty, value);
        }

        public static Predicate<object> GetBy(ItemsControl element)
        {
            return (Predicate<object>)element.GetValue(ByProperty);
        }

        private static void OnByChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ItemsControl itemsControl &&
                itemsControl.Items.CanFilter)
            {
                itemsControl.Items.Filter = (Predicate<object>)e.NewValue;
            }
        }
    }
}
