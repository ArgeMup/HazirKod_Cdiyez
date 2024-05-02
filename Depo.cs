// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Linq;
using System.Collections.Generic;

namespace ArgeMup.HazirKod
{
    public interface IDepo_Eleman
    {
        string Adı { get; set; }
        /// <summary>
        /// İçeriği
        /// </summary>
        string this[int SıraNo] { get; set; }
        /// <summary>
        /// Tüm İçeriği
        /// </summary>
        string[] İçeriği { get; set; }
        /// <summary>
        /// Elemanı
        /// </summary>
        IDepo_Eleman this[string ElemanAdıDizisi] { get; }
        /// <summary>
        /// Tüm Elemanları
        /// </summary>
        IDepo_Eleman[] Elemanları { get; }
        /// <summary>
        /// Elemanın İçeriği 
        /// </summary>
        string this[string ElemanAdıDizisi, int SıraNo] { get; set; }
        /// <summary>
        /// Elemanın İçeriğinin Türü ile Birlikte Değerlendirilmesi
        /// Okuma : BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği türünde bilgi üretir
        /// Yazma : Yeni değer türünden yazıya dönüştürerek ilerler
        /// </summary>
        object this[string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği] { get; set; }
        /// <summary>
        /// YazıyaDönüştür aşamasında eleneceğini belirtir
        /// </summary>
        bool İçiBoşOlduğuİçinSilinecek { get; }

        /// <param name="ElemanAdıDizisi">Ayraçlar.ElemanAdıDizisi karakteri / ile ayrılmış sıralı eleman göstergesi E1/E2/E3 ile E1 içindeki E2 içindeki E3 anlaşılır</param>
        /// <param name="İçeriği">Elemanın içeriği (İçeriği ve Elemanları olmayan Elemanlar YazıyaDönüştürme aşamasında görmezden gelinip silinir)</param>
        /// <param name="SıraNo">Aynı eleman adı ile birden fazla içerik tutmak için kullanılabilir</param>
        void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, decimal HassasSayı, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, int TamSayı, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0);
        void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0);

        string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0);
        double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);
        decimal Oku_HassasSayı(string ElemanAdıDizisi, decimal BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);
        int Oku_TamSayı(string ElemanAdıDizisi, int BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);
        byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0);
        DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);
        bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0);

        IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false, bool BağımsızKopyaOluştur = false);
        string YazıyaDönüştür(string ElemanAdıDizisi, bool SadeceElemanları = false, bool DoğrulamaKoduEkle = true);
        void Ekle(string ElemanAdıDizisi, string Eleman, bool DoğrulamaKoduOlmalı = true);
        void Sil(string ElemanAdıDizisi, bool Sadeceİçeriğini = false, bool SadeceElemanlarını = false);
        /// <summary>
        /// <br>ElemanAdıDizisi ile seçilen elemana ait içerik veya alt elemanları sıralamak için kullanılabilir</br>
        /// <br>EnAzBir_ElemanAdıVeyaİçeriği_Değişti bayrağını mutlaka kurar</br>
        /// </summary>
        /// <param name="ElemanAdıDizisi">İstenilen elemanın adı</param>
        /// <param name="SıraNo">
        /// <br>-2 : Elemanın İÇERİĞİNİ istenilen türe çevirip sıralar</br>
        /// <br>-1 : Elemanın ALT_ELEMANLARINI, ALT_ELEMANIN_ADI bilgisini istenilen türe çevirip sıralar</br>
        /// <br>0 ... : Elemanın ALT_ELEMANLARINI, ALT_ELEMANIN_BELİRTİLEN_SIRANO içeriğini istenilen türe çevirip sıralar</br></param>
        /// <param name="BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği">Karşılaştırma yapılacak türün tayin edilmesi ve içeriğin olmaması durumunda kullanılır</param>
        /// <param name="Tersten">İşaretlenirse
        /// <br>Yazı : Z'den A'ya</br>
        /// <br>Sayı : Büyükten küçüğe</br>
        /// <br>Tarih Saat : Yeniden eskiye</br>
        /// <br>Bit : 1'den 0'a doğru sıralanır</br></param>
        void Sırala(string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten = false);
        void Sırala(string ElemanAdıDizisi, List<string> ElemanAdıSıralaması);
        List<string> Listele(string ElemanAdıDizisi);
    }

    public class Depo_Ayraçlar
    {
        public static readonly char ElemanAdıDizisi = '/';

        public static readonly char Eleman = '\n';
        public static readonly char Eleman2 = '\r';
        public static readonly char AdıVeİçerik = '>';

        static readonly string Vekil_Eleman = " ?'#{n]$ ";
        static readonly string Vekil_Eleman2 = " ?'#{r]$ ";
        static readonly string Vekil_AdıVeİçerik = " ?'#{b]$ ";

        public static readonly string Seviye_DoğrulamaKodu = "ArgeMup.HazirKod_Cdiyez.Depo_" + AdıVeİçerik;
        public static readonly string Seviye_ArtıBir = "+" + AdıVeİçerik;
        public static readonly string Seviye_Eşittir = "=" + AdıVeİçerik;
        public static readonly string Seviye_EksiBir = "-" + AdıVeİçerik;

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
            int ayırma_karakteri_konumu = Girdi.IndexOf(ElemanAdıDizisi);
            if (ayırma_karakteri_konumu == 0 || Girdi.Last() == ElemanAdıDizisi) throw new Exception("ElemanAdıDizisi " + ElemanAdıDizisi + " ile başlayamaz, bitemez");

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
        public const string Sürüm = "V1.3";
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
        /// <summary>
        /// Elemanı
        /// </summary>
        public IDepo_Eleman this[string ElemanAdıDizisi]
        {
            get
            {
                return Bul(ElemanAdıDizisi, true, false);
            }
        }
        /// <summary>
        /// Tüm Elemanları
        /// </summary>
        public IDepo_Eleman[] Elemanları
        {
            get
            {
                if (_Elemanları == null) return new IDepo_Eleman[0];

                return _Elemanları;
            }
        }
        /// <summary>
        /// Elemanın İçeriği 
        /// </summary>
        public string this[string ElemanAdıDizisi, int SıraNo]
        {
            get
            {
                return Oku(ElemanAdıDizisi, null, SıraNo);
            }
            set
            {
                Yaz(ElemanAdıDizisi, value, SıraNo);
            }
        }
        /// <summary>
        /// Elemanın İçeriğinin Türü ile Birlikte Değerlendirilmesi
        /// Okuma : BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği türünde bilgi üretir
        /// Yazma : Yeni değer türünden yazıya dönüştürerek ilerler
        /// </summary>
        public object this[string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği]
        {
            get
            {
                if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is string) return Oku(ElemanAdıDizisi, (string)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is double) return Oku_Sayı(ElemanAdıDizisi, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is decimal) return Oku_HassasSayı(ElemanAdıDizisi, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is int) return Oku_TamSayı(ElemanAdıDizisi, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is bool) return Oku_Bit(ElemanAdıDizisi, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is DateTime) return Oku_TarihSaat(ElemanAdıDizisi, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is byte[]) return Oku_BaytDizisi(ElemanAdıDizisi, (byte[])BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else throw new Exception("Desteklenmeyen tür " + BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği.GetType().FullName);
            }
            set
            {
                if (value is string) Yaz(ElemanAdıDizisi, (string)value, SıraNo);
                else if (value is double) Yaz(ElemanAdıDizisi, (double)value, SıraNo);
                else if (value is decimal) Yaz(ElemanAdıDizisi, (decimal)value, SıraNo);
                else if (value is int) Yaz(ElemanAdıDizisi, (int)value, SıraNo);
                else if (value is bool) Yaz(ElemanAdıDizisi, (bool)value, SıraNo);
                else if (value is DateTime) Yaz(ElemanAdıDizisi, (DateTime)value, SıraNo);
                else if (value is byte[]) Yaz(ElemanAdıDizisi, (byte[])value, SıraNo);
                else throw new Exception("Desteklenmeyen tür " + value.GetType().FullName);
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
            public Eleman_(List<string> Eleman_AdıİçeriğiElemaları, int BirÖncekiSeviye, Depo_ Depo)
            {
                // seviye0>eleman1>içerik[0]>içerik[1]\n                0
                // seviye1>eleman1_alteleman1>içerik[0]\n               1
                // seviye1>eleman1_alteleman2>içerik[0]\n               1
                // seviye2>eleman1_alteleman2_alteleman1>içerik[0]\n    2
                // seviye0>eleman2>içerik[0]>içerik[1]\n                0
                
                if (Depo == null) throw new Exception("Depo boş olamaz");
                _Depo = Depo;
                _Elemanları = new Eleman_[0];

                string[] dizi_içerik = null;
                int okunan_seviye = -1, sonrakinin_seviyesi = -1;

                _SıradakiniOku_(ref okunan_seviye);
                if (okunan_seviye != BirÖncekiSeviye + 1) throw new Exception("Seviye bilgisi uyumsuz - " + Eleman_AdıİçeriğiElemaları[0]);

                _Adı = Depo_Ayraçlar.KullancıYazısı_İlkHalineGetir(dizi_içerik[1]);
                _İçeriği = new string[dizi_içerik.Length - 2 /*Seviye + adı*/];
                for (int i = 0; i < _İçeriği.Length; i++)
                {
                    _İçeriği[i] = Depo_Ayraçlar.KullancıYazısı_İlkHalineGetir(dizi_içerik[i + 2]);
                    if (string.IsNullOrEmpty(_İçeriği[i])) _İçeriği[i] = null;
                }

                Eleman_AdıİçeriğiElemaları.RemoveAt(0); 

            YenidenDene:
                _SıradakiniOku_(ref sonrakinin_seviyesi);
                if (sonrakinin_seviyesi <= okunan_seviye)
                {
                    Elemanları_BoşMu();
                    return;
                }

                //sonraki eleman şimdiki elemanın alt elemanı olduğundan içerden devam edecek
                Array.Resize(ref _Elemanları, _Elemanları.Length + 1);
                _Elemanları[_Elemanları.Length - 1] = new Eleman_(Eleman_AdıİçeriğiElemaları, okunan_seviye, Depo);
                goto YenidenDene;

                void _SıradakiniOku_(ref int _Seviye_)
                {
                    while (Eleman_AdıİçeriğiElemaları.Count > 0 && string.IsNullOrEmpty(Eleman_AdıİçeriğiElemaları[0])) Eleman_AdıİçeriğiElemaları.RemoveAt(0);
                    if (Eleman_AdıİçeriğiElemaları.Count <= 0) { _Seviye_ = -1; return; }

                    _YenidenDene_:
                    string _sıradaki_ = Eleman_AdıİçeriğiElemaları[0];
                    dizi_içerik = _sıradaki_.Split(Depo_Ayraçlar.AdıVeİçerik);
                    if (dizi_içerik == null || dizi_içerik.Length < 2 /*seviye + adı*/) throw new Exception("içerik uygun değil " + _sıradaki_);
                    
                    if (!int.TryParse(dizi_içerik[0], out _Seviye_))
                    {
                        //seviye takibi yaparak işaretlenmiş satırları çöz
                        int _düzeltme_bir_önceki_seviye_ = okunan_seviye;
                        for (int _düzeltme_i_ = 0; _düzeltme_i_ < Eleman_AdıİçeriğiElemaları.Count; _düzeltme_i_++)
                        {
                            string _düzeltme_içerik_ = Eleman_AdıİçeriğiElemaları[_düzeltme_i_];
                            if (string.IsNullOrEmpty(_düzeltme_içerik_)) continue;

                            if (_düzeltme_içerik_.StartsWith(Depo_Ayraçlar.Seviye_ArtıBir)) { _düzeltme_bir_önceki_seviye_++; Eleman_AdıİçeriğiElemaları[_düzeltme_i_] = _düzeltme_bir_önceki_seviye_ + _düzeltme_içerik_.Remove(0, 1); }
                            else if (_düzeltme_içerik_.StartsWith(Depo_Ayraçlar.Seviye_Eşittir)) { Eleman_AdıİçeriğiElemaları[_düzeltme_i_] = _düzeltme_bir_önceki_seviye_ + _düzeltme_içerik_.Remove(0, 1); }
                            else if (_düzeltme_içerik_.StartsWith(Depo_Ayraçlar.Seviye_EksiBir)) { _düzeltme_bir_önceki_seviye_--; Eleman_AdıİçeriğiElemaları[_düzeltme_i_] = _düzeltme_bir_önceki_seviye_ + _düzeltme_içerik_.Remove(0, 1); }
                            else
                            {
                                string[] _düzeltme_içerik_dizi_ = _düzeltme_içerik_.Split(Depo_Ayraçlar.AdıVeİçerik);
                                if (_düzeltme_içerik_dizi_ == null || _düzeltme_içerik_dizi_.Length < 2 /*seviye + adı*/) throw new Exception("içerik uygun değil " + _düzeltme_içerik_);
                                if (!int.TryParse(_düzeltme_içerik_dizi_[0], out _düzeltme_bir_önceki_seviye_)) throw new Exception("seviye uygun değil" + _düzeltme_içerik_dizi_[0]);
                            }
                        }
                        goto _YenidenDene_; 
                    }
                    
                    if (_Seviye_ < 0) throw new Exception("seviye uygun değil" + _sıradaki_ + " " + _Seviye_);
                }
            }
            //Eski sistemden okumak için
            public Eleman_(ref List<string> Eleman_AdıİçeriğiElemaları, int Seviye, Depo_ Depo)
            {
                // eleman1>içerik[0]>içerik[1]\n                    0
                // >eleman1_alteleman1>içerik[0]\n                  1
                // >eleman1_alteleman2>içerik[0]\n                  1
                // >>eleman1_alteleman2_alteleman1>içerik[0]\n      2
                // eleman2>içerik[0]>içerik[1]\n                    0

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
            public Eleman_ Bul_Getir(string ElemanAdıDizisi)
            {
                if (string.IsNullOrEmpty(ElemanAdıDizisi)) return this;
                else
                {
                    //alttaki elemanlardan bahsediyor
                    if (_Elemanları == null) return null;

                    string geriyekalan = Depo_Ayraçlar.ElemanAdıDizisi_Ayıkla(ref ElemanAdıDizisi);
                    Eleman_ bulunan = Array.Find(_Elemanları, x => x._Adı == ElemanAdıDizisi);
                    if (bulunan == null) return null;

                    return bulunan.Bul_Getir(geriyekalan);
                }
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
            public override string ToString()
            {
                int adet = İçeriği.Length;
                string içerik = "Adı : " + _Adı + ", İçeriği [" + adet + "]";

                if (adet > 0)
                {
                    if (adet > 5) adet = 5;

                    içerik += " {";

                    for (int i = 0; i < adet; i++) içerik += (_İçeriği[i] == null ? "boş" : _İçeriği[i]) + ", ";

                    içerik = içerik.TrimEnd(',', ' ') + "}";
                }

                adet = Elemanları.Length;
                içerik += ", Elemanları [" + adet + "]";

                if (adet > 0)
                {
                    if (adet > 5) adet = 5;

                    içerik += " {";

                    for (int i = 0; i < adet; i++) içerik += (_Elemanları[i] == null ? "boş" : _Elemanları[i]._Adı) + ", ";

                    içerik = içerik.TrimEnd(',', ' ') + "}";
                }

                return içerik;
            }

            double Dönüştür(string İçerik, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği)
            {
                if (double.TryParse(İçerik, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double dönüştürülen)) return dönüştürülen;
                else return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            decimal Dönüştür(string İçerik, decimal BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği)
            {
                if (decimal.TryParse(İçerik, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal dönüştürülen)) return dönüştürülen;
                else return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            int Dönüştür(string İçerik, int BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği)
            {
                if (int.TryParse(İçerik, out int dönüştürülen)) return dönüştürülen;
                else return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            byte[] Dönüştür(string İçerik, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği)
            {
                try { return Dönüştürme.D_HexYazı.BaytDizisine(İçerik); }
                catch (Exception) { return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği; }
            }
            DateTime Dönüştür(string İçerik, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği)
            {
                if (DateTime.TryParseExact(İçerik, Dönüştürme.D_TarihSaat.Şablon_Tarih_Saat_MiliSaniye, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime dönüştürülen)) return dönüştürülen;
                else return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }
            bool Dönüştür(string İçerik, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği)
            {
                if (bool.TryParse(İçerik, out bool dönüştürülen)) return dönüştürülen;
                else return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
            }

            class _Sıralayıcı_Yazı_ : IComparer<string>
            {
                object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
                bool Tersten;
                Eleman_ Eleman;

                enum Tür_ { Yazı, Sayı, HassasSayı, TamSayı, Bit, TarihSaat };
                Tür_ Tür;

                public _Sıralayıcı_Yazı_(object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten, Eleman_ Eleman)
                {
                    this.BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
                    this.Tersten = Tersten;
                    this.Eleman = Eleman;

                    if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is string) Tür = Tür_.Yazı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is double) Tür = Tür_.Sayı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is decimal) Tür = Tür_.HassasSayı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is int) Tür = Tür_.TamSayı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is bool) Tür = Tür_.Bit;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is DateTime) Tür = Tür_.TarihSaat;
                    else throw new Exception("Desteklenmeyen tür " + BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği.GetType().FullName);
                }

                public int Compare(string A, string B)
                {
                    int sonuç = 0;

                    switch (Tür)
                    {
                        case Tür_.Yazı:
                            sonuç = A.CompareTo(B);
                            break;

                        case Tür_.Sayı:
                            double Sayı_A = Eleman.Dönüştür(A, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
                            double Sayı_B = Eleman.Dönüştür(B, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);

                            if (Sayı_A > Sayı_B) sonuç = 1;
                            else if (Sayı_A < Sayı_B) sonuç = -1;
                            break;

                        case Tür_.HassasSayı:
                            decimal HassasSayı_A = Eleman.Dönüştür(A, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
                            decimal HassasSayı_B = Eleman.Dönüştür(B, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);

                            if (HassasSayı_A > HassasSayı_B) sonuç = 1;
                            else if (HassasSayı_A < HassasSayı_B) sonuç = -1;
                            break;

                        case Tür_.TamSayı:
                            int TamSayı_A = Eleman.Dönüştür(A, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
                            int TamSayı_B = Eleman.Dönüştür(B, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);

                            if (TamSayı_A > TamSayı_B) sonuç = 1;
                            else if (TamSayı_A < TamSayı_B) sonuç = -1;
                            break;

                        case Tür_.Bit:
                            bool Bit_A = Eleman.Dönüştür(A, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
                            bool Bit_B = Eleman.Dönüştür(B, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);

                            if (Bit_A && !Bit_B) sonuç = 1;
                            else if (!Bit_A && Bit_B) sonuç = -1;
                            break;

                        case Tür_.TarihSaat:
                            DateTime TarihSaat_A = Eleman.Dönüştür(A, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
                            DateTime TarihSaat_B = Eleman.Dönüştür(B, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);

                            if (TarihSaat_A > TarihSaat_B) sonuç = 1;
                            else if (TarihSaat_A < TarihSaat_B) sonuç = -1;
                            break;
                    }

                    if (Tersten) sonuç *= -1;
                    return sonuç;
                }
            }
            class _Sıralayıcı_Eleman_ : IComparer<Eleman_>
            {
                int SıraNo; //-1 : Adı, 0 ... : İçerik SıraNo
                object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
                bool Tersten;

                enum Tür_ { Yazı, Sayı, HassasSayı, TamSayı, Bit, TarihSaat };
                Tür_ Tür;

                public _Sıralayıcı_Eleman_(int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten)
                {
                    this.SıraNo = SıraNo;
                    this.BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;
                    this.Tersten = Tersten;

                    if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is string) Tür = Tür_.Yazı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is double) Tür = Tür_.Sayı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is decimal) Tür = Tür_.HassasSayı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is int) Tür = Tür_.TamSayı;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is bool) Tür = Tür_.Bit;
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is DateTime) Tür = Tür_.TarihSaat;
                    else throw new Exception("Desteklenmeyen tür " + BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği.GetType().FullName);
                }

                public int Compare(Eleman_ A, Eleman_ B)
                {
                    int sonuç = 0;

                    switch (Tür)
                    {
                        case Tür_.Yazı:
                            string Yazı_A = SıraNo < 0 ? A.Adı : A.Oku(null, (string)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                            string Yazı_B = SıraNo < 0 ? B.Adı : B.Oku(null, (string)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                            sonuç = Yazı_A.CompareTo(Yazı_B);
                            break;

                        case Tür_.Sayı:
                            double Sayı_A = SıraNo < 0 ? A.Dönüştür(A.Adı, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : A.Oku_Sayı(null, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                            double Sayı_B = SıraNo < 0 ? B.Dönüştür(B.Adı, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : B.Oku_Sayı(null, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                            if (Sayı_A > Sayı_B) sonuç = 1;
                            else if (Sayı_A < Sayı_B) sonuç = -1;
                            break;

                        case Tür_.HassasSayı:
                            decimal HassasSayı_A = SıraNo < 0 ? A.Dönüştür(A.Adı, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : A.Oku_HassasSayı(null, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                            decimal HassasSayı_B = SıraNo < 0 ? B.Dönüştür(B.Adı, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : B.Oku_HassasSayı(null, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                            if (HassasSayı_A > HassasSayı_B) sonuç = 1;
                            else if (HassasSayı_A < HassasSayı_B) sonuç = -1;
                            break;

                        case Tür_.TamSayı:
                            int TamSayı_A = SıraNo < 0 ? A.Dönüştür(A.Adı, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : A.Oku_TamSayı(null, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                            int TamSayı_B = SıraNo < 0 ? B.Dönüştür(B.Adı, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : B.Oku_TamSayı(null, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                            if (TamSayı_A > TamSayı_B) sonuç = 1;
                            else if (TamSayı_A < TamSayı_B) sonuç = -1;
                            break;

                        case Tür_.Bit:
                            bool Bit_A = SıraNo < 0 ? A.Dönüştür(A.Adı, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : A.Oku_Bit(null, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                            bool Bit_B = SıraNo < 0 ? B.Dönüştür(B.Adı, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : B.Oku_Bit(null, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                            if (Bit_A && !Bit_B) sonuç = 1;
                            else if (!Bit_A && Bit_B) sonuç = -1;
                            break;

                        case Tür_.TarihSaat:
                            DateTime TarihSaat_A = SıraNo < 0 ? A.Dönüştür(A.Adı, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : A.Oku_TarihSaat(null, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                            DateTime TarihSaat_B = SıraNo < 0 ? B.Dönüştür(B.Adı, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği) : B.Oku_TarihSaat(null, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                            if (TarihSaat_A > TarihSaat_B) sonuç = 1;
                            else if (TarihSaat_A < TarihSaat_B) sonuç = -1;
                            break;
                    }

                    if (Tersten) sonuç *= -1;
                    return sonuç;
                }
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
            public string this[int SıraNo] //içeriği
            {
                get
                {
                    if (_İçeriği == null || SıraNo >= _İçeriği.Length) return null;
                    else return _İçeriği[SıraNo];
                }
                set
                {
                    if (string.IsNullOrEmpty(value)) value = null;

                    if (_İçeriği == null)
                    {
                        if (value == null) return;

                        _İçeriği = new string[SıraNo + 1];
                    }
                    else if (SıraNo >= _İçeriği.Length)
                    {
                        if (value == null) return;

                        Array.Resize(ref _İçeriği, SıraNo + 1);
                    }

                    if (_İçeriği[SıraNo] != value)
                    {
                        _İçeriği[SıraNo] = value;
                        _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                    }
                }
            }
            public string[] İçeriği //Tüm İçeriği
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
            public IDepo_Eleman this[string ElemanAdıDizisi] //Elemanı
            {
                get
                {
                    return Bul(ElemanAdıDizisi, true, false);
                }
            }
            public IDepo_Eleman[] Elemanları //Tüm Elemanları
            {
                get
                {
                    if (_Elemanları == null) return new IDepo_Eleman[0];
                    else return _Elemanları;
                }
            }
            public string this[string ElemanAdıDizisi, int SıraNo] //Elemanın İçeriği 
            {
                get
                {
                    return Oku(ElemanAdıDizisi, null, SıraNo);
                }
                set
                {
                    Yaz(ElemanAdıDizisi, value, SıraNo);
                }
            }
            public object this[string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği] //Elemanın İçeriğinin Türü ile Birlikte Değerlendirilmesi
            {
                get
                {
                    if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is string) return Oku(ElemanAdıDizisi, (string)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is double) return Oku_Sayı(ElemanAdıDizisi, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is decimal) return Oku_HassasSayı(ElemanAdıDizisi, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is int) return Oku_TamSayı(ElemanAdıDizisi, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is bool) return Oku_Bit(ElemanAdıDizisi, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is DateTime) return Oku_TarihSaat(ElemanAdıDizisi, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is byte[]) return Oku_BaytDizisi(ElemanAdıDizisi, (byte[])BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else throw new Exception("Desteklenmeyen tür " + BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği.GetType().FullName);
                }
                set
                {
                    if (value is string) Yaz(ElemanAdıDizisi, (string)value, SıraNo);
                    else if (value is double) Yaz(ElemanAdıDizisi, (double)value, SıraNo);
                    else if (value is decimal) Yaz(ElemanAdıDizisi, (decimal)value, SıraNo);
                    else if (value is int) Yaz(ElemanAdıDizisi, (int)value, SıraNo);
                    else if (value is bool) Yaz(ElemanAdıDizisi, (bool)value, SıraNo);
                    else if (value is DateTime) Yaz(ElemanAdıDizisi, (DateTime)value, SıraNo);
                    else if (value is byte[]) Yaz(ElemanAdıDizisi, (byte[])value, SıraNo);
                    else throw new Exception("Desteklenmeyen tür " + value.GetType().FullName);
                }
            }
            public bool İçiBoşOlduğuİçinSilinecek
            {
                get
                {
                    bool durum_içerik = İçeriği_BoşMu();
                    bool durum_elemanları = Elemanları_BoşMu();

                    if (string.IsNullOrEmpty(_Adı)) return true;
                    if (!durum_içerik) return false;
                    return durum_elemanları;
                }
            }

            public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo)
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
                        bulunan = new Eleman_(ElemanAdıDizisi, _Depo);
                        Array.Resize(ref _Elemanları, _Elemanları.Length + 1);
                        _Elemanları[_Elemanları.Length - 1] = bulunan;
                        bulunan.Yaz(geriyekalan, İçeriği, SıraNo);
                    }
                }
            }
            public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, Sayı.ToString(System.Globalization.CultureInfo.InvariantCulture), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, decimal HassasSayı, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, HassasSayı.ToString(System.Globalization.CultureInfo.InvariantCulture), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, int TamSayı, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, TamSayı.ToString(), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, Dönüştürme.D_HexYazı.BaytDizisinden(BaytDizisi), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, TarihSaat.ToString(Dönüştürme.D_TarihSaat.Şablon_Tarih_Saat_MiliSaniye, System.Globalization.CultureInfo.InvariantCulture), SıraNo);
            }
            public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo)
            {
                Yaz(ElemanAdıDizisi, Bit.ToString(), SıraNo);
            }

            public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

                string okunan = bulunan[SıraNo];
                if (string.IsNullOrEmpty(okunan)) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

                return okunan;
            }
            public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                string içerik = Oku(ElemanAdıDizisi, null, SıraNo);
                return Dönüştür(içerik, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
            }
            public decimal Oku_HassasSayı(string ElemanAdıDizisi, decimal BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                string içerik = Oku(ElemanAdıDizisi, null, SıraNo);
                return Dönüştür(içerik, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
            }
            public int Oku_TamSayı(string ElemanAdıDizisi, int BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                string içerik = Oku(ElemanAdıDizisi, null, SıraNo);
                return Dönüştür(içerik, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
            }
            public byte[] Oku_BaytDizisi(string ElemanAdıDizisi, byte[] BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                string içerik = Oku(ElemanAdıDizisi, null, SıraNo);
                return Dönüştür(içerik, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
            }
            public DateTime Oku_TarihSaat(string ElemanAdıDizisi, DateTime BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                string içerik = Oku(ElemanAdıDizisi, null, SıraNo);
                return Dönüştür(içerik, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
            }
            public bool Oku_Bit(string ElemanAdıDizisi, bool BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                string içerik = Oku(ElemanAdıDizisi, null, SıraNo);
                return Dönüştür(içerik, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği);
            }

            public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur, bool BağımsızKopyaOluştur)
            {
                IDepo_Eleman bulunan = Bul_Getir(ElemanAdıDizisi);

                if (bulunan == null && YoksaOluştur)
                {
                    Yaz(ElemanAdıDizisi, (string)null, 0);
                    bulunan = Bul_Getir(ElemanAdıDizisi);
                }

                if (!BağımsızKopyaOluştur || bulunan == null) return bulunan;

                Depo_ Bağımsız_yeni_depo = new Depo_(bulunan.YazıyaDönüştür(null, false, false));
                if (Bağımsız_yeni_depo.Elemanları.Length == 0)
                {
                    //Sadece adı olan elemanlardan oluştuğundan (içerik veya elemanı yok) yeni bir nesne oluşturulacak
                    Bağımsız_yeni_depo.Yaz(bulunan.Adı, "");
                }
                return Bağımsız_yeni_depo.Elemanları[0];
            }
            public string YazıyaDönüştür(string ElemanAdıDizisi, bool SadeceElemanları, bool DoğrulamaKoduEkle)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null) return "";

                string eleman_adı_ve_içeriği = "";
                int BirÖncekiSeviye = -5;

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

                if (DoğrulamaKoduEkle && !string.IsNullOrEmpty(eleman_adı_ve_içeriği))
                {
                    eleman_adı_ve_içeriği = Depo_Ayraçlar.Seviye_DoğrulamaKodu + ArgeMup.HazirKod.DoğrulamaKodu.Üret.Yazıdan(eleman_adı_ve_içeriği) + Depo_Ayraçlar.Eleman + eleman_adı_ve_içeriği;
                }

                return eleman_adı_ve_içeriği;

                void _YazıyaDönüştür_(Eleman_ Eleman, int Seviye)
                {
                    if (Eleman == null || Eleman.İçiBoşOlduğuİçinSilinecek) return;

                    //Seviye belirteci
                    if (Seviye <= 9) eleman_adı_ve_içeriği += Seviye.ToString() + Depo_Ayraçlar.AdıVeİçerik;
                    else
                    {
                        int seviye_farkı = Seviye - BirÖncekiSeviye;
                        if (seviye_farkı == 1) eleman_adı_ve_içeriği += Depo_Ayraçlar.Seviye_ArtıBir;
                        else if (seviye_farkı == 0) eleman_adı_ve_içeriği += Depo_Ayraçlar.Seviye_Eşittir;
                        else if (seviye_farkı == -1) eleman_adı_ve_içeriği += Depo_Ayraçlar.Seviye_EksiBir;
                        else eleman_adı_ve_içeriği += Seviye.ToString() + Depo_Ayraçlar.AdıVeİçerik;
                    }
                    BirÖncekiSeviye = Seviye;

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
            public void Ekle(string ElemanAdıDizisi, string YazıOlarakElemanlar, bool DoğrulamaKoduOlmalı)
            {
                List<Eleman_> okunanlar = _Depo.YazıOlarakElemanlar_Ayıkla(YazıOlarakElemanlar, DoğrulamaKoduOlmalı);

                if (okunanlar.Count > 0)
                {
                    Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);

                    if (bulunan == null)
                    {
                        Yaz(ElemanAdıDizisi, (string)null, 0);
                        bulunan = Bul_Getir(ElemanAdıDizisi);
                    }

                    if (bulunan._Elemanları == null) bulunan._Elemanları = new Eleman_[okunanlar.Count];
                    else Array.Resize(ref bulunan._Elemanları, bulunan._Elemanları.Length + okunanlar.Count);

                    Array.Copy(okunanlar.ToArray(), 0, bulunan.Elemanları, bulunan._Elemanları.Length - okunanlar.Count, okunanlar.Count);

                    _Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
                }
            }
            public void Sil(string ElemanAdıDizisi, bool Sadeceİçeriğini, bool SadeceElemanlarını)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan != null)
                {
                    if (!Sadeceİçeriğini && !SadeceElemanlarını) bulunan.Adı = null; //tamamen silinsin
                    else
                    {
                        if (Sadeceİçeriğini) bulunan._İçeriği = null;
                        if (SadeceElemanlarını) bulunan._Elemanları = null;
                    }
                }
            }
            public void Sırala(string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null) return;

                if (SıraNo == -2)
                {
                    if (bulunan._İçeriği == null || bulunan._İçeriği.Length == 0) return;

                    List<string> l = bulunan._İçeriği.ToList();
                    l.Sort(new _Sıralayıcı_Yazı_(BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, Tersten, bulunan));
                    bulunan._İçeriği = l.ToArray();
                }
                else if (SıraNo >= -1)
                {
                    if (bulunan._Elemanları == null || bulunan._Elemanları.Length == 0) return;

                    List<Eleman_> l = bulunan._Elemanları.ToList();
                    l.Sort(new _Sıralayıcı_Eleman_(SıraNo, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, Tersten));
                    bulunan._Elemanları = l.ToArray();
                }
                else return;

                bulunan._Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
            }
            public void Sırala(string ElemanAdıDizisi, List<string> ElemanAdıSıralaması)
            {
                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null || bulunan._Elemanları == null || bulunan._Elemanları.Length == 0 || ElemanAdıSıralaması == null || ElemanAdıSıralaması.Count == 0) return;

                List<Eleman_> YeniElamanListesi = new List<Eleman_>();
                List<Eleman_> EskiElamanListesi = bulunan._Elemanları.ToList();

                //Verilen sıralamaya göre yeni listeyi oluştur
                foreach (string SıradakiElamanAdı in ElemanAdıSıralaması)
                {
                    if (string.IsNullOrEmpty(SıradakiElamanAdı)) continue;

                    Eleman_ bulunan_alt_eleman = EskiElamanListesi.FirstOrDefault(x => x._Adı == SıradakiElamanAdı);
                    if (bulunan_alt_eleman == null) bulunan_alt_eleman = new Eleman_(SıradakiElamanAdı, bulunan._Depo); //Sıralamada geçen fakat mevcut olmayan elemanı oluştur, istenen konuma yerleştir
                    else EskiElamanListesi.Remove(bulunan_alt_eleman);

                    YeniElamanListesi.Add(bulunan_alt_eleman);
                }

                //Yeni sıralamada olmayan elemanları listenin sonuna ekle
                foreach (Eleman_ bulunan_alt_eleman in EskiElamanListesi)
                {
                    YeniElamanListesi.Add(bulunan_alt_eleman);
                }

                bulunan._Elemanları = YeniElamanListesi.ToArray();
                bulunan._Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
            }
            public List<string> Listele(string ElemanAdıDizisi)
            {
                List<string> ElemanAdıListesi = new List<string>();

                Eleman_ bulunan = Bul_Getir(ElemanAdıDizisi);
                if (bulunan == null || bulunan._Elemanları == null || bulunan._Elemanları.Length == 0) return ElemanAdıListesi;

                foreach (Eleman_ alt_eleman in bulunan._Elemanları)
                {
                    ElemanAdıListesi.Add(alt_eleman._Adı);
                }

                return ElemanAdıListesi;
            }
            #endregion
        }
        List<Eleman_> YazıOlarakElemanlar_Ayıkla(string YazıOlarakElemanlar, bool DoğrulamaKoduOlmalı)
        {
            List<Eleman_> okunanlar = new List<Eleman_>();

            if (!string.IsNullOrWhiteSpace(YazıOlarakElemanlar))
            {
                YazıOlarakElemanlar = YazıOlarakElemanlar.Replace(Depo_Ayraçlar.Eleman2.ToString(), null);
                
                string[] dizi = YazıOlarakElemanlar.Split(Depo_Ayraçlar.Eleman);
                if (dizi != null && dizi.Length > 0)
                {
                    List<string> Elemanlar = dizi.ToList();
                    bool EskiTipte = false;

                    if (YazıOlarakElemanlar.StartsWith(Depo_Ayraçlar.Seviye_DoğrulamaKodu))
                    {
                        string[] dizi_başlık = dizi[0].Split(Depo_Ayraçlar.AdıVeİçerik);
                        if (dizi_başlık == null || dizi_başlık.Length != 2) throw new Exception("Doğruluk kontrolü içeriği uygun değil " + dizi[0]);

                        //başlığı sil
                        YazıOlarakElemanlar = YazıOlarakElemanlar.Substring(dizi[0].Length + 1 /*Depo_Ayraçlar.AdıVeİçerik*/);

                        string doko_hesaplanan = ArgeMup.HazirKod.DoğrulamaKodu.Üret.Yazıdan(YazıOlarakElemanlar);
                        if (doko_hesaplanan != dizi_başlık[1]) throw new Exception("Doğrulama kodu uyumsuz");

                        //başlığı sil
                        Elemanlar.RemoveAt(0);
                    }
                    else if (DoğrulamaKoduOlmalı) throw new Exception("Doğrulama kodu yok");
                    else EskiTipte = !YazıOlarakElemanlar.StartsWith("0" + Depo_Ayraçlar.AdıVeİçerik);

                    while (Elemanlar.Count > 0)
                    {
                        Eleman_ üretilen;
                        if (EskiTipte) üretilen = new Eleman_(ref Elemanlar, 0, this);
                        else üretilen = new Eleman_(Elemanlar, -1, this);

                        if (!üretilen.İçiBoşOlduğuİçinSilinecek) okunanlar.Add(üretilen);
                    }
                }
            }

            return okunanlar;
        }

        public Depo_(string YazıOlarakElemanlar = null, Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null, bool DoğrulamaKoduOlmalı = true)
        {
            Ekle(YazıOlarakElemanlar, DoğrulamaKoduOlmalı);

            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;

            this.GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti;
        }
        public override string ToString()
        {
            int adet = Elemanları.Length;
            string içerik = (_EnAzBir_ElemanAdıVeyaİçeriği_Değişti ? "Değişiklik yapıldı, " : null) + "Elemanları [" + adet + "]";

            if (adet > 0)
            {
                if (adet > 5) adet = 5;

                içerik += " {";

                for (int i = 0; i < adet; i++) içerik += (_Elemanları[i] == null ? "boş" : _Elemanları[i]._Adı) + ", ";

                içerik = içerik.TrimEnd(',', ' ') + "}";
            }

            return içerik;
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
                bulunan = new Eleman_(ElemanAdıDizisi, this);
                Array.Resize(ref _Elemanları, _Elemanları.Length + 1);
                _Elemanları[_Elemanları.Length - 1] = bulunan;
                bulunan.Yaz(geriyekalan, İçeriği, SıraNo);
            }
        }
        public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0)
        {
            Bul(ElemanAdıDizisi, true, false).Yaz(null, Sayı, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, decimal HassasSayı, int SıraNo = 0)
        {
            Bul(ElemanAdıDizisi, true, false).Yaz(null, HassasSayı, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, int TamSayı, int SıraNo = 0)
        {
            Bul(ElemanAdıDizisi, true, false).Yaz(null, TamSayı, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int SıraNo = 0)
        {
            Bul(ElemanAdıDizisi, true, false).Yaz(null, BaytDizisi, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, DateTime TarihSaat, int SıraNo = 0)
        {
            Bul(ElemanAdıDizisi, true, false).Yaz(null, TarihSaat, SıraNo);
        }
        public void Yaz(string ElemanAdıDizisi, bool Bit, int SıraNo = 0)
        {
            Bul(ElemanAdıDizisi, true, false).Yaz(null, Bit, SıraNo);
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
        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_Sayı(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public decimal Oku_HassasSayı(string ElemanAdıDizisi, decimal BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_HassasSayı(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
        }
        public int Oku_TamSayı(string ElemanAdıDizisi, int BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan == null) return BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği;

            return bulunan.Oku_TamSayı(null, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
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

        public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false, bool BağımsızKopyaOluştur = false)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi)) throw new Exception("ElemanDizisi boş olamaz");

            //alttaki elemanlardan bahsediyor
            IDepo_Eleman bulunan = null;
            if (_Elemanları == null) goto OluşturmayıKontrolEt;

            string elm_adı = ElemanAdıDizisi;
            string geriyekalan = Depo_Ayraçlar.ElemanAdıDizisi_Ayıkla(ref elm_adı);

            bulunan = Array.Find(_Elemanları, x => x.Adı == elm_adı);
            if (bulunan == null) goto OluşturmayıKontrolEt;

            bulunan = bulunan.Bul(geriyekalan, YoksaOluştur, BağımsızKopyaOluştur);
            return bulunan;

        OluşturmayıKontrolEt:
            if (YoksaOluştur)
            {
                Yaz(ElemanAdıDizisi, (string)null);
                bulunan = Bul(ElemanAdıDizisi, false, BağımsızKopyaOluştur);
            }

            return bulunan;
        }
        public string YazıyaDönüştür(string ElemanAdıDizisi = null, bool SadeceElemanları = false, bool DoğrulamaKoduEkle = true)
        {
            if (string.IsNullOrEmpty(ElemanAdıDizisi))
            {
                if (_Elemanları == null || _Elemanları.Length == 0) return "";

                string çıktı = "";
                foreach (Eleman_ Eleman in _Elemanları)
                {
                    string oluşturulan = Eleman.YazıyaDönüştür(null, false, false);
                    if (!string.IsNullOrEmpty(oluşturulan)) çıktı += oluşturulan;
                }

                if (DoğrulamaKoduEkle && !string.IsNullOrEmpty(çıktı))
                {
                    çıktı = Depo_Ayraçlar.Seviye_DoğrulamaKodu + ArgeMup.HazirKod.DoğrulamaKodu.Üret.Yazıdan(çıktı) + Depo_Ayraçlar.Eleman + çıktı;
                }

                return çıktı;
            }
            else
            {
                IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
                if (bulunan == null) return "";

                return bulunan.YazıyaDönüştür(null, SadeceElemanları, DoğrulamaKoduEkle);
            }
        }
        public void Ekle(string YazıOlarakElemanlar, bool DoğrulamaKoduOlmalı = true)
        {
            List<Eleman_> okunanlar = YazıOlarakElemanlar_Ayıkla(YazıOlarakElemanlar, DoğrulamaKoduOlmalı);

            if (okunanlar.Count > 0)
            {
                if (_Elemanları == null) _Elemanları = new Eleman_[okunanlar.Count];
                else Array.Resize(ref _Elemanları, _Elemanları.Length + okunanlar.Count);

                Array.Copy(okunanlar.ToArray(), 0, Elemanları, _Elemanları.Length - okunanlar.Count, okunanlar.Count);

                EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;
            }
        }
        public void Sil(string ElemanAdıDizisi, bool Sadeceİçeriğini = false, bool SadeceElemanlarını = false)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan != null) bulunan.Sil(null, Sadeceİçeriğini, SadeceElemanlarını);
        }
        public void Sırala(string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten = false)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan != null) bulunan.Sırala(null, SıraNo, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, Tersten);
        }
        public void Sırala(string ElemanAdıDizisi, List<string> ElemanAdıSıralaması)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan != null) bulunan.Sırala(null, ElemanAdıSıralaması);
        }
        public List<string> Listele(string ElemanAdıDizisi)
        {
            IDepo_Eleman bulunan = Bul(ElemanAdıDizisi);
            if (bulunan != null) return bulunan.Listele(null);

            return new List<string>();
        }
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
        /// <summary>
        /// Elemanı
        /// </summary>
        public IDepo_Eleman this[string ElemanAdıDizisi]
        {
            get
            {
                return Bul(ElemanAdıDizisi, true, false);
            }
        }
        /// <summary>
        /// Tüm Elemanları
        /// </summary>
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
        /// <summary>
        /// Elemanın İçeriği 
        /// </summary>
        public string this[string ElemanAdıDizisi, int SıraNo]
        {
            get
            {
                return Oku(ElemanAdıDizisi, null, SıraNo);
            }
            set
            {
                Yaz(ElemanAdıDizisi, value, SıraNo);
            }
        }
        /// <summary>
        /// Elemanın İçeriğinin Türü ile Birlikte Değerlendirilmesi
        /// Okuma : BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği türünde bilgi üretir
        /// Yazma : Yeni değer türünden yazıya dönüştürerek ilerler
        /// </summary>
        public object this[string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği]
        {
            get
            {
                if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is string) return Oku(ElemanAdıDizisi, (string)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is double) return Oku_Sayı(ElemanAdıDizisi, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is decimal) return Oku_HassasSayı(ElemanAdıDizisi, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is int) return Oku_TamSayı(ElemanAdıDizisi, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is bool) return Oku_Bit(ElemanAdıDizisi, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is DateTime) return Oku_TarihSaat(ElemanAdıDizisi, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is byte[]) return Oku_BaytDizisi(ElemanAdıDizisi, (byte[])BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                else throw new Exception("Desteklenmeyen tür " + BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği.GetType().FullName);
            }
            set
            {
                if (value is string) Yaz(ElemanAdıDizisi, (string)value, SıraNo);
                else if (value is double) Yaz(ElemanAdıDizisi, (double)value, SıraNo);
                else if (value is decimal) Yaz(ElemanAdıDizisi, (decimal)value, SıraNo);
                else if (value is int) Yaz(ElemanAdıDizisi, (int)value, SıraNo);
                else if (value is bool) Yaz(ElemanAdıDizisi, (bool)value, SıraNo);
                else if (value is DateTime) Yaz(ElemanAdıDizisi, (DateTime)value, SıraNo);
                else if (value is byte[]) Yaz(ElemanAdıDizisi, (byte[])value, SıraNo);
                else throw new Exception("Desteklenmeyen tür " + value.GetType().FullName);
            }
        }

        Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null;
        ArgeMup.HazirKod.Depo_ Depo;
        System.Threading.Mutex Kilit;
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
            public override string ToString()
            {
                return AsılEleman.ToString();
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
            public string this[int SıraNo] //içeriği
            {
                get
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    string okunan = AsılEleman[SıraNo];

                    Depo.Kilit.ReleaseMutex();

                    return okunan;
                }
                set
                {
                    if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                    AsılEleman[SıraNo] = value;
                    Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                    Depo.Kilit.ReleaseMutex();
                }
            }
            public string[] İçeriği //Tüm İçeriği
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
            public IDepo_Eleman this[string ElemanAdıDizisi] //Elemanı
            {
                get
                {
                    return Bul(ElemanAdıDizisi, true, false);
                }
            }
            public IDepo_Eleman[] Elemanları //Tüm Elemanları
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
            public string this[string ElemanAdıDizisi, int SıraNo] //Elemanın İçeriği 
            {
                get
                {
                    return Oku(ElemanAdıDizisi, null, SıraNo);
                }
                set
                {
                    Yaz(ElemanAdıDizisi, value, SıraNo);
                }
            }
            public object this[string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği] //Elemanın İçeriğinin Türü ile Birlikte Değerlendirilmesi
            {
                get
                {
                    if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is string) return Oku(ElemanAdıDizisi, (string)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is double) return Oku_Sayı(ElemanAdıDizisi, (double)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is decimal) return Oku_HassasSayı(ElemanAdıDizisi, (decimal)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is int) return Oku_TamSayı(ElemanAdıDizisi, (int)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is bool) return Oku_Bit(ElemanAdıDizisi, (bool)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is DateTime) return Oku_TarihSaat(ElemanAdıDizisi, (DateTime)BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else if (BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği is byte[]) return Oku_BaytDizisi(ElemanAdıDizisi, (byte[])BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);
                    else throw new Exception("Desteklenmeyen tür " + BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği.GetType().FullName);
                }
                set
                {
                    if (value is string) Yaz(ElemanAdıDizisi, (string)value, SıraNo);
                    else if (value is double) Yaz(ElemanAdıDizisi, (double)value, SıraNo);
                    else if (value is decimal) Yaz(ElemanAdıDizisi, (decimal)value, SıraNo);
                    else if (value is int) Yaz(ElemanAdıDizisi, (int)value, SıraNo);
                    else if (value is bool) Yaz(ElemanAdıDizisi, (bool)value, SıraNo);
                    else if (value is DateTime) Yaz(ElemanAdıDizisi, (DateTime)value, SıraNo);
                    else if (value is byte[]) Yaz(ElemanAdıDizisi, (byte[])value, SıraNo);
                    else throw new Exception("Desteklenmeyen tür " + value.GetType().FullName);
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

            public void Yaz(string ElemanAdıDizisi, string İçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, İçeriği, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, Sayı, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, decimal HassasSayı, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, HassasSayı, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, int TamSayı, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, TamSayı, SıraNo);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Yaz(ElemanAdıDizisi, BaytDizisi, SıraNo);
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

            public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                string okunan = AsılEleman.Oku(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                double okunan = AsılEleman.Oku_Sayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public decimal Oku_HassasSayı(string ElemanAdıDizisi, decimal BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                decimal okunan = AsılEleman.Oku_HassasSayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public int Oku_TamSayı(string ElemanAdıDizisi, int BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, int SıraNo)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                int okunan = AsılEleman.Oku_TamSayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

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

            public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur, bool BağımsızKopyaOluştur)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                IDepo_Eleman okunan = AsılEleman.Bul(ElemanAdıDizisi, YoksaOluştur, BağımsızKopyaOluştur);
                if (okunan != null) okunan = new Depo_Kilitli_Eleman_(okunan, Depo);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public string YazıyaDönüştür(string ElemanAdıDizisi, bool SadeceElemanları, bool DoğrulamaKoduEkle)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                string okunan = AsılEleman.YazıyaDönüştür(ElemanAdıDizisi, SadeceElemanları, DoğrulamaKoduEkle);

                Depo.Kilit.ReleaseMutex();

                return okunan;
            }
            public void Ekle(string ElemanAdıDizisi, string YazıOlarakElemanlar, bool DoğrulamaKoduOlmalı)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Ekle(ElemanAdıDizisi, YazıOlarakElemanlar, DoğrulamaKoduOlmalı);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Sil(string ElemanAdıDizisi, bool Sadeceİçeriğini, bool SadeceElemanlarını)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Sil(ElemanAdıDizisi, Sadeceİçeriğini, SadeceElemanlarını);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Sırala(string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Sırala(ElemanAdıDizisi, SıraNo, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, Tersten);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public void Sırala(string ElemanAdıDizisi, List<string> ElemanAdıSıralaması)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                AsılEleman.Sırala(ElemanAdıDizisi, ElemanAdıSıralaması);
                Depo.EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

                Depo.Kilit.ReleaseMutex();
            }
            public List<string> Listele(string ElemanAdıDizisi)
            {
                if (!Depo.Kilit.WaitOne(Depo.Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

                List<string> ElemanAdıListesi = AsılEleman.Listele(ElemanAdıDizisi);

                Depo.Kilit.ReleaseMutex();

                return ElemanAdıListesi;
            }
            #endregion
        }

        public Depo_(string YazıOlarakElemanlar = null, Action<Depo_> GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = null, bool DoğrulamaKoduOlmalı = true)
        {
            Depo = new ArgeMup.HazirKod.Depo_(YazıOlarakElemanlar, null, DoğrulamaKoduOlmalı);
            Kilit = new System.Threading.Mutex();

            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = false;
            this.GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti = GeriBildirimİşlemi_EnAzBir_ElemanAdıVeyaİçeriği_Değişti;
        }
        public override string ToString()
        {
            return Depo.ToString();
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
        public void Yaz(string ElemanAdıDizisi, double Sayı, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, Sayı, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Yaz(string ElemanAdıDizisi, decimal HassasSayı, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, HassasSayı, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Yaz(string ElemanAdıDizisi, int TamSayı, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, TamSayı, SıraNo);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Yaz(string ElemanAdıDizisi, byte[] BaytDizisi, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Yaz(ElemanAdıDizisi, BaytDizisi, SıraNo);
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

        public string Oku(string ElemanAdıDizisi, string BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = null, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            string okunan = Depo.Oku(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public double Oku_Sayı(string ElemanAdıDizisi, double BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            double okunan = Depo.Oku_Sayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public decimal Oku_HassasSayı(string ElemanAdıDizisi, decimal BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            decimal okunan = Depo.Oku_HassasSayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public int Oku_TamSayı(string ElemanAdıDizisi, int BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği = default, int SıraNo = 0)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            int okunan = Depo.Oku_TamSayı(ElemanAdıDizisi, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, SıraNo);

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

        public IDepo_Eleman Bul(string ElemanAdıDizisi, bool YoksaOluştur = false, bool BağımsızKopyaOluştur = false)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            IDepo_Eleman okunan = Depo.Bul(ElemanAdıDizisi, YoksaOluştur, BağımsızKopyaOluştur);
            if (okunan != null) okunan = new Depo_Kilitli_Eleman_(okunan, this);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public string YazıyaDönüştür(string ElemanAdıDizisi = null, bool SadeceElemanları = false, bool DoğrulamaKoduEkle = true)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            string okunan = Depo.YazıyaDönüştür(ElemanAdıDizisi, SadeceElemanları, DoğrulamaKoduEkle);

            Kilit.ReleaseMutex();

            return okunan;
        }
        public void Ekle(string YazıOlarakElemanlar, bool DoğrulamaKoduOlmalı = true)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Ekle(YazıOlarakElemanlar, DoğrulamaKoduOlmalı);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Sil(string ElemanAdıDizisi, bool Sadeceİçeriğini = false, bool SadeceElemanlarını = false)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Sil(ElemanAdıDizisi, Sadeceİçeriğini, SadeceElemanlarını);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        /// <summary>
        /// ElemanAdıDizisi ile seçilen elemana ait içerik veya alt elemanları sıralamak için kullanılabilir
        /// EnAzBir_ElemanAdıVeyaİçeriği_Değişti bayrağını mutlaka kurar
        /// </summary>
        /// <param name="ElemanAdıDizisi">İstenilen elemanın adı</param>
        /// <param name="SıraNo">-2 : Elemanın İÇERİĞİNİ istenilen türe çevirip sıralar \n -1 : Elemanın ALT_ELEMANLARINI, ALT_ELEMANIN_ADI bilgisini istenilen türe çevirip sıralar \n 0 ... : Elemanın ALT_ELEMANLARINI, ALT_ELEMANIN_BELİRTİLEN_SIRANO içeriğini istenilen türe çevirip sıralar</param>
        /// <param name="BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği">Karşılaştırma yapılacak türün tayin edilmesi ve içeriğin olmaması durumunda kullanılır</param>
        /// <param name="Tersten">İşaretlenirse -> Yazı : Z den A ya \n Sayı Büyükten küçüğe \n Tarih Saat : Yeniden Eskiye doğru sıralanır</param>
        public void Sırala(string ElemanAdıDizisi, int SıraNo, object BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, bool Tersten = false)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Sırala(ElemanAdıDizisi, SıraNo, BulunamamasıVeyaBoşOlmasıDurumundakiİçeriği, Tersten);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public void Sırala(string ElemanAdıDizisi, List<string> ElemanAdıSıralaması)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            Depo.Sırala(ElemanAdıDizisi, ElemanAdıSıralaması);
            EnAzBir_ElemanAdıVeyaİçeriği_Değişti = true;

            Kilit.ReleaseMutex();
        }
        public List<string> Listele(string ElemanAdıDizisi)
        {
            if (!Kilit.WaitOne(Kilit_Devralma_ZamanAşımı_msn)) throw new Exception("Kilit devralınamadı");

            List<string> ElemanAdıListesi = Depo.Listele(ElemanAdıDizisi);

            Kilit.ReleaseMutex();

            return ElemanAdıListesi;
        }
    }
}