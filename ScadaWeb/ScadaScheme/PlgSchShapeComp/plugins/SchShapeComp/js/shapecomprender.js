/*
 * SVG components rendering
 *
 * Author   : Messie MOUKIMOU
 * Created  : 2023
 * Modified : 
 *
 * Requires:
 * - jquery
 * - schemecommon.js
 * - schemerender.js
 */

/********** Static SVG Shape Renderer **********/


scada.scheme.addInfoTooltipToDiv = function (targetDiv, text) {

	if (targetDiv instanceof jQuery) {
		targetDiv = targetDiv[0];
	}

	if (!targetDiv) return;
	if (!targetDiv) return;

	// Create tooltip
	const tooltip = document.createElement('div');
	tooltip.style.position = 'absolute';
	tooltip.style.bottom = '45%';
	tooltip.style.left = '50%';
	tooltip.style.transform = 'translateX(-50%)';
	tooltip.style.padding = '10px';
	tooltip.style.backgroundColor = 'black';
	tooltip.style.color = 'white';
	tooltip.style.borderRadius = '5px';
	tooltip.style.zIndex = '10';
	tooltip.style.whiteSpace = 'nowrap';
	tooltip.style.marginBottom = '5px';

	tooltip.textContent = text;

	targetDiv.style.position = 'relative';
	targetDiv.appendChild(tooltip);
}


scada.scheme.handleBlinking = function (divComp, blinking) {
	divComp.removeClass("no-blink slow-blink fast-blink");
	switch (blinking) {
		case 0:
			break;
		case 1:
			divComp.addClass("slow-blink");
			break;
		case 2:
			divComp.addClass("fast-blink");
			break;
	}
}
scada.scheme.updateStyles = function (divComp, cond) {
	if (cond.Color) divComp.css("color", cond.Color);
	if (cond.BackgroundColor) divComp.css("background-color", cond.BackgroundColor);
	if (cond.TextContent) scada.scheme.addInfoTooltipToDiv(divComp[0], cond.TextContent);
	if (cond.Rotation) divComp.css("transform", "rotate(" + cond.Rotation + "deg)");
	if (cond.IsVisible !== undefined) divComp.css("visibility", cond.IsVisible ? "visible" : "hidden");
	if (cond.Width) divComp.css("width", cond.Width);
	if (cond.Height) divComp.css("height", cond.Height);
}

scada.scheme.applyRotation = function (divComp, props) {
	if (props.Rotation && props.Rotation > 0) {
		divComp.css({
			"transform": "rotate(" + props.Rotation + "deg)",
		})
	}
}
scada.scheme.updateColors = function (divComp, cnlDataExt, isHovered, props) {
	var statusColor = cnlDataExt.Color;

	var backColor = chooseColor(isHovered, props.BackColor, props.BackColorOnHover);
	var borderColor = chooseColor(isHovered, props.BorderColor, props.BorderColorOnHover);

	setBackColor(divComp, backColor, true, statusColor);
	setBorderColor(divComp, borderColor, true, statusColor);
}


/**************** Custom SVG *********************/
scada.scheme.CustomSVGRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.CustomSVGRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype,
);
scada.scheme.CustomSVGRenderer.constructor =
	scada.scheme.CustomSVGRenderer;

scada.scheme.CustomSVGRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;
	var divComp = $("<div id='comp" + component.id + "'></div>");
	this.prepareComponent(divComp, component, false, true);
	scada.scheme.applyRotation(divComp, props);

	if (props.SvgCode) {
		
		props.SvgCode = props.SvgCode.replace(/<svg[^>]*?(\s+width\s*=\s*["'][^"']*["'])/g, '<svg');
		props.SvgCode = props.SvgCode.replace(/<svg[^>]*?(\s+height\s*=\s*["'][^"']*["'])/g, '<svg');
	}
	divComp.append(props.SvgCode);
	component.dom = divComp;
};

scada.scheme.CustomSVGRenderer.prototype.updateData = function (
	component,
	renderContext,
) {
	var props = component.props;

	if (props.InCnlNum <= 0) {
		return;
	}

	var divComp = component.dom;
	var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);

	applyRotation(divComp, props);

	if (!props.Conditions || cnlDataExt.Stat <= 0) { return; }

	var cnlVal = cnlDataExt.Val;

	for (var cond of props.Conditions) {
        if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
            // Set CSS properties based on Condition
			scada.scheme.updateStyles(divComp, cond);
          
			scada.scheme.handleBlinking(divComp, cond.Blinking);
            break;
        }
    }
	
};











scada.scheme.SvgShapeRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.SvgShapeRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype
);

scada.scheme.SvgShapeRenderer.constructor =
	scada.scheme.SvgShapeRenderer;


scada.scheme.SvgShapeRenderer.prototype.createSvgElement = function (shapeType, props) {
	var svgElement;
	var svgNamespace = "http://www.w3.org/2000/svg";

	switch (shapeType) {
		case "Polygon":
			svgElement = document.createElementNS(svgNamespace, "polygon");
			svgElement.setAttribute("points", "0,0 50,0 50,50 0,50");
			break;
		case "Triangle":
			svgElement = document.createElementNS(svgNamespace, "polygon");
			svgElement.setAttribute("points", "0,0 50,50 100,0");
			break;
		case "Rectangle":
			svgElement = document.createElementNS(svgNamespace, "rect");
			svgElement.setAttribute("width", "100");
			svgElement.setAttribute("height", "100");
			break;
		case "Circle":
			svgElement = document.createElementNS(svgNamespace, "circle");
			svgElement.setAttribute("cx", "50");
			svgElement.setAttribute("cy", "50");
			svgElement.setAttribute("r", "50");
			break;
		case "Line":
			svgElement = document.createElementNS(svgNamespace, "line");
			svgElement.setAttribute("x1", "0");
			svgElement.setAttribute("y1", "0");
			svgElement.setAttribute("x2", "100");
			svgElement.setAttribute("y2", "100");
			break;
		case "Polyline":
			svgElement = document.createElementNS(svgNamespace, "polyline");
			svgElement.setAttribute("points", "20,20 40,25 60,40 80,120 120,140 200,180");
			break;
		default:
			console.warn("Unrecognized shape type: " + shapeType);
			return null;
	}

	if (["Polygon", "Triangle", "Rectangle", "Circle", "Polyline"].includes(shapeType)) {
		svgElement.setAttribute('fill', props.BackColor || 'none');
	}
	svgElement.setAttribute('stroke', props.BorderColor || 'black');
	svgElement.setAttribute('stroke-width', props.BorderWidth || '1');

	return svgElement;
};

scada.scheme.SvgShapeRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;
	var shapeType = props.ShapeType;
	
	var divComp = $("<div id='comp" + component.id + "'></div>");
	this.prepareComponent(divComp, component, false, true);
	

	var svgElement = this.createSvgElement(shapeType, props);

	var svgNamespace = "http://www.w3.org/2000/svg";
	var svgContainer = document.createElementNS(svgNamespace, "svg");
	svgContainer.appendChild(svgElement);
	svgContainer.setAttribute('viewBox', '0 0 ' + props.Width + ' ' + props.Height);
	svgContainer.style.width = "100%";
	svgContainer.style.height = "100%";

	var divSvgComp = $("<div class='svgcomp'> </div>")
	divSvgComp.append(svgContainer);
	divComp.css({ "overflow": "hidden" });
		
	divComp.append(divSvgComp);
	if (props.Rotation && props.Rotation > 0) {
		divComp.css({
			"transform": "rotate(" + props.Rotation + "deg)",
		})
	}

	component.dom = divComp;
};
scada.scheme.SvgShapeRenderer.prototype.updateData = function (
	component,
	renderContext,
) {
	var props = component.props;

	if (props.InCnlNum > 0) {
		var divComp = component.dom;
		var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);

		// choose and set colors of the component
		var statusColor = cnlDataExt.Color;
		var isHovered = divComp.is(":hover");

		var backColor = this.chooseColor(
			isHovered,
			props.BackColor,
			props.BackColorOnHover,
		);
		var borderColor = this.chooseColor(
			isHovered,
			props.BorderColor,
			props.BorderColorOnHover,
		);

		
		var svgElement = divComp.find("svg > *");
		svgElement.attr("fill", backColor);
		svgElement.attr("stroke", borderColor);

		divComp.find(".svgcomp").css({
			"width": props.Width +"px",
			"height": props.Height + "px",
		})
		if (props.Rotation && props.Rotation > 0) {
			divComp.css({
				"transform": "rotate(" + props.Rotation + "deg)",
			})
		}
		this.setBackColor(divComp, backColor, true, statusColor);
		this.setBorderColor(divComp, borderColor, true, statusColor);

		if (props.Conditions && cnlDataExt.Stat > 0) {
			var cnlVal = cnlDataExt.Val;

			for (var cond of props.Conditions) {
				if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
					// Set CSS properties based on Condition
					if (cond.Color) {
						divComp.css("color", cond.Color);
					}
					if (cond.BackgroundColor) {
						divComp.css("background-color", cond.BackgroundColor);
					}
					if (cond.TextContent) {
						
						scada.scheme.addInfoTooltipToDiv(divComp[0], cond.TextContent);
					}
					if (cond.Rotation) {
						divComp.css(
							"transform", "rotate(" + props.Rotation + "deg)",
						)
					}

					divComp.css("visibility", cond.IsVisible ? "visible" : "hidden");
					divComp.css("width", cond.Width);
					divComp.css("height", cond.Height);

					// Handle Blinking
					if (cond.Blinking == 1) {
						divComp.addClass("slow-blink");
					} else if (cond.Blinking == 2) {
						divComp.addClass("fast-blink");
					} else {
						divComp.removeClass("slow-blink fast-blink");
					}

					break;
				}
			}
		}
	}
};

/******* Polygon shape */

scada.scheme.PolygonRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.PolygonRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype,
);
scada.scheme.PolygonRenderer.constructor =
	scada.scheme.PolygonRenderer;

scada.scheme.PolygonRenderer.prototype.generatePolygonPath = function (
	numPoints,
) {
	
	// Check that numPoints is a valid value
	var validPoints = [3, 4, 5, 6, 8, 10];
	if (!validPoints.includes(numPoints)) {
		return ""; 
	}

	// Generate the points of the polygon
	var path = "";
	for (var i = 0; i < numPoints; i++) {
		var angle = (2 * Math.PI * i) / numPoints;
		var x = 50 + 50 * Math.cos(angle);
		var y = 50 + 50 * Math.sin(angle);
		path += x + "% " + y + "%, ";
	}

	// Remove trailing comma and space
	path = path.slice(0, -2);

	return "polygon(" + path + ")";
};

scada.scheme.PolygonRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;
	
	var divComp = $("<div id='comp" + component.id + "'></div>");
	this.prepareComponent(divComp, component);

	var polygonPath = this.generatePolygonPath(props.NumberOfSides);

	divComp.css({
		width: "200px",
		height: "200px",
		background: props.BackColor,
		"clip-path": this.generatePolygonPath(props.NumberOfSides),
		"border-width": props.BorderWidth + "px",
		"border-color": props.BorderColor,
	});
	if (props.Rotation && props.Rotation > 0) {
		divComp.css({
			"transform": "rotate(" + props.Rotation + "deg)",
		})
	}
	
	component.dom = divComp;
};

scada.scheme.PolygonRenderer.prototype.updateData = function (
	component,
	renderContext,
) {
	var props = component.props;
	
	if (props.InCnlNum > 0) {
		var divComp = component.dom;
		var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);

		// choose and set colors of the component
		var statusColor = cnlDataExt.Color;
		var isHovered = divComp.is(":hover");

		var backColor = this.chooseColor(
			isHovered,
			props.BackColor,
			props.BackColorOnHover,
		);
		var borderColor = this.chooseColor(
			isHovered,
			props.BorderColor,
			props.BorderColorOnHover,
		);

		if (props.Rotation && props.Rotation > 0) {
			divComp.css({
				"transform": "rotate(" + props.Rotation + "deg)",
			})
		}

		this.setBackColor(divComp, backColor, true, statusColor);
		this.setBorderColor(divComp, borderColor, true, statusColor);

		// Conditions
		if (props.Conditions && cnlDataExt.Stat > 0) {
			var cnlVal = cnlDataExt.Val;

			for (var cond of props.Conditions) {
				if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
					// Set CSS properties based on Condition
					if (cond.Color) {
						divComp.css("color", cond.Color);
					}
					if (cond.BackgroundColor) {
						divComp.css("background-color", cond.BackgroundColor);
					}
					if (cond.TextContent) {
						scada.scheme.addInfoTooltipToDiv(divComp[0], cond.TextContent);
					}
					if (cond.Rotation) {
						divComp.css(
							"transform", "rotate(" + props.Rotation + "deg)",
						)
					}
					divComp.css("visibility", cond.IsVisible ? "visible" : "hidden");
					divComp.css("width", cond.Width);
					divComp.css("height", cond.Height);

					// Handle Blinking
					if (cond.Blinking == 1) {
						divComp.addClass("slow-blink");
					} else if (cond.Blinking == 2) {
						divComp.addClass("fast-blink");
					} else {
						divComp.removeClass("slow-blink fast-blink");
					}

					break;
				}
			}
		}
	}
};




/** BarGraph */


scada.scheme.BarGraphRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.BarGraphRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype
);
scada.scheme.BarGraphRenderer.constructor = scada.scheme.BarGraphRenderer;

scada.scheme.BarGraphRenderer.prototype.createDom = function (component, renderContext) {
	var props = component.props;
	
	var divComp = $("<div id='comp" + component.id + "'></div>");

	var bar = $("<div class='bar' style='height:" + props.Value + "%" + ";background-color:" + props.BarColor + "' data-value='" + parseInt(props.Value) + "'></div>");
	
	divComp.append(bar);

	this.prepareComponent(divComp, component);

	divComp.css({
		"border": props.BorderWidth + "px solid " + props.BorderColor,
		"display": "flex",
		"align-items": "flex-end", 
		"justify-content": "center" 
	});

	component.dom = divComp;
};


scada.scheme.BarGraphRenderer.prototype.updateData = function (component, renderContext) {
	var props = component.props;
	if (props.InCnlNum > 0) {
		var divComp = component.dom;
		var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);
		
		divComp.css({
			"border": props.BorderWidth + "px solid " + props.BorderColor,
			"background-color": props.BackColor,
		})
		divComp.find('.bar').css({
			"background-color": props.BarColor,
			"height": props.Value + "%",
		});
		divComp.find('.bar').attr('data-value', parseInt(props.Value));

	}
	
	if (cnlDataExt.Stat > 0 && props.Conditions) {
		var cnlVal = cnlDataExt.Val;

		for (var condition of props.Conditions) {
			if (scada.scheme.calc.conditionSatisfied(condition, cnlVal)) {
				if (scada.scheme.calc.conditionSatisfied(condition, cnlVal)) {
					var barStyles = {}; 

					if (condition.Level === "Min") {
						barStyles.height = "10%";
					} else if (condition.Level === "Low") {
						barStyles.height = "30%";
					} else if (condition.Level === "Medium") {
						barStyles.height = "50%";
					} else if (condition.Level === "High") {
						barStyles.height = "70%";
					} else if (condition.Level === "Max") {
						barStyles.height = "100%";
					}
					if (condition.FillColor) {
						barStyles['background-color'] = condition.FillColor;
					}

					divComp.find('.bar').css(barStyles);

					if (condition.TextContent) {
						scada.scheme.addInfoTooltipToDiv(divComp[0], condition.TextContent);
					}
					// Set other CSS properties based on Condition
					if (condition.Color) {
						divComp.css("color", condition.Color);
					}
					if (condition.BackgroundColor) {
						divComp.css("background-color", condition.BackgroundColor);
					}
					if (condition.TextContent) {
						divComp.text(condition.TextContent);
					}
					divComp.css("visibility", condition.IsVisible ? "visible" : "hidden");
					if (condition.Width) {
						divComp.css("width", condition.Width);
					}
					if (condition.Height) {
						divComp.css("height", condition.Height);
					}

					// Handle Blinking
					if (condition.Blinking == 1) {
						divComp.addClass("slow-blink");
					} else if (condition.Blinking == 2) {
						divComp.addClass("fast-blink");
					} else {
						divComp.removeClass("slow-blink fast-blink");
					}
				}
			}
		}
	}

};



/********** Renderer Map **********/

// Add components to the renderer map
scada.scheme.rendererMap.set("Scada.Web.Plugins.SchShapeComp.SvgShape", new scada.scheme.SvgShapeRenderer);
scada.scheme.rendererMap.set("Scada.Web.Plugins.SchShapeComp.CustomSVG", new scada.scheme.CustomSVGRenderer);
scada.scheme.rendererMap.set("Scada.Web.Plugins.SchShapeComp.Polygon", new scada.scheme.PolygonRenderer);
scada.scheme.rendererMap.set("Scada.Web.Plugins.SchShapeComp.BarGraph", new scada.scheme.BarGraphRenderer);
