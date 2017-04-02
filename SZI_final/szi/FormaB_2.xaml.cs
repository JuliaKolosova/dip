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
        int izmena_n = 0;
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
            for (int i = 0; i < Glavnaya.Children.Count; i++)
                Glavnaya.Children[i].Visibility = Visibility.Collapsed;
            label_RB.Visibility = Visibility.Visible;
            STP_radioButton.Visibility = Visibility.Visible;
            label_N_rb.Visibility = Visibility.Visible;
            STP_radioButton2.Visibility = Visibility.Visible;
            rect3.Visibility = Visibility.Visible;
            back_b_2.Visibility = Visibility.Collapsed;
            page = 1;
            UpdateB_2();

        }

        private void UpdateB_2()
        {
            
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select text,variant,norma,izmena from REQUIREMENTS_TMP Where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration,id_doc));
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
                if (!reader.IsDBNull(3))
                    izmena_n = reader.GetInt16(3);

                if (!reader.IsDBNull(2))
                {
                    if (bool.Parse(reader.GetString(2)))
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
            if (izmena_n==1)
            {
                reader = connection.ReadData(string.Format("Select count(*) from NORMA where id_req in (select id from REQUIREMENTS_TMP Where id_doc='{0}')", id_doc));
                while (reader.Read())
                    if (reader.GetInt16(0) == 0)
                        radioButton_N_v1.IsEnabled = false;
            }
            connection.Close();
        }

        private void Next_b_2_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            delete_str = "";
            switch (page)
            {
                case 1:
                    reader = connection.ReadData(string.Format("Select norma from REQUIREMENTS_TMP where id_req = '{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    bool chek=true,sav=true;
                    while (reader.Read())
                        if (reader.IsDBNull(0))
                        {
                            MessageBox.Show("Сохраните данные!");
                            sav = false;
                        }
                        else
                        {
                            chek = Boolean.Parse(reader.GetString(0));
                            sav = true;
                        }
                    if (sav)
                        if (chek)
                        {
                            label_RB.Visibility = Visibility.Collapsed;
                            STP_radioButton.Visibility = Visibility.Collapsed;
                            label_N_rb.Visibility = Visibility.Collapsed;
                            STP_radioButton2.Visibility = Visibility.Collapsed;


                            back_b_2.Visibility = Visibility.Visible;
                            rect3.Visibility = Visibility.Collapsed;                            
                            page++;

                            if (izmena_n == 0)
                            {
                                UpdateB_2_2();
                                ST_norma.Visibility = Visibility.Visible;
                                Scroll_st_norma.Visibility = Visibility.Visible;
                                title_norma.Visibility = Visibility.Visible;
                                btn.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                ST_norma_izm.Visibility = Visibility.Visible;
                                title_norma_izm.Visibility = Visibility.Visible;
                                Scroll_st_norma_izm.Visibility = Visibility.Visible;
                                UpdateB_2_2_izm();
                            }

                        }
                        else
                        {
                            if (iteration > 0)
                            {
                                label_RB.Visibility = Visibility.Collapsed;
                                STP_radioButton.Visibility = Visibility.Collapsed;
                                label_N_rb.Visibility = Visibility.Collapsed;
                                STP_radioButton2.Visibility = Visibility.Collapsed;
                                page = page + 2;
                                UpdateB_2_3();
                                ST_adres.Visibility = Visibility.Visible;
                            }
                            else
                            {
                                page = 7;
                                label_RB.Visibility = Visibility.Collapsed;
                                STP_radioButton.Visibility = Visibility.Collapsed;
                                label_N_rb.Visibility = Visibility.Collapsed;
                                back_b_2.Visibility = Visibility.Visible;
                                STP_radioButton2.Visibility = Visibility.Collapsed;
                                //Next_b_2.Visibility = Visibility.Collapsed;
                                //Save.Visibility = Visibility.Collapsed;
                                ST_fact.Visibility = Visibility.Visible;
                                //Scroll_st_fact.Visibility = Visibility.Visible;
                                textBlock_isk.Visibility = Visibility.Collapsed;
                                rect2.Visibility = Visibility.Collapsed;
                                rect3.Visibility = Visibility.Collapsed;
                                UpdateB_2_5();
                            }
                        }
                    break;

                case 2:
                    page++;
                    ST_norma.Visibility = Visibility.Collapsed;
                    Scroll_st_norma.Visibility = Visibility.Collapsed;
                    title_norma.Visibility = Visibility.Collapsed;
                    Scroll_st_norma_izm.Visibility = Visibility.Collapsed;
                    title_norma_izm.Visibility = Visibility.Collapsed;
                    btn.Visibility = Visibility.Collapsed;
                    ST_adres.Visibility = Visibility.Visible;
                    UpdateB_2_3();
                    break;

                case 3:
                    reader = connection.ReadData(string.Format("Select variant,adresat_norm from REQUIREMENTS_TMP where id_req = '{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    int var = -1, adresat = -1;
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
                            ST_adres.Visibility = Visibility.Collapsed;
                            //Next_b_2.Visibility = Visibility.Collapsed;
                            //Save.Visibility = Visibility.Collapsed;
                            ST_fact.Visibility = Visibility.Visible;
                            ////Scroll_st_fact.Visibility = Visibility.Visible;
                            textBlock_isk.Visibility = Visibility.Collapsed;
                            rect2.Visibility = Visibility.Collapsed;
                            rect3.Visibility = Visibility.Collapsed;
                            UpdateB_2_5();
                            page = 6;
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
                                    connection2.WriteData(string.Format("Delete from NORMA where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                                    connection2.WriteData(string.Format("Delete from fact_thing where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                                    connection2.WriteData(string.Format("Delete from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                                    connection2.WriteData(string.Format("UPDATE REQUIREMENTS_tmp set defendant_choise=null,prizn_isk=null,explain_tr=null,izm_norma=null,court_costs=null, distribution_of_costs=null, court_of_appeal=null where id_req = '{0}' and iteration = '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                                }
                                connection2.WriteData(string.Format("Update REQUIREMENTS_TMP set choice = '{0}' where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", eForm.variant, id_req, iteration, id_doc));
                                connection2.Close();
                                switch (eForm.variant)
                                {
                                    case 1:
                                        page++;
                                        ST_adres.Visibility = Visibility.Collapsed;
                                        ST_explanat.Visibility = Visibility.Visible;
                                        UpdateB2_4();
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
                case 4:
                    ST_explanat.Visibility = Visibility.Collapsed;
                    //Next_b_2.Visibility = Visibility.Collapsed;
                    // Save.Visibility = Visibility.Collapsed;
                    ST_fact.Visibility = Visibility.Visible;
                    //Scroll_st_fact.Visibility = Visibility.Visible;
                    textBlock_isk.Visibility = Visibility.Collapsed;
                    rect2.Visibility = Visibility.Collapsed;
                    rect3.Visibility = Visibility.Collapsed;
                    page=5;
                    UpdateB_2_5();
                    break;

                case 5:
                    reader = connection.ReadData(string.Format("Select count(*) from fact_thing where id_req = ( select id from REQUIREMENTS_TMP where id_req = '{0}' and iteration='{1}' and id_doc='{2}')", id_req, iteration, id_doc));
                    int count_fact = 0;
                    while (reader.Read())
                        count_fact = reader.GetInt16(0);
                    if (count_fact > 0)
                    {
                        ST_defendant_choise.Visibility = Visibility.Visible;
                        ST_fact.Visibility = Visibility.Collapsed;
                        page = page + 6;
                        UpdateB_567();
                    }
                    else
                        MessageBox.Show("Должно быть хотя бы одно фактическое обстоятельство");
                    break;

                case 6:
                    goto case 5;

                case 7:
                    goto case 5;

                case 10:
                    ST_edit_predmet.Visibility = Visibility.Collapsed;
                    label_RB.Visibility = Visibility.Visible;
                    STP_radioButton.Visibility = Visibility.Visible;
                    page = 1;
                    UpdateB_2();
                    break;
                case 11:
                    SQLite connection3 = new SQLite();
                    reader = connection3.ReadData(string.Format("Select defendant_choise,prizn_isk from REQUIREMENTS_tmp where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    int choice_cos = 0;
                    while (reader.Read())
                        choice_cos = reader.GetInt16(0)+ reader.GetInt16(1);
                    if (choice_cos==2)
                    {
                        ST_costs.Visibility = Visibility.Visible;
                        ST_defendant_choise.Visibility = Visibility.Collapsed;
                        Next_b_2.Content = "Закрыть";
                        page = page + 3;
                        UpdateB_141517();
                    }

                    connection3.Close();
                    break;
                case 12:
                    goto case 11;

                case 13:
                    goto case 11;
                case 14:
                    fw_b2.Close();
                    break;
                case 15:
                    goto case 14;

                case 16:
                    goto case 14;
            }
            connection.Close();
        }

        private void UpdateB_141517()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            reader = connection.ReadData(string.Format("Select court_costs, distribution_of_costs, court_of_appeal from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    textBox_court_costs.Text = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    textBox_distribution_of_costs.Text = reader.GetString(1);
                if (!reader.IsDBNull(2))
                    textBox_court_of_appeal.Text = reader.GetString(2);
            }
            connection.Close();
        }

        private void UpdateB_2_2_izm()
        {
            ST_norma_izm.Children.Clear();

            SQLite connection = new SQLite();
            SQLiteDataReader reader_norma, reader2;
            
            //SELECT * FROM norma f where f.id_req in (select d.id from REQUIREMENTS_TMP d where d.id_doc=6 and d.iteration = (select max(k.iteration) from REQUIREMENTS_TMP k where k.id_doc=6 and k.id_req=d.id_req )  )

            reader_norma = connection.ReadData(string.Format("Select id_norma, text from NORMA where id_req in (select id from REQUIREMENTS_TMP where id_doc='{0}')", id_doc));
            reader2 = connection.ReadData(string.Format("Select izm_norma from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
            string[] arr = null;
            while (reader2.Read())
                if (!reader2.IsDBNull(0))
                    arr = reader2.GetString(0).Split(';');
            while (reader_norma.Read())
            {
                CheckBox ch_norma = new CheckBox() { Tag = reader_norma.GetInt16(0).ToString(), VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 0, 5) };
                ch_norma.Content = new TextBlock()
                {
                    TextWrapping = TextWrapping.Wrap,
                    Text = UpgradeNorma(reader_norma.GetString(1))
                };
                ch_norma.FontSize = 16;
                ch_norma.Foreground = colortext;

                if (arr != null)
                    if (arr.Contains(reader_norma.GetInt16(0).ToString()))
                        ch_norma.IsChecked = true;

                ST_norma_izm.Children.Add(ch_norma);
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
                    connection.WriteData(string.Format("Insert into REQUIREMENTS_TMP (id_doc,id_req,iteration,text,variant,norma,izmena) values ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')", id_doc, id_req, iteration, reader.GetString(0), variant,false, izmena_n));
            }
            connection.Close();
        }

        private void UpdateB_567()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            Int16 defendant_choise = 1, prizn_isk=1;
            reader = connection.ReadData(string.Format("Select defendant_choise,prizn_isk from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    defendant_choise = reader.GetInt16(0);
            switch (defendant_choise)
            {
                case 1:
                    def_ch_1.IsChecked = true;
                    break;

                case 2:
                    def_ch_2.IsChecked = true;
                    break;

                case 3:
                    def_ch_3.IsChecked = true;
                    break;
            }
            switch (prizn_isk)
            {
                case 1:
                    priz_isk_1.IsChecked = true;
                    break;

                case 2:
                    priz_isk_2.IsChecked = true;
                    break;

                case 3:
                    priz_isk_3.IsChecked = true;
                    break;
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
            reader = connection.ReadData(string.Format("Select text from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration-1));
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    textBlock_isk.Text = reader.GetString(0);

            reader = connection.ReadData(string.Format("Select text from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    textBox_edit.Text = reader.GetString(0);

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

            reader = connection.ReadData(string.Format("Select count(*) from NORMA where id_req=(select id from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}')", id_req, iteration, id_doc));
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

        private void UpdateB2_4()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            reader = connection.ReadData(string.Format("Select explain_tr from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}'", id_req, iteration, id_doc));
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    textBox.Text = reader.GetString(0);
            connection.Close();
        }

        private void UpdateB_2_5()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            int count_f = 0;
            bool is_norma = false;

            reader = connection.ReadData(string.Format("Select id,text from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}'", id_req, iteration, id_doc));
            while (reader.Read())
            {
                textBlock_header_fact.Text = string.Format("   Фактические обстоятельства, указанные истцом в обосновании требования '{0}'. Указанные в исковом заявлении правовые состояния, действия, события излагаются как отдельные юридические факты:", reader.GetString(1));
                id = reader.GetInt16(0);
            }

            reader = connection.ReadData(string.Format("Select norma from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}'", id_req, 0, id_doc));
            while (reader.Read())
            {
                is_norma = Boolean.Parse(reader.GetString(0));
            }

            reader = connection.ReadData(string.Format("Select count(*) from fact_thing where id_req='{0}'", id));
            while (reader.Read())
            {
                count_f = reader.GetInt16(0);
            }
            first = true;
            ST_fact_list.Children.Clear();
            if (count_f==0)
            {
                AddFact(null, true, is_norma);
            }
            else
            {
                reader = connection.ReadData(string.Format("Select id, text, choise, list_norm from fact_thing where id_req='{0}'", id));
                while (reader.Read())
                {
                    AddFact(reader, first, is_norma);
                    first = false;
                }
            }
            connection.Close();
        }

        public string UpgradeNorma(string norma)
        {
            string[] str_arr = norma.Split('~');
            string str_norma = "";
            if (str_arr[0] != "")
                str_norma += "пункт "+str_arr[0]+" ";
            if (str_arr[1] != "")
                str_norma += "часть " + str_arr[1] + " ";
            if (str_arr[2] != "")
                str_norma += "статьи " + str_arr[2] + " ";
            if (str_arr[3] != "")
                str_norma += str_arr[3] + " ";
            if (str_arr[4] != "")
                str_norma += "издан " + str_arr[4] + " ";
            else if (str_arr[5] != "")
                str_norma += "издан " + str_arr[5] + " ";
            if (str_arr[7] != "")
                str_norma += "от " + str_arr[7] + " ";
            if (str_arr[8] != "")
                str_norma += "№ " + str_arr[8] + " ";
            if (str_arr[6] != "")
                str_norma += '"' + str_arr[6] + '"'+" ";
            str_norma = str_norma.Remove(str_norma.Length - 1);
            str_norma += ';';
            return str_norma;
        }

        private void AddFact(SQLiteDataReader reader, bool first, bool is_norma)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader_norma;
            StackPanel st_fact_norma = new StackPanel() { Margin = new Thickness(20,0,0,0)};
            var id_req_iter0 = 0;
            var ch_err =0;
            if (izmena_n == 0)
            {
                reader_norma = connection.ReadData(string.Format("select id,choice from REQUIREMENTS_TMP where id_doc = '{0}' and id_req = '{1}' and iteration = 0", id_doc, id_req));
                while (reader_norma.Read())
                {
                    id_req_iter0 = reader_norma.GetInt16(0);
                    if (!reader_norma.IsDBNull(1))
                        ch_err = reader_norma.GetInt16(1);
                }            
                if (ch_err != 3)
                    reader_norma = connection.ReadData(string.Format("SELECT id_norma,text FROM norma where id_req in (select id from REQUIREMENTS_TMP where id_doc = '{0}' and id_req = '{1}' and iteration = (select max(iteration) from REQUIREMENTS_TMP where id_doc = '{0}' and id_req = '{1}'))", id_doc, id_req));
                else
                    reader_norma = connection.ReadData(string.Format("SELECT id_norma,text FROM norma where id_req ='{0}'", id_req_iter0));
            }
            else
            {
                reader_norma = connection.ReadData(string.Format("SELECT izm_norma FROM REQUIREMENTS_TMP where id_doc = '{0}' and id_req = '{1}' and iteration = '{2}'", id_doc,id_req,0));

            }
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10) });

            Image img_add = new Image();
            img_add.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));
            img_add.Width = 20;
            img_add.Height = 20;

            Image img_del = new Image();
            img_del.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\delete.png", UriKind.Absolute));
            img_del.Width = 20;
            img_del.Height = 20;

            Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            if (first)
            {
                btn.Content = img_add;
                btn.Click += new RoutedEventHandler(AddFact);
            }
            else
            {
                btn.Content = img_del;
                btn.Click += new RoutedEventHandler(delFact);
            }

            TextBox textbox_1_1 = new TextBox();
            textbox_1_1.Padding = new Thickness(1, 1, 1, 1);
            textbox_1_1.FontSize = 16;
            textbox_1_1.AcceptsReturn = false;
            textbox_1_1.Foreground = colortext;
            textbox_1_1.Height = 25;
            textbox_1_1.Margin = new Thickness(0, 0, 20, 5);
            textbox_1_1.BorderBrush = color_border;



            RadioButton rb_norma = new RadioButton();
           // rb_norma.GroupName = "Fact";
            rb_norma.Content = "указанная истцом норма права:";
            rb_norma.VerticalContentAlignment = VerticalAlignment.Center;
            rb_norma.FontSize = 16;
            rb_norma.Foreground = colortext;
           // rb_norma.IsChecked = true;
            rb_norma.Margin = new Thickness(0, 5, 0, 5);


            RadioButton rb_probel = new RadioButton();
           // rb_probel.GroupName = "Fact";
            rb_probel.Content = "имеет место пробел в праве;";
            rb_probel.VerticalContentAlignment = VerticalAlignment.Center;
            rb_probel.FontSize = 16;
            rb_probel.Foreground = colortext;
            rb_probel.Margin = new Thickness(0, 5, 0, 5);



            RadioButton rb_otkaz = new RadioButton();
           // rb_otkaz.GroupName = "Fact";
            rb_otkaz.Content = new TextBlock() {
                TextWrapping = TextWrapping.Wrap,  
                         Text = "юридические основания для обоснования указываемого им обстоятельства, как юридического факта, истец не указал."
            };
            //rb_otkaz.Content = "юридические основания для обоснования указываемого им обстоятельства как юридического факта" + "истец не указал";
            rb_otkaz.VerticalContentAlignment = VerticalAlignment.Center;
            rb_otkaz.FontSize = 16;
            rb_otkaz.Foreground = colortext;
            rb_otkaz.Width = 780;
            rb_otkaz.HorizontalAlignment = HorizontalAlignment.Left;
            rb_otkaz.Margin = new Thickness(0, 5, 0, 5);

            string[] arr = null;
            if (reader != null)
            {
                textbox_1_1.Tag = reader.GetInt16(0).ToString();
                textbox_1_1.Text = reader.GetString(1);

                if (!reader.IsDBNull(2))
                {
                    switch (reader.GetInt16(2))
                    {
                        case 1:
                            rb_norma.IsChecked = true;
                            break;
                        case 2:
                            rb_probel.IsChecked = true;
                            break;
                        case 3:
                            rb_otkaz.IsChecked = true;
                            break;
                    }
                }
                
                if (!reader.IsDBNull(3))
                {
                    arr = reader.GetString(3).Split(';');
                }
            }
            else
            {
                rb_norma.IsChecked = true;
            }

            if (izmena_n == 0)
            {
                while (reader_norma.Read())
                {
                    CheckBox ch_norma = new CheckBox() { Tag = reader_norma.GetInt16(0).ToString(), VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 0, 5) };
                    ch_norma.Content = new TextBlock()
                    {
                        TextWrapping = TextWrapping.Wrap,
                        Text = UpgradeNorma(reader_norma.GetString(1))
                    };
                    ch_norma.FontSize = 16;
                    ch_norma.Foreground = colortext;

                    if (arr != null)
                        if (arr.Contains(reader_norma.GetInt16(0).ToString()))
                            ch_norma.IsChecked = true;

                    st_fact_norma.Children.Add(ch_norma);
                }
            }
            else
            {
                string[] arr_izm = null;
                while (reader_norma.Read())
                    if(!reader_norma.IsDBNull(0))
                    {
                        arr_izm = reader_norma.GetString(0).Split(';');
                    }
                if (arr_izm!=null)
                {
                    for (int i = 0; i < arr_izm.Length; i++)
                    {
                        SQLiteDataReader read_norm_izm = connection.ReadData(string.Format("select text from norma where id_norma = '{0}'", arr_izm[i]));
                        while (read_norm_izm.Read())
                        {
                            CheckBox ch_norma = new CheckBox() { Tag = arr_izm[i], VerticalContentAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 0, 5) };
                            ch_norma.Content = new TextBlock()
                            {
                                TextWrapping = TextWrapping.Wrap,
                                Text = UpgradeNorma(read_norm_izm.GetString(0))
                            };
                            ch_norma.FontSize = 16;
                            ch_norma.Foreground = colortext;

                            if (arr != null)
                                if (arr.Contains(arr_izm[i].ToString()))
                                    ch_norma.IsChecked = true;

                            st_fact_norma.Children.Add(ch_norma);
                        }
                    }
                }
            }

            Grid.SetRow(textbox_1_1, 0);
            Grid.SetColumn(textbox_1_1, 0);
            grid.Children.Add(textbox_1_1);
            Grid.SetRow(btn, 0);
            Grid.SetColumn(btn, 1);
            grid.Children.Add(btn);

            Grid.SetRow(rb_norma, 1);
            Grid.SetColumn(rb_norma, 0);
            grid.Children.Add(rb_norma);


            if (!(bool)rb_norma.IsChecked)
            {
                st_fact_norma.Visibility = Visibility.Collapsed;
            }

            if (!is_norma)
            {
                rb_norma.IsEnabled = false;
                rb_norma.IsChecked = false;
                rb_probel.IsChecked = true;
            }

            Grid.SetRow(st_fact_norma, 2);
            Grid.SetColumn(st_fact_norma, 0);
            grid.Children.Add(st_fact_norma);

            Grid.SetRow(rb_probel, 3);
            Grid.SetColumn(rb_probel, 0);
            grid.Children.Add(rb_probel);

            Grid.SetRow(rb_otkaz, 4);
            Grid.SetColumn(rb_otkaz, 0);
            grid.Children.Add(rb_otkaz);

            rb_otkaz.Checked += new RoutedEventHandler(no_vis_st);
            rb_norma.Checked += new RoutedEventHandler(vis_st);
            rb_probel.Checked += new RoutedEventHandler(no_vis_st);

            ST_fact_list.Children.Add(grid);
            connection.Close();
        }

        private void AddFact(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            bool is_norma = false;
            reader = connection.ReadData(string.Format("Select norma from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'  and id_doc='{2}'", id_req, iteration, id_doc));
            while (reader.Read())
                is_norma = Boolean.Parse(reader.GetString(0));
            connection.Close();
            AddFact(null,false, is_norma);
        }

        private void vis_st(object sender, RoutedEventArgs e)
        {
            RadioButton rb_norma = sender as RadioButton;
            Grid grid = (Grid)rb_norma.Parent;
            StackPanel st_ch = grid.Children[3] as StackPanel;
            st_ch.Visibility = Visibility.Visible;
        }

        private void no_vis_st(object sender, RoutedEventArgs e)
        {
            RadioButton rb_norma = sender as RadioButton;
            Grid grid = (Grid)rb_norma.Parent;
            StackPanel st_ch = grid.Children[3] as StackPanel;
            st_ch.Visibility = Visibility.Collapsed;
        }

        private void delFact(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;
            var textbox_name = grid.Children[0] as TextBox;
            StackPanel parentStack = (StackPanel)grid.Parent;
            var grids = parentStack.Children;
            grids.Remove(grid);
            //StackPanel childrenStack = (StackPanel)grid.Children[0];
            //var textbox_name = childrenStack.Children[1] as TextBox;
            if (textbox_name.Tag != null)
                delete_str += textbox_name.Tag.ToString() + ";";
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

            Label str_6_1_norma = new Label() { Content = "Введите текст нормы:", FontSize = 16, Margin = new Thickness(0, 0, 0, 0), Foreground = colortext };

            TextBox textbox_6_2 = new TextBox();
            textbox_6_2.Padding = new Thickness(1, 1, 1, 1);
            textbox_6_2.TextWrapping = TextWrapping.Wrap;
            textbox_6_2.FontSize = 16;
            textbox_6_2.AcceptsReturn = false;
            textbox_6_2.Foreground = colortext;
            textbox_6_2.Height = 75;
            textbox_6_2.BorderBrush = color_border;
            textbox_6_2.Width = 700;

            if (reader != null)
            {
                if (arr[9] != "")
                {
                    textbox_6_2.Text = arr[9];
                }
            }

            Grid.SetColumnSpan(str_6_1_norma, 2);
            Grid.SetRow(str_6_1_norma, 5);
            Grid.SetColumn(str_6_1_norma, 0);
            grid.Children.Add(str_6_1_norma);

            Grid.SetColumnSpan(textbox_6_2, 2);
            Grid.SetRow(textbox_6_2, 6);
            Grid.SetColumn(textbox_6_2, 0);
            grid.Children.Add(textbox_6_2);

            Rectangle rec = new Rectangle() { Fill = colortext, Height = 1 };
            rec.Margin = new Thickness(0, 7, 0, 7);
            Grid.SetColumnSpan(rec, 2);
            Grid.SetRow(rec, 7);
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
            rect3.Visibility = Visibility.Visible;
            back_b_2.Visibility = Visibility.Collapsed;
            page = 1;
            UpdateB_2();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void back_b_2_Click(object sender, RoutedEventArgs e)
        {
            delete_str = "";
            switch (page)
            {
                case 1:
                    label_RB.Visibility = Visibility.Collapsed;
                    STP_radioButton.Visibility = Visibility.Collapsed;
                    ST_edit_predmet.Visibility = Visibility.Visible;
                    page = 10;
                    UpdateB_10();
                    break;

                case 2:
                    ST_norma.Visibility = Visibility.Collapsed;
                    Scroll_st_norma.Visibility = Visibility.Collapsed;
                    title_norma.Visibility = Visibility.Collapsed;
                    btn.Visibility = Visibility.Collapsed;
                    Scroll_st_norma_izm.Visibility = Visibility.Collapsed;
                    title_norma_izm.Visibility = Visibility.Collapsed;

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
                        rect3.Visibility = Visibility.Visible;
                        page--;
                        UpdateB_2();
                    }
                    break;
                case 3:
                    ST_adres.Visibility = Visibility.Collapsed;
                    if (izmena_n == 0)
                    {
                        if ((iteration > 0) && (NormaOrRequest()))
                        {
                            label_RB.Visibility = Visibility.Visible;
                            STP_radioButton.Visibility = Visibility.Visible;
                            UpdateB_2();
                            page = page - 2;
                        }
                        else
                        {
                            ST_norma.Visibility = Visibility.Visible;
                            Scroll_st_norma.Visibility = Visibility.Visible;
                            title_norma.Visibility = Visibility.Visible;
                            btn.Visibility = Visibility.Visible;
                            UpdateB_2_2();
                            page--;
                        }
                    }
                    else
                    {
                        //nen
                        if (iteration > 0)
                        {
                            label_RB.Visibility = Visibility.Visible;
                            STP_radioButton.Visibility = Visibility.Visible;
                           // label_N_rb.Visibility = Visibility.Visible;
                           // STP_radioButton2.Visibility = Visibility.Visible;
                            //back_b_2.Visibility = Visibility.Hidden;
                           // rect3.Visibility = Visibility.Visible;
                            page=page-2;
                            UpdateB_2();
                        }
                        else
                        {
                            Scroll_st_norma_izm.Visibility = Visibility.Visible;
                            title_norma_izm.Visibility = Visibility.Visible;
                            UpdateB_2_2_izm();
                            page--;
                        }
                    }
                    break;

                case 4:
                    ST_explanat.Visibility = Visibility.Collapsed;
                    ST_adres.Visibility = Visibility.Visible;
                    UpdateB_2_3();
                    page--;
                    break;

                case 5:
                    page--;
                    ST_explanat.Visibility = Visibility.Visible;
                    ST_fact.Visibility = Visibility.Collapsed;
                    //Scroll_st_fact.Visibility = Visibility.Collapsed;
                    textBlock_isk.Visibility = Visibility.Visible;
                    rect2.Visibility = Visibility.Visible;
                    rect3.Visibility = Visibility.Visible;
                    // Next_b_2.Visibility = Visibility.Visible;
                    //   Save.Visibility = Visibility.Visible;
                    UpdateB2_4();
                    break;

                case 6:
                    ST_adres.Visibility = Visibility.Visible;
                    /// Next_b_2.Visibility = Visibility.Visible;
                    // Save.Visibility = Visibility.Visible;
                    ST_fact.Visibility = Visibility.Collapsed;
                    //Scroll_st_fact.Visibility = Visibility.Collapsed;
                    textBlock_isk.Visibility = Visibility.Visible;
                    rect2.Visibility = Visibility.Visible;
                    rect3.Visibility = Visibility.Visible;
                    page = 3;
                    UpdateB_2_3();
                    break;
                case 7:
                    page = 1;
                    UpdateB_2();
                    label_RB.Visibility = Visibility.Visible;
                    STP_radioButton.Visibility = Visibility.Visible;
                    label_N_rb.Visibility = Visibility.Visible;
                    STP_radioButton2.Visibility = Visibility.Visible;
                    // Next_b_2.Visibility = Visibility.Visible;
                    //Save.Visibility = Visibility.Visible;
                    ST_fact.Visibility = Visibility.Collapsed;
                    //Scroll_st_fact.Visibility = Visibility.Collapsed;
                    textBlock_isk.Visibility = Visibility.Visible;
                    rect2.Visibility = Visibility.Visible;
                    rect3.Visibility = Visibility.Visible;
                    back_b_2.Visibility = Visibility.Collapsed;
                    break;

                case 10:
                    ST_edit_predmet.Visibility = Visibility.Collapsed;
                    ST_adres.Visibility = Visibility.Visible;
                    iteration--;
                    UpdateB_2_3();
                    page = 3;
                    break;
                case 11:
                    ST_defendant_choise.Visibility = Visibility.Collapsed;
                    page = page - 6;
                    ST_fact.Visibility = Visibility.Visible;
                    UpdateB_2_5();
                    break;
                case 12:
                    goto case 11;
                case 13:
                    goto case 11;
                case 14:
                    ST_defendant_choise.Visibility = Visibility.Visible;
                    page = page - 3;
                    ST_costs.Visibility = Visibility.Collapsed;
                    Next_b_2.Content = "Далее";
                    UpdateB_567();
                    break;
                case 15:
                    goto case 14;
                case 16:
                    goto case 14;

            }
        }




        // bnthfwbz ,jkmit 0 jghtltktzv ,skj bpvtytybt jcyjdfybz bcrf bkb ghtlvtn
        // проверка формулировки (итерация больше 0) изменение предмета или основание иска
        // Если меняли нормы возращаем false
        private bool NormaOrRequest()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            reader = connection.ReadData(string.Format("Select choice from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration-1, id_doc));
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    if (reader.GetInt16(0) == 2)
                        return false;
                    else
                        return true;
                else
                    return false;
            }
            connection.Close();
            return true;
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
                        connection.WriteData(string.Format("Delete from fact_thing where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                        connection.WriteData(string.Format("Delete from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                        connection.WriteData(string.Format("Update REQUIREMENTS_TMP set adresat_norm=NULL,choice=NULL,explain_tr=NULL, defendant_choise=NULL,prizn_isk=NULL,izm_norma=NULL,court_costs=null, distribution_of_costs=null, court_of_appeal=null  Where id_req='{0}' and iteration = '{1}'  and id_doc='{2}'", id_req, iteration, id_doc));
                    }
                    connection.WriteData(string.Format("Update REQUIREMENTS_TMP set variant='{0}' Where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", variant, id_req, iteration, id_doc));
                    string norm_str = "";
                    reader = connection.ReadData(string.Format("Select norma from REQUIREMENTS_tmp where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    while (reader.Read())
                        if (!reader.IsDBNull(0))
                            norm_str = reader.GetString(0);
                    if ((bool)radioButton_N_v1.IsChecked)
                    {
                        if (norm_str == "False")
                        {
                            connection.WriteData(string.Format("Update REQUIREMENTS_TMP set adresat_norm=NULL,choice=NULL,explain_tr=NULL, defendant_choise=NULL,prizn_isk=NULL,court_costs=null, distribution_of_costs=null, court_of_appeal=null  Where id_req='{0}' and iteration = '{1}'  and id_doc='{2}'", id_req, iteration, id_doc));
                            connection.WriteData(string.Format("Delete from fact_thing where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration >= '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                        }
                        connection.WriteData(string.Format("Update REQUIREMENTS_TMP set norma='{0}' Where id_req='{1}' and iteration = '{2}'  and id_doc='{3}'", true, id_req, iteration, id_doc));
                    }
                    else
                    {
                        if (norm_str == "True")
                        {
                            connection.WriteData(string.Format("Delete from NORMA where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration >= '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                            connection.WriteData(string.Format("Delete from fact_thing where id_req in (select id from REQUIREMENTS_tmp where id_req = '{0}' and iteration >= '{1}' and id_doc = '{2}')", id_req, iteration, id_doc));
                            connection.WriteData(string.Format("Delete from REQUIREMENTS_tmp where id_req = '{0}' and iteration > '{1}' and id_doc = '{2}'", id_req, iteration, id_doc));
                            connection.WriteData(string.Format("Update REQUIREMENTS_TMP set adresat_norm=NULL,choice=NULL,explain_tr=NULL, defendant_choise=NULL,prizn_isk=NULL  Where id_req='{0}' and iteration = '{1}'  and id_doc='{2}'", id_req, iteration, id_doc));

                        }
                        connection.WriteData(string.Format("Update REQUIREMENTS_TMP set norma='{0}' Where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", false, id_req, iteration, id_doc));
                    }
                    break;

                case 2:
                    if (Scroll_st_norma.Visibility == Visibility.Visible)
                    {
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
                                var tb_text_norma = grid.Children[7] as TextBox;

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
                                str_doc += date_pick.SelectedDate.ToString().Split(' ')[0] + "~";

                                textBox_str = str_1_4_norma.Children[3] as TextBox;
                                str_doc += textBox_str.Text + "~" + tb_text_norma.Text;

                                if (id_norma == -1)
                                {
                                    reader = connection.ReadData(string.Format("Select id from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}'   and id_doc='{2}'", id_req, iteration, id_doc));
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
                    }
                    else
                    {
                        string list_norm_izm = null;
                        foreach (CheckBox check in ST_norma_izm.Children)
                        {
                            if ((bool)check.IsChecked)
                                list_norm_izm += check.Tag.ToString() + ";";
                        }
                        if (list_norm_izm == null)
                            MessageBox.Show("Ни одна норма не выбрана!");
                        else
                        {
                            list_norm_izm = list_norm_izm.Remove(list_norm_izm.Length-1);
                            connection.WriteData(string.Format("Update REQUIREMENTS_TMP set izm_norma='{0}' Where id_req='{1}' and iteration='{2}'  and id_doc='{3}'", list_norm_izm, id_req, iteration, id_doc));
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
                    reader = connection.ReadData(string.Format("Select adresat_norm from REQUIREMENTS_TMP where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
                    int adr_norm = adres * 10 + norm;
                    while (reader.Read())
                        if (!reader.IsDBNull(0))
                            adr_norm = reader.GetInt16(0);
                    if (adr_norm!= adres * 10 + norm)
                    {
                        connection.WriteData(string.Format(" delete from NORMA where id_req in ( select id from REQUIREMENTS_TMP Where id_req='{0}' and iteration>'{1}' and id_doc='{2}')", id_req, iteration, id_doc));
                        connection.WriteData(string.Format(" Delete from REQUIREMENTS_TMP Where id_req='{0}' and iteration>'{1}' and id_doc='{2}'",  id_req, iteration, id_doc));
                    }
                    connection.WriteData(string.Format("Update REQUIREMENTS_TMP set adresat_norm='{0}' Where id_req='{1}' and iteration='{2}' and id_doc='{3}'", adres*10+norm, id_req, iteration,id_doc));
                    break;

                case 4:
                    if (textBox.Text.Length == 0)
                    {
                        MessageBox.Show("Заполните поле!");
                    }
                    else
                    {
                        connection.WriteData(string.Format("Update REQUIREMENTS_TMP set explain_tr='{0}' where id_doc='{1}' and id_req='{2}' and iteration='{3}'", textBox.Text, id_doc, id_req, iteration));
                    }
                    break;

                case 5:
                    goto case 7;

                case 6:
                    goto case 7;

                case 7:
                    if (delete_str != "")
                    {
                        string[] arr = delete_str.Split(';');
                        for (int i = 0; i < arr.Length - 1; i++)
                            connection.WriteData(string.Format("Delete from fact_thing where id = '{0}'",arr[i]));
                    }
                    foreach (Grid grids in ST_fact_list.Children)
                    {
                        int choise_f = 0;
                        TextBox tb = grids.Children[0] as TextBox;
                        if (tb.Text.Length != 0)
                        {
                            RadioButton rb_1 = grids.Children[2] as RadioButton;
                            StackPanel st_f = grids.Children[3] as StackPanel;
                            RadioButton rb_2 = grids.Children[4] as RadioButton;
                            RadioButton rb_3 = grids.Children[5] as RadioButton;


                            if (!(bool)rb_1.IsChecked)
                            {
                                if ((bool)rb_2.IsChecked)
                                    choise_f = 2;
                                else
                                    choise_f = 3;
                                if (tb.Tag != null)
                                {
                                    connection.WriteData(string.Format("Update fact_thing set text='{0}', choise='{1}', list_norm=NULL  where id='{2}'", tb.Text, choise_f, tb.Tag));
                                }
                                else
                                {
                                    connection.WriteData(string.Format("Insert into fact_thing (id_req,text,choise) values ('{0}','{1}','{2}')",id,tb.Text,choise_f));
                                }
                            }
                            else
                            {
                                string list_f = "";
                                choise_f = 1;
                                foreach (CheckBox check in st_f.Children)
                                {
                                    if ((bool)check.IsChecked)
                                        list_f += check.Tag.ToString() + ";";
                                }
                                list_f = list_f.Remove(list_f.Length - 1);

                                if (tb.Tag != null)
                                {
                                    connection.WriteData(string.Format("Update fact_thing set text='{0}', choise='{1}', list_norm='{2}'  where id='{3}'", tb.Text, choise_f, list_f, tb.Tag));
                                }
                                else
                                {
                                    connection.WriteData(string.Format("Insert into fact_thing (id_req,text,choise,list_norm) values ('{0}','{1}','{2}','{3}')", id, tb.Text, choise_f,list_f));
                                }
                            }
                            
                        }
                    }

                    ST_fact_list.Children.Clear();
                    UpdateB_2_5();
                    break;

                case 10:
                    if (textBox_edit.Text.Length==0)
                    {
                        MessageBox.Show("Заполните поле формулировки");
                    }
                    else
                    {
                        SQLite connection10 = new SQLite();
                        reader = connection10.ReadData(string.Format("Select count(*) from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration='{2}'", id_doc, id_req, iteration));
                        var count_req = 0;
                        while (reader.Read())
                            count_req = reader.GetInt16(0);
                        if (count_req==0)
                            connection10.WriteData(string.Format("Insert into REQUIREMENTS_TMP (id_doc,id_req,iteration,text,norma,izmena) values ('{0}','{1}','{2}','{3}','{4}','{5}')", id_doc,id_req, iteration,textBox_edit.Text,false,izmena_n));
                        else
                            connection10.WriteData(string.Format("Update REQUIREMENTS_TMP set text='{0}' where id_doc='{1}' and id_req='{2}' and iteration='{3}'", textBox_edit.Text,id_doc, id_req, iteration));
                        connection10.Close();
                    }
                    break;
                case 11:
                    int ch_d = 1, priz_isk =1;
                    if ((bool)def_ch_2.IsChecked)
                        ch_d = 2;
                    else if ((bool)def_ch_3.IsChecked)
                        ch_d = 3;
                    if ((bool)priz_isk_2.IsChecked)
                        priz_isk = 2;
                    else if ((bool)priz_isk_3.IsChecked)
                        priz_isk = 3;
                    SQLite connection11 = new SQLite();
                    connection11.WriteData(string.Format("Update REQUIREMENTS_TMP set defendant_choise='{0}', prizn_isk='{1}' where id_doc='{2}' and id_req='{3}' and iteration='{4}'", ch_d, priz_isk, id_doc, id_req, iteration));
                    connection11.Close();
                    break;
                case 12:
                    goto case 11;
                case 13:
                    goto case 11;
                case 14:
                    SQLite connection14 = new SQLite();
                    connection.WriteData(string.Format("Update REQUIREMENTS_TMP set court_costs='{0}', distribution_of_costs='{1}', court_of_appeal='{2}' where id_doc='{3}' and id_req='{4}' and iteration='{5}'", textBox_court_costs.Text, textBox_distribution_of_costs.Text, textBox_court_of_appeal.Text, id_doc, id_req, iteration));
                    connection14.Close();
                    break;
                case 15:
                    goto case 14;
                case 16:
                    goto case 14;

            }
            connection.Close();
        }
    }
}
