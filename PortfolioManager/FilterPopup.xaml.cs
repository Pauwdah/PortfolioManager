using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace PortfolioManager
{

    public partial class FilterPopup : Window
    {

        private static short newElementGap = 10;
        private static short defaultElementGap = 3;
        private static short marginLeft = 20;
        private static short textBoxStartPosY = 20;
        private static short elementHeight = 20;
        private static short elementWidth = 100;

        public FilterPopup()
        {
            InitializeComponent();
            CreateNewFilterUIComponent();

            this.Closing += (sender, e) =>
            {
                SaveFilter();
            };
        }

        public void CreateNewFilterUIComponent()
        {

            for (short i = 0; i < SharedData.FilterList.Count; i++)
            {
                CreateFilterTextBox(elementWidth, elementHeight, [marginLeft, newElementGap, 0, 0], SharedData.FilterList[i]);
                CreateRemoveFilterButton(elementWidth, elementHeight, [marginLeft, defaultElementGap, 0, 0], "Remove Filter");

                if (i == SharedData.FilterList.Count - 1)
                {
                    CreateNewFilterButton(elementWidth, elementHeight, [marginLeft, newElementGap, 0, 0], "Add Filter");
                }
            }
        }

        public void CreateFilterTextBox(short width, short height, short[] margin, string content)
        {


            TextBox textBox = new TextBox();
            textBox.Margin = new Thickness(margin[0], margin[1], margin[2], margin[3]);
            textBox.Width = width;
            textBox.Height = height;
            textBox.Text = content;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;


            FilterPopupMainGrid.Children.Add(textBox);

        }

        public void AddFilter_Click(object sender, RoutedEventArgs e)
        {
            FilterPopupMainGrid.Children.RemoveAt(FilterPopupMainGrid.Children.Count - 1); // remove add filter button IF its the last child
            CreateFilterTextBox(elementWidth, elementHeight, [marginLeft, newElementGap, 0, 0], "New Filter");
            SharedData.FilterList.Add("New Filter");

            CreateRemoveFilterButton(elementWidth, elementHeight, [marginLeft, defaultElementGap, 0, 0], "Remove Filter");
            CreateNewFilterButton(elementWidth, elementHeight, [marginLeft, newElementGap, 0, 0], "Add Filter");
        }
        public void CreateNewFilterButton(short width, short height, short[] margin, string content)
        {
            Dispatcher.Invoke(() =>
            {
                Button button = new Button
                {
                    Content = content,
                    Width = width,
                    Height = height,
                    Margin = new Thickness(margin[0], margin[1], margin[2], margin[3]),
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left


                };

                button.Click += AddFilter_Click;
                FilterPopupMainGrid.Children.Add(button);

            });
        }




        public void CreateRemoveFilterButton(short width, short height, short[] margin, string content)
        {
            Dispatcher.Invoke(() =>
            {
                Button button = new Button
                {
                    Content = content,
                    Width = width,
                    Height = height,
                    Margin = new Thickness(margin[0], margin[1], margin[2], margin[3]),
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(1),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left

                };
                button.Click += RemoveFilter_Click;
                FilterPopupMainGrid.Children.Add(button);
            });
        }
        public void RemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            // filter are odd children, buttons are even children
            //exclude the last child

            List<TextBox> textBoxes = new List<TextBox>();
            List<Button> buttons = new List<Button>();

            foreach (var child in FilterPopupMainGrid.Children)
            {
                if (child is Button)
                {
                    buttons.Add((Button)child);
                }
                else if (child is TextBox)
                {
                    textBoxes.Add((TextBox)child);
                }


                //what button was clicked?
                if (sender == child)
                {
                    FilterPopupMainGrid.Children.Remove((Button)child);
                    FilterPopupMainGrid.Children.Remove(textBoxes[buttons.IndexOf((Button)child)]);
                    SharedData.FilterList.RemoveAt(buttons.IndexOf((Button)child));
                    break;
                }
            }




        }

        public void SaveFilter()
        {
            SharedData.FilterList.Clear();
            foreach (var item in FilterPopupMainGrid.Children)
            {
                if (item is TextBox)
                {
                    SharedData.FilterList.Add(((TextBox)item).Text);
                }
            }
        }
    }
}
