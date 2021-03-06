﻿Imports Pathfinding.GridItem

Public MustInherit Class Pathfinding
    Implements IPathfinding

    Public Enum PathMessageType
        PathFound
        PathBlocked
        PathError
        Running
        StartEndPointsEqual
        NotInitialized
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

    Protected Property StartPoint As Point = New Point(-1, -1) Implements IPathfinding.StartPoint
    Protected Property StopPoint As Point = New Point(-1, -1) Implements IPathfinding.StopPoint

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

    Public MustOverride Function FindPath(startPoint As Point, endPoint As Point, ByRef lstPathPoints As List(Of List(Of PathPoint))) As PathMessageType Implements IPathfinding.FindPath

    Protected Function GetNeighborPoint(curPoint As Point, newDirection As Direction) As Point
        Select Case newDirection
            Case Direction.Left
                curPoint = New Point(curPoint.X - 1, curPoint.Y)
            Case Direction.Top
                curPoint = New Point(curPoint.X, curPoint.Y - 1)
            Case Direction.Right
                curPoint = New Point(curPoint.X + 1, curPoint.Y)
            Case Direction.Bottom
                curPoint = New Point(curPoint.X, curPoint.Y + 1)
            Case Direction.TopLeft
                curPoint = New Point(curPoint.X - 1, curPoint.Y - 1)
            Case Direction.TopRight
                curPoint = New Point(curPoint.X + 1, curPoint.Y - 1)
            Case Direction.BottomLeft
                curPoint = New Point(curPoint.X - 1, curPoint.Y + 1)
            Case Direction.BottomRight
                curPoint = New Point(curPoint.X + 1, curPoint.Y + 1)
        End Select

        Return curPoint
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

        Dim neighborItem As GridItem = lstGridItem.Item(index)
        If neighborItem.GetItemType() = GridItemType.WallItem Then
            Return True
        End If

        Return False
    End Function

    Protected Function SetValueOfSurroundingBlock(curItem As GridItem, curPoint As Point, newDirection As Direction, ByRef lstBlocksWithNewValues As List(Of Point)) As GridValueReturnType
        Dim neighborPoint As Point = GetNeighborPoint(curPoint, newDirection)
        Dim nextStepValue As Integer = 10

        ' neighbor griditem on map?
        If Not inArea(neighborPoint) Then
            Return GridValueReturnType.DoesNotExists
        End If

        Dim nextItem As GridItem = lstGridItem.Item(GridView.GetIndex(neighborPoint.X, neighborPoint.Y, myColumnCount))

        If nextItem.GetItemType() <> GridItemType.WallItem Then
            If newDirection = Direction.TopLeft Or newDirection = Direction.TopRight Or newDirection = Direction.BottomLeft Or newDirection = Direction.BottomRight Then
                ' diagonal blocks
                nextStepValue = 14
            End If

            ' update if not set or new possible value is better than current value
            If nextItem.GridValue < 0 Or nextItem.GridValue > curItem.GridValue + nextStepValue Then
                nextItem.GridValue = curItem.GridValue + nextStepValue

                lstBlocksWithNewValues.Add(neighborPoint)

                Return GridValueReturnType.ValueUpdated
            End If
        Else
            Return GridValueReturnType.IsWall
        End If

        Return GridValueReturnType.Unchanged
    End Function

    ''' <summary>
    ''' sets values for each available block on the map
    ''' </summary>
    Protected Sub ComputeBlocksValues()
        Dim lstBlocksWithNewValue As New List(Of Point)

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
            If myItem.GridValue >= 0 Then
                For Each myDirection As Direction In lstDirectionsBase
                    SetValueOfSurroundingBlock(myItem, curPoint, myDirection, lstBlocksWithNewValue)
                Next

                If DriveDiagonal Then
                    SetValueOfSurroundingBlock(myItem, curPoint, Direction.TopLeft, lstBlocksWithNewValue)
                    SetValueOfSurroundingBlock(myItem, curPoint, Direction.TopRight, lstBlocksWithNewValue)
                    SetValueOfSurroundingBlock(myItem, curPoint, Direction.BottomLeft, lstBlocksWithNewValue)
                    SetValueOfSurroundingBlock(myItem, curPoint, Direction.BottomRight, lstBlocksWithNewValue)
                End If
            End If
        End While
    End Sub

    Protected Sub SaveBestPoint(curPathPoint As PathPoint, newDirection As Direction, ByRef minValue As Integer, ByRef lstPossiblePoints As List(Of PathPoint))
        Dim neighborPoint As Point = GetNeighborPoint(curPathPoint.Point, newDirection)
        Dim newValue As Integer = GetGridValue(neighborPoint)

        ' if diag -> check other 2 sides, if at least one is a wall -> don't save it!
        Select Case newDirection
            Case Direction.TopLeft
                If isWall(curPathPoint.Point, Direction.Top) Then Return
                If isWall(curPathPoint.Point, Direction.Left) Then Return
            Case Direction.TopRight
                If isWall(curPathPoint.Point, Direction.Top) Then Return
                If isWall(curPathPoint.Point, Direction.Right) Then Return
            Case Direction.BottomLeft
                If isWall(curPathPoint.Point, Direction.Bottom) Then Return
                If isWall(curPathPoint.Point, Direction.Left) Then Return
            Case Direction.BottomRight
                If isWall(curPathPoint.Point, Direction.Bottom) Then Return
                If isWall(curPathPoint.Point, Direction.Right) Then Return
        End Select

        ' better value found? [h/v griditems "and" diagonal]
        If newValue <= curPathPoint.StepValue And newValue <= minValue Then
            'If newValue <= curPathPoint.StepValue And newValue <= minValue Then
            ' mark old bad values as deleteable
            If DriveDiagonal Then
                Dim lstDeleteableValues As New List(Of PathPoint)

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
            End If

            ' store the good values
            Dim myPathPoint As New PathPoint
            myPathPoint.Point = neighborPoint
            myPathPoint.StepValue = newValue
            myPathPoint.EstimationValue = myPathPoint.StepValue
            myPathPoint.Direction = newDirection

            ' double estimation for points, who are not in the direction as before
            If curPathPoint.Direction <> newDirection Then
                myPathPoint.EstimationValue += myPathPoint.EstimationValue
            End If

            lstPossiblePoints.Add(myPathPoint)

            minValue = newValue
        End If
    End Sub

    Protected Sub ResetValues()
        For Each myItem As GridItem In lstGridItem
            myItem.SetItemType(myItem.GetItemType(), True)
        Next
    End Sub

    Protected Function GetGridValue(newPoint As Point) As Integer
        Return GetGridValue(newPoint.X, newPoint.Y)
    End Function

    Protected Function GetGridValue(x As Integer, y As Integer) As Integer
        ' coordinates are in the grid?
        If inArea(New Point(x, y)) Then
            Dim value As Integer = lstGridItem(GridView.GetIndex(x, y, ColumnCount)).GridValue

            If value >= 0 Then Return value
        End If

        ' return high value, if it doesn't exists, so it won't be chosen as a good path
        Return lstGridItem(GridView.GetIndex(StopPoint.X, StopPoint.Y, ColumnCount)).GridValue + 1
    End Function

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
        If lstPathPoints.Count >= 1 Then
            Return CountRotations(lstPathPoints.Item(0))
        End If

        Return 0
    End Function

    Public Shared Function GetIndex(newPoint As Point, ColumnCount As Integer) As Integer
        Return GetIndex(newPoint.X, newPoint.Y, columnCount)
    End Function

    Public Shared Function GetIndex(x As Integer, y As Integer, ColumnCount As Integer) As Integer
        Return y * columnCount + x
    End Function
End Class
