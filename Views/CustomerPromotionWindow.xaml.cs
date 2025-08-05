using System;
using System.Collections.Generic;
using System.Windows;
using AppOrderNilon.Models;

namespace AppOrderNilon.Views
{
    public partial class CustomerPromotionWindow : Window
    {
        private Customer currentCustomer;
        private List<CustomerPromotion> promotions;

        public CustomerPromotionWindow(Customer customer)
        {
            InitializeComponent();
            currentCustomer = customer;
            LoadData();
            InitializeControls();
        }

        private void LoadData()
        {
            LoadPromotions();
        }

        private void LoadPromotions()
        {
            promotions = new List<CustomerPromotion>
            {
                new CustomerPromotion 
                { 
                    PromotionId = 1, 
                    Title = "Giảm 10% cho đơn hàng đầu tiên", 
                    Description = "Áp dụng cho tất cả sản phẩm nilon, chỉ áp dụng cho khách hàng mới",
                    DiscountPercent = 10,
                    ExpiryDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new CustomerPromotion 
                { 
                    PromotionId = 2, 
                    Title = "Miễn phí vận chuyển", 
                    Description = "Cho đơn hàng từ 500,000 VNĐ trở lên, áp dụng cho toàn quốc",
                    DiscountPercent = 0,
                    ExpiryDate = new DateTime(2025, 8, 15),
                    IsActive = true
                },
                new CustomerPromotion 
                { 
                    PromotionId = 3, 
                    Title = "Tích điểm thưởng", 
                    Description = "Tích 1 điểm cho mỗi 10,000 VNĐ mua hàng. Đổi 1,000 điểm = 50,000 VNĐ",
                    DiscountPercent = 0,
                    ExpiryDate = new DateTime(2025, 12, 31),
                    IsActive = true
                },
                new CustomerPromotion 
                { 
                    PromotionId = 4, 
                    Title = "Giảm 15% cho khách hàng VIP", 
                    Description = "Áp dụng cho khách hàng có ghi chú VIP, giảm tối đa 200,000 VNĐ",
                    DiscountPercent = 15,
                    ExpiryDate = new DateTime(2025, 6, 30),
                    IsActive = true
                },
                new CustomerPromotion 
                { 
                    PromotionId = 5, 
                    Title = "Mua 2 tặng 1", 
                    Description = "Mua 2 cuộn nilon cùng loại, tặng 1 cuộn nilon khác cùng giá trị",
                    DiscountPercent = 0,
                    ExpiryDate = new DateTime(2025, 5, 31),
                    IsActive = true
                }
            };
        }

        private void InitializeControls()
        {
            txtCustomerName.Text = currentCustomer?.CustomerName ?? "Khách hàng";
            
            // Load promotions
            icPromotions.ItemsSource = promotions;

            // Update reward points display
            UpdateRewardPointsDisplay();
        }

        private void UpdateRewardPointsDisplay()
        {
            // Sample data - in real app, this would come from database
            int currentPoints = 1250;
            int usedPoints = 500;
            int totalPoints = currentPoints + usedPoints;

            txtCurrentPoints.Text = currentPoints.ToString("N0");
            txtUsedPoints.Text = usedPoints.ToString("N0");
            txtTotalPoints.Text = totalPoints.ToString("N0");
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }


} 