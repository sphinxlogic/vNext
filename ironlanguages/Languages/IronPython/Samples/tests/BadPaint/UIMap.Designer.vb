'------------------------------------------------------------------------------
'  <auto-generated>
'      This code was generated by coded UI test builder.
'      Version: 10.0.0.0
'
'      Changes to this file may cause incorrect behavior and will be lost if
'      the code is regenerated.
'  </auto-generated>
' ------------------------------------------------------------------------------

Imports System
Imports System.CodeDom.Compiler
Imports System.Collections.Generic
Imports System.Drawing
Imports System.Text.RegularExpressions
Imports System.Windows.Input
Imports Microsoft.VisualStudio.TestTools.UITest.Extension
Imports Microsoft.VisualStudio.TestTools.UITesting
Imports Microsoft.VisualStudio.TestTools.UITesting.WinControls
Imports Microsoft.VisualStudio.TestTools.UITesting.WpfControls
Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard
Imports Mouse = Microsoft.VisualStudio.TestTools.UITesting.Mouse
Imports MouseButtons = System.Windows.Forms.MouseButtons

Namespace BadPaint
    
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Partial Public Class UIMap
        
        '''<summary>
        '''InsertPythonCode - Use 'InsertPythonCodeParams' to pass parameters into this method.
        '''</summary>
        Public Sub InsertPythonCode()
            Dim uI_currentLanguageComboBox As WpfComboBox = Me.UIBadPaintWindow.UI_currentLanguageComboBox
            Dim uI_codeTextBoxEdit As WpfEdit = Me.UIBadPaintWindow.UI_codeTextBoxEdit

            'Select 'IronPython 2.7' in '_currentLanguage' combo box
            uI_currentLanguageComboBox.SelectedItem = Me.InsertPythonCodeParams.UI_currentLanguageComboBoxSelectedItem

            'Type the following paragraph in '_codeTextBox' text box 
            ''import clr
            'clr.AddReference("PresentationFramework")
            'import System
            'System.Windows.MessageBox.Show(str(dir()))'
            uI_codeTextBoxEdit.Text = Me.InsertPythonCodeParams.UI_codeTextBoxEditText

            'Click '_codeTextBox' text box
            Mouse.Click(uI_codeTextBoxEdit, New Point(568, 110))
        End Sub
        
        '''<summary>
        '''RunPythonCode
        '''</summary>
        Public Sub RunPythonCode()
            Dim uIBadPaintTitleBar As WpfTitleBar = Me.UIBadPaintWindow.UIBadPaintTitleBar
            Dim uIRunCodeButton As WpfButton = Me.UIBadPaintWindow.UIRunCodeButton

            'Click 'BadPaint' title bar
            Mouse.Click(uIBadPaintTitleBar, New Point(464, 10))

            'Click 'Run Code' button
            Mouse.Click(uIRunCodeButton, New Point(77, 14))
        End Sub
        
        '''<summary>
        '''AssertPythonExecute - Use 'AssertPythonExecuteExpectedValues' to pass parameters into this method.
        '''</summary>
        Public Sub AssertPythonExecute()
            Dim uIApplicationSystem__bText As WinText = Me.UIApplicationSystem__bWindow.UIApplicationSystem__bText

            'Verify that '['Application', 'System', '__builtins__', '__doc__...' label's property 'DisplayText' equals '['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr']'
            Assert.AreEqual(Me.AssertPythonExecuteExpectedValues.UIApplicationSystem__bTextDisplayText, uIApplicationSystem__bText.DisplayText)
        End Sub
        
        '''<summary>
        '''CloseBadPaint
        '''</summary>
        Public Sub CloseBadPaint()
            Dim uIOKButton As WinButton = Me.UIOKWindow.UIOKButton
            Dim uICloseButton As WpfButton = Me.UIBadPaintWindow.UIBadPaintTitleBar.UICloseButton

            'Click 'OK' button
            Mouse.Click(uIOKButton, New Point(63, 17))

            'Click 'Close' button
            Mouse.Click(uICloseButton, New Point(13, 10))
        End Sub
        
        #Region "Properties"
        Public Overridable ReadOnly Property InsertPythonCodeParams() As InsertPythonCodeParams
            Get
                If (Me.mInsertPythonCodeParams Is Nothing) Then
                    Me.mInsertPythonCodeParams = New InsertPythonCodeParams()
                End If
                Return Me.mInsertPythonCodeParams
            End Get
        End Property
        
        Public Overridable ReadOnly Property AssertPythonExecuteExpectedValues() As AssertPythonExecuteExpectedValues
            Get
                If (Me.mAssertPythonExecuteExpectedValues Is Nothing) Then
                    Me.mAssertPythonExecuteExpectedValues = New AssertPythonExecuteExpectedValues()
                End If
                Return Me.mAssertPythonExecuteExpectedValues
            End Get
        End Property
        
        Public ReadOnly Property UIBadPaintWindow() As UIBadPaintWindow
            Get
                If (Me.mUIBadPaintWindow Is Nothing) Then
                    Me.mUIBadPaintWindow = New UIBadPaintWindow()
                End If
                Return Me.mUIBadPaintWindow
            End Get
        End Property
        
        Public ReadOnly Property UIApplicationSystem__bWindow() As UIApplicationSystem__bWindow
            Get
                If (Me.mUIApplicationSystem__bWindow Is Nothing) Then
                    Me.mUIApplicationSystem__bWindow = New UIApplicationSystem__bWindow()
                End If
                Return Me.mUIApplicationSystem__bWindow
            End Get
        End Property
        
        Public ReadOnly Property UIOKWindow() As UIOKWindow
            Get
                If (Me.mUIOKWindow Is Nothing) Then
                    Me.mUIOKWindow = New UIOKWindow()
                End If
                Return Me.mUIOKWindow
            End Get
        End Property
        #End Region
        
        #Region "Fields"
        Private mInsertPythonCodeParams As InsertPythonCodeParams
        
        Private mAssertPythonExecuteExpectedValues As AssertPythonExecuteExpectedValues
        
        Private mUIBadPaintWindow As UIBadPaintWindow
        
        Private mUIApplicationSystem__bWindow As UIApplicationSystem__bWindow
        
        Private mUIOKWindow As UIOKWindow
        #End Region
    End Class
    
    '''<summary>
    '''Parameters to be passed into 'InsertPythonCode'
    '''</summary>
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Public Class InsertPythonCodeParams
        
        #Region "Fields"
        '''<summary>
        '''Select 'IronPython 2.7' in '_currentLanguage' combo box
        '''</summary>
        Public UI_currentLanguageComboBoxSelectedItem As String = "IronPython 2.7"
        
        '''<summary>
        '''Type the following paragraph in '_codeTextBox' text box 
        ''''import clr
        '''clr.AddReference("PresentationFramework")
        '''import System
        '''System.Windows.MessageBox.Show(str(dir()))'
        '''</summary>
        Public UI_codeTextBoxEditText As String = "import clr"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"clr.AddReference(""PresentationFramework"")"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"import System"&Global.Microsoft.VisualBasic.ChrW(13)&Global.Microsoft.VisualBasic.ChrW(10)&"System.Wind"& _ 
            "ows.MessageBox.Show(str(dir()))"
        #End Region
    End Class
    
    '''<summary>
    '''Parameters to be passed into 'AssertPythonExecute'
    '''</summary>
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Public Class AssertPythonExecuteExpectedValues
        
        #Region "Fields"
        '''<summary>
        '''Verify that '['Application', 'System', '__builtins__', '__doc__...' label's property 'DisplayText' equals '['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr']'
        '''</summary>
        Public UIApplicationSystem__bTextDisplayText As String = "['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr"& _ 
            "']"
        #End Region
    End Class
    
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Public Class UIBadPaintWindow
        Inherits WpfWindow
        
        Public Sub New()
            MyBase.New
            Me.SearchProperties(WpfWindow.PropertyNames.Name) = "BadPaint"
            Me.SearchProperties.Add(New PropertyExpression(WpfWindow.PropertyNames.ClassName, "HwndWrapper", PropertyExpressionOperator.Contains))
            Me.WindowTitles.Add("BadPaint")
        End Sub
        
        #Region "Properties"
        Public ReadOnly Property UI_currentLanguageComboBox() As WpfComboBox
            Get
                If (Me.mUI_currentLanguageComboBox Is Nothing) Then
                    Me.mUI_currentLanguageComboBox = New WpfComboBox(Me)
                    Me.mUI_currentLanguageComboBox.SearchProperties(WpfComboBox.PropertyNames.AutomationId) = "_currentLanguage"
                    Me.mUI_currentLanguageComboBox.WindowTitles.Add("BadPaint")
                End If
                Return Me.mUI_currentLanguageComboBox
            End Get
        End Property
        
        Public ReadOnly Property UI_codeTextBoxEdit() As WpfEdit
            Get
                If (Me.mUI_codeTextBoxEdit Is Nothing) Then
                    Me.mUI_codeTextBoxEdit = New WpfEdit(Me)
                    Me.mUI_codeTextBoxEdit.SearchProperties(WpfEdit.PropertyNames.AutomationId) = "_codeTextBox"
                    Me.mUI_codeTextBoxEdit.WindowTitles.Add("BadPaint")
                End If
                Return Me.mUI_codeTextBoxEdit
            End Get
        End Property
        
        Public ReadOnly Property UIBadPaintTitleBar() As UIBadPaintTitleBar
            Get
                If (Me.mUIBadPaintTitleBar Is Nothing) Then
                    Me.mUIBadPaintTitleBar = New UIBadPaintTitleBar(Me)
                End If
                Return Me.mUIBadPaintTitleBar
            End Get
        End Property
        
        Public ReadOnly Property UIRunCodeButton() As WpfButton
            Get
                If (Me.mUIRunCodeButton Is Nothing) Then
                    Me.mUIRunCodeButton = New WpfButton(Me)
                    Me.mUIRunCodeButton.SearchProperties(WpfButton.PropertyNames.AutomationId) = "UpdateButton"
                    Me.mUIRunCodeButton.WindowTitles.Add("BadPaint")
                End If
                Return Me.mUIRunCodeButton
            End Get
        End Property
        #End Region
        
        #Region "Fields"
        Private mUI_currentLanguageComboBox As WpfComboBox
        
        Private mUI_codeTextBoxEdit As WpfEdit
        
        Private mUIBadPaintTitleBar As UIBadPaintTitleBar
        
        Private mUIRunCodeButton As WpfButton
        #End Region
    End Class
    
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Public Class UIBadPaintTitleBar
        Inherits WpfTitleBar
        
        Public Sub New(ByVal searchLimitContainer As UITestControl)
            MyBase.New(searchLimitContainer)
            Me.SearchProperties(WpfTitleBar.PropertyNames.AutomationId) = "TitleBar"
            Me.WindowTitles.Add("BadPaint")
        End Sub
        
        #Region "Properties"
        Public ReadOnly Property UICloseButton() As WpfButton
            Get
                If (Me.mUICloseButton Is Nothing) Then
                    Me.mUICloseButton = New WpfButton(Me)
                    Me.mUICloseButton.SearchProperties(WpfButton.PropertyNames.AutomationId) = "Close"
                    Me.mUICloseButton.WindowTitles.Add("BadPaint")
                End If
                Return Me.mUICloseButton
            End Get
        End Property
        #End Region
        
        #Region "Fields"
        Private mUICloseButton As WpfButton
        #End Region
    End Class
    
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Public Class UIApplicationSystem__bWindow
        Inherits WinWindow
        
        Public Sub New()
            MyBase.New
            Me.SearchProperties(WinWindow.PropertyNames.Name) = "['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr"& _ 
                "']"
            Me.SearchProperties(WinWindow.PropertyNames.ClassName) = "Static"
            Me.WindowTitles.Add("['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr"& _ 
                    "']")
        End Sub
        
        #Region "Properties"
        Public ReadOnly Property UIApplicationSystem__bText() As WinText
            Get
                If (Me.mUIApplicationSystem__bText Is Nothing) Then
                    Me.mUIApplicationSystem__bText = New WinText(Me)
                    Me.mUIApplicationSystem__bText.SearchProperties(WinText.PropertyNames.Name) = "['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr"& _ 
                        "']"
                    Me.mUIApplicationSystem__bText.WindowTitles.Add("['Application', 'System', '__builtins__', '__doc__', '__file__', '__name__', 'clr"& _ 
                            "']")
                End If
                Return Me.mUIApplicationSystem__bText
            End Get
        End Property
        #End Region
        
        #Region "Fields"
        Private mUIApplicationSystem__bText As WinText
        #End Region
    End Class
    
    <GeneratedCode("Coded UITest Builder", "10.0.30319.1")>  _
    Public Class UIOKWindow
        Inherits WinWindow
        
        Public Sub New()
            MyBase.New
            Me.SearchProperties(WinWindow.PropertyNames.Name) = "OK"
            Me.SearchProperties(WinWindow.PropertyNames.ClassName) = "Button"
            Me.WindowTitles.Add("OK")
        End Sub
        
        #Region "Properties"
        Public ReadOnly Property UIOKButton() As WinButton
            Get
                If (Me.mUIOKButton Is Nothing) Then
                    Me.mUIOKButton = New WinButton(Me)
                    Me.mUIOKButton.SearchProperties(WinButton.PropertyNames.Name) = "OK"
                    Me.mUIOKButton.WindowTitles.Add("OK")
                End If
                Return Me.mUIOKButton
            End Get
        End Property
        #End Region
        
        #Region "Fields"
        Private mUIOKButton As WinButton
        #End Region
    End Class
End Namespace
