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
    public static class SınıfKarşılaştırma_KoduÜreteci
    {
        #region Adım 1
        /*
        A.Kullanılacak her sınıfın içine alttaki region eklenmeli
        B.Sınıf içindeki her bir değişken [JsonInclude], [JsonIgnore] ile işaretlenerek filtrelenir.
        C.[JsonListKey(0)] [JsonListKey(1)] gibi birden fazla sınıf içi değişkene eklenerek dizi, liste gibi anahtarı olmayan nesnelerin karşılaştırma aşamasında düzgün eşleştirilebilmesi için eklenebilir, eklenmez ise indis sırasında kontrol edilir 
        
        #region ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci
        partial void Karşılaştır_Yazdır(Siparis B, ref string aciklama, string yolOneki);

        public virtual string Karşılaştır(Siparis B, string yolOneki = "")
        {
            string aciklama = null;
            Karşılaştır_Yazdır(B, ref aciklama, yolOneki);
            return aciklama ?? string.Empty;
        }
        #endregion

        */
        #endregion

        #region Adım 2
        /*
        A.Kodu üreten işlemi koda ekle
        B.Uygulamayı çalıştır, partial sınıfların üretildiğinden emin ol
        C.Üretilen dosyaların projeye eklendiğinden emin ol

        #if DEBUG
            ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.Karşılaştırma_için_gerekli_cs_dosyalarını_üret(
                hedefKlasor: @"Uretilen\cs",
                typeof(Siparis),
                typeof(Musteri),
                typeof(Kalem)
            );
        #endif
        */
        #endregion

        #region Adım 3
        /*
        string sonuç = NesneA.Karşılaştır(NesneB);
        işlemi ile aynı tip iki nesne arasındaki tüm farklar, tüm iç elemanlarıyla birlikte karşılaştırılır
        */
        #endregion

        #region Çıktı
        /*
        Değişken adı:Eski:Yeni

        Kullanıcı:null:Dalguş
        Türü:null:Gıcık kuş
        Gıcıklık[Fiziksel]
         Isırır_Mı:isterse:istersen
         Çirkin_Mi:true:çok
        Gıcıklık[Duysal]
         Ötüş:Gıcık:işte
        */
        #endregion

        #region Kütüphane
        //Bunlar debug değil iken kullanılıyor
        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
        public class JsonListKeyAttribute : Attribute
        {
            public int Order { get; }
            public JsonListKeyAttribute(int order = 0) { Order = order; }
        }

        public static string DegerStr(object v)
        {
            if (v == null) return "null";
            return v.ToString();
        }
        public static void DeğişkenAdınıOluştur(StringBuilder sb, ref string oncekiKok, string prefix, string segment, string eski, string yeni)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                oncekiKok = null;
                sb.AppendLine(segment + ":" + eski + ":" + yeni);
            }
            else
            {
                if (prefix != oncekiKok)
                {
                    sb.AppendLine(prefix);
                    oncekiKok = prefix;
                }
                sb.AppendLine(" " + segment + ":" + eski + ":" + yeni);
            }
        }

#if DEBUG
        /// <summary>
        /// Verilen tiplerin her biri için ayrı bir .cs dosyası üretir.
        /// Dosya adı: TipAdi.Karşılaştır.g.cs
        /// </summary>
        public static void Karşılaştırma_için_gerekli_cs_dosyalarını_üret(string hedefKlasor, params Type[] tipler)
        {
            if (!Directory.Exists(hedefKlasor))
                Directory.CreateDirectory(hedefKlasor);

            foreach (var tip in tipler)
            {
                string kod = TamDosyaUret(tip);
                string dosyaAdi = Path.Combine(hedefKlasor, $"{tip.Name}.Karsilastir.g.cs");
                File.WriteAllText(dosyaAdi, kod, System.Text.Encoding.UTF8);
            }
        }

        public static string TamDosyaUret(Type tip)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// Otomatik üretildi — FarkKoduUreteci tarafından");
            sb.AppendLine("// Bu dosyayı elle düzenlemeyin.");
            sb.AppendLine();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Linq;");
            sb.AppendLine("using System.Text;");
            sb.AppendLine();

            bool nsVar = !string.IsNullOrEmpty(tip.Namespace);
            string g = nsVar ? "    " : "";
            string g2 = g + "    ";
            string g3 = g2 + "    ";

            if (nsVar) { sb.AppendLine($"namespace {tip.Namespace}"); sb.AppendLine("{"); }

            sb.AppendLine($"{g}public partial class {tip.Name}");
            sb.AppendLine($"{g}{{");

            var alanlar = JsonIncludeAlanlariAl(tip);

            // ── Karşılaştır_Yazdır implementasyonu ───────────────────
            sb.AppendLine($"{g2}partial void Karşılaştır_Yazdır({tip.Name} B, ref string aciklama, string yolOneki)");
            sb.AppendLine($"{g2}{{");
            sb.AppendLine($"{g3}var sb = new StringBuilder();");
            sb.AppendLine($"{g3}string _oncekiYol_ = null;");
            sb.AppendLine();
            foreach (var alan in alanlar)
                AlanKoduYaz(alan, sb, g3);
            sb.AppendLine($"{g3}aciklama = sb.Length > 0 ? sb.ToString() : string.Empty;");
            sb.AppendLine($"{g2}}}");
            sb.AppendLine();

            // ── Karşılaştır_NullKarsiHesapla implementasyonu ──────────
            // solNull=true  → null:Deger  (A yok, B var)
            // solNull=false → Deger:null  (A var, B yok)
            sb.AppendLine($"{g2}void Karşılaştır_NullKarsiHesapla(ref string aciklama, string yolOneki, bool solNull)");
            sb.AppendLine($"{g2}{{");
            sb.AppendLine($"{g3}var sb = new StringBuilder();");
            sb.AppendLine($"{g3}string _oncekiYol_ = null;");
            sb.AppendLine();
            foreach (var alan in alanlar)
                NullKarsiAlanKoduYaz(alan, sb, g3);
            sb.AppendLine($"{g3}aciklama = sb.Length > 0 ? sb.ToString() : string.Empty;");
            sb.AppendLine($"{g2}}}");
            sb.AppendLine();
            sb.AppendLine($"{g2}public string Karşılaştır_NullKarsi(string yolOneki = \"\", bool solNull = false)");
            sb.AppendLine($"{g2}{{");
            sb.AppendLine($"{g3}string aciklama = null;");
            sb.AppendLine($"{g3}Karşılaştır_NullKarsiHesapla(ref aciklama, yolOneki, solNull);");
            sb.AppendLine($"{g3}return aciklama ?? string.Empty;");
            sb.AppendLine($"{g2}}}");

            sb.AppendLine($"{g}}}");
            if (nsVar) sb.AppendLine("}");

            return sb.ToString();
        }

        // ════════════════════════════════════════════════════════
        // Alan yönlendiriciler
        // Sıralama: SozlukMu → ListeMi → SinifMi → basit tip
        // (List<T> IsClass=true olduğundan SinifMi önce gelirse yanlış dala düşer)
        // ════════════════════════════════════════════════════════

        private static void AlanKoduYaz(FieldInfo alan, StringBuilder sb, string g)
        {
            Type tip = alan.FieldType;
            string ad = alan.Name;
            Type altTip = Nullable.GetUnderlyingType(tip);
            bool nullable = altTip != null;
            Type gercekTip = altTip ?? tip;

            if (SozlukMu(gercekTip)) SozlukKoduYaz(ad, gercekTip, sb, g, nullable);
            else if (ListeMi(gercekTip)) ListeKoduYaz(ad, gercekTip, sb, g, nullable);
            else if (SinifMi(gercekTip)) SinifKoduYaz(ad, gercekTip, sb, g, nullable);
            else BasitTipKoduYaz(ad, sb, g);
        }

        private static void NullKarsiAlanKoduYaz(FieldInfo alan, StringBuilder sb, string g)
        {
            Type tip = alan.FieldType;
            string ad = alan.Name;
            Type altTip = Nullable.GetUnderlyingType(tip);
            Type gercekTip = altTip ?? tip;
            string dv = Temizle(ad);
            string yol = Yol(ad);

            if (SozlukMu(gercekTip))
            {
                // Value tipini çıkar — sınıfsa Karşılaştır_NullKarsi, değilse DegerStr
                Type valTip = gercekTip.GetGenericArguments().Length >= 2 ? gercekTip.GetGenericArguments()[1] : typeof(object);
                Type valAlt = Nullable.GetUnderlyingType(valTip) ?? valTip;
                bool valSinif = SinifMi(valAlt);

                sb.AppendLine($"{g}if (this.{ad} != null)");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    foreach (var _kv_{dv} in (this.{ad} as System.Collections.IDictionary)?.Cast<System.Collections.DictionaryEntry>() ?? Enumerable.Empty<System.Collections.DictionaryEntry>())");
                sb.AppendLine($"{g}    {{");
                if (valSinif)
                {
                    sb.AppendLine($"{g}        var _yol_{dv} = {YolKey(ad, $"_kv_{dv}.Key")};");
                    sb.AppendLine($"{g}        if (_kv_{dv}.Value != null)");
                    sb.AppendLine($"{g}        {{");
                    sb.AppendLine($"{g}            var _nf_{dv} = (({valAlt.Name})_kv_{dv}.Value).Karşılaştır_NullKarsi(_yol_{dv}, solNull);");
                    sb.AppendLine($"{g}            if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                    sb.AppendLine($"{g}        }}");
                }
                else
                {
                    sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _kv_{dv}.Key + \"]\", solNull ? \"null\" : ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_kv_{dv}.Value), solNull ? ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_kv_{dv}.Value) : \"null\");");
                }
                sb.AppendLine($"{g}    }}");
                sb.AppendLine($"{g}}}");
            }
            else if (ListeMi(gercekTip))
            {
                Type elemanTip = ListeElemanTipiniAl(gercekTip);
                string eTipAdi = CSharpTipAdi(elemanTip ?? typeof(object));
                bool elemanSinif = SinifMi(elemanTip ?? typeof(object));

                sb.AppendLine($"{g}if (this.{ad} != null)");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    int _idx_{dv} = 0;");
                sb.AppendLine($"{g}    foreach (var item in this.{ad}.Cast<{eTipAdi}>())");
                sb.AppendLine($"{g}    {{");
                if (elemanSinif)
                {
                    sb.AppendLine($"{g}        var _yol_{dv} = {YolKey(ad, $"_idx_{dv}.ToString()")};");
                    sb.AppendLine($"{g}        if (item != null)");
                    sb.AppendLine($"{g}        {{");
                    sb.AppendLine($"{g}            var _nf_{dv} = item.Karşılaştır_NullKarsi(_yol_{dv}, solNull);");
                    sb.AppendLine($"{g}            if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                    sb.AppendLine($"{g}        }}");
                }
                else
                {
                    sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _idx_{dv} + \"]\", solNull ? \"null\" : ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(item), solNull ? ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(item) : \"null\");");
                }
                sb.AppendLine($"{g}        _idx_{dv}++;");
                sb.AppendLine($"{g}    }}");
                sb.AppendLine($"{g}}}");
            }
            else if (SinifMi(gercekTip))
            {
                sb.AppendLine($"{g}if (this.{ad} != null)");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    var _nf_{dv} = this.{ad}.Karşılaştır_NullKarsi({yol}, solNull);");
                sb.AppendLine($"{g}    if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                sb.AppendLine($"{g}}}");
            }
            else
            {
                // Basit tip — this.alan null ise her iki taraf da null demek, atla
                sb.AppendLine($"{g}if (this.{ad} != null)");
                sb.AppendLine($"{g}    ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}\", solNull ? \"null\" : ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(this.{ad}), solNull ? ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(this.{ad}) : \"null\");");
            }
            sb.AppendLine();
        }

        // ════════════════════════════════════════════════════════
        // Yol ifadesi üreticiler
        // ════════════════════════════════════════════════════════

        // yolOneki boşsa "Alan", doluysa "Ust.Alan"
        private static string Yol(string ad)
            => $"(string.IsNullOrEmpty(yolOneki) ? \"{ad}\" : yolOneki + \".{ad}\")";

        // Liste/sözlük için: "Alan[key]" veya "Ust.Alan[key]"
        private static string YolKey(string ad, string keyDegisken)
            => $"(string.IsNullOrEmpty(yolOneki) ? \"{ad}[\" + {keyDegisken} + \"]\" : yolOneki + \".{ad}[\" + {keyDegisken} + \"]\")";

        // ════════════════════════════════════════════════════════
        // Kod yazıcılar
        // ════════════════════════════════════════════════════════

        // ── Basit tip ───────────────────────────────────────────

        private static void BasitTipKoduYaz(string ad, StringBuilder sb, string g)
        {
            sb.AppendLine($"{g}if (!Equals(this.{ad}, B.{ad}))");
            sb.AppendLine($"{g}    ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(this.{ad}), ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(B.{ad}));");
            sb.AppendLine();
        }

        // ── Sınıf tipi ──────────────────────────────────────────

        private static void SinifKoduYaz(string ad, Type tip, StringBuilder sb, string g, bool nullable)
        {
            string dv = Temizle(ad);
            string icYol = Yol(ad);

            if (nullable)
            {
                sb.AppendLine($"{g}if (this.{ad} == null && B.{ad} == null) {{ /* ikisi de null */ }}");
                sb.AppendLine($"{g}else if (this.{ad} == null)");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    var _sf_{dv} = B.{ad}.Karşılaştır_NullKarsi({icYol}, solNull: true);");
                sb.AppendLine($"{g}    if (!string.IsNullOrEmpty(_sf_{dv})) sb.Append(_sf_{dv});");
                sb.AppendLine($"{g}}}");
                sb.AppendLine($"{g}else if (B.{ad} == null)");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    var _sf_{dv} = this.{ad}.Karşılaştır_NullKarsi({icYol}, solNull: false);");
                sb.AppendLine($"{g}    if (!string.IsNullOrEmpty(_sf_{dv})) sb.Append(_sf_{dv});");
                sb.AppendLine($"{g}}}");
                sb.AppendLine($"{g}else");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    var _sf_{dv} = this.{ad}.Karşılaştır(B.{ad}, {icYol});");
                sb.AppendLine($"{g}    if (!string.IsNullOrEmpty(_sf_{dv})) sb.Append(_sf_{dv});");
                sb.AppendLine($"{g}}}");
            }
            else
            {
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    var _sf_{dv} = this.{ad}.Karşılaştır(B.{ad}, {icYol});");
                sb.AppendLine($"{g}    if (!string.IsNullOrEmpty(_sf_{dv})) sb.Append(_sf_{dv});");
                sb.AppendLine($"{g}}}");
            }
            sb.AppendLine();
        }

        // ── Liste / Dizi ────────────────────────────────────────

        private static void ListeKoduYaz(string ad, Type tip, StringBuilder sb, string g, bool nullable)
        {
            Type elemanTip = ListeElemanTipiniAl(tip);
            if (elemanTip == null) return;

            var keyAlanlar = (elemanTip.IsClass && elemanTip != typeof(string))
                ? elemanTip.GetFields(BindingFlags.Public | BindingFlags.Instance)
                           .Where(f => f.IsDefined(typeof(JsonListKeyAttribute), false))
                           .OrderBy(f => f.GetCustomAttribute<JsonListKeyAttribute>().Order)
                           .ToList()
                : new List<FieldInfo>();

            bool keyVar = keyAlanlar.Count > 0;
            bool elemanSinif = SinifMi(elemanTip);
            string eTipAdi = CSharpTipAdi(elemanTip);
            string dv = Temizle(ad);

            if (nullable)
            {
                sb.AppendLine($"{g}if (this.{ad} != null || B.{ad} != null)");
                sb.AppendLine($"{g}{{");
                g += "    ";
            }

            if (keyVar)
            {
                string keyExpr = string.Join(" + \"|\" + ", keyAlanlar.Select(k => $"x.{k.Name}"));

                sb.AppendLine($"{g}var _dA_{dv} = (this.{ad} != null ? this.{ad}.Cast<{eTipAdi}>() : Enumerable.Empty<{eTipAdi}>()).ToDictionary(x => {keyExpr});");
                sb.AppendLine($"{g}var _dB_{dv} = (B.{ad} != null ? B.{ad}.Cast<{eTipAdi}>() : Enumerable.Empty<{eTipAdi}>()).ToDictionary(x => {keyExpr});");
                sb.AppendLine($"{g}foreach (var _k_{dv} in _dA_{dv}.Keys.Union(_dB_{dv}.Keys))");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    bool _hasA_{dv} = _dA_{dv}.TryGetValue(_k_{dv}, out var _iA_{dv});");
                sb.AppendLine($"{g}    bool _hasB_{dv} = _dB_{dv}.TryGetValue(_k_{dv}, out var _iB_{dv});");

                if (elemanSinif)
                {
                    sb.AppendLine($"{g}    var _yol_{dv} = {YolKey(ad, $"_k_{dv}")};");
                    sb.AppendLine($"{g}    if (!_hasA_{dv})");
                    sb.AppendLine($"{g}    {{");
                    sb.AppendLine($"{g}        var _nf_{dv} = _iB_{dv} != null ? _iB_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: true) : string.Empty;");
                    sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                    sb.AppendLine($"{g}    }}");
                    sb.AppendLine($"{g}    else if (!_hasB_{dv})");
                    sb.AppendLine($"{g}    {{");
                    sb.AppendLine($"{g}        var _nf_{dv} = _iA_{dv} != null ? _iA_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: false) : string.Empty;");
                    sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                    sb.AppendLine($"{g}    }}");
                    sb.AppendLine($"{g}    else");
                    sb.AppendLine($"{g}    {{");
                    sb.AppendLine($"{g}        var _ef_{dv} = _iA_{dv}.Karşılaştır(_iB_{dv}, _yol_{dv});");
                    sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_ef_{dv})) sb.Append(_ef_{dv});");
                    sb.AppendLine($"{g}    }}");
                }
                else
                {
                    sb.AppendLine($"{g}    if (!_hasA_{dv})");
                    sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _k_{dv} + \"]\", \"null\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_iB_{dv}));");
                    sb.AppendLine($"{g}    else if (!_hasB_{dv})");
                    sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _k_{dv} + \"]\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_iA_{dv}), \"null\");");
                    sb.AppendLine($"{g}    else if (!Equals(_iA_{dv}, _iB_{dv}))");
                    sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _k_{dv} + \"]\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_iA_{dv}), ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_iB_{dv}));");
                }

                sb.AppendLine($"{g}}}");
            }
            else
            {
                // Index bazlı
                sb.AppendLine($"{g}var _lA_{dv} = (this.{ad} as System.Collections.IEnumerable)?.Cast<{eTipAdi}>().ToList() ?? new List<{eTipAdi}>();");
                sb.AppendLine($"{g}var _lB_{dv} = (B.{ad} as System.Collections.IEnumerable)?.Cast<{eTipAdi}>().ToList() ?? new List<{eTipAdi}>();");
                sb.AppendLine($"{g}int _max_{dv} = Math.Max(_lA_{dv}.Count, _lB_{dv}.Count);");
                sb.AppendLine($"{g}for (int _i_{dv} = 0; _i_{dv} < _max_{dv}; _i_{dv}++)");
                sb.AppendLine($"{g}{{");
                sb.AppendLine($"{g}    {eTipAdi} _iA_{dv} = _i_{dv} < _lA_{dv}.Count ? _lA_{dv}[_i_{dv}] : default;");
                sb.AppendLine($"{g}    {eTipAdi} _iB_{dv} = _i_{dv} < _lB_{dv}.Count ? _lB_{dv}[_i_{dv}] : default;");

                if (elemanSinif)
                {
                    sb.AppendLine($"{g}    var _yol_{dv} = {YolKey(ad, $"_i_{dv}.ToString()")};");
                    sb.AppendLine($"{g}    if (_iA_{dv} == null && _iB_{dv} == null) {{ /* ikisi de null */ }}");
                    sb.AppendLine($"{g}    else if (_iA_{dv} == null)");
                    sb.AppendLine($"{g}    {{");
                    sb.AppendLine($"{g}        var _nf_{dv} = _iB_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: true);");
                    sb.AppendLine($"{g}        sb.Append(_nf_{dv});");
                    sb.AppendLine($"{g}    }}");
                    sb.AppendLine($"{g}    else if (_iB_{dv} == null)");
                    sb.AppendLine($"{g}    {{");
                    sb.AppendLine($"{g}        var _nf_{dv} = _iA_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: false);");
                    sb.AppendLine($"{g}        sb.Append(_nf_{dv});");
                    sb.AppendLine($"{g}    }}");
                    sb.AppendLine($"{g}    else");
                    sb.AppendLine($"{g}    {{");
                    sb.AppendLine($"{g}        var _ef_{dv} = _iA_{dv}.Karşılaştır(_iB_{dv}, _yol_{dv});");
                    sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_ef_{dv})) sb.Append(_ef_{dv});");
                    sb.AppendLine($"{g}    }}");
                }
                else
                {
                    sb.AppendLine($"{g}    if (!Equals(_iA_{dv}, _iB_{dv}))");
                    sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _i_{dv} + \"]\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_iA_{dv}), ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_iB_{dv}));");
                }

                sb.AppendLine($"{g}}}");
            }

            if (nullable)
            {
                g = g.Substring(4);
                sb.AppendLine($"{g}}}");
            }

            sb.AppendLine();
        }

        // ── Sözlük ──────────────────────────────────────────────

        private static void SozlukKoduYaz(string ad, Type tip, StringBuilder sb, string g, bool nullable)
        {
            string dv = Temizle(ad);
            Type valTip = tip.GetGenericArguments().Length >= 2 ? tip.GetGenericArguments()[1] : typeof(object);
            Type valAlt = Nullable.GetUnderlyingType(valTip) ?? valTip;
            bool valSinif = SinifMi(valAlt);

            if (nullable)
            {
                sb.AppendLine($"{g}if (this.{ad} != null || B.{ad} != null)");
                sb.AppendLine($"{g}{{");
                g += "    ";
            }

            sb.AppendLine($"{g}var _dA_{dv} = this.{ad} ?? new();");
            sb.AppendLine($"{g}var _dB_{dv} = B.{ad} ?? new();");
            sb.AppendLine($"{g}foreach (var _k_{dv} in _dA_{dv}.Keys.Union(_dB_{dv}.Keys))");
            sb.AppendLine($"{g}{{");
            sb.AppendLine($"{g}    bool _avA_{dv} = _dA_{dv}.TryGetValue(_k_{dv}, out var _vA_{dv});");
            sb.AppendLine($"{g}    bool _avB_{dv} = _dB_{dv}.TryGetValue(_k_{dv}, out var _vB_{dv});");

            if (valSinif)
            {
                // Value sınıf — tüm null kombinasyonlarında Karşılaştır_NullKarsi çağır
                sb.AppendLine($"{g}    var _yol_{dv} = {YolKey(ad, $"_k_{dv}")};");
                sb.AppendLine($"{g}    if (!_avA_{dv})");
                sb.AppendLine($"{g}    {{");
                sb.AppendLine($"{g}        var _nf_{dv} = _vB_{dv} != null ? _vB_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: true) : string.Empty;");
                sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                sb.AppendLine($"{g}    }}");
                sb.AppendLine($"{g}    else if (!_avB_{dv})");
                sb.AppendLine($"{g}    {{");
                sb.AppendLine($"{g}        var _nf_{dv} = _vA_{dv} != null ? _vA_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: false) : string.Empty;");
                sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_nf_{dv})) sb.Append(_nf_{dv});");
                sb.AppendLine($"{g}    }}");
                sb.AppendLine($"{g}    else if (_vA_{dv} == null && _vB_{dv} == null) {{ /* ikisi de null */ }}");
                sb.AppendLine($"{g}    else if (_vA_{dv} == null)");
                sb.AppendLine($"{g}    {{");
                sb.AppendLine($"{g}        var _nf_{dv} = _vB_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: true);");
                sb.AppendLine($"{g}        sb.Append(_nf_{dv});");
                sb.AppendLine($"{g}    }}");
                sb.AppendLine($"{g}    else if (_vB_{dv} == null)");
                sb.AppendLine($"{g}    {{");
                sb.AppendLine($"{g}        var _nf_{dv} = _vA_{dv}.Karşılaştır_NullKarsi(_yol_{dv}, solNull: false);");
                sb.AppendLine($"{g}        sb.Append(_nf_{dv});");
                sb.AppendLine($"{g}    }}");
                sb.AppendLine($"{g}    else");
                sb.AppendLine($"{g}    {{");
                sb.AppendLine($"{g}        var _ef_{dv} = _vA_{dv}.Karşılaştır(_vB_{dv}, _yol_{dv});");
                sb.AppendLine($"{g}        if (!string.IsNullOrEmpty(_ef_{dv})) sb.Append(_ef_{dv});");
                sb.AppendLine($"{g}    }}");
            }
            else
            {
                // Value basit tip
                sb.AppendLine($"{g}    if (!_avA_{dv})");
                sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _k_{dv} + \"]\", \"null\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_vB_{dv}));");
                sb.AppendLine($"{g}    else if (!_avB_{dv})");
                sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _k_{dv} + \"]\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_vA_{dv}), \"null\");");
                sb.AppendLine($"{g}    else if (!Equals(_vA_{dv}, _vB_{dv}))");
                sb.AppendLine($"{g}        ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DeğişkenAdınıOluştur(sb, ref _oncekiYol_, yolOneki, \"{ad}[\" + _k_{dv} + \"]\", ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_vA_{dv}), ArgeMup.HazirKod.SınıfKarşılaştırma_KoduÜreteci.DegerStr(_vB_{dv}));");
            }

            sb.AppendLine($"{g}}}");

            if (nullable)
            {
                g = g.Substring(4);
                sb.AppendLine($"{g}}}");
            }

            sb.AppendLine();
        }

        // ════════════════════════════════════════════════════════
        // Yardımcı metotlar
        // ════════════════════════════════════════════════════════

        private static List<FieldInfo> JsonIncludeAlanlariAl(Type tip) =>
            tip.GetFields(BindingFlags.Public | BindingFlags.Instance)
               .Where(f => f.IsDefined(typeof(JsonIncludeAttribute), inherit: false)
                        && !f.IsDefined(typeof(JsonIgnoreAttribute), inherit: false))
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
            if (t == typeof(int)) return "int";
            if (t == typeof(string)) return "string";
            if (t == typeof(bool)) return "bool";
            if (t == typeof(double)) return "double";
            if (t == typeof(float)) return "float";
            if (t == typeof(long)) return "long";
            if (t == typeof(decimal)) return "decimal";
            if (t == typeof(DateTime)) return "DateTime";
            if (t == typeof(Guid)) return "Guid";
            return t.Name.Split('`')[0];
        }

        private static string Temizle(string s) =>
            new string(s.Where(c => char.IsLetterOrDigit(c) || c == '_').ToArray());
#endif
        #endregion
    }
}