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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lazer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindow startForm;
        public int[] id;
        public MainWindow()
        {
            InitializeComponent();
            startForm = this;
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            reader = connection.ReadData(string.Format("SELECT ID, NAME FROM document order by name"));
            while (reader.Read())
            {
                ListBoxItem list = new ListBoxItem();
                list.Tag = reader.GetInt16(0).ToString();
                list.Content = reader.GetString(1);
                FilesListBox.Items.Add(list);
                //clientsList.Items.Add(new Client(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            connection.Close();
        }

        /* Создание нового файла xml (кнопка и меню) */
        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateFileWindow cForm = new CreateFileWindow(startForm);
            cForm.Owner = this;
            //startForm.Visibility = Visibility.Hidden;
            cForm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            cForm.ShowDialog();
            
            SQLite connection = new SQLite();
            SQLiteDataReader reader;
            reader = connection.ReadData(string.Format("SELECT ID, NAME FROM document order by name"));
            FilesListBox.Items.Clear();
            while (reader.Read())
            {
                ListBoxItem list = new ListBoxItem();
                list.Tag = reader.GetInt16(0).ToString();
                list.Content = reader.GetString(1);
                FilesListBox.Items.Add(list);
                //clientsList.Items.Add(new Client(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            connection.Close();

        }

        /* Открытие файла xml (кнопка) */
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListBox.SelectedIndex != -1)
            {
                ListBoxItem list_isk = FilesListBox.SelectedItem as ListBoxItem;
                int id_doc = Int16.Parse(list_isk.Tag.ToString());
                ForaWindow eForm = new ForaWindow(list_isk.Content.ToString(), startForm, id_doc);
                eForm.Owner = this;
                startForm.Visibility = Visibility.Collapsed;
                eForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                eForm.ShowDialog();
                
            }
            else
            {
                MessageBox.Show("Ошибка! Не выбрано дело!");
            }
        }

        /* Открытие файла xml (двойной клик на название файла) */
        private void FilesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (FilesListBox.SelectedIndex != -1)
            {
                ListBoxItem list_isk = FilesListBox.SelectedItem as ListBoxItem;
                int id_doc = Int16.Parse(list_isk.Tag.ToString());
                ForaWindow eForm = new ForaWindow(list_isk.Content.ToString(), startForm, id_doc);
                startForm.Visibility = Visibility.Hidden;
                eForm.Owner = this;
                eForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                eForm.ShowDialog();
                
            }
        }

        /* Открытие формы выбора файла */
        private void OpenMenuBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        /* Выход из приложения (меню) */
        private void ExitMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        

        /* добавление позиции в listbox */
        public void AddItemInList(string name)
        {
            FilesListBox.Items.Add(name);
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            if(FilesListBox.SelectedIndex != -1)
            {
                ListBoxItem list_isk = FilesListBox.SelectedItem as ListBoxItem;
                int id_doc = Int16.Parse(list_isk.Tag.ToString());
                MessageBoxResult res = MessageBox.Show("Вы уверены что хотите удалить дело "+ list_isk.Content.ToString() + "?", "Внимание", MessageBoxButton.YesNo);
                if (res == MessageBoxResult.Yes)
                {

                    SQLite connection = new SQLite();
                    connection.WriteData(string.Format("delete from AGENT_PLAINTIFF where id_actor in (select ID from Actors Where id_doc ='{0}')", id_doc));
                    connection.WriteData(string.Format("delete from Actors Where id_doc ='{0}'", id_doc));
                    connection.WriteData(string.Format("delete from NORMA Where id_req in (select ID from REQUIREMENTS_TMP Where id_doc ='{0}')", id_doc));
                    connection.WriteData(string.Format("delete from fact_thing Where id_req in (select ID from REQUIREMENTS_TMP Where id_doc ='{0}')", id_doc));
                    connection.WriteData(string.Format("delete from REQUIREMENTS_TMP Where id_doc ='{0}'", id_doc));
                    connection.WriteData(string.Format("delete from document Where id ='{0}'", id_doc));

                    FilesListBox.Items.Remove(FilesListBox.SelectedItem);
                    MessageBox.Show("Удалено!");
                }
                
            }
            else
            {
                MessageBox.Show("Ошибка! Не выбрано дело!");
            }
        }
    }
}
