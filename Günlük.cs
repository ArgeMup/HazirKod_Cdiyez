// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.IO;
using ArgeMup.HazirKod.Dönüştürme;
using System.Runtime.CompilerServices;

namespace ArgeMup.HazirKod
{
    public class Günlük
    {
        public const string Sürüm = "V1.2";
        public static int Seviyesi = 0;
        static string Yolu = null;
        static System.Threading.Mutex Kilit = new System.Threading.Mutex();

        //Hedef : \\?\C:\Users\<KullanıcıAdı>\AppData\Local\Temp\<Aile>\<Uygulama>\<Sürüm>\dd_MM_yyyy_HH_mm_ss.Gunluk
        public static void Başlat(string Klasörü = null)
        {
            if (Klasörü == null) Klasörü = @"\\?\" + Klasör.Depolama(Klasör.Kapsamı.Geçici) + @"\Gunluk";
            else Klasörü = D_DosyaKlasörAdı.Düzelt(Klasörü, false);
            if (!Klasör.Oluştur(Klasörü)) throw new Exception("Klasör oluşturulamadı " + Klasörü);

            Yolu = Klasörü + @"\" + D_TarihSaat.Yazıya(DateTime.Now, D_TarihSaat.Şablon_DosyaAdı) + ".Gunluk";

            Dosya.Sil_TarihineGöre(Klasörü, 30, "*.Gunluk");
            Dosya.Sil_BoyutunaGöre(Klasörü, 50 * 1024 * 1024 /*50 MiB*/, "*.Gunluk");
            Dosya.Sil_SayısınaGöre(Klasörü, 500, "*.Gunluk");

            Ekle("Başladı " + Kendi.DosyaYolu() + " V" + Kendi.Sürümü_Dosya());
        }
        public static void Ekle(string Mesaj, int Seviye = 0, [CallerFilePath] string ÇağıranDosya = "", [CallerLineNumber] int ÇağıranSatırNo = 0, int DosyayaKaydetmeZamanAşımı_msn = 1000)
        {
            if (Seviye > Seviyesi) return; //Seviyesi nden küçük eşit mesajları yazdıracak

            string içerik = D_TarihSaat.Yazıya(DateTime.Now) + " " + Path.GetFileName(ÇağıranDosya) + ":" + ÇağıranSatırNo + " " + Mesaj.Replace("\r\n", "|").Replace('\r', '|').Replace('\n', '|') + Environment.NewLine;
            
            Console.Write(içerik);

            if (Yolu != null)
            {
                if (Kilit.WaitOne(DosyayaKaydetmeZamanAşımı_msn))
                {
                    try { File.AppendAllText(Yolu, içerik); } catch (Exception) { }
                    
                    Kilit.ReleaseMutex();
                }
            }
        }
    }
}