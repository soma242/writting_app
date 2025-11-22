namespace writting_app
{
    partial class SelectPanel
    {
        /// <summary> 
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.singleClickTreeView1 = new writting_app.SingleClickTreeView();
            this.SuspendLayout();
            // 
            // singleClickTreeView1
            // 
            this.singleClickTreeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.singleClickTreeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.singleClickTreeView1.Font = new System.Drawing.Font("MS UI Gothic", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.singleClickTreeView1.Location = new System.Drawing.Point(0, 0);
            this.singleClickTreeView1.Name = "singleClickTreeView1";
            this.singleClickTreeView1.ShowLines = false;
            this.singleClickTreeView1.ShowPlusMinus = false;
            this.singleClickTreeView1.ShowRootLines = false;
            this.singleClickTreeView1.Size = new System.Drawing.Size(150, 150);
            this.singleClickTreeView1.TabIndex = 0;
            this.singleClickTreeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.singleClickTreeView1_AfterSelect);
            // 
            // SelectPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.singleClickTreeView1);
            this.Name = "SelectPanel";
            this.Load += new System.EventHandler(this.SelectPanel_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private writting_app.SingleClickTreeView singleClickTreeView1;
    }
}
