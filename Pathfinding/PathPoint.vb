Imports Pathfinding.PathfindingSingle

Public Class PathPoint
    Public Property Point As Point = New Point(0, 0)
    Public Property StepValue As Integer = 0
    Public Property EstimationValue As Integer = 0

    ''' <summary>
    ''' (used to prevent rotations while selecting a path)
    ''' </summary>
    Public Property Direction As Pathfinding.Direction
End Class
