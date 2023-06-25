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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ArchiveApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для UserArchiveData.xaml
    /// </summary>
    public partial class UserArchiveData : Page
    {
        private List<ArchiveFile> allItems;

        public UserArchiveData()
        {
            InitializeComponent();

            // Настройка ComboBox
            FolderBox.SelectedValuePath = "Id";
            FolderBox.DisplayMemberPath = "Name";
            FolderBox.ItemsSource = DBCon.entObj.Folder.ToList();
            DGItems.ScrollIntoView(Name);

            allItems = DBCon.entObj.ArchiveFile.ToList();
        }

        private void RefreshDGBtn_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем выбранную папку
            FolderBox.SelectedItem = null;

            // Обновляем данные в DGItems
            DGItems.ItemsSource = DBCon.entObj.ArchiveFile.ToList();
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

        private void FoldersBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateDataGrid();
        }

    }
}
