// Copyright ArgeMup GNU GENERAL PUBLIC LICENSE Version 3 <http://www.gnu.org/licenses/> <https://github.com/ArgeMup/HazirKod_Cdiyez>

using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace ArgeMup.HazirKod
{
    public class Kendi
    {
        static Assembly Kendisi = Assembly.GetExecutingAssembly();
        //static FileInfo fi = new FileInfo(Kendisi.Location);
        static FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Kendisi.Location);

        /// <summary>
        /// Çıktısı : Argemup_HazirKod_C_diyez
        /// </summary>
        public static string Adı()
        {
            return Path.GetFileNameWithoutExtension(Kendisi.Location);
        }

        /// <summary>
        /// Çıktısı : Argemup_HazirKod_C_diyez.exe
        /// </summary>
        public static string DosyaAdı()
        {
            return Path.GetFileName(Kendisi.Location);
        }

        /// <summary>
        /// Çıktısı : C:\\Klasör\\Argemup_HazirKod_C_diyez.exe
        /// </summary>
        public static string DosyaYolu()
        {
            return Kendisi.Location;
        }

        /// <summary>
        /// Çıktısı : C:\\Klasör
        /// </summary>
        public static string Klasörü()
        {
            return Path.GetDirectoryName(Kendisi.Location);
        }

        /// <summary>
        /// Çıktısı : 1.2.3.4
        /// </summary>
        public static string Sürümü_Dosya()
        {
            return fvi.FileVersion;
        }

        /// <summary>
        /// Çıktısı : 1.2.3.4
        /// </summary>
        public static string Sürümü_Ürün()
        {
            return fvi.ProductVersion;
        }
    }
}