// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public enum GeriBildirim_Türü_ { BağlantıKurulmasıTekrarDenecek, BağlantıKuruldu, BilgiGeldi, BağlantıKoptu, Durduruldu };
    public delegate void GeriBildirim_Islemi_(string Kaynak, GeriBildirim_Türü_ Tür, object İçerik, object Hatırlatıcı);

    public interface IDonanımHaberlleşmesi
    {
        bool BağlantıKurulduMu();
        void Durdur();

        void Gönder(byte[] Bilgi, string Alıcı = null);
        void Gönder(string Bilgi, string Alıcı = null);
    }
}
