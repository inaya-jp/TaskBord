<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TaskBord
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        pnlMain = New Panel()
        pnlDateTime = New Panel()
        lblDateTime = New Label()
        menuChangeSettings = New ContextMenuStrip(components)
        ChangeSettingsToolStripMenuItem = New ToolStripMenuItem()
        tim1sec = New Timer(components)
        pnlWhiteBord = New Panel()
        pnlMain.SuspendLayout()
        pnlDateTime.SuspendLayout()
        menuChangeSettings.SuspendLayout()
        SuspendLayout()
        ' 
        ' pnlMain
        ' 
        pnlMain.Controls.Add(pnlWhiteBord)
        pnlMain.Controls.Add(pnlDateTime)
        pnlMain.Dock = DockStyle.Fill
        pnlMain.Location = New Point(0, 0)
        pnlMain.Margin = New Padding(0)
        pnlMain.Name = "pnlMain"
        pnlMain.Size = New Size(1258, 664)
        pnlMain.TabIndex = 0
        ' 
        ' pnlDateTime
        ' 
        pnlDateTime.Controls.Add(lblDateTime)
        pnlDateTime.Dock = DockStyle.Top
        pnlDateTime.Location = New Point(0, 0)
        pnlDateTime.Margin = New Padding(0)
        pnlDateTime.Name = "pnlDateTime"
        pnlDateTime.Size = New Size(1258, 100)
        pnlDateTime.TabIndex = 0
        ' 
        ' lblDateTime
        ' 
        lblDateTime.ContextMenuStrip = menuChangeSettings
        lblDateTime.Dock = DockStyle.Fill
        lblDateTime.Font = New Font("Yu Gothic UI", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(128))
        lblDateTime.ImageAlign = ContentAlignment.MiddleLeft
        lblDateTime.Location = New Point(0, 0)
        lblDateTime.Margin = New Padding(0)
        lblDateTime.Name = "lblDateTime"
        lblDateTime.Padding = New Padding(10, 0, 0, 0)
        lblDateTime.Size = New Size(1258, 100)
        lblDateTime.TabIndex = 0
        lblDateTime.Text = "Loading..."
        lblDateTime.TextAlign = ContentAlignment.MiddleLeft
        ' 
        ' menuChangeSettings
        ' 
        menuChangeSettings.ImageScalingSize = New Size(24, 24)
        menuChangeSettings.Items.AddRange(New ToolStripItem() {ChangeSettingsToolStripMenuItem})
        menuChangeSettings.Name = "menuChangeSettings"
        menuChangeSettings.Size = New Size(200, 36)
        ' 
        ' ChangeSettingsToolStripMenuItem
        ' 
        ChangeSettingsToolStripMenuItem.Name = "ChangeSettingsToolStripMenuItem"
        ChangeSettingsToolStripMenuItem.Size = New Size(199, 32)
        ChangeSettingsToolStripMenuItem.Text = "設定を変更する"
        ' 
        ' tim1sec
        ' 
        tim1sec.Enabled = True
        tim1sec.Interval = 1000
        ' 
        ' pnlWhiteBord
        ' 
        pnlWhiteBord.Dock = DockStyle.Fill
        pnlWhiteBord.Location = New Point(0, 100)
        pnlWhiteBord.Name = "pnlWhiteBord"
        pnlWhiteBord.Size = New Size(1258, 564)
        pnlWhiteBord.TabIndex = 1
        ' 
        ' TaskBord
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1258, 664)
        Controls.Add(pnlMain)
        Name = "TaskBord"
        Text = "TaskBord"
        pnlMain.ResumeLayout(False)
        pnlDateTime.ResumeLayout(False)
        menuChangeSettings.ResumeLayout(False)
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlMain As Panel
    Friend WithEvents pnlDateTime As Panel
    Friend WithEvents lblDateTime As Label
    Friend WithEvents tim1sec As Timer
    Friend WithEvents menuChangeSettings As ContextMenuStrip
    Friend WithEvents ChangeSettingsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents pnlWhiteBord As Panel
End Class
