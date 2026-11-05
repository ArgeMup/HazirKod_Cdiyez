// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ArgeMup.HazirKod
{ 
    public class Nesne_Değişken_Ozellikleri_
    {
        public FieldInfo _Değişken_f = null;
        public PropertyInfo _Değişken_p = null;
        public object Kaynak_Nesne;

        public object? Değeri
        {
            get
            {
                if (_Değişken_f != null)
                {
                    return _Değişken_f.GetValue(Kaynak_Nesne);
                }
                else
                {
                    return _Değişken_p.GetValue(Kaynak_Nesne);
                }
            }

            set
            {
                if (_Değişken_f != null)
                {
                    _Değişken_f.SetValue(Kaynak_Nesne, value);
                }
                else
                {
                    _Değişken_p.SetValue(Kaynak_Nesne, value);
                }
            }
        }

        public bool Getter { get => _Değişken_f != null ? true : _Değişken_p.GetMethod != null; }
        public bool Setter { get => _Değişken_f != null ? !_Değişken_f.IsInitOnly && !_Değişken_f.IsLiteral : _Değişken_p.SetMethod != null; }
        public bool Public { get => _Değişken_f != null ? _Değişken_f.IsPublic : (_Değişken_p.GetMethod?.IsPublic ?? false) || (_Değişken_p.SetMethod?.IsPublic ?? false); }
        public List<string> AttributeLer { get => _Değişken_f != null ? _Değişken_f.GetCustomAttributes(true).Select(a => a.GetType().Name).ToList() : _Değişken_p.GetCustomAttributes(true).Select(a => a.GetType().Name).ToList(); }
        public Type Tipi { get => _Değişken_f != null ? _Değişken_f.FieldType : _Değişken_p.PropertyType; }
        public bool Statik { get => _Değişken_f != null ? _Değişken_f.IsStatic : (_Değişken_p.GetMethod ?? _Değişken_p.SetMethod)!.IsStatic; }
        public bool Miras { get; set; }
    }

    public class Nesne_Değişken_
    {
        public const string Sürüm = "V1.0";

        public const BindingFlags Bayraklar = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
        public static object? Deger_Oku(object Nesne, string DegiskenAdi, BindingFlags Bayraklar = Nesne_Değişken_.Bayraklar)
        {
            var tip = Nesne.GetType();
            var p = tip.GetProperty(DegiskenAdi, Bayraklar);
            if (p != null) return p.GetValue(Nesne);

            var f = tip.GetField(DegiskenAdi, Bayraklar)
                ?? throw new MissingMemberException(tip.Name, DegiskenAdi);
            return f.GetValue(Nesne);
        }
        public static void Deger_Yaz(object Nesne, string degiskenAdi, object? deger, BindingFlags Bayraklar = Nesne_Değişken_.Bayraklar)
        {
            var tip = Nesne.GetType();
            var p = tip.GetProperty(degiskenAdi, Bayraklar);
            if (p != null) { p.SetValue(Nesne, deger); return; }

            var f = tip.GetField(degiskenAdi, Bayraklar)
                ?? throw new MissingMemberException(tip.Name, degiskenAdi);
            f.SetValue(Nesne, deger);
        }

        public Dictionary<string, Nesne_Değişken_Ozellikleri_> Liste;
        public Nesne_Değişken_(object Nesne = null, BindingFlags Bayraklar = Nesne_Değişken_.Bayraklar)
        {
            Listele(Nesne, Bayraklar);
        }
        public void Listele(object Nesne, BindingFlags Bayraklar = Nesne_Değişken_.Bayraklar)
        {
            Liste = new Dictionary<string, Nesne_Değişken_Ozellikleri_>();

            if (Nesne == null) return;
            var ilkTip = Nesne.GetType();

            for (var t = ilkTip; t != null && t != typeof(object); t = t.BaseType)
            {
                foreach (var f in t.GetFields(Bayraklar))
                {
                    if (f.IsDefined(typeof(CompilerGeneratedAttribute), false)) continue; // backing field

                    var yeni = new Nesne_Değişken_Ozellikleri_
                    {
                        Kaynak_Nesne = Nesne,
                        _Değişken_f = f,
                        Miras = f.DeclaringType != ilkTip
                    };

                    Ekle(f.Name, yeni, t);
                }

                foreach (var p in t.GetProperties(Bayraklar))
                {
                    if (p.GetIndexParameters().Length > 0) continue;

                    var yeni = new Nesne_Değişken_Ozellikleri_
                    {
                        Kaynak_Nesne = Nesne,
                        _Değişken_p = p,
                        Miras = p.DeclaringType != ilkTip
                    };

                    Ekle(p.Name, yeni, t);
                }

                void Ekle(string ad, Nesne_Değişken_Ozellikleri_ d, Type t)
                {
                    // Alt sınıf önce eklenir; override edilen aynı üye atlanır, farklı (private/new) üye tip adıyla eklenir
                    if (Liste.TryGetValue(ad, out var mevcut))
                    {
                        if (mevcut._Değişken_p != null && 
                            d._Değişken_p != null &&
                            (mevcut._Değişken_p.GetMethod ?? mevcut._Değişken_p.SetMethod)!.GetBaseDefinition() == (d._Değişken_p.GetMethod ?? d._Değişken_p.SetMethod)!.GetBaseDefinition())
                        {
                            return; // override
                        }
                        
                        ad = t.Name + "." + ad;
                    }

                    Liste.TryAdd(ad, d);
                }
            }
        }
    }
}