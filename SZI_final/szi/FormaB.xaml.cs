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
    /// Логика взаимодействия для FormaB.xaml
    /// </summary>
    public partial class FormaB : Window
    {
        private string name;
        int id;
        private  FormaB fw_B;
        public FormaB(string name, int id)
        {
            InitializeComponent();
            this.fw_B = this;
            this.name = name;
            this.id = id;
            Forma_B.Title += " «" + name + "»";
            FullListBox(id);

        }

        private void FullListBox(int id)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select id_req, text from REQUIREMENTS_TMP Where id_doc='{0}' and iteration=0", id));
            
            listBox_isk.Items.Clear();

            while (reader.Read())
            {
                ListBoxItem list = new ListBoxItem();
                list.Tag = reader.GetInt16(0).ToString();
                list.Content = reader.GetString(1);
                listBox_isk.Items.Add(list);
            }
            connection.Close();
        }

        private void OpenBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_isk.SelectedIndex==-1)
            {
                MessageBox.Show("Исковое требование не выбрано!");
            }
            else
            {
                ListBoxItem list_isk = listBox_isk.SelectedItem as ListBoxItem;
                int id_req = Int16.Parse(list_isk.Tag.ToString());
                OpenFormaB_2(id_req);
            }
        }

        private void listBox_isk_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ListBoxItem list_isk = listBox_isk.SelectedItem as ListBoxItem;
            int id_req = Int16.Parse(list_isk.Tag.ToString());
            OpenFormaB_2(id_req);
        }

        private void OpenFormaB_2(int id_req)
        {
            fw_B.Visibility = Visibility.Hidden;
            FormaB_2 eForm = new FormaB_2(fw_B, id_req,id);
            eForm.Owner = this;
            eForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            eForm.ShowDialog();
        }
    }
}
