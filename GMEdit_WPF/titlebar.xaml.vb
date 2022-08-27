Public Class titlebar
    Public Form1 As MainWindow
    Dim timer1 As New Timers.Timer(200)
    Sub Close_MouseEnter()
        Close.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
    End Sub
    Sub Close_MouseLeave()
        Close.Background = New SolidColorBrush(Color.FromArgb(255, 45, 45, 48))
    End Sub
    Sub Close_MouseDown()
        Close.Background = New SolidColorBrush(Color.FromArgb(255, 0, 122, 204))
    End Sub

    Private Sub DockPanel_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim se As DockPanel = sender
        se.Background = New SolidColorBrush(Color.FromArgb(255, 0, 122, 204))
    End Sub

    Private Sub DockPanel_MouseEnter(sender As Object, e As MouseEventArgs)
        Dim se As DockPanel = sender
        se.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
    End Sub

    Private Sub DockPanel_MouseLeave(sender As Object, e As MouseEventArgs)
        Dim se As DockPanel = sender
        se.Background = New SolidColorBrush(Color.FromArgb(255, 45, 45, 48))
    End Sub

    Private Sub Close_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(Close).X >= 0 AndAlso e.GetPosition(Close).X < 48 AndAlso e.GetPosition(Close).Y >= 0 AndAlso e.GetPosition(Close).Y < 48 Then
            If Mid(Form1.Filename.Content, Len(Form1.Filename.Content), 1) = "*" Then
                Dim n As Integer = MsgBox("是否保存""" + Mid(Form1.Filename.Content, 1, Len(Form1.Filename.Content) - 1) + """？", 3)
                If n = MsgBoxResult.Yes Then
                    Form1.savefile()
                    Form1.Timer1.Stop()
                    Form1.timer2.Stop()
                    Form1.midioutdevice.Reset()
                    Form1.midioutdevice.Close()
                    Form1.Close()
                ElseIf n = MsgBoxResult.No Then
                    Form1.Timer1.Stop()
                    Form1.timer2.Stop()
                    Form1.midioutdevice.Reset()
                    Form1.midioutdevice.Close()
                    Form1.Close()
                End If
            Else
                Form1.Timer1.Stop()
                Form1.timer2.Stop()
                Form1.midioutdevice.Reset()
                Form1.midioutdevice.Close()
                Form1.Close()
            End If
        Else
            Close.Background = New SolidColorBrush(Color.FromArgb(255, 45, 45, 48))
        End If
    End Sub

    Private Sub Minbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(Minbutton).X >= 0 AndAlso e.GetPosition(Minbutton).X < 48 AndAlso e.GetPosition(Minbutton).Y >= 0 AndAlso e.GetPosition(Minbutton).Y < 48 Then
            Form1.WindowState = WindowState.Minimized
        Else
            Minbutton.Background = New SolidColorBrush(Color.FromArgb(255, 45, 45, 48))
        End If
    End Sub

    Private Sub maxbutton_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(maxbutton).X >= 0 AndAlso e.GetPosition(maxbutton).X < 48 AndAlso e.GetPosition(maxbutton).Y >= 0 AndAlso e.GetPosition(maxbutton).Y < 48 Then
            If Form1.maxsized Then
                Form1.NormalSize()
                normalimage.Visibility = Visibility.Collapsed
                maximage.Visibility = Visibility.Visible
            Else
                Form1.MaxSize()
                maximage.Visibility = Visibility.Collapsed
                normalimage.Visibility = Visibility.Visible
            End If
        Else
            maxbutton.Background = New SolidColorBrush(Color.FromArgb(255, 45, 45, 48))
        End If
    End Sub

    Private Sub titlebody_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            If timer1.Enabled Then
                If Form1.maxsized Then
                    Form1.NormalSize()
                    normalimage.Visibility = Visibility.Collapsed
                    maximage.Visibility = Visibility.Visible
                Else
                    Form1.MaxSize()
                    maximage.Visibility = Visibility.Collapsed
                    normalimage.Visibility = Visibility.Visible
                End If
            Else
                timer1.Enabled = True
            End If
            If Form1.maxsized Then
                Mouse.Capture(sender)
            Else
                Form1.DragMove()
            End If
        End If
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        AddHandler timer1.Elapsed, AddressOf Timer1_tick
    End Sub
    Private Sub Timer1_tick()
        timer1.Enabled = False
    End Sub

    Private Sub titlebody_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
    End Sub
End Class
