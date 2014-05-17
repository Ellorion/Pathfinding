Imports Pathfinding.GridItem

''' <summary>
''' Handles all about Pathfinding (getting multible path choices)
''' </summary>
''' <remarks></remarks>
Public Class PathfindingMultible
    Inherits Pathfinding

    Private pfBase As New PathfindingSingle

    ''' <summary>
    ''' Compute path after each new block recursivly in every useful direction
    ''' </summary>
    Private Sub ComputePath(ByRef lstPathPoints As List(Of List(Of PathPoint)), lstCurPathPoints As List(Of PathPoint), maxRotations As Integer)
        Dim lstPossiblePathPoints As List(Of PathPoint) = New List(Of PathPoint)

        If lstCurPathPoints.Count = 0 Then
            Exit Sub
        End If

        Dim returnValue As PathMessageType = PathMessageType.Running

        Dim curPathPoint As PathPoint = lstCurPathPoints.Item(lstCurPathPoints.Count - 1)

        returnValue = PathMessageType.PathFound

        ' find path by moving backwards (from end to start)
        While (curPathPoint.Point.X <> StartPoint.X Or curPathPoint.Point.Y <> StartPoint.Y)
            ' remember current value
            curPathPoint.StepValue = lstGridItem(GridView.GetIndex(curPathPoint.Point.X, curPathPoint.Point.Y, ColumnCount)).GridValue
            Dim minValue As Single = GetGridValue(curPathPoint.Point)

            SaveBestPoint(curPathPoint, Direction.Left, minValue, lstPossiblePathPoints)
            SaveBestPoint(curPathPoint, Direction.Top, minValue, lstPossiblePathPoints)
            SaveBestPoint(curPathPoint, Direction.Right, minValue, lstPossiblePathPoints)
            SaveBestPoint(curPathPoint, Direction.Bottom, minValue, lstPossiblePathPoints)

            If DriveDiagonal Then
                ' diagnonal blocks
                SaveBestPoint(curPathPoint, Direction.TopLeft, minValue, lstPossiblePathPoints)
                SaveBestPoint(curPathPoint, Direction.TopRight, minValue, lstPossiblePathPoints)
                SaveBestPoint(curPathPoint, Direction.BottomLeft, minValue, lstPossiblePathPoints)
                SaveBestPoint(curPathPoint, Direction.BottomRight, minValue, lstPossiblePathPoints)
            End If

            ' choose one available block in the path
            If lstPossiblePathPoints.Count > 0 Then
                For Each myPathPoint As PathPoint In lstPossiblePathPoints
                    Dim lstNewPathPoints As List(Of PathPoint) = New List(Of PathPoint)
                    lstNewPathPoints.AddRange(lstCurPathPoints)
                    lstNewPathPoints.Add(myPathPoint)

                    ' rotation limiter active?
                    If maxRotations > 0 Then
                        Dim curRotations As Integer = CountRotations(lstNewPathPoints)

                        ' throw away paths, that use to many rotations
                        If curRotations > maxRotations Then
                            lstNewPathPoints.Clear()
                            lstCurPathPoints.Clear()
                            Exit Sub
                        End If
                    End If

                    'Debug.Print("CurRot: " + curRotations.ToString)

                    ComputePath(lstPathPoints, lstNewPathPoints, maxRotations)
                    Application.DoEvents()
                Next

                Exit Sub
            Else
                lstCurPathPoints.Clear()
                Exit While
            End If
        End While

        If lstCurPathPoints.Count > 0 Then
            lstCurPathPoints.Reverse()

            lstPathPoints.Add(lstCurPathPoints)
        End If
    End Sub

    Public Overrides Function FindPath(ByRef lstPathPoints As List(Of List(Of PathPoint))) As PathMessageType
        Dim lstPathPointsTemp As New List(Of List(Of PathPoint))
        Dim returnValue As PathMessageType = PathMessageType.Running

        If lstPathPoints Is Nothing Then
            lstPathPoints = New List(Of List(Of PathPoint))
        End If

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
        ' generate random Direction for stoppoint
        curPathPoint.Direction = Direction.Left

        Dim lstPossiblePoints As List(Of PathPoint) = New List(Of PathPoint)
        lstPossiblePoints.Add(curPathPoint)

        With pfBase
            .StartPoint = StartPoint
            .StopPoint = StopPoint
            .Init(ColumnCount, RowCount, lstGridItem)
            .AvoidRotations = True
            .DriveDiagonal = DriveDiagonal
            .FindPath(lstPathPoints)
        End With

        If lstPathPoints.Count = 1 Then
            If lstPathPoints.Item(0).Count = 0 Then
                bRunning = False
                Return PathMessageType.PathBlocked
            End If
        End If

        Dim maxRotations As Integer = CountRotations(lstPathPoints)

        ' compute paths for given amount of max. rotations
        ComputePath(lstPathPointsTemp, lstPossiblePoints, maxRotations + 1)

        lstPathPoints = lstPathPointsTemp

        bRunning = False

        If lstPathPoints.Count = 0 Then
            returnValue = PathMessageType.PathBlocked
        Else
            returnValue = PathMessageType.PathFound
        End If

        Return returnValue
    End Function
End Class

