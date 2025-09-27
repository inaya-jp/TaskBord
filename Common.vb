Imports System.Data.SqlTypes
Imports System.Drawing.Text
Imports System.IO

Public Class Common


    Public Sub Set_customFont(targetControl As Label, Optional fontSize As Single = 16.0F, Optional fontStyle As FontStyle = FontStyle.Regular)
        ' PrivateFontCollectionのインスタンスを作成
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

        ' フォントファミリが存在する場合
        If pfc.Families.Length > 0 Then
            ' フォントを作成
            Dim customFont As New Font(pfc.Families(0), fontSize, FontStyle.Bold)

            ' ターゲットコントロールにフォントを設定
            With targetControl
                .UseCompatibleTextRendering = True
                .Font = customFont
            End With
        End If

    End Sub
End Class
