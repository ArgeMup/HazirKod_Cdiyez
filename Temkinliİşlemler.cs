// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using ArgeMup.HazirKod.Ekİşlemler;
using System;

namespace ArgeMup.HazirKod
{
    public static class Temkinli
    {
        public static class Klasör
        {
            public const string Sürüm = "V1.0";

            public static string[] Listele_Dosya(string Yolu, string Filtre = "*.*", System.IO.SearchOption Kapsam = System.IO.SearchOption.AllDirectories)
            {
                try
                {
                    return ArgeMup.HazirKod.Klasör.Listele_Dosya(Yolu, Filtre, Kapsam);    
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return new string[0];
            }
            public static string[] Listele_Klasör(string Yolu, string Filtre = "*", System.IO.SearchOption Kapsam = System.IO.SearchOption.AllDirectories)
            {
                try
                {
                    return ArgeMup.HazirKod.Klasör.Listele_Klasör(Yolu, Filtre, Kapsam);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return new string[0];
            }

            public static bool Oluştur(string Yolu)
            {
                try
                {
                    ArgeMup.HazirKod.Klasör.Oluştur(Yolu);

                    return true;
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return false;
            }
            public static bool Sil(string Yolu)
            {
                try
                {
                    ArgeMup.HazirKod.Klasör.Sil(Yolu);

                    return true;
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return false;
            }
        }

        public static class Dosya
        {
            public const string Sürüm = "V1.0";

            public static string Oku_Yazı(string DosyaYolu)
            {
                try
                {
                    if (!ArgeMup.HazirKod.Dosya.VarMı(DosyaYolu)) return null;

                    return ArgeMup.HazirKod.Dosya.Oku_Yazı(DosyaYolu);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return null;
            }
            public static byte[] Oku_BaytDizisi(string DosyaYolu)
            {
                try
                {
                    if (!ArgeMup.HazirKod.Dosya.VarMı(DosyaYolu)) return null;

                    return ArgeMup.HazirKod.Dosya.Oku_BaytDizisi(DosyaYolu);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return null;
            }
            public static bool Yaz(string DosyaYolu, string Yazı)
            {
                try
                {
                    ArgeMup.HazirKod.Dosya.Yaz(DosyaYolu, Yazı);

                    return true;
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return false;
            }
            public static bool Yaz(string DosyaYolu, byte[] BaytDizisi)
            {
                try
                {
                    ArgeMup.HazirKod.Dosya.Yaz(DosyaYolu, BaytDizisi);

                    return true;
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return false;
            }
            public static bool Kopyala(string Kaynak, string Hedef)
            {
                string yedek_dosya_adı = Hedef + ".yedek";

                try
                {
                    if (ArgeMup.HazirKod.Dosya.VarMı(Hedef))
                    {
                        ArgeMup.HazirKod.Dosya.Sil(yedek_dosya_adı);
                        System.IO.File.Move(Hedef, yedek_dosya_adı);
                    }
                    else ArgeMup.HazirKod.Klasör.Oluştur(ArgeMup.HazirKod.Dosya.Klasörü(Hedef));

                    ArgeMup.HazirKod.Dosya.Kopyala(Kaynak, Hedef);
                    ArgeMup.HazirKod.Dosya.Sil(yedek_dosya_adı);

                    return true;
                }
                catch (Exception ex)
                {
                    ex.Günlük(null, Günlük.Seviye.HazirKod);

                    if (ArgeMup.HazirKod.Dosya.VarMı(yedek_dosya_adı))
                    {
                        ArgeMup.HazirKod.Dosya.Sil(Hedef);
                        System.IO.File.Move(yedek_dosya_adı, Hedef);
                    }
                }

                return false;
            }
            public static bool Sil(string Yolu)
            {
                try
                {
                    ArgeMup.HazirKod.Dosya.Sil(Yolu);

                    return true;
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return false;
            }
        }

        public static class DahaCokKarmasiklastirma
        {
            public const string Sürüm = "V1.0";

            public static string Karıştır(string Girdi, string Parola)
            {
                try
                {
                    return ArgeMup.HazirKod.DahaCokKarmasiklastirma.Karıştır(Girdi, Parola);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return null;
            }
            public static byte[] Karıştır(byte[] Girdi, byte[] Parola)
            {
                try
                {
                    return ArgeMup.HazirKod.DahaCokKarmasiklastirma.Karıştır(Girdi, Parola);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return null;
            }

            public static string Düzelt(string Girdi, string Parola)
            {
                try
                {
                    return ArgeMup.HazirKod.DahaCokKarmasiklastirma.Düzelt(Girdi, Parola);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return null;
            }
            public static byte[] Düzelt(byte[] Girdi, byte[] Parola)
            {
                try
                {
                    return ArgeMup.HazirKod.DahaCokKarmasiklastirma.Düzelt(Girdi, Parola);
                }
                catch (Exception ex) { ex.Günlük(null, Günlük.Seviye.HazirKod); }

                return null;
            }
        }

        #if HazirKod_Cdiyez_Sıkıştırma
	        public static class SıkıştırılmışDosya
	        {
	            public const string Sürüm = "V1.0";
	
	            public static bool Klasörden(string Kaynak, string HedefZipDosyasıYolu, bool YeniBirDosyaOluştur = true)
	            {
	                string yedek_dosya_adı = HedefZipDosyasıYolu + ".yedek";
	                
	                try
	                {
	                    if (ArgeMup.HazirKod.Dosya.VarMı(HedefZipDosyasıYolu))
	                    {
	                        ArgeMup.HazirKod.Dosya.Sil(yedek_dosya_adı);
	
	                        if (YeniBirDosyaOluştur)
	                        {
	                            System.IO.File.Move(HedefZipDosyasıYolu, yedek_dosya_adı);
	                        }
	                        else
	                        {
	                            System.IO.File.Copy(HedefZipDosyasıYolu, yedek_dosya_adı);
	                        }
	                    }
	
	                    ArgeMup.HazirKod.SıkıştırılmışDosya.Klasörden(Kaynak, HedefZipDosyasıYolu, YeniBirDosyaOluştur);
	                    ArgeMup.HazirKod.Dosya.Sil(yedek_dosya_adı);
	
	                    return true;
	                }
	                catch (Exception ex)
	                {
	                    ex.Günlük(null, Günlük.Seviye.HazirKod);
	
	                    if (ArgeMup.HazirKod.Dosya.VarMı(yedek_dosya_adı))
	                    {
	                        ArgeMup.HazirKod.Dosya.Sil(HedefZipDosyasıYolu);
	                        System.IO.File.Move(yedek_dosya_adı, HedefZipDosyasıYolu);
	                    }
	                }
	
	                return false;
	            }
	            public static bool Klasöre(string KaynakZipDosyasıYolu, string HedefKlasör)
	            {
	                string yedek_klasör_adı = HedefKlasör.TrimEnd('\\') + ".yedek";
	
	                try
	                {
	                    if (ArgeMup.HazirKod.Klasör.VarMı(HedefKlasör))
	                    {
	                        ArgeMup.HazirKod.Klasör.Sil(yedek_klasör_adı);
	                        System.IO.Directory.Move(HedefKlasör, yedek_klasör_adı);
	                    }
	
	                    ArgeMup.HazirKod.SıkıştırılmışDosya.Klasöre(KaynakZipDosyasıYolu, HedefKlasör);
	                    ArgeMup.HazirKod.Klasör.Sil(yedek_klasör_adı);
	
	                    return true;
	                }
	                catch (Exception ex)
	                {
	                    ex.Günlük(null, Günlük.Seviye.HazirKod);
	
	                    if (ArgeMup.HazirKod.Klasör.VarMı(yedek_klasör_adı))
	                    {
	                        ArgeMup.HazirKod.Klasör.Sil(HedefKlasör);
	                        System.IO.Directory.Move(yedek_klasör_adı, HedefKlasör);
	                    }
	                }
	
	                return false;
	            }
	        }
        #endif
    }
}