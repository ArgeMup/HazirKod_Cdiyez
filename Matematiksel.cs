// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System.Linq;

namespace ArgeMup.HazirKod
{
    public class Ortalama_
    {
        public const string Sürüm = "V1.1";

        double[] Tampon = null;
        int Kapasite = 0;
        double Toplam = 0;
        int Konum = 0;
        public double Ortalaması = 0;

        public Ortalama_(int Kapasite)
        {
            this.Kapasite = Kapasite;
        }

        public double Güncelle(double Girdi)
        {
            if (Tampon == null)
            {
                Tampon = Enumerable.Repeat(Girdi, Kapasite).ToArray();
                Toplam = Girdi * Kapasite;
                return Girdi;
            }

            Toplam = Toplam - Tampon[Konum] + Girdi;
            Tampon[Konum] = Girdi;

            if (++Konum >= Kapasite) Konum = 0;

            Ortalaması = Toplam / Kapasite;
            return Ortalaması;
        }
    }

    public class Hesapla
    {
        public static double Normalleştir(double Bilgi, double BilgiDizisi_EnDüşükDeğeri, double BilgiDizisi_EnYüksekDeğeri, double AltSınır = 0, double ÜstSınır = 1)
        {
            double Fark_Bilgi = BilgiDizisi_EnYüksekDeğeri - BilgiDizisi_EnDüşükDeğeri;
            double Fark_Sınır = ÜstSınır - AltSınır;

            if (Fark_Bilgi == 0) return Fark_Sınır / 2;

            return ( ( ( Bilgi - BilgiDizisi_EnDüşükDeğeri) / Fark_Bilgi) * Fark_Sınır) + AltSınır;
        }

        public static double EnBüyük(double A, double B) 
        {
            return A >= B ? A : B;
        }
        public static double EnBüyük(double A, double B, double C)
        {
            return A >= B && A >= C ? A : B >= A && B >= C ? B : C;
        }
        public static int EnBüyük(int A, int B)
        {
            return A >= B ? A : B;
        }
        public static int EnBüyük(int A, int B, int C)
        {
            return A >= B && A >= C ? A : B >= A && B >= C ? B : C;
        }
    }

    public class Rastgele
    {
        public const string Sürüm = "V1.0";
        static System.Random rnd = new System.Random();
        public static double Sayı(double EnKüçük = 0, double EnBüyük = double.MaxValue)
        {
            return rnd.NextDouble() * (EnBüyük - EnKüçük) + EnKüçük;
        }
        public static int Sayı(int EnKüçük = 0, int EnBüyük = int.MaxValue)
        {
            return rnd.Next(EnKüçük, EnBüyük);
        }
        public static byte[] BaytDizisi(int ElemanSayısı)
        {
            byte[] dizi = new byte[ElemanSayısı];
            rnd.NextBytes(dizi);

            return dizi;
        }
        public static string Yazı(int KarakterSayısı = 10)
        {
            return Dönüştürme.D_HexYazı.BaytDizisinden( BaytDizisi(KarakterSayısı / 2) );
        }
    }
}

