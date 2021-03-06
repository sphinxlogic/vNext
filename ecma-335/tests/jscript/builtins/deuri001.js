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


var iTestID = 73348;


// Decimal number to binary string conversion
function bin (num, fAll)
{
	num = parseInt(num);
	if (fAll==null) fAll = false;
    var sBin="";

    for (var i=0; (num>>>i || fAll) && i<32; i++)
        sBin = (num>>>i&1) + sBin;

    return sBin;
}


// Decimal number to hexidecimal string conversion
function hex (num)
{
    num = parseInt(num);
    
    var sHex = "0123456789ABCDEF".charAt(num&0x0f);

    if (num>>>4) sHex = hex(num>>>4) + sHex;
    
    return sHex;
}


// Decimal to octal conversion
function oct (num)
{
    if (num < 0)
        return "-"+oct(Math.abs(num));

    if (num < 8)
        return ""+num;
    else
        return oct(Math.floor(num/8)) + num%8+"";
}


// Hex or oct to decimal conversion
function dec (num)
{
    if (typeof num != "string") return num;
    if (num.charAt(0)!= "0") num = "0x"+num;
    return eval(num);
}


// bin string to dec number conversion
function decBin (sNum)
{
    var val = 0, cb = sNum.length;
    for (var i=0; i<cb; i++)
        if (sNum.charAt(i)=="1") val |= 1<<(cb-1-i);
    return val;
}

// hex string to dec number conversion
// Handles signed int min (0x80000000) to signed int max (0x7fffffff), for
// everything else the behaviour is undefined.  The prefixes "0x"|"0X" are optional.
function decHex (sNum)
{
    var val = 0, cb = sNum.length;
    sNum = sNum.toLowerCase();
    for (var i=(sNum.substring(0,2)=="0x" ? 2 : 0); i<cb; i++)
        val |= "0123456789abcdef".indexOf(sNum.charAt(i))<<(4*(cb-1-i));
    return val;
}

// universal trim function
// defaults to trimming tabs, newlines, and spaces.
function trim (sIn, sTChars)
{
    if (!sIn.length) return sIn;
    if (sTChars == undefined) sTChars = "\t\n ";

    var iSt=-1, iEnd=sIn.length;

    while ( ++iSt<iEnd && sTChars.indexOf(sIn.charAt(iSt)) != -1 ) {}
    while ( --iEnd>iSt && sTChars.indexOf(sIn.charAt(iEnd)) != -1 ) {}

    return sIn.substring(iSt,iEnd+1);
}


// Equvilant to VBA's Chr function
function chr (num)
{
    if (num > 255 || num < 0) return null;

    var sHex = "0123456789abcdef";
    return unescape( "%"+sHex.charAt(num>>4&0x0f)+sHex.charAt(num&0x0f) );
}


// Equivilant to VBA's Asc function
function asc (str)
{
//    return parseInt( escape(str.charAt(0), 0).substring(1,3), 16 );
    return str.charCodeAt(0);
}


// Returns a "proper form" string from the input string sIn.
// The standard token delimiters are tab, newline, space, period, hypen,
// comma, & double/single quotes.  These can be overridden with the optional
// opDlm parameter and added to if the opDlm param contains a '+' char
// as the first char.
function proper (sIn, opDlm)
{
    var i;
    var std = "\t\n .-,\"\'";
    if (opDlm == undefined) opDlm = std;
    var sDlm = opDlm.charAt(0)=='+' ? std+opDlm.substring(1,opDlm.length) : opDlm;
    var ch, ich=0, cb=sIn.length, s=sIn.toLowerCase();

    for (i=0; i<cb; i++)
    {
        ch = s.charAt(i);
        if (sDlm.indexOf(ch)>=0)
            ich = i+1;
        else
            if (ich==i)
                s = s.substring(0,i)+ch.toUpperCase()+s.substring(i+1,cb);
    }
    
    return s;
}


// Equiv to VBA's String function
function bldStr (cb, str)
{
    if ( cb==undefined || 0 == cb || 0 == (""+str).length ) return "";
	if ( str==undefined ) str = " ";

    var sBld = (str + "");

    while (sBld.length * 2 < cb) sBld += sBld;

    if (sBld.length < cb)
        sBld += bldStr(cb - sBld.length, str);
    
    return sBld;
}


// Pads the base arg with as many pad arg's to make a string who's length is equal to n.
// sPad is optional, if not present, a space is used by default.
// Requires:  bldStr
function padStr (base, n, pad)
{
	var cb = n - ("" + base).length;

	if (pad == undefined) pad = " ";

	if (cb > 0 )
	    return bldStr(cb, pad) + base;

    return ""+base;
}


// Formats a string into cbBlk sized sections, delimited by sPad, from the right
// if fRight flag is set, from the left if not.  All args are optional except sSrc.
function fmtSBlock (sSrc, cbBlk, sPad, fRight)
{
    var i;
    // Default parameters
    if (fRight == undefined) fRight = false;
    if (sPad == undefined) sPad = " ";
    if (cbBlk == undefined || 0==cbBlk) cbBlk = sSrc.length;

    var cbMod = fRight ? sSrc.length%cbBlk : 0;

    var sRet = sSrc.substring(0,cbMod);
    
    for (i=0; i<sSrc.length-cbMod; i+=cbBlk)
        sRet += (sPad + sSrc.substring(i+cbMod,i+cbMod+cbBlk));

    return sRet.substring(!cbMod*sPad.length,sRet.length);
}


// Returns the string arg's chars randomly aranged in cbChunk sized chuncks
// cbChunk is optional, defaults to 1
function unsort (sIn, cbChunk)
{
    if (cbChunk == undefined) cbChunk = 1;
	var sOut="", iStrt=0, iEnd=0;
    while (sIn.length)
    {
        iStrt = Math.floor(Math.random()*Math.ceil(sIn.length/cbChunk))*cbChunk;
        iEnd = iStrt+cbChunk;
        sOut += sIn.substring(iStrt,iEnd);
        sIn = sIn.substring(0,iStrt) + (iEnd<sIn.length ? sIn.substring(iEnd, sIn.length) : "");
    }
    return sOut;
}


// Parameterized random string generator
// cb is the number of characters you want the string to return [optional, defaults to 100].
// bfType is a bit mask specifying the characters you want the string to contain [optional, 
// defaults to alphanumeric].  sDomain [optional] is a user supplied source string.
// Requires: chr
function genRndStr (cb, bfType, sDomain)
{
    var i;
    var sSource = "";
    if ( sDomain != undefined && typeof sDomain == "string")
        sSource = sDomain;

    if (bfType == undefined || bfType > 0)
    {
        if (cb == undefined) cb = 100;
        if (bfType == undefined) bfType = 0x01|0x02|0x04;

        var rgfWhichChars = new Array(256);
        for (i=1; i<rgfWhichChars.length; i++)
            rgfWhichChars[i] = 0;

        if (bfType & 0x01) // ucase alpha
            for (i=0x41; i<=0x5A; i++) rgfWhichChars[i]=1;

        if (bfType & 0x02) // lcase alpha
            for (i=0x61; i<=0x7A; i++) rgfWhichChars[i]=1;

        if (bfType & 0x04) // numbers
            for (i=0x30; i<=0x39; i++) rgfWhichChars[i]=1;

        if (bfType & 0x08) // all other printable chars (non-extended)
        {
            for (i=0x21; i<=0x2E; i++) rgfWhichChars[i]=1;
            for (i=0x3A; i<=0x40; i++) rgfWhichChars[i]=1;
            for (i=0x5B; i<=0x60; i++) rgfWhichChars[i]=1;
            for (i=0x7B; i<=0x7E; i++) rgfWhichChars[i]=1;
        }
        if (bfType & 0x10) // space
            rgfWhichChars[0x20]=1;

        if (bfType & 0x20) // tab
            rgfWhichChars[0x09]=1;

        if (bfType & 0x40) // newline
            rgfWhichChars[0x0A]=1;

        if (bfType & 0x80) // all other special chars
        {
            rgfWhichChars[0x08]=1; // backspace
            rgfWhichChars[0x0C]=1; // formfeed
            rgfWhichChars[0x0D]=1; // carriage return
        }
        if (bfType & 0x100) // non-printable non-extended chars
        {
            for (i=0x01; i<=0x07; i++) rgfWhichChars[i]=1;
                                           rgfWhichChars[0x0B]=1;
            for (i=0x0E; i<=0x1F; i++) rgfWhichChars[i]=1;
                                           rgfWhichChars[0x7F]=1;
        }
        if (bfType & 0x200) // extended chars (0x7f-0xff)
            for (i=0x80; i<=0xff; i++) rgfWhichChars[i]=1;

        // Create restricted char string
        for (i=0x00; i<0xff; i++)
            if (rgfWhichChars[i]) sSource += chr(i);
    }

    // Create random result string from restricted char string
    var sRet = "";
    for (i=0; i<cb; i++)
        sRet += sSource.charAt( Math.floor(Math.random()*sSource.length) );

    return sRet;
}
/* -------------------------------------------------------------------------
  Lib:		hlpUri
   
		
 
  Component:	JScript
 
  Area: 	URI
 
  Purpose:	Library routines for testing URI encoding and decoding
		methods
 ---------------------------------------------------------------------------
  
 
	[00]	21-Oct-1999	    Angelach: Created
 -------------------------------------------------------------------------*/

// see if the incomming number is representing a uriReserved
// character or #

function isReserved (num)
  {
      if ( num == 35 || num == 36 || num == 38 || num == 43 ||
	   num == 44 || num == 47 || num == 58 || num == 59 ||
	   num == 61 || num == 63 || num == 64 )
	 return true ;
       else
	 return false ;
  }

// see if the incomming number is representing an English alphabet

function isAlpha (num)
  {
      if ( ( num >= 65 && num <= 90 ) || ( num >= 97 && num <= 122  ) )
	 return true ;
       else
	 return false ;
  }

// see if the incomming number is representing a decimal digit

function isNumber (num)
  {
      if ( num >= 48 && num <= 57 )
	 return true ;
       else
	 return false ;
  }

// see if the incomming number is representing a uriMark

function isMark (num)
  {
      if ( num == 33 || num == 39 || num == 40 || num == 41 ||
	   num == 42 || num == 45 || num == 46 || num == 95 || num == 126 )
	 return true ;
       else
	 return false ;
  }

// converting a (pair of) number(s) to an escape sequence.  This function is
// created through observing results from encodeURI:
// num1 is the primary number: expected to be in the range of [0x00, 0xDBFF]
// and [0xE000, 0xFFFF]
// num1 is the secondary number: when num1 is in the range of [0xD800, 0xDBFF],
// it is expected to be in the range of [0xDC00, 0xDFFF]; otherwise, this
// number is ignored
// if num3 is 1, num1 will be checked to see if it is a uriReserved number

function getEncoding (num1, num2, num3)
  {
      var iTmp ;
      var sTmp = "" ;
					    // these are characters that won't be
      if ( isAlpha (num1) || isNumber(num1) || isMark(num1) )
	   return chr(num1) ;		    // these are characters that won't be

      if ( num3 == 1 )
	if ( isReserved(num1) ) 	    // these are uriReserved characters
	  return chr(num1) ;		    // that won't be encoded by encodeURI

      if ( num1 < 0x10 )		    // char in [0, 0x7f] is
	  sTmp = "%0" + hex (num1) ;	    // represented by 0zzzzzzz
      else				    // which is its ASCII value
	if ( num1 < 0x80 )		    // in hex
	   sTmp = "%" + hex (num1) ;	    // char in [0x80, 07ff] is
	else				    // represented by 2 octets:
	  if ( num1 < 0x800 )		    // 110yyyyy 10zzzzzz
	    sTmp = "%" + hex (0xc0+Math.floor(num1/0x40)) + "%" + hex(0x80+(num1 % 0x40)) ;
	  else				    // char in [0x800, 0xd7ff] or [0xe000, 0xffff]
	    if ( num1 < 0xD800 || num1 >= 0xE000 )  // is represented by 3
	      { 			    // octets: 1110xxxx 10yyyyyy 10zzzzzz
		 iTmp = Math.floor(num1 / 0x40) ;
		 sTmp = "%" + hex (0xE0 + Math.floor(iTmp/0x40)) ;
		 sTmp = sTmp + "%" + hex (0x80 + (iTmp % 0x40)) ;
		 sTmp = sTmp + "%" + hex (0x80 + (num1 % 0x40)) ;
	       }			    // char in [0xd800, 0xdbff] followed by
	     else			    // char in [0xdc00, 0xdfff] are represented
	       {			    // by 4 octets: 11110uuu 10uuwwwww 10xxyyyy
		  iTmp = num1 - 0xD7C0 ;    // and 10zzzzzzzz
		  sTmp = "%F" + hex (Math.floor(iTmp / 0x100)) ;
		  sTmp = sTmp + "%" + hex (Math.floor((iTmp - Math.floor(iTmp/0x100) * 0x100)/0x04) + 0x80) ;
		  sTmp = sTmp + "%" + hex(((num1-0xD800) % 0x04)*0x10 + ((Math.floor((num2 - 0xDC00)/0x40)) % 0x10) + 0x80) ;
		  sTmp = sTmp + "%" + hex(num2 % 0x40 + 0x80)
               }

       return sTmp ;
  }

// return true if Exp and Act are identical; otherwise, false is returned
//  this is needed because of posponed bug 314108
@if(@aspx) expando @end   function Verify (Exp, Act, Desc, Bug){
      if ( Act != Exp )
	{
	  if ( Bug )
	    apLogFailInfo (Desc, Exp, Act, Bug) ;
	  else
	    apLogFailInfo (Desc, Exp, Act, "") ;

	  return false ;
	}
      else
	return true ;
  }
/* -------------------------------------------------------------------------
  Test: 	DEURI001
   
		
 
  Component:	JScript
 
  Major Area:	Global method
 
  Test Area:	decodeURI
 
  Keyword:	decodeURI, fromCharCode
 
 ---------------------------------------------------------------------------
  Purpose:	Verify the functionality of decodeURI
 
  Scenarios:	1.  decodeURI with an empty string

		2.  decodeURI with an English alphabet

		3.  decodeURI with more than one alphabets

		4.  decodeURI with a decimal digit

		5.  decodeURI with more than one decimal digits

		6.  decodeURI with a #

		7.  decodeURI with more than one #'s

		8.  decodeURI with a uriMark ( -|_|.|!|~|*|'|(|) )

		9.  decodeURI with more than one uriMarks

		10. decodeURI with a uriReserved Char ( ;|/|?|:|@|&|=|+|$|, )

		11. decodeURI with more than one Reserved Chars

		12. decodeURI with a charcter that needs to be encoded

		13. decodeURI with more than one chars that need to be encoded

		14. decodeURI with 1 set of 'escaped' character

		15. decodeURI with more than 1 set of 'escaped' character

		16. decodeURI with different types of characters

		17. decodeURI with constant

		18. decodeURI with build-in object - not instantiated

		19. decodeURI with build-in object - instantiated

		20. decodeURI with user-defined object

		21. decodeURI with results from escape or encodeURIComponent

		22. decodeURI with argument other than string

		23. decodeURI repeatedly many times

  Abstract:	Call decodeURI and compare its output with expected value;
		verify that decodeURI returns corrected encoded string.  Also
		call decodeURI with output from decodeURI; verify that output
		from decodeURI can be dcoded.
 ---------------------------------------------------------------------------
  Category:	Functionality
 
  Product:	JScript
 
  Related Files: lib.js, string.js, hlpUri.js
 
  Notes:
 ---------------------------------------------------------------------------
  
 
	[00]	01-Nov-1999	    Angelach: Created Test
	[01]	20-Oct-2000	    Angelach: moved scenario 15 to deURI004
					      moved scenario 16 to deURI005
 -------------------------------------------------------------------------*/

function deuri001 ()
  {
     apInitTest( "deURI001 ");

     var cPer = "%" ;

     var Str1 = "" ;
     var Str2 = "" ;
     var Str3 = "" ;
     var Str4 = "" ;
     var iTmp = 0 ;
     var jTmp = 0 ;

     var funcObj = new Function ("x", "y", "return(x+y)") ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 1: decodeURI with an empty string");

     Str2 = decodeURI ("") ;
     Verify ( "", Str2, "decodeURI.1.1" ) ; // no decoding is expected

     Str1 = "" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.1.2" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 2: decodeURI with an alphabet letter");

     Str2 = decodeURI ("q") ;
     Verify ( "q", Str2, "decodeURI.2.1" ) ;// no decoding is expected

     Str2 = decodeURI ("Z") ;
     Verify ( "Z", Str2, "decodeURI.2.2" ) ;

     Str1 = "D" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.2.3" ) ;

     Str1 = "l" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.2.4" ) ;

     Str2 = decodeURI ("j".toUpperCase()) ;
     Verify ( "J", Str2, "decodeURI.2.5" ) ;

     Str2 = decodeURI (String.fromCharCode(115)) ;
     Verify ( "s", Str2, "decodeURI.2.6" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 3: decodeURI with more than 1 alphabet letters");

     Str2 = decodeURI ("publoaxkeg") ;	    // no decoding is expected
     Verify ( "publoaxkeg", Str2, "decodeURI.3.1" ) ;

     Str2 = decodeURI ("wwwwwwwwwwwwwwwwwwwwwwwwww") ;
     Verify ( "wwwwwwwwwwwwwwwwwwwwwwwwww", Str2, "decodeURI.3.2" ) ;

     Str2 = decodeURI ("OFNVCKTUB") ;
     Verify ( "OFNVCKTUB", Str2, "decodeURI.3.3" ) ;

     Str2 = decodeURI ("PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP") ;
     Verify ( "PPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPPP", Str2, "decodeURI.3.4" ) ;

     Str2 = decodeURI ("aZbYcXdWeVfUgThSiRjQkPlOmNnMoLpKqJrIwHtGuFvEwDxCyBzAzByCxDwEvFuGtHsIrJqKpLoMnNmOlPkQjRiWhTgufVeWdXcYbZ") ;
     Verify ( "aZbYcXdWeVfUgThSiRjQkPlOmNnMoLpKqJrIwHtGuFvEwDxCyBzAzByCxDwEvFuGtHsIrJqKpLoMnNmOlPkQjRiWhTgufVeWdXcYbZ", Str2, "decodeURI.3.5" ) ;

     Str1 = "z" ;
     for ( iTmp = 121 ; iTmp > 96 ; iTmp-- ) // lower case alphabets
       {
          Str1 = Str1.concat(String.fromCharCode(iTmp)) ;
	  Str2 = decodeURI (Str1) ;

	  Verify ( Str1, Str2, "decodeURI.3.6."+iTmp )
	}

     for ( iTmp = 90 ; iTmp > 64 ; iTmp-- ) // all upper case alphabets
       {
          Str1 = Str1.concat(String.fromCharCode(iTmp)) ;
	  Str2 = decodeURI (Str1) ;

	  Verify ( Str1, Str2, "decodeURI.3.7."+iTmp )
        } 

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 4: decodeURI with a decimal digit");

     Str2 = decodeURI ("5") ;		    // no encoding is expected
     Verify ( "5", Str2, "decodeURI.4.1" ) ;

     Str1 = "9" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.4.2" ) ;

     iTmp = 4 ;
     Str2 = decodeURI (iTmp.toString ()) ;
     Verify ( "4", Str2, "decodeURI.4.3" ) ;

     iTmp = 8 ;
     Str2 = decodeURI (iTmp.toString ()) ;
     Verify ( "8", Str2, "decodeURI.4.4" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 5: decodeURI with more than 1 decimal digits");

     Str2 = decodeURI ("3333333333") ;
     Verify ( "3333333333", Str2, "decodeURI.5.1" ) ;

     Str2 = decodeURI ("9081726354") ;
     Verify ( "9081726354", Str2, "decodeURI.5.2" ) ;

     Str1 = "666666666666666666666666666666666666666"
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.5.3" ) ;

     Str1 = "918366598137983333333339391849651673491034637645555555555518348161000"
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.5.4" ) ;

     iTmp = 13579 ;
     Str2 = decodeURI (iTmp) ;
     Verify ( "13579", Str2, "decodeURI.5.5" ) ;

     iTmp = 0xD800 ;
     Str1 = 0xD800 + "" ;
     Str2 = decodeURI (iTmp) ;
     Verify ( Str1, Str2, "decodeURI.5.6" ) ;

     iTmp = 0x80024012 ;
     Str1 = 0x80024012 + "" ;
     Str2 = decodeURI (iTmp) ;
     Verify ( Str1, Str2, "decodeURI.5.7" ) ;

     Str1 = "" ;
     for ( iTmp = 257 ; iTmp >= 0 ; iTmp-- )
       {
	  jTmp = iTmp % 10 ;

	  Str1 = Str1.concat(jTmp.toString ()) ;
	  Str2 = decodeURI (Str1) ;

	  Verify ( Str1, Str2, "decodeURI.5.8."+iTmp )
	}

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 6: decodeURI with a #");

     Str2 = decodeURI ("#") ;		    // no decoding is expected
     Verify ( "#", Str2, "decodeURI.6.1" ) ;

     Str1 = "#" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.6.2" ) ;

     Str2 = decodeURI (unescape("#")) ;
     Verify ( "#", Str2, "decodeURI.6.3" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 7: decodeURI with more than 1 #'s");

     Str2 = decodeURI ("##") ;
     Verify ( "##", Str2, "decodeURI.7.1" ) ;

     Str1 = "########################"
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.7.2" ) ;

     Str1 = "#" ;
     for ( iTmp = 0 ; iTmp < 300 ; iTmp++ )
       {
	  Str1 = Str1.concat("#") ;
	  Str2 = decodeURI (Str1) ;

	  Verify ( Str1, Str2, "decodeURI.7.3."+iTmp )
	}

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 8: decodeURI with an uriMark");

     Str2 = decodeURI ("*") ;		    // no decoding is expected
     Verify ( "*", Str2, "decodeURI.8.1" ) ;

     Str1 = "!" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.8.2" ) ;

     Str2 = decodeURI ("-_.!~*'()".charAt(1)) ;
     Verify ( "_", Str2, "decodeURI.8.3" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 9: decodeURI with more than 1 uriMarks");

     Str2 = decodeURI ("~~~~~~~~~~~~~~~~~~~~~~~~~~") ;
     Verify ( "~~~~~~~~~~~~~~~~~~~~~~~~~~", Str2, "decodeURI.9.1" ) ;

     Str1 = decodeURI ("(((") ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.9.2" ) ;

     Str2 = decodeURI (" )))))))))))))  ".substring(1, 14)) ;
     Verify ( ")))))))))))))", Str2, "decodeURI.9.3" ) ;

     Str2 = decodeURI ("--_________*___('!.))'))))).!!!!!!!*'~") ;
     Verify ( "--_________*___('!.))'))))).!!!!!!!*'~", Str2, "decodeURI.9.4" ) ;

     Str1 = "('~!._-*~!._--_.!~*'())-_*'().!~()'*~.!~*'(!._--_)-_.!~*)~!._-('*~!._-'(('*))"
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.9.5" ) ;

     Str2 = decodeURI ("abcde'!))*...!-(_.(~__...??".substr(6, 5)) ;
     Verify ( "!))*.", Str2, "decodeURI.9.6" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 10: decodeURI with an uriReserved character");

     Str2 = decodeURI ("&") ;		    // no decoding is expected
     Verify ( "&", Str2, "decodeURI.10.1" ) ;

     Str1 = "$" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.10.2" ) ;

     Str2 = decodeURI (";%^$=+abc".charAt(5)) ;
     Verify ( "+", Str2, "decodeURI.10.3" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 11: decodeURI with more than 1 uriReserved characters");

     Str2 = decodeURI ("//////////////////") ;
     Verify ( "//////////////////", Str2, "decodeURI.11.1" ) ;

     Str1 = ",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,," ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.11.2" ) ;

     Str2 = decodeURI ("[[   :::::  ]]".substring(5, 10)) ;
     Verify ( ":::::", Str2, "decodeURI.11.3" ) ;

     Str2 = decodeURI (";/,?$@&=????;&//::+") ;
     Verify ( ";/,?$@&=????;&//::+", Str2, "decodeURI.11.4" ) ;

     Str1 = ";/??::@/?@&&,,,&&;;&&==@@==:////:@===+++++$$+++$$$$,,$$$,,,,," ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.11.5" ) ;

     Str1 = "%FFFF ;:?,$/+?=:&,/434*abe**"
     Str2 = decodeURI (Str1.substr(8, 11)) ;
     Verify ( "?,$/+?=:&,/", Str2, "decodeURI.11.6" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 12: decodeURI with non alpha-numeric, #, uirMark, or uirReserved");

     Str2 = decodeURI (" ") ;		    // no decoding is expected
     Verify ( " ", Str2, "decodeURI.12.1" ) ;

     Str2 = decodeURI ("[") ;
     Verify ( "[", Str2, "decodeURI.12.2" ) ;

     Str2 = decodeURI (String.fromCharCode(0)) ;  // unprintable character
     Str1 = String.fromCharCode(0) ;
     Verify ( Str1, Str2, "decodeURI.12.3" ) ;

     Str2 = decodeURI ('\t') ;			   // unprintable character
     Verify ( '\t', Str2, "decodeURI.12.4" ) ;

     Str1 = '\f' ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.12.5" ) ;

     Str1 = String.fromCharCode(127) ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.12.6" ) ;

     for ( iTmp = 128 ; iTmp < 65536 ; iTmp += 32 )
       {
	 Str1 = String.fromCharCode(128) ;
	 Str2 = decodeURI (Str1) ;
	 Verify ( Str1, Str2, "decodeURI.12.6." + iTmp ) ;
       }

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 13: decodeURI with more than 1 non alpha-numeric, #, uirMark, or uirReserved");

     Str1 = "|[><]|" ;			    // in the range of [0, 127]
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.13.1" ) ;

     Str1 = String.fromCharCode(31, 30, 29, 28, 27, 26, 25, 24, 23, 22, 21, 20) ;
     Str1 = Str1 + String.fromCharCode(19, 18, 17, 16, 15, 14, 13, 12, 11, 10, 9) ;
     Str1 = Str1 + String.fromCharCode(8, 7, 6, 5, 4, 3, 2, 1, 0) ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.13.2" ) ;

     Str1 = "????????????????" ;		    // 218, 227, 234, 142, 177, 200, 245, 191
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.13.3" ) ;

     Str1 = "" ;
     for ( iTmp = 65535 ; iTmp > 127 ; iTmp -= 32 )
       {
	 Str1 = Str1+ String.fromCharCode(iTmp, iTmp-3, iTmp-6, iTmp-9, iTmp-12, iTmp-15) ;
	 Str1 = Str1+ String.fromCharCode(iTmp-18, iTmp-21, iTmp-24, iTmp-27, iTmp-30) ;
	 Str2 = decodeURI (Str1) ;
	 Verify ( Str1, Str2, "decodeURI.13.4." + iTmp ) ;
       }

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 14: decodeURI with 1 set of 'escaped' character");

     Str2 = decodeURI ("%0C") ; 	    // in the range of [0, 127]
     Str1 = String.fromCharCode(0x0C) ;
     Verify ( "\f", Str2, "decodeURI.14.1" ) ;

     Str2 = decodeURI ("%22") ;
     Str1 = String.fromCharCode(0x22) ;
     Verify ( Str1, Str2, "decodeURI.14.2" ) ;
     Str3 = encodeURI(Str2) ;
     Verify ( "%22", Str3, "encodeURI.14.2" ) ;

     Str2 = decodeURI ("%5e") ;
     Verify ( "^", Str2, "encodeURI.14.3" ) ;
     Str3 = encodeURI(Str2) ;
     Verify ( "%5E", Str3, "encodeURI.14.3" ) ;

     Str4 = "%7B" ;
     Str2 = decodeURI (Str4) ;
     Verify ( "{", Str2, "decodeURI.14.4" ) ;
     Str3 = encodeURI(Str2) ;
     Verify ( Str4, Str3, "encodeURI.14.4" ) ;

     Str1 = getEncoding (140) ;
     Str2 = decodeURI (Str1) ;		    // in the range of [128, 2047]
     Verify ( String.fromCharCode(140), Str2, "encodeURI.14.5" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.5" ) ;

     Str1 = getEncoding (253) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(253), Str2, "encodeURI.14.6" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.6" ) ;

     Str1 = getEncoding (597) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(597), Str2, "encodeURI.14.7" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.7" ) ;

     Str1 = getEncoding (1001) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(1001), Str2, "encodeURI.14.7" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.7" ) ;

     Str1 = getEncoding (1655) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(1655), Str2, "encodeURI.14.8" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.8" ) ;

     Str1 = getEncoding (2046) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(2046), Str2, "encodeURI.14.9" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.9" ) ;

     Str1 = getEncoding (2050) ;	    // in the range of [2048, 55295]
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(2050), Str2, "encodeURI.14.10" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.10" ) ;

     Str1 = getEncoding (4617) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(4617), Str2, "encodeURI.14.11" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.11" ) ;

     Str1 = getEncoding (10959) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(10959), Str2, "encodeURI.14.12" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.12" ) ;

     Str1 = getEncoding (20422) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(20422), Str2, "encodeURI.14.13" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.13" ) ;

     Str1 = getEncoding (31711) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(31711), Str2, "encodeURI.14.14" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.14" ) ;

     Str1 = getEncoding (44298) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(44298), Str2, "encodeURI.14.15" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.15" ) ;

     Str1 = getEncoding (55291) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(55291), Str2, "encodeURI.14.16" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.16" ) ;

     Str1 = getEncoding (57400) ;	    // in the range of [57344, 65535]
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(57400), Str2, "encodeURI.14.17" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.17" ) ;

     Str1 = getEncoding (60133) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(60133), Str2, "encodeURI.14.18" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.18" ) ;

     Str1 = getEncoding (63219) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(63219), Str2, "encodeURI.14.19" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.19" ) ;

     Str1 = getEncoding (65534) ;
     Str2 = decodeURI (Str1) ;
     Verify ( String.fromCharCode(65534), Str2, "encodeURI.14.20" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.14.20" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 15: decodeURI with more than 1 set of 'escaped' character");
/* [1]
     Str2 = decodeURI ("%14%5C%0E%1D%25") ;	// in the range of [0, 127]
     Str1 = String.fromCharCode(0x14, 0x5C, 0x0E, 0x1D, 0x25) ;
     Verify ( Str1, Str2, "decodeURI.15.1" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( "%14%5C%0E%1D%25", Str3, "encodeURI.15.1" ) ;

     Str2 = decodeURI ("%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04") ;
     Str1 = String.fromCharCode(0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04, 0x04) ;
     Str1 = Str1 + String.fromCharCode(0x04, 0x04, 0x04, 0x04, 0x04, 0x04) ;
     Str1 = Str1 + String.fromCharCode(0x04, 0x04, 0x04, 0x04) ;
     Verify ( Str1, Str2, "decodeURI.15.2" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( "%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04%04", Str3, "encodeURI.15.2" ) ;
					    // in the range of [128, 2047]
     Str4 = getEncoding(0x82)+getEncoding(0xAA)+getEncoding(0x82)+getEncoding(0xAD) ;
     Str4 = Str4+getEncoding(0x81)+getEncoding(0xC5)+getEncoding(0x83)+getEncoding(0x47) ;
     Str2 = decodeURI (Str4) ;
     Str1 = String.fromCharCode (0x82, 0xAA, 0x82, 0xAD, 0x81, 0xC5, 0x83, 0x47) ;
     Verify ( Str1, Str2, "decodeURI.15.3" ) ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str4, Str3, "encodeURI.15.3" ) ;

     Str4 = "" ;
     Str1 = "" ;
     for ( iTmp = 2047 ; iTmp > 127 ; iTmp -= 16 )
       {
	  Str4 = Str4+getEncoding(iTmp)+getEncoding(iTmp-12)+getEncoding(iTmp-6) ;
	  Str4 = Str4+getEncoding(iTmp-15)+getEncoding(iTmp-3)+getEncoding(iTmp-9) ;
	  Str1 = Str1+String.fromCharCode (iTmp, iTmp-12, iTmp-6, iTmp-15, iTmp-3, iTmp-9) ;

	  Str2 = decodeURI (Str4) ;
	  Verify ( Str1, Str2, "decodeURI.15.4."+iTmp ) ;
	  Str3 = encodeURI (Str2) ;
	  Verify ( Str4, Str3, "encodeURI.15.4."+iTmp ) ;
       }

     Str4 = "" ;			    // in the range of [2048, 55295]
     Str1 = "" ;
     for ( iTmp = 55295 ; iTmp > 2047 ; iTmp -= 418 )
       {
	  Str4 = Str4+getEncoding(iTmp-176)+getEncoding(iTmp-110)+getEncoding(iTmp-88) ;
	  Str4 = Str4+getEncoding(iTmp-44)+getEncoding(iTmp-154)+getEncoding(iTmp-374) ;
	  Str4 = Str4+getEncoding(iTmp)+getEncoding(iTmp-33)+getEncoding(iTmp-198) ;
	  Str4 = Str4+getEncoding(iTmp-22)+getEncoding(iTmp-319)+getEncoding(iTmp-132)
	  Str4 = Str4+getEncoding(iTmp-143)+getEncoding(iTmp-55)+getEncoding(iTmp-165) ;
	  Str4 = Str4+getEncoding(iTmp-11)+getEncoding(iTmp-187)+getEncoding(iTmp-99) ;
	  Str4 = Str4+getEncoding(iTmp-209)+getEncoding(iTmp-407)+getEncoding(iTmp-231) ;
	  Str4 = Str4+getEncoding(iTmp-242)+getEncoding(iTmp-385)+getEncoding(iTmp-264) ;
	  Str4 = Str4+getEncoding(iTmp-341)+getEncoding(iTmp-286)+getEncoding(iTmp-297) ;
	  Str4 = Str4+getEncoding(iTmp-308)+getEncoding(iTmp-121)+getEncoding(iTmp-396) ;
	  Str4 = Str4+getEncoding(iTmp-275)+getEncoding(iTmp-352)+getEncoding(iTmp-363) ;
	  Str4 = Str4+getEncoding(iTmp-77)+getEncoding(iTmp-253)+getEncoding(iTmp-330) ;
	  Str4 = Str4+getEncoding(iTmp-220)+getEncoding(iTmp-66)+getEncoding(iTmp-242) ;

	  Str1 = Str1+String.fromCharCode (iTmp-176, iTmp-110, iTmp-88, iTmp-44, iTmp-154) ;
	  Str1 = Str1+String.fromCharCode (iTmp-374, iTmp, iTmp-33, iTmp-198, iTmp-22) ;
	  Str1 = Str1+String.fromCharCode (iTmp-319, iTmp-132, iTmp-143, iTmp-55) ;
	  Str1 = Str1+String.fromCharCode (iTmp-165, iTmp-11, iTmp-187, iTmp-99, iTmp-209) ;
	  Str1 = Str1+String.fromCharCode (iTmp-407, iTmp-231, iTmp-242, iTmp-385, iTmp-264) ;
	  Str1 = Str1+String.fromCharCode (iTmp-341, iTmp-286, iTmp-297, iTmp-308, iTmp-121) ;
	  Str1 = Str1+String.fromCharCode (iTmp-396, iTmp-275, iTmp-352, iTmp-363, iTmp-77) ;
	  Str1 = Str1+String.fromCharCode (iTmp-253, iTmp-330, iTmp-220, iTmp-66, iTmp-242) ;

	  Str2 = decodeURI (Str4) ;
	  Verify ( Str1, Str2, "decodeURI.15.5."+iTmp ) ;
	  Str3 = encodeURI (Str2) ;
	  Verify ( Str4, Str3, "encodeURI.15.5."+iTmp ) ;
       }

     Str4 = "" ;			    // in the range of [55296, 56319]
     Str1 = "" ;			    // followed by [56320, 57343]
     jTmp = 56320 ;
     for ( iTmp = 56319 ; iTmp > 55295 ; iTmp -= 16 )
       {
	  Str4 = Str4+getEncoding(iTmp,jTmp)+getEncoding(iTmp-5,jTmp+15) ;
	  Str4 = Str4+getEncoding(iTmp-10,jTmp+5)+getEncoding(iTmp-15,jTmp+10) ;
	  Str1 = Str1+String.fromCharCode (iTmp, jTmp, iTmp-5, jTmp+15, iTmp-10, jTmp+5, iTmp-15, jTmp+10) ;

	  Str2 = decodeURI (Str4) ;
	  Verify ( Str1, Str2, "decodeURI.15.6."+iTmp ) ;
	  Str3 = encodeURI (Str2) ;
	  Verify ( Str4, Str3, "encodeURI.15.6."+iTmp ) ;

	  jTmp += 16 ;
       }

     Str4 = "" ;			    // in the range of [57344, 65535]
     Str1 = "" ;
     for ( iTmp = 65535 ; iTmp > 57343 ; iTmp -= 32 )
       {
	  Str4 = Str4+getEncoding(iTmp-25)+getEncoding(iTmp-20)+getEncoding(iTmp-30) ;
	  Str4 = Str4+getEncoding(iTmp-15)+getEncoding(iTmp-5)+getEncoding(iTmp)+getEncoding(iTmp-10) ;
	  Str1 = Str1+String.fromCharCode (iTmp-25, iTmp-20, iTmp-30, iTmp-15, iTmp-5, iTmp, iTmp-10) ;

	  Str2 = decodeURI (Str4) ;
	  Verify ( Str1, Str2, "decodeURI.15.7."+iTmp ) ;
	  Str3 = encodeURI (Str2) ;
	  Verify ( Str4, Str3, "encodeURI.15.7."+iTmp ) ;
       }

     for ( iTmp = 0 ; iTmp < 32 ; iTmp++ )
       {
	 Str4 = getEncoding(2048+iTmp*1664) + getEncoding(128+iTmp*60) ;
	 Str4 = Str4 + getEncoding(55296+iTmp*32, 56320+iTmp*32) ;
	 Str4 = Str4 + getEncoding(iTmp) + getEncoding(57344+iTmp*256) ;
	 Str1 = String.fromCharCode(2048+iTmp*1664, 128+iTmp*60, 55296+iTmp*32, 56320+iTmp*32, iTmp, 57344+iTmp*256) ;

	 Str2 = decodeURI (Str4) ;
	 Verify ( Str1, Str2, "decodeURI.15.8." + iTmp ) ;
	 Str3 = encodeURI (Str2) ;
	 Verify ( Str4, Str3, "encodeURI.15.8." + iTmp) ;
       }
*/
    //----------------------------------------------------------------------
     apInitScenario( "Scenario 16: decodeURI with different types of characters");
/* [1]
     Str4 = "http://msdn" + String.fromCharCode(181, 216) + ".microsoft.com/scripting/" ;
     Str4 = Str4 + getEncoding(8291)+getEncoding(11644)+getEncoding(11)+getEncoding(50000) ;
     Str1 = "http://msdn" + String.fromCharCode(181, 216) + ".microsoft.com/scripting/" ;
     Str1 = Str1 + String.fromCharCode(8291, 11644, 11, 50000) ;
     Str2 = decodeURI (Str4) ;
     Verify ( Str1, Str2, "decodeURI.16.1") ;
     Str3 = encodeURI (Str2) ;
     Str4 = "http://msdn" + getEncoding(181) + getEncoding(216) + ".microsoft.com/scripting/" ;
     Str4 = Str4 + getEncoding(8291)+getEncoding(11644)+getEncoding(11)+getEncoding(50000) ;
     Verify ( Str4, Str3, "encodeURI.16.1") ;

     Str4 = getEncoding(41432) + getEncoding(55601, 56985) + getEncoding(2617) ;
     Str4 = Str4 + getEncoding(65109) + getEncoding(1500) + getEncoding(11) ;
     Str4 = Str4 + "/%7C$$$$%7C.testing.com" ;
     Str1 = String.fromCharCode(41432, 55601, 56985, 2617, 65109, 1500, 11) ;
     Str1 = Str1 + "/|$$$$|.testing.com" ;
     Str2 = decodeURI (Str4) ;
     Verify ( Str1, Str2, "decodeURI.16.2") ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str4, Str3, "encodeURI.16.2") ;

     Str1 = "ftp://^^&*" + String.fromCharCode(1008, 710, 4993, 55829, 57207, 260, 33, 64009) ;
     Str1 = Str1 + "." + String.fromCharCode(7272, 12093) + "/end of Str1/" ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.16.3") ;

     Str4 = "%21%23%24" ;		     // build a string containing
     for ( iTmp = 38 ; iTmp < 60 ; iTmp++ )  // #, uriReserveds, uriMarks,
	Str4 = Str4 + "%" + hex(iTmp) ;      // alphas, and digits
     Str4 = Str4 + "%3d" ;
     for ( iTmp = 63 ; iTmp < 91 ; iTmp++ )
	Str4 = Str4 + "%" + hex(iTmp) ;
     Str4 = Str4 + "%5f" ;
     for ( iTmp = 97 ; iTmp < 123 ; iTmp++ )
	Str4 = Str4 + "%" + hex(iTmp) ;
     Str4 = Str4 + "%7e" ;
     Str1 = "!%23%24%26'()*%2B%2C-.%2F0123456789%3A%3B%3d%3F%40" ;
     Str1 = Str1 +"ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz~" ;
     Str2 = decodeURI (Str4) ;
     Verify ( Str1, Str2, "decodeURI.16.4." + iTmp ) ;

     Str1 = "!%2523%2524%2526'()*%252B%252C-.%252F0123456789%253A%253B%253d%253F%2540" ;
     Str1 = Str1 +"ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz~" ;
     Str3 = encodeURI (Str2) ;
     Verify ( Str1, Str3, "decodeURI.16.5." + iTmp) ;

     Str4 = "" ;
     Str1 = "" ;
     for ( iTmp = 0 ; iTmp < 128 ; iTmp++ )
       {
	 Str4 = Str4 + getEncoding(57344+iTmp*27) + getEncoding(2048+iTmp*341) ;
	 Str4 = Str4 + getEncoding(2048+iTmp*103) + getEncoding(2048+iTmp*273) ;
	 Str4 = Str4 + getEncoding(55296+iTmp*5, 57343-iTmp*1) + getEncoding(128+iTmp*8) ;
	 Str4 = Str4 + getEncoding(57344+iTmp*51) + getEncoding(2048+iTmp*205) ;
	 Str4 = Str4 + getEncoding(2048+iTmp*69) + getEncoding(2048+iTmp*375) ;
	 Str4 = Str4 + getEncoding(57344+iTmp*45) + getEncoding(57344+iTmp*9) ;
	 Str4 = Str4 + getEncoding(55296+iTmp*7, 57343-iTmp*5) + getEncoding(2048+iTmp*171) ;
	 Str4 = Str4 + getEncoding(55296+iTmp*1, 57343-iTmp*3) + getEncoding(iTmp, 0, 1) ;
	 Str4 = Str4 + getEncoding(128+iTmp*14) + getEncoding(57344+iTmp*15) ;
	 Str4 = Str4 + getEncoding(2048+iTmp*409) + getEncoding(128+iTmp*2) ;
	 Str4 = Str4 + getEncoding(128+iTmp*11) + getEncoding(2048+iTmp*307) ;
	 Str4 = Str4 + getEncoding(57344+iTmp*57) + getEncoding(57344+iTmp*39) ;
	 Str4 = Str4 + getEncoding(57344+iTmp*33) + getEncoding(55296+iTmp*3, 57343-iTmp*7) ;
	 Str4 = Str4 + getEncoding(2048+iTmp*239) + getEncoding(128+iTmp*5) ;
	 Str4 = Str4 + getEncoding(57344+iTmp*3) + getEncoding(2048+iTmp*35) ;
	 Str4 = Str4 + getEncoding(57344+iTmp*21) + getEncoding (2048+iTmp*137) ;

	 Str1 = Str1 + String.fromCharCode(57344+iTmp*27, 2048+iTmp*341, 2048+iTmp*103) ;
	 Str1 = Str1 + String.fromCharCode(2048+iTmp*273, 55296+iTmp*5, 57343-iTmp*1) ;
	 Str1 = Str1 + String.fromCharCode(128+iTmp*8, 57344+iTmp*51, 2048+iTmp*205) ;
	 Str1 = Str1 + String.fromCharCode(2048+iTmp*69, 2048+iTmp*375, 57344+iTmp*45) ;
	 Str1 = Str1 + String.fromCharCode(57344+iTmp*9, 55296+iTmp*7, 57343-iTmp*5) ;
	 Str1 = Str1 + String.fromCharCode(2048+iTmp*171, 55296+iTmp*1, 57343-iTmp*3) ;
	 Str1 = Str1 + String.fromCharCode(iTmp, 128+iTmp*14, 57344+iTmp*15, 2048+iTmp*409) ;
	 Str1 = Str1 + String.fromCharCode(128+iTmp*2, 128+iTmp*11, 2048+iTmp*307) ;
	 Str1 = Str1 + String.fromCharCode(57344+iTmp*57, 57344+iTmp*39, 57344+iTmp*33) ;
	 Str1 = Str1 + String.fromCharCode(55296+iTmp*3, 57343-iTmp*7, 2048+iTmp*239) ;
	 Str1 = Str1 + String.fromCharCode(128+iTmp*5, 57344+iTmp*3, 2048+iTmp*35) ;
	 Str1 = Str1 + String.fromCharCode(57344+iTmp*21, 2048+iTmp*137) ;

	 Str2 = decodeURI (Str4) ;
	 Verify ( Str1, Str2, "decodeURI.16.5." + iTmp ) ;

	 Str3 = encodeURI (Str2) ;
	 Verify ( Str4, Str3, "decodeURI.16.5." + iTmp) ;
       }
*/
    //----------------------------------------------------------------------
     apInitScenario( "Scenario 17: decodeURI with constant");

     Str2 = decodeURI (true) ;
     Verify ( "true", Str2, "decodeURI.17.1" ) ; // no encoding is expected

     Str2 = decodeURI (false) ;
     Verify ( "false", Str2, "decodeURI.17.2" ) ;

     Str2 = decodeURI (null) ;
     Verify ( "null", Str2, "decodeURI.17.3" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 18: decodeURI with build-in object - not instantiated");

    @cc_on

     Str1 = Array ;
     Str2 = decodeURI(Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.1" ) ;

     if (parseFloat(@_jscript_version)>=7)
       Str3 = "function%20Array()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
       Str3 = "%0Afunction%20Array()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
@if (@_fast)
  //     Str3 = "%0Afunction%20Array()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
@end
     Str2 = decodeURI(Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.2" ) ;

     Str1 = Boolean ;
     Str2 = decodeURI(Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.3" ) ;

     if (parseFloat(@_jscript_version)>=7)
       Str3 = "function%20Boolean()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
       Str3 = "%0Afunction%20Boolean()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.4" ) ;

     Str1 = Date ;
     Str2 = decodeURI(Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.5" ) ;

     if (parseFloat(@_jscript_version)>=7)
     Str3 = "function%20Date()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
     Str3 = "%0Afunction%20Date()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.6" ) ;

     Str1 = Enumerator ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.7" ) ;

     if (parseFloat(@_jscript_version)>=7)
     Str3 = "function%20Enumerator()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
     Str3 = "%0Afunction%20Enumerator()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.8" ) ;

     Str1 = Error ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.9" ) ;

     Str3 = "%5Bobject%20Error%5D" ;
     Str1 = "[object Error]"
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.10" ) ;

     Str1 = Function ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.11" ) ;

     if (parseFloat(@_jscript_version)>=7)
     Str3 = "function%20Function()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
     Str3 = "%0Afunction%20Function()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.12" ) ;

     Str1 = Math ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.13" ) ;

     Str3 = "%5Bobject%20Math%5D" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.14" ) ;

     Str1 = Number ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.15" ) ;

     if (parseFloat(@_jscript_version)>=7)
     Str3 = "function%20Number()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
     Str3 = "%0Afunction%20Number()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.18" ) ;

     Str1 = RegExp ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.17" ) ;

     if (parseFloat(@_jscript_version)>=7)
     Str3 = "function%20RegExp()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
     Str3 = "%0Afunction%20RegExp()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.18" ) ;

     Str1 = String ;
     Str2 = decodeURI (Str1) ;
     Verify ( Str1, Str2, "decodeURI.18.19" ) ;

     if (parseFloat(@_jscript_version)>=7)
     Str3 = "function%20String()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D" ;
     else
     Str3 = "%0Afunction%20String()%20%7B%0A%20%20%20%20%5Bnative%20code%5D%0A%7D%0A" ;
     Str2 = decodeURI (Str3) ;
     Verify ( Str1, Str2, "decodeURI.18.20" ) ;

    //----------------------------------------------------------------------
     if( new Date("12/25/2002 12:00:00 PST").getTimezoneOffset()/60 == 8) {
     apInitScenario( "Scenario 19: decodeURI with build-in object - instantiated");

     Str2 = decodeURI( new Array (10) ) ;
     Str1 = ",,,,,,,,," ;
     Verify ( Str1, Str2, "decodeURI.19.1" ) ;

     Str2 = decodeURI( new Array (2, 3, 4) ) ;
     Str1 = "2,3,4" ;
     Verify ( Str1, Str2, "decodeURI.19.2" ) ;

     Str2 = decodeURI (new Boolean (true)) ;
     Str1 = "true" ;
     Verify ( Str1, Str2, "decodeURI.19.3" ) ;

     Str2 = decodeURI (new Date(25542)) ;
     Str1 = "Wed Dec 31 16:00:25 PST 1969" ;
     Verify ( Str1, Str2, "decodeURI.19.4" ) ;

     Str2 = decodeURI (ScriptEngine () ) ;
     Str1 = "JScript" ;
     Verify ( Str1, Str2, "decodeURI.19.5" ) ;
     }

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 20: decodeURI with user-defined object");

     Str2 = decodeURI (funcObj) ;
     @if( @_jscript_version>=7)
	Str1 = "function anonymous(x, y) {\nreturn(x+y)\n}" ;
     @else
     	Str1 = "function anonymous(x, y) {\nreturn(x+y)\n}" ;
     @end
     Verify ( Str1, Str2, "decodeURI.20.1" ) ;

     Str2 = decodeURI (new Verify ("a", "a", "c")) ;
     Str1 = "[object Object]" ;
     Verify ( Str1, Str2, "decodeURI.20.2" ) ;

     Str2 = decodeURI (funcObj(3, 5)) ;
     Str1 = "8" ;
     Verify ( Str1, Str2, "decodeURI.20.3" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 21: decodeURI with result from escape or encodeURIComponent");

     Str2 = decodeURI (escape("on]0sdX,QBLW^lIDA@xUFwkR<&hb#za4=E7G[%5cVS?NMYiq9e/$1O!62ZP~(\\.J8 CjK*>3pHg\"ftr'yu)`T|:=-}vm;+_{")) ;
     Str1 = "on]0sdX%2CQBLW^lIDA@xUFwkR<%26hb%23za4%3DE7G[%5cVS%3FNMYiq9e/%241O!62ZP~(\\.J8 CjK*>3pHg\"ftr'yu)`T|%3A%3D-}vm%3B+_{" ;
     Verify ( Str1, Str2, "decodeURI.21.1" ) ;

     Str2 = decodeURI (encodeURIComponent("on]0sdX,QBLW^lIDA@xUFwkR<&hb#za4=E7G[%5cVS?NMYiq9e/$1O!62ZP~(\\.J8 CjK*>3pHg\"ftr'yu)`T|:=-}vm;+_{")) ;
     Str1 = "on]0sdX%2CQBLW^lIDA%40xUFwkR<%26hb%23za4%3DE7G[%5cVS%3FNMYiq9e%2F%241O!62ZP~(\\.J8 CjK*>3pHg\"ftr'yu)`T|%3A%3D-}vm%3B%2B_{" ;
     Verify ( Str1, Str2, "decodeURI.21.2" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 22: decodeURI with argument other than string");

     Str2 = decodeURI () ;		    // no argument
     Str1 = "undefined" ;
     Verify ( Str1, Str2, "decodeURI.22.1" ) ;

     Str2 = decodeURI (32767) ; 	    // numeric value
     Str1 = "32767" ;
     Verify ( Str1, Str2, "decodeURI.22.2" ) ;

     Str2 = decodeURI (5.3e+200*100) ;
     Str1 = "5.3e+202" ;
     Verify ( Str1, Str2, "decodeURI.22.3" ) ;

     Str2 = decodeURI (Math.floor(1202.88)) ;
     Str1 = "1202" ;
     Verify ( Str1, Str2, "decodeURI.22.4" ) ;

     Str2 = decodeURI (iTmp) ;
     Str1 = iTmp + "" ;
     Verify ( Str1, Str2, "decodeURI.22.5" ) ;

    //----------------------------------------------------------------------
     apInitScenario( "Scenario 23: decodeURI repeatedly many times");

     Str1 = "(Calling decodeURI many times 98 & 2, does it work correctly?) - " ;
     Str1 = Str1 + String.fromCharCode (6, 7839, 227, 55565, 56555, 31801) ;
     Str3 = encodeURI (Str1) ;
     for ( iTmp = 0 ; iTmp < 100 ; iTmp++ )
       {
	  Str2 = decodeURI (Str3) ;
	  Verify ( Str1, Str2, "decodeURI.23." + iTmp ) ;
       }

     apEndTest();
  }


deuri001();


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
