Public Class Consumer

    Public Id As String

    Private SourceLog As Log
    Private Offset As Long

    Public Sub New(id As String, sourceLog As Log)
        Me.Id = id
        Me.SourceLog = sourceLog
    End Sub

    Public Async Function ExecuteAsync() As Task
        Dim command As Command
        Dim message As String

        Me.Offset = Await Log.GetConsumerOffset(Me.Id)

        For i As Long = Me.Offset + 1L To Me.SourceLog.LogOffset
            message = Me.SourceLog.GetMessage(i)
            command = Newtonsoft.Json.JsonConvert.DeserializeObject(Of Command)(message)
            Await Me.ExecuteCommand(command)
            Await Log.SetConsumerOffset(Me.Id, i)
        Next
    End Function

    Private Function ExecuteCommand(command As Command) As Task
        Select Case command.CommandType
            Case Command.CommandTypeE.SendElectronicMailing
                Return ExecuteElectronicMailingAsync(command)

        End Select

        Return Task.CompletedTask
    End Function

    Private Async Function ExecuteElectronicMailingAsync(command As Command) As Task
        Dim emailing As ElectronicMailing

        emailing = Newtonsoft.Json.JsonConvert.DeserializeObject(Of ElectronicMailing)(command.State.ToString)

        Await emailing.InitializeAsync
        Await emailing.ExecuteAsync
    End Function

End Class
