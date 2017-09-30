Public Class Command

    Public Enum CommandTypeE
        Unknown = 0
        SendElectronicMailing = 1
    End Enum

    Public Property CommandType As CommandTypeE
    Public Property State As Object

    Public Function Serialize() As String
        Return Newtonsoft.Json.JsonConvert.SerializeObject(Me)
    End Function

End Class
