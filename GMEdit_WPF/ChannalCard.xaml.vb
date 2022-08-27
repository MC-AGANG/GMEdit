Public Class ChannalCard
    Public Form1 As MainWindow
    Public id As Integer
    Dim firstload As Boolean = True
    Dim firstload2 As Integer = 2
    Private Sub MuteButtom_MouseDown(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub MuteButtom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            If Form1.muted(id) Then
                MuteButtom.Background = Brushes.DarkRed
                Form1.muted(id) = False
                For i = 1 To 16
                Next
            Else

                MuteButtom.Background = Brushes.Red
                Form1.muted(id) = True
                Form1.solo(id) = False
                SoloButtom.Background = Brushes.DarkGreen
                Form1.midioutdevice.Reset()
                Form1.setinstrument()
            End If
        End If
    End Sub

    Private Sub SoloButtom_MouseDown(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub SoloButtom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim flag As Boolean
            If Form1.solo(id) Then
                flag = False
                SoloButtom.Background = Brushes.DarkGreen
                Form1.solo(id) = False
                For i = 1 To 16
                    If Form1.solo(i) Then
                        flag = True
                        Exit For
                    End If
                Next
                If Not flag Then
                    For i = 1 To 16
                        Form1.muted(i) = False
                        Form1.cc(i).MuteButtom.Background = Brushes.DarkRed
                    Next
                End If
            Else
                SoloButtom.Background = Brushes.Lime
                Form1.solo(id) = True
                Form1.muted(id) = False
                MuteButtom.Background = Brushes.DarkRed
                For i = 1 To 16
                    If Not Form1.solo(i) Then
                        Form1.muted(i) = True
                        Form1.cc(i).MuteButtom.Background = Brushes.Red
                    End If
                Next
                Form1.midioutdevice.Reset()
                Form1.setinstrument()
            End If
        End If
    End Sub

    Private Sub MainBoard_MouseEnter(sender As Object, e As MouseEventArgs)
        Form1.tip.Content = "音符数量：" + CStr(Form1.notecount(id))
        If Form1.currentchannal <> id Then
            MainBoard.Background = New SolidColorBrush(Color.FromRgb(62, 62, 64))
        End If
    End Sub

    Private Sub MainBoard_MouseLeave(sender As Object, e As MouseEventArgs)
        If Form1.currentchannal <> id Then
            If Form1.currentchannal <> id Then
                MainBoard.Background = New SolidColorBrush(Color.FromRgb(37, 37, 38))
            End If
        End If
    End Sub

    Private Sub MainBoard_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If Form1.currentchannal <> id Then
            For i = 0 To Form1.notecount(Form1.currentchannal) - 1
                Form1.note(Form1.currentchannal, i).IsEnabled = False
                Panel.SetZIndex(Form1.note(Form1.currentchannal, i), 1)
                Panel.SetZIndex(Form1.volumeb(Form1.currentchannal, i), 1)
            Next
            Form1.currentchannal = id
            For i = 0 To Form1.notecount(id) - 1
                Form1.note(Form1.currentchannal, i).IsEnabled = True
                Panel.SetZIndex(Form1.note(id, i), 2)
                Panel.SetZIndex(Form1.volumeb(id, i), 2)
            Next
            MainBoard.Background = New SolidColorBrush(Color.FromRgb(0, 122, 204))
            For i = 1 To 16
                If i <> id Then
                    Form1.cc(i).MainBoard.Background = New SolidColorBrush(Color.FromRgb(37, 37, 38))
                End If
            Next
            If id = 10 Then
                Form1.Backboard1.drum.Visibility = Visibility.Visible
                Form1.Backboard1.pno.Visibility = Visibility.Collapsed
            Else
                Form1.Backboard1.drum.Visibility = Visibility.Collapsed
                Form1.Backboard1.pno.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    Private Sub InstrumentList_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        If firstload Then
            firstload = False
        Else
            Form1.instruments(id) = InstrumentList.SelectedIndex
            Form1.setinstrument()
            Form1.edit()
        End If

    End Sub

    Private Sub MuteButtom_MouseEnter(sender As Object, e As MouseEventArgs)
        Form1.tip.Content = "静音"
    End Sub

    Private Sub SoloButtom_MouseEnter(sender As Object, e As MouseEventArgs)
        Form1.tip.Content = "独奏"
    End Sub

    Private Sub tb_TextChanged(sender As Object, e As TextChangedEventArgs)
        If firstload2 <> 0 Then
            firstload2 -= 1
        Else
            Form1.edit()
        End If
    End Sub
End Class
