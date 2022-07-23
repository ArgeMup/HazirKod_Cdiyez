// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Threading;
using System.Collections.Generic;

namespace ArgeMup.HazirKod
{
    //static
    public class D
    {
        public const string Sürüm = "V1.2";

        #region Değişkenler
        readonly static Değişkenler De = new Değişkenler();
        readonly static Mutex Kilit = new Mutex();
        #endregion

        public static object Oku(string Adı, object BulunamamasıDurumundakiİçeriği = null)
        {
            Kilit.WaitOne();
            object çıktı = De.Oku(Adı, BulunamamasıDurumundakiİçeriği);
            Kilit.ReleaseMutex();

            return çıktı;
        }
        public static void Yaz(string Adı, object İçeriği)
        {
            Kilit.WaitOne();
            De.Yaz(Adı, İçeriği);
            Kilit.ReleaseMutex();
        }
    }

    //Dinamik
    [Serializable]
    public class Değişkenler
    {
        public const string Sürüm = "V1.0";

        #region Değişkenler
        Dictionary<string, object> Liste = new Dictionary<string, object>();
        #endregion

        public object Oku(string Adı, object BulunamamasıDurumundakiİçeriği = null)
        {
            if (string.IsNullOrEmpty(Adı)) return BulunamamasıDurumundakiİçeriği;

            if (!Liste.TryGetValue(Adı, out object okunan)) return BulunamamasıDurumundakiİçeriği;
            
            if (okunan != null) return okunan;
                
            Liste.Remove(Adı);
            return BulunamamasıDurumundakiİçeriği;
        }
        public void Yaz(string Adı, object İçeriği)
        {
            if (string.IsNullOrEmpty(Adı)) throw new Exception("Adı boş olamaz");

            if (İçeriği == null)
            {
                if (Liste.ContainsKey(Adı)) Liste.Remove(Adı);
            }
            else Liste[Adı] = İçeriği;
        }
    }
}

