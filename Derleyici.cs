// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;

namespace ArgeMup.HazirKod
{
    public class Derleyici_ : IDisposable
    {
        class Derleyici_Bir_İşlem_
        {
            public MethodInfo Metod = null;
            public object Örnek = null;
        }

        Assembly DerlenenDll = null;
        string DerlenenDll_DosyaYolu = "";
        Dictionary<string, Derleyici_Bir_İşlem_> İşlemler = null;

        public Derleyici_(string[] KaynakDosyaYolları, string[] Kardeşleri = null, string ÇıktıKlasörü = null)
        {
            #region Çıktı Klasörü
            if (string.IsNullOrEmpty(ÇıktıKlasörü))
            {
                ÇıktıKlasörü = AppDomain.CurrentDomain.BaseDirectory;
            }
            else ÇıktıKlasörü = ÇıktıKlasörü.TrimEnd('\\') + "\\";
            Directory.CreateDirectory(ÇıktıKlasörü);
            #endregion

            #region cs dosyalarının birleştirilmesi
            DerlenenDll_DosyaYolu = ÇıktıKlasörü + "_" + Path.GetRandomFileName().Replace(".", "") + "_.dll";
            string[] Kodlar = new string[KaynakDosyaYolları.Length];
            for (int i = 0; i < Kodlar.Length; i++) Kodlar[i] = File.ReadAllText(KaynakDosyaYolları[i]);
            #endregion

            #region Kardeşlerin dahil edilmesi
            int KardeşAdet = 1;
            if (Kardeşleri != null) KardeşAdet += Kardeşleri.Length;
            string[] YeniKardeşleri = new string[KardeşAdet];
            YeniKardeşleri[0] = AppDomain.CurrentDomain.FriendlyName;
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
            string Hatalar = "";

            if (results.Errors.Count > 0)
            {
                for (int i = 0; i < results.Errors.Count; i++)
                {
                    string hata = "Satır " + results.Errors[i].Line + ", " + results.Errors[i].Column + " " + results.Errors[i].ErrorNumber + " " + results.Errors[i].ErrorText;
                    Hatalar += hata + "|";
                }
            }

            provider.Dispose();
            if (!string.IsNullOrEmpty(Hatalar)) throw new Exception(Hatalar);
            #endregion

            DerlenenDll = AppDomain.CurrentDomain.Load(File.ReadAllBytes(DerlenenDll_DosyaYolu));
            İşlemler = new Dictionary<string, Derleyici_Bir_İşlem_>();
        }
        public string[] Listele_AlanAdıVeSınıf()
        {
            List<string> Liste = new List<string>();

            Type[] tipler = DerlenenDll.GetTypes();
            foreach (var biri in tipler)
            {
                Liste.Add(biri.FullName);
            }

            return Liste.ToArray();
        }
        public string[] Listele_İşlem(string AlanAdıVeSınıf)
        {
            List<string> Liste = new List<string>();

            MethodInfo[] tipler = DerlenenDll.GetType(AlanAdıVeSınıf).GetMethods();
            foreach (var biri in tipler)
            {
                string detay = biri.ReturnType.ToString() + "|";
                detay += biri.Name + "|";
                foreach (var par in biri.GetParameters())
                {
                    detay += par.ParameterType.ToString() + " " + par.Name + "|";
                }

                Liste.Add(detay.Trim('|'));
            }

            return Liste.ToArray();
        }
        public object Çağır(string AlanAdıVeSınıf, string İşlem, object[] Parametreler = null, int ZamanAşımı = 0)
        {
            object çıktı = null;
            Derleyici_Bir_İşlem_ Kopyası = null;

            if (!İşlemler.TryGetValue(AlanAdıVeSınıf + İşlem, out Kopyası))
            {
                Kopyası = new Derleyici_Bir_İşlem_();

                Type tipi = DerlenenDll.GetType(AlanAdıVeSınıf);
                if (tipi == null) throw new Exception("AlanAdıVeSınıf bulunamadı");

                Kopyası.Metod = tipi.GetMethod(İşlem);
                if (tipi == null) throw new Exception("İşlem bulunamadı");

                Kopyası.Örnek = Activator.CreateInstance(tipi);

                İşlemler[AlanAdıVeSınıf + İşlem] = Kopyası;
            }

            if (ZamanAşımı <= 0) çıktı = Kopyası.Metod.Invoke(Kopyası.Örnek, Parametreler);
            else
            {
                Thread görev = new Thread(delegate () 
                {
                    çıktı = Kopyası.Metod.Invoke(Kopyası.Örnek, Parametreler);
                });
                görev.Start();

                ZamanAşımı = Environment.TickCount + ZamanAşımı;
                while (ZamanAşımı > Environment.TickCount && görev.IsAlive) { Thread.Sleep(1); }
                if (görev.IsAlive)
                {
                    görev.Abort();
                    throw new Exception("İşlem tamamlanmadan durduruldu");
                }
            }

            return çıktı;
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

                    DerlenenDll = null;

                    if (File.Exists(DerlenenDll_DosyaYolu)) File.Delete(DerlenenDll_DosyaYolu);

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
        ~Derleyici_()
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