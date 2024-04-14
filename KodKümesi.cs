// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_Derleyici
// Nuget Microsoft.CodeAnalysis.Scripting
#warning TEHLİKE - Kullanılmayan derlemeler atılamadığından ram taşması ihtimali - https://github.com/dotnet/roslyn/issues/41722

using System;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using ArgeMup.HazirKod.Ekİşlemler;

namespace ArgeMup.HazirKod
{
    public class KodKümesi_İşlem_
    {
        public string Sürüm = "V1.0";

        Dictionary<string, Microsoft.CodeAnalysis.Scripting.ScriptRunner<object>> İşlemler = new Dictionary<string, Microsoft.CodeAnalysis.Scripting.ScriptRunner<object>>();

        public void Derle(string TakmaAdı, string KodVeyaDosyaYolu, Type AracılıkYapacakOlanNesne_Tipi = null, string[] Kardeşleri = null)
        {
            if (TakmaAdı.BoşMu(true)) throw new Exception("TakmaAdı boş olamaz");
            else if (KodVeyaDosyaYolu.BoşMu(true)) throw new Exception("KodVeyaDosyaYolu boş olamaz");

            if (Kardeşleri == null) Kardeşleri = new string[] { Kendi.DosyaYolu };

            Microsoft.CodeAnalysis.Scripting.ScriptOptions DerlemeSeçenekleri = Microsoft.CodeAnalysis.Scripting.ScriptOptions.Default
                    .WithAllowUnsafe(false)
                    .WithCheckOverflow(true)
                    .WithEmitDebugInformation(false)
                    .WithFileEncoding(System.Text.Encoding.UTF8)
                    .WithOptimizationLevel(Microsoft.CodeAnalysis.OptimizationLevel.Release)
                    .WithReferences(Kardeşleri);

            if (File.Exists(KodVeyaDosyaYolu)) KodVeyaDosyaYolu = KodVeyaDosyaYolu.DosyaYolu_Oku_Yazı(); //Dosya içeriğini al

            Microsoft.CodeAnalysis.Scripting.Script<object> DerlenmişKod_önişlem = Microsoft.CodeAnalysis.CSharp.Scripting.CSharpScript.Create(KodVeyaDosyaYolu, DerlemeSeçenekleri, AracılıkYapacakOlanNesne_Tipi);
            Microsoft.CodeAnalysis.Scripting.ScriptRunner<object> DerlenmişKod = DerlenmişKod_önişlem.CreateDelegate();

            if (İşlemler.ContainsKey(TakmaAdı)) İşlemler[TakmaAdı] = DerlenmişKod;
            else İşlemler.Add(TakmaAdı, DerlenmişKod);
        }
        public object Çağır(string TakmaAdı, object AracılıkYapacakOlanNesne = null)
        {
            if (!İşlemler.TryGetValue(TakmaAdı, out Microsoft.CodeAnalysis.Scripting.ScriptRunner<object> Bulunan)) throw new Exception(TakmaAdı + " isimli derleme bulunamadı");

            System.Threading.Tasks.Task<object> İşlemSonucu = Bulunan(AracılıkYapacakOlanNesne);

            if (İşlemSonucu.Exception != null) throw İşlemSonucu.Exception;

            return İşlemSonucu.Result;
        }
    }

    public class KodKümesi_Dll_
    {
        public string Sürüm = "V1.2";

        class KodKümesi_Dll_İşlem_
        {
            public MethodInfo Metod = null;
            public object Örnek = null;
        }
        Assembly DerlenenKodKümesi = null;
        Dictionary<string, KodKümesi_Dll_İşlem_> İşlemler = new Dictionary<string, KodKümesi_Dll_İşlem_>();

        public KodKümesi_Dll_(string DerlenmişKodKümesiDosyası)
        {
            byte[] DerlenmişKodKümesiİçeriği = File.ReadAllBytes(DerlenmişKodKümesiDosyası);

            DerlenenKodKümesi = AppDomain.CurrentDomain.Load(DerlenmişKodKümesiİçeriği);
        }
        public KodKümesi_Dll_(byte[] DerlenmişKodKümesiİçeriği)
        {
            DerlenenKodKümesi = AppDomain.CurrentDomain.Load(DerlenmişKodKümesiİçeriği);
        }
        public KodKümesi_Dll_(Assembly DerlenmişKodKümesi)
        {
            DerlenenKodKümesi = DerlenmişKodKümesi;
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
            if (!İşlemler.TryGetValue(AlanAdıVeSınıf + İşlem, out KodKümesi_Dll_İşlem_ Kopyası))
            {
                Kopyası = new KodKümesi_Dll_İşlem_();

                Type tipi = DerlenenKodKümesi.GetType(AlanAdıVeSınıf);
                if (tipi == null) throw new Exception("AlanAdıVeSınıf ( " + AlanAdıVeSınıf + " ) bulunamadı");

                Kopyası.Metod = tipi.GetMethod(İşlem);
                if (Kopyası.Metod == null) throw new Exception("İşlem ( " + AlanAdıVeSınıf + "." + İşlem + " ) bulunamadı");

                if (!Kopyası.Metod.IsStatic)
                {
                    Kopyası.Örnek = Activator.CreateInstance(tipi);
                    if (Kopyası.Örnek == null) throw new Exception("İşlemin ( " + AlanAdıVeSınıf + "." + İşlem + " ) örneği edinilemedi");
                }
                    
                İşlemler[AlanAdıVeSınıf + İşlem] = Kopyası;
            }

            return Kopyası.Metod.Invoke(Kopyası.Örnek, Parametreler);
        }
        public Thread Çağır(string AlanAdıVeSınıf, string İşlem, object[] Parametreler, Action<object ,object ,string> GeriBildirim = null, object Hatrırlatıcı = null)
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
        #if !NET7_0_OR_GREATER
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
        #endif

        #if NET7_0_OR_GREATER
            public static byte[] Derle(string[] DerlenecekDosyalarVeyaKaynakKod, string ÇıktıDosyaAdı = null, string[] Kardeşleri = null)
            {
                if (DerlenecekDosyalarVeyaKaynakKod == null || DerlenecekDosyalarVeyaKaynakKod.Length == 0) throw new Exception("DerlenecekDosyalarVeyaKaynakKod boş olamaz");

                #region cs dosyalarının birleştirilmesi
                Microsoft.CodeAnalysis.SyntaxTree[] Kodlar = new Microsoft.CodeAnalysis.SyntaxTree[DerlenecekDosyalarVeyaKaynakKod.Length];
                for (int i = 0; i < Kodlar.Length; i++)
                {
                    //dosya yolu ise içeriğini al
                    if (File.Exists(DerlenecekDosyalarVeyaKaynakKod[i])) DerlenecekDosyalarVeyaKaynakKod[i] = File.ReadAllText(DerlenecekDosyalarVeyaKaynakKod[i]);

                    Kodlar[i] = Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree.ParseText(DerlenecekDosyalarVeyaKaynakKod[i]);
                }
                #endregion

                #region Kardeşlerin dahil edilmesi
                if (Kardeşleri == null)
                {
                    //https://github.com/dotnet/roslyn/issues/49498
                    List<string> l = new List<string>();
                    l.Add(Kendi.DosyaYolu);
                    l.Add(typeof(object).GetTypeInfo().Assembly.Location);
                    foreach (var a in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                    {
                        l.Add(Assembly.Load(a).Location);
                    }
              
                    Kardeşleri = l.ToArray();
                }
                #endregion

                #region Derleme
                Microsoft.CodeAnalysis.MetadataReference[] Kardeşler = new Microsoft.CodeAnalysis.MetadataReference[Kardeşleri.Length];
                for (int i = 0; i < Kardeşler.Length; i++)
                {
                    Kardeşler[i] = Microsoft.CodeAnalysis.MetadataReference.CreateFromFile(Kardeşleri[i]);
                }

                Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions ops = new Microsoft.CodeAnalysis.CSharp.CSharpCompilationOptions(
                    Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: Microsoft.CodeAnalysis.OptimizationLevel.Release,
                    checkOverflow:true, 
                    allowUnsafe:false);

                Microsoft.CodeAnalysis.CSharp.CSharpCompilation compilation = Microsoft.CodeAnalysis.CSharp.CSharpCompilation.Create(
                    ÇıktıDosyaAdı ?? Path.GetRandomFileName(),
                    Kodlar,
                    Kardeşler,
                    ops);
                #endregion

                byte[] Çıktı = null;
                string ÇıktıKlasörü = ( ÇıktıDosyaAdı != null ? Klasör.ÜstKlasör(ÇıktıDosyaAdı) : Kendi.Klasörü ) + "\\";
                #region Hata durumu incelemesi
                using (var ms = new MemoryStream())
                {
                    Microsoft.CodeAnalysis.Emit.EmitResult result = compilation.Emit(ms);
                    if (!result.Success)
                    {
                        List<string> hatalar = new List<string>();
                        foreach (Microsoft.CodeAnalysis.Diagnostic hata in result.Diagnostics)
                        {
                            string açıklama = hata.Severity + " " + hata.Id + " : " + hata.GetMessage();
                            hatalar.Add(açıklama);
                        }

                        File.WriteAllLines(ÇıktıKlasörü + "Derleme Hataları.txt", hatalar.ToArray());
                        throw new Exception("Derleme Hatalı, Derleme Hataları.txt dosyasına bakınız");
                    }
                    else if (File.Exists(ÇıktıKlasörü + "Derleme Hataları.txt")) File.Delete(ÇıktıKlasörü + "Derleme Hataları.txt");
                
                    ms.Seek(0, SeekOrigin.Begin);
                    Çıktı = ms.ToArray();
                    if (ÇıktıDosyaAdı != null) Çıktı.Dosyaİçeriği_Yaz(ÇıktıDosyaAdı);
                }
                #endregion

                return Çıktı;
            }
        #else
            public static Assembly Derle(string[] DerlenecekDosyalarVeyaKaynakKod, string ÇıktıDosyaAdı = null, string[] Kardeşleri = null)
            {
                if (DerlenecekDosyalarVeyaKaynakKod == null || DerlenecekDosyalarVeyaKaynakKod.Length == 0) throw new Exception("DerlenecekDosyalarVeyaKaynakKod boş olamaz");

                #region cs dosyalarının birleştirilmesi
                string[] Kodlar = new string[DerlenecekDosyalarVeyaKaynakKod.Length];
                for (int i = 0; i < Kodlar.Length; i++)
                {
                    //dosya yolu ise içeriğini al
                    if (File.Exists(DerlenecekDosyalarVeyaKaynakKod[i])) Kodlar[i] = File.ReadAllText(DerlenecekDosyalarVeyaKaynakKod[i]);
                    else Kodlar[i] = DerlenecekDosyalarVeyaKaynakKod[i];
                }
				#endregion

                #region Kardeşlerin dahil edilmesi
                if (Kardeşleri == null)
                {
                    List<string> l = new List<string>();
                    l.Add(Kendi.DosyaYolu);
                    l.Add(typeof(object).GetTypeInfo().Assembly.Location);
                    foreach (var a in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
                    {
                        l.Add(Assembly.Load(a).Location);
                    }

                    Kardeşleri = l.ToArray();
                }
                #endregion

                #region Derleme
                System.CodeDom.Compiler.CompilerParameters options = new System.CodeDom.Compiler.CompilerParameters()
                {
                    GenerateExecutable = false,
                    OutputAssembly = ÇıktıDosyaAdı,
                    GenerateInMemory = ÇıktıDosyaAdı == null,
                    IncludeDebugInformation = false,
                };
                options.ReferencedAssemblies.AddRange(Kardeşleri);

                System.CodeDom.Compiler.CodeDomProvider provider = System.CodeDom.Compiler.CodeDomProvider.CreateProvider("CSharp");
                System.CodeDom.Compiler.CompilerResults results = provider.CompileAssemblyFromSource(options, Kodlar);
                #endregion

                string ÇıktıKlasörü = (ÇıktıDosyaAdı != null ? Klasör.ÜstKlasör(ÇıktıDosyaAdı) : Kendi.Klasörü) + "\\";
                #region Hata durumu incelemesi
                if (results.Errors.Count > 0)
                {
                    List<string> hatalar = new List<string>();

                    foreach (System.CodeDom.Compiler.CompilerError biri in results.Errors)
                    {
                        string hata = "Satır " + biri.Line + ", " + biri.Column + " " + biri.ErrorNumber + " " + biri.ErrorText + " " + biri.FileName;
                        hatalar.Add(hata);
                    }

                    File.WriteAllLines(ÇıktıKlasörü + "Derleme Hataları.txt", hatalar.ToArray());
                    throw new Exception("Derleme Hatalı, Derleme Hataları.txt dosyasına bakınız");
                }
                else if (File.Exists(ÇıktıKlasörü + "Derleme Hataları.txt")) File.Delete(ÇıktıKlasörü + "Derleme Hataları.txt");
                #endregion

                return results.CompiledAssembly;
            }
        #endif
    }
}
#endif