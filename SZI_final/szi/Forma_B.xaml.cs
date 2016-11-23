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
    /// Логика взаимодействия для Forma_B.xaml
    /// </summary>
    public partial class Forma_B : Window
    {
        private FormaB fw_b;
        private int id_req, id_doc, id;
        //id - уникальный ключ в таблице формулировок
        private SolidColorBrush colortext = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FF378B1E"));
        private SolidColorBrush colortext2 = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFEA2727"));
        private SolidColorBrush color_border = new System.Windows.Media.SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFDBFFD0"));
        string delete_str;
        int page = 1;
        int iteration = 0;
        public Forma_B(FormaB fw_b, int id_req, int id_doc)
        {
            this.fw_b = fw_b;
            this.id_req = id_req;
            this.id_doc = id_doc;

            delete_str = "";
            InitializeComponent();
        }
        private void back_b_2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Next_b_2_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            back_b_2.Visibility = Visibility.Collapsed;
            page = 1;
            UpdateB_2();

        }

        private void UpdateB_2()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select text,variant,norma from REQUIREMENTS_TMP where id_doc='{0}' and id_req='{1}' and iteration=0", id_doc, id_req));
            while (reader.Read())
            {
                textBlock_isk.Text = reader.GetString(0);
                if (!reader.IsDBNull(1))
                {
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
                    if (reader.GetString(2) == "True")
                    {
                        radioButton_N_v1.IsChecked = true;
                    }
                    else
                    {
                        radioButton_N_v2.IsChecked = true;
                    }
                }
            }
            connection.Close();
        }
    }
}
