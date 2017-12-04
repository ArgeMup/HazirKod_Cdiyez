// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;

namespace ArgeMup.HazirKod
{
    public class KlavyeFareGozlemcisi_ : IDisposable
    {
        public const string Sürüm = "V1.0";

        #region Değişkenler
        public DateTime SonKlavyeFareOlayıAnı = DateTime.Now;

        const int WH_KEYBOARD_LL = 13;
        const int WH_MOUSE_LL = 14;

        int KlavyeGözlemcisi_Handle = 0;
        int FareGözlemcisi_Handle = 0;

        W32_2.Win32HookProcHandler KlavyeGirişimiOldu = null;
        W32_2.Win32HookProcHandler FareGirişimiOldu = null; 
        #endregion

        public KlavyeFareGozlemcisi_(out bool Sonuç)
        {
            Sonuç = false;

            try
            {
                KlavyeGirişimiOldu = new W32_2.Win32HookProcHandler(KlavyeOlayı);
                KlavyeGözlemcisi_Handle = W32_2.SetWindowsHookEx(WH_KEYBOARD_LL, KlavyeGirişimiOldu, W32_1.LoadLibrary("User32"), 0);

                FareGirişimiOldu = new W32_2.Win32HookProcHandler(FareOlayı);
                FareGözlemcisi_Handle = W32_2.SetWindowsHookEx(WH_MOUSE_LL, FareGirişimiOldu, W32_1.LoadLibrary("User32"), 0);

                if (KlavyeGözlemcisi_Handle != 0 && FareGözlemcisi_Handle != 0) Sonuç = true;
            }
            catch (Exception) { }
        }

        private int KlavyeOlayı(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0) SonKlavyeFareOlayıAnı = DateTime.Now;
            return W32_2.CallNextHookEx(KlavyeGözlemcisi_Handle, nCode, wParam, lParam);
        }
        private int FareOlayı(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0) SonKlavyeFareOlayıAnı = DateTime.Now;
            return W32_2.CallNextHookEx(FareGözlemcisi_Handle, nCode, wParam, lParam);
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                if (KlavyeGözlemcisi_Handle != 0)
                {
                    if (W32_2.UnhookWindowsHookEx(KlavyeGözlemcisi_Handle))
                    {
                        KlavyeGözlemcisi_Handle = 0;
                        KlavyeGirişimiOldu = null;
                    }
                }

                if (FareGözlemcisi_Handle != 0)
                {
                    if (W32_2.UnhookWindowsHookEx(FareGözlemcisi_Handle))
                    {
                        FareGözlemcisi_Handle = 0;
                        FareGirişimiOldu = null;
                    }
                }

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~KlavyeFareGozlemcisi_()
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

