function AIN(FBM){
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i"
	print "TYPE = AIN" > $iCP ".i"
	print "DESCRP = " FBM > $iCP ".i"
	print "IOM_ID = " $iLbug > $iCP ".i"
	print "SCI = 24" > $iCP ".i"
	print "HSCO1 = 100" > $iCP ".i"
	print "KSCALE = 1.8" > $iCP ".i"
	print "BSCALE = 32" > $iCP ".i"
}
#Function to create an AOUT block for the iccdrvr task
function AOUT(FBM){
	print "ADD " Compound ":" $iLbug "_" i > $iCP ".i"
	print "TYPE = AOUT" > $iCP ".i"
	print "DESCRP = " FBM > $iCP ".i"
	print "IOM_ID = " $iLbug > $iCP ".i"
	print "PNT_NO = " i > $iCP ".i"
	print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
	print "END" > $iCP ".i"
}
#Function to create an BIN block for the iccdrvr task
function BIN(FBM){
	print "ADD " $iLbug ":" ECBPrefix i > $iCP ".i"
	print "TYPE = BIN" > $iCP ".i"
	print "IOM_ID = " ECBPrefix i > $iCP ".i"
	print "PNT_NO = CURRENT" > $iCP ".i"
	print "END" > $iCP ".i"

}
#Function to create an BOUT block for the iccdrvr task
function BOUT(FBM){
	print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
	print "TYPE = ROUT" > $iCP ".i"
	print "IOM_ID = " ECBPrefix i > $iCP ".i"
	print "END" > $iCP ".i"
}
#Function to create an ECB1 block for the iccdrvr task
function ECB1(FBM){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
	print "TYPE = ECB1" > $iCP ".i"
	print "DEV_ID = " $iLbug > $iCP ".i"
	print "HWTYPE = " FBM > $iCP ".i"
	print "SWTYPE = 1" > $iCP ".i"
	print "CHAN = " $iCH > $iCP ".i"
	print "END" > $iCP ".i"
}
#Function to create an ECB201 block for the iccdrvr task
function ECB201(FBM){
	#ECBHwy = int(substr($iLbug,1,2))
	#if (ECBHwy > 9) ECBNewHwy = sprintf("%c",ECBHwy+55)
	#else ECBNewHwy = ECBHwy
	ECBPrefix = ECBNewHwy substr($iLbug,2)
	ECBPrefix = substr($iLbug,2)
	#ECBPrefix = substr($iLbug,3)
	if(Input == "RIN" || Input == "BIN"){
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
	if(Input == "ROUT" || Input == "BOUT"){
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
	print echo "echo Enabling " ECBPrefix i > ECBScript 
	print omset " -l 1 " $iCP "_ECB:" ECBPrefix i ".ACTION" > ECBScript
}
function ECB202(FBM){
	print "ADD " $iCP "_ECB:" $iLbug > $iCP ".i"
	print "TYPE = ECB202" > $iCP ".i"
	print "DEV_ID = " $iLbug > $iCP ".i"
	print "HWTYPE = " FBM > $iCP ".i"
	print "SWTYPE = " FBM > $iCP ".i"
	print "CHAN = " $iCH > $iCP ".i"
	print "END" > $iCP ".i"
	ECBPrefix = substr($iLbug,2)
}
#Function to create an MAIN block for the iccdrvr task
function MAIN(FBM){
	if(FBM == "202"){
	print "ADD " Compound ":" $iLbug > $iCP ".i"
	print "TYPE = MAIN" > $iCP ".i"
	print "DESCRP = " FBM > $iCP ".i"
	print "IOM_ID = " $iLbug > $iCP ".i"
	for (j=1;j<=8;j++){
		print "SCI_" j " = 24" > $iCP ".i"
		print "HSCO" j " = 100" > $iCP ".i"
	}
	print "KSCALE = 1.8" > $iCP ".i"
	print "BSCALE = 32" > $iCP ".i"
}

}
#Function to create an RIN block for the iccdrvr task
function RIN(){
	print "ADD " $iLbug ":" ECBPrefix i > $iCP ".i"
	print "TYPE = RIN" > $iCP ".i"
	print "IOM_ID = " ECBPrefix i > $iCP ".i"
	print "PNT_NO = CURRENT" > $iCP ".i"
	print "SCI = 0" > $iCP ".i"
	print "HSCI1 = 65535" > $iCP ".i"
	print "LSCI1 = 0" > $iCP ".i"
	print "END" > $iCP ".i"
}
#Function to create an ROUT block for the iccdrvr task
function ROUT(){
	print "ADD " Compound ":" ECBPrefix i > $iCP ".i"
	print "TYPE = ROUT" > $iCP ".i"
	print "IOM_ID = " ECBPrefix i > $iCP ".i"
	print "PNT_NO = CURRENT" > $iCP ".i"
	print "SCO = 3" > $iCP ".i"
	print "MEAS = " Compound ":" $iLbug ".PNT_" i > $iCP ".i"
	print "END" > $iCP ".i"
}
#Function that will run the iccdrvr task if the user wants to
function icc(CP){
	print "Run driver task for " CP "? (Y/N)"
	getline answer < "-"
	if(answer == "Y"){
		#Creates file to tell user which CPs the iccdrvr task has been run on
		print "icc_drvr.tsk run for " CP > "ICC_DRVR.txt"
		command = "/opt/fox/ciocfg/api/iccdrvr.tsk.exe -i " CP  ".i -o " CP ".o"
		system(command)
	}
}

BEGIN{
	FS = ","
	CPPrev = ""
	ECBScript = "ChildECBEnable.bat"
	omset = "D:\\opt\\fox\\bin\\tools\\omset"
	print "@echo off" > ECBScript
	print "echo This script will enable all hart child ECBs on all CPs." > ECBScript
	print "pause" > ECBScript
	#Map proper fields
	getline
	for(i=1;i<=NF;i++){
		#loops through the text file and seperates based on the commas and assigns 			#each column to an array
		if($i == "FTA") iLbug = i
		if($i == "Mod Type") iFBMType = i
		if($i == "CP") iCP = i
		if($i == "CHAN") iCH = i
		if($i == "SPARE") iSP = i
	}
}

#Checks if there is another FBM that needs to be built
$iLbug !~ /-/{ 
	#Sets the initial value of the CP parameter. This condition will only be met once when 	#script starts
	if(CPPrev == ""){
		#Sets CP and prints the necessary text for the iccdrvr task
		CPPrev = $iCP
		print "OPEN " $iCP " ALL IOCHKOUT" > $iCP ".i"
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP "."
	}
	#This condition checks if there is a new CP in the list
	if(CPPrev != $iCP){
		#Prints the closing lines for the iccdrvr file for the previous CP
		print "CLOSE" > CPPrev ".i"
		print "EXIT" > CPPrev ".i"
		#Notifies the user that the iccdrvr file is being created
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP "."
		#Closes the iccdrvr file and then goes to icc function to run iccdrvr task
		close(CPPrev ".i")
		icc(CPPrev)
		#Begins to create the next iccdrvr file 
		print "OPEN " $iCP " ALL IOCHKOUT" > $iCP ".i"
		#Sets the CP parameter to the next CP name
		CPPrev = $iCP
		print "Creating " $iCP ".i iccdrvr.tsk input file for CP " $iCP "."
	}
	#Checks if the FBM belongs to the same CP
	if(CPPrev == $iCP){
		#Begins printing necessary information for the iicdrvr task
		print "ADD " $iLbug > $iCP ".i"
		print "TYPE = COMPND" > $iCP ".i"
		print "ON = 1" > $iCP ".i"
		print "END" > $iCP ".i"
		Compound = $iLbug
		#These conditions check which FMB type is being created and then it creates the 			#necessary ECBs, Blocks, and child ECBs if needed
		if($iFBMType == "FBM202"){
			ECB1(202)
			if($iSP == "AS"){
				for(i=1;i<=8;i++){
					print("Setup " $iLBug " point " i "? (AIN/AOUT)")
					getline Input < "-"
					if (Input == "RIN"){
						ECB201(248)
						AIN()
					}
					if (Input == "ROUT"){
						ECB201(248)
						AOUT()
					}
				}	
			}
			else{
				MAIN(202)
			}
		}	
		if($iFBMType == "FBM248"){
			ECB200(248)
			if($iSP == "DS"){
			for(i=1;i<=8;i++){
				ECB201(248)
				BIN(248)
			}
			if($iSP == "AS"){
				for(i=1;i<=4;i++){
					ECB201(248)
					RIN()
				}
				for(i=5;i<=8;i++){
					ECB201(248)
					ROUT()
				}
			}
			Else{
				print("Setup " $iFBMType ":")			
				for(i=1;i<=8;i++){
					print("Setup point " i "? (RIN/ROUT/BIN/BOUT)")
					getline Input < "-"
					if (Input == "RIN"){
						ECB201(248)
						RIN()
					}
					if (Input == "ROUT"){
						ECB201(248)
						ROUT()
					}
					if (Input == "BIN"){
						ECB201(248)
						BIN()
					}
					if (Input == "BOUT"){
						ECB201(248)
						BOUT()
					}
				}
			}
		}
	}
}
END{
	print "CLOSE" > $iCP ".i"
	print "EXIT" > $iCP ".i"
	print "Created " ECBScript " omset script to enable hart child ECBs."
	close($iCP ".i")
	icc($iCP)
}
