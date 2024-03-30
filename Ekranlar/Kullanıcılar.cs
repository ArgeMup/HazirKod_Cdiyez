// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_Görsel
using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ArgeMup.HazirKod.Ekranlar
{
    partial class Kullanıcılar_ÖnYüz_ : Form
    {
        public enum İşlemTürü_ { Boşta, Giriş, ParolaDeğiştirme, Ayarlar };

        Kullanıcılar.Ayarlar_Üst_ Ayarlar_Üst_Kopya;
        ListeKutusu Kullanıcılar_Liste;
        ListeKutusu Roller_Liste;

        public Kullanıcılar_ÖnYüz_()
        {
            InitializeComponent();
        }
        public void Başlat(İşlemTürü_ İşlemTürü)
        {
            Ayarlar_Üst_Kopya = Sınıf_Kopyala(Kullanıcılar._Ayarlar_Üst_);

            switch (İşlemTürü)
            {
                default:
                case İşlemTürü_.Giriş:
                    Ekran_Ayarlar.Visible = false;

                    Kullanıcılar._Ayarlar_Üst_.GeçerliKullanıcı = null;
                    Ekran_Giriş_Kullanıcı.Items.AddRange(Kullanıcılar._Ayarlar_Üst_.KullanıcılarVeParolalar.Keys.ToArray());

                    Width = Ekran_Giriş_YeniParola_2.Width * 2;
                    Left = (Width - Ekran_Giriş.Width) / 2;
                    Height = Ekran_Giriş_Tamam.Top + Ekran_Giriş_Tamam.Height + 50;
                    Top = (Height - Ekran_Giriş_Tamam.Top - Ekran_Giriş_Tamam.Height - 50) / 2;
                    Ekran_Giriş.Dock = DockStyle.Fill;
                    Ekran_Giriş_Parola.KeyDown += Ekran_Giriş_Parola_KeyDown;
                    break;

                case İşlemTürü_.ParolaDeğiştirme:
                    Ekran_Ayarlar.Visible = false;

                    Ekran_Giriş_Kullanıcı.Items.Add(Kullanıcılar._Ayarlar_Üst_.GeçerliKullanıcı.Adı);
                    Ekran_Giriş_Kullanıcı.SelectedIndex = 0;
                    Ekran_Giriş_Kullanıcı.Enabled = false;
                    Ekran_Giriş_Parola.Focus();

                    Ekran_Giriş_YeniParola_1.Visible = true;
                    Ekran_Giriş_YeniParola_2.Visible = true;
                    Ekran_Giriş_YeniParola.Visible = true;
                    Ekran_Giriş_YeniParolaTekrar.Visible = true;
                    Ekran_Giriş_Tamam.Text = "Kaydet";

                    Width = Ekran_Giriş_YeniParola_2.Width * 2;
                    Left = (Width - Ekran_Giriş.Width) / 2;
                    Height = Ekran_Giriş_Tamam.Top + Ekran_Giriş_Tamam.Height + 50;
                    Top = (Height - Ekran_Giriş_Tamam.Top - Ekran_Giriş_Tamam.Height - 50) / 2;
                    Ekran_Giriş.Dock = DockStyle.Fill;
                    Ekran_Giriş_YeniParolaTekrar.KeyDown += Ekran_Giriş_Parola_KeyDown;
                    break;

                case İşlemTürü_.Ayarlar:
                    Ekran_Ayarlar.Dock = DockStyle.Fill;
                    Ekran_Giriş.Visible = false;

                    List<string> Kullanıcı_isimleri = new List<string>();
                    Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Kişiler.ForEach(x => Kullanıcı_isimleri.Add(x.Adı));

                    Kullanıcılar_Liste = new ListeKutusu();
                    Kullanıcılar_Liste.Dock = DockStyle.Fill;
                    Kullanıcılar_Ayraç.Panel1.Controls.Add(Kullanıcılar_Liste);
                    Kullanıcılar_Ayraç.SplitterDistance = Kullanıcılar_Ayraç.Width / 2;
                    ListeKutusu.Ayarlar_ ListeKutusu_Ayarlar = new ListeKutusu.Ayarlar_(true, true, ListeKutusu.Ayarlar_.ElemanKonumu_.AdanZyeSıralanmış, true, false, true);
                    Kullanıcılar_Liste.Başlat(null, Kullanıcı_isimleri, "Kullamıcılar", ListeKutusu_Ayarlar);
                    Kullanıcılar_Liste.GeriBildirim_İşlemi += Kullanıcılar_Liste_GeriBildirim_İşlemi;

                    Roller_Liste = new ListeKutusu();
                    Roller_Liste.Dock = DockStyle.Fill;
                    Roller_Ayraç.Panel1.Controls.Add(Roller_Liste);
                    Roller_Ayraç.SplitterDistance = Roller_Ayraç.Width / 3;
                    Roller_Liste.Başlat(null, Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Roller.Keys.ToList(), "Roller", ListeKutusu_Ayarlar);
                    Roller_Liste.GeriBildirim_İşlemi += Roller_Liste_GeriBildirim_İşlemi;
                    Kullanıcılar_Rol.Items.Clear();
                    Kullanıcılar_Rol.Items.AddRange(Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Roller.Keys.ToArray());

                    Roller_Tablo.Rows.Clear();
                    for (int i = 0; i < Kullanıcılar._Ayarlar_Üst_.Tümİzinler.Count(); i++)
                    {
                        Enum izin = Kullanıcılar._Ayarlar_Üst_.Tümİzinler.ElementAt(i);
                        if (!izin.GeçerliMi()) continue;

                        int SatırNo = Roller_Tablo.Rows.Add(new object[] { izin.Yazdır(), false });
                        Roller_Tablo.Rows[SatırNo].Tag = i;
                    }
                    Roller_Tablo.ClearSelection();

                    Uyarı.Visible = !Kullanıcılar._Ayarlar_Üst_.ParolaKontrolüGerekiyorMu;
                    break;
            }
        }

        Kullanıcılar.Ayarlar_Üst_ Sınıf_Kopyala(Kullanıcılar.Ayarlar_Üst_ Kaynak)
        {
            Kullanıcılar.Ayarlar_Üst_ Hedef = Kaynak.Kopyala(DönüştürmeHatasıOldu_YeniTipiBelirle, false) as Kullanıcılar.Ayarlar_Üst_;
            
            if (Hedef.Ayarlar_Alt == null && Kaynak.Ayarlar_Alt != null) Hedef.Ayarlar_Alt = new Kullanıcılar.Ayarlar_Üst_.Ayarlar_Alt_();
            Hedef.GeriBildirimİşlemi_Ayarlar_Değişti = Kaynak.GeriBildirimİşlemi_Ayarlar_Değişti;

            return Hedef;

            Type DönüştürmeHatasıOldu_YeniTipiBelirle(string DeğişkeninAdı, Type DenenenTip)
            {
                if (DenenenTip == typeof(Enum) && (DeğişkeninAdı == "Tümİzinler" || DeğişkeninAdı == "İzin_AyarlardaDeğişiklikYapabilir")) return Kullanıcılar._Ayarlar_Üst_.İzin_AyarlardaDeğişiklikYapabilir.GetType();

                throw new Exception("DönüştürmeHatasıOldu_YeniTipiBelirle " + DeğişkeninAdı + " " + DenenenTip.ToString());
            }
        }
        Kullanıcılar.Ayarlar_Üst_.Ayarlar_Kullanıcı_ Bul_Kullanıcı(string Adı)
        {
            return Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Kişiler.FirstOrDefault(x => x.Adı == Adı);
        }
        bool[] Bul_Rol(string Adı)
        {
            return Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Roller.FirstOrDefault(x => x.Key == Adı).Value;
        }

        #region Ekran Ayarlar
        bool Ayarlar_Kaydet(string Eski_KökParola, bool RollerGüncellendi = false)
        {
            if (!Kullanıcılar._Ayarlar_Üst_.Ayarlar_DosyayaKaydet(Eski_KökParola)) 
            {
                Kullanıcılar._Ayarlar_Üst_ = Sınıf_Kopyala(Ayarlar_Üst_Kopya);
                return false; 
            }
            else Ayarlar_Üst_Kopya = Sınıf_Kopyala(Kullanıcılar._Ayarlar_Üst_);

            Uyarı.Visible = !Kullanıcılar._Ayarlar_Üst_.ParolaKontrolüGerekiyorMu;
            ÖnYüzler_Kaydet_Kullanıcılar.Enabled = false;
            ÖnYüzler_Kaydet_Roller.Enabled = false;

            if (RollerGüncellendi)
            {
                Kullanıcılar_Rol.Items.Clear();
                Kullanıcılar_Rol.Items.AddRange(Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Roller.Keys.ToArray());
            }

            return true;
        }

        #region Kullanıcılar
        private bool Kullanıcılar_Liste_GeriBildirim_İşlemi(string Adı, ListeKutusu.İşlemTürü Türü, string YeniAdı = null)
        {
            Kullanıcılar.Ayarlar_Üst_.Ayarlar_Kullanıcı_ Kullanıcı;

            if (Adı.DoluMu())
            {
                switch (Türü)
                {
                    case ListeKutusu.İşlemTürü.ElemanSeçildi:
                        Kullanıcı = Bul_Kullanıcı(Adı);
                        Kullanıcılar_Rol.Text = Kullanıcı.RolAdı.DoluMu() ? Kullanıcı.RolAdı : null;

                        Kullanıcılar_Parola.Text = Kullanıcılar._Ayarlar_Üst_.KullanıcılarVeParolalar.ContainsKey(Kullanıcı.Adı) ? "*" : null;
                        Kullanıcılar_Parola.Tag = null;

                        ÖnYüzler_Kaydet_Kullanıcılar.Enabled = false;
                        return true;

                    case ListeKutusu.İşlemTürü.YeniEklendi:
                        Kullanıcılar._Ayarlar_Üst_.Kullanıcı_Ekle(Adı);
                        return Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola);

                    case ListeKutusu.İşlemTürü.AdıDeğiştirildi:
                        string mesaj = "Bu işlem ile kullanıcının parolası silinecek. Parolayı tekrar belirlemeyi unutmayınız." +
                            Environment.NewLine + Environment.NewLine + "İşleme devam etmek istiyor musunuz?";
                        DialogResult Dr = MessageBox.Show(mesaj, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                        if (Dr == DialogResult.No) return false;

                        Kullanıcılar._Ayarlar_Üst_.Kullanıcı_AdınıDeğiştir(Adı, YeniAdı);
                        bool sonuç = Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola);
                        return sonuç;

                    case ListeKutusu.İşlemTürü.Silindi:
                        Kullanıcılar._Ayarlar_Üst_.Kullanıcı_Sil(Adı);
                        return Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola);
                }
            }

            return false;
        }
        private void Kullanıcılar_Parola_TextChanged(object sender, EventArgs e)
        {
            Kullanıcılar_Parola.Tag = "*";
            Kullanıcılar_AyarDeğişti(sender, e);
        }
        private void Kullanıcılar_AyarDeğişti(object sender, EventArgs e)
        {
            ÖnYüzler_Kaydet_Kullanıcılar.Enabled = true;
        }
        private void ÖnYüzler_Kaydet_Kullanıcılar_Click(object sender, EventArgs e)
        {
            if (Kullanıcılar_Liste.SeçilenEleman_Adı.BoşMu()) return;

            Kullanıcılar.Ayarlar_Üst_.Ayarlar_Kullanıcı_ Kullanıcı = Bul_Kullanıcı(Kullanıcılar_Liste.SeçilenEleman_Adı);
            if (Kullanıcı == null)
            {
                Kullanıcı = new Kullanıcılar.Ayarlar_Üst_.Ayarlar_Kullanıcı_
                {
                    Adı = Kullanıcılar_Liste.SeçilenEleman_Adı
                };

                Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Kişiler.Add(Kullanıcı);
            }

            string Eski_KökParola = Kullanıcılar._Ayarlar_Üst_.KökParola;
            if (Kullanıcılar_Parola.Text.BoşMu(true) || Kullanıcılar_Rol.Text.BoşMu(true))
            {
                Kullanıcılar._Ayarlar_Üst_.Parola_EkleDeğiştirSil(Kullanıcı.Adı, null);
            }
            else if (Kullanıcılar_Parola.Tag != null)
            {
                Kullanıcılar._Ayarlar_Üst_.Parola_EkleDeğiştirSil(Kullanıcı.Adı, Kullanıcılar_Parola.Text);
            }
            //else değişikik yapılmadı, önceki parolayı koru

            Kullanıcı.RolAdı = Kullanıcılar_Rol.Text;
            Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt_Araİşlemler(Kullanıcılar._Ayarlar_Üst_.KökParola);
            if (!Ayarlar_Kaydet(Eski_KökParola)) return;

            if (Eski_KökParola.BoşMu(true) && Kullanıcılar._Ayarlar_Üst_.KökParola.DoluMu(true))
            {
                Hide();
                while (Kullanıcılar._Ayarlar_Üst_.GeçerliKullanıcı == null)
                {
                    MessageBox.Show("İlk kez bir parola girildiğinden, kullanıcı girişi yapınız", Text);

                    #region Kullanıcınn giriş yapması
                    bool Bitti = false;
                    Kullanıcılar.Önyüz_Giriş(GeriBildirimİşlemi_Önyüz_Giriş, false, Font.Size);
                    while (!Bitti && ArgeMup.HazirKod.ArkaPlan.Ortak.Çalışsın) { System.Threading.Thread.Sleep(35); Application.DoEvents(); }

                    void GeriBildirimİşlemi_Önyüz_Giriş(bool Başarılı)
                    {
                        Bitti = true;
                    }
                }
                Show();
                #endregion
            }
            else if (Kullanıcılar._Ayarlar_Üst_.KökParola.BoşMu(true))
            {
                Kullanıcılar._Ayarlar_Üst_.GeçerliKullanıcı = null;
            }

            Kullanıcılar_Liste_GeriBildirim_İşlemi(Kullanıcılar_Liste.SeçilenEleman_Adı, ListeKutusu.İşlemTürü.ElemanSeçildi);
        }
        #endregion

        #region Roller
        private bool Roller_Liste_GeriBildirim_İşlemi(string Adı, ListeKutusu.İşlemTürü Türü, string YeniAdı = null)
        {
            bool[] Rol_Dizisi;

            if (Adı.DoluMu())
            {
                switch (Türü)
                {
                    case ListeKutusu.İşlemTürü.ElemanSeçildi:
                        Rol_Dizisi = Bul_Rol(Adı);
                        foreach (DataGridViewRow satır in Roller_Tablo.Rows)
                        {
                            satır.Cells[Roller_Tablo_Etkin.Index].Value = Rol_Dizisi[(int)satır.Tag];
                        }
                        ÖnYüzler_Kaydet_Roller.Enabled = false;
                        Roller_Tablo.ClearSelection();
                        return true;

                    case ListeKutusu.İşlemTürü.YeniEklendi:
                        Kullanıcılar._Ayarlar_Üst_.Rol_Ekle(Adı);
                        return Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola, true);

                    case ListeKutusu.İşlemTürü.AdıDeğiştirildi:
                        Kullanıcılar._Ayarlar_Üst_.Rol_AdınıDeğiştir(Adı, YeniAdı);
                        return Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola, true);

                    case ListeKutusu.İşlemTürü.Silindi:
                        Kullanıcılar._Ayarlar_Üst_.Rol_Sil(Adı);
                        return Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola, true);
                }
            }

            return false;
        }
        private void Roller_Tablo_AyarDeğişti(object sender, DataGridViewCellEventArgs e)
        {
            ÖnYüzler_Kaydet_Roller.Enabled = true;
        }
        private void ÖnYüzler_Kaydet_Roller_Click(object sender, EventArgs e)
        {
            if (Roller_Liste.SeçilenEleman_Adı.BoşMu()) return;

            bool[] Rol_Dizisi = new bool[Kullanıcılar._Ayarlar_Üst_.Tümİzinler.Count()];

            foreach (DataGridViewRow satır in Roller_Tablo.Rows)
            {
                Rol_Dizisi[(int)satır.Tag] = (bool)satır.Cells[Roller_Tablo_Etkin.Index].Value;
            }

            Kullanıcılar._Ayarlar_Üst_.Ayarlar_Alt.Roller[Roller_Liste.SeçilenEleman_Adı] = Rol_Dizisi;

            Ayarlar_Kaydet(Kullanıcılar._Ayarlar_Üst_.KökParola);
        }
        #endregion
        #endregion

        #region Ekran Giriş
        private void Ekran_Giriş_Kullanıcı_SelectedIndexChanged(object sender, EventArgs e)
        {
            Ekran_Giriş_Parola.Focus();
        }
        private void Ekran_Giriş_Parola_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) Ekran_Giriş_Tamam_Click(null, null);
        }
        private void Ekran_Giriş_Tamam_Click(object sender, EventArgs e)
        {
            if (Ekran_Giriş_Tamam.Text == "Tamam")
            {
                //Giriş
                if (!Kullanıcılar._Ayarlar_Üst_.Parola_Kontrol(Ekran_Giriş_Kullanıcı.Text, Ekran_Giriş_Parola.Text)) return;
            }
            else
            {
                //Parola değiştirme
                if (Ekran_Giriş_YeniParola.Text.BoşMu(true)) Ekran_Giriş_YeniParola.Text = null;

                if (Ekran_Giriş_YeniParola.Text.Length < 6 ||
                    Ekran_Giriş_YeniParola.Text != Ekran_Giriş_YeniParolaTekrar.Text ||
                    !Kullanıcılar._Ayarlar_Üst_.Parola_Kontrol(Ekran_Giriş_Kullanıcı.Text, Ekran_Giriş_Parola.Text))
                {
                    MessageBox.Show(
                        "Alttaki kuralları sağladığınızı kontrol ediniz." + Environment.NewLine + Environment.NewLine +
                        "Mevcut parolanızı giriniz" + Environment.NewLine +
                        "En az 6 karakterden oluşan yeni bir parola belirleyiniz" + Environment.NewLine +
                        "Yeni parolanızın başlangıç ve bitişinde boşluk karakterini kullanmayınız" + Environment.NewLine +
                        "Yeni parola ve tekrar parolasının birbirinin aynısı olduğudan emin olunuz",
                        "Parola Değiştirme Ekranı");
                    return;
                }

                string Eski_KökParola = Kullanıcılar._Ayarlar_Üst_.KökParola;
                Kullanıcılar._Ayarlar_Üst_.Parola_EkleDeğiştirSil(Ekran_Giriş_Kullanıcı.Text, Ekran_Giriş_YeniParola.Text);
                Ayarlar_Kaydet(Eski_KökParola);
            }

            Close();
        }
        #endregion
    }

    static class Kullanıcılar_Ekİşlemler
    {
        public static string Yazdır(this Enum İzin)
        {
            return İzin.ToString().Replace('_', ' ');
        }
        public static bool GeçerliMi(this Enum İzin)
        {
            return !İzin.Yazdır().EndsWith(" ");
        }
    }

    public static class Kullanıcılar
    {
        public const string Sürüm = "V1.1";
        public static string KökParola
        {
            get
            {
                return _Ayarlar_Üst_.KökParola;
            }
        }
        public static bool ParolaKontrolüGerekiyorMu
        {
            get
            {
                return _Ayarlar_Üst_.ParolaKontrolüGerekiyorMu;
            }
        }
        public static string KullanıcıAdı
        {
            get
            {
                if (_Ayarlar_Üst_.ParolaKontrolüGerekiyorMu)
                {
                    if (_Ayarlar_Üst_.GeçerliKullanıcı == null) return null;

                    return _Ayarlar_Üst_.GeçerliKullanıcı.Adı;
                }

                return null;
            }
        }
        public delegate void GeriBildirimİşlemi_Önyüz_Giriş(bool Başarılı);
        /// <summary>
        /// Önyüz_Ayarlar() veya Önyüz_ParolaDeğiştir() ile başlatılan bir işlem ayarlarda değişiklik yaptığında çağırılır.
        /// <br></br>
        /// <br>Girdilerin kullanım durumları iyi değerlendirilmelidir.</br>
        /// <br></br>
        /// <br>Mevcut_KökParola == Eski_KökParola  -> Değişiklik yok</br>
        /// <br>Eski_KökParola dolu ise             -> Bu parola ile dosyalarınız düzeltilmelidir</br>
        /// <br>Mevcut_KökParola dolu ise           -> Bu parola ile dosyalarınız karıştırılmalıdır</br>
        /// <br></br>
        /// <br>AyarlarDosyaYolu ile belirtilen dosya içeriği DEĞİŞTİRİLMEMELİDİR.</br>
        /// </summary>
        /// <param name="AyarlarDosyaYolu">Mevcut kullanıcı bilgilerinin kaydedildiği dosyanın adı</param>
        public delegate void GeriBildirimİşlemi_Önyüz_Ayarlar_Değişti(string AyarlarDosyaYolu, string Mevcut_KökParola, string Eski_KökParola);

        public static void Başlat(IEnumerable<Enum> Tümİzinler, Enum İzin_AyarlardaDeğişiklikYapabilir, GeriBildirimİşlemi_Önyüz_Ayarlar_Değişti GeriBildirimİşlemi_Ayarlar_Değişti, string AyarlarDosyaYolu = null, string SihirliKelime = null)
        {
            if (Tümİzinler == null || Tümİzinler.Where(x => x.GeçerliMi()).Count() == 0 ||
                İzin_AyarlardaDeğişiklikYapabilir == null ||
                !Tümİzinler.Contains(İzin_AyarlardaDeğişiklikYapabilir) ||
                GeriBildirimİşlemi_Ayarlar_Değişti == null) throw new ArgumentException("Girdileri kontrol ediniz");

            AyarlarDosyaYolu = AyarlarDosyaYolu ?? Kendi.Klasörü + @"\ArgeMup.HazirKod_Cdiyez.Ekranlar.Kullanıcılar.Ayarlar";
            if (!System.IO.File.Exists(AyarlarDosyaYolu)) _Ayarlar_Üst_ = new Ayarlar_Üst_();
            else _Ayarlar_Üst_ = Ayarlar_Üst_.Sınıf_Oluştur(typeof(Ayarlar_Üst_), AyarlarDosyaYolu.DosyaYolu_Oku_Yazı()) as Ayarlar_Üst_;

            if (_Ayarlar_Üst_ == null || _Ayarlar_Üst_.KullanıcılarVeParolalar == null) throw new Exception("Ayarlar_Üst == null || Ayarlar_Üst.KullanıcılarVeParolalar == null");

            _Ayarlar_Üst_.Tümİzinler = Tümİzinler;
            _Ayarlar_Üst_.İzin_AyarlardaDeğişiklikYapabilir = İzin_AyarlardaDeğişiklikYapabilir;
            _Ayarlar_Üst_.AyarlarDosyaYolu = AyarlarDosyaYolu;
            _Ayarlar_Üst_.SihirliKelime = SihirliKelime ?? "ArGeMuP Kullanıcılar_Ayarlar_";
            _Ayarlar_Üst_.GeriBildirimİşlemi_Ayarlar_Değişti = GeriBildirimİşlemi_Ayarlar_Değişti;
            _Ayarlar_Üst_.ParolaKontrolüGerekiyorMu = _Ayarlar_Üst_.KullanıcılarVeParolalar.Count > 0;

            if (!_Ayarlar_Üst_.ParolaKontrolüGerekiyorMu) _Ayarlar_Üst_.Ayarlar_Alt_Araİşlemler(null);
        }
        public static Form Önyüz_Giriş(GeriBildirimİşlemi_Önyüz_Giriş GeriBildirimİşlemi, bool Küçültülmüş = false, float KarakterKümesi_Büyüklüğü = 0)
        {
            if (GeriBildirimİşlemi == null) throw new ArgumentException("GeriBildirimİşlemi_Başarılı == null");
            if (_Ayarlar_Üst_ == null) throw new Exception("Ayarlar_Üst == null");

            _Ayarlar_Üst_.KökParola = null;
            if (!_Ayarlar_Üst_.ParolaKontrolüGerekiyorMu) { GeriBildirimİşlemi(true); return null; }
            _Ayarlar_Üst_.Ayarlar_Alt = null;

            Kullanıcılar_ÖnYüz_ ÖnYüz = new Kullanıcılar_ÖnYüz_();
            ÖnYüz.FormClosed += (a,b) => { GeriBildirimİşlemi(_Ayarlar_Üst_.KökParola.DoluMu(true) && _Ayarlar_Üst_.Ayarlar_Alt != null && _Ayarlar_Üst_.GeçerliKullanıcı != null); };
            if (Küçültülmüş) ÖnYüz.WindowState = FormWindowState.Minimized;
            if (KarakterKümesi_Büyüklüğü > 0) ÖnYüz.Font = new System.Drawing.Font(ÖnYüz.Font.FontFamily, KarakterKümesi_Büyüklüğü);
            ÖnYüz.Başlat(Kullanıcılar_ÖnYüz_.İşlemTürü_.Giriş);
            ÖnYüz.Show();
            return ÖnYüz;
        }
        public static Form Önyüz_Ayarlar(float KarakterKümesi_Büyüklüğü = 0)
        {
            if (_Ayarlar_Üst_ == null) throw new Exception("Ayarlar_Üst == null");

            Kullanıcılar_ÖnYüz_ ÖnYüz = new Kullanıcılar_ÖnYüz_();
            if (KarakterKümesi_Büyüklüğü > 0) ÖnYüz.Font = new System.Drawing.Font(ÖnYüz.Font.FontFamily, KarakterKümesi_Büyüklüğü);
            ÖnYüz.Başlat(Kullanıcılar_ÖnYüz_.İşlemTürü_.Ayarlar);
            ÖnYüz.Show();

            return ÖnYüz;
        }
        public static Form Önyüz_ParolaDeğiştir(float KarakterKümesi_Büyüklüğü = 0)
        {
            if (_Ayarlar_Üst_ == null || _Ayarlar_Üst_.GeçerliKullanıcı == null) throw new Exception("Ayarlar_Üst(" + (_Ayarlar_Üst_ == null) + ") == null || Ayarlar_Üst.GeçerliKullanıcı(" + (_Ayarlar_Üst_.GeçerliKullanıcı == null) + ") == null");

            Kullanıcılar_ÖnYüz_ ÖnYüz = new Kullanıcılar_ÖnYüz_();
            if (KarakterKümesi_Büyüklüğü > 0) ÖnYüz.Font = new System.Drawing.Font(ÖnYüz.Font.FontFamily, KarakterKümesi_Büyüklüğü);
            ÖnYüz.Başlat(Kullanıcılar_ÖnYüz_.İşlemTürü_.ParolaDeğiştirme);
            ÖnYüz.Show();

            return ÖnYüz;
        }
        public static bool İzinliMi(Enum İzin)
        {
            if (!_Ayarlar_Üst_.ParolaKontrolüGerekiyorMu) return true;
            
            if (_Ayarlar_Üst_.GeçerliKullanıcı == null) return false;

            return _Ayarlar_Üst_.GeçerliKullanıcı.İzinliMi(İzin);
        }
        public static bool İzinliMi(IEnumerable<Enum> İzinler, bool Veya_1_Ve_0 = true)
        {
            if (!_Ayarlar_Üst_.ParolaKontrolüGerekiyorMu) return true;

            if (_Ayarlar_Üst_.GeçerliKullanıcı == null) return false;

            return _Ayarlar_Üst_.GeçerliKullanıcı.İzinliMi(İzinler, Veya_1_Ve_0);
        }

        #region İç Kullanım
        public static Ayarlar_Üst_ _Ayarlar_Üst_;
        public class Ayarlar_Üst_
        {
            [Değişken_.Niteliği.Adını_Değiştir("K")] public Dictionary<string, string> KullanıcılarVeParolalar = new Dictionary<string, string>(); //Sadece parolası olan kullanıcıları tutar
            [Değişken_.Niteliği.Adını_Değiştir("A")] public string Ayarlar_Alt_Yazı;

            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public string KökParola = null;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public bool ParolaKontrolüGerekiyorMu = true;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public string SihirliKelime;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public string AyarlarDosyaYolu;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public Ayarlar_Kullanıcı_ GeçerliKullanıcı;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public IEnumerable<Enum> Tümİzinler; //anlamlı eleman / enum -> "abc_def" / enum.Yazdır -> "abc def" | anlamsız eleman / enum -> "abc_def_" / enum.Yazdır -> "abc def "
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public Enum İzin_AyarlardaDeğişiklikYapabilir;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public Ayarlar_Alt_ Ayarlar_Alt;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public GeriBildirimİşlemi_Önyüz_Ayarlar_Değişti GeriBildirimİşlemi_Ayarlar_Değişti;
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] int HatalıGirişDenemesi_Sabiti = Rastgele.Sayı(4, 8);
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] int HatalıGirişDenemesi_Sayısı;

            #region Yardımcı Sınıflar
            public class Ayarlar_Alt_
            {
                [Değişken_.Niteliği.Adını_Değiştir("K")] public List<Ayarlar_Kullanıcı_> Kişiler = new List<Ayarlar_Kullanıcı_>(); //Tüm kullanıcıları tutar
                [Değişken_.Niteliği.Adını_Değiştir("R")] public Dictionary<string, bool[]> Roller = new Dictionary<string, bool[]>();
            }
            public class Ayarlar_Kullanıcı_
            {
                [Değişken_.Niteliği.Adını_Değiştir("K", 0)] public string Adı;
                [Değişken_.Niteliği.Adını_Değiştir("K", 1)] public string RolAdı;

                [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public bool[] Rol_İzinleri;

                public bool İzinliMi(Enum İzin)
                {
                    return İzin != null && Rol_İzinleri[Convert.ToInt32(İzin)];
                }
                public bool İzinliMi(IEnumerable<Enum> İzinler, bool _1_Veya_0_Ve)
                {
                    if (İzinler == null) return false;

                    if (_1_Veya_0_Ve)
                    {
                        foreach (Enum izin in İzinler)
                        {
                            if (İzinliMi(izin)) return true;
                        }

                        return false;
                    }
                    else
                    {
                        foreach (Enum izin in İzinler)
                        {
                            if (!İzinliMi(izin)) return false;
                        }

                        return true;
                    }
                }
            }
            #endregion
            #region İşlemler
            [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] static Değişken_ Değişken = new Değişken_() { Filtre_BoşVeyaVarsayılanDeğerdeİse_HariçTut = true };
            public static object Sınıf_Oluştur(Type Tipi, string İçerik)
            {
                Depo_ Depo = new Depo_(İçerik);
                object sınıf = Değişken.Üret(Tipi, Depo["ArGeMuP"]);

                return sınıf;
            }
            public static string Sınıf_Kaydet(object Sınıf)
            {
                if (Sınıf == null) throw new Exception("Sınıf == null");

                Depo_ Depo = new Depo_();
                Değişken.Depola(Sınıf, Depo["ArGeMuP"]);

                return Depo.YazıyaDönüştür();
            }
            public void Ayarlar_Alt_Araİşlemler(string Kök_Parola)
            {
                int adet_tüm_izinler = Tümİzinler.Count();

                if (Ayarlar_Alt == null)
                {
                    string ayrlr = Ayarlar_Alt_Yazı.Taban64ten().Yazıya();
                    if (ParolaKontrolüGerekiyorMu) ayrlr = ayrlr.Düzelt(Kök_Parola);

                    Ayarlar_Alt = Sınıf_Oluştur(typeof(Ayarlar_Alt_), ayrlr) as Ayarlar_Alt_;
                }

                //İzin disizi boyut miktarı kontrolü
                int adet_roller = Ayarlar_Alt.Roller.Count;
                for (int i = 0; i < adet_roller; i++)
                {
                    KeyValuePair<string, bool[]> Rol = Ayarlar_Alt.Roller.ElementAt(i);

                    if (Rol.Value.Length != adet_tüm_izinler)
                    {
                        bool[] Rol_Dizisi = Rol.Value;
                        Array.Resize(ref Rol_Dizisi, adet_tüm_izinler);
                        Ayarlar_Alt.Roller[Rol.Key] = Rol_Dizisi;
                    }
                }

                //Kullanıcıların izinlerinin oluşturulması
                foreach (Ayarlar_Kullanıcı_ Kişi in Ayarlar_Alt.Kişiler)
                {
                    if (Kişi.RolAdı.DoluMu(true))
                    {
                        if (!Ayarlar_Alt.Roller.TryGetValue(Kişi.RolAdı, out Kişi.Rol_İzinleri)) Kişi.Rol_İzinleri = new bool[adet_tüm_izinler];
                    }
                }
            }
            public bool Ayarlar_DosyayaKaydet(string Eski_KökParola)
            {
                //Son yapılan ayarlama ile ile durumu güncelle
                ParolaKontrolüGerekiyorMu = KullanıcılarVeParolalar.Count > 0;
                Ayarlar_Alt_Araİşlemler(KökParola);

                //son yapılan ayarlama uygun mu kontrolü
                if (ParolaKontrolüGerekiyorMu)
                {
                    int AyarlarıDeğiştirebilenKullanıcıSayısı = Ayarlar_Alt.Kişiler.Where(x => KullanıcılarVeParolalar.ContainsKey(x.Adı) && x.İzinliMi(İzin_AyarlardaDeğişiklikYapabilir)).Count();
                    if (AyarlarıDeğiştirebilenKullanıcıSayısı < 1)
                    {
                        MessageBox.Show("Son değişiklik ile hiçbir kullanıcı bu sayfaya ulaşamayacak." +
                            Environment.NewLine + Environment.NewLine +
                            "Öncelikle ayarları değiştirebilme hakkına sahip bir rol oluşturun" + Environment.NewLine +
                            "Sonra bu rolu parolası olan bir kulanıcıya eşleyin." +
                            Environment.NewLine + Environment.NewLine +
                            "Eğer parolası olan tek bir yönetici var ve onun adını değiştirmek istiyorsanız" + Environment.NewLine +
                            "Öncelikle yeni ad ile yeni bir yönetici oluşturunuz ve parolasını belirleyiniz." + Environment.NewLine +
                            "Sonra eski yöneticiyi siliniz.",
                            "İşlem iptal edildi");

                        return false;
                    }
                }

                /* "Kullanıcılar.AyarlarDosyaYolu" içerigini "Kullanıcılar.Ayarlar_Üst" sınıfını oluşturmak için kullan
                    "Kullanıcılar.Ayarlar_Üst.KullanıcılarVeParolalar" -> Kullanıcı adı / Kök parolanın şifrelenmiş hali - Şifre -> AES(SHA(Kullanıcı adı + SHA(Kullanıcı parolası) + Sihirli kelime))
                    "Kullanıcılar.Ayarlar_Üst.Ayarlar_Alt_Yazı" dönüştür | girdi -> taban64 ten bayt dizisine -> yazıya
                      Kök parolayı "Kullanıcılar.Ayarlar_Üst.Ayarlar_Alt_Yazı" şifresini çözmek için kullan
                       Elde edilen içeriği "Kullanıcılar.Ayarlar_Üst.Ayarlar_Alt" sınıfı oluşturmak için kullan */
                Ayarlar_Alt_Yazı = Sınıf_Kaydet(Ayarlar_Alt);
                if (KökParola.DoluMu(true)) Ayarlar_Alt_Yazı = Ayarlar_Alt_Yazı.Karıştır(KökParola);
                Ayarlar_Alt_Yazı = Ayarlar_Alt_Yazı.BaytDizisine().Taban64e();
                string içerik_ayarlar_üst = Sınıf_Kaydet(this);
                AyarlarDosyaYolu.DosyaYolu_Yaz(içerik_ayarlar_üst);

                GeriBildirimİşlemi_Ayarlar_Değişti(AyarlarDosyaYolu, KökParola, Eski_KökParola);
                return true;
            }
            
            public void Kullanıcı_Ekle(string KullanıcıAdı)
            {
                if (Ayarlar_Alt == null || KullanıcıAdı.BoşMu(true)) throw new Exception("Ayarlar_Alt(" + (Ayarlar_Alt == null) + ") == null || KullanıcıAdı.BoşMu(true) " + KullanıcıAdı);

                Ayarlar_Alt.Kişiler.Add(new Ayarlar_Kullanıcı_() { Adı = KullanıcıAdı });
            }
            public void Kullanıcı_AdınıDeğiştir(string Eski_KullanıcıAdı, string Yeni_KullanıcıAdı)
            {
                if (Ayarlar_Alt == null || Eski_KullanıcıAdı.BoşMu(true) || Yeni_KullanıcıAdı.BoşMu(true)) throw new Exception("Ayarlar_Alt(" + (Ayarlar_Alt == null) + ") == null || Eski_KullanıcıAdı.BoşMu(true) " + Eski_KullanıcıAdı + " || Yeni_KullanıcıAdı.BoşMu(true) " + Yeni_KullanıcıAdı);

                Ayarlar_Alt.Kişiler.Where(x => x.Adı == Eski_KullanıcıAdı).ToList().ForEach(x => x.Adı = Yeni_KullanıcıAdı);
                Kullanıcı_Sil(Eski_KullanıcıAdı);
            }
            public void Kullanıcı_Sil(string KullanıcıAdı)
            {
                if (Ayarlar_Alt == null || KullanıcıAdı.BoşMu(true)) throw new Exception("Ayarlar_Alt(" + (Ayarlar_Alt == null) + ") == null || KullanıcıAdı.BoşMu(true) " + KullanıcıAdı);

                Ayarlar_Kullanıcı_ Kullanıcı = Ayarlar_Alt.Kişiler.FirstOrDefault(x => x.Adı == KullanıcıAdı);
                Ayarlar_Alt.Kişiler.Remove(Kullanıcı);

                Parola_EkleDeğiştirSil(KullanıcıAdı, null);
            }
            public void Rol_Ekle(string RolAdı)
            {
                if (Ayarlar_Alt == null || RolAdı.BoşMu(true)) throw new Exception("Ayarlar_Alt(" + (Ayarlar_Alt == null) + ") == null || RolAdı.BoşMu(true) " + RolAdı);

                Ayarlar_Alt.Roller.Add(RolAdı, new bool[Tümİzinler.Count()]);
            }
            public void Rol_AdınıDeğiştir(string Eski_RolAdı, string Yeni_RolAdı)
            {
                if (Ayarlar_Alt == null || Eski_RolAdı.BoşMu(true) || Yeni_RolAdı.BoşMu(true)) throw new Exception("Ayarlar_Alt(" + (Ayarlar_Alt == null) + ") == null || Eski_RolAdı.BoşMu(true) " + Eski_RolAdı + " || Yeni_RolAdı.BoşMu(true) " + Yeni_RolAdı);

                bool[] içerik = Ayarlar_Alt.Roller[Eski_RolAdı];
                Ayarlar_Alt.Roller.Remove(Eski_RolAdı);
                Ayarlar_Alt.Roller.Add(Yeni_RolAdı, içerik);
                
                Ayarlar_Alt.Kişiler.Where(x => x.RolAdı == Eski_RolAdı).ToList().ForEach(x => x.RolAdı = Yeni_RolAdı);
            }
            public void Rol_Sil(string RolAdı)
            {
                if (Ayarlar_Alt == null || RolAdı.BoşMu(true)) throw new Exception("Ayarlar_Alt(" + (Ayarlar_Alt == null) + ") == null || RolAdı.BoşMu(true) " + RolAdı);

                Ayarlar_Alt.Roller.Remove(RolAdı);
                Ayarlar_Alt.Kişiler.Where(x => x.RolAdı == RolAdı).ToList().ForEach(x => x.RolAdı = null);
            }
            public void Parola_EkleDeğiştirSil(string KullanıcıAdı, string Parola)
            {
                if (KullanıcıAdı.BoşMu(true)) throw new Exception("KullanıcıAdı.BoşMu(true) " + KullanıcıAdı);

                if (Parola.BoşMu(true))
                {
                    //sil
                    KullanıcılarVeParolalar.Remove(KullanıcıAdı);

                    if (KullanıcılarVeParolalar.Count() <= 0) KökParola = null;
                }
                else
                {
                    //ekle değiştir
                    if (KökParola.BoşMu(true)) KökParola = Rastgele.Yazı(64);

                    string kullanıcı_parolası = ArgeMup.HazirKod.Dönüştürme.D_GeriDönülemezKarmaşıklaştırmaMetodu.Yazıdan(Parola, 32);
                    kullanıcı_parolası = KullanıcıAdı + kullanıcı_parolası + SihirliKelime;
                    kullanıcı_parolası = ArgeMup.HazirKod.Dönüştürme.D_GeriDönülemezKarmaşıklaştırmaMetodu.Yazıdan(kullanıcı_parolası, 32);

                    KullanıcılarVeParolalar[KullanıcıAdı] = KökParola.Karıştır(kullanıcı_parolası);
                }
            }
            public bool Parola_Kontrol(string KullanıcıAdı, string Parola)
            {
                if (HatalıGirişDenemesi_Sayısı < HatalıGirişDenemesi_Sabiti &&
                    KullanıcıAdı.DoluMu(true) &&
                    Parola.DoluMu(true) &&
                    KullanıcılarVeParolalar.TryGetValue(KullanıcıAdı, out string KaydedilenKökParola))
                {
                    string kullanıcı_parolası = ArgeMup.HazirKod.Dönüştürme.D_GeriDönülemezKarmaşıklaştırmaMetodu.Yazıdan(Parola, 32);
                    kullanıcı_parolası = KullanıcıAdı + kullanıcı_parolası + SihirliKelime;
                    kullanıcı_parolası = ArgeMup.HazirKod.Dönüştürme.D_GeriDönülemezKarmaşıklaştırmaMetodu.Yazıdan(kullanıcı_parolası, 32);

                    string oluşturulan_kök_parola = KaydedilenKökParola.Düzelt(kullanıcı_parolası);
                    if (oluşturulan_kök_parola.DoluMu(true))
                    {
                        Ayarlar_Alt_Araİşlemler(oluşturulan_kök_parola);

                        GeçerliKullanıcı = Ayarlar_Alt.Kişiler.FirstOrDefault(x => x.Adı == KullanıcıAdı);
                        if (GeçerliKullanıcı != null)
                        {
                            KökParola = oluşturulan_kök_parola;
                            HatalıGirişDenemesi_Sayısı = 0;
                            return true;
                        }
                    }
                }

#if DEBUG
                System.Threading.Thread.Sleep(Rastgele.Sayı(500, 5500));
#endif
                HatalıGirişDenemesi_Sayısı++;
                return false;
            }
            #endregion
        }        
        #endregion
    }
}
#endif