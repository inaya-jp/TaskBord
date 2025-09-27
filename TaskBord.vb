Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection.Emit
Imports System.Runtime.InteropServices

Public Class TaskBord

    ' クラスレベルのフィールド
    Private WTVGOTHIC_R_FC As PrivateFontCollection
    Private WTVGOTHIC_R As Font

    Private Sub tim1sec_Tick(sender As Object, e As EventArgs) Handles tim1sec.Tick
        ' 1秒ごとに現在の日時を更新
        lblDateTime.Text = DateTime.Now.ToString(My.Settings.DateTimeFormat)
    End Sub

    Private Sub 設定を変更するToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ChangeSettingsToolStripMenuItem.Click
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

    Private Function GetResourceFont() As PrivateFontCollection
        ' リソースからフォントを読み込み、PrivateFontCollectionに追加する
        Dim pfc = New System.Drawing.Text.PrivateFontCollection()

        Dim fontBufB = My.Resources.Resources.WTVGOTHIC_R

        ' メモリストリームを使用してフォントデータを読み込む
        Using fontStream As New MemoryStream(fontBufB)
            Dim fontData(fontBufB.Length - 1) As Byte
            fontStream.Read(fontData, 0, fontBufB.Length)

            Dim fontPtr = Runtime.InteropServices.Marshal.AllocCoTaskMem(fontBufB.Length)
            Runtime.InteropServices.Marshal.Copy(fontData, 0, fontPtr, fontBufB.Length)

            pfc.AddMemoryFont(fontPtr, fontBufB.Length)

            Runtime.InteropServices.Marshal.FreeCoTaskMem(fontPtr)
        End Using

        Return pfc
    End Function

    Private Sub TaskBord_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        Me.WTVGOTHIC_R_FC = GetResourceFont()
        Dim fsty As FontStyle = FontStyle.Bold
        Me.WTVGOTHIC_R = New Font(Me.WTVGOTHIC_R_FC.Families(0), 16.0F, fsty)

        With lblDateTime
            .UseCompatibleTextRendering = True
            .Font = Me.WTVGOTHIC_R
        End With


    End Sub
End Class