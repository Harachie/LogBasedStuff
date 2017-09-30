Module Program

    Sub Main()
        Dim emailLog As New Log("email")

        emailLog.InitializeAsync().Wait()
        emailLog.Write("1").Wait()
        emailLog.Write("ä").Wait()
        emailLog.Write("ö").Wait()
        emailLog.Write("ü").Wait()
    End Sub

End Module
