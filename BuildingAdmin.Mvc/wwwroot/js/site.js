$(function(){
    if(document.cookie.indexOf('acceptedCookies=') === -1){
        $("#cookie-nav").show();
        $("#sectionsNav").addClass('fixed-top-with-cookie');
    }
    $.each($("form :input:not(:button):not(:checkbox)"), function(index){
        var errorText = $(this).parent().siblings('.text-danger').text();
        if(errorText != '' && errorText !== undefined){
            this.setCustomValidity(errorText);
            $(this).parent().parent().addClass('has-danger');
        }else{
           CustomValidity(this);
        }
    });
    $("form :input:not(:button):not(:checkbox)").on("keyup blur oninvalid",function(){
        CustomValidity(this);
    });
    $("form :input:not(:button):not(:checkbox)").on("oninput",function(){
        textbox.setCustomValidity('');
    });
    if($("#WithPersonalReading").is(":checked")){
        $("#displayName").show();
    }
    else
    {
        $("#setDefaultValue").show();
    }
})

function CustomValidity(textbox){
    
    errorText = HasError(textbox);
    
    textbox.setCustomValidity(errorText);
    var icon = $(textbox).siblings('.form-control-feedback').children(".material-icons");
    if(errorText !== ''){
        $(textbox).parent().parent().addClass('has-danger');
        $(textbox).parent().parent().removeClass('has-success');
        $(icon).text("clear");
    }else{
        $(textbox).parent().parent().removeClass('has-danger');
        $(textbox).parent().parent().addClass('has-success');
        $(icon).text("done");
    }
    $('#submit').prop('disabled', $('form > .has-danger').length !== 0);
}

function HasError(textbox){
    var errorText = '';

    if (textbox.value == '' && $(textbox).data().valRequired) {
        errorText = $(textbox).data().valRequired;
    }
    else if (textbox.validity.patternMismatch){
        errorText = $(textbox).data().valRegex;
    }else if(textbox.validity.tooShort){
        errorText = $(textbox).data().valLength;
    }else if($(textbox).is('[type="email"]') && textbox.validity.typeMismatch){
        errorText = $(textbox).data().valEmail;
    }else if(($(textbox).attr('id') === "ConfirmPassword" || $(textbox).attr('id') === "Password") && $("#Password").val() !== $(textbox).val()){
        errorText = 'Parolele nu sunt identice';
    }
    return errorText;
}
$("#accepted-cookie").on("click", function(event){
    event.preventDefault();
    $("#cookie-nav").hide();
    $("#sectionsNav").removeClass('fixed-top-with-cookie');
    var expiration_date = new Date();
    var cookie_string = '';
    expiration_date.setFullYear(expiration_date.getFullYear() + 1);
    document.cookie = 'acceptedCookies=true; path=/; expires=' + expiration_date.toUTCString();
});


$("#Terms").click(function(){
    var err = '';
    if(!$("#Terms").prop('checked')){
      err = '@Localizer["In order to get register you need to agree with terms and conditions"]';
    }
    $("#Terms").parent().siblings('.error').children('.text-danger').text(err);
    $('#submit').prop('disabled', err !== '' || $('form > .has-danger').length !== 0);
 });