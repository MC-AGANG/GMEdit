Public Class LoadPage
    Dim timer1 As Forms.Timer
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        timer1 = New Forms.Timer
        timer1.Interval = 500
        timer1.Enabled = True
        AddHandler timer1.Tick, AddressOf timer1_tick
    End Sub
    Private Sub timer1_tick()
        Dim w As New MainWindow
        w.Show()
        timer1.Enabled = False
        Close()
    End Sub
End Class
