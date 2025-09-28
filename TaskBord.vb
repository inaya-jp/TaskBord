Imports System.Drawing.Text
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Collections.Specialized
Imports Newtonsoft.Json
Imports System.Runtime.CompilerServices

Public Class TaskBord

    Private lblManager As LabelManager

    Private Sub TaskBord_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' 各ラベルにカスタムフォントを設定
        Dim commonInstance As New Common()
        commonInstance.Set_customFont(lblDateTime)

        ' LabelManagerを初期化
        lblManager = New LabelManager(pnlWhiteBord)

        ' 起動時に自動的にラベルを復元
        lblManager.LoadLabels()

    End Sub

    Private Sub tim1sec_Tick(sender As Object, e As EventArgs) Handles tim1sec.Tick
        ' 1秒ごとに現在の日時を更新
        lblDateTime.Text = DateTime.Now.ToString(My.Settings.DateTimeFormat)

        'ラベル情報を自動保存
        SaveLabelsIfPossible()
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

    Private Sub pnlWhiteBord_DoubleClick(sender As Object, e As EventArgs) Handles pnlWhiteBord.DoubleClick
        ' MouseEventArgsとしてキャスト
        Dim mouseEventArgs As MouseEventArgs = DirectCast(e, MouseEventArgs)

        ' ダブルクリックした位置を取得
        Dim clickLocation As Point = mouseEventArgs.Location

        ' その位置にラベルを追加
        AddNewLabelAtLocation(clickLocation)

        ' ダミーボタンにフォーカスを移動
        btnDummy.Focus()
    End Sub

    ' 指定した位置にラベルを追加するメソッド
    Private Sub AddNewLabelAtLocation(location As Point)
        ' カスタム入力ダイアログを表示
        Dim dialogResult As LabelDialogResult = ShowLabelDialog("新しいラベル", "", Color.Black, 12.0F)

        ' キャンセルまたは空白の場合は作成しない
        If dialogResult.IsCanceled OrElse String.IsNullOrWhiteSpace(dialogResult.Text) Then
            Return
        End If

        ' 新しいラベルを作成
        Dim newLabel As New System.Windows.Forms.Label()

        ' ラベルのプロパティを設定
        newLabel.Text = dialogResult.Text
        newLabel.AutoSize = True
        newLabel.BackColor = Color.Transparent
        newLabel.Location = location ' ダブルクリックした位置
        newLabel.ForeColor = dialogResult.ForeColor

        ' フォントを適用
        Dim commonInstance As New Common()
        commonInstance.Set_customFont(newLabel)
        ' サイズを変更
        newLabel.Font = New Font(newLabel.Font.FontFamily, dialogResult.FontSize, newLabel.Font.Style)

        ' イベントハンドラを追加
        AddLabelEventHandlers(newLabel)

        ' パネルに追加
        pnlWhiteBord.Controls.Add(newLabel)

        ' すぐに保存
        If lblManager IsNot Nothing Then
            lblManager.SaveLabels()
        End If
    End Sub

    ' ラベルにイベントハンドラを追加するメソッド
    Private Sub AddLabelEventHandlers(label As Label)
        AddHandler label.MouseDown, AddressOf Label_MouseDown
        AddHandler label.MouseMove, AddressOf Label_MouseMove
        AddHandler label.MouseUp, AddressOf Label_MouseUp
        AddHandler label.DoubleClick, AddressOf Label_DoubleClick

        ' 右クリックメニューを追加
        Dim contextMenu As New ContextMenuStrip()

        ' プロパティ変更メニュー
        Dim editPropsItem As New ToolStripMenuItem("プロパティを変更")
        AddHandler editPropsItem.Click, Sub() EditLabelProperties(label)
        contextMenu.Items.Add(editPropsItem)

        ' 区切り線
        contextMenu.Items.Add(New ToolStripSeparator())

        ' 削除メニュー
        Dim deleteItem As New ToolStripMenuItem("削除")
        AddHandler deleteItem.Click, Sub() DeleteLabel(label)
        contextMenu.Items.Add(deleteItem)

        label.ContextMenuStrip = contextMenu
    End Sub

    ' ラベルのプロパティを編集
    Private Sub EditLabelProperties(label As Label)
        Dim dialogResult As LabelDialogResult = ShowLabelDialog(
            "プロパティ変更",
            label.Text,
            label.ForeColor,
            label.Font.Size)

        If Not dialogResult.IsCanceled Then
            If Not String.IsNullOrWhiteSpace(dialogResult.Text) Then
                'テキストが空白でない場合、テキストを更新
                label.Text = dialogResult.Text
            Else
                'テキストが空白の場合、ラベルを削除
                DeleteLabel(label)
            End If
            label.ForeColor = dialogResult.ForeColor

            ' フォントサイズを変更
            label.Font = New Font(label.Font.FontFamily, dialogResult.FontSize, label.Font.Style)

            SaveLabelsIfPossible()
        End If

        btnDummy.Focus()
    End Sub

    ' ラベルを削除
    Private Sub DeleteLabel(label As Label)

        pnlWhiteBord.Controls.Remove(label)
        label.Dispose()
        SaveLabelsIfPossible()

    End Sub

    ' ラベル保存のヘルパーメソッド
    Private Sub SaveLabelsIfPossible()
        If lblManager IsNot Nothing Then
            lblManager.SaveLabels()
        End If
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

        Dim label As Label = DirectCast(sender, Label)
        label.BringToFront() ' ラベルを最前面に移動
    End Sub

    ' MouseUpイベントでドラッグ終了
    Private Sub Label_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            isDragging = False

            ' ドラッグ終了時に保存
            SaveLabelsIfPossible()
        End If
    End Sub

    ' MouseMoveイベントでラベルを移動
    Private Sub Label_MouseMove(sender As Object, e As MouseEventArgs)
        If isDragging Then
            Dim label As Label = DirectCast(sender, Label)

            ' 新しい位置を計算
            Dim newX = label.Left + (e.X - lastPoint.X)
            Dim newY = label.Top + (e.Y - lastPoint.Y)

            ' ラベルの位置を更新
            label.Location = New Point(newX, newY)
        End If
    End Sub

    Private Sub Label_DoubleClick(sender As Object, e As EventArgs)
        Dim label As Label = DirectCast(sender, Label)
        EditLabelProperties(label)
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

        Public Sub New(label As Label)
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
        Private _parentForm As TaskBord

        Public Sub New(container As Control)
            _container = container
            ' 親フォームの参照を取得
            _parentForm = DirectCast(container.FindForm(), TaskBord)
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

                ' コンテナ内のすべてのLabelを取得
                For Each ctrl As Control In _container.Controls
                    If TypeOf ctrl Is Label Then
                        Dim label As Label = DirectCast(ctrl, Label)
                        Dim info As New LabelInfo(label)

                        ' JSONにシリアライズして保存
                        Dim json As String = JsonConvert.SerializeObject(info)
                        My.Settings.LabelData.Add(json)
                    End If
                Next

                ' 設定を保存
                My.Settings.Save()

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
                ' 既存のラベルをクリア
                ClearLabels()

                If My.Settings.LabelData Is Nothing OrElse My.Settings.LabelData.Count = 0 Then
                    Return
                End If

                ' 保存されたラベルを復元
                For Each json As String In My.Settings.LabelData
                    If Not String.IsNullOrEmpty(json) Then
                        Dim info As LabelInfo = JsonConvert.DeserializeObject(Of LabelInfo)(json)
                        CreateLabelFromInfo(info)
                    End If
                Next

            Catch ex As Exception
                MessageBox.Show($"復元中にエラーが発生しました: {ex.Message}",
                              "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End Sub

        ''' <summary>
        ''' LabelInfoからラベルを作成
        ''' </summary>
        Private Sub CreateLabelFromInfo(info As LabelInfo)
            Dim newLabel As New Label() With {
                .Text = info.Text,
                .Location = New Point(info.X, info.Y),
                .AutoSize = info.AutoSize,
                .Font = New Font(info.FontName, info.FontSize, info.FontStyle),
                .ForeColor = Color.FromArgb(info.ForeColorArgb),
                .BackColor = Color.Transparent
            }

            ' カスタムフォントを適用
            Dim commonInstance As New Common()
            commonInstance.Set_customFont(newLabel)
            ' フォントサイズを保持
            newLabel.Font = New Font(newLabel.Font.FontFamily, info.FontSize, newLabel.Font.Style)

            ' イベントハンドラを追加
            _parentForm.AddLabelEventHandlers(newLabel)

            ' コンテナに追加
            _container.Controls.Add(newLabel)
        End Sub

        ''' <summary>
        ''' すべてのラベルをクリア
        ''' </summary>
        Public Sub ClearLabels()
            Dim labelsToRemove As New List(Of Control)

            For Each ctrl As Control In _container.Controls
                If TypeOf ctrl Is Label Then
                    labelsToRemove.Add(ctrl)
                End If
            Next

            For Each label As Control In labelsToRemove
                _container.Controls.Remove(label)
                label.Dispose()
            Next
        End Sub
    End Class

    Private Sub TaskBord_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        Dim closingSave As Boolean = False
        Dim retryCount As Integer = 0

        While closingSave = False
            If lblManager IsNot Nothing Then
                lblManager.SaveLabels()
                closingSave = True
            Else
                ' 保存に失敗した場合、再試行
                retryCount += 1
            End If

            ' 再試行回数が多すぎる場合、クローズをキャンセルする
            If retryCount > 5 Then
                MessageBox.Show("ラベルの保存に失敗しました。アプリケーションを閉じることができません。",
                                "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                e.Cancel = True
                Exit While
            End If
        End While
    End Sub

    ' ラベルダイアログの結果クラス
    Public Class LabelDialogResult
        Public Property Text As String
        Public Property ForeColor As Color
        Public Property FontSize As Single
        Public Property IsCanceled As Boolean

        Public Sub New(text As String, foreColor As Color, fontSize As Single, isCanceled As Boolean)
            Me.Text = text
            Me.ForeColor = foreColor
            Me.FontSize = fontSize
            Me.IsCanceled = isCanceled
        End Sub
    End Class

    ' カスタムラベルダイアログ
    Public Function ShowLabelDialog(title As String, defaultText As String, defaultColor As Color, defaultSize As Single) As LabelDialogResult
        Dim form As New Form()
        Dim textBox As New TextBox()
        Dim colorButton As New Button()
        Dim sizeNumeric As New NumericUpDown()
        Dim buttonOk As New Button()
        Dim buttonCancel As New Button()
        Dim selectedColor As Color = defaultColor
        Dim result As New LabelDialogResult("", Color.Black, 12.0F, True) ' デフォルトはキャンセル

        With form
            .Text = title
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .StartPosition = FormStartPosition.CenterScreen
            .MinimizeBox = False
            .MaximizeBox = False
            .Size = New Size(400, 220)
        End With

        ' テキストラベル
        Dim lblText As New Label() With {
            .Text = "テキスト:",
            .Location = New Point(12, 15),
            .Size = New Size(60, 20)
        }

        ' テキストボックス
        With textBox
            .Text = defaultText
            .Location = New Point(80, 12)
            .Size = New Size(280, 20)
            .TabIndex = 0
        End With

        ' 色ラベル
        Dim lblColor As New Label() With {
            .Text = "文字色:",
            .Location = New Point(12, 50),
            .Size = New Size(60, 20)
        }

        ' 色選択ボタン
        With colorButton
            .Text = "色を選択"
            .Location = New Point(80, 47)
            .Size = New Size(100, 25)
            .BackColor = defaultColor
            .TabIndex = 1
        End With

        ' サイズラベル
        Dim lblSize As New Label() With {
            .Text = "サイズ:",
            .Location = New Point(12, 85),
            .Size = New Size(60, 20)
        }

        ' サイズ選択
        With sizeNumeric
            .Location = New Point(80, 82)
            .Size = New Size(80, 20)
            .Minimum = 6
            .Maximum = 72
            .DecimalPlaces = 1
            .Value = CDec(defaultSize)
            .TabIndex = 2
        End With

        ' OKボタン
        With buttonOk
            .Text = "OK"
            .Location = New Point(210, 150)
            .Size = New Size(75, 25)
            .TabIndex = 3
        End With

        ' キャンセルボタン
        With buttonCancel
            .Text = "キャンセル"
            .Location = New Point(291, 150)
            .Size = New Size(75, 25)
            .TabIndex = 4
        End With

        ' 色選択ボタンのクリックイベント
        AddHandler colorButton.Click, Sub()
                                          Using colorDialog As New ColorDialog()
                                              colorDialog.Color = selectedColor
                                              colorDialog.FullOpen = True
                                              If colorDialog.ShowDialog() = DialogResult.OK Then
                                                  selectedColor = colorDialog.Color
                                                  colorButton.BackColor = selectedColor
                                              End If
                                          End Using
                                      End Sub

        ' OKボタンのクリックイベント
        AddHandler buttonOk.Click, Sub()
                                       result = New LabelDialogResult(
                                           textBox.Text,
                                           selectedColor,
                                           CSng(sizeNumeric.Value),
                                           False)
                                       form.Close()
                                   End Sub

        ' キャンセルボタンのクリックイベント
        AddHandler buttonCancel.Click, Sub()
                                           result = New LabelDialogResult("", Color.Black, 12.0F, True)
                                           form.Close()
                                       End Sub

        form.Controls.AddRange({lblText, textBox, lblColor, colorButton, lblSize, sizeNumeric, buttonOk, buttonCancel})
        form.AcceptButton = buttonOk
        form.CancelButton = buttonCancel

        ' テキストボックスを選択状態に
        textBox.SelectAll()
        textBox.Focus()

        form.ShowDialog()
        Return result
    End Function
End Class