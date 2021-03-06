Option Strict Off
Option Explicit On

Imports System
Imports System.CodeDom
Imports System.CodeDom.Compiler
Imports System.Collections
Imports CSymphony.Lib

Namespace CSymphony.CSy
	Public Class CSyUnit
		 Inherits GraphUnit
		Overrides Public Sub Build()
			ns("MyNameSpace")

			declClass(TypePublic, "MyClass")
			Dim typeDateTime As TypeRef = New TypeRef("System.DateTime")

			declMethod(MemPublic, "System.Void", "MyMethod")
			
			enter (typeDateTime.Expr())
		End Sub
	End Class
End Namespace
			