// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.DonanımHaberleşmesi;
using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public class YanUygulama
    {
        public delegate void GeriBildirim_İşlemi_Uygulama_(bool BağlantıKuruldu, byte[] Bilgi, string Açıklama);

        public class Şebeke_ : IDisposable
        {
            public const string Sürüm = "V1.0";
            string DosyaYolu;
            string TcpSunucu_ErişimNoktası;
            string Edinilen_DoğrulamaKodu = null, Hesaplanan_DoğrulamaKodu = null;
            GeriBildirim_İşlemi_Uygulama_ GeriBildirim_İşlemi_Uygulama;

            Çalıştır_ ÇalıştırılanUygulamalarDeposu;
            TcpSunucu_ TcpSunucu;
            IDonanımHaberleşmesi TcpSunucu_DonanımHaberleşmesi;
            string TcpSunucu_BağlantıKuruldu;
            Process ÇalıştırılanUygulama;
            FileStream KilitDosyası;

            public Şebeke_(string DosyaYolu, GeriBildirim_İşlemi_Uygulama_ GeriBildirim_İşlemi_Uygulama, Çalıştır_ ÇalıştırılanUygulamalarDeposu, IDepo_Eleman Ayarlar, string AğAdresi_Uygulama = null, string EnDüşükSürüm = "0.0", string AğAdresi_DoğrulamaKodu = null)
            {
                if (DosyaYolu.BoşMu(true)) throw new Exception("DosyaYolu.BoşMu(true)");
                if (GeriBildirim_İşlemi_Uygulama == null) throw new Exception("GeriBildirim_İşlemi_Uygulama == null");
                if (ÇalıştırılanUygulamalarDeposu == null) throw new Exception("ÇalıştırılanUygulamalarDeposu == null");

                this.DosyaYolu = DosyaYolu;
                this.GeriBildirim_İşlemi_Uygulama = GeriBildirim_İşlemi_Uygulama;
                this.ÇalıştırılanUygulamalarDeposu = ÇalıştırılanUygulamalarDeposu;

                Başlat(AğAdresi_Uygulama, AğAdresi_DoğrulamaKodu, EnDüşükSürüm, Ayarlar);
            }
            public bool BağlantıKuruldu { get => TcpSunucu_BağlantıKuruldu != null; }
            public void Gönder(byte[] Girdi)
            {
                if (TcpSunucu_BağlantıKuruldu == null) throw new Exception("Bağlantı kurulmadı");

                TcpSunucu_DonanımHaberleşmesi.Gönder(Girdi, TcpSunucu_BağlantıKuruldu);
            }

            void Başlat(string AğAdresi_Uygulama = null, string AğAdresi_DoğrulamaKodu = null, string EnDüşükSürüm = "0.0", IDepo_Eleman Ayarlar = null)
            {
                TcpSunucu_BağlantıKuruldu = null;
                if (disposedValue) return;

                Dosya.AğÜzerinde_ Dosya_AğÜzerinde_DoğrulamaKodu = null, Dosya_AğÜzerinde_Uygulama = null;

                if (Edinilen_DoğrulamaKodu.BoşMu(true) && AğAdresi_DoğrulamaKodu.DoluMu(true))
                {
                    Dosya_AğÜzerinde_DoğrulamaKodu = new Dosya.AğÜzerinde_(new Uri(AğAdresi_DoğrulamaKodu), null, GeriBildirim_Islemi_AğÜzerinde_DoğrulamaKodu);
                }
                else
                {
                    GeriBildirim_Islemi_AğÜzerinde_DoğrulamaKodu(true, null, null);
                }

                void GeriBildirim_Islemi_AğÜzerinde_DoğrulamaKodu(bool Sonuç_AğÜzerinde_DoğrulamaKodu, Uri _AğAdresi_DoğrulamaKodu_, object Çıktı_DoğrulamaKodu)
                {
                    Dosya_AğÜzerinde_DoğrulamaKodu?.Dispose(); Dosya_AğÜzerinde_DoğrulamaKodu = null;

                    if (!Sonuç_AğÜzerinde_DoğrulamaKodu)
                    {
                        Edinilen_DoğrulamaKodu = Ayarlar?.Oku(AğAdresi_DoğrulamaKodu.Replace("/", "'"));
                        if (Edinilen_DoğrulamaKodu.BoşMu(true))
                        {
                            GeriBildirim_İşlemi_Uygulama(false, null, "AğAdresi_DoğrulamaKodu indirilemedi <" + AğAdresi_DoğrulamaKodu + ">");
                            return;
                        }
                    }

                    if (Çıktı_DoğrulamaKodu != null)
                    {
                        Edinilen_DoğrulamaKodu = (Çıktı_DoğrulamaKodu as byte[]).Yazıya();
                        Ayarlar?.Yaz(AğAdresi_DoğrulamaKodu.Replace("/", "'"), Edinilen_DoğrulamaKodu);
                    }

                    if (!Dosya.GüncelMi(DosyaYolu, EnDüşükSürüm))
                    {
                        if (AğAdresi_Uygulama.BoşMu(true))
                        {
                            GeriBildirim_İşlemi_Uygulama(false, null, "AğAdresi_Uygulama içeriği boş");
                        }
                        else
                        {
                            Dosya_AğÜzerinde_Uygulama = new Dosya.AğÜzerinde_(new Uri(AğAdresi_Uygulama), DosyaYolu, GeriBildirim_Islemi_AğÜzerinde_Uygulama, 30000);
                        }
                    }
                    else
                    {
                        GeriBildirim_Islemi_AğÜzerinde_Uygulama(true, null, null);
                    }
                }
                void GeriBildirim_Islemi_AğÜzerinde_Uygulama(bool Sonuç_AğÜzerinde_Uygulama, Uri _AğAdresi_Uygulama_, object Çıktı_Uygulama)
                {
                    Dosya_AğÜzerinde_Uygulama?.Dispose(); Dosya_AğÜzerinde_Uygulama = null;

                    if (!Sonuç_AğÜzerinde_Uygulama || !File.Exists(DosyaYolu))
                    {
                        GeriBildirim_İşlemi_Uygulama(false, null, "AğAdresi_Uygulama indirilemedi <" + AğAdresi_Uygulama + ">");
                        return;
                    }

                    if (KilitDosyası == null)
                    {
                        KilitDosyası = new FileStream(DosyaYolu, FileMode.Open, FileAccess.Read, FileShare.Read);
                        Hesaplanan_DoğrulamaKodu = null;
                    }

                    if (Edinilen_DoğrulamaKodu.DoluMu(true))
                    {
                        if (Hesaplanan_DoğrulamaKodu.BoşMu(true)) Hesaplanan_DoğrulamaKodu = DoğrulamaKodu.Üret.Akıştan(KilitDosyası);

                        if (Hesaplanan_DoğrulamaKodu != Edinilen_DoğrulamaKodu)
                        {
                            KilitDosyası.Dispose(); KilitDosyası = null;

                            if (!Dosya.Sil(DosyaYolu))
                            {
                                GeriBildirim_İşlemi_Uygulama(false, null, "Dosya silinemedi <" + DosyaYolu + ">");
                            }
                            else
                            {
                                if (_AğAdresi_Uygulama_ == null)
                                {
                                    //Doğrudan çağırıldı
                                    GeriBildirim_Islemi_AğÜzerinde_DoğrulamaKodu(true, null, null);
                                }
                                else
                                {
                                    //Dosya.AğÜzerinde_ işleminden çağırıldı
                                    GeriBildirim_Islemi_AğÜzerinde_Uygulama(false, null, null);
                                }
                            }
                            
                            return;
                        }
                    }

                    if (TcpSunucu == null)
                    {
                        TcpSunucu = new TcpSunucu_(0, GeriBildirim_Islemi_TcpSunucu, SatırSatırGönderVeAl: false, Güvenli: true);
                        while (TcpSunucu.EtkinErişimNoktası() == 0 && ArkaPlan.Ortak.Çalışsın) Thread.Sleep(5);

                        TcpSunucu_ErişimNoktası = TcpSunucu.EtkinErişimNoktası().ToString();
                        TcpSunucu_DonanımHaberleşmesi = TcpSunucu;
                    }
                    else TcpSunucu.Durdur(null); //etkin bağlantıları durdur

                    try { ÇalıştırılanUygulama?.Kill(); } catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }
                    ÇalıştırılanUygulama = ÇalıştırılanUygulamalarDeposu.UygulamayıDoğrudanÇalıştır(DosyaYolu, new string[] { TcpSunucu_ErişimNoktası });
                }
            }
            void GeriBildirim_Islemi_TcpSunucu(string Kaynak, GeriBildirim_Türü_ Tür, object İçerik, object Hatırlatıcı)
            {
                if (TcpSunucu_BağlantıKuruldu != null && TcpSunucu_BağlantıKuruldu != Kaynak)
                {
                    //Korsan bağlantı
                    TcpSunucu.Durdur(Kaynak);
                    return;
                }

                if (ÇalıştırılanUygulama == null || ÇalıştırılanUygulama.HasExited) Tür = GeriBildirim_Türü_.BağlantıKoptu;

                switch (Tür)
                {
                    case GeriBildirim_Türü_.BağlantıKoptu:
                        TcpSunucu_BağlantıKuruldu = null;
                        Başlat();
                        break;

                    case GeriBildirim_Türü_.BağlantıKuruldu:
                        TcpSunucu_BağlantıKuruldu = ErişimNoktası_PID_UygunMu(Kaynak) ? Kaynak : null;
                        break;

                    //case GeriBildirim_Türü_.BilgiGeldi:
                    //case GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek:
                    //case GeriBildirim_Türü_.Durduruldu:
                    //    break;
                }

                GeriBildirim_İşlemi_Uygulama(TcpSunucu_BağlantıKuruldu != null, İçerik as byte[], Tür.ToString());
            }

            bool ErişimNoktası_PID_UygunMu(string ErişimNoktası)
            {
                bool Bitti = false, Başarılı = false;
                string kıstas = "*TCP*" + ErişimNoktası + "*:" + TcpSunucu_ErişimNoktası + "*ESTABLISHED*" + ÇalıştırılanUygulama.Id.Yazıya();
                KomutSatırıUygulaması_ KomutSatırıUygulaması = null;

                KomutSatırıUygulaması = new KomutSatırıUygulaması_("netstat.exe", "-no", GeriBildirim_Islemi_);
                while (!Bitti && ArkaPlan.Ortak.Çalışsın) Thread.Sleep(5);

                return Başarılı;

                void GeriBildirim_Islemi_(string Kaynak, GeriBildirim_Türü_ Tür, object İçerik, object Hatırlatıcı)
                {
                    switch (Tür)
                    {
                        case GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek:
                        case GeriBildirim_Türü_.BağlantıKoptu:
                        case GeriBildirim_Türü_.Durduruldu:
                            KomutSatırıUygulaması?.Dispose();
                            KomutSatırıUygulaması = null;
                            Bitti = true;
                            break;

                        case GeriBildirim_Türü_.BilgiGeldi:
                            /*
                              Proto  Local Address(ErişimNoktası)   Foreign Address        State           PID(ÇalıştırılanUygulama.Id)
                              TCP    127.0.0.1:6437                 127.0.0.1:6434         ESTABLISHED     11996
                             */

                            if ((İçerik as string).BenzerMi(kıstas))
                            {
                                Başarılı = true;
                                (KomutSatırıUygulaması as IDonanımHaberleşmesi).Durdur();
                            }
                            break;
                    }
                }
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

                        DosyaYolu = null;
                        TcpSunucu_BağlantıKuruldu = null;
                        TcpSunucu?.Dispose();
                        KilitDosyası?.Dispose();
                        try { ÇalıştırılanUygulama?.Kill(); } catch (Exception) { }
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~YanUygulama_Şebeke_()
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

        public class Şube_ : IDisposable
        {
            public const string Sürüm = "V1.0";
            GeriBildirim_İşlemi_Uygulama_ GeriBildirim_İşlemi_Uygulama;
            IDonanımHaberleşmesi Tcpİstemci;
            bool Tcpİstemci_BağlantıKuruldu;

            public Şube_(int ErişimNoktası, GeriBildirim_İşlemi_Uygulama_ GeriBildirim_İşlemi_Uygulama)
            {
                if (ErişimNoktası <= 0) throw new Exception("ErişimNoktası <= 0");
                if (GeriBildirim_İşlemi_Uygulama == null) throw new Exception("GeriBildirim_İşlemi_Uygulama == null");

                this.GeriBildirim_İşlemi_Uygulama = GeriBildirim_İşlemi_Uygulama;

                Tcpİstemci_Güvenli_ Tcpİstemci_Asıl = new Tcpİstemci_Güvenli_(ErişimNoktası, GeriBildirim_Islemi: GeriBildirim_Islemi_Tcpİstemci);
                Tcpİstemci = Tcpİstemci_Asıl;
            }
            public void Gönder(byte[] Girdi)
            {
                if (!Tcpİstemci_BağlantıKuruldu) throw new Exception("Bağlantı kurulmadı");

                Tcpİstemci.Gönder(Girdi);
            }
            void GeriBildirim_Islemi_Tcpİstemci(string Kaynak, GeriBildirim_Türü_ Tür, object İçerik, object Hatırlatıcı)
            {
                switch (Tür)
                {
                    case GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek:
                    case GeriBildirim_Türü_.BağlantıKoptu:
                    case GeriBildirim_Türü_.Durduruldu:
                        Tcpİstemci_BağlantıKuruldu = false;
                        break;

                    case GeriBildirim_Türü_.BağlantıKuruldu:
                        Tcpİstemci_BağlantıKuruldu = true;
                        break;

                    //case GeriBildirim_Türü_.BilgiGeldi:
                    //    break;
                }

                GeriBildirim_İşlemi_Uygulama(Tcpİstemci_BağlantıKuruldu, İçerik as byte[], Tür.ToString());
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

                        Tcpİstemci_BağlantıKuruldu = false;
                        Tcpİstemci?.Durdur();
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~YanUygulama_Şebeke_()
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

        public class İçDoğrulamaKodu
        {
            public const int DoğrulamaKodu_ÇıktıKarakterSayısı = 64;

            public static byte[] Ekle(byte[] Girdi)
            {
                byte[] birarada = new byte[DoğrulamaKodu_ÇıktıKarakterSayısı + Girdi.Length];
                byte[] doko = ArgeMup.HazirKod.DoğrulamaKodu.Üret.BaytDizisinden(Girdi, DoğrulamaKodu_ÇıktıKarakterSayısı);

                Array.Copy(doko, 0, birarada, 0, DoğrulamaKodu_ÇıktıKarakterSayısı);
                Array.Copy(Girdi, 0, birarada, DoğrulamaKodu_ÇıktıKarakterSayısı, Girdi.Length);

                return birarada;
            }

            public static bool KontrolEt(ref byte[] Girdi)
            {
                int içerik_Sayısı = Girdi.Length - DoğrulamaKodu_ÇıktıKarakterSayısı;
                if (içerik_Sayısı <= 0) return false;

                byte[] doko = new byte[DoğrulamaKodu_ÇıktıKarakterSayısı];
                byte[] içerik = new byte[içerik_Sayısı];
                
                Array.Copy(Girdi, 0, doko, 0, DoğrulamaKodu_ÇıktıKarakterSayısı);
                Array.Copy(Girdi, DoğrulamaKodu_ÇıktıKarakterSayısı, içerik, 0, içerik_Sayısı);

                byte[] doko_hesaplanan = ArgeMup.HazirKod.DoğrulamaKodu.Üret.BaytDizisinden(içerik, DoğrulamaKodu_ÇıktıKarakterSayısı);
                if (!doko.SequenceEqual(doko_hesaplanan)) return false;

                Girdi = içerik;
                return true;
            }
        }
    }
}
