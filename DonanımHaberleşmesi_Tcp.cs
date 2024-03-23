// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public class Tcpİstemci_ : IDisposable, IDonanımHaberleşmesi
    {
        public const string Sürüm = "V1.2";

        #region Genel Görüşe Açık
        public object Hatırlatıcı = null;
        public bool SatırSatırGönderVeAl = true;
        public int TekrarDeneme_ZamanAşımı_msn = 5000;
        public int BilgiGönderme_ZamanAşımı_msn = 15000;
        #endregion

        #region İç Kullanım
        int ErişimNoktası = -1, Sessizlik_ZamanAşımı_Anı, Sessizlik_ZamanAşımı_msn = 0, AksayanBilgiAlımı_ZamanAşımı_msn;
        string IpVeyaAdı = "";
        bool Çalışşsın = true;

        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

        TcpClient İstemci = null;
        NetworkStream Aracı = null;
        StreamReader Alıcı = null;
        StreamWriter Verici = null;
        #endregion
        
        public Tcpİstemci_(int ErişimNoktası, string IpVeyaAdı = "127.0.0.1", GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, int Sessizlik_ZamanAşımı_msn = 0, int AksayanBilgiAlımı_ZamanAşımı_msn = 5)
        {
            if (ErişimNoktası < 0) throw new Exception("ErişimNoktası >= 0 olmalı");

            this.ErişimNoktası = ErişimNoktası;
            this.IpVeyaAdı = IpVeyaAdı;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;
            this.Sessizlik_ZamanAşımı_msn = Sessizlik_ZamanAşımı_msn;
            this.AksayanBilgiAlımı_ZamanAşımı_msn = AksayanBilgiAlımı_ZamanAşımı_msn;

            new Thread(() => Görev_İşlemi_Tcpİstemci()).Start();
        }
        public Tcpİstemci_(TcpClient İstemci, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, int Sessizlik_ZamanAşımı_msn = 0, int AksayanBilgiAlımı_ZamanAşımı_msn = 5)
        {
            this.İstemci = İstemci;
            this.IpVeyaAdı = İstemci.Client.RemoteEndPoint.ToString();
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;
            this.Sessizlik_ZamanAşımı_msn = Sessizlik_ZamanAşımı_msn;
            this.AksayanBilgiAlımı_ZamanAşımı_msn = AksayanBilgiAlımı_ZamanAşımı_msn;

            new Thread(() => Görev_İşlemi_Tcpİstemci()).Start();
        }
        void Görev_İşlemi_Tcpİstemci()
        {
            while (Çalışşsın)
            {
                try
                {
                    if (İstemci == null || !İstemci.Connected || ErişimNoktası == -1)
                    {
                        if (ErişimNoktası >= 0) İstemci = new TcpClient(IpVeyaAdı, ErişimNoktası);
                        else ErişimNoktası--;

                        İstemci.ReceiveTimeout = TekrarDeneme_ZamanAşımı_msn;
                        İstemci.SendTimeout = BilgiGönderme_ZamanAşımı_msn;

                        Aracı = İstemci.GetStream();

                        if (SatırSatırGönderVeAl)
                        {
                            Alıcı = new StreamReader(Aracı);
                            Verici = new StreamWriter(Aracı);
                        }

                        GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.BağlantıKuruldu, null, Hatırlatıcı);

                        if (Sessizlik_ZamanAşımı_msn > 0) Sessizlik_ZamanAşımı_Anı = Environment.TickCount + Sessizlik_ZamanAşımı_msn;
                    }

                    int sayac = 0;
                    while (Çalışşsın && İstemci.Connected)
                    {
                        object çıktı = null;
                        if (SatırSatırGönderVeAl) çıktı = SatırSonu.Sil(Alıcı.ReadLine());
                        else
                        {
                            int ilk_gelen_bilgi = Aracı.ReadByte();
                            if (ilk_gelen_bilgi >= 0)
                            {
                                int Tampon_Adet = 1;
                                byte[] Tampon = new byte[1];
                                Tampon[0] = (byte)ilk_gelen_bilgi;

                                int AksayanBilgiAlımı_ZamanAşımı_msn_anı = Environment.TickCount + AksayanBilgiAlımı_ZamanAşımı_msn;
                                while (AksayanBilgiAlımı_ZamanAşımı_msn_anı > Environment.TickCount || İstemci.Available > 0)
                                {
                                    int adet_bekleyen = İstemci.Available;
                                    if (adet_bekleyen > 0)
                                    {
                                        Array.Resize(ref Tampon, Tampon_Adet + adet_bekleyen);

                                        int adet_okunan = Aracı.Read(Tampon, Tampon_Adet, adet_bekleyen);
                                        Tampon_Adet += adet_okunan;

                                        AksayanBilgiAlımı_ZamanAşımı_msn_anı = Environment.TickCount + AksayanBilgiAlımı_ZamanAşımı_msn;
                                    }
                                    
                                    Thread.Sleep(1);
                                }

                                if (Tampon.Length != Tampon_Adet) Array.Resize(ref Tampon, Tampon_Adet);
                                çıktı = Tampon;
                            }
                        }

                        if (çıktı == null) throw new Exception();
                        else
                        {
                            GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.BilgiGeldi, çıktı, Hatırlatıcı);

                            if (sayac++ > 100) { sayac = 0; Thread.Sleep(1); } //cpu yüzdesini düşürmek için

                            if (Sessizlik_ZamanAşımı_msn > 0) Sessizlik_ZamanAşımı_Anı = Environment.TickCount + Sessizlik_ZamanAşımı_msn;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (Sessizlik_ZamanAşımı_msn > 0 && Sessizlik_ZamanAşımı_Anı < Environment.TickCount) Çalışşsın = false;
                    else
                    {
                        int bekleme = TekrarDeneme_ZamanAşımı_msn;
                        bool yeniden_başlat = true;

                        if (ex.HResult == -2147467259) //bağlantı kurulamadı
                        {
                            if (ErişimNoktası < 0)
                            {
                                Çalışşsın = false;
                            }
                            else
                            {
                                GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek, null, Hatırlatıcı);
                            }
                        }
                        else if (ex.HResult == -2146232800) //cihaz bilgi göndermedi
                        {
                            yeniden_başlat = false;
                            bekleme = 1; //cpu yüzdesini düşürmek için
                        }
                        else
                        {
                            if (ErişimNoktası < 0)
                            {
                                Çalışşsın = false;
                            }

                            GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.BağlantıKoptu, null, Hatırlatıcı);
                        }

                        if (yeniden_başlat) Durdur();
                        Thread.Sleep(bekleme);
                    }
                }
            }

            Durdur(true);

            GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
        }
        public void Durdur(bool TamamenDurdur = false)
        {
            if (TamamenDurdur)
            {
                Çalışşsın = false;
                TekrarDeneme_ZamanAşımı_msn = 1;
            }

            try { if (Alıcı != null) { Alıcı.Close(); Alıcı.Dispose(); } } catch (Exception) { }
            Alıcı = null;
            try { if (Verici != null) { Verici.Close(); Verici.Dispose(); } } catch (Exception) { }
            Verici = null;
            try { if (Aracı != null) { Aracı.Close(); Aracı.Dispose(); } } catch (Exception) { }
            Aracı = null;
            try { if (İstemci != null) { İstemci.Close(); İstemci.Dispose(); } } catch (Exception) { }
            İstemci = null;
        }

        #region IDonanımHaberlleşmesi
        bool IDonanımHaberleşmesi.BağlantıKurulduMu()
        {
            return İstemci == null ? false : İstemci.Connected;
        }
        void IDonanımHaberleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            if (İstemci == null || !İstemci.Connected || Aracı == null) throw new Exception("Bağlantı Kurulmadı");

            Aracı.Write(Bilgi, 0, Bilgi.Length);
            Aracı.Flush();

            if (Sessizlik_ZamanAşımı_msn > 0) Sessizlik_ZamanAşımı_Anı = Environment.TickCount + Sessizlik_ZamanAşımı_msn;
        }
        void IDonanımHaberleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            if (İstemci == null || !İstemci.Connected || Verici == null) throw new Exception("Bağlantı Kurulmadı");

            Verici.Write(Bilgi + SatırSonu.Karakteri);
            Verici.Flush();

            if (Sessizlik_ZamanAşımı_msn > 0) Sessizlik_ZamanAşımı_Anı = Environment.TickCount + Sessizlik_ZamanAşımı_msn;
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                    Durdur(true);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~Tcpİstemci_()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
    public class Tcpİstemci_Güvenli_ : IDisposable, IDonanımHaberleşmesi
    {
        public const string Sürüm = "V1.0";

        #region Genel Görüşe Açık
        public object Hatırlatıcı { get => Tcpİstemci == null ? null : Tcpİstemci.Hatırlatıcı; }
        public readonly bool SatırSatırGönderVeAl = false;
        public int TekrarDeneme_ZamanAşımı_msn { get => Tcpİstemci == null ? 5000 : Tcpİstemci.TekrarDeneme_ZamanAşımı_msn; }
        public int BilgiGönderme_ZamanAşımı_msn { get => Tcpİstemci == null ? 15000 : Tcpİstemci.BilgiGönderme_ZamanAşımı_msn; }
        #endregion

        #region İç Kullanım
        const string ElŞıkışma = "Ne Mutlu TÜRK'üm Diyene";
        int GüvenliBağlantıKuruldu = 0;
        Tcpİstemci_ Tcpİstemci = null; IDonanımHaberleşmesi DonanımHaberleşmesi = null;
        byte[] Simetrik_Karmaşıklaştırma_Parola_Giden = null, Simetrik_Karmaşıklaştırma_Parola_Gelen = null;
        DahaCokKarmasiklastirma_ Simetrik_Karmaşıklaştırma = new DahaCokKarmasiklastirma_();
        DahaCokKarmasiklastirma_Asimetrik_ Asimetrik_Karmaşıklaştırma_Giden = null, Asimetrik_Karmaşıklaştırma_Gelen = null;
        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;
        #endregion

        public Tcpİstemci_Güvenli_(int ErişimNoktası, string IpVeyaAdı = "127.0.0.1", GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = false, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, int Sessizlik_ZamanAşımı_msn = 0, int AksayanBilgiAlımı_ZamanAşımı_msn = 5)
        {
            if (SatırSatırGönderVeAl) throw new Exception("Güvenli && SatırSatırGönderVeAl kullanılamaz");
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;

            Tcpİstemci = new Tcpİstemci_(ErişimNoktası, IpVeyaAdı, Güvenli_GeriBildirim_Islemi, Hatırlatıcı, false, TekrarDeneme_ZamanAşımı_msn, BilgiGönderme_ZamanAşımı_msn = 15000, Sessizlik_ZamanAşımı_msn, AksayanBilgiAlımı_ZamanAşımı_msn);
            DonanımHaberleşmesi = Tcpİstemci;
        }
        public Tcpİstemci_Güvenli_(TcpClient İstemci, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = false, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, int Sessizlik_ZamanAşımı_msn = 0, int AksayanBilgiAlımı_ZamanAşımı_msn = 5)
        {
            if (SatırSatırGönderVeAl) throw new Exception("Güvenli && SatırSatırGönderVeAl kullanılamaz");
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;

            Tcpİstemci = new Tcpİstemci_(İstemci, Güvenli_GeriBildirim_Islemi, Hatırlatıcı, false, TekrarDeneme_ZamanAşımı_msn, BilgiGönderme_ZamanAşımı_msn, Sessizlik_ZamanAşımı_msn, AksayanBilgiAlımı_ZamanAşımı_msn);
            DonanımHaberleşmesi = Tcpİstemci;
        }

        /*
        A -> B  A|asimetrik açık şifre
        B -> A  B|asimetrik ile şifrelenmiş - simetrik olarak a dan b ye gönderme parolası
        A -> B  C|simetrik ile şifrelenmiş - El şıkışma
        */
        void Güvenli_GeriBildirim_Islemi(string Kaynak, GeriBildirim_Türü_ Tür, object İçerik, object Hatırlatıcı)
        {
            switch (Tür)
            {
                case GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek:
                    GeriBildirim_Islemi?.Invoke(Kaynak, Tür, İçerik, Hatırlatıcı);
                    break;

                case GeriBildirim_Türü_.BağlantıKuruldu:
                    Asimetrik_Karmaşıklaştırma_Giden = new DahaCokKarmasiklastirma_Asimetrik_();
                    DonanımHaberleşmesi.Gönder(Dönüştürme.D_Yazı.BaytDizisine("A|" + Asimetrik_Karmaşıklaştırma_Giden.AçıkAnahtar));
                    break;

                case GeriBildirim_Türü_.BilgiGeldi:
                    if (GüvenliBağlantıKuruldu == 2) GeriBildirim_Islemi?.Invoke(Kaynak, Tür, Simetrik_Karmaşıklaştırma.Düzelt((byte[])İçerik, Simetrik_Karmaşıklaştırma_Parola_Gelen), Hatırlatıcı);
                    else
                    {
                        string gelen = Dönüştürme.D_Yazı.BaytDizisinden((byte[])İçerik);
                        if (gelen.StartsWith("A|"))
                        {
                            Asimetrik_Karmaşıklaştırma_Gelen = new DahaCokKarmasiklastirma_Asimetrik_(gelen.Substring(2));
                            Simetrik_Karmaşıklaştırma_Parola_Gelen = Rastgele.BaytDizisi(64);
                            DonanımHaberleşmesi.Gönder(Dönüştürme.D_Yazı.BaytDizisine("B|" + Dönüştürme.D_BaytDizisi.Taban64e(Asimetrik_Karmaşıklaştırma_Gelen.Karıştır(Simetrik_Karmaşıklaştırma_Parola_Gelen))));

                            Asimetrik_Karmaşıklaştırma_Gelen.Dispose();
                            Asimetrik_Karmaşıklaştırma_Gelen = null;
                        }
                        else if (gelen.StartsWith("B|"))
                        {
                            Simetrik_Karmaşıklaştırma_Parola_Giden = Asimetrik_Karmaşıklaştırma_Giden.Düzelt(Dönüştürme.D_Yazı.Taban64ten(gelen.Substring(2)));
                            DonanımHaberleşmesi.Gönder(Dönüştürme.D_Yazı.BaytDizisine("C|" + Dönüştürme.D_BaytDizisi.Taban64e(Simetrik_Karmaşıklaştırma.Karıştır(Dönüştürme.D_Yazı.BaytDizisine(ElŞıkışma), Simetrik_Karmaşıklaştırma_Parola_Giden))));

                            Asimetrik_Karmaşıklaştırma_Giden.Dispose();
                            Asimetrik_Karmaşıklaştırma_Giden = null;
                            Interlocked.Increment(ref GüvenliBağlantıKuruldu);
                        }
                        else if (gelen.StartsWith("C|"))
                        {
                            if (Dönüştürme.D_Yazı.BaytDizisinden(Simetrik_Karmaşıklaştırma.Düzelt(Dönüştürme.D_Yazı.Taban64ten(gelen.Substring(2)), Simetrik_Karmaşıklaştırma_Parola_Gelen)) == ElŞıkışma)
                            {
                                if (Interlocked.Increment(ref GüvenliBağlantıKuruldu) == 2) GeriBildirim_Islemi?.Invoke(Kaynak, GeriBildirim_Türü_.BağlantıKuruldu, null, Hatırlatıcı);
                            }
                            else Durdur();
                        }
                        else Durdur();
                    }
                    break;

                case GeriBildirim_Türü_.BağlantıKoptu:
                case GeriBildirim_Türü_.Durduruldu:
                    Durdur();
                    GeriBildirim_Islemi?.Invoke(Kaynak, Tür, İçerik, Hatırlatıcı);
                    break;
            }
        }

        void Durdur(bool TamamenDurdur = false)
        {
            if (TamamenDurdur)
            {
                Tcpİstemci?.Dispose();
                Tcpİstemci = null;
                DonanımHaberleşmesi = null;
                Simetrik_Karmaşıklaştırma.Dispose();
            }
            else Tcpİstemci?.Durdur();

            Asimetrik_Karmaşıklaştırma_Giden?.Dispose(); Asimetrik_Karmaşıklaştırma_Giden = null;
            Asimetrik_Karmaşıklaştırma_Gelen?.Dispose(); Asimetrik_Karmaşıklaştırma_Gelen = null;
            Simetrik_Karmaşıklaştırma_Parola_Giden = null;
            Simetrik_Karmaşıklaştırma_Parola_Gelen = null;
            Interlocked.Exchange(ref GüvenliBağlantıKuruldu, 0);
        }

        #region IDonanımHaberlleşmesi
        bool IDonanımHaberleşmesi.BağlantıKurulduMu()
        {
            return DonanımHaberleşmesi == null ? false : DonanımHaberleşmesi.BağlantıKurulduMu();

            //Güvenli bağlantı kurulmasını değil, fiziksel bağlantı kurulmasını belirtiyor
            //Güvenli bağlantı için GeriBildirim_Türü_.BağlantıKuruldu beklenmeli
        }
        void IDonanımHaberleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            if (GüvenliBağlantıKuruldu != 2) throw new Exception("Bağlantı Kurulmadı");

            DonanımHaberleşmesi.Gönder(Simetrik_Karmaşıklaştırma.Karıştır(Bilgi, Simetrik_Karmaşıklaştırma_Parola_Giden), Alıcı);
        }
        void IDonanımHaberleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            throw new Exception("Güvenli && SatırSatırGönderVeAl kullanılamaz");
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)

                    Durdur(true);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~Tcpİstemci_Güvenli_()
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
    public class TcpSunucu_ : IDisposable, IDonanımHaberleşmesi
    {
        public const string Sürüm = "V1.2";

        #region Genel Görüşe Açık
        public object Hatırlatıcı = null;
        public bool SatırSatırGönderVeAl = true;
        public int TekrarDeneme_ZamanAşımı_msn = 5000;
        public int BilgiGönderme_ZamanAşımı_msn = 15000;
        #endregion

        #region İç Kullanım
        int ErişimNoktası, Sessizlik_ZamanAşımı_msn;
        bool Çalışşsın = true;
        bool SadeceYerel = true;
        bool Güvenli = false;

        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

        TcpListener Sunucu = null;
        Dictionary<string, IDonanımHaberleşmesi> İstemciler = new Dictionary<string, IDonanımHaberleşmesi>();
        #endregion

        public TcpSunucu_(int ErişimNoktası, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, bool SadeceYerel = true, int Sessizlik_ZamanAşımı_msn = 0, bool Güvenli = false)
        {
            this.ErişimNoktası = ErişimNoktası;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;
            this.SadeceYerel = SadeceYerel;
            this.Sessizlik_ZamanAşımı_msn = Sessizlik_ZamanAşımı_msn;

            this.Güvenli = Güvenli;
            if (Güvenli && SatırSatırGönderVeAl) throw new Exception("Güvenli && SatırSatırGönderVeAl kullanılamaz");

            new Thread(() => Görev_İşlemi_TcpSunucu()).Start();
        }
        public int EtkinErişimNoktası()
        {
            if (Sunucu == null) return 0;

            return ((IPEndPoint)Sunucu.LocalEndpoint).Port;
        }
        public void Durdur(string Alıcı)
        {
            if (string.IsNullOrEmpty(Alıcı))
            {
                foreach (IDonanımHaberleşmesi istemci in İstemciler.Values)
                {
                    istemci.Durdur();
                }
            }
            else if (İstemciler.ContainsKey(Alıcı)) İstemciler[Alıcı].Durdur();
        }

        void Görev_İşlemi_TcpSunucu()
        {
            while (Çalışşsın)
            {
                try
                {
                    if (Sunucu == null)
                    {
                        Sunucu = new TcpListener(SadeceYerel ? IPAddress.Loopback : IPAddress.Any, ErişimNoktası);
                        Sunucu.Start();
                    }

                    TcpClient gelen = Sunucu.AcceptTcpClient();
                    if (Güvenli)
                    {
                        Tcpİstemci_Güvenli_ Tcpİstemci = new Tcpİstemci_Güvenli_(gelen, GeriBildirim_Islemi, Hatırlatıcı, SatırSatırGönderVeAl, TekrarDeneme_ZamanAşımı_msn, BilgiGönderme_ZamanAşımı_msn, Sessizlik_ZamanAşımı_msn);
                        İstemciler.Add(gelen.Client.RemoteEndPoint.ToString(), Tcpİstemci);
                    }
                    else
                    {
                        Tcpİstemci_ Tcpİstemci = new Tcpİstemci_(gelen, GeriBildirim_Islemi, Hatırlatıcı, SatırSatırGönderVeAl, TekrarDeneme_ZamanAşımı_msn, BilgiGönderme_ZamanAşımı_msn, Sessizlik_ZamanAşımı_msn);
                        İstemciler.Add(gelen.Client.RemoteEndPoint.ToString(), Tcpİstemci);
                    }

                    if (!Sunucu.Pending())
                    {
                        KeyValuePair<string, IDonanımHaberleşmesi>[] BağlantısıKopanlar = İstemciler.Where(x => !x.Value.BağlantıKurulduMu()).ToArray();
                        foreach (var biri in BağlantısıKopanlar)
                        {
                            biri.Value.Durdur();
                            İstemciler.Remove(biri.Key);
                        }
                    }
                }
                catch (Exception)
                {
                    Durdur();

                    if (Çalışşsın) Thread.Sleep(1000);
                }
            }

            Durdur(true);

            GeriBildirim_Islemi?.Invoke(ErişimNoktası.ToString(), GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
        }
        void Durdur(bool TamamenDurdur = false)
        {
            if (TamamenDurdur)
            {
                Çalışşsın = false;

                foreach (var biri in İstemciler.Values)
                {
                    try { biri.Durdur(); } catch (Exception) { }
                }
            }

            try { if (Sunucu != null) {  Sunucu.Server.Close(); Sunucu.Server.Dispose(); Sunucu.Stop(); } } catch (Exception) { }
            Sunucu = null;
        }
        void Gönder(byte[] Bilgi, string Alıcı)
        {
            if (!İstemciler.ContainsKey(Alıcı)) throw new Exception("Alıcı listede bulunmuyor");

            try
            {
                İstemciler[Alıcı].Gönder(Bilgi);
            }
            catch (Exception)
            {
                //bağlantı sıkıntılı sil
                İstemciler[Alıcı].Durdur();

                throw new Exception("Alıcı ile bağlantı koptuğu için gönderilemedi");
            }
        }
        void Gönder(string Bilgi, string Alıcı)
        {
            if (!İstemciler.ContainsKey(Alıcı)) throw new Exception("Alıcı listede bulunmuyor");

            try
            {
                İstemciler[Alıcı].Gönder(Bilgi);
            }
            catch (Exception)
            {
                //bağlantı sıkıntılı sil
                İstemciler[Alıcı].Durdur();

                throw new Exception("Alıcı ile bağlantı koptuğu için gönderilemedi");
            }
        }
        
        #region IDonanımHaberlleşmesi
        bool IDonanımHaberleşmesi.BağlantıKurulduMu()
        {
            return İstemciler.Count > 0;
        }
        void IDonanımHaberleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            if (string.IsNullOrEmpty(Alıcı))
            {
                if (İstemciler.Count == 0) throw new Exception("Henüz hiçbir bağlantı kurulu olmadığı için gönderilemedi");

                //tümüne gönder
                foreach (var biri in İstemciler.Keys)
                {
                    Gönder(Bilgi, biri);
                }
            }
            else
            {
                Gönder(Bilgi, Alıcı);
            }
        }
        void IDonanımHaberleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            if (string.IsNullOrEmpty(Alıcı))
            {
                if (İstemciler.Count == 0) throw new Exception("Henüz hiçbir bağlantı kurulu olmadığı için gönderilemedi");

                //tümüne gönder
                foreach (var biri in İstemciler.Keys)
                {
                    Gönder(Bilgi, biri);
                }
            }
            else
            {
                Gönder(Bilgi, Alıcı);
            }
        }
        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                    Durdur(true);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~TcpSunucu_()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
