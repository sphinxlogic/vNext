//# ==++== 
//# 
//#   
//#    Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//#   
//#    The use and distribution terms for this software are contained in the file
//#    named license.txt, which can be found in the root of this distribution.
//#    By using this software in any fashion, you are agreeing to be bound by the
//#    terms of this license.
//#   
//#    You must not remove this notice, or any other, from this software.
//#   
//# 
//# ==--== 
//####################################################################################
@cc_on


import System;

var NULL_DISPATCH = null;
var apGlobalObj;
var apPlatform;
var lFailCount;


var iTestID = 53663;

var g_OnDBCS = false;
var g_lang = "";
  
function GetLang()
{
	g_lang = apGlobalObj.apGetLangExt(apGlobalObj.LangHost());
	if(g_lang == "JP" || g_lang == "KO" || g_lang == "CHS" || g_lang == "CHT")
		g_OnDBCS = true;
}

function Function1 ( Arg) {
    return Arg + Arg;
}


function ucase01() {

    apInitTest("ucase01 ");
    GetLang();
    var Expected , Result;
    var Temp;

    apInitScenario("Scenario 1 - Use ucase$ with a function");
    Temp = "abc";
    Result = ("" +Function1(Temp)).toUpperCase();
    Expected = "ABCABC";
    if ( Result != Expected ) {
	 apLogFailInfo("(ucase with function returned " + Result + " not " + Expected + ")" ,"","","");
    }
    

	    apInitScenario("Scenario 1 - Use ucase$ with a function (DBCS)");
    if(g_OnDBCS)
    {	    Temp = "???|??F??L??";
	    Result = ("" +Function1(Temp)).toUpperCase();
	    Expected = "???|??F??L?????|??F??L??";
	    if ( Result != Expected ) {
		 apLogFailInfo("(ucase with function returned " + Result + " not " + Expected + ")" ,"","","");
	    }
    }

    apEndTest();
}


ucase01();


if(lFailCount >= 0) System.Environment.ExitCode = lFailCount;
else System.Environment.ExitCode = 1;

function apInitTest(stTestName) {
    lFailCount = 0;

    apGlobalObj = new Object();
    apGlobalObj.apGetPlatform = function Funca() { return "Rotor" }
    apGlobalObj.LangHost = function Funcb() { return 1033;}
    apGlobalObj.apGetLangExt = function Funcc(num) { return "EN"; }

    apPlatform = apGlobalObj.apGetPlatform();
    var sVer = "1.0";  //navigator.appVersion.toUpperCase().charAt(navigator.appVersion.toUpperCase().indexOf("MSIE")+5);
    apGlobalObj.apGetHost = function Funcp() { return "Rotor " + sVer; }
    print ("apInitTest: " + stTestName);
}

function apInitScenario(stScenarioName) {print( "\tapInitScenario: " + stScenarioName);}

function apLogFailInfo(stMessage, stExpected, stActual, stBugNum) {
    lFailCount = lFailCount + 1;
    print ("***** FAILED:");
    print ("\t\t" + stMessage);
    print ("\t\tExpected: " + stExpected);
    print ("\t\tActual: " + stActual);
}

function apGetLocale(){ return 1033; }
function apWriteDebug(s) { print("dbg ---> " + s) }
function apEndTest() {}
