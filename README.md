# 🚀 MAB Converter | Ultimate Media Downloader & Converter

![License](https://img.shields.io/badge/License-MIT-green.svg)
![Version](https://img.shields.io/badge/Version-1.0.0-blue.svg)
![Platform](https://img.shields.io/badge/Platform-Windows-lightgrey.svg)
![Framework](https://img.shields.io/badge/Framework-WPF%20%7C%20C%23-purple.svg)

**MAB Converter**, YouTube, LinkedIn, Instagram, X (Twitter) ve daha birçok platformdan yüksek kaliteli (4K/1080p) video ve ses indirmeyi, kırpmayı ve dönüştürmeyi sağlayan profesyonel bir Windows masaüstü uygulamasıdır. 

MAB Tech tarafından geliştirilen bu araç, sıfır kalite kaybı (Remux/Stream Copy) felsefesiyle ve modern "Glassmorphism" (Cam Efekti) UI tasarımıyla öne çıkar.

## ✨ Öne Çıkan Özellikler

- **🔥 Kayıpsız İndirme (Zero Quality Loss):** YouTube'un WebM formatlarını işlerken kaliteyi bozmadan orijinal 4K/1080p MP4 olarak remux eder.
- **🎬 Akıllı Kırpma:** İndirme esnasında belirlediğiniz Dakika:Saniye aralığını CRF-18 yüksek kalitede keser.
- **🧠 Akıllı Codec Motoru:** LinkedIn veya Instagram gibi platformların desteklenmeyen formatlarını anında standart H.264 MP4'e çevirerek "Siyah Ekran" sorununu kökten çözer.
- **🎨 Modern Glass UI:** Işık/Karanlık (Light/Dark) tema desteği ve akıcı animasyonlara sahip yuvarlatılmış cam tasarımı.
- **⚡ Otomatik Bağımlılık Yönetimi:** Arka planda çalışan `yt-dlp` ve `ffmpeg` motorlarını ilk açılışta otomatik olarak günceller ve yapılandırır.

## 📥 Kurulum & İndirme

Projeyi kaynak kodundan derlemekle uğraşmak istemiyorsanız, doğrudan kurulabilir `.exe` versiyonunu indirebilirsiniz.

1. **[Releases klasörüne gidin](./Releases/)** veya GitHub Releases sekmesine tıklayın.
2. `MAB_Converter_Setup_v1.0.exe` dosyasını indirin.
3. Kurulumu çalıştırın (Tüm bağımlılıklar ve motorlar otomatik olarak kurulacaktır).
4. Masaüstündeki MAB Converter ikonuna tıklayarak keyfini çıkarın!

## 💻 Geliştiriciler İçin (Kaynak Kodu)

Projeyi kendi bilgisayarınızda geliştirmek istiyorsanız:

1. Bu depoyu klonlayın: `git clone https://github.com/KULLANICI_ADIN/MAB_Converter.git`
2. Visual Studio 2022 ile `MAB_Converter.sln` dosyasını açın.
3. Gerekli NuGet paketlerini (YoutubeDLSharp vb.) geri yükleyin (Restore).
4. Projeyi derleyip çalıştırın.

## 🛠️ Kullanılan Teknolojiler
* **C# / WPF:** Arayüz ve temel programlama mantığı.
* **YoutubeDLSharp:** Medya analiz ve indirme köprüsü.
* **yt-dlp & FFmpeg:** Arka plan medya motorları (Gelişmiş komut setleriyle optimize edilmiştir).
* **Inno Setup:** Kurulum (Setup) paketlemesi.

## 📝 Lisans
Bu proje **MIT Lisansı** ile lisanslanmıştır. Daha fazla bilgi için `LICENSE` dosyasına göz atabilirsiniz.

---
*Geliştirici:* **[Mertcan Boztoprak (MAB Tech)](https://www.linkedin.com/in/mertcan-boztoprak)**
