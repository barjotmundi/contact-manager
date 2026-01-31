/// <reference types="jquery" />
export function getRequestVerificationToken() {
    // Razor renders this input from @Html.AntiForgeryToken()
    const token = $('input[name="__RequestVerificationToken"]').val();
    return typeof token === "string" ? token : "";
}
export function configureAntiforgeryForAjax() {
    $.ajaxSetup({
        beforeSend: (xhr, settings) => {
            const method = (settings.type || "GET").toUpperCase();
            // Only attach token to unsafe verbs
            if (method === "POST" || method === "PUT" || method === "DELETE" || method === "PATCH") {
                const token = getRequestVerificationToken();
                if (token) {
                    // ASP.NET Core will accept this header name
                    xhr.setRequestHeader("RequestVerificationToken", token);
                }
            }
        }
    });
}
//# sourceMappingURL=antiforgery.js.map