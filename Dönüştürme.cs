// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Linq;

namespace ArgeMup.HazirKod.Dönüştürme
{
    public static class D_Yazı
    {
        public const string Sürüm = "V1.3";

        public static byte[] BaytDizisine(string Girdi)
        {
            if (string.IsNullOrEmpty(Girdi)) return null;
            return System.Text.Encoding.UTF8.GetBytes(Girdi);
        }
        public static string BaytDizisinden(byte[] Girdi, int Boyut = int.MinValue)
        {
            if (Girdi == null) return "";
            if (Boyut == int.MinValue) Boyut = Girdi.Length;
            if (Boyut > Girdi.Length) Boyut = Girdi.Length;

            return System.Text.Encoding.UTF8.GetString(Girdi, 0, Boyut).TrimEnd('\0');
        }

        public static byte[] Taban64ten(string Girdi)
        {
            if (Girdi == null) return null;

            return Convert.FromBase64String(Girdi);
        }
    }

    public static class D_HexYazı
    {
        public const string Sürüm = "V1.1";

        public static byte[] BaytDizisine(string Girdi)
        {
            if (string.IsNullOrEmpty(Girdi)) return null;

            int BaşlangıçKonumu = 0;
            if (Girdi.StartsWith("0x")) BaşlangıçKonumu += 2;

            return Enumerable.Range(BaşlangıçKonumu, Girdi.Length - BaşlangıçKonumu)
                     .Where(x => x % 2 == 0)
                     .Select(x => Convert.ToByte(Girdi.Substring(x, 2), 16))
                     .ToArray();
        }
        public static string BaytDizisinden(byte[] Girdi, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
        {
            if (Girdi == null || Girdi.Length == 0) return "";

            if (Adet == int.MinValue) Adet = Girdi.Length - BaşlangıçKonumu;
            if (Adet > Girdi.Length - BaşlangıçKonumu) Adet = Girdi.Length - BaşlangıçKonumu;

            string Çıktı = "";
            for (int w = 0; w < Adet; w++) Çıktı += Girdi[w].ToString("X2");
            return Çıktı;
        }
    }

    public static class D_Sayı
    {
        public const string Sürüm = "V1.1";

        public static readonly char ayraç_kesir = Convert.ToChar(System.Globalization.CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);
        public static readonly char ayraç_tamsayı = Convert.ToChar(System.Globalization.CultureInfo.InvariantCulture.NumberFormat.NumberGroupSeparator);
        public static double Yazıdan(string Girdi, bool TamKontrol = true, bool GeçersizKarakterleriSil = true)
        {
            if (!string.IsNullOrEmpty(Girdi))
            {
                if (TamKontrol)
                {
                    //tamsayı   ,
                    //kesir     .
                    int adet_kesir = Girdi.Count(x => x == ayraç_kesir);
                    int adet_tamsayı = Girdi.Count(x => x == ayraç_tamsayı);

                    if (adet_tamsayı == 0)
                    {
                        if (adet_kesir > 1)
                        {
                            //100.200.300.400
                            Girdi = Girdi.Replace(ayraç_kesir.ToString(), "");
                            //100200300400
                        }
                        else if (adet_kesir == 1)
                        {
                            if (Girdi.IndexOf(ayraç_kesir) == 0)
                            {
                                //.100
                                Girdi = "0" + Girdi;
                                //0.100
                            }
                            //else
                            //{
                            //    //100.200
                            //}
                        }
                    }
                    else if (adet_kesir == 0)
                    {
                        if (adet_tamsayı > 1)
                        {
                            //100,200,300,400
                            Girdi = Girdi.Replace(ayraç_tamsayı.ToString(), "");
                            //100200300400
                        }
                        else if (adet_tamsayı == 1)
                        {
                            if (Girdi.IndexOf(ayraç_tamsayı) == 0)
                            {
                                //,100
                                Girdi = "0" + Girdi;
                                //0,100
                            }
                            
                            //100,200
                            Girdi = Girdi.Replace(ayraç_tamsayı, ayraç_kesir);
                            //100.200
                        }
                    }
                    else 
                    {
                        if (adet_tamsayı == 1 && Girdi.LastIndexOf(ayraç_tamsayı) > Girdi.LastIndexOf(ayraç_kesir))
	                    {
                            //100.200.300,400
                            Girdi = Girdi.Replace(ayraç_kesir.ToString(), "");
                            //100200300,400
                            Girdi = Girdi.Replace(ayraç_tamsayı, ayraç_kesir);
                            //100200300.400
                        }
                        else if (adet_kesir == 1 && Girdi.LastIndexOf(ayraç_kesir) > Girdi.LastIndexOf(ayraç_tamsayı))
                        {
                            //100,200,300.400
                            Girdi = Girdi.Replace(ayraç_tamsayı.ToString(), "");
                            //100200300.400
                        }
                        else Girdi = Girdi.Replace(ayraç_tamsayı.ToString(), "+" + ayraç_tamsayı + "+").Replace(ayraç_kesir.ToString(), "-" + ayraç_kesir + "-");
                    }
                }

                if (double.TryParse(Girdi, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out double Çıktı))
                {
                    return Çıktı;
                }

                if (GeçersizKarakterleriSil)
                {
                    string yeni = "";
                    bool Enazbirkarakterbulundu = false;
                    foreach (char krt in Girdi)
                    {
                        if (krt == ayraç_tamsayı || krt == ayraç_kesir || krt == '+' || krt == '-' || (krt >= '0' && krt <= '9'))
                        {
                            yeni += krt;
                            Enazbirkarakterbulundu = true;
                        }
                        else if (Enazbirkarakterbulundu) break;
                    }

                    //tekrar dene
                    if (double.TryParse(yeni, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out Çıktı))
                    {
                        return Çıktı;
                    }
                }
            }

            throw new Exception(Girdi + " sayıya dönüştürülemiyor");
        }
        public static string Yazıya(double Girdi)
        {
            return Girdi.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public static class D_DosyaKlasörAdı
    {
        public const string Sürüm = "V1.2";

        public static string KullanılmayacakKarakterler_DosyaAdı { get { return new string(System.IO.Path.GetInvalidFileNameChars()); } }
        public static string KullanılmayacakKarakterler_KlasörYolu { get { return new string(System.IO.Path.GetInvalidPathChars()); } }
        
        public static string Düzelt(string Girdi, bool GeçersizKarakterleriSil = true)
        {
            Girdi = Girdi.Trim().TrimEnd('\\');

            if (GeçersizKarakterleriSil)
            {
                string kök = "", kls = "", dsy = "";

                int konum_bölüm = Girdi.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                if (konum_bölüm >= 0)
                {
                    dsy = Girdi.Substring(konum_bölüm + 1);
                    kls = Girdi.Substring(0, konum_bölüm + 1);
                }
                else dsy = Girdi;

                foreach (char c in KullanılmayacakKarakterler_DosyaAdı)
                {
                    dsy = dsy.Replace(c.ToString(), "");
                }

                foreach (char c in KullanılmayacakKarakterler_KlasörYolu)
                {
                    kls = kls.Replace(c.ToString(), "");
                }

                string birleştirilmiş = (string.IsNullOrEmpty(kls) ? null : kls) + dsy;

                kök = System.IO.Path.GetPathRoot(birleştirilmiş);
                string kls_köksüz = birleştirilmiş.Substring(kök.Length);
                string tekli = System.IO.Path.DirectorySeparatorChar.ToString();
                string ikili = tekli + tekli;
                while (kls_köksüz.Contains(ikili)) kls_köksüz = kls_köksüz.Replace(ikili, tekli);

                Girdi = kök + kls_köksüz;
            }
            
            return Girdi;
        }
    }

    public static class D_TarihSaat
    {
        public const string Sürüm = "V1.1";

        public const string Şablon_UtcZamanFarkı = "zzz";
        public const string Şablon_HaftanınGünü = "ddd";
        public const string Şablon_Tarih = "dd.MM.yyyy";
        public const string Şablon_Saat = "HH:mm:ss";
        public const string Şablon_MiliSaniye = "fff";
        
        public const string Şablon_Tarih_Saat_MiliSaniye = "dd.MM.yyyy HH:mm:ss.fff";
        public const string Şablon_Tarih_Saat = "dd.MM.yyyy HH:mm:ss";
        public const string Şablon_DosyaAdı = "dd_MM_yyyy_HH_mm_ss";
        public const string Şablon_DosyaAdı2 = "yyyy_MM_dd_HH_mm_ss_fff";
        
        public static string Yazıya(DateTime Girdi, string Şablon = Şablon_Tarih_Saat_MiliSaniye, System.Globalization.CultureInfo Kültür = null)
        {
            return Girdi.ToString(Şablon, Kültür == null ? System.Globalization.CultureInfo.InvariantCulture : Kültür);
        }
        public static string Yazıya(double Girdi, string Şablon = Şablon_Tarih_Saat_MiliSaniye, System.Globalization.CultureInfo Kültür = null)
        {
            return Yazıya(Tarihe(Girdi), Şablon, Kültür);
        }
        public static DateTime Tarihe(string Girdi)
        {
            return Tarihe(Sayıya(Girdi));
        }
        public static DateTime Tarihe(double Girdi)
        {
            return DateTime.FromOADate(Girdi);
        }
        public static double Sayıya(DateTime Girdi)
        {
            return Girdi.ToOADate();
        }
        public static double Sayıya(string Girdi)
        {
            if (!string.IsNullOrEmpty(Girdi))
            {
                if (Girdi.Length >= Şablon_Tarih_Saat_MiliSaniye.Length)
                {
                    if (DateTime.TryParseExact(Girdi.Substring(0, Şablon_Tarih_Saat_MiliSaniye.Length), Şablon_Tarih_Saat_MiliSaniye, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime yeni))
                    {
                        return yeni.ToOADate();
                    }
                }

                if (Girdi.Length >= Şablon_Tarih_Saat.Length)
                {
                    if (DateTime.TryParseExact(Girdi.Substring(0, Şablon_Tarih_Saat.Length), Şablon_Tarih_Saat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime yeni))
                    {
                        return yeni.ToOADate();
                    }
                }

                if (Girdi.Length >= Şablon_DosyaAdı.Length)
                {
                    if (DateTime.TryParseExact(Girdi.Substring(0, Şablon_DosyaAdı.Length), Şablon_DosyaAdı, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime yeni))
                    {
                        return yeni.ToOADate();
                    }
                }

                if (Girdi.Length >= Şablon_DosyaAdı2.Length)
                {
                    if (DateTime.TryParseExact(Girdi.Substring(0, Şablon_DosyaAdı2.Length), Şablon_DosyaAdı2, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime yeni))
                    {
                        return yeni.ToOADate();
                    }
                }

                if (Girdi.Length >= Şablon_Tarih.Length)
                {
                    if (DateTime.TryParseExact(Girdi.Substring(0, Şablon_Tarih.Length), Şablon_Tarih, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime yeni))
                    {
                        return yeni.ToOADate();
                    }
                }

                if (Girdi.Length >= Şablon_Saat.Length)
                {
                    if (DateTime.TryParseExact(Girdi.Substring(0, Şablon_Saat.Length), Şablon_Saat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeLocal, out DateTime yeni))
                    {
                        return yeni.ToOADate();
                    }
                }

                return D_Sayı.Yazıdan(Girdi);
            }

            throw new Exception(Girdi + " tarihe dönüştürülemiyor");
        }
    }
#if NET7_0_OR_GREATER
    public static class D_SadeceTarih
    {
        public const string Sürüm = "V1.0";

        public const string Şablon_Tarih = "dd.MM.yyyy";

        public static string Yazıya(DateOnly Girdi, string Şablon = Şablon_Tarih, System.Globalization.CultureInfo Kültür = null)
        {
            return Girdi.ToString(Şablon, Kültür == null ? System.Globalization.CultureInfo.InvariantCulture : Kültür);
        }
      
        public static DateOnly SadeceTarihe(string Girdi, string Şablon = Şablon_Tarih, System.Globalization.CultureInfo Kültür = null, System.Globalization.DateTimeStyles Tip = System.Globalization.DateTimeStyles.AssumeLocal)
        {
            //DateOnly .net 8 ile gelecek
            //if (System.DateOnly.TryParseExact(Girdi, Şablon, Kültür, Tip, out System.DateOnly yeni)) return yeni;
            if (DateTime.TryParseExact(Girdi, Şablon, Kültür, Tip, out DateTime yeni)) return DateOnly.FromDateTime(yeni);

            throw new Exception("Tarihe dönüştürülemedi " + Girdi);
        }
    }
    public static class D_SadeceSaat
    {
        public const string Sürüm = "V1.0";

        public const string Şablon_Saat = "HH:mm:ss.ffffff";

        public static string Yazıya(TimeOnly Girdi, string Şablon = Şablon_Saat, System.Globalization.CultureInfo Kültür = null)
        {
            return Girdi.ToString(Şablon, Kültür == null ? System.Globalization.CultureInfo.InvariantCulture : Kültür);
        }

        public static TimeOnly SadeceSaate(string Girdi, string Şablon = Şablon_Saat, System.Globalization.CultureInfo Kültür = null, System.Globalization.DateTimeStyles Tip = System.Globalization.DateTimeStyles.AssumeLocal)
        {
            //TimeOnly .net 8 ile gelecek
            //if (System.TimeOnly.TryParseExact(Girdi, Şablon, Kültür, Tip, out System.TimeOnly yeni)) return yeni;
            try
            {
                string[] dizi = Girdi.Split(new string[] { ":", "." }, System.StringSplitOptions.None);
                int saat = int.Parse(dizi[0]), dakika = int.Parse(dizi[1]), saniye = int.Parse(dizi[2]), milisaniye = int.Parse(dizi[3].Substring(0, 3)), mikrosaniye = int.Parse(dizi[3].Substring(3));
                return new TimeOnly(saat, dakika, saniye, milisaniye, mikrosaniye);
            }
            catch (Exception)
            {
                throw new Exception("Saate dönüştürülemedi " + Girdi);
            }
        }
    }
#endif

    public static class D_Akış
    {
        public const string Sürüm = "V1.1";

        public static byte[] BaytDizisine(System.IO.MemoryStream Girdi, int Boyut = int.MinValue)
        {
            if (Girdi == null) return null;
            if (Boyut == int.MinValue) Boyut = (int)Girdi.Length;
            if (Boyut > Girdi.Length) Boyut = (int)Girdi.Length;

            byte[] Çıktı = new byte[Boyut];
			#if NET7_0_OR_GREATER
            	Girdi.ReadExactly(Çıktı, 0, Boyut);
			#else
	            int adt = Girdi.Read(Çıktı, 0, Boyut);
	            if (adt != Boyut) throw new Exception("adt != Boyut");
			#endif

            return Çıktı;
        }
        public static void BaytDizisinden(byte[] Girdi, ref System.IO.MemoryStream Çıktı, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
        {
            if (Girdi == null) return;
            if (Adet == int.MinValue) Adet = Girdi.Length - BaşlangıçKonumu;
            if (Adet > Girdi.Length - BaşlangıçKonumu) Adet = Girdi.Length - BaşlangıçKonumu;

            Çıktı.Write(Girdi, 0, Adet);
        }
    }

    public static class D_Nesne
    {
        #if NET7_0_OR_GREATER
            /// <summary>
            /// Sadece public değişkenler dahil edilir
            /// 
            /// Bir sınıfı açıp kapatırken, ilgili sınıfın ve içindeki diğer sınıfların
            /// alttaki kıstaslardan birisine uygun olduğunu teyit et
            /// tersi durumda ThrowInvalidOperationException_ConstructorParameterIncompleteBinding
            /// 
            /// A)Başlatıcı işlem parametresiz olmalı
            /// B)Başlatıcı işlemin parametrelerinin isimleri ile json içeriğinde geçen isimler aynı olmalı
            /// 
            /// public class Örnek_Üst
            /// {
            ///    public string Yaz1, Yaz2;
            ///    public List<Örnek_Alt> Alttakiler;
            ///
            ///    public Örnek_Üst() { }                            //A maddesine örnek
            ///    public Örnek_Üst(string Yaz1, string Yaz2) { }    //B maddesine örnek
            /// }
            /// public class Örnek_Alt
            /// {
            ///    public string Yaz1, Yaz2;
            ///
            ///    public Örnek_Alt() { }                            //A maddesine örnek
            ///    public Örnek_Alt(string Yaz1, string Yaz2) { }    //B maddesine örnek
            /// }
            /// 
            /// Örnek_Üst ü = new Örnek_Üst() { Yaz1 = "ust_1_", Yaz2 = "ust_2_", Alttakiler = new List<Örnek_Alt>() };
            /// ü.Alttakiler.Add(new Örnek_Alt() { Yaz1 = "alt_a_1", Yaz2 = "alt_a_2" });
            /// byte[] dizi = D_Nesne.BaytDizisine(ü);
            /// Trace.WriteLine(dizi.Yazıya());
            /// Örnek_Üst üü = (Örnek_Üst)D_Nesne.BaytDizisinden(dizi, typeof(Örnek_Üst));
            /// 
            /// {"Yaz1":"ust_1_","Yaz2":"ust_2_","Alttakiler":[{"Yaz1":"alt_a_1","Yaz2":"alt_a_2"}]}
            /// </summary>

            public const string Sürüm = "V1.2";
            static readonly System.Text.Json.JsonSerializerOptions seçenekler = new System.Text.Json.JsonSerializerOptions() { IncludeFields = true };

            public static byte[] BaytDizisine(object Girdi)
            {
                if (Girdi == null) return null;

                return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(Girdi, Girdi.GetType(), seçenekler);
            }
            public static object BaytDizisinden(byte[] Girdi, Type Tipi)
            {
                if (Girdi == null || Girdi.Length < 1) return null;

                return System.Text.Json.JsonSerializer.Deserialize(Girdi, Tipi, seçenekler);
            }
        #else
            public const string Sürüm = "V1.1";

            public static byte[] BaytDizisine(object Girdi)
            {
                if (Girdi == null) return null;

                byte[] Çıktı;
                using (var mS = new System.IO.MemoryStream())
                {
                    var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    bf.Serialize(mS, Girdi);
                    Çıktı = mS.ToArray();
                }
                return Çıktı;
            }
            public static object BaytDizisinden(byte[] Girdi, int Adet = int.MinValue, int BaşlangıçKonumu = 0)
            {
                if (Girdi == null) return null;
                if (Adet == int.MinValue) Adet = Girdi.Length - BaşlangıçKonumu;
                if (Adet > Girdi.Length - BaşlangıçKonumu) Adet = Girdi.Length - BaşlangıçKonumu;

                object Çıktı;
                using (var mS = new System.IO.MemoryStream())
                {
                    var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    mS.Write(Girdi, BaşlangıçKonumu, Adet);
                    mS.Seek(0, System.IO.SeekOrigin.Begin);
                    Çıktı = bf.Deserialize(mS);
                }
                return Çıktı;
            }
        #endif
    }

    public static class D_WwwAdresi
    {
        public const string Sürüm = "V1.0";

        public static string IpAdresine(string Girdi)
        {
            return System.Net.Dns.GetHostAddresses(Girdi)[0].ToString();
        }
    }
		
	#if HazirKod_Cdiyez_Görsel
        public static class D_İkon
        {
            public const string Sürüm = "V1.0";

            public static System.Drawing.Icon Yazıdan(string Yazı, System.Drawing.Icon ikon, System.Drawing.Font font, System.Drawing.Color Renk, System.Drawing.Point Konum, System.Drawing.Color ArkaPlan)
            {
                System.Drawing.Brush brush = new System.Drawing.SolidBrush(Renk);
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(ikon.Width, ikon.Height);
                System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(bitmap);
                graphics.Clear(ArkaPlan);
                graphics.DrawString(Yazı, font, brush, Konum.X, Konum.Y);
                System.Drawing.Icon createdIcon = System.Drawing.Icon.FromHandle(bitmap.GetHicon());

                brush.Dispose();
                graphics.Dispose();
                bitmap.Dispose();

                return createdIcon;
            }

            public static void Yoket(System.Drawing.Icon ikon)
            {
                W32_8.DestroyIcon(ikon.Handle);
                ikon.Dispose();
            }
        }
	#endif

    public static class D_DosyaBoyutu
    {
        public const string Sürüm = "V1.1";

        public static string Yazıya(decimal d, int NoktadanSonrakiKarakterSayısı = 2)
        {
            string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
            if (d < 1) return "0B";

            int place = Convert.ToInt32(Math.Floor(Math.Log((Double)d, 1024)));
            double num = Math.Round((Double)d / Math.Pow(1024, place), NoktadanSonrakiKarakterSayısı);
            return (num).ToString() + " " + suf[place];
        }
    }

    public static class D_Süre
    {
        public const string Sürüm = "V1.3";

        public static class Yazıya
        {
            public static string SaatDakikaSaniye(int Saat, int Dakika = 0, int Saniye = 0)
            {
                TimeSpan bbb = new TimeSpan(Saat, Dakika, Saniye);

                int gün = bbb.Days;
                int yıl = gün / 365;
                gün -= yıl * 365;

                int ay = gün / 30;
                gün -= ay * 30;

                string Çıktı = "";
                if (yıl > 0) Çıktı = Çıktı.Trim() + yıl + " yıl ";
                if (ay > 0) Çıktı = Çıktı.Trim() + " " + ay + " ay ";
                if (gün > 0) Çıktı = Çıktı.Trim() + " " + gün + " gün ";
                if (bbb.Hours > 0) Çıktı = Çıktı.Trim() + " " + bbb.Hours + " sa. ";
                if (bbb.Minutes > 0) Çıktı = Çıktı.Trim() + " " + bbb.Minutes + " dk. ";
                if (Dakika > 0 || Saniye > 0)
                {
                    if (bbb.Seconds > 0) Çıktı = Çıktı.Trim() + " " + bbb.Seconds + " sn. ";

                    if (Saniye > 0)
                    {
                        if (bbb.Milliseconds > 0) Çıktı = Çıktı.Trim() + " " + bbb.Milliseconds + " msn. ";
                    }
                }

                if (Çıktı == "")
                {
                    if (Dakika > 0) Çıktı = "1 sn. den az";
                    else Çıktı = "1 dk. dan az";
                }
                return Çıktı.Trim();
            }
            public static string Saatten(int Saat)
            {
                TimeSpan bbb = new TimeSpan(Saat, 0, 0);

                int gün = bbb.Days;
                int yıl = gün / 365;
                gün -= yıl * 365;

                int ay = gün / 30;
                gün -= ay * 30;

                string Çıktı = "";
                if (yıl > 0) Çıktı = Çıktı.Trim() + yıl + " yıl ";
                if (ay > 0) Çıktı = Çıktı.Trim() + " " + ay + " ay ";
                if (gün > 0) Çıktı = Çıktı.Trim() + " " + gün + " gün ";
                if (bbb.Hours > 0) Çıktı = Çıktı.Trim() + " " + bbb.Hours + " sa. ";
                if (bbb.Minutes > 0) Çıktı = Çıktı.Trim() + " " + bbb.Minutes + " dk. ";

                if (Çıktı == "") Çıktı = "1 dk. dan az";
                return Çıktı.Trim();
            }
            public static string Saniyeden(UInt64 Saniye)
            {
                decimal sn = Saniye, dk = 0, sa = 0, gün = 0, ay = 0, yıl = 0;

                while (sn >= 60)
                {
                    dk++;
                    sn -= 60;
                }

                while (dk >= 60)
                {
                    sa++;
                    dk -= 60;
                }

                while (sa >= 24)
                {
                    gün++;
                    sa -= 24;
                }

                while (gün >= 30)
                {
                    ay++;
                    gün -= 30;
                }

                while (ay >= 12)
                {
                    yıl++;
                    ay -= 12;
                }

                string Çıktı = "";
                if (yıl > 0) Çıktı = Çıktı.Trim() +       yıl + " yıl ";
                if (ay > 0)  Çıktı = Çıktı.Trim() + " " + ay  + " ay ";
                if (gün > 0) Çıktı = Çıktı.Trim() + " " + gün + " gün ";
                if (sa > 0)  Çıktı = Çıktı.Trim() + " " + sa  + " sa. ";
                if (dk > 0)  Çıktı = Çıktı.Trim() + " " + dk  + " dk. ";
                if (sn > 0)  Çıktı = Çıktı.Trim() + " " + sn  + " sn. ";

                if (Çıktı == "") Çıktı = "1 sn. den az";
                return Çıktı.Trim();
            }
            public static string MiliSaniyeden(UInt64 MiliSaniye)
            {
                decimal msn = MiliSaniye, sn = 0, dk = 0, sa = 0, gün = 0, ay = 0, yıl = 0;

                while (msn >= 1000)
                {
                    sn++;
                    msn -= 1000;
                }

                while (sn >= 60)
                {
                    dk++;
                    sn -= 60;
                }

                while (dk >= 60)
                {
                    sa++;
                    dk -= 60;
                }

                while (sa >= 24)
                {
                    gün++;
                    sa -= 24;
                }

                while (gün >= 30)
                {
                    ay++;
                    gün -= 30;
                }

                while (ay >= 12)
                {
                    yıl++;
                    ay -= 12;
                }

                string Çıktı = "";
                if (yıl > 0) Çıktı = Çıktı.Trim() + yıl + " yıl ";
                if (ay > 0) Çıktı = Çıktı.Trim() + " " + ay + " ay ";
                if (gün > 0) Çıktı = Çıktı.Trim() + " " + gün + " gün ";
                if (sa > 0) Çıktı = Çıktı.Trim() + " " + sa + " sa. ";
                if (dk > 0) Çıktı = Çıktı.Trim() + " " + dk + " dk. ";
                if (sn > 0) Çıktı = Çıktı.Trim() + " " + sn + " sn. ";
                if (msn > 0) Çıktı = Çıktı.Trim() + " " + msn + " msn. ";

                if (Çıktı == "") Çıktı = "1 msn. den az";
                return Çıktı.Trim();
            }
        }
    }

    public static class D_BaytDizisi
    {
        public const string Sürüm = "V1.1";

        public static byte[] TümünüDeğiştir(byte[] Kaynak, byte[] Aranan, byte[] YeniBilgi)
        {
            if (Kaynak == null || Kaynak.Length == 0 || Aranan == null || Aranan.Length == 0 || YeniBilgi == null || YeniBilgi.Length == 0) return null;

            string Kay = D_HexYazı.BaytDizisinden(Kaynak);
            string Ara = D_HexYazı.BaytDizisinden(Aranan);
            string Yen = D_HexYazı.BaytDizisinden(YeniBilgi);
            string Çık = Kay.Replace(Ara, Yen);

            return D_HexYazı.BaytDizisine(Çık);
        }

        public static string Taban64e(byte[] Girdi)
        {
            if (Girdi == null || Girdi.Length == 0) return null;

            return Convert.ToBase64String(Girdi);
        }
    }
}