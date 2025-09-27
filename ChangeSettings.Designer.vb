<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ChangeSettings
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
        pnlMain = New Panel()
        grdChangeSettings = New DataGridView()
        Title = New DataGridViewTextBoxColumn()
        Value = New DataGridViewTextBoxColumn()
        pnlMain.SuspendLayout()
        CType(grdChangeSettings, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' pnlMain
        ' 
        pnlMain.Controls.Add(grdChangeSettings)
        pnlMain.Dock = DockStyle.Fill
        pnlMain.Location = New Point(0, 0)
        pnlMain.Margin = New Padding(0)
        pnlMain.Name = "pnlMain"
        pnlMain.Size = New Size(800, 244)
        pnlMain.TabIndex = 1
        ' 
        ' grdChangeSettings
        ' 
        grdChangeSettings.AllowUserToAddRows = False
        grdChangeSettings.AllowUserToDeleteRows = False
        grdChangeSettings.AllowUserToResizeColumns = False
        grdChangeSettings.AllowUserToResizeRows = False
        grdChangeSettings.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        grdChangeSettings.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCells
        grdChangeSettings.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize
        grdChangeSettings.Columns.AddRange(New DataGridViewColumn() {Title, Value})
        grdChangeSettings.Location = New Point(0, 0)
        grdChangeSettings.Name = "grdChangeSettings"
        grdChangeSettings.RowHeadersVisible = False
        grdChangeSettings.RowHeadersWidth = 62
        grdChangeSettings.Size = New Size(800, 244)
        grdChangeSettings.TabIndex = 0
        ' 
        ' Title
        ' 
        Title.HeaderText = "定数名"
        Title.MinimumWidth = 8
        Title.Name = "Title"
        Title.ReadOnly = True
        ' 
        ' Value
        ' 
        Value.HeaderText = "値"
        Value.MinimumWidth = 8
        Value.Name = "Value"
        ' 
        ' ChangeSettings
        ' 
        AutoScaleDimensions = New SizeF(10F, 25F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(800, 244)
        Controls.Add(pnlMain)
        MaximizeBox = False
        MinimizeBox = False
        Name = "ChangeSettings"
        Text = "ChangeSettings"
        pnlMain.ResumeLayout(False)
        CType(grdChangeSettings, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents pnlMain As Panel
    Friend WithEvents grdChangeSettings As DataGridView
    Friend WithEvents Title As DataGridViewTextBoxColumn
    Friend WithEvents Value As DataGridViewTextBoxColumn
End Class
