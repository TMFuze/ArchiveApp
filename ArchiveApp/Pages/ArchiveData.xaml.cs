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

            // Настройка ComboBox
            FolderBox.SelectedValuePath = "Id";
            FolderBox.DisplayMemberPath = "Name";
            FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
            DGItems.ScrollIntoView(Name);

            allItems = DBCon.entObj.ArchiveFile.ToList();
        }

        /// <summary>
        /// Обработчик изменения выбранной папки в ComboBox.
        /// </summary>
        private void FoldersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGrid();
        }

        /// <summary>
        /// Обработчик клика кнопки "Удалить папку".
        /// </summary>
        private void DelFolBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedFolder = FolderBox.SelectedItem as Folder;
            if (selectedFolder != null)
            {
                var itemsForRemoving = DBCon.entObj.ArchiveFile.Where(x => x.IdFolder == selectedFolder.Id).ToList();
                var foldersForRemoving = new List<Folder> { selectedFolder };

                try
                {
                    var result = MessageBox.Show("Вы уверены?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // Удаление выбранных папок и связанных файлов из базы данных
                        DBCon.entObj.ArchiveFile.RemoveRange(itemsForRemoving);
                        DBCon.entObj.Folder.RemoveRange(foldersForRemoving);
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

        /// <summary>
        /// Обработчик клика кнопки "Добавить папку".
        /// </summary>
        private void AddFolBtn_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new AddFolderWin();
            newForm.Show();
        }

        /// <summary>
        /// Обработчик события изменения видимости страницы.
        /// </summary>
        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                // Обновление данных при возвращении на страницу
                DBCon.entObj.ChangeTracker.Entries().ToList().ForEach(x => x.Reload());
                DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
                FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
            }
        }

        /// <summary>
        /// Обработчик клика кнопки "Добавить файл".
        /// </summary>
        private void AddFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var newForm = new AddFileWin();
            newForm.Closed += (s, args) =>
            {
                // Обновление списка всех файлов
                allItems = DBCon.entObj.ArchiveFile.ToList();

                // Обновление сетки грида
                UpdateDataGrid();

                // Выбор добавленной папки
                var addedFolderId = newForm.SelectedFolderId;
                if (addedFolderId != null)
                {
                    // Выбор пустой папки
                    FolderBox.SelectedItem = null;

                    // Задержка для обновления сетки грида
                    Dispatcher.Invoke(() =>
                    {
                        // Выбор добавленной папки после обновления сетки грида
                        FolderBox.SelectedValue = addedFolderId;
                    });
                }
            };
            newForm.Show();
        }

        /// <summary>
        /// Обработчик клика кнопки "Удалить файл".
        /// </summary>
        private void DelFileBtn_Click(object sender, RoutedEventArgs e)
        {
            var filesForRemoving = DGItems.SelectedItems.Cast<ArchiveFile>().ToList();
            try
            {
                var result = MessageBox.Show("Вы уверены?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Удаление выбранных файлов из базы данных
                    DBCon.entObj.ArchiveFile.RemoveRange(filesForRemoving);
                    DBCon.entObj.SaveChanges();
                    MessageBox.Show("Данные удалены.");

                    // Получение выбранной папки перед обновлением данных
                    var selectedFolder = FolderBox.SelectedItem as Folder;

                    // Обновление значений в DataGrid
                    UpdateDataGrid();

                    // Повторный выбор папки, если она была выбрана
                    if (selectedFolder != null)
                    {
                        FolderBox.SelectedItem = selectedFolder;
                    }
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

        /// <summary>
        /// Обработчик события KeyUp для TextBox поиска.
        /// </summary>
        private void SearchBox_KeyUp_1(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                UpdateDataGrid();
            }
        }

        /// <summary>
        /// Обновление данных в DataGrid на основе выбранных параметров поиска.
        /// </summary>
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

        private void RefreshDGBtn_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем выбранную папку
            FolderBox.SelectedItem = null;

            // Обновляем данные в DGItems
            DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
        }
    }
}
