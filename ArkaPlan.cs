// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ArgeMup.HazirKod.Dönüştürme;
using ArgeMup.HazirKod.Ekİşlemler;

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
                EşZamanlıİşlemSayısı_Çalışan < Liste.Count)
            {
                Interlocked.Increment(ref EşZamanlıİşlemSayısı_Çalışan);

                Task A1 = new Task(() =>
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

                }, TaskCreationOptions.LongRunning);

                A1.ContinueWith((A2) =>
                {
#if DEBUG
                    A2.Exception?.Günlük(null, Günlük.Seviye.HazirKod);
#endif

                    if (Interlocked.Decrement(ref EşZamanlıİşlemSayısı_Çalışan) == 0 && TümüÖğütülünceÇağırılacakİşlem != null && Liste.Count == 0) TümüÖğütülünceÇağırılacakİşlem(Hatırlatıcı);

                    ArkaPlanGörevi_Başlat();
                });

                A1.Start();
            }
        }
    }

    public class Hatırlatıcı_ : IDisposable
    {
        public const string Sürüm = "V1.2";
        public int Kilit_Devralma_ZamanAşımı_msn = 10000;
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
        /// Başlangıç           > new ArgeMup.HazirKod.ArkaPlan.Hatırlatıcı_();
        /// Kurma               > Ekle(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi, GeriBildirim_Islemi, Hatırlatıcı, SonsuzDöngüVeyaUzunİşlem);
        /// İşlem               > çıkış kodu > 0 olduğu sürece çağırılmaya devam edecek
        /// Bitiş               > Dispose
        /// 
        /// Kullanım Senaryosu 2 - Takvim
        /// İlk Açılış
        ///     Başlangıç       > new ArgeMup.HazirKod.ArkaPlan.Hatırlatıcı_();
        ///     Kurma           > Ekle(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi, GeriBildirim_Islemi, Hatırlatıcı, SonsuzDöngüVeyaUzunİşlem);
        ///     Bitiş           > AyarlarıOku(true) dan alınan çıktıları biryere kaydet
        /// İkinci ve Sonraki Açılışlar
        ///     Başlangıç       > new ArgeMup.HazirKod.ArkaPlan.Hatırlatıcı_(kaydedilen çıktıları aktar);
        ///     Kur             > Düzenle(TakmaAdıKıstası, GeriBildirim_Islemi, Hatırlatıcı, SonsuzDöngüVeyaUzunİşlem);
        ///     Bitiş           > AyarlarıOku(true) dan alınan çıktıları biryere kaydet
        /// İşlem               > TekrarlayıcıKomutCümlesi olan görevler -> SonrakiTetikleme_Kur çağırılmalı ve çıkış kodu = 0 olmalı
        ///                                             olmayan görevler -> çıkış kodu > 0 olduğu sürece çağırılmaya devam edecek
        ///
        /// Kullanım Senaryosu 3 - Basit Takvim
        /// Başlangıç + Bitiş   > Takvim den farklı olarak GeriBildirim_Islemi tanımlanmayabilir, güncel durum gerektiğinde Bul ile okunabilir
        /// Güncel durumu oku   > List<Durum_> Bul(SüresiDolanlarıDahilEt, ÇalışmayıBekleyenleriDahilEt, TakmaAdıKıstası);
        /// Bitiş               > Dispose
        /// </summary>

        /// <summary>
        /// GeriBildirim_Islemi 
        /// Func<string, object, int> GeriBildirim_Islemi
        /// int GeriBildirim_Islemi(string TakmaAdı, object Hatırlatıcı)
        /// {
        ///     Uzun sürecek işlemlerde kontrol et
        ///     ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın 
        /// 
        ///     ...
        ///     return 1000;
        /// }
        /// çıkış kodu > 0 : msn kadar erteler ve tekrar çağırır
        /// çıkış kodu < 0 : tamamen silip, tekrar çağırılmasını engeller
        /// çıkış kodu = 0 : zamanlamaya etkisi olmaz, SonrakiTetikleme_Kur veya Düzenle çağırılana kadar bekler 
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
                    return Ayarlar.Adı;
                }
                set
                {
                    Ayarlar.Adı = value;
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
                    Ayarlar.Yaz(null, value, 0);
                }
            }
            public string TekrarlayıcıKomutCümlesi
            {
                get
                {
                    return Ayarlar[1];
                }
                set
                {
                    Ayarlar.Yaz(null, value, 1);
                }
            }
            DateTime _HesaplananTetiklemeAnı;

            public bool GeriBildirim_Islemini_çalıştır, SonsuzDöngüVeyaUzunİşlem;
            public Func<string, object, int> GeriBildirim_Islemi;
            public object Hatırlatıcı;
            public int ÇıkışDeğeri;

            public IDepo_Eleman Ayarlar = null;

            public Biri_(IDepo_Eleman Ayarlar, DateTime HesaplananTetiklemeAnı, string TekrarlayıcıKomutCümlesi, Func<string, object, int> GeriBildirim_Islemi, object Hatırlatıcı, bool SonsuzDöngüVeyaUzunİşlem)
            {
                this.Ayarlar = Ayarlar;
                this.HesaplananTetiklemeAnı = HesaplananTetiklemeAnı;
                this.TekrarlayıcıKomutCümlesi = TekrarlayıcıKomutCümlesi;
                this.SonsuzDöngüVeyaUzunİşlem = SonsuzDöngüVeyaUzunİşlem;

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
        Mutex Kilit;
        List<Biri_> Liste;
        IDepo_Eleman Ayarlar = null;

        public Hatırlatıcı_(string Ayarlar = null)
        {
            // Hatırlatıcı_\n
            // >Tetikleyiciler\n
            // >>1> ...\n
            // >>2> ...\n

            ArgeMup.HazirKod.EşZamanlıÇokluErişim.Depo_ depo = new ArgeMup.HazirKod.EşZamanlıÇokluErişim.Depo_(Ayarlar);
            this.Ayarlar = depo.Bul("Hatırlatıcı_", true);

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
            Kilit = new Mutex();
            Liste = new List<Biri_>();

            // Hatırlatıcı_ dalı
            IDepo_Eleman te = Ayarlar.Bul("Tetikleyiciler");
            if (te != null)
            {
                foreach (IDepo_Eleman biri in te.Elemanları)
                {
                    string TakmaAdı = biri.Adı;
                    DateTime İlkTetikleyeceğiZaman = biri.Oku_TarihSaat(null, default, 0);
                    string TekrarlayıcıKomutCümlesi = biri.Oku(null, null, 1);

                    if (string.IsNullOrEmpty(TakmaAdı) || İlkTetikleyeceğiZaman == default) continue;

                    Ekle(TakmaAdı, İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi);
                }
            }

            #region Arkaplan görevi
            Task A1 = new Task(() =>
            {
                while (Çalışsın && Ortak.Çalışsın)
                {
                    Kilit.WaitOne();
                    int Bekleme = 1000;
                    Biri_ EnYakınGörev = null;
                    DateTime Şimdi = DateTime.Now;

                    foreach (Biri_ biri in Liste)
                    {
                        if (biri.GeriBildirim_Islemini_çalıştır &&
                            biri.GeriBildirim_Islemi != null)
                        {
                            if (biri.HesaplananTetiklemeAnı > Şimdi)
                            {
                                //daha erken
                                if (EnYakınGörev == null || biri.HesaplananTetiklemeAnı < EnYakınGörev.HesaplananTetiklemeAnı) EnYakınGörev = biri;
                            }
                            else
                            {
                                //süresi doldu
                                biri.ÇıkışDeğeri = 0;
                                biri.GeriBildirim_Islemini_çalıştır = false;

                                Task B1 = new Task(() =>
                                {
                                    biri.ÇıkışDeğeri = biri.GeriBildirim_Islemi(biri.TakmaAdı, biri.Hatırlatıcı);
                                }, biri.SonsuzDöngüVeyaUzunİşlem ? TaskCreationOptions.LongRunning : TaskCreationOptions.None);

                                B1.ContinueWith((B2) =>
                                {
#if DEBUG
                                    B2.Exception?.Günlük(null, Günlük.Seviye.HazirKod);
#endif

                                    Kilit.WaitOne();

                                    if (biri.ÇıkışDeğeri < 0) Liste.Remove(biri);                   //listeden silinir, artık çağırılmayacak
                                    else if (biri.ÇıkışDeğeri > 0) biri.Ertele(biri.ÇıkışDeğeri);   //Listede kalır, belirtilen süre sonra tekrar çağırılacak
                                    //else if (biri.ÇıkışDeğeri == 0)                               //Listede kalır, artık çağırılmayacak

                                    Kilit.ReleaseMutex();
                                });

                                B1.Start();
                            }
                        }
                    }

                    if (EnYakınGörev != null)
                    {
                        Bekleme = (int)(EnYakınGörev.HesaplananTetiklemeAnı - Şimdi).TotalMilliseconds;

                        if (Bekleme > 1000) Bekleme = 1000;
                        else if (Bekleme < 1) Bekleme = 1; //cpu yüzdesini düşürmek için
                    }

                    Kilit.ReleaseMutex();
                    Task.Delay(Bekleme).Wait();
                }
            }, TaskCreationOptions.LongRunning);

            A1.ContinueWith((A2) =>
            {
#if DEBUG
                A2.Exception?.Günlük(null, Günlük.Seviye.HazirKod);
#endif
            });

            A1.Start();
            #endregion
        }

        public void Düzenle(string TakmaAdıKıstası, Func<string, object, int> GeriBildirim_Islemi, object Hatırlatıcı = null, bool SonsuzDöngüVeyaUzunİşlem = false, bool BüyükKüçükHarfDuyarlı = true, char Ayraç = '*')
        {
            if (GeriBildirim_Islemi == null) throw new Exception("GeriBildirim_Islemi boş olmamalı");

            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı.BenzerMi(TakmaAdıKıstası, BüyükKüçükHarfDuyarlı, Ayraç));
            foreach (Biri_ b in bulunanlar)
            {
                b.GeriBildirim_Islemi = GeriBildirim_Islemi;
                b.Hatırlatıcı = Hatırlatıcı;
                b.SonsuzDöngüVeyaUzunİşlem = SonsuzDöngüVeyaUzunİşlem;
            }

            Kilit.ReleaseMutex();
        }
        public void Ekle(string TakmaAdı, DateTime İlkTetikleyeceğiZaman, string TekrarlayıcıKomutCümlesi = null, Func<string, object, int> GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SonsuzDöngüVeyaUzunİşlem = false)
        {
            if (!string.IsNullOrEmpty(TekrarlayıcıKomutCümlesi))
            {
                //Cümle hata kontrolü
                if (SonrakiTetikleme_Hesapla(İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi) == default) throw new Exception("TekrarlayıcıKomutCümlesi uygun değil");
            }

            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Liste.Add(new Biri_(Ayarlar.Bul("Tetikleyiciler/" + TakmaAdı, true), İlkTetikleyeceğiZaman, TekrarlayıcıKomutCümlesi, GeriBildirim_Islemi, Hatırlatıcı, SonsuzDöngüVeyaUzunİşlem));

            Kilit.ReleaseMutex();
        }

        public void SonrakiTetikleme_Kur(string TakmaAdıKıstası, bool GelecektekiBirZamanaKur = true, bool BüyükKüçükHarfDuyarlı = true, char Ayraç = '*')
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı.BenzerMi(TakmaAdıKıstası, BüyükKüçükHarfDuyarlı, Ayraç));
            foreach (Biri_ b in bulunanlar)
            {
                if (GelecektekiBirZamanaKur)
                {
                    while (Çalışsın && Ortak.Çalışsın && b.HesaplananTetiklemeAnı < DateTime.Now)
                    {
                        b.HesaplananTetiklemeAnı = SonrakiTetikleme_Hesapla(b.HesaplananTetiklemeAnı, b.TekrarlayıcıKomutCümlesi);
                        if (b.HesaplananTetiklemeAnı == default) break;
                    }
                }
                else
                {
                    b.HesaplananTetiklemeAnı = SonrakiTetikleme_Hesapla(b.HesaplananTetiklemeAnı, b.TekrarlayıcıKomutCümlesi);
                }

                if (b.HesaplananTetiklemeAnı == default) continue;

                b.GeriBildirim_Islemini_çalıştır = true;
            }

            Kilit.ReleaseMutex();
        }

        public void Sil(string TakmaAdıKıstası, bool BüyükKüçükHarfDuyarlı = true, char Ayraç = '*')
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<Biri_> bulunanlar = Liste.FindAll(x => x.TakmaAdı.BenzerMi(TakmaAdıKıstası, BüyükKüçükHarfDuyarlı, Ayraç));
            foreach (Biri_ b in bulunanlar)
            {
                b.Ayarlar.Sil(null);
                Liste.Remove(b);
            }

            Kilit.ReleaseMutex();
        }
        public List<Durum_> Bul(bool SüresiDolanlarıDahilEt = true, bool ÇalışmayıBekleyenleriDahilEt = true, string TakmaAdıKıstası = "*", bool BüyükKüçükHarfDuyarlı = true, char Ayraç = '*')
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<Durum_> bulunanlar = new List<Durum_>();
            DateTime şimdi = DateTime.Now;

            foreach (Biri_ hatırlatıcı in Liste.FindAll
                        (x =>
                            ((SüresiDolanlarıDahilEt && x.HesaplananTetiklemeAnı <= şimdi)
                                ||
                                (ÇalışmayıBekleyenleriDahilEt && x.HesaplananTetiklemeAnı > şimdi))
                            &&
                            x.TakmaAdı.BenzerMi(TakmaAdıKıstası, BüyükKüçükHarfDuyarlı, Ayraç)
                        )
                    )
            {
                bulunanlar.Add(new Durum_
                {
                    TakmaAdı = hatırlatıcı.TakmaAdı,
                    TekrarlayıcıKomutCümlesi = hatırlatıcı.TekrarlayıcıKomutCümlesi,
                    Hatırlatıcı = hatırlatıcı.Hatırlatıcı,
                    TetikleneceğiAn = hatırlatıcı.HesaplananTetiklemeAnı,
                    TetiklenmesiBekleniyor = hatırlatıcı.HesaplananTetiklemeAnı > şimdi
                });
            }

            Kilit.ReleaseMutex();

            return bulunanlar;
        }
        public string AyarlarıOku(bool VeDurdur = false)
        {
            if (VeDurdur) Çalışsın = false;

            return Ayarlar.YazıyaDönüştür(null);
        }

        public static DateTime SonrakiTetikleme_Hesapla(DateTime BaşlangıçNoktası, string KomutCümlesi)
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
            catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

            return default;
        }
        public void Dispose()
        {
            Çalışsın = false;
        }
    }
}