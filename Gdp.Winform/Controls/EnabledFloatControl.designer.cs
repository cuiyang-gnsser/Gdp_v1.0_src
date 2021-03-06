﻿namespace Gdp.Winform.Controls
{
    partial class EnabledFloatControl
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.checkBox_enabled = new System.Windows.Forms.CheckBox();
            this.label_name = new System.Windows.Forms.Label();
            this.textBox_value = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // checkBox_enabled
            // 
            this.checkBox_enabled.AutoSize = true;
            this.checkBox_enabled.Dock = System.Windows.Forms.DockStyle.Right;
            this.checkBox_enabled.Location = new System.Drawing.Point(91, 0);
            this.checkBox_enabled.Name = "checkBox_enabled";
            this.checkBox_enabled.Size = new System.Drawing.Size(60, 23);
            this.checkBox_enabled.TabIndex = 0;
            this.checkBox_enabled.Text = "Enable";
            this.checkBox_enabled.UseVisualStyleBackColor = true;
            this.checkBox_enabled.CheckedChanged += new System.EventHandler(this.checkBox_enabled_CheckedChanged);
            // 
            // label_name
            // 
            this.label_name.AutoSize = true;
            this.label_name.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_name.Location = new System.Drawing.Point(0, 0);
            this.label_name.Name = "label_name";
            this.label_name.Size = new System.Drawing.Size(41, 12);
            this.label_name.TabIndex = 1;
            this.label_name.Text = "Name：";
            this.label_name.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox_value
            // 
            this.textBox_value.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox_value.Location = new System.Drawing.Point(41, 0);
            this.textBox_value.Name = "textBox_value";
            this.textBox_value.Size = new System.Drawing.Size(50, 21);
            this.textBox_value.TabIndex = 2;
            this.textBox_value.Text = "0.1";
            // 
            // EnabledFloatControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox_value);
            this.Controls.Add(this.label_name);
            this.Controls.Add(this.checkBox_enabled);
            this.Name = "EnabledFloatControl";
            this.Size = new System.Drawing.Size(151, 23);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBox_enabled;
        private System.Windows.Forms.Label label_name;
        private System.Windows.Forms.TextBox textBox_value;
    }
}
