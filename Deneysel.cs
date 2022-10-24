// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_DeneyselEklentiler

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

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
                    Thread.Sleep(1000);
                    MiliSaniye -= 1000;
                }

                if (!DurmaTalepEdildi) Thread.Sleep(MiliSaniye);
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

#endif