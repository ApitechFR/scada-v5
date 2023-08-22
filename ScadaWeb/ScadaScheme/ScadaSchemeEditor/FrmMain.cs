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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Button = System.Windows.Forms.Button;
using ListViewItem = System.Windows.Forms.ListViewItem;

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

            editor.ModifiedChanged += Editor_ModifiedChanged;
            editor.PointerModeChanged += Editor_PointerModeChanged;
            editor.StatusChanged += Editor_StatusChanged;
            editor.SelectionChanged += Editor_SelectionChanged;
            editor.SelectionPropsChanged += Editor_SelectionPropsChanged;
            editor.ClipboardChanged += Editor_ClipboardChanged;
            editor.History.HistoryChanged += History_HistoryChanged;

            SchemeContext.GetInstance().SchemePath = editor.FileName;
        }

        private void LoadAvailableSymbols()
        {
            string xmlPath = Path.GetFullPath(appData.AppDirs.SymbolDir) + "\\index.xml";
            Dictionary<string, string> symbolsDictionary = new Dictionary<string, string>();

            try
            {
                if (File.Exists(xmlPath))
                {
                    //Populate available symbols from index.xml
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(xmlPath);
                    XmlNodeList entries = xmlDoc.SelectNodes("//symbol");
                    foreach (XmlNode entry in entries)
                    {
                        string name = entry.Attributes["name"].Value;
                        string path = entry.Attributes["path"].Value;

                        symbolsDictionary[name] = path;
                    }

                    ////
                    ////lvCompTypes.View = View.Details;
                    //lvCompTypes.Columns.Add("Action", 50);
                    ////lvCompTypes.Columns.Add("Action", 50);

                    //ListViewItem item1 = new ListViewItem("Élément 1"); 
                    //item1.SubItems.Add("Bouton 1");
                    //lvCompTypes.Items.Add(item1);

                    //ListViewItem item2 = new ListViewItem("Élément 2");
                    //item2.SubItems.Add(""); // Laissez la cellule vide pour le bouton
                    //lvCompTypes.Items.Add(item2);

                    lvCompTypes.DrawSubItem += ListView1_DrawSubItem;

                    availableSymbols = symbolsDictionary;
                    ListViewGroup symbolsViewGroup = new ListViewGroup("Symbols");
                    foreach (var s in availableSymbols)
                    {
                        lvCompTypes.Items.Add(new ListViewItem(s.Key, "component.png", symbolsViewGroup){ IndentCount = 1 });
                    }
                    lvCompTypes.Groups.Add(symbolsViewGroup);

                    lvCompTypes.MouseClick += ListView1_MouseClick;
                }
            }
            catch (Exception ex)
            {
                log.WriteException(ex, "Error: " + ex.Message);
            }
        }
        private void ListView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem clickedItem = lvCompTypes.GetItemAt(e.X, e.Y);
                if (clickedItem != null)
                {
                    // Créez un menu contextuel et ajoutez une option
                    ContextMenuStrip contextMenuStrip = new ContextMenuStrip();
                    ToolStripMenuItem optionMenuItem = new ToolStripMenuItem("Mon option");
                    contextMenuStrip.Items.Add(optionMenuItem);
                   // optionMenuItem.Image = Properties.Resources.MyImage; // Remplacez MyImage par le nom de votre image

                    // Associez un gestionnaire d'événements à l'option du menu
                    optionMenuItem.Click += (s, args) =>
                    {
                        MessageBox.Show("Option cliquée pour : " + clickedItem.Text);
                    };

                    // Affichez le menu contextuel à l'emplacement du clic
                    contextMenuStrip.Show(lvCompTypes, e.Location);
                }
            }
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
        private void InitScheme(string fileName = "", bool isSymbol = false)
        {
            bool loadOK;
            string errMsg;

            if (string.IsNullOrEmpty(fileName))
            {
                loadOK = true;
                errMsg = "";
                editor.NewScheme(isSymbol);

            }
            else
            {
                loadOK = editor.LoadSchemeFromFile(fileName, out errMsg,isSymbol);
            }

            appData.AssignViewStamp(editor.SchemeView);
            FillSchemeComponents();
            ShowSchemeSelection();
            SubscribeToSchemeChanges();
            editor.History.MakeCopy(editor.SchemeView);
            Text = editor.Title;

            if (!loadOK)
                ScadaUiUtils.ShowError(errMsg);


            if (editor.SchemeView.isSymbol) toolStripButton2.Enabled = true;
            if (!editor.SchemeView.isSymbol) toolStripButton2.Enabled = false;
        }

        /// <summary>
        /// Сохранить схему.
        /// </summary>
        private bool SaveScheme(bool saveAs,bool asSymbol = false)
        {
            bool result = false;
            bool refrPropGrid = propertyGrid.SelectedObject is SchemeDocument document &&
                document.Version != SchemeUtils.SchemeVersion;
            string fileName = "";

            if (string.IsNullOrEmpty(editor.FileName))
            {
                sfdScheme.FileName = !asSymbol ? Editor.DefSchemeFileName : Editor.DefSymbolFileName;
                sfdScheme.RestoreDirectory = false;
                if (asSymbol) sfdScheme.InitialDirectory = Path.GetFullPath(appData.AppDirs.SymbolDir);
                saveAs = true;
            }
            else
            {
                fileName = editor.FileName;
                sfdScheme.InitialDirectory = Path.GetDirectoryName(fileName);
                sfdScheme.FileName = Path.GetFileName(fileName);
            }

            if (saveAs && sfdScheme.ShowDialog() == DialogResult.OK)
                fileName = sfdScheme.FileName;

            if (!string.IsNullOrEmpty(fileName))
            {
                // сохранение схемы
                if (editor.SaveSchemeToFile(fileName, out string errMsg,asSymbol))
                {
                    updateSymbolIndex(Path.GetFullPath(appData.AppDirs.SymbolDir)+"\\index.xml",fileName);
                    result = true;
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

                if (File.Exists(xmlPath))
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

                XmlNode entryToUpdate = xmlDoc.SelectSingleNode($"//symbol[@symbolId='{currentSymbol.SymbolId}' or @path='{symbolFileName}']");

                if (entryToUpdate != null)
                {
                    entryToUpdate.Attributes["lastModificationDate"].Value = DateTime.Now.ToString();
                }
                else
                {
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
                            cbSchComp.Items.Add(component);
                            addComponentToTree(component);
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
            bool isGroup = component.GetType() == new ComponentGroup().GetType();
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
                if (editor.getGroupedComponents(group.ID).Count == 0)
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
            lock (((TreeViewMultipleSelection)treeView1).SelectedNodes)
            {
                foreach (TreeNode tn in ((TreeViewMultipleSelection)treeView1).SelectedNodes)
                {
                    if (tn.Tag != null && tn.Tag.ToString() != "")
                    {
                        this.noTreeviewSelectionEffect = true;
                        BaseComponent component = tn.Tag as BaseComponent;
                        compToSelect.Add(component);
                    }
                }
                editor.DeselectAll();
                foreach (BaseComponent comp in compToSelect)
                {
                    if (comp is ComponentGroup)
                    {
                        if (comp.ID != editor.SchemeView.MainSymbol.ID)
                        {
                            foreach (BaseComponent child in editor.getGroupedComponents(comp.ID))
                            {
                                this.noTreeviewSelectionEffect = true;

                                editor.SelectComponent(child.ID, true);
                            }
                        }
                    }
                    this.noTreeviewSelectionEffect = true;

                    editor.SelectComponent(comp.ID, true);

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
                        cbSchComp.Items.Add(changedObject);
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

            LoadAvailableSymbols();
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
                }
            }
        }

        private void miFileSave_Click(object sender, EventArgs e)
        {
            // сохранение схемы
            SaveScheme(false,editor.SchemeView.isSymbol);
        }

        private void miFileSaveAs_Click(object sender, EventArgs e)
        {
            // сохранение схемы с выбором имени файла
            SaveScheme(true,editor.SchemeView.isSymbol);
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
                    if ( Math.Abs(compArray.Length - editor.getGroupedComponents(comp.ID).Count() - 1) < diff)
                    {
                        diff = Math.Abs(compArray.Length - editor.getGroupedComponents(comp.ID).Count() - 1);
                        highestGroupID = comp.ID;
                    }
                }
                else
                {
                    if (Math.Abs(compArray.Length - editor.getGroupedComponents(comp.GroupId).Count()) < diff)
                    {
                        diff = Math.Abs(compArray.Length - editor.getGroupedComponents(comp.GroupId).Count());
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
                if (!editor.getGroupedComponents(highestSelectedGroupId).Contains(comp))
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
                string path = findSymboleInAvailableList(lvCompTypes.SelectedItems[0].Text);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(path);

                XmlNode mainSymbolNode = xmlDoc.SelectSingleNode(".//MainSymbol");
                XmlNode nameNode = mainSymbolNode.SelectSingleNode("Name");
                typeName = nameNode.InnerText;
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
                if (component.GroupId != -1)
                {
                    TreeNode SymbolNode = findNode(treeView1.Nodes, n => 
                        {
                            BaseComponent bc = (BaseComponent)n.Tag;
                            return (bc.ID == component.GroupId && bc.GetType() == typeof(Symbol));
                        }
                    );
                    if(SymbolNode != null)
                    {
                        toolStripButton1.Enabled = true;
                        toolStripButton1.ToolTipText = "Link to a symbol property";
                    }
                    else
                    {
                        // todo : set to false
                        toolStripButton1.Enabled = true;
                        toolStripButton1.ToolTipText = "The component has to be a symbol child";
                    }
                }
                updateAliasParametersDisplay();
            }
            else
            { 
                editor.DeselectAll();
            }


                schCompChanging = false;
        }

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            // отслеживание изменений
            if (propertyGrid.SelectedObjects != null)
            {

                editor.History.BeginPoint();
                if (propertyGrid.SelectedObject is ComponentGroup group)
                {
                    //Edit all the components within the group
                    List<BaseComponent> components = editor.getGroupedComponents(group.ID);
                    if (e.ChangedItem.Label == "X" || e.ChangedItem.Label == "Y" || e.ChangedItem.Label == "ZIndex")
                    {
                        foreach (BaseComponent component in components)
                        {
                            Point location = component.Location;
                            int valueDiff = (int)e.ChangedItem.Value - (int)e.OldValue;

                            if (e.ChangedItem.Label == "X") location.X += valueDiff;
                            else if (e.ChangedItem.Label == "Y") location.Y += valueDiff;
                            else component.ZIndex += valueDiff*100;

                            component.Location = location;

                            component.OnItemChanged(SchemeChangeTypes.ComponentChanged, component);
                        }
                    }
                    group.OnItemChanged(SchemeChangeTypes.ComponentChanged, group);
                }
                else
                {
                    foreach (object selObj in propertyGrid.SelectedObjects)
                    {
                        if (selObj is SchemeDocument document)
                            document.OnItemChanged(SchemeChangeTypes.SchemeDocChanged, selObj);
                        else if (selObj is BaseComponent component)
                            component.OnItemChanged(SchemeChangeTypes.ComponentChanged, selObj);
                    }
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
            propertyGrid.SelectedObject = new AppCode.CustomTypeDescriptor(customTypeDescriptor, newProperties);
        }
        private void propertyGrid_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            if(e.NewSelection == null || e.NewSelection.PropertyDescriptor == null)
            {
                return;
            }
            string selectedPropertyName = e.NewSelection.PropertyDescriptor.Name;
            BaseComponent selectedComponent = cbSchComp.SelectedItem as BaseComponent;
            if(selectedComponent == null)
            {
                toolStripButton1.Enabled = false;
                toolStripButton1.ToolTipText = "Select a component property to link an alias";
                return;
            }
            if (e.NewSelection.PropertyDescriptor.IsReadOnly && !selectedComponent.AliasesDictionnary.Keys.Contains(selectedPropertyName))
            {

                toolStripButton1.Enabled = false;
                toolStripButton1.ToolTipText = "This property cannot be modified";
            }
            else
            {
                // todo : set to false
                toolStripButton1.Enabled = true;
                toolStripButton1.ToolTipText = "Link to a symbol property";
            }
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            //todo: remove tis part from here
            Alias al = new Alias();
            al.Name = "alias Test";
            al.AliasTypeName = "string";
            al.Value = "Louis";
            al.isCnlLinked = false;

            Symbol s = new Symbol();
            s.Name = "okok";
            s.AliasList.Add(al);
            s.AliasCnlDictionary.Add(s.Name, -1);
            //to here

            BaseComponent selectedComponent = cbSchComp.SelectedItem as BaseComponent;
            TreeNode SymbolNode = findNode(treeView1.Nodes, n =>
                {
                    BaseComponent bc = (BaseComponent)n.Tag;
                    return (bc.ID == selectedComponent.GroupId && bc.GetType() == typeof(Symbol));
                }
            );
            /*todo: uncomment from here
            if(SymbolNode == null)
            {
                return;
            }
            to here*/
            
            //todo: uncomment first line and delete second one
            //Symbol parentSymbol = SymbolNode.Tag as Symbol;
            Symbol parentSymbol = s;

            GridItem selectedProperty = propertyGrid.SelectedGridItem;
            List<Alias> availableAliases = new List<Alias>();
            availableAliases = parentSymbol.AliasList.Where(a => a.AliasType == selectedProperty.PropertyDescriptor.PropertyType).ToList();
            string selectedPropertyName = selectedProperty.PropertyDescriptor.Name;
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
                //update mapping between compoennt properties and alias
                selectedComponent.AliasesDictionnary.Remove(selectedPropertyName);
                if(frmAliasSelection.selectedAlias != null)
                {
                    selectedComponent.AliasesDictionnary.Add(selectedPropertyName, frmAliasSelection.selectedAlias);
                }

                //Copy alias value in component parameter
                var oldProperty = selectedProperty.Value;
                if(frmAliasSelection.selectedAlias != null)
                {
                    componentProperty.SetValue(selectedComponent, frmAliasSelection.selectedAlias.Value, null);
                }

                propertyGrid.SelectedObject = selectedComponent;
                propertyGrid_PropertyValueChanged(propertyGrid, new PropertyValueChangedEventArgs(selectedProperty, oldProperty));

                return;
            }
        }

        private void miSaveSymbol_ButtonClick(object sender, EventArgs e)
        {
            SaveScheme(saveAs: false, asSymbol: true);
        }

        private void miSaveSymbolAs_Click(object sender, EventArgs e)
        {
            SaveScheme(saveAs: true, asSymbol: true);

        }

        private void newSymbolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // создание новой схемы
            if (ConfirmCloseScheme())
                InitScheme(isSymbol:true);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {  
            new FrmAlias(editor.SchemeView.MainSymbol).ShowDialog(); 
        }

        private string findSymboleInAvailableList(string name)
        {
            foreach(KeyValuePair<string, string> kvp in availableSymbols)
            {
                if (name == kvp.Key) return kvp.Value;
            }
            return "";
        }
    }
}
