// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Drawing;
using System.Windows.Forms;

namespace ArgeMup.HazirKod
{
    public class DurumBildirimi_ : IDisposable
    {
        public const string Sürüm = "V1.4";
        #region Değişkenler
        NotifyIcon Uyarıİkonu = null;
        ToolTip ipucu = null;
        Form Pencere_Ekranı = null;
        System.Timers.Timer Zamanlayıcı = null;
        int Süre_Uyarıİkonu, Süre_ipucu; 
        #endregion

        public DurumBildirimi_(Form Pencere = null)
        {
            Pencere_Ekranı = Pencere;
        }
        public void BaloncukluUyarı(string Mesaj, IWin32Window Kutucuk = null, ToolTipIcon İkon = ToolTipIcon.Warning, int ZamanAşımı = 3000)
        {
            try
            {
                if (Zamanlayıcı == null)
                {
                    Zamanlayıcı = new System.Timers.Timer();
                    Zamanlayıcı.Elapsed += new System.Timers.ElapsedEventHandler(ZamanlayıcıKesmesi);
                    Zamanlayıcı.Interval = 1000;
                    Zamanlayıcı.Enabled = true;
                }

                if (Kutucuk == null)
                {
                    if (Uyarıİkonu == null) Uyarıİkonu = new NotifyIcon();
                    else if ((string)Uyarıİkonu.Tag == Mesaj) return;
                    else { Uyarıİkonu.Dispose(); Uyarıİkonu = new NotifyIcon(); }

                    Uyarıİkonu.Visible = true;
                    if (Pencere_Ekranı != null)
                    {
                        Uyarıİkonu.Icon = Pencere_Ekranı.Icon;
                        Uyarıİkonu.Text = Pencere_Ekranı.Text;
                        Uyarıİkonu.ShowBalloonTip(ZamanAşımı, Pencere_Ekranı.Text, Mesaj, İkon);
                    }
                    else
                    {
                        Uyarıİkonu.Icon = SystemIcons.Warning;
                        Uyarıİkonu.Text = "Lütfen dikkate alınız.";
                        Uyarıİkonu.ShowBalloonTip(ZamanAşımı, "Lütfen dikkate alınız.", Mesaj, İkon);
                    }
                    Uyarıİkonu.Tag = Mesaj;

                    Süre_Uyarıİkonu = Environment.TickCount + (ZamanAşımı + 6000);
                }
                else
                {
                Devam_ipucu:
                    if (ipucu == null)
                    {
                        ipucu = new ToolTip();
                        ipucu.IsBalloon = true;
                        ipucu.ShowAlways = false;
                        ipucu.AutomaticDelay = 5000;
                    }
                    else { ipucu.Dispose(); ipucu = null; goto Devam_ipucu; }

                    ipucu.ToolTipIcon = İkon;
                    if (Pencere_Ekranı != null) ipucu.ToolTipTitle = Pencere_Ekranı.Text;
                    else ipucu.ToolTipTitle = "Lütfen dikkate alınız.";
                    ipucu.Show(string.Empty, Kutucuk);
                    ipucu.Show(Mesaj, Kutucuk, ZamanAşımı);

                    Süre_ipucu = Environment.TickCount + (ZamanAşımı * 2);
                }
                Zamanlayıcı.Start();
            }
            catch (Exception) { }
        }
        private void ZamanlayıcıKesmesi(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (Uyarıİkonu != null) if (Environment.TickCount > Süre_Uyarıİkonu) { Uyarıİkonu.Dispose(); Uyarıİkonu = null; }

                if (ipucu != null) if (Environment.TickCount > Süre_ipucu) { ipucu.Dispose(); ipucu = null; }

                if (Uyarıİkonu == null && ipucu == null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
            }
            catch (Exception) { }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).

                    if (Uyarıİkonu != null) { Uyarıİkonu.Dispose(); Uyarıİkonu = null; }
                    if (ipucu != null) { ipucu.Dispose(); ipucu = null; }
                    if (Zamanlayıcı != null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //~DurumBildirimi_()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

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

