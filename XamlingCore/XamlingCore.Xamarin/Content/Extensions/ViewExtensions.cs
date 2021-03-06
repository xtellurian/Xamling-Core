﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamlingCore.XamarinThings.Content.Extensions
{
    public static class ViewExtensions
    {
        public static ContentView Wrap(this View item,
            Thickness padding = default (Thickness),
            LayoutOptions horizontalLayoutOptions = default(LayoutOptions),
            LayoutOptions verticalLayoutOptions = default(LayoutOptions))
        {
            if(padding == default (Thickness))
            {
                padding = new Thickness(10, 10, 10, 10);
            }


            var f = new ContentView
            {
                Padding = padding,
                Content = item,
                HorizontalOptions = horizontalLayoutOptions,
                VerticalOptions = verticalLayoutOptions
            };

            return f;
        }
    }
}
