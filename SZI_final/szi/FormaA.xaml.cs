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
        private SolidColorBrush colortext = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF378B1E"));

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
                        Button_Next.IsEnabled = true;
                        StackPanel_A_1_1.Visibility = Visibility.Visible;
                        StackPanel_A_1_2.Visibility = Visibility.Visible;
                        StackPanel_A_1_3.Visibility = Visibility.Visible;
                        StackPanel_A_1_4.Visibility = Visibility.Visible;
                        toFillTextBoxesFormaA_list1();
                        break;

                    case 1:
                        Button_Next.IsEnabled = true;
                        UpdateA_2();
                        break;
                    case 2:
                        Button_Next.IsEnabled = true;
                        StackPanel_A_3_1.Visibility = Visibility.Visible;
                        StackPanel_A_3_2.Visibility = Visibility.Visible;
                        TabControlPredIst();

                        break;

                    case 5:
                        Button_Next.IsEnabled = false;
                        break;
                }
            }
        }

        //заполняем tabcontrol  о представителях истцов
        private void TabControlPredIst()
        {
            bool firstpred;
            SQLite connection = new SQLite();
            SQLiteDataReader reader_ist, reader_pred_ist;
            int count=0, count_reader_ist_i=0;
            int ist_id = 0;
            
            reader_ist = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}'  and PLAINTIFF=1", id));
            while (reader_ist.Read())
               count = reader_ist.GetInt16(0);
            if (count == 0)
            {
                label_pred_ist2.Visibility = Visibility.Visible;
                Tab_Presd_ist.Visibility = Visibility.Collapsed;
            }
            else
            {

                Tab_Presd_ist.Visibility = Visibility.Visible;
                label_pred_ist2.Visibility = Visibility.Collapsed;
                Tab_Presd_ist.Items.Clear();
                reader_pred_ist = connection.ReadData(string.Format("Select ID, NAME_ACTOR from Actors Where id_doc ='{0}' and PLAINTIFF=1", id));


                while (reader_pred_ist.Read())
                {
                    firstpred = true;
                    ist_id = reader_pred_ist.GetInt16(0);
                    StackPanel pred_ist_panel = new StackPanel(); ;
                    //pred_ist_panel.Children.Clear();
                    SQLiteDataReader count_reader_ist = connection.ReadData(string.Format("Select count(*) from AGENT_PLAINTIFF Where id_actor='{0}'", ist_id));
                    while (count_reader_ist.Read())
                         count_reader_ist_i = count_reader_ist.GetInt16(0);

                    pred_ist_panel.Children.Add(CreateHeaderPredIst(reader_pred_ist));

                    if (count_reader_ist_i == 0)
                    {
                        pred_ist_panel.Children.Add(AddStPanelPredIstec(null, firstpred));
                        firstpred = false;
                        Tab_Presd_ist.Items.Add(new TabItem
                        {
                            Header = new TextBlock { Text = reader_pred_ist.GetString(1).Split(' ')[0] },
                            Tag = new TextBlock { Text = ist_id.ToString() },
                            Content = pred_ist_panel
                        });
                    }
                    else
                    {                        
                        SQLiteDataReader reader_pred_ist2 = connection.ReadData(string.Format("Select * from AGENT_PLAINTIFF Where id_actor ='{0}'", ist_id));
                        pred_ist_panel.Children.Add(AddStPanelPredIstec(reader_pred_ist2, firstpred));
                        firstpred = false;
                        while (reader_pred_ist2.Read())
                        {
                            pred_ist_panel.Children.Add(AddStPanelPredIstec(reader_pred_ist2, firstpred));
                            // тут нужно как-то динамично считывть информацию. может быть несколько представителей у одного. возможность добавления/удаления :(
                        }
                        Tab_Presd_ist.Items.Add(new TabItem
                        {
                            Header = new TextBlock { Text = reader_pred_ist.GetString(1) },
                            Tag = new TextBlock { Text = ist_id.ToString() },
                            Content = pred_ist_panel
                            
                        });
                       
                    }
                }
            }

            connection.Close();
        }


        public Grid CreateHeaderPredIst(SQLiteDataReader reader_pred_ist)
        {
            Grid DynamicGrid = new Grid();
            DynamicGrid.Width = 550;
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            gridCol2.Width = new GridLength(30);
            DynamicGrid.ColumnDefinitions.Add(gridCol1);
            DynamicGrid.ColumnDefinitions.Add(gridCol2);
            RowDefinition gridRow1 = new RowDefinition();
            DynamicGrid.RowDefinitions.Add(gridRow1);


            Label label_info_pred_ist = new Label() { Content = string.Concat("Представители истца: ", reader_pred_ist.GetString(1)), FontSize = 18, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, Height = 31, Width = 403 };

            Image img_add = new Image();
            img_add.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));
            img_add.Width = 20;
            img_add.Height = 20;

            Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.Content = img_add;
            btn.Click += new RoutedEventHandler(AddBtnPrIst_Click);

            Grid.SetRow(label_info_pred_ist, 0);
            Grid.SetColumn(label_info_pred_ist, 0);
            DynamicGrid.Children.Add(label_info_pred_ist);
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 1);
            DynamicGrid.Children.Add(btn);
            return DynamicGrid;
        }

        public void AddBtnPrIst_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid gridParent = (Grid)btn.Parent;
            StackPanel stackParent = (StackPanel)gridParent.Parent;
            stackParent.Children.Add( AddStPanelPredIstec(null,false));
        }

        private Grid AddStPanelPredIstec(SQLiteDataReader reader_pred_ist, bool firstpred)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.Margin = new Thickness(0, 0, 0, 10);
            Label label_predist = new Label() { Content = string.Concat("•"), FontSize = 20, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_predist_name = new TextBox();
            textbox_predist_name.Padding = new Thickness(1, 1, 1, 1);
            textbox_predist_name.TextWrapping = TextWrapping.Wrap;
            textbox_predist_name.FontSize = 16;
            textbox_predist_name.AcceptsReturn = false;
            textbox_predist_name.Foreground = colortext;
            // textbox_ist_name.Height = 30;


            Image img_del = new Image();
            img_del.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\delete.png", UriKind.Absolute));
            img_del.Width = 20;
            img_del.Height = 20;

            Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.Content = img_del;
            btn.Visibility = Visibility.Collapsed;
            if (!firstpred)
            {
                //Button btn = new Button();
                btn.Visibility = Visibility.Visible;
                btn.Click += new RoutedEventHandler(deletePredIst);
            }

            //        StackPanel.Children.Add(grid);
            Label label_predist_name = new Label() { Content = "ФИО представителя:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            Label label_doc_predist = new Label() { Content = "Действующего на основании:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            Label label_doc1_predist = new Label() { Content = "  - доверенности от ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            DatePicker date_dover_ot = new DatePicker();

            Label label_doc1_2_predist = new Label() { Content = " со сроком действия до ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            DatePicker date_dover_ot_2 = new DatePicker();

            Label label_doc2_predist = new Label() { Content = "  - ордера адвоката от ", FontSize = 16, Margin = new Thickness(0, 10, 0, 10), Foreground = colortext };

            DatePicker date_order_ot = new DatePicker() { Margin = new Thickness(0, 10, 0, 10) };

            Label label_doc3_predist = new Label() { Content = "  - наименование документа, ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox tex_box_predist_doc1 = new TextBox();
            tex_box_predist_doc1.Padding = new Thickness(1, 1, 1, 1);
            tex_box_predist_doc1.TextWrapping = TextWrapping.Wrap;
            tex_box_predist_doc1.FontSize = 16;
            tex_box_predist_doc1.AcceptsReturn = true;
            tex_box_predist_doc1.Foreground = colortext;
            tex_box_predist_doc1.Width = 280;
            tex_box_predist_doc1.Height = 25;

            Label label_doc3_2_predist = new Label() { Content = "удостоверяющего служебное положение их представителей от ", FontSize = 16, Margin = new Thickness(0, 1, 1, 0), Foreground = colortext };

            DatePicker date_dover_ot_3 = new DatePicker();

            Label label_doc4_predist = new Label() { Content = "  - наименование документа, ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox tex_box_predist_doc4 = new TextBox();
            tex_box_predist_doc4.Padding = new Thickness(1, 1, 1, 1);
            tex_box_predist_doc4.TextWrapping = TextWrapping.Wrap;
            tex_box_predist_doc4.FontSize = 16;
            tex_box_predist_doc4.AcceptsReturn = true;
            tex_box_predist_doc4.Foreground = colortext;
            tex_box_predist_doc4.Width = 280;
            tex_box_predist_doc4.Height = 25;           

            Label label_doc4_2_predist = new Label() { Content = ", удостоверяющего статус и полномочия законного представителя в силу ст.", FontSize = 16, Margin = new Thickness(0, 1, 1, 0), Foreground = colortext };

            TextBox tex_box_predist_doc4_2 = new TextBox();
            tex_box_predist_doc4_2.Padding = new Thickness(1, 1, 1, 1);
            tex_box_predist_doc4_2.TextWrapping = TextWrapping.Wrap;
            tex_box_predist_doc4_2.FontSize = 16;
            tex_box_predist_doc4_2.AcceptsReturn = true;
            tex_box_predist_doc4_2.Foreground = colortext;
            tex_box_predist_doc4_2.Width = 280;
            tex_box_predist_doc4_2.Height = 25;
            if (reader_pred_ist != null)
            {
                textbox_predist_name.Text = reader_pred_ist.GetString(2);
                textbox_predist_name.Tag = reader_pred_ist.GetInt16(0).ToString();

                if (reader_pred_ist.GetString(3) != null)
                {
                    var str = reader_pred_ist.GetString(3).Split('~');
                    if ((str[0].Length != 0) && (str[1].Length != 0))
                    {
                        date_dover_ot.SelectedDate = DateTime.Parse(str[0]);
                        date_dover_ot_2.SelectedDate = DateTime.Parse(str[1]);
                    }
                    if (str[2].Length != 0)
                    {
                        date_order_ot.SelectedDate = DateTime.Parse(str[2]);
                    }

                    if ((str[3].Length != 0) && (str[4].Length != 0))
                    {
                        tex_box_predist_doc1.Text = str[3];
                        date_dover_ot_3.SelectedDate = DateTime.Parse(str[4]);
                    }
                    tex_box_predist_doc4.Text = str[5];
                    tex_box_predist_doc4_2.Text = str[6];
                }
            }
            StackPanel stp_doc1_predist = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Width = 560 };
            stp_doc1_predist.Children.Add(label_doc1_predist);
            stp_doc1_predist.Children.Add(date_dover_ot);
            stp_doc1_predist.Children.Add(label_doc1_2_predist);
            stp_doc1_predist.Children.Add(date_dover_ot_2);

            StackPanel stp_doc2_predist = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Width = 560 };
            stp_doc2_predist.Children.Add(label_doc2_predist);
            stp_doc2_predist.Children.Add(date_order_ot);

            StackPanel stp_doc3_predist = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Width = 560 };
            stp_doc3_predist.Children.Add(label_doc3_predist);
            stp_doc3_predist.Children.Add(tex_box_predist_doc1);

            StackPanel stp_doc3_2_predist = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Width = 560 };
            stp_doc3_2_predist.Children.Add(label_doc3_2_predist);
            stp_doc3_2_predist.Children.Add(date_dover_ot_3);

            StackPanel stp_doc4_predist = new StackPanel() { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Left, Width = 560 };
            stp_doc4_predist.Children.Add(label_doc4_predist);
            stp_doc4_predist.Children.Add(tex_box_predist_doc4);

            StackPanel stp_doc4_2_predist = new StackPanel() { Orientation = Orientation.Vertical, HorizontalAlignment = HorizontalAlignment.Left, Width = 560 };
            stp_doc4_2_predist.Children.Add(label_doc4_2_predist);
            stp_doc4_2_predist.Children.Add(tex_box_predist_doc4_2);

            //tex_box_ist_doc1.Height = 25;
            TextBlock label_ist2 = new TextBlock() { Text = "извещенный надлежайшим образом: документ, подтверждающий извещение:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, TextWrapping = TextWrapping.Wrap };
            TextBox tex_box_ist_doc2 = new TextBox();
            tex_box_ist_doc2.Padding = new Thickness(1, 1, 1, 1);
            tex_box_ist_doc2.TextWrapping = TextWrapping.Wrap;
            tex_box_ist_doc2.FontSize = 16;
            tex_box_ist_doc2.AcceptsReturn = true;
            tex_box_ist_doc2.Foreground = colortext;
            if (reader_pred_ist != null)
                if (!reader_pred_ist.IsDBNull(3))
                {
                    string str = reader_pred_ist.GetString(3);
                    //char[] str1 = null;
                    string[] arr = str.Split('~');
                    //int index = str.IndexOf("///");
                    if (arr[0] != "")
                    {
                        //str1 = str.Substring(0, index - 1);
                        //str.CopyTo(0, str1, 0, index - 1);
                        tex_box_predist_doc1.Text = arr[0];
                    }
                    // str.CopyTo(0, str1, index + 2, str.Length - index + 2);
                    tex_box_ist_doc2.Text = arr[1];
                }
            Grid DynamicGrid = new Grid();
            DynamicGrid.Width = 570;
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
            RowDefinition gridRow8 = new RowDefinition();
            RowDefinition gridRow9 = new RowDefinition();
            RowDefinition gridRow10 = new RowDefinition();
            gridRow10.Height = new GridLength(15);
            Rectangle rec = new Rectangle() { Fill = colortext, Height = 1 };
            rec.Margin = new Thickness(0, 7, 0, 7);
            Grid.SetColumnSpan(rec, 3);
            DynamicGrid.RowDefinitions.Add(gridRow1);
            DynamicGrid.RowDefinitions.Add(gridRow2);
            DynamicGrid.RowDefinitions.Add(gridRow3);
            DynamicGrid.RowDefinitions.Add(gridRow4);
            DynamicGrid.RowDefinitions.Add(gridRow5);
            DynamicGrid.RowDefinitions.Add(gridRow6);
            DynamicGrid.RowDefinitions.Add(gridRow7);
            DynamicGrid.RowDefinitions.Add(gridRow8);
            DynamicGrid.RowDefinitions.Add(gridRow9);
            DynamicGrid.RowDefinitions.Add(gridRow10);
            //StackPanel_A_2_2.Children.Add(grid);
            // StackPanel_A_2_2.Children.Add(grid);

            Grid.SetRow(label_predist_name, 0);
            Grid.SetColumn(label_predist_name, 1);
            DynamicGrid.Children.Add(label_predist_name);
            Grid.SetRow(label_predist, 1);
            Grid.SetColumn(label_predist, 0);
            DynamicGrid.Children.Add(label_predist);
            Grid.SetRow(textbox_predist_name, 1);
            Grid.SetColumn(textbox_predist_name, 1);
            DynamicGrid.Children.Add(textbox_predist_name);
            Grid.SetRow(btn, 1);
            Grid.SetColumn(btn, 2);
            DynamicGrid.Children.Add(btn);

            Grid.SetRow(label_doc_predist, 2);
            Grid.SetColumn(label_doc_predist, 1);
            DynamicGrid.Children.Add(label_doc_predist);

            Grid.SetColumnSpan(stp_doc1_predist, 3);
            Grid.SetRow(stp_doc1_predist, 3);
            Grid.SetColumn(stp_doc1_predist, 0);
            DynamicGrid.Children.Add(stp_doc1_predist);

            Grid.SetColumnSpan(stp_doc2_predist, 3);
            Grid.SetRow(stp_doc2_predist, 4);
            Grid.SetColumn(stp_doc2_predist, 0);
            DynamicGrid.Children.Add(stp_doc2_predist);

            
            Grid.SetColumnSpan(stp_doc3_predist, 3);
            Grid.SetRow(stp_doc3_predist, 5);
            Grid.SetColumn(stp_doc3_predist, 0);
            DynamicGrid.Children.Add(stp_doc3_predist);

            Grid.SetColumnSpan(stp_doc3_2_predist, 3);
            Grid.SetRow(stp_doc3_2_predist, 6);
            Grid.SetColumn(stp_doc3_2_predist, 0);
            DynamicGrid.Children.Add(stp_doc3_2_predist);

            Grid.SetColumnSpan(stp_doc4_predist, 3);
            Grid.SetRow(stp_doc4_predist, 7);
            Grid.SetColumn(stp_doc4_predist, 0);
            DynamicGrid.Children.Add(stp_doc4_predist);

            Grid.SetColumnSpan(stp_doc4_2_predist, 3);
            Grid.SetRow(stp_doc4_2_predist, 8);
            Grid.SetColumn(stp_doc4_2_predist, 0);
            DynamicGrid.Children.Add(stp_doc4_2_predist);

            Grid.SetRow(rec, 9);
            Grid.SetColumn(rec, 0);
            DynamicGrid.Children.Add(rec);
            return DynamicGrid;
        }

        private void deletePredIst(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            StackPanel parentStack = (StackPanel)grid.Parent;
            var grids = parentStack.Children;
            grids.Remove(grid);
            var textbox_name = grid.Children[2] as TextBox;
            SQLite connection = new SQLite();
            connection.WriteData(string.Format("delete from Actors Where id='{0}'", textbox_name.Tag));
        }

        //обновляем/заполняем таблицу с истцами
        private void UpdateA_2()
        {
            SQLite connection = new SQLite();
            //SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from ACTOR Where id_doc='{0}'", id));
            SQLiteDataReader reader_ist = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}'  and PLAINTIFF=1", id));
            while (reader_ist.Read())
                ind = reader_ist.GetInt16(0);
            reader_ist = connection.ReadData(string.Format("Select * from Actors Where id_doc ='{0}' and PLAINTIFF=1", id));
            first = false;
            StackPanel_A_2_1.Visibility = Visibility.Visible;
            Scroll_A_2_2.Visibility = Visibility.Visible;
            StackPanel_A_2_2.Visibility = Visibility.Visible;
            StackPanel_A_2_2.Children.Clear();
            if (ind == 0)
            {
                AddStPanelIstec1(null);
                first = true;
            }
            else
            {
                while (reader_ist.Read())
                {
                    AddStPanelIstec1(reader_ist);
                    first = true;
                }
            }
            connection.Close();
        }

        //обновляем/заполняем таблицу с представителями истцов
        private void UpdateA_3()
        {
            SQLite connection = new SQLite();
            //SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from ACTOR Where id_doc='{0}'", id));
            SQLiteDataReader reader_ist = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}'  and PLAINTIFF=1", id));
            while (reader_ist.Read())
                ind = reader_ist.GetInt16(0);
            reader_ist = connection.ReadData(string.Format("Select * from Actors Where id_doc ='{0}' and PLAINTIFF=1", id));
            first = false;
            StackPanel_A_2_1.Visibility = Visibility.Visible;
            Scroll_A_2_2.Visibility = Visibility.Visible;
            StackPanel_A_2_2.Visibility = Visibility.Visible;
            StackPanel_A_2_2.Children.Clear();
            if (ind == 0)
            {
                AddStPanelIstec1(null);
                first = true;
            }
            else
            {
                while (reader_ist.Read())
                {
                    AddStPanelIstec1(reader_ist);
                    first = true;
                }
            }
            connection.Close();
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
            SolidColorBrush colortext2 = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEA2727"));
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.Margin = new Thickness(0, 0, 0, 10);

            Label label_ist = new Label() { Content = string.Concat("•"), FontSize = 20, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_ist_name = new TextBox();
            textbox_ist_name.Padding = new Thickness(1,1,1,1);
            textbox_ist_name.TextWrapping = TextWrapping.Wrap;
            textbox_ist_name.FontSize = 16;
            textbox_ist_name.AcceptsReturn = false;
            textbox_ist_name.Foreground = colortext;
            textbox_ist_name.Height = 25;

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

            TextBlock label_ist_document = new TextBlock() { Text = "Если истец участвует в деле документы НЕ ЗАПОЛНЯЮТСЯ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext2, TextWrapping = TextWrapping.Wrap };

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
                        tex_box_ist_doc1.Text = arr[0];
                    }
                    if (arr[1] != "")
                    {
                        tex_box_ist_doc1.Text = arr[1];
                    }
                    // str.CopyTo(0, str1, index + 2, str.Length - index + 2);
                    
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
            RowDefinition gridRow6_2 = new RowDefinition();
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
            DynamicGrid.RowDefinitions.Add(gridRow6_2);
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

            Grid.SetColumnSpan(label_ist_document, 3);
            Grid.SetRow(label_ist_document, 2);
            Grid.SetColumn(label_ist_document, 0);
            DynamicGrid.Children.Add(label_ist_document);

            Grid.SetRow(label_ist1, 3);
            Grid.SetColumn(label_ist1, 1);
            DynamicGrid.Children.Add(label_ist1);
            Grid.SetRow(tex_box_ist_doc1, 4);
            Grid.SetColumn(tex_box_ist_doc1, 1);
            DynamicGrid.Children.Add(tex_box_ist_doc1);
            Grid.SetRow(label_ist2, 5);
            Grid.SetColumn(label_ist2, 1);
            DynamicGrid.Children.Add(label_ist2);
            Grid.SetRow(tex_box_ist_doc2, 6);
            Grid.SetColumn(tex_box_ist_doc2, 1);
            DynamicGrid.Children.Add(tex_box_ist_doc2);
            Grid.SetRow(rec, 7);
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
        

        private void Button_Next_Click(object sender, RoutedEventArgs e)
        {
            listBox.SelectedIndex++;
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
            string str;
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
                            var textbox_doc1 = grid.Children[6] as TextBox;
                            var textbox_doc2 = grid.Children[8] as TextBox;

                            str = textbox_doc1.Text + "~" + textbox_doc2.Text;
                            // reader_ist = connection.ReadData(string.Format("Select count(*) from ACTOR Where id='{0}'", textbox_name.Tag));
                            //while (reader_ist.Read())
                            //   ind = reader_ist.GetInt16(0);
                            if (textbox_name.Tag == null)
                                connection.WriteData(string.Format("INSERT INTO actors (id_doc,name_actor,actor_doc,plaintiff) VALUES ('{0}','{1}','{2}','{3}')", id, textbox_name.Text, str, 1));
                            else
                            {
                                SQLite connection1 = new SQLite();
                                connection1.WriteData(string.Format("Update actors set id_doc='{0}', name_actor='{1}', actor_doc='{2}' Where id='{3}'", id, textbox_name.Text, str, textbox_name.Tag));
                                connection1.Close();
                            }
                        }
                        UpdateA_2();
                        break;

                    case 2:
                        foreach (TabItem tbItem in Tab_Presd_ist.Items )
                        {
                            var id_ag = (TextBlock)tbItem.Tag;
                            StackPanel st = (StackPanel)tbItem.Content;
                            Grid grid_2 = (Grid)st.Children[1];
                            var textbox_name = grid_2.Children[2] as TextBox;
                            var actor_name = textbox_name.Text;
                            if (actor_name.Length != 0)
                            {
                                var stp_doc1 = grid_2.Children[5] as StackPanel;
                                var str_doc = stp_doc1.Children[1].ToString() + "~" + stp_doc1.Children[3].ToString();
                                var stp_doc2 = grid_2.Children[6] as StackPanel;
                                str_doc = str_doc + "~" + stp_doc2.Children[1].ToString();
                                var stp_doc3 = grid_2.Children[7] as StackPanel;
                                var tb_doc3 = stp_doc3.Children[1] as TextBox;
                                str_doc = str_doc + "~" + tb_doc3.Text.ToString();
                                var stp_doc3_2 = grid_2.Children[8] as StackPanel;
                                str_doc = str_doc + "~" + stp_doc3_2.Children[1].ToString();
                                var stp_doc4 = grid_2.Children[9] as StackPanel;
                                var tb_doc4 = stp_doc4.Children[1] as TextBox;
                                str_doc = str_doc + "~" + tb_doc4.Text.ToString();
                                var stp_doc4_2 = grid_2.Children[10] as StackPanel;
                                var tb_doc4_2 = stp_doc4_2.Children[1] as TextBox;
                                str_doc = str_doc + "~" + tb_doc4_2.Text.ToString();
                                if (textbox_name.Tag == null)
                                    connection.WriteData(string.Format("INSERT INTO AGENT_PLAINTIFF (ID_ACTOR,NAME_AGENT , AGENT_DOC) VALUES ('{0}','{1}','{2}')", id_ag.Text, actor_name, str_doc));
                                else
                                {
                                    SQLite connection1 = new SQLite();
                                    connection1.WriteData(string.Format("Update AGENT_PLAINTIFF set NAME_AGENT='{0}',  AGENT_DOC='{1}' Where ID_PLAINTIFF='{2}'", actor_name, str_doc, textbox_name.Tag));
                                    connection1.Close();
                                }
                            }
                        }
                        TabControlPredIst();
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
