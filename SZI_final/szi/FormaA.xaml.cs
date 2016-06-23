using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Data.SQLite;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SZI
{
    /// <summary>
    /// Логика взаимодействия для FormaA.xaml
    /// </summary>
    public partial class FormaA : Window
    {
        private int i = 0;
        private string name, item_header,item;
        private int formNumber; //какая форма А-0,Б-1,Б1-2 и т.д.
        private readonly ForaWindow fw;
        private bool first;
        private Grid grid1;
        int id;
        int ind;

        public FormaA(string name, ForaWindow fw, int formNumber,int id)
        {
            this.name = name;
            InitializeComponent();
            this.fw = fw;
            this.formNumber = formNumber;
            this.id = id;
            FormaA2.Title = " «" + name + "»";
            toFillTreeView(formNumber);
            //toFillTextBoxesFormaA_list1();
            addImg.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));
        }

        /* жмак мыши по листу дерева */
        private void treeItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            this.item = item.Header.ToString();
            var item_header = item.Parent as TreeViewItem;
            this.item_header = item_header.Header.ToString();
            //toFillTextBoxes();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grids = StackPanel.Children;
            for (int i=0;i< grids.Count;i++)
            {
                var grid = grids[i];
                grid.Visibility = Visibility.Collapsed;
            }
            if (formNumber == 0)
            {
                switch (listBox.SelectedIndex)
                {
                    case 0:
                        StackPanel_A_1_1.Visibility = Visibility.Visible;
                        StackPanel_A_1_2.Visibility = Visibility.Visible;
                        StackPanel_A_1_3.Visibility = Visibility.Visible;
                        StackPanel_A_1_4.Visibility = Visibility.Visible;
                        toFillTextBoxesFormaA_list1();
                        break;

                    case 1:
                        SQLite connection = new SQLite();
                        //SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from ACTOR Where id_doc='{0}'", id));
                        SQLiteDataReader reader_ist = connection.ReadData(string.Format("SELECT count(ID) FROM document"));
                        while (reader_ist.Read())
                            ind = reader_ist.GetInt16(0);
                        reader_ist = connection.ReadData(string.Format("Select * from Actor Where id_doc ='{0}' and PLAINTIFF=1", id));
                        first = false;
                        if (ind > 0)
                        {
                            StackPanel_A_2_1.Visibility = Visibility.Visible;
                            AddStPanelIstec(null);
                            first = true;
                        }
                        else
                        {
                            while (reader_ist.Read())
                            {
                                /* if (reader.GetInt32(0)!=0)
                                     {
                                     reader = connection.ReadData(string.Format("Select * from ACTOR Where id_doc='{0}'", id));
                                     while (reader.Read())
                                     {


                                     }
                                 }*/
                                StackPanel_A_2_1.Visibility = Visibility.Visible;
                                AddStPanelIstec(reader_ist);
                                first = true;
                            }
                        }
                        connection.Close();
                        break;
                }
            }
        }


        /* добавление истца на форму */
        private void AddStPanelIstec(SQLiteDataReader reader_ist)
        {
            i++;
            SolidColorBrush colortext = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF378B1E"));
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(20) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.Margin = new Thickness(0, 0, 0, 10);

            Label label = new Label() { Content = string.Concat(i, "."), FontSize = 20, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox = new TextBox();
            textbox.Padding = new Thickness(5, 2, 5, 2);
            textbox.TextWrapping = TextWrapping.Wrap;
            textbox.FontSize = 18;
            textbox.AcceptsReturn = true;
            textbox.Foreground = colortext;
            textbox.Height = 30;
            if (reader_ist!=null)
               textbox.Text = reader_ist.GetString(2);

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\delete.png", UriKind.Absolute));
            img.Width = 20;
            img.Height = 20;

            Button btn = new Button();
            if (first)
            {
                //Button btn = new Button();
                btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
                btn.Content = img;
                btn.Click += new RoutedEventHandler(deleteTextBox);
            }
            grid.Children.Add(label);
            Grid.SetColumn(label, 0);
            grid.Children.Add(textbox);
            Grid.SetColumn(textbox, 1);
            if (first)
            {
                grid.Children.Add(btn);
                Grid.SetColumn(btn, 3);
            }
    //        StackPanel.Children.Add(grid);

            grid1 = new Grid();
            grid1.ColumnDefinitions.Add(new ColumnDefinition());
            //grid1.
            grid1.RowDefinitions.Add(new RowDefinition());
            grid1.RowDefinitions.Add(new RowDefinition());
            grid1.RowDefinitions.Add(new RowDefinition());
            grid1.RowDefinitions.Add(new RowDefinition());
            TextBlock label1 = new TextBlock() { Text = "просивший рассмотреть дело в его отсутствие: заявление от", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };
            TextBox textbox1 = new TextBox();
            textbox1.Padding = new Thickness(1, 1, 1, 1);
            textbox1.TextWrapping = TextWrapping.Wrap;
            textbox1.FontSize = 16;
            textbox1.AcceptsReturn = true;
            textbox1.Foreground = colortext;
            //textbox1.Height = 25;
            TextBlock label2 = new TextBlock() { Text = "извещенный надлежайшим образом: документ, подтверждающий извещение:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, TextWrapping = TextWrapping.Wrap };
            TextBox textbox2 = new TextBox();
            textbox2.Padding = new Thickness(1, 1, 1, 1);
            textbox2.TextWrapping = TextWrapping.Wrap;
            textbox2.FontSize = 16;
            textbox2.AcceptsReturn = true;
            textbox2.Foreground = colortext;
            //textbox2.Height = 25;
            grid1.Children.Add(label1);
            Grid.SetRow(label1, 0);
            grid1.Children.Add(textbox1);
            Grid.SetRow(textbox1, 1);
            grid1.Children.Add(label2);
            Grid.SetRow(label2, 2);
            grid1.Children.Add(textbox2);
            Grid.SetRow(textbox2, 3);
            grid1.Visibility = Visibility.Collapsed;
            String GroupName = string.Concat("Involved", i);
            RadioButton involvT = new RadioButton();
            involvT.GroupName = GroupName;
            involvT.Content = "участвует в деле";
            involvT.Foreground = colortext;
            involvT.FontSize = 18;
            //involvT.IsChecked = true;
            

            RadioButton involvF = new RadioButton();
            involvF.GroupName = GroupName;
            involvF.Content = "не участвует в деле";
            involvF.Foreground = colortext;
            involvF.FontSize = 18;

            if (reader_ist != null)
                if (!reader_ist.IsDBNull(3))
                {
                    involvF.IsChecked = true;
                    string str = reader_ist.GetString(3);
                    //char[] str1 = null;
                    string[] arr = str.Split('~');
                    //int index = str.IndexOf("///");
                    if (arr[1]!="")
                    {
                        //str1 = str.Substring(0, index - 1);
                        //str.CopyTo(0, str1, 0, index - 1);
                        textbox1.Text = arr[1];
                    }
                    // str.CopyTo(0, str1, index + 2, str.Length - index + 2);
                    textbox2.Text = arr[0];
                }
                else
                {
                    involvT.IsChecked = true;
                }

            involvT.Checked += new RoutedEventHandler(FunInvolv);
            involvF.Checked += new RoutedEventHandler(FunInvolv);

            StackPanel st = new StackPanel();
           // st.Children.Add(grid);
            st.Children.Add(involvT);
            st.Children.Add(involvF);
            st.Children.Add(grid1);
            grid.Children.Add(st);
            StackPanel.Children.Add(grid);
        }

        public void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStPanelIstec(null);
        }
        /* удаление текстбокса с формы */
        private void deleteTextBox(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            var grids = StackPanel.Children;
            grids.Remove(grid);
        }

        private void FunInvolv(object sender, RoutedEventArgs e)
        {
            RadioButton involv = sender as RadioButton;
            StackPanel grid = (StackPanel)involv.Parent;
            var grids = grid.Children;
            if (involv.Content.ToString()!="участвует в деле")
            {
                grids[grids.Count - 1].Visibility = Visibility.Visible;
            }
            else 
                grids[grids.Count - 1].Visibility = Visibility.Collapsed;
            
        }

        private void FormaA2_Loaded(object sender, RoutedEventArgs e)
        {

        }

        /* заполнение текстбоксов нужной информацией */
        public void toFillTextBoxesFormaA_list1()
        {
            
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select * from Document Where id='{0}'", id));
            
            while (reader.Read())
            {
                if (!reader.IsDBNull(2)) TBNumber_Copy.Text = reader.GetString(2);
                if (!reader.IsDBNull(3)) { Date_Copy.DisplayDate = reader.GetDateTime(3); }
                if (!reader.IsDBNull(4)) TBPlace_Copy.Text = reader.GetString(4);
                if (!reader.IsDBNull(5)) TBName_Copy.Text = reader.GetString(5);
                if (!reader.IsDBNull(6)) TBSostav_Copy.Text = reader.GetString(6);
                if (!reader.IsDBNull(7)) TBSecretary_Copy.Text = reader.GetString(7);
            }
            connection.Close();

        }
        /* заполнение листбокса */
        private void toFillTreeView(int formNumber)
        {
            if (formNumber == 0)
            {
                listBox.Items.Clear();
                listBox.Items.Add("Общая информация о деле");
                listBox.Items.Add("Информация об истцах");
                listBox.Items.Add("Информация о представителях истцов");
                listBox.Items.Add("Информация об ответчиках");
                listBox.Items.Add("Информация о представителях ответчиков");
                listBox.Items.Add("Информация о заседании");
                listBox.SelectedIndex = 0;
            }
        }
    }
}
