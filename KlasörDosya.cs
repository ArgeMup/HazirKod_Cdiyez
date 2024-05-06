// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;
using System.Threading;
using ArgeMup.HazirKod.Ekİşlemler;

namespace ArgeMup.HazirKod
{
    public class Klasör
    {
        public const string Sürüm = "V1.4";
        public const int EşZamanlıİşlemSayısı_Sabiti = 10;

        public static string[] Listele_Dosya(string Yolu, string Filtre = "*.*", SearchOption Kapsam = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(Yolu)) return new string[0];

            return Directory.GetFiles(Yolu, Filtre, Kapsam);
        }
        public static string[] Listele_Klasör(string Yolu, string Filtre = "*", SearchOption Kapsam = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(Yolu)) return new string[0];

            return Directory.GetDirectories(Yolu, Filtre, Kapsam);
        }

        public static bool VarMı(string Yolu)
        {
            return Directory.Exists(Yolu);
        }
        public static void Oluştur(string Yolu)
        {
            Directory.CreateDirectory(Yolu);
        }
        public static void Sil(string Yolu)
        {
            if (!Directory.Exists(Yolu)) return;

            Directory.Delete(Yolu, true);
        }
        public static void Sil_İçiBoşOlanları(string Yolu)
        {
            if (!Directory.Exists(Yolu)) return;

            string[] klsler = Directory.GetDirectories(Yolu, "*", SearchOption.AllDirectories);
            for (int i = klsler.Length - 1; i >= 0; i--)
            {
                _Sil_(klsler[i]);
            }
            _Sil_(Yolu);

            void _Sil_(string _Yolu_)
            {
                string[] dsy_lar = Directory.GetFiles(_Yolu_, "*.*", SearchOption.TopDirectoryOnly);
                string[] kls_ler = Directory.GetDirectories(_Yolu_, "*", SearchOption.TopDirectoryOnly);
                if (dsy_lar.Length == 0 && kls_ler.Length == 0) Sil(_Yolu_);
            }
        }
        public static bool Kopyala(string Kaynak, string Hedef, bool HedeftekiFazlaKlasörVeDosyalarıSil = false, bool DoğrulamaKodunuKontrolEt_Yavaşlatır = true, int EşZamanlıİşlemSayısı = EşZamanlıİşlemSayısı_Sabiti, bool AynıDoğrulamaKodunaSahipİse_DiğerFarklılıklarıGörmezdenGel = false)
        {
            if (AynıDoğrulamaKodunaSahipİse_DiğerFarklılıklarıGörmezdenGel) DoğrulamaKodunuKontrolEt_Yavaşlatır = true;

            Klasör_ Kaynaktakiler = new Klasör_(Kaynak, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false, DoğrulamaKodunuÜret: DoğrulamaKodunuKontrolEt_Yavaşlatır);
            Klasör_ Hedeftekiler = new Klasör_(Hedef, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false, DoğrulamaKodunuÜret: DoğrulamaKodunuKontrolEt_Yavaşlatır);
            while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && (Kaynaktakiler.Dosyalar == null || Hedeftekiler.Dosyalar == null)) Thread.Sleep(5);

            Klasör_.Farklılık_ Farklar = Hedeftekiler.AslınaUygunHaleGetir(Kaynaktakiler, HedeftekiFazlaKlasörVeDosyalarıSil);

            if (AynıDoğrulamaKodunaSahipİse_DiğerFarklılıklarıGörmezdenGel && Farklar.Dosyalar.Count != 0)
            {
                List<Klasör_.Fark_Dosya_> Silinecekler = new List<Klasör_.Fark_Dosya_>();

                foreach (Klasör_.Fark_Dosya_ Farklılık in Farklar.Dosyalar)
                {
                    if (Farklılık.Aynı_Doğrulama_Kodu) Silinecekler.Add(Farklılık);
                }

                foreach (Klasör_.Fark_Dosya_ Farklılık in Silinecekler)
                {
                    Farklar.Dosyalar.Remove(Farklılık);
                }
            }

            return Kaynaktakiler.FizikselOlarakMevcut && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
        }
        public static bool AynıMı(string Sol, string Sağ, bool DoğrulamaKodunuKontrolEt_Yavaşlatır = true, int EşZamanlıİşlemSayısı = EşZamanlıİşlemSayısı_Sabiti)
        {
            Klasör_ Soldaki = new Klasör_(Sol, DoğrulamaKodunuÜret: DoğrulamaKodunuKontrolEt_Yavaşlatır, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle:false);
            Klasör_ Sağdaki = new Klasör_(Sağ, DoğrulamaKodunuÜret: DoğrulamaKodunuKontrolEt_Yavaşlatır, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle:false);
            while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && (Soldaki.Dosyalar == null || Sağdaki.Dosyalar == null)) Thread.Sleep(5);

            Klasör_.Farklılık_ Farklar = Soldaki.Karşılaştır(Sağdaki);

            return Soldaki.FizikselOlarakMevcut && Sağdaki.FizikselOlarakMevcut && Farklar.FarklılıkSayısı == 0;
        }
        public static bool Eşitle(string Sol, string Sağ, int EşZamanlıİşlemSayısı = EşZamanlıİşlemSayısı_Sabiti)
        {
            Klasör_ Soldaki = new Klasör_(Sol, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false, DoğrulamaKodunuÜret:false);
            Klasör_ Sağdaki = new Klasör_(Sağ, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false, DoğrulamaKodunuÜret:false);
            while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && (Soldaki.Dosyalar == null || Sağdaki.Dosyalar == null)) Thread.Sleep(5);

            Klasör_.Farklılık_ Farklar = Soldaki.Eşitle(Sağdaki);

            return (Soldaki.FizikselOlarakMevcut || Sağdaki.FizikselOlarakMevcut) && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
        }

		public enum Kapsamı { Geçici, Masaüstü, TümKullanıcılar, BuKullanıcı_BuİşletimSistemi, BuKullanıcı_TümİşletimSistemleri };
        /// <summary>
        /// Geçici                              : C:\Users\<Kullanıcı Adı>\AppData\Local\Temp\<Aile>\<Uygulama>\<Sürüm>
        /// Masaüstü                            : C:\Users\<Kullanıcı Adı>\Desktop\<Aile>\<Uygulama>\<Sürüm>
        /// TümKullanıcılar                     : C:\ProgramData\<Aile>\<Uygulama>\<Sürüm>
        /// BuKullanıcı_BuİşletimSistemi        : C:\Users\<Kullanıcı Adı>\AppData\Local\<Aile>\<Uygulama>\<Sürüm>
        /// BuKullanıcı_TümİşletimSistemleri    : C:\Users\<Kullanıcı Adı>\AppData\Roaming\<Aile>\<Uygulama>\<Sürüm>
        /// </summary>
        public static string Depolama(Kapsamı HedeflenenKapsamı = Kapsamı.BuKullanıcı_BuİşletimSistemi, string Aile = null, string Uygulama = null, string Sürüm = null)
        {
            Environment.SpecialFolder Tür = 0;
            if (Aile == null) Aile = "ArGeMuP";
            if (Uygulama == null) Uygulama = Kendi.Adı;
            if (Sürüm == null) Sürüm = Kendi.Sürümü_Dosya;

            string kls = null;
            if (HedeflenenKapsamı == Kapsamı.Geçici) kls = Path.GetTempPath().TrimEnd('\\');
            else
            {
                switch (HedeflenenKapsamı)
                {
                    case Kapsamı.Masaüstü:
                        Tür = Environment.SpecialFolder.DesktopDirectory;
                        break;

                    case Kapsamı.TümKullanıcılar:
                        Tür = Environment.SpecialFolder.CommonApplicationData;
                        break;

                    case Kapsamı.BuKullanıcı_BuİşletimSistemi:
                        Tür = Environment.SpecialFolder.LocalApplicationData;
                        break;

                    case Kapsamı.BuKullanıcı_TümİşletimSistemleri:
                        Tür = Environment.SpecialFolder.ApplicationData;
                        break;
                }

                kls = Environment.GetFolderPath(Tür);
            }

            if (!string.IsNullOrEmpty(Aile)) kls += @"\" + Aile;
            if (!string.IsNullOrEmpty(Uygulama)) kls += @"\" + Uygulama;
            if (!string.IsNullOrEmpty(Sürüm)) kls += @"\" + Sürüm;

            return kls;
		} 

        public static string ÜstKlasör(string Yolu, int Seviye = 1)
        {
            int Detay_Seviye = 0;
            DirectoryInfo Detay = new DirectoryInfo(Yolu.Trim().TrimEnd('\\'));

            while (Detay_Seviye < Seviye)
            {
                Detay = Detay.Parent;
                Detay_Seviye++;
            }

            return Detay.FullName;
        }
    }

    public class Dosya
    {
        public const string Sürüm = "V1.4";

        public static bool VarMı(string Yolu)
        {
            return File.Exists(Yolu);
        }
        public static string Klasörü(string Yolu)
        {
            return Klasör.ÜstKlasör(Yolu);
        }
        public static string SadeceAdı(string DosyaYolu)
        {
            return Path.GetFileName(DosyaYolu);
        }
        public static string SadeceSoyAdı(string DosyaYolu)
        {
            return Path.GetExtension(DosyaYolu).Substring(1);
        }
        public static string Oku_Yazı(string DosyaYolu)
        {
            return File.ReadAllText(DosyaYolu);
        }
        public static byte[] Oku_BaytDizisi(string DosyaYolu)
        {
            return File.ReadAllBytes(DosyaYolu);
        }
        public static void Yaz(string DosyaYolu, string Yazı)
        {
            Directory.CreateDirectory(Klasörü(DosyaYolu));

            File.WriteAllText(DosyaYolu, Yazı);
        }
        public static void Yaz(string DosyaYolu, byte[] BaytDizisi)
        {
            Directory.CreateDirectory(Klasörü(DosyaYolu));

            File.WriteAllBytes(DosyaYolu, BaytDizisi);
        }
        public static void Kopyala(string Kaynak, string Hedef)
        {
            Directory.CreateDirectory(Klasörü(Hedef));

            File.Copy(Kaynak, Hedef, true);
            File.SetAttributes(Hedef, File.GetAttributes(Kaynak));
            File.SetLastWriteTime(Hedef, File.GetLastWriteTime(Kaynak));
        }
        public static void Sil(string Yolu)
        {
            if (!File.Exists(Yolu)) return;

            File.Delete(Yolu);
        }
        public static bool Sil_TarihineGöre(string Klasörü, double Gün, IEnumerable<string> Filtre = null, bool Filtre_BüyükKüçükHarfDuyarlı = true, char Filtre_Ayraç = '*', int EşZamanlıİşlemSayısı = Klasör.EşZamanlıİşlemSayısı_Sabiti)
        {
            Klasör_ kls = new Klasör_(Klasörü, Filtre_Dosya: Filtre, DoğrulamaKodunuÜret: false, Filtre_BüyükKüçükHarfDuyarlı: Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç: Filtre_Ayraç, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_TarihineGöre(Gün);
        }
        public static bool Sil_BoyutunaGöre(string Klasörü, long TümDosyaların_KapladığıAlan_bayt, IEnumerable<string> Filtre = null, bool Filtre_BüyükKüçükHarfDuyarlı = true, char Filtre_Ayraç = '*', int EşZamanlıİşlemSayısı = Klasör.EşZamanlıİşlemSayısı_Sabiti)
        {
            Klasör_ kls = new Klasör_(Klasörü, Filtre_Dosya: Filtre, DoğrulamaKodunuÜret: false, Filtre_BüyükKüçükHarfDuyarlı: Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç: Filtre_Ayraç, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_BoyutunaGöre(TümDosyaların_KapladığıAlan_bayt);
        }
        public static bool Sil_SayısınaGöre(string Klasörü, int AzamiToplamDosyaSayısı, IEnumerable<string> Filtre = null, bool Filtre_BüyükKüçükHarfDuyarlı = true, char Filtre_Ayraç = '*', int EşZamanlıİşlemSayısı = Klasör.EşZamanlıİşlemSayısı_Sabiti)
        {
            Klasör_ kls = new Klasör_(Klasörü, Filtre_Dosya: Filtre, DoğrulamaKodunuÜret: false, Filtre_BüyükKüçükHarfDuyarlı: Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç: Filtre_Ayraç, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_SayısınaGöre(AzamiToplamDosyaSayısı);
		}
        public static bool Sil_SayısınaVeBoyutunaGöre(string Klasörü, int AzamiToplamDosyaSayısı, int TümDosyaların_KapladığıAlan_bayt, IEnumerable<string> Filtre = null, bool Filtre_BüyükKüçükHarfDuyarlı = true, char Filtre_Ayraç = '*', int EşZamanlıİşlemSayısı = Klasör.EşZamanlıİşlemSayısı_Sabiti)
        {
            Klasör_ kls = new Klasör_(Klasörü, Filtre_Dosya: Filtre, DoğrulamaKodunuÜret: false, Filtre_BüyükKüçükHarfDuyarlı: Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç: Filtre_Ayraç, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_SayısınaVeBoyutunaGöre(AzamiToplamDosyaSayısı, TümDosyaların_KapladığıAlan_bayt);
        }
        public static bool BaşkaBirYerdeAçıkMı(string Yolu, int ZamanAşımı_msn = 5000)
        {
            if (!File.Exists(Yolu)) return false;

            int za = Environment.TickCount + ZamanAşımı_msn;
            while (za > Environment.TickCount)
            {
                try
                {
                    FileStream KilitDosyası = new FileStream(Yolu, FileMode.Open, FileAccess.Read, FileShare.None);
                    KilitDosyası.Close();

                    return false;
                }
                catch (Exception) { }

                Thread.Sleep(15);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            return true;
        }
        public static bool GüncelMi(string DosyaYolu, string Sürüm)
        {
            if (!File.Exists(DosyaYolu)) return false;
            if (Sürüm == "0.0") return true;
            
            string[] Sürümler = Sürüm.Split('.');
            if (Sürümler.Length != 2) throw new Exception("Sürüm 0.0 şeklinde olmalı");

            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(DosyaYolu);

            int Sürüm_Y = Sürümler[0].TamSayıya(), Sürüm_D = Sürümler[1].TamSayıya();

            if (fvi.FileMajorPart < Sürüm_Y) return false;

            if (fvi.FileMajorPart == Sürüm_Y)
            {
                if (fvi.FileMinorPart < Sürüm_D) return false;
            }

            return true;
        }

        public class AğÜzerinde_ : IDisposable
        {
            public const string Sürüm = "V1.1";
            public Uri Url;
            public object HedefDosyaYolu_Veya_İçeriği;
            public bool KontrolTamamlandı = false;
            public bool Sonuç = false;

            System.Net.Http.HttpClient İstemci = null;

            /// <param name="HedefDosyaYolu">Boş ise içerik bayt dizisi olarak döndürülür</param>
            /// <param name="GeriBildirim_Tamamlandı">Sonuç İndirilenDosya_url İndirilenDosya_Adı_Veya_İçeriği/param>                                                   
            public AğÜzerinde_(Uri Url, string HedefDosyaYolu, Action<bool, Uri, object> GeriBildirim_Tamamlandı = null, int ZamanAşımı_msn = 15000, Dictionary<string, string> İstekBaşlıkları = null)
            {
                this.Url = Url;
                HedefDosyaYolu_Veya_İçeriği = HedefDosyaYolu;

                System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(1); //eşzamanlı çalışma için

                    if (HedefDosyaYolu.DoluMu(true))
                    {
                        if (BaşkaBirYerdeAçıkMı(HedefDosyaYolu, ZamanAşımı_msn)) return;
                        
                        string kls = Klasörü(HedefDosyaYolu);
                        if (kls.DoluMu(true)) Klasör.Oluştur(kls);
                    }

                    using (İstemci = new System.Net.Http.HttpClient())
                    {
                        İstemci.Timeout = TimeSpan.FromMilliseconds(ZamanAşımı_msn);
                        if (İstekBaşlıkları != null)
                        {
                            foreach (KeyValuePair<string, string> İstekBaşlığı in İstekBaşlıkları)
                            {
                                if (İstekBaşlığı.Key.BoşMu(true) || İstekBaşlığı.Value.BoşMu(true)) continue;

                                İstemci.DefaultRequestHeaders.Add(İstekBaşlığı.Key, İstekBaşlığı.Value);
                            }
                        }

                        using (System.Net.Http.HttpResponseMessage cevap = await İstemci.GetAsync(Url))
                        {
                            if (cevap.IsSuccessStatusCode)
                            {
                                byte[] içerik = await cevap.Content.ReadAsByteArrayAsync();

                                if (HedefDosyaYolu.DoluMu(true))
                                {
                                    File.WriteAllBytes(HedefDosyaYolu, içerik);
                                    Sonuç = File.Exists(HedefDosyaYolu);
                                }
                                else
                                {
                                    HedefDosyaYolu_Veya_İçeriği = içerik;
                                    Sonuç = içerik != null;
                                }
                            }
                        }
                    }
                }).ContinueWith((t) =>
                {
                    Durdur();

                    GeriBildirim_Tamamlandı?.Invoke(Sonuç, Url, HedefDosyaYolu_Veya_İçeriği);
                });
            }
            void Durdur()
            {
                if (İstemci == null) return;

                KontrolTamamlandı = true;

                İstemci.Dispose();
                İstemci = null;
            }

            #region IDisposable
            private bool disposedValue;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    Durdur();

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~Dosya_Ağda_()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }
        public class AğÜzerinde_Çoklu_ : IDisposable
        {
            public string Sürüm = "V1.0";
            public bool KontrolTamamlandı = false;
            public bool Sonuç = false;
            public AğÜzerinde_[] İstemciler = null;

            public AğÜzerinde_Çoklu_(List<Uri> Uri_ler, List<string> HedefDosyaYol_ları, Action<bool> GeriBildirim_TümüTamamlandı = null, int ZamanAşımı_msn = 15000)
            {
                long Sayac_Tamamlandı = 0;

                System.Threading.Tasks.Task.Run(async () =>
                {
                    await System.Threading.Tasks.Task.Delay(1); //eşzamanlı çalışma için

                    İstemciler = new AğÜzerinde_[Uri_ler.Count];
                    for (int i = 0; i < Uri_ler.Count; i++)
                    {
                        İstemciler[i] = new AğÜzerinde_(Uri_ler[i], HedefDosyaYol_ları[i], GeriBildirim_İşlemi, ZamanAşımı_msn);
                    }

                    int zaman_aşımı_anı = Environment.TickCount + ZamanAşımı_msn + 1000;
                    while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && Interlocked.Read(ref Sayac_Tamamlandı) != Uri_ler.Count && zaman_aşımı_anı > Environment.TickCount)
                    {
                        System.Threading.Tasks.Task.Delay(100).Wait();
                    }

                    void GeriBildirim_İşlemi(bool Onay, Uri uri, object Hedef)
                    {
                        Interlocked.Increment(ref Sayac_Tamamlandı);
                    }
                }).ContinueWith((t) =>
                {
                    if (Interlocked.Read(ref Sayac_Tamamlandı) == Uri_ler.Count)
                    {
                        Sonuç = true;
                        foreach (AğÜzerinde_ İstemci in İstemciler)
                        {
                            if (!İstemci.KontrolTamamlandı || !İstemci.Sonuç)
                            {
                                Sonuç = false;
                                break;
                            }
                        }
                    }

                    Durdur();

                    GeriBildirim_TümüTamamlandı?.Invoke(Sonuç);
                });
            }
            void Durdur()
            {
                if (İstemciler == null) return;

                KontrolTamamlandı = true;

                foreach (AğÜzerinde_ İstemci in İstemciler)
                {
                    İstemci.Dispose();
                }
                İstemciler = null;
            }

            #region IDisposable
            private bool disposedValue;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    Durdur();

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~Dosya_Ağda_()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            public void Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
            #endregion
        }
    }

    public class Klasör_
    {
        public const string Sürüm = "V1.3";

        public string Kök = "";
        public long KapladığıAlan_bayt;
        public bool FizikselOlarakMevcut;
        public List<string> Klasörler = new List<string>();
        public List<İçerik_Dosya_> Dosyalar;

        public bool TümAltKlasörlerleBirlikte;
        public int EşZamanlıİşlemSayısı;
        public bool BitmesiniBekle;
        public bool DoğrulamaKodunuÜret;
        public IEnumerable<string> Filtre_Klasör;
        public IEnumerable<string> Filtre_Dosya;
        public bool Filtre_BüyükKüçükHarfDuyarlı;
        public char Filtre_Ayraç;

        #region Tanımlar
        public class İçerik_Dosya_
        {
            public string Yolu;
            public long KapladığıAlan_bayt;
            public DateTime DeğiştirilmeTarihi;
            public string Doğrulama_Kodu;

            public İçerik_Dosya_(string Kök, string DosyaYolu, bool DoğrulamaKodunuÜret)
            {
                Yolu = DosyaYolu.Substring(Kök.Length + 1);

                if (File.Exists(DosyaYolu))
                {
                    FileInfo DosyaBilgisi = new FileInfo(DosyaYolu);
                    KapladığıAlan_bayt = DosyaBilgisi.Length;
                    DeğiştirilmeTarihi = DosyaBilgisi.LastWriteTime;
                    if (DoğrulamaKodunuÜret) Doğrulama_Kodu = DoğrulamaKodu.Üret.Dosyadan(DosyaYolu);
                }
                else
                {
                    KapladığıAlan_bayt = 0;
                    DeğiştirilmeTarihi = DateTime.MinValue;
                    Doğrulama_Kodu = null;
                }
            }
        }
        
        public enum Farklılık_Klasör { SadeceSolda = -1, Aynı, SadeceSağda };
        public class Fark_Klasör_
        {
            public string Yolu;
            public Farklılık_Klasör Farklılık;
        }
        public enum Farklılık_Dosya { SadeceSolda = -2, SoldakiDahaYeni, AynıTarihli, SağdakiDahaYeni, SadeceSağda };
        public class Fark_Dosya_
        {
            public string Yolu;
            public Farklılık_Dosya Farklılık;
            public bool Aynı_KapladığıAlan_bayt;
            public bool Aynı_Doğrulama_Kodu;
        }
        public class Farklılık_
        {
            public int FarklılıkSayısı = 0;
            public List<Fark_Klasör_> Klasörler = new List<Fark_Klasör_>();
            public List<Fark_Dosya_> Dosyalar = new List<Fark_Dosya_>();
        }
        #endregion

        #region Genel İşlemler
        public Klasör_(string KökKlasör,
            IEnumerable<string> Filtre_Klasör = null, IEnumerable<string> Filtre_Dosya = null, 
            bool TümAltKlasörlerleBirlikte = true, bool DoğrulamaKodunuÜret = true, 
            bool Filtre_BüyükKüçükHarfDuyarlı = true, char Filtre_Ayraç = '*', bool BitmesiniBekle = true, int EşZamanlıİşlemSayısı = Klasör.EşZamanlıİşlemSayısı_Sabiti)
        {
            this.TümAltKlasörlerleBirlikte = TümAltKlasörlerleBirlikte;
            this.EşZamanlıİşlemSayısı = EşZamanlıİşlemSayısı;
            this.BitmesiniBekle = BitmesiniBekle;
            this.DoğrulamaKodunuÜret = DoğrulamaKodunuÜret;

            this.Filtre_Klasör = Filtre_Klasör;
            this.Filtre_Dosya = Filtre_Dosya;
            this.Filtre_BüyükKüçükHarfDuyarlı = Filtre_BüyükKüçükHarfDuyarlı;
            this.Filtre_Ayraç = Filtre_Ayraç;

            Güncelle(KökKlasör);
        }
        public void Güncelle()
        {
            Güncelle(Kök);
        }
        public void Güncelle(string KökKlasör)
        {
            KapladığıAlan_bayt = 0;
            FizikselOlarakMevcut = false;
            Klasörler.Clear();
            Dosyalar = null;

            Kök = KökKlasör.DoluMu(true) ? D_DosyaKlasörAdı.Düzelt(KökKlasör) : KökKlasör;
            if (!Directory.Exists(Kök))
            {
                Dosyalar = new List<İçerik_Dosya_>();
                return;
            }
            FizikselOlarakMevcut = true;

            if (Filtre_Klasör == null) Filtre_Klasör = new string[] { "*" };
            if (Filtre_Dosya == null) Filtre_Dosya = new string[] { "*" };

            EşZamanlıÇokluErişim.Liste_<İçerik_Dosya_> dsy_l = new EşZamanlıÇokluErişim.Liste_<İçerik_Dosya_>();
            Action<object> TümüÖğütülünceÇağırılacakİşlem = null;
            if (!BitmesiniBekle) TümüÖğütülünceÇağırılacakİşlem = İşl_bitti;

            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<string> ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<string>(İşl_dsy, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, TümüÖğütülünceÇağırılacakİşlem: TümüÖğütülünceÇağırılacakİşlem);

            Klasörler = Directory.GetDirectories(Kök, "*", TümAltKlasörlerleBirlikte ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            Klasörler = Klasörler.FindAll(x => x.BenzerMi(Filtre_Klasör, Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç));

            //Kendi klasörü
            ö.Ekle(Kök);

            //Alt klasörleri
            for (int i = 0; i < Klasörler.Count; i++)
            {
                if (TümAltKlasörlerleBirlikte)
                {
                    ö.Ekle(Klasörler[i]);
                }
                
                Klasörler[i] = Klasörler[i].Substring(Kök.Length + 1).Trim(' ');
            }

            if (BitmesiniBekle)
            {
                while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && !ö.TümüÖğütüldüMü()) Thread.Sleep(5);
                Dosyalar = dsy_l.Durdur();
            }

            void İşl_dsy(string kls, object o)
            {
                string[] dosyalar = Directory.GetFiles(kls, "*.*", SearchOption.TopDirectoryOnly);
                foreach (string dsy in dosyalar)
                {
                    if (!dsy.BenzerMi(Filtre_Dosya, Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç)) continue;

                    İçerik_Dosya_ yeni = new İçerik_Dosya_(Kök, dsy, DoğrulamaKodunuÜret);
                    dsy_l.Add(yeni);
                    Interlocked.Add(ref KapladığıAlan_bayt, yeni.KapladığıAlan_bayt);
                }
            }
            void İşl_bitti(object o)
            {
                Dosyalar = dsy_l.Durdur();
            }
        }
        
        public Klasör_ Kopyala()
        {
            Klasör_ yeni = Listeleri_Kopyala(this);
            yeni.Kök = Kök;
            yeni.KapladığıAlan_bayt = KapladığıAlan_bayt;
            yeni.FizikselOlarakMevcut = FizikselOlarakMevcut;
            yeni.TümAltKlasörlerleBirlikte = TümAltKlasörlerleBirlikte;
            yeni.EşZamanlıİşlemSayısı = EşZamanlıİşlemSayısı;
            yeni.BitmesiniBekle = BitmesiniBekle;
            yeni.DoğrulamaKodunuÜret = DoğrulamaKodunuÜret;
            yeni.Filtre_Klasör = Filtre_Klasör;
            yeni.Filtre_Dosya = Filtre_Dosya;
            yeni.Filtre_BüyükKüçükHarfDuyarlı = Filtre_BüyükKüçükHarfDuyarlı;
            yeni.Filtre_Ayraç = Filtre_Ayraç;

            return yeni;
        }
        public static Klasör_ operator +(Klasör_ A, Klasör_ B)
        {
            A = A.Kopyala();

            foreach (string kls in B.Klasörler)
            {
                if (A.Klasörler.Contains(kls)) continue;

                A.Klasörler.Add(kls);
            }

            foreach (İçerik_Dosya_ dsy in B.Dosyalar)
            {
                if (A.Dosyalar.FirstOrDefault(x => x.Yolu == dsy.Yolu) != null) continue;

                A.Dosyalar.Add(dsy);
            }

            return A;
        }
        public static Klasör_ operator -(Klasör_ A, Klasör_ B)
        {
            A = A.Kopyala();

            foreach (string kls in B.Klasörler)
            {
                A.Klasörler.Remove(kls);
            }

            foreach (İçerik_Dosya_ dsy in B.Dosyalar)
            {
                İçerik_Dosya_ A_daki = A.Dosyalar.FirstOrDefault(x => x.Yolu == dsy.Yolu);
                if (A_daki == null) continue;

                A.Dosyalar.Remove(A_daki);
            }

            return A;
        }

        public Farklılık_ Karşılaştır(Klasör_ Sağdaki)
        {
            Klasör_ soldaki = Listeleri_Kopyala(this);
            Klasör_ sağdaki = Listeleri_Kopyala(Sağdaki);

            Farklılık_ Farklar = new Farklılık_();

            while (soldaki.Klasörler.Count > 0)
            {
                Fark_Klasör_ f_kls = new Fark_Klasör_();
                if (sağdaki.Klasörler.Contains(soldaki.Klasörler[0]))
                {
                    f_kls.Farklılık = Farklılık_Klasör.Aynı;
                    sağdaki.Klasörler.Remove(soldaki.Klasörler[0]);
                }
                else
                {
                    f_kls.Farklılık = Farklılık_Klasör.SadeceSolda;
                    Farklar.FarklılıkSayısı++;
                }

                f_kls.Yolu = soldaki.Klasörler[0];
                Farklar.Klasörler.Add(f_kls);

                soldaki.Klasörler.RemoveAt(0);
            }

            while (sağdaki.Klasörler.Count > 0)
            {
                Fark_Klasör_ f_kls = new Fark_Klasör_();
                if (soldaki.Klasörler.Contains(sağdaki.Klasörler[0]))
                {
                    f_kls.Farklılık = Farklılık_Klasör.Aynı;
                    soldaki.Klasörler.Remove(sağdaki.Klasörler[0]);
                }
                else
                {
                    f_kls.Farklılık = Farklılık_Klasör.SadeceSağda;
                    Farklar.FarklılıkSayısı++;
                }

                f_kls.Yolu = sağdaki.Klasörler[0];
                Farklar.Klasörler.Add(f_kls);
                
                sağdaki.Klasörler.RemoveAt(0);
            }

            while (soldaki.Dosyalar.Count > 0)
            {
                Fark_Dosya_ f_dsy = new Fark_Dosya_();
                İçerik_Dosya_ dsy = sağdaki.Dosyalar.Find(x => x.Yolu == soldaki.Dosyalar[0].Yolu);
                if (dsy != null && dsy.Yolu == soldaki.Dosyalar[0].Yolu)
                {
                    if (soldaki.Dosyalar[0].DeğiştirilmeTarihi < dsy.DeğiştirilmeTarihi) f_dsy.Farklılık = Farklılık_Dosya.SağdakiDahaYeni;
                    else if (soldaki.Dosyalar[0].DeğiştirilmeTarihi > dsy.DeğiştirilmeTarihi) f_dsy.Farklılık = Farklılık_Dosya.SoldakiDahaYeni;
                    else f_dsy.Farklılık = Farklılık_Dosya.AynıTarihli;

                    f_dsy.Aynı_Doğrulama_Kodu = soldaki.Dosyalar[0].Doğrulama_Kodu == dsy.Doğrulama_Kodu;
                    f_dsy.Aynı_KapladığıAlan_bayt = soldaki.Dosyalar[0].KapladığıAlan_bayt == dsy.KapladığıAlan_bayt;

                    sağdaki.Dosyalar.Remove(dsy);
                }
                else
                {
                    f_dsy.Farklılık = Farklılık_Dosya.SadeceSolda;
                }

                f_dsy.Yolu = soldaki.Dosyalar[0].Yolu;
                Farklar.Dosyalar.Add(f_dsy);

                if (f_dsy.Farklılık != Farklılık_Dosya.AynıTarihli || !f_dsy.Aynı_KapladığıAlan_bayt || !f_dsy.Aynı_Doğrulama_Kodu) Farklar.FarklılıkSayısı++;

                soldaki.Dosyalar.RemoveAt(0);  
            }

            while (sağdaki.Dosyalar.Count > 0)
            {
                Fark_Dosya_ f_dsy = new Fark_Dosya_();
                İçerik_Dosya_ dsy = soldaki.Dosyalar.Find(x => x.Yolu == sağdaki.Dosyalar[0].Yolu);
                if (dsy != null && dsy.Yolu == sağdaki.Dosyalar[0].Yolu)
                {
                    if (sağdaki.Dosyalar[0].DeğiştirilmeTarihi < dsy.DeğiştirilmeTarihi) f_dsy.Farklılık = Farklılık_Dosya.SoldakiDahaYeni;
                    else if (sağdaki.Dosyalar[0].DeğiştirilmeTarihi > dsy.DeğiştirilmeTarihi) f_dsy.Farklılık = Farklılık_Dosya.SağdakiDahaYeni;
                    else f_dsy.Farklılık = Farklılık_Dosya.AynıTarihli;

                    f_dsy.Aynı_Doğrulama_Kodu = sağdaki.Dosyalar[0].Doğrulama_Kodu == dsy.Doğrulama_Kodu;
                    f_dsy.Aynı_KapladığıAlan_bayt = sağdaki.Dosyalar[0].KapladığıAlan_bayt == dsy.KapladığıAlan_bayt;

                    soldaki.Dosyalar.Remove(dsy);
                }
                else
                {
                    f_dsy.Farklılık = Farklılık_Dosya.SadeceSağda;
                }

                f_dsy.Yolu = sağdaki.Dosyalar[0].Yolu;
                Farklar.Dosyalar.Add(f_dsy);

                if (f_dsy.Farklılık != Farklılık_Dosya.AynıTarihli || !f_dsy.Aynı_KapladığıAlan_bayt || !f_dsy.Aynı_Doğrulama_Kodu) Farklar.FarklılıkSayısı++;

                sağdaki.Dosyalar.RemoveAt(0);
            }

            return Farklar;
        }
        public Farklılık_ Eşitle(Klasör_ Sağdaki)
        {
            Farklılık_ Güncel = Karşılaştır(Sağdaki);
            Farklılık_ Sonuç = new Farklılık_();

            EşZamanlıÇokluErişim.Liste_<Fark_Klasör_> kls_l = new EşZamanlıÇokluErişim.Liste_<Fark_Klasör_>();
            EşZamanlıÇokluErişim.Liste_<Fark_Dosya_> dsy_l = new EşZamanlıÇokluErişim.Liste_<Fark_Dosya_>();
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Klasör_> kls_ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Klasör_>(İşl_Kls, null, EşZamanlıİşlemSayısı);
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Dosya_> dsy_ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Dosya_>(İşl_Dsy, null, EşZamanlıİşlemSayısı);

            foreach (Fark_Klasör_ kls in Güncel.Klasörler)
            {
                kls_ö.Ekle(kls);
            }

            foreach (Fark_Dosya_ dsy in Güncel.Dosyalar)
            {
                dsy_ö.Ekle(dsy);
            }

            while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && (!kls_ö.TümüÖğütüldüMü() || !dsy_ö.TümüÖğütüldüMü())) Thread.Sleep(5);
            Sonuç.Klasörler = kls_l.Durdur();
            Sonuç.Dosyalar = dsy_l.Durdur();

            return Sonuç;

            void İşl_Kls(Fark_Klasör_ kls, object o)
            {
                try
                {
                    switch (kls.Farklılık)
                    {
                        case Farklılık_Klasör.SadeceSolda:
                            Klasör.Oluştur(Sağdaki.Kök + @"\" + kls.Yolu);
                            break;

                        case Farklılık_Klasör.SadeceSağda:
                            Klasör.Oluştur(Kök + @"\" + kls.Yolu);
                            break;
                    }
                }
                catch (Exception)
                {
                    kls_l.Add(kls);
                }
            }
            void İşl_Dsy(Fark_Dosya_ dsy, object o)
            {
                try
                {
                    switch (dsy.Farklılık)
                    {
                        case Farklılık_Dosya.SadeceSolda:
                        case Farklılık_Dosya.SoldakiDahaYeni:
                            Dosya.Kopyala(Kök + @"\" + dsy.Yolu, Sağdaki.Kök + @"\" + dsy.Yolu);
                            break;

                        case Farklılık_Dosya.SadeceSağda:
                        case Farklılık_Dosya.SağdakiDahaYeni:
                            Dosya.Kopyala(Sağdaki.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu);
                            break;
                    }
                }
                catch (Exception)
                {
                    dsy_l.Add(dsy);
                }
            }
        }
        public Farklılık_ AslınaUygunHaleGetir(Klasör_ AsılKlasör, bool FazlaKlasörVeDosyalarıSil = false)
        {
            Farklılık_ Güncel = Karşılaştır(AsılKlasör);
            Farklılık_ Sonuç = new Farklılık_();

            EşZamanlıÇokluErişim.Liste_<Fark_Klasör_> kls_l = new EşZamanlıÇokluErişim.Liste_<Fark_Klasör_>();
            EşZamanlıÇokluErişim.Liste_<Fark_Dosya_> dsy_l = new EşZamanlıÇokluErişim.Liste_<Fark_Dosya_>();
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Klasör_> kls_ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Klasör_>(İşl_Kls, null, EşZamanlıİşlemSayısı);
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Dosya_> dsy_ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<Fark_Dosya_>(İşl_Dsy, null, EşZamanlıİşlemSayısı);

            foreach (Fark_Klasör_ kls in Güncel.Klasörler)
            {
                kls_ö.Ekle(kls);
            }

            foreach (Fark_Dosya_ dsy in Güncel.Dosyalar)
            {
                dsy_ö.Ekle(dsy);
            }

            while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && (!kls_ö.TümüÖğütüldüMü() || !dsy_ö.TümüÖğütüldüMü())) Thread.Sleep(5);
            Sonuç.Klasörler = kls_l.Durdur();
            Sonuç.Dosyalar = dsy_l.Durdur();

            return Sonuç;

            void İşl_Kls(Fark_Klasör_ kls, object o)
            {
                try
                {
                    switch (kls.Farklılık)
                    {
                        case Farklılık_Klasör.SadeceSolda:
                            if (FazlaKlasörVeDosyalarıSil)
                            {
                                Klasör.Sil(Kök + @"\" + kls.Yolu);
                            }
                            break;

                        case Farklılık_Klasör.SadeceSağda:
                            Klasör.Oluştur(Kök + @"\" + kls.Yolu);
                            break;
                    }
                }
                catch (Exception)
                {
                    kls_l.Add(kls);
                }
            }
            void İşl_Dsy(Fark_Dosya_ dsy, object o)
            {
                try
                {
                    switch (dsy.Farklılık)
                    {
                        case Farklılık_Dosya.SadeceSolda:
                            if (FazlaKlasörVeDosyalarıSil)
                            {
                                Dosya.Sil(Kök + @"\" + dsy.Yolu);
                            }
                            break;

                        case Farklılık_Dosya.SadeceSağda:
                        case Farklılık_Dosya.SağdakiDahaYeni:
                        case Farklılık_Dosya.SoldakiDahaYeni:
                            Dosya.Kopyala(AsılKlasör.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu);
                            break;

                        case Farklılık_Dosya.AynıTarihli:
                            if (!dsy.Aynı_Doğrulama_Kodu)
                            {
                                Dosya.Kopyala(AsılKlasör.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu);
                            }
                            break;
                    }
                }
                catch (Exception)
                {
                    dsy_l.Add(dsy);
                }
            }
        }

        public bool Dosya_Sil_TarihineGöre(double Gün)
        {
            int HataOldu = 0;
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_> ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

            if (FizikselOlarakMevcut)
            {
                Sırala_EskidenYeniye();
                DateTime SıfırNoktası = DateTime.Now.AddDays(Gün * -1);

                foreach (var dsy in Dosyalar)
                {
                    if (dsy.DeğiştirilmeTarihi >= SıfırNoktası) break;

                    ö.Ekle(dsy);
                }

                while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && !ö.TümüÖğütüldüMü()) Thread.Sleep(5);
            }

            return HataOldu == 0;

            void İşl(İçerik_Dosya_ dsy, object o)
            {
                if (!Temkinli.Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        public bool Dosya_Sil_BoyutunaGöre(long TümDosyaların_KapladığıAlan_bayt)
        {
            int HataOldu = 0;
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_> ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

            if (FizikselOlarakMevcut)
            {
                Sırala_EskidenYeniye();

                foreach (var dsy in Dosyalar)
                {
                    if (KapladığıAlan_bayt <= TümDosyaların_KapladığıAlan_bayt) break;

                    ö.Ekle(dsy);
                    KapladığıAlan_bayt -= dsy.KapladığıAlan_bayt;
                }

                while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && !ö.TümüÖğütüldüMü()) Thread.Sleep(5);
            }

            return HataOldu == 0;

            void İşl(İçerik_Dosya_ dsy, object o)
            {
                if (!Temkinli.Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        public bool Dosya_Sil_SayısınaGöre(int AzamiToplamDosyaSayısı)
        {
            int HataOldu = 0;
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_> ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

            if (FizikselOlarakMevcut)
            {
                Sırala_EskidenYeniye();

                AzamiToplamDosyaSayısı = Dosyalar.Count - AzamiToplamDosyaSayısı;
                foreach (var dsy in Dosyalar)
                {
                    if (AzamiToplamDosyaSayısı-- <= 0) break;

                    ö.Ekle(dsy);
                }

                while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && !ö.TümüÖğütüldüMü()) Thread.Sleep(5);
            }

            return HataOldu == 0;

            void İşl(İçerik_Dosya_ dsy, object o)
            {
                if (!Temkinli.Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        public bool Dosya_Sil_SayısınaVeBoyutunaGöre(int AzamiToplamDosyaSayısı, int TümDosyaların_KapladığıAlan_bayt)
        {
            // En yeni AzamiToplamDosyaSayısı kadar dosya tutulur
            // Haricindekiler TümDosyaların_KapladığıAlan_bayt aşıyorsa eskiden yeniye doğru silinir

            if (!FizikselOlarakMevcut ||
                Dosyalar.Count <= AzamiToplamDosyaSayısı) return true;

            Sırala_EskidenYeniye();

            //AzamiToplamDosyaSayısı kadar yeni dosyayı görünmez yap, sildirtme
            for (int i = 0; i < AzamiToplamDosyaSayısı; i++)
            {
                KapladığıAlan_bayt -= Dosyalar.Last().KapladığıAlan_bayt;
                Dosyalar.Remove(Dosyalar.Last());
            }

            int HataOldu = 0;
            ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_> ö = new ArgeMup.HazirKod.ArkaPlan.Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

            foreach (var dsy in Dosyalar)
            {
                if (KapladığıAlan_bayt <= TümDosyaların_KapladığıAlan_bayt) break;

                ö.Ekle(dsy);

                KapladığıAlan_bayt -= dsy.KapladığıAlan_bayt;
            }

            while (ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın && !ö.TümüÖğütüldüMü()) Thread.Sleep(5);
            
            return HataOldu == 0;

            void İşl(İçerik_Dosya_ dsy, object o)
            {
                if (!Temkinli.Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        #endregion

        Klasör_ Listeleri_Kopyala(Klasör_ Girdi)
        {
            Klasör_ yeni = new Klasör_("");
            yeni.Dosyalar = new List<İçerik_Dosya_>(Girdi.Dosyalar);
            yeni.Klasörler = new List<string>(Girdi.Klasörler);
            return yeni;
        }

        #region Sıralama
        class _Sıralayıcı_EskidenYeniye : IComparer<İçerik_Dosya_>
        {
            public int Compare(İçerik_Dosya_ x, İçerik_Dosya_ y)
            {
                if (x.DeğiştirilmeTarihi > y.DeğiştirilmeTarihi) return 1;  
                else if (x.DeğiştirilmeTarihi == y.DeğiştirilmeTarihi) return 0;
                else return -1;   
            }
        }
        class _Sıralayıcı_DosyaBoyutuKüçüktenBüyüğe : IComparer<İçerik_Dosya_>
        {
            public int Compare(İçerik_Dosya_ x, İçerik_Dosya_ y)
            {
                if (x.KapladığıAlan_bayt > y.KapladığıAlan_bayt) return 1;  
                else if (x.KapladığıAlan_bayt == y.KapladığıAlan_bayt) return 0;
                else return -1;   
            }
        }

        public void Sırala_EskidenYeniye()
        {
            _Sıralayıcı_EskidenYeniye srl = new _Sıralayıcı_EskidenYeniye();
            Dosyalar.Sort(srl);
        }
        public void Sırala_DosyaBoyutuKüçüktenBüyüğe()
        {
            _Sıralayıcı_DosyaBoyutuKüçüktenBüyüğe srl = new _Sıralayıcı_DosyaBoyutuKüçüktenBüyüğe();
            Dosyalar.Sort(srl);
        }
        #endregion
    }
}