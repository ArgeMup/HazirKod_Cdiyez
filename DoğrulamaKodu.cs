// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using ArgeMup.HazirKod.ArkaPlan;
using ArgeMup.HazirKod.Dönüştürme;

namespace ArgeMup.HazirKod
{
    public class DoğrulamaKodu 
    {
        public const string Sürüm = "V1.2";
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
            static public string Klasörden(string KlasörYolu, bool VeYaz)
            {
                KlasörYolu = D_DosyaKlasörAdı.Düzelt(KlasörYolu, false);

                string[] liste_d = Directory.GetFiles(KlasörYolu, "*.*", SearchOption.AllDirectories).Where((biri, içerik) => !biri.EndsWith(DoğrulamaKodu_DosyaAdı)).ToArray();
                string[] liste_k = Directory.GetDirectories(KlasörYolu, "*", SearchOption.AllDirectories);

                string GörselÇıktı = "", kod = "";
                _Ekle_("- ArGeMup Klasör Dosya Doğrulama Aracı", ref kod, ref GörselÇıktı);
                _Ekle_("- Dosya Sayısı : " + liste_d.Length, ref kod, ref GörselÇıktı);
                _Ekle_("- Klasör Sayısı : " + liste_k.Length, ref kod, ref GörselÇıktı);
                _Ekle_("- " + Kendi.Adı() + " / V" + Kendi.Sürümü_Dosya(), ref kod, ref GörselÇıktı);
                _Ekle_("- " + D_TarihSaat.Yazıya(DateTime.Now), ref kod, ref GörselÇıktı);
                _Ekle_("- https://github.com/ArgeMup/HazirKod_Cdiyez", ref kod, ref GörselÇıktı);

                foreach (string b in liste_d)
                {
                    _Ekle_("D |" + Dosyadan(b) + "|" + b.Substring(KlasörYolu.Length + 1), ref kod, ref GörselÇıktı);
                }

                foreach (string b in liste_k)
                {
                    _Ekle_("K |" + b.Substring(KlasörYolu.Length + 1), ref kod, ref GörselÇıktı);
                }

                _Ekle_("- " + Yazıdan(kod), ref kod, ref GörselÇıktı);

                if (VeYaz) File.WriteAllText(KlasörYolu + "\\" + DoğrulamaKodu_DosyaAdı, GörselÇıktı);

                return GörselÇıktı;
            }
        }

        public class KontrolEt
        {
            public enum Durum_ { DoğrulamaDosyasıYok = -3, DoğrulamaDosyasıİçeriğiHatalı, Farklı, Aynı = 1, FazlaKlasörVeyaDosyaVar };

            static public Durum_ Klasör(string KlasörYolu, int EşZamanlıİşlemSayısı = 5)
            {
                KlasörYolu = KlasörYolu.TrimEnd('\\') + "\\";

                if (!File.Exists(KlasörYolu + DoğrulamaKodu_DosyaAdı)) return Durum_.DoğrulamaDosyasıYok;
                string[] dosya_içeriği = File.ReadAllLines(KlasörYolu + DoğrulamaKodu_DosyaAdı);
                Dictionary<string, string> Dosyalar = new Dictionary<string, string>();
                List<string> Klasörler = new List<string>();

                try
                {
                    string GörselÇıktı = "", kod = "";
                    for (int i = 0; i < dosya_içeriği.Length - 1; i++)
                    {
                        _Ekle_(dosya_içeriği[i], ref kod, ref GörselÇıktı);
                        
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

                    if (Üret.Yazıdan(kod) != dosya_içeriği[dosya_içeriği.Length - 1].Remove(0, 2)) return Durum_.DoğrulamaDosyasıİçeriğiHatalı;
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

                string[] dsy_l = Directory.GetFiles(KlasörYolu, "*.*", SearchOption.AllDirectories);
                string[] kls_l = Directory.GetDirectories(KlasörYolu, "*", SearchOption.AllDirectories);
                
                while (!ö.TümüÖğütüldüMü()) Thread.Sleep(5);
                if (dsy_sayac_hata != 0) return Durum_.Farklı;

                if (Dosyalar.Count  != dsy_l.Length - 1 /*DoğrulamaKodu_DosyaAdı*/ ||
                    Klasörler.Count != kls_l.Length) return Durum_.FazlaKlasörVeyaDosyaVar;
                
                return Durum_.Aynı;

                void İşl(KeyValuePair<string, string> dsy, object o)
                {
                    string şimdiki = KlasörYolu + dsy.Key;

                    if (!File.Exists(şimdiki) ||
                        Üret.Dosyadan(şimdiki) != dsy.Value) Interlocked.Increment(ref dsy_sayac_hata);
                }
            }
        }

        static void _Ekle_(string Girdi, ref string Kod, ref string GörselÇıktı)
        {
            Kod = Üret.Yazıdan(Kod + Üret.Yazıdan(Girdi));

            GörselÇıktı += Girdi + Environment.NewLine; 
        }
    }
}