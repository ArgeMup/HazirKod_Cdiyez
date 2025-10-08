// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;
using System.Runtime.CompilerServices;
using ArgeMup.HazirKod.DonanımHaberleşmesi;
using ArgeMup.HazirKod.ArkaPlan;
using System.Text;

namespace ArgeMup.HazirKod
{
    public class Günlük
    {
        public const string Sürüm = "V1.5";

        public enum Seviye { Kapalı, BeklenmeyenDurum, Hata, Uyarı, Bilgi, Geveze, HazirKod };
        public static Seviye GenelSeviye = Seviye.Geveze;

        public static int BaytDizisi_BirSatirdakiBilgiSayisi = 16;
        public static string Şablon_Tarih_Saat_MiliSaniye = "dd.MM.yyyy-HH:mm:ss.fff";
        public static long Dosyalama_AzamiBoyutu_Bayt = 1 * 1024 * 1024 /*1 MiB*/;
        public static int Aktarma_ZamanAşımı_msn = 5000;

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
        public static void Başlat(string Klasörü = "", int UdpSunucusununErişimNoktası = 0, int AzamiToplamDosyaSayısı = 50, long TümDosyaların_KapladığıAlan_bayt = 50 * 1024 * 1024 /*50 MiB*/)
        {
            if (Klasörü != null)
            {
                if (Klasörü == "") Klasörü = @"\\?\" + Klasör.Depolama(Klasör.Kapsamı.Geçici) + @"\Gunluk";
                else Klasörü = Klasörü.TrimEnd('\\');
                Klasör.Oluştur(Klasörü);

                Dosyalama = new Dosyalama_(Klasörü + "\\", Aktarma_ZamanAşımı_msn, AzamiToplamDosyaSayısı, TümDosyaların_KapladığıAlan_bayt); 
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
                Dosyalama.Dispose();
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

            string içerik = 
                D_TarihSaat.Yazıya(DateTime.UtcNow, Şablon_Tarih_Saat_MiliSaniye) + " " + 
                Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " " +
                Mesaj;

            Ekle_(içerik, Seviyesi, Hemen);
        }
        public static void Ekle(byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, Seviye Seviyesi = Seviye.Geveze, string ÖnYazı = null, [CallerFilePath] string ÇağıranDosya = "", [CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            if (Seviyesi > GenelSeviye) return;

            string başlık = D_TarihSaat.Yazıya(DateTime.UtcNow, Şablon_Tarih_Saat_MiliSaniye) + " " + Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " " + ÖnYazı;

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

                        içerik += "\t";
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

                        BaşlangıçKonumu += şimdiki_adet;
                        Adet -= şimdiki_adet;
                        if (Adet > 0) içerik += Environment.NewLine;
                    }

                    Ekle_(içerik, Seviyesi, Hemen);
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

            #if DEBUG
                Console.WriteLine(Mesaj);

                #if NET7_0_OR_GREATER
                    System.Diagnostics.Debug.WriteLine(Mesaj);
                    #warning Günlük işlemleri ciddi oranda YAVAŞLADI
                #endif
            #endif

            Dosyalama?.Ekle(Mesaj, Hemen);
            
            if (UdpSunucusu != null)
            {
                if (Hemen) İşlem_UdpSunucusu(Mesaj, null);  
                else UdpSunucusu.Ekle(Mesaj);
            }
        }

        #region Öğütücü
        static Dosyalama_ Dosyalama = null;
        class Dosyalama_ : IDisposable
        {
            private readonly StringBuilder Sb = new StringBuilder();
            private readonly System.Threading.SemaphoreSlim Kilit = new System.Threading.SemaphoreSlim(1, 1);
            private readonly System.Threading.Timer Zamanlayici;
            private readonly int ZamanAşımı_msn;
            private readonly int Kilit_Devralma_ZamanAşımı_msn = 10000;
            private readonly string Şablon_Tarih = "dd_MM_yyyy";
            private readonly string Şablon_Saat_MiliSaniye = "HH_mm_ss_fff";

            string Dosya_Yolu_KökKlasör;
            string Dosya_Yolu;
            long Dosya_Boyutu = -1;
            (int AzamiToplamDosyaSayısı, long TümDosyaların_KapladığıAlan_bayt)? İlkAçılışİşlemleri_Null_Yapıldı_Dolu_Yapılmadı;

            public Dosyalama_(string KökKlasör, int ZamanAşımı_msn, int AzamiToplamDosyaSayısı, long TümDosyaların_KapladığıAlan_bayt)
            {
                Dosya_Yolu_KökKlasör = KökKlasör;
                İlkAçılışİşlemleri_Null_Yapıldı_Dolu_Yapılmadı = (AzamiToplamDosyaSayısı, TümDosyaların_KapladığıAlan_bayt);
                this.ZamanAşımı_msn = ZamanAşımı_msn;

                Zamanlayici = new System.Threading.Timer(async _ => await SüreDoldu(), null, System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
            }
            public void Dispose()
            {
                Zamanlayici.Dispose();
                Kilit.Dispose();
                Sb.Clear();
            }

            public async System.Threading.Tasks.Task Ekle(string Mesaj, bool Hemen)
            {
                if (!await Kilit.WaitAsync(Kilit_Devralma_ZamanAşımı_msn)) return;

                try
                {
                    Sb.AppendLine(Mesaj);

                    Zamanlayici.Change(Hemen ? 0 : (Sb.Length > (100 * 1024 /*100 KiB*/) ? 100 : ZamanAşımı_msn), System.Threading.Timeout.Infinite);
                }
                finally
                {
                    Kilit.Release();
                }
            }
            private async System.Threading.Tasks.Task SüreDoldu()
            {
                if (!await Kilit.WaitAsync(Kilit_Devralma_ZamanAşımı_msn))
                {
                    Zamanlayici.Change(ZamanAşımı_msn, System.Threading.Timeout.Infinite);
                    return;
                }

                try
                {
                    string Mesajlar = Sb.ToString();
                    int Boyutu = Sb.Length;
                    Sb.Clear();

                    if (İlkAçılışİşlemleri_Null_Yapıldı_Dolu_Yapılmadı != null)
                    {
                        (int AzamiToplamDosyaSayısı, long TümDosyaların_KapladığıAlan_bayt)? gecici = İlkAçılışİşlemleri_Null_Yapıldı_Dolu_Yapılmadı;
                        İlkAçılışİşlemleri_Null_Yapıldı_Dolu_Yapılmadı = null;

                        Klasör_ kls = new Klasör_(Dosya_Yolu_KökKlasör, Filtre_Dosya: new string[] { "*.Gunluk" }, DoğrulamaKodunuÜret: false);
                        kls.Dosya_Sil_SayısınaVeBoyutunaGöre(gecici.Value.AzamiToplamDosyaSayısı, gecici.Value.TümDosyaların_KapladığıAlan_bayt);
                        Klasör.Sil_İçiBoşOlanları(Dosya_Yolu_KökKlasör);

                        kls.Güncelle();
                        kls.Sırala_EskidenYeniye();

                        if (kls.Dosyalar.Count > 0)
                        {
                            var en_yeni = kls.Dosyalar[kls.Dosyalar.Count - 1];
                            if (en_yeni.KapladığıAlan_bayt < Dosyalama_AzamiBoyutu_Bayt &&
                                en_yeni.Yolu.Contains(D_TarihSaat.Yazıya(DateTime.UtcNow, Şablon_Tarih)))
                            {
                                Dosya_Yolu = Dosya_Yolu_KökKlasör + en_yeni.Yolu;
                                Dosya_Boyutu = en_yeni.KapladığıAlan_bayt;
                                Mesajlar = Environment.NewLine + Mesajlar;
                            }
                        }
                    }

                    if (Dosya_Boyutu == -1 ||
                        Dosya_Boyutu > Dosyalama_AzamiBoyutu_Bayt)
                    {
                        DateTime şimdi = DateTime.UtcNow;
                        string Klasörü = Dosya_Yolu_KökKlasör + D_TarihSaat.Yazıya(şimdi, Şablon_Tarih);
                        Directory.CreateDirectory(Klasörü);

                        Dosya_Yolu = Klasörü + "\\" + D_TarihSaat.Yazıya(şimdi, Şablon_Saat_MiliSaniye) + ".Gunluk";
                        Dosya_Boyutu = 0;
                    }

                    await File.AppendAllTextAsync(Dosya_Yolu, Mesajlar, Encoding.UTF8);
                    Dosya_Boyutu += Boyutu;
                }
                finally
                {
                    Kilit.Release();
                }
            }
        }

        static Öğütücü_<string> UdpSunucusu = null;
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