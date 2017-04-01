using System;
using System.Collections.Generic;
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
using System.Data.SQLite;

namespace SZI
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    
    public partial class ErrorNorma : Window
    {
        private int id_req, id_doc;
        private int iteration;
        public int variant = 0;
        public ErrorNorma(int id_req, int iteration, int id_doc)
        {
            this.id_doc = id_doc;
            this.id_req = id_req;
            this.iteration = iteration;
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (var_1.IsChecked==true)
            {
                variant = 1;
            }
            else if (var_2.IsChecked == true)
            {
                variant = 2;
            }
            else
            {
                variant = 3;
            }
            Close();
        }

        private void ErrorNorma1_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select choice,izmena from REQUIREMENTS_tmp where id_req='{0}' and iteration='{1}' and id_doc='{2}'", id_req, iteration, id_doc));
            int choise = 0, izm = 0;
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    choise = reader.GetInt16(0);
                if (!reader.IsDBNull(1))
                    izm = reader.GetInt16(1);
            }

             
                switch (choise)
                {
                    case 1:
                        var_1.IsChecked = true;
                        break;
                    case 2:
                        var_2.IsChecked = true;
                        break;
                    case 3:
                        var_3.IsChecked = true;
                        break;
                }

                if (izm == 0)
            {
                if (iteration > 0)

                {
                    reader = connection.ReadData(string.Format("Select choice from REQUIREMENTS_tmp where iteration=0 and id_doc='{0}'", id_doc));
                    while (reader.Read())
                        if (!reader.IsDBNull(0))
                            if (reader.GetInt16(0) != 1)
                                choise = reader.GetInt16(0);
                    switch (choise)
                    {
                        case 2:
                            var_3.IsEnabled = false;
                            break;
                        case 3:
                            var_2.IsEnabled = false;
                            break;
                    }
                }
                }
                else
                {
                    var_2.IsEnabled = false;
                }
                
            
            connection.Close();
        }
    }
}
