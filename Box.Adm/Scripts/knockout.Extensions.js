ko.bindingHandlers.stopBindings = {
    init: function () {
        return { 'controlsDescendantBindings': true };
    }
};

ko.bindingHandlers.date = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        try {
            var jsonDate = ko.utils.unwrapObservable(valueAccessor());
            var value = ko.bindingHandlers.date.parseJsonDateString(jsonDate);
            if ($(element).is('input'))
                $(element).val(ko.bindingHandlers.date.dateToStr(value));
            else
                $(element).text(ko.bindingHandlers.date.dateToStr(value));
        }
        catch (exc) {
        }

        $(element).change(function () {

            var modelValue = ko.utils.unwrapObservable(valueAccessor()),
                dateValue = ko.bindingHandlers.date.strToDate(element.value);

            if (dateValue != null) {
                if (modelValue != null) {
                    dateValue.setHours(modelValue.getHours());
                    dateValue.setMinutes(modelValue.getMinutes());
                }
                dateValue.setSeconds(0);
                dateValue.setMilliseconds(0);
            }

            if (ko.isWriteableObservable(valueAccessor())) {
                var value = valueAccessor();
                value(dateValue);
            }
            else {
                allBindingsAccessor()._ko_property_writers.date(dateValue);
            }
        });

    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {

        var jsonDate = ko.utils.unwrapObservable(valueAccessor());
        var value = ko.bindingHandlers.date.parseJsonDateString(jsonDate);

        if ($(element).is('input'))
            $(element).val(ko.bindingHandlers.date.dateToStr(value));
        else
            $(element).text(ko.bindingHandlers.date.dateToStr(value));
    }
};

ko.bindingHandlers.date.dateToStr = function (value) {
    if (value == null)
        return '';
    var strDate = (value.getMonth() + 1) + "/"
                            + value.getDate() + "/"
                            + value.getFullYear();

    if (_dateFormat != 'mm/dd/yy') {
        strDate = value.getDate() + "/"
                            + (value.getMonth() + 1) + "/"
                            + value.getFullYear();
    }
    return strDate;
}

ko.bindingHandlers.date.strToDate = function (str) {

    if (str == null || str.trim() == '')
        return null;

    var d = new Date();
    var parts = str.split('/');
    if (_dateFormat == 'mm/dd/yy') {
        d.setFullYear(parts[2], parseInt(parts[0]) - 1, parts[1]);
    }
    else
        d.setFullYear(parts[2], parseInt(parts[1]) - 1, parts[0]);
    d.setHours(0, 0, 0, 0);
    return d;
}

ko.bindingHandlers.date.parseJsonDateString = function (value) {
    var jsonDateRE = /^\/Date\((-?\d+)(\+|-)?(\d+)?\)\/$/;
    var arr = value && jsonDateRE.exec(value);
    if (arr) {
        return new Date(parseInt(arr[1]));
    }
    return value;
};


ko.bindingHandlers.timeHour = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        try {
            var jsonDate = ko.utils.unwrapObservable(valueAccessor());
            var value = ko.bindingHandlers.date.parseJsonDateString(jsonDate);
            if (value == null)
                value = 0;
            else
                value = value.getHours();
            $(element).val(value);
        }
        catch (exc) {
        }

        $(element).change(function () {
            var fullDate = valueAccessor(),
                hourValue = element.value;

            if (fullDate == null)
                return;
            fullDate.setHours(hourValue);
            fullDate.setSeconds(0);
            fullDate.setMilliseconds(0);

            if (ko.isWriteableObservable(fullDate)) {
                modelValue(fullDate);
            }
            else {
                allBindingsAccessor()._ko_property_writers.timeHour(fullDate);
            }
        });

    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor();
        if (value == null)
            value = 0;
        else
            value = value.getHours();
        $(element).val(value);
    }
};

ko.bindingHandlers.timeMinutes = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        try {
            var jsonDate = ko.utils.unwrapObservable(valueAccessor());
            var value = ko.bindingHandlers.date.parseJsonDateString(jsonDate);
            if (value == null)
                value = 0;
            else
                value = value.getMinutes();
            $(element).val(value);
        }
        catch (exc) {
        }

        $(element).change(function () {
            var fullDate = valueAccessor(),
                minutesValue = element.value;

            if (fullDate == null)
                return;
            fullDate.setMinutes(minutesValue, 0, 0);
            

            if (ko.isWriteableObservable(fullDate)) {
                modelValue(fullDate);
            }
            else {
                allBindingsAccessor()._ko_property_writers.timeMinutes(fullDate);
            }
        });

    },
    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = valueAccessor();
        if (value == null)
            value = 0;
        else
            value = value.getMinutes();
        $(element).val(value);
    }
};

ko.bindingHandlers.nicEdit = {

    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        element.setAttribute('value', value);
        htmlEditor.panelInstance(element);

        var nic = htmlEditor.instanceById(element);

        nic.setOnChange(function () {
            var modelValue = valueAccessor(),
                value = nic.getContent();

            if (ko.isWriteableObservable(modelValue))
                modelValue(value);
            else
                allBindingsAccessor()._ko_property_writers.nicEdit(value);

            if(ko.bindingHandlers.nicEdit.onChange!=null)
                ko.bindingHandlers.nicEdit.onChange();
            
        });

        $(element).change(function () {
            var modelValue = valueAccessor(),
                value = element.value;

            if (ko.isWriteableObservable(modelValue))
                modelValue(value);
            else
                allBindingsAccessor()._ko_property_writers.nicEdit(value);
        });

    },

    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        var value = ko.utils.unwrapObservable(valueAccessor());
        element.setAttribute('value', value);
        var nic = htmlEditor.instanceById(element);
        nic.setContent(value);
    }
};

ko.extenders.withPrevious = function (target) {
    // Define new properties for previous value and whether it's changed
    target.previous = ko.observable();
    target.changed = ko.computed(function () { return target() !== target.previous(); });

    // Subscribe to observable to update previous, before change.
    target.subscribe(function (v) {
        target.previous(v);
    }, null, 'beforeChange');

    // Return modified observable
    return target;
}

ko.bindingHandlers.booleanValue = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var observable = valueAccessor(),
            interceptor = ko.computed({
                read: function () {
                    return observable.toString();
                },
                write: function (newValue) {
                    if (ko.isWriteableObservable(observable))
                        observable(newValue === "true");
                    else
                        allBindingsAccessor()._ko_property_writers.booleanValue(newValue === "true");
                }
            });

        ko.applyBindingsToNode(element, { value: interceptor });
    }
};