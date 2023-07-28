// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.Diagnostics;

namespace ArgeMup.HazirKod
{
    public class Çalıştır_ : IDisposable
    {
        public const string Sürüm = "V1.1";
        EşZamanlıÇokluErişim.Liste_<Process> Tümü = new EşZamanlıÇokluErişim.Liste_<Process>();

        public void DosyaGezginindeGöster(string KlasörVeyaDosyaYolu)
        {
            Process Uygulama = new Process();
            Uygulama.StartInfo.UseShellExecute = false;
            Uygulama.StartInfo.FileName = "explorer.exe";
            Uygulama.StartInfo.Arguments = "/select, \"" + KlasörVeyaDosyaYolu + "\"";
            Uygulama.Start();

            //işlem hemen çıkıyor
        }
        public Process UygulamayıDoğrudanÇalıştır(string ExeDosyaYolu, string[] Girdiler = null, bool ÖnyüzüGizle = false, bool KapanırkenZorlaKapat = true)
        {
            Process Uygulama = new Process();
            Uygulama.StartInfo.UseShellExecute = false;
            Uygulama.StartInfo.FileName = "\"" + ExeDosyaYolu + "\"";
            if (Girdiler != null)
            {
                foreach (string Girdi in Girdiler)
                {
                    Uygulama.StartInfo.Arguments += "\"" + Girdi + "\" ";
                }
            }
            if (ÖnyüzüGizle)
            {
                Uygulama.StartInfo.CreateNoWindow = true;
                Uygulama.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            Uygulama.Start();

            if (KapanırkenZorlaKapat)
            {
                Tümü.Add(Uygulama);
                KontrolEt(false);
            }

            return Uygulama;
        }
        public Process UygulamayaİşletimSistemiKararVersin(string DosyaYolu, string[] Girdiler = null, bool ÖnyüzüGizle = false, bool KapanırkenZorlaKapat = true)
        {
            Process Uygulama = new Process();
            Uygulama.StartInfo.UseShellExecute = true;
            Uygulama.StartInfo.FileName = "\"" + DosyaYolu + "\"";
            if (Girdiler != null)
            {
                foreach (string Girdi in Girdiler)
                {
                    Uygulama.StartInfo.Arguments += "\"" + Girdi + "\" ";
                }
            }
            if (ÖnyüzüGizle)
            {
                Uygulama.StartInfo.CreateNoWindow = true;
                Uygulama.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            }
            Uygulama.Start();

            if (KapanırkenZorlaKapat)
            {
                Tümü.Add(Uygulama);
                KontrolEt(false);
            }

            return Uygulama;
        }
        public bool KontrolEt(bool TümünüDurdur = false)
        {
            System.Collections.Generic.List<Process> silinecekler = new System.Collections.Generic.List<Process>();

            foreach (Process u in Tümü)
            {
                try
                {
                    if (u.HasExited) silinecekler.Add(u);
                    else if (TümünüDurdur)
                    {
                        u.Kill();
                        silinecekler.Add(u);
                    }
                }
                catch (Exception)
                {
                    silinecekler.Add(u);
                }
            }

            foreach (var u in silinecekler)
            {
                Tümü.Remove(u);
            }

            //En az 1 tane çalışan var
            return Tümü.Count > 0;
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
                    KontrolEt(true);
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        //~Çalıştır_()
        //{
        //    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //    Dispose(disposing: false);
        //}

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}