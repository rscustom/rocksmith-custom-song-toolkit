using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace RocksmithToolkitLib.DLCPackage
{
    public class DLCInlay
    {
        // Creates DLC Inlay files ready for packing
        private const string MESSAGEBOX_CAPTION = "Class DLC of CMG";
        private bool bRet;
        private const string RESOURCESDIR = "\\CGM\\resources";
        private const string TMPDIR = RESOURCESDIR + "\\tmp";
        private const string NEWGUITARPACK = TMPDIR + "\\newguitarpack";
        private string AppPath = Application.ExecutablePath;

        public bool CreateInlayDLC(string guitarnamesix, string guitarname)
        {
            long RDM1 = clsCMD.RandomLong(0x00F000000, 0x00fffffff); // 9 digits
            long RDM2 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF); // 6 digits
            long RDM3 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM4 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM5 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM6 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM7 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM8 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM9 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF);
            long RDM10 = clsCMD.RandomLong(0x000018700, 0x0000F3FFF); // 6 digits
            long RDM11 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF); // 8 digits
            long RDM12 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF);
            long RDM13 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF);
            long RDM14 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF);
            long RDM15 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF);
            long RDM16 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF);
            long RDM17 = clsCMD.RandomLong(0x000989680, 0x005F5E0FF); // 8 digits
            long RDM20 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF); // 10 digits
            long RDM21 = clsCMD.RandomLong(0x005F5E100, 0x03B9AC9FF); // 9 digits
            long RDM22 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF);
            long RDM23 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF);
            long RDM24 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF);
            long RDM25 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF);
            long RDM26 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF);
            long RDM27 = clsCMD.RandomLong(0x03B9ACA00, 0x2540BE3FF); // 10 digits

            #if DEBUG

            RDM1 = 123456789; // 9 digits
            RDM2 = 123456; // 6 digits
            RDM3 = 123457;
            RDM4 = 123458;
            RDM5 = 123459;
            RDM6 = 123460;
            RDM7 = 123461;
            RDM8 = 123462;
            RDM9 = 123463;
            RDM10 = 123464;
            RDM11 = 12345678; // 8 digits
            RDM12 = 12345679;
            RDM13 = 12345680;
            RDM14 = 12345681;
            RDM15 = 12345682;
            RDM16 = 12345683;
            RDM17 = 12345684;
            RDM20 = 1234567890; // 10 digits
            RDM21 = 123456789; // 9 digits
            RDM22 = 1234567891; // 10 digits
            RDM23 = 1234567892;
            RDM24 = 1234567893;
            RDM25 = 1234567894;
            RDM26 = 1234567895;
            RDM27 = 1234567896;

            #endif

            const string IDString = "a0b1c2d3e4f5a6b7c8d9e0f";
            string ID1 = IDString + RDM1.ToString();
            string ID1MAJ = ID1.ToUpper();

            var objCmd = new clsCMD();
            // Resource File Template rsenumerable_guitars.flat
            string rsflat1 = objCmd.GetResource("RocksmithToolkitGUI.CGM.resources.templates.rsenumerable_guitars.flat");
            rsflat1 = rsflat1.Replace("%RDM20%", RDM20.ToString());
            rsflat1 = rsflat1.Replace("%RDM21%", RDM21.ToString());
            rsflat1 = rsflat1.Replace("%RDM22%", RDM22.ToString());
            rsflat1 = rsflat1.Replace("%RDM23%", RDM23.ToString());
            rsflat1 = rsflat1.Replace("%RDM24%", RDM24.ToString());
            rsflat1 = rsflat1.Replace("%RDM25%", RDM25.ToString());
            rsflat1 = rsflat1.Replace("%RDM26%", RDM26.ToString());
            rsflat1 = rsflat1.Replace("%RDM27%", RDM27.ToString());
            bRet = clsCMD.WriteTextFile(AppPath + NEWGUITARPACK + "\\flatmodels\\rs\\rsenumerable_guitars.flat", rsflat1, false);

            // Resource File Template rsenumerable_root.flat
            string rsflat2 = objCmd.GetResource("RocksmithToolkitGUI.CGM.resources.templates.rsenumerable_root.flat");
            rsflat2 = rsflat2.Replace("%RDM20%", RDM20.ToString());
            rsflat2 = rsflat2.Replace("%RDM26%", RDM26.ToString());
            rsflat2 = rsflat2.Replace("%RDM27%", RDM27.ToString());
            bRet = clsCMD.WriteTextFile(AppPath + NEWGUITARPACK + "\\flatmodels\\rs\\rsenumerable_root.flat", rsflat2, false);

            // Resource File Template guitar_g6.xblock
            string guitarxblock = objCmd.GetResource("RocksmithToolkitGUI.CGM.resources.templates.guitar_g6.xblock");
            guitarxblock = guitarxblock.Replace("%ID1%", ID1);
            guitarxblock = guitarxblock.Replace("%guitarnamesix%", guitarnamesix);
            guitarxblock = guitarxblock.Replace("%guitarname%", guitarname);
            bRet = clsCMD.WriteTextFile(AppPath + NEWGUITARPACK + "\\gamexblocks\\nguitars\\guitar_" + guitarnamesix + ".xblock", guitarxblock, false);

            // Resource File Template guitar_g6.json
            string guitarjson = objCmd.GetResource("RocksmithToolkitGUI.CGM.resources.templates.guitar_g6.json");
            guitarjson = guitarjson.Replace("%ID1MAJ%", ID1MAJ);
            guitarjson = guitarjson.Replace("%guitarnamesix%", guitarnamesix);
            guitarjson = guitarjson.Replace("%guitarname%", guitarname);
            bRet = clsCMD.WriteTextFile(AppPath + NEWGUITARPACK + "\\manifests\\guitars\\guitar_" + guitarnamesix + ".json", guitarjson, false);
            bRet = clsCMD.CopyFile(AppPath + NEWGUITARPACK + "\\manifests\\guitars\\guitar_" + guitarnamesix + ".json", AppPath + NEWGUITARPACK + "\\manifests\\songs_dlc_" + guitarnamesix + "\\dlc_guitar_" + guitarnamesix + ".json", true);

            // Resource File Template guitars.hsan
            string guitarshsan = objCmd.GetResource("RocksmithToolkitGUI.CGM.resources.templates.guitars.hsan");
            guitarshsan = guitarshsan.Replace("%ID1MAJ%", ID1MAJ);
            guitarshsan = guitarshsan.Replace("%guitarnamesix%", guitarnamesix);
            guitarshsan = guitarshsan.Replace("%guitarname%", guitarname);
            bRet = clsCMD.WriteTextFile(AppPath + NEWGUITARPACK + "\\manifests\\guitars\\guitars.hsan", guitarshsan, false);
            bRet = clsCMD.CopyFile(AppPath + NEWGUITARPACK + "\\manifests\\guitars\\guitars.hsan", AppPath + NEWGUITARPACK + "\\manifests\\songs_dlc_" + guitarnamesix + "\\dlc_" + guitarnamesix + ".hsan", true);

            // Resource File Template gn_aggregategraph.nt
            string guitarsnt = objCmd.GetResource("RocksmithToolkitGUI.CGM.resources.templates.gn_aggregategraph.nt");
            guitarsnt = guitarsnt.Replace("%guitarnamesix%", guitarnamesix);
            guitarsnt = guitarsnt.Replace("%RDM2%", RDM2.ToString());
            guitarsnt = guitarsnt.Replace("%RDM3%", RDM3.ToString());
            guitarsnt = guitarsnt.Replace("%RDM4%", RDM4.ToString());
            guitarsnt = guitarsnt.Replace("%RDM5%", RDM5.ToString());
            guitarsnt = guitarsnt.Replace("%RDM6%", RDM6.ToString());
            guitarsnt = guitarsnt.Replace("%RDM7%", RDM7.ToString());
            guitarsnt = guitarsnt.Replace("%RDM8%", RDM8.ToString());
            guitarsnt = guitarsnt.Replace("%RDM9%", RDM9.ToString());
            guitarsnt = guitarsnt.Replace("%RDM10%", RDM10.ToString());
            guitarsnt = guitarsnt.Replace("%RDM11%", RDM11.ToString());
            guitarsnt = guitarsnt.Replace("%RDM12%", RDM12.ToString());
            guitarsnt = guitarsnt.Replace("%RDM13%", RDM13.ToString());
            guitarsnt = guitarsnt.Replace("%RDM14%", RDM14.ToString());
            guitarsnt = guitarsnt.Replace("%RDM15%", RDM15.ToString());
            guitarsnt = guitarsnt.Replace("%RDM16%", RDM16.ToString());
            guitarsnt = guitarsnt.Replace("%RDM17%", RDM17.ToString());
            bRet = clsCMD.WriteTextFile(AppPath + NEWGUITARPACK + "\\" + guitarname.Replace(" ", "_").ToLower() + "_aggregategraph.nt", guitarsnt, false);

            return true;
        }
    }
}