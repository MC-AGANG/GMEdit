Public Class BackBoard
    Dim vline(4095) As Line
    Public Form1 As MainWindow
    Dim i As Integer
    Dim x1, x2 As Integer
    Dim choosebox As Rectangle
    Dim startpoint, endpoint As Point
    Dim r As Rect
    Dim choosing As Boolean = False
    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 0 To 127
            vline(i) = New Line
            With vline(i)
                .X1 = (i + 1) * 32
                .X2 = .X1
                .Y1 = 0
                .Y2 = 3071
                .StrokeThickness = 1
                If i Mod 4 = 3 Then
                    .Stroke = New SolidColorBrush(Color.FromArgb(255, 160, 160, 160))
                Else
                    .Stroke = New SolidColorBrush(Color.FromArgb(255, 117, 117, 117))
                End If
            End With
            mainboard.Children.Add(vline(i))
            Panel.SetZIndex(vline(i), 0)
            choosebox = New Rectangle
            mainboard.Children.Add(choosebox)
            With choosebox
                .BringIntoView()
                .HorizontalAlignment = HorizontalAlignment.Left
                .VerticalAlignment = VerticalAlignment.Top
                .Visibility = Visibility.Hidden
                .Stroke = New SolidColorBrush(Color.FromArgb(255, 0, 255, 0))
                .Fill = New SolidColorBrush(Color.FromArgb(32, 0, 255, 0))
            End With
        Next
    End Sub

    Private Sub sv_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        sv_pno.ScrollToVerticalOffset(sv.VerticalOffset)
        sv_volumeboard.ScrollToHorizontalOffset(sv.HorizontalOffset)
        sv_tickbar.ScrollToHorizontalOffset(sv.HorizontalOffset)
    End Sub

    Private Sub mainboard_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            If Keyboard.IsKeyDown(Key.LeftCtrl) Then
                Mouse.Capture(sender)
                startpoint = Mouse.GetPosition(mainboard)
                choosebox.Visibility = Visibility.Visible
                choosebox.Margin = New Thickness(startpoint.X, startpoint.Y, 0, 0)
                choosebox.Width = 0
                choosebox.Height = 0
                Panel.SetZIndex(choosebox, 1)
                choosing = True
            Else
                Form1.CreateNote(127 - e.GetPosition(mainboard).Y \ 24, e.GetPosition(mainboard).X \ 8)
                Form1.edit()
                Form1.dischoosenote()
            End If
        End If
    End Sub

    Private Sub mainboard_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        For i = i To mainboard.Width \ 32
            vline(i) = New Line
            With vline(i)
                .X1 = (i + 1) * 32
                .X2 = .X1
                .Y1 = 0
                .Y2 = 3071
                .StrokeThickness = 1
                If i Mod 4 = 3 Then
                    .Stroke = New SolidColorBrush(Color.FromArgb(255, 160, 160, 160))
                Else
                    .Stroke = New SolidColorBrush(Color.FromArgb(255, 117, 117, 117))
                End If
            End With
            mainboard.Children.Add(vline(i))
            Panel.SetZIndex(vline(i), 0)
        Next
    End Sub

    Private Sub TickBar1_MouseUp(sender As Object, e As MouseButtonEventArgs)
        progressbar.Margin = New Thickness((e.GetPosition(sender).X \ 8) * 8, 0, 0, 0)
        TickBar1.progressbar.Margin = New Thickness((e.GetPosition(sender).X \ 8) * 8 - 3, 0, 0, 0)
        Form1.tick = e.GetPosition(sender).X \ 8
        Form1.midioutdevice.Reset()
        Form1.setinstrument()
        Mouse.Capture(Nothing)
    End Sub

    Private Sub progressbar_MouseDown(sender As Object, e As MouseButtonEventArgs)
        e.Handled = True
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            x1 = e.GetPosition(sender).X
        End If
    End Sub

    Private Sub progressbar_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            x2 = e.GetPosition(sender).X
            If (progressbar.Margin.Left + (x2 - x1)) \ 8 >= 0 Then
                progressbar.Margin = New Thickness(((progressbar.Margin.Left + (x2 - x1)) \ 8) * 8, 0, 0, 0)
                TickBar1.progressbar.Margin = New Thickness(progressbar.Margin.Left - 3, 0, 0, 0)
                Form1.tick = (progressbar.Margin.Left + (x2 - x1)) \ 8
            End If
        End If
    End Sub

    Private Sub TickBar1_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed AndAlso e.GetPosition(sender).X >= 0 Then
            TickBar1.progressbar.Margin = New Thickness((e.GetPosition(sender).X \ 8) * 8 - 3, 0, 0, 0)
            progressbar.Margin = New Thickness((e.GetPosition(sender).X \ 8) * 8, 0, 0, 0)
            Form1.tick = e.GetPosition(sender).X \ 8
        End If
    End Sub

    Private Sub TickBar1_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
        End If
    End Sub

    Private Sub mainboard_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed AndAlso choosing Then
            If e.GetPosition(sv).X < 0 Then
                sv.ScrollToHorizontalOffset(sv.HorizontalOffset - 2)
            ElseIf e.GetPosition(sv).X > sv.ActualWidth Then
                sv.ScrollToHorizontalOffset(sv.HorizontalOffset + 2)
            ElseIf e.GetPosition(sv).y < 0 Then
                sv.ScrollToVerticalOffset(sv.VerticalOffset - 2)
            ElseIf e.GetPosition(sv).y > sv.ActualHeight Then
                sv.ScrollToVerticalOffset(sv.VerticalOffset + 2)
            End If
            Dim note As Notebar
            endpoint = e.GetPosition(mainboard)
            r = New Rect(startpoint, endpoint)
            choosebox.Margin = New Thickness(r.Left, r.Top, 0, 0)
            choosebox.Width = r.Width
            choosebox.Height = r.Height
            For i = 0 To Form1.notecount(Form1.currentchannal) - 1
                note = Form1.note(Form1.currentchannal, i)
                If note.choosed Then
                    If Not (note.Margin.Top > r.Top AndAlso note.Margin.Top + 24 < r.Top + r.Height AndAlso note.Margin.Left < r.X + r.Width AndAlso note.Margin.Left + note.Width > r.X) Then
                        Form1.dischoosenote(i)
                    End If
                Else
                    If note.Margin.Top > r.Top AndAlso note.Margin.Top + 24 < r.Top + r.Height AndAlso note.Margin.Left < r.X + r.Width AndAlso note.Margin.Left + note.Width > r.X Then
                        Form1.choosenote(i)
                    End If
                End If
            Next
        End If
    End Sub

    Private Sub sv_pno_ScrollChanged(sender As Object, e As ScrollChangedEventArgs)
        sv.ScrollToVerticalOffset(sv_pno.VerticalOffset)
    End Sub

    Private Sub mainboard_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        choosebox.Visibility = Visibility.Hidden
        choosing = False
    End Sub

    Private Sub progressbar_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
    End Sub
End Class
