/*
 * Copyright 2019 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : Scheme Editor
 * Summary  : Main form of the application
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2017
 * Modified : 2019
 */

using Scada.Scheme.Editor.AppCode;
using Scada.Scheme.Editor.Properties;
using Scada.Scheme.Model;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Model.PropertyGrid;
using Scada.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml;
using Utils;
using Button = System.Windows.Forms.Button;
using ListViewItem = System.Windows.Forms.ListViewItem;
using CM = System.ComponentModel;
using File = System.IO.File;

namespace Scada.Scheme.Editor
{
    /// <summary>
    /// Main form of the application.
    /// <para>Главная форма приложения.</para>
    /// </summary>
    public partial class FrmMain : Form, IMainForm
    {
        /// <summary>
        /// Ключ иконки компонента по умолчанию.
        /// </summary>
        private const string DefCompIcon = "component.png";

        private readonly AppData appData;   // общие данные приложения
        private readonly Settings settings; // настройки приложения
        private readonly Log log;           // журнал приложения
        private readonly Editor editor;     // редактор

        private Mutex mutex;                // объект для проверки запуска второй копии приложения
        private bool compTypesChanging;     // пользователь изменяет выбранный элемент lvCompTypes
        private bool schCompChanging;       // пользователь изменяет выбранный элемент cbSchComp
        private FormStateDTO formStateDTO;  // состояние формы для передачи
        private bool noTreeviewSelectionEffect;
        private Dictionary<string, string> availableSymbols;
        private bool existingSymbolWasConverted;
        private bool existingSchemeWasConverted;


        /// <summary>
        /// Конструктор.
        /// </summary>
        public FrmMain()
        {
            InitializeComponent();

            appData = AppData.GetAppData();
            settings = appData.Settings;
            log = appData.Log;
            editor = appData.Editor;
            mutex = null;
            compTypesChanging = false;
            schCompChanging = false;
            formStateDTO = null;
            noTreeviewSelectionEffect = false;
            existingSymbolWasConverted = false;

            editor.ModifiedChanged += Editor_ModifiedChanged;
            editor.PointerModeChanged += Editor_PointerModeChanged;
            editor.StatusChanged += Editor_StatusChanged;
            editor.SelectionChanged += Editor_SelectionChanged;
            editor.SelectionPropsChanged += Editor_SelectionPropsChanged;
            editor.ClipboardChanged += Editor_ClipboardChanged;
            editor.History.HistoryChanged += History_HistoryChanged;
            SchemeContext.GetInstance().SchemePath = editor.FileName;
        }


        /// <summary>
        /// Loads symbols from an XML file, taking into account their validity.
        /// Invalid symbols (whose associated files do not exist) are removed from the XML file.
        /// </summary>
        /// <param name="xmlPath">Path to the XML file containing the symbols.</param>
        /// <returns>Returns a dictionary containing the names and paths of valid symbols.</returns>
        private Dictionary<string, string> LoadSymbolsFromXml(string xmlPath)
        {
            Dictionary<string, string> symbolsDictionary = new Dictionary<string, string>();

            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                if (!File.Exists(xmlPath))
                {
                    return symbolsDictionary;
                }
                string xmlContent = File.ReadAllText(xmlPath);
                if (xmlContent == "")
                {
                    return symbolsDictionary;
                }

                xmlDoc.Load(xmlPath);
                XmlNodeList symbolNodes = xmlDoc.SelectNodes("//symbol");

                List<XmlNode> nodesToRemove = new List<XmlNode>();

                foreach (XmlNode symbolNode in symbolNodes)
                {
                    XmlElement symbolElement = (XmlElement)symbolNode;
                    string name = symbolElement.GetAttribute("name");
                    string path = symbolElement.GetAttribute("path");

                    if (File.Exists(path))
                    {
                        symbolsDictionary[path] = name;
                    }
                    else
                    {
                        nodesToRemove.Add(symbolNode);
                    }
                }

                foreach (XmlNode nodeToRemove in nodesToRemove)
                {
                    nodeToRemove.ParentNode.RemoveChild(nodeToRemove);
                }

                xmlDoc.Save(xmlPath);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An error occurred while processing the XML file.", ex);
            }

            return symbolsDictionary;
        }

        //<summary>
        //Deletes existing symbols from the ListView
        //</summary>
        private void RemoveExistingSymbolsFromListView()
        {
            var itemsToRemove = lvCompTypes.Items.Cast<ListViewItem>().Where(item => item.Group.Header == "Symbols").ToList();
            foreach (ListViewItem itemToRemove in itemsToRemove)
            {
                lvCompTypes.Items.Remove(itemToRemove);
            }

            var groupToRemove = lvCompTypes.Groups.Cast<ListViewGroup>().Where(group => group.Header == "Symbols").FirstOrDefault();
            if (groupToRemove != null)
            {
                lvCompTypes.Groups.Remove(groupToRemove);
            }
        }
        //<summary>
        //Add new symbols to the ListView
        //</summary>
        private void UpdateSymbolsListView(string indexPath)
        {
            availableSymbols = LoadSymbolsFromXml(indexPath);
            if (editor.SchemeView.isSymbol)
            {
                return;
            }
            ListViewGroup symbolsViewGroup = new ListViewGroup("Symbols");
            foreach (var s in availableSymbols)
            {
                lvCompTypes.Items.Add(new ListViewItem($"{s.Value} ({Path.GetFileName(s.Key)})", "component.png", symbolsViewGroup) { IndentCount = 1 });
            }
            lvCompTypes.Groups.Add(symbolsViewGroup);
        }

        private string getSymbolIndexFilePath()
        {
            //we create folder for index if not exists
            string symbolsFolderPath = Editor.getSymbolsDir(editor.FileName);

            if(symbolsFolderPath == null)
            {
                return null;
            }
            //create index file if not exists
            string indexPath = Path.Combine(symbolsFolderPath, "index.xml");
            if (!File.Exists(indexPath))
            {
                File.Create(indexPath).Close();
            }
            return indexPath;
        }

        //<summary>
        // Refresh available symbols
        // Checks if the XML file exists and that we are not in a symbol view
        // Loads symbols from the XML file
        // Updates the member variable
        // Removes existing symbols from the ListView
        // Adds new symbols to the ListView
        //</summary>
        private void RefreshAvailableSymbols()
        {
            RemoveExistingSymbolsFromListView();
            //If we are editing a symbol, we don't want to refresh the list of available symbols
            if (editor.SchemeView == null || editor.SchemeView.isSymbol)
            {
                return;
            }

            string indexPath = getSymbolIndexFilePath();
            if(indexPath == null)
            {
                return;
            }   
            try
            {
                UpdateSymbolsListView(indexPath);
            }
            catch (Exception ex)
            {
                log.WriteException(ex, "Error : " + ex.Message);
            }
        }


        private void lvCompTypes_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem clickedItem = lvCompTypes.GetItemAt(e.X, e.Y);
                string symbolPath = availableSymbols.ElementAt(clickedItem.Index-(lvCompTypes.Items.Count-availableSymbols.Count)).Key;
                if (clickedItem != null)
                {
                    ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                    ToolStripMenuItem optionMenuItem = new ToolStripMenuItem("Delete symbol");
                    contextMenuStrip.Items.Add(optionMenuItem);
                    optionMenuItem.Click += (s, args) =>
                    {
                        if (MessageBox.Show(string.Format("Delete {0} ?", clickedItem.Text), "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            DeleteSymbol(symbolPath);
                        }

                    };
                    contextMenuStrip.Show(lvCompTypes, e.Location);
                }
            }
        }
        private void DeleteSymbol(string symbolPath)
        {
            if (System.IO.File.Exists(symbolPath))
            {
                System.IO.File.Delete(symbolPath);
            }

            string indexFile = getSymbolIndexFilePath();
            XmlDocument indexXmlDocument = new XmlDocument();
            indexXmlDocument.Load(indexFile);

            XmlNodeList elements = indexXmlDocument.SelectNodes("//symbol[@path='" + symbolPath + "']");
            if (elements.Count > 0)
            {
                XmlNode elementToDelete = elements[0];
                elementToDelete.ParentNode.RemoveChild(elementToDelete);
                indexXmlDocument.Save(indexFile);
            }
            RefreshAvailableSymbols();
        }
        private void ListView1_DrawSubItem(object sender, DrawListViewSubItemEventArgs e)
        {
            e.DrawDefault = true;

            // Dessiner un bouton dans la deuxième colonne de l'élément 2
            if (e.ItemIndex == 1 && e.ColumnIndex == 1)
            {
                Button button = new Button();
                button.Text = "Cliquez";
                button.Size = e.SubItem.Bounds.Size;
                button.Location = new System.Drawing.Point(e.SubItem.Bounds.Left, e.SubItem.Bounds.Top);

                button.Click += (s, evt) =>
                {
                    MessageBox.Show("Bouton cliqué pour l'élément 2");
                };

                e.Item.ListView.Controls.Add(button);
            }
        }

        /// <summary>
        /// Локализовать форму.
        /// </summary>
        private void LocalizeForm()
        {
            if (!Localization.LoadDictionaries(appData.AppDirs.LangDir, "ScadaData", out string errMsg))
                log.WriteError(errMsg);

            if (!Localization.LoadDictionaries(appData.AppDirs.LangDir, "ScadaScheme", out errMsg))
                log.WriteError(errMsg);

            bool appDictLoaded = Localization.LoadDictionaries(appData.AppDirs.LangDir, "ScadaSchemeEditor", out errMsg);
            if (!appDictLoaded)
                log.WriteError(errMsg);

            CommonPhrases.Init();
            SchemePhrases.Init();
            AppPhrases.Init();

            if (appDictLoaded)
            {
                Translator.TranslateForm(this, "Scada.Scheme.Editor.FrmMain");
                ofdScheme.SetFilter(AppPhrases.SchemeFileFilter);
                sfdScheme.SetFilter(AppPhrases.SchemeFileFilter);
            }
        }

        /// <summary>
        /// Локализовать атрибуты для отображения свойств компонентов.
        /// </summary>
        private void LocalizeAttributes()
        {
            try
            {
                AttrTranslator attrTranslator = new AttrTranslator();
                attrTranslator.TranslateAttrs(typeof(SchemeDocument));
                attrTranslator.TranslateAttrs(typeof(BaseComponent));
                attrTranslator.TranslateAttrs(typeof(StaticText));
                attrTranslator.TranslateAttrs(typeof(DynamicText));
                attrTranslator.TranslateAttrs(typeof(StaticPicture));
                attrTranslator.TranslateAttrs(typeof(DynamicPicture));
                attrTranslator.TranslateAttrs(typeof(UnknownComponent));
                attrTranslator.TranslateAttrs(typeof(Condition));
                attrTranslator.TranslateAttrs(typeof(ImageCondition));
                attrTranslator.TranslateAttrs(typeof(Size));
                attrTranslator.TranslateAttrs(typeof(ImageListItem));
            }
            catch (Exception ex)
            {
                log.WriteException(ex, Localization.UseRussian ?
                    "Ошибка при локализации атрибутов" :
                    "Error localizing attributes");
            }
        }

        /// <summary>
        /// Проверить, что запущена вторая копия приложения.
        /// </summary>
        private bool SecondInstanceExists()
        {
            try
            {
                mutex = new Mutex(true, "ScadaSchemeEditorMutex", out bool createdNew);
                return !createdNew;
            }
            catch (Exception ex)
            {
                log.WriteException(ex, Localization.UseRussian ?
                    "Ошибка при проверке существования второй копии приложения" :
                    "Error checking existence of a second copy of the application");
                return false;
            }
        }

        /// <summary>
        /// Sets the standard component images.
        /// </summary>
        private void SetComponentImages()
        {
            // loading images from resources instead of storing in image list prevents them from corruption
            ilCompTypes.Images.Add("pointer.png", Resources.pointer);
            ilCompTypes.Images.Add("comp_st.png", Resources.comp_st);
            ilCompTypes.Images.Add("comp_dt.png", Resources.comp_dt);
            ilCompTypes.Images.Add("comp_sp.png", Resources.comp_sp);
            ilCompTypes.Images.Add("comp_dp.png", Resources.comp_dp);
            ilCompTypes.Images.Add("component.png", Resources.component);

            lvCompTypes.Items[0].ImageKey = "pointer.png";
            lvCompTypes.Items[1].ImageKey = "comp_st.png";
            lvCompTypes.Items[2].ImageKey = "comp_dt.png";
            lvCompTypes.Items[3].ImageKey = "comp_sp.png";
            lvCompTypes.Items[4].ImageKey = "comp_dp.png";
        }

        /// <summary>
        /// Заполнить список типов компонентов.
        /// </summary>
        private void FillComponentTypes()
        {
            try
            {
                lvCompTypes.BeginUpdate();
                CompLibSpec[] specs = appData.CompManager.GetSortedSpecs();

                foreach (CompLibSpec spec in specs)
                {
                    ListViewGroup listViewGroup = new ListViewGroup(spec.GroupHeader);

                    // добавление элемента с указателем
                    lvCompTypes.Items.Add(new ListViewItem(
                        AppPhrases.PointerItem, "pointer.png", listViewGroup)
                    { IndentCount = 1 });

                    // добавление компонентов
                    foreach (CompItem compItem in spec.CompItems)
                    {
                        string imageKey;
                        if (compItem.Icon == null)
                        {
                            imageKey = DefCompIcon;
                        }
                        else
                        {
                            imageKey = "image" + ilCompTypes.Images.Count;
                            ilCompTypes.Images.Add(imageKey, compItem.Icon);
                        }

                        lvCompTypes.Items.Add(new ListViewItem()
                        {
                            Text = compItem.DisplayName,
                            ImageKey = imageKey,
                            Tag = compItem.CompType.FullName,
                            Group = listViewGroup,
                            IndentCount = 1
                        });
                    }

                    lvCompTypes.Groups.Add(listViewGroup);
                }
            }
            finally
            {
                lvCompTypes.EndUpdate();
            }
        }

        /// <summary>
        /// Открыть браузер со страницей редактора.
        /// </summary>
        private void OpenBrowser()
        {
            string path = editor.GetWebPageFilePath(appData.AppDirs.WebDir);

            if (File.Exists(path))
            {
                try
                {
                    Uri startUri = new Uri(path);

                    switch (settings.Browser)
                    {
                        case Settings.Browsers.Chrome:
                            Process.Start("chrome", startUri.AbsoluteUri);
                            break;
                        case Settings.Browsers.Firefox:
                            Process.Start("firefox", startUri.AbsoluteUri);
                            break;
                        default: // Settings.Browsers.Default:
                            Process.Start(startUri.AbsoluteUri);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    log.WriteException(ex, AppPhrases.OpenBrowserError);
                    ScadaUiUtils.ShowError(AppPhrases.OpenBrowserError + ":" + Environment.NewLine + ex.Message);
                }
            }
            else
            {
                string errMsg = string.Format(CommonPhrases.NamedFileNotFound, path);
                log.WriteError(errMsg);
                ScadaUiUtils.ShowError(errMsg);
            }
        }

        /// <summary>
        /// Инициализировать схему, создав новую или загрузив из файла.
        /// </summary>
        private void InitScheme(string fileName = "")
        {
            bool loadOK;
            string errMsg;

            if (string.IsNullOrEmpty(fileName))
            {
                loadOK = true;
                errMsg = "";
                editor.NewScheme();
            }
            else
            {
                loadOK = editor.LoadSchemeFromFile(fileName, out errMsg);
            }

            appData.AssignViewStamp(editor.SchemeView);
            FillSchemeComponents();
            ShowSchemeSelection();
            SubscribeToSchemeChanges();
            editor.History.MakeCopy(editor.SchemeView);
            Text = editor.Title;

            if (!loadOK)
                ScadaUiUtils.ShowError(errMsg);

            RefreshAvailableSymbols();
            if(editor.SchemeView != null)
            {
                toolStripButton2.Enabled = editor.SchemeView.isSymbol;
                toolStripStatusLabel1.Text = editor.SchemeView != null ? (editor.SchemeView.isSymbol ? "Editing symbol" : "Editing scheme") : "";
                toolStripButton3.ToolTipText = editor.SchemeView != null ? (editor.SchemeView.isSymbol ? "Convert into scheme" : "Convert into symbol") : "";
            }
        }

        /// <summary>
        /// Сохранить схему.
        /// </summary>
        /// 
        private bool SaveScheme(bool saveAs)
        {
            string projectPath = Editor.getProjectRootPath(editor.FileName);

            //define file name and location according to the context
            if (saveAs || string.IsNullOrEmpty(editor.FileName) || existingSchemeWasConverted || existingSymbolWasConverted)
            {
                saveAs = true;
                if (string.IsNullOrEmpty(editor.FileName))
                {
                    sfdScheme.FileName = editor.SchemeView.isSymbol ? (editor.SchemeView.MainSymbol.Name == "MainSymbol" ? Editor.DefSymbolFileName : editor.SchemeView.MainSymbol.Name + ".sch") : Editor.DefSchemeFileName;
                }
                else
                {
                    string[] strings = editor.FileName.Split('\\');
                    sfdScheme.FileName = strings[strings.Length - 1];
                    sfdScheme.InitialDirectory = Path.GetDirectoryName(editor.FileName);
                }
                if (projectPath != null)
                {
                    sfdScheme.InitialDirectory = editor.SchemeView.isSymbol ? Path.Combine(Editor.getProjectRootPath(editor.FileName), "Views", "Symbols") : Path.Combine(Editor.getProjectRootPath(editor.FileName), "Views");
                }
            }
            else
            {
                sfdScheme.FileName = editor.FileName;
            }

            if (saveAs && sfdScheme.ShowDialog() != DialogResult.OK)
            {
                existingSymbolWasConverted = false;
                existingSchemeWasConverted = false;
                return false;
            }

            if (existingSymbolWasConverted && projectPath != null && MessageBox.Show("Do you want to delete the existing symbol ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                DeleteSymbol(editor.FileName);
            }
            if (existingSchemeWasConverted && projectPath != null && MessageBox.Show("Do you want to delete the existing scheme ?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (System.IO.File.Exists(editor.FileName))
                {
                    System.IO.File.Delete(editor.FileName);
                }
            }
            existingSymbolWasConverted = false;
            existingSchemeWasConverted = false;
            
            bool result = false;
            bool refrPropGrid = propertyGrid.SelectedObject is SchemeDocument document && document.Version != SchemeUtils.SchemeVersion;

            if (!string.IsNullOrEmpty(sfdScheme.FileName))
            {
                // сохранение схемы
                if (editor.SaveSchemeToFile(sfdScheme.FileName, out string errMsg))//, editor.SchemeView.isSymbol))
                {
                    if (editor.SchemeView.isSymbol)
                    {
                        string indexPath = getSymbolIndexFilePath();
                        if (indexPath != null)
                        {
                            updateSymbolIndex(indexPath, sfdScheme.FileName);
                            result = true;
                        }
                    }
                    else
                    {
                        result = true;
                    }
                    if (saveAs)
                    {
                        InitScheme(sfdScheme.FileName);
                    }
                }
                else
                {
                    log.WriteError(errMsg);
                    ScadaUiUtils.ShowError(errMsg);
                }

                // обновить свойства документа схемы, если файл сохраняется другой версией редактора
                if (refrPropGrid)
                    propertyGrid.Refresh();
            }

            return result;
        }

        private void updateSymbolIndex(string xmlPath, string symbolFileName)
        {
            try
            {
                Symbol currentSymbol = editor.SchemeView.MainSymbol;
                XmlDocument xmlDoc = new XmlDocument();

                if(File.Exists(xmlPath) && File.ReadAllText(xmlPath) != "")
                {
                    xmlDoc.Load(xmlPath);
                }
                else
                {
                    XmlDeclaration xmlDeclaration = xmlDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                    XmlElement root = xmlDoc.CreateElement("symbols");
                    xmlDoc.InsertBefore(xmlDeclaration, xmlDoc.DocumentElement);
                    xmlDoc.AppendChild(root);
                }

                XmlNode entryToUpdate = xmlDoc.SelectSingleNode($"//symbol[@path='{symbolFileName}']");

                if (entryToUpdate != null)
                {
                    entryToUpdate.Attributes["lastModificationDate"].Value = DateTime.Now.ToString();
                    entryToUpdate.Attributes["name"].Value = currentSymbol.Name != "" ? currentSymbol.Name : Path.GetFileNameWithoutExtension(symbolFileName);
                }
                else
                {
                    if (xmlDoc.SelectSingleNode($"//symbol[@symbolId='{currentSymbol.SymbolId}']") != null)
                    {
                        currentSymbol.ResetSymbolId();
                    }
                    XmlElement newEntry = xmlDoc.CreateElement("symbol");
                    newEntry.SetAttribute("name", currentSymbol.Name != "" ? currentSymbol.Name : Path.GetFileNameWithoutExtension(symbolFileName));
                    newEntry.SetAttribute("path", symbolFileName);
                    newEntry.SetAttribute("symbolId", currentSymbol.SymbolId);
                    newEntry.SetAttribute("lastModificationDate", DateTime.Now.ToString());
                    xmlDoc.DocumentElement.AppendChild(newEntry);
                }
                xmlDoc.Save(xmlPath);
            }
            catch (Exception ex)
            {
                log.WriteException(ex,"Error: " + ex.Message);
            }
        }


        /// <summary>
        /// Подтвердить возможность закрыть схему.
        /// </summary>
        private bool ConfirmCloseScheme()
        {
            if (editor.Modified)
            {
                switch (MessageBox.Show(AppPhrases.SaveSchemeConfirm, CommonPhrases.QuestionCaption,
                    MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
                {
                    case DialogResult.Yes:
                        return SaveScheme(false);
                    case DialogResult.No:
                        return true;
                    default:
                        return false;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Заполнить выпадающий список компонентов схемы.
        /// </summary>
        private void FillSchemeComponents()
        {
            try
            {
                cbSchComp.BeginUpdate();
                cbSchComp.Items.Clear();
                treeView1.Nodes.Clear();
                treeView1.SelectedNode = null;
                treeView1.SelectedNodes.Clear();

                if (editor.SchemeView != null)
                {
                    lock (editor.SchemeView)
                    {
                        cbSchComp.Items.Add(editor.SchemeView.SchemeDoc);

                        foreach (BaseComponent component in editor.SchemeView.Components.Values)
                        {
                            addComponentToTree(component);
                            BaseComponent group = editor.SchemeView.getHihghestGroup(component);
                            
                            if (group is Symbol symbol && group.ID != component.ID)
                            {
                                if(editor.SchemeView.isSymbol && symbol.ID == editor.SchemeView.MainSymbol.ID) 
                                    cbSchComp.Items.Add(component);
                            }
                            else
                            {
                                cbSchComp.Items.Add(component);
                            }
                        }
                    }
                }
            }
            finally
            {
                cbSchComp.EndUpdate();
            }
        }

        /// <summary>
        /// Отобразить свойства выбранных компонентов схемы.
        /// </summary>
        private void ShowSchemeSelection()
        {
            BaseComponent[] selection = editor.GetSelectedComponents();
            object[] selObjects;
            bool areGroups = editor.AreGroups(selection, out int groupID);
            if (!areGroups)
            {

                if (selection != null && selection.Length > 0)
                {
                    // выбор компонентов схемы
                    selObjects = selection;
                }
                else
                {
                    // выбор свойств документа схемы
                    selObjects = editor.SchemeView == null ?
                        null : new object[] { editor.SchemeView.SchemeDoc };
                }
            }
            else
            {
                editor.SchemeView.Components.TryGetValue(groupID, out BaseComponent selected);
                selObjects = new object[] { selected };
            }

            // отображение выбранных объектов
            propertyGrid.SelectedObjects = selObjects;

            // выбор объекта в выпадающем списке
            if (!schCompChanging)
            {
                cbSchComp.SelectedIndexChanged -= cbSchComp_SelectedIndexChanged;
                cbSchComp.SelectedItem = selObjects != null && selObjects.Length == 1 ? selObjects[0] : null;
                cbSchComp.SelectedIndexChanged += cbSchComp_SelectedIndexChanged;
            }

            //Nodes selection in treeview
            if (!this.noTreeviewSelectionEffect)
            {
                ArrayList newSelectedNodes = new ArrayList();
                treeView1.SelectedNode = null;

                if (areGroups)
                {
                    editor.SchemeView.Components.TryGetValue(groupID, out BaseComponent group);
                    TreeNode nodeToSelect = findNode(treeView1.Nodes, n => ((BaseComponent)n.Tag == group));
                    if (nodeToSelect != null) newSelectedNodes.Add(nodeToSelect);
                }
                else
                {
                    foreach (BaseComponent component in selection)
                    {
                        TreeNode nodeToSelect = findNode(treeView1.Nodes, n => ((BaseComponent)n.Tag == component));
                        if (nodeToSelect != null)
                        {
                            newSelectedNodes.Add(nodeToSelect);
                        }
                    }
                }
                treeView1.SelectedNodes = newSelectedNodes;
            }
            this.noTreeviewSelectionEffect = false;

            // установка доступности кнопок
            SetButtonsEnabled();

            toolStripButton2.Enabled = editor.SchemeView.isSymbol || cbSchComp.SelectedItem is Symbol;
            updateAliasParametersDisplay();
        }



        /// <summary>
        /// Подписаться на изменения схемы.
        /// </summary>
        private void SubscribeToSchemeChanges()
        {
            SchemeView schemeView = editor.SchemeView;

            if (schemeView != null)
            {
                schemeView.SchemeDoc.ItemChanged += Scheme_ItemChanged;

                foreach (BaseComponent component in schemeView.Components.Values)
                    component.ItemChanged += Scheme_ItemChanged;
            }
        }

        /// <summary>
        /// Установить доступность кнопок панели инструментов.
        /// </summary>
        private void SetButtonsEnabled()
        {
            miEditCut.Enabled = btnEditCut.Enabled = editor.SelectionNotEmpty;
            miEditCopy.Enabled = btnEditCopy.Enabled = editor.SelectionNotEmpty;
            miEditDelete.Enabled = btnEditDelete.Enabled = editor.SelectionNotEmpty;
            miEditPaste.Enabled = btnEditPaste.Enabled = editor.ClipboardNotEmpty;
            miEditPasteSpecial.Enabled = editor.ClipboardNotEmpty;
            miEditPointer.Enabled = btnEditPointer.Enabled = editor.PointerMode != PointerMode.Select;
            miEditUndo.Enabled = btnEditUndo.Enabled = editor.History.CanUndo;
            miEditRedo.Enabled = btnEditRedo.Enabled = editor.History.CanRedo;
            btnGroup.Enabled = editor.SelectionNotEmpty;
        }

        /// <summary>
        /// Выполнить метод потокобезопасно.
        /// </summary>
        private void ExecuteAction(Action action)
        {
            if (InvokeRequired)
                BeginInvoke(action);
            else
                action();
        }

        /// <summary>
        /// Обновить объект состояния формы.
        /// </summary>
        private void UpdateFormStateDTO(FormState formState)
        {
            formStateDTO = formState.GetFormStateDTO(WindowState != FormWindowState.Minimized);
        }

        /// <summary>
        /// Обновить объект состояния формы.
        /// </summary>
        private void UpdateFormStateDTO()
        {
            UpdateFormStateDTO(new FormState(this, true));
        }


        /// <summary>
        /// Выполнить заданное действие.
        /// </summary>
        public void PerformAction(FormAction formAction)
        {
            ExecuteAction(() =>
            {
                BringToFront();

                switch (formAction)
                {
                    case FormAction.New:
                        miFileNew_Click(null, null);
                        break;
                    case FormAction.Open:
                        miFileOpen_Click(null, null);
                        break;
                    case FormAction.Save:
                        miFileSave_Click(null, null);
                        break;
                    case FormAction.Cut:
                        miEditCut_Click(null, null);
                        break;
                    case FormAction.Copy:
                        miEditCopy_Click(null, null);
                        break;
                    case FormAction.Paste:
                        miEditPaste_Click(null, null);
                        break;
                    case FormAction.Undo:
                        miEditUndo_Click(null, null);
                        break;
                    case FormAction.Redo:
                        miEditRedo_Click(null, null);
                        break;
                    case FormAction.Pointer:
                        miEditPointer_Click(null, null);
                        break;
                    case FormAction.Delete:
                        miEditDelete_Click(null, null);
                        break;
                }
            });
        }

        /// <summary>
        /// Получить состояние формы.
        /// </summary>
        public FormStateDTO GetFormState()
        {
            return formStateDTO;
        }

        /// <summary>
        /// Returns a list of nodes from the component treeview
        /// </summary>
        private List<TreeNode> findNodes(TreeNodeCollection nodes, Func<TreeNode, Boolean> predicate, List<TreeNode> foundNodes = null)
        {
            if (foundNodes == null)
            {
                foundNodes = new List<TreeNode>();
            }
            foreach (TreeNode n in nodes)
            {
                if (predicate(n))
                {
                    foundNodes.Add(n);
                }
                if (n.Nodes.Count > 0)
                {
                    foundNodes = findNodes(n.Nodes, predicate, foundNodes);
                }
            }
            return foundNodes;
        }
        /// <summary>
        /// Returns a node from the component treeview
        /// </summary>
        private TreeNode findNode(TreeNodeCollection nodes, Func<TreeNode, Boolean> predicate)
        {
            TreeNode foundNode = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (predicate(nodes[i]))
                {
                    return nodes[i];
                }
                else if (nodes[i].Nodes.Count > 0)
                {
                    foundNode = findNode(nodes[i].Nodes, predicate);
                }
                if (foundNode != null)
                {
                    return foundNode;
                }
            }
            return null;
        }
        /// <summary>
        /// Adds a new component node in the treeview as 
        /// </summary>
        private void addComponentToTree(BaseComponent component)
        {
            TreeNode tn = null;
            bool isGroup = component is ComponentGroup || component.GetType().IsSubclassOf(typeof(ComponentGroup));
            tn = new TreeNode(component.ToString());

            tn.Tag = component;

            if (component.GroupId == -1)
            {
                treeView1.Nodes.Add(tn);
            }
            else
            {
                var groupNode = findNode(treeView1.Nodes, n => (n.Tag != null && ((BaseComponent)(n.Tag)).ID == component.GroupId));

                if (groupNode == null)
                {
                    treeView1.Nodes.Add(tn);
                }
                else
                {
                    groupNode.Nodes.Add(tn);
                }
            }
            if (isGroup)
            {
                List<TreeNode> groupMembers = findNodes(treeView1.Nodes, n => ((BaseComponent)(n.Tag)).GroupId == component.ID);
                foreach (TreeNode n in groupMembers)
                {
                    editComponentInTree((BaseComponent)(n.Tag));
                }
            }
            treeView1.Sort();
        }

        /// <summary>
        /// Removes empty groups from the component list
        /// </summary>
        private void removeEmptyGroups()
        {
            List<BaseComponent> emptyGroups = new List<BaseComponent>();
            foreach(BaseComponent group in editor.SchemeView.Components.Values.Where(x=>x is ComponentGroup))
            {
                if (editor.SchemeView.getGroupedComponents(group.ID).Count == 0)
                {
                    emptyGroups.Add(group);

                }
            }
            editor.History.BeginPoint();
            foreach (BaseComponent group in emptyGroups)
            {
                editor.SchemeView.Components.Remove(group.ID);
                group.OnItemChanged(SchemeChangeTypes.ComponentDeleted, group);
            }
            editor.History.EndPoint();

        }
        /// <summary>
        /// Removes empty groups from the treeview
        /// </summary>
        private void removeEmptyGroups(TreeNodeCollection nodes)
        {

            for (int i = 0; i < nodes.Count;)
            {
                if (nodes[i].Tag != null && ((BaseComponent)(nodes[i].Tag)).GetType() == new ComponentGroup().GetType() && nodes[i].Nodes.Count == 0)
                {
                    ((BaseComponent)(nodes[i].Tag)).OnItemChanged(SchemeChangeTypes.ComponentDeleted, (BaseComponent)(nodes[i].Tag));
                }
                else
                {
                    removeEmptyGroups(nodes[i].Nodes);
                    i++;
                }
            }
        }
        /// <summary>
        /// Removes the related component node from the treeview
        /// </summary>
        /// <param name="component"></param>
        private void removeComponentFromTree(BaseComponent component)
        {
            TreeNode tn = findNode(treeView1.Nodes, n => (n.Tag != null && (((BaseComponent)(n.Tag)).ID == component.ID)));
            if (tn == null)
            {

                return;
            }
            tn.Remove();
        }

        public void treeView1_onNodeSelection(object sender, TreeViewEventArgs e)
        {
            List<BaseComponent> compToSelect = new List<BaseComponent>();
            lock ((treeView1).SelectedNodes)
            {
                foreach (TreeNode tn in (treeView1).SelectedNodes)
                {
                    if (tn.Tag != null && tn.Tag.ToString() != "")
                    {
                        if (tn.Parent == null)
                        {
                            noTreeviewSelectionEffect = true;
                            BaseComponent component = tn.Tag as BaseComponent;
                            compToSelect.Add(component);
                        }
                        else if (!tn.Parent.Text.Contains("Symbol") || editor.SchemeView.MainSymbol != null)
                        {
                            noTreeviewSelectionEffect = true;
                            BaseComponent component = tn.Tag as BaseComponent;
                            compToSelect.Add(component);
                        }

                    }
                }
                editor.DeselectAll();
                bool disableDeleteBtn = false;
                foreach (BaseComponent comp in compToSelect)
                {
                    if (comp is ComponentGroup)
                    {
                        //case symbol in schema
                        if(editor.SchemeView.MainSymbol == null || comp.ID != editor.SchemeView.MainSymbol.ID)
                        {
                            foreach (BaseComponent child in editor.SchemeView.getGroupedComponents(comp.ID))
                            {
                                noTreeviewSelectionEffect = true;
                                editor.SelectComponent(child.ID, true);
                            }
                        }
                        disableDeleteBtn = disableDeleteBtn || (editor.SchemeView.isSymbol && comp.ID == editor.SchemeView.MainSymbol.ID);
                    }
                    noTreeviewSelectionEffect = true;
                    editor.SelectComponent(comp.ID, true);
                    if (disableDeleteBtn)
                    {
                        btnEditDelete.Enabled = false;
                        btnEditDelete.ToolTipText = "You cannot delete main symbol";
                    }
                    else
                    {
                        btnEditDelete.Enabled = true;
                        btnEditDelete.ToolTipText = "Delete selected components (Del)";
                    }
                }
            }
        }
        /// <summary>
        /// Updates the related node component in the treeview
        /// </summary>
        private void editComponentInTree(BaseComponent component)
        {
            TreeNode tn = findNode(treeView1.Nodes, n => (n.Tag != null && (((BaseComponent)(n.Tag)).ID == component.ID)));
            if (tn == null)
            {
                return;
            }
            tn.Tag = component;
            tn.Text = component.ToString();
            tn.Remove();
            if (component.GroupId == -1)
            {
                treeView1.Nodes.Add(tn);
            }
            else
            {
                TreeNode newParent = findNode(treeView1.Nodes, n => ((BaseComponent)(n.Tag)).ID == component.GroupId);
                if (newParent == null)
                {
                    return;
                }
                newParent.Nodes.Add(tn);
            }
        }

        private void Scheme_ItemChanged(object sender, SchemeChangeTypes changeType,
            object changedObject, object oldKey)
        {
            ExecuteAction(() =>
            {
                switch (changeType)
                {
                    case SchemeChangeTypes.ComponentAdded:
                        // привязка события на изменение компонента
                        ((BaseComponent)changedObject).ItemChanged += Scheme_ItemChanged;


                        // добавление компонента в выпадающий список
                        BaseComponent group = editor.SchemeView.getHihghestGroup((BaseComponent)changedObject);
                        if (group is Symbol && group.ID != ((BaseComponent)changedObject).ID)
                        {
                            if (editor.SchemeView.isSymbol && group.ID == editor.SchemeView.MainSymbol.ID)
                            {
                                cbSchComp.Items.Add(changedObject);
                            }
                            else
                            {
                                //do nothing
                            }
                        }
                        else
                        {
                            cbSchComp.Items.Add(changedObject);
                        }
                        addComponentToTree((BaseComponent)changedObject);

                        break;

                    case SchemeChangeTypes.ComponentChanged:
                        // обновление текста выпадающего списка при изменении отображаемого наименования выбранного объекта
                        object selItem = cbSchComp.SelectedItem;
                        if (selItem != null)
                        {
                            string newDisplayName = selItem.ToString();
                            string oldDisplayName = cbSchComp.Text;
                            if (oldDisplayName != newDisplayName)
                                cbSchComp.Items[cbSchComp.SelectedIndex] = selItem;
                        }
                        editComponentInTree((BaseComponent)changedObject);
                        break;

                    case SchemeChangeTypes.ComponentDeleted:
                        // удаление компонента из выпадающего списка
                        cbSchComp.Items.Remove(changedObject);
                        removeComponentFromTree((BaseComponent)changedObject);
                        break;
                }
                SetButtonsEnabled();
            });
        }

        private void Editor_ModifiedChanged(object sender, EventArgs e)
        {
            ExecuteAction(() => { Text = editor.Title; });
        }

        private void Editor_PointerModeChanged(object sender, EventArgs e)
        {
            ExecuteAction(() =>
            {
                // очистка типа создаваемых компонентов, если режим создания выключен
                if (!compTypesChanging && editor.PointerMode != PointerMode.Create)
                {
                    lvCompTypes.SelectedIndexChanged -= lvCompTypes_SelectedIndexChanged;
                    lvCompTypes.SelectedItems.Clear();
                    lvCompTypes.SelectedIndexChanged += lvCompTypes_SelectedIndexChanged;
                }

                // установка доступности кнопок
                SetButtonsEnabled();
            });
        }

        private void Editor_StatusChanged(object sender, EventArgs e)
        {
            // вывод статуса редактора
            ExecuteAction(() => { lblStatus.Text = editor.Status; });
        }

        private void Editor_SelectionChanged(object sender, EventArgs e)
        {
            // отображение свойств выбранных компонентов схемы
            ExecuteAction(ShowSchemeSelection);
        }

        private void Editor_SelectionPropsChanged(object sender, EventArgs e)
        {
            // обновление значений свойств
            ExecuteAction(propertyGrid.Refresh);
        }

        private void Editor_ClipboardChanged(object sender, EventArgs e)
        {
            // установка доступности кнопок
            ExecuteAction(SetButtonsEnabled);
        }

        private void History_HistoryChanged(object sender, EventArgs e)
        {
            // установка доступности кнопок
            ExecuteAction(SetButtonsEnabled);
        }


        private void FrmMain_Load(object sender, EventArgs e)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif

            // инициализация общих данных приложения
            appData.Init(Path.GetDirectoryName(Application.ExecutablePath), this);

            // локализация
            LocalizeForm();
            LocalizeAttributes();

            // проверка существования второй копии приложения
            if (SecondInstanceExists())
            {
                ScadaUiUtils.ShowInfo(AppPhrases.CloseSecondInstance);
                Close();
                log.WriteAction(Localization.UseRussian ?
                    "Вторая копия Редактора схем закрыта." :
                    "The second instance of Scheme Editor has been closed.");
                return;
            }

            // загрузка настроек приложения
            if (!settings.Load(appData.AppDirs.ConfigDir + Settings.DefFileName, out string errMsg))
            {
                log.WriteError(errMsg);
                ScadaUiUtils.ShowError(errMsg);
            }

            // загрузка компонентов
            SetComponentImages();
            appData.LoadComponents();

            // настройка элментов управления
            lvCompTypes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            lblStatus.Text = "";
            FillComponentTypes();

            // создание новой или загрузка существующей схемы
            string[] args = Environment.GetCommandLineArgs();
            InitScheme(args.Length > 1 ? args[1] : "");

            // загрузка состояния формы
            FormState formState = new FormState();
            if (formState.Load(appData.AppDirs.ConfigDir + FormState.DefFileName, out errMsg))
            {
                ImageEditor.ImageDir = formState.ImageDir;
                ofdScheme.InitialDirectory = formState.SchemeDir;
            }
            else
            {
                log.WriteError(errMsg);
                ScadaUiUtils.ShowError(errMsg);
            }
            formState.Apply(this);
            UpdateFormStateDTO();

            // запуск механизма редактора схем
            if (appData.StartEditor())
            {
                // открытие браузера со страницей редактора
                OpenBrowser();
            }
            else
            {
                ScadaUiUtils.ShowInfo(string.Format(AppPhrases.FailedToStartEditor, log.FileName));
                Close();
            }
            SchemeContext.GetInstance().SchemePath = editor.FileName;

            //RefreshAvailableSymbols();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            // проверка возможности закрыть схему
            e.Cancel = !ConfirmCloseScheme();
        }

        private void FrmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            // сохранение состояния формы
            FormState formState = new FormState(this)
            {
                SchemeDir = ofdScheme.InitialDirectory,
                ImageDir = ImageEditor.ImageDir
            };

            if (!formState.Save(appData.AppDirs.ConfigDir + FormState.DefFileName, out string errMsg))
            {
                log.WriteError(errMsg);
                ScadaUiUtils.ShowError(errMsg);
            }

            // завершение работы приложения
            appData.FinalizeApp();
        }

        private void FrmMain_MouseEnter(object sender, EventArgs e)
        {
            // активировать форму при наведении мыши
            if (ActiveForm != this)
                BringToFront();
            bool a = treeView1.SelectedNode != null;
        }

        private void FrmMain_Move(object sender, EventArgs e)
        {
            // притягивание формы к краям экрана при необходимости
            FormState correctFormState = new FormState(this, true);
            if (!correctFormState.PullToEdge(this))
                UpdateFormStateDTO(correctFormState);
        }

        private void FrmMain_Resize(object sender, EventArgs e)
        {
            UpdateFormStateDTO();
            lvCompTypes.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            // реализация горячих клавиш, которые не заданы для элементов главного меню
            if (ActiveControl != propertyGrid && e.KeyCode == Keys.Escape)
            {
                miEditPointer_Click(null, null);
            }
        }


        private void miFileNew_Click(object sender, EventArgs e)
        {
            // создание новой схемы
            if (ConfirmCloseScheme())
                InitScheme();
        }

        private void miFileOpen_Click(object sender, EventArgs e)
        {
            // открытие схемы из файла
            if (ConfirmCloseScheme())
            {
                ofdScheme.FileName = "";

                if (ofdScheme.ShowDialog() == DialogResult.OK)
                {
                    ofdScheme.InitialDirectory = Path.GetDirectoryName(ofdScheme.FileName);
                    InitScheme(ofdScheme.FileName);
                    SchemeContext.GetInstance().SchemePath = ofdScheme.FileName;
                    RefreshAvailableSymbols();
                }
            }
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            // сохранение схемы
            SaveScheme(false);
        }

        private void miFileSaveAs_Click(object sender, EventArgs e)
        {
            // сохранение схемы с выбором имени файла
            SaveScheme(true);
        }

        private void miFileOpenBrowser_Click(object sender, EventArgs e)
        {
            OpenBrowser();
        }

        private void miFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void miEditCut_Click(object sender, EventArgs e)
        {
            // копирование в буфер обмена и удаление выбранных компонентов схемы
            editor.CopyToClipboard();
            editor.DeleteSelected();
        }

        private void miEditCopy_Click(object sender, EventArgs e)
        {
            // копировать выбранные компоненты в буфер обмена
            editor.CopyToClipboard();
        }

        private void miEditPaste_Click(object sender, EventArgs e)
        {
            // включение режима вставки компонентов
            editor.PointerMode = PointerMode.Paste;
        }

        private void miEditPasteSpecial_Click(object sender, EventArgs e)
        {
            // включение режима специальной вставки компонентов
            FrmPasteSpecial frmPasteSpecial = new FrmPasteSpecial()
            {
                PasteSpecialParams = editor.PasteSpecialParams
            };

            if (frmPasteSpecial.ShowDialog() == DialogResult.OK)
                editor.PointerMode = PointerMode.PasteSpecial;
        }

        private void miEditUndo_Click(object sender, EventArgs e)
        {
            // отмена последнего действия
            schCompChanging = true;
            editor.Undo();
            schCompChanging = false;

            // обновление списка компонентов, т.к. при отмене происходит подмена объектов
            FillSchemeComponents();
            ShowSchemeSelection();
            updateAliasParametersDisplay();
        }

        private void miEditRedo_Click(object sender, EventArgs e)
        {
            // возврат последнего действия
            schCompChanging = true;
            editor.Redo();
            schCompChanging = false;

            // обновление списка компонентов, т.к. при возврате происходит подмена объектов
            FillSchemeComponents();
            ShowSchemeSelection();
            updateAliasParametersDisplay();
        }

        private void miEditPointer_Click(object sender, EventArgs e)
        {
            // включение режима выбора компонентов
            editor.PointerMode = PointerMode.Select;
        }

        private void miEditDelete_Click(object sender, EventArgs e)
        {
            // удаление выбранных компонентов схемы
            editor.DeleteSelected();
        }
        /// <summary>
        /// Updates the nodes selected in the treeview
        /// </summary>
        private void updateSelectionInTree()
        {
            BaseComponent[] selection = editor.GetSelectedComponents();
            if (selection == null)
            {
                return;
            }
            ArrayList newSelectedNodes = new ArrayList();
            treeView1.SelectedNode = null;
            foreach (BaseComponent component in selection)
            {
                TreeNode nodeToSelect = findNode(treeView1.Nodes, n => ((BaseComponent)n.Tag == component));
                if (nodeToSelect != null)
                {
                    newSelectedNodes.Add(nodeToSelect);
                }
            }
            treeView1.SelectedNodes = newSelectedNodes;
        }
        /// <summary>
        /// Returns the group ID the highest in the hierarchy of the array 
        /// 
        /// countCheck is true when the length of the array and the count of the group are equal
        /// </summary>
        private int getHighestGroupID(BaseComponent[] compArray,out bool countCheck)
        {
            int highestGroupID = -1;
            int diff = int.MaxValue;
            foreach (BaseComponent comp in compArray)
            {
                if (comp is ComponentGroup)
                {
                    if ( Math.Abs(compArray.Length - editor.SchemeView.getGroupedComponents(comp.ID).Count() - 1) < diff)
                    {
                        diff = Math.Abs(compArray.Length - editor.SchemeView.getGroupedComponents(comp.ID).Count() - 1);
                        highestGroupID = comp.ID;
                    }
                }
                else
                {
                    if (Math.Abs(compArray.Length - editor.SchemeView.getGroupedComponents(comp.GroupId).Count()) < diff)
                    {
                        diff = Math.Abs(compArray.Length - editor.SchemeView.getGroupedComponents(comp.GroupId).Count());
                        highestGroupID = comp.GroupId;
                    }
                }
            }
            countCheck = diff == 0;
            return highestGroupID;
        }

        private void miGroup_Click(object sender, EventArgs e)
        {
            BaseComponent[] selection = editor.GetSelectedComponents();
            int highestSelectedGroupId = getHighestGroupID(selection,out bool countCheck);
            bool containsComponentGroup = selection.Where(x => x is ComponentGroup).Count()>0;
            bool allSelectedAreTheSameGroup = true;

            foreach (BaseComponent comp in selection)
            {
                if (!editor.SchemeView.getGroupedComponents(highestSelectedGroupId).Contains(comp))
                {
                    if(comp is ComponentGroup)
                    {
                        if (comp.ID == highestSelectedGroupId) continue;
                    }
                    allSelectedAreTheSameGroup = false;
                    break;
                }
            }
            if (!allSelectedAreTheSameGroup)
            {
                allSelectedAreTheSameGroup = true;
                foreach (BaseComponent comp in selection)
                {
                    if (comp.GroupId == -1) continue;

                    BaseComponent group = selection.Where(x=>x.ID == comp.GroupId).FirstOrDefault();
                    if(group!=null) continue;

                    allSelectedAreTheSameGroup = false;
                    break;
                }
                highestSelectedGroupId = -1;
            }

            editor.History.BeginPoint();

            //Ungroups
            if (allSelectedAreTheSameGroup && countCheck)
            {
                if (containsComponentGroup)
                {
                    editor.SchemeView.Components.TryGetValue(highestSelectedGroupId, out BaseComponent group);
                    if (group.GroupId != -1)
                    {
                        editor.SchemeView.Components.TryGetValue(group.GroupId, out BaseComponent currentGroup);
                        group.GroupId = currentGroup.GroupId;
                        editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentChanged, group);
                    }
                    else
                    {
                        foreach (BaseComponent comp in selection)
                        {
                            if (comp.GroupId == highestSelectedGroupId)
                            {
                                comp.GroupId = -1;
                                editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentChanged, comp);
                            }
                        }
                        editor.SchemeView.Components.Remove(group.ID);
                        editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentDeleted, group);

                    }
                }
                else
                {
                    foreach (BaseComponent comp in selection)
                    {

                        editor.SchemeView.Components.TryGetValue(comp.GroupId, out BaseComponent currentGroup);
                        comp.GroupId = currentGroup.GroupId;
                        editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentChanged, comp);

                    }

                    editor.SchemeView.Components.TryGetValue(highestSelectedGroupId, out BaseComponent group);
                    editor.SchemeView.Components.Remove(highestSelectedGroupId);

                    editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentDeleted, group);

                }
            }
            //groups
            else
            {
                int minX = int.MaxValue;
                int minY = int.MaxValue;

                BaseComponent newGroup = new ComponentGroup();
                newGroup.ID = editor.SchemeView.GetNextComponentID();

                newGroup.SchemeView = editor.SchemeView;
                newGroup.ItemChanged += Scheme_ItemChanged;

                int zIndex = editor.SchemeView.Components.Values.Select(x=>x.ZIndex).OrderByDescending(x=>x).FirstOrDefault()+1;
                newGroup.ZIndex = zIndex;
                zIndex *= 100;

                if (!allSelectedAreTheSameGroup)
                    MessageBox.Show("Cannot group component from different groups", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                else
                {
                    foreach (BaseComponent c in selection.OrderBy(x=>x.ZIndex))
                    {
                        editor.SchemeView.Components.TryGetValue(c.GroupId, out BaseComponent cGroup);
                        c.ZIndex = zIndex;
                        zIndex++;
                        if (c.Location.X < minX) minX = c.Location.X;
                        if (c.Location.Y < minY) minY = c.Location.Y;

                        if (selection.Contains(cGroup)) continue;

                        c.GroupId = newGroup.ID;
                    }

                    newGroup.GroupId = highestSelectedGroupId;
                    Point location = new Point(minX, minY);
                    newGroup.Location = location;

                    editor.SchemeView.Components[newGroup.ID] = newGroup;
                    editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentAdded, newGroup);

                    foreach (BaseComponent c in selection)
                    {
                        editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentChanged, c);
                    }
                }
            }

            removeEmptyGroups();
            editor.History.EndPoint();
            updateSelectionInTree();
        }
        private void miToolsOptions_Click(object sender, EventArgs e)
        {
            // отображение формы настроек
            if (FrmSettings.ShowDialog(settings, out bool restartNeeded))
            {
                if (settings.Save(appData.AppDirs.ConfigDir + Settings.DefFileName, out string errMsg))
                {
                    if (restartNeeded)
                        ScadaUiUtils.ShowInfo(AppPhrases.RestartNeeded);
                }
                else
                {
                    ScadaUiUtils.ShowError(errMsg);
                }
            }
        }

        private void miHelpAbout_Click(object sender, EventArgs e)
        {
            // отображение формы о программе
            FrmAbout.ShowAbout(appData.AppDirs.ExeDir, log, this);
        }


        private void lvCompTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            // выбор компонента для добавления на схему
            compTypesChanging = true;

            string typeName = "";

            //Symboles
            if (lvCompTypes.SelectedItems.Count > 0 && lvCompTypes.SelectedItems[0].Group.Header == "Symbols")
            {
                editor.SymbolPath = availableSymbols.ElementAt(lvCompTypes.SelectedIndices[0]-(lvCompTypes.Items.Count - availableSymbols.Count)).Key;
                //editor.SymbolPath = findSymboleInAvailableList(lvCompTypes.SelectedItems[0].Text);
                if (File.Exists(editor.SymbolPath))
                {
                    XmlDocument xmlDoc = new XmlDocument();

                    try
                    {

                        xmlDoc.Load(editor.SymbolPath);

                        XmlNode mainSymbolNode = xmlDoc.SelectSingleNode(".//MainSymbol");
                        XmlNode nameNode = mainSymbolNode.SelectSingleNode("Name");
                        typeName = nameNode.InnerText + " - Symbol";
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                }
                else
                {
                    MessageBox.Show("Symbol not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                typeName = lvCompTypes.SelectedItems.Count > 0 ?
                    lvCompTypes.SelectedItems[0].Tag as string : "";
            }

            if (string.IsNullOrEmpty(typeName))
            {
                // включение режима выбора компонентов
                editor.PointerMode = PointerMode.Select;
            }
            else
            {
                // включение режима создания компонента
                editor.NewComponentTypeName = typeName;
                editor.PointerMode = PointerMode.Create;
            }

            compTypesChanging = false;
        }

        private void cbSchComp_SelectedIndexChanged(object sender, EventArgs e)
        {
            // отображение свойств объекта, выбранного в выпадающем списке
            schCompChanging = true;

            if (cbSchComp.SelectedItem is BaseComponent component)
            {
                editor.SelectComponent(component.ID);
                if (!editor.SchemeView.isSymbol || component.ID != editor.SchemeView.MainSymbol.ID)
                {
                    btnEditDelete.Enabled = true;
                    btnEditDelete.ToolTipText = "Delete selected components (Del)";
                }
                else
                {
                    btnEditDelete.Enabled = false;
                    btnEditDelete.ToolTipText = "You cannot delete main symbol";
                }
                updateAliasParametersDisplay();
            }
            else
            {
                editor.DeselectAll();
            }
            toolStripButton2.Enabled = editor.SchemeView.isSymbol || cbSchComp.SelectedItem is Symbol;
            schCompChanging = false;
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (cbSchComp.SelectedItem != null)
            {
                editor.History.BeginPoint();
                //Edit all the components within the group
                if (cbSchComp.SelectedItem is ComponentGroup group)
                {
                    List<BaseComponent> components = editor.SchemeView.getGroupedComponents(group.ID);
                    if (e.ChangedItem.Label == "X" || e.ChangedItem.Label == "Y" || e.ChangedItem.Label == "ZIndex")
                    {
                        foreach (BaseComponent component in components)
                        {
                            Point location = component.Location;
                            int valueDiff = (int)e.ChangedItem.Value - (int)e.OldValue;

                            if (e.ChangedItem.Label == "X") location.X += valueDiff;
                            else if (e.ChangedItem.Label == "Y") location.Y += valueDiff;
                            else component.ZIndex += valueDiff * 100;

                            component.Location = location;

                            component.OnItemChanged(SchemeChangeTypes.ComponentChanged, component);
                        }
                    }

                    group.OnItemChanged(SchemeChangeTypes.ComponentChanged, group);
                }

                else
                {
                    if (cbSchComp.SelectedItem is SchemeDocument document)
                        document.OnItemChanged(SchemeChangeTypes.SchemeDocChanged, cbSchComp.SelectedItem);
                    else if (cbSchComp.SelectedItem is BaseComponent component)
                        component.OnItemChanged(SchemeChangeTypes.ComponentChanged, cbSchComp.SelectedItem);
                }
                editor.History.EndPoint();
                updateAliasParametersDisplay();
            }
        }
        public GridItem GetRootGridItem(GridItem gridItem)
        {
            return gridItem.Parent != null ? GetRootGridItem(gridItem.Parent) : gridItem;
        }
        public List<GridItem> GetPropertyGridItems(PropertyGrid propertyGrid)
        {
            List<GridItem> gridItems = new List<GridItem>();
            var rootItem = GetRootGridItem(propertyGrid.SelectedGridItem);
            if (propertyGrid != null && rootItem != null)
            {
                GetSubGridItems(rootItem, gridItems);
            }
            return gridItems;
        }
        private void GetSubGridItems(GridItem gridItem, List<GridItem> gridItems)
        {
            if (gridItem != null)
            {
                foreach (GridItem item in gridItem.GridItems)
                {
                    if(item.PropertyDescriptor != null)
                    {
                        gridItems.Add(item);
                    }
                    GetSubGridItems(item, gridItems);
                }
            }
        }
        /// <summary>
        /// Updates propertyGrid display, considering links between component parameters and aliases
        /// </summary>
        private void updateAliasParametersDisplay()
        {
            BaseComponent selectedComponent = cbSchComp.SelectedItem as BaseComponent;
            if (selectedComponent == null)
            {
                return;
            }
            propertyGrid.SelectedObject = selectedComponent;
            var aliasRelatedPropertiesNames = selectedComponent.AliasesDictionnary.Keys.ToArray();
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(selectedComponent);
            var aliasRelatedProperties = GetPropertyGridItems(propertyGrid);
            aliasRelatedProperties = aliasRelatedProperties.Where(item => aliasRelatedPropertiesNames.Contains(item.PropertyDescriptor.Name)).ToList();
            ICustomTypeDescriptor customTypeDescriptor = TypeDescriptor.GetProvider(selectedComponent).GetTypeDescriptor(selectedComponent);
            PropertyDescriptorCollection newProperties = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
            foreach (PropertyDescriptor prop in properties)
            {
                if (!aliasRelatedPropertiesNames.Contains(prop.Name))
                {
                    newProperties.Add(prop);
                }
            }
            foreach (var aliasProperty in aliasRelatedProperties)
            {
                PropertyDescriptor aliasRelatedPropDescriptor = aliasProperty.PropertyDescriptor;
                if (aliasRelatedPropDescriptor != null)
                {
                    PropertyDescriptor customPropertyDescriptor = new CustomPropertyDescriptor(aliasRelatedPropDescriptor, aliasProperty.Label + " (Alias)", true);
                    newProperties.Add(customPropertyDescriptor);
                }
            }

            propertyGrid.SelectedObject = new CustomTypeDescriptor(customTypeDescriptor, newProperties);
        }
        
        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if(e.NewSelection == null || e.NewSelection.PropertyDescriptor == null)
            {
                return;
            }
            string selectedPropertyName = e.NewSelection.PropertyDescriptor.Name;
            BaseComponent selectedComponent = cbSchComp.SelectedItem as BaseComponent;

            toolStripButton1.Enabled = false;
            toolStripButton1.ToolTipText = "Cannot link an alias during scheme edition";
            if (editor.SchemeView.MainSymbol != null)
            {
                if (selectedComponent == null)
                {
                    toolStripButton1.Enabled = false;
                    toolStripButton1.ToolTipText = "Select a component property to link an alias";
                    return;
                }
                if (selectedComponent.ID == editor.SchemeView.MainSymbol.ID)
                {
                    toolStripButton1.Enabled = false;
                    toolStripButton1.ToolTipText = "Cannot link own alias";
                    return;
                }
                if (e.NewSelection.PropertyDescriptor.IsReadOnly && !selectedComponent.AliasesDictionnary.Keys.Contains(selectedPropertyName))
                {
                    toolStripButton1.Enabled = false;
                    toolStripButton1.ToolTipText = "This property cannot be modified";
                }
                else
                {
                    toolStripButton1.Enabled = true;
                    toolStripButton1.ToolTipText = "Link to a symbol alias";
                }
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            BaseComponent selectedComponent = cbSchComp.SelectedItem as BaseComponent;
            if (editor.SchemeView.MainSymbol == null) { return; }
            Symbol parentSymbol = editor.SchemeView.MainSymbol;
            GridItem selectedProperty = propertyGrid.SelectedGridItem;

            string selectedPropertyName = selectedProperty.PropertyDescriptor.Name;
            bool isCnlProperty = (selectedPropertyName == "InCnlNumCustom" || selectedPropertyName == "CtrlCnlNumCustom");

            bool isColor = selectedProperty.PropertyDescriptor.Attributes.OfType<CM.EditorAttribute>().Any(attribute => attribute.EditorTypeName == typeof(Model.PropertyGrid.ColorEditor).AssemblyQualifiedName);
            string selectedPropertyTypeName = isColor ? "Color" : selectedProperty.PropertyDescriptor.PropertyType.Name;
            List<Alias> availableAliases = new List<Alias>();
            availableAliases = parentSymbol.AliasList.Where(a => isCnlProperty ? a.isCnlLinked : (!a.isCnlLinked && a.AliasTypeName   == selectedPropertyTypeName)).ToList();
            int defaultSelectionIndex = -1;
            if (selectedComponent.AliasesDictionnary.ContainsKey(selectedPropertyName))
            {
                defaultSelectionIndex = availableAliases.FindIndex(a=>a.Name == selectedComponent.AliasesDictionnary[selectedPropertyName].Name);
            }
            FrmAliasSelection frmAliasSelection = new FrmAliasSelection(selectedProperty.Label, availableAliases, defaultSelectionIndex);
            if (frmAliasSelection.ShowDialog() == DialogResult.OK)
            {
                //find component property to update
                var componentProperty = selectedComponent.GetType().GetProperty(selectedPropertyName);
                if(componentProperty == null)
                {
                    return;
                }

                //update mapping between component properties and alias
                selectedComponent.AliasesDictionnary.Remove(selectedPropertyName);
                if(frmAliasSelection.selectedAlias != null)
                {
                    selectedComponent.AliasesDictionnary.Add(selectedPropertyName, frmAliasSelection.selectedAlias);
                }

                //Copy alias value in component parameter
                var oldProperty = selectedProperty.Value;

                if (frmAliasSelection.selectedAlias != null)
                {
                    
					//   componentProperty.SetValue(selectedComponent, int.TryParse(frmAliasSelection.selectedAlias.Value), null);
					                 
					if (oldProperty.GetType().Name.Equals("Int32")  )
					{
						componentProperty.SetValue(selectedComponent, (int)(frmAliasSelection.selectedAlias.Value), null);
					}
					else if (oldProperty.GetType().Name.Equals("Double") )
					{
						componentProperty.SetValue(selectedComponent, (double)frmAliasSelection.selectedAlias.Value, null);
					}
					else if (oldProperty.GetType().Name.Equals("Boolean") )
					{
						componentProperty.SetValue(selectedComponent,frmAliasSelection.selectedAlias.Value, null);
					}
					else if (oldProperty.GetType().Name.Equals("String"))
					{
						componentProperty.SetValue(selectedComponent, frmAliasSelection.selectedAlias.Value.ToString(), null);
					}
					else
					{
						Console.WriteLine($"Type {oldProperty.GetType().Name} not handled");
					}

					if (isCnlProperty)
                    {
                        var componentChannelPropertyName = selectedPropertyName.Substring(0, selectedPropertyName.Length - 6);
                        var componentChannelProperty = selectedComponent.GetType().GetProperty(componentChannelPropertyName);
                        var ChannelNumber = editor.SchemeView.MainSymbol.AliasCnlDictionary[frmAliasSelection.selectedAlias.Name];
                        componentChannelProperty.SetValue(selectedComponent, ChannelNumber, null);
                    }
                }

                propertyGrid.SelectedObject = selectedComponent;
                propertyGrid_PropertyValueChanged(propertyGrid, new PropertyValueChangedEventArgs(selectedProperty, oldProperty));
                return;
            }
        }

		private void handleUpdateAlias(object sender, OnUpdateAliasEventArgs e)
        {
            //update values in components that use aliases
            foreach (BaseComponent c in editor.SchemeView.Components.Values)
            {
                //handle deletion
                if (e.NewAlias == null)
                {
                    c.AliasesDictionnary = c.AliasesDictionnary.Where(entry => entry.Value.Name!= e.OldAlias.Name).ToDictionary(pair => pair.Key, pair => pair.Value);
                    continue;
                }
                if (!editor.SchemeView.isSymbol && c.GroupId != (cbSchComp.SelectedItem as Symbol).ID)
                {
                    continue;
                }

                //find component property to update
                var dictionnaryEntriesToModify = c.AliasesDictionnary.Where(entry => entry.Value.Name == e.NewAlias.Name).ToList();
                 
                foreach (var entry in dictionnaryEntriesToModify)
                {
                    var componentProperty = c.GetType().GetProperty(entry.Key);
                    if (componentProperty == null)
                    {
                        continue;
                    }

                    if (componentProperty.GetGetMethod().ReturnType.UnderlyingSystemType.Name.Equals("Int32"))
                    {
                        componentProperty.SetValue(c, (int)e.NewAlias.Value, null);
                    }
                    else if (componentProperty.GetGetMethod().ReturnType.UnderlyingSystemType.Name.Equals("Double"))
                    {

                        componentProperty.SetValue(c, (double)e.NewAlias.Value, null);

                    }
                    else if (componentProperty.GetGetMethod().ReturnType.UnderlyingSystemType.Name.Equals("Boolean"))
                    {

                        componentProperty.SetValue(c, e.NewAlias.Value, null);
                    }
                    else if (componentProperty.GetGetMethod().ReturnType.UnderlyingSystemType.Name.Equals("String"))
                    {

                        componentProperty.SetValue(c, e.NewAlias.Value.ToString(), null);
                    }
                    else
                    {
                        Console.WriteLine($"Type {componentProperty.GetGetMethod().ReturnType.UnderlyingSystemType.Name} not handled");
                    }

					//componentProperty.SetValue(c, e.NewAlias.Value, null);
                    if(entry.Key == "InCnlNumCustom" || entry.Key == "CtrlCnlNumCustom")
                    {
                        var componentChannelPropertyName = entry.Key.Substring(0, entry.Key.Length - 6);
                        var componentChannelProperty = c.GetType().GetProperty(componentChannelPropertyName);

						if (editor?.SchemeView?.MainSymbol?.AliasCnlDictionary != null && e?.NewAlias?.Name != null)
						{
							if (editor.SchemeView.MainSymbol.AliasCnlDictionary.TryGetValue(e.NewAlias.Name, out var ChannelNumber))
							{
								componentChannelProperty.SetValue(c, ChannelNumber, null);
							}
							else
							{
								Console.WriteLine($" {e.NewAlias.Name} Not found in the dictionnary.");
							}
						}
						
					}
				}

                c.OnItemChanged(SchemeChangeTypes.ComponentChanged, c);
            }
            updateAliasParametersDisplay();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var aliasCRUD = new FrmAliasesList(editor.SchemeView.isSymbol ? editor.SchemeView.MainSymbol : cbSchComp.SelectedItem as Symbol, editor.SchemeView.isSymbol);
            aliasCRUD.OnUpdateAlias += handleUpdateAlias;
            aliasCRUD.ShowDialog();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            if(editor.SchemeView == null)
            {
                return;
            }
            if (editor.SchemeView.isSymbol)
            {
                convertSymbolToScheme();
            }
            else
            {
                convertSchemeToSymbol();
            }
            toolStripButton2.Enabled = editor.SchemeView.isSymbol;
            toolStripStatusLabel1.Text = editor.SchemeView != null ? (editor.SchemeView.isSymbol ? "Editing symbol" : "Editing scheme") : "";
            toolStripButton3.ToolTipText = editor.SchemeView != null ? (editor.SchemeView.isSymbol ? "Convert into scheme" : "Convert into symbol") : "";

            int newCbIndex = cbSchComp.SelectedItem == null ? 0 : cbSchComp.SelectedIndex;

            if(tabControl.SelectedIndex == 1)
            {
                //force information refresh
                cbSchComp.SelectedItem = null;
                cbSchComp.SelectedItem = cbSchComp.Items[newCbIndex];
            }
            Text = editor.Title;
            RefreshAvailableSymbols();
        }
        private void convertSymbolToScheme()
        {
            if (editor.FileName != "")
            {
                existingSymbolWasConverted = true;
            }
            existingSchemeWasConverted = false;
            editor.SchemeView.isSymbol = false;
            //remove main symbol from all related components
            foreach (BaseComponent c in editor.SchemeView.Components.Values)
            {
                if (c.GroupId == editor.SchemeView.MainSymbol.ID)
                {
                    c.GroupId = -1;
                    c.OnItemChanged(SchemeChangeTypes.ComponentChanged, c);
                }
            }

            //remove main symbol from the scheme
            editor.SchemeView.Components.Remove(editor.SchemeView.MainSymbol.ID);
            editor.SchemeView.MainSymbol.OnItemChanged(SchemeChangeTypes.ComponentDeleted, editor.SchemeView.MainSymbol);
            editor.SchemeView.MainSymbol = null;

            btnEditDelete.ToolTipText = "Delete selected components (Del)";
            return;
        }
        private void convertSchemeToSymbol()
        {
            if (editor.FileName != "")
            {
                existingSchemeWasConverted = true;
            }
            existingSymbolWasConverted = false;
            editor.SchemeView.isSymbol = true;

            //create main symbol
            Symbol mainSymbol = new Symbol();
            mainSymbol.ID = editor.SchemeView.GetNextComponentID();
            mainSymbol.SchemeView = editor.SchemeView;
            mainSymbol.ItemChanged += Scheme_ItemChanged;
            mainSymbol.ZIndex = 0;
            mainSymbol.Location = new Point(0, 0);
            mainSymbol.Name = "MainSymbol";
            editor.SchemeView.MainSymbol = mainSymbol;
            editor.SchemeView.Components.Add(mainSymbol.ID, mainSymbol);
            editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentAdded, mainSymbol);
            List<BaseComponent> componentsToChange = editor.SchemeView.Components.Values.Where(c => c is Symbol && c.ID != mainSymbol.ID).ToList();
            foreach (BaseComponent c in componentsToChange)
            {
                List<BaseComponent> childrenToChange = editor.SchemeView.Components.Values.Where(x => x.GroupId == c.ID).ToList();

                foreach (BaseComponent child in childrenToChange)
                {
                    child.GroupId = (c.GroupId != -1) ? c.GroupId : -1;
                    child.AliasesDictionnary = new Dictionary<string, Alias>();
                    child.OnItemChanged(SchemeChangeTypes.ComponentChanged, child);
                }
                editor.SchemeView.Components.Remove(c.ID);
                editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentDeleted, c);
            }
            foreach (BaseComponent c in editor.SchemeView.Components.Values.Where(c=>c.GroupId == -1 && c.ID != mainSymbol.ID))
            {
                c.GroupId = mainSymbol.ID;
                editor.SchemeView.SchemeDoc.OnItemChanged(SchemeChangeTypes.ComponentChanged, c);
            }
            FillSchemeComponents();
            return;
        }

        private void btnFileNew_Click(object sender, EventArgs e)
        {
            // создание новой схемы
            if (ConfirmCloseScheme())
                InitScheme();
        }
    }
}
