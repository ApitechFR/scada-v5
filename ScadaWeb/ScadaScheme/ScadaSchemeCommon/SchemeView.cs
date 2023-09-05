/*
 * Copyright 2020 Mikhail Shiryaev
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
 * Module   : ScadaSchemeCommon
 * Summary  : Scheme view
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2019
 * Modified : 2020
 */

using Scada.Client;
using Scada.Scheme.Model;
using Scada.Scheme.Model.DataTypes;
using Scada.Scheme.Template;
using Scada.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;
using Utils;

namespace Scada.Scheme
{
    /// <summary>
    /// Scheme view.
    /// <para>Представление схемы.</para>
    /// </summary>
    public class SchemeView : BaseView
    {
        /// <summary>
        /// The maximum ID of the scheme components.
        /// </summary>
        protected int maxComponentID;
        /// <summary>
        /// The scheme arguments in template mode.
        /// </summary>
        protected TemplateArgs templateArgs;
        /// <summary>
        /// The scheme template bindings.
        /// </summary>
        protected TemplateBindings templateBindings;

        private string Symbolpath;
        /// <summary>
        /// Конструктор.
        /// </summary>
        public SchemeView()
            : base()
        {
            maxComponentID = 0;
            templateArgs = new TemplateArgs();
            templateBindings = null;

            SchemeDoc = new SchemeDocument() { SchemeView = this };
            Components = new SortedList<int, BaseComponent>();
            LoadErrors = new List<string>();
        }


        /// <summary>
        /// Получить свойства документа схемы.
        /// </summary>
        public SchemeDocument SchemeDoc { get; protected set; }

        /// <summary>
        /// Получить компоненты схемы, ключ - идентификатор компонента.
        /// </summary>
        public SortedList<int, BaseComponent> Components { get; protected set; }

        /// <summary>
        /// Получить ошибки при загрузке схемы.
        /// </summary>
        /// <remarks>Необходимо для контроля загрузки библиотек и компонентов.</remarks>
        public List<string> LoadErrors { get; protected set; }

        /// <summary>
        /// Get or set whether the current scheme is a symbol or a regular scheme
        /// </summary>
        public bool isSymbol { get; set; } = false;

        public Symbol MainSymbol { get; set; }

        /// <summary>
        /// Adds the input channels to the view.
        /// </summary>
        private void AddInCnlNums(List<int> inCnlNums, int offset)
        {
            if (inCnlNums != null)
            {
                foreach (int cnlNum in inCnlNums)
                {
                    if (cnlNum > 0)
                        AddCnlNum(cnlNum + offset);
                }
            }
        }

        /// <summary>
        /// Adds the ouput channels to the view.
        /// </summary>
        private void AddCtrlCnlNums(List<int> ctrlCnlNums, int offset)
        {
            if (ctrlCnlNums != null)
            {
                foreach (int ctrlCnlNum in ctrlCnlNums)
                {
                    if (ctrlCnlNum > 0)
                        AddCtrlCnlNum(ctrlCnlNum + offset);
                }
            }
        }

        /// <summary>
        /// Sets the view arguments.
        /// </summary>
        public override void SetArgs(string args)
        {
            base.SetArgs(args);
            templateArgs.Init(Args);
        }

        /// <summary>
        /// Updates the view title.
        /// </summary>
        public override void UpdateTitle(string s)
        {
            if (string.IsNullOrEmpty(Title))
            {
                Title = s ?? "";
                SchemeDoc.Title = Title;

                // display title
                int titleCompID = templateBindings == null ? templateArgs.TitleCompID : templateBindings.TitleCompID;
                if (titleCompID > 0 &&
                    Components.TryGetValue(titleCompID, out BaseComponent titleComponent) &&
                    titleComponent is StaticText staticText)
                {
                    staticText.Text = Title;
                }
            }
        }

        /// <summary>
        /// Загрузить представление из потока.
        /// </summary>
        public override void LoadFromStream(Stream stream)
        {
            // clear the view
            Clear();

            // load component bindings
            if (string.IsNullOrEmpty(templateArgs.BindingFileName))
            {
                templateBindings = null;
            }
            else
            {
                templateBindings = new TemplateBindings();
                templateBindings.Load(System.IO.Path.Combine(
                    SchemeContext.GetInstance().AppDirs.ConfigDir, templateArgs.BindingFileName));
            }

            // load XML document
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(stream);

            // check data format
            XmlElement rootElem = xmlDoc.DocumentElement;
            if (!rootElem.Name.Equals("SchemeView", StringComparison.OrdinalIgnoreCase) &&
                !rootElem.Name.Equals("SchemeSymbol", StringComparison.OrdinalIgnoreCase))
                throw new ScadaException(SchemePhrases.IncorrectFileFormat);

            if (rootElem.Name.Equals("SchemeSymbol", StringComparison.OrdinalIgnoreCase))
                isSymbol = true;

            // get channel offsets in template mode
            int inCnlOffset = templateArgs.InCnlOffset;
            int ctrlCnlOffset = templateArgs.CtrlCnlOffset;

            // load scheme document
            if (rootElem.SelectSingleNode("Scheme") is XmlNode schemeNode)
            {
                SchemeDoc.LoadFromXml(schemeNode);
                // установка заголовка представления
                Title = SchemeDoc.Title;
                // добавление входных каналов представления
                AddInCnlNums(SchemeDoc.CnlFilter, inCnlOffset);
            }

            if (isSymbol)
            {
                if (rootElem.SelectSingleNode("MainSymbol") is XmlNode mainSymbolNode)
                {
                    CompManager compManager = CompManager.GetInstance();
                    MainSymbol = compManager.CreateComponent(mainSymbolNode, out string errMsg) as Symbol;
                    if (MainSymbol == null)
                    {
                        LoadErrors.Add(errMsg);
                    }

                    MainSymbol.SchemeView = this;
                    MainSymbol.LoadFromXml(mainSymbolNode);
                    Components[MainSymbol.ID] = MainSymbol;

                    AddInCnlNums(MainSymbol.GetInCnlNums(), inCnlOffset);
                    AddCtrlCnlNums(MainSymbol.GetCtrlCnlNums(), ctrlCnlOffset);

                    // определение макс. идентификатора компонентов
                    if (MainSymbol.ID > maxComponentID)
                        maxComponentID = MainSymbol.ID;
                }
            }

            // load scheme components
            if (rootElem.SelectSingleNode("Components") is XmlNode componentsNode)
            {
                HashSet<string> errNodeNames = new HashSet<string>(); // имена узлов незагруженных компонентов
                CompManager compManager = CompManager.GetInstance();
                LoadErrors.AddRange(compManager.LoadErrors);
                SortedDictionary<int, ComponentBinding> componentBindings = templateBindings?.ComponentBindings;

                foreach (XmlNode compNode in componentsNode.ChildNodes)
                {
                    // создание компонента
                    BaseComponent component = compManager.CreateComponent(compNode, out string errMsg);

                    if (component == null)
                    {
                        component = new UnknownComponent { XmlNode = compNode };
                        if (errNodeNames.Add(compNode.Name))
                            LoadErrors.Add(errMsg);
                    }

                    // загрузка компонента и добавление его в представление
                    component.SchemeView = this;
                    component.LoadFromXml(compNode);
                    Components[component.ID] = component;
                    if (component.ID > maxComponentID)
                        maxComponentID = component.ID;
                    Components[component.ID] = component;


                    if (component is Symbol symbol)
                    {
                        LoadSymbol(Symbolpath, rootElem, symbol);
                    }

                    // добавление входных каналов представления
                    if (component is IDynamicComponent dynamicComponent)
                    {
                        if (componentBindings != null &&
                            componentBindings.TryGetValue(component.ID, out ComponentBinding binding))
                        {
                            dynamicComponent.InCnlNum = binding.InCnlNum;
                            dynamicComponent.CtrlCnlNum = binding.CtrlCnlNum;
                        }
                        else
                        {
                            if (inCnlOffset > 0 && dynamicComponent.InCnlNum > 0)
                                dynamicComponent.InCnlNum += inCnlOffset;
                            if (ctrlCnlOffset > 0 && dynamicComponent.CtrlCnlNum > 0)
                                dynamicComponent.CtrlCnlNum += ctrlCnlOffset;
                        }

                        AddCnlNum(dynamicComponent.InCnlNum);
                        AddCtrlCnlNum(dynamicComponent.CtrlCnlNum);
                    }

                    AddInCnlNums(component.GetInCnlNums(), inCnlOffset);
                    AddCtrlCnlNums(component.GetCtrlCnlNums(), ctrlCnlOffset);

                    // определение макс. идентификатора компонентов

                }
            }


            // load groups
            if (rootElem.SelectSingleNode("Groups") is XmlNode groupsNode)
            {
                HashSet<string> errNodeNames = new HashSet<string>();
                CompManager compManager = CompManager.GetInstance();
                LoadErrors.AddRange(compManager.LoadErrors);

                foreach (XmlNode grpNode in groupsNode.ChildNodes)
                {
                    BaseComponent group = compManager.CreateComponent(grpNode, out string errMsg);
                    if (group == null)
                    {
                        group = new UnknownComponent { XmlNode = grpNode };
                        if (errNodeNames.Add(grpNode.Name))
                            LoadErrors.Add(errMsg);
                    }

                    group.SchemeView = this;
                    group.LoadFromXml(grpNode);
                    Components[group.ID] = group;
                    AddInCnlNums(group.GetInCnlNums(), inCnlOffset);
                    AddCtrlCnlNums(group.GetCtrlCnlNums(), ctrlCnlOffset);

                    if (group.ID > maxComponentID) maxComponentID = group.ID;
                }
            }

            // load scheme images
            if (rootElem.SelectSingleNode("Images") is XmlNode imagesNode)
            {
                Dictionary<string, Image> images = SchemeDoc.Images;
                XmlNodeList imageNodes = imagesNode.SelectNodes("Image");
                foreach (XmlNode imageNode in imageNodes)
                {
                    Image image = new Image();
                    image.LoadFromXml(imageNode);
                    if (!string.IsNullOrEmpty(image.Name))
                        images[image.Name] = image;
                }
            }
        }
        
        private void LoadSymbol(string symbolPath, XmlElement rootElem, Symbol symbol)
        {
            string symbolIndexPath = symbolPath + "\\index.xml";

            if (!IsSymbolUpToDate(symbol, symbolIndexPath))
            {
                DialogResult popup = MessageBox.Show
                    (
                    $"There is a more recent version of the following symbol: \n" +
                    $"'{symbol.Name}'\n" +
                    $" Would you like to update it?",
                    "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning
                    );

                if (popup == DialogResult.Yes)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(symbolIndexPath);
                    XmlNode indexEntry = xmlDoc.SelectSingleNode($"//symbol[@symbolId='{symbol.SymbolId}']");

                    symbol.LastModificationDate = DateTime.Parse(indexEntry.Attributes["lastModificationDate"].Value);

                    LoadFromSymbolFile(indexEntry.Attributes["path"].Value, symbol);
                    
                }
                else
                {
                    LoadFromCurrentFile(rootElem, symbol);
                }
            }
            else
            {
                LoadFromCurrentFile(rootElem, symbol);
            }

        }

        private bool IsSymbolUpToDate(Symbol symbol,string symbolIndexPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(symbolIndexPath);

            XmlNode indexEntry = xmlDoc.SelectSingleNode($"//symbol[@symbolId='{symbol.SymbolId}']");

            if (indexEntry == null) return true;
            DateTime fileDate = DateTime.Parse( indexEntry.Attributes["lastModificationDate"].Value);
            bool res = fileDate <= symbol.LastModificationDate;
            return res;
        }

        private void LoadFromSymbolFile(string symbolPath, Symbol symbol)
        {
            List<BaseComponent> symbolComps = new List<BaseComponent>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(symbolPath);
            XmlElement rootElem = xmlDoc.DocumentElement;

            int inCnlOffset = templateArgs.InCnlOffset;
            int ctrlCnlOffset = templateArgs.CtrlCnlOffset;

            // load scheme components
            if (rootElem.SelectSingleNode("Components") is XmlNode componentsNode)
            {
                HashSet<string> errNodeNames = new HashSet<string>(); // имена узлов незагруженных компонентов
                CompManager compManager = CompManager.GetInstance();
                LoadErrors.AddRange(compManager.LoadErrors);
                SortedDictionary<int, ComponentBinding> componentBindings = templateBindings?.ComponentBindings;

                foreach (XmlNode compNode in componentsNode.ChildNodes)
                {
                    // создание компонента
                    BaseComponent component = compManager.CreateComponent(compNode, out string errMsg);

                    if (component == null)
                    {
                        component = new UnknownComponent { XmlNode = compNode };
                        if (errNodeNames.Add(compNode.Name))
                            LoadErrors.Add(errMsg);
                    }

                    // загрузка компонента и добавление его в представление
                    component.SchemeView = this;
                    component.LoadFromXml(compNode);
                    Point location = new Point(component.Location.X + symbol.Location.X, component.Location.Y + symbol.Location.Y);
                    component.Location = location;


                    symbolComps.Add(component);

                    if(component is Symbol sym)
                    {
                        LoadSymbol(symbolPath,rootElem, sym);
                    }

                    // добавление входных каналов представления
                    if (component is IDynamicComponent dynamicComponent)
                    {
                        if (componentBindings != null &&
                            componentBindings.TryGetValue(component.ID, out ComponentBinding binding))
                        {
                            dynamicComponent.InCnlNum = binding.InCnlNum;
                            dynamicComponent.CtrlCnlNum = binding.CtrlCnlNum;
                        }
                        else
                        {
                            if (inCnlOffset > 0 && dynamicComponent.InCnlNum > 0)
                                dynamicComponent.InCnlNum += inCnlOffset;
                            if (ctrlCnlOffset > 0 && dynamicComponent.CtrlCnlNum > 0)
                                dynamicComponent.CtrlCnlNum += ctrlCnlOffset;
                        }

                        AddCnlNum(dynamicComponent.InCnlNum);
                        AddCtrlCnlNum(dynamicComponent.CtrlCnlNum);
                    }

                    AddInCnlNums(component.GetInCnlNums(), inCnlOffset);
                    AddCtrlCnlNums(component.GetCtrlCnlNums(), ctrlCnlOffset);

                    // определение макс. идентификатора компонентов
                
                }
            }


            // load groups
            if (rootElem.SelectSingleNode("Groups") is XmlNode groupsNode)
            {
                HashSet<string> errNodeNames = new HashSet<string>();
                CompManager compManager = CompManager.GetInstance();
                LoadErrors.AddRange(compManager.LoadErrors);

                foreach (XmlNode grpNode in groupsNode.ChildNodes)
                {
                    BaseComponent group = compManager.CreateComponent(grpNode, out string errMsg);
                    if (group == null)
                    {
                        group = new UnknownComponent { XmlNode = grpNode };
                        if (errNodeNames.Add(grpNode.Name))
                            LoadErrors.Add(errMsg);
                    }

                    group.SchemeView = this;
                    group.LoadFromXml(grpNode);
                    Point location = new Point(group.Location.X + symbol.Location.X, group.Location.Y + symbol.Location.Y);
                    group.Location = location;
                    symbolComps.Add(group);
                    AddInCnlNums(group.GetInCnlNums(), inCnlOffset);
                    AddCtrlCnlNums(group.GetCtrlCnlNums(), ctrlCnlOffset);

                }
            }

            // load scheme images
            if (rootElem.SelectSingleNode("Images") is XmlNode imagesNode)
            {
                Dictionary<string, Image> images = SchemeDoc.Images;
                XmlNodeList imageNodes = imagesNode.SelectNodes("Image");
                foreach (XmlNode imageNode in imageNodes)
                {
                    Image image = new Image();
                    image.LoadFromXml(imageNode);
                    if (!string.IsNullOrEmpty(image.Name))
                        images[image.Name] = image;
                }
            }
            SetNewSymbolCompsIDs(symbolComps,symbol);
        }

        private void SetNewSymbolCompsIDs(List<BaseComponent> components,Symbol symbol)
        {
            List<int> ID = components.Select(x => x.ID).ToList();
            foreach(BaseComponent comp in components.Where(x=> !ID.Contains(x.GroupId)))
            {
                comp.GroupId = symbol.ID;
            }
            foreach (BaseComponent group in components.Where(x=>x is ComponentGroup)) 
            {
                int oldId = group.ID;
                group.ID = GetNextComponentID();
                foreach(BaseComponent comp in components.Where(x=>x.GroupId== oldId)) 
                {
                    if (comp == null) break;

                    comp.GroupId = group.ID;
                }
                Components[group.ID] = group;
                group.OnItemChanged(SchemeChangeTypes.ComponentAdded, group);
            }
            foreach (BaseComponent component in components)
            {
                if (component is ComponentGroup) continue;
                component.ID = GetNextComponentID();
                Components[component.ID] = component;
            }
        }

        private void LoadFromCurrentFile(XmlElement rootelem,Symbol symbol)
        {
            List<BaseComponent> components = new List<BaseComponent>();
            if (rootelem.SelectSingleNode("Symbols") is XmlNode symbolNodes)
            {
                XmlNode symbolNode = null;
                foreach (XmlNode node in symbolNodes.ChildNodes)
                {
                    if (node.GetChildAsString("SymbolId") == symbol.SymbolId)
                    {
                        symbolNode = node;
                        break;
                    }
                }
                if (symbolNode != null)
                {
                    // get channel offsets in template mode
                    int inCnlOffset = templateArgs.InCnlOffset;
                    int ctrlCnlOffset = templateArgs.CtrlCnlOffset;

                    if (symbolNode.SelectSingleNode("Components") is XmlNode componentsNode)
                    {
                        HashSet<string> errNodeNames = new HashSet<string>(); // имена узлов незагруженных компонентов
                        CompManager compManager = CompManager.GetInstance();
                        LoadErrors.AddRange(compManager.LoadErrors);
                        SortedDictionary<int, ComponentBinding> componentBindings = templateBindings?.ComponentBindings;

                        foreach (XmlNode compNode in componentsNode.ChildNodes)
                        {
                            // создание компонента
                            BaseComponent component = compManager.CreateComponent(compNode, out string errMsg);

                            if (component == null)
                            {
                                component = new UnknownComponent { XmlNode = compNode };
                                if (errNodeNames.Add(compNode.Name))
                                    LoadErrors.Add(errMsg);
                            }

                            // загрузка компонента и добавление его в представление
                            component.SchemeView = this;
                            component.LoadFromXml(compNode);
                            Point location = new Point(component.Location.X + symbol.Location.X, component.Location.Y + symbol.Location.Y);
                            component.Location = location;

                            components.Add(component);
                            if (component is Symbol sym)
                            {
                                LoadSymbol(Symbolpath, symbolNode as XmlElement, sym);
                            }

                            // добавление входных каналов представления
                            if (component is IDynamicComponent dynamicComponent)
                            {
                                if (componentBindings != null &&
                                    componentBindings.TryGetValue(component.ID, out ComponentBinding binding))
                                {
                                    dynamicComponent.InCnlNum = binding.InCnlNum;
                                    dynamicComponent.CtrlCnlNum = binding.CtrlCnlNum;
                                }
                                else
                                {
                                    if (inCnlOffset > 0 && dynamicComponent.InCnlNum > 0)
                                        dynamicComponent.InCnlNum += inCnlOffset;
                                    if (ctrlCnlOffset > 0 && dynamicComponent.CtrlCnlNum > 0)
                                        dynamicComponent.CtrlCnlNum += ctrlCnlOffset;
                                }

                                AddCnlNum(dynamicComponent.InCnlNum);
                                AddCtrlCnlNum(dynamicComponent.CtrlCnlNum);
                            }

                            AddInCnlNums(component.GetInCnlNums(), inCnlOffset);
                            AddCtrlCnlNums(component.GetCtrlCnlNums(), ctrlCnlOffset);

                        }
                    }


                    // load groups
                    if (symbolNode.SelectSingleNode("Groups") is XmlNode groupsNode)
                    {
                        HashSet<string> errNodeNames = new HashSet<string>();
                        CompManager compManager = CompManager.GetInstance();
                        LoadErrors.AddRange(compManager.LoadErrors);

                        foreach (XmlNode grpNode in groupsNode.ChildNodes)
                        {
                            BaseComponent group = compManager.CreateComponent(grpNode, out string errMsg);
                            if (group == null)
                            {
                                group = new UnknownComponent { XmlNode = grpNode };
                                if (errNodeNames.Add(grpNode.Name))
                                    LoadErrors.Add(errMsg);
                            }

                            group.SchemeView = this;
                            group.LoadFromXml(grpNode);
                            components.Add(group);

                            AddInCnlNums(group.GetInCnlNums(), inCnlOffset);
                            AddCtrlCnlNums(group.GetCtrlCnlNums(), ctrlCnlOffset);

                        }
                    }
                }
                SetNewSymbolCompsIDs(components, symbol);
            }
        }

        /// <summary>
        /// Загрузить схему из файла.
        /// </summary>
        public bool LoadFromFile(string fileName,string symbolPath, out string errMsg)
        {
            Symbolpath = symbolPath;
            try
            {
                using (FileStream fileStream =
                    new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    LoadFromStream(fileStream);
                }

                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = SchemePhrases.LoadSchemeViewError + ": " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Сохранить схему в файле.
        /// </summary>
        public bool SaveToFile(string fileName ,out string errMsg, bool asSymbol = false)
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDecl);

                // запись заголовка представления
                XmlElement rootElem = xmlDoc.CreateElement("SchemeView");
                if (asSymbol||isSymbol)
                    rootElem = xmlDoc.CreateElement("SchemeSymbol");

                rootElem.SetAttribute("title", SchemeDoc.Title);
                xmlDoc.AppendChild(rootElem);

                // запись документа схемы
                XmlElement documentElem = xmlDoc.CreateElement("Scheme");
                rootElem.AppendChild(documentElem);
                SchemeDoc.SaveToXml(documentElem);

                // запись компонентов схемы
                CompManager compManager = CompManager.GetInstance();
                HashSet<string> prefixes = new HashSet<string>();
                XmlElement componentsElem = xmlDoc.CreateElement("Components");
                XmlElement groupsElem = xmlDoc.CreateElement("Groups");
                XmlElement symbols = xmlDoc.CreateElement("Symbols");
                rootElem.AppendChild(componentsElem);
                rootElem.AppendChild(groupsElem);
                rootElem.AppendChild(symbols);

                List<string> symbolsList = new List<string>();

                foreach (BaseComponent component in Components.Values)
                {
                    if (getHihghestGroup(component) is Symbol sym && sym.ID != component.ID) 
                    { continue; }
                    if (component is UnknownComponent)
                    {
                        componentsElem.AppendChild(((UnknownComponent)component).XmlNode);
                    }
                    else
                    {
                        Type compType = component.GetType();
                        CompLibSpec compLibSpec = compManager.GetSpecByType(compType);

                        // добавление пространства имён
                        if (compLibSpec != null && !prefixes.Contains(compLibSpec.XmlPrefix))
                        {
                            rootElem.SetAttribute("xmlns:" + compLibSpec.XmlPrefix, compLibSpec.XmlNs);
                            prefixes.Add(compLibSpec.XmlPrefix);
                        }

                        // создание XML-элемента компонента
                        XmlElement componentElem = compLibSpec == null ?
                            xmlDoc.CreateElement(compType.Name) /*стандартный компонент*/ :
                            xmlDoc.CreateElement(compLibSpec.XmlPrefix, compType.Name, compLibSpec.XmlNs);
                        if ((isSymbol || asSymbol) && component.ID == MainSymbol.ID)
                        {
                            componentElem = xmlDoc.CreateElement("MainSymbol");
                        }

                        component.SaveToXml(componentElem);


                        if (component is ComponentGroup) 
                        { 
                            if(component is Symbol symbol)
                            {
                                componentsElem.AppendChild(componentElem);

                                if ((isSymbol || asSymbol) && symbol.ID == MainSymbol.ID)
                                {   
                                    groupsElem.AppendChild(componentElem);
                                    rootElem.AppendChild(componentElem);
                                }
                                else
                                {
                                    if (!symbolsList.Contains(symbol.SymbolId))
                                    {
                                        XmlElement symbolTemplate = componentElem.Clone() as XmlElement;

                                        symbols.AppendChild(symbolTemplate);
                                        saveSymbolTemplateToXml(symbolTemplate, symbol,compManager);
                                        symbolsList.Add(symbol.SymbolId);
                                    }
                                }
                            }
                            else groupsElem.AppendChild(componentElem); 
                        }
                        else componentsElem.AppendChild(componentElem);

                    }
                }

                // запись изображений схемы
                XmlElement imagesElem = xmlDoc.CreateElement("Images");
                rootElem.AppendChild(imagesElem);

                foreach (Image image in SchemeDoc.Images.Values)
                {
                    XmlElement imageElem = xmlDoc.CreateElement("Image");
                    imagesElem.AppendChild(imageElem);
                    image.SaveToXml(imageElem);
                }

                xmlDoc.Save(fileName);
                errMsg = "";
                return true;
            }
            catch (Exception ex)
            {
                errMsg = SchemePhrases.SaveSchemeViewError + ": " + ex.Message;
                return false;
            }
        }

        public void saveSymbolTemplateToXml(XmlElement node,Symbol symbol, CompManager compManager)
        {
            XmlElement componentsElem = node.OwnerDocument.CreateElement("Components");
            XmlElement groupsElem = node.OwnerDocument.CreateElement("Groups");

            node.AppendChild(componentsElem);
            node.AppendChild(groupsElem);

            foreach (BaseComponent comp in getGroupedComponents(symbol.ID))
            {
                BaseComponent component = comp.Clone();
                Point location = new Point(component.Location.X - symbol.Location.X, 
                                           component.Location.Y - symbol.Location.Y);
                component.Location = location;

                if (component is UnknownComponent)
                {
                    componentsElem.AppendChild(((UnknownComponent)component).XmlNode);
                }
                else
                {
                    Type compType = component.GetType();
                    CompLibSpec compLibSpec = compManager.GetSpecByType(compType);



                    XmlElement componentElem = compLibSpec == null ?
                        node.OwnerDocument.CreateElement(compType.Name) /*стандартный компонент*/ :
                        node.OwnerDocument.CreateElement(compLibSpec.XmlPrefix, compType.Name, compLibSpec.XmlNs);

                    if (component is ComponentGroup)
                    {
                        if (component is Symbol)
                        {
                            componentsElem.AppendChild(componentElem);
                        }
                        else groupsElem.AppendChild(componentElem);
                    }
                    else componentsElem.AppendChild(componentElem);

                    component.SaveToXml(componentElem);
                }
            }
        }


        /// <summary>
        /// Returns every BaseComponents in the group
        /// </summary>


        public BaseComponent getHihghestGroup(BaseComponent comp)
        {
            int groupID = comp.GroupId;
            if(isSymbol && groupID == MainSymbol.ID)
            {
                return comp;
            }
            if (groupID == -1)
            {
                return comp;
            }

            BaseComponent group = Components.Values.Where(x => x.ID == groupID).FirstOrDefault();
            if (group == null) return comp;
            while (group.GroupId != -1)
            {
                group = getHihghestGroup(group);
            }
            return group;
        }


        public List<BaseComponent> getGroupedComponents(int groupID)
        {
            List<BaseComponent> groupedComponents = new List<BaseComponent>();
            if (groupID == -1) return groupedComponents;
            foreach (BaseComponent component in Components.Values)
            {
                if (component.GroupId == groupID) { groupedComponents.Add(component); }
            }

            foreach (BaseComponent componentGroup in Components.Values.Where(x => x.GroupId == groupID).DefaultIfEmpty().ToList())
            {
                if (componentGroup == null) break;
                groupedComponents.AddRange(getGroupedComponents(componentGroup.ID));
            }

            return groupedComponents;

        }
        /// <summary>
        /// Очистить представление.
        /// </summary>
        public override void Clear()
        {
            base.Clear();
            maxComponentID = 0;
            SchemeDoc.SetToDefault();
            SchemeDoc.SchemeView = this;
            Components.Clear();
            LoadErrors.Clear();
        }

        /// <summary>
        /// Получить следующий идентификатор компонента схемы.
        /// </summary>
        public int GetNextComponentID()
        {
            return ++maxComponentID;
        }
    }
}
