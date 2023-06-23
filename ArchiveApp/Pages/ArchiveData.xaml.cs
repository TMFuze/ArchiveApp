using ArchiveApp.AppFiles;
using ArchiveApp.DBFrm;
using ArchiveApp.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArchiveApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ArchiveData.xaml
    /// </summary>
    public partial class ArchiveData : Page
    {
        private List<ArchiveFile> allItems;     

        public ArchiveData()
        {
            InitializeComponent();


            FolderBox.SelectedValuePath = "Id";
            FolderBox.DisplayMemberPath = "Name";
            FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
            DGItems.ScrollIntoView(Name);

            allItems = DBCon.entObj.ArchiveFile.ToList();
        }

        

        private void FoldersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
                UpdateDataGrid();
            
        }

        private void DelFolBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = FolderBox.SelectedItem as Folder;
            if (selectedFolder != null)
            {
                var itemsForRemoving = DBCon.entObj.ArchiveFile.Where(x => x.IdFolder == selectedFolder.Id).ToList();
                var productsForRemoving = new List<Folder> { selectedFolder };

                try
                {
                    var result = MessageBox.Show("Вы уверены?",
                        "Уведомление",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        DBCon.entObj.ArchiveFile.RemoveRange(itemsForRemoving);
                        DBCon.entObj.Folder.RemoveRange(productsForRemoving);
                        DBCon.entObj.SaveChanges();
                        MessageBox.Show("Данные удалены.");

                        // Обновление данных в DGItems и FoldersBox
                        DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
                        FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
                    }
                    else
                    {
                        // Отмена удаления, обновление только DGItems
                        DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message.ToString());
                }
            }
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

        private void SearchBox_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                UpdateDataGrid();
            }
        }

        private void UpdateDataGrid()
        {
            var selectedFolder = FolderBox.SelectedItem as Folder;
            string searchText = SearchBox.Text.ToLower();

            // Получение коллекции всех элементов или элементов для выбранной папки
            var items = (selectedFolder != null) ? allItems.Where(x => x.IdFolder == selectedFolder.Id) : allItems;

            // Выполнение фильтрации поискового запроса
            var filteredItems = items.Where(item =>
                (item.Name != null && item.Name.ToLower().Contains(searchText)) ||
                (item.Description != null && item.Description.ToLower().Contains(searchText))
            );

            // Обновление отображения в DataGrid
            DGItems.ItemsSource = filteredItems;

            // Выбор первого элемента, если есть результаты
            if (filteredItems.Any())
            {
                DGItems.SelectedIndex = 0;
            }
        }
    }
}
