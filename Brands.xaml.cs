using LR4__2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LR4__2.Views
{
    /// <summary>
    /// Логика взаимодействия для Brands.xaml
    /// </summary>
    public partial class Brands : Page
    {
        private List<Manufacturer> _allBrand = new List<Manufacturer>();
        private List<Product> _allProd = new List<Product>();
        private bool _sortAscending = true;

        private int _pageSize = 8;
        private int _currentPage = 1;
        private int _totalPage = 1;
        public Brands()
        {
            InitializeComponent();

            var products = BeautySingleton.GetDbContext().Products.Include(b => b.Manufacturer).ToList();
            
            string basePath = AppDomain.CurrentDomain.BaseDirectory;

            foreach(var product in products)
            {
                if (!string.IsNullOrEmpty(product.MainImagePath))
                {
                    product.MainImagePath = System.IO.Path.Combine(basePath, product.MainImagePath);
                }
            }

            _allProd = products;
            lviewBrands.ItemsSource = products;

            ShowPage(1);
        }

        //Pagination Page
        private void ShowPage(int page)
        {
            var list = GetFilteredAndSortedList();

            if (list == null || list.Count == 0)
            {
                lviewBrands.ItemsSource = null;
                tbPageInfo.Text = "Нет товаров";
                pageNumbersPanel.Children.Clear();
                return;
            }

            _totalPage = (int)Math.Ceiling(list.Count / (double)_pageSize);
            if (_totalPage < 1) _totalPage = 1;

            if (page < 1) page = 1;
            if (page > _totalPage) page = _totalPage;

            _currentPage = page;

            var pageItems = list.Skip((_currentPage - 1) * _pageSize).Take(_pageSize).ToList();

            lviewBrands.ItemsSource = pageItems;
            tbPageInfo.Text = $"Страница: {_currentPage} из {_totalPage}";

            CreatePageButtons();
        }

        private List<Product> GetFilteredAndSortedList()
        {
            string searchText = tbSearch.Text?.Trim() ?? "";

            IEnumerable<Product> query = _allProd;

            if (!string.IsNullOrWhiteSpace(searchText))
            {
                query = query.Where(p => (p.Title != null && p.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)) || (p.Manufacturer != null && p.Manufacturer.Name != null && p.Manufacturer.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)));
            }

            query = _sortAscending ? query.OrderBy(x => x.Cost) : query.OrderByDescending(x => x.Cost);

            return query.ToList();
        }

        private void CreatePageButtons()
        {
            pageNumbersPanel.Children.Clear();

            for (int i = 1; i <= _totalPage; i++)
            {
                Button btn = new Button
                {
                    Content = i.ToString(),
                    Width = 36,
                    Height = 32,
                    Margin = new Thickness(3,0,3,0),
                    Tag = i,
                    FontSize = 14
                };

                if (i == _currentPage)
                {
                    btn.Background = Brushes.CornflowerBlue;
                    btn.Foreground = Brushes.White;
                    btn.FontWeight = FontWeights.Bold;
                }

                btn.Click += PageNumber_Click;
                pageNumbersPanel.Children.Add(btn);
            }
        }

        private void PageNumber_Click(object sender, RoutedEventArgs e)
        {
            int page = (int)((Button)sender).Tag;

            ShowPage(page);
        }

        //sorting
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var items = lviewBrands.ItemsSource as IEnumerable<Product>;

            if (items == null) return;

            var sorted = _sortAscending ? items.OrderBy(x => x.Cost).ToList() : items.OrderByDescending(x => x.Cost).ToList();

            _sortAscending = !_sortAscending;

            lviewBrands.ItemsSource = sorted;

            if (sender is Button btn) btn.Content = _sortAscending ? "Сортировать по возрастанию" : "Сортировать по убыванию";

            ShowPage(1);
        }

        //search
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ShowPage(1);
        }

        //click through "enter"
        private void tbSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click_1(sender, e);
            }
        }

        //prevpage btn
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ShowPage(_currentPage - 1);
        }

        //nextpage btn
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            ShowPage(_currentPage + 1);
        }
    }
}