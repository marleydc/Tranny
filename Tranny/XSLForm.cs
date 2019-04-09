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
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace Tranny
{
    public partial class XSLForm : TranForm
    {
        public XSLForm()
        {
            InitializeComponent();

            string RunningPath = AppDomain.CurrentDomain.BaseDirectory;

            string dir = string.Format(@"{0}Highlighting", RunningPath); // Insert the path to your xshd-files.
            FileSyntaxModeProvider fsmProvider; // Provider
            if (Directory.Exists(dir))
            {
                fsmProvider = new FileSyntaxModeProvider(dir); // Create new provider with the highlighting directory.
                HighlightingManager.Manager.AddSyntaxModeFileProvider(fsmProvider); // Attach to the text editor.
                TextEditor.SetHighlighting("XSL-Tranny"); // Activate the highlighting, use the name from the SyntaxDefinition node.
            }
        }

        private void XSLForm_Load(object sender, EventArgs e)
        {

        }
    }
}
