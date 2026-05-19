// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Text;

#if DEBUG
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
#endif

namespace ArgeMup.HazirKod
{
    public static class SınıfKopyalama_KoduÜreteci
    {
        #region Adım 1
        /*
        A. Kopyalanacak her sınıfın içine alttaki region eklenmeli
        B. Sınıf içindeki her bir değişken [JsonInclude], [JsonIgnore] ile işaretlenerek filtrelenir.
        C. Sınıfın kendisi partial olarak işaretlenmeli
        D. Gerekli ise IDerinKopyala_ eklenebilir

        public partial class Deneme_Sınıfı_ : ArgeMup.HazirKod.SınıfKopyalama_KoduÜreteci.IDerinKopyala_
        {
            [JsonInclude]
            public int Değişken_1 = 5;

            #region ArgeMup.HazirKod.SınıfKopyalama_KoduÜreteci
            partial void DerinKopyala_İç(object Nesne);

            public virtual Deneme_Sınıfı_ DerinKopyala()
            {
                var kopya = new Deneme_Sınıfı_();
                DerinKopyala_İç(kopya);
                return kopya;
            }
            object ArgeMup.HazirKod.SınıfKopyalama_KoduÜreteci.IDerinKopyala_.DerinKopyala_Nesne()
            {
                return DerinKopyala();
            }
            #endregion
        }

        */
        #endregion

        #region Adım 2
        /*
        A. Kodu üreten işlemi koda ekle
        B. Uygulamayı çalıştır, partial sınıfların üretildiğinden emin ol
        C. Üretilen dosyaların projeye eklendiğinden emin ol

        #if DEBUG
            ArgeMup.HazirKod.SınıfKopyalama_KoduÜreteci.Kopyalama_için_gerekli_cs_dosyalarını_üret(
                hedefKlasor: @"Uretilen\cs",
                typeof(Deneme_Sınıfı_),
                typeof(varsa Deneme_Sınıfı_ içindeki sınıf)
            );
        #endif
        */
        #endregion

        #region Adım 3
        /*
        DerinKopyala() metodu nesnenin tüm [JsonInclude] alanlarını yeni bir nesneye derin olarak kopyalar.
        Class tipi alanlar için DerinKopyala() özyinelemeli çağrılır.
        Liste/dizi ve sözlük alanları için yeni koleksiyon oluşturulur.

        Deneme_Sınıfı_ kopya = nesne.DerinKopyala();

        // veya interface üzerinden:
        ArgeMup.HazirKod.SınıfKopyalama_KoduÜreteci.IDerinKopyala_ IKopyala = nesne;
        Deneme_Sınıfı_ kopya = IKopyala.DerinKopyala_Nesne() as Deneme_Sınıfı_;
        */
        #endregion

        #region Kütüphane
        public const string Sürüm = "V1.0";
        // Bunlar debug değil iken kullanılıyor
        public interface IDerinKopyala_
        {
            object DerinKopyala_Nesne();
        }

#if DEBUG
        /// <summary>
        /// Verilen tiplerin her biri için ayrı bir .cs dosyası üretir.
        /// Dosya adı: TipAdi.DerinKopyala.g.cs
        /// </summary>
        public static void Kopyalama_için_gerekli_cs_dosyalarını_üret(string hedefKlasor, params Type[] tipler)
        {
            if (!Directory.Exists(hedefKlasor))
                Directory.CreateDirectory(hedefKlasor);

            foreach (var tip in tipler)
            {
                string kod = TamDosyaUret(tip);
                string dosyaAdi = Path.Combine(hedefKlasor, $"{tip.Name}.DerinKopyala.g.cs");
                File.WriteAllText(dosyaAdi, kod, System.Text.Encoding.UTF8);
            }
        }

        public static string TamDosyaUret(Type tip)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Otomatik üretildi — ArgeMup.HazirKod.SınıfKopyalama_KoduÜreteci tarafından");
            sb.AppendLine($"// {ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.Sürüm} {DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss.fff")}");
            sb.AppendLine("// Bu dosyayı elle düzenlemeyin.");
            sb.AppendLine();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine();

            bool nsVar = !string.IsNullOrEmpty(tip.Namespace);
            string g  = nsVar ? "    " : "";
            string g2 = g + "    ";
            string g3 = g2 + "    ";

            if (nsVar) { sb.AppendLine($"namespace {tip.Namespace}"); sb.AppendLine("{"); }

            sb.AppendLine($"{g}public partial class {tip.Name}");
            sb.AppendLine($"{g}{{");

            var alanlar = JsonIncludeAlanlariAl(tip);

            // ── DerinKopyala_İç implementasyonu ────────────────────────────────
            sb.AppendLine($"{g2}partial void DerinKopyala_İç(object Nesne)");
            sb.AppendLine($"{g2}{{");
            sb.AppendLine($"{g2}    {tip.Name} kopya = Nesne as {tip.Name};");
            sb.AppendLine($"{g2}    if (kopya == null) throw new Exception(\"Bu sınıf kopyalamak için uygun değil {tip.Name}\");");
            foreach (var alan in alanlar)
                AlanKoduYaz(alan, sb, g3);
            sb.AppendLine($"{g2}}}");

            sb.AppendLine($"{g}}}");
            if (nsVar) sb.AppendLine("}");

            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════════════
        // Alan yönlendiriciler
        // Sıralama: SozlukMu → ListeMi → SinifMi → basit tip
        // (List<T> IsClass=true olduğundan SinifMi önce gelirse yanlış dala düşer)
        // ════════════════════════════════════════════════════════════════

        private static void AlanKoduYaz(FieldInfo alan, StringBuilder sb, string g)
        {
            Type tip       = alan.FieldType;
            string ad      = alan.Name;
            Type altTip    = Nullable.GetUnderlyingType(tip);
            Type gercekTip = altTip ?? tip;

            if      (SozlukMu(gercekTip)) SozlukKoduYaz(ad, gercekTip, sb, g);
            else if (ListeMi(gercekTip))  ListeKoduYaz(ad, gercekTip, sb, g);
            else if (SinifMi(gercekTip))  SinifKoduYaz(ad, sb, g);
            else                          BasitTipKoduYaz(ad, sb, g);
        }

        // ════════════════════════════════════════════════════════════════
        // Kod yazıcılar
        // ════════════════════════════════════════════════════════════════

        // ── Basit tip ─────────────────────────────────────────────────────
        // int, bool, string, double, enum, DateTime, Guid vb. — doğrudan atama yeterli

        private static void BasitTipKoduYaz(string ad, StringBuilder sb, string g)
        {
            sb.AppendLine($"{g}kopya.{ad} = this.{ad};");
        }

        // ── Sınıf tipi ────────────────────────────────────────────────────
        // DerinKopyala() özyinelemeli çağrılır; null ise null kalır

        private static void SinifKoduYaz(string ad, StringBuilder sb, string g)
        {
            sb.AppendLine($"{g}kopya.{ad} = this.{ad}?.DerinKopyala();");
        }

        // ── Liste / Dizi ──────────────────────────────────────────────────
        // Eleman basit tip ise yeni koleksiyon — eleman class ise DerinKopyala() ile seç

        private static void ListeKoduYaz(string ad, Type gercekTip, StringBuilder sb, string g)
        {
            Type elemanTip = ListeElemanTipiniAl(gercekTip);
            if (elemanTip == null)
            {
                sb.AppendLine($"{g}// {ad}: eleman tipi belirlenemedi, atlandı");
                return;
            }

            string eTipAdi     = CSharpTipAdi(elemanTip);
            bool   elemanSinif = SinifMi(elemanTip);

            if (gercekTip.IsArray)
            {
                // Dizi
                if (elemanSinif)
                    sb.AppendLine($"{g}kopya.{ad} = this.{ad} != null ? this.{ad}.Select(x => x?.DerinKopyala()).ToArray() : null;");
                else
                    sb.AppendLine($"{g}kopya.{ad} = this.{ad} != null ? ({eTipAdi}[])this.{ad}.Clone() : null;");
            }
            else
            {
                // List<T>, IList<T> vb.
                if (elemanSinif)
                    sb.AppendLine($"{g}kopya.{ad} = this.{ad} != null ? this.{ad}.Cast<{eTipAdi}>().Select(x => x?.DerinKopyala()).ToList() : null;");
                else
                    sb.AppendLine($"{g}kopya.{ad} = this.{ad} != null ? new List<{eTipAdi}>(this.{ad}.Cast<{eTipAdi}>()) : null;");
            }
        }

        // ── Sözlük ────────────────────────────────────────────────────────
        // Value basit tip ise kopya constructor — value class ise DerinKopyala() ile yeni sözlük

        private static void SozlukKoduYaz(string ad, Type gercekTip, StringBuilder sb, string g)
        {
            var  args     = gercekTip.GetGenericArguments();
            Type keyTip   = args.Length >= 1 ? args[0] : typeof(object);
            Type valTip   = args.Length >= 2 ? args[1] : typeof(object);
            Type valAlt   = Nullable.GetUnderlyingType(valTip) ?? valTip;
            bool valSinif = SinifMi(valAlt);

            string kTipAdi = CSharpTipAdi(keyTip);
            string vTipAdi = CSharpTipAdi(valTip);

            if (valSinif)
                sb.AppendLine($"{g}kopya.{ad} = this.{ad} != null ? this.{ad}.ToDictionary(kv => kv.Key, kv => kv.Value?.DerinKopyala()) : null;");
            else
                sb.AppendLine($"{g}kopya.{ad} = this.{ad} != null ? new Dictionary<{kTipAdi}, {vTipAdi}>(this.{ad}) : null;");
        }

        // ════════════════════════════════════════════════════════════════
        // Yardımcı metotlar  (SınıfKarşılaştırma_KoduÜreteci ile aynı mantık)
        // ════════════════════════════════════════════════════════════════

        private static List<FieldInfo> JsonIncludeAlanlariAl(Type tip) =>
            tip.GetFields(BindingFlags.Public | BindingFlags.Instance)
               .Where(f =>  f.IsDefined(typeof(JsonIncludeAttribute), inherit: false)
                         && !f.IsDefined(typeof(JsonIgnoreAttribute),  inherit: false))
               .ToList();

        private static bool SozlukMu(Type t) => typeof(IDictionary).IsAssignableFrom(t);

        private static bool ListeMi(Type t) =>
            t.IsArray || (typeof(IEnumerable).IsAssignableFrom(t) && t != typeof(string));

        // Liste ve sözlük hariç tutulur — her ikisi de IsClass=true olduğundan
        private static bool SinifMi(Type t) =>
            t.IsClass && t != typeof(string) && !t.IsArray
            && !typeof(IDictionary).IsAssignableFrom(t)
            && !typeof(IEnumerable).IsAssignableFrom(t);

        private static Type ListeElemanTipiniAl(Type t)
        {
            if (t.IsArray) return t.GetElementType();
            var args = t.GetGenericArguments();
            return args.Length > 0 ? args[0] : typeof(object);
        }

        private static string CSharpTipAdi(Type t)
        {
            if (t == typeof(int))      return "int";
            if (t == typeof(string))   return "string";
            if (t == typeof(bool))     return "bool";
            if (t == typeof(double))   return "double";
            if (t == typeof(float))    return "float";
            if (t == typeof(long))     return "long";
            if (t == typeof(decimal))  return "decimal";
            if (t == typeof(DateTime)) return "DateTime";
            if (t == typeof(Guid))     return "Guid";
            return t.Name.Split('`')[0];
        }

        private static string Temizle(string s) =>
            new string(s.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
#endif
        #endregion
    }
}
