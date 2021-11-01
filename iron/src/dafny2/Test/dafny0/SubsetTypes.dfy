// RUN: %dafny /print:"%t.print" "%s" > "%t"
// RUN: %diff "%s.expect" "%t"

module AssignmentToNat {
  method M(x0: int, x1: int, x2: int, x3: int)
  {
    var a: nat := x0;  // error
    a := x1;  // error
    var z;
    z := var b: nat := x2; b;  // error
    z := var b: nat :| b == x3; b;  // error
  }
  method M'(x0: int, x1: int, x2: int, x3: int, x4: int)
  {
    var c: nat :| c == x0;  // error
    c :| c == x1;  // error

    var g := new G;
    g.u := x2;  // error

    var y := F(x3);  // error

    M''(x4);  // error
  }
  method M''(x: nat)

  function method F(q: nat): int
  function method F'(x: int): nat
  {
    F      // error (regarding result of F(x))
      (x)  // error (regaring argument to F)
  }

  class G {
    var u: nat
  }

  function method Pf(n: nat): int
  function method Pg(x: int): nat

  method P(x: int, f: nat ~> int) returns (g: int ~> nat)
    requires f.requires(x)  // error
  {
    var y := f(x);  // error
    g := z => z;  // error
  }
  method Q(x: int) {
    var f := Pf;
    var g := Pg;
    var a := f(x);  // error
    var id := (u: int) => u;
    g := id;  // error
  }

  function Id(x: int): nat
  {
    x  // error
  }
}

module AssignmentToSetNat {
  method M(x0: set<int>, x1: set<int>, x2: set<int>, x3: set<int>)
  {
    var a: set<nat> := x0;  // error
    a := x1;  // error
    var z;
    z := var b: set<nat> := x2; b;  // error
    z := var b: set<nat> :| b == x3; b;  // error
  }
  method M'(x0: set<int>, x1: set<int>, x2: set<int>, x3: set<int>, x4: set<int>)
  {
    var c: set<nat> :| c == x0;  // error
    c :| c == x1;  // error

    var g := new G;
    g.u := x2;  // error

    var y := F(x3);  // error

    M''(x4);  // error
  }
  method M''(x: set<nat>)

  function method F(q: set<nat>): set<int>
  function method F'(x: set<int>): set<nat>
  {
    F      // error (regarding result of F(x) in the body)
      (x)  // error (regaring argument to F)
  }

  class G {
    var u: set<nat>
  }

  function method Pf(n: set<nat>): set<int>
  function method Pg(x: set<int>): set<nat>

  method P(x: set<int>, f: set<nat> ~> set<int>) returns (g: set<int> ~> set<nat>)
    requires f.requires(x)  // error
  {
    var y := f(x);  // error
    g := z => z;  // error
  }
  method Q(x: set<int>) {
    var f := Pf;
    var g := Pg;
    var a := f(x);  // error
    var id := u => u;
    g := id;  // error
  }

  function Id(x: set<int>): set<nat>
  {
    x  // error
  }
}

module OutParameters {
  method P<T>(x: T) returns (y: T) { y := x; }

  method M() returns (x: int, n: nat) {
    if {
      case true =>  x := P<nat>(n);
      case true =>  n := P<nat>(x);  // error (x)
      case true =>  x := P<int>(n);
      case true =>  n := P<int>(x);  // error (n)
    }
  }
}

module Contravariance {
  method M0(a: int ~> int, b: int ~> nat, c: nat ~> int, d: nat ~> nat) returns (r: int ~> int) {
    if {
      case true =>  r := a;
      case true =>  r := b;
      case true =>  r := c;  // error
      case true =>  r := d;  // error
    }
  }
  method M1(a: int ~> int, b: int ~> nat, c: nat ~> int, d: nat ~> nat) returns (r: int ~> nat) {
    if {
      case true =>  r := a;  // error
      case true =>  r := b;
      case true =>  r := c;  // error
      case true =>  r := d;  // error
    }
  }
  method M2(a: int ~> int, b: int ~> nat, c: nat ~> int, d: nat ~> nat) returns (r: nat ~> int) {
    if {
      case true =>  r := a;
      case true =>  r := b;
      case true =>  r := c;
      case true =>  r := d;
    }
  }
  method M3(a: int ~> int, b: int ~> nat, c: nat ~> int, d: nat ~> nat) returns (r: nat ~> nat) {
    if {
      case true =>  r := a;  // error
      case true =>  r := b;
      case true =>  r := c;  // error
      case true =>  r := d;
    }
  }
}

module AssignSuchThat {
  type SmallInt = x | 0 <= x < 10
  method M() {
    var a: int :| a == 11;
    var b: SmallInt :| b == 11;  // error
  }
}

module SoftCasts {
  method P(n: nat) returns (g: int ~> nat)
  {
    if
    case true =>
      g := z => if z < 0 then -z else z;
    case true =>
      g := z => if z <= 0 then -z else z-1;
    case true =>
      g := (z: int) => if z < 0 then -z else z-1;  // error: may fail to return a nat
    case true =>
      g := z => n;
  }
}

module AssignmentsFromNewAllocation {
  // Many of the tests in this module are regression tests
  class Person { }
  class Cell<T> { }

  method J(N: nat, p: Person) {
    var a: array<Person?>;
    var b: array<Person>;
    if * {
      var c;
      c := new Person[0](_ => null);  // fine
      c := new Person[1](_ => null);  // error: null is not a Person
    }
    a := new Person?[N](_ => p);
    a := new Person?[N](_ => null);
    b := new Person[N](_ => p);
    b := new Person[N](_ => null);  // error: null is not a Person
  }

  method K(p: Person) returns (a: array<Person?>, b: array<Person>) {
    if * {
      a := new Person[0];  // error: cannot assign array<Person> to array<Person?>
    } else if * {
      a := new Person[100](_ => p);  // error: ditto
    } else if * {
      a := new Person[] [p, p];  // error: ditto
    } else if * {
      a := b;  // error: ditto
    }
  }

  method L(p: Person) returns (a: array<Person?>, b: array<Person>) {
    if * {
      b := new Person?[100];  // error: cannot assign array<Person?> to array<Person>
    } else if * {
      b := new Person?[100](_ => p);  // error: ditto
    } else if * {
      b := new Person?[] [p, p];  // error: ditto
    } else if * {
      b := a;  // error: ditto
    }
  }

  method M(N: nat, p: Person) {
    var a: array<Person?>;
    var b: array<Person>;
    if * {
      a := b;  // error
    } else if * {
      b := a;  // error
    } else if * {
      a := new Person?[N](_ => p);
      b := a;  // error
    } else if * {
      a := new Person[N](_ => p);  // error: cannot assign array<Person> to array<Person?>
    } else if * {
      b := new Person?[N](_ => p);  // error: cannot assign array<Person?> to array<Person>
    }
  }

  method N(p: Person) returns (a: array<Person>, b: array<Person?>)
  {
    var c := new Person[1] [p];
    var d := new Person?[1];
    if * {
      assume b.Length == 0;  // the next line is an error even in this case
      a := b;  // error
    } else if * {
      assume a.Length == 0;  // the next line is an error even in this case
      b := a;  // error
    } else if * {
      a := c;
      b := c;  // error
    } else if * {
      b := d;
      a := d;  // error
    }
  }

  // unlike array, seq is co-variant in its argument type
  method O(p: Person) returns (a: seq<Person>, b: seq<Person?>)
  {
    var c: seq<Person> := [p];
    var d: seq<Person?> := [p];
    var e: seq<Person?> := [null];
    if * {
      assume |b| == 0;
      a := b;  // fine, given the previous line
    } else if * {
      b := a;
    } else if * {
      a := b;  // error
    } else if * {
      a := c;
      b := c;
    } else if * {
      b := d;
      a := d;
    } else if * {
      b := e;
      a := e;  // error
    }
  }

  method P(cc: Cell<Person?>, dd: Cell<Person>)
  {
    var c: Cell<Person?>;
    var d: Cell<Person>;
    if * {
      c := new Cell<Person?>;
    } else if * {
      d := new Cell<Person?>;  // error: Cell<Person?> is not assignable to Cell<Person>
    } else if * {
      c := new Cell<Person>;  // error: Cell<Person> is not assignable to Cell<Person?>
    } else if * {
      d := new Cell<Person>;
    } else if * {
      c := dd;  // error: Cell<Person> is not assignable to Cell<Person?>
    } else if * {
      d := cc;  // error: Cell<Person?> is not assignable to Cell<Person>
    }
  }
}

module RegressionsSubsetElementTypes {
  type byte = x | 0 <= x < 256

  datatype Dt =
    | One(byte)
    | Many(seq<byte>)
    | Sany(set<byte>)
    | Bany(multiset<byte>)
    | Pany(map<byte,byte>)

  method M0() returns (dt: Dt)
  {
    if
    case true =>  dt := One(500);  // error
    case true =>  dt := Many([500]);  // error
    case true =>  dt := Sany({500});  // error
    case true =>  dt := Bany(multiset{500});  // error
  }
  method M1() returns (dt: Dt)
  {
    if
    case true =>  dt := Pany(map[500 := 20]);  // error
    case true =>  dt := Pany(map[20 := 500]);  // error
  }
  method M2(x: int, s: seq<int>, t: set<int>, u: multiset<int>, m: map<int,int>) returns (dt: Dt)
  {
    if
    case true =>  dt := One(x);  // error
    case true =>  dt := Many(s);  // error
    case true =>  dt := Sany(t);  // error
    case true =>  dt := Bany(u);  // error
    case true =>  dt := Pany(m);  // error
  }
  method M3(s: seq<int>, t: set<int>, u: multiset<int>, m: map<int,int>) returns (dt: Dt)
  {
    if
    case true =>
    case forall i :: 0 <= i < |s| ==> 0 <= s[i] < 256 =>
      dt := Many(s);
    case forall x :: x in t ==> 0 <= x < 256 =>
      dt := Sany(t);
    case forall x :: x in u ==> 0 <= x < 256 =>
      dt := Bany(u);
    case forall x :: x in m.Keys ==> 0 <= x < 256 && 0 <= m[x] < 256 =>
      dt := Pany(m);
  }

  method M4(dm: Dt) returns (x: int, s: seq<int>, t: set<int>, u: multiset<int>, m: map<int,int>)
  {
    match dm
    case One(a) =>  x := a;
    case Many(b) =>  s := b;
    case Sany(c) =>  t := c;
    case Bany(d) =>  u := d;
    case Pany(e) =>  m := e;
  }
}
