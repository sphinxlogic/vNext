// RUN: %dafny /verifyAllModules /allocated:1 /print:"%t.print" /env:0 /dprint:- "%s" > "%t"
// RUN: %diff "%s.expect" "%t"
include "../../dafny0/MatchBraces.dfy"
