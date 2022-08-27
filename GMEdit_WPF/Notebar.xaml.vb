Imports NAudio.Midi
Public Class Notebar
    Dim x1, x2, y1, y2, w1 As Integer
    Public Form1 As MainWindow
    Public id As Integer
    Dim lastnote As Integer
    Dim midie As NoteEvent
    Public channal As Integer
    Public choosed As Boolean = False
    Dim canmove As Boolean
    Dim cansize As Boolean
    Private Sub spart_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(spart)
            If Not choosed Then
                w1 = Width
                x1 = e.GetPosition(Me).X
                Form1.dischoosenote()
            Else
                x1 = e.GetPosition(Me).X
                For i = 0 To Form1.notecount(channal) - 1
                    If Form1.note(channal, i).choosed Then
                        Form1.note(channal, i).w1 = Form1.note(channal, i).Width
                    End If
                Next
            End If
        End If
    End Sub

    Private Sub spart_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        Form1.lastlength = Width \ 8
        Form1.lastvolume = 100 - Form1.volumeb(Form1.currentchannal, id).Margin.Top
        Form1.init_play(Form1.currentchannal)
        cansize = True
        Form1.edit()
    End Sub

    Private Sub mb_IsEnabledChanged(sender As Object, e As DependencyPropertyChangedEventArgs)
        If IsEnabled Then
            Opacity = 1
            Form1.volumeb(channal, id).IsEnabled = True
            Form1.volumeb(channal, id).Opacity = 1
        Else
            Opacity = 0.1
            Form1.volumeb(channal, id).IsEnabled = False
            Form1.volumeb(channal, id).Opacity = 0.1
        End If
    End Sub

    Private Sub mb_Loaded(sender As Object, e As RoutedEventArgs)
        canmove = True
        cansize = True
    End Sub

    Private Sub ppart_MouseEnter(sender As Object, e As MouseEventArgs)
        Form1.tip.Content = CStr(Margin.Left \ 8) + "," + numbertoname(Val(lb.Content))
    End Sub

    Private Sub spart_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            If choosed Then
                x2 = e.GetPosition(Me).X
                For i = 0 To Form1.notecount(channal) - 1
                    If Form1.note(channal, i).choosed Then
                        If (Form1.note(channal, i).w1 + ((x2 - x1) \ 8) * 8) < 8 Then
                            cansize = False
                        End If
                    End If
                Next
                If cansize Then
                    For i = 0 To Form1.notecount(channal) - 1
                        If Form1.note(channal, i).choosed Then
                            Form1.note(channal, i).Width = Form1.note(channal, i).w1 + ((x2 - x1) \ 8) * 8
                            Form1.volumeb(channal, i).Width = Form1.note(channal, i).Width
                        End If
                    Next
                End If
            Else
                x2 = e.GetPosition(Me).X
                If w1 + ((x2 - x1) \ 8) * 8 >= 8 Then
                    Width = w1 + ((x2 - x1) \ 8) * 8
                    Form1.volumeb(Form1.currentchannal, id).Width = Width
                End If
            End If
        End If
    End Sub

    Private Sub ppart_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If e.LeftButton = MouseButtonState.Pressed Then
            If Not choosed Then
                x1 = e.GetPosition(ppart).X
                y1 = e.GetPosition(ppart).Y
                lastnote = Val(lb.Content)
                midie = New NoteEvent(0, Form1.currentchannal, MidiCommandCode.NoteOn, lastnote, Int(1.28 * (100 - Form1.volumeb(Form1.currentchannal, id).Margin.Top)) - 1)
                Form1.midioutdevice.Send(midie.GetAsShortMessage)
                Form1.dischoosenote()
            Else
                x1 = e.GetPosition(ppart).X
                y1 = e.GetPosition(ppart).Y
            End If
        ElseIf e.RightButton = MouseButtonState.Pressed Then
            Form1.DeleteNote(id, Form1.currentchannal)
            Form1.dischoosenote()
        End If
    End Sub

    Private Sub ppart_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Form1.tip.Content = CStr(Margin.Left \ 8) + "," + numbertoname(Val(lb.Content))
            If e.GetPosition(Form1.Backboard1.sv).X < 0 Then
                Form1.Backboard1.sv.ScrollToHorizontalOffset(Form1.Backboard1.sv.HorizontalOffset - 2)
            ElseIf e.GetPosition(Form1.Backboard1.sv).X > Form1.Backboard1.sv.ActualWidth Then
                Form1.Backboard1.sv.ScrollToHorizontalOffset(Form1.Backboard1.sv.HorizontalOffset + 2)
            ElseIf e.GetPosition(Form1.Backboard1.sv).Y < 0 Then
                Form1.Backboard1.sv.ScrollToVerticalOffset(Form1.Backboard1.sv.VerticalOffset - 2)
            ElseIf e.GetPosition(Form1.Backboard1.sv).Y > Form1.Backboard1.sv.ActualHeight Then
                Form1.Backboard1.sv.ScrollToVerticalOffset(Form1.Backboard1.sv.VerticalOffset + 2)
            End If
            Mouse.Capture(ppart)
            If choosed Then
                x2 = e.GetPosition(ppart).X
                y2 = e.GetPosition(ppart).Y
                For i = 0 To Form1.notecount(channal) - 1
                    If Form1.note(channal, i).choosed Then
                        If (Form1.note(channal, i).mb.Margin.Left + (x2 - x1)) < 0 OrElse Form1.note(channal, i).mb.Margin.Top + ((y2 - y1) \ 24) * 24 < 0 OrElse Form1.note(channal, i).mb.Margin.Top + ((y2 - y1) \ 24) * 24 > 3048 Then
                            canmove = False
                        End If
                    End If
                Next
                If canmove Then
                    For i = 0 To Form1.notecount(channal) - 1
                        If Form1.note(channal, i).choosed Then
                            Form1.note(channal, i).mb.Margin = New Thickness(((Form1.note(channal, i).mb.Margin.Left + (x2 - x1)) \ 8) * 8, Form1.note(channal, i).mb.Margin.Top + ((y2 - y1) \ 24) * 24, 0, 0)
                            Form1.volumeb(Form1.currentchannal, i).Margin = New Thickness(Form1.note(channal, i).mb.Margin.Left, Form1.volumeb(Form1.currentchannal, i).Margin.Top, 0, 0)
                            Form1.note(channal, i).lb.Content = CStr(127 - (Form1.note(channal, i).mb.Margin.Top \ 24))
                        End If
                    Next
                End If
            Else
                x2 = e.GetPosition(ppart).X
                y2 = e.GetPosition(ppart).Y
                If (mb.Margin.Left + (x2 - x1)) \ 8 >= 0 AndAlso mb.Margin.Top + ((y2 - y1) \ 24) * 24 >= 0 AndAlso mb.Margin.Top + ((y2 - y1) \ 24) * 24 <= 3048 Then
                    mb.Margin = New Thickness(((mb.Margin.Left + (x2 - x1)) \ 8) * 8, mb.Margin.Top + ((y2 - y1) \ 24) * 24, 0, 0)
                    Form1.volumeb(Form1.currentchannal, id).Margin = New Thickness(Margin.Left, Form1.volumeb(Form1.currentchannal, id).Margin.Top, 0, 0)
                    lb.Content = CStr(127 - (mb.Margin.Top \ 24))
                    If Val(lb.Content) <> lastnote Then
                        midie = New NoteEvent(0, Form1.currentchannal, MidiCommandCode.NoteOff, lastnote, 64)
                        Form1.midioutdevice.Send(midie.GetAsShortMessage)
                        lastnote = Val(lb.Content)
                        midie = New NoteEvent(0, Form1.currentchannal, MidiCommandCode.NoteOn, lastnote, Int(1.28 * (100 - Form1.volumeb(Form1.currentchannal, id).Margin.Top)) - 1)
                        Form1.midioutdevice.Send(midie.GetAsShortMessage)
                    End If
                End If
            End If
        End If
    End Sub

    Private Sub ppart_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        Form1.lastlength = Width \ 8
        Form1.lastvolume = 100 - Form1.volumeb(Form1.currentchannal, id).Margin.Top
        midie = New NoteEvent(0, Form1.currentchannal, MidiCommandCode.NoteOff, lastnote, 64)
        Form1.midioutdevice.Send(midie.GetAsShortMessage)
        Form1.init_play(Form1.currentchannal)
        canmove = True
        Form1.edit()
    End Sub
    Function numbertoname(number As Integer) As String
        Dim a As String
        Dim b As Integer = number \ 12 - 1
        Dim c As String = ""
        Dim result As String
        Select Case number Mod 12
            Case 0
                a = "C"
            Case 1
                a = "C" : c = "#"
            Case 2
                a = "D"
            Case 3
                a = "D" : c = "#"
            Case 4
                a = "E"
            Case 5
                a = "F"
            Case 6
                a = "F" : c = "#"
            Case 7
                a = "G"
            Case 8
                a = "G" : c = "#"
            Case 9
                a = "A"
            Case 10
                a = "A" : c = "#"
            Case 11
                a = "B"
            Case Else
                a = ""
        End Select
        result = a + c + CStr(b)
        Return result
    End Function
End Class
