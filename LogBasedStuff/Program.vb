Module Program

    Sub Main()
        Dim emailingsLog As New Log("emailings")
        Dim emailingsConsumer As New Consumer("emailings", emailingsLog)
        '   Dim consumerLog As New Log("consumers")
        '  Dim mailing As New ElectronicMailing With {.Template = "VutexInfoMail", .Filter = "InterestedInVutex"}
        '    Dim command As New Command With {.CommandType = Command.CommandTypeE.SendElectronicMailing, .State = mailing}

        emailingsLog.InitializeAsync().Wait()
        emailingsConsumer.ExecuteAsync.Wait()

        '   emailingsLog.Write(command.Serialize)
    End Sub

End Module
