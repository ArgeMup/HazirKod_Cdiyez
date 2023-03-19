// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ArgeMup.HazirKod.ArkaPlan;
using ArgeMup.HazirKod.Dönüştürme;

namespace ArgeMup.HazirKod
{
    public class DoğrulamaKodu 
    {
        public const string Sürüm = "V1.3";
        public const string DoğrulamaKodu_DosyaAdı = "ArgeMup.HazirKod_Cdiyez.DogrulamaKodu";

        public class Üret
        {
            static public string Dosyadan(string DosyaYolu, int DosyaKullanılıyorİseZamanAşımı_msn = 10000)
            {
                if (string.IsNullOrEmpty(DosyaYolu) || !File.Exists(DosyaYolu)) throw new Exception("Dosya bulunamadı " + DosyaYolu);

                int za = Environment.TickCount + DosyaKullanılıyorİseZamanAşımı_msn;
                while (za > Environment.TickCount)
                {
                    try
                    {
                        byte[] çıktı = null;

                        using (var SHA512 = System.Security.Cryptography.SHA512.Create())
                        {
                            using (var stream = File.OpenRead(DosyaYolu))
                            {
                                çıktı = SHA512.ComputeHash(stream);
                            }
                        }

                        return D_HexYazı.BaytDizisinden(çıktı);
                    }
                    catch (Exception) 
                    { 
                        System.Threading.Thread.Sleep( 100 );  
                    }
                }

                throw new Exception("Dosya açılamadı " + DosyaYolu);
            }
            static public string Yazıdan(string Yazı)
            {
                return D_HexYazı.BaytDizisinden(BaytDizisinden(D_Yazı.BaytDizisine(Yazı)));
            }
            static public byte[] BaytDizisinden(byte[] Dizi, int BaşlangıçKonumu = 0, int Adet = -1)
            {
                if (Adet == -1) Adet = Dizi.Length - BaşlangıçKonumu;

                byte[] çıktı = null;
                using (var SHA512 = System.Security.Cryptography.SHA512.Create())
                {
                    çıktı = SHA512.ComputeHash(Dizi, BaşlangıçKonumu, Adet);
                }

                return çıktı;
            }
            static public string Akıştan(Stream Akış)
            {
                byte[] çıktı = null;
                using (var SHA512 = System.Security.Cryptography.SHA512.Create())
                {
                    çıktı = SHA512.ComputeHash(Akış);
                }

                return D_HexYazı.BaytDizisinden(çıktı);
            }
            static public string Klasörden(string KlasörYolu, bool VeYaz, SearchOption Kapsamı = SearchOption.AllDirectories, string SihirliKelime = "ArGeMuP")
            {
                Salkım_ Salkım = new Salkım_
                {
                    GörselÇıktı = "",
                    Kod = "",
                    SihirliKelime = SihirliKelime
                };

                KlasörYolu = D_DosyaKlasörAdı.Düzelt(KlasörYolu, false);

                string[] liste_d = Directory.GetFiles(KlasörYolu, "*.*", Kapsamı).Where((biri, içerik) => !biri.EndsWith(DoğrulamaKodu_DosyaAdı)).ToArray();
                string[] liste_k = Directory.GetDirectories(KlasörYolu, "*", Kapsamı);

                Salkım.Ekle("- ArGeMup Klasör Dosya Doğrulama Aracı");
                Salkım.Ekle("- " + (Kapsamı == SearchOption.AllDirectories ? "Tüm alt klasörleriyle birlikte" : "Sadece üst klasör"));
                Salkım.Ekle("- Dosya Sayısı : " + liste_d.Length);
                Salkım.Ekle("- Klasör Sayısı : " + liste_k.Length);
                Salkım.Ekle("- https://github.com/ArgeMup/HazirKod_Cdiyez");
                Salkım.Ekle("A " + Kendi.Adı + " / V" + Kendi.Sürümü_Dosya);
                Salkım.Ekle("A " + D_TarihSaat.Yazıya(DateTime.Now));

                foreach (string b in liste_d)
                {
                    Salkım.Ekle("D |" + Dosyadan(b) + "|" + b.Substring(KlasörYolu.Length + 1));
                }

                foreach (string b in liste_k)
                {
                    Salkım.Ekle("K |" + b.Substring(KlasörYolu.Length + 1));
                }

                Salkım.Ekle("- " + Yazıdan(Salkım.Kod));

                if (VeYaz) File.WriteAllText(KlasörYolu + "\\" + DoğrulamaKodu_DosyaAdı, Salkım.GörselÇıktı);

                return Salkım.Kod;
            }
        }

        public class KontrolEt
        {
            public enum Durum_ { DoğrulamaDosyasıYok = -3, DoğrulamaDosyasıİçeriğiHatalı, Farklı, Aynı = 1, FazlaKlasörVeyaDosyaVar };

            static public Durum_ Klasör(string KlasörYolu, SearchOption Kapsamı = SearchOption.AllDirectories, string SihirliKelime = "ArGeMuP", int EşZamanlıİşlemSayısı = 5)
            {
                Salkım_ Salkım = new Salkım_
                {
                    GörselÇıktı = "",
                    Kod = "",
                    SihirliKelime = SihirliKelime
                };

                KlasörYolu = D_DosyaKlasörAdı.Düzelt(KlasörYolu, false) + "\\";

                if (!File.Exists(KlasörYolu + DoğrulamaKodu_DosyaAdı)) return Durum_.DoğrulamaDosyasıYok;
                string[] dosya_içeriği = File.ReadAllLines(KlasörYolu + DoğrulamaKodu_DosyaAdı);
                Dictionary<string, string> Dosyalar = new Dictionary<string, string>();
                List<string> Klasörler = new List<string>();

                try
                {
                    for (int i = 0; i < dosya_içeriği.Length - 1; i++)
                    {
                        Salkım.Ekle(dosya_içeriği[i]);
                        
                        if (dosya_içeriği[i].StartsWith("D |"))
                        {
                            string[] dizi = dosya_içeriği[i].Split('|');
                            if (dizi.Length == 3)
                            {
                                Dosyalar.Add(dizi[2], dizi[1]);
                            }
                        }

                        if (dosya_içeriği[i].StartsWith("K |"))
                        {
                            string[] dizi = dosya_içeriği[i].Split('|');
                            if (dizi.Length == 2)
                            {
                                Klasörler.Add(dizi[1]);
                            }
                        }
                    }

                    if (Üret.Yazıdan(Salkım.Kod) != dosya_içeriği[dosya_içeriği.Length - 1].Remove(0, 2)) return Durum_.DoğrulamaDosyasıİçeriğiHatalı;
                }
                catch (Exception) { return Durum_.DoğrulamaDosyasıİçeriğiHatalı; }

                long dsy_sayac_hata = 0;
                Öğütücü_<KeyValuePair<string, string>> ö = new Öğütücü_<KeyValuePair<string, string>>(İşl, EşZamanlıİşlemSayısı: EşZamanlıİşlemSayısı);
                foreach (var b in Dosyalar)
                {
                    ö.Ekle(b);
                }

                foreach (var b in Klasörler)
                {
                    string şimdiki = KlasörYolu + b;

                    if (!Directory.Exists(şimdiki)) return Durum_.Farklı;
                }

                string[] dsy_l = Directory.GetFiles(KlasörYolu, "*.*", Kapsamı);
                string[] kls_l = Directory.GetDirectories(KlasörYolu, "*", Kapsamı);
                
                while (!ö.TümüÖğütüldüMü()) System.Threading.Thread.Sleep(5);
                if (dsy_sayac_hata != 0) return Durum_.Farklı;

                if (Dosyalar.Count  != dsy_l.Length - 1 /*DoğrulamaKodu_DosyaAdı*/ ||
                    Klasörler.Count != kls_l.Length) return Durum_.FazlaKlasörVeyaDosyaVar;
                
                return Durum_.Aynı;

                void İşl(KeyValuePair<string, string> dsy, object o)
                {
                    string şimdiki = KlasörYolu + dsy.Key;

                    if (!File.Exists(şimdiki) ||
                        Üret.Dosyadan(şimdiki) != dsy.Value) System.Threading.Interlocked.Increment(ref dsy_sayac_hata);
                }
            }
        }

        class Salkım_
        {
            public string Kod;
            public string GörselÇıktı;
            public string SihirliKelime;

            public void Ekle(string Girdi)
            {
                if (Girdi[0] != 'A') Kod = Üret.Yazıdan(SihirliKelime + Kod + Girdi);

                GörselÇıktı += Girdi + Environment.NewLine;
            }
        }
    }
}