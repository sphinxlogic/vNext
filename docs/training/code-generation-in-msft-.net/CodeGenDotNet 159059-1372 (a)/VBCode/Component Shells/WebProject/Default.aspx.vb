Option Strict On
Option Explicit On 

Public Class _Default
   Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

   'This call is required by the Web Form Designer.
   <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

   End Sub
   Protected WithEvents Image1 As System.Web.UI.WebControls.Image
   Protected WithEvents Label1 As System.Web.UI.WebControls.Label
   Protected WithEvents HyperLink1 As System.Web.UI.WebControls.HyperLink
   Protected WithEvents HyperLink2 As System.Web.UI.WebControls.HyperLink
   Protected WithEvents lblName As System.Web.UI.WebControls.Label

   'NOTE: The following placeholder declaration is required by the Web Form Designer.
   'Do not delete or move it.
   Private designerPlaceholderDeclaration As System.Object

   Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
      'CODEGEN: This method call is required by the Web Form Designer
      'Do not modify it using the code editor.
      InitializeComponent()
   End Sub

#End Region

   Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
      Me.lblName.Text = User.Identity.Name
   End Sub

End Class
