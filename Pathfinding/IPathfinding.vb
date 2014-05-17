Public Interface IPathfinding
    ' Properties
    Property DriveDiagonal As Boolean
    Property UseAnimation As Boolean
    Property AvoidRotations As Boolean

    ReadOnly Property isRunning As Boolean
    Property DebugMode As Boolean

    ReadOnly Property ColumnCount As Integer
    ReadOnly Property RowCount As Integer
    Property StartPoint As Point
    Property StopPoint As Point

    ' Methods
    Sub Init(columnCount As Integer, rowCount As Integer, lstGridItem As List(Of GridItem))
    Sub UpdateGridList(lstGridItem As List(Of GridItem))
    Function FindPath(ByRef lstPathPoints As List(Of List(Of PathPoint))) As Pathfinding.PathMessageType

    ' Events
    Event GridItemValueChanged()
End Interface
