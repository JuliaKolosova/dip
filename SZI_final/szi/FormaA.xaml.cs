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
        private string name;
        private readonly ForaWindow fw;
        private bool first;
        private Grid grid1;
        int id;
        int ind, id_req;
        string delete_str;
        private SolidColorBrush colortext = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF378B1E"));
        private SolidColorBrush color_border = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDBFFD0"));

        public FormaA(string name, ForaWindow fw, int id)
        {
            this.name = name;
            InitializeComponent();
            this.fw = fw;
            this.id = id;
            delete_str = "";
            FormaA2.Title = " «" + name + "»";
            toFillTreeView();
            //toFillTextBoxesFormaA_list1();
            addImg.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));
        }


        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grids = StackPanel.Children;
            for (int i=0;i< grids.Count;i++)
            {
                var grid = grids[i];
                grid.Visibility = Visibility.Collapsed;
            }
            delete_str = "";

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
                        UpdateA_2(1);
                        break;
                    case 2:
                        Button_Next.IsEnabled = true;
                        StackPanel_A_3_1.Visibility = Visibility.Visible;
                        StackPanel_A_3_2.Visibility = Visibility.Visible;
                        TabControlPredIst(-1,1);
                        break;
                    case 3:
                        Button_Next.IsEnabled = true;
                        UpdateA_2(2);
                        break;
                    case 4:
                        Button_Next.IsEnabled = true;
                        StackPanel_A_3_1.Visibility = Visibility.Visible;
                        StackPanel_A_3_2.Visibility = Visibility.Visible;
                        TabControlPredIst(-1,2);
                        break;
                    case 5:
                        StackPanel_A_4.Visibility = Visibility.Visible;
                        WriteIstecOtvet();
                        Button_Next.IsEnabled = false;
                        break;
                
            }
        }

        //заполнить истцами и ответчиками текстблок
        private void WriteIstecOtvet()
        {
            Image img_add = new Image();
            img_add.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));
            img_add.Width = 20;
            img_add.Height = 20;
            AddBtn_FM.Content = img_add;

            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}' group by PLAINTIFF", id));
            int[] count= new int[2];
            //int count_otv = 0;
            tblock_perech.Text = "";
            int i = 0;
            while (reader.Read())
            {
                count[i] = reader.GetInt16(0);
                i++;
            }
            if (count[0] == 0 && count[1] == 0)
            {
                tblock_perech.Text = "Пожалуйста, введите информацию об истацах и ответчиках";
            }
            else if (count[1] == 0)
            {
                tblock_perech.Text = "Пожалуйста, введите информацию об ответчиках";
            }
            else if (count[0] == 0)
            {
                tblock_perech.Text = "Пожалуйста, введите информацию об истацах";
            }
            else
            {
                string str_ist="", str_otv="";
                reader = connection.ReadData(string.Format("Select * from Actors Where id_doc ='{0}' order by PLAINTIFF", id));
                while (reader.Read())
                {
                    if (reader.GetInt16(4)==1)
                        str_ist += reader.GetString(2)+','+'\n';
                    else
                        str_otv += reader.GetString(2) + ',' + '\n';

                    //count_otv = reader.GetInt16(1);
                }
                str_ist=str_ist.Remove(str_ist.Length - 2);
                str_otv=str_otv.Remove(str_otv.Length - 2);
                tblock_perech.Text = str_ist + '\n' + "к" + '\n' + str_otv;
            }
            reader = connection.ReadData(string.Format("Select PROSECUTOR,PUBLIC_MEETING from document Where id ='{0}'", id));
            while (reader.Read())
            {
                if(!reader.IsDBNull(0))
                    tB_PROKUROR.Text = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    if (reader.GetString(1) == "True")
                        rb_otcr.IsChecked = true;
                    else
                        rb_zacr.IsChecked = true;
                
            }

            FillReq();

            connection.Close();
        }

        private void FillReq()
        {
            StackPanel_A_4_2.Children.Clear();
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_TMP Where id_doc ='{0}' and iteration=0", id));
            int count_req = 0;
            while (reader.Read())
            {
                count_req = reader.GetInt16(0);

            }
            if (count_req != 0)
            {
                reader = connection.ReadData(string.Format("Select id_req,text from REQUIREMENTS_TMP Where id_doc ='{0}' and iteration=0", id));
                var first_req = true;
                while (reader.Read())
                {
                    if (first_req)
                    {
                        tB_formulirovka.Tag = reader.GetInt16(0).ToString();
                        tB_formulirovka.Text = reader.GetString(1);
                        first_req = false;
                    }
                    else
                    {
                        Update_formul(reader);
                    }

                }
            }
            connection.Close();
        }

        //заполняем tabcontrol  о представителях истцов
        private void TabControlPredIst(int index, int plaintiff)
        {
            bool firstpred;
            SQLite connection = new SQLite();
            SQLiteDataReader reader_ist, reader_pred_ist;
            int count=0, count_reader_ist_i=0;
            int ist_id = 0;
            if (plaintiff==1)
            {
                label_pred_ist.Content = "     Информация о представителях истцов:";
                label_pred_ist2.Content = "Введите информацию об истцах!";
            }
            else
            {
                label_pred_ist.Content = "     Информация о представителях ответчиков:";
                label_pred_ist2.Content = "Введите информацию об ответчиках!";
            }
            reader_ist = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}'  and PLAINTIFF={1}", id,plaintiff));
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
                reader_pred_ist = connection.ReadData(string.Format("Select ID, NAME_ACTOR from Actors Where id_doc ='{0}' and PLAINTIFF={1}", id, plaintiff));

                
                while (reader_pred_ist.Read())
                {
                    firstpred = true;
                    ist_id = reader_pred_ist.GetInt16(0);
                    StackPanel pred_ist_panel = new StackPanel(); ;
                    //pred_ist_panel.Children.Clear();
                    SQLiteDataReader count_reader_ist = connection.ReadData(string.Format("Select count(*) from AGENT_PLAINTIFF Where id_actor='{0}'", ist_id));
                    while (count_reader_ist.Read())
                         count_reader_ist_i = count_reader_ist.GetInt16(0);

                    pred_ist_panel.Children.Add(CreateHeaderPredIst(reader_pred_ist,plaintiff));
                    ScrollViewer skroll = new ScrollViewer();
                    
                    if (count_reader_ist_i == 0)
                    {
                        pred_ist_panel.Children.Add(AddStPanelPredIstec(null, firstpred, plaintiff));
                        firstpred = false;
                        skroll.Content = pred_ist_panel;
                        Tab_Presd_ist.Items.Add(new TabItem
                        {
                            Header = new TextBlock { Text = reader_pred_ist.GetString(1).Split(' ')[0] },
                            Tag = new TextBlock { Text = ist_id.ToString() },
                            Content = skroll
                        });
                    }
                    else
                    {                        
                        SQLiteDataReader reader_pred_ist2 = connection.ReadData(string.Format("Select ID_PLAINTIFF, ID_ACTOR, NAME_AGENT, AGENT_DOC, NAME_AGENT_RP from AGENT_PLAINTIFF Where id_actor ='{0}'", ist_id));
                        //pred_ist_panel.Children.Add(AddStPanelPredIstec(reader_pred_ist2, firstpred));
                        firstpred = false;
                        while (reader_pred_ist2.Read())
                        {
                            pred_ist_panel.Children.Add(AddStPanelPredIstec(reader_pred_ist2, firstpred, plaintiff));
                            // тут нужно как-то динамично считывть информацию. может быть несколько представителей у одного. возможность добавления/удаления :(
                        }
                        skroll.Content = pred_ist_panel;
                        Tab_Presd_ist.Items.Add(new TabItem
                        {
                            Header = new TextBlock { Text = reader_pred_ist.GetString(1) },
                            Tag = new TextBlock { Text = ist_id.ToString() },
                            Content = skroll

                        });
                       
                    }
                }
            }

            connection.Close();
            if (index != -1)
            {
                Tab_Presd_ist.SelectedIndex = index;
            }
            else
            {
                Tab_Presd_ist.SelectedIndex = 0;
            }

        }


        public Grid CreateHeaderPredIst(SQLiteDataReader reader_pred_ist, int  plaintiff)
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


            Label label_info_pred_ist = new Label() {  FontSize = 18, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, Height = 31, Width = 403 };
            if (plaintiff==1)
            {
                label_info_pred_ist.Content = string.Concat("Представители истца: ", reader_pred_ist.GetString(1));
            }
            else
            {
                label_info_pred_ist.Content = string.Concat("Представители ответчика: ", reader_pred_ist.GetString(1));
            }
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
            if (listBox.SelectedIndex == 1)
                stackParent.Children.Add(AddStPanelPredIstec(null, false,1)); 
            else
                stackParent.Children.Add(AddStPanelPredIstec(null, false,2)); 
            
        }

        private Grid AddStPanelPredIstec(SQLiteDataReader reader_pred_ist, bool firstpred, int plaintiff)
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
            textbox_predist_name.Height = 25;
            textbox_predist_name.BorderBrush = color_border;

            TextBox textbox_predist_name_rp = new TextBox();
            textbox_predist_name_rp.Padding = new Thickness(1, 1, 1, 1);
            textbox_predist_name_rp.TextWrapping = TextWrapping.Wrap;
            textbox_predist_name_rp.FontSize = 16;
            textbox_predist_name_rp.AcceptsReturn = false;
            textbox_predist_name_rp.Foreground = colortext;
            textbox_predist_name_rp.Height = 25;
            textbox_predist_name_rp.BorderBrush = color_border;

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

            Label label_predist_name_rp = new Label() { Content = "ФИО представителя (в родительном падеже):", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            Label label_doc_predist = new Label() { Content = "Действующего на основании:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            Label label_doc1_predist = new Label() { Content = "  - доверенности от ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            DatePicker date_dover_ot = new DatePicker( ) { Width=120 };
            //date_dover_ot.SelectedDateChanged += new EventHandler<SelectionChangedEventArgs>(selectdate);

            Label label_doc1_2_predist = new Label() { Content = " со сроком действия до ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            DatePicker date_dover_ot_2 = new DatePicker() { Width = 120 };

            Label label_doc2_predist = new Label() { Content = "  - ордера адвоката от ", FontSize = 16, Margin = new Thickness(0, 10, 0, 10), Foreground = colortext };

            DatePicker date_order_ot = new DatePicker() { Margin = new Thickness(0, 10, 0, 10), Width = 120 };

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

            DatePicker date_dover_ot_3 = new DatePicker() { Width = 120 };

            Label label_doc4_predist = new Label() { Content = "  - наименование документа, ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox tex_box_predist_doc4 = new TextBox();
            tex_box_predist_doc4.Padding = new Thickness(1, 1, 1, 1);
            tex_box_predist_doc4.TextWrapping = TextWrapping.Wrap;
            tex_box_predist_doc4.FontSize = 16;
            tex_box_predist_doc4.AcceptsReturn = true;
            tex_box_predist_doc4.Foreground = colortext;
            tex_box_predist_doc4.Width = 280;
            tex_box_predist_doc4.Height = 25;
            tex_box_predist_doc4.BorderBrush = color_border;

            Label label_doc4_2_predist = new Label() { Content = ", удостоверяющего статус и полномочия законного представителя в силу ст.", FontSize = 16, Margin = new Thickness(0, 1, 1, 0), Foreground = colortext };

            TextBox tex_box_predist_doc4_2 = new TextBox();
            tex_box_predist_doc4_2.Padding = new Thickness(1, 1, 1, 1);
            tex_box_predist_doc4_2.TextWrapping = TextWrapping.Wrap;
            tex_box_predist_doc4_2.FontSize = 16;
            tex_box_predist_doc4_2.AcceptsReturn = true;
            tex_box_predist_doc4_2.Foreground = colortext;
            tex_box_predist_doc4_2.Width = 280;
            tex_box_predist_doc4_2.Height = 25;
            tex_box_predist_doc4_2.BorderBrush = color_border;
            if (reader_pred_ist != null)
            {
                textbox_predist_name.Text = reader_pred_ist.GetString(2);
                textbox_predist_name.Tag = reader_pred_ist.GetInt16(0).ToString();
                textbox_predist_name_rp.Text = reader_pred_ist.GetString(4);

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
            RowDefinition gridRow11 = new RowDefinition();
            RowDefinition gridRow12 = new RowDefinition();
            gridRow12.Height = new GridLength(15);
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
            DynamicGrid.RowDefinitions.Add(gridRow11);
            DynamicGrid.RowDefinitions.Add(gridRow12);
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

            Grid.SetRow(label_predist_name_rp, 2);
            Grid.SetColumn(label_predist_name_rp, 1);
            DynamicGrid.Children.Add(label_predist_name_rp);
            Grid.SetRow(textbox_predist_name_rp, 3);
            Grid.SetColumn(textbox_predist_name_rp, 1);
            DynamicGrid.Children.Add(textbox_predist_name_rp);

            Grid.SetRow(label_doc_predist, 4);
            Grid.SetColumn(label_doc_predist, 1);
            DynamicGrid.Children.Add(label_doc_predist);

            Grid.SetColumnSpan(stp_doc1_predist, 5);
            Grid.SetRow(stp_doc1_predist, 5);
            Grid.SetColumn(stp_doc1_predist, 0);
            DynamicGrid.Children.Add(stp_doc1_predist);

            Grid.SetColumnSpan(stp_doc2_predist, 3);
            Grid.SetRow(stp_doc2_predist, 6);
            Grid.SetColumn(stp_doc2_predist, 0);
            DynamicGrid.Children.Add(stp_doc2_predist);

            
            Grid.SetColumnSpan(stp_doc3_predist, 3);
            Grid.SetRow(stp_doc3_predist, 7);
            Grid.SetColumn(stp_doc3_predist, 0);
            DynamicGrid.Children.Add(stp_doc3_predist);

            Grid.SetColumnSpan(stp_doc3_2_predist, 3);
            Grid.SetRow(stp_doc3_2_predist, 8);
            Grid.SetColumn(stp_doc3_2_predist, 0);
            DynamicGrid.Children.Add(stp_doc3_2_predist);

            Grid.SetColumnSpan(stp_doc4_predist, 3);
            Grid.SetRow(stp_doc4_predist, 9);
            Grid.SetColumn(stp_doc4_predist, 0);
            DynamicGrid.Children.Add(stp_doc4_predist);

            Grid.SetColumnSpan(stp_doc4_2_predist, 3);
            Grid.SetRow(stp_doc4_2_predist, 10);
            Grid.SetColumn(stp_doc4_2_predist, 0);
            DynamicGrid.Children.Add(stp_doc4_2_predist);

            Grid.SetRow(rec, 11);
            Grid.SetColumn(rec, 0);
            DynamicGrid.Children.Add(rec);
            return DynamicGrid;
        }
        private void selectdate (object sender, EventArgs date )
        {

        }
        private void deletePredIst(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            StackPanel parentStack = (StackPanel)grid.Parent;
            var grids = parentStack.Children;
            grids.Remove(grid);
            var textbox_name = grid.Children[2] as TextBox;
            if (textbox_name.Tag != null)
                delete_str += textbox_name.Tag.ToString()+";";
            //SQLite connection = new SQLite();
            //connection.WriteData(string.Format("delete from Actors Where id='{0}'", textbox_name.Tag));
        }

        //обновляем/заполняем таблицу с истцами
        private void UpdateA_2(int plaintiff)
        {
            SQLite connection = new SQLite();
            //SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from ACTOR Where id_doc='{0}'", id));
            SQLiteDataReader reader_ist = connection.ReadData(string.Format("Select count(*) from ACTORs Where id_doc='{0}'  and PLAINTIFF={1}", id,plaintiff));
            while (reader_ist.Read())
                ind = reader_ist.GetInt16(0);
            reader_ist = connection.ReadData(string.Format("Select * from Actors Where id_doc ='{0}' and PLAINTIFF={1}", id, plaintiff));
            first = false;
            StackPanel_A_2_1.Visibility = Visibility.Visible;
            if (plaintiff==1)
            {
                label_Copy11.Content = "     Информация об истцах:";
            }
            else
            {
                label_Copy11.Content = "     Информация об ответчиках:";
            }
            Scroll_A_2_2.Visibility = Visibility.Visible;
            StackPanel_A_2_2.Visibility = Visibility.Visible;
            StackPanel_A_2_2.Children.Clear();
            if (ind == 0)
            {
                AddStPanelIstec1(null,plaintiff);
                first = true;
            }
            else
            {
                while (reader_ist.Read())
                {
                    AddStPanelIstec1(reader_ist, plaintiff);
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
            textbox.BorderBrush = color_border;
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
            textbox1.BorderBrush = color_border;

            TextBlock label2 = new TextBlock() { Text = "извещенный надлежайшим образом: документ, подтверждающий извещение:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, TextWrapping = TextWrapping.Wrap };
            TextBox textbox2 = new TextBox();
            textbox2.Padding = new Thickness(1, 1, 1, 1);
            textbox2.TextWrapping = TextWrapping.Wrap;
            textbox2.FontSize = 16;
            textbox2.AcceptsReturn = true;
            textbox2.Foreground = colortext;
            textbox2.BorderBrush = color_border;

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
        private void AddStPanelIstec1(SQLiteDataReader reader_ist, int plaintiff)
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
            textbox_ist_name.BorderBrush = color_border;

            Label label_ist_rp = new Label() { Content = string.Concat("ФИО в Родительном падеже:"), FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_ist_name_rp = new TextBox();
            textbox_ist_name_rp.Padding = new Thickness(1, 1, 1, 1);
            textbox_ist_name_rp.TextWrapping = TextWrapping.Wrap;
            textbox_ist_name_rp.FontSize = 16;
            textbox_ist_name_rp.AcceptsReturn = false;
            textbox_ist_name_rp.Foreground = colortext;
            textbox_ist_name_rp.Height = 25;
            textbox_ist_name_rp.BorderBrush = color_border;

            if (reader_ist != null)
            {
                textbox_ist_name.Text = reader_ist.GetString(2);
                textbox_ist_name_rp.Text = reader_ist.GetString(3);
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

            TextBlock label_ist_document = new TextBlock() {  FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext2, TextWrapping = TextWrapping.Wrap };
            if (plaintiff==1)
            {
                label_ist_document.Text = "Если истец участвует в деле документы НЕ ЗАПОЛНЯЮТСЯ";
            }
            else
            {
                label_ist_document.Text = "Если ответчик участвует в деле документы НЕ ЗАПОЛНЯЮТСЯ";
            }
            TextBlock label_ist1 = new TextBlock() { Text = "просивший рассмотреть дело в его отсутствие: заявление от", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext , TextWrapping = TextWrapping.Wrap };
            TextBox tex_box_ist_doc1 = new TextBox();
            tex_box_ist_doc1.Padding = new Thickness(1, 1, 1, 1);
            tex_box_ist_doc1.TextWrapping = TextWrapping.Wrap;
            tex_box_ist_doc1.FontSize = 16;
            tex_box_ist_doc1.AcceptsReturn = true;
            tex_box_ist_doc1.Foreground = colortext;
            tex_box_ist_doc1.BorderBrush = color_border;
            //tex_box_ist_doc1.Height = 25;
            TextBlock label_ist2 = new TextBlock() { Text = "извещенный надлежайшим образом: документ, подтверждающий извещение:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext, TextWrapping = TextWrapping.Wrap };
            TextBox tex_box_ist_doc2 = new TextBox();
            tex_box_ist_doc2.Padding = new Thickness(1, 1, 1, 1);
            tex_box_ist_doc2.TextWrapping = TextWrapping.Wrap;
            tex_box_ist_doc2.FontSize = 16;
            tex_box_ist_doc2.AcceptsReturn = true;
            tex_box_ist_doc2.Foreground = colortext;
            tex_box_ist_doc2.BorderBrush = color_border;


            if (reader_ist != null)
                if (!reader_ist.IsDBNull(4))
                {
                    string str = reader_ist.GetString(4);
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
            RowDefinition gridRow2_2 = new RowDefinition();
            RowDefinition gridRow2_3 = new RowDefinition();
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
            DynamicGrid.RowDefinitions.Add(gridRow2_2);
            DynamicGrid.RowDefinitions.Add(gridRow2_3);
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

            Grid.SetRow(label_ist_rp, 2);
            Grid.SetColumn(label_ist_rp, 1);
            DynamicGrid.Children.Add(label_ist_rp);
            Grid.SetRow(textbox_ist_name_rp, 3);
            Grid.SetColumn(textbox_ist_name_rp, 1);
            DynamicGrid.Children.Add(textbox_ist_name_rp);

            Grid.SetColumnSpan(label_ist_document, 3);
            Grid.SetRow(label_ist_document, 4);
            Grid.SetColumn(label_ist_document, 0);
            DynamicGrid.Children.Add(label_ist_document);

            Grid.SetRow(label_ist1, 5);
            Grid.SetColumn(label_ist1, 1);
            DynamicGrid.Children.Add(label_ist1);
            Grid.SetRow(tex_box_ist_doc1, 6);
            Grid.SetColumn(tex_box_ist_doc1, 1);
            DynamicGrid.Children.Add(tex_box_ist_doc1);
            Grid.SetRow(label_ist2, 7);
            Grid.SetColumn(label_ist2, 1);
            DynamicGrid.Children.Add(label_ist2);
            Grid.SetRow(tex_box_ist_doc2, 8);
            Grid.SetColumn(tex_box_ist_doc2, 1);
            DynamicGrid.Children.Add(tex_box_ist_doc2);
            Grid.SetRow(rec, 9);
            Grid.SetColumn(rec, 0);
            DynamicGrid.Children.Add(rec);
            StackPanel_A_2_2.Children.Add(DynamicGrid);
            //StackPanel.Children.Add(DynamicGrid);
        }

        public void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listBox.SelectedIndex==1)
                AddStPanelIstec1(null,1);
            else
                AddStPanelIstec1(null, 2);
        }
        /* удаление текстбокса с формы */
        private void deleteTextBox(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            var grids = StackPanel_A_2_2.Children;
            grids.Remove(grid);
            var textbox_name = grid.Children[2] as TextBox;
            if (textbox_name.Tag != null)
                delete_str += textbox_name.Tag.ToString() + ";";
            //SQLite connection = new SQLite();
            //connection.WriteData(string.Format("delete from Actors Where id='{0}'", textbox_name.Tag));
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
        private void toFillTreeView()
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

        /* сохранение данных */
        private void SaveData()
        {
            bool isFull = false;
            SQLite connection = new SQLite();
            SQLiteDataReader reader_ist;
            string str;

                switch (listBox.SelectedIndex)
                {
                    case 0:
                        connection.WriteData(string.Format("Update Document set number='{0}', date='{1}', locate='{2}', NAME_COURT='{3}', CONTENT_COURT='{4}', SECRETARY='{5}'  Where id='{6}'", TBNumber_Copy.Text, Date_Copy.SelectedDate, TBPlace_Copy.Text, TBName_Copy.Text,TBSostav_Copy.Text,TBSecretary_Copy.Text, id));
                        break;
                    case 1:

                        foreach (Grid grid in StackPanel_A_2_2.Children)
                        {
                            var textbox_name = grid.Children[2] as TextBox;
                            var textbox_name_rp = grid.Children[5] as TextBox;
                            var textbox_doc1 = grid.Children[8] as TextBox;
                            var textbox_doc2 = grid.Children[10] as TextBox;

                            str = textbox_doc1.Text + "~" + textbox_doc2.Text;
                            // reader_ist = connection.ReadData(string.Format("Select count(*) from ACTOR Where id='{0}'", textbox_name.Tag));
                            //while (reader_ist.Read())
                            //   ind = reader_ist.GetInt16(0);
                            if (textbox_name.Text.Length != 0)
                            { 
                                if (textbox_name.Tag == null)
                                    connection.WriteData(string.Format("INSERT INTO actors (id_doc,name_actor,name_actor_rp,actor_doc,plaintiff) VALUES ('{0}','{1}','{2}','{3}','{4}')", id, textbox_name.Text, textbox_name_rp.Text, str, 1));
                                else
                                {
                                    SQLite connection1 = new SQLite();
                                    connection1.WriteData(string.Format("Update actors set id_doc='{0}', name_actor='{1}',name_actor_rp='{2}', actor_doc='{3}' Where id='{4}'", id, textbox_name.Text, textbox_name_rp.Text, str, textbox_name.Tag));
                                    connection1.Close();
                                }
                            }
                        }
                        if (delete_str!="")
                        {
                            string[] arr = delete_str.Split(';');
                            for (int i=0;i<arr.Length-1;i++)
                            {
                                connection.WriteData(string.Format("delete from actors where ID='{0}'", arr[i]));
                                connection.WriteData(string.Format("delete from AGENT_PLAINTIFF where ID_ACTOR='{0}'", arr[i]));
                            }
                        }

                        UpdateA_2(1);
                        break;

                    case 2:
                        var ind_selected = Tab_Presd_ist.SelectedIndex;
                        foreach (TabItem tbItem in Tab_Presd_ist.Items )
                        {
                            var id_ag = (TextBlock)tbItem.Tag;
                            ScrollViewer skw= (ScrollViewer)tbItem.Content;
                            StackPanel st = (StackPanel)skw.Content;
                            Grid grid_2 = (Grid)st.Children[1];
                            var textbox_name = grid_2.Children[2] as TextBox;
                            var textbox_name_rp = grid_2.Children[5] as TextBox;
                            var actor_name = textbox_name.Text;
                            if (actor_name.Length != 0)
                            {
                                var stp_doc1 = grid_2.Children[7] as StackPanel;
                                var str_doc = stp_doc1.Children[1].ToString() + "~" + stp_doc1.Children[3].ToString();
                                var stp_doc2 = grid_2.Children[8] as StackPanel;
                                str_doc = str_doc + "~" + stp_doc2.Children[1].ToString();
                                var stp_doc3 = grid_2.Children[9] as StackPanel;
                                var tb_doc3 = stp_doc3.Children[1] as TextBox;
                                str_doc = str_doc + "~" + tb_doc3.Text.ToString();
                                var stp_doc3_2 = grid_2.Children[10] as StackPanel;
                                str_doc = str_doc + "~" + stp_doc3_2.Children[1].ToString();
                                var stp_doc4 = grid_2.Children[11] as StackPanel;
                                var tb_doc4 = stp_doc4.Children[1] as TextBox;
                                str_doc = str_doc + "~" + tb_doc4.Text.ToString();
                                var stp_doc4_2 = grid_2.Children[12] as StackPanel;
                                var tb_doc4_2 = stp_doc4_2.Children[1] as TextBox;
                                str_doc = str_doc + "~" + tb_doc4_2.Text.ToString();
                                if (textbox_name.Tag == null)
                                    connection.WriteData(string.Format("INSERT INTO AGENT_PLAINTIFF (ID_ACTOR,NAME_AGENT ,NAME_AGENT_rp , AGENT_DOC) VALUES ('{0}','{1}','{2}','{3}')", id_ag.Text, actor_name, textbox_name_rp.Text, str_doc));
                                else
                                {
                                    SQLite connection1 = new SQLite();
                                    connection1.WriteData(string.Format("Update AGENT_PLAINTIFF set NAME_AGENT='{0}', NAME_AGENT_rp='{1}',  AGENT_DOC='{2}' Where ID_PLAINTIFF='{3}'", actor_name, textbox_name_rp.Text, str_doc, textbox_name.Tag));
                                    connection1.Close();
                                }
                            }
                        }
                        if (delete_str != "")
                        {
                            string[] arr = delete_str.Split(';');
                            for (int i = 0; i < arr.Length - 1; i++)
                            {
                                connection.WriteData(string.Format("delete from  AGENT_PLAINTIFF where ID_PLAINTIFF='{0}'", arr[i]));
                            }
                        }
                        TabControlPredIst(ind_selected,1);
                        break;

                    case 3:

                        foreach (Grid grid in StackPanel_A_2_2.Children)
                        {
                            var textbox_name = grid.Children[2] as TextBox;
                            var textbox_doc1 = grid.Children[6] as TextBox;
                            var textbox_doc2 = grid.Children[8] as TextBox;

                            str = textbox_doc1.Text + "~" + textbox_doc2.Text;

                            if (textbox_name.Text.Length != 0)
                            {
                                if (textbox_name.Tag == null)
                                    connection.WriteData(string.Format("INSERT INTO actors (id_doc,name_actor,actor_doc,plaintiff) VALUES ('{0}','{1}','{2}','{3}')", id, textbox_name.Text, str, 2));
                                else
                                {
                                    SQLite connection1 = new SQLite();
                                    connection1.WriteData(string.Format("Update actors set id_doc='{0}', name_actor='{1}', actor_doc='{2}' Where id='{3}'", id, textbox_name.Text, str, textbox_name.Tag));
                                    connection1.Close();
                                }
                            }
                        }
                        UpdateA_2(2);
                        break;
                    case 4:
                        ind_selected = Tab_Presd_ist.SelectedIndex;
                        var not = true;
                        var not_ind = -1;
                        foreach (TabItem tbItem in Tab_Presd_ist.Items)
                        {
                            var id_ag = (TextBlock)tbItem.Tag;
                            StackPanel st = (StackPanel)tbItem.Content;
                            Grid grid_2 = (Grid)st.Children[1];
                            var textbox_name = grid_2.Children[2] as TextBox;
                            var actor_name = textbox_name.Text;
                            if (actor_name.Length != 0)
                            {
                                var stp_doc1 = grid_2.Children[5] as StackPanel;
                               
                                var stp_doc2 = grid_2.Children[6] as StackPanel;
                                if (DateTime.Parse(stp_doc1.Children[1].ToString())> DateTime.Parse(stp_doc1.Children[3].ToString()))                           
                                {
                                    MessageBox.Show("Сроки доверенности указаны не корректно");
                                    not = false;
                                    not_ind = Tab_Presd_ist.Items.IndexOf(tbItem);
                                    break;
                                }
                                var str_doc = stp_doc1.Children[1].ToString() + "~" + stp_doc1.Children[3].ToString();
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
                        if (not)
                            TabControlPredIst(ind_selected, 2);
                        else
                            TabControlPredIst(not_ind, 2);
                        break;
                    case 5:
                        bool zass = false;

                        reader_ist = connection.ReadData(string.Format("select max(id_req) from REQUIREMENTS_TMP Where id_doc='{0}'", id));
                            while(reader_ist.Read())                        
                                if (!reader_ist.IsDBNull(0))
                                    id_req = reader_ist.GetInt16(0)+1;
                        
                        if (rb_otcr.IsChecked == true)
                            zass = true;
                        connection.WriteData(string.Format("Update Document set PROSECUTOR='{0}', PUBLIC_MEETING='{1}' Where id='{2}'", tB_PROKUROR.Text, zass,  id));
                        if (tB_formulirovka.Text==null|| tB_formulirovka.Text.Length == 0)
                        {
                            connection.WriteData(string.Format("delete from REQUIREMENTS_TMP Where id_req='{0}'", tB_formulirovka.Tag));
                        }
                        else
                        if (tB_formulirovka.Tag==null)
                        {
                            connection.WriteData(string.Format("INSERT INTO REQUIREMENTS_TMP (ID_doc,id_req,text,iteration) VALUES ('{0}','{1}','{2}',0)", id,id_req, tB_formulirovka.Text));
                            id_req++;
                        //                        connection.WriteData(string.Format("INSERT INTO REQUIREMENTS (ID_doc,text,iteration) VALUES ('{0}','{1}',0)", id, tB_formulirovka.Text));
                        }
                        else
                        {
                            connection.WriteData(string.Format("Update REQUIREMENTS_TMP set text='{0}' where id_req='{1}' and iteration=0", tB_formulirovka.Text, tB_formulirovka.Tag));
                        }

                        foreach (Grid grid in StackPanel_A_4_2.Children)
                        {
                            var textbox_req = grid.Children[0] as TextBox;
                            if (textbox_req.Text == null || textbox_req.Text.Length == 0)
                            {
                                if (textbox_req.Tag != null)
                                    connection.WriteData(string.Format("delete from REQUIREMENTS_TMP Where id_req='{0}'", textbox_req.Tag));

                            }
                            else
                                if (textbox_req.Tag == null)
                                {
                                    connection.WriteData(string.Format("INSERT INTO REQUIREMENTS_TMP (ID_doc,id_req,text,iteration) VALUES ('{0}','{1}','{2}',0)", id, id_req, textbox_req.Text));
                                    id_req++;
                                }
                                else
                                {
                                    connection.WriteData(string.Format("Update REQUIREMENTS_TMP set text='{0}' where id_req='{1}' and iteration=0", textbox_req.Text, textbox_req.Tag));
                                }
                        }
                        if (delete_str != "")
                        {
                            string[] arr = delete_str.Split(';');
                            for (int i = 0; i < arr.Length - 1; i++)
                            {
                                connection.WriteData(string.Format("delete from REQUIREMENTS_TMP Where id_req='{0}'", arr[i]));
                            }
                        }
                        WriteIstecOtvet();
                        break;
                
            }

            connection.Close();
        }

        private void AddBtn_Click_FM(object sender, RoutedEventArgs e)
        {
            Update_formul(null);

        }

        private void Update_formul(SQLiteDataReader req)
        {
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(461) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(31) });
            grid.RowDefinitions.Add(new RowDefinition());

            TextBox tb_fotmul = new TextBox();
            tb_fotmul.Padding = new Thickness(1, 1, 1, 1);
            tb_fotmul.TextWrapping = TextWrapping.WrapWithOverflow;
            tb_fotmul.FontSize = 16;
            tb_fotmul.AcceptsReturn = false;
            tb_fotmul.Foreground = colortext;
            tb_fotmul.Height = 30;
            tb_fotmul.Width = 451;
            tb_fotmul.BorderBrush = color_border;
            tb_fotmul.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Margin = new Thickness(0, 1, 0, 1);
            RowDefinition row_tb = new RowDefinition();
            if (req!=null)
            {
                tb_fotmul.Tag = req.GetInt16(0).ToString();
                tb_fotmul.Text = req.GetString(1);
            }
            Grid_formulirovka.RowDefinitions.Add(row_tb);

            Image img_del = new Image();
            img_del.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\delete.png", UriKind.Absolute));
            img_del.Width = 20;
            img_del.Height = 20;

            Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.Content = img_del;
            btn.Click += new RoutedEventHandler(DelFormulirovka);
            //btn.Margin = new Thickness(0, 0, 0, 10);
            Grid.SetRow(tb_fotmul, 0);
            Grid.SetColumn(tb_fotmul, 0);
            grid.Children.Add(tb_fotmul);

            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            StackPanel_A_4_2.Children.Add(grid);
            //Grid_formulirovka.Children.Add(tb_fotmul);
        }

        private void DelFormulirovka(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            var grids = StackPanel_A_4_2.Children;
            grids.Remove(grid);
            var textbox_name = grid.Children[0] as TextBox;
            if (textbox_name.Tag!=null)
                delete_str += textbox_name.Tag.ToString() + ";";
           // SQLite connection = new SQLite();
           // connection.WriteData(string.Format("delete from REQUIREMENTS Where id_req='{0}'", textbox_name.Tag));
        }

        private void FormaA2_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            //FormaA frm = sender as FormaA;
            //frm.Close();
            fw.Visibility = Visibility.Visible;
            fw.ForaWindow_Update();
           
        }
    }
}
