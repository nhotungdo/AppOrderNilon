using System;
using System.Threading.Tasks;
using System.Windows;
using AppOrderNilon.Models;
using AppOrderNilon.Services;

namespace AppOrderNilon.Views
{
    public partial class CategoryFormWindow : Window
    {
        private readonly CategoryService _categoryService;
        private readonly AppOrderNilonContext _context;
        private readonly Category? _category;
        private readonly bool _isEditMode;

        public CategoryFormWindow()
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _categoryService = new CategoryService(_context);
            _isEditMode = false;
            SetupForAdd();
        }

        public CategoryFormWindow(Category category)
        {
            InitializeComponent();
            _context = new AppOrderNilonContext();
            _categoryService = new CategoryService(_context);
            _category = category;
            _isEditMode = true;
            SetupForEdit();
        }

        private void SetupForAdd()
        {
            txtTitle.Text = "THÊM DANH MỤC MỚI";
            gridQuantity.Visibility = Visibility.Collapsed;
        }

        private void SetupForEdit()
        {
            txtTitle.Text = "SỬA DANH MỤC";
            txtCategoryName.Text = _category?.CategoryName ?? "";
            txtDescription.Text = _category?.Description ?? "";
            txtQuantity.Text = _category?.Quantity.ToString() ?? "0";
            gridQuantity.Visibility = Visibility.Visible;
        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtCategoryName.Text))
                {
                    MessageBox.Show("Vui lòng nhập tên danh mục.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                    txtCategoryName.Focus();
                    return;
                }

                if (_isEditMode && _category != null)
                {
                    // Update existing category
                    _category.CategoryName = txtCategoryName.Text.Trim();
                    _category.Description = txtDescription.Text.Trim();

                    var success = await _categoryService.UpdateCategoryAsync(_category);
                    if (success)
                    {
                        MessageBox.Show("Cập nhật danh mục thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể cập nhật danh mục.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    // Create new category
                    var newCategory = new Category
                    {
                        CategoryName = txtCategoryName.Text.Trim(),
                        Description = txtDescription.Text.Trim(),
                        Quantity = 0
                    };

                    var success = await _categoryService.CreateCategoryAsync(newCategory);
                    if (success)
                    {
                        MessageBox.Show("Thêm danh mục thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                        DialogResult = true;
                        Close();
                    }
                    else
                    {
                        MessageBox.Show("Không thể thêm danh mục.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            _context?.Dispose();
            base.OnClosed(e);
        }
    }
} 