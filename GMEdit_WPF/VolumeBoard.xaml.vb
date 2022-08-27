Public Class VolumeBoard
    Dim vline(4095) As Line
    Dim i As Integer
    Dim firstload As Boolean = True
    Dim secondload As Boolean = True
    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 0 To 4095
            vline(i) = New Line
            With vline(i)
                .X1 = (i + 1) * 32
                .X2 = .X1
                .Y1 = 0
                .Y2 = 99
                .StrokeThickness = 1
                If i Mod 4 = 3 Then
                    .Stroke = New SolidColorBrush(Color.FromArgb(255, 160, 160, 160))
                Else
                    .Stroke = New SolidColorBrush(Color.FromArgb(255, 117, 117, 117))
                End If
            End With
            mb.Children.Add(vline(i))
        Next
    End Sub

    Private Sub UserControl_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        'If firstload Then
        '    firstload = False
        'ElseIf secondload Then
        '    secondload = False
        'Else
        '    For i = i To ActualWidth \ 32
        '        vline(i) = New Line
        '        With vline(i)
        '            .X1 = (i + 1) * 32
        '            .X2 = .X1
        '            .Y1 = 0
        '            .Y2 = 99
        '            .StrokeThickness = 1
        '            If i Mod 4 = 3 Then
        '                .Stroke = New SolidColorBrush(Color.FromArgb(255, 160, 160, 160))
        '            Else
        '                .Stroke = New SolidColorBrush(Color.FromArgb(255, 117, 117, 117))
        '            End If
        '        End With
        '        mb.Children.Add(vline(i))
        '    Next
        'End If
    End Sub
End Class
