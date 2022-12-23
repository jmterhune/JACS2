Imports System.DirectoryServices
Imports System.DirectoryServices.AccountManagement
Imports System.Web.UI.MobileControls.Adapters
Imports DotNetNuke.Entities

Public Module ADController
    Public Function GetUserLogins() As List(Of ADUser)
        Dim userList As New List(Of ADUser)
        Using ctx As PrincipalContext = New PrincipalContext(ContextType.Domain, "jud12.local")
            Using search As New PrincipalSearcher(New UserPrincipal(ctx))
                For Each result In search.FindAll()
                    Dim de As DirectoryEntry = TryCast(result.GetUnderlyingObject(), DirectoryEntry)
                    userList.Add(New ADUser() With {.UserName = de.Properties("samAccountName").Value, .LastLoginDate = de.Properties("LastLogonDate").Value})
                Next
            End Using
        End Using
        Return userList
    End Function
    Public Function GetADUsers() As List(Of ADUser)
        Try
            Dim impersonate As New Impersonator("LTracker", "jud12", "$ecret$quirrel22")
            Dim lstADUsers As List(Of ADUser) = New List(Of ADUser)()
            Dim DomainPath As String = "LDAP://JUDMANDC04.JUD12.LOCAL/DC=jud12,DC=local"
            Dim searchRoot As DirectoryEntry = New DirectoryEntry(DomainPath)
            Dim search As DirectorySearcher = New DirectorySearcher(searchRoot)
            search.Filter = "(&(objectClass=user))"
            search.PropertiesToLoad.Add("samaccountname")
            search.PropertiesToLoad.Add("lastLogon")
            Dim result As SearchResult
            Dim resultCol As SearchResultCollection = search.FindAll

            If resultCol IsNot Nothing Then

                For counter As Integer = 0 To resultCol.Count - 1
                    Dim UserNameEmailString As String = String.Empty
                    result = resultCol(counter)

                    If result.Properties.Contains("samaccountname") Then
                        Dim objSurveyUsers As ADUser = New ADUser()
                        objSurveyUsers.UserName = CType(result.Properties("samaccountname")(0), String)
                        Try
                            Dim logDate = result.Properties("lastLogon")(0)
                            If logDate IsNot Nothing Then
                                Dim loggDate = DateTime.FromFileTime(CLng(logDate))
                                objSurveyUsers.LastLoginDate = DateTime.FromFileTime(CLng(logDate)).ToString()
                            End If

                        Catch

                        End Try

                        If Not String.IsNullOrEmpty(objSurveyUsers.LastLoginDate) Then
                            lstADUsers.Add(objSurveyUsers)
                        End If
                    End If
                Next
            End If
            impersonate.Dispose()
            Return lstADUsers
        Catch ex As Exception
            Return New List(Of ADUser)
        End Try
    End Function
End Module
