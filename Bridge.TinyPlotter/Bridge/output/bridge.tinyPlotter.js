(function (globals) {
    "use strict";

    /** @namespace Bridge.TinyPlotter */
    
    /**
     * A minimalistic plotter for single variable functions.
     *
     * @public
     * @class Bridge.TinyPlotter.TinyPlotter
     */
    Bridge.define('Bridge.TinyPlotter.TinyPlotter', {
        settings: null,
        canvas: null,
        config: {
            init: function () {
                this.settings = { curves: new System.Collections.Generic.List$1(Object)(), drawBorder: true, drawXAxis: true, drawYAxis: true, height: 500, width: 800, deltaX: 0.01, xMin: -10, xMax: 10, yMin: -10, yMax: 10 };
            }
        },
        constructor$1: function (settings) {
            this.settings = settings;
            this.initializePlot();
        },
        constructor$3: function (f, xmin, xmax, ymin, ymax) {
            var black = { red: 0, green: 0, blue: 0 };
            this.settings.curves.add({ map: f, color: black });
            this.settings.xMin = xmin;
            this.settings.xMax = xmax;
            this.settings.yMin = ymin;
            this.settings.yMax = ymax;
            this.initializePlot();
        },
        constructor: function () {
            this.initializePlot();
        },
        constructor$4: function (functions) {
            if (functions === void 0) { functions = []; }
            var $t;
    
            var black = { red: 0, green: 0, blue: 0 };
            $t = Bridge.getEnumerator(functions);
            while ($t.moveNext()) {
                var func = $t.getCurrent();
                var curve = { color: black, map: func };
                this.settings.curves.add(curve);
            }
            this.initializePlot();
    },
    constructor$2: function (f, xmin, xmax) {
        var black = { red: 0, green: 0, blue: 0 };
        this.settings.curves.add({ map: f, color: black });
        this.settings.xMin = xmin;
        this.settings.xMax = xmax;
        this.initializePlot();
    },
    setViewport: function (xmin, xmax, ymin, ymax) {
        this.settings.xMin = xmin;
        this.settings.xMax = xmax;
        this.settings.yMin = ymin;
        this.settings.yMax = ymax;
    },
    initializePlot: function () {
        this.canvas = document.createElement('canvas');
        this.canvas.height = this.settings.height;
        this.canvas.width = this.settings.width;
        if (this.settings.drawBorder) {
            this.canvas.style.border = "1px solid black";
        }
        var ctx = this.canvas.getContext("2d");
        var image = ctx.createImageData(this.canvas.width, this.canvas.height);
        if (this.settings.drawXAxis) {
            this.drawXAxis(image);
        }
        if (this.settings.drawXAxis) {
            this.drawYAxis(image);
        }
        ctx.putImageData(image, 0, 0);
    },
    /**
     * Draws all curves within Settings.Curves on the canvas.
     *
     * @instance
     * @public
     * @this Bridge.TinyPlotter.TinyPlotter
     * @memberof Bridge.TinyPlotter.TinyPlotter
     * @return  {void}
     */
    draw: function () {
        var $t;
        if (!this.settingsLookGood()) {
            throw new System.Exception("Settings are invalid");
        }
    
        var ctx = this.canvas.getContext("2d");
        var image = ctx.createImageData(this.canvas.width, this.canvas.height);
    
        if (this.settings.drawXAxis) {
            this.drawXAxis(image);
        }
    
        if (this.settings.drawXAxis) {
            this.drawYAxis(image);
        }
    
        $t = Bridge.getEnumerator(this.settings.curves);
        while ($t.moveNext()) {
            var curve = $t.getCurrent();
            this.drawCurve(curve, image);
        }
    
        ctx.putImageData(image, 0, 0);
    },
    drawXAxis: function (image) {
        var xmin = this.settings.xMin;
        var xmax = this.settings.xMax;
        var ymin = this.settings.yMin;
        var ymax = this.settings.yMax;
        var step = this.settings.deltaX;
    
        for (var x = xmin; x <= xmax; x += 0.01) {
            var point = { x: x, y: 0 };
            var pointFromPlain = this.fromPointOnPlain(point, xmin, xmax, ymin, ymax, this.canvas.height, this.canvas.width);
            this.setPixel(image, Bridge.Int.clip32(pointFromPlain.x), Bridge.Int.clip32(pointFromPlain.y), { red: 0, blue: 0, green: 0 });
        }
    },
    drawYAxis: function (image) {
        var xmin = this.settings.xMin;
        var xmax = this.settings.xMax;
        var ymin = this.settings.yMin;
        var ymax = this.settings.yMax;
        var step = this.settings.deltaX;
    
        for (var y = ymin; y <= ymax; y += 0.01) {
            var point = { x: 0, y: y };
            var pointFromPlain = this.fromPointOnPlain(point, xmin, xmax, ymin, ymax, this.canvas.height, this.canvas.width);
            this.setPixel(image, Bridge.Int.clip32(pointFromPlain.x), Bridge.Int.clip32(pointFromPlain.y), { red: 0, blue: 0, green: 0 });
        }
    },
    drawCurve: function (curve, image) {
        var xmin = this.settings.xMin;
        var xmax = this.settings.xMax;
        var ymin = this.settings.yMin;
        var ymax = this.settings.yMax;
        var step = this.settings.deltaX;
    
        for (var x = xmin; x <= xmax; x += step) {
            var y = curve.map(x);
            if (y < ymin || y > ymax || isNaN(y)) {
                continue;
            }
    
            var point = { x: x, y: y };
            var pointFromPlain = this.fromPointOnPlain(point, xmin, xmax, ymin, ymax, this.canvas.height, this.canvas.width);
            this.setPixel(image, Bridge.Int.clip32(pointFromPlain.x), Bridge.Int.clip32(pointFromPlain.y), curve.color);
        }
    },
    rescale: function (value, realRange, projection) {
        var percentageOfProjection = (Math.abs(projection.to - projection.from) * Math.abs(value - realRange.from)) / Math.abs(realRange.to - realRange.from);
    
        return percentageOfProjection + projection.from;
    },
    settingsLookGood: function () {
        return this.settings.xMin < this.settings.xMax && this.settings.deltaX < Math.abs(this.settings.xMax - this.settings.xMin) && this.settings.yMin < this.settings.yMax && this.settings.height > 0 && this.settings.width > 0;
    },
    fromPointOnPlain: function (p, xmin, xmax, ymin, ymax, height, width) {
        var projectedPoint = { x: this.rescale(p.x, { from: xmin, to: xmax }, { from: 0, to: width }), y: height - this.rescale(p.y, { from: ymin, to: ymax }, { from: 0, to: height }) };
    
    
        return projectedPoint;
    },
    setPixel: function (img, x, y, color) {
        var index = ((((x + ((y * (img.width | 0)) | 0)) | 0)) * 4) | 0;
        img.data[index] = color.red;
        img.data[((index + 1) | 0)] = color.green;
        img.data[((index + 2) | 0)] = color.blue;
        img.data[((index + 3) | 0)] = 255; // alpha
    }
    });
    
    Bridge.init();
})(this);
