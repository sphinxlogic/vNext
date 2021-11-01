#light

// ----------------------------
// Listing 19-2.

namespace ExpertFSharp.Types

open System
open System.Security.Permissions

[<assembly: SecurityPermission(SecurityAction.RequestMinimum, Execution = true);
  assembly: System.Runtime.InteropServices.ComVisible(false);
  assembly: System.CLSCompliant(true);
  assembly: PermissionSet(SecurityAction.RequestOptional, Name = "Nothing")>]
type RadialPoint(angle,radius) = 
    member x.Angle = angle
    member x.Radius = radius
    member x.Stretch(factor) = RadialPoint(angle=x.Angle, radius=x.Radius * factor)
    member x.Warp(transform:Converter<_,_>) = 
        RadialPoint(angle=transform.Invoke(x.Angle), radius=x.Radius)
    static member Circle(count) = 
        seq { for i in 1..count ->
                  RadialPoint(angle=2.0*Math.PI/float(count), radius=1.0) }
    new() = RadialPoint(angle=0.0, radius=0.0)
