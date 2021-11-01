// RUN: %dafny /env:0 /compile:0 "%s" > "%t.result"
// RUN: %diff "%s.expect" "%t.result"

module A {
  export Spec provides *
  export Body reveals *

  datatype T = CT1 | CT2

  function getCT1() : T { CT1 }
  function getCT2() : T { CT2 }

  class AClass {
     function method getCT1(): T { CT1 }
     constructor Init() {} 
  }

}

module B {
  import A`Spec

  function f(): A.T { A.getCT1() }
  lemma Test(x : A.T)
  ensures x == A.getCT1() || x == A.getCT2() { } //can't prove this here

  method TestClass() {
    var a := new A.AClass.Init();
    var f := a.getCT1();
    assert f == A.getCT1();// can't prove this here
  }
}

module C {
  import A`Body

  function f(): A.T { A.CT1 }

  lemma Test()
  ensures A.CT1 == A.getCT1() { }

  lemma Test2(x : A.T)
  ensures x == A.getCT1() || x == A.getCT2() { }
 
  method TestClass() {
    var a := new A.AClass.Init();
    var f := a.getCT1();
    assert f == A.getCT1();
  }
 
}
