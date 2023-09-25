namespace WY_App.UserControls
{
    partial class numControl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.name = new System.Windows.Forms.GroupBox();
            this.uiDoubleUpDown1 = new Sunny.UI.UIDoubleUpDown();
            this.x = new System.Windows.Forms.Label();
            this.uiDoubleUpDown2 = new Sunny.UI.UIDoubleUpDown();
            this.Y = new System.Windows.Forms.Label();
            this.name.SuspendLayout();
            this.SuspendLayout();
            // 
            // name
            // 
            this.name.Controls.Add(this.uiDoubleUpDown2);
            this.name.Controls.Add(this.Y);
            this.name.Controls.Add(this.uiDoubleUpDown1);
            this.name.Controls.Add(this.x);
            this.name.ForeColor = System.Drawing.Color.White;
            this.name.Location = new System.Drawing.Point(3, 3);
            this.name.Name = "name";
            this.name.Size = new System.Drawing.Size(232, 72);
            this.name.TabIndex = 0;
            this.name.TabStop = false;
            this.name.Text = "点";
            // 
            // uiDoubleUpDown1
            // 
            this.uiDoubleUpDown1.Decimal = 5;
            this.uiDoubleUpDown1.DecimalPlaces = 5;
            this.uiDoubleUpDown1.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.uiDoubleUpDown1.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.uiDoubleUpDown1.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDoubleUpDown1.ForeColor = System.Drawing.Color.White;
            this.uiDoubleUpDown1.Location = new System.Drawing.Point(28, 10);
            this.uiDoubleUpDown1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiDoubleUpDown1.MinimumSize = new System.Drawing.Size(100, 0);
            this.uiDoubleUpDown1.Name = "uiDoubleUpDown1";
            this.uiDoubleUpDown1.RectColor = System.Drawing.Color.Black;
            this.uiDoubleUpDown1.ShowText = false;
            this.uiDoubleUpDown1.Size = new System.Drawing.Size(202, 20);
            this.uiDoubleUpDown1.Style = Sunny.UI.UIStyle.Custom;
            this.uiDoubleUpDown1.TabIndex = 3;
            this.uiDoubleUpDown1.Text = "uiDoubleUpDown1";
            this.uiDoubleUpDown1.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // x
            // 
            this.x.AutoSize = true;
            this.x.ForeColor = System.Drawing.Color.White;
            this.x.Location = new System.Drawing.Point(10, 18);
            this.x.Name = "x";
            this.x.Size = new System.Drawing.Size(11, 12);
            this.x.TabIndex = 2;
            this.x.Text = "X";
            // 
            // uiDoubleUpDown2
            // 
            this.uiDoubleUpDown2.Decimal = 5;
            this.uiDoubleUpDown2.DecimalPlaces = 5;
            this.uiDoubleUpDown2.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.uiDoubleUpDown2.FillColor2 = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.uiDoubleUpDown2.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.uiDoubleUpDown2.ForeColor = System.Drawing.Color.White;
            this.uiDoubleUpDown2.Location = new System.Drawing.Point(28, 40);
            this.uiDoubleUpDown2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.uiDoubleUpDown2.MinimumSize = new System.Drawing.Size(100, 0);
            this.uiDoubleUpDown2.Name = "uiDoubleUpDown2";
            this.uiDoubleUpDown2.RectColor = System.Drawing.Color.Black;
            this.uiDoubleUpDown2.ShowText = false;
            this.uiDoubleUpDown2.Size = new System.Drawing.Size(202, 20);
            this.uiDoubleUpDown2.Style = Sunny.UI.UIStyle.Custom;
            this.uiDoubleUpDown2.TabIndex = 5;
            this.uiDoubleUpDown2.Text = "uiDoubleUpDown2";
            this.uiDoubleUpDown2.TextAlignment = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Y
            // 
            this.Y.AutoSize = true;
            this.Y.ForeColor = System.Drawing.Color.White;
            this.Y.Location = new System.Drawing.Point(10, 48);
            this.Y.Name = "Y";
            this.Y.Size = new System.Drawing.Size(11, 12);
            this.Y.TabIndex = 4;
            this.Y.Text = "Y";
            // 
            // numControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.Controls.Add(this.name);
            this.Name = "numControl";
            this.Size = new System.Drawing.Size(235, 77);
            this.name.ResumeLayout(false);
            this.name.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox name;
        private Sunny.UI.UIDoubleUpDown uiDoubleUpDown2;
        private System.Windows.Forms.Label Y;
        private Sunny.UI.UIDoubleUpDown uiDoubleUpDown1;
        private System.Windows.Forms.Label x;
    }
}
