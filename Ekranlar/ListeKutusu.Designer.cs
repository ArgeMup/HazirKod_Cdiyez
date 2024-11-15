﻿using System.Drawing;
using System.Windows.Forms;

namespace ArgeMup.HazirKod.Ekranlar
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
            this.components = new System.ComponentModel.Container();
            this.SeçimKutusu = new System.Windows.Forms.ListBox();
            this.AramaÇubuğu = new System.Windows.Forms.TextBox();
            this.Adı = new System.Windows.Forms.TextBox();
            this.Ekle = new System.Windows.Forms.Button();
            this.AdıDeğiştir = new System.Windows.Forms.Button();
            this.Sil = new System.Windows.Forms.Button();
            this.Tabla_AltTuşlar = new System.Windows.Forms.Panel();
            this.Göster = new System.Windows.Forms.Button();
            this.Gizle = new System.Windows.Forms.Button();
            this.İpucu = new System.Windows.Forms.ToolTip(this.components);
            this.SeçimKutusu_TümünüSeç = new System.Windows.Forms.Button();
            this.AramaÇubuğunuTemizle = new System.Windows.Forms.Button();
            this.KonumunuDeğiştir_Aşağı = new System.Windows.Forms.Button();
            this.KonumunuDeğiştir_Yukarı = new System.Windows.Forms.Button();
            this.Sabit = new System.Windows.Forms.Button();
            this.KonumunuDeğiştir_Kaydet = new System.Windows.Forms.Button();
            this.KonumunuDeğiştir_İptal = new System.Windows.Forms.Button();
            this.Tabla_ÜstTuşlar = new System.Windows.Forms.Panel();
            this.SağTuşMenü_Grup = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.SağTuşMenü_Grup_Listesi = new System.Windows.Forms.ToolStripComboBox();
            this.SağTuşMenü_Grup_Ayraç = new System.Windows.Forms.ToolStripSeparator();
            this.SağTuşMenü_Grup_Listele = new System.Windows.Forms.ToolStripMenuItem();
            this.SağTuşMenü_Grup_Ekle = new System.Windows.Forms.ToolStripMenuItem();
            this.SağTuşMenü_Grup_Sil = new System.Windows.Forms.ToolStripMenuItem();
            this.Tabla_AltTuşlar.SuspendLayout();
            this.Tabla_ÜstTuşlar.SuspendLayout();
            this.SağTuşMenü_Grup.SuspendLayout();
            this.SuspendLayout();
            // 
            // SeçimKutusu
            // 
            this.SeçimKutusu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SeçimKutusu.FormattingEnabled = true;
            this.SeçimKutusu.ItemHeight = 16;
            this.SeçimKutusu.Location = new System.Drawing.Point(0, 25);
            this.SeçimKutusu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SeçimKutusu.Name = "SeçimKutusu";
            this.SeçimKutusu.Size = new System.Drawing.Size(257, 138);
            this.SeçimKutusu.TabIndex = 0;
            this.SeçimKutusu.SelectedIndexChanged += new System.EventHandler(this.SeçimKutusu_SelectedIndexChanged);
            this.SeçimKutusu.DoubleClick += new System.EventHandler(this.SeçimKutusu_DoubleClick);
            this.SeçimKutusu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SeçimKutusu_KeyPress);
            // 
            // AramaÇubuğu
            // 
            this.AramaÇubuğu.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.AramaÇubuğu.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AramaÇubuğu.Location = new System.Drawing.Point(0, 0);
            this.AramaÇubuğu.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AramaÇubuğu.Name = "AramaÇubuğu";
            this.AramaÇubuğu.Size = new System.Drawing.Size(30, 22);
            this.AramaÇubuğu.TabIndex = 1;
            this.İpucu.SetToolTip(this.AramaÇubuğu, "Arama Kutusu");
            this.AramaÇubuğu.TextChanged += new System.EventHandler(this.AramaÇubuğu_TextChanged);
            this.AramaÇubuğu.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.AramaÇubuğu_KeyPress);
            // 
            // Adı
            // 
            this.Adı.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Adı.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Adı.Location = new System.Drawing.Point(0, 0);
            this.Adı.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Adı.Name = "Adı";
            this.Adı.Size = new System.Drawing.Size(99, 22);
            this.Adı.TabIndex = 2;
            this.İpucu.SetToolTip(this.Adı, "Adı");
            this.Adı.TextChanged += new System.EventHandler(this.Adı_TextChanged);
            // 
            // Ekle
            // 
            this.Ekle.AutoSize = true;
            this.Ekle.BackColor = System.Drawing.Color.YellowGreen;
            this.Ekle.Dock = System.Windows.Forms.DockStyle.Right;
            this.Ekle.Location = new System.Drawing.Point(99, 0);
            this.Ekle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Ekle.Name = "Ekle";
            this.Ekle.Size = new System.Drawing.Size(32, 25);
            this.Ekle.TabIndex = 3;
            this.Ekle.Text = "+";
            this.İpucu.SetToolTip(this.Ekle, "Ekle");
            this.Ekle.UseVisualStyleBackColor = false;
            this.Ekle.Visible = false;
            this.Ekle.Click += new System.EventHandler(this.Ekle_Click);
            // 
            // AdıDeğiştir
            // 
            this.AdıDeğiştir.AutoSize = true;
            this.AdıDeğiştir.BackColor = System.Drawing.Color.Orange;
            this.AdıDeğiştir.Dock = System.Windows.Forms.DockStyle.Right;
            this.AdıDeğiştir.Location = new System.Drawing.Point(131, 0);
            this.AdıDeğiştir.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AdıDeğiştir.Name = "AdıDeğiştir";
            this.AdıDeğiştir.Size = new System.Drawing.Size(32, 25);
            this.AdıDeğiştir.TabIndex = 4;
            this.AdıDeğiştir.Text = "±";
            this.İpucu.SetToolTip(this.AdıDeğiştir, "Adını Değiştir");
            this.AdıDeğiştir.UseVisualStyleBackColor = false;
            this.AdıDeğiştir.Visible = false;
            this.AdıDeğiştir.Click += new System.EventHandler(this.AdıDeğiştir_Gizle_Göster_Click);
            // 
            // Sil
            // 
            this.Sil.AutoSize = true;
            this.Sil.BackColor = System.Drawing.Color.Salmon;
            this.Sil.Dock = System.Windows.Forms.DockStyle.Right;
            this.Sil.Location = new System.Drawing.Point(229, 0);
            this.Sil.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Sil.Name = "Sil";
            this.Sil.Size = new System.Drawing.Size(28, 25);
            this.Sil.TabIndex = 5;
            this.Sil.Text = "-";
            this.İpucu.SetToolTip(this.Sil, "Sil");
            this.Sil.UseVisualStyleBackColor = false;
            this.Sil.Visible = false;
            this.Sil.Click += new System.EventHandler(this.Sil_Click);
            // 
            // Tabla_AltTuşlar
            // 
            this.Tabla_AltTuşlar.Controls.Add(this.Adı);
            this.Tabla_AltTuşlar.Controls.Add(this.Ekle);
            this.Tabla_AltTuşlar.Controls.Add(this.AdıDeğiştir);
            this.Tabla_AltTuşlar.Controls.Add(this.Göster);
            this.Tabla_AltTuşlar.Controls.Add(this.Gizle);
            this.Tabla_AltTuşlar.Controls.Add(this.Sil);
            this.Tabla_AltTuşlar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Tabla_AltTuşlar.Location = new System.Drawing.Point(0, 163);
            this.Tabla_AltTuşlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Tabla_AltTuşlar.Name = "Tabla_AltTuşlar";
            this.Tabla_AltTuşlar.Size = new System.Drawing.Size(257, 25);
            this.Tabla_AltTuşlar.TabIndex = 6;
            // 
            // Göster
            // 
            this.Göster.AutoSize = true;
            this.Göster.BackColor = System.Drawing.Color.Khaki;
            this.Göster.Dock = System.Windows.Forms.DockStyle.Right;
            this.Göster.Location = new System.Drawing.Point(163, 0);
            this.Göster.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Göster.Name = "Göster";
            this.Göster.Size = new System.Drawing.Size(33, 25);
            this.Göster.TabIndex = 7;
            this.Göster.Text = "●";
            this.İpucu.SetToolTip(this.Göster, "Görünür duruma getir");
            this.Göster.UseVisualStyleBackColor = false;
            this.Göster.Visible = false;
            this.Göster.Click += new System.EventHandler(this.AdıDeğiştir_Gizle_Göster_Click);
            // 
            // Gizle
            // 
            this.Gizle.AutoSize = true;
            this.Gizle.BackColor = System.Drawing.Color.Khaki;
            this.Gizle.Dock = System.Windows.Forms.DockStyle.Right;
            this.Gizle.Location = new System.Drawing.Point(196, 0);
            this.Gizle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Gizle.Name = "Gizle";
            this.Gizle.Size = new System.Drawing.Size(33, 25);
            this.Gizle.TabIndex = 6;
            this.Gizle.Text = "◌";
            this.İpucu.SetToolTip(this.Gizle, "Gizle");
            this.Gizle.UseVisualStyleBackColor = false;
            this.Gizle.Visible = false;
            this.Gizle.Click += new System.EventHandler(this.AdıDeğiştir_Gizle_Göster_Click);
            // 
            // İpucu
            // 
            this.İpucu.AutomaticDelay = 1500;
            this.İpucu.AutoPopDelay = 10000;
            this.İpucu.InitialDelay = 1500;
            this.İpucu.IsBalloon = true;
            this.İpucu.ReshowDelay = 300;
            this.İpucu.UseAnimation = false;
            this.İpucu.UseFading = false;
            // 
            // SeçimKutusu_TümünüSeç
            // 
            this.SeçimKutusu_TümünüSeç.AutoSize = true;
            this.SeçimKutusu_TümünüSeç.BackColor = System.Drawing.Color.LightBlue;
            this.SeçimKutusu_TümünüSeç.Dock = System.Windows.Forms.DockStyle.Right;
            this.SeçimKutusu_TümünüSeç.Location = new System.Drawing.Point(63, 0);
            this.SeçimKutusu_TümünüSeç.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.SeçimKutusu_TümünüSeç.Name = "SeçimKutusu_TümünüSeç";
            this.SeçimKutusu_TümünüSeç.Size = new System.Drawing.Size(35, 25);
            this.SeçimKutusu_TümünüSeç.TabIndex = 8;
            this.SeçimKutusu_TümünüSeç.Text = "✓";
            this.İpucu.SetToolTip(this.SeçimKutusu_TümünüSeç, "Tümünü seç veya bırak");
            this.SeçimKutusu_TümünüSeç.UseVisualStyleBackColor = false;
            this.SeçimKutusu_TümünüSeç.Visible = false;
            this.SeçimKutusu_TümünüSeç.Click += new System.EventHandler(this.SeçimKutusu_TümünüSeç_Click);
            // 
            // AramaÇubuğunuTemizle
            // 
            this.AramaÇubuğunuTemizle.AutoSize = true;
            this.AramaÇubuğunuTemizle.BackColor = System.Drawing.Color.Wheat;
            this.AramaÇubuğunuTemizle.Dock = System.Windows.Forms.DockStyle.Right;
            this.AramaÇubuğunuTemizle.Location = new System.Drawing.Point(30, 0);
            this.AramaÇubuğunuTemizle.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.AramaÇubuğunuTemizle.Name = "AramaÇubuğunuTemizle";
            this.AramaÇubuğunuTemizle.Size = new System.Drawing.Size(33, 25);
            this.AramaÇubuğunuTemizle.TabIndex = 7;
            this.AramaÇubuğunuTemizle.Text = "X";
            this.İpucu.SetToolTip(this.AramaÇubuğunuTemizle, "Arama çubuğunu temizle");
            this.AramaÇubuğunuTemizle.UseVisualStyleBackColor = false;
            this.AramaÇubuğunuTemizle.Visible = false;
            this.AramaÇubuğunuTemizle.Click += new System.EventHandler(this.AramaÇubuğunuTemizle_Click);
            // 
            // KonumunuDeğiştir_Aşağı
            // 
            this.KonumunuDeğiştir_Aşağı.AutoSize = true;
            this.KonumunuDeğiştir_Aşağı.BackColor = System.Drawing.Color.Thistle;
            this.KonumunuDeğiştir_Aşağı.Dock = System.Windows.Forms.DockStyle.Right;
            this.KonumunuDeğiştir_Aşağı.Location = new System.Drawing.Point(158, 0);
            this.KonumunuDeğiştir_Aşağı.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.KonumunuDeğiştir_Aşağı.Name = "KonumunuDeğiştir_Aşağı";
            this.KonumunuDeğiştir_Aşağı.Size = new System.Drawing.Size(36, 25);
            this.KonumunuDeğiştir_Aşağı.TabIndex = 9;
            this.KonumunuDeğiştir_Aşağı.Text = "▼";
            this.İpucu.SetToolTip(this.KonumunuDeğiştir_Aşağı, "Listede aşağı taşı");
            this.KonumunuDeğiştir_Aşağı.UseVisualStyleBackColor = false;
            this.KonumunuDeğiştir_Aşağı.Visible = false;
            this.KonumunuDeğiştir_Aşağı.Click += new System.EventHandler(this.KonumunuDeğiştir_Click);
            // 
            // KonumunuDeğiştir_Yukarı
            // 
            this.KonumunuDeğiştir_Yukarı.AutoSize = true;
            this.KonumunuDeğiştir_Yukarı.BackColor = System.Drawing.Color.Thistle;
            this.KonumunuDeğiştir_Yukarı.Dock = System.Windows.Forms.DockStyle.Right;
            this.KonumunuDeğiştir_Yukarı.Location = new System.Drawing.Point(194, 0);
            this.KonumunuDeğiştir_Yukarı.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.KonumunuDeğiştir_Yukarı.Name = "KonumunuDeğiştir_Yukarı";
            this.KonumunuDeğiştir_Yukarı.Size = new System.Drawing.Size(36, 25);
            this.KonumunuDeğiştir_Yukarı.TabIndex = 10;
            this.KonumunuDeğiştir_Yukarı.Text = "▲";
            this.İpucu.SetToolTip(this.KonumunuDeğiştir_Yukarı, "Listede yukarı taşı");
            this.KonumunuDeğiştir_Yukarı.UseVisualStyleBackColor = false;
            this.KonumunuDeğiştir_Yukarı.Visible = false;
            this.KonumunuDeğiştir_Yukarı.Click += new System.EventHandler(this.KonumunuDeğiştir_Click);
            // 
            // Sabit
            // 
            this.Sabit.AutoSize = true;
            this.Sabit.BackColor = System.Drawing.Color.LightGray;
            this.Sabit.Dock = System.Windows.Forms.DockStyle.Right;
            this.Sabit.Location = new System.Drawing.Point(230, 0);
            this.Sabit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Sabit.Name = "Sabit";
            this.Sabit.Size = new System.Drawing.Size(27, 25);
            this.Sabit.TabIndex = 11;
            this.Sabit.Text = "!";
            this.İpucu.SetToolTip(this.Sabit, "Seçilen kayıt uygulamanın düzgün\r\nçalışabilmesi için gereklidir");
            this.Sabit.UseVisualStyleBackColor = false;
            this.Sabit.Visible = false;
            // 
            // KonumunuDeğiştir_Kaydet
            // 
            this.KonumunuDeğiştir_Kaydet.AutoSize = true;
            this.KonumunuDeğiştir_Kaydet.BackColor = System.Drawing.Color.Thistle;
            this.KonumunuDeğiştir_Kaydet.Dock = System.Windows.Forms.DockStyle.Right;
            this.KonumunuDeğiştir_Kaydet.Location = new System.Drawing.Point(98, 0);
            this.KonumunuDeğiştir_Kaydet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.KonumunuDeğiştir_Kaydet.Name = "KonumunuDeğiştir_Kaydet";
            this.KonumunuDeğiştir_Kaydet.Size = new System.Drawing.Size(32, 25);
            this.KonumunuDeğiştir_Kaydet.TabIndex = 12;
            this.KonumunuDeğiştir_Kaydet.Text = "+";
            this.İpucu.SetToolTip(this.KonumunuDeğiştir_Kaydet, "Listenin son halini kaydeder");
            this.KonumunuDeğiştir_Kaydet.UseVisualStyleBackColor = false;
            this.KonumunuDeğiştir_Kaydet.Visible = false;
            this.KonumunuDeğiştir_Kaydet.Click += new System.EventHandler(this.KonumunuDeğiştir_Kaydet_Click);
            // 
            // KonumunuDeğiştir_İptal
            // 
            this.KonumunuDeğiştir_İptal.AutoSize = true;
            this.KonumunuDeğiştir_İptal.BackColor = System.Drawing.Color.Thistle;
            this.KonumunuDeğiştir_İptal.Dock = System.Windows.Forms.DockStyle.Right;
            this.KonumunuDeğiştir_İptal.Location = new System.Drawing.Point(130, 0);
            this.KonumunuDeğiştir_İptal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.KonumunuDeğiştir_İptal.Name = "KonumunuDeğiştir_İptal";
            this.KonumunuDeğiştir_İptal.Size = new System.Drawing.Size(28, 25);
            this.KonumunuDeğiştir_İptal.TabIndex = 13;
            this.KonumunuDeğiştir_İptal.Text = "-";
            this.İpucu.SetToolTip(this.KonumunuDeğiştir_İptal, "Listeyi ilk haline döndürür");
            this.KonumunuDeğiştir_İptal.UseVisualStyleBackColor = false;
            this.KonumunuDeğiştir_İptal.Visible = false;
            this.KonumunuDeğiştir_İptal.Click += new System.EventHandler(this.KonumunuDeğiştir_İptal_Click);
            // 
            // Tabla_ÜstTuşlar
            // 
            this.Tabla_ÜstTuşlar.Controls.Add(this.AramaÇubuğu);
            this.Tabla_ÜstTuşlar.Controls.Add(this.AramaÇubuğunuTemizle);
            this.Tabla_ÜstTuşlar.Controls.Add(this.SeçimKutusu_TümünüSeç);
            this.Tabla_ÜstTuşlar.Controls.Add(this.KonumunuDeğiştir_Kaydet);
            this.Tabla_ÜstTuşlar.Controls.Add(this.KonumunuDeğiştir_İptal);
            this.Tabla_ÜstTuşlar.Controls.Add(this.KonumunuDeğiştir_Aşağı);
            this.Tabla_ÜstTuşlar.Controls.Add(this.KonumunuDeğiştir_Yukarı);
            this.Tabla_ÜstTuşlar.Controls.Add(this.Sabit);
            this.Tabla_ÜstTuşlar.Dock = System.Windows.Forms.DockStyle.Top;
            this.Tabla_ÜstTuşlar.Location = new System.Drawing.Point(0, 0);
            this.Tabla_ÜstTuşlar.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Tabla_ÜstTuşlar.Name = "Tabla_ÜstTuşlar";
            this.Tabla_ÜstTuşlar.Size = new System.Drawing.Size(257, 25);
            this.Tabla_ÜstTuşlar.TabIndex = 7;
            // 
            // SağTuşMenü_Grup
            // 
            this.SağTuşMenü_Grup.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.SağTuşMenü_Grup.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SağTuşMenü_Grup_Listesi,
            this.SağTuşMenü_Grup_Ayraç,
            this.SağTuşMenü_Grup_Listele,
            this.SağTuşMenü_Grup_Ekle,
            this.SağTuşMenü_Grup_Sil});
            this.SağTuşMenü_Grup.Name = "SağTuşMenü_Seçim";
            this.SağTuşMenü_Grup.ShowImageMargin = false;
            this.SağTuşMenü_Grup.Size = new System.Drawing.Size(196, 142);
            this.SağTuşMenü_Grup.Opening += new System.ComponentModel.CancelEventHandler(this.SağTuşMenü_Grup_Opening);
            // 
            // SağTuşMenü_Grup_Listesi
            // 
            this.SağTuşMenü_Grup_Listesi.Name = "SağTuşMenü_Grup_Listesi";
            this.SağTuşMenü_Grup_Listesi.Size = new System.Drawing.Size(121, 28);
            this.SağTuşMenü_Grup_Listesi.TextChanged += new System.EventHandler(this.SağTuşMenü_Grup_Listesi_TextChanged);
            // 
            // SağTuşMenü_Grup_Ayraç
            // 
            this.SağTuşMenü_Grup_Ayraç.Name = "SağTuşMenü_Grup_Ayraç";
            this.SağTuşMenü_Grup_Ayraç.Size = new System.Drawing.Size(192, 6);
            // 
            // SağTuşMenü_Grup_Listele
            // 
            this.SağTuşMenü_Grup_Listele.Name = "SağTuşMenü_Grup_Listele";
            this.SağTuşMenü_Grup_Listele.Size = new System.Drawing.Size(195, 24);
            this.SağTuşMenü_Grup_Listele.Text = "Gruptakileri listele";
            this.SağTuşMenü_Grup_Listele.Click += new System.EventHandler(this.SağTuşMenü_Grup_Seç_Click);
            // 
            // SağTuşMenü_Grup_Ekle
            // 
            this.SağTuşMenü_Grup_Ekle.Name = "SağTuşMenü_Grup_Ekle";
            this.SağTuşMenü_Grup_Ekle.Size = new System.Drawing.Size(195, 24);
            this.SağTuşMenü_Grup_Ekle.Text = "Seçilenleri gruba ekle";
            this.SağTuşMenü_Grup_Ekle.Click += new System.EventHandler(this.SağTuşMenü_Grup_Ekle_Click);
            // 
            // SağTuşMenü_Grup_Sil
            // 
            this.SağTuşMenü_Grup_Sil.Name = "SağTuşMenü_Grup_Sil";
            this.SağTuşMenü_Grup_Sil.Size = new System.Drawing.Size(195, 24);
            this.SağTuşMenü_Grup_Sil.Text = "Grubu sil";
            this.SağTuşMenü_Grup_Sil.Click += new System.EventHandler(this.SağTuşMenü_Grup_Sil_Click);
            // 
            // ListeKutusu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SeçimKutusu);
            this.Controls.Add(this.Tabla_ÜstTuşlar);
            this.Controls.Add(this.Tabla_AltTuşlar);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "ListeKutusu";
            this.Size = new System.Drawing.Size(257, 188);
            this.ParentChanged += new System.EventHandler(this.ListeKutusu_ParentChanged);
            this.Tabla_AltTuşlar.ResumeLayout(false);
            this.Tabla_AltTuşlar.PerformLayout();
            this.Tabla_ÜstTuşlar.ResumeLayout(false);
            this.Tabla_ÜstTuşlar.PerformLayout();
            this.SağTuşMenü_Grup.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private Button KonumunuDeğiştir_Yukarı;
        private Button KonumunuDeğiştir_Aşağı;
        private Button Sabit;
        private Button KonumunuDeğiştir_Kaydet;
        private Button KonumunuDeğiştir_İptal;
        private ContextMenuStrip SağTuşMenü_Grup;
        private ToolStripComboBox SağTuşMenü_Grup_Listesi;
        private ToolStripSeparator SağTuşMenü_Grup_Ayraç;
        private ToolStripMenuItem SağTuşMenü_Grup_Listele;
        private ToolStripMenuItem SağTuşMenü_Grup_Ekle;
        private ToolStripMenuItem SağTuşMenü_Grup_Sil;
    }
}
