// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;

namespace ArgeMup.HazirKod
{
    //Değişkenler
    internal static class D
    {
        public const string Sürüm = "V1.0";

        #region Değişkenler
        struct BirDeğişken_
        {
            public object Sahibi;
            public string Adı;
            public WeakReference Nesne;
            
            public BirDeğişken_(object Sahibi_, string Adı_, object Nesne_)
            {
                Sahibi = Sahibi_;
                Adı = Adı_;
                Nesne = new WeakReference(Nesne_, false); 
            }
        }
        static System.Collections.Generic.List<BirDeğişken_> Liste = new System.Collections.Generic.List<BirDeğişken_>();
        static System.Threading.Mutex Muteks = new System.Threading.Mutex();
        #endregion

        static public object Oku(object Kendisi, string Adı, object BulunamamasıDurumundakiDeğeri = null, bool BulunamamasıDurumundaYaz = true)
        {
            if (string.IsNullOrEmpty(Adı)) return BulunamamasıDurumundakiDeğeri;

            Muteks.WaitOne();

            for (int i = 0; i < Liste.Count; i++)
            {
                if (Liste[i].Adı == Adı)
                {
                    if (!Liste[i].Nesne.IsAlive || Liste[i].Nesne.Target == null) { Liste.RemoveAt(i); break; }
                    else { Muteks.ReleaseMutex(); return Liste[i].Nesne.Target; }
                }
            }

            if (Kendisi != null && BulunamamasıDurumundakiDeğeri != null && BulunamamasıDurumundaYaz)
            {
                Liste.Add(new BirDeğişken_(Kendisi, Adı, BulunamamasıDurumundakiDeğeri));
            }

            Muteks.ReleaseMutex();

            return BulunamamasıDurumundakiDeğeri;
        }

        static public bool Yaz(object Kendisi, string Adı, object Değeri)
        {
            if (string.IsNullOrEmpty(Adı)) return false;

            Muteks.WaitOne();

            for (int i = 0; i < Liste.Count; i++)
            {
                if (Liste[i].Adı == Adı)
                {
                    bool sahibi = false;
                    if (Liste[i].Sahibi.Equals(Kendisi) &&
                        Liste[i].Sahibi.GetHashCode() == Kendisi.GetHashCode() &&
                        Liste[i].Sahibi.GetType() == Kendisi.GetType()) sahibi = true;

                    if (sahibi)
                    {
                        if (Değeri != null) Liste.Add(new BirDeğişken_(Liste[i].Sahibi, Liste[i].Adı, Değeri));
                        Liste.RemoveAt(i);
                    }
                    Muteks.ReleaseMutex();
                    return sahibi;
                }
            }

            if (Kendisi != null && Değeri != null) Liste.Add(new BirDeğişken_(Kendisi, Adı, Değeri));

            Muteks.ReleaseMutex();

            return true;
        }


    }
}

