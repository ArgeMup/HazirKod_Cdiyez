// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public class Tcpİstemci_ : IDisposable, IDonanımHaberlleşmesi
    {
        public const string Sürüm = "V1.2";

        #region Genel Görüşe Açık
        public object Hatırlatıcı = null;
        public bool SatırSatırGönderVeAl = true;
        public int TekrarDeneme_ZamanAşımı_msn = 5000;
        public int BilgiGönderme_ZamanAşımı_msn = 15000;
        #endregion

        #region İç Kullanım
        int ErişimNoktası = -1;
        string IpVeyaAdı = "";
        bool Çalışşsın = true;

        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

        TcpClient İstemci = null;
        NetworkStream Aracı = null;
        StreamReader Alıcı = null;
        StreamWriter Verici = null;
        #endregion
        
        public Tcpİstemci_(int ErişimNoktası, string IpVeyaAdı = "127.0.0.1", GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000)
        {
            if (ErişimNoktası < 0) throw new Exception("ErişimNoktası > 0 olmalı");

            this.ErişimNoktası = ErişimNoktası;
            this.IpVeyaAdı = IpVeyaAdı;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;

            new Thread(() => Görev_İşlemi_Tcpİstemci()).Start();
        }
        public Tcpİstemci_(TcpClient İstemci, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000)
        {
            this.İstemci = İstemci;
            this.IpVeyaAdı = İstemci.Client.RemoteEndPoint.ToString();
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;

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
                                int adet_bekleyen = İstemci.Available;

                                byte[] tampon_GelenBilgi = new byte[adet_bekleyen + 1];
                                tampon_GelenBilgi[0] = (byte)ilk_gelen_bilgi;

                                int adet_okunan = Aracı.Read(tampon_GelenBilgi, 1, adet_bekleyen);
                                if (adet_okunan != adet_bekleyen) Array.Resize(ref tampon_GelenBilgi, adet_okunan + 1);
                                çıktı = tampon_GelenBilgi;
                            }
                        }

                        if (çıktı == null) throw new Exception();
                        else
                        {
                            GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.BilgiGeldi, çıktı, Hatırlatıcı);

                            if (sayac++ > 100) { sayac = 0; Thread.Sleep(1); } //cpu yüzdesini düşürmek için
                        }
                    }
                }
                catch (Exception ex)
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

            Durdur(true);

            GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
        }
        void Durdur(bool TamamenDurdur = false)
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
        bool IDonanımHaberlleşmesi.BağlantıKurulduMu()
        {
            return (İstemci == null || Aracı == null) ? false : İstemci.Connected;
        }
        void IDonanımHaberlleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberlleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            if (İstemci == null || !İstemci.Connected || Aracı == null) throw new Exception("Bağlantı Kurulmadı");

            Aracı.Write(Bilgi, 0, Bilgi.Length);
            Aracı.Flush();
        }
        void IDonanımHaberlleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            if (İstemci == null || !İstemci.Connected || Verici == null) throw new Exception("Bağlantı Kurulmadı");

            Verici.Write(Bilgi + SatırSonu.Karakteri);
            Verici.Flush();
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

    public class TcpSunucu_ : IDisposable, IDonanımHaberlleşmesi
    {
        public const string Sürüm = "V1.1";

        #region Genel Görüşe Açık
        public object Hatırlatıcı = null;
        public bool SatırSatırGönderVeAl = true;
        public int TekrarDeneme_ZamanAşımı_msn = 5000;
        public int BilgiGönderme_ZamanAşımı_msn = 15000;
        #endregion

        #region İç Kullanım
        int ErişimNoktası;
        bool Çalışşsın = true;
        bool SadeceYerel = true;

        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

        TcpListener Sunucu = null;
        Dictionary<string, IDonanımHaberlleşmesi> İstemciler = new Dictionary<string, IDonanımHaberlleşmesi>();
        #endregion

        public TcpSunucu_(int ErişimNoktası, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, bool SadeceYerel = true)
        {
            this.ErişimNoktası = ErişimNoktası;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;
            this.SadeceYerel = SadeceYerel;

            new Thread(() => Görev_İşlemi_TcpSunucu()).Start();
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

                    if (Sunucu.Pending())
                    {
                        TcpClient gelen = Sunucu.AcceptTcpClient();
                        Tcpİstemci_ Tcpİstemci = new Tcpİstemci_(gelen, GeriBildirim_Islemi, Hatırlatıcı, SatırSatırGönderVeAl, TekrarDeneme_ZamanAşımı_msn, BilgiGönderme_ZamanAşımı_msn);

                        İstemciler.Add(gelen.Client.RemoteEndPoint.ToString(), Tcpİstemci);
                    }
                    else
                    {
                        Thread.Sleep(TekrarDeneme_ZamanAşımı_msn);

                        GeriBildirim_Islemi?.Invoke(ErişimNoktası.ToString(), GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek, null, Hatırlatıcı);

                        YenidenDene:
                        foreach (var biri in İstemciler)
                        {
                            if (!biri.Value.BağlantıKurulduMu())
                            {
                                biri.Value.Durdur();
                                İstemciler.Remove(biri.Key);
                                goto YenidenDene;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    Durdur();

                    Thread.Sleep(TekrarDeneme_ZamanAşımı_msn);
                }
            }

            Durdur(true);

            GeriBildirim_Islemi?.Invoke(ErişimNoktası.ToString(), GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
        }
        void Durdur(bool TamamenDurdur = false)
        {
            if (TamamenDurdur)
            {
                TekrarDeneme_ZamanAşımı_msn = 1;
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
        public int EtkinErişimNoktası()
        {
            if (Sunucu == null) return 0;

            return ((IPEndPoint)Sunucu.LocalEndpoint).Port;
        }

        #region IDonanımHaberlleşmesi
        bool IDonanımHaberlleşmesi.BağlantıKurulduMu()
        {
            return İstemciler.Count > 0;
        }
        void IDonanımHaberlleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberlleşmesi.Gönder(byte[] Bilgi, string Alıcı)
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
        void IDonanımHaberlleşmesi.Gönder(string Bilgi, string Alıcı)
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
