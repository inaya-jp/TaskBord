Imports System.Runtime.InteropServices

Public Class TaskBord
    Private Sub tim1sec_Tick(sender As Object, e As EventArgs) Handles tim1sec.Tick
        ' 1秒ごとに現在の日時を更新
        lblDateTime.Text = DateTime.Now.ToString(My.Settings.DateTimeFormat)
    End Sub

    Private Sub 設定を変更するToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles 設定を変更するToolStripMenuItem.Click
        ' クリックされたメニューアイテム
        Dim menuItem = DirectCast(sender, ToolStripMenuItem)
        ' コンテキストメニュー
        Dim contextMenu = DirectCast(menuItem.Owner, ContextMenuStrip)
        ' 右クリックされたパネルを特定する
        Dim sourcePanel = contextMenu.SourceControl.Name

        ' ChangeSettingsフォームを開き、右クリックされたパネルの名前を渡す
        Dim settingsForm As New ChangeSettings(sourcePanel)
        settingsForm.Show()

    End Sub
End Class