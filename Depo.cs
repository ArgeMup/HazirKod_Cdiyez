// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Dönüştürme;
using System.Collections.Generic;
using System;

namespace ArgeMup.HazirKod
{
    public class Depo_
    {
        public const string Sürüm = "V1.0";
        public bool EnAzBirElemanAdıVeyaİçeriğiDeğişti = false;

        public List<Depo_Elemanı_> Elemanları = null;
        public class Depo_Elemanı_
        {
            public bool Silmekİçinİşaretlendi
            {
                get
                {
                    if (string.IsNullOrEmpty(Adı)) return true;
                    if (!string.IsNullOrEmpty(İçeriği)) return false;
                    if (Elemanları == null || Elemanları.Count == 0) return true;

                    int MevcutElemanSayısı = 0;
                    foreach (Depo_Elemanı_ Eleman in Elemanları)
                    {
                        if (Eleman.Silmekİçinİşaretlendi) continue;

                        MevcutElemanSayısı++;
                    }

                    return MevcutElemanSayısı == 0;
                }
            }
            public string Adı
            {
                get
                {
                    return _Depo.Ayraçlar.KullancıYazısı_İlkHalineGetir(_Adı);
                }
                set
                {
                    _Adı = _Depo.Ayraçlar.KullancıYazısı_Uygunlaştır(value);
                    _Depo.EnAzBirElemanAdıVeyaİçeriğiDeğişti = true;
                }
            }
            public string İçeriği
            {
                get
                {
                    return _Depo.Ayraçlar.KullancıYazısı_İlkHalineGetir(_İçeriği);
                }
                set
                {
                    _İçeriği = _Depo.Ayraçlar.KullancıYazısı_Uygunlaştır(value);
                    _Depo.EnAzBirElemanAdıVeyaİçeriğiDeğişti = true;
                }
            }
            public string Uzun_Adı
            {
                get
                {
                    return _Adı;
                }
            }
            public string Uzun_İçeriği
            {
                get
                {
                    return _İçeriği;
                }
            }
            public List<Depo_Elemanı_> Elemanları = null;

            readonly Depo_ _Depo = null;
            string _Adı = null;
            string _İçeriği = null;

            public Depo_Elemanı_(string Adı, string İçeriği, Depo_ Depo)
            {
                if (Depo == null) throw new Exception("Depo boş olamaz");

                _Depo = Depo;
                this.Adı = Adı;
                this.İçeriği = İçeriği;
            }
            public Depo_Elemanı_(string Eleman_AdıİçeriğiElemaları, int Seviye, Depo_ Depo)
            {
                // Adı'İçeriği'Alt Elemanları
                // baslık1''A%$?]1{#AAA%$?]1{#C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD?%#{1]$B%$?]1{#BBB

                if (Depo == null) throw new Exception("Depo boş olamaz");
                _Depo = Depo;

                if (!string.IsNullOrEmpty(Eleman_AdıİçeriğiElemaları))
                {
                    string[] dizi = Eleman_AdıİçeriğiElemaları.Split(_Depo.Ayraçlar.AdıVeİçeriğiVeElemanları);
                    if (dizi != null && dizi.Length >= 2 && !string.IsNullOrEmpty(dizi[0]))
                    {
                        Adı = dizi[0];
                        İçeriği = string.IsNullOrEmpty(dizi[1]) ? null : dizi[1];

                        if (dizi.Length == 3 && !string.IsNullOrEmpty(dizi[2]))
                        {
                            // Alt Elemanları
                            // A%$?]1{#AAA%$?]1{#C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD?%#{1]$B%$?]1{#BBB
                            Seviye++;
                            string ayr_Eleman = _Depo.Ayraçlar.Vekil_Eleman(Seviye); // ?%#{1]$

                            dizi = dizi[2].Split(new string[] { ayr_Eleman }, StringSplitOptions.None);
                            if (dizi != null && dizi.Length > 0)
                            {
                                Elemanları = new List<Depo_Elemanı_>();
                                string ayr_AdıVeİçeriğiVeElemanları = _Depo.Ayraçlar.Vekil_AdıVeİçeriğiVeElemanları(Seviye); // %$?]1{#

                                foreach (string elm in dizi)
                                {
                                    // A%$?]1{#AAA%$?]1{#C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD
                                    string ayıklanmış_eleman = elm.Replace(ayr_AdıVeİçeriğiVeElemanları, _Depo.Ayraçlar.AdıVeİçeriğiVeElemanları.ToString());
                                    // A'AAA'C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD

                                    Depo_Elemanı_ yeni = new Depo_Elemanı_(ayıklanmış_eleman, Seviye, _Depo);
                                    if (!yeni.Silmekİçinİşaretlendi) Elemanları.Add(yeni);
                                }

                                if (Elemanları.Count == 0) Elemanları = null;
                            }
                        }
                    }
                }
            }

            /// <param name="ElemanAdıDizisi">Dizi_Ayırma_Elemanı karakteri ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
            /// <param name="İçeriği">Elemanın içeriği</param>
            public void Yaz(string ElemanAdıDizisi, string İçeriği)
            {
                if (string.IsNullOrEmpty(ElemanAdıDizisi)) this.İçeriği = İçeriği;
                else
                {
                    ElemanAdıDizisi = ElemanAdıDizisi.Trim(_Depo.Ayraçlar.ElemanAdıDizisi);

                    //alttaki elemanlardan bahsediyor
                    string geriyekalan = _Depo.Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

                    if (Elemanları == null) Elemanları = new List<Depo_Elemanı_>();

                    Depo_Elemanı_ bulunan = Elemanları.Find(x => x.Adı == ElemanAdıDizisi);
                    if (bulunan != null) bulunan.Yaz(geriyekalan, İçeriği);
                    else
                    {
                        //bulunamadığından yeni bir eleman oluştur
                        bulunan = new Depo_Elemanı_(ElemanAdıDizisi, null, _Depo);
                        bulunan.Yaz(geriyekalan, İçeriği);
                        Elemanları.Add(bulunan);
                    }
                }
            }
            /// <param name="ElemanAdıDizisi">Dizi_Ayırma_Elemanı karakteri ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
            public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null)
            {
                Depo_Elemanı_ bulunan = Bul(ElemanAdıDizisi);
                if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

                string okunan = bulunan.İçeriği;
                if (string.IsNullOrEmpty(okunan)) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

                return okunan;
            }
            public Depo_Elemanı_ Bul(string ElemanAdıDizisi)
            {
                if (string.IsNullOrEmpty(ElemanAdıDizisi)) return this;
                else
                {
                    ElemanAdıDizisi = ElemanAdıDizisi.Trim(_Depo.Ayraçlar.ElemanAdıDizisi);

                    //alttaki elemanlardan bahsediyor
                    if (Elemanları == null) return null;

                    string geriyekalan = _Depo.Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

                    Depo_Elemanı_ bulunan = Elemanları.Find(x => x.Adı == ElemanAdıDizisi);
                    if (bulunan == null) return null;

                    return bulunan.Bul(geriyekalan);
                }
            }
            public string[] Listele(string ElemanAdıDizisi, bool Adı = false, bool İçeriği = true, bool ElemanAdları = false)
            {
                string[] dizi = new string[0];
                if (!Adı && !İçeriği && !ElemanAdları) return dizi;

                Depo_Elemanı_ bulunan = Bul(ElemanAdıDizisi);
                if (bulunan == null || bulunan.Elemanları == null || bulunan.Elemanları.Count == 0) return dizi;

                dizi = new string[(Adı ? bulunan.Elemanları.Count : 0) + (İçeriği ? bulunan.Elemanları.Count : 0) + (ElemanAdları ? bulunan.Elemanları.Count : 0)];
                int syc = 0;
                foreach (Depo_Elemanı_ de in bulunan.Elemanları)
                {
                    if (Adı) dizi[syc++] = de.Adı;
                    if (İçeriği) dizi[syc++] = de.İçeriği;
                    if (ElemanAdları)
                    {
                        if (de.Elemanları == null || de.Elemanları.Count == 0) syc++;
                        else
                        {
                            string birarada = "";
                            de.Elemanları.ForEach(x => birarada += x.Adı + _Depo.Ayraçlar.ElemanAdıDizisi);
                            dizi[syc++] = birarada.TrimEnd(_Depo.Ayraçlar.ElemanAdıDizisi);
                        }
                    }
                }

                return dizi;
            }

            #region Dönüştürme
            public void Yaz(string ElemanAdıDizisi, double Sayı)
            {
                Yaz(ElemanAdıDizisi, D_Sayı.Yazıya(Sayı));
            }
            public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
            {
                Yaz(ElemanAdıDizisi, D_HexYazı.BaytDizisinden(BaytDizisi, Adet, BaşlangıçKonumu));
            }
            public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat)
            {
                Yaz(ElemanAdıDizisi, D_TarihSaat.Yazıya(TarihSaat));
            }
            public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default)
            {
                try
                {
                    return D_Sayı.Yazıdan(Oku(ElemanAdıDizisi));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null)
            {
                try
                {
                    return D_HexYazı.BaytDizisine(Oku(ElemanAdıDizisi));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default)
            {
                try
                {
                    return D_TarihSaat.Tarihe(Oku(ElemanAdıDizisi));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            #endregion
        };

        public class Ayraçlar_
        {
            public char ElemanAdıDizisi = '/';

            public char Eleman = '"';
            public char AdıVeİçeriğiVeElemanları = '\'';
            public string Vekil_Eleman_Başla = "?%#{", Vekil_Eleman_Bitir = "]$";
            public string Vekil_AdıVeİçeriğiVeElemanları_Başla = "%$?]", Vekil_AdıVeİçeriğiVeElemanları_Bitir = "{#";

            public string Vekil_Eleman(int Seviye)
            {
                return Vekil_Eleman_Başla + Seviye + Vekil_Eleman_Bitir;
            }
            public string Vekil_AdıVeİçeriğiVeElemanları(int Seviye)
            {
                return Vekil_AdıVeİçeriğiVeElemanları_Başla + Seviye + Vekil_AdıVeİçeriğiVeElemanları_Bitir;
            }

            public string KullancıYazısı_Uygunlaştır(string KullanıcıYazısı)
            {
                if (string.IsNullOrEmpty(KullanıcıYazısı)) return KullanıcıYazısı;
                return KullanıcıYazısı.Replace(Eleman.ToString(), Vekil_Eleman(-1)).Replace(AdıVeİçeriğiVeElemanları.ToString(), Vekil_AdıVeİçeriğiVeElemanları(-1));
            }
            public string KullancıYazısı_İlkHalineGetir(string KullanıcıYazısı)
            {
                if (string.IsNullOrEmpty(KullanıcıYazısı)) return KullanıcıYazısı;
                return KullanıcıYazısı.Replace(Vekil_Eleman(-1), Eleman.ToString()).Replace(Vekil_AdıVeİçeriğiVeElemanları(-1), AdıVeİçeriğiVeElemanları.ToString());
            }

            public string ElemanAdıDizisi_Ayıkla(ref string Girdi)
            {
                int ayırma_karakteri_konumu = Girdi.IndexOf(ElemanAdıDizisi);
                string geriyekalan = "";
                string asıl = Girdi;

                if (ayırma_karakteri_konumu >= 0)
                {
                    Girdi = Girdi.Substring(0, ayırma_karakteri_konumu);

                    ayırma_karakteri_konumu++;
                    if (asıl.Length > ayırma_karakteri_konumu) geriyekalan = asıl.Substring(ayırma_karakteri_konumu);
                }

                return geriyekalan;
            }
        }
        public Ayraçlar_ Ayraçlar = new Ayraçlar_();

        public Depo_(string YazıOlarakElemanlar = null)
        {
            // Eleman                   " -> ?%#{seviye]$
            // AdıVeİçeriğiVeElemanları ' -> %$?]seviye{#
            //
            // Elemanın Adı ' Elamanın İçeriği '(istege bagli) Elemanın Alt Elemanları
            //
            // baslık1''alteleman1"baslık2'bilgi2'hata	0-> baslık1''A%$?]1{#AAA%$?]1{#C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD?%#{1]$B%$?]1{#BBB"baslık2'bilgi2'hata
            //
            // alteleman1
            // A'AAA'alteleman2"B'BBB				    1-> A%$?]1{#AAA%$?]1{#C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD?%#{1]$B%$?]1{#BBB
            //
            // alteleman2
            // C'CCC'alteleman3"D'DDD				    2-> C%$?]2{#CCC%$?]2{#E%$?]3{#EEE?%#{3]$F%$?]3{#FFF?%#{2]$D%$?]2{#DDD
            //
            // alteleman3
            // E'EEE"F'FFF                              3-> E%$?]3{#EEE?%#{3]$F%$?]3{#FFF

            if (!string.IsNullOrEmpty(YazıOlarakElemanlar))
            {
                string[] dizi = YazıOlarakElemanlar.Split(Ayraçlar.Eleman);
                if (dizi != null && dizi.Length > 0)
                {
                    Elemanları = new List<Depo_Elemanı_>();

                    foreach (string Eleman in dizi)
                    {
                        Depo_Elemanı_ yeni = new Depo_Elemanı_(Eleman, 0, this);
                        if (!yeni.Silmekİçinİşaretlendi) Elemanları.Add(yeni);
                    }

                    if (Elemanları.Count == 0) Elemanları = null;
                }
            }

            EnAzBirElemanAdıVeyaİçeriğiDeğişti = false;
        }

        /// <param name="ElemanAdıDizisi">Dizi_Ayırma_Elemanı karakteri ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        /// <param name="İçeriği">Elemanın içeriği</param>
        public void Yaz(string ElemanAdıDizisi, string İçeriği)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi)) throw new Exception("ElemanDizisi boş olamaz");
            else ElemanAdıDizisi = ElemanAdıDizisi.Trim(Ayraçlar.ElemanAdıDizisi);

            //alttaki elemanlardan bahsediyor
            string geriyekalan = Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

            if (Elemanları == null) Elemanları = new List<Depo_Elemanı_>();

            Depo_Elemanı_ bulunan = Elemanları.Find(x => x.Adı == ElemanAdıDizisi);
            if (bulunan != null) bulunan.Yaz(geriyekalan, İçeriği);
            else
            {
                //bulunamadığından yeni bir eleman oluştur
                bulunan = new Depo_Elemanı_(ElemanAdıDizisi, null, this);
                bulunan.Yaz(geriyekalan, İçeriği);
                Elemanları.Add(bulunan);
            }
        }
        /// <param name="ElemanAdıDizisi">Dizi_Ayırma_Elemanı karakteri ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null)
        {
            Depo_Elemanı_ bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            string okunan = bulunan.İçeriği;
            if (string.IsNullOrEmpty(okunan)) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return okunan;
        }
        public Depo_Elemanı_ Bul(string ElemanAdıDizisi)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi)) throw new Exception("ElemanDizisi boş olamaz");
            else ElemanAdıDizisi = ElemanAdıDizisi.Trim(Ayraçlar.ElemanAdıDizisi);

            //alttaki elemanlardan bahsediyor
            if (Elemanları == null) return null;

            string geriyekalan = Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

            Depo_Elemanı_ bulunan = Elemanları.Find(x => x.Adı == ElemanAdıDizisi);
            if (bulunan == null) return null;

            return bulunan.Bul(geriyekalan);
        }
        public string[] Listele(string ElemanAdıDizisi, bool Adı = false, bool İçeriği = true, bool ElemanAdları = false)
        {
            string[] dizi = new string[0];
            if (!İçeriği && !Adı && !ElemanAdları) return dizi;

            if (string.IsNullOrEmpty(ElemanAdıDizisi))
            {
                //en üst dal
                if (Elemanları == null || Elemanları.Count == 0) return dizi;

                dizi = new string[(Adı ? Elemanları.Count : 0) + (İçeriği ? Elemanları.Count : 0) + (ElemanAdları ? Elemanları.Count : 0)];
                int syc = 0;
                foreach (Depo_Elemanı_ de in Elemanları)
                {
                    if (Adı) dizi[syc++] = de.Adı;
                    if (İçeriği) dizi[syc++] = de.İçeriği;
                    if (ElemanAdları)
                    {
                        if (de.Elemanları == null || de.Elemanları.Count == 0) syc++;
                        else
                        {
                            string birarada = "";
                            de.Elemanları.ForEach(x => birarada += x.Adı + Ayraçlar.ElemanAdıDizisi);
                            dizi[syc++] = birarada.TrimEnd(Ayraçlar.ElemanAdıDizisi);
                        }
                    }
                }

                return dizi;
            }
            else
            {
                //alt dallardan biri
                Depo_Elemanı_ bulunan = Bul(ElemanAdıDizisi);
                if (bulunan == null) return dizi;

                return bulunan.Listele(null, Adı, İçeriği, ElemanAdları);
            }
        }

        #region Dönüştürme
        public void Yaz(string ElemanAdıDizisi, double Sayı)
        {
            Yaz(ElemanAdıDizisi, D_Sayı.Yazıya(Sayı));
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
        {
            Yaz(ElemanAdıDizisi, D_HexYazı.BaytDizisinden(BaytDizisi, Adet, BaşlangıçKonumu));
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat)
        {
            Yaz(ElemanAdıDizisi, D_TarihSaat.Yazıya(TarihSaat));
        }
        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default)
        {
            try
            {
                return D_Sayı.Yazıdan(Oku(ElemanAdıDizisi));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null)
        {
            try
            {
                return D_HexYazı.BaytDizisine(Oku(ElemanAdıDizisi));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default)
        {
            try
            {
                return D_TarihSaat.Tarihe(Oku(ElemanAdıDizisi));
            }
            catch (Exception) { }

            return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
        }
        #endregion

        public string YazıyaDönüştür()
        {
            if (Elemanları == null || Elemanları.Count == 0) return "";

            string çıktı = "";

            foreach (Depo_Elemanı_ Eleman in Elemanları)
            {
                string yeni = _YazıyaDönüştür_(Eleman, -1);
                if (string.IsNullOrEmpty(yeni)) continue;

                çıktı += yeni + Ayraçlar.Eleman;
            }
            çıktı = çıktı.TrimEnd(Ayraçlar.Eleman);

            string ayr_Eleman = Ayraçlar.Vekil_Eleman(0); // ?%#{0]$
            string ayr_AdıVeİçeriğiVeElemanları = Ayraçlar.Vekil_AdıVeİçeriğiVeElemanları(0); // %$?]0{#
            çıktı = çıktı.Replace(ayr_Eleman, Ayraçlar.Eleman.ToString());
            çıktı = çıktı.Replace(ayr_AdıVeİçeriğiVeElemanları, Ayraçlar.AdıVeİçeriğiVeElemanları.ToString());

            return çıktı;
        }
        string _YazıyaDönüştür_(Depo_Elemanı_ Eleman, int Seviye)
        {
            if (Eleman == null || Eleman.Silmekİçinİşaretlendi) return "";

            Seviye++;
            string çıktı = "";
            if (Eleman.Elemanları != null)
            {
                string ayr_Eleman = Ayraçlar.Vekil_Eleman(Seviye + 1); // ?%#{1]$

                foreach (Depo_Elemanı_ AltEleman in Eleman.Elemanları)
                {
                    if (AltEleman.Silmekİçinİşaretlendi) continue;

                    string yeni = _YazıyaDönüştür_(AltEleman, Seviye);
                    if (string.IsNullOrEmpty(yeni)) continue;

                    çıktı += ayr_Eleman + yeni;
                }

                if (!string.IsNullOrEmpty(çıktı)) çıktı = çıktı.Substring(ayr_Eleman.Length);
            }

            string ayr_AdıVeİçeriğiVeElemanları = Ayraçlar.Vekil_AdıVeİçeriğiVeElemanları(Seviye); // %$?]1{#
            return Eleman.Uzun_Adı + ayr_AdıVeİçeriğiVeElemanları + Eleman.Uzun_İçeriği + (string.IsNullOrEmpty(çıktı) ? null : ayr_AdıVeİçeriğiVeElemanları + çıktı);
        }
    }
}