// ***********************************************************************
// <copyright file="AdornerExtensions.cs"
//            project="System.Windows"
//            assembly="System.Windows"
//            solution="SevenUpdate"
//            company="Seven Software">
//     Copyright (c) Seven Software. All rights reserved.
// </copyright>
// <author username="sevenalive">Robert Baker</author>
// ***********************************************************************
namespace System.Windows.Controls
{
    using System.Windows.Documents;

    /// <summary>
    /// The adorner extensions
    /// </summary>
    public static class AdornerExtensions
    {
        #region Public Methods

        /// <summary>
        /// Determines whether the adorner layer contains an element
        /// </summary>
        /// <typeparam name="T">
        /// The type of element to check
        /// </typeparam>
        /// <param name="adr">
        /// The adorner.
        /// </param>
        /// <param name="elem">
        /// The element
        /// </param>
        /// <returns>
        /// <see langword="true"/> if the adorner layer contains the element otherwise, <see langword="false"/>.
        /// </returns>
        public static bool Contains<T>(this AdornerLayer adr, UIElement elem)
        {
            if (adr == null)
            {
                return false;
            }

            var adorners = adr.GetAdorners(elem);

            if (adorners == null)
            {
                return false;
            }

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Removes the adorners
        /// </summary>
        /// <typeparam name="T">
        /// The adorner control to remove
        /// </typeparam>
        /// <param name="adr">
        /// The adorner
        /// </param>
        /// <param name="elem">
        /// The element
        /// </param>
        /// <typeparameter name="T">
        ///   The type of element
        /// </typeparameter>
        public static void RemoveAdorners<T>(this AdornerLayer adr, UIElement elem)
        {
            var adorners = adr.GetAdorners(elem);

            if (adorners == null)
            {
                return;
            }

            for (var i = adorners.Length - 1; i >= 0; i--)
            {
                if (adorners[i] is T)
                {
                    adr.Remove(adorners[i]);
                }
            }
        }

        /// <summary>
        /// Removes all.
        /// </summary>
        /// <param name="adr">
        /// The adorner layer
        /// </param>
        /// <param name="elem">
        /// The element
        /// </param>
        public static void RemoveAll(this AdornerLayer adr, UIElement elem)
        {
            try
            {
                var adorners = adr.GetAdorners(elem);

                if (adorners == null)
                {
                    return;
                }

                foreach (var toRemove in adorners)
                {
                    adr.Remove(toRemove);
                }
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Removes all recursive.
        /// </summary>
        /// <param name="adr">
        /// The adorner layer
        /// </param>
        /// <param name="element">
        /// The element.
        /// </param>
        public static void RemoveAllRecursive(this AdornerLayer adr, UIElement element)
        {
            try
            {
                Action<UIElement> recurse = null;
                recurse = delegate(UIElement elem)
                    {
                        adr.RemoveAll(elem);
                        if (elem is Panel)
                        {
                            foreach (UIElement e in ((Panel)elem).Children)
                            {
                                recurse(e);
                            }
                        }
                        else if (elem is Decorator)
                        {
                            recurse(((Decorator)elem).Child);
                        }
                        else if (elem is ContentControl)
                        {
                            if (((ContentControl)elem).Content is UIElement)
                            {
                                recurse(((ContentControl)elem).Content as UIElement);
                            }
                        }
                    };

                recurse(element);
            }
            catch (Exception)
            {
            }
        }

        #endregion
    }
}