// ==++==
//
//   
//    Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//   
//    The use and distribution terms for this software are contained in the file
//    named license.txt, which can be found in the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by the
//    terms of this license.
//   
//    You must not remove this notice, or any other, from this software.
//   
//
// ==--==

using System;
using System.Runtime.Serialization;

class PlaceHolder {
   public int holder;
}

class Test {

   public void ForceCollection( )
   {
       PlaceHolder p = new PlaceHolder();
   }

   public long GetID( ObjectIDGenerator IDGen )
   {
       PlaceHolder p = new PlaceHolder();
       p.holder = 0;
       bool temp;
       lock (p) {
       return IDGen.GetId(p, out temp); }
   }

   public  void RunTest ()
   {
       ObjectIDGenerator IDGen = new ObjectIDGenerator();
       int i;

       for (i = 0; i < 100; i++)
         GetID(IDGen); 
   } 

   public static void Main ()
   {
       Test T = new Test();
       T.RunTest();
    
       System.GC.Collect();
   } 
}


