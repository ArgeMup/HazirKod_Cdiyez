using System.Drawing;
using System.Windows.Forms;

namespace ArgeMup.HazirKod
{
    partial class ListeKutusu
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SeçimKutusu = new ListBox();
            AramaÇubuğu = new TextBox();
            Adı = new TextBox();
            Ekle = new Button();
            AdıDeğiştir = new Button();
            Sil = new Button();
            Tabla_AltTuşlar = new Panel();
            Göster = new Button();
            Gizle = new Button();
            İpucu = new ToolTip(components);
            SeçimKutusu_TümünüSeç = new Button();
            AramaÇubuğunuTemizle = new Button();
            AşağıTaşı = new Button();
            YukarıTaşı = new Button();
            Sabit = new Button();
            Tabla_ÜstTuşlar = new Panel();
            Tabla_AltTuşlar.SuspendLayout();
            Tabla_ÜstTuşlar.SuspendLayout();
            SuspendLayout();
            // 
            // SeçimKutusu
            // 
            SeçimKutusu.Dock = DockStyle.Fill;
            SeçimKutusu.FormattingEnabled = true;
            SeçimKutusu.ItemHeight = 20;
            SeçimKutusu.Location = new Point(0, 30);
            SeçimKutusu.Name = "SeçimKutusu";
            SeçimKutusu.Size = new Size(235, 175);
            SeçimKutusu.TabIndex = 0;
            SeçimKutusu.SelectedIndexChanged += SeçimKutusu_SelectedIndexChanged;
            SeçimKutusu.KeyPress += SeçimKutusu_KeyPress;
            // 
            // AramaÇubuğu
            // 
            AramaÇubuğu.BorderStyle = BorderStyle.FixedSingle;
            AramaÇubuğu.Dock = DockStyle.Fill;
            AramaÇubuğu.Location = new Point(0, 0);
            AramaÇubuğu.Name = "AramaÇubuğu";
            AramaÇubuğu.Size = new Size(62, 27);
            AramaÇubuğu.TabIndex = 1;
            İpucu.SetToolTip(AramaÇubuğu, "Arama Kutusu");
            AramaÇubuğu.TextChanged += AramaÇubuğu_TextChanged;
            AramaÇubuğu.KeyPress += AramaÇubuğu_KeyPress;
            // 
            // Adı
            // 
            Adı.BorderStyle = BorderStyle.FixedSingle;
            Adı.Dock = DockStyle.Fill;
            Adı.Location = new Point(0, 0);
            Adı.Name = "Adı";
            Adı.Size = new Size(83, 27);
            Adı.TabIndex = 2;
            İpucu.SetToolTip(Adı, "Adı");
            Adı.TextChanged += Adı_TextChanged;
            // 
            // Ekle
            // 
            Ekle.AutoSize = true;
            Ekle.BackColor = Color.YellowGreen;
            Ekle.Dock = DockStyle.Right;
            Ekle.Location = new Point(83, 0);
            Ekle.Name = "Ekle";
            Ekle.Size = new Size(46, 30);
            Ekle.TabIndex = 3;
            Ekle.Text = "Ekle";
            Ekle.UseVisualStyleBackColor = false;
            Ekle.Visible = false;
            Ekle.Click += Ekle_Click;
            // 
            // AdıDeğiştir
            // 
            AdıDeğiştir.AutoSize = true;
            AdıDeğiştir.BackColor = Color.Orange;
            AdıDeğiştir.Dock = DockStyle.Right;
            AdıDeğiştir.Location = new Point(129, 0);
            AdıDeğiştir.Name = "AdıDeğiştir";
            AdıDeğiştir.Size = new Size(71, 30);
            AdıDeğiştir.TabIndex = 4;
            AdıDeğiştir.Text = "Değiştir";
            AdıDeğiştir.UseVisualStyleBackColor = false;
            AdıDeğiştir.Visible = false;
            AdıDeğiştir.Click += AdıDeğiştir_Gizle_Göster_Click;
            // 
            // Sil
            // 
            Sil.AutoSize = true;
            Sil.BackColor = Color.Salmon;
            Sil.Dock = DockStyle.Right;
            Sil.Location = new Point(200, 0);
            Sil.Name = "Sil";
            Sil.Size = new Size(35, 30);
            Sil.TabIndex = 5;
            Sil.Text = "Sil";
            Sil.UseVisualStyleBackColor = false;
            Sil.Visible = false;
            Sil.Click += Sil_Click;
            // 
            // Tabla_AltTuşlar
            // 
            Tabla_AltTuşlar.Controls.Add(Adı);
            Tabla_AltTuşlar.Controls.Add(Ekle);
            Tabla_AltTuşlar.Controls.Add(AdıDeğiştir);
            Tabla_AltTuşlar.Controls.Add(Sil);
            Tabla_AltTuşlar.Dock = DockStyle.Bottom;
            Tabla_AltTuşlar.Location = new Point(0, 205);
            Tabla_AltTuşlar.Name = "Tabla_AltTuşlar";
            Tabla_AltTuşlar.Size = new Size(235, 30);
            Tabla_AltTuşlar.TabIndex = 6;
            // 
            // Göster
            // 
            Göster.AutoSize = true;
            Göster.BackColor = Color.Khaki;
            Göster.Dock = DockStyle.Right;
            Göster.Location = new Point(156, 0);
            Göster.Name = "Göster";
            Göster.Size = new Size(28, 30);
            Göster.TabIndex = 7;
            Göster.Text = "●";
            İpucu.SetToolTip(Göster, "Görünür duruma getir");
            Göster.UseVisualStyleBackColor = false;
            Göster.Visible = false;
            Göster.Click += AdıDeğiştir_Gizle_Göster_Click;
            // 
            // Gizle
            // 
            Gizle.AutoSize = true;
            Gizle.BackColor = Color.Khaki;
            Gizle.Dock = DockStyle.Right;
            Gizle.Location = new Point(184, 0);
            Gizle.Name = "Gizle";
            Gizle.Size = new Size(28, 30);
            Gizle.TabIndex = 6;
            Gizle.Text = "◌";
            İpucu.SetToolTip(Gizle, "Gizle");
            Gizle.UseVisualStyleBackColor = false;
            Gizle.Visible = false;
            Gizle.Click += AdıDeğiştir_Gizle_Göster_Click;
            // 
            // İpucu
            // 
            İpucu.AutomaticDelay = 100;
            İpucu.AutoPopDelay = 10000;
            İpucu.InitialDelay = 100;
            İpucu.IsBalloon = true;
            İpucu.ReshowDelay = 20;
            İpucu.UseAnimation = false;
            İpucu.UseFading = false;
            // 
            // SeçimKutusu_TümünüSeç
            // 
            SeçimKutusu_TümünüSeç.AutoSize = true;
            SeçimKutusu_TümünüSeç.BackColor = Color.LightBlue;
            SeçimKutusu_TümünüSeç.Dock = DockStyle.Right;
            SeçimKutusu_TümünüSeç.Location = new Point(62, 0);
            SeçimKutusu_TümünüSeç.Name = "SeçimKutusu_TümünüSeç";
            SeçimKutusu_TümünüSeç.Size = new Size(30, 30);
            SeçimKutusu_TümünüSeç.TabIndex = 8;
            SeçimKutusu_TümünüSeç.Text = "✓";
            İpucu.SetToolTip(SeçimKutusu_TümünüSeç, "Tümünü seç veya bırak");
            SeçimKutusu_TümünüSeç.UseVisualStyleBackColor = false;
            SeçimKutusu_TümünüSeç.Visible = false;
            SeçimKutusu_TümünüSeç.Click += SeçimKutusu_TümünüSeç_Click;
            // 
            // AramaÇubuğunuTemizle
            // 
            AramaÇubuğunuTemizle.AutoSize = true;
            AramaÇubuğunuTemizle.BackColor = Color.Wheat;
            AramaÇubuğunuTemizle.Dock = DockStyle.Right;
            AramaÇubuğunuTemizle.Location = new Point(34, 0);
            AramaÇubuğunuTemizle.Name = "AramaÇubuğunuTemizle";
            AramaÇubuğunuTemizle.Size = new Size(28, 30);
            AramaÇubuğunuTemizle.TabIndex = 7;
            AramaÇubuğunuTemizle.Text = "X";
            İpucu.SetToolTip(AramaÇubuğunuTemizle, "Arama çubuğunu temizle");
            AramaÇubuğunuTemizle.UseVisualStyleBackColor = false;
            AramaÇubuğunuTemizle.Visible = false;
            AramaÇubuğunuTemizle.Click += AramaÇubuğunuTemizle_Click;
            // 
            // AşağıTaşı
            // 
            AşağıTaşı.AutoSize = true;
            AşağıTaşı.BackColor = Color.Thistle;
            AşağıTaşı.Dock = DockStyle.Right;
            AşağıTaşı.Location = new Point(124, 0);
            AşağıTaşı.Name = "AşağıTaşı";
            AşağıTaşı.Size = new Size(32, 30);
            AşağıTaşı.TabIndex = 9;
            AşağıTaşı.Text = "▼";
            İpucu.SetToolTip(AşağıTaşı, "Listede aşağı taşı");
            AşağıTaşı.UseVisualStyleBackColor = false;
            AşağıTaşı.Visible = false;
            AşağıTaşı.Click += KonumunuDeğiştir_Click;
            // 
            // YukarıTaşı
            // 
            YukarıTaşı.AutoSize = true;
            YukarıTaşı.BackColor = Color.Thistle;
            YukarıTaşı.Dock = DockStyle.Right;
            YukarıTaşı.Location = new Point(92, 0);
            YukarıTaşı.Name = "YukarıTaşı";
            YukarıTaşı.Size = new Size(32, 30);
            YukarıTaşı.TabIndex = 10;
            YukarıTaşı.Text = "▲";
            İpucu.SetToolTip(YukarıTaşı, "Listede yukarı taşı");
            YukarıTaşı.UseVisualStyleBackColor = false;
            YukarıTaşı.Visible = false;
            YukarıTaşı.Click += KonumunuDeğiştir_Click;
            // 
            // Sabit
            // 
            Sabit.AutoSize = true;
            Sabit.BackColor = Color.LightGray;
            Sabit.Dock = DockStyle.Right;
            Sabit.Location = new Point(212, 0);
            Sabit.Name = "Sabit";
            Sabit.Size = new Size(23, 30);
            Sabit.TabIndex = 11;
            Sabit.Text = "!";
            İpucu.SetToolTip(Sabit, "Seçilen kayıt uygulamanın düzgün\r\nçalışabilmesi için gereklidir");
            Sabit.UseVisualStyleBackColor = false;
            Sabit.Visible = false;
            // 
            // Tabla_ÜstTuşlar
            // 
            Tabla_ÜstTuşlar.Controls.Add(AramaÇubuğunuTemizle);
            Tabla_ÜstTuşlar.Controls.Add(AramaÇubuğu);
            Tabla_ÜstTuşlar.Controls.Add(SeçimKutusu_TümünüSeç);
            Tabla_ÜstTuşlar.Controls.Add(YukarıTaşı);
            Tabla_ÜstTuşlar.Controls.Add(AşağıTaşı);
            Tabla_ÜstTuşlar.Controls.Add(Göster);
            Tabla_ÜstTuşlar.Controls.Add(Gizle);
            Tabla_ÜstTuşlar.Controls.Add(Sabit);
            Tabla_ÜstTuşlar.Dock = DockStyle.Top;
            Tabla_ÜstTuşlar.Location = new Point(0, 0);
            Tabla_ÜstTuşlar.Name = "Tabla_ÜstTuşlar";
            Tabla_ÜstTuşlar.Size = new Size(235, 30);
            Tabla_ÜstTuşlar.TabIndex = 7;
            // 
            // ListeKutusu
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(SeçimKutusu);
            Controls.Add(Tabla_ÜstTuşlar);
            Controls.Add(Tabla_AltTuşlar);
            Name = "ListeKutusu";
            Size = new Size(235, 235);
            ParentChanged += ListeKutusu_ParentChanged;
            Tabla_AltTuşlar.ResumeLayout(false);
            Tabla_AltTuşlar.PerformLayout();
            Tabla_ÜstTuşlar.ResumeLayout(false);
            Tabla_ÜstTuşlar.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        private ListBox SeçimKutusu;
        private TextBox AramaÇubuğu;
        private TextBox Adı;
        private Button Ekle;
        private Button AdıDeğiştir;
        private Button Sil;
        private Panel Tabla_AltTuşlar;
        private ToolTip İpucu;
        private Button Gizle;
        private Button Göster;
        private Panel Tabla_ÜstTuşlar;
        private Button AramaÇubuğunuTemizle;
        private Button SeçimKutusu_TümünüSeç;
        private Button YukarıTaşı;
        private Button AşağıTaşı;
        private Button Sabit;
    }
}
