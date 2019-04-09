using ICSharpCode.TextEditor;
using Saxon.Api;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Tranny
{
    public partial class Main : Form
    {
        private int _findStartPos = 0;
        private string _findString = "";
        public Main()
        {
            InitializeComponent();
        }

        private void tsmNew_Click(object sender, EventArgs e)
        {
            XSLForm newMDIChild = new XSLForm();
            // Set the Parent Form of the Child window.  
            newMDIChild.MdiParent = this;
            // Display the new form.  
            newMDIChild.Show();
        }


        private void tsmNewXML_Click(object sender, EventArgs e)
        {
            XMLForm newMDIChild = new XMLForm();
            // Set the Parent Form of the Child window.  
            newMDIChild.MdiParent = this;
            // Display the new form.  
            newMDIChild.Show(); 
        }

        private void tsmTransform_Click(object sender, EventArgs e)
        {
            Transform();
        }

        private void Transform()
        { 
            var processor = new Processor();
            XsltCompiler compiler = processor.NewXsltCompiler();

            XMLForm frmXML = null;
            XSLForm frmXSL = null;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                // Find the XSL form and XML forms
                foreach (Form frm in this.MdiChildren)
                {
                    if (frm.Tag == null)
                    {
                        if (frm.Name == "XMLForm")
                            frmXML = (XMLForm)frm;
                        else if (frm.Name == "XSLForm")
                            frmXSL = (XSLForm)frm;
                    }
                }

                if (frmXML == null)
                    throw new ApplicationException("No XML form found");
                if (frmXSL == null)
                    throw new ApplicationException("No XSL form found");
                if (frmXSL.TextEditor.Text == "")
                    throw new ApplicationException("XSL form is empty");
                if (frmXML.TextEditor.Text == "")
                    throw new ApplicationException("XML form is empty");

                TextReader xslReader = new StringReader(frmXSL.TextEditor.Text);

                // Compile stylesheet
                XsltExecutable executable = compiler.Compile(xslReader);

                // Do transformation to a destination
                RawDestination destination = new RawDestination();
                using (MemoryStream xmlStream = new MemoryStream(Encoding.UTF8.GetBytes(frmXML.TextEditor.Text ?? "")))
                {
                    Xslt30Transformer transformer = executable.Load30();
                    transformer.ApplyTemplates(xmlStream, destination);
                }

                string result;
                if (this.indentResultToolStripMenuItem.Checked)
                {
                    // Indent the XML
                    var stringBuilder = new StringBuilder();
                    var element = XElement.Parse(destination.XdmValue.ToString());

                    var settings = new XmlWriterSettings();
                    settings.OmitXmlDeclaration = false;
                    settings.Indent = true;
                    settings.NewLineOnAttributes = false;
                    using (var xmlWriter = XmlWriter.Create(stringBuilder, settings))
                    {
                        element.Save(xmlWriter);
                    }
                    result = stringBuilder.ToString();
                }
                else
                    result = destination.XdmValue.ToString();

                // Get / create the form
                XMLForm frmResult = null;
                if (this.overwriteOutputToolStripMenuItem.Checked)
                {
                    foreach (Form frm in this.MdiChildren)
                    {
                        if (frm.Tag != null)
                        {   
                            if (frm.Tag.ToString() == "Output")
                            {
                                frmResult = (XMLForm)frm;
                                break;
                            }
                        }
                    }

                }
                // If form has not been got then create it
                if (frmResult == null)
                {
                    frmResult = new XMLForm();
                    frmResult.Owner = this;
                }

                frmResult.MdiParent = this;
                frmResult.TextEditor.Text = result;
                frmResult.Tag = "Output";
                frmResult.Show();
            }
            catch (Exception ex)
            {
                string err = ex.Message + "\n";
                err += ex.GetType().ToString() + "\n";
                foreach (StaticError staticErr in compiler.ErrorList)
                {
                    err += staticErr.Message + " (line no : " + staticErr.LineNumber + ")\n";
                }

                MessageBox.Show(err);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog theDialog = new OpenFileDialog();
            theDialog.Title = "Open Text File";
            theDialog.Filter = "XML files|*.xml;*.xsl;*.xslt;*.xsd|All files|*.*";
            if (theDialog.ShowDialog() == DialogResult.OK)
            {
                string text = File.ReadAllText(theDialog.FileName.ToString());
                TranForm newChildForm;

                if (theDialog.FileName.EndsWith("xsl"))
                    newChildForm = new XSLForm();
                else
                    newChildForm = new XMLForm();

                // Set the Parent Form of the Child window.  
                newChildForm.MdiParent = this;
                // Display the new form.  
                newChildForm.Show();

                newChildForm.TextEditor.Text = text;
                newChildForm.Text = theDialog.FileName.ToString();
                newChildForm.Dirty = false;
            }
        }

        private void indentToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileVertical);
        }

        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.TileHorizontal);
        }

        private void tileCascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(System.Windows.Forms.MdiLayout.Cascade);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            // Load the XML configs file
            XmlDocument document = new XmlDocument();
            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;

            string fpath = string.Format(@"{0}insets.xml", RunningPath); // Insert the path to your xshd-files.
            document.Load(fpath);

            foreach (XmlNode categoryNode in document.SelectNodes("INSETS/CATEGORY"))
            {

                ToolStripMenuItem InsertMenuItem = new ToolStripMenuItem();
                InsertMenuItem.Text = categoryNode.Attributes["name"].Value;

                foreach (XmlNode node in categoryNode.SelectNodes("ITEM"))
                {
                    if (node.Attributes["name"] != null)
                    {
                        ToolStripItem tsi = InsertMenuItem.DropDownItems.Add(node.Attributes["name"].Value);
                        tsi.Tag = node.SelectSingleNode("text()").Value;

                        tsi.Click += new EventHandler(Tooltip_HandleClick);
                    }
                    else if (node.Attributes["separator"] != null)
                    {
                        InsertMenuItem.DropDownItems.Add(new ToolStripSeparator());
                    }
                }

                this.insertToolStripMenuItem.DropDownItems.Add(InsertMenuItem);
            }
        }

        private void Tooltip_HandleClick(object sender, EventArgs e)
        {
            TranForm tranForm = (TranForm)this.ActiveMdiChild;
            if (tranForm != null)
                tranForm.TextEditor.ActiveTextAreaControl.TextArea.InsertString(((ToolStripItem)sender).Tag.ToString());
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            TranForm tranForm = (TranForm)this.ActiveMdiChild;

            if (e.Control && e.KeyCode == Keys.T)
                Transform();
            else if (e.Control && e.KeyCode == Keys.I)
                tranForm.Indent();
            else if (e.Control && e.KeyCode == Keys.S)
                Save();
            else if (e.Control && e.KeyCode == Keys.F)
                Find();
            else if (e.KeyCode == Keys.F3)
                FindNext();
        }

        private void Find()
        {
            DialogResult result = System.Windows.Forms.DialogResult.None;
            result = InputBox.Show("Find",
                "Searchstring",
                "Value",
                out _findString);

            if (result == System.Windows.Forms.DialogResult.OK)
                FindNext();
        }

        private void FindNext()
        {
            TranForm tranForm = (TranForm)this.ActiveMdiChild;
            if (tranForm != null)
            {
                int lastPos = tranForm.TextEditor.ActiveTextAreaControl.TextArea.Caret.Offset + 2;
                lastPos = tranForm.TextEditor.Text.IndexOf("\n", lastPos);

                int pos = tranForm.TextEditor.Text.IndexOf(_findString, lastPos);
                if (pos > 0)
                {
                    var lineNumber = tranForm.TextEditor.Text.Take(pos).Count(c => c == '\n') + 1;
                    tranForm.TextEditor.ActiveTextAreaControl.JumpTo(lineNumber - 1);

                    TextLocation start = new TextLocation(0, lineNumber-1);
                    TextLocation end = new TextLocation(0, lineNumber);
                    tranForm.TextEditor.ActiveTextAreaControl.SelectionManager.SetSelection(start, end);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.ActiveMdiChild == null)
                throw new ApplicationException("No child form is active to save");

            TranForm currentForm = (TranForm)this.ActiveMdiChild;
            currentForm.SaveAs();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string helpText = "Tranny Version 0.0.0.1\n\n";
            helpText += "Freeware - GNU license\n\n\n";
            helpText += "Released by Harley Clark\n\n\n";
            helpText += "Acknowledgements with thanks\n";
            helpText += "Based on Xtrans application released by Sergei Sokolov\n";
            helpText += "Transformation Engine : Saxon (XSLT 3.1)\n";
            helpText += "Text Editor : ICSharpCode TextEditor\n\n";
            helpText += "With suggestions mail to  : marleydc@hotmail.com\n";

            MessageBox.Show(helpText, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Main_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void Main_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach (string fileName in files)
            {
                string text = File.ReadAllText(fileName.ToString());
                TranForm newChildForm;

                if (fileName.EndsWith("xsl"))
                    newChildForm = new XSLForm();
                else
                    newChildForm = new XMLForm();

                // Set the Parent Form of the Child window.  
                newChildForm.MdiParent = this;
                // Display the new form.  
                newChildForm.Show();

                newChildForm.TextEditor.Text = text;
                newChildForm.Text = fileName.ToString();
                newChildForm.Dirty = false;
            }
        }

        private void tsmFile_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        { 
            if (this.ActiveMdiChild == null)
                throw new ApplicationException("No child form is active to save");

            TranForm currentForm = (TranForm)this.ActiveMdiChild;
            currentForm.Save();
        }

        private void Main_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.ActiveMdiChild != null)
            {
                TranForm currentForm = (TranForm)this.ActiveMdiChild;
                currentForm.TextEditor_KeyUp(sender, e);
            }
        }
    }
}
