// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Threading;

namespace ArgeMup.HazirKod.DonanımHaberleşmesi
{
    public class KomutSatırıUygulaması_ : IDisposable, IDonanımHaberlleşmesi
    {
        public const string Sürüm = "V1.0";

        #region Genel Görüşe Açık
        public object Hatırlatıcı = null;
        public bool SatırSatırGönderVeAl = true;
        public int TekrarDeneme_ZamanAşımı_msn = 5000;
        #endregion

        #region İç Kullanım
        string DosyaYolu = "";
        string Parametreler = "";
        bool Çalışşsın = true;

        GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

        Process Uygulama = null;
        string UzunAdı = "";
        #endregion

        public KomutSatırıUygulaması_(string DosyaYolu, string Parametreler = "", GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000)
        {
            this.DosyaYolu = DosyaYolu;
            this.Parametreler = Parametreler;
            this.GeriBildirim_Islemi = GeriBildirim_Islemi;
            this.Hatırlatıcı = Hatırlatıcı;
            this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
            this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;

            UzunAdı = (DosyaYolu + " " + Parametreler).Trim();

            new Thread(() => Görev_İşlemi_KomutSatırıUygulaması()).Start();
        }
        void Görev_İşlemi_KomutSatırıUygulaması()
        {
            while (Çalışşsın)
            {
                try
                {
                    if (Uygulama == null || Uygulama.HasExited)
                    {
                        Uygulama = new Process();
                        Uygulama.StartInfo.FileName = DosyaYolu;
                        Uygulama.StartInfo.Arguments = Parametreler;

                        Uygulama.StartInfo.CreateNoWindow = true;
                        Uygulama.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Uygulama.StartInfo.UseShellExecute = false;
                        Uygulama.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage); //veya Encoding.UTF8; veya Encoding.GetEncoding(1254) veya 857
                        Uygulama.StartInfo.StandardErrorEncoding = Encoding.GetEncoding(CultureInfo.CurrentCulture.TextInfo.OEMCodePage); //diğer uygulamada da Console.OutputEncoding ve Console.InputEncoding için de yazılması gerekebiliyor

                        Uygulama.StartInfo.RedirectStandardError = true;
                        Uygulama.StartInfo.RedirectStandardOutput = true;
                        Uygulama.StartInfo.RedirectStandardInput = true;
                        Uygulama.ErrorDataReceived += Uygulama_OutputDataReceived;
                        Uygulama.Exited += Uygulama_Exited;
                        Uygulama.OutputDataReceived += Uygulama_OutputDataReceived;
                        Uygulama.EnableRaisingEvents = true;

                        Uygulama.Start();
                        Uygulama.BeginOutputReadLine();
                        Uygulama.BeginErrorReadLine();

                        GeriBildirim_Islemi?.Invoke(UzunAdı, GeriBildirim_Türü_.BağlantıKuruldu, null, Hatırlatıcı);
                    }

                    return;
                }
                catch (Exception)
                {
                    Durdur();
                    
                    GeriBildirim_Islemi?.Invoke(UzunAdı, GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek, null, Hatırlatıcı);
                    
                    Thread.Sleep(TekrarDeneme_ZamanAşımı_msn);
                }
            }

            Durdur(true);

            GeriBildirim_Islemi?.Invoke(UzunAdı, GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
        }
        private void Uygulama_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            object çıktı;
            if (SatırSatırGönderVeAl) çıktı = e.Data;
            else çıktı = Dönüştürme.D_Metin.BaytDizisine(e.Data);

            if (çıktı == null) return;

            GeriBildirim_Islemi?.Invoke(UzunAdı, GeriBildirim_Türü_.BilgiGeldi, çıktı, Hatırlatıcı);
        }
        private void Uygulama_Exited(object sender, EventArgs e)
        {
            GeriBildirim_Islemi?.Invoke(UzunAdı, GeriBildirim_Türü_.BağlantıKoptu, null, Hatırlatıcı);
            new Thread(() => Görev_İşlemi_KomutSatırıUygulaması()).Start();
        }
        
        void Durdur(bool TamamenDurdur = false)
        {
            if (TamamenDurdur)
            {
                Çalışşsın = false;
                TekrarDeneme_ZamanAşımı_msn = 1;
            }

            try { if (Uygulama != null) { Uygulama.Kill(); } } catch (Exception) { }
            Uygulama = null;
        }

        #region IDonanımHaberlleşmesi
        bool IDonanımHaberlleşmesi.BağlantıKurulduMu()
        {
            return Uygulama == null ? false : !Uygulama.HasExited;
        }
        void IDonanımHaberlleşmesi.Durdur()
        {
            Durdur(true);
        }
        void IDonanımHaberlleşmesi.Gönder(byte[] Bilgi, string Alıcı)
        {
            if (Uygulama == null || Uygulama.HasExited) throw new Exception("Bağlantı Kurulmadı");

            Uygulama.StandardInput.Write(Bilgi);
            Uygulama.StandardInput.Flush();
        }
        void IDonanımHaberlleşmesi.Gönder(string Bilgi, string Alıcı)
        {
            if (Uygulama == null || Uygulama.HasExited) throw new Exception("Bağlantı Kurulmadı");

            Uygulama.StandardInput.WriteLine(Bilgi);
            Uygulama.StandardInput.Flush();
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
        ~KomutSatırıUygulaması_()
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
