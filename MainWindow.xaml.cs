using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Курсовой
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>


    public partial class MainWindow : Window
    {
        private string dbFileName;
        private string Query;
        DataTable dTable = new DataTable();
        private SQLiteDataAdapter adapter;
        SQLiteCommandBuilder cmdBuilder;
        public string guesWord;
        Grid grid = new Grid();
        public int simCount;
        public int simAnsw;
        public Label[] labelArray;
        public char[] enterWord=new char[100];
        public int enterCount;
        public int score;

        public MainWindow()
        {
            InitializeComponent();
            tabControl.Visibility = Visibility.Collapsed;
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            dbFileName = "pole.sqlite";
            if (!File.Exists(dbFileName)) // Если Бд не существует создание бд и формирование таблицы из запроса
            {
                SQLiteConnection.CreateFile(dbFileName); 
                SQLiteConnection DbConnection = new SQLiteConnection("Data Source=" + dbFileName + "; Version=3;"); 
                string commandText = "CREATE TABLE Words (id INTEGER PRIMARY KEY AUTOINCREMENT,word VARCHAR(15), question VARCHAR(50),сategory VARCHAR(50));";
                SQLiteCommand Command = new SQLiteCommand(commandText, DbConnection);
                DbConnection.Open(); 
                Command.ExecuteNonQuery();
                Query = "SELECT id AS 'ID', word AS 'Слово', question AS 'Вопрос',сategory AS 'Категория' FROM Words";
                adapter = new SQLiteDataAdapter(Query, DbConnection);             
                dataGrid.Items.Clear();          
                DataTable dTable = new DataTable("Words");
                dataGrid.ItemsSource = dTable.DefaultView;
                cmdBuilder = new SQLiteCommandBuilder(adapter);
                adapter.Fill(dTable);
            }
            else //если бд существует подключится к ней и сформировать таблицу из запроса
            {
                SQLiteConnection DbConnection = new SQLiteConnection("Data Source=" + dbFileName + "; Version=3;");
                DbConnection.Open();
                Query = "SELECT id AS 'ID', word AS 'Слово', question AS 'Вопрос',сategory AS 'Категория' FROM Words";
                adapter = new SQLiteDataAdapter(Query, DbConnection);
                dataGrid.Items.Clear();
                DataTable dTable = new DataTable("Words");
                dataGrid.ItemsSource = dTable.DefaultView;
                cmdBuilder = new SQLiteCommandBuilder(adapter);
                adapter.Fill(dTable);
            }
        }




        private void button_Click(object sender, RoutedEventArgs e) // открытие страницы выбора категории для игрока
        {
            SQLiteConnection con = new SQLiteConnection("Data Source=" + dbFileName + "; Version=3;");
            SQLiteCommand sqlcmd = new SQLiteCommand();
            SQLiteDataAdapter sqladp = new SQLiteDataAdapter();
            DataSet ds = new DataSet();
            sqlcmd.Connection = con;
            sqlcmd.CommandType = CommandType.Text;

            sqlcmd.CommandText = "SELECT DISTINCT сategory FROM Words";  //Создание выборки всех неповторяющихся значений категорий
            sqladp.SelectCommand = sqlcmd;
            sqladp.Fill(ds, "defaultTable");
            comboBox.DataContext = ds.Tables["defaultTable"].DefaultView;
            comboBox.ItemsSource = ds.Tables["defaultTable"].DefaultView;
            comboBox.DisplayMemberPath = ds.Tables["defaultTable"].Columns[0].ToString();
            con.Close();
            tabControl.Visibility = Visibility.Visible;
            tabControl.SelectedIndex = 0;
        }




        private void button1_Click(object sender, RoutedEventArgs e) //открытие формы для логина в режим администратора
        {
            Window1 logForm = new Window1();
            logForm.Owner = this;
            logForm.ShowDialog();
        }




        private void button2_Click(object sender, RoutedEventArgs e) //обновить таблицу
        {
            adapter.Update((dataGrid.ItemsSource as DataView).Table);
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            dTable.Clear();
            dataGrid.ItemsSource = dTable.DefaultView;
            adapter.Fill(dTable);
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            dataGrid.ItemsSource = dTable.DefaultView;

        }




        private void button3_Click(object sender, RoutedEventArgs e) //отменить не применённые изменения в таблице
        {
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            dTable.Clear();
            dataGrid.ItemsSource = dTable.DefaultView;
            adapter.Fill(dTable);
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            dataGrid.ItemsSource = dTable.DefaultView;
        }




        private void button4_Click(object sender, RoutedEventArgs e) //кнопка назад
        {
            tabControl.Visibility = Visibility.Hidden;
        }




        private void dataGrid_AutoGeneratedColumns(object sender, EventArgs e) //задание размеров для таблицы администратора
        {
            if (dataGrid.Columns.Count > 2)
            {
                dataGrid.Columns[0].MaxWidth = 0;
                dataGrid.Columns[1].MinWidth = 195;
                dataGrid.Columns[2].MinWidth = 580;
                dataGrid.Columns[3].MinWidth = 200;
                ((DataGridTextColumn)dataGrid.Columns[1]).EditingElementStyle = (Style)this.FindResource("sty_txtDesc15");
                ((DataGridTextColumn)dataGrid.Columns[2]).EditingElementStyle = (Style)this.FindResource("sty_txtDesc50");
                ((DataGridTextColumn)dataGrid.Columns[3]).EditingElementStyle = (Style)this.FindResource("sty_txtDesc50");
            }

        }




        private void button5_Click(object sender, RoutedEventArgs e) //кнопка удалить
        {
            SQLiteConnection DbConnection = new SQLiteConnection("Data Source=" + dbFileName + "; Version=3;");
            DbConnection.Open();
            int selectedIndex = dataGrid.SelectedIndex;
            dataGrid.ItemsSource = dTable.DefaultView;
            adapter.Fill(dTable);

                if (selectedIndex !=null)
                {
                    
                    var row = dTable.Rows[selectedIndex];
                    row.Delete();

                    adapter.Update(dTable);


                }
              
            dataGrid.ItemsSource = null;
            dataGrid.Items.Clear();
            dTable.Clear();
            dataGrid.ItemsSource = dTable.DefaultView;
            adapter.Fill(dTable);

        }

        private void button6_Click(object sender, RoutedEventArgs e) //переход к игре
        {
            if(comboBox.SelectedIndex!=-1) //если пользователь выбрал категорию
            {
                String category = comboBox.Text;
                SQLiteConnection con2 = new SQLiteConnection("Data Source=" + dbFileName + "; Version=3;");
                SQLiteCommand sqlcmd2 = new SQLiteCommand();
                SQLiteDataAdapter sqladp2 = new SQLiteDataAdapter();
                DataSet ds2 = new DataSet();
                sqlcmd2.Connection = con2;
                sqlcmd2.CommandType = CommandType.Text;
                sqlcmd2.CommandText = "SELECT id,word,question FROM Words WHERE сategory ='" + category + "'";
                sqladp2.SelectCommand = sqlcmd2;
                sqladp2.Fill(ds2, "defaultTable");

                wordAnsw.Content = "";
                tabControl.SelectedIndex = 2;
                int count_words = ds2.Tables["defaultTable"].Rows.Count;
                Random rnd = new Random();
                int randomValue = rnd.Next(0, count_words);
                guesWord = ds2.Tables["defaultTable"].Rows[randomValue][1].ToString().ToUpper();
                simCount = 0;
                simAnsw = 0;
                foreach (var el in guesWord) if ((char.IsDigit(el)) || (char.IsLetter(el))) simCount++;
                
                gridGame.Children.Clear();
                Label[] labelArray = new Label[simCount];
                for (int i = 0; i < labelArray.Length; i++) //Создание массива вопросительных знаков под выбранное слово
                {
                    labelArray[i] = new Label();
                    labelArray[i].Content = "?";
                    labelArray[i].Name = String.Format("GLabel{0}", i);
                    labelArray[i].HorizontalContentAlignment = HorizontalAlignment.Center;
                    labelArray[i].Margin = new Thickness(1, 20, 11, 22);
                    labelArray[i].VerticalAlignment = VerticalAlignment.Top;
                    labelArray[i].Style = (Style)labelArray[i].FindResource("wordLabel");
                    gridGame.Children.Add(labelArray[i]);
                    Grid.SetColumn(labelArray[i], 15 / simCount - 1 + i);
                    Grid.SetRow(labelArray[i], 0);
                }
                Array.Clear(enterWord, 0, 100);
                enterCount = 0;
                label6.Content = "";
                score = 6;
                labelScore.Content = score;
                image_angry.Opacity = 0;
                image_fun.Opacity = 0;
                image_stand.Opacity = 100;
                image_love.Opacity = 0;
                image_rain.Opacity = 0;
                textBlock.Text = ds2.Tables["defaultTable"].Rows[randomValue][2].ToString();
            }   
        }




        private void button7_Click(object sender, RoutedEventArgs e) //Кнопка назад
        {
            tabControl.SelectedIndex = 0;
        }




        private void button9_Click(object sender, RoutedEventArgs e) //открытие формы для введения буквы
        {
            Window2 wordForm = new Window2();
            wordForm.Owner = this;
            wordForm.tabControl.SelectedIndex = 0;
            wordForm.ShowDialog();
            
        }




        private void button9_Copy_Click(object sender, RoutedEventArgs e) //открытие формы для введения слова целиком
        {
            Window2 wordForm = new Window2();
            wordForm.Owner = this;
            wordForm.tabControl.SelectedIndex = 1;
            wordForm.ShowDialog();
        }




        private void button10_Click(object sender, RoutedEventArgs e) //кнопка заново
        {
            tabControl.SelectedIndex = 0;
        }

        private void button8_Click(object sender, RoutedEventArgs e)
        {
            Storyboard storyboard_name = (Storyboard)(FindResource("weel"));
            storyboard_name.Stop(); 
            storyboard_name.Begin();
            
        }
    }
}