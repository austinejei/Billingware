(function () {
    $(function () {

        var basicAuthUI = '<div class="input">' +
            '<input placeholder="e.g. ApiKey 1234rtrsf" id="basic_auth_value" name="basic_auth_value" type="text" style="width:70px;"/>' +
            '</div>';
        $(basicAuthUI).insertBefore('#api_selector div.input:last-child');
        $("#input_apiKey").parent().hide();

        $('#basic_auth_value').change(addAuthorization);


    });

    function addAuthorization() {
        var basicAuthValue = $('#basic_auth_value').val();

        if (basicAuthValue && basicAuthValue.trim() !== "") {
            swaggerUi.api.clientAuthorizations.add("key", new SwaggerClient.ApiKeyAuthorization("Authorization", basicAuthValue, "header"));
        }
    }

})();