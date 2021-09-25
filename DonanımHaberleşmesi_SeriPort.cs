// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.IO.Ports;
using System.Threading;

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public class SeriPort_ : IDisposable, IDonanımHaberlleşmesi
    {
        public const string Sürüm = "V1.0";

        #region Genel Görüşe Açık
        public object Hatırlatıcı = null;
        public bool SatırSatırGönderVeAl = true;
        public int TekrarDeneme_ZamanAşımı_msn = 5000;
        public int BilgiGönderme_ZamanAşımı_msn = 15000;
        #endregion

        #region İç Kullanım
        string ErişimNoktası = "";
        int BitHızı = 0;
        bool Çalışşsın = true;

        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

        SerialPort SeriPort = null;
        #endregion
        
        public SeriPort_(string ErişimNoktası, int BitHızı, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000)
        {
            this.ErişimNoktası = ErişimNoktası;
            this.BitHızı = BitHızı;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
            this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;

            new Thread(() => Görev_İşlemi_SeriPort()).Start();
        }
        void Görev_İşlemi_SeriPort()
        {
            while (Çalışşsın)
            {
                try
                {
                    if (SeriPort == null || !SeriPort.IsOpen)
                    {
                        SeriPort = new SerialPort(ErişimNoktası, BitHızı, Parity.None, 8, StopBits.One);
                        SeriPort.Open();
                        SeriPort.DiscardOutBuffer();
                        SeriPort.DiscardInBuffer();
                        SeriPort.ReadTimeout = TekrarDeneme_ZamanAşımı_msn;
                        SeriPort.WriteTimeout = BilgiGönderme_ZamanAşımı_msn;

                        GeriBildirim_Islemi?.Invoke(ErişimNoktası, GeriBildirim_Türü_.BağlantıKuruldu, null, Hatırlatıcı);
                    }

                    int sayac = 0;
                    while (Çalışşsın && SeriPort.IsOpen)
                    {
                        object çıktı = null;
                        if (SatırSatırGönderVeAl) çıktı = SeriPort.ReadLine();
                        else
                        {
                            int ilk_gelen_bilgi = SeriPort.ReadByte();
                            if (ilk_gelen_bilgi >= 0)
                            {
                                int adet_bekleyen = SeriPort.BytesToRead;

                                byte[] tampon_GelenBilgi = new byte[adet_bekleyen + 1];
                                tampon_GelenBilgi[0] = (byte)ilk_gelen_bilgi;

                                int adet_okunan = SeriPort.Read(tampon_GelenBilgi, 1, adet_bekleyen);
                                if (adet_okunan != adet_bekleyen) Array.Resize(ref tampon_GelenBilgi, adet_okunan + 1);
                                çıktı = tampon_GelenBilgi;
                            }
                        }

                        if (çıktı == null) throw new Exception();
                        else
                        {
                            GeriBildirim_Islemi?.Invoke(ErişimNoktası, GeriBildirim_Türü_.BilgiGeldi, çıktı, Hatırlatıcı);

                            if (sayac++ > 100) { sayac = 0; Thread.Sleep(1); } //cpu yüzdesini düşürmek için
                        }
                    }
                }
                catch (TimeoutException) { Thread.Sleep(1); } //cpu yüzdesini düşürmek için
                catch (Exception) 
                {
                    GeriBildirim_Islemi?.Invoke(ErişimNoktası, GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek, null, Hatırlatıcı);

                    Durdur();
                    Thread.Sleep(TekrarDeneme_ZamanAşımı_msn); 
                }
            }

            Durdur(true);

            GeriBildirim_Islemi?.Invoke(ErişimNoktası, GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
        }
        void Durdur(bool TamamenDurdur = false)
        {
            if (TamamenDurdur)
            {
                Çalışşsın = false;
                TekrarDeneme_ZamanAşımı_msn = 1;
            }

            try { if (SeriPort != null) { SeriPort.Close(); SeriPort.Dispose(); } } catch (Exception) { }
            SeriPort = null;

            GeriBildirim_Islemi?.Invoke(ErişimNoktası, GeriBildirim_Türü_.BağlantıKoptu, null, Hatırlatıcı);
        }

        #region IDonanımHaberlleşmesi
        bool IDonanımHaberlleşmesi.BağlantıKurulduMu()
        {
            return SeriPort == null ? false : SeriPort.IsOpen;
        }
        void IDonanımHaberlleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberlleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            if (SeriPort == null || !SeriPort.IsOpen) throw new Exception("Bağlantı Kurulmadı");

            SeriPort.Write(Bilgi, 0, Bilgi.Length);
        }
        void IDonanımHaberlleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            if (SeriPort == null || !SeriPort.IsOpen) throw new Exception("Bağlantı Kurulmadı");

            SeriPort.WriteLine(Bilgi);
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
        ~SeriPort_()
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
