using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class CategoryManagementWindow : Window
    {
        private readonly CategoryService _categoryService;
        private readonly AppOrderNilonContext _context;
        private List<Category> _allCategories;
        private Category? _selectedCategory;

        public CategoryManagementWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _categoryService = new CategoryService(_context);
            _allCategories = new List<Category>();
            LoadDataAsync();
        }

        private async void LoadDataAsync()
        {
            try
            {
                txtStatus.Text = "Đang tải dữ liệu...";
                _allCategories = await _categoryService.GetAllCategoriesAsync();
                dgCategories.ItemsSource = _allCategories;
                txtStatus.Text = $"Đã tải {_allCategories.Count} danh mục";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                txtStatus.Text = "Lỗi khi tải dữ liệu";
            }
        }

        private async void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                var searchTerm = txtSearch.Text.Trim();
                if (string.IsNullOrEmpty(searchTerm))
                {
                    dgCategories.ItemsSource = _allCategories;
                }
                else
                {
                    var filteredCategories = await _categoryService.SearchCategoriesAsync(searchTerm);
                    dgCategories.ItemsSource = filteredCategories;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DgCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedCategory = dgCategories.SelectedItem as Category;
            btnEdit.IsEnabled = _selectedCategory != null;
            btnDelete.IsEnabled = _selectedCategory != null;
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var categoryForm = new CategoryFormWindow();
            if (categoryForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần sửa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var categoryForm = new CategoryFormWindow(_selectedCategory);
            if (categoryForm.ShowDialog() == true)
            {
                LoadDataAsync();
            }
        }

        private async void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedCategory == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục cần xóa.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var result = MessageBox.Show(
                $"Bạn có chắc chắn muốn xóa danh mục '{_selectedCategory.CategoryName}'?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    txtStatus.Text = "Đang xóa...";
                    var success = await _categoryService.DeleteCategoryAsync(_selectedCategory.CategoryId);
                    
                    if (success)
                    {
                        MessageBox.Show("Xóa danh mục thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDataAsync();
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa danh mục.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        txtStatus.Text = "Lỗi khi xóa";
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xóa danh mục: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    txtStatus.Text = "Lỗi khi xóa";
                }
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
} 