namespace TopServer2k18
{
    partial class FormMain
    {
        /// <summary>
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.rtb_console = new System.Windows.Forms.RichTextBox();
            this.pb_image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_image)).BeginInit();
            this.SuspendLayout();
            // 
            // rtb_console
            // 
            this.rtb_console.Location = new System.Drawing.Point(461, 12);
            this.rtb_console.Name = "rtb_console";
            this.rtb_console.Size = new System.Drawing.Size(460, 433);
            this.rtb_console.TabIndex = 0;
            this.rtb_console.Text = "";
            // 
            // pb_image
            // 
            this.pb_image.Location = new System.Drawing.Point(12, 12);
            this.pb_image.Name = "pb_image";
            this.pb_image.Size = new System.Drawing.Size(443, 433);
            this.pb_image.TabIndex = 1;
            this.pb_image.TabStop = false;
            // 
            // FormMain
            // 
            this.ClientSize = new System.Drawing.Size(935, 457);
            this.Controls.Add(this.pb_image);
            this.Controls.Add(this.rtb_console);
            this.Name = "FormMain";
            this.Text = "TopServer2k18";
            ((System.ComponentModel.ISupportInitialize)(this.pb_image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtb_console;
        private System.Windows.Forms.PictureBox pb_image;
    }
}

