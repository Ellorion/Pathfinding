Public Class RandomGen
    Private Shared rnd As New Random

    Public Shared Function nextInt(min As Integer, max As Integer) As Integer
        Return rnd.Next(min, max)
    End Function
End Class