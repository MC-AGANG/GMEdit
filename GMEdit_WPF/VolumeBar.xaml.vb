Public Class VolumeBar
    Dim y1, y2, h1 As Integer
    Public Form1 As MainWindow
    Public id As Integer
    Private Sub Rectangle_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            h1 = Margin.Top
            y1 = e.GetPosition(Form1).Y
        End If
    End Sub

    Private Sub Rectangle_MouseMove(sender As Object, e As MouseEventArgs)
        Form1.tip.Content = "音量：" + CStr(100 - Margin.Top) + "%"
        If e.LeftButton = MouseButtonState.Pressed Then
            y2 = e.GetPosition(Form1).Y
            If h1 + y2 - y1 < 0 Then
                Margin = New Thickness(Margin.Left, 0, 0, 0)
            ElseIf h1 + y2 - y1 >= 100 Then
                Margin = New Thickness(Margin.Left, 99, 0, 0)
            Else
                Margin = New Thickness(Margin.Left, h1 + y2 - y1, 0, 0)
            End If
        End If
    End Sub
    Private Sub Rectangle_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        Form1.lastlength = Width \ 8
        Form1.lastvolume = 100 - Form1.volumeb(Form1.currentchannal, id).Margin.Top
        Form1.edit()
    End Sub
End Class
