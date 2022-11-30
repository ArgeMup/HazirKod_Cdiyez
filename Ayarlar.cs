// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.IO;

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
        EşZamanlıÇokluErişim.Depo_ Depo;
        IDepo_Eleman Depo_Ayarlar = null;
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

            string okunan = "";
            if (File.Exists(AyarlarDosyasıYolu))  okunan = File.ReadAllText(_AyarlarDosyasıYolu);
            Action<EşZamanlıÇokluErişim.Depo_> İşlem = null;
            if (DeğişiklikleriKaydetmeAralığı_Sn <= 0) İşlem = DeğişiklikleriKaydet_Hemen;
            Depo = new EşZamanlıÇokluErişim.Depo_(okunan, İşlem);

            if (!File.Exists(AyarlarDosyasıYolu))
            {
                Depo.Yaz("Kendi", Sürüm);
                Depo.Yaz("Kendi/Oluşturulma", DateTime.Now);
                Depo.Yaz("Kendi/Konum", AyarlarDosyasıYolu);
                Depo.Yaz("Kendi/Bilgisayar ve kullanıcı aAdı", Kendi.BilgisayarAdı + "/" + Kendi.KullanıcıAdı);
                Depo.Yaz("Kendi/Bütünlük kontrolü", "ArGeMuP");
                Depo.Yaz("Kendi/Son kayıt", DateTime.Now);

                Depo.Yaz("Uygulama", Kendi.Sürümü_Dosya);
                Depo.Yaz("Uygulama/Ad", Kendi.Adı);
                Depo.Yaz("Uygulama/Konum", Kendi.DosyaYolu);

                Depo.Yaz("Ayarlar", "ArGeMuP");
            }
            else
            {
                if (Parola != null)
                {
                    try
                    {
                        IDepo_Eleman ayrl = Depo.Bul("Ayarlar");
                        string ayrlr_içeriği = Karmaşıklaştırma.Düzelt(ayrl[0], Parola);
                        ayrl.Sil(null);
                        Depo.Ekle(ayrlr_içeriği);
                    }
                    catch (Exception) { }
                }

                try 
                {
                    okunan = Depo.Oku("Kendi/Bütünlük kontrolü");
                    if (string.IsNullOrEmpty(okunan) || okunan != DoğrulamaKodu.Üret.Yazıdan(Depo.Bul("Ayarlar").YazıyaDönüştür(null))) throw new Exception(); 
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

            Depo_Ayarlar = Depo.Bul("Ayarlar");
            Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
        }
        public void DeğişiklikleriKaydet(bool VeDurdur = false)
        {
            if (Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti) DeğişiklikleriKaydet_Hemen(null);
            
            if (VeDurdur) Durdur();
        }
        void DeğişiklikleriKaydet_Hemen(EşZamanlıÇokluErişim.Depo_ _)
        {
            if (Depo_Ayarlar == null) return;

            if (File.Exists(_AyarlarDosyasıYolu) && !File.Exists(_AyarlarDosyasıYolu + ".yedek")) File.Copy(_AyarlarDosyasıYolu, _AyarlarDosyasıYolu + ".yedek", false);

            ArgeMup.HazirKod.Depo_ birarada = new ArgeMup.HazirKod.Depo_();
            string Ayarlar = Depo_Ayarlar.YazıyaDönüştür(null);
            birarada.Ekle(Depo.Bul("Uygulama").YazıyaDönüştür(null));
            birarada.Ekle(Depo.Bul("Kendi").YazıyaDönüştür(null));
            birarada.Yaz("Kendi/Bütünlük kontrolü", DoğrulamaKodu.Üret.Yazıdan(Ayarlar));
            birarada.Yaz("Kendi/Son kayıt", DateTime.Now);

            if (Parola == null) birarada.Ekle(Ayarlar);
            else birarada.Yaz("Ayarlar", Karmaşıklaştırma.Karıştır(Ayarlar, Parola));

            File.WriteAllText(_AyarlarDosyasıYolu, birarada.YazıyaDönüştür());

            Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;

            File.Delete(_AyarlarDosyasıYolu + ".yedek");
        }

        private void Durdur()
        {
            try
            {
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
            Depo_Ayarlar.Yaz(ElemanAdıDizisi, İçeriği, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0)
        {
            Depo_Ayarlar.Yaz(ElemanAdıDizisi, Sayı, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, int SıraNo = 0)
        {
            Depo_Ayarlar.Yaz(ElemanAdıDizisi, BaytDizisi, Adet, BaşlangıçKonumu, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0)
        {
            Depo_Ayarlar.Yaz(ElemanAdıDizisi, TarihSaat, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0)
        {
            Depo_Ayarlar.Yaz(ElemanAdıDizisi, Bit, SıraNo);
        }

        public string Oku(string ElemanAdıDizisi, string BulunamamasıDurumundakiİçeriği = "", int SıraNo = 0)
        {
            return Depo_Ayarlar.Oku(ElemanAdıDizisi);
        }
        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            return Depo_Ayarlar.Oku_Sayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            return Depo_Ayarlar.Oku_BaytDizisi(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            return Depo_Ayarlar.Oku_TarihSaat(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            return Depo_Ayarlar.Oku_Bit(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        public IDepo_Eleman[] Elemanları(string ElemanAdıDizisi)
        {
            return Bul(ElemanAdıDizisi).Elemanları;
        }
        public string[] İçeriği(string ElemanAdıDizisi)
        {
            return Bul(ElemanAdıDizisi).İçeriği;
        }
        public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false)
        {
            return Depo_Ayarlar.Bul(ElemanAdıDizisi, YoksaOluştur);
        }
        public void Sil(string ElemanAdıDizisi)
        {
            Depo_Ayarlar.Sil(ElemanAdıDizisi);
        }
    }
}