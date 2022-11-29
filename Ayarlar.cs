// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Dönüştürme;
using System;
using System.IO;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public class Ayarlar_ 
    {
        public const string Sürüm = "V5.0";
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
        Depo_ Depo;
        Mutex Kilit;
        System.Timers.Timer Zamanlayıcı = null;
        DahaCokKarmasiklastirma_ Karmaşıklaştırma;
        #endregion
       
        public Ayarlar_(string AyarlarİçinParola = null, string AyarlarDosyası = null, int DeğişiklikleriKaydetmeAralığı_Sn = 5*60)
        {
            if (string.IsNullOrEmpty(AyarlarDosyası)) _AyarlarDosyasıYolu = Kendi.DosyaYolu + ".Ayarlar";    
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
                Depo.Yaz("Kendi/Bilgisayar ve kullanıcı aAdı", Kendi.BilgisayarAdı + "/" + Kendi.KullanıcıAdı);
                Depo.Yaz("Kendi/Bütünlük kontrolü", "ArGeMuP");

                Depo.Yaz("Uygulama", Kendi.Sürümü_Dosya);
                Depo.Yaz("Uygulama/Ad", Kendi.Adı);
                Depo.Yaz("Uygulama/Konum", Kendi.DosyaYolu);

                Depo.Yaz("Ayarlar", "ArGeMuP");
            }
            else
            {
                Depo = new Depo_(File.ReadAllText(_AyarlarDosyasıYolu));
                
                if (Parola != null)
                {
                    try
                    {
                        Depo_.IEleman ayrlr = Depo.Bul("Ayarlar");
                        string ayrlr_içeriği = Karmaşıklaştırma.Düzelt(ayrlr[0], Parola);
                        ayrlr.Ekle(null, ayrlr_içeriği);
                        ayrlr[0] = null;
                    }
                    catch (Exception) { }
                }

                try 
                {
                    string okunan = Depo.Oku("Kendi/Bütünlük kontrolü");
                    if (string.IsNullOrEmpty(okunan) || okunan != DoğrulamaKodu.Üret.Yazıdan(Depo.Bul("Ayarlar").YazıyaDönüştür(null, true))) throw new Exception(); 
                }
                catch (Exception) { throw new Exception("Bütünlük Kontrolü Hatalı - " + _AyarlarDosyasıYolu); }
            }

            if (DeğişiklikleriKaydetmeAralığı_Sn > 0)
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
            if (Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti)
            {
                if (File.Exists(_AyarlarDosyasıYolu) && !File.Exists(_AyarlarDosyasıYolu + ".yedek")) File.Copy(_AyarlarDosyasıYolu, _AyarlarDosyasıYolu + ".yedek", false);

                Kilit.WaitOne();

                string çıktı, Ayarlar = Depo.Bul("Ayarlar").YazıyaDönüştür(null, true);
                Depo.Yaz("Kendi/Bütünlük kontrolü", DoğrulamaKodu.Üret.Yazıdan(Ayarlar));

                if (Parola == null) çıktı = Depo.YazıyaDönüştür();
                else
                {
                    string Uygulama = Depo.Bul("Uygulama").YazıyaDönüştür(null);
                    string Kendi = Depo.Bul("Kendi").YazıyaDönüştür(null);

                    Depo_ gecici = new Depo_();
                    gecici.Yaz("Ayarlar", Karmaşıklaştırma.Karıştır(Ayarlar, Parola));
                    Ayarlar = gecici.YazıyaDönüştür(null);

                    çıktı = Kendi + Uygulama + Ayarlar;
                }

                File.WriteAllText(_AyarlarDosyasıYolu, çıktı);

                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;

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

        /// <param name="ElemanAdıDizisi">Ayraçlar.ElemanAdıDizisi karakteri / ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        /// <param name="İçeriği">Elemanın içeriği (İçeriği ve Elemanları olmayan Elemanlar YazıyaDönüştürme aşamasında görmezden gelinip silinir)</param>
        /// <param name="SıraNo">Aynı eleman adı ile birden fazla içerik tutmak için kullanılabilir</param>
        public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0)
        {
            Kilit.WaitOne();

            Depo.Yaz("Ayarlar" + (string.IsNullOrEmpty(ElemanAdıDizisi) ? null : Depo.Ayraçlar.ElemanAdıDizisi.ToString() + ElemanAdıDizisi), İçeriği, SıraNo);

            Kilit.ReleaseMutex();

            if (Zamanlayıcı == null) DeğişiklikleriKaydet();
        }
        public string Oku(string ElemanAdıDizisi, string BulunamamasıDurumundakiİçeriği = "", int SıraNo = 0)
        {
            Kilit.WaitOne();

            string okunan = Depo.Oku("Ayarlar" + (string.IsNullOrEmpty(ElemanAdıDizisi) ? null : Depo.Ayraçlar.ElemanAdıDizisi.ToString() + ElemanAdıDizisi), BulunamamasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }

        #region Dönüştürme
        public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, D_Sayı.Yazıya(Sayı), SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, D_HexYazı.BaytDizisinden(BaytDizisi, Adet, BaşlangıçKonumu), SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, D_TarihSaat.Yazıya(TarihSaat), SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, Bit.ToString(), SıraNo);
        }

        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            try
            {
                return D_Sayı.Yazıdan(Oku(ElemanAdıDizisi, null, SıraNo));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            try
            {
                return D_HexYazı.BaytDizisine(Oku(ElemanAdıDizisi, null, SıraNo));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            try
            {
                return D_TarihSaat.Tarihe(Oku(ElemanAdıDizisi, null, SıraNo));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            try
            {
                return bool.Parse(Oku(ElemanAdıDizisi, null, SıraNo));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        #endregion

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public string[] Listele_Elemanları(string ElemanAdıDizisi)
        {
            string[] çıktı = null;

            Depo_.IEleman eleman = Bul(ElemanAdıDizisi);
            if (eleman != null)
            {
                çıktı = new string[eleman.Elemanları.Length];
                for (int i = 0; i < eleman.Elemanları.Length; i++)
                {
                    çıktı[i] = eleman.Elemanları[i].Adı;
                }
            }
            else çıktı = new string[0];

            return çıktı;
        }
        public string[] Listele_İçeriği(string ElemanAdıDizisi)
        {
            string[] çıktı = null; 

            Depo_.IEleman eleman = Bul(ElemanAdıDizisi);
            if (eleman != null) çıktı = eleman.İçeriği;
            else çıktı = new string[0];

            return çıktı;
        }

        /// <summary>
        /// Ayarlar içinde kullanılan kilit mekanizmasından bağımsız çalıştırılacağından, uygulama kapanmadan önce tüm yazma okuma işlemleri bitmiş olmalı
        /// </summary>
        public Depo_.IEleman Bul(string ElemanAdıDizisi)
        {
            Kilit.WaitOne();

            Depo_.IEleman Eleman = Depo.Bul("Ayarlar" + (string.IsNullOrEmpty(ElemanAdıDizisi) ? null : Depo.Ayraçlar.ElemanAdıDizisi.ToString() + ElemanAdıDizisi));

            Kilit.ReleaseMutex();

            return Eleman;
        }
    }
}