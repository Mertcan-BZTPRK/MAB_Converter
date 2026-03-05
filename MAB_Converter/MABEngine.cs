using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YoutubeDLSharp;
using YoutubeDLSharp.Options;

namespace MAB_Converter
{
    public class MABEngine
    {
        private YoutubeDL ytdl;
        private string currentDir;

        public MABEngine()
        {
            currentDir = AppDomain.CurrentDomain.BaseDirectory;

            ytdl = new YoutubeDL
            {
                YoutubeDLPath = Path.Combine(currentDir, "yt-dlp.exe"),
                FFmpegPath = Path.Combine(currentDir, "ffmpeg.exe")
            };
        }

        public async Task InitializeEngineAsync()
        {
            if (!File.Exists(Path.Combine(currentDir, "yt-dlp.exe")))
            {
                await Utils.DownloadYtDlp();
            }
            if (!File.Exists(Path.Combine(currentDir, "ffmpeg.exe")))
            {
                await Utils.DownloadFFmpeg();
            }
        }

        public async Task<(List<string> Qualities, string ThumbnailUrl, string Title)> AnalyzeVideoAsync(string url)
        {
            var res = await ytdl.RunVideoDataFetch(url);

            if (!res.Success)
                throw new Exception("Video bilgileri alınamadı. Linki kontrol edin.");

            var videoData = res.Data;

            var availableQualities = videoData.Formats
                .Where(f => f.Height != null && f.Height > 0)
                .Select(f => f.Height.Value)
                .Distinct()
                .OrderByDescending(h => h)
                .Select(h => $"MP4 - {h}p")
                .ToList();

            // +önemli ÇÖZÜM 1: LinkedIn gibi siteler çözünürlük (Height) bilgisini gizler.
            // Eğer liste boş kaldıysa (video formatı bulunamadıysa), zorla "Maksimum Kalite" seçeneğini ekliyoruz.
            if (availableQualities.Count == 0)
            {
                availableQualities.Add("MP4 - Maksimum Kalite");
            }

            availableQualities.Add("MP3 - Sadece Ses");

            return (availableQualities, videoData.Thumbnail, videoData.Title);
        }

        private string CleanFileName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "MAB_Video_" + DateTime.Now.Ticks;

            string cleanName = name
                .Replace("ğ", "g").Replace("Ğ", "G")
                .Replace("ü", "u").Replace("Ü", "U")
                .Replace("ş", "s").Replace("Ş", "S")
                .Replace("ı", "i").Replace("İ", "I")
                .Replace("ö", "o").Replace("Ö", "O")
                .Replace("ç", "c").Replace("Ç", "C");

            cleanName = Regex.Replace(cleanName, @"[^a-zA-Z0-9\s\-]", "");
            cleanName = cleanName.Replace(" ", "_");

            if (string.IsNullOrWhiteSpace(cleanName)) return "MAB_Video_" + DateTime.Now.Ticks;
            return cleanName;
        }

        public async Task<string> DownloadVideoAsync(string url, string formatQuality, string startTime, string endTime, string videoTitle, IProgress<DownloadProgress> progress)
        {
            var options = new OptionSet();
            string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            ytdl.OutputFolder = downloadsFolder;
            ytdl.OutputFileTemplate = "%(title)s.%(ext)s";
            options.RestrictFilenames = true;

            options.AddCustomOption("--ffmpeg-location", Path.Combine(currentDir, "ffmpeg.exe"));

            bool isStartActive = !string.IsNullOrWhiteSpace(startTime) && startTime != "00:00:00";
            bool isEndActive = !string.IsNullOrWhiteSpace(endTime) && endTime != "00:00:00";

            if (isStartActive || isEndActive)
            {
                string startStr = isStartActive ? startTime : "00:00:00";
                string endStr = isEndActive ? endTime : "inf";
                options.AddCustomOption("--download-sections", $"*{startStr}-{endStr}");
            }

            if (formatQuality.Contains("MP3"))
            {
                options.Format = "bestaudio/best";
                options.ExtractAudio = true;
                options.AudioFormat = AudioConversionFormat.Mp3;
                options.AudioQuality = 0;
            }
            else
            {
                // +önemli ÇÖZÜM 2: Artık sadece içinde "p" olan sayıları (1080p gibi) arıyoruz. 
                // Eğer bulamazsa (LinkedIn "Maksimum Kalite" seçeneği gibi), limiti 9999 (Sınırsız) yapıyoruz ki 4 piksel saçmalığı yaşanmasın!
                var match = Regex.Match(formatQuality, @"(\d+)p");
                string height = match.Success ? match.Groups[1].Value : "9999";

                if (url.Contains("youtube.com") || url.Contains("youtu.be"))
                {
                    options.Format = $"bestvideo[height<={height}]+bestaudio/bestvideo+bestaudio/best";
                    options.MergeOutputFormat = DownloadMergeFormat.Mkv;
                    options.AddCustomOption("--remux-video", "mp4");
                }
                else
                {
                    // LinkedIn, Instagram, X vb. siteler için:
                    // Height sınırı koymadan, direkt sunucudaki en iyi video ve sesi bulup CRF 18 kalitesiyle MP4 yapıyoruz!
                    options.Format = "bestvideo+bestaudio/best";
                    options.AddCustomOption("--recode-video", "mp4");
                    options.AddCustomOption("--postprocessor-args", "ffmpeg:-c:v libx264 -crf 18 -preset fast -c:a aac -b:a 192k");
                }
            }

            var res = await ytdl.RunVideoDownload(url, progress: progress, overrideOptions: options);

            if (!res.Success)
                throw new Exception("İndirme başarısız!\n" + string.Join("\n", res.ErrorOutput));

            return res.Data;
        }
    }
}