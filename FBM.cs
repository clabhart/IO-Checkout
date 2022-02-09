using IOCheckoutTool.Properties;
using Microsoft.Toolkit.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

namespace IOCheckoutTool
{
    public class FBM : IDisposable
    {
        public string FBMType { get; set; }
        public string FBMName { get; set; }
        public string RedundantName { get; set; }
        public string Channel { get; set; }
        public string CP { get; set; }
        public string ECB { get; set; }
        public string RedundantECB { get; set; }
        public string OMSET { get; set; }
        public static string Strategy { get; set; }
        public static string Compound { get; set; }
        public CP Controller { get; set; }
        public DirectoryInfo Project { get; set; }
        public StreamWriter DriverWriter { get; set; }
        public StreamWriter ECBWriter { get; set; }
        public XmlDocument DAFile { get; set; }
        public XmlElement DA { get; set; }

        internal static readonly string[][] FBMs =
        {
            new string[]{"FBM201", "201", "1"},
            new string[]{"FBM202", "202", "1"},
            new string[]{"FBM203", "203", "1"},
            new string[]{"FBM204", "204", "2"},
            new string[]{"FBM206", "206", "4"},
            new string[]{"FBM206B", "206", "4"},
            new string[]{"FBM207", "207", "5"},
            new string[]{"FBM214", "214", "214"},
            new string[]{"FBM214B", "214", "214"},
            new string[]{"FBM215", "215", "215"},
            new string[]{"FBM216", "216", "216"},
            new string[]{"FBM217", "217", "217"},
            new string[]{"FBM217R", "217", "217"},
            new string[]{"FBM218", "218", "218"},
            new string[]{"FBM220", "220", "220"},
            new string[]{"FBM221", "221", "221"},
            new string[]{"FBM223", "223", "223"},
            new string[]{"FBM224", "224", "224"},
            new string[]{"FBM228", "228", "228"},
            new string[]{"FBM230", "230", "230"},
            new string[]{"FBM231", "231", "231"},
            new string[]{"FBM232", "232", "232"},
            new string[]{"FBM233", "233", "233"},
            new string[]{"FBM237", "237", "237"},
            new string[]{"FBM238", "238", "238"},
            new string[]{"FBM239", "239", "239"},
            new string[]{"FBM240", "240", "240"},
            new string[]{"FBM240R", "240", "240"},
            new string[]{"FBM241", "241", "241"},
            new string[]{"FBM242", "242", "5"},
            new string[]{"FBM244", "244", "244"},
            new string[]{"FBM245", "245", "245"},
            new string[]{"FBM247", "247", "247"},
            new string[]{"FBM248", "248", "248"}
        };

        public virtual void Build()
        {
            if (FBMName.Length > 9)
            {
                ECB = FBMName[0..^3];
            }
            else
            {
                ECB = FBMName;
            }
            if (RedundantName != null)
            {
                if (RedundantName.Length > 9)
                {
                    RedundantECB = RedundantName[0..^3];
                }
                else
                {
                    RedundantECB = RedundantName;
                }
            }
            if (FBMName.Length > 6)
            {
                Strategy = string.Concat(FBMName[..6], "CHKOUT");
            }
            else
            {
                Strategy = string.Concat(FBMName, "CHKOUT");
            }
            Compound = string.Concat(FBMName, "_IO");
            OMSET = "D:\\opt\\fox\\bin\\tools\\omset";
            int row = GetIndexes(FBMs, FBMType);
            if (File.Exists(Path.Combine(Project.FullName, string.Concat(CP, ".i"))))
            {
                DriverWriter = new StreamWriter(Path.Combine(Project.FullName, string.Concat(CP, ".i")), append: true);
                ECBWriter = new StreamWriter(Path.Combine(Project.FullName, string.Concat("ChildECBEnable_", CP, ".bat")), append: true);
                Append(DirectAccessCreateFBM(GetFBMTemplare(FBMType), FBMName));
                Append(DirectAccessUpdateECBAttribute(FBMName, "CHAN", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
                Append(DirectAccessUpdateECBAttribute(FBMName, "DEV_ID", FBMName));
                Append(DirectAccessUpdateECBAttribute(FBMName, "HWTYPE", FBMs[row][1]));
                Append(DirectAccessUpdateECBAttribute(FBMName, "SWTYPE", FBMs[row][2]));
                CreateCompound();
            }
            else
            {
                DriverWriter = new StreamWriter(Path.Combine(Project.FullName, string.Concat(CP, ".i")));
                ECBWriter = new StreamWriter(Path.Combine(Project.FullName, string.Concat("ChildECBEnable_", CP, ".bat")));
                WriteDriver(string.Concat("OPEN ", CP, " ALL IOCHKOUT"));
                Append(DirectAccessCreateFBM(GetFBMTemplare(FBMType), FBMName));
                Append(DirectAccessUpdateECBAttribute(FBMName, "CHAN", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
                Append(DirectAccessUpdateECBAttribute(FBMName, "DEV_ID", FBMName));
                Append(DirectAccessUpdateECBAttribute(FBMName, "HWTYPE", FBMs[row][1]));
                Append(DirectAccessUpdateECBAttribute(FBMName, "SWTYPE", FBMs[row][2]));
                CreateCompound();
                Batch();
            }
        }

        private static int GetIndexes(string[][] Items, string SearchChar)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i][0].Contains(SearchChar, StringComparison.InvariantCultureIgnoreCase))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Append(XmlNode node)
        {
            DA.AppendChild(node);
        }

        public void WriteDriver(string line)
        {
            DriverWriter.WriteLine(line);
        }

        public void WriteECB(string line)
        {
            ECBWriter.WriteLine(line);
        }

        public XmlNode DirectAccessCreateFBM(string template, string name)
        {
            XmlElement fbm = DAFile.CreateElement("CreateFBM");
            fbm.SetAttribute("Template", template);
            fbm.SetAttribute("FBM", name);
            fbm.SetAttribute("Controller", CP);
            return fbm;
        }

        public XmlNode DirectAccessUpdateBlockAttribute(string block, string parameter, string value)
        {
            XmlElement attribute = DAFile.CreateElement("UpdateBlockAttribute");
            attribute.SetAttribute("Strategy", Strategy);
            attribute.SetAttribute("Block", block);
            attribute.SetAttribute("ParmName", parameter);
            attribute.SetAttribute("ParmValue", value);
            return attribute;
        }

        public XmlNode DirectAccessCreateBlockAddressCxn(string sink, string parameter, string source)
        {
            XmlElement attribute = DAFile.CreateElement("CreateBlockAddressCxn");
            attribute.SetAttribute("Strategy", Strategy);
            attribute.SetAttribute("Sink", sink);
            attribute.SetAttribute("SinkParm", parameter);
            attribute.SetAttribute("SinkValue", source);
            return attribute;
        }

        public XmlNode DirectAccessUpdateECBAttribute(string ecb, string parameter, string value)
        {
            XmlElement attribute = DAFile.CreateElement("UpdateECBAttribute");
            attribute.SetAttribute("Compound", string.Concat(CP, "_ECB"));
            attribute.SetAttribute("ECB", ecb);
            attribute.SetAttribute("ParmName", parameter);
            attribute.SetAttribute("ParmValue", value);
            return attribute;
        }

        public XmlNode DirectAccessCreateCompound()
        {
            XmlElement compound = DAFile.CreateElement("CreateCompound");
            compound.SetAttribute("Compound", Compound);
            compound.SetAttribute("Controller", CP);
            return compound;
        }

        public XmlNode DirectAccessCreateStrategy()
        {
            XmlElement strategy = DAFile.CreateElement("CreateStrategy");
            strategy.SetAttribute("Template", "$Strategy");
            strategy.SetAttribute("Strategy", Strategy);
            strategy.SetAttribute("Compound", Compound);
            return strategy;
        }

        public XmlNode DirectAccessCreateBlock(string template, string name)
        {
            XmlElement block = DAFile.CreateElement("CreateBlock");
            block.SetAttribute("Template", template);
            block.SetAttribute("Block", name);
            block.SetAttribute("Strategy", Strategy);
            return block;
        }

        public XmlNode DirectAccessCreateECB(string name)
        {
            XmlElement ecb = DAFile.CreateElement("CreateDevice");
            ecb.SetAttribute("Template", "$DEV_ECB201");
            ecb.SetAttribute("Device", name);
            ecb.SetAttribute("FBM", FBMName);
            return ecb;
        }

        public XmlNode DirectAccessDeployCompound()
        {
            XmlElement ecb = DAFile.CreateElement("DeployCompound");
            ecb.SetAttribute("Compound", Compound);
            ecb.SetAttribute("Cascade", "Yes");
            ecb.SetAttribute("Reason", "Reason1");
            return ecb;
        }

        private static string GetFBMTemplare(string fbm)
        {
            return fbm switch
            {
                "201" => Resources._201,
                "202" => Resources._202,
                "203" => Resources._203,
                "204" => Resources._204,
                "207" => Resources._207,
                "214" => Resources._214,
                "215" => Resources._215,
                "216" => Resources._216,
                "217" => Resources._217,
                "217R" => Resources._217R,
                "218" => Resources._218,
                "237" => Resources._237,
                "238" => Resources._238,
                "239" => Resources._239,
                "240" => Resources._240,
                "240R" => Resources._240R,
                "241" => Resources._241,
                "242" => Resources._242,
                "247" => Resources._247,
                "248" => Resources._248,
                _ => string.Empty,
            };
        }

        protected void CreateCompound()
        {
            WriteDriver(string.Concat("ADD ", Compound));
            WriteDriver("TYPE = COMPND ");
            WriteDriver("ON = 1");
            WriteDriver("END");
            Append(DirectAccessCreateCompound());
            Append(DirectAccessCreateStrategy());
        }

        protected void Batch()
        {
            WriteECB("@echo off");
            WriteECB("echo This script will enable all hart child ECBs on all CPs.");
            WriteECB("pause");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (ECBWriter != null)
                {
                    ECBWriter.Close();
                    ECBWriter.Dispose();
                }
                if (DriverWriter != null)
                {
                    DriverWriter.Close();
                    DriverWriter.Dispose();
                }
                if (!string.IsNullOrEmpty(FBMName))
                {
                    DA.AppendChild(DirectAccessDeployCompound());
                }
                if (string.IsNullOrEmpty(FBMName))
                {
                    DriverWriter = new StreamWriter(Path.Combine(Project.FullName, string.Concat(CP, ".i")), append: true);
                    WriteDriver("CLOSE");
                    WriteDriver("EXIT");
                    DriverWriter.Close();
                    DriverWriter.Dispose();
                }
            }
        }

        public virtual void AOUT(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = AOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT  = 1");
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver(string.Concat("MEAS = ", Compound, ":", FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.AOUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "MEAS", blockname));
        }

        public virtual void BIN(int i)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = BIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", blockname));
            WriteDriver("PNT_NO = DI 20 80");
            WriteDriver("MA  = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.BIN, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", "DI 20 80"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
        }

        public virtual void BOUT(int i)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = BOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", blockname));
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("IN = ", Compound, ":", FBMName, ".CIN_", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("PNT_NO = DO");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.BOUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", "DO"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "IN", string.Concat(FBMName, ".CIN_", i.ToString(CultureInfo.InvariantCulture))));
        }

        public virtual void CIN(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = CIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("MA  = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.CIN, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
        }

        public virtual void COUT(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = COUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("IN = ", Compound, ":", FBMName, ".IN_", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.COUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "IN", string.Concat(FBMName, ".IN_", i.ToString(CultureInfo.InvariantCulture))));
        }

        public virtual void CINR(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = CINR");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("IOMIDR = ", RedundantName));
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.CINR, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMIDR", RedundantName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
        }

        public virtual void COUTR(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = COUTR");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver(string.Concat("IOMIDR = ", RedundantName));
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver(string.Concat("IN = ", Compound, ":", FBMName, ".CO_", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.COUTR, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "IN", string.Concat(FBMName, ".CO_", i.ToString(CultureInfo.InvariantCulture))));
        }

        public virtual void ECB1()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB1");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 1");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void ECB2()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB2");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 2");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void ECB4()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB4");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = 206"));
            WriteDriver("SWTYPE = 4");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void ECB5()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB5");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 5");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void ECB53()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB53");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void ECB200()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB200");
            WriteDriver(string.Concat("DEV_ID = ", FBMName[..6]));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void ECB201(int i, string blockktype = "")
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string compound = string.Concat(CP, "_ECB");
            string devid = string.Concat(ECB[1..^0], i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", compound, ":", blockname));
            WriteDriver("TYPE = ECB201");
            WriteDriver(string.Concat("DEV_ID = ", devid));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("PARENT = ", CP, "_ECB:", FBMName));
            WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("DVOPTS = 4-20");
            WriteDriver("END");
            WriteECB(string.Concat("echo Enabling ", ECB, i.ToString(CultureInfo.InvariantCulture)));
            WriteECB(string.Concat(OMSET, " -l 1 ", compound, ":", blockname, ".ACTION"));
            Append(DirectAccessCreateECB(blockname));
            Append(DirectAccessUpdateECBAttribute(blockname, "DEV_ID", devid));
            Append(DirectAccessUpdateECBAttribute(blockname, "HWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "SWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture))));
            Append(DirectAccessUpdateECBAttribute(blockname, "DVOPTS", "4-20"));
        }

        public virtual void ECB202()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB202");
            WriteDriver(string.Concat("DEV_ID = ", FBMName[..6]));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public virtual void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 2");
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("MA  = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "2"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }

        public virtual void MCIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }

        public virtual void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            WriteDriver("MA  = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }

        public virtual void RIN(int i)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = RIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", blockname));
            WriteDriver("PNT_NO = CURRENT");
            WriteDriver("SCI = 0");
            WriteDriver("HSCI1 = 65535");
            WriteDriver("LSCI1 = 0");
            WriteDriver("MA  = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.RIN, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", iomid));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", "CURRENT"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "SCI", "0"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "HSCI1", "65535"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "LSCI1", "0"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
        }

        public virtual void RINR(int i)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string redundantname = string.Concat(RedundantECB, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = RINR");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOMID1 = ", blockname));
            WriteDriver(string.Concat("IOMID2 = ", redundantname));
            WriteDriver("PNT_NO = CURRENT");
            WriteDriver("SCI = 0");
            WriteDriver("HSCI1 = 65535");
            WriteDriver("LSCI1 = 0");
            WriteDriver("MA  = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.RINR, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMID1", blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMID2", redundantname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", "CURRENT"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "SCI", "0"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "HSCI1", "65535"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "LSCI1", "0"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
        }

        public virtual void ROUT(int i)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = ROUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", blockname));
            WriteDriver("PNT_NO = CURRENT");
            WriteDriver("SCO = 3");
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("MEAS = ", Compound, ":", FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.ROUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", "CURRENT"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "SCI", "0"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "MEAS", string.Concat(FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture))));
        }

        public virtual void ROUTR(int i)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string redundantname = string.Concat(RedundantECB, "_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = ROUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOMID1 = ", blockname));
            WriteDriver(string.Concat("IOMID2 = ", RedundantName));
            WriteDriver("PNT_NO = CURRENT");
            WriteDriver("SCO = 3");
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("MEAS = ", Compound, ":", FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.ROUTR, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMID1", blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMID2", redundantname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", "CURRENT"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "SCI", "0"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "MEAS", string.Concat(FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture))));
        }
    }

    public class FBM201 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB1();
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture), " = 100"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture)), "100"));
            }
            WriteDriver("END");
        }
    }

    public class FBM202 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB1();
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 24"));
                WriteDriver(string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture), " = 100"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "24"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture)), "100"));
            }
            WriteDriver("KSCALE = 1.8");
            WriteDriver("BSCALE = 32");
            WriteDriver("END");
            Append(DirectAccessUpdateBlockAttribute(FBMName, "KSCALE", "1.8"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "BSCALE", "32"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }
    }

    public class FBM203 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB1();
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 24"));
                WriteDriver(string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture), " = 100"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "24"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture)), "100"));
            }
            WriteDriver("KSCALE = 1.8");
            WriteDriver("BSCALE = 32");
            WriteDriver("END");
            Append(DirectAccessUpdateBlockAttribute(FBMName, "KSCALE", "1.8"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "BSCALE", "32"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }
    }

    public class FBM204 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB2();
            for (int i = 5; i <= 8; ++i)
            {
                AOUT(i);
            }
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void AOUT(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            string mainconnection = string.Concat(Compound, ":", FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = AOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT  = 1");
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver(string.Concat("MEAS = ", mainconnection));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.AOUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "MEAS", mainconnection.Replace(Compound, "", StringComparison.InvariantCultureIgnoreCase)));
        }

        public override void MAIN()
        {
            string main = FBMName;
            WriteDriver(string.Concat("ADD ", Compound, ":", main));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 1");
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(main, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(main, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(main, "IOM_ID", FBMName));
            for (int i = 1; i <= 4; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture), " = 100"));
                Append(DirectAccessUpdateBlockAttribute(main, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessUpdateBlockAttribute(main, string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture)), "100"));
            }
            for (int i = 5; i <= 8; ++i)
            {
                string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", blockname, ".OUT"));
                Append(DirectAccessCreateBlockAddressCxn(main, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(blockname, ".OUT")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }
    }

    public class FBM206 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB4();
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            string main = FBMName;
            WriteDriver(string.Concat("ADD ", Compound, ":", main));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 1");
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver(string.Concat("MA = 1"));
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(main, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(main, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(main, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 8"));
                WriteDriver(string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture), " = 100"));
                Append(DirectAccessUpdateBlockAttribute(main, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "8"));
                Append(DirectAccessUpdateBlockAttribute(main, string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture)), "100"));
            }
            WriteDriver("END");
        }
    }

    public class FBM206B : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB4();
            for (int i = 5; i <= 8; ++i)
            {
                AOUT(i);
            }
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void AOUT(int i)
        {
            string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
            string mainconnection = string.Concat(Compound, ":", FBMName, ".PNT_", i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = AOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT  = 1");
            WriteDriver("MA  = 1");
            WriteDriver(string.Concat("PNT_NO = ", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver(string.Concat("MEAS = ", mainconnection));
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.AOUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(blockname, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(blockname, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "MA", "1"));
            Append(DirectAccessUpdateBlockAttribute(blockname, "PNT_NO", i.ToString(CultureInfo.InvariantCulture)));
            Append(DirectAccessCreateBlockAddressCxn(blockname, "MEAS", mainconnection.Replace(Compound, "", StringComparison.InvariantCultureIgnoreCase)));
        }

        public override void MAIN()
        {
            string main = FBMName;
            WriteDriver(string.Concat("ADD ", Compound, ":", main));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 1");
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver(string.Concat("MA = 1"));
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(main, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(main, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(main, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            for (int i = 1; i <= 4; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 8"));
                WriteDriver(string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture), " = 100"));
                Append(DirectAccessUpdateBlockAttribute(main, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "8"));
                Append(DirectAccessUpdateBlockAttribute(main, string.Concat("HSCO", i.ToString(CultureInfo.InvariantCulture)), "100"));
            }
            for (int i = 5; i <= 8; ++i)
            {
                string blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", blockname, ".OUT"));
                Append(DirectAccessCreateBlockAddressCxn(main, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(blockname, ".OUT")));
            }
            WriteDriver("END");
        }
    }

    public class FBM207 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            base.MCIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM214 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            for (int i = 1; i <= 8; ++i)
            {
                base.ECB201(i);
                base.RIN(i);
            }
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }
    }

    public class FBM214B : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            for (int i = 1; i <= 8; ++i)
            {
                base.ECB201(i);
                base.RIN(i);
            }
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }
    }

    public class FBM215 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            base.MAIN();
            for (int i = 1; i <= 8; ++i)
            {
                base.ECB201(i);
                base.ROUT(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM216 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB202();
            for (int i = 1; i <= 8; ++i)
            {
                base.ECB201(i);
                base.RIN(i);
            }
            MAIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }
    }

    public class FBM217 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            base.MCIN();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM217R : FBM
    {
        public FBM217R(string redundant)
        {
            base.RedundantName = redundant;
        }

        public override void Build()
        {
            base.Build();
            ECB5();
            for (int i = 1; i <= 32; ++i)
            {
                base.CINR(i);
            }
            MCOUT();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void ECB5()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB5");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 5");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", RedundantName));
            WriteDriver("TYPE = ECB5");
            WriteDriver(string.Concat("DEV_ID = ", RedundantName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 5");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }

        public override void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            string blockname;
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            for (int i = 1; i <= 32; ++i)
            {
                blockname = string.Concat(FBMName, "_", i.ToString(CultureInfo.InvariantCulture));
                WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", blockname, ".CIN"));
            }
            WriteDriver("END");
        }
    }

    public class FBM218 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB202();
            base.MAIN();
            for (int i = 1; i <= 8; ++i)
            {
                base.ECB201(i);
                base.ROUT(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM220 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM221 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM223 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM224 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM228 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM230 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM231 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB202();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM232 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM233 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB202();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM237 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB53();
            MAIN();
            for (int i = 1; i <= 8; ++i)
            {
                base.AOUT(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }
    }

    public class FBM238 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            MCIN();
            MCOUT();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MCIN()
        {
            string blockname = string.Concat(FBMName, "_CIN");
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = MCIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("MA = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCIN, blockname));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }

        public override void MCOUT()
        {
            string blockname = string.Concat(FBMName, "_COUT");
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            WriteDriver("MA = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCOUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }
    }

    public class FBM239 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            MCIN();
            MCOUT();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MCIN()
        {
            string blockname = string.Concat(FBMName, "_CIN");
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = MCIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("MA = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCIN, blockname));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }

        public override void MCOUT()
        {
            string blockname = string.Concat(FBMName, "_COUT");
            WriteDriver(string.Concat("ADD ", Compound, ":", blockname));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 1");
            WriteDriver("MA = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCOUT, blockname));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "1"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }
    }

    public class FBM240 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            MCOUT();
            for (int i = 1; i <= 8; ++i)
            {
                base.COUT(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }
    }

    public class FBM240R : FBM
    {
        public FBM240R(string redundant)
        {
            base.RedundantName = redundant;
        }

        public override void Build()
        {
            base.Build();
            ECB5();
            MCOUT();
            for (int i = 1; i <= 8; ++i)
            {
                base.COUTR(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 1");
            WriteDriver("END");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
        }

        public override void ECB5()
        {
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", FBMName));
            WriteDriver("TYPE = ECB5");
            WriteDriver(string.Concat("DEV_ID = ", FBMName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 5");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
            WriteDriver(string.Concat("ADD ", CP, "_ECB:", RedundantName));
            WriteDriver("TYPE = ECB5");
            WriteDriver(string.Concat("DEV_ID = ", RedundantName));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver("SWTYPE = 5");
            WriteDriver(string.Concat("CHAN = ", Channel.Replace("CH", "", StringComparison.InvariantCultureIgnoreCase)));
            WriteDriver("END");
        }
    }

    public class FBM241 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            for (int i = 1; i <= 8; ++i)
            {
                base.CIN(i);
            }
            for (int i = 9; i <= 16; ++i)
            {
                base.COUT(i);
            }
            MCOUT();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 1");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 8; ++i)
            {
                WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", FBMName, "_", i.ToString(CultureInfo.InvariantCulture), ".CIN"));
            }
            for (int i = 9; i <= 16; ++i)
            {
                WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", FBMName, "_", i.ToString(CultureInfo.InvariantCulture), ".COUT"));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }
    }

    public class FBM242 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB5();
            base.MCOUT();
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }
    }

    public class FBM244 : FBM
    {
        public override void Build()
        {
            base.Build();
            base.ECB200();
            for (int i = 1; i <= 4; ++i)
            {
                base.ECB201(i);
                base.RIN(i);
            }
            MAIN();
            for (int i = 5; i <= 8; ++i)
            {
                base.ECB201(i);
                base.ROUT(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA  = 0");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 4; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
            }
            for (int i = 5; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "0"));
            WriteDriver("END");
        }
    }

    public class FBM245 : FBM
    {
        public FBM245(string redundant)
        {
            base.RedundantName = redundant;
        }

        public override void Build()
        {
            base.Build();
            base.ECB202();
            for (int i = 1; i <= 4; ++i)
            {
                ECB201(i);
                base.RINR(i);
            }
            MAIN();
            for (int i = 5; i <= 8; ++i)
            {
                ECB201(i);
                base.ROUTR(i);
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA  = 1");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            for (int i = 1; i <= 4; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
            }
            for (int i = 5; i <= 8; ++i)
            {
                WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT"));
                Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT")));
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "1"));
            WriteDriver("END");
        }

        public override void ECB201(int i, string blockktype = "")
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string compound = string.Concat(CP, "_ECB");
            string devid = string.Concat(blockname[..5], i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", compound, ":", blockname));
            WriteDriver("TYPE = ECB201");
            WriteDriver(string.Concat("DEV_ID = ", devid));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("PARENT = ", CP, "_ECB:", FBMName));
            WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("DVOPTS = 4-20");
            WriteDriver("END");
            WriteECB(string.Concat("echo Enabling ", ECB, i.ToString(CultureInfo.InvariantCulture)));
            WriteECB(string.Concat(OMSET, " -l 1 ", compound, ":", blockname, ".ACTION"));
            Append(DirectAccessCreateECB(blockname));
            Append(DirectAccessUpdateECBAttribute(blockname, "DEV_ID", devid));
            Append(DirectAccessUpdateECBAttribute(blockname, "HWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "SWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture))));
            Append(DirectAccessUpdateECBAttribute(blockname, "DVOPTS", "4-20"));
            string redundantname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string redundantdevid = string.Concat(blockname[..5], i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", compound, ":", redundantname));
            WriteDriver("TYPE = ECB201");
            WriteDriver(string.Concat("DEV_ID = ", redundantdevid));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("PARENT = ", CP, "_ECB:", FBMName));
            WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture)));
            WriteDriver("DVOPTS = 4-20");
            WriteDriver("END");
            WriteECB(string.Concat("echo Enabling ", ECB, i.ToString(CultureInfo.InvariantCulture)));
            WriteECB(string.Concat(OMSET, " -l 1 ", compound, ":", redundantname, ".ACTION"));
            Append(DirectAccessCreateECB(redundantname));
            Append(DirectAccessUpdateECBAttribute(redundantname, "DEV_ID", redundantdevid));
            Append(DirectAccessUpdateECBAttribute(redundantname, "HWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(redundantname, "SWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(redundantname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture))));
            Append(DirectAccessUpdateECBAttribute(redundantname, "DVOPTS", "4-20"));
        }
    }

    public class FBM247 : FBM
    {
        public FBM247(List<string> blocks)
        {
            Blocks = new List<string>();
            Blocks = blocks;
        }

        private readonly List<string> Blocks;

        public override void Build()
        {
            base.Build();
            base.ECB200();
            int i = 1;
            bool analog = false;
            bool digital = false;
            foreach (string block in Blocks)
            {
                switch (block)
                {
                    case "RIN":
                        analog = true;
                        ECB201(i, "IN");
                        base.RIN(i);
                        i++;
                        break;

                    case "ROUT":
                        analog = true;
                        ECB201(i, "OUT");
                        base.ROUT(i);
                        i++;
                        break;

                    case "BIN":
                        digital = true;
                        ECB201(i, "IN");
                        base.BIN(i);
                        i++;
                        break;

                    case "BOUT":
                        digital = true;
                        ECB201(i, "OUT");
                        base.BOUT(i);
                        i++;
                        break;
                }
            }
            if (analog)
            {
                MAIN();
            }
            else if (digital)
            {
                MCOUT();
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void ECB201(int i, string blockktype)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string compound = string.Concat(CP, "_ECB");
            string devid = string.Concat(ECB[1..^0], i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", compound, ":", blockname));
            WriteDriver("TYPE = ECB201");
            WriteDriver(string.Concat("DEV_ID = ", devid));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("PARENT = ", CP, "_ECB:", FBMName));
            Append(DirectAccessCreateECB(blockname));
            Append(DirectAccessUpdateECBAttribute(blockname, "DEV_ID", devid));
            Append(DirectAccessUpdateECBAttribute(blockname, "HWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "SWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "DVOPTS", "4-20"));
            switch (blockktype)
            {
                case "IN":
                    WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture), " I LPWR"));
                    Append(DirectAccessUpdateECBAttribute(blockname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture), " I LPWR")));
                    break;

                case "OUT":
                    WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture), " O LPWR"));
                    Append(DirectAccessUpdateECBAttribute(blockname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture), " O LPWR")));
                    break;
            }
            WriteDriver("DVOPTS = 4-20");
            WriteDriver("END");
            Append(DirectAccessUpdateECBAttribute(blockname, "DVOPTS", "4-20"));
            WriteECB(string.Concat("echo Enabling ", ECB, i.ToString(CultureInfo.InvariantCulture)));
            WriteECB(string.Concat(base.OMSET, " -l 1 ", CP, "_ECB:", ECB, i.ToString(CultureInfo.InvariantCulture), ".ACTION"));
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 0");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            int i = 1;
            foreach (string block in Blocks.Where(x => x == "RIN" || x == "ROUT"))
            {
                switch (block)
                {
                    case "RIN":
                        WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                        WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                        Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
                        break;

                    case "ROUT":
                        WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                        WriteDriver(string.Concat("SCO_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                        WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT"));
                        Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                        Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT")));
                        break;
                }
                ++i;
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "0"));
            WriteDriver("END");
        }

        public override void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 0");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            int i = 1;
            foreach (string block in Blocks.Where(x => x == "BIN" || x == "BOUT"))
            {
                switch (block)
                {
                    case "BIN":
                        WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, i.ToString(CultureInfo.InvariantCulture), ".CIN"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, i.ToString(CultureInfo.InvariantCulture), ".CIN")));
                        break;

                    case "BOUT":
                        WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, i.ToString(CultureInfo.InvariantCulture), ".COUT"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, i.ToString(CultureInfo.InvariantCulture), ".COUT")));
                        break;
                }
                ++i;
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "0"));
            WriteDriver("END");
        }
    }

    public class FBM248 : FBM
    {
        public FBM248(List<string> blocks)
        {
            Blocks = new List<string>();
            Blocks = blocks;
        }

        private readonly List<string> Blocks;

        public override void Build()
        {
            base.Build();
            base.ECB200();
            int i = 1;
            bool analog = false;
            bool digital = false;
            foreach (string block in Blocks)
            {
                switch (block)
                {
                    case "RIN":
                        analog = true;
                        ECB201(i, "IN");
                        base.RIN(i);
                        i++;
                        break;

                    case "ROUT":
                        analog = true;
                        ECB201(i, "OUT");
                        base.ROUT(i);
                        i++;
                        break;

                    case "BIN":
                        digital = true;
                        ECB201(i, "IN");
                        base.BIN(i);
                        i++;
                        break;

                    case "BOUT":
                        digital = true;
                        ECB201(i, "OUT");
                        base.BOUT(i);
                        i++;
                        break;
                }
            }
            if (analog)
            {
                MAIN();
            }
            else if (digital)
            {
                MCOUT();
            }
            DriverWriter.Close();
            DriverWriter.Dispose();
            ECBWriter.Close();
            ECBWriter.Dispose();
        }

        public override void ECB201(int i, string blockktype)
        {
            string blockname = string.Concat(ECB, "_", i.ToString(CultureInfo.InvariantCulture));
            string compound = string.Concat(CP, "_ECB");
            string devid = string.Concat(ECB[1..^0], i.ToString(CultureInfo.InvariantCulture));
            WriteDriver(string.Concat("ADD ", compound, ":", blockname));
            WriteDriver("TYPE = ECB201");
            WriteDriver(string.Concat("DEV_ID = ", devid));
            WriteDriver(string.Concat("HWTYPE = ", FBMType));
            WriteDriver(string.Concat("SWTYPE = ", FBMType));
            WriteDriver(string.Concat("PARENT = ", CP, "_ECB:", FBMName));
            Append(DirectAccessCreateECB(blockname));
            Append(DirectAccessUpdateECBAttribute(blockname, "DEV_ID", devid));
            Append(DirectAccessUpdateECBAttribute(blockname, "HWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "SWTYPE", FBMType));
            Append(DirectAccessUpdateECBAttribute(blockname, "DVOPTS", "4-20"));
            switch (blockktype)
            {
                case "IN":
                    WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture), " I LPWR"));
                    Append(DirectAccessUpdateECBAttribute(blockname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture), " I LPWR")));
                    break;

                case "OUT":
                    WriteDriver(string.Concat("DVNAME = CH", i.ToString(CultureInfo.InvariantCulture), " O LPWR"));
                    Append(DirectAccessUpdateECBAttribute(blockname, "DVNAME", string.Concat("CH", i.ToString(CultureInfo.InvariantCulture), " O LPWR")));
                    break;
            }
            WriteDriver("DVOPTS = 4-20");
            WriteDriver("END");
            Append(DirectAccessUpdateECBAttribute(blockname, "DVOPTS", "4-20"));
            WriteECB(string.Concat("echo Enabling ", ECB, i.ToString(CultureInfo.InvariantCulture)));
            WriteECB(string.Concat(base.OMSET, " -l 1 ", CP, "_ECB:", ECB, i.ToString(CultureInfo.InvariantCulture), ".ACTION"));
        }

        public override void MAIN()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MAIN");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 0");
            Append(DirectAccessCreateBlock(Resources.MAIN, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            int i = 1;
            foreach (string block in Blocks.Where(x => x == "RIN" || x == "ROUT"))
            {
                switch (block)
                {
                    case "RIN":
                        WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                        WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS"));
                        Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".MEAS")));
                        break;

                    case "ROUT":
                        WriteDriver(string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                        WriteDriver(string.Concat("SCO_", i.ToString(CultureInfo.InvariantCulture), " = 3"));
                        WriteDriver(string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT"));
                        Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                        Append(DirectAccessUpdateBlockAttribute(FBMName, string.Concat("SCI_", i.ToString(CultureInfo.InvariantCulture)), "3"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("MEAS_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, "_", i.ToString(CultureInfo.InvariantCulture), ".OUT")));
                        break;
                }
                ++i;
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "0"));
            WriteDriver("END");
        }

        public override void MCOUT()
        {
            WriteDriver(string.Concat("ADD ", Compound, ":", FBMName));
            WriteDriver("TYPE = MCOUT");
            WriteDriver(string.Concat("DESCRP = FBM", FBMType));
            WriteDriver(string.Concat("IOM_ID = ", FBMName));
            WriteDriver("IOMOPT = 0");
            WriteDriver("MA = 0");
            Append(DirectAccessCreateBlock(Resources.MCOUT, FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "DESCRP", string.Concat("FBM", FBMType)));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOM_ID", FBMName));
            Append(DirectAccessUpdateBlockAttribute(FBMName, "IOMOPT", "0"));
            int i = 1;
            foreach (string block in Blocks.Where(x => x == "BIN" || x == "BOUT"))
            {
                switch (block)
                {
                    case "BIN":
                        WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, i.ToString(CultureInfo.InvariantCulture), ".CIN"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, i.ToString(CultureInfo.InvariantCulture), ".CIN")));
                        break;

                    case "BOUT":
                        WriteDriver(string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture), " = ", Compound, ":", base.ECB, i.ToString(CultureInfo.InvariantCulture), ".COUT"));
                        Append(DirectAccessCreateBlockAddressCxn(FBMName, string.Concat("IN_", i.ToString(CultureInfo.InvariantCulture)), string.Concat(base.ECB, i.ToString(CultureInfo.InvariantCulture), ".COUT")));
                        break;
                }
                ++i;
            }
            Append(DirectAccessUpdateBlockAttribute(FBMName, "MA", "0"));
            WriteDriver("END");
        }
    }
}