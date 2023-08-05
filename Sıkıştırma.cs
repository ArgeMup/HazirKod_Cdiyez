// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_Sıkıştırma

    using System;
    using System.IO;
    using System.IO.Compression;

    namespace ArgeMup.HazirKod
    {
        public class SıkıştırılmışDosya
        {
            public const string Sürüm = "V1.0";

            public static bool Klasörden(string Kaynak, string HedefZipDosyasıYolu, bool YeniBirDosyaOluştur = true)
            {
                return Klasörden(new Klasör_(Kaynak, DoğrulamaKodunuÜret:false), HedefZipDosyasıYolu, YeniBirDosyaOluştur);
            }
            public static void Klasöre(string KaynakZipDosyasıYolu, string HedefKlasör)
            {
                Klasöre(Listele(KaynakZipDosyasıYolu, false), HedefKlasör);
            }

            public static bool Klasörden(Klasör_ Kaynak, string HedefZipDosyasıYolu, bool YeniBirDosyaOluştur = true)
            {
                string yedek_dosya_adı = HedefZipDosyasıYolu + ".yedek";
                ZipArchiveMode tip = ZipArchiveMode.Create;

                if (File.Exists(HedefZipDosyasıYolu))
                {
                    if (File.Exists(yedek_dosya_adı)) File.Delete(yedek_dosya_adı);

                    if (YeniBirDosyaOluştur)
                    {
                        File.Move(HedefZipDosyasıYolu, yedek_dosya_adı);
                    }
                    else
                    {
                        File.Copy(HedefZipDosyasıYolu, yedek_dosya_adı);

                        tip = ZipArchiveMode.Update;
                    }
                }
                else Klasör.Oluştur(Klasör.ÜstKlasör(HedefZipDosyasıYolu));
            
                try
                {
                    using (ZipArchive archive = ZipFile.Open(HedefZipDosyasıYolu, tip))
                    {
                        for (int SıraNo = 0; SıraNo < Kaynak.Klasörler.Count; SıraNo++)
                        {
                            archive.CreateEntry(Kaynak.Klasörler[SıraNo] + @"\", CompressionLevel.Optimal);
                        }

                        for (int SıraNo = 0; SıraNo < Kaynak.Dosyalar.Count; SıraNo++)
                        {
                            ZipArchiveEntry biri = null;

                            using (FileStream K = new FileStream(Kaynak.Kök + @"\" + Kaynak.Dosyalar[SıraNo].Yolu, FileMode.Open, FileAccess.Read))
                            {
                                biri = archive.CreateEntry(Kaynak.Dosyalar[SıraNo].Yolu, CompressionLevel.Optimal);
                                biri.LastWriteTime = Kaynak.Dosyalar[SıraNo].DeğiştirilmeTarihi;

                                using (Stream H = biri.Open())
                                {
                                    long KaynakDosyaBoyutu = K.Length, KaynakOkunmuşAdet = 0;
                                    int KaynakOkunacakDosyaBoyutu;
                                    byte[] Tampon = new byte[4 * 1024];

                                    while (KaynakOkunmuşAdet < KaynakDosyaBoyutu)
                                    {
                                        if (KaynakDosyaBoyutu - KaynakOkunmuşAdet > Tampon.Length) KaynakOkunacakDosyaBoyutu = Tampon.Length;
                                        else KaynakOkunacakDosyaBoyutu = (int)KaynakDosyaBoyutu - (int)KaynakOkunmuşAdet;

                                        KaynakOkunacakDosyaBoyutu = K.Read(Tampon, 0, KaynakOkunacakDosyaBoyutu);
                                        H.Write(Tampon, 0, KaynakOkunacakDosyaBoyutu);

                                        KaynakOkunmuşAdet += KaynakOkunacakDosyaBoyutu;
                                    }
                                }
                            }
                        } 
                    }

                    if (File.Exists(yedek_dosya_adı)) Dosya.Sil(yedek_dosya_adı);

                    return true;
                }
                catch (Exception ex)
                {
                    Günlük.Ekle(ex.ToString(), Günlük.Seviye.HazirKod);
                }

                if (File.Exists(yedek_dosya_adı))
                {
                    if (File.Exists(HedefZipDosyasıYolu)) File.Delete(HedefZipDosyasıYolu);

                    File.Move(yedek_dosya_adı, HedefZipDosyasıYolu);
                }

                return false;
            }
            public static Klasör_ Listele(string KaynakZipDosyasıYolu, bool DoğrulamaKodunuÜret = true)
            {
                Klasör_ çıktı = new Klasör_("");

                using (ZipArchive Arşiv = ZipFile.OpenRead(KaynakZipDosyasıYolu))
                {
                    foreach (ZipArchiveEntry Biri in Arşiv.Entries)
                    {
                        if (string.IsNullOrEmpty(Biri.Name))
                        {
                            //klasör
                            çıktı.Klasörler.Add(Biri.FullName.TrimEnd('\\'));
                        }
                        else
                        {
                            //dosya
                            Klasör_.İçerik_Dosya_ Yeni = new Klasör_.İçerik_Dosya_("", Biri.FullName, false);
                            Yeni.KapladığıAlan_bayt = Biri.Length;
                            Yeni.DeğiştirilmeTarihi = Biri.LastWriteTime.DateTime;
                            Yeni.Yolu = Biri.FullName;

                            if (DoğrulamaKodunuÜret)
                            {
                                using (Stream Akış = Biri.Open())
                                {
                                    Yeni.Doğrulama_Kodu = DoğrulamaKodu.Üret.Akıştan(Akış);
                                }
                            }
                            
                            çıktı.Dosyalar.Add(Yeni);
                            çıktı.KapladığıAlan_bayt += Yeni.KapladığıAlan_bayt;
                        }
                    }
                }

                çıktı.Kök = KaynakZipDosyasıYolu;
                return çıktı;
            }    
            public static void Klasöre(Klasör_ ListelenmişZipDosyasıİçeriği, string HedefKlasör)
            {
                foreach (string kls in ListelenmişZipDosyasıİçeriği.Klasörler)
                {
                    Klasör.Oluştur(HedefKlasör + @"\" + kls);
                }
                Klasör.Oluştur(HedefKlasör);

                using (ZipArchive Arşiv = ZipFile.Open(ListelenmişZipDosyasıİçeriği.Kök, ZipArchiveMode.Read))
                {
                    foreach (Klasör_.İçerik_Dosya_ dsy in ListelenmişZipDosyasıİçeriği.Dosyalar)
                    {
                        ZipArchiveEntry zae = Arşiv.GetEntry(dsy.Yolu);
                        zae.ExtractToFile(HedefKlasör + @"\" + dsy.Yolu, true);
                    }
                }
            }
        }
    }

#endif