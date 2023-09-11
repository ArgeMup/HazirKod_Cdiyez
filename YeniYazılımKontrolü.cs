// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ArgeMup.HazirKod
{
    public class YeniYazılımKontrolü_ : IDisposable
    {
    	public string Sürüm = "V1.5";
        public bool KontrolTamamlandı = false;
        public delegate void YeniYazılımKontrolü_GeriBildirim_(bool Sonuç, string Açıklama);

        Dosya.AğÜzerinde_ İstemci = null;
        YeniYazılımKontrolü_GeriBildirim_ GeriBildirim_İşlemi = null;
        string HedefDosyaYolu = Kendi.DosyaYolu;

        public void Başlat(Uri DosyaKonumu, YeniYazılımKontrolü_GeriBildirim_ GeriBildirim = null, string HedefDosyaYolu = null, int ZamanAşımı_msn = 15000)
        {
            string İndirilenDosyanınAdı = "ArgeMuP.HazirKod.YeniYazılımKontrolü." + Path.GetRandomFileName();
            if (HedefDosyaYolu == null) İndirilenDosyanınAdı = Kendi.Klasörü + @"\" + İndirilenDosyanınAdı; 
            else
            {
                İndirilenDosyanınAdı = Path.GetDirectoryName(HedefDosyaYolu) + @"\" + İndirilenDosyanınAdı;
                this.HedefDosyaYolu = HedefDosyaYolu;
            }

            if (Directory.Exists(Path.GetDirectoryName(İndirilenDosyanınAdı)))
            {
                string[] ÖncekiDenemeler = Directory.GetFiles(Path.GetDirectoryName(İndirilenDosyanınAdı), "ArgeMuP.HazirKod.YeniYazılımKontrolü.*", SearchOption.TopDirectoryOnly);
                if (ÖncekiDenemeler != null) foreach (string dsy in ÖncekiDenemeler) Dosya.Sil(dsy);
            }
            
            GeriBildirim_İşlemi = GeriBildirim;

            İstemci = new Dosya.AğÜzerinde_(DosyaKonumu, İndirilenDosyanınAdı, Tamamlandı, ZamanAşımı_msn);
        }
        public void Durdur()
        {
            if (İstemci == null) return;

            KontrolTamamlandı = true;

            İstemci.Dispose();
            İstemci = null;

            GeriBildirim_İşlemi?.Invoke(false, "Durduruldu");
        }

        void Tamamlandı(bool Sonuç, Uri İndirilenDosya_url, string İndirilenDosya_Adı)
        {
            try
            {
                if (Sonuç)
                {
                    FileVersionInfo gelen = FileVersionInfo.GetVersionInfo(İndirilenDosya_Adı), şimdiki = null;

                    bool gelen_daha_yeni = false, HedefDosyaVarMı = File.Exists(HedefDosyaYolu);
                    if (HedefDosyaVarMı)
                    {
                        şimdiki = FileVersionInfo.GetVersionInfo(HedefDosyaYolu);

                        if (gelen.FileMajorPart > şimdiki.FileMajorPart) gelen_daha_yeni = true;
                        else if (gelen.FileMajorPart == şimdiki.FileMajorPart)
                        {
                            if (gelen.FileMinorPart > şimdiki.FileMinorPart) gelen_daha_yeni = true;
                            else if (gelen.FileMinorPart == şimdiki.FileMinorPart)
                            {
                                if (DoğrulamaKodu.Üret.Dosyadan(İndirilenDosya_Adı) != DoğrulamaKodu.Üret.Dosyadan(HedefDosyaYolu)) gelen_daha_yeni = true; //uzaktaki dosyanın daha sağlıklı oduğunu varsayarak devam et
                            }
                        }
                    }
                    else gelen_daha_yeni = true;

                    if (gelen_daha_yeni)
                    {
                        if (HedefDosyaVarMı)
                        {
                            string HedefDosyaYolununYedeği = HedefDosyaYolu + ".V" + şimdiki.FileVersion;
                            if (!Dosya.Sil(HedefDosyaYolununYedeği)) GeriBildirim_İşlemi?.Invoke(false, "Yedek dosya silinemedi");
                            else File.Move(HedefDosyaYolu, HedefDosyaYolununYedeği);
                        }

                        if (File.Exists(HedefDosyaYolu)) GeriBildirim_İşlemi?.Invoke(false, "Hedef dosya silinemedi");
                        else if (!Dosya.Kopyala(İndirilenDosya_Adı, HedefDosyaYolu)) GeriBildirim_İşlemi?.Invoke(false, "Hedef dosya oluşturulamadı");
                        else GeriBildirim_İşlemi?.Invoke(true, (HedefDosyaVarMı ? "Eski:V" + şimdiki.FileVersion + " " : null) + "Yeni:V" + gelen.FileVersion);
                    }
                    else GeriBildirim_İşlemi?.Invoke(true, "Güncel V" + şimdiki.FileVersion);
                }
                else GeriBildirim_İşlemi?.Invoke(false, "Hatalı");

                Dosya.Sil(İndirilenDosya_Adı);
            }
            catch (Exception ex)
            {
                GeriBildirim_İşlemi?.Invoke(false, ex.Message);
            }

            Durdur();
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

                Durdur();

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        //~YeniYazılımKontrolü_()
        //{
        //    // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //    Dispose(false);
        //}

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}