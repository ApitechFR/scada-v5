﻿using Scada.Scheme;
using Scada.Web.Plugins.SchShapeComp;
using System.IO;
using Scada.Scheme.Model.PropertyGrid;
using Scada.Web.Plugins.SchShapeComp.PropertyGrid;

namespace Scada.Web.Plugins
{
	public class PlgSchShapeCompSpec : PluginSpec, ISchemeComp
	{
		internal const string PlgVersion = "1.0.5.0";


		public override string Version =>  PlgVersion; 

		
		public override string Name => "Shape Scheme Components";
			
		public override string Descr => "A set of Shape components for display on schemes.";

		CompLibSpec ISchemeComp.CompLibSpec => new ShapeCompLibSpec(); 
		

		public override void Init()
		{
            if (SchemeContext.GetInstance().EditorMode)
            {
                if (!Localization.LoadDictionaries(Path.Combine(AppDirs.PluginsDir, "SchShapeComp","lang"),"PlgSvgComponent", out string errMsg))
                {
					Log.WriteError(errMsg);
                    
                }
				AttrTranslator attrTranslator = new AttrTranslator();
				attrTranslator.TranslateAttrs(typeof(ColorCondition));
				attrTranslator.TranslateAttrs(typeof(PolygonCondition));
				attrTranslator.TranslateAttrs(typeof(AdvancedCondition));
				attrTranslator.TranslateAttrs(typeof(PopupSize));


            }
        }
	}
}