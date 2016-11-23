using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SZI
{
    /// <summary>
    /// Логика взаимодействия для FormaB_2.xaml
    /// </summary>
    public partial class FormaB_2 : Window
    {
        private FormaB fw_b;
        private FormaB_2 fw_b2;
        private int id_req,id_doc,id;
        //id - уникальный ключ в таблице формулировок
        private bool first;
        private SolidColorBrush colortext = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF378B1E"));
        private SolidColorBrush colortext2 = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEA2727"));
        private SolidColorBrush color_border = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDBFFD0"));
        string delete_str;
        int page=1;
        int iteration = 0;
        public FormaB_2(FormaB fw_b, int id_req, int id_doc)
        {
            fw_b2 = this;
            this.fw_b = fw_b;
            this.id_req = id_req;
            this.id_doc = id_doc;
            
            delete_str = "";
            InitializeComponent();
        }

        private void FormaB_1_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            fw_b.Visibility = Visibility.Visible;
        }

        private void FormaB_1_Loaded(object sender, RoutedEventArgs e)
        {

            label_RB.Visibility = Visibility.Visible;
            STP_radioButton.Visibility = Visibility.Visible;
            label_N_rb.Visibility = Visibility.Visible;
            STP_radioButton2.Visibility = Visibility.Visible;
            rext3.Visibility = Visibility.Visible;
            back_b_2.Visibility = Visibility.Collapsed;
            page = 1;
            UpdateB_2();

        }

        private void UpdateB_2()
        {
            
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select text,variant,norma from REQUIREMENTS_TMP Where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration,id_doc));
           // SQLiteDataReader reader = connection.ReadData(string.Format("Select text,variant,norma from REQUIREMENTS Where id_req='{0}' and iteration='{1}' and id_doc = '{2}'", id_req, iteration, id_doc));

            while (reader.Read())
            {
                textBlock_isk.Text = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    switch (reader.GetInt16(1))
                    {
                        case 1:
                            radioButton_v1.IsChecked = true;
                            break;
                        case 2:
                            radioButton_v2.IsChecked = true;
                            break;
                        case 3:
                            radioButton_v3.IsChecked = true;
                            break;
                        case 4:
                            radioButton_v4.IsChecked = true;
                            break;
                        case 5:
                            radioButton_v5.IsChecked = true;
                            break;
                        case 6:
                            radioButton_v6.IsChecked = true;
                            break;
                    }
                else
                {
                    radioButton_v1.IsChecked = true;
                }
                if (!reader.IsDBNull(2))
                {
                    if (reader.GetString(2) == "True")
                    {
                        radioButton_N_v1.IsChecked = true;
                    }
                    else
                    {
                        radioButton_N_v2.IsChecked = true;
                    }

                }
                else
                {
                    radioButton_N_v2.IsChecked = true;
                }
            }
            connection.Close();
        }

        private void Next_b_2_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            switch (page)
            {
                case 1:
                    if ((bool)radioButton_N_v1.IsChecked)
                    {
                        label_RB.Visibility = Visibility.Collapsed;
                        STP_radioButton.Visibility = Visibility.Collapsed;
                        label_N_rb.Visibility = Visibility.Collapsed;
                        STP_radioButton2.Visibility = Visibility.Collapsed;
                        if (iteration > 0)
                        {
                            page = page + 2;
                            UpdateB_2_3();
                            ST_adres.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            back_b_2.Visibility = Visibility.Visible;
                            rext3.Visibility = Visibility.Collapsed;
                            page++;
                            UpdateB_2_2();
                            ST_norma.Visibility = Visibility.Visible;
                            Scroll_st_norma.Visibility = Visibility.Visible;
                            title_norma.Visibility = Visibility.Visible;
                            btn.Visibility = Visibility.Visible;
                        }
                    }
                    break;

                case 2:
                    page++;
                    ST_norma.Visibility = Visibility.Collapsed;
                    Scroll_st_norma.Visibility = Visibility.Collapsed;
                    title_norma.Visibility = Visibility.Collapsed;
                    btn.Visibility = Visibility.Collapsed;
                    ST_adres.Visibility = Visibility.Visible;
                    UpdateB_2_3();
                    break;

                case 3:
                    reader = connection.ReadData(string.Format("Select variant,adresat_norm from REQUIREMENTS_TMP where id_req = '{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    int var = -1,adresat = -1;
                    while (reader.Read())
                    {
                        
                        if (reader.IsDBNull(0)|| reader.IsDBNull(1))
                        {
                            MessageBox.Show("Сначала сохраните данные");
                        }
                        else
                        { var = reader.GetInt16(0); adresat = reader.GetInt16(1); }
                           
                    }
                    if (adresat!=-1)
                        if ((var == 1) && ((adresat == 12) || (adresat == 13) || (adresat == 21) || (adresat == 22) || (adresat == 23))
                            || (var == 2) && ((adresat == 11) || (adresat == 12) || (adresat == 13) || (adresat == 21) || (adresat == 23))
                            || (var == 3) && ((adresat == 11) || (adresat == 12) || (adresat == 13) || (adresat == 21) || (adresat == 23))
                            || (var == 4) && ((adresat == 11) || (adresat == 12) || (adresat == 13) || (adresat == 21) || (adresat == 22))
                            || (var == 5) && ((adresat == 11) || (adresat == 13) || (adresat == 21) || (adresat == 22) || (adresat == 23))
                            || (var == 6) && ((adresat == 11) || (adresat == 12) || (adresat == 13) || (adresat == 21) || (adresat == 22)))
                        {
                            MessageBox.Show("Good");
                                                }
                        else
                        {
                            ErrorNorma eForm = new ErrorNorma(id_req, iteration, id_doc);
                            eForm.Owner = this;
                            eForm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                            eForm.ShowDialog();
                            if (eForm.variant != 0)
                            {
                                SQLite connection2 = new SQLite();
                                reader = connection2.ReadData(string.Format("Select choice from REQUIREMENTS_tmp where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                                int choise = 0;
                                while (reader.Read())
                                    if (!reader.IsDBNull(0))
                                        choise = reader.GetInt16(0);
                                if (choise != eForm.variant)
                                {
                                    connection2.WriteData(string.Format("Delete from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                                    connection2.WriteData(string.Format("Delete from NORMA where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                                }
                                connection2.WriteData(string.Format("Update REQUIREMENTS_TMP set choice = '{0}' where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", eForm.variant, id_req, iteration, id_doc));
                                connection2.Close();
                                switch (eForm.variant)
                                {
                                    case 1:
                                        page++;
                                        ST_adres.Visibility = Visibility.Collapsed;
                                        ST_explanat.Visibility = Visibility.Visible;
                                        break;
                                    case 2:                                        
                                        iteration++;
                                        page = 2;
                                        ST_adres.Visibility = Visibility.Collapsed;
                                        ST_norma.Visibility = Visibility.Visible;
                                        Scroll_st_norma.Visibility = Visibility.Visible;
                                        title_norma.Visibility = Visibility.Visible;
                                        btn.Visibility = Visibility.Visible;
                                        AddReqErr(var);
                                        UpdateB_2_2();
                                        break;
                                    case 3:
                                        iteration++;
                                        ST_adres.Visibility = Visibility.Collapsed;
                                        ST_edit_predmet.Visibility = Visibility.Visible;
                                        page = 10;
                                        UpdateB_10();
                                        break;
                                }
                            }
                       
                        }
                        break;
                case 10:
                    ST_edit_predmet.Visibility = Visibility.Collapsed;
                    label_RB.Visibility = Visibility.Visible;
                    STP_radioButton.Visibility = Visibility.Visible;
                    page = 1;
                    UpdateB_2();
                    break;
            }
            connection.Close();
        }

        private void AddReqErr(int variant)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;

            reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
            var count_req = 0;
            while (reader.Read())
                count_req = reader.GetInt16(0);
            if (count_req == 0)
            {
                reader = connection.ReadData(string.Format("Select text from REQUIREMENTS_TMP where  id_req='{0}' and iteration='{1}'  and id_doc='{2}'", id_req, iteration - 1, id_doc));
                while (reader.Read())
                    connection.WriteData(string.Format("Insert into REQUIREMENTS_TMP (id_doc,id_req,iteration,text,variant) values ('{0}','{1}','{2}','{3}','{4}')", id_doc, id_req, iteration, reader.GetString(0), variant));
            }
            connection.Close();
        }

        private void UpdateB_10()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            int count_pl = 0;
            reader = connection.ReadData(string.Format("Select count(*) from ACTORS where id_doc='{0}' and PLAINTIFF='{1}'", id_doc, true));
            while (reader.Read())
                count_pl = reader.GetInt16(0);
            if (count_pl>1)
            {
                label_edit.Content += " истцов";
            }
            else
            {
                label_edit.Content += " истца";
            }
            reader = connection.ReadData(string.Format("Select NAME_ACTOR from ACTORS where id_doc='{0}' and PLAINTIFF='{1}'", id_doc, 1));
            string str = "";
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    str += reader.GetString(0).Split(' ')[0] + ',';
            if (str == "")
            {
                MessageBox.Show("Нет информации по истцам");
            }
            else
            {
                str = str.Remove(str.Length - 1) + ':';
                label_edit_ist.Content = str;
               
            }
            textBox_edit.Text = "";
            reader = connection.ReadData(string.Format("Select text from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    textBox_edit.Text = reader.GetString(0);
                    
            connection.Close();
        }
        private void UpdateB_2_3()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            int str_adr = -1;
            reader = connection.ReadData(string.Format("Select adresat_norm from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}'", id_req, iteration, id_doc));
            while (reader.Read())
                if (!reader.IsDBNull(0))
                {
                    str_adr = reader.GetInt16(0);
                    switch (str_adr/10)
                    {
                        case 1:
                            radioButton_1.IsChecked = true;
                        break;
                        case 2:
                            radioButton_2.IsChecked = true;
                            break;
                        case 3:
                            radioButton_3.IsChecked = true;
                            break;
                    }
                    switch (str_adr%10)
                    {
                        case 1:
                            radioButton_n_1.IsChecked = true;
                            break;
                        case 2:
                            radioButton_n_2.IsChecked = true;
                            break;
                        case 3:
                            radioButton_n_3.IsChecked = true;
                            break;
                        case 4:
                            radioButton_n_4.IsChecked = true;
                            break;
                    }
                }
            else
                {
                    radioButton_1.IsChecked = true;
                    radioButton_n_1.IsChecked = true;
                }
            connection.Close();
        }

        private void UpdateB_2_2()
        {
            ST_norma.Children.Clear();

            first = false;
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(35) });

            //Label title_norma = new Label() { Content = "Норма права или пункты договора, которыми обосновано требование:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            Image img_add = new Image();
            img_add.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));
            img_add.Width = 20;
            img_add.Height = 20;

           // Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.Content = img_add;
           // btn.Click += new RoutedEventHandler(addGridNorma);

            //Grid.SetRow(title_norma, 0);
            //Grid.SetColumn(title_norma, 0);
            //grid.Children.Add(title_norma);
            //Grid.SetRow(btn, 0);
            //Grid.SetColumn(btn, 1);
            //grid.Children.Add(btn);
            //ST_norma.Children.Add(grid);

            reader = connection.ReadData(string.Format("Select count(*) from NORMA where id_req=(select id from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}')", id_req,iteration, id_doc));
            int count_norma = 0;
            while (reader.Read())
            {
                count_norma = reader.GetInt16(0);
            }
            if (count_norma == 0)
            {
                AddNorma(null);
                first = true;
            }
            else
            { 
                reader = connection.ReadData(string.Format("Select id_norma, text from NORMA where id_req=(select id from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}')", id_req, iteration, id_doc));
                while (reader.Read())
                {
                    AddNorma(reader);
                    first = true;
                }
            }
            connection.Close();
        }

        private void addGridNorma(object sender, RoutedEventArgs e)
        {
            AddNorma(null);
        }

        private void delGridNorma(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            StackPanel parentStack = (StackPanel)grid.Parent;
            var grids = parentStack.Children;
            grids.Remove(grid);
            StackPanel childrenStack = (StackPanel)grid.Children[0];
            var textbox_name = childrenStack.Children[1] as TextBox;
            if (textbox_name.Tag != null)
                delete_str += textbox_name.Tag.ToString() + ";";
        }

        private void AddNorma(SQLiteDataReader reader)
        {
            string[] arr=null;
            StackPanel str_1_norma = new StackPanel() { Orientation = Orientation.Horizontal };

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(15)});
            //grid.RowDefinitions.Add(new RowDefinition());

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
                btn.Visibility = Visibility.Visible;
                btn.Click += new RoutedEventHandler(delGridNorma);
            }

            Label str_1_1_norma = new Label() { Content = "- указать приведенные истцом точно пункт ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_1_1 = new TextBox();
            textbox_1_1.Padding = new Thickness(1, 1, 1, 1);
            //textbox_1_1.TextWrapping = TextWrapping.Wrap;
            textbox_1_1.FontSize = 16;
            textbox_1_1.AcceptsReturn = false;
            textbox_1_1.Foreground = colortext;
            textbox_1_1.Height = 25;
            textbox_1_1.BorderBrush = color_border;
            textbox_1_1.Width = 80;

            Label str_1_2_norma = new Label() { Content = " часть ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_1_2 = new TextBox();
            textbox_1_2.Padding = new Thickness(1, 1, 1, 1);
            //textbox_1_2.TextWrapping = TextWrapping.Wrap;
            textbox_1_2.FontSize = 16;
            textbox_1_2.AcceptsReturn = false;
            textbox_1_2.Foreground = colortext;
            textbox_1_2.Height = 25;
            textbox_1_2.BorderBrush = color_border;
            textbox_1_2.Width = 80;

            Label str_1_3_norma = new Label() { Content = " статью ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_1_3 = new TextBox();
            textbox_1_3.Padding = new Thickness(1, 1, 1, 1);
            //textbox_1_3.TextWrapping = TextWrapping.Wrap;
            textbox_1_3.FontSize = 16;
            textbox_1_3.AcceptsReturn = false;
            textbox_1_3.Foreground = colortext;
            textbox_1_3.Height = 25;
            textbox_1_3.BorderBrush = color_border;
            textbox_1_3.Width = 80;

            Label str_1_4_norma = new Label() { Content = ";", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            if (reader != null)
            {
                textbox_1_1.Tag = reader.GetInt16(0).ToString();
                arr = reader.GetString(1).Split('~');
                if (arr[0]!="")
                {
                    textbox_1_1.Text = arr[0];
                }
                if (arr[1] != "")
                {
                    textbox_1_2.Text = arr[1];
                }
                if (arr[2] != "")
                {
                    textbox_1_3.Text = arr[2];
                }
            }
            str_1_norma.Children.Add(str_1_1_norma);
            str_1_norma.Children.Add(textbox_1_1);
            str_1_norma.Children.Add(str_1_2_norma);
            str_1_norma.Children.Add(textbox_1_2);
            str_1_norma.Children.Add(str_1_3_norma);
            str_1_norma.Children.Add(textbox_1_3);
            str_1_norma.Children.Add(str_1_4_norma);

            Grid.SetRow(str_1_norma, 0);
            Grid.SetColumn(str_1_norma, 0);
            grid.Children.Add(str_1_norma);
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            StackPanel str_2_norma = new StackPanel() { Orientation = Orientation.Horizontal };

            Label str_2_1_norma = new Label() { Content = "- вид нормативного правового акта или договор (здесь и далее в род. падеже):", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_2_1 = new TextBox();
            textbox_2_1.Padding = new Thickness(1, 1, 1, 1);
           // textbox_2_1.TextWrapping = TextWrapping.Wrap;
            textbox_2_1.FontSize = 16;
            textbox_2_1.AcceptsReturn = false;
            textbox_2_1.Foreground = colortext;
            textbox_2_1.Height = 25;
            textbox_2_1.BorderBrush = color_border;
            textbox_2_1.Width = 200;
            if (reader != null)
            {
                if (arr[3] != "")
                {
                    textbox_2_1.Text = arr[3];
                }
            }
            str_2_norma.Children.Add(str_2_1_norma);
            str_2_norma.Children.Add(textbox_2_1);

            Grid.SetColumnSpan(str_2_norma, 2);
            Grid.SetRow(str_2_norma, 1);
            Grid.SetColumn(str_2_norma, 0);
            grid.Children.Add(str_2_norma);

            StackPanel str_3_norma = new StackPanel() { Orientation = Orientation.Horizontal };

            Label str_3_1_norma = new Label() { Content = "кем издан (орган): ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_3_1 = new TextBox();
            textbox_3_1.Padding = new Thickness(1, 1, 1, 1);
           // textbox_3_1.TextWrapping = TextWrapping.Wrap;
            textbox_3_1.FontSize = 16;
            textbox_3_1.AcceptsReturn = false;
            textbox_3_1.Foreground = colortext;
            textbox_3_1.Height = 25;
            textbox_3_1.BorderBrush = color_border;
            textbox_3_1.Width = 180;

            Label str_3_2_norma = new Label() { Content = " РФ или территория органа: ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_3_2 = new TextBox();
            textbox_3_2.Padding = new Thickness(1, 1, 1, 1);
           // textbox_3_2.TextWrapping = TextWrapping.Wrap;
            textbox_3_2.FontSize = 16;
            textbox_3_2.AcceptsReturn = false;
            textbox_3_2.Foreground = colortext;
            textbox_3_2.Height = 25;
            textbox_3_2.BorderBrush = color_border;
            textbox_3_2.Width = 180;

            if (reader != null)
            {
                if (arr[4] != "")
                {
                    textbox_3_1.Text = arr[4];
                }
                if (arr[5] != "")
                {
                    textbox_3_2.Text = arr[5];
                }
            }
            str_3_norma.Children.Add(str_3_1_norma);
            str_3_norma.Children.Add(textbox_3_1);
            str_3_norma.Children.Add(str_3_2_norma);
            str_3_norma.Children.Add(textbox_3_2);

            Grid.SetColumnSpan(str_3_norma, 2);
            Grid.SetRow(str_3_norma, 2);
            Grid.SetColumn(str_3_norma, 0);
            grid.Children.Add(str_3_norma);

            StackPanel str_4_norma = new StackPanel() { Orientation = Orientation.Horizontal };

            Label str_4_1_norma = new Label() { Content = "название: ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_4_1 = new TextBox();
            textbox_4_1.Padding = new Thickness(1, 1, 1, 1);
            //textbox_4_1.TextWrapping = TextWrapping.Wrap;
            textbox_4_1.FontSize = 16;
            textbox_4_1.AcceptsReturn = false;
            textbox_4_1.Foreground = colortext;
            textbox_4_1.Height = 25;
            textbox_4_1.BorderBrush = color_border;
            textbox_4_1.Width = 380;

            if (reader != null)
            {
                if (arr[6] != "")
                {
                    textbox_4_1.Text = arr[6];
                }
            }

            str_4_norma.Children.Add(str_4_1_norma);
            str_4_norma.Children.Add(textbox_4_1);

            Grid.SetColumnSpan(str_4_norma, 2);
            Grid.SetRow(str_4_norma, 3);
            Grid.SetColumn(str_4_norma, 0);
            grid.Children.Add(str_4_norma);

            StackPanel str_5_norma = new StackPanel() { Orientation = Orientation.Horizontal };

            Label str_5_1_norma = new Label() { Content = "с их реквизитами: от ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            DatePicker date_5_1_norma = new DatePicker() { FontSize = 16 , Width = 130};

            Label str_5_2_norma = new Label() { Content = " № ", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_5_1 = new TextBox();
            textbox_5_1.Padding = new Thickness(1, 1, 1, 1);
           // textbox_5_1.TextWrapping = TextWrapping.Wrap;
            textbox_5_1.FontSize = 16;
            textbox_5_1.AcceptsReturn = false;
            textbox_5_1.Foreground = colortext;
            textbox_5_1.Height = 25;
            textbox_5_1.BorderBrush = color_border;
            textbox_5_1.Width = 150;

            Label str_5_3_norma = new Label() { Content = "  (при отсутствии номера указывается б/н).", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            if (reader != null)
            {
                if (arr[7] != "")
                {
                    date_5_1_norma.SelectedDate = DateTime.Parse(arr[7]);
                }
                if (arr[8] != "")
                {
                    textbox_5_1.Text = arr[8];
                }
            }

            str_5_norma.Children.Add(str_5_1_norma);
            str_5_norma.Children.Add(date_5_1_norma);
            str_5_norma.Children.Add(str_5_2_norma);
            str_5_norma.Children.Add(textbox_5_1);
            str_5_norma.Children.Add(str_5_3_norma);

            Grid.SetColumnSpan(str_5_norma, 2);
            Grid.SetRow(str_5_norma, 4);
            Grid.SetColumn(str_5_norma, 0);
            grid.Children.Add(str_5_norma);

            //Grid.SetColumnSpan(str_5_3_norma, 2);
            //Grid.SetRow(str_5_3_norma, 5);
            //Grid.SetColumn(str_5_3_norma, 0);
            //grid.Children.Add(str_5_3_norma);

            Rectangle rec = new Rectangle() { Fill = colortext, Height = 1 };
            rec.Margin = new Thickness(0, 7, 0, 7);
            Grid.SetColumnSpan(rec, 2);
            Grid.SetRow(rec, 5);
            Grid.SetColumn(rec, 0);
            grid.Children.Add(rec);

            ST_norma.Children.Add(grid);
        }

        private void FormaB_1_Loaded_1(object sender, RoutedEventArgs e)
        {
            label_RB.Visibility = Visibility.Visible;
            STP_radioButton.Visibility = Visibility.Visible;
            label_N_rb.Visibility = Visibility.Visible;
            STP_radioButton2.Visibility = Visibility.Visible;
            rext3.Visibility = Visibility.Visible;
            back_b_2.Visibility = Visibility.Collapsed;
            page = 1;
            UpdateB_2();
        }

        private void back_b_2_Click(object sender, RoutedEventArgs e)
        {
            if (page == 2)
            {
                ST_norma.Visibility = Visibility.Collapsed;
                Scroll_st_norma.Visibility = Visibility.Collapsed;
                title_norma.Visibility = Visibility.Collapsed;
                btn.Visibility = Visibility.Collapsed;
                if (iteration > 0)
                {
                    iteration--;
                    page = 3;
                    ST_adres.Visibility = Visibility.Visible;
                    UpdateB_2_3();
                }
                else
                {
                    label_RB.Visibility = Visibility.Visible;
                    STP_radioButton.Visibility = Visibility.Visible;
                    label_N_rb.Visibility = Visibility.Visible;
                    STP_radioButton2.Visibility = Visibility.Visible;
                    back_b_2.Visibility = Visibility.Hidden;
                    rext3.Visibility = Visibility.Visible;
                    page--;
                    UpdateB_2();
                }
            }
            else if (page==3)
            {
                ST_norma.Visibility = Visibility.Visible;
                Scroll_st_norma.Visibility = Visibility.Visible;
                title_norma.Visibility = Visibility.Visible;
                btn.Visibility = Visibility.Visible;
                ST_adres.Visibility = Visibility.Collapsed;
                UpdateB_2_2();
                page--;
            }
            else if (page == 4)
            {
                ST_explanat.Visibility = Visibility.Collapsed;
                ST_adres.Visibility = Visibility.Visible;
                UpdateB_2_3();
                page--;
            }
            else if (page == 10)
            {
                ST_edit_predmet.Visibility = Visibility.Collapsed;
                ST_adres.Visibility = Visibility.Visible;
                iteration--;
                UpdateB_2_3();
                page=3;
                
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            switch (page)
            {
                case 1:
                    delete_str = "";
                    int variant = 0;
                    if ((bool)radioButton_v1.IsChecked)
                        variant = 1;
                    if ((bool)radioButton_v2.IsChecked)
                        variant = 2;
                    if ((bool)radioButton_v3.IsChecked)
                        variant = 3;
                    if ((bool)radioButton_v4.IsChecked)
                        variant = 4;
                    if ((bool)radioButton_v5.IsChecked)
                        variant = 5;
                    if ((bool)radioButton_v6.IsChecked)
                        variant = 6;
                    reader = connection.ReadData(string.Format("Select variant from REQUIREMENTS_tmp where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    int choise = 0;
                    while (reader.Read())
                        if (!reader.IsDBNull(0))
                            choise = reader.GetInt16(0);
                    if (choise != variant)
                    {
                        connection.WriteData(string.Format("Delete from NORMA where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                        connection.WriteData(string.Format("Delete from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                    }
                    connection.WriteData(string.Format("Update REQUIREMENTS_TMP set variant='{0}' Where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", variant, id_req, iteration, id_doc));


                    if ((bool)radioButton_N_v1.IsChecked)
                    {
                        connection.WriteData(string.Format("Update REQUIREMENTS_TMP set norma='{0}' Where id_req='{1}' and iteration = '{2}'  and id_doc='{3}'", true, id_req, iteration,id_doc));
                    }
                    else
                    {
                        reader = connection.ReadData(string.Format("Select norma from REQUIREMENTS_tmp where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                        while (reader.Read())
                            if (!reader.IsDBNull(0))
                                if (reader.GetString(0)=="True")
                                {
                                    connection.WriteData(string.Format("Delete from NORMA where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration >= '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                                    connection.WriteData(string.Format("Delete from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                                }
                        connection.WriteData(string.Format("Update REQUIREMENTS_TMP set norma='{0}' Where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", false, id_req, iteration,id_doc));
                    }
                    break;

                case 2:
                    if (delete_str != "")
                    {
                        string[] arr = delete_str.Split(';');
                        for (int i = 0; i < arr.Length - 1; i++)
                        {
                            connection.WriteData(string.Format("delete from NORMA where ID_norma='{0}'", arr[i]));
                        }
                    }
                    foreach (Grid grid in ST_norma.Children)
                    {
                        if (grid.RowDefinitions.Count > 1)
                        {

                            var str_1_0_norma = grid.Children[0] as StackPanel;
                            var str_1_1_norma = grid.Children[2] as StackPanel;
                            var str_1_2_norma = grid.Children[3] as StackPanel;
                            var str_1_3_norma = grid.Children[4] as StackPanel;
                            var str_1_4_norma = grid.Children[5] as StackPanel;

                            var textBox_str = str_1_0_norma.Children[1] as TextBox;
                            int id_norma;
                            if (textBox_str.Tag == null)
                                id_norma = -1;
                            else
                                id_norma = Int16.Parse(textBox_str.Tag.ToString());
                            var str_doc = textBox_str.Text + "~";

                            textBox_str = str_1_0_norma.Children[3] as TextBox;
                            str_doc += textBox_str.Text + "~";
                            textBox_str = str_1_0_norma.Children[5] as TextBox;
                            str_doc += textBox_str.Text + "~";

                            textBox_str = str_1_1_norma.Children[1] as TextBox;
                            str_doc += textBox_str.Text + "~";

                            textBox_str = str_1_2_norma.Children[1] as TextBox;
                            str_doc += textBox_str.Text + "~";
                            textBox_str = str_1_2_norma.Children[3] as TextBox;
                            str_doc += textBox_str.Text + "~";

                            textBox_str = str_1_3_norma.Children[1] as TextBox;
                            str_doc += textBox_str.Text + "~";

                            var date_pick = str_1_4_norma.Children[1] as DatePicker;
                            str_doc += date_pick.SelectedDate.ToString() + "~";

                            textBox_str = str_1_4_norma.Children[3] as TextBox;
                            str_doc += textBox_str.Text;

                            if (id_norma == -1)
                            {
                                reader = connection.ReadData(string.Format("Select id from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'   and id_doc='{2}'", id_req,iteration, id_doc));
                                while (reader.Read())
                                    id = reader.GetInt16(0);
                                connection.WriteData(string.Format("Insert into NORMA (id_req,text) values('{0}','{1}')", id, str_doc));
                            }
                            else
                            {
                                connection.WriteData(string.Format("Update NORMA set text='{0}' Where id_norma ='{1}'", str_doc, id_norma));
                            }
                        }
                    }
                    break;
                case 3:
                    int adres = 0;
                    int norm =0;
                    if ((bool)radioButton_1.IsChecked)
                        adres = 1;                    
                    else if ((bool)radioButton_2.IsChecked)
                        adres = 2;
                    else if ((bool)radioButton_3.IsChecked)
                        adres = 3;

                    if ((bool)radioButton_n_1.IsChecked)
                        norm = 1;
                    else if ((bool)radioButton_n_2.IsChecked)
                        norm = 2;
                    else if ((bool)radioButton_n_3.IsChecked)
                        norm = 3;
                    else if ((bool)radioButton_n_4.IsChecked)
                        norm = 4;

                    connection.WriteData(string.Format("Update REQUIREMENTS_TMP set adresat_norm='{0}' Where id_req='{1}' and iteration='{2}'   and id_doc='{3}'", adres*10+norm, id_req, iteration,id_doc));
                    break;
                case 10:
                    if (textBox_edit.Text.Length==0)
                    {
                        MessageBox.Show("Заполните поле формулировки");
                    }
                    else
                    {
                        reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
                        var count_req = 0;
                        while (reader.Read())
                            count_req = reader.GetInt16(0);
                        if (count_req==0)
                            connection.WriteData(string.Format("Insert into REQUIREMENTS_TMP (id_doc,id_req,iteration,text) values ('{0}','{1}','{2}','{3}')", id_doc,id_req, iteration,textBox_edit.Text));
                        else
                            connection.WriteData(string.Format("Update REQUIREMENTS_TMP text='{0}' where id_doc='{1}' and id_req='{2}' and iteration='{3}'", textBox_edit.Text,id_doc, id_req, iteration));
                    }
                    break;
            }
            connection.Close();
        }
    }
}
