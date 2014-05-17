Imports Pathfinding.GridItem
Imports Pathfinding.PathfindingSingle

Public Class frmMain
    Dim pf As IPathfinding
    Dim WithEvents gv As GridView = Nothing

    Dim posType As GridItemType = GridItemType.DefaultItem

    Dim bStarted As Boolean = False
    Dim bDriving As Boolean = False

    Dim statusPrefix As String = "Status: "

    Dim lstPaths As List(Of List(Of PathPoint)) = Nothing

    Public ReadOnly Property isDriving As Boolean
        Get
            Return bDriving
        End Get
    End Property

    Private Sub frmMain_KeyUp(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyUp
        If e.KeyCode = Keys.F1 Then
            End
        End If

        If e.KeyCode = Keys.Escape Then
            posType = GridItemType.DefaultItem
        End If
    End Sub

    Private Sub ChangePathfinding(bMultible As Boolean)
        If bMultible Then
            pf = New PathfindingMultible
        Else
            pf = New PathfindingSingle
        End If

        pf.DebugMode = ckbDebugMode.Checked
        pf.AvoidRotations = ckbAvoidRotation.Checked

        If gv Is Nothing Then
            gv = New GridView(pf)
        Else
            gv.SwitchPathfinding(pf)
        End If
    End Sub

    Private Sub frmMain_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        ChangePathfinding(ckbMultiblePaths.Checked)

        cboPFStategies.Items.Add("Horizonal / Vertical")
        cboPFStategies.Items.Add("Horizonal / Vertical / Diagonal")
        cboPFStategies.SelectedIndex = 0

        btnStartPos.Image = CreateIcon(Brushes.Green)
        btnStopPos.Image = CreateIcon(Brushes.Red)
        btnWall.Image = CreateIcon(Brushes.Black)

        pf.DebugMode = ckbDebugMode.Checked
        pf.AvoidRotations = ckbAvoidRotation.Checked

        gv.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
        gv.GridSize = nudGridSize.Value

        gv.SetTypePosition(GridItemType.StartItem, 0, 0)
        gv.SetTypePosition(GridItemType.StopItem, 19, 9)

        Randomize()

        bStarted = True

        grpPathfinding.Focus()
        btnStartPathfinding.Focus()
    End Sub

    Private Sub pbImage_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles pbImage.MouseClick
        gv.SetTypePosition(posType, Math.Floor(e.X / gv.GridSize), Math.Floor(e.Y / gv.GridSize))
    End Sub

    Private Sub btnStartPos_Click(sender As System.Object, e As System.EventArgs) Handles btnStartPos.Click
        posType = GridItemType.StartItem
    End Sub

    Private Sub btnStopPos_Click(sender As System.Object, e As System.EventArgs) Handles btnStopPos.Click
        posType = GridItemType.StopItem
    End Sub

    Private Sub btnWall_Click(sender As System.Object, e As System.EventArgs) Handles btnWall.Click
        posType = GridItemType.WallItem
    End Sub

    Private Sub btnStartPathfinding_Click(sender As System.Object, e As System.EventArgs) Handles btnStartPathfinding.Click
        If pf.isRunning Or isDriving Then
            Exit Sub
        End If

        tsStatus.Text = statusPrefix + "finding path(s)..."
        Application.DoEvents()

        Dim startTime As Date = Now()
        Dim pfmsg As PathMessageType = pf.FindPath(lstPaths)
        Dim endTime As Date = Now()

        Dim diffTime As TimeSpan = endTime.Subtract(startTime)

        Dim statusMsg As String = ""

        Select Case pfmsg
            Case PathMessageType.PathError
                statusMsg = "Start or Stop-Position not set!"
            Case PathMessageType.PathBlocked
                statusMsg = "No available path cound be found"
        End Select

        If statusMsg.Length > 0 Then
            tsStatus.Text = statusPrefix + statusMsg
            Exit Sub
        End If

        If lstPaths.Count = 1 Then
            Dim estimationSum As Single = 0.0

            For Each myPathPoint As PathPoint In lstPaths.Item(0)
                estimationSum += myPathPoint.EstimationValue
            Next

            estimationSum = Math.Round(estimationSum, 2)

            statusMsg = "estimation: " + estimationSum.ToString
        Else
            statusMsg = "[Paths found: " + lstPaths.Count.ToString + "]"

            Dim curBestEstimation As Single = 0.0
            Dim lstBestPaths As New List(Of List(Of PathPoint))

            For Each myPathPoints As List(Of PathPoint) In lstPaths
                Dim curEstimation As Single = 0.0

                For Each myPathPoint As PathPoint In myPathPoints
                    curEstimation += myPathPoint.EstimationValue
                Next

                If curEstimation < curBestEstimation Or curBestEstimation <= 0.0 Then
                    curBestEstimation = curEstimation
                    lstBestPaths.Clear()
                    lstBestPaths.Add(myPathPoints)
                ElseIf curEstimation = curBestEstimation Then
                    lstBestPaths.Add(myPathPoints)
                End If
            Next

            statusMsg += " [Best estimation: " + curBestEstimation.ToString
            statusMsg += " - Path(s): " + lstBestPaths.Count.ToString + "]"

            lstPaths = lstBestPaths
        End If

        statusMsg += " Time(sec): " + diffTime.Seconds.ToString + "." + diffTime.Milliseconds.ToString.PadLeft(3, "0")

        tsStatus.Text = statusPrefix + statusMsg

        gv.Refresh()

        If lstPaths.Count = 1 Then
            If ckbDriveEnabled.Checked Then
                StartDriving()
            End If
        End If
    End Sub

    Public Sub StartDriving()
        If bDriving Or lstPaths Is Nothing Then
            Exit Sub
        End If

        If lstPaths.Count = 0 Then Exit Sub

        bDriving = True

        Dim tsStatusText As String = tsStatus.Text

        tsStatus.Text += " - Driving..."

        Dim lastPoint As PathPoint = Nothing

        If lstPaths.Count = 1 Then
            For Each myPathPoints As List(Of PathPoint) In lstPaths
                For Each myPathPoint As PathPoint In myPathPoints
                    If Not lastPoint Is Nothing Then
                        gv.SetTypePosition(GridItemType.DefaultItem, lastPoint.Point.X, lastPoint.Point.Y)
                    End If

                    gv.SetTypePosition(GridItemType.StartItem, myPathPoint.Point.X, myPathPoint.Point.Y)
                    lastPoint = myPathPoint

                    gv.Refresh()
                    Application.DoEvents()
                    Threading.Thread.Sleep(200)
                Next
            Next
        End If

        tsStatus.Text = tsStatusText + " - Driving: done"

        bDriving = False
    End Sub

    Public Function CreateIcon(brush As Brush) As Bitmap
        Dim width As Integer = 10
        Dim height As Integer = 10

        Dim tmpIcon As Bitmap = New Bitmap(width, height)

        Using g As Graphics = Graphics.FromImage(tmpIcon)
            g.FillRectangle(brush, 0, 0, width, height)
        End Using

        Return tmpIcon
    End Function

    Private Sub pf_GridChanged(grid As System.Drawing.Bitmap) Handles gv.GridChanged
        If Not pbImage.Image Is Nothing Then
            pbImage.Image.Dispose()
        End If

        pbImage.Image = grid
    End Sub

    Private Sub nudColumnCount_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nudColumnCount.ValueChanged
        If bStarted Then
            gv.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
        End If
    End Sub

    Private Sub nudRowCount_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nudRowCount.ValueChanged
        If bStarted Then
            gv.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
        End If
    End Sub

    Private Sub nudGridSize_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nudGridSize.ValueChanged
        If bStarted Then
            gv.GridSize = nudGridSize.Value
        End If
    End Sub

    Private Sub btnClearMap_Click(sender As System.Object, e As System.EventArgs) Handles btnClearMap.Click
        gv.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
    End Sub

    Private Sub ckbDebugMode_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckbDebugMode.CheckedChanged
        pf.DebugMode = ckbDebugMode.Checked
    End Sub

    Private Sub ckbAvoidRotation_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckbAvoidRotation.CheckedChanged
        pf.AvoidRotations = ckbAvoidRotation.Checked
    End Sub

    Private Sub cboPFStategies_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cboPFStategies.SelectedIndexChanged
        If cboPFStategies.SelectedIndex = 0 Then
            pf.DriveDiagonal = False
        ElseIf cboPFStategies.SelectedIndex = 1 Then
            pf.DriveDiagonal = True
        End If
    End Sub

    Private Sub ckbMultiblePaths_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckbMultiblePaths.CheckedChanged
        ChangePathfinding(ckbMultiblePaths.Checked)
    End Sub
End Class

