Public Class ElectronicMailing

    Public Property Id As String = Guid.NewGuid.ToString
    Public Property Template As String
    Public Property Filter As String

    Public Function InsertAsync() As Task
        Return Task.CompletedTask
    End Function

End Class
