namespace tsosii_lab1
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox_source = new System.Windows.Forms.PictureBox();
            this.pictureBox_result = new System.Windows.Forms.PictureBox();
            this.openButton = new System.Windows.Forms.Button();
            this.executeButton = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.textBox_threshold = new System.Windows.Forms.TextBox();
            this.textBox_median = new System.Windows.Forms.TextBox();
            this.textBox_clustNumber = new System.Windows.Forms.TextBox();
            this.checkBox_medianFilter = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_source)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_result)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox_source
            // 
            this.pictureBox_source.Location = new System.Drawing.Point(12, 12);
            this.pictureBox_source.Name = "pictureBox_source";
            this.pictureBox_source.Size = new System.Drawing.Size(359, 293);
            this.pictureBox_source.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_source.TabIndex = 0;
            this.pictureBox_source.TabStop = false;
            // 
            // pictureBox_result
            // 
            this.pictureBox_result.Location = new System.Drawing.Point(377, 12);
            this.pictureBox_result.Name = "pictureBox_result";
            this.pictureBox_result.Size = new System.Drawing.Size(392, 309);
            this.pictureBox_result.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_result.TabIndex = 1;
            this.pictureBox_result.TabStop = false;
            // 
            // openButton
            // 
            this.openButton.Location = new System.Drawing.Point(543, 374);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(75, 23);
            this.openButton.TabIndex = 2;
            this.openButton.Text = "open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // executeButton
            // 
            this.executeButton.Location = new System.Drawing.Point(624, 374);
            this.executeButton.Name = "executeButton";
            this.executeButton.Size = new System.Drawing.Size(75, 23);
            this.executeButton.TabIndex = 3;
            this.executeButton.Text = "run";
            this.executeButton.UseVisualStyleBackColor = true;
            this.executeButton.Click += new System.EventHandler(this.executeButton_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(462, 374);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "save";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // textBox_threshold
            // 
            this.textBox_threshold.Location = new System.Drawing.Point(117, 322);
            this.textBox_threshold.Name = "textBox_threshold";
            this.textBox_threshold.Size = new System.Drawing.Size(100, 20);
            this.textBox_threshold.TabIndex = 5;
            this.textBox_threshold.Text = "170";
            // 
            // textBox_median
            // 
            this.textBox_median.Location = new System.Drawing.Point(117, 348);
            this.textBox_median.Name = "textBox_median";
            this.textBox_median.Size = new System.Drawing.Size(100, 20);
            this.textBox_median.TabIndex = 6;
            this.textBox_median.Text = "3";
            // 
            // textBox_clustNumber
            // 
            this.textBox_clustNumber.Location = new System.Drawing.Point(117, 374);
            this.textBox_clustNumber.Name = "textBox_clustNumber";
            this.textBox_clustNumber.Size = new System.Drawing.Size(100, 20);
            this.textBox_clustNumber.TabIndex = 7;
            this.textBox_clustNumber.Text = "2";
            // 
            // checkBox_medianFilter
            // 
            this.checkBox_medianFilter.AutoSize = true;
            this.checkBox_medianFilter.Location = new System.Drawing.Point(12, 350);
            this.checkBox_medianFilter.Name = "checkBox_medianFilter";
            this.checkBox_medianFilter.Size = new System.Drawing.Size(82, 17);
            this.checkBox_medianFilter.TabIndex = 8;
            this.checkBox_medianFilter.Text = "median filter";
            this.checkBox_medianFilter.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(896, 420);
            this.Controls.Add(this.checkBox_medianFilter);
            this.Controls.Add(this.textBox_clustNumber);
            this.Controls.Add(this.textBox_median);
            this.Controls.Add(this.textBox_threshold);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.executeButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.pictureBox_result);
            this.Controls.Add(this.pictureBox_source);
            this.Name = "Form1";
            this.Text = "lab1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_source)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_result)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox_source;
        private System.Windows.Forms.PictureBox pictureBox_result;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button executeButton;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.TextBox textBox_threshold;
        private System.Windows.Forms.TextBox textBox_median;
        private System.Windows.Forms.TextBox textBox_clustNumber;
        private System.Windows.Forms.CheckBox checkBox_medianFilter;


    }
}

