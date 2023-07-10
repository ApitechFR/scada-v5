using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Schema;

namespace Scada.Scheme.Model.PropertyGrid
{
	/// <summary>
	/// 
	/// </summary>
	public partial class FrmCustomShape : Form
	{
		/// <summary>
		/// 
		/// </summary>
		public FrmCustomShape()
		{
			InitializeComponent();
			//this.btnSave.Enabled = false;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			//if (ValidateSvgCode(textBox1.Text));
			string svg = richTextBox1.Text;
			HighlightXmlErrors(richTextBox1 as RichTextBox, svg);
			

		}
		private void btnImport_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*";
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				txtFilePath.Text = openFileDialog.FileName;
				try
				{
					string svgCode = File.ReadAllText(openFileDialog.FileName);
					if (ValidateSvgCode(svgCode))
					{
						//textBox1.Text = svgCode;
						richTextBox1.Text = svgCode;
					}
				}
				catch
				{
					MessageBox.Show("An error occurred while reading the file.");
				}
			}

		}
		private void HighlightError(RichTextBox richTextBox, int startIndex, int length)
		{
			// Save the current state of the selection
			var selectionStartOriginal = richTextBox.SelectionStart;
			var selectionLengthOriginal = richTextBox.SelectionLength;
			var selectionColorOriginal = richTextBox.SelectionBackColor;

			// Select the part of the text to highlight
			richTextBox.SelectionStart = startIndex;
			richTextBox.SelectionLength = length;

			// Change the background color of the selection
			richTextBox.SelectionBackColor = Color.Red;

			
			// Restore the original state of the selection
			richTextBox.SelectionStart = selectionStartOriginal;
			richTextBox.SelectionLength = selectionLengthOriginal;
			richTextBox.SelectionBackColor = selectionColorOriginal;
		}



		private void HighlightXmlErrors(RichTextBox richTextBox, string xml)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			settings.ConformanceLevel = ConformanceLevel.Document;
			settings.ValidationType = ValidationType.Schema;

			settings.ValidationEventHandler += (object sender, ValidationEventArgs args) =>
			{
				if (args.Severity == XmlSeverityType.Error)
				{
					int errorIndex = xml.IndexOf(args.Message, StringComparison.InvariantCulture);
					if (errorIndex >= 0)
					{
						HighlightError(richTextBox, errorIndex, args.Message.Length);
						richTextBox.Refresh();
					}
				}
			};

			using (StringReader stringReader = new StringReader(xml))
			{
				using (XmlReader reader = XmlReader.Create(stringReader, settings))
				{
					while (reader.Read()) { /* Just read */ }
				}
			}
		}

		//private void richTextBox1_TextChanged(object sender, EventArgs e)
		//{
		//	string svg = richTextBox1.Text;
		//	HighlightXmlErrors(richTextBox1 as RichTextBox, svg);

		//}

		//private void textBox1_TextChanged(object sender, EventArgs e)
		//{
		//	ValidateSvgCode(textBox1.Text);
		//}

		private bool ValidateSvgCode(string svgCode)
		{
			XmlDocument xmlDocument = new XmlDocument();
			try
			{
				xmlDocument.LoadXml(svgCode);
				if (xmlDocument.DocumentElement.Name == "svg")
				{
					//btnSave.Enabled = true;
					return true;
				}
			}
			catch (XmlException)
			{
				// No action needed
			}

			//btnSave.Enabled = false;
			return false;
		}

		
	}
}
