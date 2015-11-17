using Capstone.WebScraper;
using Microsoft.Win32;
using StockNewsScraper.Scraper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace StockNewsScraper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Initialise startup components
        public MainWindow()
        {
            InitializeComponent();
            CompList = new ObservableCollection<string>();
            StockList.ItemsSource = CompList;

            // Add scraper to list
            NewsSourceList.SelectionMode = SelectionMode.Multiple;
            Scrapers = new ObservableCollection<INewsScraper>();
            Scrapers.Add(new MarketWatch());
            Scrapers.Add(new TheGuardian());
            Scrapers.Add(new NASDAQ());

            NewsSourceList.ItemsSource = Scrapers;

            OutputText.IsReadOnly = true;

        }

        public ObservableCollection<string> CompList { get; set; }
        private ObservableCollection<INewsScraper> Scrapers { get; set; }

        // Button to load text file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                using (var reader = File.OpenText(dialog.FileName))
                {
                    while (!reader.EndOfStream)
                    {
                        CompList.Add(reader.ReadLine());
                    }
                }
            }
        }

        // Button to start scraping
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            OutputText.Text = string.Empty;

            int overallProgress = 0;

            OverallProgress.Value = overallProgress;
            // Collect Price Data from Quandl
            var api = new QuandlApi();

            var selectedList = new List<INewsScraper>();

            foreach (var n in NewsSourceList.SelectedItems)
            {
                selectedList.Add((INewsScraper)n);
            }
            // Loop through each company to collect data.
            foreach (var c in CompList)
            {
                WriteToOutput("Collecting Data for: " + c);

                await Task.Run(() => api.SaveCompanyCSV(c));

                int sourceProgress = 0;
                SourceProgress.Value = sourceProgress;
                // Loop through each each nes scraper for the company
                foreach (INewsScraper s in selectedList)
                {
                    await Task.Delay(1000);
                    await Task.Run(()=> s.Scrape(c.Trim()));
                    WriteToOutput("\t Collecting from: " + s.ToString());

                    sourceProgress++;
                    SourceProgress.Value = sourceProgress * 100/ selectedList.Count;
                }

                overallProgress++;

                OverallProgress.Value = overallProgress * 100 / CompList.Count;
            }
        }

        private void WriteToOutput(string line)
        {
            OutputText.Text = OutputText.Text + (OutputText.Text == string.Empty? string.Empty : "\n") + line;
        }
    }
}
