// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;
using System.Runtime.CompilerServices;
using ArgeMup.HazirKod.DonanımHaberleşmesi;
using ArgeMup.HazirKod.ArkaPlan;

namespace ArgeMup.HazirKod
{
    public class Günlük
    {
        public const string Sürüm = "V1.3";
        public static int Seviyesi = 0;
        public static int ZamanAşımı_msn = 1000;
        public static int BaytDizisi_BirSatirdakiBilgiSayisi = 16;
        public static string Şablon_Tarih_Saat_MiliSaniye = "dd.MM.yyyy_HH:mm:ss.fff";

        static string Yolu = null;
        static int UdpSunucusuErişimNoktası = 0;

        /// <summary>
        /// - Dosyaya kaydetme özelliği etkin ise : 30 günden eski ise veya klasör boyutu 50Mib dan büyük ise veya klasörde 500 den fazla dosya var ise en eskileri silen günlük uygulamasıdır
        /// - Fazladan konsol çıktısı üretir
        /// </summary>
        /// <param name="Klasörü">
        /// - null  : dosyaya kaydetmez
        /// - ""    : Klasörü kendisi belirler -> \\?\C:\Users\<KullanıcıAdı>\AppData\Local\Temp\<Aile>\<Uygulama>\<Sürüm>\dd_MM_yyyy_HH_mm_ss.Gunluk
        /// - "Yol" : Doğrudan ilgili klasörü kullanır
        /// </param>
        /// <param name="UdpSunucusuErişimNoktası">1 den küçük değerlerde bu özellik kapalıdır</param>
        /// <exception cref="Exception">Klasör oluşturulamadı</exception>
        public static void Başlat(string Klasörü = "", int UdpSunucusununErişimNoktası = 0)
        {
            if (Klasörü != null)
            {
                if (Klasörü == "") Klasörü = @"\\?\" + Klasör.Depolama(Klasör.Kapsamı.Geçici) + @"\Gunluk";
                if (!Klasör.Oluştur(Klasörü)) throw new Exception("Klasör oluşturulamadı " + Klasörü);

                Yolu = Klasörü + @"\" + D_TarihSaat.Yazıya(DateTime.Now, D_TarihSaat.Şablon_DosyaAdı) + ".Gunluk";

                Dosya.Sil_TarihineGöre(Klasörü, 30, "*.Gunluk");
                Dosya.Sil_BoyutunaGöre(Klasörü, 50 * 1024 * 1024 /*50 MiB*/, "*.Gunluk");
                Dosya.Sil_SayısınaGöre(Klasörü, 500, "*.Gunluk");

                Dosyalama = new Öğütücü_<string>(İşlem_Dosyalama);
            }

            if (UdpSunucusununErişimNoktası > 0)
            {
                UdpSunucusuErişimNoktası = UdpSunucusununErişimNoktası;

                UdpSunucusu = new Öğütücü_<string>(İşlem_UdpSunucusu);
            }

            Ekle("Başladı " + Kendi.DosyaYolu() + " V" + Kendi.Sürümü_Dosya());
        }
        public static void Durdur()
        {
            if (Dosyalama != null)
            {
                Dosyalama.Durdur();
                Dosyalama = null;
            }

            if (UdpSunucusu != null)
            {
                UdpSunucusu.Durdur();
                UdpSunucusu = null;
            }
        }
        public static void Ekle(string Mesaj, int Seviye = 0, [CallerFilePath] string ÇağıranDosya = "", [CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            if (Seviye > Seviyesi) return; //Seviyesi nden küçük eşit mesajları yazdıracak

            string içerik = D_TarihSaat.Yazıya(DateTime.Now, Şablon_Tarih_Saat_MiliSaniye) + " " + Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " " + Mesaj.Replace("\r\n", "|").Replace('\r', '|').Replace('\n', '|');

            Ekle_(içerik);
        }
        public static void Ekle(byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, int Seviye = 0, [CallerFilePath] string ÇağıranDosya = "", [CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            if (Seviye > Seviyesi) return; //Seviyesi nden küçük eşit mesajları yazdıracak

            string başlık = D_TarihSaat.Yazıya(DateTime.Now, Şablon_Tarih_Saat_MiliSaniye) + " " + Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " ";

            if (BaytDizisi == null) Ekle_(başlık + "null");
            else
            {
                if (Adet == int.MinValue) Adet = BaytDizisi.Length - BaşlangıçKonumu;
                if (Adet > BaytDizisi.Length - BaşlangıçKonumu) Adet = BaytDizisi.Length - BaşlangıçKonumu;
                
                if (Adet < 0) Ekle_(başlık + "dizideki " + Adet + " kadar bilginin yazdırılması mümkün olmadığından atlandı");
                else
                {
                    string içerik = başlık + "[" + BaşlangıçKonumu + " - " + (BaşlangıçKonumu + Adet - 1) + "] : " + Adet + " Hex | Aralık | Ascii" + Environment.NewLine;

                    while (Adet > 0)
                    {
                        int şimdiki_adet = Adet;
                        if (şimdiki_adet > BaytDizisi_BirSatirdakiBilgiSayisi) şimdiki_adet = BaytDizisi_BirSatirdakiBilgiSayisi;

                        string Aralık_sayısı = (BaşlangıçKonumu + Adet - 1).ToString().Length.ToString();

                        içerik += başlık;
                        for (int i = 0; i < şimdiki_adet; i++)
                        {
                            içerik += BaytDizisi[BaşlangıçKonumu + i].ToString("X2") + " ";
                        }

                        içerik += string.Format("| {0," + Aralık_sayısı + "} - {1," + Aralık_sayısı + "} | ", BaşlangıçKonumu, BaşlangıçKonumu + şimdiki_adet - 1);

                        for (int i = 0; i < şimdiki_adet; i++)
                        {
                            char şimdiki = (char)BaytDizisi[BaşlangıçKonumu + i];
                            if (char.IsControl(şimdiki)) içerik += " ";
                            else içerik += şimdiki;
                        }

                        içerik += Environment.NewLine;
                        BaşlangıçKonumu += şimdiki_adet;
                        Adet -= şimdiki_adet;
                    }

                    Ekle_(içerik);
                }
            }
        }
        static void Ekle_(string Mesaj)
        {
            Console.WriteLine(Mesaj);

            if (Dosyalama != null) Dosyalama.Ekle(Mesaj);

            if (UdpSunucusu != null) UdpSunucusu.Ekle(Mesaj);
        }

        #region Takipçi
        static Öğütücü_<string> Dosyalama = null;
        static Öğütücü_<string> UdpSunucusu = null;

        static void İşlem_Dosyalama(string içerik, object Hatırlatıcı)
        {
            try
            {
                File.AppendAllText(Yolu, içerik + Environment.NewLine);
            }
            catch (Exception) { }
        }
        static void İşlem_UdpSunucusu(string içerik, object Hatırlatıcı)
        {
            bool asd = UdpVerici.Gönder(içerik + SatırSonu.Karakteri, UdpSunucusuErişimNoktası, "127.0.0.1", ZamanAşımı_msn);
        }
        #endregion
    }
}
