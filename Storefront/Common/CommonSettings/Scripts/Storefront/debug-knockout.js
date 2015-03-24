// To use in Chrome
ko.bindingHandlers.debug =
{
    init: function (element, valueAccessor) {
        console.log('Knockoutbinding:');
        console.log(element);
        console.log(ko.toJS(valueAccessor()));
    }
};