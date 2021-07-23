// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System.Linq;

namespace ArgeMup.HazirKod
{
    public class Ortalama_
    {
        public const string Sürüm = "V1.0";

        double[] Tampon = null;
        int Kapasite = 0;
        double Toplam = 0;
        int Konum = 0;

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

            return Toplam / Kapasite;
        }
    }
}

