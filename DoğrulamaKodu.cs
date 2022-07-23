// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Dönüştürme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArgeMup.HazirKod
{
    public class DoğrulamaKodu 
    {
        public const string Sürüm = "V1.1";
        public const string DoğrulamaKodu_DosyaAdı = "ArgeMup.HazirKod_Cdiyez.DogrulamaKodu";

        [Serializable]
        struct Dizin_
        {
            public Dictionary<string, byte[]> Dosyalar;
            public List<string> Klasörler;
        };

        public class Üret
        {
            static public byte[] Dosyadan(string DosyaYolu, int DosyaKullanılıyorİseZamanAşımı_msn = 10000)
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

                        return çıktı;
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
            static public string Klasörden(string KlasörYolu, bool VeYaz)
            {
                Dizin_ Dizin = new Dizin_();
                Dizin.Dosyalar = new Dictionary<string, byte[]>();
                Dizin.Klasörler = new List<string>();
                KlasörYolu = KlasörYolu.TrimEnd('\\');

                string[] liste_d = Directory.GetFiles(KlasörYolu, "*.*", SearchOption.AllDirectories);
                foreach (string b in liste_d)
                {
                    if (Path.GetFileName(b) == DoğrulamaKodu_DosyaAdı) continue;

                    Dizin.Dosyalar.Add(b.Substring(KlasörYolu.Length + 1), Dosyadan(b));
                }

                string[] liste_k = Directory.GetDirectories(KlasörYolu, "*", SearchOption.AllDirectories);
                foreach (string b in liste_k)
                {
                    Dizin.Klasörler.Add(b.Substring(KlasörYolu.Length + 1));
                }

                string GörselÇıktı = "", kod = "";
                _Ekle_("- ArGeMup Klasör Dosya Doğrulama Aracı", ref kod, ref GörselÇıktı);
                _Ekle_("- Dosya Sayısı : " + liste_d.Length, ref kod, ref GörselÇıktı);
                _Ekle_("- Klasör Sayısı : " + liste_k.Length, ref kod, ref GörselÇıktı);
                _Ekle_("- " + Kendi.Adı() + " / V" + Kendi.Sürümü_Dosya(), ref kod, ref GörselÇıktı);
                _Ekle_("- " + D_TarihSaat.Yazıya(DateTime.Now), ref kod, ref GörselÇıktı);
                _Ekle_("- https://github.com/ArgeMup/HazirKod_Cdiyez", ref kod, ref GörselÇıktı);
                _Ekle_("- " + D_HexYazı.BaytDizisinden(D_Nesne.BaytDizisine(Dizin)), ref kod, ref GörselÇıktı);
                _Ekle_("- " + Yazıdan(kod), ref kod, ref GörselÇıktı);

                if (VeYaz) File.WriteAllText(KlasörYolu + "\\" + DoğrulamaKodu_DosyaAdı, GörselÇıktı);

                return GörselÇıktı;
            }
        }

        public class KontrolEt
        {
            public enum Durum_ { DoğrulamaDosyasıYok = -3, DoğrulamaDosyasıİçeriğiHatalı, Farklı, Aynı = 1, FazlaKlasörVeyaDosyaVar };

            static public Durum_ Klasör(string KlasörYolu)
            {
                KlasörYolu = KlasörYolu.TrimEnd('\\') + "\\";

                if (!File.Exists(KlasörYolu + DoğrulamaKodu_DosyaAdı)) return Durum_.DoğrulamaDosyasıYok;
                string[] dosya_içeriği = File.ReadAllLines(KlasörYolu + DoğrulamaKodu_DosyaAdı);

                Dizin_ Dizin;
                try
                {
                    string GörselÇıktı = "", kod = "";
                    for (int i = 0; i < dosya_içeriği.Length - 1; i++)
                    {
                        _Ekle_(dosya_içeriği[i], ref kod, ref GörselÇıktı);
                    }
                    if (Üret.Yazıdan(kod) != dosya_içeriği[dosya_içeriği.Length - 1].Remove(0, 2)) return Durum_.DoğrulamaDosyasıİçeriğiHatalı;

                    Dizin = (Dizin_)D_Nesne.BaytDizisinden(D_HexYazı.BaytDizisine(dosya_içeriği[dosya_içeriği.Length - 2].Remove(0, 2)));
                }
                catch (Exception) { return Durum_.DoğrulamaDosyasıİçeriğiHatalı; }

                foreach (var b in Dizin.Dosyalar)
                {
                    string şimdiki = KlasörYolu + b.Key;

                    byte[] hesaplanan = Üret.Dosyadan(şimdiki);
                    if (hesaplanan == null || !hesaplanan.SequenceEqual(b.Value)) return Durum_.Farklı;
                }

                foreach (var b in Dizin.Klasörler)
                {
                    string şimdiki = KlasörYolu + b;

                    if (!Directory.Exists(şimdiki)) return Durum_.Farklı;
                }

                string[] liste = Directory.GetFiles(KlasörYolu, "*.*", SearchOption.AllDirectories);
                if (Dizin.Dosyalar.Count != liste.Length - 1 /*DoğrulamaKodu_DosyaAdı*/) return Durum_.FazlaKlasörVeyaDosyaVar;

                liste = Directory.GetDirectories(KlasörYolu, "*", SearchOption.AllDirectories);
                if (Dizin.Klasörler.Count != liste.Length) return Durum_.FazlaKlasörVeyaDosyaVar;

                return Durum_.Aynı;
            }
        }

        static void _Ekle_(string Girdi, ref string Kod, ref string GörselÇıktı)
        {
            Kod = Üret.Yazıdan(Kod + Üret.Yazıdan(Girdi));

            GörselÇıktı += Girdi + Environment.NewLine; 
        }
    }
}