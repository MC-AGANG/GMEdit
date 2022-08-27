Imports NAudio.Midi
Public Class DrumKeys
    Public playing(127) As Boolean
    Public Form1 As MainWindow
    Private Sub key_mousedown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Form1.channalcolors(Form1.currentchannal))
            Dim tone As Integer = Val(Mid(target.Name, 2))
            If Not playing(tone) Then
                playing(tone) = True
                Dim midie As New NoteEvent(0, 10, MidiCommandCode.NoteOn, tone, 100)
                Form1.midioutdevice.Send(midie.GetAsShortMessage)
            End If
        End If
    End Sub

    Private Sub key_mouseup(sender As Object, e As MouseButtonEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 221, 221, 221))
        Dim tone As Integer = Val(Mid(target.Name, 2))
        If playing(tone) Then
            playing(tone) = False
            Dim midie As New NoteEvent(0, 10, MidiCommandCode.NoteOff, tone, 100)
            Form1.midioutdevice.Send(midie.GetAsShortMessage)
        End If
    End Sub

    Private Sub key_mouseenter(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Form1.channalcolors(Form1.currentchannal))
            Dim tone As Integer = Val(Mid(target.Name, 2))
            If Not playing(tone) Then
                playing(tone) = True
                Dim midie As New NoteEvent(0, 10, MidiCommandCode.NoteOn, tone, 100)
                Form1.midioutdevice.Send(midie.GetAsShortMessage)
            End If
        End If
    End Sub

    Private Sub key_mouseleave(sender As Object, e As MouseEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 221, 221, 221))
        Dim tone As Integer = Val(Mid(target.Name, 2))
        If playing(tone) Then
            playing(tone) = False
            Dim midie As New NoteEvent(0, 10, MidiCommandCode.NoteOff, tone, 100)
            Form1.midioutdevice.Send(midie.GetAsShortMessage)
        End If
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 0 To 127
            playing(i) = False
        Next
    End Sub
End Class
