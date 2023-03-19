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
        public const string Sürüm = "V1.4";

        public enum Seviye { Kapalı, BeklenmeyenDurum, Hata, Uyarı, Bilgi, Geveze, HazirKod };
        public static Seviye GenelSeviye = Seviye.Geveze;

        public static int BaytDizisi_BirSatirdakiBilgiSayisi = 16;
        public static string Şablon_Tarih_Saat_MiliSaniye = "dd.MM.yyyy-HH:mm:ss.fff";

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
        public static void Başlat(string Klasörü = "", int UdpSunucusununErişimNoktası = 0, int AzamiToplamDosyaSayısı = 50, int TümDosyaların_KapladığıAlan_bayt = 50 * 1024 * 1024 /*50 MiB*/)
        {
            if (Klasörü != null)
            {
                if (Klasörü == "") Klasörü = @"\\?\" + Klasör.Depolama(Klasör.Kapsamı.Geçici) + @"\Gunluk";
                if (!Klasör.Oluştur(Klasörü)) throw new Exception("Klasör oluşturulamadı " + Klasörü);

                Yolu = Klasörü + @"\" + D_TarihSaat.Yazıya(DateTime.Now, D_TarihSaat.Şablon_DosyaAdı) + ".Gunluk";

                Dosya.Sil_SayısınaVeBoyutunaGöre(Klasörü, AzamiToplamDosyaSayısı, TümDosyaların_KapladığıAlan_bayt, "*.Gunluk");

                Dosyalama = new Öğütücü_<string>(İşlem_Dosyalama, AzamiElemanSayısı:5555);
            }

            if (UdpSunucusununErişimNoktası > 0)
            {
                UdpSunucusuErişimNoktası = UdpSunucusununErişimNoktası;

                UdpSunucusu = new Öğütücü_<string>(İşlem_UdpSunucusu, AzamiElemanSayısı: 5555);
            }

            Ekle("Başladı " + Kendi.DosyaYolu + " V" + Kendi.Sürümü_Dosya);
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
        
        public static void Ekle(string Mesaj, Seviye Seviyesi = Seviye.Geveze, [CallerFilePath] string ÇağıranDosya = "", [CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            if (Seviyesi > GenelSeviye) return;
            if (Mesaj == null) Mesaj = "";

            string içerik = D_TarihSaat.Yazıya(DateTime.Now, Şablon_Tarih_Saat_MiliSaniye) + " " + Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " " + Mesaj.Replace("\r\n", "|").Replace('\r', '|').Replace('\n', '|');

            Ekle_(içerik, Seviyesi, Hemen);
        }
        public static void Ekle(byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, Seviye Seviyesi = Seviye.Geveze, string ÖnYazı = null, [CallerFilePath] string ÇağıranDosya = "", [CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            if (Seviyesi > GenelSeviye) return;

            string başlık = D_TarihSaat.Yazıya(DateTime.Now, Şablon_Tarih_Saat_MiliSaniye) + " " + Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " " + ÖnYazı;

            if (BaytDizisi == null) Ekle_(başlık + "null", Seviyesi, Hemen);
            else
            {
                if (Adet == int.MinValue) Adet = BaytDizisi.Length - BaşlangıçKonumu;
                if (Adet > BaytDizisi.Length - BaşlangıçKonumu) Adet = BaytDizisi.Length - BaşlangıçKonumu;
                
                if (Adet < 0) Ekle_(başlık + "dizideki " + Adet + " kadar bilginin yazdırılması mümkün olmadığından atlandı", Seviyesi, Hemen);
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

                    Ekle_(içerik.TrimEnd(Environment.NewLine.ToCharArray()), Seviyesi, Hemen);
                }
            }
        }
        static void Ekle_(string Mesaj, Seviye Seviyesi, bool Hemen)
        {
            switch (Seviyesi)
            {
                case Seviye.BeklenmeyenDurum:   Mesaj = Vt100.Yazı.Renklendir(Mesaj, Vt100.Renk.Beyaz, Vt100.Renk.Kırmızı); break;
                case Seviye.Hata:               Mesaj = Vt100.Yazı.Renklendir(Mesaj, Vt100.Renk.Kırmızı, Vt100.Renk.Siyah); break;
                case Seviye.Uyarı:              Mesaj = Vt100.Yazı.Renklendir(Mesaj, Vt100.Renk.Sarı, Vt100.Renk.Siyah);    break;
                case Seviye.Bilgi:              Mesaj = Vt100.Yazı.Renklendir(Mesaj, Vt100.Renk.Yeşil, Vt100.Renk.Siyah);   break;
                default: break;
            }

            Console.WriteLine(Mesaj);

            if (Dosyalama != null)
            {
                if (Hemen) İşlem_Dosyalama(Mesaj, null);
                else Dosyalama.Ekle(Mesaj);
            }

            if (UdpSunucusu != null)
            {
                if (Hemen) İşlem_UdpSunucusu(Mesaj, null);  
                else UdpSunucusu.Ekle(Mesaj);
            }
        }

        #region Öğütücü
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
            UdpVerici.Gönder(içerik + SatırSonu.Karakteri, UdpSunucusuErişimNoktası);
        }
        #endregion
    }

    public class Vt100
    {
    	public const string Sürüm = "V1.0";
        public enum Renk { DüzYazı, Siyah, Kırmızı, Yeşil, Sarı, Mavi, Eflatun, CamGöbeği, Beyaz };

        public class Yazı
        {
            public static string Renklendir(string Yazı, Renk YazınınRengi, Renk YüzeyinRengi)
            {
                if (YazınınRengi == Renk.DüzYazı && YüzeyinRengi == Renk.DüzYazı) return Yazı;

                return "\u001b[1" + (YazınınRengi == Renk.DüzYazı ? null : ";" + (YazınınRengi + 29)) + (YüzeyinRengi == Renk.DüzYazı ? null : ";" + (YüzeyinRengi + 39)) + "m" + Yazı + "\u001b[0m";
            }

            public static string Ayıkla(string Yazı, out Renk YazınınRengi, out Renk YüzeyinRengi)
            {
                // \033[1;% d;% dmMesaj\033[0m-> \033 0x1b

                YazınınRengi = Renk.DüzYazı;
                YüzeyinRengi = Renk.DüzYazı;
                Yazı = Yazı.Trim(' ', '\r', '\n');

                try
                {
                    if (Yazı[0] == '\u001b')
                    {
                        int komum_m = Yazı.IndexOf('m');
                        string[] renkler_yazı = Yazı.Substring(4, komum_m - 4).Split(';');
                        Yazı = Yazı.Substring(komum_m + 1);
                        Yazı = Yazı.Remove(Yazı.IndexOf('\u001b'));

                        foreach(var y in renkler_yazı)
                        {
                            int s = int.Parse(y);
                            if (s > 39)
                            {
                                //Yüzey
                                YüzeyinRengi = (Renk)(s - 39);
                            }
                            else if (s > 29)
                            {
                                //Yazı
                                YazınınRengi = (Renk)(s - 29);
                            }
                        }
                    }
                }
                catch (Exception) { }

                return Yazı;
            }
        }
    }
}