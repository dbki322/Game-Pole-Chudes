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
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();     
        }




        private void button1_Click(object sender, RoutedEventArgs e)//проверка пароля
        {
            if (passwordBox.Password=="123")
            { 
                MainWindow main = this.Owner as MainWindow;
                main.tabControl.Visibility = Visibility.Visible;
                main.tabControl.SelectedIndex = 1;
                this.Close();
            }
            else MessageBox.Show("Неверный пароль", "Ошибка");
        }




        private void button_Click(object sender, RoutedEventArgs e) //кнопка назад
        {
            this.Close();
        }
    }
}
