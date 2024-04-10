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

scada.scheme.handleBlinking = function (divComp, blinking, bool) {
    if (bool) {
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
    } else {
        divComp.removeClass("no-blink slow-blink fast-blink");
    }
};
scada.scheme.updateStyles = function (divComp, cond, bool) {
    if ("color" in cond) {
        divComp.css("color", cond.color);
    }
    if ("rotation" in cond) {
        divComp.css("transform", "rotate(" + cond.rotation + "deg)");
    }

    if ("backgroundColor" in cond) {
        if (bool) {
            divComp.css("background-color", cond.backgroundColor);
        } else {
            divComp.css(
                "background-color",
                cond.backColor ? String(cond.backColor) : "",
            );
        }
    }
    if ("isVisible" in cond) {
        if (bool) {
            divComp.css("visibility", cond.isVisible ? "visible" : "hidden");
        } else {
            divComp.css("visibility", "visible");
        }
    }

    if ("width" in cond) {
        if (bool) {
            divComp.css("width", cond.width);
        } else {
            divComp.css("width", "100px");
        }
    }

    if ("height" in cond) {
        if (bool) {
            divComp.css("height", cond.height);
        } else {
            divComp.css("height", "100px");
        }
    }
};

scada.scheme.setRotate = function (divComp, props) {
    setTimeout(function () {
        var compWrapper = divComp.closest('.comp-wrapper');
        if (compWrapper.length > 0 && props.Rotation) {
            var rotation = parseInt(props.Rotation, 10);

            if (rotation) {

                compWrapper.css('transform', '');
                console.log("Transform removed from comp-wrapper of " + divComp.attr('id'));
            } else {
                var rotationTransform = 'rotate(' + rotation + 'deg)';
                console.log(rotationTransform);
                compWrapper.css('transform', rotationTransform);
                console.log("Rotation updated for comp-wrapper of " + divComp.attr('id'));
            }
        } else {
            console.error("comp-wrapper not found for " + divComp.attr('id'));
        }
    }, 0);
}

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

function mergeAndModifyConditions(conditions) {
    let result = [];
    const criteriaFields = [
        "compareOperator1",
        "compareArgument1",
        "logicalOperator",
        "compareOperator2",
        "compareArgument2",
    ];

    let groups = conditions.reduce((acc, condition) => {
        const criteriaKey = criteriaFields
            .map((field) => condition[field])
            .join("_");
        if (!acc[criteriaKey]) acc[criteriaKey] = [];
        acc[criteriaKey].push(condition);
        return acc;
    }, {});

    Object.values(groups).forEach((group) => {
        let mergedCondition = { ...group[0] };
        group.slice(1).forEach((condition) => {
            Object.keys(condition).forEach((key) => {
                if (!criteriaFields.includes(key) && condition[key] != null) {
                    // for blinking attribute
                    if (key.toLowerCase() === "blinking") {
                        if (
                            condition[key] !== 0 ||
                            mergedCondition[key] === undefined ||
                            mergedCondition[key] === null ||
                            mergedCondition[key] === 0
                        ) {
                            mergedCondition[key] = condition[key];
                        }
                    } else {
                        // for other attributes
                        if (
                            mergedCondition[key] === undefined ||
                            mergedCondition[key] === null ||
                            mergedCondition[key] === ""
                        ) {
                            mergedCondition[key] = condition[key];
                        }
                    }
                }
            });
        });

        result.push(mergedCondition);
    });

    return result;
}
scada.scheme.updateComponentData = function (component, renderContext) {
    var props = component.props;
    if (props.inCnlNum <= 0) {
        return;
    }
    var divComp = component.dom;
    var cnlDataExt = renderContext.getCnlDataExt(props.inCnlNum);

    if (props.conditions && cnlDataExt.d.stat > 0) {
        var cnlVal = cnlDataExt.d.val;
        var mergedAndModifiedConditions = mergeAndModifyConditions(
            props.conditions,
        );
        let count = 0;

        for (var cond of mergedAndModifiedConditions) {
            if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
                scada.scheme.updateStyles(divComp, cond, true);
                scada.scheme.handleBlinking(divComp, cond.blinking, true);
                count = count + 1;
            } else {
                if (count < 0) {
                    scada.scheme.updateStyles(divComp, props, false);
                    scada.scheme.handleBlinking(divComp, cond.blinking, false);
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
            "<svg",
        );
        props.SvgCode = props.SvgCode.replace(
            /<svg/g,
            "<svg height='100%' width='100%' preserveAspectRatio='none'"
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

scada.scheme.BarGraphRenderer.prototype.calculateFillingRate = function (props, cnlDataExt) {
    let valueToUse;

    if (props.MaxValue < props.MinValue) {
        return -1;
    }

    if (cnlDataExt !== null) {
        valueToUse = cnlDataExt;
    } else if (props.CtrlCnlNum !== 0) {
        valueToUse = props.CtrlCnlNum;
    } else if (props.InCnlNum !== 0) {
        valueToUse = props.InCnlNum;
    } else {
        return -1;
    }

    if (valueToUse < props.MinValue) {
        return 0;
    }

    if (valueToUse > props.MaxValue) {
        return 100;
    }

    return (valueToUse - props.MinValue) * 100 / (props.MaxValue - props.MinValue);
};


scada.scheme.BarGraphRenderer.prototype.createDom = function (
    component,
    renderContext,
) {
    var props = component.props;

    var divComp = $("<div id='comp" + component.id + "'></div>");

    if (this.calculateFillingRate(props,null) === -1) {
        var disabledBar = $(
            "<div class='bar disabled' title='Erreur de configuration : absence de données valides' style='height: 71%; background-color: #5f5f81; filter: blur(1.5px);'>" +
            "<span class='error-cross' style='position: absolute; top: 50%; left: 50%; transform: translate(-50%, -50%); font-size: 35px; color: red;'>X</span>" +
            "</div>"
        );
        divComp.append(disabledBar);
    } else {
        var bar = $(
            "<div class='bar' style='height:" +
            this.calculateFillingRate(props,null) +
            "%" +
            ";background-color:" +
            props.FillColor +
            "' data-value='" +
            this.calculateFillingRate(props, null) +
            "'></div>",
        );
        divComp.append(bar);
    }

    this.prepareComponent(divComp, component, false, true);

    divComp.css({
        border: props.BorderWidth + "px solid " + props.BorderColor,
        display: "flex",
        "align-items": "flex-end",
        "justify-content": "center",
        "background-color": props.BackColor,
        
    });
    component.dom = divComp;
    scada.scheme.setRotate(divComp, props);
};
//create prototype for set dynamic filling rate for bar graph
scada.scheme.BarGraphRenderer.prototype.setDynamicFillingRate = function (divComp, props, cnlDataExt) {
    var bar = divComp.find(".bar");
    var fillingRate = parseFloat(this.calculateFillingRate(props, cnlDataExt).toFixed(2),
    );
    bar.css({
        height: fillingRate + "%",
    });
    bar.attr("data-value", parseInt(fillingRate));
};

scada.scheme.BarGraphRenderer.prototype.updateData = function (
    component,
    renderContext,
) {
    var props = component.props;
    if (props.InCnlNum > 0 ) {
        var divComp = component.dom;
        var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);

        this.setDynamicFillingRate(divComp, props, cnlDataExt);
    }

    if (props.conditions && cnlDataExt.Stat > 0) {

        for (var cond of props.conditions) {
            if (scada.scheme.calc.conditionSatisfied(cond, cnlDataExt.Val)) {
                var barStyles = {};

                if (condition.fillColor) {
                    barStyles["color"] = condition.fillColor;
                }
            }
        }
    }
    scada.scheme.updateComponentData(component, renderContext);

};


/********** Dynamic Text Renderer **********/

// Dynamic text renderer type extends scada.scheme.StaticTextRenderer
scada.scheme.DynamicTextRenderer = function () {
    scada.scheme.StaticTextRenderer.call(this);
};

scada.scheme.DynamicTextRenderer.prototype = Object.create(scada.scheme.StaticTextRenderer.prototype);
scada.scheme.DynamicTextRenderer.constructor = scada.scheme.DynamicTextRenderer;

scada.scheme.DynamicTextRenderer.prototype._setUnderline = function (jqObj, underline) {
    if (underline) {
        jqObj.css("text-decoration", "underline");
    }
};

scada.scheme.DynamicTextRenderer.prototype._restoreUnderline = function (jqObj, font) {
    jqObj.css("text-decoration", font && font.Underline ? "underline" : "none");
};

scada.scheme.DynamicTextRenderer.prototype.createDom = function (component, renderContext) {
    scada.scheme.StaticTextRenderer.prototype.createDom.call(this, component, renderContext);

    var ShowValueKinds = scada.scheme.ShowValueKinds;
    var props = component.props;
    var spanComp = component.dom.first();
    var spanText = component.dom.children();
    var cnlNum = props.InCnlNum;


    //apply rotation
    scada.scheme.setRotate(spanComp, props);

    if (props.ShowValue > ShowValueKinds.NOT_SHOW && !props.Text) {
        spanText.text("[" + cnlNum + "]");
    }

    this.bindAction(spanComp, component, renderContext);

    // apply properties on hover
    var thisRenderer = this;

    spanComp.hover(
        function () {
            thisRenderer.setDynamicBackColor(spanComp, props.BackColorOnHover, cnlNum, renderContext);
            thisRenderer.setDynamicBorderColor(spanComp, props.BorderColorOnHover, cnlNum, renderContext);
            thisRenderer.setDynamicForeColor(spanComp, props.ForeColorOnHover, cnlNum, renderContext);
            thisRenderer._setUnderline(spanComp, props.UnderlineOnHover);
        },
        function () {
            thisRenderer.setDynamicBackColor(spanComp, props.BackColor, cnlNum, renderContext, true);
            thisRenderer.setDynamicBorderColor(spanComp, props.BorderColor, cnlNum, renderContext, true);
            thisRenderer.setDynamicForeColor(spanComp, props.ForeColor, cnlNum, renderContext, true);
            thisRenderer._restoreUnderline(spanComp, props.Font);
        }
    );
};

scada.scheme.DynamicTextRenderer.prototype.updateData = function (component, renderContext) {
    var props = component.props;

    if (props.InCnlNum > 0) {
        var ShowValueKinds = scada.scheme.ShowValueKinds;
        var spanComp = component.dom;
        var spanText = spanComp.children();
        var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);

        // show value of the appropriate input channel
        switch (props.ShowValue) {
            case ShowValueKinds.SHOW_WITH_UNIT:
                spanText.text(cnlDataExt.TextWithUnit);
                break;
            case ShowValueKinds.SHOW_WITHOUT_UNIT:
                spanText.text(cnlDataExt.Text);
                break;
        }

        // choose and set colors of the component
        var statusColor = cnlDataExt.Color;
        var isHovered = spanComp.is(":hover");

        var backColor = this.chooseColor(isHovered, props.BackColor, props.BackColorOnHover);
        var borderColor = this.chooseColor(isHovered, props.BorderColor, props.BorderColorOnHover);
        var foreColor = this.chooseColor(isHovered, props.ForeColor, props.ForeColorOnHover);

        this.setBackColor(spanComp, backColor, true, statusColor);
        this.setBorderColor(spanComp, borderColor, true, statusColor);
        this.setForeColor(spanComp, foreColor, true, statusColor);

        //update component data
        scada.scheme.updateComponentData(component, renderContext);

        // apply rotation
        scada.scheme.setRotate(spanComp, props);
    }
};




// Dynamic picture renderer type extends scada.scheme.StaticPictureRenderer
scada.scheme.DynamicPictureRenderer = function () {
    scada.scheme.StaticPictureRenderer.call(this);
};

scada.scheme.DynamicPictureRenderer.prototype = Object.create(scada.scheme.StaticPictureRenderer.prototype);
scada.scheme.DynamicPictureRenderer.constructor = scada.scheme.DynamicPictureRenderer;

scada.scheme.DynamicPictureRenderer.prototype.createDom = function (component, renderContext) {
    scada.scheme.StaticPictureRenderer.prototype.createDom.call(this, component, renderContext);

    var props = component.props;
    var divComp = component.dom;

    // apply rotation
    scada.scheme.setRotate(divComp, props);

    this.bindAction(divComp, component, renderContext);

    // apply properties on hover
    var thisRenderer = this;
    var cnlNum = props.InCnlNum;

    divComp.hover(
        function () {
            thisRenderer.setDynamicBackColor(divComp, props.BackColorOnHover, cnlNum, renderContext);
            thisRenderer.setDynamicBorderColor(divComp, props.BorderColorOnHover, cnlNum, renderContext);

            if (cnlNum <= 0) {
                var image = renderContext.getImage(props.ImageOnHoverName);
                thisRenderer.setBackgroundImage(divComp, image);
            }
        },
        function () {
            thisRenderer.setDynamicBackColor(divComp, props.BackColor, cnlNum, renderContext, true);
            thisRenderer.setDynamicBorderColor(divComp, props.BorderColor, cnlNum, renderContext, true);

            if (cnlNum <= 0) {
                var image = renderContext.getImage(props.ImageName);
                thisRenderer.setBackgroundImage(divComp, image, true);
            }
        }
    );
};


scada.scheme.DynamicPictureRenderer.prototype.updateData = function (component, renderContext) {
    var props = component.props;

    if (props.InCnlNum > 0) {
        var divComp = component.dom;
        var cnlDataExt = renderContext.getCnlDataExt(props.InCnlNum);
        var imageName = props.ImageName;

        // choose an image depending on the conditions
        if (cnlDataExt.Stat && props.Conditions) {
            var cnlVal = cnlDataExt.Val;

            for (var cond of props.Conditions) {
                if (scada.scheme.calc.conditionSatisfied(cond, cnlVal)) {
                    imageName = cond.ImageName;
                    break;
                }
            }
        }

        // set the image
        var image = renderContext.imageMap.get(imageName);
        this.setBackgroundImage(divComp, image, true);

        // choose and set colors of the component
        var statusColor = cnlDataExt.Color;
        var isHovered = divComp.is(":hover");

        var backColor = this.chooseColor(isHovered, props.BackColor, props.BackColorOnHover);
        var borderColor = this.chooseColor(isHovered, props.BorderColor, props.BorderColorOnHover);

        this.setBackColor(divComp, backColor, true, statusColor);
        this.setBorderColor(divComp, borderColor, true, statusColor);

        //update component data
        scada.scheme.updateComponentData(component, renderContext);

        // apply rotation
        scada.scheme.setRotate(divComp, props);
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
scada.scheme.rendererMap.set(
    "Scada.Web.Plugins.SchShapeComp.DynamicText",
    new scada.scheme.DynamicTextRenderer(),
);
scada.scheme.rendererMap.set(
    "Scada.Web.Plugins.SchShapeComp.DynamicPicture",
    new scada.scheme.DynamicPictureRenderer(),
);
