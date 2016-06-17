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
    /// Логика взаимодействия для MatrixWindow.xaml
    /// </summary>
    public partial class MatrixWindow : Window
    {
        private MatrixWindow matrixForm;
        private XDocument doc;
        private string name;

        private string[] directions = {
                "Защита объектов ИС" ,
                "Защита процессов и программ",
                "Защита каналов связи",
                "ПЭМИН",
                "Управление системой защиты" };

        private string[] foundation = {
                "База",
                "Структура",
                "Меры",
                "Средства" };

        private string[] stage = {
                "Определение информации, подлежащей защите",
                "Выявление угроз и каналов утечки информации",
                "Проведение оценки уязвимости и рисков",
                "Определение требований к СЗИ",
                "Осуществление выбора средств защиты",
                "Внедрение и использвание выбранных мер и средств",
                "Контроль целостности и управление защитой" };

        public MatrixWindow(string filename)
        {
            InitializeComponent();
            matrixForm = this;

            doc = XDocument.Load("files\\" + filename + ".xml");
            name = filename;

            toFillForm();
        }

        /* заполнение формы */
        public void toFillForm()
        {
            // создаем шапку
            for (int i = 0; i < 5; i++)
            {
                int k = 4 * i + 2;
                createBorder(k, 4, 1, 1);

                TextBlock textBlock1 = new TextBlock();
                textBlock1.Text = directions[i];
                textBlock1.TextWrapping = TextWrapping.Wrap;
                textBlock1.VerticalAlignment = VerticalAlignment.Center;
                textBlock1.HorizontalAlignment = HorizontalAlignment.Center;

                mGrid.Children.Add(textBlock1);
                Grid.SetColumn(textBlock1, k);
                Grid.SetRow(textBlock1, 1);
                Grid.SetColumnSpan(textBlock1, 4);

                for (int j = 0; j < 4; j++)
                {
                    createBorder(j + k, 1, 2, 1);

                    Label newLabel1 = new Label();
                    newLabel1.Content = foundation[j];
                    newLabel1.RenderTransformOrigin = new Point(0.5, 0.5);
                    newLabel1.RenderTransform = new RotateTransform(-90);
                    newLabel1.Margin = new Thickness(-40);
                    newLabel1.Width = 80;
                    newLabel1.VerticalAlignment = VerticalAlignment.Center;
                    newLabel1.HorizontalAlignment = HorizontalAlignment.Center;

                    mGrid.Children.Add(newLabel1);
                    Grid.SetColumn(newLabel1, j + k);
                    Grid.SetRow(newLabel1, 2);
                }
            }

            // создаем строки
            for (int i = 0; i < 7; i++)
            {
                createBorder(1, 1, i + 3, 1);

                TextBlock textBlock2 = new TextBlock();
                textBlock2.Text = stage[i];
                textBlock2.TextWrapping = TextWrapping.Wrap;
                textBlock2.Padding = new Thickness(5, 0, 5, 0);
                textBlock2.VerticalAlignment = VerticalAlignment.Center;

                mGrid.Children.Add(textBlock2);
                Grid.SetColumn(textBlock2, 1);
                Grid.SetRow(textBlock2, i + 3);
            }

            // создаем пустую область
            createBorder(1, 1, 1, 2);

            // заполняем матрицу
            toFillMatrix();
        }

        /* создание рамки */
        public void createBorder(int col, int colspan, int row, int rowspan)
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
            border.BorderThickness = new Thickness(1, 1, 1, 1);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));

            mGrid.Children.Add(border);
            Grid.SetColumn(border, col);
            Grid.SetColumnSpan(border, colspan);
            Grid.SetRow(border, row);
            Grid.SetRowSpan(border, rowspan);
        }

        /* заполнение матрицы */
        public void toFillMatrix()
        {
            XElement root = doc.Root;

            int numFound = 0;
            foreach (XElement found in root.Elements().ToList())
            {
                int numDirect = 0;
                foreach (XElement direct in found.Elements().ToList())
                {
                    int numStage = 0;
                    foreach (XElement stage in direct.Elements().ToList())
                    {
                        if (stage.IsEmpty)
                        {
                            createButton(numStage, numFound + numDirect * 4, false);
                        }
                        else
                        {
                            createButton(numStage, numFound + numDirect * 4, true);
                        }

                        numStage++;
                    }
                    numDirect++;
                }
                numFound++;
            }
        }

        /* создание кнопки */
        public void createButton(int row, int col, bool isExists)
        {
            row += 3;
            col += 2;

            string text = null;
            SolidColorBrush bg = null;

            if (isExists) { bg = new SolidColorBrush(Color.FromRgb(200, 255, 200)); text = "+"; }
            else { bg = new SolidColorBrush(Color.FromRgb(255, 200, 200)); text = "-"; }

            Button btn = new Button();
            btn.Content = text;
            btn.FontSize = 28;
            btn.Background = bg;
            btn.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            btn.Click += new RoutedEventHandler(Button_Click_Table);

            mGrid.Children.Add(btn);
            Grid.SetColumn(btn, col);
            Grid.SetRow(btn, row);
        }

        /* клик по кнопке */
        private void Button_Click_Table(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            int row = (int)btn.GetValue(Grid.RowProperty) - 3;
            int col = (int)btn.GetValue(Grid.ColumnProperty) - 2;

            EditWindow eForm = new EditWindow(name);
            eForm.Owner = this;
            eForm.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            eForm.ShowDialog();
        }
    }
}
