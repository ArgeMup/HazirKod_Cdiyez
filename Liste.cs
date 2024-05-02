// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace ArgeMup.HazirKod.EşZamanlıÇokluErişim
{
    public class Liste_<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable, ICollection
    {
        public const string Sürüm = "V1.1";
        public int Kilit_Devralma_ZamanAşımı_msn = 5000;

        Mutex Kilit = null;
        List<T> Elemanlar = null;

        public Liste_(List<T> KullanılacakListe = null, Mutex KullanılacakKilit = null)
        {
            if (KullanılacakListe != null) Elemanlar = KullanılacakListe;
            else Elemanlar = new List<T>();

            if (KullanılacakKilit != null) Kilit = KullanılacakKilit;
            else Kilit = new Mutex();
        }
        public void Sil(int SıraNo, int Adet = 1)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            if (SıraNo < 0) Elemanlar.Clear();
            else if (SıraNo < Elemanlar.Count)
            {
                if (Adet == 1) Elemanlar.RemoveAt(SıraNo);
                else
                {
                    if (Adet > Elemanlar.Count - SıraNo) Adet = Elemanlar.Count - SıraNo;

                    Elemanlar.RemoveRange(SıraNo, Adet);
                }
            }

            Kilit.ReleaseMutex();
        }
        public bool Oku(int SıraNo, out T Eleman, bool VeSil = false)
        {
            Eleman = default(T);
            bool sonuç = false;

            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            if (SıraNo < Elemanlar.Count)
            {
                Eleman = Elemanlar[SıraNo];
                if (VeSil) Elemanlar.RemoveAt(SıraNo);

                sonuç = true;
            }

            Kilit.ReleaseMutex();

            return sonuç;
        }
        public List<T> Durdur()
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<T> lst = Elemanlar;
            Elemanlar = null;

            Kilit.ReleaseMutex();
            Kilit.Dispose();
            Kilit = null;
            
            return lst;
        }

        #region Hassas Ara İşlemler
        public List<T> HassasAraİşlem_Başlat()
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            return Elemanlar;
        }
        public void HassasAraİşlem_Bitir(List<T> YeniListe = null)
        {
            if (YeniListe != null) Elemanlar = YeniListe;

            Kilit.ReleaseMutex();
        }
        #endregion

        #region Döngülerden erişim imkanı için ek
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Listeleyici(this);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Listeleyici(this);
        }
        class Listeleyici : IEnumerator, IEnumerator<T>
        {
            readonly List<T> ÜstSınıf;
            int konum = -1;
 
            public Listeleyici(Liste_<T> ÜstSınıf)
            {
                this.ÜstSınıf = ÜstSınıf.HassasAraİşlem_Başlat();

                if (this.ÜstSınıf == null) this.ÜstSınıf = new List<T>(); 
                else this.ÜstSınıf = new List<T>(this.ÜstSınıf);

                ÜstSınıf.HassasAraİşlem_Bitir();
            }
            public object Current
            {
                get { return ÜstSınıf[konum]; }
            }
            public bool MoveNext()
            {
                konum++;
                return (konum < ÜstSınıf.Count);
            }
            public void Reset()
            {
                konum = -1;
            }

            T IEnumerator<T>.Current => (T)Current;
            private bool disposedValue;
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects)
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                    // TODO: set large fields to null
                    disposedValue = true;
                }
            }

            // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
            // ~Listeleyici()
            // {
            //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            //     Dispose(disposing: false);
            // }

            void IDisposable.Dispose()
            {
                // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
        #endregion

        #region 1. Seviye Normal Liste Tanımlamaları
        public void Add(T value)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Elemanlar.Add(value);

            Kilit.ReleaseMutex();
        }
        public bool Contains(T value)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            bool sonuç = Elemanlar.Contains(value);

            Kilit.ReleaseMutex();

            return sonuç;
        }
        public void Clear()
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Elemanlar.Clear();

            Kilit.ReleaseMutex();
        }
        public int IndexOf(T value)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            int sonuç = Elemanlar.IndexOf(value);

            Kilit.ReleaseMutex();

            return sonuç;
        }
        public void Insert(int index, T value)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Elemanlar.Insert(index, value);

            Kilit.ReleaseMutex();
        }
        public bool Remove(T value)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            bool sonuç = Elemanlar.Remove(value);

            Kilit.ReleaseMutex();

            return sonuç;
        }
        public void RemoveAt(int index)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Elemanlar.RemoveAt(index);

            Kilit.ReleaseMutex();
        }
        public void CopyTo(T[] array, int index)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Elemanlar.CopyTo(array, index);

            Kilit.ReleaseMutex();
        }
        public bool IsReadOnly => false;
        public bool IsFixedSize => false;
        public int Count
        {
            get
            {
                if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                int sonuç = Elemanlar.Count;

                Kilit.ReleaseMutex();

                return sonuç;
            }
        }
        public object SyncRoot
        {
            get
            {
                if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                object sonuç = ((ICollection)Elemanlar).SyncRoot;

                Kilit.ReleaseMutex();

                return sonuç;
            }
        }
        public bool IsSynchronized
        {
            get => ((ICollection)Elemanlar).IsSynchronized;
        }
        public T this[int index]
        {
            get
            {
                if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                T sonuç = Elemanlar[index];

                Kilit.ReleaseMutex();

                return sonuç;
            }
            set
            {
                if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                Elemanlar[index] = value;

                Kilit.ReleaseMutex();
            }
        }
        #endregion

        #region 2. Seviye Normal Liste Tanımlamaları
        public void CopyTo(Array array, int index)
        {
            CopyTo((T[])array, index);
        }
        #endregion

        #region 3. Seviye Normal Liste Tanımlamaları
        public Liste_<T> Copy()
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<T> yeni = new List<T>();
            for (int i = 0; i < Elemanlar.Count; i++)
            {
                yeni.Add(Elemanlar[i]);
            }

            Kilit.ReleaseMutex();

            return new Liste_<T>(yeni, Kilit);
        }
        public void Sort(IComparer<T> Sıralayıcı)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Elemanlar.Sort(Sıralayıcı);

            Kilit.ReleaseMutex();
        }
        public Liste_<T> FindAll(Predicate<T> match)
        {
            if (match == null) throw new ArgumentNullException("Predicate<T> match");
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<T> yeni = new List<T>();
            for (int i = 0; i < Elemanlar.Count; i++)
            {
                if (match(Elemanlar[i]))
                {
                    yeni.Add(Elemanlar[i]);
                }
            }

            Kilit.ReleaseMutex();

            return new Liste_<T>(yeni, Kilit);
        }
        public void RemoveRange(int index, int count)
        {
            Sil(index, count);
        }
        #endregion
    }
}