using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using System;
using System.ComponentModel;


namespace Scada.Web.Plugins.SchShapeComp.PropertyGrid
{
	public partial class FrmCustomShape : Form
	{
		public string ShapeType { get; set; }
		public bool Saved { get; private set; }
		private readonly string tempFilePath = Path.Combine(Path.GetTempPath(), $"tempSVG.svg");
		private bool IsExternalEditorOpen { get; set; } = false;
		private Process externalEditorProcess;
		private FileSystemWatcher fileWatcher;

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

		//private void BtnEditExternally_Click(object sender, EventArgs e)
		//{
		//	if (IsExternalEditorOpen)
		//	{
		//		BtnDoneEditing_Click(sender, e);
		//		return;
		//	}

		//	try
		//	{
		//		File.WriteAllText(tempFilePath, webBrowser1.DocumentText);
		//		string editorPath = Properties.Settings.Default.EditorPath;

		//		if (string.IsNullOrEmpty(editorPath))
		//		{
		//			DialogResult result = MessageBox.Show("Voulez-vous sélectionner un éditeur personnalisé ou utiliser l'éditeur par défaut?",
		//												  "Choix de l'éditeur",
		//												  MessageBoxButtons.YesNoCancel,
		//												  MessageBoxIcon.Question);

		//			if (result == DialogResult.Yes)
		//			{
		//				using (OpenFileDialog openFileDialog = new OpenFileDialog())
		//				{
		//					openFileDialog.Filter = "Executable files (*.exe)|*.exe|All files (*.*)|*.*";
		//					if (openFileDialog.ShowDialog() == DialogResult.OK)
		//					{
		//						editorPath = openFileDialog.FileName;
		//						Properties.Settings.Default.EditorPath = editorPath;
		//						Properties.Settings.Default.Save();
		//					}
		//					else
		//					{
		//						return;
		//					}
		//				}
		//			}
		//			else if (result == DialogResult.No)
		//			{
		//				externalEditorProcess = Process.Start(tempFilePath);
		//				WatchFileChanges();
		//				IsExternalEditorOpen = true;
		//				((Button)sender).Text = "Done";
		//				return;
		//			}
		//			else
		//			{
		//				return;
		//			}
		//		}

		//		externalEditorProcess = Process.Start(editorPath, tempFilePath);
		//		WatchFileChanges();
		//		IsExternalEditorOpen = true;
		//		((Button)sender).Text = "Done";
		//	}
		//	catch (Exception ex)
		//	{
		//		MessageBox.Show($"Failed to open the SVG in the external editor: {ex.Message}");
		//		Console.WriteLine(ex.Message);
		//	}
		//}
		private void BtnEditExternally_Click(object sender, EventArgs e)
		{
			if (IsExternalEditorOpen)
			{
				BtnDoneEditing_Click(sender, e);
				return;
			}

			try
			{
				File.WriteAllText(tempFilePath, webBrowser1.DocumentText);

				// Utiliser l'éditeur par défaut de Windows pour ouvrir le fichier
				externalEditorProcess = Process.Start(tempFilePath);
				WatchFileChanges();
				IsExternalEditorOpen = true;
				((Button)sender).Text = "Done";
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Failed to open the SVG in the external editor: {ex.Message}");
				Console.WriteLine(ex.Message);
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
				Invoke(new Action(() =>
				{
					try
					{
						string svgCode = File.ReadAllText(tempFilePath);
						webBrowser1.DocumentText = svgCode;
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Failed to reload the SVG: {ex.Message}");
					}
				}));
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);
			if (fileWatcher != null)
			{
				fileWatcher.Changed -= OnFileChanged;  // Détacher l'événement
				fileWatcher.Dispose();
				fileWatcher = null;
			}
		}

		private void BtnDoneEditing_Click(object sender, EventArgs e)
		{
			if (IsExternalEditorOpen && externalEditorProcess != null && !externalEditorProcess.HasExited)
			{
				DialogResult result = MessageBox.Show("En cliquant sur OK, votre éditeur sera fermé. Assurez-vous d'avoir enregistré toutes vos modifications. Voulez-vous continuer?",
													  "Fermeture de l'éditeur",
													  MessageBoxButtons.OKCancel,
													  MessageBoxIcon.Warning);

				if (result == DialogResult.OK)
				{
					try
					{
						externalEditorProcess.Kill();
						externalEditorProcess.WaitForExit(); // Attend que le processus se termine
						externalEditorProcess = null;
						IsExternalEditorOpen = false;

						// Maintenant, lisez le fichier
						string svgCode = File.ReadAllText(tempFilePath);
						webBrowser1.DocumentText = svgCode;
					}
					catch (Exception ex)
					{
						MessageBox.Show($"Une erreur s'est produite lors de la lecture du fichier temporaire : {ex.Message}");
					}
				}
			}
			else
			{
				try
				{
					// Si l'éditeur n'est pas en cours d'exécution, lisez simplement le fichier
					string svgCode = File.ReadAllText(tempFilePath);
					webBrowser1.DocumentText = svgCode;
					IsExternalEditorOpen = false;
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Une erreur s'est produproduite lors de la lecture du fichier temporaire: {ex.Message}");
				}
			}

				// Réinitialiser le texte du bouton après la fermeture de l'éditeur
				btnEditExternally.Text = "Edit";
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			if (IsExternalEditorOpen && externalEditorProcess != null && !externalEditorProcess.HasExited)
			{
				DialogResult result = MessageBox.Show("Un éditeur externe est toujours ouvert. Êtes-vous sûr de vouloir quitter sans terminer l'édition?",
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
				// En cas d'erreur lors de la suppression du fichier temporaire, vous pouvez l'enregistrer dans un journal ou simplement l'ignorer, car il est dans le dossier Temp.
				MessageBox.Show($"Error deleting temporary file: {ex.Message}");
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				CleanupTemporaryFiles();
			}

			base.Dispose(disposing);
		}
	}

}

