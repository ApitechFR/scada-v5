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

using Newtonsoft.Json.Linq;
using Scada.Client;
using Scada.Data.Tables;
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
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
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

        public Dictionary<string,bool> UpdatedSymbolId = new Dictionary<string, bool>();

        private Point locFirstComponent = new Point();


        List<Tuple<string, Point>> SymbolsComponentsLocations = new List<Tuple<string, Point>>();

        public bool updated = false;

        public string symbolPathUpToDate;

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

                //List of symbols in XML file
                XmlNodeList symbolNodes = xmlDoc.SelectNodes("/SchemeView/Symbols/Symbol");
                //list that will contain the symbols instances
                List<XmlNode> symbolInstancesNodes = new List<XmlNode>();
                //List of components that belongs to a symbol instance and their location
                List<ObjetcComponentLocation> listComponentAndLocationOfScheme = FindSymbolsInstancesComponents(rootElem);

                //for each symbol in the XML file
                foreach (XmlNode symbolNode in symbolNodes)
                {
                    //get the pattern of the symbol
                    List<string>  symbolPattern = GetSymbolPattern(symbolNode);

                    int processedSymbolInstancesCount = 0;
                    int symbolComponentsCount = symbolPattern.Count();

                    //We find the locations of the instances of the symbol
                    List<Point> currentSymbolInstancesComponentsLocations = FindSymbolInstancesComponentsLocations(listComponentAndLocationOfScheme, symbolNode);
                    List<XmlNode> currentSymbolInstancesComponentsNodes = FindSymbolInstancesComponentsNodes(listComponentAndLocationOfScheme, symbolNode);

                    //if there are some instances left to process
                    while (currentSymbolInstancesComponentsLocations.Count>0)
                    {
                        //future location of the instance of the symbol
                        Point location;
                        //we clone the symbol node
                        XmlNode clonedSymbol = symbolNode.CloneNode(deep: true);

                        //we update the componentID of the cloned symbol
                        XmlNode idNode = clonedSymbol.SelectSingleNode("ID");
                        if (idNode != null)
                        {
                            idNode.InnerText = $"{findMaxID(rootElem) + processedSymbolInstancesCount}";
                        }

                        //we calculate the minimum point of the components of the first instance of the symbol
                        location = GetMinimumPoint(currentSymbolInstancesComponentsLocations, symbolComponentsCount);

                        //we update the location of the cloned symbol with the calculated minimum point
                        XmlNode locationNode = clonedSymbol.SelectSingleNode("Location");
                        if (locationNode != null)
                        {
                            XmlNode XNode = locationNode.SelectSingleNode("X");
                            XmlNode YNode = locationNode.SelectSingleNode("Y");
                            if(XNode != null && YNode != null)
                            {
                                XNode.InnerText = location.X.ToString();
                                YNode.InnerText = location.Y.ToString();
                            }
                        }

                        for (int i = 0; i < symbolComponentsCount; i++)
                        {
                            if (i == 0)
                            {
                                XmlElement childNode = xmlDoc.CreateElement("symbolInstanceRef");
                                childNode.InnerText = currentSymbolInstancesComponentsNodes[i].SelectSingleNode("RefInstanceSym").InnerText;
                                clonedSymbol.AppendChild(childNode);
                            }
                            //XmlNode clonedSymbolComponentAliasOfcomponentsNode = clonedSymbol.SelectSingleNode("Components").ChildNodes[i].SelectSingleNode("AliasListOfComponent");
                            XmlNode currentSymbolComponentAliasOfComponentsNode = currentSymbolInstancesComponentsNodes[i].SelectSingleNode("AliasListOfComponent");

                            if (currentSymbolComponentAliasOfComponentsNode != null)
                            {
                                foreach (XmlNode aliasNode in currentSymbolComponentAliasOfComponentsNode.ChildNodes)
                                {
                                    string attributeName = aliasNode.SelectSingleNode("AttributeName").InnerText;
                                    XmlNode clonedSymbolAttributeNode = clonedSymbol.SelectSingleNode("Components").ChildNodes[i].SelectSingleNode(attributeName);
                                    XmlNode currentSymbolAttributeNode = currentSymbolInstancesComponentsNodes[i].SelectSingleNode(attributeName);
                                    if (clonedSymbolAttributeNode != null && currentSymbolAttributeNode != null)
                                    {
                                        clonedSymbolAttributeNode.InnerText = currentSymbolAttributeNode.InnerText;
                                    }
                                }
                            }
                        }


                        //we update the components of the cloned symbol with the currentSymbolInstancesComponents
                        //XmlNode componentsNodeOfClonedSymbol = clonedSymbol.SelectSingleNode("Components");
                        //componentsNodeOfClonedSymbol.RemoveAll();
                        //for(int i=0;i<symbolComponentsCount;i++)
                        //{
                        //    componentsNodeOfClonedSymbol.AppendChild(currentSymbolInstancesComponentsNodes[i]);
                        //}

                        //create child node for the cloned symbol




                        //we add the cloned symbol to the components node
                        componentsNode.PrependChild(clonedSymbol);

                        //Add the cloned symbol to the list of symbol instances
                        symbolInstancesNodes.Add(clonedSymbol);

                        if(!(symbolComponentsCount <= 0) || !(symbolComponentsCount > listComponentAndLocationOfScheme.Count()))
                        {
                            //we remove the components of the first instance of the symbol from the list
                            currentSymbolInstancesComponentsLocations.RemoveRange(0, symbolComponentsCount);
                            currentSymbolInstancesComponentsNodes.RemoveRange(0, symbolComponentsCount);
                        }
                        processedSymbolInstancesCount++;
                    }
                }

                foreach (XmlNode compNode in componentsNode.ChildNodes)
                {
                    XmlNode linkNode = compNode.SelectSingleNode("LinkedSymbolID");
                    if (linkNode != null) continue;

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

                    if(MainSymbol != null)
                    {
                        for (int i = 0; i < component.AliasesDictionnary.Count; i++)
                        {
                            foreach (Alias alias in MainSymbol.AliasList)
                            {
                                if (alias.Name == component.AliasesDictionnary.Values.ToList()[i].Name)
                                {
                                    component.AliasesDictionnary[component.AliasesDictionnary.Keys.ToList()[i]] = alias;
                                }
                            }
                        }
                    }
                    if (!Components.ContainsKey(component.ID))
                        Components[component.ID] = component;
                    else 
                    { 
                        component.ID = maxComponentID + 1;
                        Components[component.ID] = component;
                    }
                    if (component.ID > maxComponentID)
                        maxComponentID = component.ID;
                    


                    if (component is Symbol symbol)
                    {
                        string instanceRef = compNode.SelectSingleNode("symbolInstanceRef")?.InnerText;
                        LoadSymbol(Symbolpath, rootElem, symbol, instanceRef);
                        component.Location = new Point(component.Location.X, component.Location.Y);
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
                foreach (XmlNode n in symbolInstancesNodes)
                {
                    if(n.ParentNode != null)
                        n.ParentNode.RemoveChild(n);
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

        private Point GetMinimumPoint(List<Point> currentSymbolInstancesComponentsLocations, int symbolComponentsCount)
        {
            if (currentSymbolInstancesComponentsLocations.Count < symbolComponentsCount || symbolComponentsCount <= 0)
            {
                return new Point(0, 0);
            }

            int minX = currentSymbolInstancesComponentsLocations.Take(symbolComponentsCount).Min(p => p.X);
            int minY = currentSymbolInstancesComponentsLocations.Take(symbolComponentsCount).Min(p => p.Y);

            return new Point(minX, minY);
        }

        /// <summary>
        /// Returns a list of ObjetcComponentLocation ({component, linkedsymbolId, symbolInstanceReference, Location}) 
        /// for every component that is part of a symbol instance
        /// </summary>
        /// <param name="root"></param>
        /// <returns></returns>
        private List<ObjetcComponentLocation> FindSymbolsInstancesComponents(XmlElement root)
        {
            List<ObjetcComponentLocation> list = new List<ObjetcComponentLocation>();

            XmlNode componentsNode = root.SelectSingleNode("//Components");

            if(componentsNode != null)
            {
                //for each component of "components" section in XML file
                foreach(XmlNode componentNode in componentsNode){
                    //if the component is not a symbol
                    if (componentNode is XmlElement && componentNode.Name != "Symbol")
                    {
                        //we get the location, the linked symbol ID and the symbol instance reference
                        XmlElement element = (XmlElement)componentNode;
                        XmlNode locationNode = element.SelectSingleNode("Location");
                        XmlNode linkedSymbIdNode = element.SelectSingleNode("LinkedSymbolID");
                        XmlNode refInstanceSymNode = element.SelectSingleNode("RefInstanceSym");

                        if (locationNode != null && linkedSymbIdNode != null && refInstanceSymNode!=null)
                        {
                            int x = int.Parse(locationNode.SelectSingleNode("X").InnerText);
                            int y = int.Parse(locationNode.SelectSingleNode("Y").InnerText);

                            list.Add(new ObjetcComponentLocation(element.Name, new Point(x, y), linkedSymbIdNode.InnerText, int.Parse(refInstanceSymNode.InnerText), element));

                        }
                    }
                }
            }

            //list of {component, linkedsymbolId, symbolInstanceReference, Location} for every component that is part of a symbol instance
            return list;
        }

        /// <summary>
        /// Returns the list of components names the provided symbol is made of
        /// </summary>
        /// <param name="symbolNode"></param>
        /// <returns></returns>
        private List<string> GetSymbolPattern(XmlNode symbolNode)
        {
            //list of components names the provided symbol is made of
            List<string> listComponents = new List<string>();

            //Parent node of every components nodes in the symbol node provided
            XmlNode componentsNode = symbolNode.SelectSingleNode("./Components");
            string symbolId = symbolNode.SelectSingleNode("SymbolId").InnerText;

            if (componentsNode != null)
            {
                //List of every component nodes in the symbol node provided
                XmlNodeList componentNodes = componentsNode.ChildNodes;
                //For each component node in the symbol node provided
                foreach (XmlNode componentNode in componentNodes)
                {
                    if (componentNode is XmlElement)
                    {
                        //We add the component name to the list
                        listComponents.Add(componentNode.Name);
                        XmlNode componentLocationNode = componentNode.SelectSingleNode("./Location");
                        Point p = new Point();
                        p.X = int.Parse(componentLocationNode.SelectSingleNode("X").InnerText);
                        p.Y = int.Parse(componentLocationNode.SelectSingleNode("Y").InnerText);

                        //We add the component name and its location to the list
                        SymbolsComponentsLocations.Add(new Tuple<string, Point>(symbolId, p));
                    }
                }
            }
            return listComponents;
        }

        private int findMaxID(XmlElement root)
        {
            int maxID = 0;

            XmlNodeList idNodes = root.SelectNodes("//ID");

            foreach (XmlNode node in idNodes)
            {
                int currentID;
                if (int.TryParse(node.InnerText, out currentID))
                {
                    if (currentID > maxID)
                    {
                        maxID = currentID;
                    }
                }
            }

            return maxID;
        }

        //private int CountPatternOccurences(List<ObjetcComponentLocation> source, XmlNode symbNode) 
        //{
        //    int count = 0;

        //    string id = symbNode.SelectSingleNode("SymbolId").InnerText;

        //    int result = source
        //        .Where(obj => obj.SymbID == id)
        //        .GroupBy(obj => new { obj.SymbID, obj.RefInstance})
        //        .Select(group => new
        //        {
        //            SymbID = group.Key.SymbID,
        //            Ref = group.Key.RefInstance
        //        })
        //        .Count();

        //    return result;
        //}

        /// <summary>
        /// Returns the location of the components that fit with the pattern
        /// </summary>
        /// <param name="source"></param>
        /// <param name="symbNode"></param>
        private List<Point> FindSymbolInstancesComponentsLocations(List<ObjetcComponentLocation> source, XmlNode symbNode)
        {
            string symbolId = symbNode.SelectSingleNode("SymbolId").InnerText;
            return source
                .Where(x => x.SymbID == symbolId)
                .Select(x => x.Position)
                .ToList();
        }
        /// <summary>
        /// Returns the nodes of the components that fit with the pattern
        /// </summary>
        /// <param name="source"></param>
        /// <param name="symbNode"></param>
        private List<XmlNode> FindSymbolInstancesComponentsNodes(List<ObjetcComponentLocation> source, XmlNode symbNode)
        {
            string symbolId = symbNode.SelectSingleNode("SymbolId").InnerText;
            return source
                .Where(x => x.SymbID == symbolId)
                .Select(x => x.Node)
                .ToList();
        }

        /// <summary>
        /// Adds a symbol into the list of components. It loads the data of the symbol either from the symbol file or from the current scheme file
        /// </summary>
        /// <param name="symbolPath"></param>
        /// <param name="rootElem"></param>
        /// <param name="symbol"></param>
        private void LoadSymbol(string symbolPath, XmlElement rootElem, Symbol symbol, string symbolInstanceRef="")
        {
            //path of the symbol file
            string symbolIndexPath = symbolPath + "\\index.xml";

            //check if the symbol is up to date, comparing last modification dates from the current scheme file and the symbol file
            if (!IsSymbolUpToDate(symbol, symbolIndexPath))
            {
                //if the symbol is not up to date, we find the symbol node from the symbol file
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(symbolIndexPath);
                XmlNode indexEntry = xmlDoc.SelectSingleNode($"/root/symbols/symbol[@symbolId='{symbol.SymbolId}']");

                //we get the path of the symbol file
                if (indexEntry.Attributes["path"].Value == symbolPathUpToDate) 
                {
                    symbol.LastModificationDate = DateTime.Parse(indexEntry.Attributes["lastModificationDate"].Value);
                    LoadFromSymbolFile(indexEntry.Attributes["path"].Value, symbol);
                    UpdatedSymbolId[symbol.SymbolId] = true;
                    updated = true;
                }
                else if (!UpdatedSymbolId.ContainsKey(symbol.SymbolId))
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
                        symbol.LastModificationDate = DateTime.Parse(indexEntry.Attributes["lastModificationDate"].Value);
                        LoadFromSymbolFile(indexEntry.Attributes["path"].Value, symbol);
                        UpdatedSymbolId.Add(symbol.SymbolId, true);
                        updated = true;
                    }
                    else
                    {
                        LoadFromCurrentFile(rootElem, symbol, symbolInstanceRef);
                        UpdatedSymbolId.Add(symbol.SymbolId, false);
                    }
                }
                else
                {
                    if(UpdatedSymbolId.ContainsKey(symbol.SymbolId) && UpdatedSymbolId[symbol.SymbolId])
                    {

                        symbol.LastModificationDate = DateTime.Parse(indexEntry.Attributes["lastModificationDate"].Value);
                        LoadFromSymbolFile(indexEntry.Attributes["path"].Value, symbol);
                    }
                    else if(UpdatedSymbolId.ContainsKey(symbol.SymbolId) && !UpdatedSymbolId[symbol.SymbolId])
                    {
                        LoadFromCurrentFile(rootElem, symbol, symbolInstanceRef);
                    }
                }
            }
            else
            {
                LoadFromCurrentFile(rootElem, symbol, symbolInstanceRef);
            }

        }

        private bool IsSymbolUpToDate(Symbol symbol,string symbolIndexPath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(symbolIndexPath);

            XmlNode indexEntry = xmlDoc.SelectSingleNode($"/root/symbols/symbol[@symbolId='{symbol.SymbolId}']");

            if (indexEntry == null) return true;
            DateTime fileDate = DateTime.Parse( indexEntry.Attributes["lastModificationDate"].Value);
            return fileDate <= symbol.LastModificationDate;
        }

        public void LoadFromSymbolFile(string symbolXMLDocumentPath, Symbol symbol)
        {
            XmlDocument symbolXMLDocument = new XmlDocument();
            symbolXMLDocument.Load(symbolXMLDocumentPath);
            XmlElement SymbolXMLRoot = symbolXMLDocument.DocumentElement;
            int inCnlOffset = templateArgs.InCnlOffset;
            int ctrlCnlOffset = templateArgs.CtrlCnlOffset;
            CompManager compManager = CompManager.GetInstance();

            // load symbol main component
            if (SymbolXMLRoot.SelectSingleNode("MainSymbol") is XmlNode mainSymbolNode)
            {
                Point location = new Point(symbol.Location.X,symbol.Location.Y);
                int Id = symbol.ID;

                Symbol newSymbol = compManager.CreateComponent(mainSymbolNode, out string errMsg) as Symbol;
                if (symbol == null)
                {
                    LoadErrors.Add(errMsg);
                }
                newSymbol.LoadFromXml(mainSymbolNode);

                newSymbol.ID=Id;
                newSymbol.Location = location;
                newSymbol.AliasList = symbol.updateAliasList(newSymbol);

                symbol = newSymbol;
            }

            // load components of the symbol
            XmlNode componentsNode = SymbolXMLRoot.SelectSingleNode("Components");
            if(componentsNode == null)
            {
                throw new Exception("An error occured while loading the symbol components. The components node is missing in the symbol file.");
            }

            
            HashSet<string> errNodeNames = new HashSet<string>(); // names of the components that were not loaded
            LoadErrors.AddRange(compManager.LoadErrors);
            SortedDictionary<int, ComponentBinding> componentBindings = templateBindings?.ComponentBindings;

            Point minXYInSymbol = new Point();
            int indice = 0;
            List<Tuple<string, Point>> tuplesFilter = SymbolsComponentsLocations.Where(t => t.Item1 == symbol.SymbolId).ToList();
            List<BaseComponent> componentsOfSymbolInstance = new List<BaseComponent>();


            //Create the components of the symbol
            for (int i = 0; i < componentsNode.ChildNodes.Count; i++)
            {

                XmlNode node = componentsNode.ChildNodes[i];
                BaseComponent component = compManager.CreateComponent(node, out string errMsg);

                if (component == null)
                {
                    component = new UnknownComponent { XmlNode = node };
                    if (errNodeNames.Add(node.Name))
                        LoadErrors.Add(errMsg);
                }

                // Load the component and add it to the view
                component.SchemeView = this;
                component.LoadFromXml(node);
                componentsOfSymbolInstance.Add(component);

                if (i == 0)
                {
                    minXYInSymbol = component.Location;
                }
                //find the minimum point of the components of the symbol
                if (component.Location.X < minXYInSymbol.X)
                {
                    minXYInSymbol.X = component.Location.X;
                }
                if (component.Location.Y < minXYInSymbol.Y)
                {
                    minXYInSymbol.Y = component.Location.Y;
                }
            }

            foreach (BaseComponent component in componentsOfSymbolInstance)
            {
                //Adjust the location of the components of the symbol
                component.Location = new Point(symbol.Location.X+component.Location.X-minXYInSymbol.X,symbol.Location.Y+component.Location.Y-minXYInSymbol.Y);

                //Remove obsolete aliases if has some
                component.updateAliasesDictionary(symbol);

                //Update the aliases of the component
                foreach (var aliasesDictionaryEntry in component.AliasesDictionnary)
                {
                    var componentProperty = component.GetType().GetProperty(aliasesDictionaryEntry.Key);
                    if (componentProperty == null)
                    {
                        continue;
                    }
                    
                    var aliasValue = symbol.AliasList.Where(x => x.Name == aliasesDictionaryEntry.Value.Name).First().Value;
                    var isInCnlLinked = aliasesDictionaryEntry.Key == "InCnlNumCustom";
                    var isCtrlCnlLinked = aliasesDictionaryEntry.Key == "CtrlCnlNumCustom";
                    var isCnlLinked = isInCnlLinked || isCtrlCnlLinked;
 
                    if ( aliasValue.GetType().Name.Equals("Int32") && isCnlLinked)
                    {
                        aliasValue = aliasValue.ToString();
                    }
                    componentProperty.SetValue(component, aliasValue, null);
                    if (isCnlLinked && component is IDynamicComponent dynamicComp)
                    {
                        var componentChannelPropertyName = aliasesDictionaryEntry.Key.Substring(0, aliasesDictionaryEntry.Key.Length - 6);
                        var componentChannelProperty = component.GetType().GetProperty(componentChannelPropertyName);
                        var ChannelNumber = symbol.AliasCnlDictionary[aliasesDictionaryEntry.Value.Name];
                        componentChannelProperty.SetValue(component, ChannelNumber, null);

                        if(isCtrlCnlLinked)
                        {
                            dynamicComp.CtrlCnlNum = FindNumberCnlNumCustom((string)aliasValue);
                        }
                        if (isInCnlLinked)
                        {
                            dynamicComp.InCnlNum = FindNumberCnlNumCustom((string)aliasValue);
                        }
                    }
                }


                if (component is Symbol sym)
                {
                    LoadSymbol(symbolXMLDocumentPath,SymbolXMLRoot, sym);
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


            // load groups
            if (SymbolXMLRoot.SelectSingleNode("Groups") is XmlNode groupsNode)
            {
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
                    componentsOfSymbolInstance.Add(group);
                    AddInCnlNums(group.GetInCnlNums(), inCnlOffset);
                    AddCtrlCnlNums(group.GetCtrlCnlNums(), ctrlCnlOffset);

                }
            }

            // load scheme images
            if (SymbolXMLRoot.SelectSingleNode("Images") is XmlNode imagesNode)
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
            SetNewSymbolCompsIDs(componentsOfSymbolInstance,symbol);
        }

        /// <summary>
        /// find the canal number stored between parenthesis in the alias value
        /// </summary>
        /// <param name="NumCustom">string including an interger between parenthesis</param>
        /// <returns>the number between parenthesis</returns>
        public static int FindNumberCnlNumCustom(string NumCustom)
        {
            int number = 0;

            // Définir l'expression régulière pour extraire les chiffres entre parenthèses
            string pattern = @"\((\d+)\)";

            // Rechercher une correspondance
            Match match = Regex.Match(NumCustom, pattern);

            if (match.Success)
            {
                // Extraire la valeur numérique et la convertir en entier
                number = int.Parse(match.Groups[1].Value);
            }
            return number;
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

        private void LoadFromCurrentFile2(XmlElement rootelem,Symbol symbol)
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
                            //if (component.Location.X + component.Location.Y <= 20) component.Location = new Point(0, 0);

                            Point location = new Point(component.Location.X + symbol.Location.X, component.Location.Y + symbol.Location.Y);
                            component.Location = location;

         //                   foreach (Alias a in symbol.AliasList){
         //                       //TODO: ne pas remplacer les valeurs déjà existantes
         //                       var dictionnaryEntriesToAdd = 
         //                           symbol.AliasList.Where(alias=>component.AliasesDictionnary.ContainsKey(alias.Name)).ToDictionary(alias => alias.Name, alias => alias.Value);
                                
         //                       var dictonnaryEntriesToDelete = component.AliasesDictionnary.Where(entry=>symbol.AliasList.Any(alias=>alias.Name == entry.Key)).ToDictionary(entry => entry.Key, entry => entry.Value);
         //                           component.AliasesDictionnary.Where(entry => entry.Value.Name == a.Name).ToList();

         //                       foreach (var entry in dictionnaryEntriesToAdd)
         //                       {
         //                           var componentProperty = component.GetType().GetProperty(entry.Key);
         //                           if (componentProperty == null)
         //                           {
         //                               continue;
         //                           }

         //                           if(a.Value.GetType().Name.Equals("Int32") && a.isCnlLinked)
         //                           {

         //                               componentProperty.SetValue(component, a.Value.ToString(), null);
         //                           }
         //                           else
         //                           {
         //                               componentProperty.SetValue(component, a.Value, null);
         //                           }

									//if (entry.Key == "InCnlNumCustom" || entry.Key == "CtrlCnlNumCustom")
         //                           {
         //                               var componentChannelPropertyName = entry.Key.Substring(0, entry.Key.Length - 6);
         //                               var componentChannelProperty = component.GetType().GetProperty(componentChannelPropertyName);
         //                               var ChannelNumber = symbol.AliasCnlDictionary[a.Name];
         //                               componentChannelProperty.SetValue(component, ChannelNumber, null);
         //                           }
         //                       }
         //                   }

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

        private void LoadFromCurrentFile(XmlElement rootelem, Symbol symbol, string instanceRef)
        {
            
            List<BaseComponent> components = new List<BaseComponent>();
            XmlNode symbolNodesContainer = rootelem.SelectSingleNode("Symbols");

            if(symbolNodesContainer == null)
            {
                throw new Exception("An error occured while loading the symbol components. The symbols node is missing in the current scheme file.");
            }

            //find the template of the symbol in "symbols" node in XML
            XmlNode symbolNode = null;
            foreach (XmlNode node in symbolNodesContainer.ChildNodes)
            {
                if (node.GetChildAsString("SymbolId") == symbol.SymbolId)
                {
                    symbolNode = node;
                    break;
                }
            }
            if (symbolNode == null)
            {
                throw new Exception("No symbol found for symbolId" + symbol.SymbolId);
            }

            // get channel offsets in template mode
            int inCnlOffset = templateArgs.InCnlOffset;
            int ctrlCnlOffset = templateArgs.CtrlCnlOffset;

            HashSet<string> errNodeNames = new HashSet<string>(); // имена узлов незагруженных компонентов
            CompManager compManager = CompManager.GetInstance();
            LoadErrors.AddRange(compManager.LoadErrors);
            SortedDictionary<int, ComponentBinding> componentBindings = templateBindings?.ComponentBindings;

            //we get instances components nodes
            List<XmlNode> instanceComponentsNodes = new List<XmlNode>();
            foreach (XmlNode componentNode in rootelem.SelectSingleNode("Components").ChildNodes)
            {
                if(componentNode.SelectSingleNode("RefInstanceSym") == null || componentNode.SelectSingleNode("RefInstanceSym").InnerText != instanceRef.ToString())
                {
                    continue;
                }

                BaseComponent component = compManager.CreateComponent(componentNode, out string errMsg);

                if (component == null)
                {
                    component = new UnknownComponent { XmlNode = componentNode };
                    if (errNodeNames.Add(componentNode.Name))
                        LoadErrors.Add(errMsg);
                }

                // загрузка компонента и добавление его в представление
                component.SchemeView = this;
                component.LoadFromXml(componentNode);
                //if (component.Location.X + component.Location.Y <= 20) component.Location = new Point(0, 0);

                Point location = new Point(component.Location.X + symbol.Location.X, component.Location.Y + symbol.Location.Y);
                component.Location = location;



                components.Add(component);
                //if (component is Symbol sym)
                //{
                //    LoadSymbol(Symbolpath, symbolNode as XmlElement, sym);
                //}

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

            //if (symbolNode.SelectSingleNode("Components") is XmlNode componentsNode)
            //{
            //    HashSet<string> errNodeNames = new HashSet<string>(); // имена узлов незагруженных компонентов
            //    CompManager compManager = CompManager.GetInstance();
            //    LoadErrors.AddRange(compManager.LoadErrors);
            //    SortedDictionary<int, ComponentBinding> componentBindings = templateBindings?.ComponentBindings;

            //    foreach (XmlNode componentNode in componentsNode.ChildNodes)
            //    {
            //        BaseComponent component = compManager.CreateComponent(componentNode, out string errMsg);

            //        if (component == null)
            //        {
            //            component = new UnknownComponent { XmlNode = componentNode };
            //            if (errNodeNames.Add(componentNode.Name))
            //                LoadErrors.Add(errMsg);
            //        }

            //        // загрузка компонента и добавление его в представление
            //        component.SchemeView = this;
            //        component.LoadFromXml(componentNode);
            //        //if (component.Location.X + component.Location.Y <= 20) component.Location = new Point(0, 0);

            //        Point location = new Point(component.Location.X + symbol.Location.X, component.Location.Y + symbol.Location.Y);
            //        component.Location = location;



            //        components.Add(component);
            //        //if (component is Symbol sym)
            //        //{
            //        //    LoadSymbol(Symbolpath, symbolNode as XmlElement, sym);
            //        //}

            //        // добавление входных каналов представления
            //        if (component is IDynamicComponent dynamicComponent)
            //        {
            //            if (componentBindings != null &&
            //                componentBindings.TryGetValue(component.ID, out ComponentBinding binding))
            //            {
            //                dynamicComponent.InCnlNum = binding.InCnlNum;
            //                dynamicComponent.CtrlCnlNum = binding.CtrlCnlNum;
            //            }
            //            else
            //            {
            //                if (inCnlOffset > 0 && dynamicComponent.InCnlNum > 0)
            //                    dynamicComponent.InCnlNum += inCnlOffset;
            //                if (ctrlCnlOffset > 0 && dynamicComponent.CtrlCnlNum > 0)
            //                    dynamicComponent.CtrlCnlNum += ctrlCnlOffset;
            //            }

            //            AddCnlNum(dynamicComponent.InCnlNum);
            //            AddCtrlCnlNum(dynamicComponent.CtrlCnlNum);
            //        }

            //        AddInCnlNums(component.GetInCnlNums(), inCnlOffset);
            //        AddCtrlCnlNums(component.GetCtrlCnlNums(), ctrlCnlOffset);

            //    }
            //}


            // load groups
            if (symbolNode.SelectSingleNode("Groups") is XmlNode groupsNode)
            {
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
            SetNewSymbolCompsIDs(components, symbol);
        }

        /// <summary>
        /// Загрузить схему из файла.
        /// </summary>
        public bool LoadFromFile(string fileName,string symbolPath, out string errMsg, string symbolUpdatedPath = "")
        {
            Symbolpath = symbolPath;
            try
            {
                using (FileStream fileStream =
                    new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    symbolPathUpToDate = symbolUpdatedPath;
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
        public bool SaveToFile(string fileName ,out string errMsg)
        {
            try
            {
                if(MainSymbol != null)
                {
                    MainSymbol.LastModificationDate = DateTime.Now;
                }
                XmlDocument xmlDoc = new XmlDocument();
                XmlDeclaration xmlDecl = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", null);
                xmlDoc.AppendChild(xmlDecl);

                // запись заголовка представления
                XmlElement rootElem = xmlDoc.CreateElement("SchemeView");
                if (isSymbol)
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

                string symbolID = "";

                List<string> symbolsList = new List<string>();

                foreach (BaseComponent component in Components.Values)
                {
                    //change
                    if ((getHihghestGroup(component) is Symbol sym && sym.ID != component.ID) && (isSymbol ) && component.ID == MainSymbol.ID)
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

                        XmlElement componentElem = componentElem = compLibSpec == null ?
                                xmlDoc.CreateElement(compType.Name) /*стандартный компонент*/ :
                                xmlDoc.CreateElement(compLibSpec.XmlPrefix, compType.Name, compLibSpec.XmlNs);

                        if ((isSymbol ) && component.ID == MainSymbol.ID)
                        {
                            componentElem = xmlDoc.CreateElement("MainSymbol");
                        }

                        component.SaveToXml(componentElem);

                        if(((getHihghestGroup(component) is Symbol symb && symb.ID != component.ID)) && !string.IsNullOrEmpty(symbolID)){
                            componentElem.AppendElem("LinkedSymbolID", symb.SymbolId);
                            componentElem.AppendElem("RefInstanceSym", component.GroupId);
                        }

                        if (component is ComponentGroup)
                        {
                            if (component is Symbol symbol)
                            {
                                symbolID = symbol.SymbolId;

                                if ((isSymbol ) && component.ID == MainSymbol.ID)
                                    componentsElem.AppendChild(componentElem);

                                if ((isSymbol ) && symbol.ID == MainSymbol.ID)
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
                                        saveSymbolTemplateToXml(symbolTemplate, symbol, compManager);
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

            if(MainSymbol == null)
            {
                while (group.GroupId != -1)
                {
                    group = getHihghestGroup(group);
                }
            }
            else
            {
                while (group.GroupId != -1 && group.GroupId != MainSymbol.ID)
                {
                    group = getHihghestGroup(group);
                }
            }
            
            return group;
        }


        /// <summary>
        /// Returns every BaseComponents in the group
        /// </summary>
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

    class ObjetcComponentLocation
    {
        public string Name { get; set; }
        public Point Position { get; set; }
        public string SymbID { get; set; }
        public int RefInstance { get; set; }
        public XmlNode Node { get; set; }


        public ObjetcComponentLocation(string name, Point pos, string symID, int refIns, XmlNode node)
        {
            Name = name;
            Position = pos;
            SymbID = symID;
            RefInstance = refIns;
            Node = node;
        }
    }
}
