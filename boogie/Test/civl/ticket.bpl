// RUN: %boogie -noinfer -typeEncoding:m -useArrayTheory "%s" > "%t"
// RUN: %diff "%s.expect" "%t"

function RightOpen (n: int) : [int]bool;
function RightClosed (n: int) : [int]bool;
axiom (forall x: int, y: int :: RightOpen(x)[y] <==> y < x);
axiom (forall x: int, y: int :: RightClosed(x)[y] <==> y <= x);

type X;
const nil: X;
var {:layer 0,1} t: int;       // next ticket to issue
var {:layer 0,2} s: int;       // current ticket permitted to critical section
var {:layer 0,2} cs: X;        // current thread in critical section
var {:layer 0,2} T: [int]bool; // set of issued tickets

// ###########################################################################
// Invariants

function {:inline} Inv1 (tickets: [int]bool, ticket: int): (bool)
{
    tickets == RightOpen(ticket)
}

function {:inline} Inv2 (tickets: [int]bool, ticket: int, lock: X): (bool)
{
    if (lock == nil) then tickets == RightOpen(ticket)
                     else tickets == RightClosed(ticket)
}

// ###########################################################################
// Yield assertions

procedure {:yields} {:layer 2} YieldSpec ({:linear "tid"} tid: X)
requires {:layer 2} tid != nil && cs == tid;
ensures  {:layer 2} tid != nil && cs == tid;
{
    yield;
    assert {:layer 2} tid != nil && cs == tid;
}

procedure {:yields} {:layer 1} Yield1 ()
requires {:layer 1} Inv1(T, t);
ensures  {:layer 1} Inv1(T, t);
{
    yield;
    assert {:layer 1} Inv1(T, t);
}

procedure {:yields} {:layer 2} Yield2 ()
requires {:layer 2} Inv2(T, s, cs);
ensures  {:layer 2} Inv2(T, s, cs);
{
    yield;
    assert {:layer 2} Inv2(T, s, cs);
}

// ###########################################################################
// Main program

procedure {:yields} {:layer 2} main ({:linear_in "tid"} xls':[X]bool)
requires {:layer 2} xls' == MapConstBool(true);
{
    var {:linear "tid"} tid: X;
    var {:linear "tid"} xls: [X]bool;

    yield;

    call InitAbstract(xls');
    xls := xls';

    par Yield1() | Yield2();

    while (*)
    invariant {:layer 1} Inv1(T, t);
    invariant {:layer 2} Inv2(T, s, cs);
    {
        par xls, tid := Allocate(xls) | Yield1() | Yield2();
        async call Customer(tid);
        par Yield1() | Yield2();
    }
    par Yield1() | Yield2();
}

procedure {:yields} {:layer 2} Allocate ({:linear_in "tid"} xls':[X]bool) returns ({:linear "tid"} xls: [X]bool, {:linear "tid"} xl: X)
ensures {:layer 1,2} xl != nil;
{
    yield;
    call xls, xl := AllocateLow(xls');
    yield;
}

procedure {:yields} {:layer 2} Customer ({:linear_in "tid"} tid: X)
requires {:layer 1} Inv1(T, t);
requires {:layer 2} Inv2(T, s, cs) && tid != nil;
{
    par Yield1() | Yield2();    
    while (*) 
    invariant {:layer 1} Inv1(T, t);
    invariant {:layer 2} Inv2(T, s, cs);
    {
        call Enter(tid);
        par Yield1() | Yield2() | YieldSpec(tid);
        call Leave(tid);
        par Yield1() | Yield2();
    }
    par Yield1() | Yield2();
}

procedure {:yields} {:layer 2} Enter ({:linear "tid"} tid: X)
requires {:layer 1} Inv1(T, t);
ensures  {:layer 1} Inv1(T, t);
requires {:layer 2} Inv2(T, s, cs) && tid != nil;
ensures  {:layer 2} Inv2(T, s, cs) && cs == tid;
{
    var m: int;

    par Yield1() | Yield2();
    call m := GetTicketAbstract(tid);
    call WaitAndEnter(tid, m);
    par Yield1() | Yield2() | YieldSpec(tid);
}

// ###########################################################################
// Abstractions of primitive atomic actions

// Note how GetTicketAbstract becomes a right mover

procedure {:yields} {:layer 1,2} InitAbstract ({:linear "tid"} xls:[X]bool)
ensures  {:layer 1} Inv1(T, t);
ensures {:atomic} |{ A: assert xls == MapConstBool(true); cs := nil; s := 0; T := RightOpen(0); return true; }|;
{
    yield;
    call Init(xls);
    par Yield1();
}

procedure {:yields} {:layer 1,2} GetTicketAbstract ({:linear "tid"} tid: X) returns (m: int)
requires {:layer 1} Inv1(T, t);
ensures  {:layer 1} Inv1(T, t);
ensures {:right} |{ A: assume !T[m]; T[m] := true; return true; }|;
{
    par Yield1();
    call m := GetTicket(tid);
    par Yield1();
}

// ###########################################################################
// Primitive atomic actions

procedure {:yields} {:layer 0,1} Init ({:linear "tid"} xls:[X]bool);
ensures {:atomic} |{ A: assert xls == MapConstBool(true); cs := nil; t := 0; s := 0; T := RightOpen(0); return true; }|;

procedure {:yields} {:layer 0,1} GetTicket ({:linear "tid"} tid: X) returns (m: int);
ensures {:atomic} |{ A: m := t; t := t + 1; T[m] := true; return true; }|;

procedure {:yields} {:layer 0,2} WaitAndEnter ({:linear "tid"} tid: X, m:int);
ensures {:atomic} |{ A: assume m == s; cs := tid; return true; }|;

procedure {:yields} {:layer 0,2} Leave ({:linear "tid"} tid: X);
ensures {:atomic} |{ A: assert cs == tid; s := s + 1; cs := nil; return true; }|;

procedure {:yields} {:layer 0,2} AllocateLow ({:linear_in "tid"} xls':[X]bool) returns ({:linear "tid"} xls: [X]bool, {:linear "tid"} xl: X);
ensures {:atomic} |{ A: assume xl != nil; return true; }|;

// ###########################################################################
// Collectors for linear domains

function {:builtin "MapConst"} MapConstBool (bool) : [X]bool;
function {:inline} {:linear "tid"} TidCollector (x: X) : [X]bool
{
  MapConstBool(false)[x := true]
}
function {:inline} {:linear "tid"} TidSetCollector (x: [X]bool) : [X]bool
{
  x
}
