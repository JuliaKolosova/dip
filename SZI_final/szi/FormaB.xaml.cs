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
        private readonly ForaWindow fw;
        private  FormaB fw_B;
        public FormaB(string name, int id, ForaWindow fw)
        {
            InitializeComponent();
            this.fw = fw;
            this.fw_B = this;
            this.name = name;
            this.id = id;
            Forma_B.Title += " «" + name + "»";
            FullListBox(id);
        }
        
        public void InsertReq(string name)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("select max(id_req) from REQUIREMENTS_TMP where id_doc='{0}'", id));
            int id_req = 0;
            while (reader.Read())
                id_req = reader.GetInt16(0) + 1;
            connection.Close();
            ListBoxItem list = new ListBoxItem();
            list.Tag = id_req.ToString();
            list.Content = "(изм.) " + name;
            listBox_isk.Items.Add(list);
            //SQLite connection2 = new SQLite();
            //connection2.WriteData(string.Format("INSERT INTO REQUIREMENTS_TMP (id_req,id_doc,iteration,text,izmena) VALUES ('{0}','{1}',0,'{2}',1)", id_req, id, name));
            //connection2.Close();
            //FullListBox(id);
        }

        public void FullListBox(int id)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select id_req, text, izmena from REQUIREMENTS_TMP Where id_doc='{0}' and iteration=0 order by izmena", id));
            
            listBox_isk.Items.Clear();

            while (reader.Read())
            {

                ListBoxItem list = new ListBoxItem();
                list.Tag = reader.GetInt16(0).ToString();
                if (reader.GetInt16(2)==0)
                    list.Content = reader.GetString(1);
                else
                    list.Content = "(изм.) "+reader.GetString(1);
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
            //SQLite connection = new SQLite();
            //SQLiteDataReader reader = connection.ReadData(string.Format("select count(*) from REQUIREMENTS_TMP where id_req='{0}'", id_req));
            //int count = 0;
            //while (reader.Read())
            //    count = reader.GetInt16(0);

            //if (count == 0)
            //{
            //    ListBoxItem list_isk = listBox_isk.SelectedItem as ListBoxItem;
            //    string text = list_isk.Content.ToString();
            //    connection.WriteData(string.Format("", id_req, id,text));
            //}
            //connection.Close();
            fw_B.Visibility = Visibility.Hidden;
            FormaB_2 eForm = new FormaB_2(fw_B, id_req,id);
            eForm.Owner = this;
            eForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            eForm.ShowDialog();
        }

        private void edit_isk_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            bool visible = true;
            SQLiteDataReader reader = connection.ReadData(string.Format("select case when ((defendant_choise is null) or (prizn_isk is null)) then 0 else 1 end from REQUIREMENTS_TMP where id_doc='{0}'", id));
            while (reader.Read() && visible)
            {
                if (reader.GetInt16(0) == 0)
                    visible = false;
            }

            if (!visible)
            {
                MessageBox.Show("С начала заполните всю информацию по формулировкам!");
            }
            else
            {
                reader = connection.ReadData(string.Format("select count(*) from REQUIREMENTS_TMP where id_doc='{0}' and choice=2", id));
                int cnt = 0;
                while (reader.Read())
                {
                    cnt = reader.GetInt16(0);
                }
                if (cnt == 0)
                {
                    //CreateRequestWindow FW_CreateReq = new CreateRequestWindow();
                    CreateReqWindow fw_createReq = new CreateReqWindow(id, fw_B);
                    fw_createReq.Owner = this;
                    fw_createReq.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    fw_createReq.ShowDialog();

                }
                else
                {
                    MessageBox.Show("Было изменение основания иска, изменение предмета недопустимо!");
                }
            }
            connection.Close();
        }

        private void Forma_B_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = false;
            fw.Visibility = Visibility.Visible;
            fw.ForaWindow_Update();
        }

        private void del_edit_isk_Click(object sender, RoutedEventArgs e)
        {
            ListBoxItem list_isk = listBox_isk.SelectedItem as ListBoxItem;
            int id_req = Int16.Parse(list_isk.Tag.ToString());
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("select izmena from REQUIREMENTS_TMP where id_req='{0}'", id_req));
            int izm = 0;
            while (reader.Read())
                izm = reader.GetInt16(0);
            if (izm==0)
            {
                MessageBox.Show("Удалить можно только измененные требования!");
            }
            else
            {
                MessageBoxResult res = MessageBox.Show("Вы уверены что хотите удалить требование " + list_isk.Content.ToString() + "?", "Внимание", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {
                    connection.WriteData(string.Format("Delete from Norma n where n.id_req in (SELECT r.id FROM REQUIREMENTS_TMP r, (select s.id_req, s.id_doc,s.iteration from REQUIREMENTS_TMP s where s.id='{0}' ) g where r.id_doc = g.id_doc and r.id_req = g.id_req and r.iteration >= g.iteration)", id_req));
                    connection.WriteData(string.Format("Delete from REQUIREMENTS_TMP r where r.id  in (SELECT r.id FROM REQUIREMENTS_TMP r, (select s.id_req, s.id_doc,s.iteration from REQUIREMENTS_TMP s where s.id='{0}' ) g where r.id_doc = g.id_doc and r.id_req = g.id_req and r.iteration >= g.iteration)", id_req));
                }


                }
            }
    }
}
