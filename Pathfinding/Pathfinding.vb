Imports Pathfinding.GridItem

''' <summary>
''' Handles all about Pathfinding incl. display
''' </summary>
''' <remarks></remarks>
Public Class Pathfinding
    Private lstGridItem As List(Of GridItem) = Nothing
    Private bRunning As Boolean = False
    Private myColumnCount As Integer = 0
    Private myRowCount As Integer = 0
    Private bDebugMode As Boolean = False

    Public Event GridItemValueChanged()

    Public ReadOnly Property ColumnCount As Integer
        Get
            Return myColumnCount
        End Get
    End Property

    Public ReadOnly Property RowCount As Integer
        Get
            Return myRowCount
        End Get
    End Property

    Public Property StartPoint As Point = New Point(-1, -1)
    Public Property StopPoint As Point = New Point(-1, -1)

    Public Sub Init(columnCount As Integer, rowCount As Integer, lstGridItem As List(Of GridItem))
        myColumnCount = columnCount
        myRowCount = rowCount

        Me.lstGridItem = lstGridItem
    End Sub

    Public Enum PathMessageType
        PathFound
        PathBlocked
        PathError
        Running
    End Enum

    Private Enum GridValueReturnType
        DoesNotExists
        ValueUpdated
        Unchanged
        IsWall
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

    Public Property DriveDiagonal As Boolean = False
    Public Property UseAnimation As Boolean = False
    Public Property AvoidRotations As Boolean = False

    Public ReadOnly Property isRunning As Boolean
        Get
            Return bRunning
        End Get
    End Property

    Public Property DebugMode As Boolean
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

    Private Function GetNeighborPoint(curPoint As Point, newDirection As Direction) As Point
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

    Private Function isWall(newPoint As Point, newDirection As Direction) As Boolean
        ' check boundary
        If newPoint.X < 0 Or newPoint.Y < 0 Or newPoint.X > ColumnCount And newPoint.Y > RowCount Then
            Return True
        End If

        Dim index As Integer = GridView.GetIndex(GetNeighborPoint(newPoint, newDirection), ColumnCount)

        If index >= lstGridItem.Count Or index < 0 Then
            Return False
        End If

        Dim leftItem As GridItem = lstGridItem.Item(index)
        If leftItem.GetItemType() = GridItemType.WallItem Then
            Return True
        End If

        Return False
    End Function

    Private Function SetValueOfSurroundingBlock(curItem As GridItem, curPoint As Point, newDirection As Direction, ByRef lstBlocksWithNewValues As List(Of Point)) As GridValueReturnType
        Dim neightborPoint As Point = GetNeighborPoint(curPoint, newDirection)

        ' neighbor griditem on map?
        If neightborPoint.X >= 0 And neightborPoint.Y >= 0 And neightborPoint.X < ColumnCount And neightborPoint.Y < RowCount Then
            Dim nextItem As GridItem = lstGridItem.Item(GridView.GetIndex(neightborPoint.X, neightborPoint.Y, myColumnCount))

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

                lstBlocksWithNewValues.Add(neightborPoint)

                Return GridValueReturnType.ValueUpdated
            End If

            If nextItem.GetItemType() = GridItemType.WallItem Then
                Return GridValueReturnType.IsWall
            End If

            Return GridValueReturnType.Unchanged
        End If

        Return GridValueReturnType.DoesNotExists
    End Function

    Public Sub UpdateGridList(lstGridItem As List(Of GridItem))
        Me.lstGridItem = lstGridItem
    End Sub

    ''' <summary>
    ''' sets values for each available block on the map
    ''' </summary>
    Private Sub ComputeBlocksValues()
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

                If lstUpdatedDirection.Count > 0 Then
                    If UseAnimation Then
                        ' update the gui
                        RaiseEvent GridItemValueChanged()
                        Application.DoEvents()
                    End If

                    lstUpdatedDirection.Clear()
                End If
            End If
        End While
    End Sub

    Private Function saveBestPoint(curPathPoint As PathPoint, newDirection As Direction, ByRef minValue As Single, ByRef lstPossiblePoints As List(Of PathPoint)) As Boolean
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

    Public Function FindPath(Optional ByRef lstPathPoints As List(Of PathPoint) = Nothing) As PathMessageType
        Dim lstPathPointsTemp As New List(Of PathPoint)
        Dim returnValue As PathMessageType = PathMessageType.Running

        Dim lstDirections As New List(Of Direction)
        lstDirections.Add(Direction.Left)
        lstDirections.Add(Direction.Top)
        lstDirections.Add(Direction.Right)
        lstDirections.Add(Direction.Bottom)

        If isRunning Then
            Return returnValue
        End If

        ' no start position set
        If StartPoint.X < 0 Or StartPoint.Y < 0 Then
            lstPathPoints = lstPathPointsTemp
            Return PathMessageType.PathError
        End If

        ' no stop position set
        If StopPoint.X < 0 Or StopPoint.Y < 0 Then
            lstPathPoints = lstPathPointsTemp
            Return PathMessageType.PathError
        End If

        bRunning = True

        ' compute values for all possible blocks
        ComputeBlocksValues()

        Dim curPathPoint As New PathPoint
        curPathPoint.Point = New Point(StopPoint.X, StopPoint.Y)
        ' generate random direction for stoppoint
        curPathPoint.Direction = lstDirections.Item(RandomGen.nextInt(0, lstDirections.Count))

        Dim lstPossiblePoints As New List(Of PathPoint)

        Dim stopPathPoint As New PathPoint
        stopPathPoint.Point = StopPoint
        lstPathPointsTemp.Add(stopPathPoint)

        returnValue = PathMessageType.PathFound

        ' find path by moving backwards (from end to start)
        While (curPathPoint.Point.X <> StartPoint.X Or curPathPoint.Point.Y <> StartPoint.Y)
            ' remember current value
            curPathPoint.EstimationValue = lstGridItem(GridView.GetIndex(curPathPoint.Point.X, curPathPoint.Point.Y, ColumnCount)).value
            Dim minValue As Single = GetGridValue(curPathPoint.Point)

            saveBestPoint(curPathPoint, Direction.Left, minValue, lstPossiblePoints)
            saveBestPoint(curPathPoint, Direction.Top, minValue, lstPossiblePoints)
            saveBestPoint(curPathPoint, Direction.Right, minValue, lstPossiblePoints)
            saveBestPoint(curPathPoint, Direction.Bottom, minValue, lstPossiblePoints)

            If DriveDiagonal Then
                ' diagnonal blocks
                saveBestPoint(curPathPoint, Direction.TopLeft, minValue, lstPossiblePoints)
                saveBestPoint(curPathPoint, Direction.TopRight, minValue, lstPossiblePoints)
                saveBestPoint(curPathPoint, Direction.BottomLeft, minValue, lstPossiblePoints)
                saveBestPoint(curPathPoint, Direction.BottomRight, minValue, lstPossiblePoints)
            End If

            ' choose one available block in the path
            If lstPossiblePoints.Count > 0 Then
                Dim randomIndex As Integer = RandomGen.nextInt(0, lstPossiblePoints.Count)
                Dim lessRotatingPathPoint As PathPoint = Nothing

                If AvoidRotations Then
                    For Each myPathPoint As PathPoint In lstPossiblePoints
                        If curPathPoint.Direction = myPathPoint.Direction Then
                            lessRotatingPathPoint = myPathPoint
                            Exit For
                        End If
                    Next
                End If

                If lessRotatingPathPoint Is Nothing Then
                    curPathPoint = lstPossiblePoints.Item(randomIndex)
                Else
                    curPathPoint = lessRotatingPathPoint
                End If

                lstPossiblePoints.Clear()

                lstPathPointsTemp.Add(curPathPoint)

                ' stop before start point is changed or it would turn into a pathpoint
                If curPathPoint.Point.X = StartPoint.X And curPathPoint.Point.Y = StartPoint.Y Then
                    Exit While
                End If

                lstGridItem.Item(GridView.GetIndex(curPathPoint.Point.X, curPathPoint.Point.Y, ColumnCount)).SetItemType(GridItemType.PathItem)

                If UseAnimation Then
                    RaiseEvent GridItemValueChanged()
                    Application.DoEvents()
                End If
            Else
                lstPathPointsTemp.Clear()

                If DriveDiagonal Then
                    ResetValues()
                    bRunning = False
                    DriveDiagonal = False
                    returnValue = FindPath(lstPathPointsTemp)
                    DriveDiagonal = True
                Else
                    ResetValues()
                    returnValue = PathMessageType.PathBlocked
                End If

                Exit While
            End If
        End While

        If Not UseAnimation Then
            RaiseEvent GridItemValueChanged()
            Application.DoEvents()
        End If

        If Not lstPathPoints Is Nothing Then
            If lstPathPoints.Count > 1 Then lstPathPoints.Clear()
        End If

        lstPathPointsTemp.Reverse()

        lstPathPoints = lstPathPointsTemp
        bRunning = False

        Return returnValue
    End Function

    Private Sub ResetValues()
        For Each myItem As GridItem In lstGridItem
            myItem.SetItemType(myItem.GetItemType(), True)
        Next
    End Sub

    Private Function GetGridValue(newPoint As Point) As Single
        Return GetGridValue(newPoint.X, newPoint.Y)
    End Function

    Private Function GetGridValue(x As Integer, y As Integer) As Single
        ' coordinates are in the grid?
        If x >= 0 And y >= 0 And x < ColumnCount And y < RowCount Then
            Dim value As Single = lstGridItem(GridView.GetIndex(x, y, ColumnCount)).value()

            If value >= 0.0 Then Return value
        End If

        ' return high value, if it doesn't exists, so it won't be chosen as a good path
        Return lstGridItem(GridView.GetIndex(StopPoint.X, StopPoint.Y, ColumnCount)).value + 1
    End Function
End Class

