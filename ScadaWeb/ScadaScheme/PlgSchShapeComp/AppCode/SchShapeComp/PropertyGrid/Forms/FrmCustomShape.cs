using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public partial class FrmCustomShape : Form
	{
		public string ShapeType { get; set; }
		public bool Saved { get; private set; }
		private readonly string tempFilePath = Path.Combine(Path.GetTempPath(), "tempSVG.svg");

		public FrmCustomShape()
		{
			InitializeComponent();
		}

		public FrmCustomShape(string existingShapeType) : this()
		{
			ShapeType = existingShapeType;
			webBrowser1.DocumentText = ShapeType;
			Saved = false;
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			ShapeType = webBrowser1.DocumentText;
			Saved = true;
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void BtnImport_Click(object sender, EventArgs e)
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
						webBrowser1.DocumentText = svgCode;
					}
					catch (Exception ex)
					{
						MessageBox.Show($"An error occurred while reading the file: {ex.Message}");
					}
				}
			}
		}

		private void BtnEditExternally_Click(object sender, EventArgs e)
		{
			try
			{
				File.WriteAllText(tempFilePath, webBrowser1.DocumentText);

				string editorPath = Properties.Settings.Default.EditorPath;

				if (string.IsNullOrEmpty(editorPath))
				{
					DialogResult result = MessageBox.Show("Voulez-vous sélectionner un éditeur personnalisé ou utiliser l'éditeur par défaut?",
														  "Choix de l'éditeur",
														  MessageBoxButtons.YesNoCancel,
														  MessageBoxIcon.Question);

					if (result == DialogResult.Yes)
					{
						using (OpenFileDialog openFileDialog = new OpenFileDialog())
						{
							openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
							if (openFileDialog.ShowDialog() == DialogResult.OK)
							{
								editorPath = openFileDialog.FileName;

								Properties.Settings.Default.EditorPath = editorPath;
								Properties.Settings.Default.Save();
							}
							else
							{
								return;
							}
						}
					}
					else if (result == DialogResult.No)
					{
						Process.Start(tempFilePath);
						return;
					}
					else
					{
						
						return;
					}
				}

				Process.Start(editorPath, tempFilePath);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to open the SVG in the external editor: {ex.Message}");
			}
		}

		private void BtnDoneEditing_Click(object sender, EventArgs e)
		{
			try
			{
				string svgCode = File.ReadAllText(tempFilePath);
				webBrowser1.DocumentText = svgCode;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error occurred while reading the temporary file: {ex.Message}");
			}
		}

	}
}
