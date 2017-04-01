//using System;
using System;
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
            ForaWindow_Update();
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
            int index = 0, cnt_p=0,cnt_a=0;
            while (reader.Read())
                index = reader.GetInt16(0);
            reader = connection.ReadData(string.Format("Select count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=1", id));
            while (reader.Read())
            {
                cnt_p = reader.GetInt16(0);
            }
            reader = connection.ReadData(string.Format("Select count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=2", id));
            while (reader.Read())
            {
                cnt_a = reader.GetInt16(0);
            }
            reader = connection.ReadData(string.Format("Select DATE from document Where id='{0}'", id));
            bool nach_A = false;
            while (reader.Read())
            {
                if (reader.IsDBNull(0))
                    nach_A = true;
            }
                if (index == 0 || cnt_a == 0 || cnt_p == 0 || nach_A)
            {
                FormaB.ToolTip = "Нет формулировок";
                FormaB.IsEnabled = false;
            }
            else
            {
                FormaB.IsEnabled = true;
            }
            bool exp_enable = true;
            reader = connection.ReadData(string.Format("Select r.prizn_isk, r.defendant_choise from REQUIREMENTS_tmp r Where r.id_doc='{0}' and r.iteration=(select max(t.iteration) from REQUIREMENTS_tmp t Where r.id_doc=t.id_doc and r.id_req=t.id_req)", id));
            while (reader.Read())
                if (reader.IsDBNull(0))
                    exp_enable = false;
            connection.Close();
            if (exp_enable && index != 0)
                export.IsEnabled = true;
            else
            {
                export.IsEnabled = false;                
            }
           
        }

        private void ForaWindow1_Load(object sender, RoutedEventArgs e)
        {
            ForaWindow_Update();
        }

        private void FormaB_Click(object sender, RoutedEventArgs e)
        {
            FormaB BForm = new FormaB(name, id, fw);
            BForm.Owner = this;
            BForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            BForm.ShowDialog();
        }

        Microsoft.Office.Interop.Word.Application wrdApp;
        Microsoft.Office.Interop.Word._Document wrdDoc;
        System.Object oMissing = System.Reflection.Missing.Value;
        System.Object oFalse = false;

        static string ShortName(string fio)
        {
            string[] str = fio.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (str.Length != 3) throw new ArgumentException("ФИО задано в неверно формате");
            return string.Format("{0} {1}. {2}.", str[0], str[1][0], str[2][0]);
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            SQLite connection = new SQLite();
            SQLiteDataReader reader_actor;
            SQLiteDataReader reader, reader_fact, reader_fact_norm;
            string StrToAdd="", mess_err="";
            bool not_inf = false;
            int count_actor = 0, count_actor_2 = 0;
            reader = connection.ReadData(string.Format("Select count(*) from ACTORS Where id_doc='{0}'", id));
            while (reader.Read())
            {
                count_actor = reader.GetInt16(0);

            }
            if (count_actor != 0)
            {
                reader = connection.ReadData(string.Format("Select PLAINTIFF,count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=1 union all  Select PLAINTIFF,count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=2", id));
                while (reader.Read())
                {

                    if (reader.GetInt16(0) == 1)
                        count_actor = reader.GetInt16(1);
                    else
                        count_actor_2 = reader.GetInt16(1);

                }

                if (count_actor == 0)
                {
                    mess_err = " об истцах,";
                    not_inf = true;
                }
                else if (count_actor_2 == 0)
                {
                    mess_err = " об ответчиках,";
                    not_inf = true;
                }
            }
            else
            {
                mess_err = " об истцах и ответчиках,";
                not_inf = true;
            }
            reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_TMP where id_doc='{0}' and izmena=0 and iteration=0", id));
            while (reader.Read())
            {
                if (reader.GetInt16(0) == 0)
                {
                    mess_err += " формулировках искового требования,";
                    not_inf = true;
                }
            }
            
            if (not_inf)
            {
                mess_err = mess_err.Remove(mess_err.Length - 1);
                MessageBox.Show("Заполните информацию" + mess_err);
            }
            else
            {

                Microsoft.Office.Interop.Word.Selection wrdSelection;
                Microsoft.Office.Interop.Word.MailMerge wrdMailMerge;
                Microsoft.Office.Interop.Word.MailMergeFields wrdMergeFields;
                Microsoft.Office.Interop.Word.Table wrdTable;
                wrdApp = new Microsoft.Office.Interop.Word.Application();


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
                wrdApp.Visible = true;
                wrdSelection.ParagraphFormat.SpaceAfter = 0;

                string procuror = null;
                bool curt = true;
                reader = connection.ReadData(string.Format("Select NAME,NUMBER,DATE,LOCATE,NAME_COURT,CONTENT_COURT,SECRETARY,PROSECUTOR,PUBLIC_MEETING from document Where id='{0}'", id));
                while (reader.Read())
                {


                    // Microsoft.Office.Interop.Word.Application app = new Microsoft.Office.Interop.Word.Application();
                    //app.Visible = true;
                    //app.Documents.Add();

                    StrToAdd = "Дело № " + reader.GetString(1) + "\r\n";
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

                    if (!reader.IsDBNull(2) && reader.GetString(2).Length > 0)
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
                        wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphJustify;
                        wrdSelection.Font.Bold = 0;
                        wrdSelection.Font.Size = 14;
                        wrdSelection.Font.Name = "Times New Roman";
                        wrdSelection.ParagraphFormat.LineSpacing = 15;
                        // wrdSelection.ParagraphFormat.LineSpacingRule = 15;
                        wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                        wrdSelection.ParagraphFormat.FirstLineIndent = 30;
                        wrdSelection.TypeText(StrToAdd);
                    }

                    if (!reader.IsDBNull(4) && reader.GetString(4).Length > 0)
                    {
                        StrToAdd = reader.GetString(4) + " в составе:" + "\r\n";
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
                        StrToAdd += "при секретаре " + reader.GetString(6) + "\r\n";
                    }

                    if (!reader.IsDBNull(7) && reader.GetString(7).Length > 0)
                    {
                        procuror += "с участием прокурора " + reader.GetString(7) + "\r\n";
                    }
                    if (!reader.IsDBNull(8))
                        curt = Boolean.Parse(reader.GetString(8));
                    wrdSelection.TypeText(StrToAdd);
                }

                //записываем истцов или истца
                int count_actor_ist = 0;
                count_actor = 0;
                reader = connection.ReadData(string.Format("Select count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=1", id));
                while (reader.Read())
                {
                    count_actor_ist = reader.GetInt16(0);
                }
                if (count_actor_ist == 1)
                {
                    StrToAdd = "с участием истца";
                }
                else
                {
                    StrToAdd = "с участием истцов";
                }
                reader = connection.ReadData(string.Format("Select id,NAME_ACTOR_RP,ACTOR_DOC from ACTORS Where id_doc='{0}' and PLAINTIFF=1", id));

                string pred_istec = null;
                string doc_istec = null;
                string str_istec = null;
                foreach (DbDataRecord record in reader)
                {
                    // pred_istec = null;
                    StrToAdd += " " + record["NAME_ACTOR_RP"].ToString() + ",";
                    if (record["ACTOR_DOC"].ToString().Length > 1)
                    {
                        doc_istec = "В отсутствие истца " + ShortName(record["NAME_ACTOR_RP"].ToString()) + ", просившего рассмотреть дело в его отсутствие:";
                        string[] doc_str = record["ACTOR_DOC"].ToString().Split('~');
                        if (doc_str[0].Length > 0)
                        {
                            doc_istec += " заявление от " + doc_str[0] + ";";
                        }
                        if (doc_str[1].Length > 0)
                        {
                            doc_istec += " извещенный надлежайшим образом: документ, подтверждающий извещение: " + doc_str[1] + ";";
                        }
                        doc_istec += "\n";
                    }
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
                        pred_istec += "представителя истца " + record["NAME_ACTOR_RP"].ToString() + ":\r\n";
                    }
                    else
                    {
                        pred_istec += "представителей истца " + record["NAME_ACTOR_RP"].ToString() + ":\r\n";
                    }
                    reader_actor = connection.ReadData(string.Format("Select NAME_AGENT_RP,AGENT_DOC from AGENT_PLAINTIFF Where ID_ACTOR='{0}'", record["ID"].ToString()));

                    foreach (DbDataRecord record_actor in reader_actor)
                    {
                        pred_istec += record_actor["NAME_AGENT_RP"].ToString();
                        if (record_actor["AGENT_DOC"].ToString().Length > 8)
                        {
                            pred_istec += ", действующего на основании: ";
                            string[] doc_pred_string = record_actor["AGENT_DOC"].ToString().Split('~');
                            if (doc_pred_string[0].Length > 0 && doc_pred_string[1].Length > 0)
                            {
                                pred_istec += " доверенности от " + doc_pred_string[0].Remove(doc_pred_string[0].IndexOf(' ')) + " со сроком действия до " + doc_pred_string[1].Remove(doc_pred_string[1].IndexOf(' ')) + ";";
                            }
                            if (doc_pred_string[2].Length > 0)
                            {
                                pred_istec += " ордера адвоката от " + doc_pred_string[2].Remove(doc_pred_string[2].IndexOf(' ')) + ";";
                            }
                            if (doc_pred_string[3].Length > 0 && doc_pred_string[4].Length > 0)
                            {
                                pred_istec += " " + doc_pred_string[3] + ", удостоверяющего служебное положение их представителей от " + doc_pred_string[4] + ";";
                            }
                            if (doc_pred_string[5].Length > 0 && doc_pred_string[6].Length > 0)
                            {
                                pred_istec += " " + doc_pred_string[5] + ", удостоверяющего статус и полномочия законного представителя в силу ст. " + doc_pred_string[6] + ";";
                            }
                            pred_istec += "\n";
                        }
                    }


                }
                str_istec = StrToAdd.Remove(StrToAdd.Length - 1);
                StrToAdd = StrToAdd.Remove(StrToAdd.Length - 1) + ", \n";
                wrdSelection.TypeText(StrToAdd);
                wrdSelection.TypeText(doc_istec);
                wrdSelection.TypeText(pred_istec);
                //    while (reader.Read())
                //{

                //}
                //записываем ответчиков
                int count_actor_otv = 0;
                count_actor = 0;
                reader = connection.ReadData(string.Format("Select count(*) from ACTORS Where id_doc='{0}' and PLAINTIFF=2", id));
                while (reader.Read())
                {
                    count_actor_otv = reader.GetInt16(0);
                }

                if (count_actor_otv == 0)
                {
                    mess_err += " об ответчиках,";
                }
                else if (count_actor_otv == 1)
                {
                    StrToAdd = "с участием ответчика";
                }
                else
                {
                    StrToAdd = "с участием ответчиков";
                }
                reader = connection.ReadData(string.Format("Select id,NAME_ACTOR_RP,ACTOR_DOC from ACTORS Where id_doc='{0}' and PLAINTIFF=2", id));

                pred_istec = null;
                doc_istec = null;

                foreach (DbDataRecord record in reader)
                {
                    // pred_istec = null;
                    StrToAdd += " " + record["NAME_ACTOR_RP"].ToString() + ",";
                    if (record["ACTOR_DOC"].ToString().Length > 1)
                    {
                        doc_istec = "В отсутствие ответчика " + ShortName(record["NAME_ACTOR_RP"].ToString()) + ", просившего рассмотреть дело в его отсутствие:";
                        string[] doc_str = record["ACTOR_DOC"].ToString().Split('~');
                        if (doc_str[0].Length > 0)
                        {
                            doc_istec += " заявление от " + doc_str[0] + ";";
                        }
                        if (doc_str[1].Length > 0)
                        {
                            doc_istec += " извещенный надлежайшим образом: документ, подтверждающий извещение: " + doc_str[1] + ";";
                        }
                        doc_istec += "\n";
                    }
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
                        pred_istec += "представителя ответчика " + record["NAME_ACTOR_RP"].ToString() + ":\r\n";
                    }
                    else
                    {
                        pred_istec += "представителей ответчика " + record["NAME_ACTOR_RP"].ToString() + ":\r\n";
                    }
                    reader_actor = connection.ReadData(string.Format("Select NAME_AGENT_RP,AGENT_DOC from AGENT_PLAINTIFF Where ID_ACTOR='{0}'", record["ID"].ToString()));

                    foreach (DbDataRecord record_actor in reader_actor)
                    {
                        pred_istec += record_actor["NAME_AGENT_RP"].ToString();
                        if (record_actor["AGENT_DOC"].ToString().Length > 8)
                        {
                            pred_istec += ", действующего на основании: ";
                            string[] doc_pred_string = record_actor["AGENT_DOC"].ToString().Split('~');
                            if (doc_pred_string[0].Length > 0 && doc_pred_string[1].Length > 0)
                            {
                                pred_istec += " доверенности от " + doc_pred_string[0].Remove(doc_pred_string[0].IndexOf(' ')) + " со сроком действия до " + doc_pred_string[1].Remove(doc_pred_string[1].IndexOf(' ')) + ";";
                            }
                            if (doc_pred_string[2].Length > 0)
                            {
                                pred_istec += " ордера адвоката от " + doc_pred_string[2].Remove(doc_pred_string[2].IndexOf(' ')) + ";";
                            }
                            if (doc_pred_string[3].Length > 0 && doc_pred_string[4].Length > 0)
                            {
                                pred_istec += " " + doc_pred_string[3] + ", удостоверяющего служебное положение их представителей от " + doc_pred_string[4] + ";";
                            }
                            if (doc_pred_string[5].Length > 0 && doc_pred_string[6].Length > 0)
                            {
                                pred_istec += " " + doc_pred_string[5] + ", удостоверяющего статус и полномочия законного представителя в силу ст. " + doc_pred_string[6] + ";";
                            }
                            pred_istec += "\n";
                        }
                    }


                }
                StrToAdd = StrToAdd.Remove(StrToAdd.Length - 1) + ", \n";
                wrdSelection.TypeText(StrToAdd);
                wrdSelection.TypeText(doc_istec);
                wrdSelection.TypeText(pred_istec);
                wrdSelection.TypeText(procuror);
                if (str_istec.Contains(','))
                    if (curt)
                        StrToAdd = "рассмотрев в открытом судебном заседании гражданское дело по иску " + str_istec + ", которые просили";
                    else
                        StrToAdd = "рассмотрев в закрытом судебном заседании гражданское дело по иску " + str_istec + ", которые просили";
                else
                    if (curt)
                    StrToAdd = "рассмотрев в открытом судебном заседании гражданское дело по иску " + str_istec + ", который просил";
                else
                    StrToAdd = "рассмотрев в закрытом судебном заседании гражданское дело по иску " + str_istec + ", который просил";

                reader = connection.ReadData(string.Format("Select text from REQUIREMENTS_TMP where id_doc='{0}' and izmena=0 and iteration=0", id));
                while (reader.Read())
                {
                    StrToAdd += " " + reader.GetString(0) + ",";
                }
                StrToAdd = StrToAdd.Remove(StrToAdd.Length - 1) + ". \n";
                wrdSelection.TypeText(StrToAdd);
                reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_TMP where id_doc='{0}' and izmena=1", id));
                int izm = 0;
                while (reader.Read())
                {
                    izm = reader.GetInt16(0);
                }
                if (izm > 0)
                {
                    StrToAdd = "согласно уточненным исковым требованиям:";
                    reader = connection.ReadData(string.Format("select d.text from REQUIREMENTS_TMP d where d.id_doc='{0}' and (d.iteration = (select max(k.iteration) from REQUIREMENTS_TMP k where k.id_doc='{0}' and k.id_req=d.id_req and izmena=1))", id));
                    while (reader.Read())
                    {
                        StrToAdd += " " + reader.GetString(0) + ",";
                    }
                    StrToAdd = StrToAdd.Remove(StrToAdd.Length - 1) + ". \n";
                    wrdSelection.TypeText(StrToAdd);
                }

                // wrdApp.Selection.InsertNewPage();
                StrToAdd = "\nУ С Т А Н О В И Л:" + "\r\n";
                wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphCenter;
                wrdSelection.Font.Bold = 0;
                wrdSelection.Font.Size = 14;
                wrdSelection.Font.Name = "Times New Roman";
                wrdSelection.ParagraphFormat.LineSpacing = 11;
                wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                wrdSelection.TypeText(StrToAdd);
                wrdSelection.ParagraphFormat.LineSpacing = 15;
                wrdSelection.ParagraphFormat.LineUnitBefore = 0;
                wrdSelection.ParagraphFormat.FirstLineIndent = 30;
                wrdSelection.ParagraphFormat.Alignment = Microsoft.Office.Interop.Word.WdParagraphAlignment.wdAlignParagraphJustify;

                if (count_actor_ist==1)
                {
                    StrToAdd = "Истец обратился в суд с иском к ";
                }
                else
                {
                    StrToAdd = "Истецы обратились в суд с иском к ";
                }

                if (count_actor_otv == 1)
                {
                    StrToAdd += "ответчику ";
                }
                else
                {
                    StrToAdd += "ответчикам ";
                }
                reader = connection.ReadData(string.Format("Select count(*) from REQUIREMENTS_tmp Where id_doc='{0}' and iteration=0", id));
                int index = 0;
                while (reader.Read())
                    index = reader.GetInt16(0);
                if (index==1)
                {
                    StrToAdd += "с требованием ";
                }
                else
                {
                    StrToAdd += "с требованиями ";
                }

                //начальные исковые требования + нормы

                reader = connection.ReadData(string.Format("Select id,norma,text from REQUIREMENTS_tmp Where id_doc='{0}' and iteration=0 and izmena=0", id));
                int count_norm = 0;
                foreach (DbDataRecord record in reader)
                {
                    StrToAdd += record["text"].ToString();
                    if (Boolean.Parse(record["norma"].ToString()))
                    {
                        StrToAdd += ", ссылаясь на ";
                        reader_actor = connection.ReadData(string.Format("Select count(*) from NORMA where id_req='{0}'", record["id"].ToString()));
                        while (reader_actor.Read())
                            count_norm = reader_actor.GetInt16(0);
                        if (count_norm==1)
                        {
                            StrToAdd += "положение ";
                        }
                        else
                        {
                            StrToAdd += "положения ";
                        }
                        reader_actor = connection.ReadData(string.Format("Select text from NORMA where id_req='{0}'", record["id"].ToString()));
                        while (reader_actor.Read())
                            StrToAdd += UpgradeNorma(reader_actor.GetString(0)) +", ";
                        StrToAdd = StrToAdd.Remove(StrToAdd.Length - 1)+";";
                    }
                }
                StrToAdd += "\n";
                wrdSelection.TypeText(StrToAdd);

                //хдесь нужно вставлять изменные требования и основания




                // фактические обстоятельства
                string[] arr = null;
                StrToAdd = "";
                reader = connection.ReadData(string.Format("Select r.id,r.text from REQUIREMENTS_tmp r Where r.id_doc='{0}' and r.iteration=(Select max(q.iteration) from REQUIREMENTS_tmp q Where r.id_doc=q.id_doc and r.id_req=q.id_req and r.izmena=q.izmena)", id));
                foreach (DbDataRecord record in reader)
                {
                    StrToAdd += "Фактическими обстоятельствами в обоснование требования "+record["text"].ToString();
                    if (count_actor_ist==1)
                    {
                        StrToAdd += " истцом";
                    }
                    else
                        StrToAdd += " истцами";

                    //факты
                    int count_fact = 0;
                    reader_fact = connection.ReadData(string.Format("Select count(*) from fact_thing where id_req='{0}'",record["id"].ToString()));
                    while (reader_fact.Read())
                        count_fact = reader_fact.GetInt16(0);
                    if (count_fact==1)
                    {
                        StrToAdd += " указано следующее обстоятельство:\n";
                    }
                    else
                        StrToAdd += " указаны следующие обстоятельства:\n";

                    reader_fact = connection.ReadData(string.Format("Select text, choise, list_norm from fact_thing where id_req='{0}'", record["id"].ToString()));
                    foreach (DbDataRecord record_fact in reader_fact)
                    {
                        StrToAdd += record_fact["text"].ToString();
                        if (record_fact["choise"].ToString()=="1")
                        {
                            StrToAdd += " (";
                               arr = record_fact["list_norm"].ToString().Split(';');
                            for (int i=0;i<arr.Length;i++)
                            {
                                reader_fact_norm = connection.ReadData(string.Format("Select text from Norma where id_norma = '{0}'",arr[i]));
                                while (reader_fact_norm.Read())
                                    StrToAdd += UpgradeNorma(reader_fact_norm.GetString(0));
                            }
                            StrToAdd += ")\n";
                        }
                        else if (record_fact["choise"].ToString() == "2")
                        {
                            StrToAdd += " (по мнению истца имеет место пробел в праве)\n";
                        }
                        else
                        {
                            StrToAdd += " (без юридического обоснования)\n";
                        }
                    }
                     
                }
                wrdSelection.TypeText(StrToAdd);

                //признание иска
                StrToAdd = "";
                string StrToAdd2 = "\n";
                reader = connection.ReadData(string.Format("Select r.id,r.text,r.defendant_choise,r.prizn_isk from REQUIREMENTS_tmp r Where r.id_doc='{0}' and r.iteration=(Select max(q.iteration) from REQUIREMENTS_tmp q Where r.id_doc=q.id_doc and r.id_req=q.id_req and r.izmena=q.izmena)", id));
                while(reader.Read())
                {
                    StrToAdd += "Требование " + reader["text"].ToString() + " ответчик ";
                    if (reader.GetInt16(2) == 1)
                        StrToAdd += "признал. ";
                    else if (reader.GetInt16(2) == 2)
                        StrToAdd += "признал частично. ";
                    else 
                        StrToAdd += "не признал. ";

                    StrToAdd2 += "Суд считает, что признание требования " + reader["text"].ToString();
                    if (reader.GetInt16(2) == 1)
                        StrToAdd2 += " не противоречит закону и не нарушает права и законные интересы других лиц и принимается судом. ";
                    else if (reader.GetInt16(2) == 2)
                        StrToAdd2 += " противоречит закону. ";
                    else
                        StrToAdd2 += " нарушает права и законные интересы других лиц. ";
                }
                wrdSelection.TypeText(StrToAdd);
                wrdSelection.TypeText(StrToAdd2);
            }
            connection.Close();

            //if (mess_err.Length == 0)
            //    wrdApp.Visible = true;
            //else
            //{
            //    mess_err = mess_err.Remove(mess_err.Length - 1);
            //    MessageBox.Show("Заполните информацию " + mess_err);
            //    wrdDoc.Close();
            //}
            //connection.Close();
            //if ()
            //{
            //    wrdApp.Visible = true;
            //}
            //else
            //{

            //}
        }

        public string UpgradeNorma(string norma)
        {
            string[] str_arr = norma.Split('~');
            string str_norma = "";
            if (str_arr[0] != "")
                str_norma += "пункт " + str_arr[0] + " ";
            if (str_arr[1] != "")
                str_norma += "часть " + str_arr[1] + " ";
            if (str_arr[2] != "")
                str_norma += "статьи " + str_arr[2] + " ";
            if (str_arr[3] != "")
                str_norma += str_arr[3] + " ";
            if (str_arr[4] != "")
                str_norma += "издан " + str_arr[4] + " ";
            else if (str_arr[5] != "")
                str_norma += "издан " + str_arr[5] + " ";
            if (str_arr[7] != "")
                str_norma += "от " + str_arr[7] + " ";
            if (str_arr[8] != "")
                str_norma += "№ " + str_arr[8] + " ";
            if (str_arr[6] != "")
                str_norma += '"' + str_arr[6] + '"' + " ";
            if (str_arr[9] != "")
                str_norma += '(' + str_arr[9] + ')' + " ";
            str_norma = str_norma.Remove(str_norma.Length - 1);
            return str_norma;
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
