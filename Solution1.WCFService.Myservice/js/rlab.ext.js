// Фишки из ECMAScript 6
if (!Object.assign) {
	Object.defineProperty(Object, 'assign', {
		enumerable: false,
		configurable: true,
		writable: true,
		value: function (target, firstSource) {
			'use strict';
			if (target === undefined || target === null) {
				throw new TypeError('Cannot convert first argument to object');
			}

			var to = Object(target);
			for (var i = 1; i < arguments.length; i++) {
				var nextSource = arguments[i];
				if (nextSource === undefined || nextSource === null) {
					continue;
				}

				var keysArray = Object.keys(Object(nextSource));
				for (var nextIndex = 0, len = keysArray.length; nextIndex < len; nextIndex++) {
					var nextKey = keysArray[nextIndex];
					var desc = Object.getOwnPropertyDescriptor(nextSource, nextKey);
					if (desc !== undefined && desc.enumerable) {
						to[nextKey] = nextSource[nextKey];
					}
				}
			}
			return to;
		}
	});
}

if (!Object.keys) {
    Object.keys = (function () {
        'use strict';
        var hasOwnProperty = Object.prototype.hasOwnProperty,
            hasDontEnumBug = !({ toString: null }).propertyIsEnumerable('toString'),
            dontEnums = [
              'toString',
              'toLocaleString',
              'valueOf',
              'hasOwnProperty',
              'isPrototypeOf',
              'propertyIsEnumerable',
              'constructor'
            ],
            dontEnumsLength = dontEnums.length;

        return function (obj) {
            if (typeof obj !== 'object' && (typeof obj !== 'function' || obj === null)) {
                throw new TypeError('Object.keys called on non-object');
            }

            var result = [], prop, i;

            for (prop in obj) {
                if (hasOwnProperty.call(obj, prop)) {
                    result.push(prop);
                }
            }

            if (hasDontEnumBug) {
                for (i = 0; i < dontEnumsLength; i++) {
                    if (hasOwnProperty.call(obj, dontEnums[i])) {
                        result.push(dontEnums[i]);
                    }
                }
            }
            return result;
        };
    }());
}


if (!Object.values) {
    Object.values = function values(O) {
        return reduce(
            keys(O),
            function (v, k) {
                return concat(v, typeof k === 'string' && isEnumerable(O, k) ? [O[k]] : []);
            }
            , []);
    };
}

if (!Array.prototype.FindByField) {
	Array.prototype.FindByField = function (field, value) {
		if (this == null) {
			throw new TypeError('Array.prototype.FindByField called on null or undefined');
		}
		for (var i = 0; i < this.length; i++) {
			if (this[i][field] == value) {
				return this[i];
			}
		}
		return undefined;
	};
}

if (!Array.prototype.findIndex) {
	Array.prototype.findIndex = function (predicate) {
		if (this == null) {
			throw new TypeError('Array.prototype.findIndex called on null or undefined');
		}
		if (typeof predicate !== 'function') {
			throw new TypeError('predicate must be a function');
		}
		var list = Object(this);
		var length = list.length >>> 0;
		var thisArg = arguments[1];
		var value;

		for (var i = 0; i < length; i++) {
			value = list[i];
			if (predicate.call(thisArg, value, i, list)) {
				return i;
			}
		}
		return -1;
	};
}

if (!Array.prototype.map) {

    Array.prototype.map = function (callback, thisArg) {

        var T, A, k;

        if (this == null) {
            throw new TypeError(' this is null or not defined');
        }

        // 1. Положим O равным результату вызова ToObject с передачей ему
        //    значения |this| в качестве аргумента.
        var O = Object(this);

        // 2. Положим lenValue равным результату вызова внутреннего метода Get
        //    объекта O с аргументом "length".
        // 3. Положим len равным ToUint32(lenValue).
        var len = O.length >>> 0;

        // 4. Если вызов IsCallable(callback) равен false, выкидываем исключение TypeError.
        // Смотрите (en): http://es5.github.com/#x9.11
        // Смотрите (ru): http://es5.javascript.ru/x9.html#x9.11
        if (typeof callback !== 'function') {
            throw new TypeError(callback + ' is not a function');
        }

        // 5. Если thisArg присутствует, положим T равным thisArg; иначе положим T равным undefined.
        if (arguments.length > 1) {
            T = thisArg;
        }

        // 6. Положим A равным новому масиву, как если бы он был создан выражением new Array(len),
        //    где Array является стандартным встроенным конструктором с этим именем,
        //    а len является значением len.
        A = new Array(len);

        // 7. Положим k равным 0
        k = 0;

        // 8. Пока k < len, будем повторять
        while (k < len) {

            var kValue, mappedValue;

            // a. Положим Pk равным ToString(k).
            //   Это неявное преобразование для левостороннего операнда в операторе in
            // b. Положим kPresent равным результату вызова внутреннего метода HasProperty
            //    объекта O с аргументом Pk.
            //   Этот шаг может быть объединён с шагом c
            // c. Если kPresent равен true, то
            if (k in O) {

                // i. Положим kValue равным результату вызова внутреннего метода Get
                //    объекта O с аргументом Pk.
                kValue = O[k];

                // ii. Положим mappedValue равным результату вызова внутреннего метода Call
                //     функции callback со значением T в качестве значения this и списком
                //     аргументов, содержащим kValue, k и O.
                mappedValue = callback.call(T, kValue, k, O);

                // iii. Вызовем внутренний метод DefineOwnProperty объекта A с аргументами
                // Pk, Описатель Свойства
                // { Value: mappedValue,
                //   Writable: true,
                //   Enumerable: true,
                //   Configurable: true }
                // и false.

                // В браузерах, поддерживающих Object.defineProperty, используем следующий код:
                // Object.defineProperty(A, k, {
                //   value: mappedValue,
                //   writable: true,
                //   enumerable: true,
                //   configurable: true
                // });

                // Для лучшей поддержки браузерами, используем следующий код:
                A[k] = mappedValue;
            }
            // d. Увеличим k на 1.
            k++;
        }

        // 9. Вернём A.
        return A;
    };
}

if (!Array.prototype.reduce) {
    Array.prototype.reduce = function (callback/*, initialValue*/) {
        'use strict';
        if (this == null) {
            throw new TypeError('Array.prototype.reduce called on null or undefined');
        }
        if (typeof callback !== 'function') {
            throw new TypeError(callback + ' is not a function');
        }
        var t = Object(this), len = t.length >>> 0, k = 0, value;
        if (arguments.length >= 2) {
            value = arguments[1];
        } else {
            while (k < len && !(k in t)) {
                k++;
            }
            if (k >= len) {
                throw new TypeError('Reduce of empty array with no initial value');
            }
            value = t[k++];
        }
        for (; k < len; k++) {
            if (k in t) {
                value = callback(value, t[k], k, t);
            }
        }
        return value;
    };
}

if (!Array.prototype.filter) {
    Array.prototype.filter = function (fun/*, thisArg*/) {
        'use strict';

        if (this === void 0 || this === null) {
            throw new TypeError();
        }

        var t = Object(this);
        var len = t.length >>> 0;
        if (typeof fun !== 'function') {
            throw new TypeError();
        }

        var res = [];
        var thisArg = arguments.length >= 2 ? arguments[1] : void 0;
        for (var i = 0; i < len; i++) {
            if (i in t) {
                var val = t[i];

                // ПРИМЕЧАНИЕ: Технически, здесь должен быть Object.defineProperty на
                //             следующий индекс, поскольку push может зависеть от
                //             свойств на Object.prototype и Array.prototype.
                //             Но этот метод новый и коллизии должны быть редкими,
                //             так что используем более совместимую альтернативу.
                if (fun.call(thisArg, val, i, t)) {
                    res.push(val);
                }
            }
        }

        return res;
    };
}

if (!Date.prototype.getWeek) {
    Date.prototype.getWeek = function () {
        var d = new Date(Date.UTC(this.getFullYear(), this.getMonth(), this.getDate()));
        var dayNum = d.getUTCDay() || 7;
        d.setUTCDate(d.getUTCDate() + 4 - dayNum);
        var yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
        return Math.ceil((((d - yearStart) / 86400000) + 1) / 7)
    };
}

if (!Date.prototype.getUTCWeek) {
    Date.prototype.getUTCWeek = Date.prototype.getWeek;
}

if (!Date.prototype.setWeek) {
    Date.prototype.setWeek = function (week) {

        //var yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
        var yearStart = new Date(this.getUTCFullYear(), 0, 1);
        var days = 2 + (week - 1) * 7 - yearStart.getDay();
        var tick = new Date(this.getUTCFullYear(), 0, days).getTime();
        console.log(tick);
        this.setTime(tick);



        //var d = new Date(Date.UTC(this.getFullYear(), this.getMonth(), this.getDate()));
        //var yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
        //var dd = yearStart.getTime() + (((week * 7) - 1) * 86400000);
        //console.log(dd);
        //this.setTime(dd);
        ////var dayNum = d.getUTCDay() || 7;
        ////d.setUTCDate(d.getUTCDate() + 4 - dayNum);
        ////var yearStart = new Date(Date.UTC(d.getUTCFullYear(), 0, 1));
        ////return Math.ceil((((d - yearStart) / 86400000) + 1) / 7)
    };
}

if (!Date.prototype.getDOY) {
    Date.prototype.getDOY = function () {
        return Math.floor((this - new Date(this.getFullYear(), 0, 0) ) / 86400000);
    };
}

if (!Date.prototype.getUTCDOY) {
    Date.prototype.getUTCDOY = function () {
        //start = new Date(this.getUTCFullYear(), 0, 0);
        //diff = this - start + (this.getTimezoneOffset() * 60 * 1000);
        //oneDay = 1000 * 60 * 60 * 24;
        //day = Math.floor(diff / oneDay);
        //return day
        return Math.floor((this - new Date(this.getUTCFullYear(), 0, 0) + (this.getTimezoneOffset() * 60 * 1000)) / 86400000);
    };
}

if (!Date.prototype.ToESAString) {
    Date.prototype.ToESAString = function () {
        return this.getUTCFullYear()
            + '-' + ("000" + this.getUTCDOY()).slice(-3)
            + 'T' + ("00" + this.getUTCHours()).slice(-2)
            + ':' + ("00" + this.getUTCMinutes()).slice(-2)
            + ':' + ("00" + this.getUTCSeconds()).slice(-2)
            + '.' + String((this.getUTCMilliseconds() / 1000).toFixed(3)).slice(2, 5)
            + 'Z';
    };
}





// https://stackoverflow.com/questions/149055/how-can-i-format-numbers-as-money-in-javascript
if (!Number.prototype.format) {
    Number.prototype.format = function (mask) {
        var fr = mask.split(".");
        if (fr.length > 1) {
            return this.toFixed(fr[1].length);
        }
        return this.toString();
    };
}

if (!Date.prototype.format) {
    Date.prototype.format = function (mask, utc) {
        var dateFormat = function () {
            //var token = /d{1,4}|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
            var token = /d{1,5}|w|m{1,4}|yy(?:yy)?|([HhMsTt])\1?|[LloSZ]|"[^"]*"|'[^']*'/g,
                timezone = /\b(?:[PMCEA][SDP]T|(?:Pacific|Mountain|Central|Eastern|Atlantic) (?:Standard|Daylight|Prevailing) Time|(?:GMT|UTC)(?:[-+]\d{4})?)\b/g,
                timezoneClip = /[^-+\dA-Z]/g,
                pad = function (val, len) {
                    val = String(val);
                    len = len || 2;
                    while (val.length < len) val = "0" + val;
                    return val;
                };

            // Regexes and supporting functions are cached through closure
            return function (date, mask, utc) {
                var dF = dateFormat;

                // You can't provide utc if you skip other args (use the "UTC:" mask prefix)
                if (arguments.length == 1 && Object.prototype.toString.call(date) == "[object String]" && !/\d/.test(date)) {
                    mask = date;
                    date = undefined;
                }

                // Passing date through Date applies Date.parse, if necessary
                date = date ? new Date(date) : new Date;
                if (isNaN(date)) { // throw SyntaxError("invalid date");
                    return "";
                }

                mask = String(dF.masks[mask] || mask || dF.masks["default"]);

                // Allow setting the utc argument via the mask
                if (mask.slice(0, 4) == "UTC:") {
                    mask = mask.slice(4);
                    utc = true;
                }

                var _ = utc ? "getUTC" : "get",
                    d = date[_ + "Date"](),
                    D = date[_ + "Day"](),
                    w = date[_ + "Week"](),
                    m = date[_ + "Month"](),
                    y = date[_ + "FullYear"](),
                    H = date[_ + "Hours"](),
                    M = date[_ + "Minutes"](),
                    s = date[_ + "Seconds"](),
                    L = date[_ + "Milliseconds"](),
                    o = utc ? 0 : date.getTimezoneOffset(),
                    flags = {
                        d: d,
                        dd: pad(d),
                        ddd: dF.i18n.dayNames[D],
                        dddd: dF.i18n.dayNames[D + 7],
                        ddddd: Math.floor((date - new Date(y, 0, 0)) / 86400000), //http://calendarin.net/scripts/js/classes/Calendar.class.js?20
                        w: w,
                        m: m + 1,
                        mm: pad(m + 1),
                        mmm: dF.i18n.monthNames[m],
                        mmmm: dF.i18n.monthNames[m + 12],
                        yy: String(y).slice(2),
                        yyyy: y,
                        h: H % 12 || 12,
                        hh: pad(H % 12 || 12),
                        H: H,
                        HH: pad(H),
                        M: M,
                        MM: pad(M),
                        s: s,
                        ss: pad(s),
                        l: pad(L, 3),
                        L: pad(L > 99 ? Math.round(L / 10) : L),
                        t: H < 12 ? "a" : "p",
                        tt: H < 12 ? "am" : "pm",
                        T: H < 12 ? "A" : "P",
                        TT: H < 12 ? "AM" : "PM",
                        Z: utc ? "UTC" : (String(date).match(timezone) || [""]).pop().replace(timezoneClip, ""),
                        o: (o > 0 ? "-" : "+") + pad(Math.floor(Math.abs(o) / 60) * 100 + Math.abs(o) % 60, 4),
                        S: ["th", "st", "nd", "rd"][d % 10 > 3 ? 0 : (d % 100 - d % 10 != 10) * d % 10]
                    };


                //Calendar.getDayNumberInYear = function (b, c, a) {
                //    c = new Date(date.getFullYear(), 0, 0);
                //    return Math.floor((date - new Date(date.getFullYear(), 0, 0)) / 86400000)
                //};
                return mask.replace(token, function ($0) {
                    return $0 in flags ? flags[$0] : $0.slice(1, $0.length - 1);
                });
            };
        }();

        dateFormat.masks = {
            "default": "ddd mmm dd yyyy HH:MM:ss Z",
            shortDate: "m/d/yy",
            mediumDate: "mmm d, yyyy",
            longDate: "mmmm d, yyyy",
            fullDate: "dddd, mmmm d, yyyy",
            shortTime: "h:MM TT",
            mediumTime: "h:MM:ss TT",
            longTime: "h:MM:ss TT Z",
            isoDate: "yyyy-mm-dd",
            isoTime: "HH:MM:ss",
            isoDateTime: "yyyy-mm-dd'T'HH:MM:ss",
            isoUtcDateTime: "UTC:yyyy-mm-dd'T'HH:MM:ss.l'Z'"
        };

        dateFormat.i18n = {
            dayNames: [
                "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat",
                "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
            ],
            monthNames: [
                "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec",
                "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"
            ]
        };
        //http://blog.stevenlevithan.com/archives/date-time-format
        return dateFormat(this, mask, utc);
    };
}

