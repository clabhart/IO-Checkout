####################################################################
#icc_inpt_gen.awk generates a iccdrvr.tsk input file for each CP that 
#may be downloaded into the CP for IO Checkout.  The awk script
#also generates a ChildECBEnable.bat script which will perform an omset
#to enable each hart child ecbs after they are downloaded. The awk script pulls
#input from a module loading csv file containing the following fields:
#FTA:  this is the letterbug of each FBM and FCM.
#Mod Type:  this is the type of FBM, i.e. FCM, FBM216, FBM201, ...
#CP: this is the control processor
#CHAN: this is the channel/segment that the FBMs are connected to. (1,2,3,4)
#IO: this ONLY applies to FBM 247 (HART CAPABLE). "A" will create 8 RIN blocks for 8 inputs."B" will create 4 RINs/4 ROUTs. 
#The file must be sorted by FTA.
#
#FBMs included in this script: 201,202,203,207,214,215,216,217,218,237,238,241,242,247
#This script will create a RIN, ROUT, MAIN, MCIN, or MCOUT block for each FBM which
#will be placed in a compound named after the FCM.  Refer to the 
#MAIN.txt, MCIN.txt, MCOUT.txt, MAIN to ROUT.txt, MAIN to RIN.txt, and
#ECBs.txt for example of parameters used for each block type.
#
#Required files:
#icc_inpt_gen.awk - this is the awk script
#Nest_Load.txt - a comma separated value text file for the module
#loading.  The first row contains the field headings, so awk may
#map the proper fields.
####################################################################

BEGIN{
	FS=","
	CPPrev=""
	ECBScript = "ChildECBEnable.bat"
	omset = "D:\\opt\\fox\\bin\\tools\\omset"
	print "@echo off" > ECBScript
	print "echo This script will enable all hart child ECBs on all CPs." > ECBScript
	print "pause" > ECBScript
	#Map proper fields
	getline
	for (i=1;i<=NF;i++) {
		if ($i == "FTA") iLbug = i
		if ($i == "Mod Type") iFBMType = i
		if ($i == "CP") iCP = i
		if ($i == "CHAN") iCH = i  
		if ($i == "IO") iAB = i
}
}
$iLbug !~ /-/{ 
	if (CPPrev == "") {
		CPPrev = $iCP
		print "OPEN " $iCP " ALL IOCHKOUT" > $iCP ".i"
print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP "."
}
	
	if (CPPrev != $iCP) {
		print "CLOSE" > CPPrev ".i"
		print "EXIT" > CPPrev ".i"
		print "OPEN " $iCP " ALL IOCHKOUT" > $iCP ".i"
		CPPrev = $iCP
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP "."		
	}
	if (CPPrev == $iCP) {
		#Add COMPOUND
		print "ADD " $iLbug > $iCP ".i"
		print "TYPE = COMPND" > $iCP ".i"
		print "ON = 1" > $iCP ".i"
		print "END" > $iCP ".i"
		Compound = $iLbug
		if ($iFBMType == "FBM204") {	
			#Add FBM204 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB1" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 204" > $iCP ".i"
			print "SWTYPE = 1" > $iCP ".i"
			print "CHAN = 1" > $iCP ".i"
			print "END" > $iCP ".i"
			#Add 4 AOUT blocks
			for (i=1;i<=4;i++) {
				print "ADD " Compound ":" $iLbug "_"i> $iCP ".i"
				print "TYPE = AOUT" > $iCP ".i"
				print "DESCRP = " $iFBMType > $iCP ".i"
				print "IOM_ID = " $iLbug > $iCP ".i"
				print "PNT_NO = " i > $iCP ".i"
				print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
				print "END" > $iCP ".i"
			}
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOMOPT = 2" > $iCP ".i"
			print "END" > $iCP ".i"
		}
	
		if ($iFBMType == "FBM241") {
			#Add FBM241 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB5" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 241" > $iCP ".i"
			print "SWTYPE = 5" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			#Add 8 CIN blocks
			for (i=1;i<=8;i++) {
				print "ADD " Compound ":" $iLbug "_"i > $iCP ".i"
				print "TYPE = CIN" > $iCP ".i"
				print "DESCRP = " $iFBMType > $iCP ".i"
				print "IOM_ID = " $iLbug > $iCP ".i"
				print "PNT_NO = " i > $iCP ".i"
				print "END" > $iCP ".i"
			}
			#Add 8 COUT blocks
			for (i=1;i<=8;i++) {
				print "ADD " Compound ":" $iLbug "_"i+8> $iCP ".i"
				print "TYPE = COUT" > $iCP ".i"
				print "DESCRP = " $iFBMType > $iCP ".i"
				print "IOM_ID = " $iLbug > $iCP ".i"
				print "PNT_NO = " i+8 > $iCP ".i"
				print "END" > $iCP ".i"
			}
		}
				
		if ($iFBMType == "FBM214" ) {
			#Add FBM214 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB200" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 214" > $iCP ".i"
			print "SWTYPE = 214" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
				
			ECBPrefix = substr($iLbug,3)
			for (i=1;i<=8;i++) {
				#Add Child FBM214 ECBS
				print "ADD " $iCP "_ECB:"8"" ECBPrefix i > $iCP ".i"
				print "TYPE = ECB201" > $iCP ".i"
				print "DEV_ID = "8"" ECBPrefix i > $iCP ".i"
				print "HWTYPE = 214" > $iCP ".i"
				print "SWTYPE = 214" > $iCP ".i"
				print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
				print "DVNAME = CH" i > $iCP ".i"
				print "DVADDR = CH" i > $iCP ".i"
				print "DVOPTS = 4-20" > $iCP ".i"
				print "END" > $iCP ".i"
				
				#Add RINs
				print "ADD " $iLbug ":"8"" ECBPrefix i > $iCP ".i"
				print "TYPE = RIN" > $iCP ".i"
				print "IOM_ID = "8"" ECBPrefix i > $iCP ".i"
				print "PNT_NO = CURRENT" > $iCP ".i"
				print "SCI = 0" > $iCP ".i"
				print "HSCI1 = 65535" > $iCP ".i"
				print "LSCI1 = 0" > $iCP ".i"
				print "END" > $iCP ".i"
				
				#Add ECB to enable communications script omset script.
				print echo "echo Enabling " ECBPrefix i > ECBScript
print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
			}			
			#Add MAIN block
			print "ADD " $iLbug ":"8"" ECBPrefix i > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOMOPT = 0" > $iCP ".i"
			for (i=1;i<=8;i++) {
				print "SCI_" i " = 3" > $iCP ".i"
print "MEAS_" i " = " Compound ":"8"" ECBPrefix i ".MEAS" > $iCP ".i"
			}
			print "END" > $iCP ".i"
		}

		if ($iFBMType == "FBM215" ) {
			#Add FBM215 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB200" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 215" > $iCP ".i"
			print "SWTYPE = 215" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"

			ECBPrefix = substr($iLbug,2)
			for (i=1;i<=8;i++) {
				#Add Child FBM215 ECBS
				print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i"
				print "TYPE = ECB201" > $iCP ".i"
				print "DEV_ID = " ECBPrefix i > $iCP ".i"
				print "HWTYPE = 215" > $iCP ".i"
				print "SWTYPE = 215" > $iCP ".i"
				print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
				print "DVNAME = CH" i > $iCP ".i"
				print "DVADDR = CH" i > $iCP ".i"
				print "DVOPTS = 4-20" > $iCP ".i"
				print "END" > $iCP ".i"
				
				#Add ECB to enable communications script omset script.
				print echo "echo Enabling " ECBPrefix i > ECBScript
print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
			}			
			
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOMOPT = 2" > $iCP ".i"
			print "END" > $iCP ".i"
			
			for (i=1;i<=8;i++) {
				#Add ROUTs
				print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
				print "TYPE = ROUT" > $iCP ".i"
				print "IOM_ID = " ECBPrefix i > $iCP ".i"
				print "PNT_NO = CURRENT" > $iCP ".i"
				print "SCO = 3" > $iCP ".i"
				print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
				print "END" > $iCP ".i"
			}												
		}

		if ($iFBMType == "FBM242") {
			#Add FBM242 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB5" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 242" > $iCP ".i"
			print "SWTYPE = 5" > $iCP ".i"
			print "CHAN = 1" > $iCP ".i"
			print "END" > $iCP ".i"
			
			#Add MCOUT block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MCOUT" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"
		}
				
		if ($iFBMType == "FBM216" ) {
			#Add FBM216 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB202" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 216" > $iCP ".i"
			print "SWTYPE = 216" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			ECBPrefix = substr($iLbug,2)
			for (i=1;i<=8;i++) {
				#Add Child FBM216 ECBS
				print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i"
				print "TYPE = ECB201" > $iCP ".i"
				print "DEV_ID = " ECBPrefix i > $iCP ".i"
				print "HWTYPE = 216" > $iCP ".i"
				print "SWTYPE = 216" > $iCP ".i"
				print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
				print "DVNAME = CH" i > $iCP ".i"
				print "DVADDR = CH" i > $iCP ".i"
				print "DVOPTS = 4-20" > $iCP ".i"
				print "END" > $iCP ".i"
				
				#Add RINs
				print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
				print "TYPE = RIN" > $iCP ".i"
				print "IOM_ID = " ECBPrefix i > $iCP ".i"
				print "PNT_NO = CURRENT" > $iCP ".i"
				print "SCI = 0" > $iCP ".i"
				print "HSCI1 = 65535" > $iCP ".i"
				print "LSCI1 = 0" > $iCP ".i"
				print "END" > $iCP ".i"
				
				#Add ECB to enable communications script omset script.
				print echo "echo Enabling " ECBPrefix i > ECBScript
print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
			}			
			
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOMOPT = 0" > $iCP ".i"
			for (i=1;i<=8;i++) {
				print "SCI_" i " = 3" > $iCP ".i"
				print "MEAS_" i " = " Compound ":" ECBPrefix i ".MEAS" > $iCP ".i"
			}
			print "END" > $iCP ".i"
		}

		if ($iFBMType == "FBM218" ) {
			#Add FBM218 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB202" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 218" > $iCP ".i"
			print "SWTYPE = 218" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			ECBPrefix = substr($iLbug,3)
			for (i=1;i<=8;i++) {
				#Add Child FBM218 ECBS
				print "ADD " $iCP "_ECB:"8"" ECBPrefix i > $iCP ".i"
				print "TYPE = ECB201" > $iCP ".i"
				print "DEV_ID = "8"" ECBPrefix i > $iCP ".i"
				print "HWTYPE = 218" > $iCP ".i"
				print "SWTYPE = 218" > $iCP ".i"
				print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
				print "DVNAME = CH" i > $iCP ".i"
				print "DVADDR = CH" i > $iCP ".i"
				print "DVOPTS = 4-20" > $iCP ".i"
				print "END" > $iCP ".i"
				
				#Add ECB to enable communications script omset script.
				print echo "echo Enabling " ECBPrefix i > ECBScript
print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript					
			}			
			
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOMOPT = 2" > $iCP ".i"
			print "END" > $iCP ".i"
			
			for (i=1;i<=8;i++) {
				#Add ROUTs
				print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
				print "TYPE = ROUT" > $iCP ".i"
				print "IOM_ID = "8"" ECBPrefix i > $iCP ".i"
				print "PNT_NO = CURRENT" > $iCP ".i"
				print "SCO = 3" > $iCP ".i"
				print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
				print "END" > $iCP ".i"
			}						
		}

		if ($iFBMType == "FBM201") {
			#Add FBM201 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB1" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 201" > $iCP ".i"
			print "SWTYPE = 201" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"	
		}

		if ($iFBMType == "FBM237" ) {
			#Add FBM237 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB53" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 237" > $iCP ".i"
			print "SWTYPE = 237" > $iCP ".i"
			print "CHAN = 1" > $iCP ".i"
			print "END" > $iCP ".i"			
			
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOMOPT = 2" > $iCP ".i"
			print "END" > $iCP ".i"
				
			#Add 8 AOUT blocks
			for (i=1;i<=8;i++) {
				print "ADD " Compound ":" $iLbug "_"i> $iCP ".i"
				print "TYPE = AOUT" > $iCP ".i"
				print "DESCRP = " $iFBMType > $iCP ".i"
				print "IOM_ID = " $iLbug > $iCP ".i"
				print "PNT_NO = " i > $iCP ".i"
				print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
				print "END" > $iCP ".i"
			}
		}

		if ($iFBMType == "FBM202"  ) {
			#Add FBM202 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB1" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 202" > $iCP ".i"
			print "SWTYPE = 1" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			#Add MAIN block - Change SCI if necessary for appropriate thermocouple type
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			for (i=1;i<=8;i++) {
				print "SCI_" i " = 24" > $iCP ".i"
				print "HSCO" i " = 100" > $iCP ".i"
			}
			print "KSCALE = 1.8" > $iCP ".i"
			print "BSCALE = 32" > $iCP ".i"
			print "END" > $iCP ".i"
		}

		if ($iFBMType == "FBM203" ) {
			#Add FBM203 ECB ch1
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB1" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 203" > $iCP ".i"
			print "SWTYPE = 1" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			#Add MAIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MAIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			for (i=1;i<=8;i++) {
				print "SCI_" i " = 43" > $iCP ".i"
				print "HSCO" i " = 620" > $iCP ".i"
			}
			print "END" > $iCP ".i"
		}

		if ($iFBMType == "FBM207" ) {
			#Add FBM207 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB5" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 207" > $iCP ".i"
			print "SWTYPE = 5" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			#Add MCIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MCIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"
		}

		if ($iFBMType == "FBM217" ) {
			#Add FBM217 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB5" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 217" > $iCP ".i"
			print "SWTYPE = 5" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"
			
			#Add MCIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MCIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"
		}	

		if ($iFBMType == "FBM239" ) {
			#Add FBM239 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB5" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 239" > $iCP ".i"
			print "SWTYPE = 5" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $i CP ".i"
			
			#Add MCIN block
			print "ADD " Compound ":" $iLbug "_IN" > $iCP ".i"
			print "TYPE = MCIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"

			#Add MCOUT block
			print "ADD " Compound ":" $iLbug "_OUT" > $iCP ".i"
			print "TYPE = MCOUT" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"
		}

		if ($iFBMType == "FBM247" ){
			if ($iAB == "A"){
				#Add FBM247 ECB
				print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
				print "TYPE = ECB200" > $iCP ".i"
				print "DEV_ID = " $iLbug > $iCP ".i"
				print "HWTYPE = 247" > $iCP ".i"
				print "SWTYPE = 247" > $iCP ".i"
				print "CHAN = " $iCH > $iCP ".i"
				print "END" > $iCP ".i"

				#ECBHwy = int(substr($iLbug,1,2))
				#if (ECBHwy > 9) ECBNewHwy = sprintf("%c",ECBHwy+55)
				#else ECBNewHwy = ECBHwy
				ECBPrefix = ECBNewHwy substr($iLbug,2)
				ECBPrefix = substr($iLbug,2)
				#ECBPrefix = substr($iLbug,3)
				for (i=1;i<=4;i++) {
					#Add Child FBM247 ECBs
					print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i"
					print "TYPE = ECB201" > $iCP ".i"
					print "DEV_ID = " ECBPrefix i > $iCP ".i"
					print "HWTYPE = 247" > $iCP ".i"
					print "SWTYPE = 247" > $iCP ".i"
					print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
					print "DVNAME = CH" i" I LPWR"> $iCP ".i"
					print "DVOPTS = IOBAD 0.00" > $iCP ".i"
					print "END" > $iCP ".i"
				}
				for (i=5;i<=8;i++) {
					#Add Child FBM247 ECBs
					print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i"
					print "TYPE = ECB201" > $iCP ".i"
					print "DEV_ID = " ECBPrefix i > $iCP ".i"
					print "HWTYPE = 247" > $iCP ".i"
					print "SWTYPE = 247" > $iCP ".i"
					print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
					print "DVNAME = CH" i" O LPWR"> $iCP ".i"
					print "DVOPTS = IOBAD 0.00" > $iCP ".i"
					print "END" > $iCP ".i"
				}
			 	for (i=1;i<=4;i++) {	
					#Add RINs
					print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
					print "TYPE = RIN" > $iCP ".i"
					print "IOM_ID = " ECBPrefix i > $iCP ".i"
					print "PNT_NO = CURRENT" > $iCP ".i"
					print "SCI = 3" > $iCP ".i"
					print "HSCI1 = 100" > $iCP ".i"
					print "LSCI1 = 0" > $iCP ".i"
					print "END" > $iCP ".i"
			
					#Add ECB to enable communications script omset script.
					print echo "echo Enabling " ECBPrefix i > ECBScript 
					print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
				}	
				for (i=5;i<=8;i++) {	
					#Add ROUTs
					print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
					print "TYPE = ROUT" > $iCP ".i"
					print "IOM_ID = " ECBPrefix i > $iCP ".i"
					print "PNT_NO = CURRENT" > $iCP ".i"
					print "SCO = 3" > $iCP ".i"
					print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
					print "END" > $iCP ".i"
					#Add ECB to enable communications script omset script.
					print echo "echo Enabling " ECBPrefix i > ECBScript
					print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
				}	
			}
			if ($iAB == "B"){
				#Add FBM247 ECB
				print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
				print "TYPE = ECB200" > $iCP ".i"
				print "DEV_ID = " $iLbug > $iCP ".i"
				print "HWTYPE = 247" > $iCP ".i"
				print "SWTYPE = 247" > $iCP ".i"
				print "CHAN = " $iCH > $iCP ".i"
				print "END" > $iCP ".i"

				#ECBHwy = int(substr($iLbug,1,2))
				#if (ECBHwy > 9) ECBNewHwy = sprintf("%c",ECBHwy+55)
				#else ECBNewHwy = ECBHwy
				ECBPrefix = ECBNewHwy substr($iLbug,2)
				ECBPrefix = substr($iLbug,2)
				#ECBPrefix = substr($iLbug,3)
				for (i=1;i<=8;i++) {
					#Add Child FBM247 ECBs
					print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i"
					print "TYPE = ECB201" > $iCP ".i"
					print "DEV_ID = " ECBPrefix i > $iCP ".i"
					print "HWTYPE = 247" > $iCP ".i"
					print "SWTYPE = 247" > $iCP ".i"
					print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i"
					print "DVNAME = CH" i" I LPWR"> $iCP ".i"
					print "DVOPTS = IOBAD 0.00" > $iCP ".i"
					print "END" > $iCP ".i"
				}
			 	for (i=1;i<=8;i++) {	
					#Add RINs
					print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
					print "TYPE = RIN" > $iCP ".i"
					print "IOM_ID = " ECBPrefix i > $iCP ".i"
					print "PNT_NO = CURRENT" > $iCP ".i"
					print "SCI = 3" > $iCP ".i"
					print "HSCI1 = 100" > $iCP ".i"
					print "LSCI1 = 0" > $iCP ".i"
					print "END" > $iCP ".i"

					#Add ECB to enable communications script omset script.
					print echo "echo Enabling " ECBPrefix i > ECBScript
					print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
				}		
			}
		}

		if ($iFBMType == "FBM238" ) {
			#Add FBM238 ECB
			print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
			print "TYPE = ECB5" > $iCP ".i"
			print "DEV_ID = " $iLbug > $iCP ".i"
			print "HWTYPE = 238" > $iCP ".i"
			print "SWTYPE =5" > $iCP ".i"
			print "CHAN = " $iCH > $iCP ".i"
			print "END" > $iCP ".i"

			#Add MCIN block
			print "ADD " Compound ":" $iLbug > $iCP ".i"
			print "TYPE = MCIN" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"

			#Add MCOUT block
			print "ADD " Compound ":" $iLbug "_O" > $iCP ".i"
			print "TYPE = MCOUT" > $iCP ".i"
			print "DESCRP = " $iFBMType > $iCP ".i"
			print "IOM_ID = " $iLbug > $iCP ".i"
			print "END" > $iCP ".i"
		}	

	}
}	    	

END {
	print "CLOSE" > $iCP ".i"
	print "EXIT" > $iCP ".i"
	print "Created " ECBScript " omset script to enable hart child ECBs."
}