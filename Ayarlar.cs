// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Dönüştürme;
using System;
using System.IO;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public class Ayarlar_ : IDisposable 
    {
        public const string Sürüm = "V1.0";
        public string AyarlarDosyasıYolu
        {
            get
            {
                return _AyarlarDosyasıYolu;
            }
        }

        #region Değişkenler
        string Parola;
        string _AyarlarDosyasıYolu;
        Depo_ Depo = null;
        Mutex Kilit;
        System.Timers.Timer Zamanlayıcı;
        DahaCokKarmasiklastirma_ Karmaşıklaştırma;
        #endregion
       
        public Ayarlar_(string AyarlarİçinParola = null, string AyarlarDosyası = null, int DeğişiklikleriKaydetmeAralığı_Sn = 5*60)
        {
            if (string.IsNullOrEmpty(AyarlarDosyası)) _AyarlarDosyasıYolu = Kendi.DosyaYolu() + ".Ayarlar";    
            else _AyarlarDosyasıYolu = AyarlarDosyası;
			
			if (!File.Exists(AyarlarDosyasıYolu)) 
            {
                Directory.CreateDirectory(Path.GetDirectoryName(AyarlarDosyasıYolu));
                FileStream gecici = File.Create(AyarlarDosyasıYolu);
                gecici.Close();
                File.Delete(AyarlarDosyasıYolu);
            }
          
            if (string.IsNullOrEmpty(AyarlarİçinParola)) Parola = null;
            else
            {
                Parola = AyarlarİçinParola;
                Karmaşıklaştırma = new DahaCokKarmasiklastirma_();
            }

            if (!File.Exists(AyarlarDosyasıYolu))
            {
                Depo = new Depo_();
                Depo.Yaz("Kendi", Sürüm);
                Depo.Yaz("Kendi/Oluşturulma", DateTime.Now);
                Depo.Yaz("Kendi/Konum", AyarlarDosyasıYolu);
                Depo.Yaz("Kendi/BilgisayarVeKullanıcıAdı", Kendi.BilgisayarAdı() + "/" + Kendi.KullanıcıAdı());

                Depo.Yaz("Uygulama", Kendi.Sürümü_Dosya());
                Depo.Yaz("Uygulama/Ad", Kendi.Adı());
            }
            else
            {
                string tümü = File.ReadAllText(_AyarlarDosyasıYolu);
                if (Parola != null) tümü = Karmaşıklaştırma.Düzelt(tümü, Parola);
                Depo = new Depo_(tümü);

                string okunan = Depo.Oku("Doğrulama/Bütünlük Kontrolü"), hesaplanan = "Hata";
                Depo.Yaz("Doğrulama/Bütünlük Kontrolü", null);
                try { hesaplanan = DoğrulamaKodu.Üret.Yazıdan(Depo.YazıyaDönüştür()); } catch (Exception) { }
                if (okunan != hesaplanan) throw new Exception("Bütünlük Kontrolü Hatalı");
            }

            if (Zamanlayıcı == null && DeğişiklikleriKaydetmeAralığı_Sn > 0)
            {
                Zamanlayıcı = new System.Timers.Timer();
                Zamanlayıcı.Elapsed += new System.Timers.ElapsedEventHandler(ZamanlayıcıKesmesi);
                Zamanlayıcı.Interval = DeğişiklikleriKaydetmeAralığı_Sn * 1000;
                Zamanlayıcı.AutoReset = false;
                Zamanlayıcı.Enabled = true;
            }

            if (Kilit == null) Kilit = new Mutex();

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }
        public void DeğişiklikleriKaydet(bool VeDurdur = false)
        {
            if (Depo.EnAzBirElemanAdıVeyaİçeriğiDeğişti)
            {
                if (File.Exists(_AyarlarDosyasıYolu) && !File.Exists(_AyarlarDosyasıYolu + ".yedek")) File.Copy(_AyarlarDosyasıYolu, _AyarlarDosyasıYolu + ".yedek", false);

                Kilit.WaitOne();

                Depo.Yaz("Doğrulama", DateTime.Now);
                Depo.Yaz("Doğrulama/Bütünlük Kontrolü", null);
                Depo.Yaz("Doğrulama/Bütünlük Kontrolü", DoğrulamaKodu.Üret.Yazıdan(Depo.YazıyaDönüştür()));

                string tümü = Depo.YazıyaDönüştür();
                if (Parola != null) tümü = Karmaşıklaştırma.Karıştır(tümü, Parola);

                File.WriteAllText(_AyarlarDosyasıYolu, tümü);
                Depo.EnAzBirElemanAdıVeyaİçeriğiDeğişti = false;
                File.Delete(_AyarlarDosyasıYolu + ".yedek");

                Kilit.ReleaseMutex();
            }
            
            if (VeDurdur) Durdur();
        }

        private void Durdur()
        {
            try
            {
				if (Kilit != null) { Kilit.Dispose(); Kilit = null; }

                if (Zamanlayıcı != null) { Zamanlayıcı.Dispose(); Zamanlayıcı = null; }
                if (Karmaşıklaştırma != null) { Karmaşıklaştırma.Dispose(); Karmaşıklaştırma = null; }
            }
            catch (Exception) { }
        }
        private void ZamanlayıcıKesmesi(object source, System.Timers.ElapsedEventArgs e)
        {
            DeğişiklikleriKaydet();
            Zamanlayıcı.Enabled = true;
        }
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            DeğişiklikleriKaydet(true);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string Oku(string ElemanAdıDizisi, string BulunamamasıDurumundakiİçeriği = "")
        {
            Kilit.WaitOne();

            string okunan = Depo.Oku("Ayarlar/" + ElemanAdıDizisi, BulunamamasıDurumundakiİçeriği);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public void Yaz(string ElemanAdıDizisi, string İçeriği)
        {
            Kilit.WaitOne();

            Depo.Yaz("Ayarlar/" + ElemanAdıDizisi, İçeriği);

            Kilit.ReleaseMutex();
        }

        #region Dönüştürme
        public void Yaz(string ElemanAdıDizisi, double Sayı)
        {
            Yaz(ElemanAdıDizisi, D_Sayı.Yazıya(Sayı));
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
        {
            Yaz(ElemanAdıDizisi, D_HexYazı.BaytDizisinden(BaytDizisi, Adet, BaşlangıçKonumu));
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat)
        {
            Yaz(ElemanAdıDizisi, D_TarihSaat.Yazıya(TarihSaat));
        }
        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default)
        {
            try
            {
                return D_Sayı.Yazıdan(Oku(ElemanAdıDizisi));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null)
        {
            try
            {
                return D_HexYazı.BaytDizisine(Oku(ElemanAdıDizisi));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default)
        {
            try
            {
                return D_TarihSaat.Tarihe(Oku(ElemanAdıDizisi));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string[] Listele(string ElemanAdıDizisi, bool Adı = false, bool İçeriği = true, bool ElemanAdları = false)
        {
            return Depo.Listele(ElemanAdıDizisi, Adı, İçeriği, ElemanAdları);
        }

        /// <summary>
        /// Ayarlar içinde kullanılan kilit mekanizmasından bağımsız çalıştırılacağından, uygulama kapanmadan önce tüm yazma okuma işlemleri bitmiş olmalı
        /// </summary>
        public Depo_.Depo_Elemanı_ Bul(string ElemanAdıDizisi)
        {
            return Depo.Bul(ElemanAdıDizisi);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                DeğişiklikleriKaydet(true);

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
        ~Ayarlar_()
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