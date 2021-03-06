﻿// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public class Udp_Dinleyici_ : IDisposable, IDonanımHaberlleşmesi
    {
        public const string Sürüm = "V1.0";

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

        UdpClient Alıcı = null;
        #endregion
        
        public Udp_Dinleyici_(int ErişimNoktası, string IpVeyaAdı = "127.0.0.1", GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000)
        {
            if (ErişimNoktası < 0) return; //Sadece verici olarak çalışılacak

            this.ErişimNoktası = ErişimNoktası;
            this.IpVeyaAdı = IpVeyaAdı;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;

            new Thread(() => Görev_İşlemi_Udp_Dinleyici()).Start();
        }
        void Görev_İşlemi_Udp_Dinleyici()
        {
            int sayac = 0;

            while (Çalışşsın)
            {
                try
                {
                    if (Alıcı == null)
                    {
                        Alıcı = new UdpClient(ErişimNoktası);
                        Alıcı.Client.ReceiveTimeout = TekrarDeneme_ZamanAşımı_msn;
                    }

                    IPEndPoint RemoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
                    byte[] dizi = Alıcı.Receive(ref RemoteIpEndPoint);
                    if (dizi == null) throw new Exception();

                    object çıktı = null;
                    if (SatırSatırGönderVeAl) çıktı = Dönüştürme.D_Metin.BaytDizisinden(dizi).TrimEnd(' ', '\r', '\n');
                    else çıktı = dizi;

                    GeriBildirim_Islemi?.Invoke(RemoteIpEndPoint.ToString(), GeriBildirim_Türü_.BilgiGeldi, çıktı, Hatırlatıcı);

                    if (sayac++ > 100) { sayac = 0; Thread.Sleep(1); } //cpu yüzdesini düşürmek için
                }
                catch (Exception ex)
                {
                    if (ex.HResult == -2147467259)
                    {
                        //bağlantı isteği veya bilgi gelmediğinde geliyor
                        Thread.Sleep(1); //cpu yüzdesini düşürmek için
                    }
                    else
                    {
                        Thread.Sleep(TekrarDeneme_ZamanAşımı_msn);
                    }

                    GeriBildirim_Islemi?.Invoke(IpVeyaAdı, GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek, null, Hatırlatıcı);

                    Durdur();
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
        }
        void Gönder(byte[] Bilgi, string Alıcı)
        {
            if (string.IsNullOrEmpty(Alıcı)) throw new Exception("Alıcı boş olamaz");

            string[] dizi = Alıcı.Split(':');
            if (dizi.Length != 2) throw new Exception("Alıcı Adres:Port şeklinde olmalı");

            UdpClient Verici = new UdpClient(dizi[0], Convert.ToInt32(dizi[1]));
            Verici.Client.SendTimeout = BilgiGönderme_ZamanAşımı_msn;
            Verici.Send(Bilgi, Bilgi.Length);
        }

        #region IDonanımHaberlleşmesi
        bool IDonanımHaberlleşmesi.BağlantıKurulduMu()
        {
            throw new Exception("Udp için Kullanılamaz");
        }
        void IDonanımHaberlleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberlleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            Gönder(Bilgi, Alıcı);
        }
        void IDonanımHaberlleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            byte[] dizi = Dönüştürme.D_Metin.BaytDizisine(Bilgi);
            Gönder(dizi, Alıcı);
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
        ~Udp_Dinleyici_()
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
