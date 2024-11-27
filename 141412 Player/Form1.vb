
Imports System.IO
Imports System.Threading
Imports Microsoft.Web.WebView2.Core
Public Class Form1
    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        KeyPreview = True
        WebView21.Size = Size
        WebView21.Location = New Point(0, 0)
        WebView21.Width = Width
        WebView21.Height = Height
        WebView21.Source = New Uri("file:///" & Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), "html\index.html"))
        Await WebView21.EnsureCoreWebView2Async()
    End Sub

    Private Sub Form1_SizeChanged(sender As Object, e As EventArgs) Handles Me.SizeChanged
        WebView21.Size = Size
        WebView21.Location = New Point(0, 0)
        WebView21.Width = Width
        WebView21.Height = Height
    End Sub

    Private Sub WebView21_KeyDown(sender As Object, e As KeyEventArgs) Handles WebView21.KeyDown
        If e.KeyCode = 79 Then
            ' Can we PLEASE change this.
            Dim FolderBrowserDialog1 As New FolderBrowserDialog
            FolderBrowserDialog1.RootFolder = Environment.SpecialFolder.MyComputer
            FolderBrowserDialog1.ShowNewFolderButton = False
            If FolderBrowserDialog1.ShowDialog = DialogResult.OK Then
                My.Settings.StartupPath = FolderBrowserDialog1.SelectedPath
                Stage1(FolderBrowserDialog1.SelectedPath)
            End If
        End If
    End Sub

    Async Function Stage1(SelectedPath) As Task
        ' process files in selected folder
        Dim InitialFolder As New DirectoryInfo(SelectedPath)

        ' Define audio file extensions
        Dim audioExtensions As String() = {".mp3", ".wav", ".aac", ".flac", ".ogg", ".wma", ".m4a"}

        ' Get audio files in the folder
        Dim audioFiles = InitialFolder.GetFiles().Where(Function(f) audioExtensions.Contains(f.Extension.ToLower())).ToArray()

        If audioFiles.Length > 0 Then
            ' Do something with the audio files
            Dim send As String
            ' weird ahh management
            For Each audioFile In audioFiles
                send += audioFile.FullName & "|"
            Next
            Await idk(send)
        Else
            Console.WriteLine("No audio files found in the selected folder.")
        End If
    End Function
    Private Sub menuChoice(ByVal sender As Object, ByVal e As EventArgs)
        Dim item = CType(sender, ToolStripMenuItem)
        Dim selection = CInt(item.Tag)
        '-- etc
    End Sub
    Private Sub WebView21_MouseUp(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles WebView21.MouseUp
        If e.Button <> Windows.Forms.MouseButtons.Right Then Return
        Dim cms = New ContextMenuStrip
        Dim item1 = cms.Items.Add("foo")
        item1.Tag = 1
        AddHandler item1.Click, AddressOf menuChoice
        Dim item2 = cms.Items.Add("bar")
        item2.Tag = 2
        AddHandler item2.Click, AddressOf menuChoice
        '-- etc
        '..
        cms.Show(WebView21, e.Location)
    End Sub

    Function EscapeForJavaScript(input As String) As String
        Return input.Replace("\", "\\") _
                .Replace("""", "\""") _
                .Replace("'", "\'") _
                .Replace(vbCr, "\r") _
                .Replace(vbLf, "\n") _
                .Replace(vbTab, "\t")
    End Function
    Async Function idk(idkok) As Task
        idkok = EscapeForJavaScript(idkok)
        Await WebView21.ExecuteScriptAsync("try { handle('" + idkok + "'); } catch (e) { console.error('Error:', e.message); }")
    End Function

    Private Async Sub WebView21_NavigationCompleted(sender As Object, e As CoreWebView2NavigationCompletedEventArgs) Handles WebView21.NavigationCompleted
        ' Check if navigation was successful
        If e.IsSuccess Then
            If Not String.IsNullOrEmpty(My.Settings.StartupPath) Then
                Await wtf()
            End If
        Else
            Console.WriteLine("Navigation failed.")
        End If
    End Sub

    Async Function wtf() As Task
        Await Stage1(My.Settings.StartupPath)
    End Function
End Class
