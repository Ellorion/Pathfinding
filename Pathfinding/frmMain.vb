Imports Pathfinding.GridItem
Imports Pathfinding.Pathfinding

Public Class frmMain
    Dim pf As New Pathfinding
    Dim WithEvents gw As New GridView(pf)

    Dim posType As GridItemType = GridItemType.DefaultItem

    Dim bStarted As Boolean = False
    Dim bDriving As Boolean = False

    Dim statusPrefix As String = "Status: "

    Dim lstPathPoints As List(Of PathPoint) = Nothing

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

    Private Sub frmMain_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        cboPFStategies.Items.Add("Horizonal / Vertical")
        cboPFStategies.Items.Add("Horizonal / Vertical / Diagonal")
        cboPFStategies.SelectedIndex = 0

        btnStartPos.Image = CreateIcon(Brushes.Green)
        btnStopPos.Image = CreateIcon(Brushes.Red)
        btnWall.Image = CreateIcon(Brushes.Black)

        pf.UseAnimation = ckbAnimation.Checked
        pf.DebugMode = ckbDebugMode.Checked
        pf.AvoidRotations = ckbAvoidRotation.Checked

        gw.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
        gw.GridSize = nudGridSize.Value

        gw.SetTypePosition(GridItemType.StartItem, 2 * gw.GridSize, 1 * gw.GridSize)
        gw.SetTypePosition(GridItemType.StopItem, 14 * gw.GridSize, 8 * gw.GridSize)

        gw.SetTypePosition(GridItemType.WallItem, 8 * gw.GridSize, 2 * gw.GridSize)
        gw.SetTypePosition(GridItemType.WallItem, 8 * gw.GridSize, 3 * gw.GridSize)
        gw.SetTypePosition(GridItemType.WallItem, 8 * gw.GridSize, 4 * gw.GridSize)
        gw.SetTypePosition(GridItemType.WallItem, 8 * gw.GridSize, 5 * gw.GridSize)

        Randomize()

        bStarted = True

        grpPathfinding.Focus()
        btnStartPathfinding.Focus()
    End Sub

    Private Sub pbImage_MouseClick(sender As Object, e As System.Windows.Forms.MouseEventArgs) Handles pbImage.MouseClick
        gw.SetTypePosition(posType, e.X, e.Y)
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

        Dim startTime As Date = Now()
        Dim pfmsg As PathMessageType = pf.FindPath(lstPathPoints)
        Dim endTime As Date = Now()

        Dim diffTime As TimeSpan = endTime.Subtract(startTime)

        Dim statusMsg As String = ""

        Select Case pfmsg
            Case PathMessageType.PathError
                statusMsg = "Start or Stop-Position not set!"
                'MsgBox(statusMsg, , "Error")
            Case PathMessageType.PathBlocked
                statusMsg = "No available path cound be found"
                'MsgBox(statusMsg, , "Info")
        End Select

        If lstPathPoints.Count > 0 Then
            Dim estimationSum As Single = 0.0

            For Each myPoint As PathPoint In lstPathPoints
                estimationSum += myPoint.EstimationValue
            Next

            estimationSum = Math.Round(estimationSum, 2)

            statusMsg = "estimation: " + estimationSum.ToString

            statusMsg += " Time(sec): " + diffTime.Seconds.ToString + "." + diffTime.Milliseconds.ToString
        End If

        tsStatus.Text = statusPrefix + statusMsg

        If ckbDriveEnabled.Checked Then
            StartDriving()
        End If
    End Sub

    Public Sub StartDriving()
        If bDriving Or lstPathPoints Is Nothing Then
            Exit Sub
        End If

        If lstPathPoints.Count = 0 Then Exit Sub

        bDriving = True

        Dim tsStatusText As String = tsStatus.Text

        tsStatus.Text += " - Driving..."

        Dim lastPoint As PathPoint = Nothing
        For Each myPoint As PathPoint In lstPathPoints
            If Not lastPoint Is Nothing Then
                gw.SetTypePosition(GridItemType.DefaultItem, lastPoint.Point.X * gw.GridSize, lastPoint.Point.Y * gw.GridSize)
            End If

            gw.SetTypePosition(GridItemType.StartItem, myPoint.Point.X * gw.GridSize, myPoint.Point.Y * gw.GridSize)
            lastPoint = myPoint

            Application.DoEvents()
            Threading.Thread.Sleep(100)
        Next

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

    Private Sub pf_GridChanged(grid As System.Drawing.Bitmap) Handles gw.GridChanged
        If Not pbImage.Image Is Nothing Then
            pbImage.Image.Dispose()
        End If

        pbImage.Image = grid
    End Sub

    Private Sub ckbAnimation_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ckbAnimation.CheckedChanged
        pf.UseAnimation = ckbAnimation.Checked
    End Sub

    Private Sub nudColumnCount_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nudColumnCount.ValueChanged
        If bStarted Then
            gw.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
        End If
    End Sub

    Private Sub nudRowCount_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nudRowCount.ValueChanged
        If bStarted Then
            gw.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
        End If
    End Sub

    Private Sub nudGridSize_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nudGridSize.ValueChanged
        If bStarted Then
            gw.GridSize = nudGridSize.Value
        End If
    End Sub

    Private Sub btnClearMap_Click(sender As System.Object, e As System.EventArgs) Handles btnClearMap.Click
        gw.CreateGrid(nudColumnCount.Value, nudRowCount.Value)
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
End Class

