// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_Görsel
using ArgeMup.HazirKod.Ekİşlemler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ArgeMup.HazirKod.Ekranlar
{
    public partial class Kullanıcılar : UserControl
    {
        public const string Sürüm = "V1.0";
        public event GeriBildirim_DeğişiklikleriKaydet_ GeriBildirim_Değişiklikleri_Kaydet;
        public delegate void GeriBildirim_DeğişiklikleriKaydet_();
        public event GeriBildirim_GirişBaşarılı_ GeriBildirim_GirişBaşarılı;
        public delegate void GeriBildirim_GirişBaşarılı_();
        public enum İşlemTürü_ { Boşta, Giriş, ParolaDeğiştirme, Ayarlar };

        Kullanıcılar_Ayarlar_ Ayarlar_Kullanıcılar;
        ListeKutusu Kullanıcılar_Liste;
        ListeKutusu Roller_Liste;

        public Kullanıcılar()
        {
            InitializeComponent();

            Kullanıcılar_Liste = new ListeKutusu();
            Kullanıcılar_Liste.Dock = DockStyle.Fill;
            splitContainer2.Panel1.Controls.Add(Kullanıcılar_Liste);

            Roller_Liste = new ListeKutusu();
            Roller_Liste.Dock = DockStyle.Fill;
            splitContainer1.Panel1.Controls.Add(Roller_Liste);
        }
        public void Başlat(İşlemTürü_ İşlemTürü, List<string> İzinler, Kullanıcılar_Ayarlar_ Kullanıcılar)
        {
            Ayarlar_Kullanıcılar = Kullanıcılar;

            List<string> Kişiler_Yazı = new List<string>();
            Kullanıcılar.Kişiler.ForEach(x => Kişiler_Yazı.Add(x.Adı));

            switch (İşlemTürü)
            {
                default:
                case İşlemTürü_.Giriş:
                    Ayarlar_Kullanıcılar.GeçerliKullanıcı = null;
                    Ekran_Giriş_Kullanıcı.Items.AddRange(Kişiler_Yazı.ToArray());
                    Ekran_Giriş_Kullanıcı.DroppedDown = true;

                    Ekran_Giriş.Dock = DockStyle.Fill;
                    Ekran_Giriş.Visible = true;
                    break;

                case İşlemTürü_.ParolaDeğiştirme:
                    Ekran_Giriş_Kullanıcı.Items.Add(Ayarlar_Kullanıcılar.GeçerliKullanıcı.Adı);
                    Ekran_Giriş_Kullanıcı.SelectedIndex = 0;
                    Ekran_Giriş_Kullanıcı.Enabled = false;
                    Ekran_Giriş_Parola.Focus();

                    Ekran_Giriş_YeniParola_1.Visible = true;
                    Ekran_Giriş_YeniParola_2.Visible = true;
                    Ekran_Giriş_YeniParola.Visible = true;
                    Ekran_Giriş_YeniParolaTekrar.Visible = true;
                    Ekran_Giriş_Tamam.Text = "Kaydet";
                    Ekran_Giriş.Dock = DockStyle.Fill;
                    Ekran_Giriş.Visible = true;
                    break;

                case İşlemTürü_.Ayarlar:
                    ListeKutusu.Ayarlar_ ListeKutusu_Ayarlar = new ListeKutusu.Ayarlar_(true, true, false, true, false, true);
                    Kullanıcılar_Liste.Başlat(null, Kişiler_Yazı, "Kullamıcılar", ListeKutusu_Ayarlar);
                    Kullanıcılar_Liste.GeriBildirim_İşlemi += Kullanıcılar_Liste_GeriBildirim_İşlemi;

                    Roller_Liste.Başlat(null, Kullanıcılar.Roller.Keys.ToList(), "Roller", ListeKutusu_Ayarlar);
                    Roller_Liste.GeriBildirim_İşlemi += Roller_Liste_GeriBildirim_İşlemi;
                    Kullanıcılar_Rol.Items.Clear();
                    Kullanıcılar_Rol.Items.AddRange(Kullanıcılar.Roller.Keys.ToArray());

                    Roller_Tablo.Rows.Clear();
                    for (int i = 0; i < İzinler.Count; i++)
                    {
                        string İzin = İzinler[i];
                        if (İzin.BoşMu() || İzin == "Boşta") continue;

                        int SatırNo = Roller_Tablo.Rows.Add(new object[] { İzin, false });
                        Roller_Tablo.Rows[SatırNo].Tag = i;
                    }

                    Uyarı.Visible = !Kullanıcılar.ParolaKontrolüGerekiyorMu;

                    Ekran_Ayarlar.Dock = DockStyle.Fill;
                    Ekran_Ayarlar.Visible = true;
                    break;
            }
        }
        Ayarlar_Kullanıcı_ Bul_Kullanıcı(string Adı)
        {
            return Ayarlar_Kullanıcılar.Kişiler.FirstOrDefault(x => x.Adı == Adı);
        }
        bool[] Bul_Rol(string Adı)
        {
            return Ayarlar_Kullanıcılar.Roller.FirstOrDefault(x => x.Key == Adı).Value;
        }

        #region Ekran Ayarlar
        void Ayarlar_Kaydet()
        {
            Ayarlar_Kullanıcılar.Başlat();
            GeriBildirim_Değişiklikleri_Kaydet?.Invoke();

            Uyarı.Visible = !Ayarlar_Kullanıcılar.ParolaKontrolüGerekiyorMu;
            ÖnYüzler_Kaydet_Kullanıcılar.Enabled = false;
            ÖnYüzler_Kaydet_Roller.Enabled = false;
        }

        #region Kullanıcılar
        private bool Kullanıcılar_Liste_GeriBildirim_İşlemi(string Adı, ListeKutusu.İşlemTürü Türü, string YeniAdı = null)
        {
            Ayarlar_Kullanıcı_ Kullanıcı;

            if (Adı.DoluMu())
            {
                switch (Türü)
                {
                    case ListeKutusu.İşlemTürü.ElemanSeçildi:
                        Kullanıcı = Bul_Kullanıcı(Adı);

                        Kullanıcılar_Parola.Text = Kullanıcı.Parolası;
                        Kullanıcılar_Rol.Text = Kullanıcı.RolAdı;
                        ÖnYüzler_Kaydet_Kullanıcılar.Enabled = false;
                        return true;

                    case ListeKutusu.İşlemTürü.YeniEklendi:
                        Ayarlar_Kullanıcılar.Kişiler.Add(new Ayarlar_Kullanıcı_() { Adı = Adı });
                        Ayarlar_Kaydet();
                        return true;

                    case ListeKutusu.İşlemTürü.AdıDeğiştirildi:
                        Kullanıcı = Bul_Kullanıcı(Adı);
                        Kullanıcı.Adı = YeniAdı;
                        Ayarlar_Kaydet();
                        return true;

                    case ListeKutusu.İşlemTürü.Silindi:
                        Kullanıcı = Bul_Kullanıcı(Adı);
                        Ayarlar_Kullanıcılar.Kişiler.Remove(Kullanıcı);
                        Ayarlar_Kaydet();
                        return true;
                }
            }

            return false;
        }
        private void Kullanıcılar_AyarDeğişti(object sender, EventArgs e)
        {
            ÖnYüzler_Kaydet_Kullanıcılar.Enabled = true;
        }
        private void ÖnYüzler_Kaydet_Kullanıcılar_Click(object sender, EventArgs e)
        {
            if (Kullanıcılar_Liste.SeçilenEleman_Adı.BoşMu()) return;

            if (Kullanıcılar_Parola.Text.BoşMu()) Kullanıcılar_Parola.Text = null;

            Ayarlar_Kullanıcı_ Kullanıcı = Bul_Kullanıcı(Kullanıcılar_Liste.SeçilenEleman_Adı);
            if (Kullanıcı == null)
            {
                Kullanıcı = new Ayarlar_Kullanıcı_();
                Kullanıcı.Adı = Kullanıcılar_Liste.SeçilenEleman_Adı;

                Ayarlar_Kullanıcılar.Kişiler.Add(Kullanıcı);
            }

            Kullanıcı.Parolası = Kullanıcılar_Parola.Text;
            Kullanıcı.RolAdı = Kullanıcılar_Rol.Text;

            Ayarlar_Kaydet();
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
                        Ayarlar_Kullanıcılar.Roller.Add(Adı, new bool[Ayarlar_Kullanıcılar.İzinDizisiElemanSayısı]);
                        Ayarlar_Kaydet();
                        return true;

                    case ListeKutusu.İşlemTürü.AdıDeğiştirildi:
                        Rol_Dizisi = Bul_Rol(Adı);
                        Ayarlar_Kullanıcılar.Roller.Remove(Adı);
                        Ayarlar_Kullanıcılar.Roller.Add(YeniAdı, Rol_Dizisi);
                        Ayarlar_Kaydet();
                        return true;

                    case ListeKutusu.İşlemTürü.Silindi:
                        Ayarlar_Kullanıcılar.Roller.Remove(Adı);
                        Ayarlar_Kaydet();
                        return true;
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

            if (!Ayarlar_Kullanıcılar.Roller.TryGetValue(Roller_Liste.SeçilenEleman_Adı, out bool[] Rol_Dizisi))
            {
                Rol_Dizisi = new bool[Ayarlar_Kullanıcılar.İzinDizisiElemanSayısı];
            }

            foreach (DataGridViewRow satır in Roller_Tablo.Rows)
            {
                Rol_Dizisi[(int)satır.Tag] = (bool)satır.Cells[Roller_Tablo_Etkin.Index].Value;
            }

            Ayarlar_Kullanıcılar.Roller[Roller_Liste.SeçilenEleman_Adı] = Rol_Dizisi;
            Ayarlar_Kaydet();
        }
        #endregion
        #endregion

        #region Ekran Giriş
        private void Ekran_Giriş_Tamam_Click(object sender, EventArgs e)
        {
            if ((sender as Button).Text == "Tamam")
            {
                //Giriş
                if (Ayarlar_Kullanıcılar.ParolaKontrol(Ekran_Giriş_Kullanıcı.Text, Ekran_Giriş_Parola.Text))
                {
                    GeriBildirim_GirişBaşarılı?.Invoke();
                }
            }
            else
            {
                //Parola değiştirme
                Ekran_Giriş_YeniParola.Text = Ekran_Giriş_YeniParola.Text.Trim();

                if (Ekran_Giriş_YeniParola.Text.Length < 4 ||
                    Ekran_Giriş_YeniParola.Text != Ekran_Giriş_YeniParolaTekrar.Text)
                {
                    MessageBox.Show(
                        "Alttaki kuralları sağladığınızı kontrol ediniz." + Environment.NewLine + Environment.NewLine +
                        "En az 4 karakter girilmeli" + Environment.NewLine +
                        "Boşluk karakteri olmamalı" + Environment.NewLine +
                        "Yeni parola ve tekrar parolası birbirinin aynısı olmalı",
                        "Parola Değiştirme Ekranı");
                    return;
                }

                Ayarlar_Kullanıcı_ Kullancı = Bul_Kullanıcı(Ekran_Giriş_Kullanıcı.Text);
                Kullancı.Parolası = Ekran_Giriş_YeniParola.Text;
                GeriBildirim_Değişiklikleri_Kaydet?.Invoke();
            }
        }
        #endregion
    }

    #region İç Kullanım Tanımlamaları
    public static class Ekİşlemler
    {
        public static string Yazdır(this Enum İzin)
        {
            return İzin.ToString().Replace('_', ' ');
        }
    }
    public class Kullanıcılar_Ayarlar_
    {
        [Değişken_.Niteliği.Adını_Değiştir("K")] public List<Ayarlar_Kullanıcı_> Kişiler = new List<Ayarlar_Kullanıcı_>();
        [Değişken_.Niteliği.Adını_Değiştir("R")] public Dictionary<string, bool[]> Roller = new Dictionary<string, bool[]>();

        [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public int İzinDizisiElemanSayısı;
        [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public Ayarlar_Kullanıcı_ GeçerliKullanıcı;
        [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public bool ParolaKontrolüGerekiyorMu;

        [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] int HatalıGirişDenemesi_Sabiti = Rastgele.Sayı(4, 8);
        [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] int HatalıGirişDenemesi_Sayısı = 0;

        public void Başlat(int İzinDizisiElemanSayısı = 0, bool TamKontrol = false)
        {
            if (İzinDizisiElemanSayısı > 0) this.İzinDizisiElemanSayısı = İzinDizisiElemanSayısı;

            if (TamKontrol)
            {
                foreach (KeyValuePair<string, bool[]> Rol in Roller)
                {
                    if (Rol.Value.Length != this.İzinDizisiElemanSayısı)
                    {
                        bool[] Rol_Dizisi = Rol.Value;
                        Array.Resize(ref Rol_Dizisi, this.İzinDizisiElemanSayısı);
                        Roller[Rol.Key] = Rol_Dizisi;
                    }
                }
            }

            foreach (Ayarlar_Kullanıcı_ Kullanıcı in Kişiler)
            {
                if (Kullanıcı.RolAdı.BoşMu() ||
                    !Roller.TryGetValue(Kullanıcı.RolAdı, out Kullanıcı.Rol_İzinleri)) Kullanıcı.Rol_İzinleri = new bool[this.İzinDizisiElemanSayısı];
            }

            ParolaKontrolüGerekiyorMu = _ParolaKontrolüGerekiyorMu_();
            bool _ParolaKontrolüGerekiyorMu_()
            {
                if (Kişiler.Count == 0) return false;

                int ParolaSahibiKullanıcıSayısı = Kişiler.Where(x => x.Parolası.DoluMu()).Count();
                return ParolaSahibiKullanıcıSayısı > 0;
            }
        }
        public bool ParolaKontrol(string KullanıcıAdı, string Parola)
        {
            GeçerliKullanıcı = null;
            if (!ParolaKontrolüGerekiyorMu) return true;
            else if (Parola.BoşMu(true)) return false;

            if (HatalıGirişDenemesi_Sayısı >= HatalıGirişDenemesi_Sabiti)
            {
                System.Threading.Thread.Sleep(Rastgele.Sayı(500, 5500));
            }
            else
            {
                GeçerliKullanıcı = Kişiler.FirstOrDefault(x => x.Adı == KullanıcıAdı && x.Parolası == Parola);

                if (GeçerliKullanıcı == null)
                {
                    HatalıGirişDenemesi_Sayısı++;
                    System.Threading.Thread.Sleep(Rastgele.Sayı(500, 5500));
                }
                else HatalıGirişDenemesi_Sayısı = 0;
            }

            return GeçerliKullanıcı != null;
        }
        public bool İzinliMi(Enum İzin)
        {
            if (ParolaKontrolüGerekiyorMu)
            {
                if (GeçerliKullanıcı == null) return false;

                return İzin == null ? true : GeçerliKullanıcı.İzinliMi(İzin);
            }

            return true;
        }
        public bool İzinliMi(IEnumerable<Enum> İzinler)
        {
            foreach (Enum izin in İzinler)
            {
                if (İzinliMi(izin)) return true;
            }

            return false;
        }
        public string KullanıcıAdı
        {
            get
            {
                if (ParolaKontrolüGerekiyorMu)
                {
                    if (GeçerliKullanıcı == null) return null;

                    return GeçerliKullanıcı.Adı;
                }

                return null;
            }
        }
    }
    public class Ayarlar_Kullanıcı_
    {
        [Değişken_.Niteliği.Adını_Değiştir("K", 0)] public string Adı;
        [Değişken_.Niteliği.Adını_Değiştir("K", 1)] public string Parolası;
        [Değişken_.Niteliği.Adını_Değiştir("K", 2)] public string RolAdı;

        [Değişken_.Niteliği.Bunu_Kesinlikle_Kullanma] public bool[] Rol_İzinleri;
        public bool İzinliMi(Enum İzin)
        {
            return Rol_İzinleri[Convert.ToInt32(İzin)];
        }
        public bool İzinliMi(IEnumerable<Enum> İzinler)
        {
            foreach (Enum izin in İzinler)
            {
                if (İzinliMi(izin)) return true;
            }

            return false;
        }
    }
    #endregion
}
#endif