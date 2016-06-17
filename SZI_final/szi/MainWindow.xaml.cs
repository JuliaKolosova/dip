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

namespace SZI
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindow startForm;
        public int id;
        public MainWindow()
        {
            InitializeComponent();
            startForm = this;
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("SELECT ID, NAME FROM document"));
            while (reader.Read())
            {
                id = reader.GetInt32(0);
                FilesListBox.Items.Add(reader.GetString(1));
                //clientsList.Items.Add(new Client(reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetString(3)));
            }
            connection.Close();
        }

        /* Создание нового файла xml (кнопка и меню) */
        private void CreateBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateFileWindow cForm = new CreateFileWindow(startForm);
            cForm.Owner = this;
            cForm.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            cForm.ShowDialog();
        }

        /* Открытие файла xml (кнопка) */
        private void SelectBtn_Click(object sender, RoutedEventArgs e)
        {
            if (FilesListBox.SelectedItem != null)
            {
                ForaWindow eForm = new ForaWindow(FilesListBox.SelectedItem.ToString(), startForm,id);
                eForm.Owner = this;
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
            if (FilesListBox.SelectedItem != null)
            {
                ForaWindow eForm = new ForaWindow(FilesListBox.SelectedItem.ToString(), startForm,id);
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
    }
}
