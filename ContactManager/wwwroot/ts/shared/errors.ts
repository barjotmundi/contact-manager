export function extractErrorMessage(err: any): string {
    // Most of our API errors come back in responseJSON, check the common fields first.
    const rj = err?.responseJSON;

    if (rj?.title && typeof rj.title === "string") return rj.title;
    if (rj?.message && typeof rj.message === "string") return rj.message;

    // Sometimes responseJSON itself is just a plain string.
    if (typeof rj === "string" && rj.trim()) return rj;

    const rt = err?.responseText;
    if (typeof rt === "string" && rt.trim()) {
        // Some endpoints send JSON as raw text, try to parse it, otherwise just show the text.
        try {
            const parsed = JSON.parse(rt);
            if (parsed?.message) return String(parsed.message);
            if (parsed?.title) return String(parsed.title);
        } catch {
            return rt;
        }
    }

    // jQuery uses status=0 for "request never completed" (offline, blocked, CORS, etc.).
    if (err?.status === 0) return "Network error. Please check your connection and try again.";
    if (err?.statusText && typeof err.statusText === "string" && err.statusText.trim()) return err.statusText;
    if (err instanceof Error && err.message) return err.message;

    return "Something went wrong. Please try again.";
}

export function createGlobalErrorBanner() {
    // Cache DOM lookups so we don't keep querying the page on every show/hide.
    const $globalError = $("#globalError");
    const $globalErrorMsg = $("#globalErrorMsg");
    const $globalErrorClose = $("#globalErrorClose");

    function show(message: string): void {
        // Never show an empty banner—fallback to a generic message.
        const msg = (message || "").trim() || "Unable to perform action. Please try again.";
        $globalErrorMsg.text(msg);
        $globalError.removeClass("d-none");

        // If the banner is off-screen, nudge the page so the user actually sees it.
        const el = $globalError[0];
        el?.scrollIntoView?.({ behavior: "smooth", block: "nearest" });
    }

    function hide(): void {
        $globalError.addClass("d-none");
        $globalErrorMsg.text("");
    }

    // Simple close button wiring.
    $globalErrorClose.on("click", hide);

    return { show, hide };
}
