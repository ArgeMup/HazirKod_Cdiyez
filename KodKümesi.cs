// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public class KodKümesi_ : IDisposable
    {
    	public string Sürüm = "V1.0";
        public delegate void KodKümesi_GeriBildirim_(object Çıktı, object Hatırlatıcı, string HataMesajı);
        
        class KodKümesi_İşlem_
        {
            public MethodInfo Metod = null;
            public object Örnek = null;
        }

        Assembly DerlenenKodKümesi = null;
        Dictionary<string, KodKümesi_İşlem_> İşlemler = null;

        public KodKümesi_(string[] DerlenecekDosyalarVeyaKaynakKod, string ÇıktıKlasörü = null, string[] Kardeşleri = null)
        {
            #region Çıktı Klasörü
            if (string.IsNullOrEmpty(ÇıktıKlasörü)) ÇıktıKlasörü = AppDomain.CurrentDomain.BaseDirectory;
            ÇıktıKlasörü = ÇıktıKlasörü.TrimEnd('\\') + "\\";
            Directory.CreateDirectory(ÇıktıKlasörü);
            #endregion

            #region cs dosyalarının birleştirilmesi
            string DerlenenDll_DosyaYolu = ÇıktıKlasörü + "_" + Path.GetRandomFileName().Replace(".", "") + "_.dll";
            string[] Kodlar = new string[DerlenecekDosyalarVeyaKaynakKod.Length];
            for (int i = 0; i < Kodlar.Length; i++)
            {
                if (File.Exists(DerlenecekDosyalarVeyaKaynakKod[i])) Kodlar[i] = File.ReadAllText(DerlenecekDosyalarVeyaKaynakKod[i]);
                else Kodlar[i] = DerlenecekDosyalarVeyaKaynakKod[i];
            }
            #endregion

            #region Kardeşlerin dahil edilmesi
            int KardeşAdet = 1;
            if (Kardeşleri != null) KardeşAdet += Kardeşleri.Length;
            string[] YeniKardeşleri = new string[KardeşAdet];
            YeniKardeşleri[0] = AppDomain.CurrentDomain.BaseDirectory + AppDomain.CurrentDomain.FriendlyName;
            if (Kardeşleri != null)
            {
                for (int i = 0; i < Kardeşleri.Length; i++) YeniKardeşleri[1 + i] = Kardeşleri[i];
            }
            #endregion

            #region Derleme
            CompilerParameters options = new CompilerParameters()
            {
                GenerateExecutable = false,
                OutputAssembly = DerlenenDll_DosyaYolu,
                GenerateInMemory = false
            };
            options.ReferencedAssemblies.AddRange(YeniKardeşleri);

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerResults results = provider.CompileAssemblyFromSource(options, Kodlar);
            #endregion

            #region Hata durumu incelemesi
            if (results.Errors.Count > 0)
            {
                List<string> hatalar = new List<string>();

                foreach (CompilerError biri in results.Errors)
                {
                    string hata = "Satır " + biri.Line + ", " + biri.Column + " " + biri.ErrorNumber + " " + biri.ErrorText + " " + biri.FileName;
                    hatalar.Add(hata);
                }
                File.WriteAllLines(ÇıktıKlasörü + "Derleme Hataları.txt", hatalar.ToArray());

                throw new Exception("Derleme Hatalı, Derleme Hataları.txt dosyasına bakınız");
            }
            else if (File.Exists(ÇıktıKlasörü + "Derleme Hataları.txt")) File.Delete(ÇıktıKlasörü + "Derleme Hataları.txt");
            #endregion

            DerlenmişKodKümesiDosyasınıAç(DerlenenDll_DosyaYolu);
        }
        public KodKümesi_(string DerlenmişKodKümesiDosyası)
        {
            DerlenmişKodKümesiDosyasınıAç(DerlenmişKodKümesiDosyası);
        }
        void DerlenmişKodKümesiDosyasınıAç(string DosyaYolu)
        {
            byte[] içeriği = File.ReadAllBytes(DosyaYolu);
            DerlenenKodKümesi = AppDomain.CurrentDomain.Load(içeriği);

            İşlemler = new Dictionary<string, KodKümesi_İşlem_>();
        }
        public string[] Listele_AlanAdıVeSınıf(bool Tümünü = false)
        {
            List<string> Liste = new List<string>();

            Type[] tipler = DerlenenKodKümesi.GetTypes();
            foreach (var biri in tipler)
            {
                if (Tümünü || biri.IsClass) Liste.Add(biri.FullName);
            }

            return Liste.ToArray();
        }
        public string[] Listele_İşlem(string AlanAdıVeSınıf, bool Tümünü = false)
        {
            List<string> Liste = new List<string>();

            MethodInfo[] tipler = DerlenenKodKümesi.GetType(AlanAdıVeSınıf).GetMethods();
            foreach (var biri in tipler)
            {
                if (Tümünü || biri.DeclaringType.FullName.StartsWith(AlanAdıVeSınıf))
                {
                    string detay = biri.ReturnType.ToString() + "|";
                    detay += biri.Name + "|";
                    foreach (var par in biri.GetParameters())
                    {
                        detay += par.ParameterType.ToString() + " " + par.Name + "|";
                    }

                    Liste.Add(detay.Trim('|'));
                }
            }

            return Liste.ToArray();
        }
        public object Çağır(string AlanAdıVeSınıf, string İşlem, object[] Parametreler = null)
        {
            if (!İşlemler.TryGetValue(AlanAdıVeSınıf + İşlem, out KodKümesi_İşlem_ Kopyası))
            {
                Kopyası = new KodKümesi_İşlem_();

                Type tipi = DerlenenKodKümesi.GetType(AlanAdıVeSınıf);
                if (tipi == null) throw new Exception("AlanAdıVeSınıf ( " + AlanAdıVeSınıf + " ) bulunamadı");

                Kopyası.Metod = tipi.GetMethod(İşlem);
                if (Kopyası.Metod == null) throw new Exception("İşlem ( " + AlanAdıVeSınıf + "." + İşlem + " ) bulunamadı");

                Kopyası.Örnek = Activator.CreateInstance(tipi);
                if (Kopyası.Örnek == null) throw new Exception("İşlemin ( " + AlanAdıVeSınıf + "." + İşlem + " ) örneği edinilemedi");

                İşlemler[AlanAdıVeSınıf + İşlem] = Kopyası;
            }

            return Kopyası.Metod.Invoke(Kopyası.Örnek, Parametreler);
        }
        public Thread Çağır(string AlanAdıVeSınıf, string İşlem, object[] Parametreler, KodKümesi_GeriBildirim_ GeriBildirim = null, object Hatrırlatıcı = null)
        {
            Thread görev = new Thread(delegate () 
            {
                object çıktı = null;
                string HataMesajı = "";

                try
                {
                    çıktı = Çağır(AlanAdıVeSınıf, İşlem, Parametreler);
                }
                catch (Exception ex) 
                { 
                    while (ex != null)
                    {
                        HataMesajı += '|' + ex.Message;
                        ex = ex.InnerException;
                    }
                }

                GeriBildirim?.Invoke(çıktı, Hatrırlatıcı, HataMesajı);
            });
            görev.Start();

            return görev;
        }
        public object Çağır(string AlanAdıVeSınıf, string İşlem, object[] Parametreler, int ZamanAşımı = 0)
        {
            object Çıktı_iç = null;
            string HataMesajı_iç = "";

            Thread Görev = Çağır(AlanAdıVeSınıf, İşlem, Parametreler, (object Çıktı, object Hatırlatıcı, string HataMesajı) => 
            {
                Çıktı_iç = Çıktı;
                HataMesajı_iç = HataMesajı;
            });

            ZamanAşımı = Environment.TickCount + ZamanAşımı;
            while (ZamanAşımı > Environment.TickCount && Görev.IsAlive) { Thread.Sleep(1); }
            if (Görev.IsAlive)
            {
                Görev.Abort();
                throw new Exception(AlanAdıVeSınıf + "." + İşlem + " zaman aşımı sebebiyle durduruldu");
            }

            if (!string.IsNullOrEmpty(HataMesajı_iç))
            {
                throw new Exception(AlanAdıVeSınıf + "." + İşlem + " iç hata oluştu" + HataMesajı_iç);
            }

            return Çıktı_iç;
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

                    DerlenenKodKümesi = null;

                    if (İşlemler != null)
                    {
                        İşlemler.Clear();
                        İşlemler = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~KodKümesi_()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

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
}