// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_DeneyselEklentiler
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ArgeMup.HazirKod.Dönüştürme
{
    public static class D_Parmakİzi
    {
        public const string Sürüm = "V1.0";
        
        public static string Yazıya()
        {
            /* 
            * Kullanılacak ise  
            * Solution Explorer -> Proje -> References -> Add Reference
            * Assemblies -> Framework -> System.Management
            * 
            * veya
            * 
            * Nuget -> System.Management
            */
        
            string Çıktı = "";
            System.Management.ManagementClass mc = new System.Management.ManagementClass("Win32_DiskDrive");
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementBaseObject mo in moc)
            {
                var gecici = mo["InterfaceType"];
                if (gecici == null || gecici.ToString() == "USB") continue;
        
                gecici = mo["Model"];
                if (gecici != null) Çıktı += gecici.ToString() + ", ";
        
                gecici = mo["SerialNumber"];
                if (gecici != null) Çıktı += gecici.ToString() + ", ";
        
                gecici = mo["Signature"];
                if (gecici != null) Çıktı += gecici.ToString();
        
                if (Çıktı != "") break;
            }
        
            moc.Dispose();
            mc.Dispose();
        
            return Çıktı;
        }
    }
}

namespace ArgeMup.HazirKod.ArkaPlan
{
    public class Görev_
    {
        public const string Sürüm = "V0.0";

        /// <summary>
        /// Görev içinde bekleme yapılacak ise Thread.Sleep(1000); kullanılmalı
        /// </summary>
        public int Kilit_Devralma_ZamanAşımı_msn = 5000;

        public class Detaylar_
        {
            public int ÇalıştırılacakAdım = 0;
            public object KullanıcıNesnesi = null;
            public bool DurmaTalepEdildi = false;

            public void Bekle(int MiliSaniye)
            {
                while (MiliSaniye > 1000 && !DurmaTalepEdildi)
                {
                    System.Threading.Thread.Sleep(1000);
                    MiliSaniye -= 1000;
                }

                if (!DurmaTalepEdildi) System.Threading.Thread.Sleep(MiliSaniye);
            }
        }

        Task AnaGörev = null;
        bool Çalışsın = true;
        List<Görev_Biri_> Görevler = new List<Görev_Biri_>();
        Mutex Kilit = new Mutex();

        class Görev_Biri_
        {
            public string TakmaAdı = null;
            public Detaylar_ Detaylar = new Detaylar_();
            public DateTime ZamanAşımıAnı = DateTime.Now;
            public Func<Detaylar_, int> Çalıştırılacakİşlem = null;
            public Task Kendisi = null;
        }

        /// <param name="Çalıştırılacakİşlem"> Geri dönüş değeri <0 ise görevi siler, >0 ise Gecikme_msn olarak kullanılır</param>
        /// <param name="İlkTetiklemeGecikmesi_msn"> uint.MaxValue ise çalıştırmaz</param>
        /// <exception cref="Exception">Kilit devralınamadı</exception>
        public void Ekle(string TakmaAdı, Func<Detaylar_, int> Çalıştırılacakİşlem, uint İlkTetiklemeGecikmesi_msn = uint.MinValue, object KullanıcıNesnesi = null)
        {
            Görev_Biri_ yeni = new Görev_Biri_();
            yeni.TakmaAdı = TakmaAdı;
            yeni.Detaylar.KullanıcıNesnesi = KullanıcıNesnesi;
            yeni.Çalıştırılacakİşlem = Çalıştırılacakİşlem;

            if (İlkTetiklemeGecikmesi_msn == uint.MaxValue) yeni.ZamanAşımıAnı = DateTime.MaxValue;
            else yeni.ZamanAşımıAnı = DateTime.Now + TimeSpan.FromMilliseconds(İlkTetiklemeGecikmesi_msn);

            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");
            Görevler.Add(yeni);
            Kilit.ReleaseMutex();

            Uyandır();
        }
        public int MevcutMu(string TakmaAdı)
        {
            List<Görev_Biri_> bulunanlar = null;
            if (string.IsNullOrEmpty(TakmaAdı)) bulunanlar = Görevler;
            else bulunanlar = Görevler.FindAll(x => x.TakmaAdı == TakmaAdı);

            return bulunanlar.Count;
        }
        public void ZamanlamayaMüdahaleEt(string TakmaAdı, uint TetiklemeGecikmesi_msn = uint.MinValue, bool DurmasınıTalepEt = false)
        {
            List<Görev_Biri_> bulunanlar = null;
            if (string.IsNullOrEmpty(TakmaAdı)) bulunanlar = Görevler;
            else bulunanlar = Görevler.FindAll(x => x.TakmaAdı == TakmaAdı);

            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");
            foreach (var G in bulunanlar)
            {
                G.Detaylar.DurmaTalepEdildi = DurmasınıTalepEt;
                G.ZamanAşımıAnı = DateTime.Now + TimeSpan.FromMilliseconds(TetiklemeGecikmesi_msn);
            }
            Kilit.ReleaseMutex();

            Uyandır();
        }

        void Uyandır()
        {
            bool YenidenOluştur = false;

            Console.WriteLine("---1 AnaGörev.Status " + (AnaGörev == null ? "null" : AnaGörev.Status.ToString()));
            if (AnaGörev == null) YenidenOluştur = true;
            else if (AnaGörev.Status >= TaskStatus.RanToCompletion)
            {
                Console.WriteLine("---2 AnaGörev.Status " + AnaGörev.Status);

                try { AnaGörev.Dispose(); } catch { }
                YenidenOluştur = true;
            }

            if (Çalışsın && YenidenOluştur)
            {
                AnaGörev = Task.Run(() => { ArkaPlanGörevi(); }).ContinueWith((t) =>
                {
                    Console.WriteLine("---3 AnaGörev.Status " + AnaGörev.Status);

                    if (Çalışsın && t.IsFaulted)
                    {
                        Thread.Sleep(1000);
                        Uyandır(); //tekrar başlat
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);

                Console.WriteLine("---4 AnaGörev.Status " + AnaGörev.Status);
            }
        }
        async void ArkaPlanGörevi()
        {
            if (Çalışsın && Görevler.Count > 0)
            {
            YenidenDene:
                DateTime EnGerideki = DateTime.MaxValue;
                DateTime Şimdi = DateTime.Now;
                foreach (var G in Görevler)
                {
                    if (G.ZamanAşımıAnı == DateTime.MinValue)
                    {
                        if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) continue;

                        Görevler.Remove(G);
                        Kilit.ReleaseMutex();

                        goto YenidenDene;
                    }

                    if (G.ZamanAşımıAnı > Şimdi)
                    {
                        if (G.ZamanAşımıAnı < EnGerideki) EnGerideki = G.ZamanAşımıAnı;
                        continue;
                    }

                    bool YenidenOluştur = false;

                    if (G.Kendisi == null) YenidenOluştur = true;
                    else if (G.Kendisi.Status >= TaskStatus.RanToCompletion)
                    {
                        try { G.Kendisi.Dispose(); } catch { }
                        YenidenOluştur = true;
                    }

                    if (YenidenOluştur)
                    {
                        int gecikme1 = 0;
                        G.Kendisi = Task.Run(() => { gecikme1 = G.Çalıştırılacakİşlem(G.Detaylar); }).ContinueWith((t) =>
                        {
                            if (t.IsCompleted)
                            {
                                if (gecikme1 < 0) G.ZamanAşımıAnı = DateTime.MinValue;
                                else G.ZamanAşımıAnı = DateTime.Now + TimeSpan.FromMilliseconds(gecikme1);
                            }

                            Uyandır();
                            Console.WriteLine("Uyandır bitişten");
                        });
                    }
                }

                int g_ = (int)(EnGerideki - Şimdi).TotalMilliseconds;
                if (g_ < 0) g_ = 5;
                Console.WriteLine("g_ " + g_ + " " + (AnaGörev == null ? "null" : AnaGörev.Status.ToString()));
                throw new Exception("daa");

                //Task.Delay(1).ContinueWith((t) => { Uyandır(); }); //eğer çalışmıyorsa kendini tekrar aç
                //üstten bakınca rantocomplete gibi göründüğünden ölmüş gibi değerlendirilip yeniden başlatılabilecek 
            }
        }
    }
}

#if HazirKod_Cdiyez_Görsel
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Threading;
    using System.Windows.Forms;

    namespace ArgeMup.HazirKod
    {
        public class MesajPanosu_ : UserControl
        {
            public string Sürüm = "V1.0";      

            #region Girdiler
            public enum Girdi_Yazı_Tip_ { Basit, Değiştirilebilir, Tuş, Onay, Seçim, Yüzde, Grup };
            public class Girdi_Yazı_
            {
                public Girdi_Yazı_Tip_ Tip = Girdi_Yazı_Tip_.Basit;
                public string Metin = "";
                public string İpucu = "";

                public bool Üyelik_Tıklandı = false;
                public bool Üyelik_YazıDeğiştirildi = false;
                public Font Görünüm = null;
            }
            public class Girdi_Resim_
            {
                public Image Resim = null;
                public string İpucu = "";
                public bool Tıklanabilir = false;
            }
            public class Girdi_Mesaj_
            {
                public Girdi_Resim_ SolaDayalıResim;
                public List<Girdi_Yazı_> Yazılar;

                public string Tanım = "";
                public object Hatırlatıcı = null;
            }

            public class Ayarlar_
            {
                public int CanlandırmaHızı_X = 15;
                public int CanlandırmaHızı_Y = 5;

                public int ResimİleYazıArasıMesafe = 5;
                public int İkiMesajArasıMesafe = 5;
                public int İkiYazıArasıMesafe = 5;

                public int Genişlik_Değiştirilebilir = 250;
                public int Genişlik_Yüzde = 250;
                public int Genişlik_Grup = 250;

                public bool Çerçeveli = false;

                /// <param name="ResimBoyutu">ResimBoyutu = null ise veya x, y nin herhangi biri 0 ise bu değeri eklenen yazıların toplam yüksekliğinden hesaplar.</param>
                public Size Varsayılan_Resim_Boyut = new Size();
                public Image Varsayılan_Resim = null;
                public Font Varsayılan_Yazı_GörselDüzen = null; 
            }
            public Ayarlar_ Ayarlar = new Ayarlar_();

            public event ÜyelikŞablonu Tıklandıİşlemi = null, YazıDeğiştiİşlemi = null;
            public delegate void ÜyelikŞablonu(int MesajNo, int YazıNo);
            #endregion

            #region Çıktılar
            public class Çıktı_Mesaj_
            {
                public Panel Pano = null;
                public PictureBox SolaDayalıResim = null;
                public Control[] Yazılar = null;

                public string Tanım = "";
                public object Hatırlatıcı = null;
            }
            #endregion

            #region Arkaplan Değişkenleri
            enum Bekleyen_İşlem_Tipi_ { Ekle, Taşı, Sil, YenidenÇizdir, SolaDayalıResmiDeğiştir, YazıyıDeğiştir, YazıyıDurgunlaştır, YazıyıEtkinleştir };
            enum Bekleyen_Ekranlama_Tipi_ { Ekle, ÖneGetir, PozisyonunuDeğiştir, Sil, KaydırmaÇubuğunuGizle, SolaDayalıResmiDeğiştir, YazıyıDeğiştir, YazıyıDurgunlaştır, YazıyıEtkinleştir };
            List<Bekleyenİşlem_> Bekleyenİşlemler = new List<Bekleyenİşlem_>();
            List<Ekrandaki_> Ekrandakiler = new List<Ekrandaki_>();
            List<Ekrandaki_> EkrandanSilinenler = new List<Ekrandaki_>();
            List<BekleyenEkranlama_> BekleyenEkranlama = new List<BekleyenEkranlama_>();
            Thread ArkaPlanGörevi;
            ManualResetEvent ArkaPlanİşlemiÇalışsınMı;
            bool Çalışsın = false;
            int YaratılanPanoSayısı = 0;
            
            class Bekleyenİşlem_
            {
                public Bekleyen_İşlem_Tipi_ İşlemTipi;
                public int MesajNo, YazıNo;
                public Girdi_Mesaj_ Mesaj;
                public object Değer;
            }
            class Ekrandaki_
            {
                public Panel Pano;
                public PictureBox SolaDayalıResim;
                public List<Control> Yazılar;
                public string Tanım;
                public object Hatırlatıcı;

                public Point HedefKonum;
                public bool EkranGüncellemesiGerekiyor;
            }
            class BekleyenEkranlama_
            {
                public Bekleyen_Ekranlama_Tipi_ EkranlamaTipi;
                public object Nesne, Değer;
            }
            #endregion
            
            public MesajPanosu_()
            {
                InitializeComponent();
            }

            /// <param name="MesajNo">Anormal bir değer gelmesi durumunda en alta ekler.</param>
            public bool Ekle(Girdi_Mesaj_ Mesaj, int MesajNo = int.MaxValue)
            {
                if (Mesaj.SolaDayalıResim == null && Mesaj.Yazılar.Count == 0) return false;

                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.Ekle, MesajNo = MesajNo, Mesaj = Mesaj} );

                ArkaPlandaÇalışanYazılımıKontrolEt();
                return true;
            }
            public void Taşı(int MesajNo = 0, int Hedef_MesajNo = int.MaxValue)
            {
                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.Taşı, MesajNo = MesajNo, Değer = Hedef_MesajNo });

                ArkaPlandaÇalışanYazılımıKontrolEt();
            }
            public void Sil(int MesajNo = 0)
            {
                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.Sil, MesajNo = MesajNo });

                ArkaPlandaÇalışanYazılımıKontrolEt();
            }
            public Çıktı_Mesaj_ Oku(int MesajNo = 0)
            {
                if (Ekrandakiler.Count == 0) return null;

                if (MesajNo >= Ekrandakiler.Count) MesajNo = Ekrandakiler.Count - 1;
                if (MesajNo < 0) MesajNo = 0;

                Çıktı_Mesaj_ msg = new Çıktı_Mesaj_();

                msg.Pano = Ekrandakiler[MesajNo].Pano;
                msg.SolaDayalıResim = Ekrandakiler[MesajNo].SolaDayalıResim;
                msg.Yazılar = Ekrandakiler[MesajNo].Yazılar.ToArray();
                msg.Hatırlatıcı = Ekrandakiler[MesajNo].Hatırlatıcı;
                msg.Tanım = Ekrandakiler[MesajNo].Tanım;

                return msg;
            }
            public string Oku(int MesajNo = 0, int YazıNo = 0)
            {
                if (Ekrandakiler.Count == 0) return "";

                if (MesajNo >= Ekrandakiler.Count) MesajNo = Ekrandakiler.Count - 1;
                if (MesajNo < 0) MesajNo = 0;

                if (Ekrandakiler[MesajNo].Yazılar.Count == 0) return "";

                if (YazıNo >= Ekrandakiler[MesajNo].Yazılar.Count) YazıNo = Ekrandakiler[MesajNo].Yazılar.Count - 1;
                if (YazıNo < 0) YazıNo = 0;

                if (Ekrandakiler[MesajNo].Yazılar[YazıNo] is ProgressBar) return (Ekrandakiler[MesajNo].Yazılar[YazıNo] as ProgressBar).Value.ToString();
                else return Ekrandakiler[MesajNo].Yazılar[YazıNo].Text;
            }

            /// <summary>
            /// Yüzde tipi kullanılıyor ise Yazı değişkenine 0 ile 100 arasında bir sayının metin karşılığı girilmelidir.
            /// </summary>
            public void YazıyıDeğiştir(int MesajNo, int YazıNo, string Yazı)
            {
                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.YazıyıDeğiştir, MesajNo = MesajNo, YazıNo = YazıNo, Değer = Yazı });

                ArkaPlandaÇalışanYazılımıKontrolEt();
            }
            public void YazıyıDurgunlaştır(int MesajNo, int YazıNo)
            {
                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.YazıyıDurgunlaştır, MesajNo = MesajNo, YazıNo = YazıNo });

                ArkaPlandaÇalışanYazılımıKontrolEt();
            }
            public void YazıyıEtkinleştir(int MesajNo, int YazıNo)
            {
                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.YazıyıEtkinleştir, MesajNo = MesajNo, YazıNo = YazıNo });

                ArkaPlandaÇalışanYazılımıKontrolEt();
            }
            public void SolaDayalıResmiDeğiştir(int MesajNo, Image Resim)
            {
                Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.SolaDayalıResmiDeğiştir, MesajNo = MesajNo, Değer = Resim });

                ArkaPlandaÇalışanYazılımıKontrolEt();
            }

            public void Uyar(int MesajNo, int YazıNo, string Uyarı, ToolTipIcon İkon = ToolTipIcon.Warning, int ZamanAşımı = 10000, string Başlık = "ilgili MesajNo daki mesajın tanımı")
            {
                if (Ekrandakiler.Count == 0) return;

                if (MesajNo >= Ekrandakiler.Count) MesajNo = Ekrandakiler.Count - 1;
                if (MesajNo < 0) MesajNo = 0;

                if (Başlık == "ilgili MesajNo daki mesajın tanımı") this.Uyarı.ToolTipTitle = Ekrandakiler[MesajNo].Tanım;
                else this.Uyarı.ToolTipTitle = Başlık;

                this.Uyarı.ToolTipIcon = İkon;

                Control Eleman;
                if (YazıNo == -1)
                {
                    if (Ekrandakiler[MesajNo].SolaDayalıResim == null) return;
                    Eleman = Ekrandakiler[MesajNo].SolaDayalıResim;
                }
                else
                {
                    if (Ekrandakiler[MesajNo].Yazılar.Count == 0) return;

                    if (YazıNo >= Ekrandakiler[MesajNo].Yazılar.Count) YazıNo = Ekrandakiler[MesajNo].Yazılar.Count - 1;
                    if (YazıNo < 0) YazıNo = 0;

                    Eleman = Ekrandakiler[MesajNo].Yazılar[YazıNo];
                }

                if (AnaPano.InvokeRequired)
                {
                    AnaPano.Invoke(new Action(() =>
                    {
                        this.Uyarı.Show(string.Empty, Eleman);
                        this.Uyarı.Show(Uyarı, Eleman, ZamanAşımı);
                    }));
                }
                else
                {
                    this.Uyarı.Show(string.Empty, Eleman);
                    this.Uyarı.Show(Uyarı, Eleman, ZamanAşımı);
                }      
            }
            public bool YenidenÇizdir(int MesajNo = 0)
            {
                if (MesajNo < 0 || MesajNo >= Ekrandakiler.Count)
                {
                    for (int i = 0; i < Ekrandakiler.Count; i++)
                    {
                        Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.YenidenÇizdir, MesajNo = i });
                    }
                }
                else Bekleyenİşlemler.Add(new Bekleyenİşlem_() { İşlemTipi = Bekleyen_İşlem_Tipi_.YenidenÇizdir, MesajNo = MesajNo });

                ArkaPlandaÇalışanYazılımıKontrolEt();
                return true;
            }

            public bool EkranaYansımayanMesajVarMı(bool SilinenlerideDahilEt = false)
            {
                if (Bekleyenİşlemler.Count + 
                    (SilinenlerideDahilEt ? EkrandanSilinenler.Count : 0)
                    == 0) return false;

                ArkaPlandaÇalışanYazılımıKontrolEt();
                return true;
            }
            public int MesajNo(string Tanım)
            {
                for (int i = 0; i < Ekrandakiler.Count; i++)
                {
                    if (Ekrandakiler[i].Tanım == Tanım) return i;
                }
                return -1;
            }

            //////////////////////////////////////////////////////////////////////////////////////////
            
            void ArkaPlandaÇalışanYazılımıKontrolEt()
            {
                if (ArkaPlanGörevi == null ||
                    (ArkaPlanGörevi.ThreadState != ThreadState.Running && ArkaPlanGörevi.ThreadState != ThreadState.WaitSleepJoin)) 
                {
                    ArkaPlanGörevi = new Thread(ArkaPlandaÇalışanUygulama);
                    ArkaPlanGörevi.Start();  
                }

                if (ArkaPlanİşlemiÇalışsınMı == null) ArkaPlanİşlemiÇalışsınMı = new ManualResetEvent(true);
                else ArkaPlanİşlemiÇalışsınMı.Set();

                Çalışsın = true;
            }
            void ArkaPlandaÇalışanUygulama()
            {
                while (Çalışsın)
                {
                    Bekleyenİşlemler_Yap();

                    EkrandakileriHareketlendir();
                    EkrandanSilinenleriHareketlendir();

                    if (BekleyenEkranlama.Count > 0) EkranıTazele();
                    else
                    {
                        AnaPano.Invoke((Action)(() => { AnaPano.AutoScroll = true; }));

                        ArkaPlanİşlemiÇalışsınMı.WaitOne();
                        ArkaPlanİşlemiÇalışsınMı.Reset();
                    }
                }
            }

            void EkrandakileriHareketlendir()
            {       
                for (int i = 0; i < Ekrandakiler.Count; i++)
                {
                    if (Ekrandakiler[i].EkranGüncellemesiGerekiyor)
                    {
                        int GüncellemeDurumu = 0;
                        Point pt = Ekrandakiler[i].Pano.Location;

                        if (Ayarlar.CanlandırmaHızı_X == 0) pt.X = Ekrandakiler[i].HedefKonum.X;
                        if (Ayarlar.CanlandırmaHızı_Y == 0) pt.Y = Ekrandakiler[i].HedefKonum.Y;

                        if (pt.X > Ekrandakiler[i].HedefKonum.X)
                        {
                            pt.X -= Ayarlar.CanlandırmaHızı_X;
                            if (pt.X <= Ekrandakiler[i].HedefKonum.X) pt.X = Ekrandakiler[i].HedefKonum.X;
                        }
                        else if (pt.X < Ekrandakiler[i].HedefKonum.X)
                        {
                            pt.X += Ayarlar.CanlandırmaHızı_X;
                            if (pt.X >= Ekrandakiler[i].HedefKonum.X) pt.X = Ekrandakiler[i].HedefKonum.X;
                        }
                        else GüncellemeDurumu++;

                        if (pt.Y > Ekrandakiler[i].HedefKonum.Y)
                        {
                            pt.Y -= Ayarlar.CanlandırmaHızı_Y;
                            if (pt.Y <= Ekrandakiler[i].HedefKonum.Y) pt.Y = Ekrandakiler[i].HedefKonum.Y;
                        }
                        else if (pt.Y < Ekrandakiler[i].HedefKonum.Y)
                        {
                            pt.Y += Ayarlar.CanlandırmaHızı_Y;
                            if (pt.Y >= Ekrandakiler[i].HedefKonum.Y) pt.Y = Ekrandakiler[i].HedefKonum.Y;
                        }
                        else GüncellemeDurumu++;

                        if (GüncellemeDurumu == 2) Ekrandakiler[i].EkranGüncellemesiGerekiyor = false;

                        BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.PozisyonunuDeğiştir, Nesne = Ekrandakiler[i].Pano, Değer = pt });
                    }
                }
            }
            void EkrandanSilinenleriHareketlendir()
            {
                for (int i = 0; i < EkrandanSilinenler.Count; i++)
                {
                    Point pt = EkrandanSilinenler[i].Pano.Location;

                    if (Ayarlar.CanlandırmaHızı_X == 0) pt.X = EkrandanSilinenler[i].HedefKonum.X;

                    pt.X -= Ayarlar.CanlandırmaHızı_X;
                    if (pt.X <= EkrandanSilinenler[i].HedefKonum.X)
                    {
                        BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.Sil, Nesne = EkrandanSilinenler[i].Pano });
                        EkrandanSilinenler.RemoveAt(i);
                        continue;
                    }
                
                    BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.PozisyonunuDeğiştir, Nesne = EkrandanSilinenler[i].Pano, Değer = pt });
                }
            }
            void EkranıTazele()
            {
                AnaPano.Invoke((Action)(() =>
                {
                    foreach (var biri in BekleyenEkranlama)
                    {
                        switch (biri.EkranlamaTipi)
                        {
                            case Bekleyen_Ekranlama_Tipi_.Ekle:
                                AnaPano.Controls.Add(biri.Nesne as Panel);
                                (biri.Nesne as Panel).BringToFront();
                                break;
                            case Bekleyen_Ekranlama_Tipi_.ÖneGetir:
                                (biri.Nesne as Panel).BringToFront();
                                break;
                            case Bekleyen_Ekranlama_Tipi_.PozisyonunuDeğiştir:
                                (biri.Nesne as Panel).Location = (Point)biri.Değer;
                                break;
                            case Bekleyen_Ekranlama_Tipi_.Sil:
                                foreach (Control n in (biri.Nesne as Panel).Controls) n.Dispose();
                                (biri.Nesne as Panel).Controls.Clear();
                                (biri.Nesne as Panel).Dispose();
                                break;
                            case Bekleyen_Ekranlama_Tipi_.KaydırmaÇubuğunuGizle:
                                AnaPano.AutoScroll = false;
                                break;
                            case Bekleyen_Ekranlama_Tipi_.SolaDayalıResmiDeğiştir:
                                (biri.Nesne as PictureBox).Image = (Image)biri.Değer;
                                break;
                            case Bekleyen_Ekranlama_Tipi_.YazıyıDeğiştir:
                                int YüzdeDeğeri = 0;
                                if (biri.Nesne is ComboBox && (biri.Değer as string).Contains(";"))
                                {
                                    (biri.Nesne as ComboBox).Items.Clear();
                                    (biri.Nesne as ComboBox).Items.AddRange((biri.Değer as string).Split(';'));
                                }
                                else if (biri.Nesne is ProgressBar && int.TryParse((biri.Değer as string), out YüzdeDeğeri)) (biri.Nesne as ProgressBar).Value = YüzdeDeğeri;
                                else (biri.Nesne as Control).Text = (string)biri.Değer;
                                break;
                            case Bekleyen_Ekranlama_Tipi_.YazıyıDurgunlaştır:
                                (biri.Nesne as Control).Enabled = false;
                                break;
                            case Bekleyen_Ekranlama_Tipi_.YazıyıEtkinleştir:
                                (biri.Nesne as Control).Enabled = true;
                                break;
                        }
                    }

                    AnaPano.Refresh();
                }));

                BekleyenEkranlama.Clear();
            }

            void Bekleyenİşlemler_Yap()
            {
                if (Bekleyenİşlemler.Count > 0)
                {
                    if (Bekleyenİşlemler[0].MesajNo >= Ekrandakiler.Count)
                    {
                        if (Bekleyenİşlemler[0].İşlemTipi == Bekleyen_İşlem_Tipi_.Ekle) Bekleyenİşlemler[0].MesajNo = Ekrandakiler.Count;
                        else Bekleyenİşlemler[0].MesajNo = Ekrandakiler.Count - 1;
                    }
                    if (Bekleyenİşlemler[0].MesajNo < 0) Bekleyenİşlemler[0].MesajNo = 0;

                    switch (Bekleyenİşlemler[0].İşlemTipi)
                    {
                        case Bekleyen_İşlem_Tipi_.Ekle:
                            Bekleyenİşlemler_Ekle();
                            break;
                        case Bekleyen_İşlem_Tipi_.Taşı:
                            Bekleyenİşlemler_Taşı();
                            break;
                        case Bekleyen_İşlem_Tipi_.Sil:
                            if (Ekrandakiler.Count > 0) Bekleyenİşlemler_Sil();
                            break;
                        case Bekleyen_İşlem_Tipi_.YenidenÇizdir:
                            Bekleyenİşlemler_YenidenÇizdir();
                            break;
                        case Bekleyen_İşlem_Tipi_.SolaDayalıResmiDeğiştir:
                            Bekleyenİşlemler_SolaDayalıResmiDeğiştir();
                            break;
                        case Bekleyen_İşlem_Tipi_.YazıyıDeğiştir:
                            Bekleyenİşlemler_YazıyıDeğiştir();
                            break;
                        case Bekleyen_İşlem_Tipi_.YazıyıDurgunlaştır:
                            Bekleyenİşlemler_YazıyıDurgunlaştır();
                            break;
                        case Bekleyen_İşlem_Tipi_.YazıyıEtkinleştir:
                            Bekleyenİşlemler_YazıyıEtkinleştir();
                            break;
                    }
                    Bekleyenİşlemler.RemoveAt(0);

                    if (AnaPano.AutoScroll) BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.KaydırmaÇubuğunuGizle });
                }
            }
            void Bekleyenİşlemler_Ekle()
            {
                if (Bekleyenİşlemler[0].Mesaj.Yazılar.Count == 0) Bekleyenİşlemler[0].Mesaj.Yazılar.Add(new Girdi_Yazı_() { Metin = " " });

                Ekrandaki_ Ekrn = new Ekrandaki_();
                Ekrn.EkranGüncellemesiGerekiyor = true;
                Ekrn.Pano = new Panel();
                Ekrn.Pano.Tag = ++YaratılanPanoSayısı;
                Ekrn.Hatırlatıcı = Bekleyenİşlemler[0].Mesaj.Hatırlatıcı;
                Ekrn.Tanım = Bekleyenİşlemler[0].Mesaj.Tanım;

                Ekrn.Yazılar = new List<Control>(); 
                int EnUzunEtiketinUzunluğu = 0;
                int EtiketlerinToplamYüksekliği = 0;
                if (Bekleyenİşlemler[0].Mesaj.Yazılar != null)
                {
                    for (int i = 0; i < Bekleyenİşlemler[0].Mesaj.Yazılar.Count; i++)
                    {
                        Control YeniEtiket = null;
                        switch (Bekleyenİşlemler[0].Mesaj.Yazılar[i].Tip)
                        {
                            case Girdi_Yazı_Tip_.Basit:
                                YeniEtiket = new Label()
                                {
                                    AutoSize = true
                                };
                                break;
                            case Girdi_Yazı_Tip_.Değiştirilebilir:
                                YeniEtiket = new TextBox()
                                {
                                    Width = Ayarlar.Genişlik_Değiştirilebilir
                                };
                                break;
                            case Girdi_Yazı_Tip_.Tuş:
                                YeniEtiket = new Button()
                                {
                                    AutoSize = true
                                };
                                Bekleyenİşlemler[0].Mesaj.Yazılar[i].Üyelik_Tıklandı = true; 
                                break;
                            case Girdi_Yazı_Tip_.Onay:
                                YeniEtiket = new CheckBox()
                                {
                                    AutoSize = true
                                };
                                break;
                            case Girdi_Yazı_Tip_.Seçim:
                                YeniEtiket = new RadioButton()
                                {
                                    AutoSize = true
                                };
                                break;
                            case Girdi_Yazı_Tip_.Yüzde:
                                YeniEtiket = new ProgressBar()
                                {
                                    Width = Ayarlar.Genişlik_Yüzde,
                                    Maximum = 100,
                                    Value = 0
                                };
                                break;
                            case Girdi_Yazı_Tip_.Grup:
                                YeniEtiket = new ComboBox()
                                {
                                    Width = Ayarlar.Genişlik_Grup,
                                    DropDownStyle = ComboBoxStyle.DropDownList
                                };
                                if (!string.IsNullOrEmpty(Bekleyenİşlemler[0].Mesaj.Yazılar[i].Metin))
                                {
                                    if (Bekleyenİşlemler[0].Mesaj.Yazılar[i].Metin.Contains(";")) (YeniEtiket as ComboBox).Items.AddRange(Bekleyenİşlemler[0].Mesaj.Yazılar[i].Metin.Split(';'));
                                    else (YeniEtiket as ComboBox).Items.Add(Bekleyenİşlemler[0].Mesaj.Yazılar[i].Metin);
                                    (YeniEtiket as ComboBox).SelectedIndex = 0;
                                }
                                break;
                        }

                        if (YeniEtiket.GetType() != typeof(ComboBox)) YeniEtiket.Text = Bekleyenİşlemler[0].Mesaj.Yazılar[i].Metin;
                        if (!string.IsNullOrEmpty(Bekleyenİşlemler[0].Mesaj.Yazılar[i].İpucu)) İpucu.SetToolTip(YeniEtiket, Bekleyenİşlemler[0].Mesaj.Yazılar[i].İpucu);
                        if (Bekleyenİşlemler[0].Mesaj.Yazılar[i].Görünüm != null) YeniEtiket.Font = Bekleyenİşlemler[0].Mesaj.Yazılar[i].Görünüm;
                        else if (Ayarlar.Varsayılan_Yazı_GörselDüzen != null) YeniEtiket.Font = Ayarlar.Varsayılan_Yazı_GörselDüzen;
                        if (i > 0) YeniEtiket.Location = new Point(0, Ekrn.Yazılar[i - 1].Location.Y + Ekrn.Yazılar[i - 1].Size.Height + Ayarlar.İkiYazıArasıMesafe);
                        else YeniEtiket.Location = new Point(0, 0);
                        if (Bekleyenİşlemler[0].Mesaj.Yazılar[i].Üyelik_Tıklandı)
                        {
                            YeniEtiket.Click += Tıklandı;
                            YeniEtiket.Tag = i;
                            YeniEtiket.Cursor = Cursors.Hand;
                        }
                        if (Bekleyenİşlemler[0].Mesaj.Yazılar[i].Üyelik_YazıDeğiştirildi)
                        {
                            YeniEtiket.TextChanged += YazıDeğişti;
                            YeniEtiket.Tag = i;
                        }

                        Ekrn.Yazılar.Add(YeniEtiket);
                        Ekrn.Pano.Controls.Add(YeniEtiket);

                        if (YeniEtiket.Width > EnUzunEtiketinUzunluğu) EnUzunEtiketinUzunluğu = YeniEtiket.Width;
                    }
                    EtiketlerinToplamYüksekliği = Ekrn.Yazılar[Ekrn.Yazılar.Count - 1].Location.Y + Ekrn.Yazılar[Ekrn.Yazılar.Count - 1].Height;
                }
                
                int ResiminYüksekliği = 0;
                int ResiminUzunluğu = 0;
                if (Bekleyenİşlemler[0].Mesaj.SolaDayalıResim != null)
                {
                    Ekrn.SolaDayalıResim = new PictureBox();

                    if (!string.IsNullOrEmpty(Bekleyenİşlemler[0].Mesaj.SolaDayalıResim.İpucu)) İpucu.SetToolTip(Ekrn.SolaDayalıResim, Bekleyenİşlemler[0].Mesaj.SolaDayalıResim.İpucu);

                    if (Bekleyenİşlemler[0].Mesaj.SolaDayalıResim.Resim != null) Ekrn.SolaDayalıResim.Image = Bekleyenİşlemler[0].Mesaj.SolaDayalıResim.Resim;
                    else if (Ayarlar.Varsayılan_Resim != null) Ekrn.SolaDayalıResim.Image = Ayarlar.Varsayılan_Resim;
                    
                    if (Ayarlar.Varsayılan_Resim_Boyut.IsEmpty) Ekrn.SolaDayalıResim.SizeMode = PictureBoxSizeMode.Zoom;
                    else Ekrn.SolaDayalıResim.SizeMode = PictureBoxSizeMode.StretchImage;

                    if (Ayarlar.Varsayılan_Resim_Boyut.Height <= 0) Ekrn.SolaDayalıResim.Height = EtiketlerinToplamYüksekliği;
                    else if (Ayarlar.Varsayılan_Resim_Boyut.Height > 0) Ekrn.SolaDayalıResim.Height = Ayarlar.Varsayılan_Resim_Boyut.Height;

                    if (Ayarlar.Varsayılan_Resim_Boyut.Width <= 0) Ekrn.SolaDayalıResim.Width = EtiketlerinToplamYüksekliği;
                    else if (Ayarlar.Varsayılan_Resim_Boyut.Width > 0) Ekrn.SolaDayalıResim.Width = Ayarlar.Varsayılan_Resim_Boyut.Width;

                    int fark = 0;
                    if (EtiketlerinToplamYüksekliği > Ekrn.SolaDayalıResim.Height) Ekrn.SolaDayalıResim.Location = new Point(0, (EtiketlerinToplamYüksekliği - Ekrn.SolaDayalıResim.Height) / 2);
                    else if (EtiketlerinToplamYüksekliği < Ekrn.SolaDayalıResim.Height) fark = (Ekrn.SolaDayalıResim.Height - EtiketlerinToplamYüksekliği) / 2;

                    Ekrn.Pano.Controls.Add(Ekrn.SolaDayalıResim);

                    foreach (var etkt in Ekrn.Yazılar) etkt.Location = new Point(Ekrn.SolaDayalıResim.Width + Ayarlar.ResimİleYazıArasıMesafe, etkt.Location.Y + fark);

                    ResiminYüksekliği = Ekrn.SolaDayalıResim.Height;
                    ResiminUzunluğu = Ekrn.SolaDayalıResim.Width;

                    if (Bekleyenİşlemler[0].Mesaj.SolaDayalıResim.Tıklanabilir)
                    {
                        Ekrn.SolaDayalıResim.Click += Tıklandı;
                        Ekrn.SolaDayalıResim.Tag = -1;
                        Ekrn.SolaDayalıResim.Cursor = Cursors.Hand;
                    }
                }

                Ekrn.Pano.Height = EtiketlerinToplamYüksekliği > ResiminYüksekliği ? EtiketlerinToplamYüksekliği : ResiminYüksekliği;
                Ekrn.Pano.Width = ResiminUzunluğu + Ayarlar.ResimİleYazıArasıMesafe + EnUzunEtiketinUzunluğu;
                if (Ayarlar.Çerçeveli)
                {
                    Ekrn.Pano.Height += 3;
                    Ekrn.Pano.BorderStyle = BorderStyle.FixedSingle;
                    Ekrn.Pano.Width = AnaPano.Width - 1;
                    Ekrn.Pano.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
                }
                else { Ekrn.Pano.AutoSizeMode = AutoSizeMode.GrowAndShrink; Ekrn.Pano.AutoSize = true; }

                if (Bekleyenİşlemler[0].MesajNo == 0)
                {
                    Ekrn.Pano.Location = new Point(AnaPano.Width + 10, 0);
                    Ekrn.HedefKonum = new Point(0, 0);
                }
                else
                {
                    Ekrn.Pano.Location = new Point(AnaPano.Width + 10, Ekrandakiler[Bekleyenİşlemler[0].MesajNo - 1].HedefKonum.Y + Ekrandakiler[Bekleyenİşlemler[0].MesajNo - 1].Pano.Height + Ayarlar.İkiMesajArasıMesafe);
                    Ekrn.HedefKonum = new Point(0, Ekrn.Pano.Location.Y);
                }

                for (int i = Bekleyenİşlemler[0].MesajNo; i < Ekrandakiler.Count; i++)
                {
                    Ekrandakiler[i].HedefKonum.Y += Ekrn.Pano.Height + Ayarlar.İkiMesajArasıMesafe;
                    Ekrandakiler[i].EkranGüncellemesiGerekiyor = true;
                }

                Ekrandakiler.Insert(Bekleyenİşlemler[0].MesajNo, Ekrn);
                
                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.Ekle, Nesne = Ekrn.Pano });
            }
            void Bekleyenİşlemler_Sil()
            {
                int EnUzunMalzeme = 0;
                foreach (Control mlz in Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano.Controls)
                {
                    if (EnUzunMalzeme < mlz.Width) EnUzunMalzeme = mlz.Width;
                }

                Ekrandakiler[Bekleyenİşlemler[0].MesajNo].HedefKonum.X -= EnUzunMalzeme + 10;
                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.ÖneGetir, Nesne = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano });

                for (int i = Bekleyenİşlemler[0].MesajNo + 1; i < Ekrandakiler.Count; i++)
                {
                    Ekrandakiler[i].HedefKonum.Y -= Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano.Height + Ayarlar.İkiMesajArasıMesafe;
                    Ekrandakiler[i].EkranGüncellemesiGerekiyor = true;
                }

                EkrandanSilinenler.Add(Ekrandakiler[Bekleyenİşlemler[0].MesajNo]);
                Ekrandakiler.RemoveAt(Bekleyenİşlemler[0].MesajNo);
            } 
            void Bekleyenİşlemler_Taşı()
            {
                int kaynak = Bekleyenİşlemler[0].MesajNo;
                int hedef = (int)Bekleyenİşlemler[0].Değer;

                if (hedef >= Ekrandakiler.Count) hedef = Ekrandakiler.Count - 1;
                if (hedef < 0) hedef = 0;

                if (kaynak == hedef) return;

                Ekrandakiler[kaynak].EkranGüncellemesiGerekiyor = true;
                
                if (kaynak > hedef)
                {
                    //Yukarı Çıkıyor

                    Ekrandakiler[kaynak].HedefKonum = Ekrandakiler[hedef].HedefKonum;

                    for (int i = hedef; i < kaynak; i++)
                    {
                        Ekrandakiler[i].HedefKonum.Y += Ekrandakiler[kaynak].Pano.Height + Ayarlar.İkiMesajArasıMesafe;
                        Ekrandakiler[i].EkranGüncellemesiGerekiyor = true;
                    }
                }
                else
                {
                    //Aşağı iniyor

                    int Mesafe = 0;
                    for (int i = kaynak + 1; i <= hedef; i++)
                    {
                        Mesafe += Ekrandakiler[i].Pano.Height + Ayarlar.İkiMesajArasıMesafe;

                        Ekrandakiler[i].HedefKonum.Y -= Ekrandakiler[kaynak].Pano.Height + Ayarlar.İkiMesajArasıMesafe;
                        Ekrandakiler[i].EkranGüncellemesiGerekiyor = true;
                    }

                    Ekrandakiler[kaynak].HedefKonum.Y += Mesafe;
                }

                Ekrandaki_ gecici = Ekrandakiler[kaynak];
                Ekrandakiler.RemoveAt(kaynak);
                Ekrandakiler.Insert(hedef, gecici);
                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.ÖneGetir, Nesne = gecici.Pano });
            }
            void Bekleyenİşlemler_YenidenÇizdir()
            {
                AnaPano.Invoke((Action)(() =>
                {
                    Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano.BringToFront();

                    int EtiketlerinToplamYüksekliği = 0;
                    int EnUzunEtiketinUzunluğu = 0;
                    Point[] EtiketlerinKonumu = new Point[Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar.Count];
                    for (int i = 0; i < Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar.Count; i++)
                    {
                        EtiketlerinKonumu[i] = new Point(Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[i].Location.X, EtiketlerinToplamYüksekliği);
                        EtiketlerinToplamYüksekliği += Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[i].Height + Ayarlar.İkiYazıArasıMesafe;
                        if (Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[i].Width > EnUzunEtiketinUzunluğu) EnUzunEtiketinUzunluğu = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[i].Width;
                    }
                    EtiketlerinToplamYüksekliği -= Ayarlar.İkiYazıArasıMesafe;

                    int ResiminYüksekliği = 0;
                    int ResiminUzunluğu = 0;
                    if (Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim != null)
                    {
                        int YazılarınKonumu_X = 0, YazılarınKonumu_Y = 0;

                        if (Ayarlar.Varsayılan_Resim_Boyut.Height == 0) Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height = EtiketlerinToplamYüksekliği;
                        else if (Ayarlar.Varsayılan_Resim_Boyut.Height > 0) Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height = Ayarlar.Varsayılan_Resim_Boyut.Height;

                        if (Ayarlar.Varsayılan_Resim_Boyut.Width == 0) Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Width = EtiketlerinToplamYüksekliği;
                        else if (Ayarlar.Varsayılan_Resim_Boyut.Width > 0) Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Width = Ayarlar.Varsayılan_Resim_Boyut.Width;

                        YazılarınKonumu_X = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Width + Ayarlar.ResimİleYazıArasıMesafe;

                        if (Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height < EtiketlerinToplamYüksekliği)
                        {
                            int fark = EtiketlerinToplamYüksekliği - Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height;
                            fark /= 2;

                            Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Location = new Point(0, fark);
                        }
                        else if (Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height > EtiketlerinToplamYüksekliği)
                        {
                            int fark = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height - EtiketlerinToplamYüksekliği;
                            YazılarınKonumu_Y = fark / 2;

                            Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Location = new Point(0, 0);
                        }

                        for (int i = 0; i < Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar.Count; i++)
                        {
                            Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[i].Location = new Point(YazılarınKonumu_X, EtiketlerinKonumu[i].Y + YazılarınKonumu_Y);
                        }

                        ResiminYüksekliği = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Height;
                        ResiminUzunluğu = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim.Width;
                    }

                    Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano.Height = EtiketlerinToplamYüksekliği > ResiminYüksekliği ? EtiketlerinToplamYüksekliği : ResiminYüksekliği;
                    if (!Ayarlar.Çerçeveli) Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano.Width = ResiminUzunluğu + Ayarlar.ResimİleYazıArasıMesafe + EnUzunEtiketinUzunluğu;
                    else Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Pano.Height += 3;
                }));

                if (Bekleyenİşlemler[0].MesajNo > 0)
                {
                    for (int i = Bekleyenİşlemler[0].MesajNo + 1; i < Ekrandakiler.Count; i++)
                    {
                        Ekrandakiler[i].HedefKonum.Y = Ekrandakiler[i-1].HedefKonum.Y + Ekrandakiler[i - 1].Pano.Height + Ayarlar.İkiMesajArasıMesafe;
                        Ekrandakiler[i].EkranGüncellemesiGerekiyor = true;
                    }
                }
            }
            void Bekleyenİşlemler_SolaDayalıResmiDeğiştir()
            {
                if (Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim == null) return;

                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.SolaDayalıResmiDeğiştir, Nesne = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].SolaDayalıResim, Değer = Bekleyenİşlemler[0].Değer });
            }
            void Bekleyenİşlemler_YazıyıDeğiştir()
            {
                if (Bekleyenİşlemler[0].YazıNo >= Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar.Count) return;

                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.YazıyıDeğiştir, Nesne = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[Bekleyenİşlemler[0].YazıNo], Değer = Bekleyenİşlemler[0].Değer });
            }
            void Bekleyenİşlemler_YazıyıDurgunlaştır()
            {
                if (Bekleyenİşlemler[0].YazıNo >= Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar.Count) return;

                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.YazıyıDurgunlaştır, Nesne = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[Bekleyenİşlemler[0].YazıNo] });
            }
            void Bekleyenİşlemler_YazıyıEtkinleştir()
            {
                if (Bekleyenİşlemler[0].YazıNo >= Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar.Count) return;

                BekleyenEkranlama.Add(new BekleyenEkranlama_() { EkranlamaTipi = Bekleyen_Ekranlama_Tipi_.YazıyıEtkinleştir, Nesne = Ekrandakiler[Bekleyenİşlemler[0].MesajNo].Yazılar[Bekleyenİşlemler[0].YazıNo] });
            }

            #region Designer Eklntileri
            System.ComponentModel.IContainer components = null;
            Panel AnaPano;
            ToolTip İpucu, Uyarı;

            void InitializeComponent()
            {
                components = new System.ComponentModel.Container();
                AnaPano = new Panel();
                İpucu = new ToolTip(components);
                Uyarı = new ToolTip(components);
                SuspendLayout();
                // 
                // AnaPano
                // 
                AnaPano.AutoScroll = true;
                AnaPano.Dock = DockStyle.Fill;
                AnaPano.Location = new Point(0, 0);
                AnaPano.Name = "AnaPano";
                AnaPano.Size = new Size(50, 50);
                AnaPano.TabIndex = 0;
                AnaPano.MouseEnter += MesajPanosu_MouseEnter;
                // 
                // MesajPanosu_
                // 
                AutoScaleDimensions = new SizeF(6F, 13F);
                AutoScaleMode = AutoScaleMode.Font;
                Controls.Add(AnaPano);
                Name = "MesajPanosu_";
                Size = new Size(50, 50);
                ResumeLayout(false);
                // 
                // İpucu
                // 
                İpucu.IsBalloon = true;
                İpucu.InitialDelay = 0;
                // 
                // Uyarı
                // 
                Uyarı.IsBalloon = true;
            }
            void MesajPanosu_MouseEnter(object sender, EventArgs e)
            {
                if (AnaPano.VerticalScroll.Visible) AnaPano.Focus();
            }
            void Tıklandı(object sender, EventArgs e)
            {
                int YazıNo = (int)((sender as Control).Tag);
                int MesajYaratılışNo = (int)((sender as Control).Parent.Tag);

                int MesajNo;
                for (MesajNo = 0; MesajNo < Ekrandakiler.Count; MesajNo++)
                {
                    if ((int)(Ekrandakiler[MesajNo].Pano.Tag) == MesajYaratılışNo) break;
                }
                if (MesajNo == Ekrandakiler.Count) return;

                try { Tıklandıİşlemi(MesajNo, YazıNo); } catch (Exception) { }
            }
            void YazıDeğişti(object sender, EventArgs e)
            {
                int YazıNo = (int)((sender as Control).Tag);
                int MesajYaratılışNo = (int)((sender as Control).Parent.Tag);

                int MesajNo;
                for (MesajNo = 0; MesajNo < Ekrandakiler.Count; MesajNo++)
                {
                    if ((int)(Ekrandakiler[MesajNo].Pano.Tag) == MesajYaratılışNo) break;
                }
                if (MesajNo == Ekrandakiler.Count) return;

                try { YazıDeğiştiİşlemi(MesajNo, YazıNo); } catch (Exception) { }
            }
            protected override void Dispose(bool disposing)
            {
                Çalışsın = false;
                if (ArkaPlanİşlemiÇalışsınMı != null) ArkaPlanİşlemiÇalışsınMı.Set();

                if (disposing && (components != null))
                {
                    components.Dispose();
                }
                base.Dispose(disposing);

                if (ArkaPlanGörevi != null) { ArkaPlanGörevi.Abort(); ArkaPlanGörevi = null; }
                if (ArkaPlanİşlemiÇalışsınMı != null) { ArkaPlanİşlemiÇalışsınMı.Dispose(); ArkaPlanİşlemiÇalışsınMı = null; }
                if (Tıklandıİşlemi != null) { Tıklandıİşlemi = null; }

                Bekleyenİşlemler.Clear();
                Ekrandakiler.Clear();
                EkrandanSilinenler.Clear();
                BekleyenEkranlama.Clear();
        }
            #endregion
        }
    }
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO.Pipes;
using System.Threading;
using System.ComponentModel;
using ArgeMup.HazirKod.Dönüştürme;

namespace ArgeMup.HazirKod
{
    public class UygulamalarArasıHaberlesme_ : IDisposable
    {
        public const string Sürüm = "V1.1";

        public UygulamalarArasıHaberlesme_()
        {
            Ortak.Alıcı_Durumu = AlıcıDurum_.Etkin;
            Ortak.Alıcı_İzinVerilenGöndericilerListesi = null;
            Ortak.Adı = "";
            Ortak.GelenKutusu_Öncelikli = new List<Alınan_Mesaj_>();
            Ortak.Alıcı = null;
            Ortak.Zamanlayıcı_GelenKutusu = null;
            Ortak.Zamanlayıcı_GidenKutusu = null;
            Ortak.Alıcı_AzamiSayısı = 0;

            GelenKutusu = null;
            GidenKutusu = null;

            HataSebebi = "";
        }

        #region Ortak
        struct Ortak_
        {
            public string Adı;
            public AlıcıDurum_ Alıcı_Durumu;
            public List<string> Alıcı_İzinVerilenGöndericilerListesi;
            public List<Alınan_Mesaj_> GelenKutusu_Öncelikli;

            public NamedPipeServerStream[] Alıcı;
            public int Alıcı_AzamiSayısı;
            public int HaberleşmeZamanAşımı_msn;

            public bool Durdur;
            public System.Timers.Timer Zamanlayıcı_GelenKutusu;
            public System.Timers.Timer Zamanlayıcı_GidenKutusu;
        };
        Ortak_ Ortak = new Ortak_();
        public List<Alınan_Mesaj_> GelenKutusu;
        public BindingList<Gönderilecek_Mesaj_> GidenKutusu;   
        public string HataSebebi;

        public bool Baslat(string KendiAdı, AlıcıDurum_ AlıcınınDurumu = AlıcıDurum_.Etkin, _CallBack_ÖncelikliMesajİşleyicisi_ ÖncelikliMesajİşleyicisi = null, List<string> İzinVerilenGöndericilerListesi = null, int HaberleşmeZamanAşımı_msn = 5000, int İzinVerilenAlıcıSayısı = 2)
        {
            try
            {
                HataSebebi = "";
                if (KendiAdı == "") HataSebebi += "-KendiAdı Boş Olamaz.";
                if (AlıcınınDurumu == AlıcıDurum_.İzinVerilenGöndericilerListesineBağlı && İzinVerilenGöndericilerListesi == null) HataSebebi += "-İzinVerilenGöndericilerListesi Boş Olamaz.";
                if (HaberleşmeZamanAşımı_msn < 100) HataSebebi += "-AlıcınınZamanAşımı 100 ms den Az Olamaz.";

                if (Ortak.Adı != "" && Ortak.Adı != KendiAdı) HataSebebi += "-Çalışırken KendiAdı değiştirilemez.";
                if (Ortak.Adı.Length > 256) HataSebebi += "-KendiAdı 256 karakterden fazla olamaz.";

                if (HataSebebi != "") return false;

                if (ÖncelikliMesajİşleyicisi_ == null) ÖncelikliMesajİşleyicisi_ += ÖncelikliMesajİşleyicisi;

                if (GelenKutusu == null) GelenKutusu = new List<Alınan_Mesaj_>();

                if (GidenKutusu == null)
                {
                    GidenKutusu = new BindingList<Gönderilecek_Mesaj_>();
                    GidenKutusu.AddingNew += new AddingNewEventHandler(GönderilecekMesajListesineElemanEklendi);
                }

                if (Ortak.Zamanlayıcı_GelenKutusu == null)
                {
                    Ortak.Zamanlayıcı_GelenKutusu = new System.Timers.Timer(1);
                    Ortak.Zamanlayıcı_GelenKutusu.Elapsed += Zamanlayıcıİşlemleri_GelenKutusu;
                    Ortak.Zamanlayıcı_GelenKutusu.Enabled = true;
                }

                if (Ortak.Zamanlayıcı_GidenKutusu == null)
                {
                    Ortak.Zamanlayıcı_GidenKutusu = new System.Timers.Timer(1);
                    Ortak.Zamanlayıcı_GidenKutusu.Elapsed += Zamanlayıcıİşlemleri_GidenKutusu;
                    Ortak.Zamanlayıcı_GidenKutusu.Enabled = true;
                }

                Ortak.Adı = KendiAdı;
                Ortak.Alıcı_Durumu = AlıcınınDurumu;
                Ortak.Alıcı_İzinVerilenGöndericilerListesi = İzinVerilenGöndericilerListesi;
                Ortak.HaberleşmeZamanAşımı_msn = HaberleşmeZamanAşımı_msn;
                Ortak.Durdur = false;

                Ortak.Alıcı_AzamiSayısı = İzinVerilenAlıcıSayısı;
                Ortak.Alıcı = new NamedPipeServerStream[Ortak.Alıcı_AzamiSayısı];
                for (int i = 0; i < Ortak.Alıcı_AzamiSayısı; i++) Alıcı_Başlat(i);

                Ortak.Zamanlayıcı_GelenKutusu.Start();
                Ortak.Zamanlayıcı_GidenKutusu.Start();
                return true;
            }
            catch (Exception ex) { HataSebebi += "-" + ex.Message; }

            Durdur();
            return false;
        }
        public void Durdur()
        {
            try
            {
                Ortak.Durdur = true;
                Ortak.Adı = "";

                if (GelenKutusu != null && Ortak.GelenKutusu_Öncelikli != null)
                {
                    GelenKutusu.AddRange(Ortak.GelenKutusu_Öncelikli);
                    Ortak.GelenKutusu_Öncelikli.Clear();
                }

                if (Ortak.Zamanlayıcı_GelenKutusu != null) Ortak.Zamanlayıcı_GelenKutusu.Stop();
                if (Ortak.Zamanlayıcı_GidenKutusu != null) Ortak.Zamanlayıcı_GidenKutusu.Stop();

                if (Ortak.Alıcı != null)
                {
                    for (int i = 0; i < Ortak.Alıcı_AzamiSayısı; i++)
                    {
                        if (Ortak.Alıcı[i] != null)
                        {
                            if (Ortak.Alıcı[i].IsConnected) Ortak.Alıcı[i].Disconnect();
                            Ortak.Alıcı[i].Dispose();
                            Ortak.Alıcı[i] = null;
                        }
                    }
                    Ortak.Alıcı = null;
                }
            }
            catch (Exception) { }
        }
        
        void Biriktir(ref byte[] Çıktı, string Girdi)
        {
            Biriktir(ref Çıktı, D_Yazı.BaytDizisine(Girdi));
        }
        void Biriktir(ref byte[] Çıktı, byte[] Girdi)
        {
            byte[] Adet = new byte[4];
            Adet[0] = (byte)(Girdi.Length >> 24);
            Adet[1] = (byte)(Girdi.Length >> 16);
            Adet[2] = (byte)(Girdi.Length >> 08);
            Adet[3] = (byte)(Girdi.Length >> 00);

            long Csm = Adet.Sum(x => (long)x);
            Csm += Girdi.Sum(x => (long)x);

            byte[] Csm_ = new byte[4];
            Csm_[0] = (byte)(Csm >> 24);
            Csm_[1] = (byte)(Csm >> 16);
            Csm_[2] = (byte)(Csm >> 08);
            Csm_[3] = (byte)(Csm >> 00);

            Çıktı = Çıktı.Concat(Adet).ToArray();
            Çıktı = Çıktı.Concat(Girdi).ToArray();
            Çıktı = Çıktı.Concat(Csm_).ToArray();
        }
        #endregion

        #region Gönderici                
        public enum GöndericiDurum_ { İşlemGörmedi = 1, BağlantıKurulamıyor, ZamanAşımıOluştu, Başarılı, ParametreUygunsuzluğu, AlıcıPasif, İzinVerilenGöndericilerListesiEngelledi };
        public class Gönderilecek_Mesaj_
        {
            public string BilgisayarAdıVeyaIp = ".";
            public string AlıcınınAdı = "Mecburi";
            public string Konu = "Mecburi";
            public byte[] Mesaj = new byte[0];
            public bool ÖncelikliMesaj = false;
            public int İlkBağlantıZamanAşımı_msn = 10000;

            public GöndericiDurum_ Durum = GöndericiDurum_.İşlemGörmedi;
            public object Hatırlatıcı = null;
            public int KalanDenemeSayısı = int.MaxValue;
        }

        public bool HemenGonder(ref Gönderilecek_Mesaj_ Gönderilecek_Mesaj)
        {
            NamedPipeClientStream Gönderici = null;
            bool sonuç = false;
            try
            {
                if (string.IsNullOrEmpty(Ortak.Adı) ||
                    string.IsNullOrEmpty(Gönderilecek_Mesaj.AlıcınınAdı) ||
                    string.IsNullOrEmpty(Gönderilecek_Mesaj.Konu) )
                {
                    Gönderilecek_Mesaj.Durum = GöndericiDurum_.ParametreUygunsuzluğu; 
                    return false;
                }

                try
                {
                    Gönderici = new NamedPipeClientStream(Gönderilecek_Mesaj.BilgisayarAdıVeyaIp, Gönderilecek_Mesaj.AlıcınınAdı, PipeDirection.InOut, PipeOptions.Asynchronous);
                    Gönderici.Connect(Gönderilecek_Mesaj.İlkBağlantıZamanAşımı_msn);
                }
                catch (Exception) { Gönderilecek_Mesaj.Durum = GöndericiDurum_.BağlantıKurulamıyor; return false; }

                string cevap_metin = Oku(Gönderici, Ortak.HaberleşmeZamanAşımı_msn);
                int cevap_rakam = 0;
                if (cevap_metin != "_HosGeldin_")
                {
                    if (!cevap_metin.StartsWith("_") || !cevap_metin.EndsWith("_")) { Gönderilecek_Mesaj.Durum = GöndericiDurum_.ZamanAşımıOluştu; goto Çıkış; }
                    cevap_metin = cevap_metin.Trim('_');
                    int.TryParse(cevap_metin, out cevap_rakam);
                    if (cevap_rakam == 0 || cevap_rakam > (int)GöndericiDurum_.İzinVerilenGöndericilerListesiEngelledi) { Gönderilecek_Mesaj.Durum = GöndericiDurum_.ZamanAşımıOluştu; goto Çıkış; }

                    Gönderilecek_Mesaj.Durum = (GöndericiDurum_)cevap_rakam;
                    goto Çıkış;
                }

                byte[] AraTampon = new byte[0];
                Biriktir(ref AraTampon, "_hOSbULDUM_");
                Biriktir(ref AraTampon, Environment.MachineName);
                Biriktir(ref AraTampon, Ortak.Adı);
                Biriktir(ref AraTampon, Gönderilecek_Mesaj.Konu);
                Biriktir(ref AraTampon, Gönderilecek_Mesaj.ÖncelikliMesaj.ToString());
                Biriktir(ref AraTampon, Gönderilecek_Mesaj.Mesaj.Length.ToString());
                if (!Yaz(Gönderici, Ortak.HaberleşmeZamanAşımı_msn, AraTampon, AraTampon.Length)) goto Çıkış;

                cevap_metin = Oku(Gönderici, Ortak.HaberleşmeZamanAşımı_msn);
                if (cevap_metin != "_Basla_")
                {
                    if (!cevap_metin.StartsWith("_") || !cevap_metin.EndsWith("_")) { Gönderilecek_Mesaj.Durum = GöndericiDurum_.ZamanAşımıOluştu; goto Çıkış; }
                    cevap_metin = cevap_metin.Trim('_');
                    int.TryParse(cevap_metin, out cevap_rakam);
                    if (cevap_rakam == 0 || cevap_rakam > (int)GöndericiDurum_.İzinVerilenGöndericilerListesiEngelledi) { Gönderilecek_Mesaj.Durum = GöndericiDurum_.ZamanAşımıOluştu; goto Çıkış; }

                    Gönderilecek_Mesaj.Durum = (GöndericiDurum_)cevap_rakam;
                    goto Çıkış;
                }

                if (!Yaz(Gönderici, Ortak.HaberleşmeZamanAşımı_msn, Gönderilecek_Mesaj.Mesaj)) goto Çıkış;
                if (Oku(Gönderici, Ortak.HaberleşmeZamanAşımı_msn) != "_Bitir_") goto Çıkış;
                if (!Yaz(Gönderici, Ortak.HaberleşmeZamanAşımı_msn, "_Bitti_")) goto Çıkış;

                Gönderilecek_Mesaj.Durum = GöndericiDurum_.Başarılı;
                sonuç = true;  
            }
            catch (Exception) { Gönderilecek_Mesaj.Durum = GöndericiDurum_.ParametreUygunsuzluğu;  }

            Çıkış:
            try { Gönderici.Close(); } catch (Exception) { }

            return sonuç;
        }
        void Zamanlayıcıİşlemleri_GidenKutusu(object sender, EventArgs e)
        {
            Ortak.Zamanlayıcı_GidenKutusu.Stop();

            for (int i = 0; i < GidenKutusu.Count && !Ortak.Durdur;)
            {
                try
                {
                    Gönderilecek_Mesaj_ Mesaj = GidenKutusu[i];
                    if (Mesaj.Durum < GöndericiDurum_.Başarılı)
                    {
                        if (Mesaj.KalanDenemeSayısı > 0)
                        {
                            Mesaj.KalanDenemeSayısı--;

                            if (HemenGonder(ref Mesaj)) { GidenKutusu.RemoveAt(i); continue; }
                            else if (Mesaj.Durum > GöndericiDurum_.Başarılı) Mesaj.Mesaj = new byte[0];
                        }
                    }
                }
                catch (Exception) { }

                i++;
            }

            Ortak.Zamanlayıcı_GidenKutusu.Interval = 60000;
            Ortak.Zamanlayıcı_GidenKutusu.Start();
        }
        void GönderilecekMesajListesineElemanEklendi(object sender, AddingNewEventArgs e)
        {
            Ortak.Zamanlayıcı_GidenKutusu.Interval = 1;
        }
        public int GidenKutusu_KonudanMesajıBul(string Konu)
        {
            try
            {
                for (int i = 0; i < GidenKutusu.Count; i++)
                {
                    if (GidenKutusu[i].Konu == Konu) return i;
                }
            }
            catch (Exception) { }
            return -1;
        }

        string Oku(NamedPipeClientStream Akış, int ZamanAşımı_msn)
        {
            byte[] Okunan = null;
            if (Oku(Akış, ZamanAşımı_msn, out Okunan)) return D_Yazı.BaytDizisinden(Okunan);
            else return "";
        }
        bool Oku(NamedPipeClientStream Akış, int ZamanAşımı_msn, out byte[] Çıktı)
        {
            Çıktı = null;

            if (!Oku(Akış, ZamanAşımı_msn, out Çıktı, 4)) return false;
            long Csm = Çıktı.Sum(x => (long)x);

            long Adet;
            Adet = Çıktı[0] << 24;
            Adet += Çıktı[1] << 16;
            Adet += Çıktı[2] << 8;
            Adet += Çıktı[3] << 0;

            if (!Oku(Akış, ZamanAşımı_msn, out Çıktı, (int)Adet)) return false;
            Csm += Çıktı.Sum(x => (long)x);

            byte[] AlınanCsm = null;
            if (!Oku(Akış, ZamanAşımı_msn, out AlınanCsm, 4)) return false;
            long AlınanCsm_;
            AlınanCsm_ = AlınanCsm[0] << 24;
            AlınanCsm_ += AlınanCsm[1] << 16;
            AlınanCsm_ += AlınanCsm[2] << 8;
            AlınanCsm_ += AlınanCsm[3] << 0;
            if (AlınanCsm_ != Csm) return false;

            return true;
        }
        bool Oku(NamedPipeClientStream Akış, int ZamanAşımı_msn, out byte[] Çıktı, int Adet)
        {
            Çıktı = new byte[Adet];

            try
            {
                int Tik = Environment.TickCount + ZamanAşımı_msn;
                IAsyncResult Döngü = Akış.BeginRead(Çıktı, 0, Adet, null, null);

                while (Environment.TickCount < Tik && !Döngü.IsCompleted) Thread.Sleep(2);
                if (Döngü.IsCompleted)
                {
                    Tik = Akış.EndRead(Döngü);
                    if (Tik == Adet) return true;
                }
            }
            catch (Exception) { }
            return false;
        }
        bool Yaz(NamedPipeClientStream Akış, int ZamanAşımı_msn, string Girdi)
        {
            if (Yaz(Akış, ZamanAşımı_msn, D_Yazı.BaytDizisine(Girdi))) return true;
            else return false;
        }
        bool Yaz(NamedPipeClientStream Akış, int ZamanAşımı_msn, byte[] Girdi)
        {
            byte[] AraTampon = new byte[0];
            Biriktir(ref AraTampon, Girdi);
            if (!Yaz(Akış, ZamanAşımı_msn, AraTampon, AraTampon.Length)) return false;

            return true;
        }
        bool Yaz(NamedPipeClientStream Akış, int ZamanAşımı_msn, byte[] Girdi, int Adet)
        {
            try
            {
                int Tik = Environment.TickCount + ZamanAşımı_msn;
                IAsyncResult Döngü = Akış.BeginWrite(Girdi, 0, Adet, null, null);

                while (Environment.TickCount < Tik && !Döngü.IsCompleted) Thread.Sleep(2);
                if (Döngü.IsCompleted)
                {
                    Akış.EndWrite(Döngü);
                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }
        #endregion

        #region Alıcı
        public enum ÖncelikliMesajİşleyicisiSonuc_ { GelenKutusunaTaşı, Sil };
        public enum AlıcıDurum_ { Etkin, İzinVerilenGöndericilerListesineBağlı, Durgun };
        public struct Alınan_Mesaj_
        {
            public string GöndericininBilgisayarAdı;
            public string GondericininAdı;
            public DateTime Zamanı;
            public string Konu;
            public byte[] Mesaj;
        }
        public delegate ÖncelikliMesajİşleyicisiSonuc_ _CallBack_ÖncelikliMesajİşleyicisi_(Alınan_Mesaj_ Mesaj);
        public event _CallBack_ÖncelikliMesajİşleyicisi_ ÖncelikliMesajİşleyicisi_;
        
        void Alıcı_İşlemi(IAsyncResult result)
        {
            try
            {
                NamedPipeServerStream Sunucu = Ortak.Alıcı[(int)result.AsyncState];
                Sunucu.EndWaitForConnection(result);

                if (Ortak.Durdur || Ortak.Adı == "") return;
                else if (!Sunucu.IsConnected) goto Çıkış;
                else if (Ortak.Alıcı_Durumu == AlıcıDurum_.Durgun) { Yaz(Sunucu, Ortak.HaberleşmeZamanAşımı_msn, "_" + ((int)GöndericiDurum_.AlıcıPasif).ToString() + "_"); goto Çıkış; }

                if (!Yaz(Sunucu, Ortak.HaberleşmeZamanAşımı_msn, "_HosGeldin_")) goto Çıkış;
                if (Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn) != "_hOSbULDUM_") goto Çıkış;

                Alınan_Mesaj_ Yeni = new Alınan_Mesaj_();
                Yeni.Zamanı = DateTime.Now;
                Yeni.GöndericininBilgisayarAdı = Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn);
                Yeni.GondericininAdı = Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn);
                Yeni.Konu = Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn);
                bool Öncelikli = Convert.ToBoolean(Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn));
                int BilgiMiktarı = Convert.ToInt32(Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn));

                if (Ortak.Alıcı_Durumu == AlıcıDurum_.İzinVerilenGöndericilerListesineBağlı)
                {
                    foreach (var talepkar in Ortak.Alıcı_İzinVerilenGöndericilerListesi)
                    {
                        if (talepkar == Yeni.GondericininAdı) goto Devam;
                    }
                    Yaz(Sunucu, Ortak.HaberleşmeZamanAşımı_msn, "_" + ((int)GöndericiDurum_.İzinVerilenGöndericilerListesiEngelledi).ToString() + "_");
                    goto Çıkış;
                }
                
                Devam:
                if (!Yaz(Sunucu, Ortak.HaberleşmeZamanAşımı_msn, "_Basla_")) goto Çıkış;
                if (!Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn, out Yeni.Mesaj)) goto Çıkış;
                if (!Yaz(Sunucu, Ortak.HaberleşmeZamanAşımı_msn, "_Bitir_")) goto Çıkış;
                if (Oku(Sunucu, Ortak.HaberleşmeZamanAşımı_msn) != "_Bitti_") goto Çıkış;

                if (Öncelikli)
                {
                    Ortak.GelenKutusu_Öncelikli.Add(Yeni);
                    Ortak.Zamanlayıcı_GelenKutusu.Interval = 1;
                }
                else GelenKutusu.Add(Yeni);
            }
            catch { }

            Çıkış:
            Alıcı_Başlat((int)result.AsyncState);
        }
        void Alıcı_Başlat(int No)
        {
            try
            {
                if (Ortak.Alıcı[No] != null)
                {
                    if (Ortak.Alıcı[No].IsConnected) Ortak.Alıcı[No].Disconnect();
                    Ortak.Alıcı[No].Dispose();
                    Ortak.Alıcı[No] = null;
                }

                //PipeSecurity ps = new PipeSecurity();
                //ps.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, System.Security.AccessControl.AccessControlType.Allow));
                Ortak.Alıcı[No] = new NamedPipeServerStream(Ortak.Adı, PipeDirection.InOut, Ortak.Alıcı_AzamiSayısı, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024/*, ps*/);
                Ortak.Alıcı[No].BeginWaitForConnection(new AsyncCallback(Alıcı_İşlemi), No);
            }
            catch (Exception) { }
        }
        void Zamanlayıcıİşlemleri_GelenKutusu(object sender, EventArgs e)
        {
            Ortak.Zamanlayıcı_GelenKutusu.Stop();

            for (int i = 0; i < Ortak.GelenKutusu_Öncelikli.Count && !Ortak.Durdur;)
            {
                try
                {
                    if (ÖncelikliMesajİşleyicisi_ == null) GelenKutusu.Add(Ortak.GelenKutusu_Öncelikli[i]);
                    else if (ÖncelikliMesajİşleyicisi_(Ortak.GelenKutusu_Öncelikli[i]) == ÖncelikliMesajİşleyicisiSonuc_.GelenKutusunaTaşı) GelenKutusu.Add(Ortak.GelenKutusu_Öncelikli[i]);

                    Ortak.GelenKutusu_Öncelikli.RemoveAt(i);
                }
                catch (Exception) { i++; }

                Thread.Sleep(1);
            }

            Ortak.Zamanlayıcı_GelenKutusu.Interval = 60000;
            Ortak.Zamanlayıcı_GelenKutusu.Start();
        }
        public int GelenKutusu_KonudanMesajıBul(string Konu)
        {
            try
            {
                for (int i = 0; i < GelenKutusu.Count; i++)
                {
                    if (GelenKutusu[i].Konu == Konu) return i;
                }
            }
            catch (Exception) { }
            return -1;
        }

        string Oku(NamedPipeServerStream Akış, int ZamanAşımı_msn)
        {
            byte[] Okunan = null;
            if (Oku(Akış, ZamanAşımı_msn, out Okunan)) return D_Yazı.BaytDizisinden(Okunan);
            else return "";
        }
        bool Oku(NamedPipeServerStream Akış, int ZamanAşımı_msn, out byte[] Çıktı)
        {
            Çıktı = null;

            if (!Oku(Akış, ZamanAşımı_msn, out Çıktı, 4)) return false;
            long Csm = Çıktı.Sum(x => (long)x);

            long Adet;
            Adet = Çıktı[0] << 24;
            Adet += Çıktı[1] << 16;
            Adet += Çıktı[2] << 8;
            Adet += Çıktı[3] << 0;

            if (!Oku(Akış, ZamanAşımı_msn, out Çıktı, (int)Adet)) return false;
            Csm += Çıktı.Sum(x => (long)x);

            byte[] AlınanCsm = null;
            if (!Oku(Akış, ZamanAşımı_msn, out AlınanCsm, 4)) return false;
            long AlınanCsm_;
            AlınanCsm_ = AlınanCsm[0] << 24;
            AlınanCsm_ += AlınanCsm[1] << 16;
            AlınanCsm_ += AlınanCsm[2] << 8;
            AlınanCsm_ += AlınanCsm[3] << 0;
            if (AlınanCsm_ != Csm) return false;

            return true;
        }
        bool Oku(NamedPipeServerStream Akış, int ZamanAşımı_msn, out byte[] Çıktı, int Adet)
        {
            Çıktı = new byte[Adet];

            try
            {
                int Tik = Environment.TickCount + ZamanAşımı_msn;
                IAsyncResult Döngü = Akış.BeginRead(Çıktı, 0, Adet, null, null);

                while (Environment.TickCount < Tik && !Döngü.IsCompleted) Thread.Sleep(2);
                if (Döngü.IsCompleted)
                {
                    Tik = Akış.EndRead(Döngü);
                    if (Tik == Adet) return true;
                }
            }
            catch (Exception) { }
            return false;
        }
        bool Yaz(NamedPipeServerStream Akış, int ZamanAşımı_msn, string Girdi)
        {
            if (Yaz(Akış, ZamanAşımı_msn, D_Yazı.BaytDizisine(Girdi))) return true;
            else return false;
        }
        bool Yaz(NamedPipeServerStream Akış, int ZamanAşımı_msn, byte[] Girdi)
        {
            byte[] AraTampon = new byte[0];
            Biriktir(ref AraTampon, Girdi);
            if (!Yaz(Akış, ZamanAşımı_msn, AraTampon, AraTampon.Length)) return false;

            return true;
        }
        bool Yaz(NamedPipeServerStream Akış, int ZamanAşımı_msn, byte[] Girdi, int Adet)
        {
            try
            {
                int Tik = Environment.TickCount + ZamanAşımı_msn;
                IAsyncResult Döngü = Akış.BeginWrite(Girdi, 0, Adet, null, null);

                while (Environment.TickCount < Tik && !Döngü.IsCompleted) Thread.Sleep(2);
                if (Döngü.IsCompleted)
                {
                    Akış.EndWrite(Döngü);
                    return true;
                }
            }
            catch (Exception) { }
            return false;
        }
        #endregion

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

                    if (Ortak.Zamanlayıcı_GidenKutusu != null) { Ortak.Zamanlayıcı_GidenKutusu.Dispose(); Ortak.Zamanlayıcı_GidenKutusu = null; }
                    if (Ortak.Zamanlayıcı_GelenKutusu != null) { Ortak.Zamanlayıcı_GelenKutusu.Dispose(); Ortak.Zamanlayıcı_GelenKutusu = null; }
                    if (Ortak.Alıcı_İzinVerilenGöndericilerListesi != null) { Ortak.Alıcı_İzinVerilenGöndericilerListesi.Clear(); Ortak.Alıcı_İzinVerilenGöndericilerListesi = null; }
                    if (GelenKutusu != null) { GelenKutusu.Clear(); GelenKutusu = null; }
                    if (GidenKutusu != null) { GidenKutusu.AddingNew -= new AddingNewEventHandler(GönderilecekMesajListesineElemanEklendi); GidenKutusu.Clear(); GidenKutusu = null; }
                    if (ÖncelikliMesajİşleyicisi_ != null)
                    {
                        Delegate[] nesneler = ÖncelikliMesajİşleyicisi_.GetInvocationList();
                        foreach (Delegate nesne in nesneler) { ÖncelikliMesajİşleyicisi_ -= (_CallBack_ÖncelikliMesajİşleyicisi_)nesne; }
                        ÖncelikliMesajİşleyicisi_ = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UygulamalarArasıHaberlesme_() {
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

#endif