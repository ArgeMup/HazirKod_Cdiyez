// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ArgeMup.HazirKod.ArkaPlan
{
    public class Öğütücü_<Tipi>
    {
        /// <summary>
        /// Görev içinde bekleme yapılacak ise Thread.Sleep(1000); kullanılmalı
        /// </summary>
        public int Kilit_Devralma_ZamanAşımı_msn = 5000;
        
        List<Tipi> Liste = new List<Tipi>();
        Action<Tipi, object> Çağırılacakİşlem = null;
        Task Görev = null;
        Mutex Kilit = new Mutex();
        bool Çalışsın = true;
        int AzamiElemanSayısı = 0;
        int GöreviSilmedenÖnceBeklenecekSüre_msn = 60000;
        object Hatırlatıcı = null;

        public Öğütücü_(Action<Tipi, object> Çağırılacakİşlem, object Hatırlatıcı = null, int AzamiElemanSayısı = 5555, int GöreviSilmedenÖnceBeklenecekSüre_msn = 60000)
        {
            this.AzamiElemanSayısı = AzamiElemanSayısı;
            this.Çağırılacakİşlem = Çağırılacakİşlem;
            this.Hatırlatıcı = Hatırlatıcı;
            this.GöreviSilmedenÖnceBeklenecekSüre_msn = GöreviSilmedenÖnceBeklenecekSüre_msn;
        }
        public void Ekle(Tipi Eleman)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Liste.Add(Eleman);
            if (Liste.Count > AzamiElemanSayısı)
            {
                //en eski 1/2 üyeyü sil
                Liste.RemoveRange(0, AzamiElemanSayısı / 2);
                Console.WriteLine("HazirKod_Cdiyez Takipçi listesinde " + Liste.Count + " adet girdi mevcut, işlenemediğinden ilk 1/2 üyesi silindi");
            }

            Kilit.ReleaseMutex();

            bool YenidenOluştur = false;

            if (Görev == null) YenidenOluştur = true;
            else if (Görev.Status >= TaskStatus.RanToCompletion)
            {
                try { Görev.Dispose(); } catch { }
                YenidenOluştur = true;
            }

            if (YenidenOluştur)
            {
                Görev = new Task(() => { ArkaPlanGörevi(); });
                Görev.Start();
            }
        }
        public List<Tipi> Durdur()
        {
            Çalışsın = false;
            return Liste;
        }

        void ArkaPlanGörevi()
        {
            int za = Environment.TickCount + GöreviSilmedenÖnceBeklenecekSüre_msn;

            while (Çalışsın && za > Environment.TickCount)
            {
                while (Çalışsın && Liste.Count > 0)
                {
                    if (Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn))
                    {
                        Tipi içerik = Liste[0];
                        Liste.RemoveAt(0);
                        Kilit.ReleaseMutex();

                        try { Çağırılacakİşlem(içerik, Hatırlatıcı); } catch { }
                    }

                    za = Environment.TickCount + GöreviSilmedenÖnceBeklenecekSüre_msn;
                }

                if (Çalışsın) Thread.Sleep(1000);
            }
        }
    }
}
