jQuery.extend(jQuery.validator.messages, {
    required: "Este campo es obrigatorio",
    email: "Introduzca un correo electrónico válido",    
    url: "Introduzca una URL válida",
    date: "Introduzca una fecha válida",
    number: "Introduzca un número válido",
    digits: "Sólo introduzca dígitos",
    equalTo: "Por favor, informe el mismo valor de nuevo",
    minlength: jQuery.validator.format("Informe al menos {0} caracteres")
});

jQuery.validator.setDefaults({
    errorPlacement:
    function (error, element) {
            offset = element.offset();
            error.insertAfter(element)
            error.addClass('field-validation-error');  // add a class to the wrapper
            error.css('position', 'absolute');            
            error.css('right', 0);
            error.css('top',0);            
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
