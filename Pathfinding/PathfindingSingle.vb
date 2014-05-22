Imports Pathfinding.GridItem

''' <summary>
''' Handles all about Pathfinding (returns a Integer random path)
''' </summary>
Public Class PathfindingSingle
    Inherits Pathfinding

    Private lstPathPoints As New List(Of List(Of PathPoint))

    Public Sub New()
    End Sub

    Public Sub New(columnCount As Integer, rowCount As Integer, lstGridItem As List(Of GridItem))
        Me.Init(columnCount, rowCount, lstGridItem)
    End Sub

    Public Overrides Function FindPath(newStartPoint As Point, newStopPoint As Point, ByRef lstPathPoints As List(Of List(Of PathPoint))) As PathMessageType
        Dim lstIntegerPathPoints As New List(Of PathPoint)
        Dim returnValue As PathMessageType = PathMessageType.Running

        If isRunning Then
            Return returnValue
        End If

        Me.lstPathPoints.Clear()

        StartPoint = newStartPoint
        StopPoint = newStopPoint

        ' no start position set
        If StartPoint.X < 0 Or StartPoint.Y < 0 Then
            Return PathMessageType.PathError
        End If

        ' no stop position set
        If StopPoint.X < 0 Or StopPoint.Y < 0 Then
            Return PathMessageType.PathError
        End If

        If lstGridItem Is Nothing Then
            Return PathMessageType.NotInitialized
        End If

        lstGridItem(GetIndex(StartPoint, ColumnCount)).SetItemType(GridItemType.StartItem)
        lstGridItem(GetIndex(StopPoint, ColumnCount)).SetItemType(GridItemType.StopItem)

        If StartPoint.X = StopPoint.X And StartPoint.Y = StopPoint.Y Then
            Return PathMessageType.StartEndPointsEqual
        End If

        bRunning = True

        ' compute values for all possible blocks
        ComputeBlocksValues()

        'Return PathMessageType.Running

        Dim curPathPoint As New PathPoint
        curPathPoint.Point = New Point(StopPoint.X, StopPoint.Y)
        ' generate fixed direction for stop-point
        curPathPoint.Direction = Direction.Left

        Dim lstPossiblePoints As New List(Of PathPoint)

        Dim stopPathPoint As New PathPoint
        stopPathPoint.Point = StopPoint
        lstIntegerPathPoints.Add(stopPathPoint)

        returnValue = PathMessageType.PathFound

        ' find path by moving backwards (from end to start)
        While (curPathPoint.Point.X <> startPoint.X Or curPathPoint.Point.Y <> startPoint.Y)
            ' remember current value
            curPathPoint.StepValue = lstGridItem(GridView.GetIndex(curPathPoint.Point.X, curPathPoint.Point.Y, ColumnCount)).GridValue
            Dim minValue As Integer = curPathPoint.StepValue

            SaveBestPoint(curPathPoint, Direction.Left, minValue, lstPossiblePoints)
            SaveBestPoint(curPathPoint, Direction.Top, minValue, lstPossiblePoints)
            SaveBestPoint(curPathPoint, Direction.Right, minValue, lstPossiblePoints)
            SaveBestPoint(curPathPoint, Direction.Bottom, minValue, lstPossiblePoints)

            If DriveDiagonal Then
                ' diagnonal blocks
                SaveBestPoint(curPathPoint, Direction.TopLeft, minValue, lstPossiblePoints)
                SaveBestPoint(curPathPoint, Direction.TopRight, minValue, lstPossiblePoints)
                SaveBestPoint(curPathPoint, Direction.BottomLeft, minValue, lstPossiblePoints)
                SaveBestPoint(curPathPoint, Direction.BottomRight, minValue, lstPossiblePoints)
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

                lstIntegerPathPoints.Add(curPathPoint)

                ' stop before start point is changed or it would turn into a pathpoint
                If curPathPoint.Point.X = startPoint.X And curPathPoint.Point.Y = startPoint.Y Then
                    Exit While
                End If

                lstGridItem.Item(GridView.GetIndex(curPathPoint.Point.X, curPathPoint.Point.Y, ColumnCount)).SetItemType(GridItemType.PathItem)
            Else
                lstIntegerPathPoints.Clear()

                ResetValues()

                If DriveDiagonal Then
                    bRunning = False
                    DriveDiagonal = False
                    lstPathPoints.Clear()
                    returnValue = FindPath(StartPoint, StopPoint, lstPathPoints)
                    DriveDiagonal = True
                Else
                    returnValue = PathMessageType.PathBlocked
                End If

                Exit While
            End If
        End While

        lstIntegerPathPoints.Reverse()

        Me.lstPathPoints.Add(lstIntegerPathPoints)

        lstPathPoints = Me.lstPathPoints

        bRunning = False

        Return returnValue
    End Function
End Class

