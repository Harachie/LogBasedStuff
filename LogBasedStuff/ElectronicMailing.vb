Public Class ElectronicMailing

    Public Enum StateE
        Created = 0
        InsertedMailTasks = 1
        ConsumersFinished = 2
        Finished = 3
    End Enum

    Public Property Id As String = Guid.NewGuid.ToString
    Public Property Template As String
    Public Property Filter As String
    Public Property DegreeOfParallelism As Integer = 2

    Private State As StateE
    Private ConsumersCreated As Boolean
    Private Consumers As New List(Of Consumer)
    Private MailingLog As Log

    Public Async Function InitializeAsync() As Task
        Dim message As String

        Me.MailingLog = New Log("mailing_" & Me.Id)
        Await Me.MailingLog.InitializeAsync

        For i As Long = 1 To Me.MailingLog.LogOffset
            message = Me.MailingLog.GetMessage(i)
            Me.State = [Enum].Parse(GetType(StateE), message)
        Next
    End Function

    Public Function ExecuteAsync() As Task
        Return Me.ContinueAsync
    End Function

    Public Function ContinueAsync() As Task
        Select Case Me.State
            Case StateE.Created
                Return Me.InsertTasksAsync

            Case StateE.InsertedMailTasks
                Return Me.WaitForConsumersAsync

            Case StateE.ConsumersFinished
                Return Me.CleanupAsync

            Case StateE.Finished
                Return Task.CompletedTask

        End Select

        Throw New InvalidOperationException("Cannot continue.")
    End Function

    Public Async Function InsertTasksAsync() As Task
        Await Me.MailingLog.Write(StateE.InsertedMailTasks)
        Await Me.WaitForConsumersAsync
    End Function

    Private Async Function CreateConsumersAsync() As Task
        Dim log As Log
        Dim logName As String

        If Not Me.ConsumersCreated Then
            For i As Integer = 1 To Me.DegreeOfParallelism
                logName = "mailing_" & Me.Id & "_consumer_" & i.ToString
                log = New Log(logName)
                Await log.InitializeAsync
                Me.Consumers.Add(New Consumer(logName, log))
            Next
        End If
    End Function

    Private Async Function ExecuteConsumersAsync() As Task
        Dim tasks As New List(Of Task)

        For Each consumer In Me.Consumers
            tasks.Add(consumer.ExecuteAsync)
        Next

        Await Task.WhenAll(tasks)
    End Function

    Public Async Function WaitForConsumersAsync() As Task
        Await Me.CreateConsumersAsync
        Await Me.ExecuteConsumersAsync
        Await Me.MailingLog.Write(StateE.ConsumersFinished)
        Await Me.CleanupAsync
    End Function

    Public Async Function CleanupAsync() As Task
        'move consumer logs to done folder, NOT the mail log, that's done by the mailing consumer
        Await Me.MailingLog.Write(StateE.Finished)
    End Function

End Class
