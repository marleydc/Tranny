namespace Tranny
{
    partial class XMLForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
	        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(XMLForm));
            this.components = new System.ComponentModel.Container();
            this.TextEditor = new ICSharpCode.TextEditor.TextEditorControl();
            this.SuspendLayout();
            // 
            // TextEditor
            // 
            this.TextEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextEditor.IsReadOnly = false;
            this.TextEditor.Font = new System.Drawing.Font("Consolas", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextEditor.Location = new System.Drawing.Point(0, 0);
            this.TextEditor.Name = "TextEditor";
            this.TextEditor.Size = new System.Drawing.Size(730, 420);
            this.TextEditor.TabIndex = 0;
            this.TextEditor.Text = "";
            this.TextEditor.TextChanged += new System.EventHandler(this.TextEditor_TextChanged);
            this.TextEditor.SetHighlighting("XML");
            this.TextEditor.ShowLineNumbers = true;
            this.TextEditor.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextEditor_KeyPress);
            this.TextEditor.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextEditor_KeyUp);
            // 
            // XMLForm
            // 
            this.ClientSize = new System.Drawing.Size(730, 420);
            this.Controls.Add(this.TextEditor);
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "XMLForm";
            this.Text = "New XML";
            this.ResumeLayout(false);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.TranForm_FormClosing);

        }

        #endregion
    }
}
