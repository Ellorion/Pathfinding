Imports Pathfinding.GridItem

''' <summary>
''' Handles all about Pathfinding (returns a Integer random path)
''' </summary>
Public Class PathfindingSingle
    Inherits Pathfinding

    Public Overrides Function FindPath(ByRef lstPathPoints As List(Of List(Of PathPoint))) As PathMessageType
        Dim lstIntegerPathPoints As New List(Of PathPoint)
        Dim returnValue As PathMessageType = PathMessageType.Running

        If lstPathPoints Is Nothing Then
            lstPathPoints = New List(Of List(Of PathPoint))
        Else
            lstPathPoints.Clear()
        End If

        If isRunning Then
            Return returnValue
        End If

        ' no start position set
        If StartPoint.X < 0 Or StartPoint.Y < 0 Then
            lstPathPoints.Add(lstIntegerPathPoints)
            Return PathMessageType.PathError
        End If

        ' no stop position set
        If StopPoint.X < 0 Or StopPoint.Y < 0 Then
            lstPathPoints.Add(lstIntegerPathPoints)
            Return PathMessageType.PathError
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
        While (curPathPoint.Point.X <> StartPoint.X Or curPathPoint.Point.Y <> StartPoint.Y)
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
                If curPathPoint.Point.X = StartPoint.X And curPathPoint.Point.Y = StartPoint.Y Then
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
                    returnValue = FindPath(lstPathPoints)
                    DriveDiagonal = True
                Else
                    returnValue = PathMessageType.PathBlocked
                End If

                Exit While
            End If
        End While

        lstIntegerPathPoints.Reverse()

        lstPathPoints.Add(lstIntegerPathPoints)
        bRunning = False

        Return returnValue
    End Function
End Class

