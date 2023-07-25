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

namespace Scada.Web.Plugins.SchSvgComp.PropertyGrid
{
	/// <summary>
	/// 
	/// </summary>
	public partial class FrmCustomShape : Form
	{
		public string ShapeType { get; set; }
		public bool Saved { get; private set; }

		public FrmCustomShape()
		{
			InitializeComponent();
		}

		public FrmCustomShape(string existingShapeType) : this()
		{
			ShapeType = existingShapeType;
			richTextBox1.Text = ShapeType;
			Saved = false;
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			string svg = richTextBox1.Text;
			if (ValidateAndHighlightErrors(svg))
			{
				MessageBox.Show("The SVG code contains errors. Please correct them before saving.");
			}
			else
			{
				ShapeType = svg;
				Saved = true;
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void btnImport_Click(object sender, EventArgs e)
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog())
			{
				openFileDialog.Filter = "SVG files (*.svg)|*.svg|All files (*.*)|*.*";
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					txtFilePath.Text = openFileDialog.FileName;
					try
					{
						string svgCode = File.ReadAllText(openFileDialog.FileName);
						richTextBox1.Text = svgCode;
					}
					catch
					{
						MessageBox.Show("An error occurred while reading the file.");
					}
				}
			}
		}

		private bool ValidateAndHighlightErrors(string svgCode)
		{
			bool hasErrors = false;
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ConformanceLevel = ConformanceLevel.Document,
				ValidationType = ValidationType.Schema
			};

			settings.ValidationEventHandler += (object sender, ValidationEventArgs args) =>
			{
				if (args.Severity == XmlSeverityType.Error)
				{
					int errorIndex = svgCode.IndexOf(args.Message, StringComparison.InvariantCulture);
					if (errorIndex >= 0)
					{
						HighlightError(richTextBox1, errorIndex, args.Message.Length);
						richTextBox1.Refresh();
						hasErrors = true;
					}
				}
			};

			using (StringReader stringReader = new StringReader(svgCode))
			{
				using (XmlReader reader = XmlReader.Create(stringReader, settings))
				{
					try
					{
						while (reader.Read()) { /* Just read */ }
					}
					catch (XmlException)
					{
						hasErrors = true;
					}
				}
			}

			return hasErrors;
		}

		private void HighlightError(RichTextBox richTextBox, int startIndex, int length)
		{
			int selectionStartOriginal = richTextBox.SelectionStart;
			int selectionLengthOriginal = richTextBox.SelectionLength;
			Color selectionColorOriginal = richTextBox.SelectionBackColor;

			richTextBox.SelectionStart = startIndex;
			richTextBox.SelectionLength = length;
			richTextBox.SelectionBackColor = Color.Red;

			richTextBox.SelectionStart = selectionStartOriginal;
			richTextBox.SelectionLength = selectionLengthOriginal;
			richTextBox.SelectionBackColor = selectionColorOriginal;
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			ValidateAndHighlightErrors(richTextBox1.Text);
		}

	}
}
