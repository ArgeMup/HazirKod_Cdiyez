// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

#if HazirKod_Cdiyez_Görsel
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ArgeMup.HazirKod.Ekİşlemler;

namespace ArgeMup.HazirKod
{
    public partial class ListeKutusu : UserControl
    {
        public const string Sürüm = "V1.0";
        public event GeriBildirim_İşlemi_ GeriBildirim_İşlemi;
        public int SeçilenEleman_SıraNo
        {
            get
            {
                if (!SeçimKutusu.Enabled || SeçimKutusu.SelectedIndex < 0 || SeçimKutusu.Text.BoşMu() || SeçimKutusu.SelectedItems.Count != 1) return -1;

                return SeçimKutusu.SelectedIndex;
            }
        }
        public string SeçilenEleman_Adı
        {
            get
            {
                int SıraNo = SeçilenEleman_SıraNo;
                return SıraNo >= 0 ? SeçimKutusu.Items[SıraNo] as string : null;
            }
            set
            {
                if (Tüm_Elemanlar.Contains(value))
                {
                    if (!SeçimKutusu.Items.Contains(value)) SeçimKutusu.Items.Add(value);
                    SeçimKutusu.Text = value;
                }
            }
        }
        public List<string> SeçilenEleman_Adları
        {
            get
            {
                List<string> l = new List<string>();
                foreach (string eleman in SeçimKutusu.SelectedItems)
                {
                    l.Add(eleman);
                }

                return l;
            }
        }
        public List<int> SeçilenEleman_SıraNoları
        {
            get
            {
                List<int> l = new List<int>();
                foreach (int eleman in SeçimKutusu.SelectedIndices)
                {
                    l.Add(eleman);
                }

                return l;
            }
        }
        public string GizliElemanBaşlangıcı;
        public List<string> Sabit_Elemanlar, Tüm_Elemanlar;
        public enum İşlemTürü { YeniEklendi, ElemanSeçildi, AdıDeğiştirildi, KonumuDeğiştirildi, Gizlendi, GörünürDurumaGetirildi, Silindi };

        public delegate bool GeriBildirim_İşlemi_(string Adı, İşlemTürü Türü, string YeniAdı = null);
        bool Eklenebilir, AdıDeğiştirilebilir, KonumuDeğiştirilebilir, Silinebilir, Gizlenebilir, İşlemYapmadanÖnceSor, İşlemYapmadanÖnceSor_KonumDeğiştirme, İşlemYaptıktanSonraSeç;
        string ListeninAçıklaması;
        string[] Yasakİçerik = null;

        public ListeKutusu()
        {
            InitializeComponent();

            Başlat();
        }
        private void ListeKutusu_ParentChanged(object sender, EventArgs e)
        {
            Tabla_ÜstTuşlar.Height = Font.Height * 2;
            Tabla_AltTuşlar.Height = Tabla_ÜstTuşlar.Height;
        }

        public void Başlat(List<string> Sabit_Elemanlar = null, List<string> Diğer_Elemanlar = null, string ListeninAçıklaması = null,
            bool Eklenebilir = true, bool AdıDeğiştirilebilir = true, bool KonumuDeğiştirilebilir = true, bool Silinebilir = true,
            bool Gizlenebilir = true, bool GizliOlanlarıGöster = true, string GizliElemanBaşlangıcı = ".:Gizli:. ",
            string[] Yasakİçerik = null, bool ÇokluSeçim = false, bool İşlemYaptıktanSonraSeç = true,
            bool İşlemYapmadanÖnceSor = true, bool İşlemYapmadanÖnceSor_KonumDeğiştirme = false)
        {
            if (Gizlenebilir || GizliOlanlarıGöster)
            {
                if (GizliElemanBaşlangıcı.BoşMu()) throw new Exception("GizliElemanBaşlangıcı boş olamaz");
            }

            this.Sabit_Elemanlar = Sabit_Elemanlar == null || Sabit_Elemanlar.Count == 0 ? null : Sabit_Elemanlar;
            this.Eklenebilir = Eklenebilir;
            this.AdıDeğiştirilebilir = AdıDeğiştirilebilir;
            this.KonumuDeğiştirilebilir = KonumuDeğiştirilebilir;
            this.Silinebilir = Silinebilir;
            this.Gizlenebilir = Gizlenebilir;
            this.İşlemYapmadanÖnceSor = İşlemYapmadanÖnceSor;
            this.İşlemYapmadanÖnceSor_KonumDeğiştirme = İşlemYapmadanÖnceSor_KonumDeğiştirme;
            this.İşlemYaptıktanSonraSeç = İşlemYaptıktanSonraSeç;
            this.ListeninAçıklaması = ListeninAçıklaması;
            this.GizliElemanBaşlangıcı = GizliElemanBaşlangıcı;
            this.Yasakİçerik = Yasakİçerik;

            Adı.Visible = Eklenebilir || AdıDeğiştirilebilir;

            Tüm_Elemanlar = new List<string>();
            if (this.Sabit_Elemanlar != null) { this.Sabit_Elemanlar = this.Sabit_Elemanlar.Distinct().ToList(); Tüm_Elemanlar.AddRange(this.Sabit_Elemanlar); }
            if (Diğer_Elemanlar != null) Tüm_Elemanlar.AddRange(Diğer_Elemanlar);
            Tüm_Elemanlar = Tüm_Elemanlar.Distinct().ToList();
            if (!GizliOlanlarıGöster) Tüm_Elemanlar = Tüm_Elemanlar.Where(x => !x.StartsWith(GizliElemanBaşlangıcı)).ToList();

            SeçimKutusu.SelectionMode = ÇokluSeçim ? SelectionMode.MultiExtended : SelectionMode.One;
            İpucunuDüzenle();
            Sırala();

            AramaÇubuğu_TextChanged(null, null);
        }

        private void AramaÇubuğu_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                if (SeçimKutusu.Items.Count > 0)
                {
                    SeçimKutusu.SelectedIndex = 0;
                    SeçimKutusu.Focus();
                }
            }
        }
        private void AramaÇubuğu_TextChanged(object sender, EventArgs e)
        {
            Filtrele(SeçimKutusu, Tüm_Elemanlar, AramaÇubuğu.Text);
            TuşlarıDüzenle();

            AramaÇubuğunuTemizle.Visible = AramaÇubuğu.Text.DoluMu(true);
            SeçimKutusu_TümünüSeç.Visible = SeçimKutusu.SelectionMode != SelectionMode.One && SeçimKutusu.Items.Count > 0;
        }
        private void AramaÇubuğunuTemizle_Click(object sender, EventArgs e)
        {
            AramaÇubuğu.Text = null;
        }

        private void SeçimKutusu_TümünüSeç_Click(object sender, EventArgs e)
        {
            if (SeçilenEleman_SıraNoları.Count > 0) SeçimKutusu.ClearSelected();
            else
            {
                for (int i = 0; i < SeçimKutusu.Items.Count; i++)
                {
                    SeçimKutusu.SetSelected(i, true);
                }
            }
        }
        private void SeçimKutusu_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                AramaÇubuğu.Focus();
            }
        }
        private void SeçimKutusu_SelectedIndexChanged(object sender, EventArgs e)
        {
            string adı = SeçilenEleman_Adı;

            bool? sonuç = GeriBildirim_İşlemi?.Invoke(adı, İşlemTürü.ElemanSeçildi);
            if (sonuç != null && !sonuç.Value) SeçimKutusu.ClearSelected();

            if (Adı.Text == adı) TuşlarıDüzenle();
            else Adı.Text = adı;
        }

        private void Adı_TextChanged(object sender, EventArgs e)
        {
            if (Yasakİçerik != null)
            {
                foreach (string içerik in Yasakİçerik)
                {
                    Adı.Text = Adı.Text.Replace(içerik, null);
                }
            }

            TuşlarıDüzenle();
        }
        private void Ekle_Click(object sender, EventArgs e)
        {
            string mesaj = "Listeye yeni bir kayıt eklenecek. İşleme devam etmek istiyor musunuz?" +
                Environment.NewLine + Environment.NewLine +
                Adı.Text;
            if (!OnayAl(mesaj)) return;

            bool? sonuç = GeriBildirim_İşlemi?.Invoke(Adı.Text, İşlemTürü.YeniEklendi);
            if (sonuç != null && !sonuç.Value) return;

            Tüm_Elemanlar.Add(Adı.Text);
            Sırala();
            Filtrele(SeçimKutusu, Tüm_Elemanlar, AramaÇubuğu.Text);
            TuşlarıDüzenle();
            İpucunuDüzenle();
            if (İşlemYaptıktanSonraSeç) SeçimKutusu.SelectedIndex = SeçimKutusu.Items.IndexOf(Adı.Text);
        }
        private void AdıDeğiştir_Gizle_Göster_Click(object sender, EventArgs e)
        {
            string adı_eski = SeçilenEleman_Adı, adı_yeni;
            string mesaj;
            İşlemTürü Türü;
            if (sender == AdıDeğiştir)
            {
                mesaj = "Listedeki kayıt adı değiştirilecek.";
                Türü = İşlemTürü.AdıDeğiştirildi;
                adı_yeni = Adı.Text;
            }
            else if (sender == Göster)
            {
                mesaj = "Listedeki kayıt görünür duruma getirilecek.";
                Türü = İşlemTürü.GörünürDurumaGetirildi;
                adı_yeni = adı_eski.Substring(GizliElemanBaşlangıcı.Length);
            }
            else
            {
                mesaj = "Listedeki kayıt gizlenecek.";
                Türü = İşlemTürü.Gizlendi;
                adı_yeni = GizliElemanBaşlangıcı + adı_eski;
            }

            mesaj += " İşleme devam etmek istiyor musunuz?" +
                Environment.NewLine + Environment.NewLine +
                adı_eski + " -> " + adı_yeni;
            if (!OnayAl(mesaj)) return;

            bool? sonuç = GeriBildirim_İşlemi?.Invoke(adı_eski, Türü, adı_yeni);
            if (sonuç != null && !sonuç.Value) return;

            Tüm_Elemanlar.Remove(adı_eski);
            Tüm_Elemanlar.Add(adı_yeni);
            Sırala();
            Filtrele(SeçimKutusu, Tüm_Elemanlar, AramaÇubuğu.Text);
            TuşlarıDüzenle();
            if (İşlemYaptıktanSonraSeç) SeçimKutusu.SelectedIndex = SeçimKutusu.Items.IndexOf(adı_yeni);
        }
        private void KonumunuDeğiştir_Click(object sender, EventArgs e)
        {
            string adı = SeçilenEleman_Adı, mesaj;
            int konumu = SeçilenEleman_SıraNo;
            if (sender == YukarıTaşı)
            {
                mesaj = "Listedeki kayıt yukarı taşınacak.";
                konumu--;
            }
            else
            {
                mesaj = "Listedeki kayıt aşağı taşınacak.";
                konumu++;
            }

            if (İşlemYapmadanÖnceSor_KonumDeğiştirme)
            {
                mesaj += " İşleme devam etmek istiyor musunuz?" +
                Environment.NewLine + Environment.NewLine +
                adı;
                if (!OnayAl(mesaj)) return;
            }

            bool? sonuç = GeriBildirim_İşlemi?.Invoke(adı, İşlemTürü.KonumuDeğiştirildi);
            if (sonuç != null && !sonuç.Value) return;

            Tüm_Elemanlar.Remove(adı);
            Tüm_Elemanlar.Insert(konumu, adı);

            Sırala();
            Filtrele(SeçimKutusu, Tüm_Elemanlar, AramaÇubuğu.Text);
            TuşlarıDüzenle();
            if (İşlemYaptıktanSonraSeç) SeçimKutusu.SelectedIndex = konumu;
        }
        private void Sil_Click(object sender, EventArgs e)
        {
            string adı = SeçilenEleman_Adı;
            string mesaj = "Listedeki kayıt KALICI olarak SİLİNECEK. İşleme devam etmek istiyor musunuz?" +
                Environment.NewLine + Environment.NewLine +
                adı;
            if (!OnayAl(mesaj)) return;

            bool? sonuç = GeriBildirim_İşlemi?.Invoke(Adı.Text, İşlemTürü.Silindi);
            if (sonuç != null && !sonuç.Value) return;

            Tüm_Elemanlar.Remove(adı);
            Filtrele(SeçimKutusu, Tüm_Elemanlar, AramaÇubuğu.Text);
            TuşlarıDüzenle();
            İpucunuDüzenle();
        }

        void Sırala()
        {
            if (!KonumuDeğiştirilebilir) Tüm_Elemanlar.Sort(new Sıralayıcı(GizliElemanBaşlangıcı));
        }
        class Sıralayıcı : IComparer<string>
        {
            string GizliElemanBaşlangıcı;
            public Sıralayıcı(string GizliElemanBaşlangıcı)
            {
                this.GizliElemanBaşlangıcı = GizliElemanBaşlangıcı;
            }

            public int Compare(string A, string B)
            {
                bool sol = A.StartsWith(GizliElemanBaşlangıcı);
                bool sağ = B.StartsWith(GizliElemanBaşlangıcı);
                if (sol && sağ) return A.CompareTo(B);
                else if (sol) return 1;
                else if (sağ) return -1;
                else return A.CompareTo(B);
            }
        }
        void TuşlarıDüzenle()
        {
            bool görülebilir_adı = Eklenebilir || AdıDeğiştirilebilir, görülebilir_ekle = false, görülebilir_adı_değiştir = false, görülebilir_konumu_değiştir_yukarı = false, görülebilir_konumu_değiştir_aşağı = false, görülebilir_sil = false, görülebilir_gizle = false, görülebilir_göster = false, SabitMi = false;

            if (Adı.Text.DoluMu(true))
            {
                bool MevcutMu = Tüm_Elemanlar.Contains(Adı.Text);
                int SeçiliOlanınKonumu = SeçilenEleman_SıraNo;
                string SeçiliOlanınAdı = SeçilenEleman_Adı;
                int SeçiliElemanSayısı = SeçimKutusu.SelectedItems.Count;
                bool SeçiliMi = SeçiliOlanınKonumu >= 0;
                SabitMi = Sabit_Elemanlar == null ? false : Sabit_Elemanlar.Contains(SeçiliOlanınAdı);

                görülebilir_ekle = Eklenebilir && !MevcutMu;

                if (KonumuDeğiştirilebilir)
                {
                    görülebilir_konumu_değiştir_yukarı = SeçiliMi && SeçiliOlanınKonumu > 0;
                    görülebilir_konumu_değiştir_aşağı = SeçiliMi && SeçiliOlanınKonumu < (SeçimKutusu.Items.Count - 1);
                }

                if (!SabitMi && SeçiliMi && SeçiliElemanSayısı == 1)
                {
                    görülebilir_adı_değiştir = AdıDeğiştirilebilir && !MevcutMu && SeçimKutusu.Text != Adı.Text;

                    if (MevcutMu)
                    {
                        görülebilir_sil = Silinebilir;

                        if (Gizlenebilir)
                        {
                            bool BaşlangıçVarmı = SeçiliOlanınAdı.StartsWith(GizliElemanBaşlangıcı);
                            görülebilir_göster = BaşlangıçVarmı && !Tüm_Elemanlar.Contains(SeçiliOlanınAdı.Substring(GizliElemanBaşlangıcı.Length));
                            görülebilir_gizle = !BaşlangıçVarmı && !Tüm_Elemanlar.Contains(GizliElemanBaşlangıcı + SeçiliOlanınAdı);
                        }
                    }
                }
            }

            Ekle.Visible = görülebilir_ekle;
            AdıDeğiştir.Visible = görülebilir_adı_değiştir;
            YukarıTaşı.Visible = görülebilir_konumu_değiştir_yukarı;
            AşağıTaşı.Visible = görülebilir_konumu_değiştir_aşağı;
            Sil.Visible = görülebilir_sil;
            Gizle.Visible = görülebilir_gizle;
            Göster.Visible = görülebilir_göster;
            Tabla_AltTuşlar.Visible = görülebilir_adı || görülebilir_ekle || görülebilir_adı_değiştir || görülebilir_sil;
            Sabit.Visible = SabitMi;
        }
        bool OnayAl(string Mesaj)
        {
            if (İşlemYapmadanÖnceSor)
            {
                DialogResult Dr = MessageBox.Show(Mesaj, ListeninAçıklaması, MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (Dr == DialogResult.No) return false;
            }

            return true;
        }
        void İpucunuDüzenle()
        {
            İpucu.SetToolTip(SeçimKutusu, ListeninAçıklaması + Environment.NewLine + Environment.NewLine +
                "Toplam " + Tüm_Elemanlar.Count + " kayıt mevcut" + (Sabit_Elemanlar == null ? null : ", " + Sabit_Elemanlar.Count + " sabit") + Environment.NewLine + Environment.NewLine +
                (SeçimKutusu.SelectionMode != SelectionMode.One ? "Birden fazla seçim yapmak için CTRL tuşunu basılı tutunuz" : null));
        }

        public static void Filtrele(ListBox ListeKutucuğu, List<string> Liste = null, string Aranan = null)
        {
            ListeKutucuğu.Items.Clear();

            if (Liste != null)
            {
                string[] arananlar, bulunanlar;

                if (string.IsNullOrEmpty(Aranan)) bulunanlar = Liste.ToArray();
                else
                {
                    arananlar = Aranan.Trim().ToLower().Split(' ');
                    bulunanlar = Liste.FindAll(x => KontrolEt(x)).ToArray();
                }

                ListeKutucuğu.Items.AddRange(bulunanlar);

                bool KontrolEt(string Girdi)
                {
                    Girdi = Girdi.ToLower();

                    foreach (string arn in arananlar)
                    {
                        if (!Girdi.Contains(arn)) return false;
                    }

                    return true;
                }
            }

            ListeKutucuğu.Enabled = ListeKutucuğu.Items.Count > 0;
        }
    }
}
#endif