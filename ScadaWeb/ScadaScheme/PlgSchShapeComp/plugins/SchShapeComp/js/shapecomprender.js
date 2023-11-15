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
	const tooltip = document.createElement("div");
	tooltip.style.position = "absolute";
	tooltip.style.bottom = "45%";
	tooltip.style.left = "50%";
	tooltip.style.transform = "translateX(-50%)";
	tooltip.style.padding = "10px";
	tooltip.style.backgroundColor = "black";
	tooltip.style.color = "white";
	tooltip.style.borderRadius = "5px";
	tooltip.style.zIndex = "10";
	tooltip.style.whiteSpace = "nowrap";
	tooltip.style.marginBottom = "5px";

	tooltip.textContent = text;

	targetDiv.style.position = "relative";
	targetDiv.appendChild(tooltip);
};

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
};
scada.scheme.updateStyles = function (divComp, cond) {
	if (cond.Color) divComp.css("color", cond.Color);
	if (cond.BackgroundColor)
		divComp.css("background-color", cond.BackgroundColor);
	if (cond.TextContent)
		scada.scheme.addInfoTooltipToDiv(divComp[0], cond.TextContent);
	if (cond.Rotation)
		divComp.css("transform", "rotate(" + cond.Rotation + "deg)");
	if (cond.IsVisible !== undefined)
		divComp.css("visibility", cond.IsVisible ? "visible" : "hidden");
	if (cond.Width) divComp.css("width", cond.Width);
	if (cond.Height) divComp.css("height", cond.Height);
};

scada.scheme.setRotate = function (divComp, props) {

	setTimeout(function () {
		var compWrapper = divComp.closest('.comp-wrapper');
		if (compWrapper.length > 0) {
			var rotationTransform = 'rotate(' + props.Rotation + 'deg)';

			console.log(rotationTransform);

			if (props.Rotation === 0) {
				compWrapper.css('transform', '');
				console.log("Transform removed from comp-wrapper of " + divComp.attr('id'));
			} else {
				compWrapper.css('transform', rotationTransform);
				console.log("Rotation updated for comp-wrapper of " + divComp.attr('id'));
			}
		} else {
			console.error("comp-wrapper not found for " + divComp.attr('id'));
		}
	}, 0);
}

scada.scheme.applyRotation = function (divCompId, props) {
	console.log("Running applyRotation");
	console.log(props.Rotation);
	var compDiv = $('#comp' + divCompId);
	console.log(compDiv);
	$(document).ready(function () {
		console.log("Document ready!!!");
		
		
			var parentDiv = compDiv.closest('.comp-wrapper');
			console.log(parentDiv);
			if (parentDiv.length > 0) {
				var existingTransform = parentDiv.css('transform');
				var rotationTransform = 'rotate(' + props.rotation + 'deg)';

				if (existingTransform && existingTransform !== 'none') {
					parentDiv.css('transform', existingTransform + ' ' + rotationTransform);
				} else {
					parentDiv.css('transform', rotationTransform);
				}
			} else {
				console.error("The parent element with class 'comp-wrapper' was not found.");
			}
		
	});
};


scada.scheme.updateColors = function (divComp, cnlDataExt, isHovered, props) {
	var statusColor = cnlDataExt.Color;

	var backColor = chooseColor(
		isHovered,
		props.BackColor,
		props.BackColorOnHover,
	);
	var borderColor = chooseColor(
		isHovered,
		props.BorderColor,
		props.BorderColorOnHover,
	);

	setBackColor(divComp, backColor, true, statusColor);
	setBorderColor(divComp, borderColor, true, statusColor);
};

scada.scheme.updateComponentData = function (component, renderContext) {
	var props = component.props;

	if (props.InCnlNum <= 0) {
		return;
	}
	

	var divComp = component.dom;
	var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);
	//set backcolor
	this.setBackColor(divComp, props.BackColor);

	if (!props.Conditions || cnlDataExt.Stat <= 0) {
		return;
	}

	var cnlVal = cnlDataExt.Val;

	for (var cond of props.Conditions) {
		if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
			scada.scheme.updateStyles(divComp, cond);
			scada.scheme.handleBlinking(divComp, cond.Blinking);
			if (cond.Rotation !== -1 && cond.Rotation !== props.Rotation) {
				scada.scheme.setRotate(divComp, cond);
			}
			break;
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
scada.scheme.CustomSVGRenderer.constructor = scada.scheme.CustomSVGRenderer;



scada.scheme.CustomSVGRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;
	var divComp = $("<div id='comp" + component.id + "'></div>");
	this.prepareComponent(divComp, component, false, true);
	
	//set backcolor
	this.setBackColor(divComp, props.BackColor);
	
	if (props.SvgCode) {
		props.SvgCode = props.SvgCode.replace(
			/<svg[^>]*?(\s+width\s*=\s*["'][^"']*["'])/g,
			"<svg ",
		);
		props.SvgCode = props.SvgCode.replace(
			/<svg[^>]*?(\s+height\s*=\s*["'][^"']*["'])/g,
			"<svg height='100%' width='100%'  preserveAspectRatio='none'",
		);
	}
	divComp.append(props.SvgCode);
	component.dom = divComp;
	
	scada.scheme.setRotate(divComp, props);
	
};

scada.scheme.CustomSVGRenderer.prototype.updateData = function (
	component,
	renderContext,
) {
	scada.scheme.updateComponentData(component, renderContext);
	scada.scheme.setRotate(component.dom, component.props);
};

/**
 * Basic Shape Renderer
 */

scada.scheme.BasicShapeRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.BasicShapeRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype,
);

scada.scheme.BasicShapeRenderer.constructor = scada.scheme.BasicShapeRenderer;

scada.scheme.BasicShapeRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;
	var shapeType = props.ShapeType;

	var divComp = $("<div id='comp" + component.id + "'></div>");
	var shape = $("<div class='shape '></div>");
	this.prepareComponent(divComp, component, false, true);
	if (shapeType == "Line") {
		shape.addClass(shapeType.toLowerCase());
		shape.css({
			"border-color": props.BorderColor,
			"border-width": props.BorderWidth,
			"border-style": "solid",
			"background-color": props.BackColor,
		});

		divComp.css({
			display: "flex",
			"align-items": "center",
			"justify-content": "center",
		});

		divComp.append(shape);
	} else {
		divComp.addClass(shapeType.toLowerCase());
		this.setBackColor(divComp, props.BackColor);
		this.setBorderColor(divComp, props.BorderColor);
		if (props.BorderWidth > 0) {
			this.setBorderWidth(divComp, props.BorderWidth);
		}
	}


	component.dom = divComp;
	scada.scheme.setRotate(divComp, props);
};

scada.scheme.BasicShapeRenderer.prototype.updateData = function (
	component,
	renderContext,
) {
	scada.scheme.updateComponentData(component, renderContext);
	scada.scheme.setRotate(component.dom, component.props);
};

/**
 *  BarGraph
 * */
scada.scheme.BarGraphRenderer = function () {
	scada.scheme.ComponentRenderer.call(this);
};

scada.scheme.BarGraphRenderer.prototype = Object.create(
	scada.scheme.ComponentRenderer.prototype,
);
scada.scheme.BarGraphRenderer.constructor = scada.scheme.BarGraphRenderer;

scada.scheme.BarGraphRenderer.prototype.createDom = function (
	component,
	renderContext,
) {
	var props = component.props;

	var divComp = $("<div id='comp" + component.id + "'></div>");

	var bar = $(
		"<div class='bar' style='height:" +
			props.Value +
			"%" +
			";background-color:" +
			props.BarColor +
			"' data-value='" +
			parseInt(props.Value) +
			"'></div>",
	);

	divComp.append(bar);

	this.prepareComponent(divComp, component);

	divComp.css({
		border: props.BorderWidth + "px solid " + props.BorderColor,
		display: "flex",
		"align-items": "flex-end",
		"justify-content": "center",
	});

	component.dom = divComp;
};

scada.scheme.BarGraphRenderer.prototype.updateData = function (
	component,
	renderContext,
) {
	var props = component.props;
	if (props.InCnlNum > 0) {
		var divComp = component.dom;
		var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);

		divComp.css({
			border: props.BorderWidth + "px solid " + props.BorderColor,
			"background-color": props.BackColor,
		});
		divComp.find(".bar").css({
			"background-color": props.BarColor,
			height: props.Value + "%",
		});
		divComp.find(".bar").attr("data-value", parseInt(props.Value));
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
						barStyles["background-color"] = condition.FillColor;
					}

					divComp.find(".bar").css(barStyles);

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
scada.scheme.rendererMap.set(
	"Scada.Web.Plugins.SchShapeComp.BasicShape",
	new scada.scheme.BasicShapeRenderer(),
);
scada.scheme.rendererMap.set(
	"Scada.Web.Plugins.SchShapeComp.CustomSVG",
	new scada.scheme.CustomSVGRenderer(),
);
scada.scheme.rendererMap.set(
	"Scada.Web.Plugins.SchShapeComp.BarGraph",
	new scada.scheme.BarGraphRenderer(),
);
