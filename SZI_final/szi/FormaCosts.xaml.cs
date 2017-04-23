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
using System.Linq;

namespace Lazer
{
    /// <summary>
    /// Логика взаимодействия для FormaCosts.xaml
    /// </summary>
    public partial class FormaCosts : Window
    {
        private string name;
        private readonly ForaWindow fw;
        private int id_doc;
        public FormaCosts(string name, ForaWindow fw,int id_doc)
        {
            this.id_doc = id_doc;
            this.name = name;
            this.fw = fw;
            InitializeComponent();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            fw.ForaWindow_Update();
            fw.Visibility = Visibility.Visible;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            reader = connection.ReadData(string.Format("Select court_costs,distribution_of_costs,court_of_appeal from document where id='{0}'",id_doc));
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                    textBox_court_costs.Text = reader.GetString(0);
                if (!reader.IsDBNull(1))
                    textBox_distribution_of_costs.Text = reader.GetString(0);
                if (!reader.IsDBNull(2))
                    textBox_court_of_appeal.Text = reader.GetString(0);
            }
            connection.Close();

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (textBox_court_costs.Text.Trim().Length == 0)
                MessageBox.Show("Заполните информацию о судебных расходах истца!");
            else if (textBox_distribution_of_costs.Text.Trim().Length == 0)
                MessageBox.Show("Заполните информацию о распрделении судебных расходов!");
            else if (textBox_court_of_appeal.Text.Trim().Length == 0)
                MessageBox.Show("Заполните информацию об апелляционном суде!");
            else
            {
                SQLite connection = new SQLite();
                connection.WriteData(string.Format("Update document set court_costs='{0}',distribution_of_costs='{1}',court_of_appeal='{2}' where id='{3}'", textBox_court_costs.Text, textBox_distribution_of_costs.Text, textBox_court_of_appeal.Text,id_doc));
                connection.Close();
            }
        }
    }
}
