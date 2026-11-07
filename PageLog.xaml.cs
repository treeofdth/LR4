using demo.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace demo.Views
{
    /// <summary>
    /// Логика взаимодействия для PageLog.xaml
    /// </summary>
    public partial class PageLog : Page
    {
        public PageLog()
        {
            InitializeComponent();

            var list = DemoSingleton.GetDbContext().Users.ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            User newUser = new User
            {
                Fio = tbName.Text.Trim()
            };

            try
            {
                using (DemoContext db = new DemoContext())
                {
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                MessageBox.Show("Record's added");
            }
            catch (Exception ex)
            {

            }
        }
    }
}