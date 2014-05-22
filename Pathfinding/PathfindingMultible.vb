Imports Pathfinding.GridItem

''' <summary>
''' Handles all about Pathfinding (getting multible path choices)
''' </summary>
''' <remarks></remarks>
Public Class PathfindingMultible
    Inherits Pathfinding

    Private pfInteger As New PathfindingSingle
    Private lstPathPoints As New List(Of List(Of PathPoint))

    ''' <summary>
    ''' Compute path after each new block recursivly in every useful direction
    ''' </summary>
    Private Sub ComputePath(ByVal lstCurPathPoints As List(Of PathPoint), maxRotations As Integer)
        If lstCurPathPoints.Count = 0 Then
            Exit Sub
        End If

        Dim returnValue As PathMessageType = PathMessageType.Running

        Dim curPathPoint As PathPoint = lstCurPathPoints.Item(lstCurPathPoints.Count - 1)

        returnValue = PathMessageType.PathFound

        ' find path by moving backwards (from end to start)
        If (curPathPoint.Point.X <> StartPoint.X Or curPathPoint.Point.Y <> StartPoint.Y) Then
            ' remember current value
            curPathPoint.StepValue = GetGridValue(curPathPoint.Point)
            Dim minValue As Integer = curPathPoint.StepValue

            Dim lstPossiblePathPoints As New List(Of PathPoint)

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

            ' check all available (useful) directions
            For Each myPathPoint As PathPoint In lstPossiblePathPoints
                Dim lstNewPathPoints As New List(Of PathPoint)(lstCurPathPoints)
                lstNewPathPoints.Add(myPathPoint)

                Dim curRotations As Integer = CountRotations(lstNewPathPoints)

                ' throw away paths, that use to many rotations
                If curRotations > maxRotations Then
                    Exit Sub
                End If

                ComputePath(lstNewPathPoints, maxRotations)
            Next

            Exit Sub
        End If

        If lstCurPathPoints.Count > 0 Then
            lstCurPathPoints.Reverse()
            lstPathPoints.Add(lstCurPathPoints)
        End If
    End Sub

    Public Overrides Function FindPath(ByRef lstPathPoints As List(Of List(Of PathPoint))) As PathMessageType
        Dim returnValue As PathMessageType = PathMessageType.Running

        If isRunning Then
            Return returnValue
        End If

        Me.lstPathPoints.Clear()

        ' no start position set
        If StartPoint.X < 0 Or StartPoint.Y < 0 Then
            Return PathMessageType.PathError
        End If

        ' no stop position set
        If StopPoint.X < 0 Or StopPoint.Y < 0 Then
            Return PathMessageType.PathError
        End If

        bRunning = True

        ' compute values for all possible blocks
        ComputeBlocksValues()

        Dim curPathPoint As New PathPoint
        curPathPoint.Point = New Point(StopPoint.X, StopPoint.Y)
        ' generate random Direction for stoppoint
        curPathPoint.Direction = Direction.Left

        With pfInteger
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

        If maxRotations < 3 Then
            maxRotations = 3
        End If

        Dim lstPossiblePoints As New List(Of PathPoint)
        lstPossiblePoints.Add(curPathPoint)

        ' compute paths for given amount of max. rotations
        For curRotationCount As Integer = 2 To maxRotations
            ComputePath(lstPossiblePoints, curRotationCount)

            If Me.lstPathPoints.Count > 0 Then
                Exit For
            End If
        Next

        bRunning = False

        If lstPathPoints.Count = 0 Then
            returnValue = PathMessageType.PathBlocked
        Else
            returnValue = PathMessageType.PathFound
        End If

        lstPathPoints = Me.lstPathPoints

        Return returnValue
    End Function
End Class

