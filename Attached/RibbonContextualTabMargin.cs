using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    public class RibbonContextualTabMargin : Behavior<RibbonTabHeader>
    {

        private void ConfigureMargin()
        {
            if (this.AssociatedObject != null)
            {
                var header = (RibbonTabHeader) AssociatedObject;

                if (header.IsContextualTab)
                {

                    var tab = GetRibbonTab(header);
                    var tabs = GetTabs(header).OfType<RibbonTab>().ToList();

                    if (tabs.IndexOf(tab) == 0)
                    {
                        header.Margin = new Thickness(2, 0, 0, 0);
                    }
                    //else if (tabs.IndexOf(tab) == tabs.Count() - 1)
                    //{
                    //    header.Margin = new Thickness(0, 0, 2, 0);
                    //}
                }
               
            }
        }

        private RibbonTab GetRibbonTab(RibbonTabHeader header)
        {
            ItemsControl tabHeaderItemsControl = ItemsControl.ItemsControlFromItemContainer(header);
            Ribbon ribbon = header.Ribbon;
            if (tabHeaderItemsControl != null && ribbon != null)
            {
                int index = tabHeaderItemsControl.ItemContainerGenerator.IndexFromContainer(header);
                return ribbon.ItemContainerGenerator.ContainerFromIndex(index) as RibbonTab;
            }
            return null;
        }

        private IEnumerable<RibbonTab> GetTabs(RibbonTabHeader header)
        {
            Ribbon ribbon = header.Ribbon;

            var headerTab = GetRibbonTab(header);
            var contextualTabHeader = headerTab.ContextualTabGroup;

            if (ribbon != null)
            {
                int itemCount = ribbon.Items.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    RibbonTab tab = ribbon.ItemContainerGenerator.ContainerFromIndex(i) as RibbonTab;
                    if (tab != null && GetTabHeader(tab) != null && GetTabHeader(tab).IsContextualTab && object.Equals(contextualTabHeader.Header, tab.ContextualTabGroupHeader))
                    {
                        yield return tab;
                    }
                }
            }
        }

        private RibbonTabHeader GetTabHeader(RibbonTab tab)
        {
            Ribbon ribbon = tab.Ribbon;
            if (ribbon != null)
            {
                int index = ribbon.ItemContainerGenerator.IndexFromContainer(tab);
                if (index >= 0)
                {

                    var grid = VisualTreeHelper.GetChild(ribbon,0);

                    RibbonTabHeaderItemsControl headerItemsControl = null;

                    for (var i = 0; i < VisualTreeHelper.GetChildrenCount(grid) && headerItemsControl ==  null; i++)
                    {
                        headerItemsControl = VisualTreeHelper.GetChild(grid, i) as RibbonTabHeaderItemsControl;                        
                    }
                    
                    if (headerItemsControl != null)
                    {
                        return headerItemsControl.ItemContainerGenerator.ContainerFromIndex(index) as RibbonTabHeader;
                    }
                }
            }
            return null;
        }


        protected override void OnChanged()
        {
            ConfigureMargin();
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            ConfigureMargin();
        }

        protected override void OnAttached()
        {
            ConfigureMargin();
        }       
    }

    public static class ConfigContextualTabBehaviorExtension
    {
        public static bool GetConfigContextualTab(DependencyObject obj)
        {
            return (bool)obj.GetValue(ConfigContextualTabProperty);
        }
        public static void SetConfigContextualTab(DependencyObject obj, bool value)
        {
            obj.SetValue(ConfigContextualTabProperty, value);
        }
        public static readonly DependencyProperty ConfigContextualTabProperty =
            DependencyProperty.RegisterAttached("ConfigContextualTab", typeof(bool), typeof(ConfigContextualTabBehaviorExtension), new PropertyMetadata(false, OnConfigContextualTabChanged));

        private static void OnConfigContextualTabChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var behaviors = Interaction.GetBehaviors(sender);

            // Remove the existing behavior instances
            foreach (var old in behaviors.OfType<RibbonContextualTabMargin>().ToArray())
                behaviors.Remove(old);

            if (args.NewValue != null && ((bool)args.NewValue) == true)
            {
                // Creates a new behavior and attaches to the target
                var behavior = new RibbonContextualTabMargin();

                // Apply the behavior
                behaviors.Add(behavior);
            }
        }
    }
}
