$(function(){
    if($("#WithPersonalReading").is(":checked")){
        $("#displayName").show();
    }
    else
    {
        $("#setDefaultValue").show();
    }
})

 $("#WithPersonalReading").change(function(){
    $("#displayName").toggle();
    $("#setDefaultValue").toggle();
 });