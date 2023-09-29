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
        public IEnumerable<string> Filtre_Değişkenİsimleri = null;
        public bool Filtre_Değişkenİsimlerini_DahilEt1_HariçTut0 = true;
        public bool Filtre_BoşVeyaVarsayılanDeğerdeİse_HariçTut = false;
        public bool Filtre_BüyükKüçükHarfDuyarlı = true;
        public char Filtre_Ayraç = '*';
        public int İzinVerilen_İçİçeÇağrıSayısı_Sabiti = 15;
        public void Depola(object Nesne, IDepo_Eleman Depo)
        {
            Yaz(Nesne, Depo, 0, 0);
        }
        public object Üret(Type Tipi, IDepo_Eleman Depo)
        {
            return Oku(Tipi, Depo, Depo[0] /*Sadece basit tipteki değişkenlerin okunabilmesi için*/, 0);
        }

        bool KontrolEt_Kullanılsın_Mı(FieldInfo Değişken, out string Adı, out int SıraNo)
        {
            Adı = Değişken.Name;
            SıraNo = 0;

            //getter setter leri atla
            if (Değişken.GetCustomAttribute<System.Runtime.CompilerServices.CompilerGeneratedAttribute>() != null) return false; 

            //Nitelik kontrolü
            Niteliği.Adını_DeğiştirAttribute AdınıDeğiştir = Değişken.GetCustomAttribute<Niteliği.Adını_DeğiştirAttribute>();
            if (AdınıDeğiştir != null && AdınıDeğiştir.KullanılacakAdı.DoluMu(true))
            {
                Adı = AdınıDeğiştir.KullanılacakAdı;
                if (Basit_Tipte_Mi(Değişken.FieldType) && AdınıDeğiştir.KullanılacakSıraNo >= 0) SıraNo = AdınıDeğiştir.KullanılacakSıraNo;
            }

            if (Değişken.GetCustomAttribute<Niteliği.Bunu_Kesinlikle_KullanAttribute>() != null) return true;
            if (Değişken.GetCustomAttribute<Niteliği.Bunu_Kesinlikle_KullanmaAttribute>() != null) return false;
            
            //Filtre kontrolü
            if (Filtre_Değişkenİsimleri != null)
            {
                if (Filtre_Değişkenİsimlerini_DahilEt1_HariçTut0)
                {
                    if (Adı.BenzerMi(Filtre_Değişkenİsimleri, Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç)) return true;
                    
                    return false;
                }
                else
                {
                    if (Adı.BenzerMi(Filtre_Değişkenİsimleri, Filtre_BüyükKüçükHarfDuyarlı, Filtre_Ayraç)) return false;
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
        void Yaz(object Nesne, IDepo_Eleman Depo, int Konum, int İçİçeÇağrıSayısı)
        {
            if (Nesne == null || ++İçİçeÇağrıSayısı > İzinVerilen_İçİçeÇağrıSayısı_Sabiti) return; //Çok fazla iç içe çağrı yapıldı

            Type Tipi = Nesne.GetType();
            if (Basit_Tipte_Mi(Tipi))
            {
                if ( !(Nesne.BoşVeyaVarsayılanDeğerdeMi() && Filtre_BoşVeyaVarsayılanDeğerdeİse_HariçTut) )
                {
                    if (Tipi == typeof(DateTime)) Depo.Yaz(null, (DateTime)Nesne, Konum);
                    else Depo.Yaz(null, Convert.ToString(Nesne, System.Globalization.CultureInfo.InvariantCulture), Konum);
                }
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
                        Yaz(alt_dizi_elm, bilinen_tipte_dizi_mi ? Depo : Depo[sıra_no.ToString()], sıra_no++, İçİçeÇağrıSayısı);
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
                        Yaz(içerik, Depo[anahtar], Basit_Tipte_Mi(Tipi_Sıradaki) ? 0 : sıra_no++, İçİçeÇağrıSayısı);
                    }
                }
                else throw new Exception(Depo.Adı + " sözlüğünün anahtar bölümü tek başına yazıya dönüştürülebilen basit tipte olmalıdır");
            }
            else
            {
                FieldInfo[] Elemanları = Tipi.GetFields(Filtre_TipTürü);
                if (Elemanları != null)
                {
                    List<string> Kullanılanİsimler = new List<string>();
                    for (int i = 0; i < Elemanları.Length && !Tipi.IsEnum; i++)
                    {
                        FieldInfo eleman = Elemanları[i];
                        bool snç = KontrolEt_Kullanılsın_Mı(eleman, out string Adı, out int SıraNo);

                        string GeciciAdı = Adı + (SıraNo > 0 ? "[" + SıraNo.Yazıya() + "]": null);
                        if (Kullanılanİsimler.Contains(GeciciAdı)) throw new Exception(Tipi.Name + " içindeki " + eleman.Name + (eleman.Name != Adı ? " (" + GeciciAdı + ")" : null) + " değişken adı olarak zaten kullanıldı, farklı bir isim seçiniz");
                        else Kullanılanİsimler.Add(GeciciAdı); //kullanılmayacak olsa bile ileride çakışma olmaması için şimdiden uyarabilmek adına listeye ekle
                        if (!snç) continue; //istenmeyenleri atla

                        Yaz(eleman.GetValue(Nesne), Depo[Adı], Basit_Tipte_Mi(eleman.FieldType) ? SıraNo : i, İçİçeÇağrıSayısı);
                    }
                }
            }
        }
        object Oku(Type Tipi, IDepo_Eleman Depo, string İçerik, int İçİçeÇağrıSayısı)
        {
            object Nesne;
            if (++İçİçeÇağrıSayısı > İzinVerilen_İçİçeÇağrıSayısı_Sabiti) return null; //Çok fazla iç içe çağrı yapıldı

            if (Basit_Tipte_Mi(Tipi))
            {
                if (İçerik.DoluMu())
                {
                    try
                    {
                        if (Tipi.IsEnum) return Enum.Parse(Tipi, İçerik);
                        else
                        {
                            if (Tipi == typeof(DateTime)) return İçerik.TarihSaate();
                            else return Convert.ChangeType(İçerik, Tipi, System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                    catch (Exception) { } // Tip dönüşüm hatası : Değişkenin ilk değerini koruması için geri dön 
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
                            object değeri = Oku(Tipi_eleman, Depo, Depo[i], İçİçeÇağrıSayısı);
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
                                object değeri = Oku(Tipi_eleman, Depo.Elemanları[i], Depo.Elemanları[i][0], İçİçeÇağrıSayısı);
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
                                object değeri = Oku(Tipi_eleman, biri, biri[0], İçİçeÇağrıSayısı);
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
                        object değeri = Oku(Tipi, Depo, Depo[i], İçİçeÇağrıSayısı);
                        if (değeri != null) ((IList)Nesne).Add(değeri);
                    }
                }
                else
                {
                    foreach (IDepo_Eleman eleman in Depo.Elemanları)
                    {
                        object değeri = Oku(Tipi, eleman, eleman[0], İçİçeÇağrıSayısı);
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
                        object anahtar = Oku(tip_anahtar, eleman, eleman.Adı, İçİçeÇağrıSayısı);
                        object içerik = Oku(tip_içerik, eleman, eleman[0], İçİçeÇağrıSayısı);
                            
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
                if (!KontrolEt_Kullanılsın_Mı(eleman, out string Adı, out int SıraNo)) continue; //istenmeyenleri atla

                IDepo_Eleman eleman_depo = Depo.Bul(Adı);
                if (eleman_depo == null)
                {
                    Niteliği.Sürüm_Geçişi_İçin_Eski_AdıAttribute Sürüm_Geçişi_İçin_Eski_Adı = eleman.GetCustomAttribute<Niteliği.Sürüm_Geçişi_İçin_Eski_AdıAttribute>();
                    if (Sürüm_Geçişi_İçin_Eski_Adı != null && Sürüm_Geçişi_İçin_Eski_Adı.EskiAdı.DoluMu(true))
                    {
                        eleman_depo = Depo.Bul(Sürüm_Geçişi_İçin_Eski_Adı.EskiAdı);
                        if (Basit_Tipte_Mi(eleman.FieldType) && Sürüm_Geçişi_İçin_Eski_Adı.EskiSıraNo >= 0) SıraNo = Sürüm_Geçişi_İçin_Eski_Adı.EskiSıraNo;
                    }

                    if (eleman_depo == null) continue; //kaydı olmayanları atla
                }

                object üretilen = Oku(eleman.FieldType, eleman_depo, eleman_depo[Basit_Tipte_Mi(eleman.FieldType) ? SıraNo : i], İçİçeÇağrıSayısı);
                if (üretilen != null) eleman.SetValue(Nesne, üretilen);
            }

            return Nesne;
        }

        #region Nitelik
        public class Niteliği
        {
            /// <summary>
            /// Değişkenin kendi adı yerine burada belirtilen adı kullanılır
            /// </summary>
            [AttributeUsage(AttributeTargets.Field)]
            public class Adını_DeğiştirAttribute : Attribute
            {
                public string KullanılacakAdı;
                public int KullanılacakSıraNo;

                public Adını_DeğiştirAttribute(string Adı, int SıraNo = 0)
                {
                    KullanılacakAdı = Adı;
                    KullanılacakSıraNo = SıraNo;
                }
            }

            /// <summary>
            /// Depodan nesneye aktarırken kendi adı için bir içerik bulamaması durumunda buradaki ad ile tekrar kontrol eder
            /// </summary>
            [AttributeUsage(AttributeTargets.Field)]
            public class Sürüm_Geçişi_İçin_Eski_AdıAttribute : Attribute
            {
                public string EskiAdı;
                public int EskiSıraNo;

                public Sürüm_Geçişi_İçin_Eski_AdıAttribute(string Adı, int SıraNo = 0)
                {
                    EskiAdı = Adı;
                    EskiSıraNo = SıraNo;
                }
            }

            /// <summary>
            /// Belirtilen nesneyi dahil eder
            /// <br>Bir filtre ile çakışması önemsizdir</br>
            /// </summary>
            [AttributeUsage(AttributeTargets.Field)]
            public class Bunu_Kesinlikle_KullanAttribute : Attribute { }

            /// <summary>
            /// Belirtilen nesneyi hariç tutar
            /// <br>Bir filtre ile çakışması önemsizdir</br>
            /// </summary>
            [AttributeUsage(AttributeTargets.Field)]
            public class Bunu_Kesinlikle_KullanmaAttribute : Attribute { }
        }
        #endregion
    }
}