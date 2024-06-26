﻿// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using ArgeMup.HazirKod.Dönüştürme;
using ArgeMup.HazirKod.Ekİşlemler;

namespace ArgeMup.HazirKod
{
    public class UygulamaOncedenCalistirildiMi_Basit : IDisposable
    {
        public const string Sürüm = "V1.1";
        static FileStream KilitDosyası = null;

        /// <summary>
        /// Kontrol eder.
        /// </summary>
        /// <param name="KilitDosyasiYolu">C:\\KilitDosyasi.mup gibi</param>
        /// <returns>true ise daha önce calıstırılmış, false ise ilk kez çalıştırıldı</returns>
        static public bool KontrolEt(string KilitDosyasiYolu = "")
        {
            try
            {
                if (KilitDosyası == null)
                {
                    if (string.IsNullOrEmpty(KilitDosyasiYolu))
                    {
                        KilitDosyasiYolu = Kendi.DosyaYolu + ".Kilit.mup";
                    }

                    KilitDosyası = new FileStream(KilitDosyasiYolu, FileMode.OpenOrCreate, FileAccess.Read, FileShare.None);
                }

                return false;
            }
            catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

            return true;
        }
        static public void Durdur()
        {
            if (KilitDosyası != null)
            {
                KilitDosyası.Close();
                File.Delete(KilitDosyası.Name);
            }
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

                    Durdur();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UygulamaOncedenCalistirildiMi_() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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

    public class UygulamaOncedenCalistirildiMi_ : IDisposable
    {
        public const string Sürüm = "V1.4";
        Mutex OrtakNesne = null;

        public bool KontrolEt(string OrtakNesneAdı = "")
        {
            if (OrtakNesneAdı == "") OrtakNesneAdı = Kendi.Adı;
            if (OrtakNesne != null) { OrtakNesne.Dispose(); OrtakNesne = null; }

            bool Evet = true;
            OrtakNesneAdı = "UygulamaOncedenCalistirildiMi_" + D_HexYazı.BaytDizisinden(D_GeriDönülemezKarmaşıklaştırmaMetodu.BaytDizisinden(D_Yazı.BaytDizisine(OrtakNesneAdı)));
            OrtakNesne = new Mutex(false, OrtakNesneAdı, out Evet);

            return !Evet;
        }
        public int DiğerUygulamayıÖneGetir(bool EkranıKapla = false)
        {
            int Adet = 0;
            try
            {
                string Adı = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
                
                #if DEBUG
                if (Adı.EndsWith(".vshost")) Adı = Adı.Remove(Adı.Length - ".vshost".Length);
                #endif

                W32_6.EnumWindows(delegate (IntPtr hWnd, int lParam)
                {
                    uint windowPid;
                    W32_5.GetWindowThreadProcessId(hWnd, out windowPid);
                    if (windowPid == Process.GetCurrentProcess().Id) return true;

                    int length = W32_4.GetWindowTextLength(hWnd);
                    if (length == 0) return true;

                    StringBuilder stringBuilder = new StringBuilder(length);
                    W32_4.GetWindowText(hWnd, stringBuilder, length + 1);
                    if (stringBuilder.ToString().Contains(Adı))
                    {
                        #if HazirKod_Cdiyez_Görsel
                            W32_7.WINDOWPLACEMENT wp = new W32_7.WINDOWPLACEMENT();
                            W32_7.GetWindowPlacement(hWnd, ref wp);
                            if (wp.showCmd == 0 || wp.showCmd == 2 || EkranıKapla)
                            {
                                if (EkranıKapla) wp.showCmd = 3;
                                else wp.showCmd = 1;

                                W32_7.SetWindowPlacement(hWnd, ref wp);
                            }
                        #endif
                        W32_3.SetForegroundWindow(hWnd);
                        Adet++;
                    }
                    return true;
                }, 0);
            }
            catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

            return Adet;
        }
        public int DiğerUygulamayıKapat(bool ZorlaKapat = false)
        {
            int Adet = 0;
            try
            {
                string Adı = Path.GetFileNameWithoutExtension(AppDomain.CurrentDomain.FriendlyName);
                
#if DEBUG
                if (Adı.EndsWith(".vshost")) Adı = Adı.Remove(Adı.Length - ".vshost".Length);
#endif

                int Şimdiki = Process.GetCurrentProcess().Id;
                Process[] Diğerleri = Process.GetProcessesByName(Adı);
                if (Diğerleri != null)
                {
                    foreach (Process Diğeri in Diğerleri)
                    {
                        if (Diğeri.Id == Şimdiki) continue;

                        if (ZorlaKapat) Diğeri.Kill();
                        else Diğeri.Close();
                        Adet++;
                    }
                }
            }
            catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

            return Adet;
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

                    if (OrtakNesne != null) { OrtakNesne.Dispose(); OrtakNesne = null; }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UygulamaOncedenCalistirildiMi_() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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

