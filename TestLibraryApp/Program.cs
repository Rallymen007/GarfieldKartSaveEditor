using GarfieldKartSaveLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace TestLibraryApp {
    class Program {
        static void Main(string[] args) {
            GarfieldKartSaveLoader loader = new GarfieldKartSaveLoader();
            string path = GetPath();
            GarfieldKartSave save = loader.LoadSave(path);

            loader.Save(save);

            Console.WriteLine(save);
            Console.ReadLine();
        }

        static string GetPath() {
            Guid localLowId = new Guid("A520A1A4-1780-4FF6-BD18-167343C5AF16");
            return GetKnownFolderPath(localLowId) + "\\\\Anuman Interactive\\\\Garfield Kart Furious Racing";
        }

        static string GetKnownFolderPath(Guid knownFolderId) {
            IntPtr pszPath = IntPtr.Zero;
            try {
                int hr = SHGetKnownFolderPath(knownFolderId, 0, IntPtr.Zero, out pszPath);
                if (hr >= 0)
                    return Marshal.PtrToStringAuto(pszPath);
                throw Marshal.GetExceptionForHR(hr);
            } finally {
                if (pszPath != IntPtr.Zero)
                    Marshal.FreeCoTaskMem(pszPath);
            }
        }

        [DllImport("shell32.dll")]
        static extern int SHGetKnownFolderPath([MarshalAs(UnmanagedType.LPStruct)] Guid rfid, uint dwFlags, IntPtr hToken, out IntPtr pszPath);
    }
}
