// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public enum GeriBildirim_Türü_ { BağlantıKurulmasıTekrarDenecek, BağlantıKuruldu, BilgiGeldi, BağlantıKoptu, Durduruldu };
    public delegate void GeriBildirim_Islemi_(string Kaynak, GeriBildirim_Türü_ Tür, object İçerik, object Hatırlatıcı);

    public interface IDonanımHaberleşmesi
    {
        /// <returns> <= 0 ise Bağlantı kurulmadı, tersi durumda bağlantı kuruldu </returns>
        int BağlantıKurulduMu();
        void Durdur();

        void Gönder(byte[] Bilgi, string Alıcı = null);
        void Gönder(string Bilgi, string Alıcı = null);
    }

    public class SatırSonu
    {
        public const string Sürüm = "V1.0";
        public const string Karakteri = "\r\n";

        public static string Sil(string Cümle)
        {
            return Cümle == null ? null : Cümle.TrimEnd('\r', '\n', ' ');
        }
        public static string Düzelt(string Cümle)
        {
            return Cümle == null ? null : Sil(Cümle) + Karakteri;
        }
    }
}
