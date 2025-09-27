Imports System.Drawing.Text
Imports System.IO
Imports System.Reflection.Emit
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Collections.Specialized
Imports Newtonsoft.Json

Public Class TaskBord

    Private Sub TaskBord_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddNewLabel()

        ' 各ラベルにカスタムフォントを設定
        Dim commonInstance As New Common()
        commonInstance.Set_customFont(lblDateTime)

        ' LabelManagerを初期化
        Dim LabelManager = New LabelManager(pnlWhiteBord)

        ' 起動時に自動的にラベルを復元（オプション）
        LabelManager.LoadLabels()

    End Sub

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

    ' 新しいラベルを追加するメソッド
    Private Sub AddNewLabel()
        ' 新しいラベルを作成
        Dim newLabel As New System.Windows.Forms.Label()

        ' ラベルのプロパティを設定
        newLabel.Text = "新しいラベル"
        newLabel.AutoSize = True
        newLabel.BackColor = Color.Transparent
        newLabel.Location = New Point(100, 100)

        ' フォントを適用（オプション）
        Dim commonInstance As New Common()
        commonInstance.Set_customFont(newLabel)

        ' ドラッグ機能を追加
        AddHandler newLabel.MouseDown, AddressOf Label_MouseDown
        AddHandler newLabel.MouseMove, AddressOf Label_MouseMove
        AddHandler newLabel.MouseUp, AddressOf Label_MouseUp

        ' フォームにラベルを追加
        pnlWhiteBord.Controls.Add(newLabel)

    End Sub

    ' ドラッグ機能のイベントハンドラ
    Private isDragging As Boolean = False
    Private lastPoint As Point

    ' MouseDownイベントでドラッグ開始
    Private Sub Label_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            isDragging = True
            lastPoint = e.Location
        End If
    End Sub

    ' MouseUpイベントでドラッグ終了
    Private Sub Label_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            isDragging = False
        End If
    End Sub

    ' MouseMoveイベントでラベルを移動
    Private Sub Label_MouseMove(sender As Object, e As MouseEventArgs)
        If isDragging Then
            Dim label As System.Windows.Forms.Label = DirectCast(sender, System.Windows.Forms.Label)

            ' 新しい位置を計算
            Dim newX = label.Left + (e.X - lastPoint.X)
            Dim newY = label.Top + (e.Y - lastPoint.Y)

            ' ラベルの位置を更新
            label.Location = New Point(newX, newY)
        End If
    End Sub


    ''' <summary>
    ''' ラベルの情報を保持するクラス
    ''' </summary>
    <Serializable>
    Public Class LabelInfo
        Public Property Text As String
        Public Property X As Integer
        Public Property Y As Integer
        Public Property FontName As String
        Public Property FontSize As Single
        Public Property FontStyle As FontStyle
        Public Property ForeColorArgb As Integer
        Public Property AutoSize As Boolean

        Public Sub New()
        End Sub

        Public Sub New(label As System.Windows.Forms.Label)
            Text = label.Text
            X = label.Location.X
            Y = label.Location.Y
            FontName = label.Font.Name
            FontSize = label.Font.Size
            FontStyle = label.Font.Style
            ForeColorArgb = label.ForeColor.ToArgb()
            AutoSize = label.AutoSize
        End Sub
    End Class

    ''' <summary>
    ''' ラベルの保存と復元を管理するクラス
    ''' </summary>
    Public Class LabelManager
        Private _container As Control

        Public Sub New(container As Control)
            _container = container
        End Sub

        ''' <summary>
        ''' すべてのラベルを保存
        ''' </summary>
        Public Sub SaveLabels()
            Try
                ' StringCollectionを初期化
                If My.Settings.LabelData Is Nothing Then
                    My.Settings.LabelData = New StringCollection()
                Else
                    My.Settings.LabelData.Clear()
                End If

                ' コンテナ内のすべてのTransparentLabelを取得
                For Each ctrl As Control In _container.Controls
                    If TypeOf ctrl Is System.Windows.Forms.Label OrElse TypeOf ctrl Is System.Windows.Forms.Label Then
                        Dim label As System.Windows.Forms.Label = DirectCast(ctrl, System.Windows.Forms.Label)
                        Dim info As New LabelInfo(label)

                        ' JSONにシリアライズして保存
                        Dim json As String = JsonConvert.SerializeObject(info)
                        My.Settings.LabelData.Add(json)
                    End If
                Next

                ' 設定を保存
                My.Settings.Save()

                MessageBox.Show($"{My.Settings.LabelData.Count}個のラベルを保存しました。",
                              "保存完了", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show($"保存中にエラーが発生しました: {ex.Message}",
                              "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' 保存されたラベルを復元
        ''' </summary>
        Public Sub LoadLabels()
            Try
                ' 既存のラベルをクリア（オプション）
                ClearLabels()

                If My.Settings.LabelData Is Nothing OrElse My.Settings.LabelData.Count = 0 Then
                    MessageBox.Show("保存されたラベルがありません。", "情報",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Return
                End If

                ' 保存されたラベルを復元
                For Each json As String In My.Settings.LabelData
                    If Not String.IsNullOrEmpty(json) Then
                        Dim info As LabelInfo = JsonConvert.DeserializeObject(Of LabelInfo)(json)
                        CreateLabelFromInfo(info)
                    End If
                Next

                MessageBox.Show($"{My.Settings.LabelData.Count}個のラベルを復元しました。",
                              "復元完了", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show($"復元中にエラーが発生しました: {ex.Message}",
                              "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' LabelInfoからラベルを作成
        ''' </summary>
        Private Sub CreateLabelFromInfo(info As LabelInfo)
            ' TransparentLabelを使用
            Dim newLabel As New System.Windows.Forms.Label() With {
                .Text = info.Text,
                .Location = New Point(info.X, info.Y),
                .AutoSize = info.AutoSize,
                .Font = New Font(info.FontName, info.FontSize, info.FontStyle),
                .ForeColor = Color.FromArgb(info.ForeColorArgb)
            }

            ' カスタムフォントを適用（必要に応じて）
            Dim commonInstance As New Common()
            commonInstance.Set_customFont(newLabel)

            ' コンテナに追加
            _container.Controls.Add(newLabel)
        End Sub

        ''' <summary>
        ''' すべてのラベルをクリア
        ''' </summary>
        Public Sub ClearLabels()
            Dim labelsToRemove As New List(Of Control)

            For Each ctrl As Control In _container.Controls
                If TypeOf ctrl Is System.Windows.Forms.Label OrElse TypeOf ctrl Is System.Windows.Forms.Label Then
                    labelsToRemove.Add(ctrl)
                End If
            Next

            For Each label As Control In labelsToRemove
                _container.Controls.Remove(label)
                label.Dispose()
            Next
        End Sub
    End Class
End Class