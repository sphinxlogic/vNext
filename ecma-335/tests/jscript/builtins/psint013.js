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


var iTestID = 53167;



@if(!@aspx)
	function obFoo() {};
@else
	expando function obFoo() {};
@end

var m_scen = "";

function psint013() {
 @if(@_fast)
    var sCat, sAct, sExp;
 @end

    apInitTest("psInt013 ");

    var INT_MIN =  -4294967295;
    var INT_MAX =  4294967295;

    //----------------------------------------------------------------------------
    apInitScenario("1. number, decimal, integer");

    m_scen = "number, decimal, integer";


    sCat = "min pos: 1";
    sExp = 1;
    sAct = parseInt(1,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 8983524";
    sExp = Number.NaN;
    sAct = parseInt(8983524,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos: 2147483647";
    sExp = 9020;
    sAct = parseInt(2147483647,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg: -1";
    sExp = -1;
    sAct = parseInt(-1,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -609";
    sExp = -48;
    sAct = parseInt(-609,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg: -2147483648";
    sExp = -9020;
    sAct = parseInt(-2147483648,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp = 0;
    sAct = parseInt(0,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("2. number, decimal, float");

    m_scen = "number, decimal, float";


    sCat = "min pos: 2.2250738585072014e-308";
    sExp =  2;
    sAct = parseInt(2.2250738585072014e-308,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 1008.31965";
    sExp = 64.;
    sAct = parseInt(1008.31965,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos";
    sExp = 1;
    sAct = parseInt(1.7976931348623158e308,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "> max pos float (1.#INF)";
    sExp = Number.NaN;
    sAct = parseInt(1.797693134862315807e309,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg";
    sExp = -2;
    sAct = parseInt(-2.225073858507202e-308,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: -448.18999837";
    sExp = -36.;
    sAct = parseInt(-448.18999837,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg";
    sExp =  -1;
    sAct = parseInt(-1.797693134862315807e308,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "< min neg float (-1.#INF)";
    sExp = Number.NaN;
    sAct = parseInt(-1.797693134862315807e309,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp = 0;
    sAct = parseInt(0.0,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("3. number, hexidecimal");

    m_scen = "number, hexidecimal";

    sCat = "min pos: 0x1";
    sExp = 1;
    sAct = parseInt(0x1,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 0xD4Bb9";
    sExp = Number.NaN;
    sAct = parseInt(0xD4Bb9,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos: 0xffffffff";
    sExp = 34;
    sAct = parseInt(0xffffffff,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg: -0x1";
    sExp = -1;
    sAct = parseInt(-0x1,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -0xfC";
    sExp = -170;
    sAct = parseInt(-0xfC,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -0xb15870";
    sExp = -2565450;
    sAct = parseInt(-0xb15870,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg: -0xffffffff";
    sExp = -34;
    sAct = parseInt(-0xffffffff,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp = 0;
    sAct = parseInt(0x0,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("4. number, octal");

    m_scen = "number, octal";

    sCat = "min pos";
    sExp = 1;
    sAct = parseInt(01,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 021013604067";
    sExp =  18;
    sAct = parseInt(021013604067,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos: 037777777777";
    sExp =  34;
    sAct = parseInt(037777777777,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg: -01";
    sExp =  -1;
    sAct = parseInt(-01,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -035164014173";
    sExp =  -3;
    sAct = parseInt(-035164014173,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg: -037777777777";
    sExp =  -34;
    sAct = parseInt(-037777777777,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp =  0;
    sAct = parseInt(00,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    
    //----------------------------------------------------------------------------
    apInitScenario("5. num string, decimal, integer");

    m_scen = "number, decimal, integer";

    sCat = "min pos: 1";
    sExp = 1;
    sAct = parseInt("1",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 463069357";
    sExp = 19654;
    sAct = parseInt("463069357",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos: 2147483647";
    sExp = 9020;
    sAct = parseInt("2147483647",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg: -1";
    sExp = -1;
    sAct = parseInt("-1",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -401449";
    sExp = -16484;
    sAct = parseInt("-401449",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp = 0;
    sAct = parseInt("0",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("6. num string, decimal, float");

    m_scen = "num string, decimal, float";

    sCat = "min pos: 2.2250738585072014e-308";
    sExp =  2;
    sAct = parseInt("2.2250738585072014e-308",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: -6742.8622527";
    sExp = -3554.;
    sAct = parseInt("-6742.8622527",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos";
    sExp = 1;
    sAct = parseInt("1.7976931348623158e308",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "> max pos float (1.#INF)";
    sExp = 1;
    sAct = parseInt("1.797693134862315807e309",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg";
    sExp = -2;
    sAct = parseInt("-2.2250738585072012595e-308",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: -9912.1922990";
    sExp = Number.NaN;
    sAct = parseInt("-9912.1922990",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg";
    sExp = -1;
    sAct = parseInt("-1.797693134862315807e308",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "< min neg float (-1.#INF)";
    sExp = -1;
    sAct = parseInt("-1.797693134862315807e309",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp = 0;
    sAct = parseInt("0.0",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

       
    //----------------------------------------------------------------------------
    apInitScenario("7. num string, hexidecimal");

    m_scen = "num string, hexidecimal";

    sCat = "min pos: 0x1";
    sExp = 0;
    sAct = parseInt("0x1",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 0x14";
    sExp = 0;
    sAct = parseInt("0x14",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos: 0xFFfFffFf";
    sExp = 0;
    sAct = parseInt("0xFFfFffFf",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg: -0x1";
    sExp = 0;
    sAct = parseInt("-0x1",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -0x54e6";
    sExp = 0;
    sAct = parseInt("-0x54e6",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg: -0xffFffFFF";
    sExp = 0;
    sAct = parseInt("-0xffFffFFF",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp = 0;
    sAct = parseInt("0x0",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("8. num string, octal");

    m_scen = "num string, octal";

    sCat = "min pos";
    sExp = 1;
    sAct = parseInt("01",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min pos < n < max pos: 06706";
    sExp =  3526;
    sAct = parseInt("06706",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max pos: 037777777777";
    sExp =  INT_MAX;
    sAct = parseInt("037777777777",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "max neg: -01";
    sExp =  -1;
    sAct = parseInt("-01",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg < n < max neg: -0201615";
    sExp =  -66445;
    sAct = parseInt("-0201615",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "min neg: -037777777777";
    sExp =  INT_MIN;
    sAct = parseInt("-037777777777",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "pos zero";
    sExp =  0;
    sAct = parseInt(00,8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("9. Alpha string");

    m_scen = "alpha string";

    sCat = "single space";
    sExp = Number.NaN;
    sAct = parseInt(" ",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "multiple spaces";
    sExp = Number.NaN;
    sAct = parseInt("                                                                   ",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as false";
    sExp = Number.NaN;
    sAct = parseInt("false",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as single word";
    sExp = Number.NaN;
    sAct = parseInt("foo",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as single word, leading space";
    sExp = Number.NaN;
    sAct = parseInt(" foo",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as single word, trailing space";
    sExp = Number.NaN;
    sAct = parseInt("foo ",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as multiple word";
    sExp = Number.NaN;
    sAct = parseInt("foo bar",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as multiple word, leading space";
    sExp = Number.NaN;
    sAct = parseInt(" foo bar",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "as multiple word, trailing space";
    sExp = Number.NaN;
    sAct = parseInt("foo bar ",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "zls";
    sExp = Number.NaN;
    sAct = parseInt("",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("10. random int number+string (printable chars only, no numbers)");

    m_scen = "random number+string, printable chars only";

    sCat = "205979647}Sa}OZ^U*WAV.\'x:CNZc\"cc!&<D\"\"REYKy] #tz},|QuMK<Htg\"	=y~zdA[?CyLW%?rS]pp&CjB(W\:K>Nou\D(n\'e&hw Z>[(bk";
    sExp = 133;
    sAct = parseInt("205979647}Sa}OZ^U*WAV.\'x:CNZc\"cc!&<D\"\"REYKy] #tz},|QuMK<Htg\"	=y~zdA[?CyLW%?rS]pp&CjB(W\:K>Nou\D(n\'e&hw Z>[(bk",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "333905919Y<=giSIC]E!Lr<RtnH	QS(up$+|ENSl*zimO%uGw*m%?YmR<-b|hw$cP*)~aNx\nN^b;M,T?{bJHZL;(;|^*oUZ[\)*ao^fNB\"w{T";
    sExp = 219;
    sAct = parseInt("333905919Y<=giSIC]E!Lr<RtnH	QS(up$+|ENSl*zimO%uGw*m%?YmR<-b|hw$cP*)~aNx\nN^b;M,T?{bJHZL;(;|^*oUZ[\)*ao^fNB\"w{T",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "1490812927SrMPZe#YB*oVS\nV*F^NbRF}kk[MO$UO`:LgXR\tFL sMz.&YwAG[:&M.z+TtoJNx@%jbjv,ycY%m*AxW<Qq\"z]ENR&m,yh?S\'m=Pe";
    sExp = 12;
    sAct = parseInt("1490812927SrMPZe#YB*oVS\nV*F^NbRF}kk[MO$UO`:LgXR\tFL sMz.&YwAG[:&M.z+TtoJNx@%jbjv,ycY%m*AxW<Qq\"z]ENR&m,yh?S\'m=Pe",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    //----------------------------------------------------------------------------
    apInitScenario("11. random int number+string (non-printable chars)");

    m_scen = "random number+string, non-printable chars";

    sCat = "1703804927??????.????????????????????.???.???????????.???????????????????????????.?????????????????.???";
    sExp = 963;
    sAct = parseInt("1703804927??????.????????????????????.???.???????????.???????????????????????????.?????????????????.???",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "429981695.????.???????????..????????.????.????????????????.???.??????.????.?.???????????.??????????????.??.";
    sExp = 34;
    sAct = parseInt("429981695.????.???????????..????????.????.????????????????.???.??????.????.?.???????????.??????????????.??.",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "585695231??.??????????????.?..????????.??.?????????.?????????..?????..??.????????????????.??????.??????";
    sExp = 5;
    sAct = parseInt("585695231??.??????????????.?..????????.??.?????????.?????????..?????..??.????????????????.??????.??????",8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("12. random string(printable chars only)+number");

    m_scen = "random string(printable chars only)+number";

    sCat = "}Sa}OZ^U*WAV.\'x:CNZc\"cc!&<D\"\"REYKy] #tz},|QuMK<Htg\"	=y~zdA[?CyLW%?rS]pp&CjB(W\:K>Nou\D(n\'e&hw Z>[(bk205979647";
    sExp = Number.NaN;
    sAct = parseInt("}Sa}OZ^U*WAV.\'x:CNZc\"cc!&<D\"\"REYKy] #tz},|QuMK<Htg\"	=y~zdA[?CyLW%?rS]pp&CjB(W\:K>Nou\D(n\'e&hw Z>[(bk205979647",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "<=giSIC]E!Lr<RtnH	QS(up$+|ENSl*zimO%uGw*m%?YmR<-b|hw$cP*)~aNx\nN^b;M,T?{bJHZL;(;|^*oUZ[\)*ao^fNB\"w{T333905919Y";
    sExp = Number.NaN;
    sAct = parseInt("<=giSIC]E!Lr<RtnH	QS(up$+|ENSl*zimO%uGw*m%?YmR<-b|hw$cP*)~aNx\nN^b;M,T?{bJHZL;(;|^*oUZ[\)*ao^fNB\"w{T333905919Y",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "SrMPZe#YB*oVS\nV*F^NbRF}kk[MO$UO`:LgXR\tFL sMz.&YwAG[:&M.z+TtoJNx@%jbjv,ycY%m*AxW<Qq\"z]ENR&m,yh?S\'m=Pe1490812927";
    sExp = Number.NaN;
    sAct = parseInt("SrMPZe#YB*oVS\nV*F^NbRF}kk[MO$UO`:LgXR\tFL sMz.&YwAG[:&M.z+TtoJNx@%jbjv,ycY%m*AxW<Qq\"z]ENR&m,yh?S\'m=Pe1490812927",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("13. random string(non-printable chars)+number");

    m_scen = "random string(non-printable chars)+number";

    sCat = ".???.?????..?.?????.????.??????.??.????????..?.???.??????????????.????.?.?.???.???.?..????.?????.2049966079";
    sExp = Number.NaN;
    sAct = parseInt(".???.?????..?.?????.????.??????.??.????????..?.???.??????????????.????.?.?.???.???.?..????.?????.2049966079",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = ".???????????..?.???????.?????..?????.?????????.????????.?????.?.??????????????..??????????.???908066815";
    sExp = Number.NaN;
    sAct = parseInt(".???????????..?.???????.?????..?????.?????????.????????.?????.?.??????????????..??????????.???908066815",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "????.?????...?????????????????..????.??????.?.???????.???????.???.???????????.??.??.????????????2028077055";
    sExp = Number.NaN;
    sAct = parseInt("????.?????...?????????????????..????.??????.?.???????.???????.???.???????????.??.??.????????????2028077055",8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("14. objects, built-in, non-exec");

    m_scen = "objects, built-in, non-exec";

    sCat = "Math";
    sExp = Number.NaN;
    sAct = parseInt(Math,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("15. objects, built-in, exec, not inst");

    m_scen = "objects, built-in, exec, not inst";

    sCat = "Boolean";
    sExp = Number.NaN;
    sAct = parseInt(Boolean,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "Date";
    sExp = Number.NaN;
    sAct = parseInt(Date,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("16. objects, built-in, exec, inst");

    m_scen = "objects, built-in, exec, inst";

    sCat = "new Number()";
    sExp = 0;
    sAct = parseInt(new Number(),8);

    if (sAct != sExp)
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "new Object()";
    sExp = Number.NaN;
    sAct = parseInt(new Object(),8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("17. objects, user-def, not inst");

    m_scen = "objects, user-def, not inst";

    sCat = "obFoo";
    sExp = Number.NaN;
    sAct = parseInt(obFoo,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("18. objects, user-def, inst");

    m_scen = "objects, user-def, inst";

    sCat = "new obFoo()";
    sExp = Number.NaN;
    sAct = parseInt(new obFoo(),8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("19. constants");

    m_scen = "constants";

    sCat = "true";
    sExp = Number.NaN;
    sAct = parseInt(true,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");

    sCat = "false";
    sExp = Number.NaN;
    sAct = parseInt(false,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("20. null");

    m_scen = "null";

    sCat = "";
    sExp = Number.NaN;
    sAct = parseInt(null,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");


    //----------------------------------------------------------------------------
    apInitScenario("21. undefined");
    
    m_scen = "undefined";
    
    var obUndef;
    sCat = "";
    sExp = Number.NaN;
    sAct = parseInt(obUndef,8);

    if (!isNaN(sAct))
        apLogFailInfo( m_scen+(sCat.length?"--"+sCat:"")+" failed", sExp, sAct, "");



    apEndTest();

}




psint013();


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
