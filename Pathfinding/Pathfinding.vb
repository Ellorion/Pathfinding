Imports Pathfinding.GridItem

Public MustInherit Class Pathfinding
    Implements IPathfinding

    Public Enum PathMessageType
        PathFound
        PathBlocked
        PathError
        Running
    End Enum

    Public Enum Direction
        Left
        Top
        Right
        Bottom

        TopLeft
        TopRight
        BottomLeft
        BottomRight
    End Enum

    Protected lstGridItem As List(Of GridItem) = Nothing
    Protected bRunning As Boolean = False
    Protected myColumnCount As Integer = 0
    Protected myRowCount As Integer = 0
    Protected bDebugMode As Boolean = False

    Public Event GridItemValueChanged() Implements IPathfinding.GridItemValueChanged

    Public ReadOnly Property ColumnCount As Integer Implements IPathfinding.ColumnCount
        Get
            Return myColumnCount
        End Get
    End Property

    Public ReadOnly Property RowCount As Integer Implements IPathfinding.RowCount
        Get
            Return myRowCount
        End Get
    End Property

    Public Property StartPoint As Point = New Point(-1, -1) Implements IPathfinding.StartPoint
    Public Property StopPoint As Point = New Point(-1, -1) Implements IPathfinding.StopPoint

    Public Sub Init(columnCount As Integer, rowCount As Integer, lstGridItem As List(Of GridItem)) Implements IPathfinding.Init
        myColumnCount = columnCount
        myRowCount = rowCount

        Me.lstGridItem = lstGridItem
    End Sub

    Protected Enum GridValueReturnType
        DoesNotExists
        ValueUpdated
        Unchanged
        IsWall
    End Enum

    Public Property DriveDiagonal As Boolean = False Implements IPathfinding.DriveDiagonal
    Public Property AvoidRotations As Boolean = False Implements IPathfinding.AvoidRotations

    Public ReadOnly Property isRunning As Boolean Implements IPathfinding.isRunning
        Get
            Return bRunning
        End Get
    End Property

    Public Property DebugMode As Boolean Implements IPathfinding.DebugMode
        Get
            Return bDebugMode
        End Get

        Set(value As Boolean)
            If bDebugMode = value Then
                Return
            End If

            For Each myItem As GridItem In lstGridItem
                myItem.DebugMode = value
            Next

            bDebugMode = value

            RaiseEvent GridItemValueChanged()
        End Set
    End Property

    Public MustOverride Function FindPath(ByRef lstPathPoints As List(Of List(Of PathPoint))) As PathMessageType Implements IPathfinding.FindPath

    Protected Function GetNeighborPoint(curPoint As Point, newDirection As Direction) As Point
        Dim newPoint As Point = Nothing

        Select Case newDirection
            Case Direction.Left
                newPoint = New Point(curPoint.X - 1, curPoint.Y)
            Case Direction.Top
                newPoint = New Point(curPoint.X, curPoint.Y - 1)
            Case Direction.Right
                newPoint = New Point(curPoint.X + 1, curPoint.Y)
            Case Direction.Bottom
                newPoint = New Point(curPoint.X, curPoint.Y + 1)
            Case Direction.TopLeft
                newPoint = New Point(curPoint.X - 1, curPoint.Y - 1)
            Case Direction.TopRight
                newPoint = New Point(curPoint.X + 1, curPoint.Y - 1)
            Case Direction.BottomLeft
                newPoint = New Point(curPoint.X - 1, curPoint.Y + 1)
            Case Direction.BottomRight
                newPoint = New Point(curPoint.X + 1, curPoint.Y + 1)
        End Select

        Return newPoint
    End Function

    Protected Function inArea(newPoint As Point) As Boolean
        If newPoint.X >= 0 And newPoint.Y >= 0 And newPoint.X < ColumnCount And newPoint.Y < RowCount Then
            Return True
        End If

        Return False
    End Function

    Protected Function isWall(newPoint As Point, newDirection As Direction) As Boolean
        ' check boundary
        If Not inArea(newPoint) Then Return True

        Dim index As Integer = GridView.GetIndex(GetNeighborPoint(newPoint, newDirection), ColumnCount)

        If index >= lstGridItem.Count Or index < 0 Then
            Return True
        End If

        Dim leftItem As GridItem = lstGridItem.Item(index)
        If leftItem.GetItemType() = GridItemType.WallItem Then
            Return True
        End If

        Return False
    End Function

    Protected Function SetValueOfSurroundingBlock(curItem As GridItem, curPoint As Point, newDirection As Direction, ByRef lstBlocksWithNewValues As List(Of Point)) As GridValueReturnType
        Dim neighborPoint As Point = GetNeighborPoint(curPoint, newDirection)

        ' neighbor griditem on map?
        If Not inArea(neighborPoint) Then
            Return GridValueReturnType.DoesNotExists
        End If

        Dim nextItem As GridItem = lstGridItem.Item(GridView.GetIndex(neighborPoint.X, neighborPoint.Y, myColumnCount))

        If nextItem.value < 0.0 And nextItem.GetItemType() <> GridItemType.WallItem Then
            If newDirection = Direction.Left Or newDirection = Direction.Top Or newDirection = Direction.Right Or newDirection = Direction.Bottom Then
                ' horizinal & vertical
                nextItem.value_rel = 1
            Else
                ' diagonal blocks
                Dim factor As Integer = 1
                nextItem.value_rel = Math.Sqrt(factor * factor + factor * factor)
            End If

            nextItem.value = curItem.value + nextItem.value_rel

            lstBlocksWithNewValues.Add(neighborPoint)

            Return GridValueReturnType.ValueUpdated
        End If

        If nextItem.GetItemType() = GridItemType.WallItem Then
            Return GridValueReturnType.IsWall
        End If

        Return GridValueReturnType.Unchanged
    End Function

    Public Sub UpdateGridList(lstGridItem As List(Of GridItem)) Implements IPathfinding.UpdateGridList
        Me.lstGridItem = lstGridItem
    End Sub

    ''' <summary>
    ''' sets values for each available block on the map
    ''' </summary>
    Protected Sub ComputeBlocksValues()
        Dim lstBlocksWithNewValue As New List(Of Point)
        Dim lstUpdatedDirection As New List(Of Direction)

        Dim lstDirectionsBase As New List(Of Direction)
        lstDirectionsBase.Add(Direction.Left)
        lstDirectionsBase.Add(Direction.Top)
        lstDirectionsBase.Add(Direction.Right)
        lstDirectionsBase.Add(Direction.Bottom)

        lstBlocksWithNewValue.Add(New Point(StartPoint.X, StartPoint.Y))

        ResetValues()

        ' while there are still points needed to be checked
        While (lstBlocksWithNewValue.Count > 0)
            Dim curPoint As Point = lstBlocksWithNewValue.Item(0) ' pick first item in list

            ' delete point from list (will be handled now)
            lstBlocksWithNewValue.Remove(curPoint)

            Dim myItem As GridItem = lstGridItem(GridView.GetIndex(curPoint, ColumnCount))

            ' start with startItem, then check every neighbor that got a value
            If myItem.value >= 0.0 Then
                Dim bAnyUpdated As Boolean = False

                For Each myDirection As Direction In lstDirectionsBase
                    Dim return_type As GridValueReturnType = SetValueOfSurroundingBlock(myItem, curPoint, myDirection, lstBlocksWithNewValue)

                    Select Case return_type
                        Case GridValueReturnType.ValueUpdated
                            lstUpdatedDirection.Add(myDirection)
                    End Select
                Next

                If DriveDiagonal Then
                    If SetValueOfSurroundingBlock(myItem, curPoint, Direction.TopLeft, lstBlocksWithNewValue) = GridValueReturnType.ValueUpdated Then lstUpdatedDirection.Add(Direction.TopLeft)
                    If SetValueOfSurroundingBlock(myItem, curPoint, Direction.TopRight, lstBlocksWithNewValue) = GridValueReturnType.ValueUpdated Then lstUpdatedDirection.Add(Direction.TopRight)
                    If SetValueOfSurroundingBlock(myItem, curPoint, Direction.BottomLeft, lstBlocksWithNewValue) = GridValueReturnType.ValueUpdated Then lstUpdatedDirection.Add(Direction.BottomLeft)
                    If SetValueOfSurroundingBlock(myItem, curPoint, Direction.BottomRight, lstBlocksWithNewValue) = GridValueReturnType.ValueUpdated Then lstUpdatedDirection.Add(Direction.BottomRight)
                End If

                'If lstUpdatedDirection.Count > 0 Then
                lstUpdatedDirection.Clear()
                'End If
            End If
        End While
    End Sub

    Protected Function SaveBestPoint(curPathPoint As PathPoint, newDirection As Direction, ByRef minValue As Single, ByRef lstPossiblePoints As List(Of PathPoint)) As Boolean
        Dim lstDeleteableValues As New List(Of PathPoint)
        Dim newValue As Single = GetGridValue(GetNeighborPoint(curPathPoint.Point, newDirection))

        ' if diag -> check other 2 sides, if at least one is a wall -> don't save it!
        Select Case newDirection
            Case Direction.TopLeft
                If isWall(curPathPoint.Point, Direction.Top) Then Return 0.0
                If isWall(curPathPoint.Point, Direction.Left) Then Return 0.0
            Case Direction.TopRight
                If isWall(curPathPoint.Point, Direction.Top) Then Return 0.0
                If isWall(curPathPoint.Point, Direction.Right) Then Return 0.0
            Case Direction.BottomLeft
                If isWall(curPathPoint.Point, Direction.Bottom) Then Return 0.0
                If isWall(curPathPoint.Point, Direction.Left) Then Return 0.0
            Case Direction.BottomRight
                If isWall(curPathPoint.Point, Direction.Bottom) Then Return 0.0
                If isWall(curPathPoint.Point, Direction.Right) Then Return 0.0
        End Select

        ' better value found?
        If newValue <= curPathPoint.EstimationValue And newValue <= minValue Then
            ' mark old bad values as deleteable
            For Each myPoint As PathPoint In lstPossiblePoints
                If GetGridValue(myPoint.Point) > newValue Then
                    lstDeleteableValues.Add(myPoint)
                End If
            Next

            ' and remove them
            For Each myPoint As PathPoint In lstDeleteableValues
                lstPossiblePoints.Remove(myPoint)
            Next

            lstDeleteableValues.Clear()

            ' store the good values
            Dim myPathPoint As New PathPoint
            myPathPoint.Point = GetNeighborPoint(curPathPoint.Point, newDirection)
            myPathPoint.EstimationValue = newValue
            myPathPoint.Direction = newDirection

            lstPossiblePoints.Add(myPathPoint)
        End If

        minValue = Math.Min(newValue, minValue)

        Return True
    End Function

    Protected Sub ResetValues()
        For Each myItem As GridItem In lstGridItem
            myItem.SetItemType(myItem.GetItemType(), True)
        Next
    End Sub

    Protected Function GetGridValue(newPoint As Point) As Single
        Return GetGridValue(newPoint.X, newPoint.Y)
    End Function

    Protected Function GetGridValue(x As Integer, y As Integer) As Single
        ' coordinates are in the grid?
        If x >= 0 And y >= 0 And x < ColumnCount And y < RowCount Then
            Dim value As Single = lstGridItem(GridView.GetIndex(x, y, ColumnCount)).value()

            If value >= 0.0 Then Return value
        End If

        ' return high value, if it doesn't exists, so it won't be chosen as a good path
        Return lstGridItem(GridView.GetIndex(StopPoint.X, StopPoint.Y, ColumnCount)).value + 1
    End Function

    Protected Sub RaiseGridItemValueChangedEvent()
        RaiseEvent GridItemValueChanged()
    End Sub

    Public Shared Function CountRotations(lstPathPoints As List(Of PathPoint)) As Integer
        Dim numRotation As Integer = 0

        If lstPathPoints.Count = 0 Then
            Return numRotation
        End If

        Dim lastDirection As Direction = lstPathPoints.Item(0).Direction

        For Each myPathPoint As PathPoint In lstPathPoints
            If lastDirection <> myPathPoint.Direction Then
                numRotation += 1
            End If

            lastDirection = myPathPoint.Direction
        Next

        Return numRotation
    End Function

    Public Shared Function CountRotations(lstPathPoints As List(Of List(Of PathPoint))) As Integer
        If lstPathPoints.Count = 1 Then
            Return CountRotations(lstPathPoints.Item(0))
        End If

        Return 0
    End Function
End Class
