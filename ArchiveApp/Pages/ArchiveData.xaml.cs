using ArchiveApp.AppFiles;
using ArchiveApp.DBFrm;
using ArchiveApp.Windows;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArchiveApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ArchiveData.xaml
    /// </summary>
    public partial class ArchiveData : Page
    {
        public ArchiveData()
        {
            InitializeComponent();
            

            FolderBox.SelectedValuePath = "Id";
            FolderBox.DisplayMemberPath = "Name";
            FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
            DGItems.ScrollIntoView(Name);
        }

        private void FoldersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedFolder = FolderBox.SelectedItem as Folder;
            if (selectedFolder != null)
            {
                DGItems.ItemsSource = DBCon.entObj.ArchiveFile.Where(x => x.IdFolder == selectedFolder.Id).ToList();
            }
        }

        private void DelFolBtn_Click(object sender, RoutedEventArgs e)
        {
           // var itemsForRemoving = DBCon.entObj.ArchiveFile.Where(x => x.IdFolder == FoldersBox.SelectedIndex).ToList();
            //var productsForRemoving = FolderBox.Items.Cast<Folder>().ToList();
            
            //try
            //{

            //    var result = MessageBox.Show("Вы уверены?",
            //       "Уведомление",
            //       MessageBoxButton.YesNo,
            //       MessageBoxImage.Question);
            //    if (result == MessageBoxResult.Yes)
            //    {
            //        DBCon.entObj.Folder.RemoveRange(productsForRemoving);
            //        //DBCon.entObj.ArchiveFile.RemoveRange(itemsForRemoving);
            //        DBCon.entObj.SaveChanges();
            //        MessageBox.Show("Данные удалены.");
            //        DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
            //    }
            //    else
            //    {
            //        DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message.ToString());
            //}
        }

        private void AddFolBtn_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new AddFolderWin();
            newForm.Show();
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                DBCon.entObj.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
                FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
                
                

            }
        }

        private void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new AddFileWin();
            newForm.Show();
        }

        private void DelFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var productsForRemoving = DGItems.SelectedItems.Cast<ArchiveFile>().ToList();
            try
            {

                var result = MessageBox.Show("Вы уверены?",
                   "Уведомление",
                   MessageBoxButton.YesNo,
                   MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    DBCon.entObj.ArchiveFile.RemoveRange(productsForRemoving);
                    DBCon.entObj.SaveChanges();
                    MessageBox.Show("Данные удалены.");
                    DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
                }
                else
                {
                    DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
