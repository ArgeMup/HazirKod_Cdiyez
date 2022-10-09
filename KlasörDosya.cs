// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;

namespace ArgeMup.HazirKod
{
    public class Klasör
    {
        public const string Sürüm = "V1.0";

        public static bool Oluştur(string Yolu)
        {
            try
            {
                if (Directory.Exists(Yolu)) return true;

                Yolu = D_DosyaKlasörAdı.Düzelt(Yolu, false);
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
        public static bool Kopyala(string Kaynak, string Hedef)
        {
            return AslınaUygunHaleGetir(Kaynak, Hedef, false);
        }
        public static bool AynıMı(string Sol, string Sağ)
        {
            Klasör_ Soldaki = new Klasör_(Sol);
            Klasör_ Sağdaki = new Klasör_(Sağ);
            Klasör_.Farklılık_ Farklar = Soldaki.Karşılaştır(Sağdaki);

            return Soldaki.FizikselOlarakMevcut && Sağdaki.FizikselOlarakMevcut && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
        }
        public static bool Eşitle(string Sol, string Sağ)
        {
            Klasör_ Soldaki = new Klasör_(Sol);
            Klasör_ Sağdaki = new Klasör_(Sağ);
            Klasör_.Farklılık_ Farklar = Soldaki.Eşitle(Sağdaki);

            return (Soldaki.FizikselOlarakMevcut || Sağdaki.FizikselOlarakMevcut) && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
        }
        public static bool AslınaUygunHaleGetir(string Asıl, string Kopya, bool FazlaKlasörVeDosyalarıSil = false)
        {
            Klasör_ Aslolan = new Klasör_(Asıl);
            Klasör_ Kopyası = new Klasör_(Kopya);
            Klasör_.Farklılık_ Farklar = Kopyası.AslınaUygunHaleGetir(Aslolan, FazlaKlasörVeDosyalarıSil);

            return Aslolan.FizikselOlarakMevcut && (Farklar.Klasörler.Count == 0) && (Farklar.Dosyalar.Count == 0);
        }

		public enum Kapsamı { Geçici, TümKullanıcılar, BuKullanıcı_BuİşletimSistemi, BuKullanıcı_TümİşletimSistemleri };     
        /// <summary>
        /// Geçici                              : C:\Users\<Kullanıcı Adı>\AppData\Local\Temp\<Aile>\<Uygulama>\<Sürüm>
        /// TümKullanıcılar                     : C:\ProgramData\<Aile>\<Uygulama>\<Sürüm>
        /// BuKullanıcı_BuİşletimSistemi        : C:\Users\<Kullanıcı Adı>\AppData\Local\<Aile>\<Uygulama>\<Sürüm>
        /// BuKullanıcı_TümİşletimSistemleri    : C:\Users\<Kullanıcı Adı>\AppData\Roaming\<Aile>\<Uygulama>\<Sürüm>
        /// </summary>
        public static string Depolama(Kapsamı HedeflenenKapsamı = Kapsamı.BuKullanıcı_BuİşletimSistemi, string Aile = null, string Uygulama = null, string Sürüm = null)
        {
            Environment.SpecialFolder Tür = 0;
            if (Aile == null) Aile = "ArGeMuP";
            if (Uygulama == null) Uygulama = Kendi.Adı();
            if (Sürüm == null) Sürüm = Kendi.Sürümü_Dosya();

            string kls = null;
            if (HedeflenenKapsamı == Kapsamı.Geçici) kls = Path.GetTempPath().TrimEnd('\\');
            else
            {
                switch (HedeflenenKapsamı)
                {
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

            string kök = Path.GetPathRoot(Yolu).TrimEnd(Path.DirectorySeparatorChar);
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
        public const string Sürüm = "V1.1";

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
        public static bool Sil_TarihineGöre(string Klasörü, double Gün, string Filtre)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true);
            bool EnAz1HataOldu = false;
            
            if (kls.FizikselOlarakMevcut)
            {
                foreach (var dsy in kls.Dosyalar)
                {
                    if (dsy.DeğiştirilmeTarihi.AddDays(Gün) < DateTime.Now) 
                    {
                        if (!Dosya.Sil(kls.Kök + @"\" + dsy.Yolu)) EnAz1HataOldu = true;
                    }
                }
            }

            return EnAz1HataOldu;
        }
        public static bool Sil_BoyutunaGöre(string Klasörü, long TümDosyaların_KapladığıAlan_bayt, string Filtre)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true);
            bool EnAz1HataOldu = false;

            if (kls.FizikselOlarakMevcut)
            {
                kls.Sırala_EskidenYeniye();

                foreach (var dsy in kls.Dosyalar)
                {
                    if (kls.KapladığıAlan_bayt <= TümDosyaların_KapladığıAlan_bayt) break;
                    
                    if (!Dosya.Sil(kls.Kök + @"\" + dsy.Yolu)) EnAz1HataOldu = true;
                    
                    kls.KapladığıAlan_bayt -= dsy.KapladığıAlan_bayt;
                }
            }

            return EnAz1HataOldu;
        }
        public static bool Sil_SayısınaGöre(string Klasörü, int AzamiToplamDosyaSayısı, string Filtre)
        {
            Klasör_ kls = new Klasör_(Klasörü, "*", Filtre, true);
            bool EnAz1HataOldu = false;
            
            if (kls.FizikselOlarakMevcut)
            {
                kls.Sırala_EskidenYeniye();

                AzamiToplamDosyaSayısı = kls.Dosyalar.Count - AzamiToplamDosyaSayısı;
                foreach (var dsy in kls.Dosyalar)
                {
                    if (AzamiToplamDosyaSayısı-- <= 0) break;
                    
                    if (!Dosya.Sil(kls.Kök + @"\" + dsy.Yolu)) EnAz1HataOldu = true;
                }
            }

            return EnAz1HataOldu;
        }
    }

    [Serializable]
    public class Klasör_
    {
        [NonSerialized]
        public const string Sürüm = "V1.1";

        public string Kök = "";
        public long KapladığıAlan_bayt = 0;
        public bool FizikselOlarakMevcut = false;
        public List<string> Klasörler = new List<string>();
        public List<İçerik_Dosya_> Dosyalar = new List<İçerik_Dosya_>();

        #region Tanımlar
        [Serializable]
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
            public List<Fark_Klasör_> Klasörler = new List<Fark_Klasör_>();
            public List<Fark_Dosya_> Dosyalar = new List<Fark_Dosya_>();
        }
        #endregion

        #region Genel İşlemler
        public Klasör_(string KökKlasör, string Filtre_Klasör = "*", string Filtre_Dosya = "*.*", bool TümAltKlasörlerleBirlikte = true)
        {
            Güncelle(KökKlasör, Filtre_Klasör, Filtre_Dosya, TümAltKlasörlerleBirlikte);
        }
        public void Güncelle(string KökKlasör, string Filtre_Klasör = "*", string Filtre_Dosya = "*.*", bool TümAltKlasörlerleBirlikte = true)
        {
            KapladığıAlan_bayt = 0;
            FizikselOlarakMevcut = false;
            Klasörler.Clear();
            Dosyalar.Clear();

            Kök = KökKlasör.TrimEnd('\\');
            if (!Directory.Exists(Kök)) return;
            FizikselOlarakMevcut = true;

            //Kendi klasörü
            string[] dosyalar = Directory.GetFiles(Kök, Filtre_Dosya, SearchOption.TopDirectoryOnly);
            foreach (string dsy in dosyalar)
            {
                İçerik_Dosya_ yeni = new İçerik_Dosya_(Kök, dsy);
                Dosyalar.Add(yeni);
                KapladığıAlan_bayt += yeni.KapladığıAlan_bayt;
            }

            //Alt klasörleri
            Klasörler = Directory.GetDirectories(Kök, Filtre_Klasör, TümAltKlasörlerleBirlikte ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly).ToList();
            for (int i = 0; i < Klasörler.Count; i++)
            {
                if (TümAltKlasörlerleBirlikte)
                {
                    dosyalar = Directory.GetFiles(Klasörler[i], Filtre_Dosya, SearchOption.TopDirectoryOnly);
                    foreach (string dsy in dosyalar)
                    {
                        İçerik_Dosya_ yeni = new İçerik_Dosya_(Kök, dsy);
                        Dosyalar.Add(yeni);
                        KapladığıAlan_bayt += yeni.KapladığıAlan_bayt;
                    }
                }
                
                Klasörler[i] = Klasörler[i].Substring(Kök.Length + 1).Trim(' ');
            }
        }
       
        public Farklılık_ Karşılaştır(Klasör_ Sağdaki)
        {
            Klasör_ soldaki = (Klasör_)D_Nesne.BaytDizisinden(D_Nesne.BaytDizisine(this));
            Klasör_ sağdaki = (Klasör_)D_Nesne.BaytDizisinden(D_Nesne.BaytDizisine(Sağdaki));

            Farklılık_ Farklar = new Farklılık_();

            while (soldaki.Klasörler.Count > 0)
            {
                Fark_Klasör_ f_kls = new Fark_Klasör_();
                if (sağdaki.MevcutMu_Klasör(soldaki.Klasörler[0]))
                {
                    f_kls.Farklılık = Farklılık_Klasör.Aynı;
                    sağdaki.Klasörler.Remove(soldaki.Klasörler[0]);
                }
                else
                {
                    f_kls.Farklılık = Farklılık_Klasör.SadeceSolda;
                }

                f_kls.Yolu = soldaki.Klasörler[0];
                Farklar.Klasörler.Add(f_kls);

                soldaki.Klasörler.RemoveAt(0);
            }

            while (sağdaki.Klasörler.Count > 0)
            {
                Fark_Klasör_ f_kls = new Fark_Klasör_();
                if (soldaki.MevcutMu_Klasör(sağdaki.Klasörler[0]))
                {
                    f_kls.Farklılık = Farklılık_Klasör.Aynı;
                    soldaki.Klasörler.Remove(sağdaki.Klasörler[0]);
                }
                else
                {
                    f_kls.Farklılık = Farklılık_Klasör.SadeceSağda;
                }

                f_kls.Yolu = sağdaki.Klasörler[0];
                Farklar.Klasörler.Add(f_kls);
                
                sağdaki.Klasörler.RemoveAt(0);
            }

            while (soldaki.Dosyalar.Count > 0)
            {
                Fark_Dosya_ f_dsy = new Fark_Dosya_();
                İçerik_Dosya_ dsy = sağdaki.MevcutMu_Dosya(soldaki.Dosyalar[0].Yolu);
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
               
                soldaki.Dosyalar.RemoveAt(0);
            }

            while (sağdaki.Dosyalar.Count > 0)
            {
                Fark_Dosya_ f_dsy = new Fark_Dosya_();
                İçerik_Dosya_ dsy = soldaki.MevcutMu_Dosya(sağdaki.Dosyalar[0].Yolu);
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
                
                sağdaki.Dosyalar.RemoveAt(0);
            }

            return Farklar;
        }
        public Farklılık_ Eşitle(Klasör_ Sağdaki)
        {
            Farklılık_ Güncel = Karşılaştır(Sağdaki);
            Farklılık_ Sonuç = new Farklılık_();

            foreach (Fark_Klasör_ kls in Güncel.Klasörler)
            {
                switch (kls.Farklılık)
                {
                    case Farklılık_Klasör.SadeceSolda:
                        if (!Klasör.Oluştur(Sağdaki.Kök + @"\" + kls.Yolu)) Sonuç.Klasörler.Add(kls);
                        break;

                    case Farklılık_Klasör.SadeceSağda:
                        if (!Klasör.Oluştur(Kök + @"\" + kls.Yolu)) Sonuç.Klasörler.Add(kls);
                        break;
                }
            }

            foreach (Fark_Dosya_ dsy in Güncel.Dosyalar)
            {
                switch (dsy.Farklılık)
                {
                    case Farklılık_Dosya.SadeceSolda:
                    case Farklılık_Dosya.SoldakiDahaYeni:
                        if (!Dosya.Kopyala(Kök + @"\" + dsy.Yolu, Sağdaki.Kök + @"\" + dsy.Yolu)) Sonuç.Dosyalar.Add(dsy);
                        break;
                        
                    case Farklılık_Dosya.SadeceSağda:
                    case Farklılık_Dosya.SağdakiDahaYeni:
                        if (!Dosya.Kopyala(Sağdaki.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu)) Sonuç.Dosyalar.Add(dsy);
                        break;
                } 
            }

            return Sonuç;
        }
        public Farklılık_ AslınaUygunHaleGetir(Klasör_ AsılKlasör, bool FazlaKlasörVeDosyalarıSil = false)
        {
            Farklılık_ Güncel = Karşılaştır(AsılKlasör);
            Farklılık_ Sonuç = new Farklılık_();

            foreach (Fark_Klasör_ kls in Güncel.Klasörler)
            {
                switch (kls.Farklılık)
                {
                    case Farklılık_Klasör.SadeceSolda:
                        if (FazlaKlasörVeDosyalarıSil)
                        {
                            if (!Klasör.Sil(Kök + @"\" + kls.Yolu)) Sonuç.Klasörler.Add(kls);
                        }
                        break;

                    case Farklılık_Klasör.SadeceSağda:
                        if (!Klasör.Oluştur(Kök + @"\" + kls.Yolu)) Sonuç.Klasörler.Add(kls);
                        break;
                }
            }

            foreach (Fark_Dosya_ dsy in Güncel.Dosyalar)
            {
                switch (dsy.Farklılık)
                {
                    case Farklılık_Dosya.SadeceSolda:
                        if (FazlaKlasörVeDosyalarıSil)
                        {
                            if (!Dosya.Sil(Kök + @"\" + dsy.Yolu)) Sonuç.Dosyalar.Add(dsy);
                        }
                        break;

                    case Farklılık_Dosya.SadeceSağda:
                    case Farklılık_Dosya.SağdakiDahaYeni:
                    case Farklılık_Dosya.SoldakiDahaYeni:
                        if (!Dosya.Kopyala(AsılKlasör.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu)) Sonuç.Dosyalar.Add(dsy);
                        break;

                    case Farklılık_Dosya.AynıTarihli:
                        if (!dsy.Aynı_Doğrulama_Kodu)
                        {
                            if (!Dosya.Kopyala(AsılKlasör.Kök + @"\" + dsy.Yolu, Kök + @"\" + dsy.Yolu)) Sonuç.Dosyalar.Add(dsy);
                        }
                        break;
                }
            }

            return Sonuç;
        }
        #endregion

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

        #region İçKullanım
        bool MevcutMu_Klasör(string Yolu)
        {
            return Klasörler.Contains(Yolu);
        }
        İçerik_Dosya_ MevcutMu_Dosya(string KısaYolu)
        {
            return Dosyalar.Find(x => x.Yolu == KısaYolu);
        }
        #endregion
    }
}