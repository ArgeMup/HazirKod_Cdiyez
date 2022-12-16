// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_Görsel
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Windows.Forms;

    namespace ArgeMup.HazirKod
    {
        public class UygulamaBostaBekliyor_ : IDisposable
        {
            public const string Sürüm = "V1.5";

            #region Değişkenler
            public Durum_ Durum = Durum_.Boşta;
            public enum Durum_ { Boşta, SüreninGeçmesiniBekliyor, GörüntüSeçiliyor, GörüntüBaşlatılıyor, GörüntüOynatılıyor };
            public List<string> UzaktakiOynatılacakVideolarListesi;
            public const string AğdakiReklamVideolarınınYolu = ""; //Ortak Klasör Yolu Girilmeli

            string YerelKlasör;
            int ZamanAşımı;
            bool DurdurKapat, DosyalarıÖnceKopyalaSonraOynat;
            BackgroundWorker ArkaPlan;
            Process Oynatıcı;
            Ayarlar_Xml_ Ayarlar;
            KlavyeFareGozlemcisi_ Gözlemci; 
            #endregion

            public UygulamaBostaBekliyor_(string YerelDepolamaKonumu = "", int Süre_Sn = 5 * 60, List<string> VideolarınBulunduklarıKlasörler = null, bool AynıGörüntüyüSürekliTekrarla = false, int SesSeviyesiYüzdesi = 0, bool ÖnceKopyalaSonraOynat = true)
            {
                UzaktakiOynatılacakVideolarListesi = new List<string>();
                if (VideolarınBulunduklarıKlasörler != null)
                {
                    foreach (var dosya in VideolarınBulunduklarıKlasörler)
                    {
                        string yol = Path.GetDirectoryName(dosya+"\\");
                        if (!Directory.Exists(yol)) continue;
                        UzaktakiOynatılacakVideolarListesi.AddRange(Directory.GetFiles(yol));
                    }
                }
                else if(Directory.Exists(AğdakiReklamVideolarınınYolu)) UzaktakiOynatılacakVideolarListesi = Directory.GetFiles(AğdakiReklamVideolarınınYolu).ToList();

                if (YerelDepolamaKonumu == "") YerelKlasör = Kendi.Klasörü + "\\UygulamaBoştaBekliyor\\";
                else if (YerelDepolamaKonumu.Contains("\\")) YerelKlasör = YerelDepolamaKonumu;
                else YerelKlasör = Kendi.Klasörü + "\\" + YerelDepolamaKonumu + "\\UygulamaBoştaBekliyor\\";

                bool var;
                Ayarlar = new Ayarlar_Xml_(out var, "", YerelKlasör + "UygulamaBostaBekliyor.Ayarlar");
                
                if (UzaktakiOynatılacakVideolarListesi.Count == 0)
                {
                    if (Directory.Exists(YerelKlasör)) UzaktakiOynatılacakVideolarListesi = Directory.GetFiles(YerelKlasör).ToList();
                    UzaktakiOynatılacakVideolarListesi.Remove(Ayarlar.AyarlarDosyasıYolunuAl());
                    if (UzaktakiOynatılacakVideolarListesi.Count == 0) return;
                }
                else
                {
                    //Fazla Dosya Kontrolü
                    string[] Dizi = Directory.GetFiles(YerelKlasör);
                    foreach (var nesne in Dizi)
                    {
                        string DosyaAdı = Path.GetFileName(nesne);
                        if (DosyaAdı != Path.GetFileName(Ayarlar.AyarlarDosyasıYolunuAl()))
                        {
                            var = false;
                            foreach (var uzak in UzaktakiOynatılacakVideolarListesi)
                            {
                                if (uzak.Contains(DosyaAdı)) { var = true; break; }
                            }

                            if (!var) File.Delete(nesne);
                        }
                    }
                }

                if (Süre_Sn < 5) ZamanAşımı = 5;
                else ZamanAşımı = Süre_Sn;
                
                Gözlemci = new KlavyeFareGozlemcisi_();
                
                if (!Directory.Exists(YerelKlasör)) Directory.CreateDirectory(YerelKlasör);

                KayitDefteri_ KayıtDefteri = new KayitDefteri_("HKEY_CURRENT_USER\\Software\\Microsoft\\MediaPlayer");
                if (UzaktakiOynatılacakVideolarListesi.Count == 1) AynıGörüntüyüSürekliTekrarla = true;
                Süre_Sn = 0; if (AynıGörüntüyüSürekliTekrarla) Süre_Sn = 1;
                KayıtDefteri.Yaz("Preferences", "ModeLoop", Süre_Sn);
                KayıtDefteri.Yaz("Preferences", "Volume", SesSeviyesiYüzdesi);

                DosyalarıÖnceKopyalaSonraOynat = ÖnceKopyalaSonraOynat;
                
                DurdurKapat = false;
                Durum = Durum_.SüreninGeçmesiniBekliyor;
                ArkaPlan = new BackgroundWorker();
                ArkaPlan.DoWork += ArkaPlandaÇalışanUygulama;
                ArkaPlan.RunWorkerAsync();
            }
            public int BoştaGeçenSaniye()
            {
                return (int)(DateTime.Now.Subtract(Gözlemci.SonKlavyeFareOlayıAnı)).TotalSeconds;
            }

            void ArkaPlandaÇalışanUygulama(object sender, DoWorkEventArgs e)
            {
                FileInfo Yerel = null;
                FileInfo Uzak;
                int önceki_TotalProcessorTime_Tick = 0; 
                TimeSpan önceki_TotalProcessorTime = new TimeSpan(0);
                int KapatÇık = 0;
                int Bekleme = 1000;

                while (!DurdurKapat)
                {
                    try
                    {
                        switch (Durum)
                        {
                            case Durum_.SüreninGeçmesiniBekliyor:
                            case Durum_.Boşta:
                            default:
                                Bekleme = 5000;
                                if (BoştaGeçenSaniye() > ZamanAşımı) { Durum = Durum_.GörüntüSeçiliyor; Bekleme = 1; }
                                break;

                            case Durum_.GörüntüSeçiliyor:
                                if (UzaktakiOynatılacakVideolarListesi.Count == 0) return;
                                foreach (var görüntü in UzaktakiOynatılacakVideolarListesi)
                                {
                                    Yerel = new FileInfo(YerelKlasör + Path.GetFileName(görüntü));
                                    Uzak = new FileInfo(görüntü);

                                    if (!DosyalarıÖnceKopyalaSonraOynat) Yerel = Uzak;

                                    if (Ayarlar.Oku(Yerel.FullName) != "Kullanıldı")
                                    {
                                        if (DosyalarıÖnceKopyalaSonraOynat)
                                        {
                                            bool YenidenKopyala = false;
                                            if (File.Exists(Yerel.FullName))
                                            {
                                                if (Yerel.LastWriteTime != Uzak.LastWriteTime) YenidenKopyala = true;
                                                else if (Yerel.Length != Uzak.Length) YenidenKopyala = true;
                                            }
                                            else YenidenKopyala = true;

                                            if (YenidenKopyala) File.Copy(Uzak.FullName, Yerel.FullName, true);
                                        }

                                        Durum = Durum_.GörüntüBaşlatılıyor;
                                        break;
                                    }
                                }

                                if (Durum != Durum_.GörüntüBaşlatılıyor)
                                {
                                    List<Depo_Xml.Biri> Liste = Ayarlar.Listele();
                                    foreach (var nesne in Liste) Ayarlar.Sil(nesne.Adı);
                                }
                                break;

                            case Durum_.GörüntüBaşlatılıyor:
                                Oynatıcı = new Process();
                                Oynatıcı.StartInfo.FileName = "mplayer2.exe";
                                Oynatıcı.StartInfo.Arguments = "/play /fullscreen \"" + Yerel.FullName + "\"";
                                Oynatıcı.Start();

                                Bekleme = 500;
                                KapatÇık = 0;
                                önceki_TotalProcessorTime = Oynatıcı.TotalProcessorTime;
                                önceki_TotalProcessorTime_Tick = Environment.TickCount;
                                Ayarlar.Yaz(Yerel.FullName, "Kullanıldı");
                                Durum = Durum_.GörüntüOynatılıyor;
                                break;

                            case Durum_.GörüntüOynatılıyor:
                                double yüzde = Oynatıcı.TotalProcessorTime.Subtract(önceki_TotalProcessorTime).TotalMilliseconds / (Environment.TickCount - önceki_TotalProcessorTime_Tick) / Environment.ProcessorCount * 100;
                                önceki_TotalProcessorTime = Oynatıcı.TotalProcessorTime;
                                önceki_TotalProcessorTime_Tick = Environment.TickCount;

                                if (BoştaGeçenSaniye() < ZamanAşımı) KapatÇık = 100;
                                else if (Oynatıcı.HasExited || !Oynatıcı.Responding) KapatÇık = 100;
                                else if (yüzde < 1) { if (++KapatÇık > 2) KapatÇık = 100; }
                                else KapatÇık = 0;

                                if (KapatÇık == 100)
                                {
                                    Oynatıcı.Kill();
                                    Oynatıcı.Dispose();
                                    Oynatıcı = null;
                                    Durum = Durum_.SüreninGeçmesiniBekliyor;
                                }
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        if (Oynatıcı != null) { Oynatıcı.Dispose(); Oynatıcı = null; }
                        Durum = Durum_.SüreninGeçmesiniBekliyor;
                    }

                    Thread.Sleep(Bekleme);
                }
            }
            
            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).

                        DurdurKapat = true;

                        if (Gözlemci != null) { Gözlemci.Dispose(); Gözlemci = null; }

                        if (ArkaPlan != null) { ArkaPlan.Dispose(); ArkaPlan = null; }

                        if (UzaktakiOynatılacakVideolarListesi != null) { UzaktakiOynatılacakVideolarListesi.Clear(); UzaktakiOynatılacakVideolarListesi = null; } 
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    if (Oynatıcı != null) { Oynatıcı.Kill(); Oynatıcı.Dispose(); Oynatıcı = null; }

                    //disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            ~UygulamaBostaBekliyor_()
            {
                // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
                Dispose(false);
            }

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
#endif