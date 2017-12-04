// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Windows.Forms;

namespace ArgeMup.HazirKod
{
    public class KayitDefteri_ : IDisposable
    {
		public const string Sürüm = "V1.1";
        string AnaDal;
        
        public KayitDefteri_(string ÜstDal = "")
        {
            if (string.IsNullOrEmpty(ÜstDal)) AnaDal = "HKEY_CURRENT_USER\\Software\\ArgeMup\\" + Application.ProductName;
            else AnaDal = ÜstDal;

            if (!AnaDal.EndsWith("\\")) AnaDal += "\\";
        }

        public object Oku(string AltDal, string Parametre)
        {
            AltDal = AltDal.Trim('\\', ' ');
            Parametre = Parametre.Trim('\\', ' ');
            return Microsoft.Win32.Registry.GetValue(AnaDal + AltDal, Parametre, null);
        }
        public bool Yaz(string AltDal, string Parametre, object Ayar)
        {
            try
            {
                AltDal = AltDal.Trim('\\', ' ');
                Parametre = Parametre.Trim('\\', ' ');
                Microsoft.Win32.Registry.SetValue(AnaDal + AltDal, Parametre, Ayar);
                return true;
            }
            catch (Exception) { }

            return false;
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

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~KayitDefteri_() {
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

