Public Class InformationWindow
    Public form1 As MainWindow
    Private Sub Window_Closed(sender As Object, e As EventArgs)
        form1.IsEnabled = True

    End Sub

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        netv.Content += Mid(Environment.Version.ToString, 1, 5) + " / VB 16.9"
        mididevece.Content += NAudio.Midi.MidiOut.DeviceInfo(0).ProductName
    End Sub

    Private Sub visitwebsite_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("https://www.agang.org")
    End Sub

    Private Sub jionQQ_Click(sender As Object, e As RoutedEventArgs)
        Process.Start("https://jq.qq.com/?_wv=1027&k=dbbOLJQ2")
    End Sub
End Class
