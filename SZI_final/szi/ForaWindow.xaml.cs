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
            fw = this;
            this.startForm = startForm;
            this.name = name;
            this.id = id;
            ForaWindow1.Title += " «" + name+"»";
        }
        string name;

        private void FormaA_Click(object sender, RoutedEventArgs e)
        {
            FormaA AForm = new FormaA(name,fw,0,id);
            AForm.Owner = this;
            AForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AForm.ShowDialog();
        }

        private void ForaWindow1_Load(object sender, RoutedEventArgs e)
        {

            //startForm.Close();
        }
    }
}
