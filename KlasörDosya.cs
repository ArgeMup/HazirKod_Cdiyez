// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;
using ArgeMup.HazirKod.ArkaPlan;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public class Klasör
    {
        public const string Sürüm = "V1.2";

        public static bool Oluştur(string Yolu, bool Düzelt = true)
        {
            try
            {
                if (Directory.Exists(Yolu)) return true;

                if (Düzelt) Yolu = D_DosyaKlasörAdı.Düzelt(Yolu, false);
                Directory.CreateDirectory(Yolu);

                if (Directory.Exists(Yolu)) return true;
            }
            catch (Exception) { }

            return false;
        }
        public static bool Sil(string Yolu)
        {
            try
            {
                if (!Directory.Exists(Yolu)) return true;

                Directory.Delete(Yolu, true);
                
                if (!Directory.Exists(Yolu)) return true;
            }
            catch (Exception) { }

            return false;
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
        public static bool Kopyala(string Kaynak, string Hedef, int EşZamanlıİşlemSayısı = 5)
        {
            return AslınaUygunHaleGetir(Kaynak, Hedef, false, EşZamanlıİşlemSayısı);
        }
        public static bool AynıMı(string Sol, string Sağ, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ Soldaki = new Klasör_(Sol, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle:false);
            Klasör_ Sağdaki = new Klasör_(Sağ, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle:false);
            while (Soldaki.Dosyalar == null || Sağdaki.Dosyalar == null) Thread.Sleep(5);

            Klasör_.Farklılık_ Farklar = Soldaki.Karşılaştır(Sağdaki);

            return Soldaki.FizikselOlarakMevcut && Sağdaki.FizikselOlarakMevcut && Farklar.FarklılıkSayısı == 0;
        }
        public static bool Eşitle(string Sol, string Sağ, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ Soldaki = new Klasör_(Sol, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false);
            Klasör_ Sağdaki = new Klasör_(Sağ, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false);
            while (Soldaki.Dosyalar == null || Sağdaki.Dosyalar == null) Thread.Sleep(5);

            Klasör_.Farklılık_ Farklar = Soldaki.Eşitle(Sağdaki, EşZamanlıİşlemSayısı);

            return (Soldaki.FizikselOlarakMevcut || Sağdaki.FizikselOlarakMevcut) && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
        }
        public static bool AslınaUygunHaleGetir(string Asıl, string Kopya, bool FazlaKlasörVeDosyalarıSil = false, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ Aslolan = new Klasör_(Asıl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false);
            Klasör_ Kopyası = new Klasör_(Kopya, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, BitmesiniBekle: false);
            while (Aslolan.Dosyalar == null || Kopyası.Dosyalar == null) Thread.Sleep(5);

            Klasör_.Farklılık_ Farklar = Kopyası.AslınaUygunHaleGetir(Aslolan, FazlaKlasörVeDosyalarıSil, EşZamanlıİşlemSayısı);

            return Aslolan.FizikselOlarakMevcut && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
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

        public static string ÜstKlasör(string Yolu, int Seviye = 1, bool KökeUlaşıncaDur = false)
        {
            Yolu = D_DosyaKlasörAdı.Düzelt(Yolu);

            string kök = Path.GetPathRoot(Yolu);
            if (!string.IsNullOrEmpty(kök)) kök = kök.TrimEnd(Path.DirectorySeparatorChar);
            if (string.IsNullOrEmpty(kök))
            {
                int konum_bölüm = Yolu.TrimStart(Path.DirectorySeparatorChar).IndexOf(Path.DirectorySeparatorChar);
                if (konum_bölüm < 0) return null;
                
                kök = Yolu.Substring(0, konum_bölüm);
            }
            Yolu = Yolu.Substring(kök.Length);

            while (Seviye-- > 0)
            {
                int konum_bölüm = Yolu.LastIndexOf(Path.DirectorySeparatorChar);
                if (konum_bölüm < 0)
                {
                    if (KökeUlaşıncaDur) return kök;

                    if (Seviye >= 0) return null;
                    else return kök;
                }

                Yolu = Yolu.Substring(0, konum_bölüm);
            }

            return kök + Yolu;
        }
    }

    public class Dosya
    {
        public const string Sürüm = "V1.2";

        public static bool Kopyala(string Kaynak, string Hedef)
        {
            string yedek_dosya_adı = Hedef + ".yedek";

            try
            {
                if (File.Exists(Hedef))
                {
                    if (File.Exists(yedek_dosya_adı)) File.Delete(yedek_dosya_adı);

                    File.Move(Hedef, yedek_dosya_adı);
                }
                else Klasör.Oluştur(Path.GetDirectoryName(Hedef));

                File.Copy(Kaynak, Hedef);
                File.SetAttributes(Hedef, File.GetAttributes(Kaynak));
                File.SetLastWriteTime(Hedef, File.GetLastWriteTime(Kaynak));

                if (File.Exists(yedek_dosya_adı)) File.Delete(yedek_dosya_adı);

                return File.Exists(Hedef);
            }
            catch (Exception) { }

            if (File.Exists(yedek_dosya_adı))
            {
                if (File.Exists(Hedef)) File.Delete(Hedef);

                File.Move(yedek_dosya_adı, Hedef);
            }

            return false;
        }
        public static bool Sil(string Yolu)
        {
            try
            {
                if (!File.Exists(Yolu)) return true;

                File.Delete(Yolu);

                if (!File.Exists(Yolu)) return true;
            }
            catch (Exception) { }

            return false;
        }
        public static bool Sil_TarihineGöre(string Klasörü, double Gün, string Filtre, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true, EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_TarihineGöre(Gün, EşZamanlıİşlemSayısı);
        }
        public static bool Sil_BoyutunaGöre(string Klasörü, long TümDosyaların_KapladığıAlan_bayt, string Filtre, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true, EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_BoyutunaGöre(TümDosyaların_KapladığıAlan_bayt, EşZamanlıİşlemSayısı);
        }
        public static bool Sil_SayısınaGöre(string Klasörü, int AzamiToplamDosyaSayısı, string Filtre, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true, EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_SayısınaGöre(AzamiToplamDosyaSayısı, EşZamanlıİşlemSayısı);
		}
        public static bool Sil_SayısınaVeBoyutunaGöre(string Klasörü, int AzamiToplamDosyaSayısı, int TümDosyaların_KapladığıAlan_bayt, string Filtre, int EşZamanlıİşlemSayısı = 5)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true, EşZamanlıİşlemSayısı);
            return kls.Dosya_Sil_SayısınaVeBoyutunaGöre(AzamiToplamDosyaSayısı, TümDosyaların_KapladığıAlan_bayt, EşZamanlıİşlemSayısı);
        }
    }

    public class Klasör_
    {
        public const string Sürüm = "V1.2";

        public string Kök = "";
        public long KapladığıAlan_bayt = 0;
        public bool FizikselOlarakMevcut = false;
        public List<string> Klasörler = new List<string>();
        public List<İçerik_Dosya_> Dosyalar = null;

        #region Tanımlar
        public class İçerik_Dosya_
        {
            public string Yolu;
            public long KapladığıAlan_bayt;
            public DateTime DeğiştirilmeTarihi;
            public string Doğrulama_Kodu;

            public İçerik_Dosya_(string Kök, string DosyaYolu)
            {
                Yolu = D_DosyaKlasörAdı.Düzelt(DosyaYolu.Substring(Kök.Length + 1), false);

                if (File.Exists(DosyaYolu))
                {
                    FileInfo DosyaBilgisi = new FileInfo(DosyaYolu);
                    KapladığıAlan_bayt = DosyaBilgisi.Length;
                    DeğiştirilmeTarihi = DosyaBilgisi.LastWriteTime;
                    Doğrulama_Kodu = DoğrulamaKodu.Üret.Dosyadan(DosyaYolu);
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
        public Klasör_(string KökKlasör, string Filtre_Klasör = "*", string Filtre_Dosya = "*.*", bool TümAltKlasörlerleBirlikte = true, int EşZamanlıİşlemSayısı = 5, bool BitmesiniBekle = true)
        {
            Güncelle(KökKlasör, Filtre_Klasör, Filtre_Dosya, TümAltKlasörlerleBirlikte, EşZamanlıİşlemSayısı, BitmesiniBekle);
        }
        public void Güncelle(string KökKlasör, string Filtre_Klasör = "*", string Filtre_Dosya = "*.*", bool TümAltKlasörlerleBirlikte = true, int EşZamanlıİşlemSayısı = 5, bool BitmesiniBekle = true)
        {
            KapladığıAlan_bayt = 0;
            FizikselOlarakMevcut = false;
            Klasörler.Clear();
            Dosyalar = null;

            Kök = KökKlasör.TrimEnd('\\');
            if (!Directory.Exists(Kök))
            {
                Dosyalar = new List<İçerik_Dosya_>();
                return;
            }
            FizikselOlarakMevcut = true;

            EşZamanlıÇokluErişim.Liste_<İçerik_Dosya_> dsy_l = new EşZamanlıÇokluErişim.Liste_<İçerik_Dosya_>();
            Action<object> TümüÖğütülünceÇağırılacakİşlem = null;
            if (!BitmesiniBekle) TümüÖğütülünceÇağırılacakİşlem = İşl_bitti;
            
            Öğütücü_<string> ö = new Öğütücü_<string>(İşl_dsy, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı, TümüÖğütülünceÇağırılacakİşlem: TümüÖğütülünceÇağırılacakİşlem);

            Klasörler = Directory.GetDirectories(Kök, Filtre_Klasör, TümAltKlasörlerleBirlikte ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            
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
                string[] dosyalar = Directory.GetFiles(kls, Filtre_Dosya, SearchOption.TopDirectoryOnly);
                foreach (string dsy in dosyalar)
                {
                    İçerik_Dosya_ yeni = new İçerik_Dosya_(Kök, dsy);
                    dsy_l.Add(yeni);
                    Interlocked.Add(ref KapladığıAlan_bayt, yeni.KapladığıAlan_bayt);
                }
            }
            void İşl_bitti(object o)
            {
                Dosyalar = dsy_l.Durdur();
            }
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
        public Farklılık_ Eşitle(Klasör_ Sağdaki, int EşZamanlıİşlemSayısı = 5)
        {
            Farklılık_ Güncel = Karşılaştır(Sağdaki);
            Farklılık_ Sonuç = new Farklılık_();

            EşZamanlıÇokluErişim.Liste_<Fark_Klasör_> kls_l = new EşZamanlıÇokluErişim.Liste_<Fark_Klasör_>();
            EşZamanlıÇokluErişim.Liste_<Fark_Dosya_> dsy_l = new EşZamanlıÇokluErişim.Liste_<Fark_Dosya_>();
            Öğütücü_<Fark_Klasör_> kls_ö = new Öğütücü_<Fark_Klasör_>(İşl_Kls, null, EşZamanlıİşlemSayısı);
            Öğütücü_<Fark_Dosya_> dsy_ö = new Öğütücü_<Fark_Dosya_>(İşl_Dsy, null, EşZamanlıİşlemSayısı);

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
                switch (kls.Farklılık)
                {
                    case Farklılık_Klasör.SadeceSolda:
                        if (!Klasör.Oluştur(Sağdaki.Kök + @"\" + kls.Yolu)) kls_l.Add(kls);
                        break;

                    case Farklılık_Klasör.SadeceSağda:
                        if (!Klasör.Oluştur(Kök + @"\" + kls.Yolu)) kls_l.Add(kls);
                        break;
                }
            }
            void İşl_Dsy(Fark_Dosya_ dsy, object o)
            {
                switch (dsy.Farklılık)
                {
                    case Farklılık_Dosya.SadeceSolda:
                    case Farklılık_Dosya.SoldakiDahaYeni:
                        if (!Dosya.Kopyala(Kök + @"\" + dsy.Yolu, Sağdaki.Kök + @"\" + dsy.Yolu)) dsy_l.Add(dsy);
                        break;

                    case Farklılık_Dosya.SadeceSağda:
                    case Farklılık_Dosya.SağdakiDahaYeni:
                        if (!Dosya.Kopyala(Sağdaki.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu)) dsy_l.Add(dsy);
                        break;
                }
            }
        }
        public Farklılık_ AslınaUygunHaleGetir(Klasör_ AsılKlasör, bool FazlaKlasörVeDosyalarıSil = false, int EşZamanlıİşlemSayısı = 5)
        {
            Farklılık_ Güncel = Karşılaştır(AsılKlasör);
            Farklılık_ Sonuç = new Farklılık_();

            EşZamanlıÇokluErişim.Liste_<Fark_Klasör_> kls_l = new EşZamanlıÇokluErişim.Liste_<Fark_Klasör_>();
            EşZamanlıÇokluErişim.Liste_<Fark_Dosya_> dsy_l = new EşZamanlıÇokluErişim.Liste_<Fark_Dosya_>();
            Öğütücü_<Fark_Klasör_> kls_ö = new Öğütücü_<Fark_Klasör_>(İşl_Kls, null, EşZamanlıİşlemSayısı);
            Öğütücü_<Fark_Dosya_> dsy_ö = new Öğütücü_<Fark_Dosya_>(İşl_Dsy, null, EşZamanlıİşlemSayısı);

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
                switch (kls.Farklılık)
                {
                    case Farklılık_Klasör.SadeceSolda:
                        if (FazlaKlasörVeDosyalarıSil)
                        {
                            if (!Klasör.Sil(Kök + @"\" + kls.Yolu)) kls_l.Add(kls);
                        }
                        break;

                    case Farklılık_Klasör.SadeceSağda:
                        if (!Klasör.Oluştur(Kök + @"\" + kls.Yolu)) kls_l.Add(kls);
                        break;
                }
            }
            void İşl_Dsy(Fark_Dosya_ dsy, object o)
            {
                switch (dsy.Farklılık)
                {
                    case Farklılık_Dosya.SadeceSolda:
                        if (FazlaKlasörVeDosyalarıSil)
                        {
                            if (!Dosya.Sil(Kök + @"\" + dsy.Yolu)) dsy_l.Add(dsy);
                        }
                        break;

                    case Farklılık_Dosya.SadeceSağda:
                    case Farklılık_Dosya.SağdakiDahaYeni:
                    case Farklılık_Dosya.SoldakiDahaYeni:
                        if (!Dosya.Kopyala(AsılKlasör.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu)) dsy_l.Add(dsy);
                        break;

                    case Farklılık_Dosya.AynıTarihli:
                        if (!dsy.Aynı_Doğrulama_Kodu)
                        {
                            if (!Dosya.Kopyala(AsılKlasör.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu)) dsy_l.Add(dsy);
                        }
                        break;
                }
            }
        }

        public bool Dosya_Sil_TarihineGöre(double Gün, int EşZamanlıİşlemSayısı = 5)
        {
            int HataOldu = 0;
            Öğütücü_<İçerik_Dosya_> ö = new Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

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
                if (!Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        public bool Dosya_Sil_BoyutunaGöre(long TümDosyaların_KapladığıAlan_bayt, int EşZamanlıİşlemSayısı = 5)
        {
            int HataOldu = 0;
            Öğütücü_<İçerik_Dosya_> ö = new Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

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
                if (!Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        public bool Dosya_Sil_SayısınaGöre(int AzamiToplamDosyaSayısı, int EşZamanlıİşlemSayısı = 5)
        {
            int HataOldu = 0;
            Öğütücü_<İçerik_Dosya_> ö = new Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

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
                if (!Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        public bool Dosya_Sil_SayısınaVeBoyutunaGöre(int AzamiToplamDosyaSayısı, int TümDosyaların_KapladığıAlan_bayt, int EşZamanlıİşlemSayısı = 5)
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
            Öğütücü_<İçerik_Dosya_> ö = new Öğütücü_<İçerik_Dosya_>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);

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
                if (!Dosya.Sil(Kök + @"\" + dsy.Yolu)) Interlocked.Increment(ref HataOldu);
            }
        }
        #endregion

        Klasör_ Listeleri_Kopyala(Klasör_ Girdi)
        {
            Klasör_ yeni = new Klasör_(":");
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