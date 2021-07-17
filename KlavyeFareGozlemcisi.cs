// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod>

using System;
using System.Collections.Generic;

namespace ArgeMup.HazirKod
{
    public class KlavyeFareGozlemcisi_ : IDisposable
    {
        public const string Sürüm = "V1.1";

        #region Değişkenler LL
        public DateTime SonKlavyeFareOlayıAnı = DateTime.Now;

        const int WH_KEYBOARD_LL = 13;
        const int WH_MOUSE_LL = 14;

        int KlavyeGözlemcisi_Handle = 0;
        int FareGözlemcisi_Handle = 0;

        W32_2.Win32HookProcHandler KlavyeGirişimiOldu = null;
        W32_2.Win32HookProcHandler FareGirişimiOldu = null;   
        #endregion

        #region Değişkenler KısayolTuşu
        public delegate void KısayolTuşu_Basıldı(int Hatırlatıcı);
        struct KısayolTuşu_Biri_
        {
            public KısayolTuşu_Basıldı İşlem;
            public int Hatırlatıcı;
            public string YazıŞeklinde;
        }
        Dictionary<UInt64, KısayolTuşu_Biri_> KısayolTuşları = null;
        UInt64 KısayolTuşu_GüncelDeğeri = 0;
        #endregion

        #region Değişkenler KısayolTuşu Tanıt
        public delegate void KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi_(string BasılanTuşlar);
        KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi_ KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi = null;
        #endregion

        public KlavyeFareGozlemcisi_(bool Fareyi_İzle = true, bool Klavyeyi_İzle = true, bool KısayolTuşlarınıİzle = false)
        {
            if (Fareyi_İzle)
            {
                FareGirişimiOldu = new W32_2.Win32HookProcHandler(FareOlayı);
                FareGözlemcisi_Handle = W32_2.SetWindowsHookEx(WH_MOUSE_LL, FareGirişimiOldu, W32_1.LoadLibrary("User32"), 0);

                if (FareGözlemcisi_Handle == 0)
                {
                    Dispose();
                    throw new Exception("Fare döngüsüne üye olunamadı");
                }
            }

            if (KısayolTuşlarınıİzle)
            {
                KlavyeGirişimiOldu = new W32_2.Win32HookProcHandler(KlavyeOlayı_KısayolTuşları);
                KlavyeGözlemcisi_Handle = W32_2.SetWindowsHookEx(WH_KEYBOARD_LL, KlavyeGirişimiOldu, W32_1.LoadLibrary("User32"), 0);

                if (KlavyeGözlemcisi_Handle == 0)
                {
                    Dispose();
                    throw new Exception("Klavye döngüsüne üye olunamadı");
                }

                KısayolTuşları = new Dictionary<ulong, KısayolTuşu_Biri_>();
            }
            else if (Klavyeyi_İzle)
            {
                KlavyeGirişimiOldu = new W32_2.Win32HookProcHandler(KlavyeOlayı);
                KlavyeGözlemcisi_Handle = W32_2.SetWindowsHookEx(WH_KEYBOARD_LL, KlavyeGirişimiOldu, W32_1.LoadLibrary("User32"), 0);

                if (KlavyeGözlemcisi_Handle == 0)
                {
                    Dispose();
                    throw new Exception("Klavye döngüsüne üye olunamadı");
                }
            }
        }

        public void KısayolTuşu_Tanıt_Başlat(KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi_ Çağırılacak_İşlem)
        {
            if (KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi != null) throw new Exception("Öncelikle etkin işlemi tamamlayın");
            if (Çağırılacak_İşlem == null) throw new Exception("Çağırılacak_İşlem boş olamaz");

            KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi = Çağırılacak_İşlem;
            KısayolTuşu_GüncelDeğeri = 0;
        }
        void KısayolTuşu_Tanıt_Ekle(UInt64 Tuş)
        {
            UInt32 sadece_başlık_olarak = (UInt32)(Tuş >> 32);
            if (sadece_başlık_olarak == 0) return; //başlık bilgisi yok 

            UInt32 sadece_tuş_olarak = (UInt32)(Tuş & 0x00000000FFFFFFFF);
            if (sadece_tuş_olarak == 46) return; //del tuşu
            if (sadece_tuş_olarak == 32) return; //boşluk tuşu

            string basılan_Tuş = "";
            if ((Tuş & (((UInt64)1) << 63)) > 0) basılan_Tuş += "sol-shift ";
            if ((Tuş & (((UInt64)1) << 62)) > 0) basılan_Tuş += "sağ-shift ";
            if ((Tuş & (((UInt64)1) << 61)) > 0) basılan_Tuş += "sol-ctrl ";
            if ((Tuş & (((UInt64)1) << 60)) > 0) basılan_Tuş += "sağ-ctrl ";
            if ((Tuş & (((UInt64)1) << 59)) > 0) basılan_Tuş += "alt ";
            if ((Tuş & (((UInt64)1) << 58)) > 0) basılan_Tuş += "win ";

            for (int i = 32; i < 32 + 24; i++)
            {
                if ((Tuş & (((UInt64)1) << i)) > 0) basılan_Tuş += "F" + (i - 31) + " ";
            }

            if (sadece_tuş_olarak >= 48 && sadece_tuş_olarak <= 57) basılan_Tuş += (sadece_tuş_olarak - 48).ToString();                //normal 0-9
            else if (sadece_tuş_olarak >= 96 && sadece_tuş_olarak <= 105) basılan_Tuş += "TT" + (sadece_tuş_olarak - 96).ToString();   //tuş takımı 0-9
            else if (sadece_tuş_olarak >= 65 && sadece_tuş_olarak <= 90) basılan_Tuş += ((char)sadece_tuş_olarak).ToString();          //A - Z
            else if (sadece_tuş_olarak > 0) basılan_Tuş += "(" + sadece_tuş_olarak.ToString() + ")";                                                              //diğerleri

            KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi(basılan_Tuş.TrimEnd());
        }
        public void KısayolTuşu_Tanıt_Bitir()
        {
            if (KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi == null) throw new Exception("Öncelikle işlemi başlatın");

            KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi = null;
        }

        public void KısayolTuşu_Ekle(string Tuşlar, KısayolTuşu_Basıldı Çağırılacak_İşlem, int Hatırlatıcı)
        {
            if (string.IsNullOrEmpty(Tuşlar) ||
                Çağırılacak_İşlem == null) throw new Exception("Hatalı parametre");
            
            KısayolTuşu_Biri_ yeni = new KısayolTuşu_Biri_();
            yeni.Hatırlatıcı = Hatırlatıcı;
            yeni.İşlem = Çağırılacak_İşlem;
            yeni.YazıŞeklinde = Tuşlar;
            UInt64 Tuş = 0;

            if (Tuşlar.Contains("sol-shift"))
            {
                Tuşlar = Tuşlar.Replace("sol-shift", "");
                Tuş |= (UInt64)1 << 63;
            }
            if (Tuşlar.Contains("sağ-shift"))
            {
                Tuşlar = Tuşlar.Replace("sağ-shift", "");
                Tuş |= (UInt64)1 << 62;
            }
            if (Tuşlar.Contains("sol-ctrl"))
            {
                Tuşlar = Tuşlar.Replace("sol-ctrl", "");
                Tuş |= (UInt64)1 << 61;
            }
            if (Tuşlar.Contains("sağ-ctrl"))
            {
                Tuşlar = Tuşlar.Replace("sağ-ctrl", "");
                Tuş |= (UInt64)1 << 60;
            }
            if (Tuşlar.Contains("alt"))
            {
                Tuşlar = Tuşlar.Replace("alt", "");
                Tuş |= (UInt64)1 << 59;
            }
            if (Tuşlar.Contains("win"))
            {
                Tuşlar = Tuşlar.Replace("win", "");
                Tuş |= (UInt64)1 << 58;
            }

            for (int i = 31 + 24; i >= 32; i--)
            {
                string yz = "F" + (i - 31);
                if (Tuşlar.Contains(yz))
                {
                    Tuşlar = Tuşlar.Replace(yz, "");
                    Tuş |= (UInt64)1 << i;
                }
            }

            Tuşlar = Tuşlar.Trim();

            if (Tuşlar.Length == 3/*TTX*/ && Tuşlar.StartsWith("TT"))
            {
                Tuş += (byte)(((byte)Tuşlar[2]) + (byte)48);  //tuş takımı 0-9
            }
            else if (Tuşlar.Length <= 5/*(XYZ)*/ && Tuşlar.StartsWith("(") && Tuşlar.EndsWith(")"))
            {
                Tuş += Convert.ToByte(Tuşlar.Replace("(", "").Replace(")", "")); //Diğerleri
            }
            else if (Tuşlar.Length == 1/*X*/)
            {
                Tuş += (byte)Tuşlar[0]; //normal 0-9  //A - Z
            }
            else throw new Exception("Hatalı parametre");

            if (KısayolTuşları.ContainsKey(Tuş)) throw new Exception("Zaten eklenmiş");

            KısayolTuşları.Add(Tuş, yeni);
        }
        public void KısayolTuşu_Sil(int Hatırlatıcı)
        {
            Yenidenadene:
            foreach (var biri in KısayolTuşları)
            {
                if (biri.Value.Hatırlatıcı == Hatırlatıcı)
                {
                    KısayolTuşları.Remove(biri.Key);
                    goto Yenidenadene;
                }
            }
        }
        public void KısayolTuşu_Sil(string Tuşlar)
        {
            Yenidenadene:
            foreach (var biri in KısayolTuşları)
            {
                if (biri.Value.YazıŞeklinde == Tuşlar)
                {
                    KısayolTuşları.Remove(biri.Key);
                    goto Yenidenadene;
                }
            }
        }

        private int FareOlayı(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0) SonKlavyeFareOlayıAnı = DateTime.Now;
            return W32_2.CallNextHookEx(FareGözlemcisi_Handle, nCode, wParam, lParam);
        }
        private int KlavyeOlayı(int nCode, IntPtr wParam, IntPtr lParam)
        {
            //var wp = wParam;
            //var lp = System.Runtime.InteropServices.Marshal.ReadInt32(lParam);
            //Console.WriteLine("klavye nCode " + nCode + " wp " + wp + " lp " + lp);

            if (nCode >= 0) SonKlavyeFareOlayıAnı = DateTime.Now;
            return W32_2.CallNextHookEx(KlavyeGözlemcisi_Handle, nCode, wParam, lParam);
        }
        private int KlavyeOlayı_KısayolTuşları(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int okunan = System.Runtime.InteropServices.Marshal.ReadInt32(lParam);
            bool basıldı = true;

            if ((int)wParam == 256)
            {
                //basıldı
                if (okunan == 160) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 63;         //sol-shift
                else if (okunan == 161) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 62;    //sağ-shift
                else if (okunan == 162) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 61;    //sol-ctrl
                else if (okunan == 163) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 60;    //sağ-ctrl
                else if (okunan == 164) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 59;    //alt
                else if (okunan == 91) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 58;     //win
                else if (okunan >= 112 && okunan <= 135) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << (okunan - 112 + 32); //F1 - F24
                else
                {
                    KısayolTuşu_GüncelDeğeri &= 0xFFFFFFFF00000000;
                    KısayolTuşu_GüncelDeğeri |= (uint)okunan;                   //normal 0-9 //tuş takımı 0-9 //A - Z //diğerleri
                }
            }
            else if ((int)wParam == 260)
            {
                //basıldı
                if (okunan == 164) KısayolTuşu_GüncelDeğeri |= (UInt64)1 << 59;         //alt
            }
            else if ((int)wParam == 257)
            {
                //çekildi
                if (okunan == 160) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << 63);        //sol-shift
                else if (okunan == 161) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << 62);   //sağ-shift
                else if (okunan == 162) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << 61);   //sol-ctrl
                else if (okunan == 163) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << 60);   //sağ-ctrl
                else if (okunan == 164) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << 59);   //alt
                else if (okunan == 91) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << 58);    //win
                else if (okunan >= 112 && okunan <= 135) KısayolTuşu_GüncelDeğeri &= ~(((UInt64)1) << (32 + (okunan - 112))); //F1 - F24
                else KısayolTuşu_GüncelDeğeri &= 0xFFFFFFFF00000000;            //normal 0-9 //tuş takımı 0-9 //A - Z //diğerleri

                basıldı = false;
            }

            if (basıldı)
            {
                if (KısayolTuşu_Tanıt_YeniTuşaBasıldı_Islemi == null)
                {
                    //normal çalışma anı
                    if (KısayolTuşları.TryGetValue(KısayolTuşu_GüncelDeğeri, out KısayolTuşu_Biri_ biri))
                    {
                        biri.İşlem(biri.Hatırlatıcı);
                    }   
                }
                else KısayolTuşu_Tanıt_Ekle(KısayolTuşu_GüncelDeğeri); //tanıtma anı
            }

            return KlavyeOlayı(nCode, wParam, lParam);
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

                if (KlavyeGözlemcisi_Handle != 0)
                {
                    if (W32_2.UnhookWindowsHookEx(KlavyeGözlemcisi_Handle))
                    {
                        KlavyeGözlemcisi_Handle = 0;
                        KlavyeGirişimiOldu = null;
                    }
                }

                if (FareGözlemcisi_Handle != 0)
                {
                    if (W32_2.UnhookWindowsHookEx(FareGözlemcisi_Handle))
                    {
                        FareGözlemcisi_Handle = 0;
                        FareGirişimiOldu = null;
                    }
                }

                //disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~KlavyeFareGozlemcisi_()
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

