// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_SeriPort
    using System;
    using System.IO.Ports;
    using System.Threading;
    using System.Linq;

    namespace ArgeMup.HazirKod.DonanımHaberleşmesi
    {
        public class SeriPort_ : IDisposable, IDonanımHaberleşmesi
        {
            public const string Sürüm = "V1.1";

            #region Genel Görüşe Açık
            public object Hatırlatıcı = null;
            public bool SatırSatırGönderVeAl = true;
            public int TekrarDeneme_ZamanAşımı_msn = 5000;
            public int BilgiGönderme_ZamanAşımı_msn = 15000;
            #endregion

            #region İç Kullanım
            string ErişimNoktası = "", ErişimNoktasıAçıklaması = "";
            System.Collections.Generic.List<string> ErişimNoktaları = null;
            int BitHızı = 921600;
            int BitSayısı = 8;
            Parity Doğrulama = Parity.None;
            StopBits DurBitSayısı = StopBits.One;
            bool Çalışşsın = true;

            GeriBildirim_Islemi_ GeriBildirim_Islemi = null;

            SerialPort SeriPort = null;
            #endregion
            
            /// <param name="ErişimNoktası">COMX olması durumunda ilk mümkün olan erişim noktasını açar </param>
            public SeriPort_(string ErişimNoktası, int BitHızı, GeriBildirim_Islemi_ GeriBildirim_Islemi = null, object Hatırlatıcı = null, bool SatırSatırGönderVeAl = true, int TekrarDeneme_ZamanAşımı_msn = 5000, int BilgiGönderme_ZamanAşımı_msn = 15000, int BitSayısı = 8, string Doğrulama = "Yok", double DurBitSayısı = 1.0)
            {
                this.ErişimNoktası = ErişimNoktası;
                this.BitHızı = BitHızı;
                this.BitSayısı = BitSayısı;
                this.Doğrulama = (Doğrulama == "Yok") ? Parity.None : (Doğrulama == "Tek") ? Parity.Odd : (Doğrulama == "Çift") ? Parity.Even : (Doğrulama == "Boşluk") ? Parity.Space : Parity.Mark;
                this.DurBitSayısı = (DurBitSayısı == 1.0) ? StopBits.One : (DurBitSayısı == 1.5) ? StopBits.OnePointFive : (DurBitSayısı == 2.0) ? StopBits.Two : StopBits.None;
                this.GeriBildirim_Islemi = GeriBildirim_Islemi;
                this.Hatırlatıcı = Hatırlatıcı;
                this.SatırSatırGönderVeAl = SatırSatırGönderVeAl;
                this.TekrarDeneme_ZamanAşımı_msn = TekrarDeneme_ZamanAşımı_msn;
                this.BilgiGönderme_ZamanAşımı_msn = BilgiGönderme_ZamanAşımı_msn;

                new Thread(() => Görev_İşlemi_SeriPort()).Start();
            }
            void Görev_İşlemi_SeriPort()
            {
                string _erişim_noktası = ErişimNoktası;
                ErişimNoktasıAçıklaması = ErişimNoktası;

                while (Çalışşsın)
                {
                    try
                    {
                        if (SeriPort == null || !SeriPort.IsOpen)
                        {
                            if (ErişimNoktası == "COMx")
                            {
                                if (ErişimNoktaları == null || ErişimNoktaları.Count == 0)
                                {
                                    string[] dizi = SerialPort.GetPortNames();
                                    if (dizi == null || dizi.Length == 0) throw new Exception("Hiçbir erişim noktası mevcut değil");

                                    ErişimNoktaları = dizi.ToList();
                                }

                                ErişimNoktasıAçıklaması = "COMx " + ErişimNoktaları[0];
                                _erişim_noktası = ErişimNoktaları[0];
                                ErişimNoktaları.RemoveAt(0);
                            }

                            Thread.Sleep(1); //Bazen oluşan açamama hatasını gidermek için 
                            SeriPort = new SerialPort(_erişim_noktası, BitHızı, Doğrulama, BitSayısı, DurBitSayısı);
                            SeriPort.Open();
                            SeriPort.DiscardOutBuffer();
                            SeriPort.DiscardInBuffer();
                            SeriPort.ReadTimeout = TekrarDeneme_ZamanAşımı_msn;
                            SeriPort.WriteTimeout = BilgiGönderme_ZamanAşımı_msn;

                            GeriBildirim_Islemi?.Invoke(ErişimNoktasıAçıklaması, GeriBildirim_Türü_.BağlantıKuruldu, null, Hatırlatıcı);
                        }
                        
                        int sayac = 0;
                        while (Çalışşsın && SeriPort.IsOpen)
                        {
                            object çıktı = null;
                            if (SatırSatırGönderVeAl) çıktı = SatırSonu.Sil(SeriPort.ReadLine());
                            else
                            {
                                int ilk_gelen_bilgi = SeriPort.ReadByte();
                                if (ilk_gelen_bilgi >= 0)
                                {
                                    int adet_bekleyen = SeriPort.BytesToRead;

                                    byte[] tampon_GelenBilgi = new byte[adet_bekleyen + 1];
                                    tampon_GelenBilgi[0] = (byte)ilk_gelen_bilgi;

                                    int adet_okunan = SeriPort.Read(tampon_GelenBilgi, 1, adet_bekleyen);
                                    if (adet_okunan != adet_bekleyen) Array.Resize(ref tampon_GelenBilgi, adet_okunan + 1);
                                    çıktı = tampon_GelenBilgi;
                                }
                            }

                            if (çıktı == null) throw new Exception();
                            else
                            {
                                GeriBildirim_Islemi?.Invoke(ErişimNoktasıAçıklaması, GeriBildirim_Türü_.BilgiGeldi, çıktı, Hatırlatıcı);

                                if (sayac++ > 100) { sayac = 0; Thread.Sleep(1); } //cpu yüzdesini düşürmek için
                            }
                        }
                    }
                    catch (TimeoutException) { Thread.Sleep(1); } //cpu yüzdesini düşürmek için
                    catch (Exception) 
                    {
                        GeriBildirim_Islemi?.Invoke(ErişimNoktasıAçıklaması, GeriBildirim_Türü_.BağlantıKurulmasıTekrarDenecek, null, Hatırlatıcı);

                        Durdur();

                        if (Çalışşsın) Thread.Sleep(1000);
                    }
                }

                Durdur(true);

                GeriBildirim_Islemi?.Invoke(ErişimNoktasıAçıklaması, GeriBildirim_Türü_.Durduruldu, null, Hatırlatıcı);
            }
            void Durdur(bool TamamenDurdur = false)
            {
                if (TamamenDurdur)
                {
                    Çalışşsın = false;
                    TekrarDeneme_ZamanAşımı_msn = 1;
                }

                try { if (SeriPort != null) { SeriPort.Close(); SeriPort.Dispose(); } } catch (Exception) { }
                SeriPort = null;

                GeriBildirim_Islemi?.Invoke(ErişimNoktasıAçıklaması, GeriBildirim_Türü_.BağlantıKoptu, null, Hatırlatıcı);
            }

            #region IDonanımHaberlleşmesi
            bool IDonanımHaberleşmesi.BağlantıKurulduMu()
            {
                return SeriPort == null ? false : SeriPort.IsOpen;
            }
            void IDonanımHaberleşmesi.Durdur()
            {
                Durdur(true);
            }
            void IDonanımHaberleşmesi.Gönder(byte[] Bilgi, string Alıcı)
            {
                if (SeriPort == null || !SeriPort.IsOpen) throw new Exception("Bağlantı Kurulmadı");

                SeriPort.Write(Bilgi, 0, Bilgi.Length);
            }
            void IDonanımHaberleşmesi.Gönder(string Bilgi, string Alıcı)
            {
                if (SeriPort == null || !SeriPort.IsOpen) throw new Exception("Bağlantı Kurulmadı");

                SeriPort.Write(Bilgi + SatırSonu.Karakteri);
            }
            #endregion

            #region IDisposable Support
            private bool disposedValue = false; // To detect redundant calls

            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: dispose managed state (managed objects).

                        Durdur(true);
                    }

                    // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                    // TODO: set large fields to null.

                    //disposedValue = true;
                }
            }

            // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
            ~SeriPort_()
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
#endif