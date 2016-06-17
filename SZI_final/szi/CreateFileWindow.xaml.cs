using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SZI
{
    /// <summary>
    /// Логика взаимодействия для CreateFilePage.xaml
    /// </summary>
    public partial class CreateFileWindow : Window
    {

        private readonly MainWindow mainWindow;

        public CreateFileWindow(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        /* Создать новый файл и перейти к его редактиованию */
        private void CreateFileBtn_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            connection.WriteData(string.Format("INSERT INTO Document (Name) VALUES ('{0}')", NewFileName.Text));
            mainWindow.AddItemInList(NewFileName.Text);
            connection.Close();
            Close();
        }
    }
}
