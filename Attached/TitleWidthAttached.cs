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

namespace ModernRibbon.Attached
{
    public class TitleWidthBehavior : Behavior<ContentPresenter>
    {
        private void SizeTitle()
        {
            if (this.AssociatedObject != null && this.AssociatedObject.Content is string)
            {
                var panel = (RibbonTitlePanel) AssociatedObject.Parent;


                var iconMargin = 20;

                var contextualTabs = panel.Children.OfType<RibbonContextualTabGroupItemsControl>().FirstOrDefault();
                var appmenus = panel.Children.OfType<Grid>().FirstOrDefault();                

                if (appmenus != null && contextualTabs != null)
                {
                    Point contextPoint = contextualTabs.TransformToAncestor(panel).Transform(new Point(0, 0));

                    var left = contextPoint.X - appmenus.ActualWidth - iconMargin;
                    var right = panel.ActualWidth - contextPoint.X - contextualTabs.ActualWidth;

                    AssociatedObject.Width = left > right ? left : right;

                    return;
                }

                if (appmenus != null || contextualTabs != null)
                {
                    if (appmenus != null)
                    {
                        AssociatedObject.Width = panel.ActualWidth - appmenus.ActualWidth - iconMargin;
                    }
                    else
                    {
                        Point point = contextualTabs.TransformToAncestor(panel).Transform(new Point(0, 0));

                        var left = point.X - iconMargin;
                        var right = panel.ActualWidth - point.X - contextualTabs.ActualWidth;

                        AssociatedObject.Width = left > right ? left : right;
                    }                 
                }

                


            }
        }

        protected override void OnAttached()
        {
            ((RibbonTitlePanel)this.AssociatedObject.Parent).SizeChanged += (o, s) => SizeTitle();
        }       

        protected override void OnDetaching()
        {
            ((RibbonTitlePanel)this.AssociatedObject.Parent).SizeChanged -= (o, s) => SizeTitle(); base.OnDetaching();
        }
    }
}
