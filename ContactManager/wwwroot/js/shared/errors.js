export function extractErrorMessage(err) {
    // Most of our API errors come back in responseJSON, check the common fields first.
    const rj = err === null || err === void 0 ? void 0 : err.responseJSON;
    if ((rj === null || rj === void 0 ? void 0 : rj.title) && typeof rj.title === "string")
        return rj.title;
    if ((rj === null || rj === void 0 ? void 0 : rj.message) && typeof rj.message === "string")
        return rj.message;
    // Sometimes responseJSON itself is just a plain string.
    if (typeof rj === "string" && rj.trim())
        return rj;
    const rt = err === null || err === void 0 ? void 0 : err.responseText;
    if (typeof rt === "string" && rt.trim()) {
        // Some endpoints send JSON as raw text, try to parse it, otherwise just show the text.
        try {
            const parsed = JSON.parse(rt);
            if (parsed === null || parsed === void 0 ? void 0 : parsed.message)
                return String(parsed.message);
            if (parsed === null || parsed === void 0 ? void 0 : parsed.title)
                return String(parsed.title);
        }
        catch (_a) {
            return rt;
        }
    }
    // jQuery uses status=0 for "request never completed" (offline, blocked, CORS, etc.).
    if ((err === null || err === void 0 ? void 0 : err.status) === 0)
        return "Network error. Please check your connection and try again.";
    if ((err === null || err === void 0 ? void 0 : err.statusText) && typeof err.statusText === "string" && err.statusText.trim())
        return err.statusText;
    if (err instanceof Error && err.message)
        return err.message;
    return "Something went wrong. Please try again.";
}
export function createGlobalErrorBanner() {
    // Cache DOM lookups so we don't keep querying the page on every show/hide.
    const $globalError = $("#globalError");
    const $globalErrorMsg = $("#globalErrorMsg");
    const $globalErrorClose = $("#globalErrorClose");
    function show(message) {
        var _a;
        // Never show an empty banner—fallback to a generic message.
        const msg = (message || "").trim() || "Unable to perform action. Please try again.";
        $globalErrorMsg.text(msg);
        $globalError.removeClass("d-none");
        // If the banner is off-screen, nudge the page so the user actually sees it.
        const el = $globalError[0];
        (_a = el === null || el === void 0 ? void 0 : el.scrollIntoView) === null || _a === void 0 ? void 0 : _a.call(el, { behavior: "smooth", block: "nearest" });
    }
    function hide() {
        $globalError.addClass("d-none");
        $globalErrorMsg.text("");
    }
    // Simple close button wiring.
    $globalErrorClose.on("click", hide);
    return { show, hide };
}
//# sourceMappingURL=errors.js.map