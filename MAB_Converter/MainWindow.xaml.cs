using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using YoutubeDLSharp;

namespace MAB_Converter
{
    public partial class MainWindow : Window
    {
        private bool isDarkMode = true;
        private MABEngine engine;

        public MainWindow()
        {
            InitializeComponent();
            CheckSystemTheme();
            ApplyTheme();

            engine = new MABEngine();
            InitializeMABEngine();
        }

        private async void InitializeMABEngine()
        {
            lblStatus.Text = "Motor dosyaları kontrol ediliyor...";
            btnAnalyze.IsEnabled = false;

            await engine.InitializeEngineAsync();

            lblStatus.Text = "Sistem Hazır. Bağlantı bekleniyor...";
            btnAnalyze.IsEnabled = true;
        }

        // --- ANALİZ ET BUTONU TIKLANMA OLAYI ---
        private async void btnAnalyze_Click(object sender, RoutedEventArgs e)
        {
            string url = txtUrl.Text.Trim();
            if (string.IsNullOrEmpty(url))
            {
                CustomMessageBox customMsg = new CustomMessageBox("Uyarı", "Lütfen geçerli bir video linki girin!", isDarkMode);
                customMsg.Owner = this;
                customMsg.ShowDialog();
                return;
            }

            try
            {
                lblStatus.Text = "Video analiz ediliyor, lütfen bekleyin...";
                btnAnalyze.IsEnabled = false;
                cmbFormat.Items.Clear();

                gridVideoInfo.Visibility = Visibility.Collapsed;

                var result = await engine.AnalyzeVideoAsync(url);

                foreach (var quality in result.Qualities)
                {
                    cmbFormat.Items.Add(quality);
                }

                if (cmbFormat.Items.Count > 0)
                    cmbFormat.SelectedIndex = 0;

                lblVideoTitle.Text = result.Title;
                if (!string.IsNullOrEmpty(result.ThumbnailUrl))
                {
                    imgThumbnail.Source = new BitmapImage(new Uri(result.ThumbnailUrl));
                }

                gridVideoInfo.Visibility = Visibility.Visible;

                lblStatus.Text = "Analiz tamamlandı. Ayarlarınızı yapıp indirebilirsiniz.";
                btnDownload.IsEnabled = true;
            }
            catch (Exception ex)
            {
                CustomMessageBox customMsg = new CustomMessageBox("Hata", $"Analiz başarısız:\n{ex.Message}", isDarkMode);
                customMsg.Owner = this;
                customMsg.ShowDialog();
                lblStatus.Text = "Analiz başarısız.";
            }
            finally
            {
                btnAnalyze.IsEnabled = true;
            }
        }

        // --- İNDİR BUTONU TIKLANMA OLAYI ---
        private async void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            string url = txtUrl.Text.Trim();
            string selectedFormat = cmbFormat.SelectedItem?.ToString();

            string startTime = $"00:{txtStartMin.Text.PadLeft(2, '0')}:{txtStartSec.Text.PadLeft(2, '0')}";
            string endTime = $"00:{txtEndMin.Text.PadLeft(2, '0')}:{txtEndSec.Text.PadLeft(2, '0')}";

            // +önemli ÇÖZÜM: Motora göndermemiz gereken "videoTitle" bilgisini ekledik!
            string videoTitle = lblVideoTitle.Text;

            try
            {
                btnDownload.IsEnabled = false;
                btnAnalyze.IsEnabled = false;
                pbDownload.Value = 0;
                lblStatus.Text = "İndirme başlatılıyor...";

                var progress = new Progress<DownloadProgress>(p =>
                {
                    pbDownload.Value = p.Progress * 100;
                    lblStatus.Text = $"İndiriliyor: %{Math.Round(p.Progress * 100, 1)} - Hız: {p.DownloadSpeed}";
                });

                // +önemli: videoTitle parametresini eksiksiz olarak motora iletiyoruz. Hata veren satır onarıldı!
                string filePath = await engine.DownloadVideoAsync(url, selectedFormat, startTime, endTime, videoTitle, progress);

                lblStatus.Text = "İndirme Tamamlandı!";

                CustomMessageBox customMsg = new CustomMessageBox("Başarılı", $"Video başarıyla indirildi!\n\nDosya Yolu:\n{filePath}", isDarkMode, filePath);
                customMsg.Owner = this;
                customMsg.ShowDialog();
            }
            catch (Exception ex)
            {
                CustomMessageBox customMsg = new CustomMessageBox("Hata", $"İndirme başarısız oldu:\n{ex.Message}", isDarkMode);
                customMsg.Owner = this;
                customMsg.ShowDialog();
                lblStatus.Text = "İndirme iptal edildi veya başarısız oldu.";
            }
            finally
            {
                btnDownload.IsEnabled = true;
                btnAnalyze.IsEnabled = true;
                pbDownload.Value = 0;
            }
        }

        // --- SİSTEM TEMASINI OKUMA VE UYGULAMA ---
        private void CheckSystemTheme()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize"))
                {
                    if (key != null && key.GetValue("AppsUseLightTheme") != null)
                    {
                        int lightTheme = (int)key.GetValue("AppsUseLightTheme");
                        isDarkMode = (lightTheme == 0);
                    }
                }
            }
            catch { isDarkMode = true; }
        }

        private void ApplyTheme()
        {
            if (isDarkMode)
            {
                btnThemeToggle.Content = "☀️ Light Mod";
                this.Resources["BgColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1E1E1E"));
                this.Resources["PanelColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#2D2D30"));
                this.Resources["TextColor"] = new SolidColorBrush(Colors.White);
                this.Resources["TitleBarColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#151515"));

                txtUrl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                txtUrl.Foreground = new SolidColorBrush(Colors.White);

                txtStartMin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                txtStartMin.Foreground = new SolidColorBrush(Colors.White);
                txtStartSec.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                txtStartSec.Foreground = new SolidColorBrush(Colors.White);

                txtEndMin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                txtEndMin.Foreground = new SolidColorBrush(Colors.White);
                txtEndSec.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                txtEndSec.Foreground = new SolidColorBrush(Colors.White);
                cmbFormat.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3E3E42"));
                cmbFormat.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                btnThemeToggle.Content = "🌙 Dark Mod";
                this.Resources["BgColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                this.Resources["PanelColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFFF"));
                this.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1C1E21"));
                this.Resources["TitleBarColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#E4E6E9"));

                txtUrl.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                txtUrl.Foreground = new SolidColorBrush(Colors.Black);

                txtStartMin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                txtStartMin.Foreground = new SolidColorBrush(Colors.Black);
                txtStartSec.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                txtStartSec.Foreground = new SolidColorBrush(Colors.Black);

                txtEndMin.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                txtEndMin.Foreground = new SolidColorBrush(Colors.Black);
                txtEndSec.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                txtEndSec.Foreground = new SolidColorBrush(Colors.Black);
                cmbFormat.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                cmbFormat.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        // --- PENCERE KONTROLLERİ ---
        private void BtnThemeToggle_Click(object sender, RoutedEventArgs e)
        {
            isDarkMode = !isDarkMode;
            ApplyTheme();
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void BtnMaximize_Click(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Normal)
                this.WindowState = WindowState.Maximized;
            else
                this.WindowState = WindowState.Normal;
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // --- MAB TECH LİNKEDİN YÖNLENDİRMESİ ---
        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                e.Handled = true;

                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://www.linkedin.com/in/mertcan-boztoprak",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                CustomMessageBox customMsg = new CustomMessageBox("Hata", $"Tarayıcı açılamadı:\n{ex.Message}", isDarkMode);
                customMsg.Owner = this;
                customMsg.ShowDialog();
            }
        }
    }
}