########################################################################################## 
#Developed by Cole Labhart for creating database for IO testing during FATs              
#Contact cole.labhart@se.com for questions and concerns                                  
#Based on the icc_inpt_gen script developed by                                           
#icc_inpt_gen2.awk generates a iccdrvr.tsk input file for each CP that 
#may be downloaded into the CP for IO Checkout.  The awk script
#also generates a ChildECBEnable.bat script which will perform an omset
#to enable each hart child ecbs after they are downloaded. The awk script pulls
#input from a module loading csv file containing the following fields:
#FTA:  this is the letterbug of each FBM and FCM.
#Mod Type:  this is the type of FBM, i.e. FCM, FBM216, FBM201, ...
#CP: this is the control processor
#CHAN: this is the channel/segment that the FBMs are connected to. (1,2,3,4)
#SPARE: this ONLY applies to FBM 247 & FBM248 (HART CAPABLE). 
#1 will create 8 RINs and connect them to a MAIN block
#2 will create 4 RINs and 4 ROUTs and connect them to a MAIN block
#3 will create 8 BINs and connect them to a MCOUT block
#4 will create 4 BINs and $ BOUTs  adn connect them to a MCOUT block
#The file must be sorted by FTA.
#FBMs included in this script: 201,202,203,207,214,215,216,217,218,237,238,241,242,247
#This script will create a RIN, ROUT, MAIN, MCIN, or MCOUT block for each FBM which
#will be placed in a compound named after the FCM.  Refer to the 
#MAIN.txt, MCIN.txt, MCOUT.txt, MAIN to ROUT.txt, MAIN to RIN.txt, and
#ECBs.txt for example of parameters used for each block type.
#Required files:
#icc_inpt_gen.awk - this is the awk script
#Nest_Load.txt - a comma separated value text file for the module loading.  
#The first row contains the field headings, so awk maymap the proper fields.
########################################################################################


#Function to create an AOUT block for the iccdrvr task
function AOUT(FBM){
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i";
	print "TYPE = AOUT" > $iCP ".i";
	print "DESCRP = FBM" FBM > $iCP ".i";
	print "IOM_ID = " $iLbug > $iCP ".i";
	print "PNT_NO = " i > $iCP ".i";
	print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function to create an BIN block for the iccdrvr task
function BIN(FBM){
	print "ADD " $iLbug ":" ECBPrefix i > $iCP ".i";
	print "TYPE = BIN" > $iCP ".i";
	print "IOM_ID = " ECBPrefix i > $iCP ".i";
	print "PNT_NO = DI 20 80" > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function to create an BOUT block for the iccdrvr task
function BOUT(FBM){
	print "ADD " Compound ":" ECBPrefix i > $iCP ".i";
	print "TYPE = BOUT" > $iCP ".i";
	print "IOM_ID = " ECBPrefix i > $iCP ".i";
	print "IN = " Compound ":" $iLbug ".CIN_" i > $iCP ".i";
	print "PNT_NO = DO" > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function to create an CIN block for the iccdrvr task
function CIN(FBM){
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i";
	print "TYPE = CIN" > $iCP ".i";
	print "DESCRP = FBM" FBM > $iCP ".i";
	print "IOM_ID = " $iLbug > $iCP ".i";
	print "PNT_NO = " i > $iCP ".i";
	print "END" > $iCP ".i";
}
#......................................................aDDED bY mTp
#Function to create an CINR block for the iccdrvr task
function CINR(FBM){
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i";
	print "TYPE = CINR" > $iCP ".i";
	print "DESCRP = FBM" FBM > $iCP ".i";
	print "IOM_ID = " $iLbug > $iCP ".i";
	print "IOMIDR = " $iR > $iCP ".i";
	print "PNT_NO = " i > $iCP ".i";
	print "END" > $iCP ".i";
}
#......................................................aDDED bY mTp		
#Function to create an COUT block for the iccdrvr task
function COUT(FBM){	
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i";
	print "TYPE = COUT" > $iCP ".i";
	print "DESCRP = FBM" FBM > $iCP ".i";
	print "IOM_ID = " $iLbug > $iCP ".i";
	print "IN = " Compound ":" $iLbug ".IN_" i > $iCP ".i";
	print "PNT_NO = " i > $iCP ".i";
	print "END" > $iCP ".i";
}
#......................................................aDDED bY mTp
#Function to create an COUTR block for the iccdrvr task
function COUTR(FBM){
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i";
	print "TYPE = COUTR" > $iCP ".i";
	print "DESCRP = FBM" FBM > $iCP ".i";
	print "IOM_ID = " $iLbug > $iCP ".i";
	print "IOMIDR = " $iR > $iCP ".i";
	print "PNT_NO = " i > $iCP ".i";
	print "IN = " Compound ":" $iLbug ".CO_" i > $iCP ".i";
	print "END" > $iCP ".i";
}
#......................................................aDDED bY mTp
#Function to create an ECB1 block for the iccdrvr task
function ECB1(FBM){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i";
	print "TYPE = ECB1" > $iCP ".i";
	print "DEV_ID = " $iLbug > $iCP ".i";
	print "HWTYPE = " FBM > $iCP ".i";
	print "SWTYPE = 1" > $iCP ".i";
	print "CHAN = " $iCH > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function to create an ECB5 block for the iccdrvr task
function ECB5(FBM){
	if(FBM == "217R" || FBM == "240R"){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i";
	print "TYPE = ECB5" > $iCP ".i";
	print "DEV_ID = " $iLbug > $iCP ".i";
	print "HWTYPE = " substr(FBM,1,3) > $iCP ".i";
	print "SWTYPE = 5" > $iCP ".i";
	print "CHAN = " $iCH > $iCP ".i";
	print "END" > $iCP ".i";
	print "ADD " $iCP "_ECB:" $iR > $iCP ".i";
	print "TYPE = ECB5" > $iCP ".i";
	print "DEV_ID = " $iR > $iCP ".i";
	print "HWTYPE = " substr(FBM,1,3) > $iCP ".i";
	print "SWTYPE = 5" > $iCP ".i";
	print "CHAN = " $iCH > $iCP ".i";
	print "END" > $iCP ".i";
	}
	else{
		print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i";
		print "TYPE = ECB5" > $iCP ".i";
		print "DEV_ID = " $iLbug > $iCP ".i";
		print "HWTYPE = " substr(FBM,1,3) > $iCP ".i";
		print "SWTYPE = 5" > $iCP ".i";
		print "CHAN = " $iCH > $iCP ".i";
		print "END" > $iCP ".i";
	}
}
#Function to create an ECB53 block for the iccdrvr task
function ECB53(FBM){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i";
	print "TYPE = ECB53" > $iCP ".i";
	print "DEV_ID = " $iLbug > $iCP ".i";
	print "HWTYPE = " FBM > $iCP ".i";
	print "SWTYPE = " FBM > $iCP ".i";
	print "CHAN = "  $iCH > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function to create an ECB200 block for the iccdrvr task
function ECB200(FBM){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i";
	print "TYPE = ECB200" > $iCP ".i";
	print "DEV_ID = " $iLbug > $iCP ".i";
	print "HWTYPE = " FBM > $iCP ".i";
	print "SWTYPE = " FBM > $iCP ".i";
	print "CHAN = " $iCH > $iCP ".i";
	print "END" > $iCP ".i"	;
	ECBPrefix = substr($iLbug,3);
}
#Function to create an ECB201 block for the iccdrvr task
function ECB201(FBM){
	ECBPrefix = substr($iLbug,2);
	if(Input == "RIN" || Input == "BIN"){
		print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i";
		print "TYPE = ECB201" > $iCP ".i";
		print "DEV_ID = " ECBPrefix i > $iCP ".i";
		print "HWTYPE = " FBM > $iCP ".i";
		print "SWTYPE = " FBM > $iCP ".i";
		print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i";
		if(FBM == "247" || FBM == "248"){
			print "DVNAME = CH" i " I LPWR"> $iCP ".i";
		}
		else{
			print "DVNAME = CH" i> $iCP ".i";
		}				
		print "DVOPTS = 4-20" > $iCP ".i";
		print "END" > $iCP ".i";
	}
	if(Input == "ROUT" || Input == "BOUT"){
		print "ADD " $iCP "_ECB:" ECBPrefix i > $iCP ".i";
		print "TYPE = ECB201" > $iCP ".i";
		print "DEV_ID = " ECBPrefix i > $iCP ".i";
		print "HWTYPE = " FBM > $iCP ".i";
		print "SWTYPE = " FBM > $iCP ".i";
		print "PARENT = "  $iCP "_ECB:" $iLbug > $iCP ".i";
		if(FBM == "247" || FBM == "248"){
			print "DVNAME = CH" i" O LPWR"> $iCP ".i";
		}
		else{
			print "DVNAME = CH" i> $iCP ".i";
		}				
		print "DVOPTS = 4-20" > $iCP ".i";
		print "END" > $iCP ".i";
	}
	print echo "echo Enabling " ECBPrefix i > ECBScript;
	print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript;
}
function ECB202(FBM){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i";
	print "TYPE = ECB202" > $iCP ".i";
	print "DEV_ID = " $iLbug > $iCP ".i";
	print "HWTYPE = " FBM > $iCP ".i";
	print "SWTYPE = " FBM > $iCP ".i";
	print "CHAN = " $iCH > $iCP ".i";
	print "END" > $iCP ".i";
	ECBPrefix = substr($iLbug,2);
}
#Function to create an MAIN block for the iccdrvr task
function MAIN(FBM,Type){
	#These conditions check which type of FBM is being built and creates the MAIN block 	#with the proper configuration
	if(FBM == "202"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "IOMOPT = 1" > $iCP ".i";
		for (j=1;j<=8;j++){
			print "SCI_" j " = 24" > $iCP ".i";
			print "HSCO" j " = 100" > $iCP ".i";
		}
		print "KSCALE = 1.8" > $iCP ".i";
		print "BSCALE = 32" > $iCP ".i";
		print "END" > $iCP ".i";
	}
	else if(FBM == "203"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "IOMOPT = 1" > $iCP ".i";
		for (j=1;j<=8;j++) {
			print "SCI_" j " = 43" > $iCP ".i";
			print "HSCO" j " = 620" > $iCP ".i";
		}
		print "KSCALE = 1.8" > $iCP ".i";
		print "BSCALE = 32" > $iCP ".i";
		print "END" > $iCP ".i";
	}
	else if(FBM == "204"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT = 0" > $iCP ".i";
		for (j=1;j<=4;j++){
			print "SCI_" j " = 3" > $iCP ".i";
			print "MEAS_" j " = " Compound ":" $iLbug "_" j ".PNT_" j > $iCP ".i";
		}
		print "END" > $iCP ".i";
	}
	else if(FBM == "237"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT = 0" > $iCP ".i";
		for (j=1;j<=8;j++){
			print "SCI_" j " = 3" > $iCP ".i";
			print "MEAS_" j " = " Compound ":" $iLbug "_" j ".PNT_" j > $iCP ".i";
		}
		print "END" > $iCP ".i";
	}
	else if(FBM == "214" || FBM == "216"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT = 0" > $iCP ".i";
		for (j=1;j<=8;j++){
			print "SCI_" j " = 3" > $iCP ".i";
			print "MEAS_" j " = " Compound ":" ECBPrefix j ".MEAS" > $iCP ".i";
		}
		print "END" > $iCP ".i";
	}	
	else if(FBM == "248" || FBM == "247"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT = 0" > $iCP ".i";
		for (j=1;j<=8;j++){
			print "SCI_" j " = 3" > $iCP ".i";
			if(Type[j] == "IN"){
				print "MEAS_" j " = " $iLbug ":" ECBPrefix j ".MEAS" > $iCP ".i";
			}
			if(Type[j] == "OUT"){
				print "MEAS_" j " = " $iLbug ":" ECBPrefix j ".OUT" > $iCP ".i";
			}
		}
		print "END" > $iCP ".i";
	}
	else{
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MAIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT = 2" > $iCP ".i";
		print "END" > $iCP ".i";
	}
}
#Function to create an MCIN block for the iccdrvr task
function MCIN(FBM){
	if(FBM == "238" || FBM == "239"){
		print "ADD " Compound ":" $iLbug "_CIN" > $iCP ".i";
		print "TYPE = MCIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "END" > $iCP ".i";
	}
	else{
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MCIN" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "END" > $iCP ".i";
	}
}
#Function to create an MCOUT block for the iccdrvr task
function MCOUT(FBM,Type){
	if(FBM == "248" || FBM == "247"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MCOUT" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "IOMOPT= 0" > $iCP ".i";
		for (j=1;j<=8;j++){
			if(Type[j] == "IN"){
				print "IN_" j " = " $iLbug ":" ECBPrefix j ".CIN" > $iCP ".i";
			}
			if(Type[j] == "OUT"){
				print "IN_" j " = " $iLbug ":" ECBPrefix j ".COUT" > $iCP ".i";
			}
		}	
		print "END" > $iCP ".i";
	}
	else if(FBM == "241"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MCOUT" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT= 0" > $iCP ".i";
		for (j=1;j<=16;j++){
			if(Type[j] == "IN"){
				print "IN_" j " = " $iLbug ":" $iLbug "_" j ".CIN" > $iCP ".i";
			}
			if(Type[j] == "OUT"){
				print "IN_" j " = " $iLbug ":" $iLbug "_"  j ".COUT" > $iCP ".i";
			}
		}	
		print "END" > $iCP ".i";
	}
	else if(FBM == "238" || FBM == "239"){
		print "ADD " Compound ":" $iLbug "_COUT" > $iCP ".i";
		print "TYPE = MCOUT" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "IOMOPT= 1	" > $iCP ".i";
		print "END" > $iCP ".i";
	}
	else if(FBM == "240R"){
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MCOUT" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOMOPT= 0" > $iCP ".i";
		print "END" > $iCP ".i";
	}
	else{
		print "ADD " Compound ":" $iLbug > $iCP ".i";
		print "TYPE = MCOUT" > $iCP ".i";
		print "DESCRP = FBM" FBM > $iCP ".i";
		print "IOM_ID = " $iLbug > $iCP ".i";
		print "IOMOPT= 1" > $iCP ".i";
		print "END" > $iCP ".i";
	}
}
#Function to create an RIN block for the iccdrvr task
function RIN(){
	print "ADD " $iLbug ":" ECBPrefix i > $iCP ".i";
	print "TYPE = RIN" > $iCP ".i";
	print "IOM_ID = " ECBPrefix i > $iCP ".i";
	print "PNT_NO = CURRENT" > $iCP ".i";
	print "SCI = 0" > $iCP ".i";
	print "HSCI1 = 65535" > $iCP ".i";
	print "LSCI1 = 0" > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function to create an ROUT block for the iccdrvr task
function ROUT(){
	print "ADD " Compound ":" ECBPrefix i > $iCP ".i";
	print "TYPE = ROUT" > $iCP ".i";
	print "IOM_ID = " ECBPrefix i > $iCP ".i";
	print "PNT_NO = CURRENT" > $iCP ".i";
	print "SCO = 3" > $iCP ".i";
	print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i";
	print "END" > $iCP ".i";
}
#Function that will run the iccdrvr task if the user wants to
function icc(CP){
	print "Run driver task for " CP "? (Y/N)";
	getline answer < "-";
	if(answer == "Y"){
		#Creates file to tell user which CPs the iccdrvr task has been run on
		command = "/opt/fox/ciocfg/api/iccdrvr.tsk.exe -i " CP  ".i -o " CP ".o";
		system(command);
		print "icc_drvr.tsk run for " CP;
		print "Enable Child ECBs for " CP "? (Y/N)";
		getline answer < "-";
		if(answer == "Y"){
			#Runs the batch file that was created
			system(ECBScript);
			print "Enabled Child ECBs for " CP;
		}
	}
}

BEGIN{
	FS = ",";
	CPPrev = "";
	#Map proper fields
	getline;
	for(i=1;i<=NF;i++){
		#loops through the text file and seperates based on the commas and assigns 			
		#each column to an array
		if($i == "FTA") iLbug = i;
		if($i == "Mod Type") iFBMType = i;
		if($i == "CP") iCP = i;
		if($i == "CHAN") iCH = i;
		if($i == "SETUP") iSP = i;
		if($i == "REDUNDANT") iR = i;
		catch = "false";
	}
}

#Checks if there is another FBM that needs to be built
$iLbug !~ /-/{ 
	#Sets the initial value of the CP parameter. This condition will only be met once when script starts
	if(CPPrev == ""){
		#Sets CP and prints the necessary text for the iccdrvr task
		CPPrev = $iCP;
		print "OPEN " $iCP " ALL IOCHKOUT" > $iCP ".i";
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP ".";
		ECBScript = "ChildECBEnable_" $iCP ".bat";
		omset = "D:\\opt\\fox\\bin\\tools\\omset";
		print "@echo off" > ECBScript;
		print "echo This script will enable all hart child ECBs on all CPs." > ECBScript;
		print "pause" > ECBScript;
	}
	#This condition checks if there is a new CP in the list
	if(CPPrev != $iCP){
		#Prints the closing lines for the iccdrvr file for the previous CP
		print "CLOSE" > CPPrev ".i";
		print "EXIT" > CPPrev ".i";
		#Notifies the user that the iccdrvr file is being created
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP ".";
		#Closes the iccdrvr file and then goes to icc function to run iccdrvr task
		close(CPPrev ".i");
		icc(CPPrev);
		#Begins to create the next iccdrvr file 
		print "OPEN " $iCP " ALL IOCHKOUT" > $iCP ".i";
		#Sets the CP parameter to the next CP name
		CPPrev = $iCP;
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP ".";
		ECBScript = "ChildECBEnable_" $iCP ".bat";
		omset = "D:\\opt\\fox\\bin\\tools\\omset";
		print "@echo off" > ECBScript;
		print "echo This script will enable all hart child ECBs on all CPs." > ECBScript;
		print "pause" > ECBScript;
	}
	#Checks if the FBM belongs to the same CP
	if(CPPrev == $iCP){
		#Begins printing necessary information for the iccdrvr task
		print "ADD " $iLbug > $iCP ".i";
		print "TYPE = COMPND" > $iCP ".i";
		print "ON = 1" > $iCP ".i";
		print "END" > $iCP ".i";
		Compound = $iLbug;
		#These conditions check which FMB type is being created and then it creates the 			
		#necessary ECBs, Blocks, and child ECBs if needed
		if($iFBMType == "FBM201"){
			ECB1(201);
			MAIN(201);
		}
		else if($iFBMType == "FBM202"){	
			ECB1(202);
			MAIN(202);
		}			
		else if($iFBMType == "FBM203" ){
			ECB1(203);
			MAIN(203);
		}
		else if($iFBMType == "FBM204"){	
			ECB1(204);
			for(i=1;i<=4;i++){
				AOUT(204);
			}
			MAIN(204);
		}
		else if($iFBMType == "FBM207" ){
			ECB5(207);
			MCIN(207);
		}
		else if($iFBMType == "FBM214" ){
			ECB200(214);
			for(i=1;i<=8;i++){
				Input = "RIN";
				ECB201(214);
				RIN();
			}
			MAIN(214);
		}
		else if($iFBMType == "FBM215" ){
			ECB200(215);
			for(i=1;i<=8;i++){
				Input = "ROUT";
				ECB201(215);
				ROUT();
			}
			MAIN(215);
		}
		else if($iFBMType == "FBM216" ){
			ECB202(216);
			for(i=1;i<=8;i++){
				Input = "RIN";
				ECB201(216);
				RIN();
			}
			MAIN(216);
		}
		else if($iFBMType == "FBM217" ){
			ECB5(217);
			MCIN(217);
		}
		#......................................................aDDED bY mTp
		else if($iFBMType == "FBM217R" ){
			ECB5(217R);
			for(i=1;i<=32;i++){
				CINR(217R);
			}
		}
		#......................................................aDDED bY mTp
		else if($iFBMType == "FBM218" ){
			ECB202(218);
			for(i=1;i<=8;i++){
				Input = "ROUT";
				ECB201(218);
				ROUT();
			}
			MAIN(218);
		}
		else if($iFBMType == "FBM237" ){
			ECB53(237);
			for(i=1;i<=8;i++){
				AOUT(237);
			}
			MAIN(237);
		}
		else if($iFBMType == "FBM238" ){
			ECB5(238);
			MCIN(238);
			MCOUT(238);
		}
		else if($iFBMType == "FBM239" ){
			ECB5(239);
			MCIN(239);
			MCOUT(239);
		}
		else if($iFBMType == "FBM240" ){
			ECB5(240);
			for(i=1;i<=8;i++){
				COUT(240);
			}
		}
		#......................................................aDDED bY mTp
		else if($iFBMType == "FBM240R" ){
			ECB5(240R);
			MCOUT(240R);
			for(i=9;i<=16;i++){
				COUTR(240R);
			}
		}
		#......................................................aDDED bY mTp
		else if($iFBMType == "FBM241"){
			ECB5(241);
			for (i=1;i<=8;i++){
				CIN(241);
				t[i] = "IN";	
			}
			for (i=9;i<=16;i++){
				COUT(241);
				t[i] = "OUT";
			}
			MCOUT(241,t);
		}
		else if($iFBMType == "FBM242"){
			ECB5(242);
			MCOUT(242);
		}
		else if($iFBMType == "FBM247" || $iFBMType = "FBM248"){
			if($iFBMType == "FBM247"){
				FBMHolder = 247;
				ECB200(247);
			}
			if($iFBMType == "FBM248"){
				FBMHolder = 248;
				ECB202(248);
			}
			if($iSP == "1"){
				for(i=1;i<=8;i++){
					Input = "RIN";
					ECB201(FBMHolder);
					RIN();
					t[i] = "IN";
				}
				MAIN(FBMHolder ,t);
			}
			if($iSP == "2"){
				for(i=1;i<=5;i++){
					Input = "RIN";
					ECB201(FBMHolder);
					RIN();
					t[i] = "IN";
				}
				for(i=5;i<=8;i++){
					Input = "ROUT";
					ECB201(FBMHolder);
					ROUT();
					t[i] = "OUT";
				}
				MAIN(FBMHolder,t);
			}
			if($iSP == "3"){
				for(i=1;i<=8;i++){
					Input = "BIN";
					ECB201(FBMHolder);
					BIN();
					t[i] = "IN";
				}
				MCOUT(FBMHolder,t);
			}
			if($iSP == "4"){
				for(i=1;i<=5;i++){
					Input = "BIN";
					ECB201(FBMHolder);
					BIN();
					t[i] = "IN";
				}
				for(i=5;i<=8;i++){
					Input = "BOUT";
					ECB201(FBMHolder);
					BOUT();
					t[i] = "OUT";
				}
				MCOUT(247,t);
			}
			if($iSP == "5"){
				print("Setup FBM:" FBMHolder);
				print("Setup " $1 ":");	
				for(i=1;i<=8;i++){
					print("Point " i "? (RIN/ROUT/BIN/BOUT)");
					getline Input < "-";
					if(Input == "RIN" || Input == "ROUT" || Input == "BIN" || Input == "BOUT"){
						catch = "true";
					}
					while(catch == "false"){
						print("ERROR: Invalid Block type");
						print("Point " i "? (RIN/ROUT/BIN/BOUT)");
						getline Input < "-";
						if(Input == "RIN" || Input == "ROUT" || Input == "BIN" || Input == "BOUT"){
							catch = "true";
						}						
					}
					if(Input == "RIN"){
						ECB201(FBMHolder);
						RIN();
						t[i] = "IN";
						AD = "A";
					}
					if(Input == "ROUT"){
						ECB201(FBMHolder);
						ROUT();
						t[i] = "OUT";
						AD = "A";
					}
					if(Input == "BIN"){
						ECB201(FBMHolder);
						BIN();
						t[i] = "IN";
						AD = "D";
					}
					if(Input == "BOUT"){
						ECB201(FBMHolder);
						BOUT();
						t[i] = "OUT";
						AD = "D";
					}
					catch = "false";
				}
				if(AD == "A"){
					MAIN(FBMHolder,t);
				}
				if(AD == "D"){
					MCIN(FBMHolder,t);
				}
			}
		}
	}
}
END{
	print "CLOSE" > $iCP ".i";
	print "EXIT" > $iCP ".i";
	print "Created " ECBScript " omset script to enable hart child ECBs.";
	close($iCP ".i");
	icc($iCP);
}
