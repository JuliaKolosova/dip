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
                        SQLiteDataReader reader_ist = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}'  and PLAINTIFF=1", id));
                        while (reader_ist.Read())
                            ind = reader_ist.GetInt16(0);
                        reader_ist = connection.ReadData(string.Format("Select * from Actors Where id_doc ='{0}' and PLAINTIFF=1", id));
                        first = false;
                        if (ind == 0)
                        {
                            StackPanel_A_2_1.Visibility = Visibility.Visible;
                            Scroll.Visibility = Visibility.Visible;
                            StackPanel_A_2_2.Visibility = Visibility.Visible;
                            AddStPanelIstec1(null);
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
                                Scroll.Visibility = Visibility.Visible;
                                StackPanel_A_2_2.Visibility = Visibility.Visible;
                                AddStPanelIstec1(reader_ist);
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
        //пробное РАБОТАЕТ
        private void AddStPanelIstec1(SQLiteDataReader reader_ist)
        {
            //i++;
            SolidColorBrush colortext = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF378B1E"));
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.Margin = new Thickness(0, 0, 0, 10);

            Label label_ist = new Label() { Content = string.Concat("•"), FontSize = 20, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_ist_name = new TextBox();
            textbox_ist_name.Padding = new Thickness(1,1,1,1);
            textbox_ist_name.TextWrapping = TextWrapping.Wrap;
            textbox_ist_name.FontSize = 16;
            textbox_ist_name.AcceptsReturn = true;
            textbox_ist_name.Foreground = colortext;
           // textbox_ist_name.Height = 30;
            if (reader_ist != null)
            {
                textbox_ist_name.Text = reader_ist.GetString(2);
                textbox_ist_name.Tag= reader_ist.GetInt16(0).ToString();
            }
                

            Image img_del = new Image();
            img_del.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\delete.png", UriKind.Absolute));
            img_del.Width = 20;
            img_del.Height = 20;

            Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.Content = img_del;
            btn.Visibility = Visibility.Collapsed;
            if (first)
            {
                //Button btn = new Button();
                btn.Visibility = Visibility.Visible;
                btn.Click += new RoutedEventHandler(deleteTextBox);
            }


            //        StackPanel.Children.Add(grid);
            TextBlock label_ist_name = new TextBlock() { Text = "ФИО:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, TextWrapping = TextWrapping.Wrap };

            TextBlock label_ist1 = new TextBlock() { Text = "просивший рассмотреть дело в его отсутствие: заявление от", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext , TextWrapping = TextWrapping.Wrap };
            TextBox tex_box_ist_doc1 = new TextBox();
            tex_box_ist_doc1.Padding = new Thickness(1, 1, 1, 1);
            tex_box_ist_doc1.TextWrapping = TextWrapping.Wrap;
            tex_box_ist_doc1.FontSize = 16;
            tex_box_ist_doc1.AcceptsReturn = true;
            tex_box_ist_doc1.Foreground = colortext;
            //tex_box_ist_doc1.Height = 25;
            TextBlock label_ist2 = new TextBlock() { Text = "извещенный надлежайшим образом: документ, подтверждающий извещение:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, TextWrapping = TextWrapping.Wrap };
            TextBox tex_box_ist_doc2 = new TextBox();
            tex_box_ist_doc2.Padding = new Thickness(1, 1, 1, 1);
            tex_box_ist_doc2.TextWrapping = TextWrapping.Wrap;
            tex_box_ist_doc2.FontSize = 16;
            tex_box_ist_doc2.AcceptsReturn = true;
            tex_box_ist_doc2.Foreground = colortext;
            //textbox2.Height = 25;
            

            if (reader_ist != null)
                if (!reader_ist.IsDBNull(3))
                {
                    string str = reader_ist.GetString(3);
                    //char[] str1 = null;
                    string[] arr = str.Split('~');
                    //int index = str.IndexOf("///");
                    if (arr[0] != "")
                    {
                        //str1 = str.Substring(0, index - 1);
                        //str.CopyTo(0, str1, 0, index - 1);
                        tex_box_ist_doc1.Text = arr[0];
                    }
                    // str.CopyTo(0, str1, index + 2, str.Length - index + 2);
                    tex_box_ist_doc2.Text = arr[1];
                }
            Grid grid1 = new Grid();
            grid1.ColumnDefinitions.Add(new ColumnDefinition());
            //grid1.
           /* grid1.RowDefinitions.Add(new RowDefinition());
            grid1.Children.Add(label_ist);
            Grid.SetRow(label, 0);
            grid1.Children.Add(textbox_ist_name);
            Grid.SetRow(textbox_ist_name, 1);
            grid1.Children.Add(label_ist1);
            Grid.SetRow(label_ist1, 2);
            grid1.Children.Add(tex_box_ist_doc1);
            Grid.SetRow(tex_box_ist_doc1, 3);
            grid1.Children.Add(label_ist2);
            Grid.SetRow(label_ist2, 4);
            grid1.Children.Add(tex_box_ist_doc2);
            Grid.SetRow(tex_box_ist_doc2, 5);

            grid.Children.Add(grid1);
            Grid.SetColumn(grid1, 0);
            if (first)
            {
                grid.Children.Add(btn);
                Grid.SetColumn(btn, 1);
            }
            */
            Grid DynamicGrid = new Grid();
            DynamicGrid.Width = 430;
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            ColumnDefinition gridCol3 = new ColumnDefinition();
            gridCol1.Width = new GridLength(30);
            gridCol3.Width = new GridLength(30);
            DynamicGrid.ColumnDefinitions.Add(gridCol1);
            DynamicGrid.ColumnDefinitions.Add(gridCol2);
            DynamicGrid.ColumnDefinitions.Add(gridCol3);
            RowDefinition gridRow1 = new RowDefinition();
            RowDefinition gridRow2 = new RowDefinition();
            RowDefinition gridRow3 = new RowDefinition();
            RowDefinition gridRow4 = new RowDefinition();
            RowDefinition gridRow5 = new RowDefinition();
            RowDefinition gridRow6 = new RowDefinition();
            RowDefinition gridRow7 = new RowDefinition();
            gridRow7.Height = new GridLength(15);
            Rectangle rec = new Rectangle() { Fill = colortext, Height =1 };
            rec.Margin =  new Thickness(0, 7, 0, 7);
            Grid.SetColumnSpan(rec, 3);
            DynamicGrid.RowDefinitions.Add(gridRow1);
            DynamicGrid.RowDefinitions.Add(gridRow2);
            DynamicGrid.RowDefinitions.Add(gridRow3);
            DynamicGrid.RowDefinitions.Add(gridRow4);
            DynamicGrid.RowDefinitions.Add(gridRow5);
            DynamicGrid.RowDefinitions.Add(gridRow6);
            DynamicGrid.RowDefinitions.Add(gridRow7);
            //StackPanel_A_2_2.Children.Add(grid);
            // StackPanel_A_2_2.Children.Add(grid);

            Grid.SetRow(label_ist_name, 0);
            Grid.SetColumn(label_ist_name, 1);
            DynamicGrid.Children.Add(label_ist_name);
            Grid.SetRow(label_ist, 1);
            Grid.SetColumn(label_ist, 0);
            DynamicGrid.Children.Add(label_ist);
            Grid.SetRow(textbox_ist_name, 1);
            Grid.SetColumn(textbox_ist_name, 1);
            DynamicGrid.Children.Add(textbox_ist_name);
                Grid.SetRow(btn, 1);
                Grid.SetColumn(btn, 2);
                DynamicGrid.Children.Add(btn);
            

            Grid.SetRow(label_ist1, 2);
            Grid.SetColumn(label_ist1, 1);
            DynamicGrid.Children.Add(label_ist1);
            Grid.SetRow(tex_box_ist_doc1, 3);
            Grid.SetColumn(tex_box_ist_doc1, 1);
            DynamicGrid.Children.Add(tex_box_ist_doc1);
            Grid.SetRow(label_ist2, 4);
            Grid.SetColumn(label_ist2, 1);
            DynamicGrid.Children.Add(label_ist2);
            Grid.SetRow(tex_box_ist_doc2, 5);
            Grid.SetColumn(tex_box_ist_doc2, 1);
            DynamicGrid.Children.Add(tex_box_ist_doc2);
            Grid.SetRow(rec, 6);
            Grid.SetColumn(rec, 0);
            DynamicGrid.Children.Add(rec);
            StackPanel_A_2_2.Children.Add(DynamicGrid);
            //StackPanel.Children.Add(DynamicGrid);
        }

        public void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddStPanelIstec1(null);
        }
        /* удаление текстбокса с формы */
        private void deleteTextBox(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            var grids = StackPanel_A_2_2.Children;
            grids.Remove(grid);
            var textbox_name = grid.Children[2] as TextBox;
            SQLite connection = new SQLite();
            connection.WriteData(string.Format("delete from Actors Where id='{0}'", textbox_name.Tag));
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
                if (!reader.IsDBNull(3) && reader.GetString(3)!="") { Date_Copy.SelectedDate = DateTime.Parse(reader.GetString(3)); }
                if (!reader.IsDBNull(4)) TBPlace_Copy.Text = reader.GetString(4);
                if (!reader.IsDBNull(5)) TBName_Copy.Text = reader.GetString(5);
                if (!reader.IsDBNull(6)) TBSostav_Copy.Text = reader.GetString(6);
                if (!reader.IsDBNull(7)) TBSecretary_Copy.Text = reader.GetString(7);
            }
            connection.Close();

        }
        

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
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

        /* сохранение данных */
        private void SaveData()
        {
            bool isFull = false;
            SQLite connection = new SQLite();
            SQLiteDataReader reader_ist;
            if (formNumber == 0)
            {
                switch (listBox.SelectedIndex)
                {
                    case 0:
                        connection.WriteData(string.Format("Update Document set number='{0}', date='{1}', locate='{2}', NAME_COURT='{3}', CONTENT_COURT='{4}', SECRETARY='{5}'  Where id='{6}'", TBNumber_Copy.Text, Date_Copy.SelectedDate, TBPlace_Copy.Text, TBName_Copy.Text,TBSostav_Copy.Text,TBSecretary_Copy.Text, id));
                        break;
                    case 1:
                        
                        foreach (Grid grid in StackPanel_A_2_2.Children)
                        {
                            var textbox_name = grid.Children[2] as TextBox;
                            var textbox_doc1 = grid.Children[5] as TextBox;
                            var textbox_doc2 = grid.Children[7] as TextBox;
                            var str = textbox_doc1.Text + "~" + textbox_doc2.Text;
                           // reader_ist = connection.ReadData(string.Format("Select count(*) from ACTOR Where id='{0}'", textbox_name.Tag));
                            //while (reader_ist.Read())
                             //   ind = reader_ist.GetInt16(0);
                            if (textbox_name.Tag==null)
                                connection.WriteData(string.Format("INSERT INTO actors (id_doc,name_actor,actor_doc,plaintiff) VALUES ('{0}','{1}','{2}','{3}')", id, textbox_name.Text,str,1));
                            else
                                connection.WriteData(string.Format("Update actors set id_doc='{0}', name_actor='{1}', actor_doc='{2}' Where id='{3}'", id, textbox_name.Text, str, textbox_name.Tag));                        }
                        break;
                }
            }

            connection.Close();
            /*
                        // удаляем старые поля
                        foreach (XElement point in Stage.Elements().ToList())
                point.Remove();

            // добавляем новые поля
            foreach (Grid grid in DataPanel.Children)
            {
                var textbox = grid.Children[1] as TextBox;
                if (textbox.Text != "")
                {
                    XElement point = new XElement("Point", textbox.Text);
                    Stage.Add(point);
                    isFull = true;
                }
            }

            // сохраняем файл
            doc.Save("files\\" + name + ".xml");

            // обновляем matrixForm

            updateMForm(isFull);
            */
        }
    }
}
