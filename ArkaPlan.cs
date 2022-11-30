// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Threading;
using System.Threading.Tasks;
using ArgeMup.HazirKod.Dönüştürme;

namespace ArgeMup.HazirKod.ArkaPlan
{
    public class Ortak
    {
        public static bool Çalışsın = true;
    }

    public class Öğütücü_<T>
    {
        public const string Sürüm = "V1.0";

        /// <summary>
        /// Görev içinde bekleme yapılacak ise Thread.Sleep(1000); kullanılmalı
        /// </summary>
        public int Kilit_Devralma_ZamanAşımı_msn = 5000;
        public EşZamanlıÇokluErişim.Liste_<T> Liste = new EşZamanlıÇokluErişim.Liste_<T>();

        Action<T, object> Çağırılacakİşlem = null;
        Action<object> TümüÖğütülünceÇağırılacakİşlem = null;
        bool Çalışsın = true;
        int AzamiElemanSayısı = int.MaxValue;
        long EşZamanlıİşlemSayısı = 1;
        long EşZamanlıİşlemSayısı_Çalışan = 0;
        int GöreviSilmedenÖnceBeklenecekSüre_msn = 60000;
        object Hatırlatıcı = null;
        long Gecikme_BitmeAnı = 0;

        public Öğütücü_(Action<T, object> Çağırılacakİşlem, object Hatırlatıcı = null, long EşZamanlıİşlemSayısı = 1, int AzamiElemanSayısı = int.MaxValue, int GöreviSilmedenÖnceBeklenecekSüre_msn = 5, Action<object> TümüÖğütülünceÇağırılacakİşlem = null)
        {
            if (Çağırılacakİşlem == null) throw new Exception("Çağırılacakİşlem boş olamaz");

            this.AzamiElemanSayısı = AzamiElemanSayısı;
            this.EşZamanlıİşlemSayısı = EşZamanlıİşlemSayısı;
            this.Çağırılacakİşlem = Çağırılacakİşlem;
            this.TümüÖğütülünceÇağırılacakİşlem = TümüÖğütülünceÇağırılacakİşlem;
            this.Hatırlatıcı = Hatırlatıcı;
            this.GöreviSilmedenÖnceBeklenecekSüre_msn = GöreviSilmedenÖnceBeklenecekSüre_msn;
        }
        public void Ekle(T Eleman, int Gecikme_msn = 5)
        {
            if (Liste.Count > AzamiElemanSayısı)
            {
                //en eski 1/2 üyeyü sil
                Liste.Sil(0, AzamiElemanSayısı / 2);
                Günlük.Ekle("HazirKod_Cdiyez Takipçi listesinde " + Liste.Count + " adet girdi mevcut, işlenemediğinden ilk 1/2 üyesi silindi", Günlük.Seviye.HazirKod);
            }
            Liste.Add(Eleman);

            Interlocked.Exchange(ref Gecikme_BitmeAnı, Environment.TickCount + Gecikme_msn);

            ArkaPlanGörevi_Başlat();
        }
        public bool TümüÖğütüldüMü()
        {
            return Liste.Count == 0 && Interlocked.Read(ref EşZamanlıİşlemSayısı_Çalışan) == 0;
        }
        public void Durdur()
        {
            Çalışsın = false;
        }

        void ArkaPlanGörevi_Başlat()
        {
            if (Çalışsın &&
                Ortak.Çalışsın &&
                EşZamanlıİşlemSayısı_Çalışan < EşZamanlıİşlemSayısı &&
                EşZamanlıİşlemSayısı_Çalışan < Liste.Count )
            {
                Interlocked.Increment(ref EşZamanlıİşlemSayısı_Çalışan);

                Task.Run(() => { ArkaPlanGörevi(); }).ContinueWith((t) =>
                {
                    if (Interlocked.Decrement(ref EşZamanlıİşlemSayısı_Çalışan) == 0 && TümüÖğütülünceÇağırılacakİşlem != null && Liste.Count == 0) TümüÖğütülünceÇağırılacakİşlem(Hatırlatıcı);

                    ArkaPlanGörevi_Başlat();
                });
            }
        }
        void ArkaPlanGörevi()
        {
            int za = Environment.TickCount + GöreviSilmedenÖnceBeklenecekSüre_msn;

            do
            {
                while (Çalışsın && Ortak.Çalışsın && Gecikme_BitmeAnı > Environment.TickCount) Thread.Sleep(5);

                if (Liste.Oku(0, out T Eleman, true))
                {
                    Çağırılacakİşlem(Eleman, Hatırlatıcı);
                    
                    za = Environment.TickCount + GöreviSilmedenÖnceBeklenecekSüre_msn;
                }
                else Thread.Sleep(100);
            }
            while (Çalışsın && Ortak.Çalışsın && za > Environment.TickCount);
        }
    }

    public class Hatırlatıcı_
    {
        public const string Sürüm = "V1.1";
        public int TekrarHatırlatmaGecikmesi_msn = 0;
        public class Durum_
        {
            public string TakmaAdı;
            public bool TetiklenmesiBekleniyor;
            public DateTime TetikleneceğiAn;
            public object Hatırlatıcı;
            public string TekrarlayıcıKomutCümlesi;
        }

        /// <summary>
        /// Kullanım Senaryosu 1 - Görevler
        /// başlangıç   > new ArgeMup.HazirKod.ArkaPlan.Hatırlatıcı_();
        /// Kurma       > Kur(TakmaAdı, GeriBildirim_Islemi, Hatırlatıcı);
        /// İşlem       > çıkış kodu > 0 olduğu sürece çağırılmaya devam edecek
        /// bitiş       > gerekli değil
        /// 
        /// Kullanım Senaryosu 2 - Takvim
        /// İlk Açılış
        ///     başlangıç   > new ArgeMup.HazirKod.ArkaPlan.Hatırlatıcı_();
        ///     Kurma       > Kur(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi, GeriBildirim_Islemi, Hatırlatıcı);
        ///     bitiş       > AyarlarıOku(true) dan alınan çıktıları biryere kaydet
        /// İkinci ve Sonraki Açılışlar
        ///     başlangıç   > new ArgeMup.HazirKod.ArkaPlan.Hatırlatıcı_(kaydedilen çıktıları aktar);
        ///     Kur         > Kur(TakmaAdı, GeriBildirim_Islemi, Hatırlatıcı);
        ///     bitiş       > AyarlarıOku(true) dan alınan çıktıları biryere kaydet
        /// İşlem           > çıkış kodu = 0 olduğu sürece zamanlama, tekrarlama sistemi tarafından kendiliğinden hesaplanmaya devam edecek
        ///
        /// Kullanım Senaryosu 3 - Basit Takvim
        /// başlangıç + bitiş   > Takvim deki gibi farklı olarak GeriBildirim_Islemi ve Hatırlatıcı tanımlanmayabilir
        /// Kurma               > Kur(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi);
        /// İşlem               > Düzenli olarak Listele komutunun çağırılması ile güncel durumlar okunabilir
        /// </summary>

        /// <summary>
        /// GeriBildirim_Islemi 
        /// Func<string, object, int> GeriBildirim_Islemi
        /// int GeriBildirim_Islemi(string TakmaAdı, object Hatırlatıcı)
        /// {
        ///     ...
        ///     return 1000;
        /// }
        /// çıkış kodu > 0 : msn kadar erteler ve tekrar çağırır
        /// çıkış kodu < 0 : tamamen silip, tekrar çağırılmasını engeller
        /// çıkış kodu = 0 : tekrarlayıcı sistemi sonraki tetiklemenin zamanlamasını Kur_SonrakiTetikleme komutu ile yeniden hesaplar
        /// </summary>

        /// <summary>
        /// Tekrarlayıcı Sistem Komut Cümlesi       örnek son tetikleme anı : 01.02.3000 04:05:06
        /// y2071   yıl 2071 olarak düzeltilir      01.02.2071 04:05:06
        /// y+1     + 1 yıl                         01.02.3001 04:05:06
        /// a+4     + 4 ay                          01.06.3000 04:05:06
        /// h+1     + 1 hafta                       08.02.3000 04:05:06
        /// g+7     + 7 gün                         08.02.3000 04:05:06
        /// s8      saat 8 olarak düzeltilir        01.02.3000 08:05:06
        /// s+9     + 9 saat                        01.02.3000 13:05:06
        /// d10     dakika 10 olarak düzeltilir     01.02.3000 04:10:06
        /// d+11    + 11 dakika                     01.02.3000 04:16:06
        /// sn12    saniye 12 olarak düzeltilir     01.02.3000 04:05:12
        /// sn+13   + 13 saniye                     01.02.3000 04:05:19
        /// 
        /// Örnek cümle : y+1 s8 sn+5 (soldan sağa doğru yorumlar)
        ///
        /// komut cümlesi kurma aşamasında tanımlanır ve kalıcı olarak kaydedilir
        /// Kur_SonrakiTetikleme komutu çağırıldığında, sonraki zamanlama komut cümlesine göre hesaplanır ve kalıcı olarak kaydedilir
        /// </summary>

        class Biri_
        {
            public string TakmaAdı
            {
                get
                {
                    return _TakmaAdı;
                }
                set
                {
                    _TakmaAdı = value;
                    Ayarlar.Yaz(null, value, 0);
                }
            }
            public DateTime HesaplananTetiklemeAnı
            {
                get
                {
                    return _HesaplananTetiklemeAnı;
                }
                set
                {
                    _HesaplananTetiklemeAnı = value;
                    Ayarlar.Yaz(null, value, 1);
                }
            }
            public string TekrarlayıcıKomutCümlesi
            {
                get
                {
                    return _TekrarlayıcıKomutCümlesi;
                }
                set
                {
                    _TekrarlayıcıKomutCümlesi = value;
                    Ayarlar.Yaz(null, value, 2);
                }
            }
            string _TakmaAdı;
            DateTime _HesaplananTetiklemeAnı;
            string _TekrarlayıcıKomutCümlesi = null;

            public bool GeriBildirim_Islemini_çalıştır = true;
            public Func<string, object, int> GeriBildirim_Islemi = null;
            public object Hatırlatıcı = null;

            public IDepo_Eleman Ayarlar = null;

            public Biri_(string TakmaAdı, DateTime HesaplananTetiklemeAnı, string TekrarlayıcıKomutCümlesi, Func<string, object, int> GeriBildirim_Islemi, object Hatırlatıcı)
            {
                this._TakmaAdı = TakmaAdı;
                this._HesaplananTetiklemeAnı = HesaplananTetiklemeAnı;
                this._TekrarlayıcıKomutCümlesi = TekrarlayıcıKomutCümlesi;

                this.GeriBildirim_Islemini_çalıştır = this._HesaplananTetiklemeAnı >= DateTime.Now;
                this.GeriBildirim_Islemi = GeriBildirim_Islemi;
                this.Hatırlatıcı = Hatırlatıcı;
            }

            public void Ertele(int MiliSaniye)
            {
                HesaplananTetiklemeAnı = DateTime.Now.AddMilliseconds(MiliSaniye);
                GeriBildirim_Islemini_çalıştır = true;
            }
        }

        bool Çalışsın = true;
        CancellationTokenSource İptalEtmeAnahtarıKaynağı = null;
        EşZamanlıÇokluErişim.Liste_<Biri_> Liste = new EşZamanlıÇokluErişim.Liste_<Biri_>();
        IDepo_Eleman Ayarlar = null;
        long UcuzKilit = 0;

        public Hatırlatıcı_(string Ayarlar = null)
        {
            // Hatırlatıcı_\n
            // >Tetikleyiciler\n
            // >>1> ...\n
            // >>2> ...\n

            ArgeMup.HazirKod.EşZamanlıÇokluErişim.Depo_ depo = new ArgeMup.HazirKod.EşZamanlıÇokluErişim.Depo_(Ayarlar);
            depo.Yaz("Hatırlatıcı_", "ArGeMuP");
            this.Ayarlar = depo.Bul("Hatırlatıcı_");

            Başlat();
        }
        public Hatırlatıcı_(IDepo_Eleman Ayarlar)
        {
            if (Ayarlar == null) throw new Exception("Ayarlar boş olamaz");
            this.Ayarlar = Ayarlar;

            Başlat();
        }
        void Başlat()
        {
            // Hatırlatıcı_ dalı

            IDepo_Eleman te = Ayarlar.Bul("Tetikleyiciler");
            if (te != null)
            {
                foreach (IDepo_Eleman biri in te.Elemanları)
                {
                    string TakmaAdı = biri.Oku(null, null, 0);
                    DateTime İlkTetikleyeceğiZaman = biri.Oku_TarihSaat(null, default, 1);
                    string TekrarlayıcıKomutCümlesi = biri.Oku(null, null, 2);

                    if (string.IsNullOrEmpty(TakmaAdı) || İlkTetikleyeceğiZaman == default(DateTime)) continue;

                    Kur(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi);
                }
            }
        }

        public void Kur(string TakmaAdı, Func<string, object, int> GeriBildirim_Islemi, object Hatırlatıcı = null)
        {
            if (GeriBildirim_Islemi == null) throw new Exception("GeriBildirim_Islemi boş olmamalı");

            EşZamanlıÇokluErişim.Liste_<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı == TakmaAdı);
            if (bulunanlar.Count > 0)
            {
                foreach (Biri_ b in bulunanlar)
                {
                    b.GeriBildirim_Islemi = GeriBildirim_Islemi;
                    b.Hatırlatıcı = Hatırlatıcı;
                }
            }
            
            ArkaPlanGörevi_Başlat();
        }
        public void Kur(string TakmaAdı, DateTime İlkTetikleyeceğiZaman, string TekrarlayıcıKomutCümlesi = null, Func<string, object, int> GeriBildirim_Islemi = null, object Hatırlatıcı = null)
        {
            if (!string.IsNullOrEmpty(TekrarlayıcıKomutCümlesi))
            {
                if (SonrakiTetikleme_Hesapla(İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi) == default) throw new Exception("TekrarlayıcıKomutCümlesi uygun değil");
            }
            
            Biri_ yeni = new Biri_(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi, GeriBildirim_Islemi, Hatırlatıcı);
            Liste.Add(yeni);

            Ayarlar.Yaz("Tetikleyiciler/" + Liste.Count, yeni.TakmaAdı, 0);
            Ayarlar.Yaz("Tetikleyiciler/" + Liste.Count, yeni.HesaplananTetiklemeAnı, 1);
            Ayarlar.Yaz("Tetikleyiciler/" + Liste.Count, yeni.TekrarlayıcıKomutCümlesi, 2);
            yeni.Ayarlar = Ayarlar.Bul("Tetikleyiciler/" + Liste.Count);

            if (GeriBildirim_Islemi != null) ArkaPlanGörevi_Başlat();
        }

        public DateTime SonrakiTetikleme_Kur(string TakmaAdı, bool GelecektekiBirZamanaKur = true)
        {
            EşZamanlıÇokluErişim.Liste_<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı == TakmaAdı);
            if (bulunanlar == null || bulunanlar.Count == 0) return default;
            
            if (GelecektekiBirZamanaKur)
            {
                while (Çalışsın && Ortak.Çalışsın && bulunanlar[0].HesaplananTetiklemeAnı < DateTime.Now)
                {
                    bulunanlar[0].HesaplananTetiklemeAnı = SonrakiTetikleme_Hesapla(bulunanlar[0].HesaplananTetiklemeAnı, bulunanlar[0].TekrarlayıcıKomutCümlesi);
                    if (bulunanlar[0].HesaplananTetiklemeAnı == default) return default;
                }
            }
            else
            {
                bulunanlar[0].HesaplananTetiklemeAnı = SonrakiTetikleme_Hesapla(bulunanlar[0].HesaplananTetiklemeAnı, bulunanlar[0].TekrarlayıcıKomutCümlesi);
                if (bulunanlar[0].HesaplananTetiklemeAnı == default) return default;
            }

            bulunanlar[0].GeriBildirim_Islemini_çalıştır = true;
            ArkaPlanGörevi_Başlat(); //değişiklikleri işlet

            return bulunanlar[0].HesaplananTetiklemeAnı;
        }
        public DateTime SonrakiTetikleme_Hesapla(DateTime BaşlangıçNoktası, string KomutCümlesi)
        {
            if (string.IsNullOrEmpty(KomutCümlesi)) return default;

            try
            {
                bool sonuç = false;
                string[] d = KomutCümlesi.Split(' ');
                if (d != null && d.Length > 0)
                {
                    foreach (string s in d)
                    {
                        double aralık = D_Sayı.Yazıdan(s);
                        bool ekle = s.Contains("+");

                        if (s.StartsWith("y"))
                        {
                            sonuç = true;
                            if (ekle) BaşlangıçNoktası = BaşlangıçNoktası.AddYears((int)aralık);
                            else BaşlangıçNoktası = new DateTime((int)aralık, BaşlangıçNoktası.Month, BaşlangıçNoktası.Day, BaşlangıçNoktası.Hour, BaşlangıçNoktası.Minute, BaşlangıçNoktası.Second);
                        }
                        else if (s.StartsWith("a"))
                        {
                            if (ekle)
                            {
                                sonuç = true;
                                BaşlangıçNoktası = BaşlangıçNoktası.AddMonths((int)aralık);
                            }
                        }
                        else if (s.StartsWith("g"))
                        {
                            if (ekle)
                            {
                                sonuç = true;
                                BaşlangıçNoktası = BaşlangıçNoktası.AddDays(aralık);
                            }
                        }
                        else if (s.StartsWith("h"))
                        {
                            if (ekle)
                            {
                                sonuç = true;
                                BaşlangıçNoktası = BaşlangıçNoktası.AddDays(aralık * 7);
                            }
                        }
                        else if (s.StartsWith("d"))
                        {
                            sonuç = true;
                            if (ekle) BaşlangıçNoktası = BaşlangıçNoktası.AddMinutes(aralık);
                            else BaşlangıçNoktası = new DateTime(BaşlangıçNoktası.Year, BaşlangıçNoktası.Month, BaşlangıçNoktası.Day, BaşlangıçNoktası.Hour, (int)aralık, BaşlangıçNoktası.Second);
                        }
                        else if (s.StartsWith("sn"))
                        {
                            sonuç = true;
                            if (ekle) BaşlangıçNoktası = BaşlangıçNoktası.AddSeconds(aralık);
                            else BaşlangıçNoktası = new DateTime(BaşlangıçNoktası.Year, BaşlangıçNoktası.Month, BaşlangıçNoktası.Day, BaşlangıçNoktası.Hour, BaşlangıçNoktası.Minute, (int)aralık);
                        }
                        else if (s.StartsWith("s"))
                        {
                            sonuç = true;
                            if (ekle) BaşlangıçNoktası = BaşlangıçNoktası.AddHours(aralık);
                            else BaşlangıçNoktası = new DateTime(BaşlangıçNoktası.Year, BaşlangıçNoktası.Month, BaşlangıçNoktası.Day, (int)aralık, BaşlangıçNoktası.Minute, BaşlangıçNoktası.Second);
                        }
                    }
                }

                if (sonuç) return BaşlangıçNoktası;
            }
            catch (Exception) { }

            return default;
        }
        
        public void Sil(string TakmaAdı)
        {
            EşZamanlıÇokluErişim.Liste_<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı == TakmaAdı);
            foreach (Biri_ b in bulunanlar)
            {
                Liste.Remove(b);
            }

            if (bulunanlar.Count > 0) ArkaPlanGörevi_Başlat(); //değişiklikleri işlet
        }
        public Durum_ Bul(string TakmaAdı)
        {
            EşZamanlıÇokluErişim.Liste_<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı == TakmaAdı);
            Durum_ Durum = null;

            if (bulunanlar != null && bulunanlar.Count > 0)
            {
                Durum = new Durum_();
                Durum.TakmaAdı = bulunanlar[0].TakmaAdı;
                Durum.TekrarlayıcıKomutCümlesi = bulunanlar[0].TekrarlayıcıKomutCümlesi;

                Durum.Hatırlatıcı = bulunanlar[0].Hatırlatıcı;
                
                Durum.TetikleneceğiAn = bulunanlar[0].HesaplananTetiklemeAnı;
                Durum.TetiklenmesiBekleniyor = bulunanlar[0].HesaplananTetiklemeAnı > DateTime.Now;
            }

            return Durum;
        }
        public Durum_[] Bul(bool SadeceSüresiDolanları = true)
        {
            DateTime şimdi = DateTime.Now;
            EşZamanlıÇokluErişim.Liste_<Biri_> bulunanlar = null;
            if (SadeceSüresiDolanları) bulunanlar = Liste.FindAll(x => x.HesaplananTetiklemeAnı <= şimdi);
            else bulunanlar = Liste.Copy();

            Durum_[] çıktı = new Durum_[bulunanlar.Count];
            for (int i = 0; i < bulunanlar.Count; i++)
            {
                çıktı[i] = new Durum_();
                çıktı[i].TakmaAdı = bulunanlar[i].TakmaAdı;
                çıktı[i].TekrarlayıcıKomutCümlesi = bulunanlar[i].TekrarlayıcıKomutCümlesi;

                çıktı[i].Hatırlatıcı = bulunanlar[i].Hatırlatıcı;

                çıktı[i].TetikleneceğiAn = bulunanlar[i].HesaplananTetiklemeAnı;
                çıktı[i].TetiklenmesiBekleniyor = bulunanlar[i].HesaplananTetiklemeAnı > DateTime.Now;
            }

            return çıktı;
        }
        public string AyarlarıOku(bool VeDurdur = false)
        {
            if (VeDurdur)
            {
                Çalışsın = false;
                ArkaPlanGörevi_Başlat();
            }

            return Ayarlar.YazıyaDönüştür(null);
        }

        void ArkaPlanGörevi_Başlat()
        {
            if (Interlocked.Increment(ref UcuzKilit) > 1) return;

            if (İptalEtmeAnahtarıKaynağı != null)
            {
                İptalEtmeAnahtarıKaynağı.Cancel();
                Thread.Sleep(5);
                İptalEtmeAnahtarıKaynağı.Dispose();
                İptalEtmeAnahtarıKaynağı = null;
            }

            if (Çalışsın && Ortak.Çalışsın)
            {
                DateTime EnYakınTetikleme = DateTime.MaxValue;
                Biri_ EnYakınİşlem = null;
                foreach (Biri_ b in Liste)
                {
                    if (b.HesaplananTetiklemeAnı < EnYakınTetikleme &&
                        b.GeriBildirim_Islemini_çalıştır &&
                        b.GeriBildirim_Islemi != null)
                    {
                        EnYakınTetikleme = b.HesaplananTetiklemeAnı;
                        EnYakınİşlem = b;
                    }
                }

                if (EnYakınİşlem != null)
                {
                    İptalEtmeAnahtarıKaynağı = new CancellationTokenSource();
                    Task.Run(() =>
                    {
                        EnYakınİşlem.GeriBildirim_Islemini_çalıştır = false;
                        ArkaPlanGörevi(EnYakınİşlem, İptalEtmeAnahtarıKaynağı.Token);
                    }, İptalEtmeAnahtarıKaynağı.Token).ContinueWith((t) =>
                    {
                        ArkaPlanGörevi_Başlat();
                    });
                }
            }

            Interlocked.Exchange(ref UcuzKilit, 0);
        }
        void ArkaPlanGörevi(Biri_ EnYakınİşlem, CancellationToken İptalEtmeAnahtarı)
        {
            while ( Çalışsın && 
                    Ortak.Çalışsın && 
                    !İptalEtmeAnahtarı.IsCancellationRequested && 
                    EnYakınİşlem.HesaplananTetiklemeAnı > DateTime.Now)
            {
                TimeSpan gecikme = EnYakınİşlem.HesaplananTetiklemeAnı - DateTime.Now;
                if (gecikme.TotalMilliseconds > int.MaxValue) İptalEtmeAnahtarı.WaitHandle.WaitOne(int.MaxValue);
                else if (gecikme.TotalMilliseconds > 0) İptalEtmeAnahtarı.WaitHandle.WaitOne(gecikme);
            }

            if (Çalışsın && Ortak.Çalışsın && EnYakınİşlem.HesaplananTetiklemeAnı <= DateTime.Now)
            {
                int sonuç = 0;
                Task.Run(() =>
                {
                    sonuç = EnYakınİşlem.GeriBildirim_Islemi(EnYakınİşlem.TakmaAdı, EnYakınİşlem.Hatırlatıcı);
                }).ContinueWith((t) =>
                {
                    if (sonuç < 0) Liste.Remove(EnYakınİşlem);      //kendi kendini silebilmesi için
                    else if (sonuç > 0) EnYakınİşlem.Ertele(sonuç); //kendini geciktirebimesi için
                    else /*if (sonuç == 0)*/                        //tekrarlama kıstasları üzerinden tetiklet
                    {
                        if (TekrarHatırlatmaGecikmesi_msn > 0) EnYakınİşlem.Ertele(TekrarHatırlatmaGecikmesi_msn);
                        //else                                      //tekrarlama kıstasları üzerinden tetiklet
                    }

                    ArkaPlanGörevi_Başlat();                        //biten görev dolayısıyla zamanlamayı tekrar gözden geçir
                });
            }
            else EnYakınİşlem.GeriBildirim_Islemini_çalıştır = true;
        }
    }
}