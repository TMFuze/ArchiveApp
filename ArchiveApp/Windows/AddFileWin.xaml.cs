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
    /// Логика взаимодействия для AddFileWin.xaml
    /// </summary>
    public partial class AddFileWin : Window
    {

        public int? SelectedFolderId { get; private set; }


        public AddFileWin()
        {
            InitializeComponent();
            FoldersBx.SelectedValuePath = "Id";
            FoldersBx.DisplayMemberPath = "Name";
            FoldersBx.ItemsSource = DBCon.entObj.Folder.ToList();
        }

        private void AddFilBtn_Click(object sender, RoutedEventArgs e)
        {
            if (DBCon.entObj.ArchiveFile.Count(x => x.Name == Descriptions.Text) > 0)
            {
                MessageBox.Show("Такой файл уже есть!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            else
            {
                try
                {
                    if (FileName.Text == null | FileName.Text.Trim() == "" | Descriptions.Text == null | Descriptions.Text.Trim() == "")
                    {
                        MessageBox.Show("Заполните все строки!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    ArchiveFile recipeObj = new ArchiveFile()
                    {
                        IdFolder = Convert.ToInt32(FoldersBx.SelectedValue),
                        Name = FileName.Text,
                        Description = Descriptions.Text,
                    };
                    DBCon.entObj.ArchiveFile.Add(recipeObj);
                    DBCon.entObj.SaveChanges();
                    MessageBox.Show("Файл добавлен!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Установка выбранной папки для передачи в главную форму
                    SelectedFolderId = recipeObj.IdFolder;

                    FoldersBx.SelectedItem = null;
                    Descriptions.Text = "";
                    FileName.Text = "";
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Критический сбой работы приложения: " + ex.Message.ToString(), "Критический сбой работы приложения", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
    }
    }

