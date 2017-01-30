//using System;
using System.Collections.Generic;
using System.Data.Common;
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

        Microsoft.Office.Interop.Word.Application wrdApp;
        Microsoft.Office.Interop.Word._Document wrdDoc;
        System.Object oMissing = System.Reflection.Missing.Value;
        System.Object oFalse = false;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            string StrToAdd;
            SQLiteDataReader reader_actor;
            SQLiteDataReader reader = connection.ReadData(string.Format("Select NAME,NUMBER,DATE,LOCATE,NAME_COURT,CONTENT_COURT,SECRETARY,PROSECUTOR,PUBLIC_MEETING from document Where id='{0}'", id));
            while (reader.Read())
            {

            
            // Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
            //app.Visible = true;
            //app.Documents.Add();
            Microsoft.Office.Interop.Word.Selection wrdSelection;
            Microsoft.Office.Interop.Word.MailMerge wrdMailMerge;
            Microsoft.Office.Interop.Word.MailMergeFields wrdMergeFields;
            Microsoft.Office.Interop.Word.Table wrdTable;
            wrdApp = new Microsoft.Office.Interop.Word.Application();
            wrdApp.Visible = true;

            // Add a new document.
            wrdDoc = wrdApp.Documents.Add(ref oMissing, ref oMissing,
                ref oMissing, ref oMissing);
            wrdDoc.Select();

            wrdSelection = wrdApp.Selection;
            wrdMailMerge = wrdDoc.MailMerge;

                // Create a string and insert it into the document.    
                //wrdSelection.ParagraphFormat.SpaceAfter = 0;
                //var StrToAdd = "Заявка на проведение закупки № \r\nот " + System.DateTime.Now.ToString("dd.MM.yyyy");
                //wrdSelection.ParagraphFormat.Alignment =
                //     Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                //wrdSelection.Font.Bold = 1;
                //wrdSelection.Font.Size = 16;
                //wrdSelection.Font.Name = "Times New Roman";
                //wrdSelection.ParagraphFormat.LineSpacing = 11;
                //wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                // wrdSelection.TypeText(StrToAdd);
                
                wrdSelection.ParagraphFormat.SpaceAfter = 0;
                StrToAdd = "Дело № " + reader.GetString(1)+ "\r\n";
                wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                //wrdSelection.Font.Bold = 1;
                wrdSelection.Font.Size = 14;
                wrdSelection.Font.Name = "Times New Roman";
                wrdSelection.ParagraphFormat.LineSpacing = 11;
                wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                wrdSelection.TypeText(StrToAdd);

                StrToAdd = "Р Е Ш Е Н И Е" + "\r\n";
                wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                wrdSelection.Font.Bold = 1;
                wrdSelection.Font.Size = 14;
                wrdSelection.Font.Name = "Times New Roman";
                wrdSelection.ParagraphFormat.LineSpacing = 11;
                wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                wrdSelection.TypeText(StrToAdd);

                StrToAdd = "Именем Российской Федерации" + "\r\n" + "\r\n";
                wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                wrdSelection.Font.Bold = 0;
                wrdSelection.Font.Size = 14;
                wrdSelection.Font.Name = "Times New Roman";
                wrdSelection.ParagraphFormat.LineSpacing = 11;
                wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                wrdSelection.TypeText(StrToAdd);

                if (!reader.IsDBNull(2)&&reader.GetString(2).Length>0)
                {
                    var str_date = reader.GetString(2).Split(' ')[0];
                    var month = int.Parse(str_date.Split('.')[1]);
                    StrToAdd = "«" + str_date.Split('.')[0] + "» " + GetMonth(month) + str_date.Split('.')[2] + " года";
                    if (!reader.IsDBNull(3) && reader.GetString(3).Length > 0)
                    {
                        StrToAdd += "                         " + reader.GetString(3) + "\r\n" + "\r\n";
                    }
                    else
                        StrToAdd += "\r\n" + "\r\n";
                    wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphLeft;
                    wrdSelection.Font.Bold = 0;
                    wrdSelection.Font.Size = 14;
                    wrdSelection.Font.Name = "Times New Roman";
                    wrdSelection.ParagraphFormat.LineSpacing = 11;
                    wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                    wrdSelection.TypeText(StrToAdd);
                }

                if (!reader.IsDBNull(4) && reader.GetString(4).Length > 0)
                {
                    StrToAdd = reader.GetString(4)+ " в составе" + "\r\n";            
                }

                if (!reader.IsDBNull(5) && reader.GetString(5).Length > 0)
                {
                    if (reader.GetString(5).Contains(","))
                        StrToAdd += "судей ";
                    else
                        StrToAdd += "судьи ";
                    StrToAdd += reader.GetString(5) + "\r\n";
                }
                if (!reader.IsDBNull(6) && reader.GetString(6).Length > 0)
                {
                    StrToAdd += "при секретаре "+reader.GetString(6) + "\r\n";
                }

                wrdSelection.TypeText(StrToAdd);
            }
            //записываем истцов или истца
            int count_actor = 0;
            reader = connection.ReadData(string.Format("Select count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=1", id));
            while (reader.Read())
            {
                count_actor = reader.GetInt16(0);
            }

            if (count_actor == 1)
            {
                StrToAdd = "с участием истца";
            }
            else
            {
                StrToAdd = "с участием истцов";
            }
            reader = connection.ReadData(string.Format("Select id,NAME_ACTOR_RP,ACTOR_DOC from ACTORS Where id_doc='{0}' and PLAINTIFF=1", id));

            string pred_istec = null ;

            foreach (DbDataRecord record in reader)
            {
                pred_istec = null;
                StrToAdd += " " + record["NAME_ACTOR_RP"].ToString()+",";
                reader_actor = connection.ReadData(string.Format("Select count(*) from AGENT_PLAINTIFF Where ID_ACTOR='{0}'", record["ID"].ToString()));
                while (reader_actor.Read())
                {
                    count_actor = reader.GetInt16(0);
                }

                if (count_actor == 0)
                {
                    break;
                }
                else if (count_actor == 1)
                {
                    pred_istec = "представителя истца "+ record["NAME_ACTOR_RP"].ToString() + "\r\n";
                }
                else
                {
                    pred_istec = "представителей истца " + record["NAME_ACTOR_RP"].ToString() + "\r\n";
                }
                reader_actor = connection.ReadData(string.Format("Select NAME_AGENT_RP,AGENT_DOC from AGENT_PLAINTIFF Where ID_ACTOR='{0}'", record["ID"].ToString()));

                foreach (DbDataRecord record_actor in reader_actor)
                {
                    pred_istec += record_actor["NAME_AGENT_RP"].ToString();
                    if(record_actor["AGENT_DOC"].ToString().Length>8)
                    {
                        pred_istec += ", действующего";
                    }
                }


            }
            StrToAdd = StrToAdd.Remove(StrToAdd.Length - 1) + ".";

            //    while (reader.Read())
            //{

            //}

            connection.Close();
        }


        private string GetMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "января ";

                case 2:
                    return "февраля ";

                case 3:
                    return "марта ";

                case 4:
                    return "апреля ";

                case 5:
                    return "мая ";

                case 6:
                    return "июня ";

                case 7:
                    return "июля ";

                case 8:
                    return "августа ";

                case 9:
                    return "сентября ";

                case 10:
                    return "октября ";

                case 11:
                    return "ноября ";

                case 12:
                    return "декабря ";
                    
            }
            return "0";
        }
    }
}
