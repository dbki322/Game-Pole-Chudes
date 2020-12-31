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


namespace Курсовой
{
    /// <summary>
    /// Логика взаимодействия для Window2.xaml
    /// </summary>

public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e) //кнопка назад
        {
            this.Close();
        }

        private void button2_Click(object sender, RoutedEventArgs e) //ввод слова целиком
        {
            MainWindow main = this.Owner as MainWindow;
            if (textBox1.Text.ToString().ToUpper() == main.guesWord.ToUpper())
            {
                main.score = main.score + main.guesWord.Length - main.simAnsw;
                main.tabControl.SelectedIndex = 4;
                main.labelWinScore.Content = main.score;
                this.Close();
            }
            else
            {
                main.tabControl.SelectedIndex = 3;
                this.Close();
            }
        }

        private void button1_Click(object sender, RoutedEventArgs e) //ввод буквы
        {
                MainWindow main = this.Owner as MainWindow;
                var answerChar = textBox.Text.ToCharArray();
                if (char.IsLetter(answerChar[0])) answerChar = textBox.Text.ToString().ToUpper().ToCharArray();
                for (int a = 0; a < main.enterCount; a++)
                    if (answerChar[0] == main.enterWord[a])
                    { 
                        MessageBox.Show("Ошибка.Вы уже вводили эту букву");
                        return;
                    }
                bool trueAnsw = false;
                for (int i = 0; i < main.guesWord.Length ; i++) //проверка слова на наличие буквы
                {
                    if (answerChar[0] == main.guesWord[i]) //если буква есть
                    {
                        var glabel= String.Format("GLabel{0}", i);
                        Label Cell = main.gridGame.Children.OfType<Label>().FirstOrDefault(label => label.Name.Equals(glabel));
                        Cell.Content = main.guesWord[i].ToString().ToUpper();
                        trueAnsw = true;
                       
                        main.image_angry.Opacity = 0;
                        main.image_fun.Opacity = 100;
                        main.image_stand.Opacity = 0;
                        main.image_love.Opacity = 100;
                        main.image_rain.Opacity = 0;
                        main.wordAnsw.Content = "Вы ввели правильную букву!!!";
                        main.score += 1;
                        main.labelScore.Content = main.score;
                        main.simAnsw += 1;
                        if (main.simAnsw == main.simCount) //если введены все буквы
                        {   
                            main.tabControl.SelectedIndex = 4;
                            main.labelWinScore.Content = main.score;                      
                        }
                    }
                }
                if (trueAnsw == false) //если буквы не было
                {
                    main.wordAnsw.Content = "Вы ввели неправильную букву!!!";
                    main.score -= 1;
                    main.labelScore.Content = main.score;
                    main.image_angry.Opacity = 100;
                    main.image_fun.Opacity = 0;
                    main.image_stand.Opacity = 0;
                    main.image_love.Opacity = 0;
                    main.image_rain.Opacity = 100;
                    if (main.score == 0) //если очки закончились
                    {
                        main.tabControl.SelectedIndex = 3;
                    }
                }
                main.enterWord.SetValue(answerChar[0], main.enterCount);
                main.enterCount += 1;
                main.label6.Content += answerChar[0].ToString();
                main.label6.Content += ", ";
                this.Close();
        }
    }
}
