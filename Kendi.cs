﻿// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ArgeMup.HazirKod
{
    public class Kendi
    {
        public const string Sürüm = "V1.1";
        static string KendiKonumu = null; 
        static FileVersionInfo fvi = null; 
        //static FileInfo fi = null;

        /// <summary>
        /// Çıktısı : Argemup_HazirKod_C_diyez
        /// </summary>
        public static string Adı
        {
            get
            {
                Başlat();

                return Path.GetFileNameWithoutExtension(KendiKonumu);
            }
        }

        /// <summary>
        /// Çıktısı : Argemup_HazirKod_C_diyez.exe
        /// </summary>
        public static string DosyaAdı
        {
            get
            {
                Başlat();

                return Path.GetFileName(KendiKonumu);
            }
        }    

        /// <summary>
        /// Çıktısı : Argemup
        /// </summary>
        public static string KullanıcıAdı
        {
            get
            {
                return System.Environment.UserName;
            }
        }

        /// <summary>
        /// Çıktısı : Argemup
        /// </summary>
        public static string BilgisayarAdı
        {
            get
            {
                return System.Environment.MachineName;
            }
        }

        /// <summary>
        /// Çıktısı : C:\\Klasör\\Argemup_HazirKod_C_diyez.exe
        /// </summary>
        public static string DosyaYolu
        {
            get
            {
                Başlat();

                return KendiKonumu;
            }
        }     
            
        /// <summary>
        /// Çıktısı : C:\\Klasör
        /// </summary>
        public static string Klasörü
        {
            get
            {
                Başlat();

                return Path.GetDirectoryName(KendiKonumu);
            }
        }

        /// <summary>
        /// Çıktısı : 1.2.3.4
        /// </summary>
        public static string Sürümü_Dosya
        {
            get
            {
                Başlat();

                return fvi.FileVersion;
            }
        }

        /// <summary>
        /// Çıktısı : 1.2.3.4
        /// </summary>
        public static string Sürümü_Ürün
        {
            get
            {
                Başlat();

                return fvi.ProductVersion;
            }
        }

        static void Başlat()
        {
            if (string.IsNullOrEmpty(KendiKonumu))
            {
                KendiKonumu = Assembly.GetExecutingAssembly().Location;

                if (string.IsNullOrEmpty(KendiKonumu) || !File.Exists(KendiKonumu)) KendiKonumu = Process.GetCurrentProcess().MainModule.FileName;

                if (string.IsNullOrEmpty(KendiKonumu) || !File.Exists(KendiKonumu)) KendiKonumu = AppContext.BaseDirectory + @"\" + Assembly.GetExecutingAssembly().ManifestModule.Name;

                if (string.IsNullOrEmpty(KendiKonumu) || !File.Exists(KendiKonumu)) throw new Exception("Kodun konumu okunamadı");
        
                fvi = FileVersionInfo.GetVersionInfo(KendiKonumu);
            }
        }
    }
}