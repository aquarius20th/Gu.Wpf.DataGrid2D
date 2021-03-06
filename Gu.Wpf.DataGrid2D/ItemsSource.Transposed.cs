﻿namespace Gu.Wpf.DataGrid2D
{
    using System;
    using System.Collections;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public static partial class ItemsSource
    {
        public static readonly DependencyProperty TransposedSourceProperty = DependencyProperty.RegisterAttached(
            "TransposedSource",
            typeof(IEnumerable),
            typeof(ItemsSource),
            new PropertyMetadata(
                default(IEnumerable),
                OnTransposedSourceChanged,
                CoerceTransposedSource));

        public static readonly DependencyProperty PropertySourceProperty = DependencyProperty.RegisterAttached(
            "PropertySource",
            typeof(object),
            typeof(ItemsSource),
            new PropertyMetadata(
                default(object),
                OnPropertySourceChanged));

        /// <summary>
        /// Helper for setting TransposedSource property on a DataGrid.
        /// </summary>
        /// <param name="element">DataGrid to set TransposedSource property on.</param>
        /// <param name="value">TransposedSource property value.</param>
        public static void SetTransposedSource(this DataGrid element, IEnumerable value)
        {
            element.SetValue(TransposedSourceProperty, value);
        }

        /// <summary>
        /// Helper for reading TransposedSource property from a DataGrid.
        /// </summary>
        /// <param name="element">DataGrid to read TransposedSource property from.</param>
        /// <returns>TransposedSource property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static IEnumerable GetTransposedSource(this DataGrid element)
        {
            return (IEnumerable)element.GetValue(TransposedSourceProperty);
        }

        /// <summary>
        /// Helper for setting PropertySource property on a DependencyObject.
        /// </summary>
        /// <param name="element">DependencyObject to set PropertySource property on.</param>
        /// <param name="value">PropertySource property value.</param>
        public static void SetPropertySource(DependencyObject element, object value)
        {
            element.SetValue(PropertySourceProperty, value);
        }

        /// <summary>
        /// Helper for reading PropertySource property from a DependencyObject.
        /// </summary>
        /// <param name="element">DependencyObject to read PropertySource property from.</param>
        /// <returns>PropertySource property value.</returns>
        [AttachedPropertyBrowsableForChildren(IncludeDescendants = false)]
        [AttachedPropertyBrowsableForType(typeof(DataGrid))]
        public static object GetPropertySource(DependencyObject element)
        {
            return element.GetValue(PropertySourceProperty);
        }

        private static void OnTransposedSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var dataGrid = (DataGrid)d;
            var source = (IEnumerable)e.NewValue;
            if (source == null)
            {
                BindingOperations.ClearBinding(dataGrid, ItemsControl.ItemsSourceProperty);
                BindingOperations.ClearBinding(dataGrid, ItemsSourceProxyProperty);
                return;
            }

            dataGrid.Bind(ItemsSourceProxyProperty)
                    .OneWayTo(dataGrid, ItemsControl.ItemsSourceProperty);
            UpdateItemsSource(dataGrid);
        }

        private static object CoerceTransposedSource(DependencyObject dependencyObject, object baseValue)
        {
            if (baseValue == null)
            {
                return null;
            }

            if (baseValue is IEnumerable)
            {
                return baseValue;
            }

            return CreateSingletonEnumerable(baseValue);
        }

        private static void OnPropertySourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
            {
                d.SetCurrentValue(TransposedSourceProperty, null);
            }
            else
            {
                var source = CreateSingletonEnumerable(e.NewValue);
                d.SetCurrentValue(TransposedSourceProperty, source);
            }
        }

        private static IEnumerable CreateSingletonEnumerable(object item)
        {
            var array = Array.CreateInstance(item.GetType(), 1);
            array.SetValue(item, 0);
            return array;
        }
    }
}
