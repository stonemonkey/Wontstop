// Copyright (c) Costin Morariu. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace MvvmToolkit.WinRT.Utils
{
    /// <summary>
    /// Helper methods for searching in the visual tree of a DependencyObject.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Recursively search for the first child object with name starting from a parent root 
        /// object.
        /// </summary>
        /// <param name="root">The parent in the visual three from where the search propagates down.
        /// </param>
        /// <param name="name">The name of the object.</param>
        /// <returns>The object found or null in case the visual tree doesn't contain any object 
        /// with specified name.</returns>
        public static FrameworkElement FindChild(this DependencyObject root, string name)
        {
            FrameworkElement child = null;
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                child = VisualTreeHelper.GetChild(root, i) as FrameworkElement;
                if ((child != null) && string.Equals(name, child.Name, StringComparison.Ordinal))
                {
                    break;
                }

                child = child.FindChild(name);
            }

            return child;
        }

        /// <summary>
        /// Recursively search for all children objects of specified type starting from a parent 
        /// root object.
        /// </summary>
        /// <typeparam name="T">The type of the children to find.</typeparam>
        /// <param name="root">The parent in the visual three from where the search propagates down.
        /// </param>
        /// <returns>A list containing all children objects with specified type. In case none are
        /// found, an empty list.</returns>
        public static IList<T> FindChildrenOf<T>(this DependencyObject root)
            where T : DependencyObject
        {
            var children = new List<T>();

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(root); i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                if (child is T)
                {
                    children.Add(child as T);
                }
                else
                {
                    children.AddRange(child.FindChildrenOf<T>());
                }
            }

            return children;
        }

        /// <summary>
        /// Recursively search for the first parent object of specified type starting from a child 
        /// leaf object.
        /// </summary>
        /// <typeparam name="T">The type of the parent to find.</typeparam>
        /// <param name="child">The child in the visual three from where the search propagates up.
        /// </param>
        /// <returns>The object found or null in case the visual tree doesn't contain any object 
        /// with specified type.</returns>
        public static T FindParentOf<T>(this DependencyObject child)
            where T : DependencyObject
        {
            var parentObject = VisualTreeHelper.GetParent(child);
            if (parentObject == null)
            {
                return null;
            }

            T parent = parentObject as T;
            if (parent != null)
            {
                return parent;
            }

            return parentObject.FindParentOf<T>();
        }
    }
}
