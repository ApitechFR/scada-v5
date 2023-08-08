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
	console.log(props)
	
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
						divComp.text(cond.TextContent);
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
			console.log(props.Rotation)
			divComp.css({
				"transform": "rotate(" + props.Rotation + "deg)",
			})
		}

		this.setBackColor(divComp, backColor, true, statusColor);
		this.setBorderColor(divComp, borderColor, true, statusColor);

		// Advanced Conditions
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
						divComp.text(cond.TextContent);
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
/**************** Custom SVG *********************/
scada.scheme.CustomSVGRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.CustomSVGRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype,
);
scada.scheme.CustomSVGRenderer.constructor =
	scada.scheme.CustomSVGRenderer;



scada.scheme.CustomSVGRenderer.prototype.generateSVG = function (
	codeSVG,
	strokeColor,
	fillColor,
	strokeWidth,
	viewBoxX,
	viewBoxY,
	viewBoxWidth,
	viewBoxHeight,
	width,
	height,
) {
	if (typeof codeSVG !== 'string' || codeSVG.trim().length === 0) {
		console.error("Invalid SVG code");
		return;
	}

	var parser = new DOMParser();
	var doc = parser.parseFromString(codeSVG, "image/svg+xml");

	if (doc.querySelector('parsererror')) {
		console.error("Error parsing SVG");
		return;
	}

	var svg = doc.querySelector("svg");
	if (!svg) {
		console.error("No svg element found in the SVG code");
		return;
	}

	svg.setAttribute("xmlns", "http://www.w3.org/2000/svg");
	svg.setAttribute("viewBox", `${viewBoxX} ${viewBoxY} ${viewBoxWidth} ${viewBoxHeight}`);
	svg.setAttribute("width", width +"%");
	svg.setAttribute("height", height +"%");
	svg.setAttribute("style", "cursor:move");

	var elements = svg.querySelectorAll("*");
	elements.forEach(el => {
			el.setAttribute("stroke", strokeColor);
			el.setAttribute("fill", fillColor );
			el.setAttribute("stroke-width", strokeWidth );
		
	});

	const serializer = new XMLSerializer();
	codeSVG = serializer.serializeToString(svg);

	return codeSVG;
};

scada.scheme.CustomSVGRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;

	var divComp = $("<div id='comp" + component.id + "'></div>");
	this.prepareComponent(divComp, component,false,true);

	var svg = this.generateSVG(
		props.SvgCode,
		props.BorderColor,
		props.BackColor,
		props.BorderWidth,
		props.ViewBoxX,
		props.ViewBoxY,
		props.ViewBoxWidth,
		props.ViewBoxHeight,
		props.Width,
		props.Height);

	divComp.append(svg);
	if (props.Rotation && props.Rotation > 0) {
		divComp.css({
			"transform": "rotate(" + props.Rotation + "deg)",
		})
	}
	component.dom = divComp;
};

scada.scheme.CustomSVGRenderer.prototype.updateData = function (
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
				"transform": "rotate(" + props.Rotation +"deg)",
			})
		}
		this.setBackColor(divComp, backColor, true, statusColor);
		this.setBorderColor(divComp, borderColor, true, statusColor);

		divComp.removeClass("no-blink slow-blink fast-blink");
		if ( props.Conditions && cnlDataExt.Stat > 0) {
			var cnlVal = cnlDataExt.Val;
			for (var cond of props.Conditions) {
				if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
					divComp.css("background-color", cond.Color);
					switch (cond.Blinking) {
						case 0:
							divComp.addClass("no-blink");
							break;
						case 1:
							divComp.addClass("slow-blink");
							break;
						case 2:
							divComp.addClass("fast-blink");
							break;
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
	console.log(props);

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
				if (condition.Level === "Min") {
					divComp.find('.bar').css('height', "10%");
					divComp.find('.bar').css('background-color', condition.FillColor);
				}
				else if (condition.Level === "Low") {
					divComp.find('.bar').css('height', "30%");
					divComp.find('.bar').css('background-color', condition.FillColor);
				}
				else if (condition.Level === "Medium") {
					divComp.find('.bar').css('height', "50%");
					divComp.find('.bar').css('background-color', condition.FillColor);
				}
				else if (condition.Level === "High") {
					divComp.find('.bar').css('height', "70%");
					divComp.find('.bar').css('background-color', condition.FillColor);
				}
				else if (condition.Level === "Max" ) {
					divComp.find('.bar').css('background-color', condition.FillColor);
					divComp.find('.bar').css('height', "100%");
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
