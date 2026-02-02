type ConfirmOptions = {
    title?: string;
    message: string;
    confirmText?: string;
    confirmBtnClass?: string; // e.g., "btn-danger", "btn-primary"
};

export function createConfirm(): (opts: ConfirmOptions, onConfirm: () => void | Promise<void>) => void {
    const $confirmEl = $("#confirmModal");
    const confirmModal =
        $confirmEl.length > 0
            ? new (window as any).bootstrap.Modal($confirmEl[0], { backdrop: "static", keyboard: true })
            : null;

    const $confirmTitle = $("#confirmModalLabel");
    const $confirmBody = $("#confirmModalBody");
    const $confirmOk = $("#confirmModalOk");

    // Avoid the aria-hidden focus warning on close
    $confirmEl.on("hide.bs.modal", () => {
        const active = document.activeElement as HTMLElement | null;
        if (active && $confirmEl[0].contains(active)) active.blur();
    });

    return function showConfirm(opts: ConfirmOptions, onConfirm: () => void | Promise<void>): void {
        if (!confirmModal || $confirmEl.length === 0) {
            const ok = window.confirm(opts.message);
            if (ok) void onConfirm();
            return;
        }

        $confirmTitle.text(opts.title ?? "Confirm");
        $confirmBody.text(opts.message);

        $confirmOk.text(opts.confirmText ?? "Confirm");

        const btnClass = opts.confirmBtnClass ?? "btn-danger";
        $confirmOk.attr("class", `btn ${btnClass}`);

        $confirmOk.off("click").on("click", async () => {
            $confirmOk.prop("disabled", true);
            try {
                confirmModal.hide();
                await onConfirm();
            } finally {
                $confirmOk.prop("disabled", false);
            }
        });

        confirmModal.show();
    };
}
