﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GeekyTool.Core.Extensions
{
    public static class XAMLExtensions
    {
        /// <summary>
        ///     Render a UIElement into a bitmap IRandomAccessStream
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the IRandomAccessStream</returns>
        public static async Task<IRandomAccessStream> RenderToRandomAccessStream(this UIElement element)
        {
            if (element == null) throw new NullReferenceException();

            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(element);

            var pixelBuffer = await rtb.GetPixelsAsync();
            var pixels = pixelBuffer.ToArray();

            var displayInformation = DisplayInformation.GetForCurrentView();

            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                (uint)rtb.PixelWidth,
                (uint)rtb.PixelHeight,
                displayInformation.RawDpiX,
                displayInformation.RawDpiY,
                pixels);

            await encoder.FlushAsync();
            stream.Seek(0);

            return stream;
        }

        /// <summary>
        /// Applys a blur to a UI element
        /// </summary>
        /// <param name="sourceElement">UIElement to blur, generally an Image control, but can be anything</param>
        /// <param name="blurAmount">Level of blur to apply</param>
        /// <returns>Blurred UIElement as BitmapImage</returns>
        //public static async Task<BitmapImage> BlurElementAsync(this UIElement sourceElement, float blurAmount = 2.0f)
        //{
        //    var stream = await RenderToRandomAccessStream(sourceElement);

        //    var canvasDecive = new CanvasDevice();
        //    var bitmap = await CanvasBitmap.LoadAsync(canvasDecive, stream);

        //    var renderer = new CanvasRenderTarget(canvasDecive,
        //        bitmap.SizeInPixels.Width,
        //        bitmap.SizeInPixels.Height,
        //        bitmap.Dpi);

        //    using (var ds = renderer.CreateDrawingSession())
        //    {
        //        var blur = new GaussianBlurEffect
        //        {
        //            BlurAmount = blurAmount,
        //            Source = bitmap
        //        };
        //        ds.DrawImage(blur);
        //    }

        //    stream.Seek(0);
        //    await renderer.SaveAsync(stream, CanvasBitmapFileFormat.Png);

        //    var image = new BitmapImage();
        //    await image.SetSourceAsync(stream);

        //    return image;
        //}

        /// <summary>
        ///     Render a UIElement into a bitmap
        /// </summary>
        /// <param name="element">The element to render</param>
        /// <returns>An awaitable task that returns the BitmapImage</returns>
        public static async Task<BitmapImage> RenderToBitmapImage(this UIElement element)
        {
            using (var stream = await element.RenderToRandomAccessStream())
            {
                var image = new BitmapImage();
                image.SetSource(stream);
                return image;
            }
        }

        /// <summary>
        ///     Traverses the Visual Tree and returns a list of elements of type T
        /// </summary>
        /// <typeparam name="T">The type of elements to find</typeparam>
        /// <param name="parent">The root of the Visual Tree</param>
        /// <returns>A list of elements of type T, or null</returns>
        public static IEnumerable<T> FindChildren<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            return parent._FindChildren<T>();
        }

        /// <summary>
        ///     A helper function for FindChildren
        ///     Traverses the Visual Tree and returns a list of elements of type T
        /// </summary>
        /// <typeparam name="T">The type of elements to find</typeparam>
        /// <param name="parent">The root of the Visual Tree</param>
        /// <param name="list">a list to be used in the recusive calls</param>
        /// <returns>A list of elements of type T, or null</returns>
        private static IEnumerable<T> _FindChildren<T>(this DependencyObject parent, List<T> list = null)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid. 
            if (parent == null) return null;

            if (list == null) list = new List<T>();

            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child

                var childType = child as T;
                if (childType == null)
                {
                    _FindChildren(child, list);
                }
                else
                {
                    list.Add(childType);
                }
            }

            return list;
        }
    }
}
