// RUN: %dafny /env:0 /compile:0 "%s" > "%t.result"
// RUN: %diff "%s.expect" "%t.result"

module A {
   export Spec provides T, AClass, AClass.f, AClass.Init
   export Body reveals T, AClass
 
   type T = int
   class AClass {
     function method f(): T { 0 }
     function method g(): int { f() }
     constructor Init() { }
   }
}

module B {
  import A = A`Spec
  
  method Test() {
    var a := new A.AClass.Init();
    var f : A.T := a.f();
  }

}

module C {
  import A = A`Body

  method Test() {
    var a := new A.AClass.Init();
    var f : int := a.f();
    var g : A.T := a.g();
    var e := f == g;
  }

}
