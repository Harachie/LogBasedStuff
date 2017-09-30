Public Class Log

    Public Shared LogDirectory As String = "C:\Users\Harachie\Documents\Visual Studio 2017\Projects\LogBasedStuff\LogBasedStuff\Logs"

    Public LogTopic As String
    Public LogOffset As Long = 0
    Private LogEntries As New Dictionary(Of Long, String)
    Private FileStream As IO.FileStream
    Private LogWriter As IO.BinaryWriter

    Public ReadOnly Property Topic As String
        Get
            Return Me.LogTopic
        End Get
    End Property

    Public Sub New(topic As String)
        Me.LogTopic = topic
    End Sub

    Public Async Function InitializeAsync() As Task
        Dim logFilePath As String

        logFilePath = IO.Path.Combine(LogDirectory, Me.LogTopic)
        Await Me.InitializeLogFileAsync(logFilePath)
    End Function

    Private Function InitializeLogFileAsync(logFilePath As String) As Task
        Dim offset As Long
        Dim value As String

        Using s As New IO.FileStream(logFilePath, IO.FileMode.OpenOrCreate)
            Using sr As New IO.BinaryReader(s)
                While (sr.BaseStream.Position <> sr.BaseStream.Length)
                    value = sr.ReadString
                    offset += 1
                    Me.LogEntries(offset) = value
                End While
            End Using
        End Using

        Me.LogOffset = offset
        Me.FileStream = New IO.FileStream(logFilePath, IO.FileMode.Append)
        Me.LogWriter = New IO.BinaryWriter(Me.FileStream)

        Return Task.CompletedTask
    End Function

    Public Function Write(message As String) As Task(Of Long)
        SyncLock Me.LogWriter
            Me.LogWriter.Write(message)
            Me.LogWriter.Flush()
            Me.LogOffset += 1
        End SyncLock

        Return Task.FromResult(Of Long)(Me.LogOffset)
    End Function

    Public Function GetMessage(offset As Long) As String
        Dim r As String = Nothing

        Me.LogEntries.TryGetValue(offset, r)

        Return r
    End Function

    Public Shared Function GetConsumerOffset(id As String) As Task(Of Long)
        Dim filePath As String
        Dim text As String

        filePath = IO.Path.Combine(LogDirectory, id & "_offset")

        If IO.File.Exists(filePath) Then
            text = IO.File.ReadAllText(filePath)
            Return Task.FromResult(Long.Parse(text))
        End If

        Return Task.FromResult(0L)
    End Function

    Public Shared Function SetConsumerOffset(id As String, offset As Long) As Task
        Dim filePath As String

        filePath = IO.Path.Combine(LogDirectory, id & "_offset")
        IO.File.WriteAllText(filePath, offset.ToString)

        Return Task.CompletedTask
    End Function

End Class
