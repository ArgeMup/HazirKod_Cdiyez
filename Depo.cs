// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Dönüştürme;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public interface IDepo_Eleman
    {
        string Adı { get; set; }
        /// <summary>
        /// İçeriği
        /// </summary>
        string this[int i] { get; set; } //İçeriği
        string[] İçeriği { get; set; }
        IDepo_Eleman[] Elemanları { get; }
        bool İçiBoşOlduğuİçinSilinecek { get; }

        /// <param name="ElemanAdıDizisi">Ayraçlar.ElemanAdıDizisi karakteri / ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        /// <param name="İçeriği">Elemanın içeriği (İçeriği ve Elemanları olmayan Elemanlar YazıyaDönüştürme aşamasında görmezden gelinip silinir)</param>
        /// <param name="SıraNo">Aynı eleman adı ile birden fazla içerik tutmak için kullanılabilir</param>
        void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0);

        string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0);
        double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);
        byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0);
        DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);
        bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);

        IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false);
        string YazıyaDönüştür(string ElemanAdıDizisi, bool SadeceElemanları = false);
        void Ekle(string ElemanAdıDizisi, string Eleman);
        void Sil(string ElemanAdıDizisi);
    }

    public class Depo_Ayraçlar
    {
        public static readonly char ElemanAdıDizisi = '/';

        public static readonly char Eleman = '\n';
        public static readonly char Eleman2 = '\r';
        public static readonly char AdıVeİçerik = '>';

        static string Vekil_Eleman = " ?'#{n]$ ";
        static string Vekil_Eleman2 = " ?'#{r]$ ";
        static string Vekil_AdıVeİçerik = " ?'#{b]$ ";

        public static string KullancıYazısı_Depoya_Göre_Uygunlaştır(string KullanıcıYazısı)
        {
            if (string.IsNullOrEmpty(KullanıcıYazısı)) return null;
            return KullanıcıYazısı.Replace(Eleman.ToString(), Vekil_Eleman).Replace(Eleman2.ToString(), Vekil_Eleman2).Replace(AdıVeİçerik.ToString(), Vekil_AdıVeİçerik);
        }
        public static string KullancıYazısı_İlkHalineGetir(string KullanıcıYazısı)
        {
            if (string.IsNullOrEmpty(KullanıcıYazısı)) return null;
            return KullanıcıYazısı.Replace(Vekil_Eleman, Eleman.ToString()).Replace(Vekil_Eleman2, Eleman2.ToString()).Replace(Vekil_AdıVeİçerik, AdıVeİçerik.ToString());
        }

        public static string ElemanAdıDizisi_Ayıkla(ref string Girdi)
        {
            Girdi = Girdi.Trim(ElemanAdıDizisi);

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

    public class Depo_
    {
        public const string Sürüm = "V1.0";
        public bool EnAzBir_ElemanAdıVeyaİçeriği_Değişti
        {
            get
            {
                return _EnAzBir_ElemanAdıVeyaİçeriği_Değişti;
            }
            set
            {
                if (value)
                {
                    _EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                    if (GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti != null) GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti(this);
                }
                else _EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;
            }
        }
        public IDepo_Eleman[] Elemanları
        {
            get
            {
                if (_Elemanları == null) return new IDepo_Eleman[0];

                return _Elemanları;
            }
        }

        bool _EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;
        Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null;

        Eleman_[] _Elemanları = null;
        class Eleman_ : IDepo_Eleman
        {
            readonly Depo_ _Depo = null;
            public string _Adı = null;
            public string[] _İçeriği = null;
            public Eleman_[] _Elemanları = null;

            public Eleman_(string Adı, Depo_ Depo)
            {
                _Adı = Adı;
                _Depo = Depo;
            }
            public Eleman_(ref List<string> Eleman_AdıİçeriğiElemaları, int Seviye, Depo_ Depo)
            {
                // eleman1'içerik[0]'içerik[1]\n
                // 'eleman1_alteleman1'içerik[0]\n

                if (Depo == null) throw new Exception("Depo boş olamaz");
                _Depo = Depo;
                _Elemanları = new Eleman_[0];

                int okunan_seviye = SıradakiniOku(ref Eleman_AdıİçeriğiElemaları, out string eleman);
                if (okunan_seviye > Seviye + 1) throw new Exception("Seviye bilgisi uyumsuz - " + eleman);

                string[] dizi = eleman.Split(Depo_Ayraçlar.AdıVeİçerik);
                _Adı = Depo_Ayraçlar.KullancıYazısı_İlkHalineGetir(dizi[0]);
                _İçeriği = new string[dizi.Length - 1];
                for (int i = 0; i < _İçeriği.Length; i++)
                {
                    _İçeriği[i] = Depo_Ayraçlar.KullancıYazısı_İlkHalineGetir(dizi[i + 1]);
                    if (string.IsNullOrEmpty(_İçeriği[i])) _İçeriği[i] = null;
                }

                Eleman_AdıİçeriğiElemaları.RemoveAt(0);
            YenidenDene:
                int sonrakinin_seviyesi = SıradakiniOku(ref Eleman_AdıİçeriğiElemaları, out _);
                if (sonrakinin_seviyesi <= Seviye)
                {
                    Elemanları_BoşMu();
                    return;
                }

                //sonraki eleman şimdiki elemanın alt elemanı olduğundan içerden devam edecek
                Array.Resize(ref _Elemanları, _Elemanları.Length + 1);
                _Elemanları[_Elemanları.Length - 1] = new Eleman_(ref Eleman_AdıİçeriğiElemaları, Seviye + 1, Depo);
                goto YenidenDene;

                int SıradakiniOku(ref List<string> Tümü, out string Cümle)
                {
                    while (Tümü.Count > 0 && string.IsNullOrEmpty(Tümü[0])) Tümü.RemoveAt(0);
                    if (Tümü.Count <= 0)
                    {
                        Cümle = null;
                        return -1;
                    }

                    string sıradaki = Tümü[0];

                    int sıradaki_seviye = sıradaki.Length;
                    sıradaki = sıradaki.TrimStart(Depo_Ayraçlar.AdıVeİçerik);
                    sıradaki_seviye -= sıradaki.Length;

                    Cümle = sıradaki;
                    return sıradaki_seviye;
                }
            }

            public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0)
            {
                if (string.IsNullOrEmpty(ElemanAdıDizisi)) this[SıraNo] = İçeriği;
                else
                {
                    //alttaki elemanlardan bahsediyor
                    string geriyekalan = Depo_Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

                    if (_Elemanları == null) _Elemanları = new Eleman_[0];

                    Eleman_ bulunan = Array.Find(_Elemanları, x => x._Adı == ElemanAdıDizisi);
                    if (bulunan != null) bulunan.Yaz(geriyekalan, İçeriği, SıraNo);
                    else
                    {
                        //bulunamadığından yeni bir eleman oluştur
                        if (!string.IsNullOrEmpty(İçeriği))
                        {
                            bulunan = new Eleman_(ElemanAdıDizisi, _Depo);
                            Array.Resize(ref _Elemanları, _Elemanları.Length + 1);
                            _Elemanları[_Elemanları.Length - 1] = bulunan;
                            bulunan.Yaz(geriyekalan, İçeriği, SıraNo);
                        }
                    }
                }
            }
            public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

                string okunan = bulunan[SıraNo];
                if (string.IsNullOrEmpty(okunan)) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

                return okunan;
            }
            public Eleman_ Bul_Getir(string ElemanAdıDizisi, bool YoksaOluştur = false)
            {
                if (string.IsNullOrEmpty(ElemanAdıDizisi)) return this;
                else
                {
                    //alttaki elemanlardan bahsediyor
                    Eleman_ bulunan = null;
                    if (_Elemanları == null) goto OluşturmayıKontrolEt;

                    string geriyekalan = Depo_Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

                    bulunan = Array.Find(_Elemanları, x => x._Adı == ElemanAdıDizisi);
                    if (bulunan == null) goto OluşturmayıKontrolEt;

                    return bulunan.Bul_Getir(geriyekalan, YoksaOluştur);

                    OluşturmayıKontrolEt:
                    if (YoksaOluştur)
                    {
                        Yaz(ElemanAdıDizisi, "ArGeMuP");
                        bulunan = Bul_Getir(ElemanAdıDizisi, false);
                    }

                    return bulunan;
                }
            }
            public string YazıyaDönüştür(string ElemanAdıDizisi, bool SadeceElemanları)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null) return "";

                string eleman_adı_ve_içeriği = "";

                if (SadeceElemanları)
                {
                    _YazıyaDönüştür_(bulunan, -1);

                    int konum_n = eleman_adı_ve_içeriği.IndexOf(Depo_Ayraçlar.Eleman);
                    eleman_adı_ve_içeriği = eleman_adı_ve_içeriği.Substring(konum_n + 1);
                }
                else
                {
                    _YazıyaDönüştür_(bulunan, 0);
                }

                return eleman_adı_ve_içeriği;

                void _YazıyaDönüştür_(Eleman_ Eleman, int Seviye)
                {
                    if (Eleman == null || Eleman.İçiBoşOlduğuİçinSilinecek) return;

                    //Seviye belirteci
                    for (int i = 0; i < Seviye; i++) eleman_adı_ve_içeriği += Depo_Ayraçlar.AdıVeİçerik;

                    //Adı
                    eleman_adı_ve_içeriği += Depo_Ayraçlar.KullancıYazısı_Depoya_Göre_Uygunlaştır(Eleman.Adı);

                    //İçeriği
                    foreach (string içerik in Eleman.İçeriği)
                    {
                        eleman_adı_ve_içeriği += Depo_Ayraçlar.AdıVeİçerik + Depo_Ayraçlar.KullancıYazısı_Depoya_Göre_Uygunlaştır(içerik);
                    }

                    //bitiş karakteri
                    eleman_adı_ve_içeriği += Depo_Ayraçlar.Eleman;

                    //Alt elemanları
                    if (Eleman._Elemanları != null)
                    {
                        foreach (Eleman_ AltEleman in Eleman._Elemanları)
                        {
                            _YazıyaDönüştür_(AltEleman, Seviye + 1);
                        }
                    }
                }
            }
            public void Ekle(string ElemanAdıDizisi, string YazıOlarakElemanlar)
            {
                if (!string.IsNullOrEmpty(YazıOlarakElemanlar))
                {
                    string[] dizi = YazıOlarakElemanlar.Split(Depo_Ayraçlar.Eleman);
                    if (dizi != null && dizi.Length > 0)
                    {
                        List<string> Elemanlar = dizi.ToList();
                        List<Eleman_> gecici = new List<Eleman_>();

                        while (Elemanlar.Count > 0)
                        {
                            Eleman_ yeni = new Eleman_(ref Elemanlar, 0, _Depo);
                            if (!yeni.İçiBoşOlduğuİçinSilinecek) gecici.Add(yeni);
                        }

                        if (gecici.Count > 0)
                        {
                            bool Çıkışta_içeriği_temizle = true;
                            Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);

                            if (bulunan == null)
                            {
                                Yaz(ElemanAdıDizisi, "ArGeMuP");
                                bulunan = Bul_Getir(ElemanAdıDizisi);
                            }
                            else Çıkışta_içeriği_temizle = false;

                            if (bulunan._Elemanları == null) bulunan._Elemanları = new Eleman_[gecici.Count];
                            else Array.Resize(ref bulunan._Elemanları, bulunan._Elemanları.Length + gecici.Count);

                            Array.Copy(gecici.ToArray(), 0, bulunan.Elemanları, bulunan._Elemanları.Length - gecici.Count, gecici.Count);

                            if (Çıkışta_içeriği_temizle) bulunan[0] = null;

                            _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                        }
                    }
                }
            }
            public void Sil(string ElemanAdıDizisi)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan != null) bulunan.Adı = null;
            }

            #region İç Kullanım
            bool Elemanları_BoşMu()
            {
                if (_Elemanları == null) return true;

                List<Eleman_> DoluOlanlar = new List<Eleman_>();
                if (_Elemanları.Length > 0)
                {
                    foreach (Eleman_ Eleman in _Elemanları)
                    {
                        if (Eleman.İçiBoşOlduğuİçinSilinecek) continue;
                        DoluOlanlar.Add(Eleman);
                    }
                }

                if (DoluOlanlar.Count == 0)
                {
                    if (_Elemanları.Length > 0) _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                    _Elemanları = null;
                    return true;
                }
                else if (_Elemanları.Length != DoluOlanlar.Count)
                {
                    _Elemanları = DoluOlanlar.ToArray();
                    _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                }
                return false;
            }
            bool İçeriği_BoşMu()
            {
                if (_İçeriği == null) return true;
                if (_İçeriği.Length > 0)
                {
                    foreach (string biri in _İçeriği)
                    {
                        if (string.IsNullOrEmpty(biri)) continue;

                        //boş değil, en sondan itibaren boş girdileri sil
                        int sayac_boşluk = 0;
                        for (int i = _İçeriği.Length - 1; i >= 0; i--)
                        {
                            if (string.IsNullOrEmpty(_İçeriği[i])) sayac_boşluk++;
                            else break;
                        }

                        if (sayac_boşluk > 0) Array.Resize(ref _İçeriği, _İçeriği.Length - sayac_boşluk);
                        return false;
                    }
                }

                _İçeriği = null;
                return true;
            }
            #endregion

            #region IEleman
            public string Adı
            {
                get
                {
                    return _Adı;
                }
                set
                {
                    if (string.IsNullOrEmpty(value)) value = null;

                    if (_Adı != value)
                    {
                        _Adı = value;
                        _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                    }
                }
            }
            public string this[int i] //içeriği
            {
                get
                {
                    if (_İçeriği == null || i >= _İçeriği.Length) return null;
                    else return _İçeriği[i];
                }
                set
                {
                    if (string.IsNullOrEmpty(value)) value = null;

                    if (_İçeriği == null)
                    {
                        if (value == null) return;

                        _İçeriği = new string[i + 1];
                    }
                    else if (i >= _İçeriği.Length)
                    {
                        if (value == null) return;

                        Array.Resize(ref _İçeriği, i + 1);
                    }

                    if (_İçeriği[i] != value)
                    {
                        _İçeriği[i] = value;
                        _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                    }
                }
            }
            public string[] İçeriği
            {
                get
                {
                    if (_İçeriği == null) return new string[0];
                    else return _İçeriği;
                }
                set
                {
                    _İçeriği = value;
                    _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                }
            }
            public IDepo_Eleman[] Elemanları
            {
                get
                {
                    if (_Elemanları == null) return new IDepo_Eleman[0];
                    else return _Elemanları;
                }
            }
            public bool İçiBoşOlduğuİçinSilinecek
            {
                get
                {
                    if (string.IsNullOrEmpty(_Adı)) return true;
                    if (!İçeriği_BoşMu()) return false;
                    return Elemanları_BoşMu();
                }
            }

            public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, D_Sayı.Yazıya(Sayı), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet, int BaşlangıçKonumu, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, D_HexYazı.BaytDizisinden(BaytDizisi, Adet, BaşlangıçKonumu), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, D_TarihSaat.Yazıya(TarihSaat), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, Bit.ToString(), SıraNo);
            }

            public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                try
                {
                    return D_Sayı.Yazıdan(Oku(ElemanAdıDizisi, null, SıraNo));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                try
                {
                    return D_HexYazı.BaytDizisine(Oku(ElemanAdıDizisi, null, SıraNo));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                try
                {
                    return D_TarihSaat.Tarihe(Oku(ElemanAdıDizisi, null, SıraNo));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                try
                {
                    return bool.Parse(Oku(ElemanAdıDizisi, null, SıraNo));
                }
                catch (Exception) { }

                return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }

            public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur)
            {
                return Bul_Getir(ElemanAdıDizisi, YoksaOluştur);
            }
            #endregion
        }

        public Depo_(string YazıOlarakElemanlar = null, Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null)
        {
            Ekle(YazıOlarakElemanlar);

            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;

            this.GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti;
        }

        /// <param name="ElemanAdıDizisi">Ayraçlar.ElemanAdıDizisi karakteri / ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        /// <param name="İçeriği">Elemanın içeriği (İçeriği ve Elemanları olmayan Elemanlar YazıyaDönüştürme aşamasında görmezden gelinip silinir)</param>
        /// <param name="SıraNo">Aynı eleman adı ile birden fazla içerik tutmak için kullanılabilir</param>
        public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi)) throw new Exception("ElemanAdıDizisi boş olamaz");

            //alttaki elemanlardan bahsediyor
            string geriyekalan = Depo_Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

            if (_Elemanları == null) _Elemanları = new Eleman_[0];

            Eleman_ bulunan = Array.Find(_Elemanları, x => x.Adı == ElemanAdıDizisi);
            if (bulunan != null) bulunan.Yaz(geriyekalan, İçeriği, SıraNo);
            else
            {
                //bulunamadığından yeni bir eleman oluştur
                if (!string.IsNullOrEmpty(İçeriği))
                {
                    bulunan = new Eleman_(ElemanAdıDizisi, this);
                    Array.Resize(ref _Elemanları, _Elemanları.Length + 1);
                    _Elemanları[_Elemanları.Length - 1] = bulunan;
                    bulunan.Yaz(geriyekalan, İçeriği, SıraNo);
                }
            }
        }
        public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi)) throw new Exception("ElemanAdıDizisi boş olamaz");

            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            string okunan = bulunan[SıraNo];
            if (string.IsNullOrEmpty(okunan)) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return okunan;
        }
        public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi)) throw new Exception("ElemanDizisi boş olamaz");

            //alttaki elemanlardan bahsediyor
            IDepo_Eleman bulunan = null;
            if (_Elemanları == null) goto OluşturmayıKontrolEt;

            string geriyekalan = Depo_Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);

            bulunan = Array.Find(_Elemanları, x => x.Adı == ElemanAdıDizisi);
            if (bulunan == null) goto OluşturmayıKontrolEt;

            bulunan = bulunan.Bul(geriyekalan, YoksaOluştur);
            return bulunan; 

            OluşturmayıKontrolEt:
            if (YoksaOluştur)
            {
                Yaz(ElemanAdıDizisi, "ArGeMuP");
                bulunan = Bul(ElemanAdıDizisi, false);
            }

            return bulunan;
        }

        #region Dönüştürme
        public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, D_Sayı.Yazıya(Sayı), SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, D_HexYazı.BaytDizisinden(BaytDizisi, Adet, BaşlangıçKonumu), SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, D_TarihSaat.Yazıya(TarihSaat), SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0)
        {
            Yaz(ElemanAdıDizisi, Bit.ToString(), SıraNo);
        }

        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_Sayı(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_BaytDizisi(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_TarihSaat(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_Bit(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }

        public string YazıyaDönüştür(string ElemanAdıDizisi = null, bool SadeceElemanları = false)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi))
            {
                if (_Elemanları == null || _Elemanları.Length == 0) return "";

                string çıktı = "";
                foreach (Eleman_ Eleman in _Elemanları)
                {
                    string oluşturulan = Eleman.YazıyaDönüştür(null, false);
                    if (!string.IsNullOrEmpty(oluşturulan)) çıktı += oluşturulan;
                }

                return çıktı;
            }
            else
            {
                IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
                if (bulunan == null) return "";

                return bulunan.YazıyaDönüştür(null, SadeceElemanları);
            }
        }
        public void Ekle(string YazıOlarakElemanlar)
        {
            // eleman1'içerik[0]'içerik[1]\n                    0
            // 'eleman1_alteleman1'içerik[0]\n                  1
            // 'eleman1_alteleman2'içerik[0]\n                  1
            // ''eleman1_alteleman2_alteleman1'içerik[0]\n      2
            // eleman2'içerik[0]'içerik[1]\n                    0

            if (!string.IsNullOrEmpty(YazıOlarakElemanlar))
            {
                string[] dizi = YazıOlarakElemanlar.Split(Depo_Ayraçlar.Eleman);
                if (dizi != null && dizi.Length > 0)
                {
                    List<string> Elemanlar = dizi.ToList();
                    List<Eleman_> gecici = new List<Eleman_>();

                    while (Elemanlar.Count > 0)
                    {
                        Eleman_ yeni = new Eleman_(ref Elemanlar, 0, this);
                        if (!yeni.İçiBoşOlduğuİçinSilinecek) gecici.Add(yeni);
                    }

                    if (gecici.Count > 0)
                    {
                        if (_Elemanları == null) _Elemanları = new Eleman_[gecici.Count];
                        else Array.Resize(ref _Elemanları, _Elemanları.Length + gecici.Count);

                        Array.Copy(gecici.ToArray(), 0, Elemanları, _Elemanları.Length - gecici.Count, gecici.Count);

                        EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                    }
                }
            }
        }
        public void Sil(string ElemanAdıDizisi)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan != null) bulunan.Sil(null);
        }
        #endregion
    }
}

namespace ArgeMup.HazirKod.EşZamanlıÇokluErişim
{
    public class Depo_
    {
        public const string Sürüm = "V1.0";
        public int Kilit_Devralma_ZamanAşımı_msn = 10000;
        public bool EnAzBir_ElemanAdıVeyaİçeriği_Değişti
        {
            get
            {
                return Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti;
            }
            set
            {
                if (value)
                {
                    if (Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti)
                    {
                        if (GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti != null) GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti(this);
                    }
                }
                else Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;
            }
        }
        public IDepo_Eleman[] Elemanları
        {
            get
            {
                if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                IDepo_Eleman[] elemanlar = Depo.Elemanları;
                if (elemanlar != null)
                {
                    IDepo_Eleman[] yeni = new IDepo_Eleman[elemanlar.Length];
                    for (int i = 0; i < elemanlar.Length; i++)
                    {
                        yeni[i] = new Depo_Kilitli_Eleman_(elemanlar[i], this);
                    }
                    elemanlar = yeni;
                }

                Kilit.ReleaseMutex();

                return elemanlar;
            }
        }

        Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null;
        ArgeMup.HazirKod.Depo_ Depo;
        Mutex Kilit;
        class Depo_Kilitli_Eleman_ : IDepo_Eleman
        {
            IDepo_Eleman AsılEleman;
            Depo_ Depo;

            public Depo_Kilitli_Eleman_(IDepo_Eleman AsılEleman, Depo_ Depo_Kilitli)
            {
                this.AsılEleman = AsılEleman;
                this.Depo = Depo_Kilitli;

                this.Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;
            }

            public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, İçeriği, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                string okunan = AsılEleman.Oku(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public string YazıyaDönüştür(string ElemanAdıDizisi, bool SadeceElemanları)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                string okunan = AsılEleman.YazıyaDönüştür(ElemanAdıDizisi, SadeceElemanları);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public void Ekle(string ElemanAdıDizisi, string YazıOlarakElemanlar)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Ekle(ElemanAdıDizisi, YazıOlarakElemanlar);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Sil(string ElemanAdıDizisi)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Sil(ElemanAdıDizisi);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }

            #region IEleman
            public string Adı
            {
                get
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    string okunan = AsılEleman.Adı;

                    Depo.Kilit.ReleaseMutex();

                    return okunan;
                }
                set
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    AsılEleman.Adı = value;
                    Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                    Depo.Kilit.ReleaseMutex();
                }
            }
            public string this[int i] //içeriği
            {
                get
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    string okunan = AsılEleman[i];

                    Depo.Kilit.ReleaseMutex();

                    return okunan;
                }
                set
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    AsılEleman[i] = value;
                    Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                    Depo.Kilit.ReleaseMutex();
                }
            }
            public string[] İçeriği
            {
                get
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    string[] okunan = AsılEleman.İçeriği;

                    Depo.Kilit.ReleaseMutex();

                    return okunan;
                }
                set
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    AsılEleman.İçeriği = value;
                    Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                    Depo.Kilit.ReleaseMutex();
                }
            }
            public IDepo_Eleman[] Elemanları
            {
                get
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    IDepo_Eleman[] elemanlar = AsılEleman.Elemanları;
                    if (elemanlar != null)
                    {
                        IDepo_Eleman[] yeni = new IDepo_Eleman[elemanlar.Length];
                        for (int i = 0; i < elemanlar.Length; i++)
                        {
                            yeni[i] = new Depo_Kilitli_Eleman_(elemanlar[i], Depo);
                        }
                        elemanlar = yeni;
                    }

                    Depo.Kilit.ReleaseMutex();

                    return elemanlar;
                }
            }
            public bool İçiBoşOlduğuİçinSilinecek
            {
                get
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    bool okunan = AsılEleman.İçiBoşOlduğuİçinSilinecek;

                    Depo.Kilit.ReleaseMutex();

                    return okunan;
                }
            }

            public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, Sayı, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet, int BaşlangıçKonumu, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, BaytDizisi, Adet, BaşlangıçKonumu, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, TarihSaat, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, Bit, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }

            public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                double okunan = AsılEleman.Oku_Sayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                byte[] okunan = AsılEleman.Oku_BaytDizisi(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                DateTime okunan = AsılEleman.Oku_TarihSaat(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                bool okunan = AsılEleman.Oku_Bit(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }

            public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                IDepo_Eleman okunan = AsılEleman.Bul(ElemanAdıDizisi, YoksaOluştur);
                if (okunan != null) okunan = new Depo_Kilitli_Eleman_(okunan, Depo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            #endregion
        }

        public Depo_(string YazıOlarakElemanlar = null, Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null)
        {
            Depo = new ArgeMup.HazirKod.Depo_(YazıOlarakElemanlar);
            Kilit = new Mutex();

            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;
            this.GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti;
        }

        /// <param name="ElemanAdıDizisi">Ayraçlar.ElemanAdıDizisi karakteri / ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        /// <param name="İçeriği">Elemanın içeriği (İçeriği ve Elemanları olmayan Elemanlar YazıyaDönüştürme aşamasında görmezden gelinip silinir)</param>
        /// <param name="SıraNo">Aynı eleman adı ile birden fazla içerik tutmak için kullanılabilir</param>
        public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, İçeriği, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            string okunan = Depo.Oku(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();
            
            return okunan;
        }
        public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            IDepo_Eleman okunan = Depo.Bul(ElemanAdıDizisi, YoksaOluştur);
            if (okunan != null) okunan = new Depo_Kilitli_Eleman_(okunan, this);

            Kilit.ReleaseMutex();

            return okunan;
        }

        #region Dönüştürme
        public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, Sayı, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int Adet = int.MinValue, int BaşlangıçKonumu = 0, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, BaytDizisi, Adet, BaşlangıçKonumu, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, TarihSaat, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, Bit, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }

        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            double okunan = Depo.Oku_Sayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            byte[] okunan = Depo.Oku_BaytDizisi(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            DateTime okunan = Depo.Oku_TarihSaat(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            bool okunan = Depo.Oku_Bit(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }

        public string YazıyaDönüştür(string ElemanAdıDizisi = null, bool SadeceElemanları = false)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            string okunan = Depo.YazıyaDönüştür(ElemanAdıDizisi, SadeceElemanları);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public void Ekle(string YazıOlarakElemanlar)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Ekle(YazıOlarakElemanlar);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Sil(string ElemanAdıDizisi)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Sil(ElemanAdıDizisi);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        #endregion
    }
}