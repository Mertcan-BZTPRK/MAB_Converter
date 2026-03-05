using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace MAB_Converter
{
    public partial class CustomMessageBox : Window
    {
        private string _filePath = "";

        public CustomMessageBox(string title, string message, bool isDarkMode, string filePath = "")
        {
            InitializeComponent();
            txtTitle.Text = title;
            txtMessage.Text = message;
            _filePath = filePath;

            if (!isDarkMode)
            {
                this.Resources["BgColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#F0F2F5"));
                this.Resources["TextColor"] = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1C1E21"));
            }

            // Eğer dosya yolu gönderildiyse (yani başarılı bir indirme yapıldıysa) Klasörü Aç butonunu göster
            if (!string.IsNullOrEmpty(_filePath))
            {
                btnOpenFolder.Visibility = Visibility.Visible;
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // explorer.exe'ye "/select," parametresi verirsek direkt inen dosyayı işaretleyerek açar
                Process.Start("explorer.exe", $"/select,\"{_filePath}\"");
            }
            catch { }
            this.Close();
        }
    }
}