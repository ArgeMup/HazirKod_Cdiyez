﻿// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace ArgeMup.HazirKod
{
    public class YeniYazılımKontrolü_ : IDisposable
    {
    	public string Sürüm = "V1.4";
        public bool KontrolTamamlandı = false;
        public delegate void YeniYazılımKontrolü_GeriBildirim_(bool Sonuç, string Açıklama);

        WebClient İstemci = null;
        YeniYazılımKontrolü_GeriBildirim_ GeriBildirim_İşlemi = null;
        string İndirilenDosyanınAdı = "ArgeMuP.HazirKod.YeniYazılımKontrolü." + Path.GetRandomFileName();
        string HedefDosyaYolu = Kendi.DosyaYolu;

        public void Başlat(Uri DosyaKonumu, YeniYazılımKontrolü_GeriBildirim_ GeriBildirim = null, string HedefDosyaYolu = null)
        {
            if (HedefDosyaYolu == null) İndirilenDosyanınAdı = Kendi.Klasörü + @"\" + İndirilenDosyanınAdı; 
            else
            {
                İndirilenDosyanınAdı = Path.GetDirectoryName(HedefDosyaYolu) + @"\" + İndirilenDosyanınAdı;
                this.HedefDosyaYolu = HedefDosyaYolu;
                Klasör.Oluştur(Path.GetDirectoryName(İndirilenDosyanınAdı));
            }

            string[] ÖncekiDenemeler = Directory.GetFiles(Path.GetDirectoryName(İndirilenDosyanınAdı), "ArgeMuP.HazirKod.YeniYazılımKontrolü.*", SearchOption.TopDirectoryOnly);
            if (ÖncekiDenemeler != null) foreach (string dsy in ÖncekiDenemeler) Dosya.Sil(dsy);

            İstemci = new WebClient();

            GeriBildirim_İşlemi = GeriBildirim;

            İstemci.DownloadFileAsync(DosyaKonumu, İndirilenDosyanınAdı);
            İstemci.DownloadFileCompleted += new AsyncCompletedEventHandler(İstemci_DownloadFileCompleted);
        }
        public void Durdur()
        {
            KontrolTamamlandı = true;
            if (İstemci == null) return;

            if (İstemci.IsBusy) İstemci.CancelAsync();
            İstemci.Dispose();
            İstemci = null;

            GeriBildirim_İşlemi?.Invoke(false, "Durduruldu");
        }
        void İstemci_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)
                {
                    FileVersionInfo gelen = FileVersionInfo.GetVersionInfo(İndirilenDosyanınAdı), şimdiki = null;

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
                                if (DoğrulamaKodu.Üret.Dosyadan(İndirilenDosyanınAdı) != DoğrulamaKodu.Üret.Dosyadan(HedefDosyaYolu)) gelen_daha_yeni = true; //uzaktaki dosyanın daha sağlıklı oduğunu varsayarak devam et
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
                        else if (!Dosya.Kopyala(İndirilenDosyanınAdı, HedefDosyaYolu)) GeriBildirim_İşlemi?.Invoke(false, "Hedef dosya oluşturulamadı");
                        else GeriBildirim_İşlemi?.Invoke(true, (HedefDosyaVarMı ? "Eski:V" + şimdiki.FileVersion + " " : null) + "Yeni:V" + gelen.FileVersion);
                    }
                    else GeriBildirim_İşlemi?.Invoke(true, "Güncel V" + şimdiki.FileVersion);
                }
                else GeriBildirim_İşlemi?.Invoke(false, e.Error.Message);

                Dosya.Sil(İndirilenDosyanınAdı);
                Durdur();
            }
            catch (Exception ex)
            {
                GeriBildirim_İşlemi?.Invoke(false, ex.Message);
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

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~YeniYazılımKontrolü_()
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