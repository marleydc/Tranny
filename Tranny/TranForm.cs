using ICSharpCode.TextEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Tranny
{
    public class TranForm : System.Windows.Forms.Form
    {
        public ICSharpCode.TextEditor.TextEditorControl TextEditor { get; set; }
        public bool Dirty { get; set; }
        private int _findStartPos = 0;
        private string _findString = "";

        /*
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // TranForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "TranForm";
            this.ResumeLayout(false);
        }
        */

        public void Save()
        {
            try
            {
                if (this.Text.StartsWith("New"))
                    this.SaveAs();
                else
                {
                    File.WriteAllText(this.Text, this.TextEditor.Text);
                    this.Dirty = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void SaveAs()
        {
            try
            {
                using (var sfd = new SaveFileDialog())
                {
                    sfd.Filter = "XML files|*.xml;*.xsl;*.xslt;*.xsd|All files|*.*";
                    sfd.Title = "Save File";
                    sfd.FilterIndex = 2;

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(sfd.FileName, this.TextEditor.Text);
                        this.Text = sfd.FileName;
                        this.Dirty = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Find()
        {
            DialogResult result = System.Windows.Forms.DialogResult.None;
            result = InputBox.Show("Find",
                "Searchstring",
                "Value",
                out _findString);

            if (result == System.Windows.Forms.DialogResult.OK)
                FindNext(); 
        }

        public void FindNext()
        {
            int lastPos = this.TextEditor.ActiveTextAreaControl.TextArea.Caret.Offset + 2;
            lastPos = this.TextEditor.Text.IndexOf("\n", lastPos);

            int pos = this.TextEditor.Text.IndexOf(_findString, lastPos);
            if (pos > 0)
            {
                var lineNumber = this.TextEditor.Text.Take(pos).Count(c => c == '\n') + 1;
                this.TextEditor.ActiveTextAreaControl.JumpTo(lineNumber - 1);

                TextLocation start = new TextLocation(0, lineNumber - 1);
                TextLocation end = new TextLocation(0, lineNumber);
                this.TextEditor.ActiveTextAreaControl.SelectionManager.SetSelection(start, end);
            }
        }

        protected void TranForm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            if (this.Dirty)
            {
                DialogResult result = MessageBox.Show("Save changes to " + this.Text + "?", "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result.Equals(DialogResult.Yes))
                {
                    Save();
                }
                else if (result.Equals(DialogResult.Cancel))
                    e.Cancel = true;
            }
        }

        protected void TextEditor_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            this.Dirty = true;
        }

        public void TextEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.Oemcomma)
            {
                MessageBox.Show("LT");
            }
        }

        protected void TextEditor_TextChanged(object sender, EventArgs e)
        {
            this.Dirty = true;
        }

        /// Indent the XML
        public void Indent()
        {
            var element = XElement.Parse(this.TextEditor.Text);

            using (MemoryStream memStream = new MemoryStream())
            {
                var settings = new XmlWriterSettings();
                settings.OmitXmlDeclaration = false;
                settings.Indent = true;
                settings.NewLineOnAttributes = false;
                settings.Encoding = Encoding.UTF8;

                using (var xmlWriter = XmlWriter.Create(memStream, settings))
                {
                    element.Save(xmlWriter);
                    xmlWriter.Flush();
                    xmlWriter.Close();
                }
                memStream.Position = 0;

                using (XmlReader reader = XmlReader.Create(memStream))
                {
                    reader.Read();
                    reader.MoveToContent();
                    this.TextEditor.Text = reader.ReadOuterXml();
                    reader.Close();
                }
                memStream.Close();
            }
        }
    }
}
