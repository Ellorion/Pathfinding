''' <summary>
''' Contains data of a GridItem
''' </summary>
''' <remarks></remarks>

Public Class GridItem
    Dim bmpGridItem As Bitmap = Nothing
    Dim gridSize As Integer

    Dim penStart As Pen = Pens.Green
    Dim penStop As Pen = Pens.Red
    Dim penDefault As Pen = Pens.Black
    Dim penPath As Pen = Pens.Blue
    Dim brushWall As Brush

    Dim type As GridItemType
    Dim myGridValue As Integer
    Dim myStepValue As Integer
    Dim bDebugMode As Boolean

    Public Enum GridItemType
        DefaultItem
        StartItem
        StopItem
        WallItem
        PathItem
    End Enum

    Public Property DebugMode As Boolean
        Get
            Return bDebugMode
        End Get

        Set(value As Boolean)

            If value Then
                brushWall = Brushes.Yellow
            Else
                brushWall = Brushes.Black
            End If

            bDebugMode = value

            DrawGridItem()
        End Set
    End Property

    ''' <summary>
    '''  absolute value of a point (used for pathfinding)
    ''' </summary>
    Property GridValue As Integer
        Get
            Return myGridValue
        End Get

        Set(value As Integer)
            myGridValue = value
        End Set
    End Property

    ''' <summary>
    ''' relative value of a point (used for estimation)
    ''' </summary>
    Property StepValue As Integer
        Get
            Return myStepValue
        End Get

        Set(value As Integer)
            myStepValue = value
        End Set
    End Property


    Sub New(newGridSize As Integer, newDebugMode As Boolean)
        gridSize = newGridSize

        DebugMode = newDebugMode

        SetItemType(GridItemType.DefaultItem)
    End Sub

    Public Function GetItem() As Bitmap
        If bmpGridItem Is Nothing Then
            DrawGridItem()
        End If

        Return bmpGridItem
    End Function

    Public Sub SetItemType(newType As GridItemType)
        SetItemType(newType, False)
    End Sub

    Public Sub SetItemType(newType As GridItemType, reseting As Boolean)
        If Not reseting Then
            If newType = GridItemType.WallItem And GetItemType() = GridItemType.WallItem Then
                newType = GridItemType.DefaultItem
            End If
        Else
            If newType = GridItemType.PathItem Then
                newType = GridItemType.DefaultItem
            End If
        End If

        type = newType

        Select Case type
            Case GridItemType.DefaultItem
                myGridValue = -1
            Case GridItemType.StartItem
                myGridValue = 0
            Case GridItemType.StopItem
                myGridValue = -2
            Case GridItemType.WallItem
                myGridValue = -3
        End Select

        ' "mark" for update at next drawing request
        bmpGridItem = Nothing
    End Sub

    Public Function GetItemType() As GridItemType
        Return Me.type
    End Function

    Private Function DrawGridItem() As Bitmap
        Dim tmpGridItem As Bitmap = New Bitmap(gridSize, gridSize)
        Dim myFont As Font = New Font(FontFamily.GenericMonospace, 10)
        Dim currentPen As Pen = penDefault
        Dim borderSize As Integer = 1

        Select Case type
            Case GridItemType.StartItem
                currentPen = penStart
                borderSize = 4
            Case GridItemType.StopItem
                currentPen = penStop
                borderSize = 4
            Case GridItemType.WallItem
                borderSize = 4
            Case GridItemType.PathItem
                currentPen = penPath
                borderSize = 4
        End Select

        Using g As Graphics = Graphics.FromImage(tmpGridItem)
            For i As Integer = 0 To borderSize - 1
                If type = GridItemType.WallItem Then
                    g.FillRectangle(brushWall, 0, 0, gridSize, gridSize)
                Else
                    g.DrawRectangle(currentPen, 0 + i, 0 + i, gridSize - 1 - (i * 2), gridSize - 1 - (i * 2))
                End If
            Next

            If (myGridValue > 0 And DebugMode = False) Or DebugMode = True Then
                g.DrawString(Math.Round(myGridValue, 2).ToString, myFont, Brushes.Black, 2, 2)
            End If
        End Using

        bmpGridItem = tmpGridItem

        Return tmpGridItem
    End Function
End Class