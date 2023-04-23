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

        static ArgeMup.HazirKod.DahaCokKarmasiklastirma_ DaÇoKa = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_();
        public static string Karıştır(this string Girdi, string Şifre)
        {
            return DaÇoKa.Karıştır(Girdi, Şifre);
        }
        public static string Düzelt(this string Girdi, string Şifre)
        {
            return DaÇoKa.Düzelt(Girdi, Şifre);
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

        public static string DosyaYolu_Düzelt(this string DosyaYolu, bool GeçersizKarakterleriSil = true)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_DosyaKlasörAdı.Düzelt(DosyaYolu, GeçersizKarakterleriSil);
        }
        public static string DosyaYolu_DosyaAdı(this string DosyaYolu)
        {
            return System.IO.Path.GetFileName(DosyaYolu);
        }
        public static string DosyaYolu_DosyaSoyAdı(this string DosyaYolu)
        {
            return System.IO.Path.GetExtension(DosyaYolu).Substring(1);
        }
        public static string DosyaYolu_ÜstKlasör(this string DosyaYolu, int Seviye = 1, bool KökeUlaşıncaDur = false)
        {
            return Klasör.ÜstKlasör(DosyaYolu, Seviye, KökeUlaşıncaDur);
        }
        public static string DosyaYolu_Oku_Yazı(this string DosyaYolu)
        {
            if (!System.IO.File.Exists(DosyaYolu)) return null;
            else return System.IO.File.ReadAllText(DosyaYolu);
        }
        public static byte[] DosyaYolu_Oku_BaytDizisi(this string DosyaYolu)
        {
            if (!System.IO.File.Exists(DosyaYolu)) return null;
            else return System.IO.File.ReadAllBytes(DosyaYolu);
        }
        public static void DosyaYolu_Yaz(this string DosyaYolu, string Yazı)
        {
            Klasör.Oluştur(DosyaYolu.DosyaYolu_ÜstKlasör(), false);
            System.IO.File.WriteAllText(DosyaYolu, Yazı);
        }
        public static void DosyaYolu_Yaz(this string DosyaYolu, byte[] BaytDizisi)
        {
            Klasör.Oluştur(DosyaYolu.DosyaYolu_ÜstKlasör(), false);
            System.IO.File.WriteAllBytes(DosyaYolu, BaytDizisi);
        }
        public static void Dosyaİçeriği_Yaz(this string İçerik, string DosyaYolu)
        {
            Klasör.Oluştur(DosyaYolu.DosyaYolu_ÜstKlasör(), false);
            System.IO.File.WriteAllText(DosyaYolu, İçerik);
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

        static ArgeMup.HazirKod.DahaCokKarmasiklastirma_ DaÇoKa = new ArgeMup.HazirKod.DahaCokKarmasiklastirma_();
        public static byte[] Karıştır(this byte[] Girdi, byte[] Şifre)
        {
            return DaÇoKa.Karıştır(Girdi, Şifre);
        }
        public static byte[] Düzelt(this byte[] Girdi, byte[] Şifre)
        {
            return DaÇoKa.Düzelt(Girdi, Şifre);
        }

        public static string Taban64e(this byte[] Girdi)
        {
            return ArgeMup.HazirKod.Dönüştürme.D_BaytDizisi.Taban64e(Girdi);
        }

        public static void Dosyaİçeriği_Yaz(this byte[] İçerik, string DosyaYolu)
        {
            Klasör.Oluştur(DosyaYolu.DosyaYolu_ÜstKlasör(), false);
            System.IO.File.WriteAllBytes(DosyaYolu, İçerik);
        }

        public static byte[] Günlük(this byte[] Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0, bool Hemen = false)
        {
            ArgeMup.HazirKod.Günlük.Ekle(Girdi, int.MinValue, 0, Seviyesi, ÖnYazı, ÇağıranDosya, ÇağıranSatırNo, Hemen);
            return Girdi;
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

    public static class _Ekİşlemler_object
    {
        public const string Sürüm = "V1.0";

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