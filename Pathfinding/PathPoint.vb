﻿Imports Pathfinding.Pathfinding

Public Class PathPoint
    Public Property Point As Point = New Point(0, 0)
    Public Property EstimationValue As Single = 0.0

    ''' <summary>
    ''' (used to prevent rotations while selecting a path)
    ''' </summary>
    Public Property Direction As Direction
End Class
