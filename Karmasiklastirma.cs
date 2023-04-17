// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.IO;
using System.Security.Cryptography;
using ArgeMup.HazirKod.Dönüştürme;

namespace ArgeMup.HazirKod
{
    public class DahaCokKarmasiklastirma_ : IDisposable
    {
        public const string Sürüm = "V1.2";
        public string Karıştır(string Girdi, string Parola)
        {
            return D_HexYazı.BaytDizisinden(Karıştır(D_Yazı.BaytDizisine(Girdi), D_Yazı.BaytDizisine(Parola)));
        }
        public byte[] Karıştır(byte[] Girdi, byte[] Parola)
        {
            try
            {
                Aes aes = new AesManaged();
                aes.IV = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Parola, aes.IV);
                MemoryStream ms = new MemoryStream();

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                aes.IV = pdb.GetBytes(aes.BlockSize / 8);

                CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(Girdi, 0, Girdi.Length);
                cs.Close();
                return ms.ToArray();
            }
            catch (Exception) { }
            return null;
        }

        public string Düzelt(string Girdi, string Parola)
        {
            return D_Yazı.BaytDizisinden(Düzelt(D_HexYazı.BaytDizisine(Girdi), D_Yazı.BaytDizisine(Parola)));
        }
        public byte[] Düzelt(byte[] Girdi, byte[] Parola)
        {
            try
            {
                Aes aes = new AesManaged();
                aes.IV = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

                PasswordDeriveBytes pdb = new PasswordDeriveBytes(Parola, aes.IV);
                MemoryStream ms = new MemoryStream();

                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                aes.Key = pdb.GetBytes(aes.KeySize / 8);
                aes.IV = pdb.GetBytes(aes.BlockSize / 8);

                CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(Girdi, 0, Girdi.Length);
                cs.Close();
                return ms.ToArray();
            }
            catch (Exception) { }
            return null;
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
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DahaCokKarmasiklastirma_() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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

    public class DahaCokKarmasiklastirma_Asimetrik_ : IDisposable
    {
        public const string Sürüm = "V1.0";
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

        RSACryptoServiceProvider rsa = null;
        RandomNumberGenerator rnd = null;
        DahaCokKarmasiklastirma_ dçk = null;

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
                byte[] şifre = ParolaÜret();

                if (dçk == null) dçk = new DahaCokKarmasiklastirma_();
                byte[] şifreli = dçk.Karıştır(Girdi, şifre);

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

                if (dçk == null) dçk = new DahaCokKarmasiklastirma_();
                byte[] çıktı = dçk.Düzelt(şifreli, şifre);
                return çıktı;
            }
        }

        public byte[] ParolaÜret(int KarakterSayısı_Bayt = AesParolaKarakterSayısı)
        {
            byte[] Parola = new byte[KarakterSayısı_Bayt];

            if (rnd == null) rnd = RNGCryptoServiceProvider.Create();
            rnd.GetNonZeroBytes(Parola);

            return Parola;
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
                    rnd?.Dispose();
                    dçk?.Dispose();
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

        /// <summary>
        /// Sha256 Oluşturucu
        /// </summary>
        /// <param name="ÇıktıKarakterSayısı">Geçersiz, Sadece uyumluluk için</param>
        /// <returns></returns>
        public static byte[] BaytDizisinden(byte[] Dizi, int ÇıktıKarakterSayısı = 32)
        {
            return new SHA256Managed().ComputeHash(Dizi);
        }

        public static string Yazıdan(string Girdi, int ÇıktıKarakterSayısı = 32)
        {
            byte[] Dizi1 = D_Yazı.BaytDizisine(Girdi);
            byte[] Dizi2 = BaytDizisinden(Dizi1, ÇıktıKarakterSayısı);
            return D_HexYazı.BaytDizisinden(Dizi2);
        }
    }
}

