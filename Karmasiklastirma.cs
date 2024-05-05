// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.IO;
using System.Security.Cryptography;
using ArgeMup.HazirKod.Dönüştürme;
using ArgeMup.HazirKod.Ekİşlemler;

namespace ArgeMup.HazirKod
{
    public static class DahaCokKarmasiklastirma
    {
        public const string Sürüm = "V1.2";
        public static string Karıştır(string Girdi, string Parola)
        {
            return D_HexYazı.BaytDizisinden(Karıştır(D_Yazı.BaytDizisine(Girdi), D_Yazı.BaytDizisine(Parola)));
        }
        public static byte[] Karıştır(byte[] Girdi, byte[] Parola)
        {
            byte[] çıktı;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Parola, aesAlg.IV);
                aesAlg.Key = pdb.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = pdb.GetBytes(aesAlg.BlockSize / 8);

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
	                {
	                    csEncrypt.Write(Girdi, 0, Girdi.Length);
	                }
	                çıktı = msEncrypt.ToArray();
	            }
            }

            return çıktı;
        }

        public static string Düzelt(string Girdi, string Parola)
        {
            return D_Yazı.BaytDizisinden(Düzelt(D_HexYazı.BaytDizisine(Girdi), D_Yazı.BaytDizisine(Parola)));
        }
        public static byte[] Düzelt(byte[] Girdi, byte[] Parola)
        {
            byte[] çıktı = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.IV = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Parola, aesAlg.IV);
                aesAlg.Key = pdb.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = pdb.GetBytes(aesAlg.BlockSize / 8);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream(Girdi))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                    {
		                using (MemoryStream ms = new MemoryStream())
		                {
		                    csEncrypt.CopyTo(ms);
		                    çıktı = ms.ToArray();
		                }
		            }
                }
            }

            return çıktı;
        }
    }

    public class DahaCokKarmasiklastirma_Asimetrik_ : IDisposable
    {
        public const string Sürüm = "V1.1";
        public static void AnahtarÜret(out string SadeceAçıkAnahtar, out string AçıkVeGizliAnahtarBirlikte, int AnahtarUzunluğu_Bit = 2048)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(AnahtarUzunluğu_Bit);
            rsa.PersistKeyInCsp = false;

            SadeceAçıkAnahtar = rsa.ToXmlString(false);
            AçıkVeGizliAnahtarBirlikte = rsa.ToXmlString(true);
        }

        readonly int AzamiKarakterSayısı;
        readonly int RsaParolaKarakterSayısı;
        const int AesParolaKarakterSayısı = 32; //aes anahtar uzunluğu 256 bit
        const int RsaPKCSKarakterSayısı = 11; //https://www.rfc-editor.org/rfc/rfc3447#section-7.2.1

        RSACryptoServiceProvider rsa;

        /// <summary>
        /// Yaslama -> PKCS#1 V1.5
        /// </summary>
        public DahaCokKarmasiklastirma_Asimetrik_(string Anahtar)
        {
            rsa = new RSACryptoServiceProvider();
            rsa.PersistKeyInCsp = false;
            rsa.FromXmlString(Anahtar);

            RsaParolaKarakterSayısı = rsa.KeySize / 8;
            AzamiKarakterSayısı = RsaParolaKarakterSayısı - RsaPKCSKarakterSayısı;
        }
        public DahaCokKarmasiklastirma_Asimetrik_(int AnahtarUzunluğu_Bit = 2048)
        {
            rsa = new RSACryptoServiceProvider(AnahtarUzunluğu_Bit);
            rsa.PersistKeyInCsp = false;

            RsaParolaKarakterSayısı = rsa.KeySize / 8;
            AzamiKarakterSayısı = RsaParolaKarakterSayısı - RsaPKCSKarakterSayısı;
        }

        public byte[] Karıştır(byte[] Girdi)
        {
            if (Girdi.Length <= AzamiKarakterSayısı) return rsa.Encrypt(Girdi, false);
            else
            {
                byte[] şifre = Rastgele.BaytDizisi(AesParolaKarakterSayısı);

                byte[] şifreli = DahaCokKarmasiklastirma.Karıştır(Girdi, şifre);

                int rsa_paketi_boş_alan = AzamiKarakterSayısı - AesParolaKarakterSayısı;
                byte[] rsa_paketi = new byte[AzamiKarakterSayısı];
                Array.Copy(şifre, 0, rsa_paketi, 0, AesParolaKarakterSayısı);
                Array.Copy(şifreli, 0, rsa_paketi, AesParolaKarakterSayısı, rsa_paketi_boş_alan);
                byte[] rsa_paketi_şifreli = rsa.Encrypt(rsa_paketi, false);

                byte[] çıktı = new byte[AesParolaKarakterSayısı + RsaPKCSKarakterSayısı + şifreli.Length];
                Array.Copy(rsa_paketi_şifreli, 0, çıktı, 0, rsa_paketi_şifreli.Length);
                Array.Copy(şifreli, rsa_paketi_boş_alan, çıktı, rsa_paketi_şifreli.Length, şifreli.Length - rsa_paketi_boş_alan);
                return çıktı;
            }
        }
        public byte[] Düzelt(byte[] Girdi)
        {
            if (rsa.PublicOnly) throw new Exception("Girilen anahtar düzeltemez");

            if (Girdi.Length <= RsaParolaKarakterSayısı) return rsa.Decrypt(Girdi, false);
            else
            {
                byte[] rsa_paketi_şifreli = new byte[RsaParolaKarakterSayısı];
                Array.Copy(Girdi, 0, rsa_paketi_şifreli, 0, RsaParolaKarakterSayısı);
                byte[] rsa_paketi = rsa.Decrypt(rsa_paketi_şifreli, false);

                byte[] şifre = new byte[AesParolaKarakterSayısı];
                Array.Copy(rsa_paketi, 0, şifre, 0, AesParolaKarakterSayısı);

                int rsa_paketi_boş_alan = rsa_paketi.Length - AesParolaKarakterSayısı;
                int rsa_paketi_dışında_kalan = Girdi.Length - RsaParolaKarakterSayısı;
                byte[] şifreli = new byte[rsa_paketi_boş_alan + rsa_paketi_dışında_kalan];
                Array.Copy(rsa_paketi, AesParolaKarakterSayısı, şifreli, 0, rsa_paketi_boş_alan);
                Array.Copy(Girdi, RsaParolaKarakterSayısı, şifreli, rsa_paketi_boş_alan, rsa_paketi_dışında_kalan);

                byte[] çıktı = DahaCokKarmasiklastirma.Düzelt(şifreli, şifre);
                return çıktı;
            }
        }

        public byte[] İmzala(byte[] Belge)
        {
            return rsa.SignData(Belge, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
        public byte[] İmzala(Stream Belge)
        {
            return rsa.SignData(Belge, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
        public bool İmzaGeçerliMi(byte[] Belge, byte[] İmza)
        {
            return rsa.VerifyData(Belge, İmza, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }
        public bool İmzaGeçerliMi(Stream Belge, byte[] İmza)
        {
            return rsa.VerifyData(Belge, İmza, HashAlgorithmName.SHA512, RSASignaturePadding.Pkcs1);
        }

        public bool GizliAnahtarMevcutMu
        {
            get { return !rsa.PublicOnly; }
        }
        public string AçıkAnahtar
        {
            get { return rsa.ToXmlString(false); }
        }
        public string AçıkVeGizliAnahtarlar
        {
            get { return rsa.ToXmlString(true); }
        }
        public int AnahtarGücü_Bit
        {
            get { return rsa.KeySize; }
        }

        #region IDisposable
        private bool disposedValue;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    rsa?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DahaCokKarmasiklastirma_Asimetrik_()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}

namespace ArgeMup.HazirKod.Dönüştürme
{
    public static class D_GeriDönülemezKarmaşıklaştırmaMetodu
    {
        public const string Sürüm = "V1.0";

        public static byte[] BaytDizisinden(byte[] Dizi, int ÇıktıKarakterSayısı = 32)
        {
            #if NET7_0_OR_GREATER
                return ÇıktıKarakterSayısı == 64 ? SHA512.HashData(Dizi) : ÇıktıKarakterSayısı == 32 ? SHA256.HashData(Dizi) : throw new NotSupportedException();
            #else
                byte[] çıktı;
                if (ÇıktıKarakterSayısı == 64)
                {
                    using (var SHA512 = System.Security.Cryptography.SHA512.Create())
                    {
                        çıktı = SHA512.ComputeHash(Dizi);
                    }
                }
                else if (ÇıktıKarakterSayısı == 32)
                {
                    using (var SHA256 = System.Security.Cryptography.SHA256.Create())
                    {
                        çıktı = SHA256.ComputeHash(Dizi);
                    }
                }
                else throw new NotSupportedException();
                return çıktı;
            #endif
        }

        public static byte[] Akıştan(Stream Akış, int ÇıktıKarakterSayısı = 32)
        {
            #if NET7_0_OR_GREATER
                return ÇıktıKarakterSayısı == 64 ? SHA512.HashData(Akış) : ÇıktıKarakterSayısı == 32 ? SHA256.HashData(Akış) : throw new NotSupportedException();
            #else
                byte[] çıktı;
                if (ÇıktıKarakterSayısı == 64)
                {
                    using (var SHA512 = System.Security.Cryptography.SHA512.Create())
                    {
                        çıktı = SHA512.ComputeHash(Akış);
                    }
                }
                else if (ÇıktıKarakterSayısı == 32)
                {
                    using (var SHA256 = System.Security.Cryptography.SHA256.Create())
                    {
                        çıktı = SHA256.ComputeHash(Akış);
                    }
                }
                else throw new NotSupportedException();
                return çıktı;
            #endif
        }

        public static string Yazıdan(string Girdi, int ÇıktıKarakterSayısı = 32)
        {
            byte[] Dizi1 = D_Yazı.BaytDizisine(Girdi);
            byte[] Dizi2 = BaytDizisinden(Dizi1, ÇıktıKarakterSayısı);
            return D_HexYazı.BaytDizisinden(Dizi2);
        }
    }
}

