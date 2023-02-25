using Project.DataAccess.Repository;
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

using Project.DataAccess;
using Project.DataAccess.Repository.IRepository;
using Project.Models;
using Project.DataAccess.Data;
using Bogus.DataSets;
using System.Text.Json;
using System.Net.Http;
using System.Net.WebSockets;

namespace WPF_Admin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Report> reportList;
        List<Report> reportListCurrent;
        HttpClient client;
        HttpResponseMessage response;
        private string foodID;
        private string userID;
        int page = 0;
        int pageSize = 9;

        public MainWindow()
        {
            InitializeComponent();
            reportList = new List<Report>();
            reportListCurrent = new List<Report>();
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44358");
            loadListAsync();
        }

        private async Task loadListAsync()
        {
            response = await client.GetAsync("api/FoodReport");
            if (response.IsSuccessStatusCode)
            {
                string result = response.Content.ReadAsStringAsync().Result;
                var obj = JsonSerializer.Deserialize<List<Report>>(result);
                reportList = obj;
            }
            reportListCurrent = reportList;

            setListCurrent();
        }

        private void btn_accept_Click(object sender, RoutedEventArgs e)
        {
            if (checkNull()) { return; }

            string mess = "";
            try
            {
                response = client.PostAsync("api/FoodReport/Accept?foodID=" + foodID + "&userID=" + userID, null).Result;
                if (response.IsSuccessStatusCode)
                {
                    mess = "Đã chấp nhận";
                }
                else
                {
                    mess = "Một số lỗi đã xảy ra...";
                }
                MessageBox.Show("Báo cáo món ăn: " + mess, "Thông báo");
                loadListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Một số lỗi đã xảy ra...");
            }
        }

        private void btn_reject_Click(object sender, RoutedEventArgs e)
        {
            if (checkNull()) { return; }

            string mess = "";
            try
            {
                response = client.PostAsync("api/FoodReport/Reject?foodID=" + foodID + "&userID=" + userID, null).Result;
                if (response.IsSuccessStatusCode)
                {
                    mess = "Đã từ chối";
                }
                else
                {
                    mess = "Một số lỗi đã xảy ra...";
                }
                MessageBox.Show("Báo cáo món ăn: " + mess, "Thông báo");
                loadListAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Một số lỗi đã xảy ra...");
            }
        }

        private bool checkNull()
        {
            foodID = (lvReports.SelectedItem as Report).FoodId + "";
            userID = (lvReports.SelectedItem as Report).UserId + "";

            if (String.IsNullOrEmpty(foodID) || String.IsNullOrEmpty(userID))
            {
                MessageBox.Show("Vui lòng chọn báo cáo!!!", "Thông báo");
                return true;
            }
            return false;
        }

        private bool IsMaximize = false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if (IsMaximize)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height = 720;

                    IsMaximize = false;
                }
                else
                {
                    this.WindowState = WindowState.Maximized;

                    IsMaximize = true;
                }
            }
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        private void btn_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        IList<Report> GetPage(IList<Report> list, int page, int pageSize)
        {
            return list.Skip(page * pageSize).Take(pageSize).ToList();
        }

        private void setListCurrent()
        {
            var checkListFilter = reportList
                .Where(r => r.Food.Name.ToLower().Contains(textBoxFilter.Text.ToLower())
                || r.User.FirstName.ToLower().Contains(textBoxFilter.Text.ToLower())
                || r.User.LastName.ToLower().Contains(textBoxFilter.Text.ToLower())).ToList();

            quantity.Text = checkListFilter.Count + "";
            
            reportListCurrent = GetPage(checkListFilter, page, pageSize)
                .ToList();

            lvReports.ItemsSource = reportListCurrent;

            if (page - 1 >= 0)
            {
                btnPage2.Content = page;
                btnBack.IsEnabled = true;
                btnPage2.Visibility = Visibility.Visible;
            }
            else
            {
                btnPage2.Visibility = Visibility.Collapsed;
                btnBack.IsEnabled = false;
            }

            if (page - 2 >= 0)
            {
                btnPage1.Content = page-1;
                btnPage1.Visibility = Visibility.Visible;
            }
            else
            {
                btnPage1.Visibility = Visibility.Collapsed;
            }

            btnPage3.Content = page+1;

            if (reportListCurrent.Count == pageSize)
            {
                btnPage4.Visibility = Visibility.Visible;
                btnPage4.Content = page + 2;
                btnNext.IsEnabled = true;
            } 
            else
            {
                btnPage4.Visibility = Visibility.Collapsed;

                btnNext.IsEnabled = false;
            }

            var theNextNextList = GetPage(checkListFilter, page+1, pageSize);
            if (theNextNextList.Count == pageSize) 
            {
                btnPage5.Visibility = Visibility.Visible;
                btnPage5.Content = page + 3;
            }
            else
            {
                btnPage5.Visibility = Visibility.Collapsed;
            }
        }

        private void Search_KeyUp(object sender, KeyEventArgs e)
        {
            page = 0;
            setListCurrent();
        }

        private void Back1_Click(object sender, RoutedEventArgs e)
        {
            page -= 1;
            setListCurrent();
        }

        private void Back2_Click(object sender, RoutedEventArgs e)
        {
            page -= 2;
            setListCurrent();
        }

        private void Next1_Click(object sender, RoutedEventArgs e)
        {
            page += 1;
            setListCurrent();
        }

        private void Next2_Click(object sender, RoutedEventArgs e)
        {
            page += 2;
            setListCurrent();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(page > 0)
            {
                page--;
            }
            setListCurrent();
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            if (reportListCurrent.Count == pageSize)
            {
                page++;
            }
            setListCurrent();
        }

        private void btnReload_Click(object sender, RoutedEventArgs e)
        {
            loadListAsync();
            MessageBox.Show("Đã tải lại báo cáo", "Thông báo");
        }
    }
}
