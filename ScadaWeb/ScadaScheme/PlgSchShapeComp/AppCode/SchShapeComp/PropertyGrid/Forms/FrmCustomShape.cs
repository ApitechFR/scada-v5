using System.ComponentModel;
using System.Windows.Forms;
using System.Diagnostics;
using System.Text;
using System.IO;
using System;
using Svg;

namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public partial class FrmCustomShape : Form
	{
		//internationnalization vars
		private const string ErrorReadingFile = "An error occurred while reading the file: {0}";
		private const string ErrorDeletingTempFile = "Error deleting temporary file: {0}";
		private const string ExternalEditorOpenWarning = "An external editor is still open. Are you sure you want to exit without finishing editing?";
		private const string CloseEditorWarning = "By clicking OK, your editor will be closed. Make sure you have saved all your changes. Do you want to continue?";
		private const string FailedToOpenSVG = "Failed to open the SVG in the external editor: {0}";
		private const string ErrorClosingExternalEditor = "An error occurred while closing the external editor: {0}";
		
		public delegate void ShapeSavedHandler(string svgData);
		public event ShapeSavedHandler ShapeSaved;

		public string ShapeType { get; set; }
		public bool Saved { get; private set; }
		private readonly string tempFilePath = Path.Combine(Path.GetTempPath(), $"tempSVG.svg");
		private bool IsExternalEditorOpen { get; set; } = false;
		private Process externalEditorProcess;
		private FileSystemWatcher fileWatcher;
		private string svgText;


		public FrmCustomShape()
		{
			InitializeComponent();
			UpdateButtonStates();
		}

		
		public FrmCustomShape(string existingShapeType) : this()
		{
			ShapeType = existingShapeType;
			if (!string.IsNullOrWhiteSpace(ShapeType))
			{
				svgText = ShapeType;
				byte[] svgBytes = Encoding.UTF8.GetBytes(svgText);
				ctrlSvgViewer1.ShowImage(svgBytes);
			}
			Saved = false;
			UpdateButtonStates();
		}


		private void ShowError(string format, Exception ex)
		{
			MessageBox.Show(string.Format(format, ex.Message));
		}
		protected virtual void OnShapeSaved(string svgData)
		{
			ShapeSaved?.Invoke(svgData);
		}
		private void UpdateButtonStates()
		{
			bool hasContent = !string.IsNullOrEmpty(svgText);
			btnEditExternally.Enabled = hasContent;
		}

		private void BtnSave_Click(object sender, EventArgs e)
		{
			CloseExternalEditor();

			ShapeType = svgText; 
			Saved = true;
			OnShapeSaved(ShapeType);

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
					ReadSvgFromFile(openFileDialog.FileName);
					UpdateButtonStates();
				}
			}
		}
		private void ReadSvgFromFile(string filePath)
		{
			try
			{
				svgText = File.ReadAllText(filePath);
				byte[] svgBytes = Encoding.UTF8.GetBytes(svgText);
				ctrlSvgViewer1.ShowImage(svgBytes);	
			}
			catch (Exception ex)
			{
				Console.WriteLine(ErrorReadingFile + ": " + ex.Message);
			}
		}

		private void BtnEditExternally_Click(object sender, EventArgs e)
		{
			if (IsExternalEditorOpen)
			{
				BtnDoneEditing_Click(sender, e);
				return;
			}

			try
			{
				File.WriteAllText(tempFilePath, svgText);

				externalEditorProcess = Process.Start(tempFilePath);
				if (externalEditorProcess != null)
				{
					externalEditorProcess.EnableRaisingEvents = true;
					externalEditorProcess.Exited += ExternalEditorProcess_Exited;
				}

				WatchFileChanges();
				IsExternalEditorOpen = true;
				((Button)sender).Text = "Done";
			}
			catch (Exception ex)
			{
				MessageBox.Show(string.Format(FailedToOpenSVG, ex.Message));
				Console.WriteLine(ex.Message);
			}
		}

		private void ExternalEditorProcess_Exited(object sender, EventArgs e)
		{
			if (InvokeRequired)
			{
				Invoke(new Action(() =>
				{
					CloseExternalEditor();
					ReadSvgFromFile(tempFilePath);
					btnEditExternally.Text = "Edit";
					IsExternalEditorOpen = false;

				}));
			}
			else
			{
				CloseExternalEditor();
				ReadSvgFromFile(tempFilePath);
				btnEditExternally.Text = "Edit";
				IsExternalEditorOpen = false;
			}
		}

		private void WatchFileChanges()
		{
			if (fileWatcher == null)
			{
				fileWatcher = new FileSystemWatcher
				{
					Path = Path.GetDirectoryName(tempFilePath),
					Filter = Path.GetFileName(tempFilePath),
					NotifyFilter = NotifyFilters.LastWrite
				};

				fileWatcher.Changed += OnFileChanged;
				fileWatcher.EnableRaisingEvents = true;
			}
		}

		private void OnFileChanged(object sender, FileSystemEventArgs e)
		{
			if (e.ChangeType == WatcherChangeTypes.Changed)
			{
				Invoke(new Action(() => {
					ReadSvgFromFile(tempFilePath);
					UpdateButtonStates();
				}));
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			CleanupTemporaryFiles();

			if (fileWatcher != null)
			{
				fileWatcher.Changed -= OnFileChanged;  
				fileWatcher.Dispose();
				fileWatcher = null;
			}
		}
		
		private void BtnDoneEditing_Click(object sender, EventArgs e)
		{
			CloseExternalEditor();

			if (!IsExternalEditorOpen)
			{
				ReadSvgFromFile(tempFilePath);
				btnEditExternally.Text = "Edit";
			}
		}

		private void CloseExternalEditor()
		{
			if (IsExternalEditorOpen && externalEditorProcess != null && !externalEditorProcess.HasExited)
			{
				DialogResult result = MessageBox.Show(CloseEditorWarning,
													  "Closing the Editor",
													  MessageBoxButtons.OKCancel,
													  MessageBoxIcon.Warning);

				if (result == DialogResult.OK)
				{
					try
					{
						externalEditorProcess.Kill();
						externalEditorProcess.WaitForExit();
						externalEditorProcess.Exited -= ExternalEditorProcess_Exited;
						IsExternalEditorOpen = false;
						btnEditExternally.Text = "Edit";
					}
					catch (Exception ex)
					{
						ShowError(ErrorClosingExternalEditor, ex);
						
					}
				}
			}
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (IsExternalEditorOpen && externalEditorProcess != null && !externalEditorProcess.HasExited)
			{
				DialogResult result = MessageBox.Show(ExternalEditorOpenWarning,
													  "Éditeur externe ouvert",
													  MessageBoxButtons.OKCancel,
													  MessageBoxIcon.Warning);

				if (result == DialogResult.Cancel)
				{
					e.Cancel = true; 
					return;
				}
			}
			base.OnClosing(e);
		}

		private void CleanupTemporaryFiles()
		{
			try
			{
				if (File.Exists(tempFilePath))
				{
					File.Delete(tempFilePath);
				}
			}
			catch (Exception ex)
			{
				ShowError(ErrorDeletingTempFile, ex);
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				CleanupTemporaryFiles();
				externalEditorProcess?.Dispose();
				fileWatcher?.Dispose();
			}
			base.Dispose(disposing);
		}

		private void BtnCancel_Click(object sender, EventArgs e)
		{
			if (IsExternalEditorOpen)
			{
				try
				{
					if (externalEditorProcess != null && !externalEditorProcess.HasExited)
					{
						externalEditorProcess.Kill();
						externalEditorProcess.WaitForExit();
						externalEditorProcess.Exited -= ExternalEditorProcess_Exited;
						IsExternalEditorOpen = false;
					}
				}
				catch (Exception ex)
				{
					ShowError(ErrorClosingExternalEditor, ex);
				}
			}

			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}

	}

}

