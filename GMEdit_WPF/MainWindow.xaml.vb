Imports System.IO
Imports NAudio.Midi
Public Class MainWindow
    Dim nx, ny, nw, nh As Integer
    Dim labely1, labely2 As Integer
    Dim labeln As Double = 20
    Dim tps As Double = 20
    Public maxsized As Boolean = False
    Public note(16, 32767) As Notebar
    Public notecount(16) As Long
    Public lastlength As Integer = 16
    Public lastvolume As Integer = 80
    Public volumeb(16, 32767) As VolumeBar
    Dim OpenFileDialog1 As New Forms.OpenFileDialog
    Dim OpenFileDialog2 As New Forms.OpenFileDialog
    Dim SaveFileDialog1 As New Forms.SaveFileDialog
    Dim FolderBrowserDialog1 As New Forms.FolderBrowserDialog
    Public tick As Long = 0
    Dim frame As Long = 0
    Public midicommandstring(16, 16383) As String
    Public midioutdevice As MidiOut
    Public Timer1 As Forms.Timer
    Public timer2 As Timers.Timer
    Public channalcolors(16) As Color
    Public currentchannal As Integer = 1
    Public muted(16) As Boolean
    Public solo(16) As Boolean
    Public cc(16) As ChannalCard
    Public instruments(16) As Integer
    Public isloop As Boolean = True
    Public isautopage As Boolean = True
    Dim x1, y1, x2, y2, h1, w1, t1, l1 As Integer
    Dim sizing As Boolean = False
    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
        tb1.Form1 = Me
        Backboard1.Form1 = Me
        Backboard1.drum.Form1 = Me
        midioutdevice = New MidiOut(0)
        Timer1 = New Forms.Timer
        Timer1.Interval = 16
        timer2 = New Timers.Timer
        timer2.Interval = 50
        Backboard1.pno.form1 = Me
        AddHandler Timer1.Tick, AddressOf playing
        AddHandler timer2.Elapsed, AddressOf playing2
        OpenFileDialog1.Filter = "GMEdit工程文件|*.gme|Minecraft函数|*.mcfunction"
        OpenFileDialog2.Filter = "MIDI序列|*.mid"
        SaveFileDialog1.Filter = "GMEdit工程文件|*.gme"
        setcolor()
        For i = 1 To 16
            notecount(i) = 0
            instruments(i) = 1
        Next
    End Sub
    Public Sub MaxSize()
        nx = Left : ny = Top : nw = Width : nh = Height
        Height = Forms.Screen.PrimaryScreen.WorkingArea.Size.Height + 17
        Width = Forms.Screen.PrimaryScreen.WorkingArea.Size.Width + 17
        Top = -9
        Left = -9
        maxsized = True
    End Sub
    Public Sub NormalSize()
        Height = nh
        Width = nw
        Top = ny
        Left = nx
        maxsized = False
    End Sub
    Public Sub CreateNote(tone As Integer, tick As Integer)
        note(currentchannal, notecount(currentchannal)) = New Notebar
        Backboard1.mainboard.Children.Add(note(currentchannal, notecount(currentchannal)))
        With note(currentchannal, notecount(currentchannal))
            .BringIntoView()
            .Form1 = Me
            .Margin = New Thickness(tick * 8, (127 - tone) * 24, 0, 0)
            .Height = 24
            .Width = lastlength * 8
            .id = notecount(currentchannal)
            .lb.Content = CStr(tone)
            .Background = New SolidColorBrush(channalcolors(currentchannal))
            .channal = currentchannal
        End With
        volumeb(currentchannal, notecount(currentchannal)) = New VolumeBar
        Backboard1.VolumeBoard1.mb.Children.Add(volumeb(currentchannal, notecount(currentchannal)))
        With volumeb(currentchannal, notecount(currentchannal))
            .BringIntoView()
            .Form1 = Me
            .Height = 100
            .Width = lastlength * 8
            .Margin = New Thickness(tick * 8, 100 - lastvolume, 0, 0)
            .id = notecount(currentchannal)
            .h.Fill = New SolidColorBrush(channalcolors(currentchannal))
            .v.Fill = New SolidColorBrush(channalcolors(currentchannal))
        End With
        If note(currentchannal, notecount(currentchannal)).Margin.Left > Backboard1.mainboard.Width - 128 Then
            Backboard1.mainboard.Width += 512
            Backboard1.sp.Width += 512
            Backboard1.VolumeBoard1.mb.Width += 512
        End If
        Panel.SetZIndex(note(currentchannal, notecount(currentchannal)), 2)
        Panel.SetZIndex(volumeb(currentchannal, notecount(currentchannal)), 2)
        notecount(currentchannal) += 1
        init_play(currentchannal)
    End Sub
    Public Sub CreateNote(tone As Integer, tick As Integer, length As Integer, volume As Integer, channal As Integer)
        note(channal, notecount(channal)) = New Notebar
        Backboard1.mainboard.Children.Add(note(channal, notecount(channal)))
        With note(channal, notecount(channal))
            .BringIntoView()
            .Form1 = Me
            .Margin = New Thickness(tick * 8, (127 - tone) * 24, 0, 0)
            .Height = 24
            .Width = length * 8
            .id = notecount(channal)
            .lb.Content = CStr(tone)
            .Background = New SolidColorBrush(channalcolors(channal))
            .channal = channal
        End With
        volumeb(channal, notecount(channal)) = New VolumeBar
        Backboard1.VolumeBoard1.mb.Children.Add(volumeb(channal, notecount(channal)))
        With volumeb(channal, notecount(channal))
            .BringIntoView()
            .Form1 = Me
            .Height = 100
            .Width = length * 8
            .Margin = New Thickness(tick * 8, 100 - volume, 0, 0)
            .id = notecount(channal)
            .h.Fill = New SolidColorBrush(channalcolors(channal))
            .v.Fill = New SolidColorBrush(channalcolors(channal))
        End With
        If note(channal, notecount(channal)).Margin.Left > Backboard1.mainboard.Width - 128 Then
            Backboard1.mainboard.Width += 1024
            Backboard1.sp.Width += 1024
            Backboard1.VolumeBoard1.mb.Width += 1024
        End If
        notecount(channal) += 1
    End Sub
    Public Sub DeleteNote(id As Integer, channal As Integer)
        Backboard1.mainboard.Children.Remove(note(channal, id))
        Backboard1.VolumeBoard1.mb.Children.Remove(volumeb(channal, id))
        For i = id To notecount(channal) - 1
            note(channal, i).id -= 1
            note(channal, i) = note(channal, i + 1)
            volumeb(channal, i).id -= 1
            volumeb(channal, i) = volumeb(channal, i + 1)
        Next
        notecount(channal) -= 1
        init_play(channal)
    End Sub
    Public Sub choosenote(id As Integer)
        note(currentchannal, id).choosed = True
        note(currentchannal, id).Background = New SolidColorBrush(Color.FromRgb(0, 255, 0))
        volumeb(currentchannal, id).v.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
        volumeb(currentchannal, id).h.Fill = New SolidColorBrush(Color.FromRgb(0, 255, 0))
    End Sub
    Public Sub choosenote()
        For i = 0 To notecount(currentchannal) - 1
            choosenote(i)
        Next
    End Sub
    Public Sub dischoosenote(id As Integer)
        note(currentchannal, id).choosed = False
        note(currentchannal, id).Background = New SolidColorBrush(channalcolors(currentchannal))
        volumeb(currentchannal, id).v.Fill = New SolidColorBrush(channalcolors(currentchannal))
        volumeb(currentchannal, id).h.Fill = New SolidColorBrush(channalcolors(currentchannal))
    End Sub
    Public Sub dischoosenote()
        For i = 0 To notecount(currentchannal) - 1
            If note(currentchannal, i).choosed Then
                dischoosenote(i)
            End If
        Next
    End Sub

    Private Sub newbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            Backboard1.Visibility = Visibility.Visible
            ChannalBoardsv.Visibility = Visibility.Visible
            If Mid(Filename.Content, Len(Filename.Content), 1) = "*" Then
                Dim n As Integer = MsgBox("是否保存""" + Mid(Filename.Content, 1, Len(Filename.Content) - 1) + """？", 3)
                If n = MsgBoxResult.Yes Then
                    savefile()
                    For i = 1 To 16
                        clearboard(i)
                        cc(i).InstrumentList.SelectedIndex = 0
                    Next
                    Filename.Content = "Untitled.gme"
                ElseIf n = MsgBoxResult.No Then
                    For i = 1 To 16
                        clearboard(i)
                        cc(i).InstrumentList.SelectedIndex = 0
                    Next
                    Filename.Content = "Untitled.gme"
                End If
            Else
                For i = 1 To 16
                    clearboard(i)
                    cc(i).InstrumentList.SelectedIndex = 0
                Next
                Filename.Content = "Untitled.gme"
            End If
            Backboard1.sv.ScrollToVerticalOffset(1200)
        End If
    End Sub

    Private Sub openbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))

            If Mid(Filename.Content, Len(Filename.Content), 1) = "*" Then
                Dim n As Integer = MsgBox("是否保存""" + Mid(Filename.Content, 1, Len(Filename.Content) - 1) + """？", 3)
                If n = MsgBoxResult.Yes Then
                    savefile()
                    If OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
                        Backboard1.Visibility = Visibility.Visible
                        ChannalBoardsv.Visibility = Visibility.Visible
                        For i = 1 To 16
                            clearboard(i)
                            cc(i).InstrumentList.SelectedIndex = 0
                        Next
                        If Mid(OpenFileDialog1.FileName, Len(OpenFileDialog1.FileName) - 2) = "gme" Then
                            Dim reader As New BinaryReader(New FileStream(OpenFileDialog1.FileName, FileMode.Open))
                            Dim data As UInteger
                            Dim tick As Integer
                            Dim tone As Integer
                            Dim volume As Integer
                            Dim length As Integer
                            For channal = 1 To 16
                                cc(channal).InstrumentList.SelectedIndex = reader.ReadInt32
                                Do
                                    data = reader.ReadInt32
                                    If data = 32767 Then Exit Do
                                    tick = data
                                    data = reader.ReadInt32
                                    volume = data Mod 256
                                    data \= 256
                                    tone = data Mod 256
                                    data \= 256
                                    length = data
                                    CreateNote(tone, tick, length, volume, channal)
                                    If channal <> currentchannal Then
                                        note(channal, notecount(channal) - 1).IsEnabled = False
                                    End If
                                Loop
                            Next
                            reader.Close()
                            Dim reader2 As New StreamReader(OpenFileDialog1.FileName)
                            Do Until reader2.ReadLine = "endofnoteevents"
                            Loop
                            reader2.ReadLine()
                            tps = Val(reader2.ReadLine)
                            tickrate.Content = CStr(tps)
                            labeln = tps
                            Timer1.Interval = 1000 \ tps
                            For i = 1 To 16
                                cc(i).tb.Text = reader2.ReadLine
                            Next
                            For i = 0 To notecount(currentchannal) - 1
                                Panel.SetZIndex(note(currentchannal, i), 1)
                            Next
                            reader2.Close()
                            For i = 1 To 16
                                SortNotes(i)
                                init_play(i)
                            Next
                            Filename.Content = OpenFileDialog1.SafeFileName
                        Else
                            Dim ticks(32767) As Integer
                            Dim tones(32767) As Integer
                            Dim volumes(32767) As Integer
                            Dim reader As New StreamReader(OpenFileDialog1.FileName)
                            Dim tone As Integer
                            Dim tick As Integer
                            Dim length As Integer
                            Dim numberreading As Boolean
                            Dim sourcestr As String
                            Dim i, j As Integer
                            Dim nc As Integer
                            Do Until reader.EndOfStream
                                sourcestr = reader.ReadLine
                                For i = 1 To Len(sourcestr)
                                    If Mid(sourcestr, i, 5) = "tick=" Then
                                        Exit For
                                    End If
                                Next
                                tick = 0
                                numberreading = False
                                For i = i To Len(sourcestr)
                                    If Mid(sourcestr, i, 1) <= "9" And Mid(sourcestr, i, 1) >= "0" Then
                                        numberreading = True
                                        tick = tick * 10 + Val(Mid(sourcestr, i, 1))
                                    ElseIf numberreading Then
                                        Exit For
                                    End If
                                Next
                                For i = 1 To Len(sourcestr)
                                    If Mid(sourcestr, i, 9) = "playsound" Then
                                        nc += 1
                                        ticks(nc) = tick
                                        tone = 0
                                        numberreading = False
                                        For j = i + 9 To Len(sourcestr)
                                            If Mid(sourcestr, j, 2) = ".l" Then
                                                Exit For
                                            End If
                                        Next
                                        For j = j To Len(sourcestr)
                                            If Mid(sourcestr, j, 1) <= "9" And Mid(sourcestr, j, 1) >= "0" Then
                                                numberreading = True
                                                tone = tone * 10 + Val(Mid(sourcestr, j, 1))
                                            ElseIf numberreading Then
                                                Exit For
                                            End If
                                        Next
                                        tones(nc) = tone
                                        For j = Len(sourcestr) To 1 Step -1
                                            If Mid(sourcestr, j, 1) = " " Then
                                                Exit For
                                            End If
                                        Next
                                        volumes(nc) = Int(Val(Mid(sourcestr, j)) * 100)
                                        If volumes(nc) > 100 Then
                                            volumes(nc) = 100
                                        ElseIf volumes(nc) < 1 Then
                                            volumes(nc) = 1
                                        End If
                                    ElseIf Mid(sourcestr, i, 9) = "stopsound" Then
                                        tone = 0
                                        numberreading = False
                                        For j = i + 9 To Len(sourcestr)
                                            If Mid(sourcestr, j, 2) = ".l" Then
                                                Exit For
                                            End If
                                        Next
                                        For j = j To Len(sourcestr)
                                            If Mid(sourcestr, j, 1) <= "9" And Mid(sourcestr, j, 1) >= "0" Then
                                                numberreading = True
                                                tone = tone * 10 + Val(Mid(sourcestr, j, 1))
                                            ElseIf numberreading Then
                                                Exit For
                                            End If
                                        Next
                                        For j = nc To 1 Step -1
                                            If tones(j) = tone Then
                                                length = tick - ticks(j)
                                                CreateNote(tone, ticks(j), length, volumes(j), 1)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                Next
                            Loop
                            reader.Close()
                            For i = 0 To notecount(currentchannal) - 1
                                Panel.SetZIndex(note(currentchannal, i), 1)
                            Next
                            SortNotes(currentchannal)
                            init_play(currentchannal)
                            Filename.Content = "Untitled.gme*"
                        End If
                        Backboard1.sv.ScrollToHorizontalOffset(0)
                    End If
                ElseIf n = MsgBoxResult.No Then
                    If OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
                        Backboard1.Visibility = Visibility.Visible
                        ChannalBoardsv.Visibility = Visibility.Visible
                        For i = 1 To 16
                            clearboard(i)
                        Next
                        If Mid(OpenFileDialog1.FileName, Len(OpenFileDialog1.FileName) - 2) = "gme" Then
                            Dim reader As New BinaryReader(New FileStream(OpenFileDialog1.FileName, FileMode.Open))
                            Dim data As UInteger
                            Dim tick As Integer
                            Dim tone As Integer
                            Dim volume As Integer
                            Dim length As Integer
                            For channal = 1 To 16
                                cc(channal).InstrumentList.SelectedIndex = reader.ReadInt32
                                Do
                                    data = reader.ReadInt32
                                    If data = 32767 Then Exit Do
                                    tick = data
                                    data = reader.ReadInt32
                                    volume = data Mod 256
                                    data \= 256
                                    tone = data Mod 256
                                    data \= 256
                                    length = data
                                    CreateNote(tone, tick, length, volume, channal)
                                    If channal <> currentchannal Then
                                        note(channal, notecount(channal) - 1).IsEnabled = False
                                    End If
                                Loop
                            Next
                            reader.Close()
                            Dim reader2 As New StreamReader(OpenFileDialog1.FileName)
                            Do Until reader2.ReadLine = "endofnoteevents"
                            Loop
                            reader2.ReadLine()
                            tps = Val(reader2.ReadLine)
                            tickrate.Content = CStr(tps)
                            labeln = tps
                            Timer1.Interval = 1000 \ tps
                            For i = 1 To 16
                                cc(i).tb.Text = reader2.ReadLine
                            Next
                            For i = 0 To notecount(currentchannal) - 1
                                Panel.SetZIndex(note(currentchannal, i), 1)
                            Next
                            reader2.Close()
                            For i = 1 To 16
                                SortNotes(i)
                                init_play(i)
                            Next
                            Filename.Content = OpenFileDialog1.SafeFileName
                        Else
                            Dim ticks(32767) As Integer
                            Dim tones(32767) As Integer
                            Dim volumes(32767) As Integer
                            Dim reader As New StreamReader(OpenFileDialog1.FileName)
                            Dim tone As Integer
                            Dim tick As Integer
                            Dim length As Integer
                            Dim numberreading As Boolean
                            Dim sourcestr As String
                            Dim i, j As Integer
                            Dim nc As Integer
                            Do Until reader.EndOfStream
                                sourcestr = reader.ReadLine
                                For i = 1 To Len(sourcestr)
                                    If Mid(sourcestr, i, 5) = "tick=" Then
                                        Exit For
                                    End If
                                Next
                                tick = 0
                                numberreading = False
                                For i = i To Len(sourcestr)
                                    If Mid(sourcestr, i, 1) <= "9" And Mid(sourcestr, i, 1) >= "0" Then
                                        numberreading = True
                                        tick = tick * 10 + Val(Mid(sourcestr, i, 1))
                                    ElseIf numberreading Then
                                        Exit For
                                    End If
                                Next
                                For i = 1 To Len(sourcestr)
                                    If Mid(sourcestr, i, 9) = "playsound" Then
                                        nc += 1
                                        ticks(nc) = tick
                                        tone = 0
                                        numberreading = False
                                        For j = i + 9 To Len(sourcestr)
                                            If Mid(sourcestr, j, 2) = ".l" Then
                                                Exit For
                                            End If
                                        Next
                                        For j = j To Len(sourcestr)
                                            If Mid(sourcestr, j, 1) <= "9" And Mid(sourcestr, j, 1) >= "0" Then
                                                numberreading = True
                                                tone = tone * 10 + Val(Mid(sourcestr, j, 1))
                                            ElseIf numberreading Then
                                                Exit For
                                            End If
                                        Next
                                        tones(nc) = tone
                                        For j = Len(sourcestr) To 1 Step -1
                                            If Mid(sourcestr, j, 1) = " " Then
                                                Exit For
                                            End If
                                        Next
                                        volumes(nc) = Int(Val(Mid(sourcestr, j)) * 100)
                                        If volumes(nc) > 100 Then
                                            volumes(nc) = 100
                                        ElseIf volumes(nc) < 1 Then
                                            volumes(nc) = 1
                                        End If
                                    ElseIf Mid(sourcestr, i, 9) = "stopsound" Then
                                        tone = 0
                                        numberreading = False
                                        For j = i + 9 To Len(sourcestr)
                                            If Mid(sourcestr, j, 2) = ".l" Then
                                                Exit For
                                            End If
                                        Next
                                        For j = j To Len(sourcestr)
                                            If Mid(sourcestr, j, 1) <= "9" And Mid(sourcestr, j, 1) >= "0" Then
                                                numberreading = True
                                                tone = tone * 10 + Val(Mid(sourcestr, j, 1))
                                            ElseIf numberreading Then
                                                Exit For
                                            End If
                                        Next
                                        For j = nc To 1 Step -1
                                            If tones(j) = tone Then
                                                length = tick - ticks(j)
                                                CreateNote(tone, ticks(j), length, volumes(j), 1)
                                                Exit For
                                            End If
                                        Next
                                    End If
                                Next
                            Loop
                            reader.Close()
                            For i = 0 To notecount(currentchannal) - 1
                                Panel.SetZIndex(note(currentchannal, i), 1)
                            Next
                            SortNotes(currentchannal)
                            init_play(currentchannal)
                            Filename.Content = "Untitled.gme*"
                        End If
                        Backboard1.sv.ScrollToHorizontalOffset(0)
                    End If
                End If
            Else
                If OpenFileDialog1.ShowDialog = Forms.DialogResult.OK Then
                    Backboard1.Visibility = Visibility.Visible
                    ChannalBoardsv.Visibility = Visibility.Visible
                    For i = 1 To 16
                        clearboard(i)
                        cc(i).InstrumentList.SelectedIndex = 0
                    Next
                    If Mid(OpenFileDialog1.FileName, Len(OpenFileDialog1.FileName) - 2) = "gme" Then
                        Dim reader As New BinaryReader(New FileStream(OpenFileDialog1.FileName, FileMode.Open))
                        Dim data As UInteger
                        Dim tick As Integer
                        Dim tone As Integer
                        Dim volume As Integer
                        Dim length As Integer
                        For channal = 1 To 16
                            cc(channal).InstrumentList.SelectedIndex = reader.ReadInt32
                            Do
                                data = reader.ReadInt32
                                If data = 32767 Then Exit Do
                                tick = data
                                data = reader.ReadInt32
                                volume = data Mod 256
                                data \= 256
                                tone = data Mod 256
                                data \= 256
                                length = data
                                CreateNote(tone, tick, length, volume, channal)
                                If channal <> currentchannal Then
                                    note(channal, notecount(channal) - 1).IsEnabled = False
                                End If
                            Loop
                        Next
                        reader.Close()
                        Dim reader2 As New StreamReader(OpenFileDialog1.FileName)
                        Do Until reader2.ReadLine = "endofnoteevents"
                        Loop
                        reader2.ReadLine()
                        tps = Val(reader2.ReadLine)
                        tickrate.Content = CStr(tps)
                        labeln = tps
                        Timer1.Interval = 1000 \ tps
                        For i = 1 To 16
                            cc(i).tb.Text = reader2.ReadLine
                        Next
                        For i = 0 To notecount(currentchannal) - 1
                            Panel.SetZIndex(note(currentchannal, i), 1)
                        Next
                        reader2.Close()
                        For i = 1 To 16
                            SortNotes(i)
                            init_play(i)
                        Next
                        Filename.Content = OpenFileDialog1.SafeFileName
                    Else
                        Dim ticks(32767) As Integer
                        Dim tones(32767) As Integer
                        Dim volumes(32767) As Integer
                        Dim reader As New StreamReader(OpenFileDialog1.FileName)
                        Dim tone As Integer
                        Dim tick As Integer
                        Dim length As Integer
                        Dim numberreading As Boolean
                        Dim sourcestr As String
                        Dim i, j As Integer
                        Dim nc As Integer
                        Do Until reader.EndOfStream
                            sourcestr = reader.ReadLine
                            For i = 1 To Len(sourcestr)
                                If Mid(sourcestr, i, 5) = "tick=" Then
                                    Exit For
                                End If
                            Next
                            tick = 0
                            numberreading = False
                            For i = i To Len(sourcestr)
                                If Mid(sourcestr, i, 1) <= "9" And Mid(sourcestr, i, 1) >= "0" Then
                                    numberreading = True
                                    tick = tick * 10 + Val(Mid(sourcestr, i, 1))
                                ElseIf numberreading Then
                                    Exit For
                                End If
                            Next
                            For i = 1 To Len(sourcestr)
                                If Mid(sourcestr, i, 9) = "playsound" Then
                                    nc += 1
                                    ticks(nc) = tick
                                    tone = 0
                                    numberreading = False
                                    For j = i + 9 To Len(sourcestr)
                                        If Mid(sourcestr, j, 2) = ".l" Then
                                            Exit For
                                        End If
                                    Next
                                    For j = j To Len(sourcestr)
                                        If Mid(sourcestr, j, 1) <= "9" And Mid(sourcestr, j, 1) >= "0" Then
                                            numberreading = True
                                            tone = tone * 10 + Val(Mid(sourcestr, j, 1))
                                        ElseIf numberreading Then
                                            Exit For
                                        End If
                                    Next
                                    tones(nc) = tone
                                    For j = Len(sourcestr) To 1 Step -1
                                        If Mid(sourcestr, j, 1) = " " Then
                                            Exit For
                                        End If
                                    Next
                                    volumes(nc) = Int(Val(Mid(sourcestr, j)) * 100)
                                    If volumes(nc) > 100 Then
                                        volumes(nc) = 100
                                    ElseIf volumes(nc) < 1 Then
                                        volumes(nc) = 1
                                    End If
                                ElseIf Mid(sourcestr, i, 9) = "stopsound" Then
                                    tone = 0
                                    numberreading = False
                                    For j = i + 9 To Len(sourcestr)
                                        If Mid(sourcestr, j, 2) = ".l" Then
                                            Exit For
                                        End If
                                    Next
                                    For j = j To Len(sourcestr)
                                        If Mid(sourcestr, j, 1) <= "9" And Mid(sourcestr, j, 1) >= "0" Then
                                            numberreading = True
                                            tone = tone * 10 + Val(Mid(sourcestr, j, 1))
                                        ElseIf numberreading Then
                                            Exit For
                                        End If
                                    Next
                                    For j = nc To 1 Step -1
                                        If tones(j) = tone Then
                                            length = tick - ticks(j)
                                            CreateNote(tone, ticks(j), length, volumes(j), 1)
                                            Exit For
                                        End If
                                    Next
                                End If
                            Next
                        Loop
                        reader.Close()
                        For i = 0 To notecount(currentchannal) - 1
                            Panel.SetZIndex(note(currentchannal, i), 1)
                        Next
                        SortNotes(currentchannal)
                        init_play(currentchannal)
                        Filename.Content = "Untitled.gme*"
                    End If
                    Backboard1.sv.ScrollToHorizontalOffset(0)
                End If
            End If
        End If
    End Sub

    Private Sub savebuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            savefile()
        End If
    End Sub

    Private Sub savenewbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            If Filename.Content <> "就绪" Then
                If Mid(Filename.Content, Len(Filename.Content), 1) = "*" Then
                    SaveFileDialog1.FileName = Mid(Filename.Content, 1, Len(Filename.Content) - 1)
                Else
                    SaveFileDialog1.FileName = Filename.Content
                End If
                If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
                    Dim fs As New FileStream(SaveFileDialog1.FileName, FileMode.Create)
                    Dim writer As New BinaryWriter(fs)
                    Dim data As UInteger
                    For channal = 1 To 16
                        writer.Write(cc(channal).InstrumentList.SelectedIndex)
                        For i = 0 To notecount(channal) - 1
                            data = note(channal, i).Margin.Left \ 8
                            writer.Write(data)
                            data = note(channal, i).Width \ 8
                            data *= 65536
                            data += Val(note(channal, i).lb.Content) * 256 + 100 - volumeb(channal, i).Margin.Top
                            writer.Write(data)
                        Next
                        writer.Write(32767)
                    Next
                    writer.Flush()
                    writer.Close()
                    Dim writer2 As New StreamWriter(SaveFileDialog1.FileName, True)
                    writer2.WriteLine(" ")
                    writer2.WriteLine("endofnoteevents")
                    writer2.WriteLine("fileversion=0")
                    writer2.WriteLine(CStr(tps))
                    For i = 1 To 16
                        writer2.WriteLine(cc(i).tb.Text)
                    Next
                    writer2.Flush()
                    writer2.Close()
                    MsgBox("保存成功！")
                    Filename.Content = getsafefilename(SaveFileDialog1.FileName)
                End If
            End If
        End If
    End Sub

    Private Sub buttom_MouseEnter(sender As Object, e As MouseEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
    End Sub

    Private Sub tickrate_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            labely1 = e.GetPosition(sender).Y
            labeln = tps
        End If
    End Sub

    Private Sub tickrate_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            labely2 = e.GetPosition(sender).Y
            If labeln + (labely1 - labely2) / 10 > 100 Then
                tps = 100
            ElseIf labeln + (labely1 - labely2) / 10 < 1 Then
                tps = 1
            Else
                tps = labeln + (labely1 - labely2) / 10
            End If
            timer2.Interval = 1000 / tps
            tickrate.Content = tps.ToString("F1")
        End If
    End Sub

    Private Sub tickrate_MouseUp(sender As Object, e As MouseButtonEventArgs)
        labeln = tps
        Mouse.Capture(Nothing)
    End Sub

    Private Sub label_MouseLeave(sender As Object, e As MouseEventArgs)
        Dim target As Label = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 51, 51, 55))
    End Sub
    Private Sub label_MouseEnter(sender As Object, e As MouseEventArgs)
        Dim target As Label = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
    End Sub
    Private Sub buttom_MouseLeave(sender As Object, e As MouseEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 45, 45, 48))
    End Sub

    Private Sub playbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            'For i = 1 To 16
            '    SortNotes(i)
            'Next
            setinstrument()
            Timer1.Enabled = True
            timer2.Start()

            playbuttom.Visibility = Visibility.Collapsed
            pausebuttom.Visibility = Visibility.Visible
        End If
    End Sub

    Private Sub pausebuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            Timer1.Enabled = False
            timer2.Stop()

            midioutdevice.Reset()
            pausebuttom.Visibility = Visibility.Collapsed
            playbuttom.Visibility = Visibility.Visible
            setinstrument()
        End If
    End Sub

    Private Sub stopbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            timer2.Stop()
            Timer1.Start()
            tick = 0
            Backboard1.progressbar.Margin = New Thickness(0, 0, 0, 0)
            Backboard1.TickBar1.progressbar.Margin = New Thickness(-3, 0, 0, 0)
            midioutdevice.Reset()
            pausebuttom.Visibility = Visibility.Collapsed
            playbuttom.Visibility = Visibility.Visible
            setinstrument()
        End If
    End Sub

    Private Sub buttom_MouseDown(sender As Object, e As MouseButtonEventArgs)
        Dim target As DockPanel = sender
        target.Background = New SolidColorBrush(Color.FromArgb(255, 0, 122, 204))
    End Sub

    Private Sub openmidifilebuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))

            If Mid(Filename.Content, Len(Filename.Content), 1) = "*" Then
                Dim n As Integer = MsgBox("是否保存""" + Mid(Filename.Content, 1, Len(Filename.Content) - 1) + """？", 3)
                If n = MsgBoxResult.Yes Then
                    savefile()
                    If OpenFileDialog2.ShowDialog = Forms.DialogResult.OK Then
                        For i = 1 To 16
                            clearboard(i)
                            cc(i).InstrumentList.SelectedIndex = 0
                        Next
                        Backboard1.Visibility = Visibility.Visible
                        ChannalBoardsv.Visibility = Visibility.Visible
                        Dim midif As New MidiFile(OpenFileDialog2.FileName, False)
                        Dim tpqn As Integer = midif.DeltaTicksPerQuarterNote
                        Dim bpm As Double = GetBPM(midif)
                        Dim multime As Integer = tpqn \ 8
                        Dim ne As NoteOnEvent
                        Dim pe As PatchChangeEvent
                        tps = Int(bpm / 7.5 * 10) / 10
                        labeln = tps
                        tickrate.Content = CStr(tps)
                        timer2.Interval = 1000 / tps
                        For Each track In midif.Events
                            For Each midie In track
                                If midie.CommandCode = MidiCommandCode.NoteOn Then
                                    ne = midie
                                    If ne.Velocity <> 0 Then
                                        If ne.Channel <> 10 Then
                                            If ne.NoteLength \ multime = 0 Then
                                                ne.NoteLength += multime
                                            End If
                                            CreateNote(ne.NoteNumber, ne.AbsoluteTime \ multime, ne.NoteLength \ multime, Int((ne.Velocity + 1) / 1.28), ne.Channel)
                                        Else
                                            CreateNote(ne.NoteNumber, ne.AbsoluteTime \ multime, 1, Int((ne.Velocity + 1) / 1.28), ne.Channel)
                                        End If
                                        If ne.Channel <> currentchannal Then
                                            note(ne.Channel, notecount(ne.Channel) - 1).IsEnabled = False
                                        End If
                                    End If
                                ElseIf midie.CommandCode = MidiCommandCode.PatchChange And midie.Channel <> 10 Then
                                    pe = midie
                                    cc(pe.Channel).InstrumentList.SelectedIndex = pe.Patch
                                    instruments(pe.Channel) = pe.Patch
                                End If
                            Next
                        Next
                        For i = 0 To notecount(currentchannal) - 1
                            Panel.SetZIndex(note(currentchannal, i), 1)
                        Next
                        For i = 1 To 16
                            SortNotes(i)
                            init_play(i)
                        Next
                        setinstrument()
                        Backboard1.sv.ScrollToHorizontalOffset(0)
                        Filename.Content = Mid(OpenFileDialog2.SafeFileName, 1, Len(OpenFileDialog2.SafeFileName) - 3) + "gme*"
                    End If
                ElseIf n = MsgBoxResult.No Then
                    If OpenFileDialog2.ShowDialog = Forms.DialogResult.OK Then
                        For i = 1 To 16
                            clearboard(i)
                            cc(i).InstrumentList.SelectedIndex = 0
                        Next
                        Backboard1.Visibility = Visibility.Visible
                        ChannalBoardsv.Visibility = Visibility.Visible
                        Dim midif As New MidiFile(OpenFileDialog2.FileName, False)
                        Dim tpqn As Integer = midif.DeltaTicksPerQuarterNote
                        Dim bpm As Double = GetBPM(midif)
                        Dim multime As Integer = tpqn \ 8
                        Dim ne As NoteOnEvent
                        Dim pe As PatchChangeEvent
                        tps = Int(bpm / 7.5 * 10) / 10
                        labeln = tps
                        tickrate.Content = CStr(tps)
                        timer2.Interval = 1000 / tps
                        For Each track In midif.Events
                            For Each midie In track
                                If midie.CommandCode = MidiCommandCode.NoteOn Then
                                    ne = midie
                                    If ne.Velocity <> 0 Then
                                        If ne.Channel <> 10 Then
                                            If ne.NoteLength \ multime = 0 Then
                                                ne.NoteLength += multime
                                            End If
                                            CreateNote(ne.NoteNumber, ne.AbsoluteTime \ multime, ne.NoteLength \ multime, Int((ne.Velocity + 1) / 1.28), ne.Channel)
                                        Else
                                            CreateNote(ne.NoteNumber, ne.AbsoluteTime \ multime, 1, Int((ne.Velocity + 1) / 1.28), ne.Channel)
                                        End If
                                        If ne.Channel <> currentchannal Then
                                            note(ne.Channel, notecount(ne.Channel) - 1).IsEnabled = False
                                        End If
                                    End If
                                ElseIf midie.CommandCode = MidiCommandCode.PatchChange And midie.Channel <> 10 Then
                                    pe = midie
                                    cc(pe.Channel).InstrumentList.SelectedIndex = pe.Patch
                                    instruments(pe.Channel) = pe.Patch
                                End If
                            Next
                        Next
                        For i = 0 To notecount(currentchannal) - 1
                            Panel.SetZIndex(note(currentchannal, i), 1)
                        Next
                        For i = 1 To 16
                            SortNotes(i)
                            init_play(i)
                        Next
                        setinstrument()
                        Backboard1.sv.ScrollToHorizontalOffset(0)
                        Filename.Content = Mid(OpenFileDialog2.SafeFileName, 1, Len(OpenFileDialog2.SafeFileName) - 3) + "gme*"
                    End If
                End If
            Else
                If OpenFileDialog2.ShowDialog = Forms.DialogResult.OK Then
                    For i = 1 To 16
                        clearboard(i)
                        cc(i).InstrumentList.SelectedIndex = 0
                    Next
                    Backboard1.Visibility = Visibility.Visible
                    ChannalBoardsv.Visibility = Visibility.Visible
                    Dim midif As New MidiFile(OpenFileDialog2.FileName, False)
                    Dim tpqn As Integer = midif.DeltaTicksPerQuarterNote
                    Dim bpm As Double = GetBPM(midif)
                    Dim multime As Integer = tpqn \ 8
                    Dim ne As NoteOnEvent
                    Dim pe As PatchChangeEvent
                    tps = Int(bpm / 7.5 * 10) / 10
                    labeln = tps
                    tickrate.Content = CStr(tps)
                    timer2.Interval = 1000 / tps
                    For Each track In midif.Events
                        For Each midie In track
                            If midie.CommandCode = MidiCommandCode.NoteOn Then
                                ne = midie
                                If ne.Velocity <> 0 Then
                                    If ne.Channel <> 10 Then
                                        If ne.NoteLength \ multime = 0 Then
                                            ne.NoteLength += multime
                                        End If
                                        CreateNote(ne.NoteNumber, ne.AbsoluteTime \ multime, ne.NoteLength \ multime, Int((ne.Velocity + 1) / 1.28), ne.Channel)
                                    Else
                                        CreateNote(ne.NoteNumber, ne.AbsoluteTime \ multime, 1, Int((ne.Velocity + 1) / 1.28), ne.Channel)
                                    End If
                                    If ne.Channel <> currentchannal Then
                                        note(ne.Channel, notecount(ne.Channel) - 1).IsEnabled = False
                                    End If
                                End If
                            ElseIf midie.CommandCode = MidiCommandCode.PatchChange And midie.Channel <> 10 Then
                                pe = midie
                                cc(pe.Channel).InstrumentList.SelectedIndex = pe.Patch
                                instruments(pe.Channel) = pe.Patch
                            End If
                        Next
                    Next
                    For i = 0 To notecount(currentchannal) - 1
                        Panel.SetZIndex(note(currentchannal, i), 1)
                    Next
                    For i = 1 To 16
                        SortNotes(i)
                        init_play(i)
                    Next
                    setinstrument()
                    Backboard1.sv.ScrollToHorizontalOffset(0)
                    Filename.Content = Mid(OpenFileDialog2.SafeFileName, 1, Len(OpenFileDialog2.SafeFileName) - 3) + "gme*"
                End If
            End If
        End If
    End Sub

    Public Sub clearboard(channal As Integer)
        For i = 0 To notecount(channal) - 1
            Backboard1.mainboard.Children.Remove(note(channal, i))
            Backboard1.VolumeBoard1.mb.Children.Remove(volumeb(channal, i))
            note(channal, i) = Nothing
            volumeb(channal, i) = Nothing
        Next
        notecount(channal) = 0
    End Sub
    Public Sub SortNotes(channal As Integer)
        Dim tempnote As Notebar
        Dim tempvolumebar As VolumeBar
        For i = notecount(channal) - 1 To 1 Step -1
            For j = 0 To i - 1
                If note(channal, i).Margin.Left < note(channal, j).Margin.Left Then
                    tempnote = note(channal, i)
                    note(channal, i) = note(channal, j)
                    note(channal, j) = tempnote
                    tempvolumebar = volumeb(channal, i)
                    volumeb(channal, i) = volumeb(channal, j)
                    volumeb(channal, j) = tempvolumebar
                    note(channal, i).id = i
                    note(channal, j).id = j
                    volumeb(channal, i).id = i
                    volumeb(channal, j).id = j
                End If
            Next
        Next
    End Sub

    Private Sub StackPanel_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 1 To 16
            muted(i) = False
            solo(i) = False
            cc(i) = New ChannalCard
            channalboard.Children.Add(cc(i))
            cc(i).colorsign.Background = New SolidColorBrush(channalcolors(i))
            cc(i).channalnum.Content = CStr(i)
            cc(i).tb.Text = "channel" + CStr(i)
            cc(i).Form1 = Me
            cc(i).id = i
        Next
        cc(10).InstrumentList.Items.Clear()
        cc(10).InstrumentList.Items.Add("0 鼓组")
        cc(10).InstrumentList.SelectedIndex = 0
        cc(10).InstrumentList.IsEnabled = False
        cc(1).MainBoard.Background = New SolidColorBrush(Color.FromRgb(0, 122, 204))
    End Sub

    Public Sub init_play(channal As Integer)
        Dim tone As Integer
        Dim volume As Integer
        For i = 0 To 16383
            midicommandstring(channal, i) = ""
        Next
        For i = 0 To notecount(channal) - 1
            tone = Val(note(channal, i).lb.Content)
            volume = 100 - volumeb(channal, i).Margin.Top
            midicommandstring(channal, note(channal, i).Margin.Left \ 8) += tone.ToString("D3") + volume.ToString("D3")
            midicommandstring(channal, (note(channal, i).Margin.Left + note(channal, i).Width) \ 8) += tone.ToString("D3") + "000"
        Next
    End Sub

    Private Sub autopagebuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            If isautopage Then
                isautopage = False
                autopagestroke.Visibility = Visibility.Collapsed
            Else
                isautopage = True
                autopagestroke.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    Private Sub loopbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            If isloop Then
                isloop = False
                loopstroke.Visibility = Visibility.Collapsed
            Else
                isloop = True
                loopstroke.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    Private Sub exportmcfbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            If FolderBrowserDialog1.ShowDialog = Forms.DialogResult.OK Then
                Dim path As String = FolderBrowserDialog1.SelectedPath
                Dim writer As StreamWriter
                Dim command As String
                For channal = 1 To 16
                    If notecount(channal) <> 0 Then
                        SortNotes(channal)
                        writer = New StreamWriter(path + "\" + cc(channal).tb.Text + ".mcfunction")
                        If channal = 10 Then
                            For i = 0 To notecount(channal) - 1
                                command = "execute at @p[scores={tick=" + CStr(note(channal, i).Margin.Left \ 8) + "}] run playsound minecraft:agm0.l" + note(channal, i).lb.Content + " record @p ~ ~ ~ " + CStr((100 - volumeb(channal, i).Margin.Top) / 100)
                                writer.WriteLine(command)
                                If note(channal, i).lb.Content = 42 Then
                                    command = "execute at @p[scores={tick=" + CStr(note(channal, i).Margin.Left) + "}] run stopsound @p record minecraft:agm0.l46"
                                    writer.WriteLine(command)
                                End If
                            Next
                        Else
                            For i = 0 To notecount(channal) - 1
                                command = "execute at @p[scores={tick=" + CStr(note(channal, i).Margin.Left \ 8) + "}] run playsound minecraft:agm" + CStr(cc(channal).InstrumentList.SelectedIndex + 1) + ".l" + note(channal, i).lb.Content + " record @p ~ ~ ~ " + CStr((100 - volumeb(channal, i).Margin.Top) / 100)
                                writer.WriteLine(command)
                                command = "execute at @p[scores={tick=" + CStr((note(channal, i).Margin.Left + note(channal, i).Width) \ 8) + "}] run stopsound @p record minecraft:agm" + CStr(cc(channal).InstrumentList.SelectedIndex + 1) + ".l" + note(channal, i).lb.Content
                                writer.WriteLine(command)
                            Next
                        End If
                        writer.Flush()
                        writer.Close()
                    End If
                Next
                MsgBox("导出成功！")
            End If
        End If
    End Sub

    Private Sub playing()
        Backboard1.progressbar.Margin = New Thickness(tick * 8, 0, 0, 0)
        Backboard1.TickBar1.progressbar.Margin = New Thickness(tick * 8 - 3, 0, 0, 0)
        If isautopage AndAlso Backboard1.progressbar.Margin.Left > Backboard1.sv.HorizontalOffset + Backboard1.sv.ActualWidth Then
            Backboard1.sv.ScrollToHorizontalOffset(Backboard1.sv.HorizontalOffset + Backboard1.sv.ActualWidth)
        End If
        If Backboard1.progressbar.Margin.Left > Backboard1.mainboard.Width Then
            tick = 0
            Backboard1.progressbar.Margin = New Thickness(0, 0, 0, 0)
            Backboard1.TickBar1.progressbar.Margin = New Thickness(-3, 0, 0, 0)
            Backboard1.sv.ScrollToHorizontalOffset(0)
            If Not isloop Then
                timer2.Enabled = False
                pausebuttom.Visibility = Visibility.Collapsed
                playbuttom.Visibility = Visibility.Visible
            End If
        End If
    End Sub

    Private Sub DockPanel_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            midioutdevice.Reset()
            setinstrument()
        End If
    End Sub

    Private Sub Form1_KeyDown(sender As Object, e As KeyEventArgs)
        If e.Key = Key.Space Then
            If pausebuttom.Visibility = Visibility.Visible Then
                Timer1.Enabled = False
                timer2.Stop()
                midioutdevice.Reset()
                pausebuttom.Visibility = Visibility.Collapsed
                playbuttom.Visibility = Visibility.Visible
                setinstrument()
            Else
                setinstrument()
                Timer1.Enabled = True
                timer2.Start()
                playbuttom.Visibility = Visibility.Collapsed
                pausebuttom.Visibility = Visibility.Visible
            End If
        ElseIf e.Key = Key.Delete Then
            Dim i As Integer = 0
            Dim n As Integer = 0
            Do Until IsNothing(note(currentchannal, i))
                If note(currentchannal, i).choosed Then
                    DeleteNote(i, currentchannal)
                    i -= 1
                    n += 1
                End If
                i += 1
            Loop
            tip.Content = "删除了" + CStr(n) + "个音符"
            init_play(currentchannal)
        ElseIf e.Key = Key.A And Keyboard.IsKeyDown(Key.LeftCtrl) Then
            choosenote()
        ElseIf e.Key = Key.S And Keyboard.IsKeyDown(Key.LeftCtrl) Then
            savefile()
        ElseIf e.Key = Key.c And Keyboard.IsKeyDown(Key.LeftCtrl) Then
            copy()
        ElseIf e.Key = Key.v And Keyboard.IsKeyDown(Key.LeftCtrl) Then
            paste()
        ElseIf e.Key = Key.x And Keyboard.IsKeyDown(Key.LeftCtrl) Then
            copy()
            Dim i As Integer = 0
            Do Until IsNothing(note(currentchannal, i))
                If note(currentchannal, i).choosed Then
                    DeleteNote(i, currentchannal)
                    i -= 1
                End If
                i += 1
            Loop
            init_play(currentchannal)
        ElseIf e.Key = Key.D And Keyboard.IsKeyDown(Key.LeftCtrl) Then
            dischoosenote()
        End If
    End Sub

    Private Sub bordern_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            y1 = e.GetPosition(borders).Y
            h1 = Height
            t1 = Top
            sizing = True
        End If
    End Sub

    Private Sub bordern_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub bordern_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            y2 = e.GetPosition(borders).Y
            If h1 + (y1 - y2) >= 400 Then
                Height = h1 + (y1 - y2)
                Top = t1 - (Height - h1)
            End If
        End If
    End Sub

    Private Sub borders_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            y1 = e.GetPosition(borders).Y
            h1 = Height
            sizing = True
        End If
    End Sub

    Private Sub borders_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            y2 = e.GetPosition(borders).Y
            If Height + y2 - y1 >= 400 Then
                Height += y2 - y1
            End If
        End If
    End Sub

    Private Sub borders_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub borderw_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            x1 = e.GetPosition(bordere).X
            w1 = Width
            l1 = Left
            sizing = True
        End If
    End Sub

    Private Sub borderw_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub borderw_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            x2 = e.GetPosition(bordere).X
            If w1 + (x1 - x2) >= 600 Then
                Width = w1 + (x1 - x2)
                Left = l1 - (Width - w1)
            End If
        End If
    End Sub

    Private Sub bordere_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            x1 = e.GetPosition(bordere).X
            w1 = Width
            sizing = True
        End If
    End Sub

    Private Sub bordere_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            x2 = e.GetPosition(bordere).X
            If Width + x2 - x1 >= 600 Then
                Width += x2 - x1
            End If
        End If
    End Sub

    Private Sub bordere_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub bordernw_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            y1 = e.GetPosition(borders).Y
            h1 = Height
            t1 = Top
            x1 = e.GetPosition(bordere).X
            w1 = Width
            l1 = Left
            sizing = True
        End If
    End Sub

    Private Sub bordernw_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            y2 = e.GetPosition(borders).Y
            x2 = e.GetPosition(bordere).X
            If h1 + (y1 - y2) >= 400 Then
                Height = h1 + (y1 - y2)
                Top = t1 - (Height - h1)
            End If
            If w1 + (x1 - x2) >= 600 Then
                Width = w1 + (x1 - x2)
                Left = l1 - (Width - w1)
            End If
        End If
    End Sub

    Private Sub bordernw_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub borderne_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            y1 = e.GetPosition(borders).Y
            h1 = Height
            t1 = Top
            x1 = e.GetPosition(bordere).X
            w1 = Width
            sizing = True
        End If
    End Sub

    Private Sub borderne_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            y2 = e.GetPosition(borders).Y
            x2 = e.GetPosition(bordere).X
            If h1 + (y1 - y2) >= 400 Then
                Height = h1 + (y1 - y2)
                Top = t1 - (Height - h1)
            End If
            If Width + x2 - x1 >= 600 Then
                Width += x2 - x1
            End If
        End If
    End Sub

    Private Sub Form1_Deactivated(sender As Object, e As EventArgs)

    End Sub

    Private Sub Form1_Activated(sender As Object, e As EventArgs)

    End Sub

    Private Sub borderne_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub bordersw_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            y1 = e.GetPosition(borders).Y
            h1 = Height
            x1 = e.GetPosition(bordere).X
            w1 = Width
            l1 = Left
            sizing = True
        End If
    End Sub

    Private Sub bordersw_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            y2 = e.GetPosition(borders).Y
            x2 = e.GetPosition(bordere).X
            If Height + y2 - y1 >= 400 Then
                Height += y2 - y1
            End If
            If w1 + (x1 - x2) >= 600 Then
                Width = w1 + (x1 - x2)
                Left = l1 - (Width - w1)
            End If
        End If
    End Sub

    Private Sub bordersw_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub copybuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            copy()
        End If
    End Sub

    Private Sub cutbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            copy()
            Dim i As Integer = 0
            Do Until IsNothing(note(currentchannal, i))
                If note(currentchannal, i).choosed Then
                    DeleteNote(i, currentchannal)
                    i -= 1
                End If
                i += 1
            Loop
            init_play(currentchannal)
        End If
    End Sub

    Private Sub pastebuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            paste()
        End If
    End Sub

    Private Sub deletebuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            Dim i As Integer = 0
            Dim n As Integer = 0
            Do Until IsNothing(note(currentchannal, i))
                If note(currentchannal, i).choosed Then
                    DeleteNote(i, currentchannal)
                    i -= 1
                    n += 1
                End If
                i += 1
            Loop
            tip.Content = "删除了" + CStr(n) + "个音符"
            init_play(currentchannal)
        End If
    End Sub

    Private Sub borderse_MouseDown(sender As Object, e As MouseButtonEventArgs)
        If e.LeftButton = MouseButtonState.Pressed Then
            Mouse.Capture(sender)
            y1 = e.GetPosition(borders).Y
            h1 = Height
            x1 = e.GetPosition(bordere).X
            w1 = Width
            sizing = True
            sizing = True
        End If

    End Sub

    Private Sub informationbuttom_MouseUp(sender As Object, e As MouseButtonEventArgs)
        If e.GetPosition(sender).X >= 0 And e.GetPosition(sender).X < 32 And e.GetPosition(sender).Y >= 0 And e.GetPosition(sender).Y < 32 Then
            Dim target As DockPanel = sender
            target.Background = New SolidColorBrush(Color.FromArgb(255, 63, 63, 65))
            Dim iw As New InformationWindow
            iw.Show()
            iw.Owner = Me
            iw.form1 = Me
            IsEnabled = False
        End If
    End Sub

    Private Sub Form1_Closed(sender As Object, e As EventArgs)

    End Sub

    Private Sub borderse_MouseMove(sender As Object, e As MouseEventArgs)
        If e.LeftButton = MouseButtonState.Pressed And sizing Then
            y2 = e.GetPosition(borders).Y
            x2 = e.GetPosition(bordere).X
            If Height + y2 - y1 >= 400 Then
                Height += y2 - y1
            End If
            If Width + x2 - x1 >= 600 Then
                Width += x2 - x1
            End If
        End If
    End Sub

    Private Sub borderse_MouseUp(sender As Object, e As MouseButtonEventArgs)
        Mouse.Capture(Nothing)
        sizing = False
    End Sub

    Private Sub playing2()
        Dim midie As NoteEvent
        Dim sourcestr As String
        For channal = 1 To 16
            If Not muted(channal) Then
                sourcestr = midicommandstring(channal, tick)
                For i = 0 To (Len(sourcestr) \ 6) - 1
                    If Mid(midicommandstring(channal, tick), i * 6 + 4, 3) <> "000" Then
                        midie = New NoteEvent(0, channal, MidiCommandCode.NoteOn, Val(Mid(sourcestr, i * 6 + 1, 3)), Int(1.28 * Val(Mid(sourcestr, i * 6 + 4, 3)) - 1))
                    Else
                        midie = New NoteEvent(0, channal, MidiCommandCode.NoteOff, Val(Mid(sourcestr, i * 6 + 1, 3)), 64)
                    End If
                    midioutdevice.Send(midie.GetAsShortMessage)
                Next
            End If
        Next
        tick += 1
    End Sub
    Public Function GetBPM(midif As MidiFile) As Double
        Dim s As String
        Dim i As Integer
        For Each track In midif.Events
            For Each midie In track
                If midie.CommandCode = MidiCommandCode.MetaEvent Then
                    s = midie.ToString
                    If Mid(s, 1, 11) = "0 SetTempo " Then
                        For i = 12 To Len(s)
                            If Mid(s, i, 3) = "bpm" Then Exit For
                        Next
                        Return Val(Mid(s, 12, i - 12))
                    End If
                End If
            Next
        Next
        Return 150
    End Function
    Sub setcolor()
        channalcolors(1) = Color.FromRgb(229, 57, 57)
        channalcolors(2) = Color.FromRgb(204, 128, 102)
        channalcolors(3) = Color.FromRgb(255, 170, 1)
        channalcolors(4) = Color.FromRgb(194, 242, 0)
        channalcolors(5) = Color.FromRgb(51, 128, 0)
        channalcolors(6) = Color.FromRgb(61, 243, 231)
        channalcolors(7) = Color.FromRgb(4, 83, 124)
        channalcolors(8) = Color.FromRgb(1, 0, 140)
        channalcolors(9) = Color.FromRgb(123, 64, 128)
        channalcolors(10) = Color.FromRgb(204, 0, 136)
        channalcolors(11) = Color.FromRgb(191, 143, 143)
        channalcolors(12) = Color.FromRgb(255, 102, 0)
        channalcolors(13) = Color.FromRgb(191, 175, 142)
        channalcolors(14) = Color.FromRgb(27, 51, 1)
        channalcolors(15) = Color.FromRgb(144, 255, 127)
        channalcolors(16) = Color.FromRgb(44, 152, 180)
    End Sub
    Public Sub setinstrument()
        Dim pe As PatchChangeEvent
        For i = 1 To 16
            If i <> 10 Then
                pe = New PatchChangeEvent(0, i, instruments(i))
                midioutdevice.Send(pe.GetAsShortMessage)
            End If
        Next
    End Sub
    Public Sub edit()
        If Mid(Filename.Content, Len(Filename.Content), 1) <> "*" And Filename.Content <> "就绪" Then
            Filename.Content += "*"
        End If
    End Sub
    Function getsafefilename(s As String)
        Dim i As Integer
        For i = Len(s) To 1 Step -1
            If Mid(s, i, 1) = "\" Or Mid(s, i, 1) = "/" Then
                Exit For
            End If
        Next
        Return Mid(s, i + 1)
    End Function
    Public Sub savefile()
        If Mid(Filename.Content, Len(Filename.Content), 1) = "*" Then
            If Mid(Filename.Content, 1, Len(Filename.Content) - 1) <> OpenFileDialog1.SafeFileName Then
                SaveFileDialog1.FileName = Mid(Filename.Content, 1, Len(Filename.Content) - 1)
                If SaveFileDialog1.ShowDialog = Forms.DialogResult.OK Then
                    Dim fs As New FileStream(SaveFileDialog1.FileName, FileMode.Create)
                    Dim writer As New BinaryWriter(fs)
                    Dim data As UInteger
                    For channal = 1 To 16
                        writer.Write(cc(channal).InstrumentList.SelectedIndex)
                        For i = 0 To notecount(channal) - 1
                            data = note(channal, i).Margin.Left \ 8
                            writer.Write(data)
                            data = note(channal, i).Width \ 8
                            data *= 65536
                            data += Val(note(channal, i).lb.Content) * 256 + 100 - volumeb(channal, i).Margin.Top
                            writer.Write(data)
                        Next
                        writer.Write(32767)
                    Next
                    writer.Flush()
                    writer.Close()
                    Dim writer2 As New StreamWriter(SaveFileDialog1.FileName, True)
                    writer2.WriteLine(" ")
                    writer2.WriteLine("endofnoteevents")
                    writer2.WriteLine("fileversion=0")
                    writer2.WriteLine(CStr(tps))
                    For i = 1 To 16
                        writer2.WriteLine(cc(i).tb.Text)
                    Next
                    writer2.Flush()
                    writer2.Close()
                    MsgBox("保存成功！")
                    Filename.Content = getsafefilename(SaveFileDialog1.FileName)
                    OpenFileDialog1.FileName = SaveFileDialog1.FileName
                End If
            Else
                SaveFileDialog1.FileName = OpenFileDialog1.FileName
                Dim fs As New FileStream(SaveFileDialog1.FileName, FileMode.Create)
                Dim writer As New BinaryWriter(fs)
                Dim data As UInteger
                For channal = 1 To 16
                    writer.Write(cc(channal).InstrumentList.SelectedIndex)
                    For i = 0 To notecount(channal) - 1
                        data = note(channal, i).Margin.Left \ 8
                        writer.Write(data)
                        data = note(channal, i).Width \ 8
                        data *= 65536
                        data += Val(note(channal, i).lb.Content) * 256 + 100 - volumeb(channal, i).Margin.Top
                        writer.Write(data)
                    Next
                    writer.Write(32767)
                Next
                writer.Flush()
                writer.Close()
                Dim writer2 As New StreamWriter(SaveFileDialog1.FileName, True)
                writer2.WriteLine(" ")
                writer2.WriteLine("endofnoteevents")
                writer2.WriteLine("fileversion=0")
                writer2.WriteLine(CStr(tps))
                For i = 1 To 16
                    writer2.WriteLine(cc(i).tb.Text)
                Next
                writer2.Flush()
                writer2.Close()
                MsgBox("保存成功！")
                Filename.Content = getsafefilename(SaveFileDialog1.FileName)
                OpenFileDialog1.FileName = SaveFileDialog1.FileName
            End If
        Else
        End If
    End Sub
    Public Sub copy()
        SortNotes(currentchannal)
        Dim starttick As Integer = 32767
        Dim data As String = "GMEditdata"
        Dim tick2, tone, length, volume As Integer
        Dim n As Integer = 0
        For i = 0 To notecount(currentchannal) - 1
            If note(currentchannal, i).choosed Then
                If starttick = 32767 Then
                    starttick = note(currentchannal, i).Margin.Left \ 8
                End If
                tick2 = note(currentchannal, i).Margin.Left \ 8 - starttick
                tone = Val(note(currentchannal, i).lb.Content)
                length = note(currentchannal, i).Width \ 8
                volume = 100 - volumeb(currentchannal, i).Margin.Top
                data += tick2.ToString("D5") + tone.ToString("D3") + length.ToString("D3") + volume.ToString("D3")
                n += 1
            End If
        Next
        Clipboard.SetText(data)
        tip.Content = "复制了" + CStr(n) + "个音符"
    End Sub
    Public Sub paste()
        Dim data As String = Clipboard.GetText
        Dim n As Integer = 0
        Dim sd As String
        If Mid(data, 1, 10) = "GMEditdata" Then
            Dim tick2, tone, length, volume As Integer
            For n = 0 To (Len(data) - 10) \ 14 - 1
                sd = Mid(data, n * 14 + 11, 14)
                tick2 = Val(Mid(sd, 1, 5))
                tone = Val(Mid(sd, 6, 3))
                length = Val(Mid(sd, 9, 3))
                volume = Val(Mid(sd, 12, 3))
                CreateNote(tone, tick + tick2, length, volume, currentchannal)
                choosenote(notecount(currentchannal) - 1)
            Next
        End If
        tip.Content = "粘贴了" + CStr(n) + "个音符"
        init_play(currentchannal)
    End Sub
End Class
