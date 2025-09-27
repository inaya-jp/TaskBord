Imports System.Xml

Public Class ChangeSettings
    ' 受け取ったデータを格納するフィールド
    Private _receivedData As String

    ' 設定名と対応する設定値を取得するためのマップを作成する
    Private ReadOnly _settingMap As New Dictionary(Of String, (SettingValue As Func(Of String), IsAllowNull As Boolean)) From {
    {"lblDateTime", (Function() My.Settings.DateTimeFormat, True)}
    }

    ' 受け取ったデータに基づいて対応する設定値を取得するメソッド
    Private Function GetSettingValue(receivedLabel As String) As String
        If _settingMap.ContainsKey(receivedLabel) Then
            Return _settingMap(receivedLabel).SettingValue()
        End If
        Return String.Empty

        '存在しない場合は空文字を返す
        Return String.Empty
    End Function

    Private Function GetIsAllowNull(key As String) As Boolean
        If _settingMap.ContainsKey(key) Then
            Return _settingMap(key).IsAllowNull
        End If

        '存在しない場合は空文字を返す
        Return String.Empty
    End Function

    Public Sub New(receivedData As String)
        'フォーム呼び出し時に受け取ったデータをフィールドに格納
        InitializeComponent()
        _receivedData = receivedData
    End Sub

    Private Sub ChangeSettings_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        ' フォームがロードされたときに、受け取ったデータを使用してDataGridViewに行を追加
        grdChangeSettings.Rows.Add(_receivedData, GetSettingValue(_receivedData))

    End Sub

    Private Sub ChangeSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        Dim value As String = ""

        ' DataGridViewの値がNothingでないことを確認
        If grdChangeSettings.Rows(0).Cells(1).Value IsNot Nothing Then
            ' DataGridViewの値を設定に保存
            value = grdChangeSettings.Rows(0).Cells(1).Value.ToString()
        Else
            ' DataGridViewの値が空白を許容しない場合、エラーメッセージを表示して終了
            If GetIsAllowNull(_receivedData) = False Then
                MessageBox.Show("この設定項目は空にできません。変更を保存できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If
        End If


        Select Case _receivedData
            ' lblDateTimeの場合、日付形式の妥当性をチェック
            Case "lblDateTime"
                Try
                    Dim dummy = DateTime.Now.ToString(value)
                Catch ex As Exception
                    MessageBox.Show("無効な日付形式です。変更を保存できませんでした。", "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End Try
            Case Else
                ' 他の設定項目に対する処理をここに追加
        End Select

        ' 設定を保存
        My.Settings.DateTimeFormat = Value
    End Sub
End Class