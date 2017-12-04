// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

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
            Biriktir(ref Çıktı, D_Metin.BaytDizisine(Girdi));
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
            if (Oku(Akış, ZamanAşımı_msn, out Okunan)) return D_Metin.BaytDizisinden(Okunan);
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
            if (Yaz(Akış, ZamanAşımı_msn, D_Metin.BaytDizisine(Girdi))) return true;
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

                PipeSecurity ps = new PipeSecurity();
                ps.AddAccessRule(new PipeAccessRule("Everyone", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, System.Security.AccessControl.AccessControlType.Allow));
                Ortak.Alıcı[No] = new NamedPipeServerStream(Ortak.Adı, PipeDirection.InOut, Ortak.Alıcı_AzamiSayısı, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 1024, 1024, ps);
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
            if (Oku(Akış, ZamanAşımı_msn, out Okunan)) return D_Metin.BaytDizisinden(Okunan);
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
            if (Yaz(Akış, ZamanAşımı_msn, D_Metin.BaytDizisine(Girdi))) return true;
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
