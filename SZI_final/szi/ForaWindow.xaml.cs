//using System;
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
    /// Логика взаимодействия для ForaWindow.xaml
    /// </summary>
    public partial class ForaWindow : Window
    {
        private ForaWindow fw;
        private readonly MainWindow startForm;
        int id;
        public ForaWindow(string name, MainWindow startForm, int id)
        {
            InitializeComponent();
            this.fw = this;
            this.startForm = startForm;
            this.name = name;
            this.id = id;
            ForaWindow1.Title += " «" + name+"»";
            
        }
        string name;

        private void FormaA_Click(object sender, RoutedEventArgs e)
        {
            fw.Visibility = Visibility.Hidden;
            FormaA AForm = new FormaA(name,fw,id);
            AForm.Owner = this;
            AForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AForm.ShowDialog();
            
        }
        public void ForaWindow_Update()
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_tmp Where id_doc='{0}' and iteration=0", id));
            int index = 0;
            while (reader.Read())
                index = reader.GetInt16(0);
            connection.Close();
            if (index == 0)
            {
                FormaB.IsEnabled = false;
            }
            else
            {
                FormaB.IsEnabled = true;
            }
        }

        private void ForaWindow1_Load(object sender, RoutedEventArgs e)
        {
            ForaWindow_Update();
        }

        private void FormaB_Click(object sender, RoutedEventArgs e)
        {
            FormaB BForm = new FormaB(name, id);
            BForm.Owner = this;
            BForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            BForm.ShowDialog();
        }
    }
}
