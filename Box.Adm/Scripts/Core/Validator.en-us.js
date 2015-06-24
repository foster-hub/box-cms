jQuery.extend(jQuery.validator.messages, {
    required: "This is a required field",
    email: "Provide a valid e-mail",    
    url: "Provide a valid url",
    date: "Provide a valid date",        
    number: "Provide a valid number",
    digits: "Provide only digits",        
    equalTo: "Please, provide the same value again",        
    minlength: jQuery.validator.format("Provide at least {0} caracteres")
});

jQuery.validator.setDefaults({
    errorPlacement:
    function (error, element) {
        offset = element.offset();
        error.insertAfter(element)
        error.addClass('field-validation-error');  // add a class to the wrapper
        error.css('position', 'absolute');
        error.css('right', 0);
        error.css('top', 0);
    }
});

// JQUERY VALIDATOR REQURIES THAT INPUT FIELD HAS NAMES
// WE DONT USE NAMES - SO GENERATE THEM
jQuery.validator.setValidator =
function (formId) {
    var form = document.getElementById(formId);
    if(form==null)
        return;

    var a = Array.prototype.slice.call(form.getElementsByTagName("input")),
        b = Array.prototype.slice.call(form.getElementsByTagName("select")),
        c = Array.prototype.slice.call(form.getElementsByTagName("textarea"));

    var inputs = a.concat(b).concat(c);
        
    var name = 0;
    for (var i = 0; i < inputs.length; i++) {
        if (inputs[i].name == null || inputs[i].name == '') {
            inputs[i].name = 'auto' + name;
            name++;
        }
    }
    $('#' + formId).validate();
}
