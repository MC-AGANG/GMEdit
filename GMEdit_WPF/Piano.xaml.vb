Imports NAudio.Midi
Public Class Piano
    Public form1 As MainWindow
    Dim playing(127) As Integer
    Private Sub DockPanel_MouseEnter(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(form1.channalcolors(form1.currentchannal))
            Dim tone As Integer = Val(Mid(target.Name, 2))
            If Not playing(tone) Then
                playing(tone) = True
                Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOn, tone, 100)
                form1.midioutdevice.Send(midie.GetAsShortMessage)
            End If
        End If
    End Sub

    Private Sub DockPanel_MouseLeave(sender As Object, e As MouseEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 241, 241, 241))
        Dim tone As Integer = Val(Mid(target.Name, 2))
        If playing(tone) Then
            playing(tone) = False
            Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOff, tone, 100)
            form1.midioutdevice.Send(midie.GetAsShortMessage)
        End If
    End Sub
    Private Sub DockPanel_MouseEnter_1(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(form1.channalcolors(form1.currentchannal))
            Dim tone As Integer = Val(Mid(target.Name, 2))
            If Not playing(tone) Then
                playing(tone) = True
                Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOn, tone, 100)
                form1.midioutdevice.Send(midie.GetAsShortMessage)
            End If
        End If
    End Sub

    Private Sub DockPanel_MouseLeave_1(sender As Object, e As MouseEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 10, 10, 10))
        Dim tone As Integer = Val(Mid(target.Name, 2))
        If playing(tone) Then
            playing(tone) = False
            Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOff, tone, 100)
            form1.midioutdevice.Send(midie.GetAsShortMessage)
        End If
    End Sub
    Private Sub DockPanel_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 241, 241, 241))
        Dim tone As Integer = Val(Mid(target.Name, 2))
        If playing(tone) Then
            playing(tone) = False
            Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOff, tone, 100)
            form1.midioutdevice.Send(midie.GetAsShortMessage)
        End If
    End Sub
    Private Sub DockPanel_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(form1.channalcolors(form1.currentchannal))
            Dim tone As Integer = Val(Mid(target.Name, 2))
            If Not playing(tone) Then
                playing(tone) = True
                Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOn, tone, 100)
                form1.midioutdevice.Send(midie.GetAsShortMessage)
            End If
        End If
    End Sub
    Private Sub DockPanel_MouseUp_1(sender As Object, e As MouseButtonEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 10, 10, 10))
        Dim tone As Integer = Val(Mid(target.Name, 2))
        If playing(tone) Then
            playing(tone) = False
            Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOff, tone, 100)
            form1.midioutdevice.Send(midie.GetAsShortMessage)
        End If
    End Sub
    Private Sub DockPanel_MouseDown_1(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(form1.channalcolors(form1.currentchannal))
            Dim tone As Integer = Val(Mid(target.Name, 2))
            If Not playing(tone) Then
                playing(tone) = True
                Dim midie As New NoteEvent(0, form1.currentchannal, MidiCommandCode.NoteOn, tone, 100)
                form1.midioutdevice.Send(midie.GetAsShortMessage)
            End If
        End If
    End Sub

    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 0 To 127
            playing(i) = False
        Next
    End Sub
End Class
