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
using System.Xml.Linq;

namespace SZI
{
    /// <summary>
    /// Логика взаимодействия для EditWindow.xaml
    /// </summary>
    public partial class EditWindow : Window
    {
        private int i = 0;
        private string name, grandpa, parent, item;
        private XDocument doc;
        private XElement Stage;

        private string[] foundation = {
                "База",
                "Структура",
                "Меры",
                "Средства" };

        private string[] direction = {
                "Защита объектов ИС" ,
                "Защита процессов и программ",
                "Защита каналов связи",
                "ПЭМИН",
                "Управление системой защиты" };

        private string[] stage = {
                "Определение информации, подлежащей защите",
                "Выявление угроз и каналов утечки информации",
                "Проведение оценки уязвимости и рисков",
                "Определение требований к СЗИ",
                "Осуществление выбора средств защиты",
                "Внедрение и использвание выбранных мер и средств",
                "Контроль целостности и управление защитой" };

        public EditWindow(string filename)
        {
            InitializeComponent();

            addImg.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\add.png", UriKind.Absolute));

            doc = XDocument.Load("files\\" + filename + ".xml");
            name = filename;
            toFillTreeView();
            toFillTextBoxes();
        }

        /* заполнение дерева */
        private void toFillTreeView()
        {
            var tree = Tree;

            for (int i = 0; i < 4; i++)
            {
                TreeViewItem item1 = new TreeViewItem();
                item1.Header = foundation[i];

                for (int j = 0; j < 5; j++)
                {
                    TreeViewItem item2 = new TreeViewItem();
                    item2.Header = direction[j];

                    for (int k = 0; k < 7; k++)
                    {
                        TreeViewItem item3 = new TreeViewItem();
                        item3.Header = stage[k];
                        item3.MouseDoubleClick += treeItem_Selected;
                        item2.Items.Add(item3);
                    }

                    item1.Items.Add(item2);
                }

                tree.Items.Add(item1);
            }
        }

        /* заполнение текстбоксов нужной информацией */
        public void toFillTextBoxes()
        {
            // удаление старых текстбоксов
            var grids = DataPanel.Children;

            while (grids.Count > 0)
            {
                var grid = grids[0];
                grids.Remove(grid);
            }

            // выделение нужного узла в дереве
            Title = "";

            foreach (TreeViewItem item1 in Tree.Items)
                if ((string)item1.Header == grandpa)
                {
                    item1.IsExpanded = true;
                    Title += (string)item1.Header + " -> ";
                    foreach (TreeViewItem item2 in item1.Items)
                        if ((string)item2.Header == parent)
                        {
                            item2.IsExpanded = true;
                            Title += (string)item2.Header + " -> ";
                            foreach (TreeViewItem item3 in item2.Items)
                                if ((string)item3.Header == item)
                                {
                                    item3.IsSelected = true;
                                    Title += (string)item3.Header;
                                }
                        }
                        else { item2.IsExpanded = false; }
                }
                else { item1.IsExpanded = false; }

            XElement root = doc.Root;

            // чтение имеющихся в файле данных и подстановка их в текстбоксы 
            foreach (XElement found in root.Elements().ToList())
                if (found.Attribute("Name").Value == grandpa)
                {
                    foreach (XElement direct in found.Elements().ToList())
                        if (direct.Attribute("Name").Value == parent)
                        {
                            foreach (XElement stage in direct.Elements().ToList())
                                if (stage.Attribute("Name").Value == item)
                                {
                                    foreach (XElement point in stage.Elements().ToList())
                                        AddTextBox(point.Value);
                                    Stage = stage;
                                }
                        }
                }
        }

        /* клик по кнопке "Добавить текстбокс" */
        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            AddTextBox(null);
        }

        /* добавление текстбокса на форму */
        private void AddTextBox(string text)
        {
            i++;

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(30) });
            grid.Margin = new Thickness(0, 0, 0, 10);

            Label label = new Label() { Content = "•", FontSize = 20, Margin = new Thickness(0, -10, 0, 0) };

            TextBox textbox = new TextBox();
            textbox.Padding = new Thickness(5, 2, 5, 2);
            textbox.TextWrapping = TextWrapping.Wrap;
            textbox.FontSize = 14;
            textbox.AcceptsReturn = true;
            textbox.Text = text;

            Image img = new Image();
            img.Source = new BitmapImage(new Uri(AppDomain.CurrentDomain.BaseDirectory + "\\images\\delete.png", UriKind.Absolute));
            img.Width = 20;
            img.Height = 20;

            Button btn = new Button();
            btn.Background = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            btn.Content = img;
            btn.Click += new RoutedEventHandler(deleteTextBox);

            grid.Children.Add(label);
            Grid.SetColumn(label, 0);
            grid.Children.Add(textbox);
            Grid.SetColumn(textbox, 1);
            grid.Children.Add(btn);
            Grid.SetColumn(btn, 2);

            DataPanel.Children.Add(grid);
        }

        /* удаление текстбокса с формы */
        private void deleteTextBox(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Grid grid = (Grid)btn.Parent;

            var grids = DataPanel.Children;
            grids.Remove(grid);
        }

        /* жмак мыши по листу дерева */
        private void treeItem_Selected(object sender, RoutedEventArgs e)
        {
            var item = sender as TreeViewItem;
            this.item = item.Header.ToString();
            var parent = item.Parent as TreeViewItem;
            this.parent = parent.Header.ToString();
            var grandpa = parent.Parent as TreeViewItem;
            this.grandpa = grandpa.Header.ToString();
            toFillTextBoxes();
        }

        /* клик по кнопке "Сохранить" */
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Message()) Save();
        }


        /* вывод вопроса о сохранении полей */
        private bool Message()
        {
            MessageBoxResult messageBoxResult = MessageBox.Show("Вы хотите сохранить изменения?", "Сохранение изменений", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) { return true; }
            else { return false; }
        }


        /* сохранение данных */
        private void Save()
        {
            bool isFull = false;

            // удаляем старые поля
            foreach (XElement point in Stage.Elements().ToList())
                point.Remove();

            // добавляем новые поля
            foreach (Grid grid in DataPanel.Children)
            {
                var textbox = grid.Children[1] as TextBox;
                if (textbox.Text != "")
                {
                    XElement point = new XElement("Point", textbox.Text);
                    Stage.Add(point);
                    isFull = true;
                }
            }

            // сохраняем файл
            doc.Save("files\\" + name + ".xml");

            // обновляем matrixForm

            updateMForm(isFull);
        }

        private void ExitMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /* обновление кнопки */
        private void updateMForm(bool isFull)
        {
            int numFound = Array.IndexOf(foundation, grandpa);
            int numDirect = Array.IndexOf(direction, parent);
            int numStage = Array.IndexOf(stage, item);

            int row = numStage + 3;
            int col = numDirect * 4 + numFound + 2;

            string text = null;
            SolidColorBrush bg = null;

            if (isFull) { bg = new SolidColorBrush(Color.FromRgb(200, 255, 200)); text = "+"; }
            else { bg = new SolidColorBrush(Color.FromRgb(255, 200, 200)); text = "-"; }

            
        }

        /* О программе (меню) */
        private void AboutMenuBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Программа предназначена для создания систем защиты информации\nСоздатели: Гилязева Софья, Качулин Сергей, Колосова Юлия\nПГНИУ, 2015", "О программе");
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            bool q = true;
            int numFound = Array.IndexOf(foundation, grandpa);
            int numDirect = Array.IndexOf(direction, parent);
            int numStage = Array.IndexOf(stage, item) + 1;

            if (numStage > 6)
            {
                numStage = 0;
                numDirect++;

                if (numDirect > 4)
                {
                    numDirect = 0;
                    numFound++;

                    if (numFound > 3)
                    {
                        numFound = 0;
                        q = false;
                    }
                }
            }

            item = stage[numStage];
            parent = direction[numDirect];
            grandpa = foundation[numFound];

            if (q)
            {
                toFillTextBoxes();
            }
            else
            {
                Close();
            }
        }
    }
}