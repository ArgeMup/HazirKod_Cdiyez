// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Linq;
using System.Collections.Generic;
using ArgeMup.HazirKod.Ekİşlemler;
using System.Collections;
using System.Reflection;

namespace ArgeMup.HazirKod
{
    public class Değişken_
    {
        public const string Sürüm = "V1.0";

        public BindingFlags Filtre_TipTürü = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
        public string[] Filtre_Değişkenİsimleri = null;
        public bool Filtre_Filtre_Değişkenİsimlerini_DahilEt1_HariçTut0 = true;
        public bool Filtre_BüyükKüçükHarfDuyarlı = true;
        public char Filtre_Ayraç = '*';
        public void Depola(object Nesne, IDepo_Eleman Depo)
        {
            Yaz(Nesne, Depo, 0);
        }
        public object Üret(Type Tipi, IDepo_Eleman Depo)
        {
            return Oku(Tipi, Depo, Depo[0] /*Sadece basit tipteki değişkenlerin okunabilmesi için*/);
        }

        bool Filtre_KontrolEt_Kullanılsın(string DeğişkenAdı)
        {
            if (Filtre_Değişkenİsimleri != null)
            {
                if (Filtre_Filtre_Değişkenİsimlerini_DahilEt1_HariçTut0)
                {
                    foreach (string f in Filtre_Değişkenİsimleri)
                    {
                        if (f.DoluMu() && DeğişkenAdı.BenzerMi(f, Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç)) return true;
                    }

                    return false;
                }
                else
                {
                    foreach (string f in Filtre_Değişkenİsimleri)
                    {
                        if (f.DoluMu() && DeğişkenAdı.BenzerMi(f, Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç)) return false;
                    }
                }
            }
                
            return true;
        }
        bool Basit_Tipte_Mi(Type Tip)
        {
            if (Tip.IsValueType)
            {
                bool struct_mı = !Tip.IsPrimitive && !Tip.Namespace.StartsWith("System") && !Tip.IsEnum;

                return !struct_mı;
            }
            else if (Tip == typeof(string)) return true;

            return false;
        }
        void Yaz(object Nesne, IDepo_Eleman Depo, int Konum)
        {
            if (Nesne == null) return;

            Type Tipi = Nesne.GetType();
            if (Basit_Tipte_Mi(Tipi))
            {
                Depo.Yaz(null, Convert.ToString(Nesne, System.Globalization.CultureInfo.InvariantCulture), Konum);
            }
            else if (Tipi.IsArray || (Tipi.IsGenericType && Tipi.GetGenericTypeDefinition() == typeof(List<>)))
            {
                if (Tipi == typeof(byte[]))
                {
                    Depo.Yaz(null, (byte[])Nesne);
                }
                else
                {
                    Type elm_tipi = Tipi.IsGenericType ? Tipi.GenericTypeArguments.First() : Tipi.IsArray ? Tipi.GetElementType() : null;
                    if (elm_tipi == null) throw new Exception(Depo.Adı + " no " + Konum + " için tip belirlenemedi");

                    int sıra_no = 0;
                    bool bilinen_tipte_dizi_mi = Basit_Tipte_Mi(elm_tipi);
                    foreach (var alt_dizi_elm in (IEnumerable)Nesne)
                    {
                        Yaz(alt_dizi_elm, bilinen_tipte_dizi_mi ? Depo : Depo[sıra_no.ToString()], sıra_no++);
                    }
                }
            }
            else if (Tipi.IsGenericType && Tipi.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = Tipi.GetGenericArguments()[0];
                if (Basit_Tipte_Mi(keyType))
                {
                    int sıra_no = 0;
                    foreach (var kvp in (IEnumerable)Nesne)
                    {
                        keyType = kvp.GetType();
                        string anahtar = keyType.GetProperty("Key").GetValue(kvp, null).ToString();
                        object içerik = keyType.GetProperty("Value").GetValue(kvp, null);

                        if (içerik == null) continue;

                        Type Tipi_Sıradaki = içerik.GetType();
                        Yaz(içerik, Depo[anahtar], Basit_Tipte_Mi(Tipi_Sıradaki) ? 0 : sıra_no++);
                    }
                }
                else throw new Exception(Depo.Adı + " sözlüğünün anahtar bölümü tek başına yazıya dönüştürülebilen basit tipte olmalıdır");
            }
            else
            {
                FieldInfo[] Elemanları = Tipi.GetFields(Filtre_TipTürü);
                if (Elemanları != null)
                {
                    for (int i = 0; i < Elemanları.Length && !Tipi.IsEnum; i++)
                    {
                        FieldInfo eleman = Elemanları[i];
                        if (eleman.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() != null) continue; //getter setter leri atla
                        if (!Filtre_KontrolEt_Kullanılsın(eleman.Name)) continue;

                        Yaz(eleman.GetValue(Nesne), Depo[eleman.Name], Basit_Tipte_Mi(eleman.FieldType) ? 0 : i);
                    }
                }
            }
        }
        object Oku(Type Tipi, IDepo_Eleman Depo, string İçerik)
        {
            object Nesne = null;

            try
            {
                if (Basit_Tipte_Mi(Tipi))
                {
                    if (İçerik.DoluMu())
                    {
                        if (Tipi.IsEnum)
                        {
                            if (Tipi.IsEnumDefined(İçerik))
                            {
                                return Enum.Parse(Tipi, İçerik);
                            }
                        }
                        else return Convert.ChangeType(İçerik, Tipi, System.Globalization.CultureInfo.InvariantCulture);
                    }

                    return null;
                }
                else if (Tipi.IsArray)
                {
                    Type Tipi_eleman = Tipi.GetElementType();

                    if (Tipi_eleman == typeof(byte))
                    {
                        return Depo.Oku_BaytDizisi(null);
                    }
                    else
                    {
                        Array Üretilen_Dizi = null;

                        if (Basit_Tipte_Mi(Tipi_eleman))
                        {
                            Üretilen_Dizi = Array.CreateInstance(Tipi_eleman, Depo.İçeriği.Length);
                            
                            for (int i = 0; i < Üretilen_Dizi.Length; i++)
                            {
                                object değeri = Oku(Tipi_eleman, Depo, Depo[i]);
                                if (değeri != null) Üretilen_Dizi.SetValue(değeri, i);
                            }
                        }
                        else if (Depo.Elemanları.Length > 0)
                        {
                            //en büyük numaradan dizideki azami eleman noyu bul
                            int enbüyük = -1;
                            foreach (IDepo_Eleman biri in Depo.Elemanları)
                            {
                                if (!int.TryParse(biri.Adı, out int değeri))
                                {
                                    enbüyük = -1;
                                    break;
                                }

                                if (değeri > enbüyük) enbüyük = değeri;
                            }

                            if (enbüyük < 0)
                            {
                                //dizideki elemanların isimlendirmelerinden en az biri uygun değil, doğrudan diziye kaydet 
                                Üretilen_Dizi = Array.CreateInstance(Tipi_eleman, Depo.Elemanları.Length);
                                for (int i = 0; i < Üretilen_Dizi.Length; i++)
                                {
                                    object değeri = Oku(Tipi_eleman, Depo.Elemanları[i], Depo.Elemanları[i][0]);
                                    if (değeri != null) Üretilen_Dizi.SetValue(değeri, i);
                                }
                            }
                            else
                            {
                                //elemanın ismini dizideki konum olacak şekilde diziye kaydet
                                enbüyük++; //0 dan başlıyor
                                Üretilen_Dizi = Array.CreateInstance(Tipi_eleman, enbüyük);
                                foreach (IDepo_Eleman biri in Depo.Elemanları)
                                {
                                    object değeri = Oku(Tipi_eleman, biri, biri[0]);
                                    if (değeri != null) Üretilen_Dizi.SetValue(değeri, int.Parse(biri.Adı));
                                }
                            }
                        }

                        return Üretilen_Dizi;
                    }
                }
                else if (Tipi.IsGenericType && Tipi.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Nesne = Activator.CreateInstance(Tipi);
                    Tipi = Tipi.GenericTypeArguments[0];
                    if (Basit_Tipte_Mi(Tipi))
                    {
                        for (int i = 0; i < Depo.İçeriği.Length; i++)
                        {
                            object değeri = Oku(Tipi, Depo, Depo[i]);
                            if (değeri != null) ((IList)Nesne).Add(değeri);
                        }
                    }
                    else
                    {
                        foreach (IDepo_Eleman eleman in Depo.Elemanları)
                        {
                            object değeri = Oku(Tipi, eleman, eleman[0]);
                            if (değeri != null) ((IList)Nesne).Add(değeri);
                        }
                    }

                    return Nesne;
                }
                else if (Tipi.IsGenericType && Tipi.GetGenericTypeDefinition() == typeof(Dictionary<,>))
                {
                    Type tip_anahtar = Tipi.GenericTypeArguments[0];
                    if (Basit_Tipte_Mi(tip_anahtar))
                    {
                        Type tip_içerik = Tipi.GenericTypeArguments[1];
                        Type dictType = typeof(Dictionary<,>).MakeGenericType(tip_anahtar, tip_içerik);
                        Nesne = Activator.CreateInstance(dictType);

                        foreach (IDepo_Eleman eleman in Depo.Elemanları)
                        {
                            object anahtar = Oku(tip_anahtar, eleman, eleman.Adı);
                            object içerik = Oku(tip_içerik, eleman, eleman[0]);
                            
                            if (anahtar == null || içerik == null) continue;

                            ((IDictionary)Nesne).Add(anahtar, içerik);
                        }

                        return Nesne;
                    }
                    else throw new Exception(Depo.Adı + " sözlüğünün anahtar bölümü tek başına yazıya dönüştürülebilen basit tipte olmalıdır");
                }
                else Nesne = Activator.CreateInstance(Tipi);

                FieldInfo[] Elemanları = Tipi.GetFields(Filtre_TipTürü);
                if (Elemanları == null || Elemanları.Length == 0) return Nesne;

                for (int i = 0; i < Elemanları.Length; i++)
                {
                    FieldInfo eleman = Elemanları[i];
                    IDepo_Eleman eleman_depo = Depo.Bul(eleman.Name);
                    if (eleman_depo == null || eleman.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() != null) continue; //kaydı olmayan veya getter setter leri atla
                    if (!Filtre_KontrolEt_Kullanılsın(eleman.Name)) continue;

                    object üretilen = Oku(eleman.FieldType, Depo[eleman.Name], Depo[eleman.Name][Basit_Tipte_Mi(eleman.FieldType) ? 0 : i]);
                    if (üretilen != null) eleman.SetValue(Nesne, üretilen);
                }
            }
            catch (Exception ex) 
            { 
                ex.Günlük(Seviyesi: Günlük.Seviye.HazirKod);
            }

            return Nesne;
        }
    }
}