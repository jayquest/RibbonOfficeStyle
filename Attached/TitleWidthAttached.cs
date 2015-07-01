using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Controls.Ribbon.Primitives;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace ModernRibbon.Attached
{
    public class TitleWidthBehavior : Behavior<ContentPresenter>
    {
        private bool first = true;

        private void SizeTitle()
        {
            if (this.AssociatedObject != null && this.AssociatedObject.Content is string)
            {

                var panel = (RibbonTitlePanel) AssociatedObject.Parent;

                var elements = panel.Children.OfType<FrameworkElement>().ToList();

                if (first)
                {
                    foreach (var e in elements)
                    {
                        if (e != this.AssociatedObject)
                        {
                            e.SizeChanged -= (o, s) => SizeTitle();
                            e.IsVisibleChanged -= (o, s) => SizeTitle();

                            e.SizeChanged += (o, s) => SizeTitle();
                            e.IsVisibleChanged += (o, s) => SizeTitle();
                        }
                    }
                    first = false;
                }

                var iconMargin = 16;

                var contextualTabs = panel.Children.OfType<RibbonContextualTabGroupItemsControl>().FirstOrDefault();
                var appmenus = panel.Children.OfType<Grid>().FirstOrDefault();

                var appMenuVisible = appmenus != null && appmenus.IsVisible && appmenus.ActualWidth != 0D;
                var appmenusWidth = appmenus != null ? appmenus.ActualWidth : 0D;

                var content = (TextBlock)VisualTreeHelper.GetChild(AssociatedObject, 0);
                content.Padding = new Thickness(0);

                var titleWidth = content.ActualWidth;

                var panelLeft = ((panel.Ribbon.ActualWidth - appmenusWidth - iconMargin) / 2);
                var panelRight = panelLeft + titleWidth;

                if (contextualTabs != null && contextualTabs.IsVisible && contextualTabs.ActualWidth != 0D)
                {
                    Point ctxTabPos = contextualTabs.TransformToAncestor(panel).Transform(new Point(0, 0));


                    var contextTabsRight = ctxTabPos.X + contextualTabs.ActualWidth;

                    var startInsideCenter = panelLeft < ctxTabPos.X && ctxTabPos.X  < panelRight;
                    var endsInsideCenter = panelLeft < contextTabsRight && contextTabsRight < panelRight;
                    var containsCenter = panelLeft > ctxTabPos.X && panelRight < contextTabsRight;

                    if (startInsideCenter || endsInsideCenter || containsCenter)
                    {
                        var width = appMenuVisible ? (ctxTabPos.X - appmenus.ActualWidth - iconMargin) : (ctxTabPos.X - iconMargin);

                        AssociatedObject.Width = width;
                        content.HorizontalAlignment = HorizontalAlignment.Center;
                    }
                    else
                    {
                        var width = 0D;
                        var padding = new Thickness(0);

                        if (ctxTabPos.X < panelLeft)
                        {
                            width = panel.ActualWidth - contextTabsRight;
                            padding = new Thickness(panelLeft - contextTabsRight,0,0,0);
                            content.HorizontalAlignment = HorizontalAlignment.Left;
                        }
                        else
                        {
                            width = appMenuVisible ? (ctxTabPos.X - appmenus.ActualWidth - iconMargin) : (ctxTabPos.X - iconMargin);
                            content.HorizontalAlignment = HorizontalAlignment.Center;
                        }

                        AssociatedObject.Width = width;
                        content.Padding = padding;
                    }
                }
                else
                {
                    var width = appMenuVisible ? (panelLeft + titleWidth - iconMargin - appmenus.ActualWidth) : (panelLeft + titleWidth - iconMargin);
                    AssociatedObject.Width = width - 22;
                    content.HorizontalAlignment = HorizontalAlignment.Right;
                    
                }
            }
        }
     



        protected override void OnAttached()
        {
            var panel = ((RibbonTitlePanel)this.AssociatedObject.Parent);

            panel.SizeChanged += (o, s) => SizeTitle();


            

        }       

        protected override void OnDetaching()
        {
            var panel = ((RibbonTitlePanel)this.AssociatedObject.Parent);

            panel.SizeChanged -= (o, s) => SizeTitle();

            var elements = panel.Children.OfType<FrameworkElement>().ToList();

            foreach (var e in elements)
            {
                if (e != this.AssociatedObject)
                {
                    e.SizeChanged -= (o, s) => SizeTitle();
                    e.IsVisibleChanged -= (o, s) => SizeTitle();
                }
            }

            base.OnDetaching();
        }
    }
}
