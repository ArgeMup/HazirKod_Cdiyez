// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

namespace ArgeMup.HazirKod.Ekİşlemler
{
    public static class _Ekİşlemler_Yazı
    {
        public const string Sürüm = "V1.0";

        public static byte[] BaytDizisine(this string Girdi)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_Yazı.BaytDizisine(Girdi);
        }
        public static byte[] BaytDizisine_HexYazıdan(this string Girdi)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_HexYazı.BaytDizisine(Girdi);
        }

        public static int TamSayıya(this string Girdi)
        {
            return int.Parse(Girdi);
        }
        public static uint İşaretsizTamSayıya(this string Girdi)
        {
            return uint.Parse(Girdi);
        }
        public static double NoktalıSayıya(this string Girdi, bool TamKontrol = true, bool GeçersizKarakterleriSil = true)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_Sayı.Yazıdan(Girdi, TamKontrol, GeçersizKarakterleriSil);
        }

        public static System.DateTime TarihSaate(this string Girdi, string Şablon = "dd.MM.yyyy HH:mm:ss.fff", System.Globalization.CultureInfo Kültür = null, System.Globalization.DateTimeStyles Tip = System.Globalization.DateTimeStyles.AssumeLocal)
        {
            if (System.DateTime.TryParseExact(Girdi, Şablon, Kültür, Tip, out System.DateTime yeni)) return yeni;

            throw new System.Exception("TarihSaate dönüştürülemedi " + Girdi);
        }
#if NET7_0_OR_GREATER
        public static System.DateOnly SadeceTarihe(this string Girdi, string Şablon = "dd.MM.yyyy", System.Globalization.CultureInfo Kültür = null, System.Globalization.DateTimeStyles Tip = System.Globalization.DateTimeStyles.AssumeLocal)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_SadeceTarih.SadeceTarihe(Girdi, Şablon, Kültür, Tip);
        }
        public static System.TimeOnly SadeceSaate(this string Girdi, string Şablon = "HH:mm:ss.ffffff", System.Globalization.CultureInfo Kültür = null, System.Globalization.DateTimeStyles Tip = System.Globalization.DateTimeStyles.AssumeLocal)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_SadeceSaat.SadeceSaate(Girdi, Şablon, Kültür, Tip);
        }
#endif
        public static object Nesneye(this string Girdi, System.Type Tipi, Değişken_.DönüştürmeHatasıOldu_YeniTipiBelirle_ DönüştürmeHatasıOldu_YeniTipiBelirle = null)
        {
            Değişken_ Değişken = new Değişken_();
            Değişken.NitelikKontrolü_Ve_Filtreleme_Yap = false;
            Değişken.DönüştürmeHatasıOldu_YeniTipiBelirle = DönüştürmeHatasıOldu_YeniTipiBelirle;

            Depo_ Depo = new Depo_(Girdi);
            return Değişken.Üret(Tipi, Depo["ArGeMuP"]);
        }

        public static string Karıştır(this string Girdi, string Şifre)
        {
            return DahaCokKarmasiklastirma.Karıştır(Girdi, Şifre);
        }
        public static string Düzelt(this string Girdi, string Şifre)
        {
            return DahaCokKarmasiklastirma.Düzelt(Girdi, Şifre);
        }

        public static byte[] Taban64ten(this string Girdi)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_Yazı.Taban64ten(Girdi);
        }

        public static bool BoşMu(this string Girdi, bool BoşluğuGörmezdenGel = false)
        {
            if (BoşluğuGörmezdenGel) return string.IsNullOrWhiteSpace(Girdi);
            else return string.IsNullOrEmpty(Girdi);
        }
        public static bool DoluMu(this string Girdi, bool BoşluğuGörmezdenGel = false)
        {
            return !BoşMu(Girdi, BoşluğuGörmezdenGel);
        }
        public static bool BenzerMi(this string Girdi, string Kıstas, bool BüyükKüçükHarfDuyarlı = true, char Ayraç = '*')
        {
            if (Girdi.BoşMu() || Kıstas.BoşMu()) return false;
            
            string Ayraçç = Ayraç.ToString();
            if (Kıstas == Ayraçç) return true;

            if (!BüyükKüçükHarfDuyarlı)
            {
                Girdi = Girdi.ToLower();
                Kıstas = Kıstas.ToLower();
            }

            if (Kıstas.StartsWith(Ayraçç)) Kıstas = Kıstas.TrimStart(Ayraç);
            else
            {
                string ilk_kıstas = Kıstas.Split(Ayraç)[0];
                if (!Girdi.StartsWith(ilk_kıstas)) return false;
            }

            if (Kıstas.EndsWith(Ayraçç)) Kıstas = Kıstas.TrimEnd(Ayraç);
            else
            {
                string[] kıstas_lar = Kıstas.Split(Ayraç);
                string son_kıstas = kıstas_lar[kıstas_lar.Length - 1];
                if (!Girdi.EndsWith(son_kıstas)) return false;
            }

            int konum = 0;
            foreach (string kst in Kıstas.Split(Ayraç))
            {
                konum = Girdi.IndexOf(kst, konum);
                if (konum < 0) return false;

                konum += kst.Length;
            }

            return true;
        }
        public static bool BenzerMi(this string Girdi, System.Collections.Generic.IEnumerable<string> Kıstaslar, bool BüyükKüçükHarfDuyarlı = true, char Ayraç = '*')
        {
            if (Girdi.BoşMu() || Kıstaslar == null) return false;

            foreach (string Kıstas in Kıstaslar)
            {
                if (Girdi.BenzerMi(Kıstas, BüyükKüçükHarfDuyarlı, Ayraç)) return true;
            }

            return false;
        }

        public static string Günlük(this string Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi, Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_int
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this int Girdi, string Şablon = null)
        {
            return Girdi.ToString(Şablon);
        }

        public static int Günlük(this int Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_uint
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this uint Girdi, string Şablon = null)
        {
            return Girdi.ToString(Şablon);
        }

        public static uint Günlük(this uint Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_double
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this double Girdi)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_Sayı.Yazıya(Girdi);
        }

        public static double Günlük(this double Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_BaytDizisi
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this byte[] Girdi, int Boyut = int.MinValue)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_Yazı.BaytDizisinden(Girdi, Boyut);
        }
        public static string HexYazıya(this byte[] Girdi, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_HexYazı.BaytDizisinden(Girdi, Adet, BaşlangıçKonumu);
        }

        public static byte[] Karıştır(this byte[] Girdi, byte[] Şifre)
        {
            return DahaCokKarmasiklastirma.Karıştır(Girdi, Şifre);
        }
        public static byte[] Düzelt(this byte[] Girdi, byte[] Şifre)
        {
            return DahaCokKarmasiklastirma.Düzelt(Girdi, Şifre);
        }

        public static string Taban64e(this byte[] Girdi)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_BaytDizisi.Taban64e(Girdi);
        }

        public static byte[] Günlük(this byte[] Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(Girdi, int.MinValue, 0, Seviyesi, ÖnYazı, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_Liste
    {
        public const string Sürüm = "V1.0";

        public static System.Collections.IList Sırala(this System.Collections.IList Girdi, string SıralanacakListeİçindeki_Değişkeninİçindeki_KontrolEdilecek_DeğişkeninAdı, System.Collections.IEnumerable YeniSıralama)
        {
            if (Girdi == null || Girdi.Count == 0 || SıralanacakListeİçindeki_Değişkeninİçindeki_KontrolEdilecek_DeğişkeninAdı.BoşMu() || YeniSıralama == null) return Girdi;

            System.Collections.Generic.Dictionary<string, object> YeniSözlük = new System.Collections.Generic.Dictionary<string, object>();
            foreach (object Eleman in Girdi)
            {
                System.Reflection.FieldInfo Eleman_AlanBilgisi = Eleman.GetType().GetField(SıralanacakListeİçindeki_Değişkeninİçindeki_KontrolEdilecek_DeğişkeninAdı, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                if (Eleman_AlanBilgisi == null) continue;

                string Eleman_İçeriği = Eleman_AlanBilgisi.GetValue(Eleman) as string;
                if (Eleman_İçeriği.BoşMu()) continue;

                YeniSözlük.Add(Eleman_İçeriği, Eleman);
            }

            System.Collections.IList YeniListe = (System.Collections.IList)System.Activator.CreateInstance(Girdi.GetType());
            foreach (string SıradakiElamanAdı in YeniSıralama)
            {
                if (SıradakiElamanAdı.BoşMu() || !YeniSözlük.TryGetValue(SıradakiElamanAdı, out object İçerik)) continue;

                YeniListe.Add(İçerik);
            }

            return YeniListe;
        }
    }

    public static class _Ekİşlemler_Sözlük
    {
        public const string Sürüm = "V1.0";

        public static System.Collections.IDictionary Sırala(this System.Collections.IDictionary Girdi, System.Collections.IEnumerable YeniSıralama)
        {
            if (Girdi == null || Girdi.Count == 0 || YeniSıralama == null) return Girdi;

            System.Collections.IDictionary YeniSözlük = (System.Collections.IDictionary)System.Activator.CreateInstance(Girdi.GetType());

            foreach (string SıradakiElamanAdı in YeniSıralama)
            {
                if (SıradakiElamanAdı.BoşMu() || !Girdi.Contains(SıradakiElamanAdı)) continue;

                YeniSözlük.Add(SıradakiElamanAdı, Girdi[SıradakiElamanAdı]);
            }

            return YeniSözlük;
        }
    }

    public static class _Ekİşlemler_TarihSaat
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this System.DateTime Girdi, string Şablon = "dd.MM.yyyy HH:mm:ss.fff", System.Globalization.CultureInfo Kültür = null)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_TarihSaat.Yazıya(Girdi, Şablon, Kültür);
        }

        public static System.DateTime Günlük(this System.DateTime Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }
#if NET7_0_OR_GREATER
    public static class _Ekİşlemler_SadeceTarih
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this System.DateOnly Girdi, string Şablon = "dd.MM.yyyy", System.Globalization.CultureInfo Kültür = null)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_SadeceTarih.Yazıya(Girdi, Şablon, Kültür);
        }

        public static System.DateOnly Günlük(this System.DateOnly Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }
    public static class _Ekİşlemler_SadeceSaat
    {
        public const string Sürüm = "V1.0";

        public static string Yazıya(this System.TimeOnly Girdi, string Şablon = "HH:mm:ss.ffffff", System.Globalization.CultureInfo Kültür = null)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_SadeceSaat.Yazıya(Girdi, Şablon, Kültür);
        }

        public static System.TimeOnly Günlük(this System.TimeOnly Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }
#endif

    public static class _Ekİşlemler_object
    {
        public const string Sürüm = "V1.0";

        public static bool BoşVeyaVarsayılanDeğerdeMi<T>(this T Nesne)
        {
            if (Nesne == null) return true;
            if (object.Equals(Nesne, default(T))) return true;

            System.Type TipinTipi = typeof(T);
            if (System.Nullable.GetUnderlyingType(TipinTipi) != null) return false;

            System.Type NesneTipi = Nesne.GetType();
            if (NesneTipi.IsValueType && NesneTipi != TipinTipi)
            {
                object gölgesi = System.Activator.CreateInstance(NesneTipi);
                return gölgesi.Equals(Nesne);
            }

            return false;
        }
        
        public static string Yazıya(this object Girdi)
        {
            Değişken_ Değişken = new Değişken_();
            Değişken.NitelikKontrolü_Ve_Filtreleme_Yap = false;

            Depo_ Depo = new Depo_();
            Değişken.Depola(Girdi, Depo["ArGeMuP"]);
            return Depo.YazıyaDönüştür();
        }
        
        public static object Kopyala(this object Girdi, Değişken_.DönüştürmeHatasıOldu_YeniTipiBelirle_ DönüştürmeHatasıOldu_YeniTipiBelirle = null, bool KontrolEt = true)
        {
            string İçerik = Girdi.Yazıya();
            object Çıktı = İçerik.Nesneye(Girdi.GetType(), DönüştürmeHatasıOldu_YeniTipiBelirle);
            if (KontrolEt && İçerik != Çıktı.Yazıya()) throw new System.Exception("Kopyalama başarısız");
            return Çıktı;
        }

        public static object Günlük(this object Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + (Girdi == null ? "null" : Girdi.GetType().FullName + " " + Girdi.ToString()), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_İstisna
    {
        public const string Sürüm = "V1.0";

        public static System.Exception Günlük(this System.Exception Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.BeklenmeyenDurum, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            string çıktı = "";
            System.Exception ist = Girdi;

            try
            {
                while (ist != null)
                {
                    çıktı += ist.ToString() + "|";
                    ist = ist.InnerException;
                }

                ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + çıktı.TrimEnd('|'), Seviyesi, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            }
            catch (System.Exception) { }

            return Girdi;
        }
    }
}