Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Management.Automation
Imports System.Management.Automation.Runspaces
Imports System.Threading.Tasks
Imports DotNetNuke.Services.Exceptions

Public Module PowerShellInvoker

    Public Function ExecutePowershellScript(ByVal script As String) As String
        Dim returnString As String = ""

        Try
            Dim runspace As Runspace = RunspaceFactory.CreateRunspace()
            runspace.Open()
            Dim pipeline As Pipeline = runspace.CreatePipeline()
            pipeline.Commands.AddScript(script)
            Dim results As Collection(Of PSObject) = pipeline.Invoke()
            runspace.Close()

            If results.Count > 0 Then
                For Each psObject In results
                    returnString += psObject.BaseObject.ToString()
                Next
            End If
        Catch ex As Exception
            Exceptions.LogException(ex)

        End Try
        Return returnString
    End Function
    Public Sub ExecutePowerShellFile(ByVal fileName As String)
        Try
            'Dim impersonate As New Impersonator("LTracker", "Jud12.Local", "$ecret$quirrel22")
            Dim startInfo = New ProcessStartInfo("powershell.exe", "-NoProfile -ExecutionPolicy ByPass -File " & fileName) With {
               .UseShellExecute = False, .Domain = "Jud12", .UserName = "LTracker", .PasswordInClearText = "$ecret$quirrel22"}
            Process.Start(startInfo)
            ' impersonate.Dispose()
        Catch ex As Exception
            Exceptions.LogException(ex)

        End Try
    End Sub

End Module
