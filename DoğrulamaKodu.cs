// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Dönüştürme;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static ArgeMup.HazirKod.DoğrulamaKodu.Üret;

namespace ArgeMup.HazirKod
{
    public class DoğrulamaKodu 
    {
        public const string Sürüm = "V1.0";

        [Serializable]
        struct Dizin_
        {
            public Dictionary<string, byte[]> Dosyalar;
            public List<string> Klasörler;
        };

        public class Üret
        {
            static public byte[] Dosyadan(string DosyaYolu)
            {
                if (string.IsNullOrEmpty(DosyaYolu) || !File.Exists(DosyaYolu)) return null;

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
            static public byte[] Yazıdan(string Yazı)
            {
                byte[] çıktı = null;
                using (var SHA512 = System.Security.Cryptography.SHA512.Create())
                {
                    çıktı = SHA512.ComputeHash(D_Yazı.BaytDizisine(Yazı));
                }

                return çıktı;
            }
            static public string Klasörden(string KlasörYolu, bool VeYaz)
            {
                Dizin_ Dizin = new Dizin_();
                Dizin.Dosyalar = new Dictionary<string, byte[]>();
                Dizin.Klasörler = new List<string>();
                string DoğrulamaKodları = "- ArGeMup Klasör Dosya Doğrulama Aracı" + Environment.NewLine;

                string[] liste = Directory.GetFiles(KlasörYolu, "*.*", SearchOption.AllDirectories);
                foreach (string b in liste)
                {
                    if (Path.GetFileName(b) == "UySuKoYa.Dogrulama.Kodu") continue;

                    Dizin.Dosyalar.Add(b.Substring(KlasörYolu.Length + 1), Dosyadan(b));
                }
                DoğrulamaKodları += "- Dosya Sayısı : " + liste.Length + Environment.NewLine;

                liste = Directory.GetDirectories(KlasörYolu, "*", SearchOption.AllDirectories);
                foreach (string b in liste)
                {
                    if (Path.GetFileName(b) == "UySuKoYa.Dogrulama.Kodu") continue;

                    Dizin.Klasörler.Add(b.Substring(KlasörYolu.Length + 1));
                }

                DoğrulamaKodları += "- Klasör Sayısı : " + liste.Length + Environment.NewLine;
                DoğrulamaKodları += "- " + Kendi.Adı() + " / V" + Kendi.Sürümü_Dosya() + Environment.NewLine;
                DoğrulamaKodları += "- " + D_TarihSaat.Yazıya(DateTime.Now) + Environment.NewLine;
                DoğrulamaKodları += D_HexYazı.BaytDizisinden(D_Nesne.BaytDizisine(Dizin));

                if (VeYaz) File.WriteAllText(KlasörYolu + "\\UySuKoYa.Dogrulama.Kodu", DoğrulamaKodları);

                return DoğrulamaKodları;
            }
        }

        public class KontrolEt
        {
            public enum Durum_ { DoğrulamaDosyasıYok = -3, DoğrulamaDosyasıİçeriğiHatalı, Farklı, Aynı = 1, FazlaKlasörVeyaDosyaVar };
            
            static public Durum_ Klasör(string KlasörYolu)
            {
                if (!File.Exists(KlasörYolu + "\\UySuKoYa.Dogrulama.Kodu")) return Durum_.DoğrulamaDosyasıYok;

                Dizin_ Dizin;
                try
                {
                    Dizin = (Dizin_)D_Nesne.BaytDizisinden(D_HexYazı.BaytDizisine(File.ReadAllLines(KlasörYolu + "\\UySuKoYa.Dogrulama.Kodu")[5]));
                }
                catch (Exception) { return Durum_.DoğrulamaDosyasıİçeriğiHatalı; }

                KlasörYolu = KlasörYolu.Trim(' ', '\\') + "\\";

                foreach (var b in Dizin.Dosyalar)
                {
                    string şimdiki = KlasörYolu + b.Key;
                    if (!File.Exists(şimdiki)) return Durum_.Farklı;

                    byte[] hesaplanan = Dosyadan(şimdiki);
                    if (!hesaplanan.SequenceEqual(b.Value)) return Durum_.Farklı;
                }

                foreach (var b in Dizin.Klasörler)
                {
                    string şimdiki = KlasörYolu + b;
                    if (!Directory.Exists(şimdiki)) return Durum_.Farklı;
                }

                string[] liste = Directory.GetFiles(KlasörYolu, "*.*", SearchOption.AllDirectories);
                if (Dizin.Dosyalar.Count != liste.Length - 1 /*UySuKoYa.Dogrulama.Kodu*/) return Durum_.FazlaKlasörVeyaDosyaVar;

                liste = Directory.GetDirectories(KlasörYolu, "*", SearchOption.AllDirectories);
                if (Dizin.Klasörler.Count != liste.Length) return Durum_.FazlaKlasörVeyaDosyaVar;

                return Durum_.Aynı;
            }
        }
    }
}