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
        public static string Şifrele(this string Girdi, string Şifre)
        {
            return DaÇoKa.Karıştır(Girdi, Şifre);
        }
        public static string ŞifresiniÇöz(this string Girdi, string Şifre)
        {
            return DaÇoKa.Düzelt(Girdi, Şifre);
        }

        public static string Günlük(this string Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi, Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
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

        public static int Günlük(this int Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
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

        public static uint Günlük(this uint Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
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

        public static double Günlük(this double Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
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

        public static byte[] Günlük(this byte[] Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(Girdi, int.MinValue, 0, Seviyesi, ÖnYazı, ÇağıranDosya, ÇağıranSatırNo);
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

        public static System.DateTime Günlük(this System.DateTime Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + Girdi.Yazıya(), Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_object
    {
        public const string Sürüm = "V1.0";

        public static object Günlük(this object Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.Geveze, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
        {
            ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + (Girdi == null ? "null" : Girdi.GetType().FullName + " " + Girdi.ToString()), Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
            return Girdi;
        }
    }

    public static class _Ekİşlemler_İstisna
    {
        public const string Sürüm = "V1.0";

        public static System.Exception Günlük(this System.Exception Girdi, string ÖnYazı = null, ArgeMup.HazirKod.Günlük.Seviye Seviyesi = ArgeMup.HazirKod.Günlük.Seviye.BeklenmeyenDurum, [System.Runtime.CompilerServices.CallerFilePath] string ÇağıranDosya = "", [System.Runtime.CompilerServices.CallerLineNumber] int ÇağıranSatırNo = 0)
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

                ArgeMup.HazirKod.Günlük.Ekle(ÖnYazı + çıktı.TrimEnd('|'), Seviyesi, ÇağıranDosya, ÇağıranSatırNo);
            }
            catch (System.Exception) { }

            return Girdi;
        }
    }
}