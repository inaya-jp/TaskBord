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

        'ラベル情報を保存
        If lblManager IsNot lblManager Then lblManager.SaveLabels()
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
        ' 入力ダイアログを表示
        Dim inputText As String = InputBox(
        "ラベルのテキストを入力してください",
        "新しいラベル",
        "")

        ' キャンセルまたは空白の場合は作成しない
        If String.IsNullOrWhiteSpace(inputText) Then
            Return
        End If

        ' 新しいラベルを作成
        Dim newLabel As New System.Windows.Forms.Label()

        ' ラベルのプロパティを設定
        newLabel.Text = inputText
        newLabel.AutoSize = True
        newLabel.BackColor = Color.Transparent
        newLabel.Location = location ' ダブルクリックした位置

        ' フォントを適用
        Dim commonInstance As New Common()
        commonInstance.Set_customFont(newLabel)

        ' ダブルクリックイベントを追加
        AddHandler newLabel.MouseDown, AddressOf Label_MouseDown
        AddHandler newLabel.MouseMove, AddressOf Label_MouseMove
        AddHandler newLabel.MouseUp, AddressOf Label_MouseUp
        AddHandler newLabel.DoubleClick, AddressOf Label_DoubleClick

        ' パネルに追加
        pnlWhiteBord.Controls.Add(newLabel)

        ' すぐに保存
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

        Dim result As InputBoxResult = ShowInputBox(
        "変更する文字列を入力してください（空白で削除）",
        "文字列入力",
        label.Text)

        If result.IsCanceled Then
            ' キャンセルが押された場合
            Return ' 何もしない
        ElseIf result.Text = "" Then
            ' 空白が入力された場合、ラベルを削除
            pnlWhiteBord.Controls.Remove(label)
            label.Dispose()
        Else
            ' 通常のテキストが入力された場合
            label.Text = result.Text
        End If

        ' 変更を保存
        If lblManager IsNot Nothing Then
            lblManager.SaveLabels()
        End If

        'ダミーボタンにフォーカスを移動する
        btnDummy.Focus()
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
                    If TypeOf ctrl Is Label OrElse TypeOf ctrl Is Label Then
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
                ' 既存のラベルをクリア（オプション）
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
            ' TransparentLabelを使用
            Dim newLabel As New Label() With {
                .Text = info.Text,
                .Location = New Point(info.X, info.Y),
                .AutoSize = info.AutoSize,
                .Font = New Font(info.FontName, info.FontSize, info.FontStyle),
                .ForeColor = Color.FromArgb(info.ForeColorArgb)
            }

            ' カスタムフォントを適用（必要に応じて）
            Dim commonInstance As New Common()
            commonInstance.Set_customFont(newLabel)
            Dim taskbordInstance As New TaskBord

            ' ドラッグ機能を追加
            AddHandler newLabel.MouseDown, AddressOf taskbordInstance.Label_MouseDown
            AddHandler newLabel.MouseMove, AddressOf taskbordInstance.Label_MouseMove
            AddHandler newLabel.MouseUp, AddressOf taskbordInstance.Label_MouseUp
            AddHandler newLabel.DoubleClick, AddressOf taskbordInstance.Label_DoubleClick


            ' コンテナに追加
            _container.Controls.Add(newLabel)
        End Sub

        ''' <summary>
        ''' すべてのラベルをクリア
        ''' </summary>
        Public Sub ClearLabels()
            Dim labelsToRemove As New List(Of Control)

            For Each ctrl As Control In _container.Controls
                If TypeOf ctrl Is Label OrElse TypeOf ctrl Is Label Then
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
            If lblManager Is lblManager Then
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

    ' カスタムInputBoxの実装
    Public Class InputBoxResult
        Public Property Text As String
        Public Property IsCanceled As Boolean

        Public Sub New(text As String, isCanceled As Boolean)
            Me.Text = text
            Me.IsCanceled = isCanceled
        End Sub
    End Class

    Public Function ShowInputBox(prompt As String, title As String, defaultText As String) As InputBoxResult
        Dim form As New Form()
        Dim textBox As New TextBox()
        Dim buttonOk As New Button()
        Dim buttonCancel As New Button()
        Dim result As New InputBoxResult("", True) ' デフォルトはキャンセル

        With form
            .Text = title
            .FormBorderStyle = FormBorderStyle.FixedDialog
            .StartPosition = FormStartPosition.CenterScreen
            .MinimizeBox = False
            .MaximizeBox = False
            .Size = New Size(400, 150)
        End With

        Dim label As New Label() With {
            .Text = prompt,
            .Location = New Point(12, 12),
            .Size = New Size(360, 40),
            .AutoSize = False
        }

        With textBox
            .Text = defaultText
            .Location = New Point(12, 55)
            .Size = New Size(360, 20)
            .TabIndex = 0
        End With

        With buttonOk
            .Text = "OK"
            .Location = New Point(216, 85)
            .Size = New Size(75, 23)
            .TabIndex = 1
        End With

        With buttonCancel
            .Text = "キャンセル"
            .Location = New Point(297, 85)
            .Size = New Size(75, 23)
            .TabIndex = 2
        End With

        ' OKボタンのクリックイベント
        AddHandler buttonOk.Click, Sub()
                                       result = New InputBoxResult(textBox.Text, False)
                                       form.Close()
                                   End Sub

        ' キャンセルボタンのクリックイベント
        AddHandler buttonCancel.Click, Sub()
                                           result = New InputBoxResult("", True)
                                           form.Close()
                                       End Sub

        form.Controls.AddRange({label, textBox, buttonOk, buttonCancel})
        form.AcceptButton = buttonOk
        form.CancelButton = buttonCancel

        ' テキストボックスを選択状態に
        textBox.SelectAll()
        textBox.Focus()

        form.ShowDialog()
        Return result
    End Function
End Class