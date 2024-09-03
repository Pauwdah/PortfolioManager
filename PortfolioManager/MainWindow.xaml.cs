using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using HtmlAgilityPack;
using Microsoft.Win32;
using Renci.SshNet;
using Renci.SshNet.Sftp;
using System.ComponentModel.DataAnnotations;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing;
using System.Xml.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Reflection.Metadata;
using System.Reflection;
using System.Windows.Data;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Media;

namespace PortfolioManager
{
    public static class SharedData
    {
        public static List<string> FilterList { get; set; } = new List<string>();
        public static List<ItemCard> ItemCardList = new List<ItemCard>();
        //public static SolidColorBrush DefaultBorderColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF333333"));
        //public static SolidColorBrush DefaultItemBackgroundColor = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFCCCCCC"));
        public static string DefaultBorderColor = "#FF333333";
        public static string DefaultItemBackgroundColor = "#FFCCCCCC";
        


    }
    public class Filter
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }

        public Filter(string Name, bool IsChecked)
        {
            this.Name = Name;
            this.IsChecked = IsChecked;
        }
    }

    public class ItemCard
    {
        private readonly MainWindow _mainWindow;
        public string Src { get; set; }
        public string Full { get; set; }
        public string Alt { get; set; }
        public byte Id { get; set; }
        public List<Filter> Filter { get; set; } = new List<Filter>();

        public Border Border { get; set; }
        public Grid Grid { get; set; }
        public System.Windows.Controls.Image Image { get; set; }
        public TextBox AltBox { get; set; }
        public TextBox IdBox { get; set; }
        public Button ChangeImage { get; set; }
        public Button DeleteButton { get; set; }
        public ComboBox ComboBox { get; set; }

        public ItemCard(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }
        public void AddFilter(string name, bool isChecked)
        {
            Filter.Add(new Filter(name, isChecked));
        }
        public void RemoveFilter(string name)
        {
            Filter.Remove(Filter.Find(x => x.Name == name));
        }
        public void ClearFilter()
        {
            Filter.Clear();
        }
        public void ReconstructHTML(string portfolioFolder, string dotHtml, ItemCard itemCard)
        {
            // Load the HTML document
            HtmlDocument doc = new HtmlDocument();
            doc.Load("./" + portfolioFolder + dotHtml);

            string docName = dotHtml.Replace(".html", "");
            docName = char.ToUpper(docName[0]) + docName.Substring(1);
            string idPrefix = dotHtml.Substring(0, 3) + "-";
            string filter = "";
            foreach (var item in Filter)
            {
                if (item.IsChecked)
                {
                    filter += " " + item.Name;
                }
            }


            HtmlNode gridDiv = doc.DocumentNode.SelectSingleNode($"//div[@id='{docName}Container']");
            HtmlNode itemDiv = HtmlNode.CreateNode($"<div class=\"item{filter}\" id=\"{idPrefix + itemCard.Id}\"></div>");
            HtmlNode imgElement = HtmlNode.CreateNode($"<img src=\"{Src}\" alt=\"{AltBox.Text}\" data-full=\"{Full}\" />");

            // Remove the existing item if it exists
            var existingItem = gridDiv.SelectSingleNode($"//div[@id='{idPrefix + itemCard.Id}']");
            if (existingItem != null)
            {
                existingItem.Remove();
            }


            if (gridDiv != null)
            {
                itemDiv.AppendChild(imgElement);
                gridDiv.AppendChild(itemDiv);
            }
            doc.Save(portfolioFolder + dotHtml);
        }

        public void CreateItemCard(string appDir, WrapPanel wrapPanel)
        {

            string imagePath = Src.TrimStart('/');
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(Path.Combine(appDir, imagePath), UriKind.Absolute);
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();


            Grid = new Grid
            {
                Width = 200,
                Height = 300,
                Margin = new Thickness(10),



            };
            Border = new Border
            {
                BorderThickness = new Thickness(2), // Set the border thickness
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultItemBackgroundColor)),
                Margin = new Thickness(5), // Set the margin
                Child = Grid, // Set the Grid as the child of the Border

                Effect = new System.Windows.Media.Effects.DropShadowEffect
                {
                    Color = System.Windows.Media.Colors.Black,
                    Direction = 320,
                    ShadowDepth = 3,
                    Opacity = 0.3,
                    BlurRadius = 3
                }
            };


            Image = new System.Windows.Controls.Image
            {
                Source = bitmap,
                Width = 200,
                Height = 300,
                Margin = new Thickness(0),


            };

            AltBox = new TextBox
            {
                Width = 200,
                Height = 20,
                Margin = new Thickness(0, 0, 0, 0),
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                BorderThickness = new Thickness(1),
                Text = Alt,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,

            };
            AltBox.TextChanged += (sender, e) =>
            {
                Alt = AltBox.Text;
            };
            IdBox = new TextBox
            {
                Width = 30,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 0),
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                BorderThickness = new Thickness(1),
                Text = Id.ToString(),
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            //IdBox.TextChanged += (sender, e) =>
            //{
            //    if (byte.TryParse(IdBox.Text, out byte id))
            //    {
            //        Id = id;
            //    }
            //};
            // subscribe to SortItems event for the IdBox
            IdBox.TextChanged += (sender, e) =>
            {
                if (byte.TryParse(IdBox.Text, out byte id))
                {
                    Id = id;
                    _mainWindow.SortItems(sender);
                }
            };

            ChangeImage = new Button
            {
                Content = "Change Image",
                Width = 100,
                Height = 23,
                Margin = new Thickness(0, 5, 0, 0),
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                BorderThickness = new Thickness(1),
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,

            };

            DeleteButton = new Button
            {
                Content = "❌",
                Width = 30,
                Height = 30,
                Margin = new Thickness(0, 0, 0, 0),
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FFFFFFFF")),
                BorderThickness = new Thickness(1),
                FontSize = 18,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,
            };

            DeleteButton.Click += _mainWindow.DeleteItem_Click;

            Grid lowerControlGrid = new Grid
            {
                Width = 200,
                Height = 50,
                Margin = new Thickness(0),
                Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultItemBackgroundColor)),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom,
            };

            ComboBox = new ComboBox
            {
                Width = 90,
                Height = 23,
                Margin = new Thickness(0, 5, 0, 0),
                BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                BorderThickness = new Thickness(1),

                SelectedIndex = 0,
                IsDropDownOpen = false,
                StaysOpenOnEdit = false,
                ItemsSource = Filter,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Top,

            };

            ComboBox.ContextMenuClosing += (sender, e) =>
            {
                Filter.Clear();
                foreach (Filter item in ComboBox.Items)
                {
                    Filter.Add(item);
                }
            };



            // Create DataTemplate for ComboBox items
            DataTemplate dataTemplate = new DataTemplate();

            // Define the visual tree of the DataTemplate
            FrameworkElementFactory stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            FrameworkElementFactory checkBoxFactory = new FrameworkElementFactory(typeof(CheckBox));
            checkBoxFactory.SetBinding(CheckBox.IsCheckedProperty, new Binding("IsChecked"));
            checkBoxFactory.SetBinding(CheckBox.ContentProperty, new Binding("Name"));

            stackPanelFactory.AppendChild(checkBoxFactory);
            dataTemplate.VisualTree = stackPanelFactory;

            // Assign DataTemplate to ComboBox
            ComboBox.ItemTemplate = dataTemplate;

            lowerControlGrid.Children.Add(AltBox);
            lowerControlGrid.Children.Add(ChangeImage);
            lowerControlGrid.Children.Add(ComboBox);
            Grid.Children.Add(Image);

            //Grid.Children.Add(AltBox);
            Grid.Children.Add(IdBox);

            //Grid.Children.Add(ChangeImage);
            Grid.Children.Add(DeleteButton);

            //Grid.Children.Add(ComboBox);
            Grid.Children.Add(lowerControlGrid);

            wrapPanel.Children.Add(Border);
        }
    }

    public partial class MainWindow : Window
    {
        private LoginPopup login = new LoginPopup();
        
        
        private const string portfolioFolder = "html/portfolio/";
        private const string illustrationDotHtml = "illustration.html";
        private const string characterDesignHTML = "character_design.html";
        private const string illustrationPreviewPath = "img/preview/illustration/";
        private const string illustrationFullscalePath = "img/fullscale/illustration/";

        private string appDir;

        HtmlDocument doc_IllustrationHTML = new HtmlDocument();


        List<byte> idList = new List<byte>();
        public ObservableCollection<Filter> Filter = new ObservableCollection<Filter>();



        public MainWindow()
        {
            InitializeComponent();
            OpenLoginWindow();
            Login();
        }

        private void OpenLoginWindow()
        {
            var popup = new LoginPopup();
            popup.ShowDialog();
        }

        public async void Login() {
            appDir = AppDomain.CurrentDomain.BaseDirectory;
            appDir = appDir.Replace("\\", "/");
            await DownloadContent();
            CreateIllustrationAddingContainer();
        }
        private async Task DownloadContent()
        {
            try
            {
                await Task.Run(() =>
                {
                    using (var sftp = new SftpClient(login.RetrieveAndDecryptHost(), login.RetrieveAndDecryptUsername(), login.RetrieveAndDecryptPassword()))
                    {
                        try { sftp.Connect();
                            // DIR CHECK
                            if (!Directory.Exists("./" + portfolioFolder))
                            {
                                Directory.CreateDirectory("./" + portfolioFolder);
                            }
                            if (!Directory.Exists("./" + illustrationPreviewPath))
                            {
                                Directory.CreateDirectory("./" + illustrationPreviewPath);

                            }
                            if (!Directory.Exists("./" + illustrationFullscalePath))
                            {
                                Directory.CreateDirectory("./" + illustrationFullscalePath);
                            }

                            //Download HTMLs
                            using (var fileStream = File.OpenWrite("./" + portfolioFolder + illustrationDotHtml))
                            {
                                sftp.DownloadFile(portfolioFolder + illustrationDotHtml, fileStream);
                            }

                            using (var fileStream = File.OpenWrite("./" + portfolioFolder + characterDesignHTML))
                            {
                                sftp.DownloadFile(portfolioFolder + characterDesignHTML, fileStream);
                            }


                            //Download Illustration


                            doc_IllustrationHTML.Load("./" + portfolioFolder + illustrationDotHtml);
                            //Filter list
                            var filterElements = doc_IllustrationHTML.DocumentNode.SelectNodes("//div[contains(@class, 'filter_buttons')]/button");
                            if (filterElements != null)
                            {
                                foreach (var filter in filterElements)
                                {
                                    if (filter != null)
                                    {
                                        var filterName = filter.GetAttributeValue("data-filter", string.Empty);
                                        if (filterName == "all") continue;
                                        SharedData.FilterList.Add(filterName);


                                    }
                                }
                            }





                            //Images
                            Dispatcher.Invoke(() =>
                            {
                                //Create ItemCard
                                //ItemCard itemCard = new ItemCard("", "", "", 0, new List<string>(), new Border(), new Grid(), new System.Windows.Controls.Image(), new TextBox(), new TextBox(), new Button(), new Button(), new ComboBox());

                                var itemElements = doc_IllustrationHTML.DocumentNode.SelectNodes("//div[@id='IllustrationContainer']/div[contains(@class, 'item')]/img");
                                if (itemElements != null)
                                {
                                    foreach (var img in itemElements)
                                    {
                                        ItemCard itemCard = new ItemCard(this);
                                        var filter = img.ParentNode.GetAttributeValue("class", string.Empty);
                                        filter = filter.Replace("item", "");

                                        string[] filterArray = filter.Split(" ");

                                        foreach (var filterName in SharedData.FilterList)
                                        {
                                            if (filterArray.Contains(filterName))
                                            {
                                                itemCard.AddFilter(filterName, true);
                                            }
                                            else
                                            {
                                                itemCard.AddFilter(filterName, false);
                                            }
                                        }

                                        if (img != null)
                                        {
                                            itemCard.Src = img.GetAttributeValue("src", string.Empty);
                                            itemCard.Full = img.GetAttributeValue("data-full", string.Empty);
                                            Debug.Write("Fullscale Path: " + itemCard.Full);



                                            try
                                            {
                                                //DOWNLOAD IMAGE
                                                if (!(File.Exists("." + itemCard.Full) && File.Exists("." + itemCard.Src)))
                                                {
                                                    using (var fileStream = File.OpenWrite("." + itemCard.Full))
                                                    {

                                                        sftp.DownloadFile(itemCard.Full, fileStream);
                                                    }
                                                    ConvertFullscaleImageToPreview(itemCard.Full, illustrationPreviewPath);
                                                }
                                                //Add ID
                                                itemCard.Id = byte.Parse(img.ParentNode.GetAttributeValue("id", string.Empty).Substring(4));

                                                //Add ALT
                                                string alt = "";
                                                itemCard.Alt = img.GetAttributeValue("alt", string.Empty);
                                                alt = img.GetAttributeValue("alt", string.Empty);



                                                //Create UI
                                                itemCard.CreateItemCard(appDir, IllustrationDynamicPanel);
                                                //Shared Data
                                                idList.Add(itemCard.Id);
                                                SharedData.ItemCardList.Add(itemCard);
                                            }
                                            catch (Exception ex)
                                            {
                                                Debug.WriteLine($"Error downloading or constructing file: {ex.Message}");
                                            }



                                        }
                                    }
                                }

                            });
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error Logging in: {ex.Message}");
                            ;
                        }
                        
                        sftp.Disconnect();
                    }
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error Logging in: {ex.Message}");
            }


        }

        public void SortItems(Object sender)
        {
            //event: we change the id of image '10' to '5'
            //-- solution
            //	1. new list gets created
            //	2. get the lowest number(10 or 5)
            //	3. copy from old list until 5 - 1 (4)
            //	4. add changed image(5)
            //	5. copy from old list from 5 + 1 (6)  to 10 -1 (9)
            //	6. skipp 10
            //	7. copy from old list from 10 +1 to end


            //List<ItemCard> sortedList = new List<ItemCard>();
        }

        #region UI 
        #region Creation


        private void CreateIllustrationAddingContainer()
        {
            Grid grid = CreateItemDisplayGrid("AddingContainer", IllustrationDynamicPanel);
            Dispatcher.Invoke(() =>
            {
                Button button = new Button
                {
                    Content = "Select Image",
                    Width = 200,
                    Height = 300,
                    Margin = new Thickness(0, 0, 0, 0),
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Opacity = 0

                };


                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri("pack://application:,,,/img/addPanel.jpg");
                bitmap.EndInit();

                System.Windows.Controls.Image image = new System.Windows.Controls.Image
                {
                    Source = bitmap,
                    Width = 200,
                    Height = 300,
                    Margin = new Thickness(0),

                };
                grid.Children.Add(image);

                button.Click += CreateNewItem_Click;

                grid.Children.Add(button);


            });
        }
        private Grid CreateItemDisplayGrid(string name, WrapPanel wrapPanel)
        {
            Grid grid = null;
            Dispatcher.Invoke(() =>
            {
                grid = new Grid
                {
                    Width = 200,
                    Height = 300,
                    Margin = new Thickness(10),
                };
                Border border = new Border
                {
                    BorderThickness = new Thickness(2),
                    BorderBrush = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultBorderColor)),
                    Background = new SolidColorBrush((System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString(SharedData.DefaultItemBackgroundColor)),
                    Margin = new Thickness(5),
                    Child = grid,


                };
                wrapPanel.Children.Add(border);
            });

            return grid;

        }
        #endregion


        #region Deletion
        /// <summary>
        /// Unload the image resources and remove the item from the grid.
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public async Task UnloadItem(Grid grid)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                foreach (var child in grid.Children)
                {
                    if (child is System.Windows.Controls.Image image)
                    {
                        image.Source = null; // Release the image resources
                    }
                }
                grid.Children.Clear();
                grid.RowDefinitions.Clear();
                grid.ColumnDefinitions.Clear();

                Border border = (Border)grid.Parent;
                WrapPanel parentPanel = (WrapPanel)border.Parent;
                parentPanel.Children.Remove(border);


            });
        }
        #endregion

        #region Reload
        private void ReloadUI()
        {
            Dispatcher.Invoke(() =>
            {
                IllustrationDynamicPanel.Children.Clear();
                //Goes through all the UI elements
                foreach (var item in SharedData.ItemCardList)
                {
                    //temp list to store the filters
                    List<Filter> tempFilterList = new List<Filter>();

                    foreach (var filterName in SharedData.FilterList)
                    {
                        if (SharedData.FilterList.Contains(filterName))
                        {
                            //get toggle state
                            Filter tmpFilter = null;

                            if (item.Filter != null)
                            {
                                // Assuming item.Filter is a list of Filter objects
                                var filter = item.Filter.FirstOrDefault(f => f.Name == filterName);
                                if (filter != null)
                                {
                                    tmpFilter = new Filter(filterName, filter.IsChecked);
                                }
                                else
                                {
                                    // Default to false if the filter is not found
                                    tmpFilter = new Filter(filterName, false);
                                }

                                tempFilterList.Add(tmpFilter);
                            }
                        }
                    }

                    item.ClearFilter();
                    item.Filter = tempFilterList;
                    item.CreateItemCard(appDir, IllustrationDynamicPanel);
                }
                CreateIllustrationAddingContainer();
            });
        }
        #endregion

        #endregion



        #region Button Click Handlers
        private void CreateNewItem_Click(object sender, RoutedEventArgs e)
        {
            // open file dialog
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg) | *.jpg; *.jpeg";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == true)
            {
                IllustrationDynamicPanel.Children.RemoveAt(IllustrationDynamicPanel.Children.Count - 1); // Remove the "Add Image" button
                foreach (string file in openFileDialog.FileNames)
                {
                    //Save image to fullscale folder
                    string fullscaleFile = illustrationFullscalePath + Path.GetFileName(file);
                    File.Copy(file, fullscaleFile, true);
                    ConvertFullscaleImageToPreview(file, illustrationPreviewPath);

                    ItemCard itemCard = new ItemCard(this);

                    itemCard.Full = '/' + fullscaleFile;
                    itemCard.Src = '/' + illustrationPreviewPath + Path.GetFileName(file);
                    itemCard.Id = Convert.ToByte(idList.Count + 1);
                    itemCard.CreateItemCard(appDir, IllustrationDynamicPanel);
                    SharedData.ItemCardList.Add(itemCard);
                    ReloadUI();
                }
            }
        }
        public async void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Grid grid = (Grid)button.Parent;
            string name = ((System.Windows.Controls.Image)grid.Children[0]).Source.ToString();
            name = name.Substring(name.LastIndexOf("/") + 1);
            string full = illustrationFullscalePath + name;
            string preview = illustrationPreviewPath + name;
            //byte id = byte.Parse(((TextBox)grid.Children[1]).Text);
            //search the actual definit textbox of id instead of relying on the order
            byte id = 0;
            foreach (var child in grid.Children)
            {
                if (child is TextBox && ((TextBox)child).Name == "IdBox")
                {

                    id = byte.Parse(((TextBox)child).Text);

                }
            }

            // Set the image source to null to release the file handle
            if (grid.Children[0] is System.Windows.Controls.Image image)
            {
                image.Source = null;
            }
            // Remove the item from the HTML
            doc_IllustrationHTML.Load("./" + portfolioFolder + illustrationDotHtml);
            var item = doc_IllustrationHTML.DocumentNode.SelectSingleNode($"//div[@id='IllustrationContainer']/div[@id='ill-{id}']");
            if (item != null)
            {
                item.Remove();
                doc_IllustrationHTML.Save(portfolioFolder + illustrationDotHtml);
            }

            await UnloadItem(grid);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            //remove the item from the list

            SharedData.ItemCardList.Remove(SharedData.ItemCardList.Find(x => x.Id == id));

            await Task.Run(() =>
            {
                try
                {
                    if (File.Exists(full))
                    {
                        File.Delete(full);
                    }
                    if (File.Exists(preview))
                    {
                        File.Delete(preview);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting file: {ex.Message}");
                }
            });
            //wait for a few seconds to let the file be deleted
            await Task.Delay(1000);

        }

        private void PublishButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in SharedData.ItemCardList)
            {
                item.ReconstructHTML(portfolioFolder, illustrationDotHtml, item);
            }
            // upload the HTML files to the server
            using (var sftp = new SftpClient(login.RetrieveAndDecryptHost(), login.RetrieveAndDecryptUsername(), login.RetrieveAndDecryptPassword()))
            {
                sftp.Connect();

                using (var fileStream = File.OpenRead(portfolioFolder + illustrationDotHtml))
                {
                    sftp.UploadFile(fileStream, portfolioFolder + illustrationDotHtml);
                }
                using (var fileStream = File.OpenRead(portfolioFolder + characterDesignHTML))
                {
                    sftp.UploadFile(fileStream, portfolioFolder + characterDesignHTML);
                }
                foreach (SftpFile file in sftp.ListDirectory(illustrationPreviewPath))
                {
                    if ((file.Name != ".") && (file.Name != ".."))
                    {


                        sftp.DeleteFile(file.FullName);

                    }
                }
                foreach (SftpFile file in sftp.ListDirectory(illustrationFullscalePath))
                {
                    if ((file.Name != ".") && (file.Name != ".."))
                    {

                        sftp.DeleteFile(file.FullName);

                    }
                }
                var previewImages = Directory.GetFiles(illustrationPreviewPath);
                var fullscaleImages = Directory.GetFiles(illustrationFullscalePath);
                foreach (var img in previewImages)
                {
                    using (var fileStream = File.OpenRead(img))
                    {
                        sftp.UploadFile(fileStream, img);
                    }
                }
                foreach (var img in fullscaleImages)
                {
                    using (var fileStream = File.OpenRead(img))
                    {
                        sftp.UploadFile(fileStream, img);
                    }
                }

                sftp.Disconnect();
            }


        }
        private void FilterMenu_Click(object sender, RoutedEventArgs e)
        {
            var popup = new FilterPopup();
            popup.ShowDialog();
            ReloadUI();
            Debug.WriteLine("Filter saved.");
        }
        private async void Redownload_Click(object sender, RoutedEventArgs e)
        {
            SharedData.ItemCardList.Clear();
            SharedData.FilterList.Clear();
            OpenLoginWindow();
            ReloadUI();
        }


        private void ExitApplication_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Utility Functions
        private void ConvertFullscaleImageToPreview(string file, string previewPath)
        {
            // Load the full-scale image
            file = file.TrimStart('/');
            Debug.WriteLine(appDir + file);
            Bitmap fullscaleImage = new Bitmap(Path.Combine(appDir, file));

            // Calculate new dimensions while keeping the aspect ratio
            int newWidth, newHeight;

            if (fullscaleImage.Width < fullscaleImage.Height)
            {
                newWidth = 800;
                newHeight = (int)((800.0 / fullscaleImage.Width) * fullscaleImage.Height);
            }
            else
            {
                newHeight = 800;
                newWidth = (int)((800.0 / fullscaleImage.Height) * fullscaleImage.Width);
            }

            // Create a new bitmap with the new dimensions
            Bitmap previewImage = new Bitmap(newWidth, newHeight);

            // Draw the full-scale image onto the new bitmap, resizing it
            using (Graphics graphics = Graphics.FromImage(previewImage))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.DrawImage(fullscaleImage, 0, 0, newWidth, newHeight);
            }

            // Save the preview image to a file
            string previewFile = Path.Combine(Path.Combine(appDir, previewPath),
                                              Path.GetFileNameWithoutExtension(file) + ".jpg");

            previewImage.Save(previewFile, ImageFormat.Jpeg);

            Debug.WriteLine($"Saved preview image to {previewFile}");
            // Clean up
            fullscaleImage.Dispose();
            previewImage.Dispose();
        }
        #endregion
    }

}
