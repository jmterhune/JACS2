Public Class ADUser
    Private _userName As String
    Public Property UserName() As String
        Get
            Return _userName
        End Get
        Set(ByVal value As String)
            _userName = value
        End Set
    End Property
    Private _lastLoginDate As String
    Public Property LastLoginDate() As String
        Get
            Return _lastLoginDate
        End Get
        Set(ByVal value As String)
            _lastLoginDate = value
        End Set
    End Property
End Class
