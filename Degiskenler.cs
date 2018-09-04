// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;

namespace ArgeMup.HazirKod
{
    //Değişkenler
    internal static class D
    {
        public const string Sürüm = "V1.1";

        #region Değişkenler
        struct BirDeğişken_
        {
            public string Adı;
            public WeakReference İçeriği;
            
            public BirDeğişken_(string Adı, object İçeriği)
            {
                this.Adı = Adı;
                this.İçeriği = new WeakReference(İçeriği, false); 
            }
        }
        static System.Collections.Generic.List<BirDeğişken_> Liste = new System.Collections.Generic.List<BirDeğişken_>();
        static System.Threading.Mutex Muteks = new System.Threading.Mutex();
        #endregion

        static public object Oku(string Adı, object BulunamamasıDurumundakiİçeriği = null, bool BulunamamasıDurumundaYaz = false)
        {
            if (string.IsNullOrEmpty(Adı)) return BulunamamasıDurumundakiİçeriği;

            Muteks.WaitOne();

            for (int i = 0; i < Liste.Count; i++)
            {
                if (Liste[i].Adı == Adı)
                {
                    if (!Liste[i].İçeriği.IsAlive || Liste[i].İçeriği.Target == null) { Liste.RemoveAt(i); break; }
                    else { Muteks.ReleaseMutex(); return Liste[i].İçeriği.Target; }
                }
            }

            if (BulunamamasıDurumundakiİçeriği != null && BulunamamasıDurumundaYaz)
            {
                Liste.Add(new BirDeğişken_(Adı, BulunamamasıDurumundakiİçeriği));
            }

            Muteks.ReleaseMutex();

            return BulunamamasıDurumundakiİçeriği;
        }
        static public void Yaz(string Adı, object İçeriği)
        {
            if (string.IsNullOrEmpty(Adı)) return;

            Muteks.WaitOne();

            for (int i = 0; i < Liste.Count; i++)
            {
                if (Liste[i].Adı == Adı)
                {
                    if (İçeriği != null) Liste.Add(new BirDeğişken_(Liste[i].Adı, İçeriği));
                    Liste.RemoveAt(i);
                    
                    Muteks.ReleaseMutex();
                    return;
                }
            }

            if (İçeriği != null) Liste.Add(new BirDeğişken_(Adı, İçeriği));

            Muteks.ReleaseMutex();

            return;
        }
    }
}

