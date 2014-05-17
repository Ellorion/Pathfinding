Imports Pathfinding.GridItem

Public Class GridView
    Private lstGridItem As List(Of GridItem) = New List(Of GridItem)
    Private iGridSize As Integer = 32
    Private grid As Bitmap = Nothing
    Private myColumnCount As Integer = 0
    Private myRowCount As Integer = 0

    Private startPoint As New Point(-1, -1)
    Private stopPoint As New Point(-1, -1)

    Public Event GridChanged(grid As Bitmap)

    Private WithEvents myPathfinding As IPathfinding = Nothing

    Sub New(ByRef pf As IPathfinding)
        SwitchPathfinding(pf)
    End Sub

    Private ReadOnly Property ColumnCount As Integer
        Get
            Return myColumnCount
        End Get
    End Property
    Private ReadOnly Property RowCount As Integer
        Get
            Return myRowCount
        End Get
    End Property

    Public Property GridSize As Integer
        Get
            Return iGridSize
        End Get
        Set(value As Integer)
            If iGridSize = value Then
                Return
            End If

            iGridSize = value

            CreateGrid(ColumnCount, RowCount)
        End Set
    End Property

    Public Sub SetTypePosition(newType As GridItemType, x As Integer, y As Integer)
        Dim item_x As Integer = x 'Math.Floor(x / GridSize)
        Dim item_y As Integer = y 'Math.Floor(y / GridSize)
        Dim posIndex As Integer = GetIndex(item_x, item_y, ColumnCount)

        If item_x > ColumnCount - 1 Then
            Return
        End If

        If item_y > RowCount - 1 Then
            Return
        End If

        ' type for new object
        Select Case newType
            Case GridItemType.StartItem
                If startPoint.X >= 0 And startPoint.Y >= 0 Then
                    lstGridItem.Item(GetIndex(startPoint.X, startPoint.Y, ColumnCount)).SetItemType(GridItem.GridItemType.DefaultItem)
                End If

                startPoint = New Point(item_x, item_y)
                ' overwrite overlapping points
                If stopPoint = startPoint Then
                    stopPoint = New Point(-1, -1)
                End If

            Case GridItemType.StopItem
                If stopPoint.X >= 0 And stopPoint.Y >= 0 Then
                    lstGridItem.Item(GetIndex(stopPoint.X, stopPoint.Y, ColumnCount)).SetItemType(GridItem.GridItemType.DefaultItem)
                End If

                stopPoint = New Point(item_x, item_y)
                ' overwrite overlapping points
                If stopPoint = startPoint Then
                    startPoint = New Point(-1, -1)
                End If

            Case GridItemType.WallItem
                ' don't cancel this

            Case Else
                Exit Sub
        End Select

        ' update points in pathfinding too
        If Not myPathfinding Is Nothing Then
            myPathfinding.StartPoint = startPoint
            myPathfinding.StopPoint = stopPoint
        End If

        lstGridItem.Item(posIndex).SetItemType(newType)

        myPathfinding.UpdateGridList(lstGridItem)

        RaiseEvent GridChanged(GetGrid())
    End Sub

    ''' <summary>
    ''' creates a grid
    ''' </summary>
    Public Function CreateGrid(columns As Integer, rows As Integer) As Bitmap
        If Not grid Is Nothing Then
            grid.Dispose()
        End If

        If columns < 1 Then columns = 1
        If rows < 1 Then rows = 1

        Dim tmpGrid As Bitmap = New Bitmap(columns * GridSize * 2, rows * GridSize * 2)

        Using g As Graphics = Graphics.FromImage(tmpGrid)
            g.FillRectangle(Brushes.White, 0, 0, columns * GridSize, rows * GridSize)
        End Using

        If Not grid Is Nothing Then
            grid.Dispose()
        End If

        grid = tmpGrid

        myColumnCount = columns
        myRowCount = rows

        lstGridItem.Clear()

        ' generate griditems for current grid
        For i As Integer = 0 To (columns * rows - 1)
            lstGridItem.Add(New GridItem(GridSize, myPathfinding.DebugMode))
        Next

        ' (re)init pathfinding with new grid data
        If Not myPathfinding Is Nothing Then
            myPathfinding.Init(ColumnCount, RowCount, lstGridItem)
        End If

        RaiseEvent GridChanged(GetGrid())

        Return tmpGrid
    End Function

    Private Function GetGrid() As Bitmap
        If grid Is Nothing Then
            CreateGrid(0, 0)
        End If

        Return DrawGrid()
    End Function

    Private Function DrawGrid() As Bitmap
        Dim tmpGrid As Bitmap = grid.Clone()
        Dim g As Graphics = Graphics.FromImage(tmpGrid)

        For y As Integer = 0 To RowCount - 1
            For x As Integer = 0 To ColumnCount - 1
                g.DrawImage(lstGridItem.Item(GetIndex(x, y, ColumnCount)).GetItem(), x * GridSize, y * GridSize)
            Next
        Next

        g.Dispose()

        Return tmpGrid
    End Function

    Public Shared Function GetIndex(newPoint As Point, ColumnCount As Integer) As Integer
        Return GetIndex(newPoint.X, newPoint.Y, columnCount)
    End Function

    Public Shared Function GetIndex(x As Integer, y As Integer, ColumnCount As Integer) As Integer
        Return y * columnCount + x
    End Function

    Private Sub myPathfinding_GridItemValueChanged() Handles myPathfinding.GridItemValueChanged
        RaiseEvent GridChanged(GetGrid())
    End Sub

    Public Sub SwitchPathfinding(pf As IPathfinding)
        myPathfinding = pf

        myPathfinding.StartPoint = startPoint
        myPathfinding.StopPoint = stopPoint
        myPathfinding.Init(ColumnCount, RowCount, lstGridItem)
    End Sub
End Class
