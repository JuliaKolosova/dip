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
    /// Логика взаимодействия для CreateReqWindow.xaml
    /// </summary>
    public partial class CreateReqWindow : Window
    {
        int id_doc;
        private readonly FormaB B_Window;
        public CreateReqWindow(int id_doc, FormaB B_Window)
        {
            this.B_Window = B_Window;
            this.id_doc = id_doc;
            InitializeComponent();
        }

        private void CreateReqBtn_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            int id_req = 0;
            SQLiteDataReader reader = connection.ReadData(string.Format("select max(id_req) from REQUIREMENTS_TMP where id_doc='{0}'", id_doc));
            while (reader.Read())
                id_req = reader.GetInt16(0) + 1;
            connection.WriteData(string.Format("INSERT INTO REQUIREMENTS_TMP (id_req,id_doc,iteration,text,izmena) VALUES ('{0}','{1}',0,'{2}',1)", id_req, id_doc, NewReqName.Text));
            connection.Close();
            B_Window.FullListBox(id_doc);
            Close();
        }


    }
}
