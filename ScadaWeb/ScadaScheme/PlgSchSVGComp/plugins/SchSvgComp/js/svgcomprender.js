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


scada.scheme.SvgShapeRenderer.prototype.createSvgElement = function (shapeType, svgCode, props) {
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
		case "Custom ...":
			var parser = new DOMParser();
			svgElement = parser.parseFromString(svgCode, "image/svg+xml").documentElement;
			break;
		default:
			console.warn("Unrecognized shape type: " + shapeType);
			return null;
	}

	// Set SVG attributes for color and stroke width
	svgElement.setAttribute('fill', props.BackColor);
	svgElement.setAttribute('stroke', props.BorderColor); props.BorderColor
	svgElement.setAttribute('stroke-width', props.BorderWidth);

	return svgElement;
};

scada.scheme.SvgShapeRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;
	var shapeType = props.ShapeType;
	var svgCode = props.SvgCode;

	var divComp = $("<div id='comp" + component.id + "'></div>");
	//this.prepareComponent(divComp, component);
	this.prepareComponent(divComp, component, false, true);

	var svgElement = this.createSvgElement(shapeType, svgCode, props);

	var svgNamespace = "http://www.w3.org/2000/svg";
	var svgContainer = document.createElementNS(svgNamespace, "svg");
	svgContainer.appendChild(svgElement);
	svgContainer.style.width = "100%";
	svgContainer.style.height = "100%";

	divComp.append(svgContainer);
	component.dom = divComp;
};
/********** Renderer Map **********/

// Add components to the renderer map
scada.scheme.rendererMap.set("Scada.Web.Plugins.SchSVGComp.SvgShape", new scada.scheme.SvgShapeRenderer);
