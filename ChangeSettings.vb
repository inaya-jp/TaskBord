Imports System.Xml

Public Class ChangeSettings
    ' 受け取ったデータを格納するフィールド
    Private _receivedData As String

    ' 設定名と対応する設定値を取得するためのマップを作成する
    Private ReadOnly _settingMap As New Dictionary(Of String, Func(Of String)) From {
        {"lblDateTime", Function() My.Settings.DateTimeFormat}
    }

    ' 受け取ったデータに基づいて対応する設定値を取得するメソッド
    Private Function GetCorrespondingSetting(receivedLabel As String) As String
        If _settingMap.ContainsKey(receivedLabel) Then
            '一致するキーが存在する場合、その関数を呼び出して値を取得
            Return _settingMap(receivedLabel).Invoke()
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
        grdChangeSettings.Rows.Add(_receivedData, GetCorrespondingSetting(_receivedData))

    End Sub

    Private Sub ChangeSettings_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        ' DataGridViewの値を設定に保存
        Dim value = grdChangeSettings.Rows(0).Cells(1).Value.ToString()

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
        My.Settings.DateTimeFormat = value
    End Sub
End Class