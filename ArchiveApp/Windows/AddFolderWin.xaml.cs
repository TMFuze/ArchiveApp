using ArchiveApp.AppFiles;
using ArchiveApp.DBFrm;
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

namespace ArchiveApp.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddFolderWin.xaml
    /// </summary>
    public partial class AddFolderWin : Window
    {
        public AddFolderWin()
        {
            InitializeComponent();
        }

        private void AddBtnFol_Click(object sender, RoutedEventArgs e)
        {
            if (DBCon.entObj.Folder.Count(x => x.Name == NameTxb.Text) > 0)
            {
                MessageBox.Show("Такой продукт уже есть!",
                "Уведомление",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
                return;
            }

            else
            {
                try
                {
                    if (NameTxb.Text == null | NameTxb.Text.Trim() == "")
                    {
                        MessageBox.Show("Заполните все строки!",
                   "Уведомление",
                   MessageBoxButton.OK,
                   MessageBoxImage.Warning);
                        return;
                    }
                    Folder productObj = new Folder()
                    {
                        Name = NameTxb.Text,


                    };
                    DBCon.entObj.Folder.Add(productObj);
                    DBCon.entObj.SaveChanges();
                    MessageBox.Show("Папка добавлена!",
                    "Уведомление",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                    this.Close();


                    NameTxb.Text = "";
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка работы приложения: " + ex.Message.ToString(),
                        "Критический сбой работы приложения",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
        }
    }
}
