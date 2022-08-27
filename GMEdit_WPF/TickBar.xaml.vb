Public Class TickBar
    Dim lb(1023) As Label
    Private Sub UserControl_Loaded(sender As Object, e As RoutedEventArgs)
        For i = 0 To 1023
            lb(i) = New Label
            With lb(i)
                .Content = CStr(i * 16)
                .HorizontalAlignment = HorizontalAlignment.Left
                .VerticalAlignment = VerticalAlignment.Bottom
                .Margin = New Thickness(i * 128, 0, 0, 0)
                If i Mod 4 = 0 Then
                    .Foreground = New SolidColorBrush(Color.FromArgb(255, 128, 128, 128))
                Else
                    .Foreground = New SolidColorBrush(Color.FromArgb(255, 96, 96, 96))
                End If
                .FontSize = 16
            End With
            mb.Children.Add(lb(i))
        Next
        progressbar.Margin = New Thickness(-3, 0, 0, 0)
    End Sub
End Class
