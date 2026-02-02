import { ContactsApi } from "./api.js";
import type { Contact, ContactId } from "../shared/types.js";
import { PhoneInput } from "./PhoneInput.js";
import { createGlobalErrorBanner, extractErrorMessage } from "../shared/errors.js";
import { configureAntiforgeryForAjax } from "../shared/antiforgery.js";

type Field = "name" | "email" | "phone";
type FieldErrors = Partial<Record<Field, string>>;

$(document).ready(() => {
    configureAntiforgeryForAjax();
    const $tbody = $("#contactsTbody");

    // Search is a normal form so Enter works without extra key handlers
    const $searchForm = $("#searchForm");
    const $searchBox = $("#searchBox");

    const $addBtn = $("#addNewBtn");
    const $modalEl = $("#contactModal");
    const modal = new (window as any).bootstrap.Modal($modalEl[0], { backdrop: true, keyboard: true });

    // One modal form handles both create + update
    const $contactForm = $("#contactForm");

    const $id = $("#contactId");
    const $save = $("#saveBtn");

    // Single place to show server / network errors
    const banner = createGlobalErrorBanner();

    const fields = {
        name: $("#name"),
        email: $("#email"),
    } as const;

    // Manages the 3-box phone input and gives back a clean formatted value
    const phone = new PhoneInput($modalEl[0] as HTMLElement);

    let currentQuery = "";
    let refreshToken = 0;

    function getVal($el: JQuery<HTMLElement>): string {
        return String($el.val() ?? "").trim();
    }

    function clearFieldError(field: Field): void {
        if (field === "phone") {
            $("#phoneError").text("");
            $("#phoneA, #phoneB, #phoneC").removeClass("is-invalid").attr("aria-invalid", "false");
            return;
        }
        const $input = fields[field];
        const $error = $("#" + field + "Error");
        $input.removeClass("is-invalid").attr("aria-invalid", "false");
        $error.text("");
    }

    function setFieldError(field: Field, message: string): void {
        if (field === "phone") {
            $("#phoneError").text(message);
            $("#phoneA, #phoneB, #phoneC").addClass("is-invalid").attr("aria-invalid", "true");
            return;
        }
        const $input = fields[field];
        const $error = $("#" + field + "Error");
        $input.addClass("is-invalid").attr("aria-invalid", "true");
        $error.text(message);
    }

    function clearValidation(): void {
        (["name", "email", "phone"] as Field[]).forEach(clearFieldError);
    }

    function applyErrors(errors: FieldErrors): void {
        clearValidation();
        (Object.keys(errors) as Field[]).forEach((k) => {
            const msg = errors[k];
            if (msg) setFieldError(k, msg);
        });
    }

    function readForm(): Contact & { id?: string } {
        return {
            id: getVal($id) || undefined,
            name: getVal(fields.name),
            email: getVal(fields.email),
            phone: phone.value, // "(123)-456-7890"
        };
    }

    function setForm(c: { id?: string; name: string; email: string; phone: string }): void {
        $id.val(c.id ?? "");
        fields.name.val(c.name ?? "");
        fields.email.val(c.email ?? "");
        phone.value = c.phone ?? "";
        clearValidation();
    }

    function clearForm(): void {
        setForm({ id: "", name: "", email: "", phone: "" });
    }

    function validate(contact: Contact): FieldErrors {
        const errors: FieldErrors = {};

        const name = (contact.name ?? "").trim();
        const email = (contact.email ?? "").trim();
        const digits = phone.rawDigits; // easier than parsing "(123)-..." back into digits

        if (!name) errors.name = "Required";
        if (!email) errors.email = "Required";

        if (digits.length === 0) {
            errors.phone = "Required";
        } else if (digits.length !== 10) {
            errors.phone = "Invalid format (123)-456-7890";
        }

        if (email) {
            const emailOk = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/.test(email);
            if (!emailOk) errors.email = "Invalid entry";
        }

        return errors;
    }

    async function refresh(): Promise<void> {
        const token = ++refreshToken;
        const q = currentQuery.trim();

        try {
            const html = q ? await ContactsApi.searchHtml(q) : await ContactsApi.listHtml();

            // Prevent older requests from overwriting newer results
            if (token !== refreshToken) return;

            $tbody.html(html);
            banner.hide();
        } catch (err) {
            if (token !== refreshToken) return;
            console.error(err);
            banner.show(extractErrorMessage(err));
        }
    }

    async function save(): Promise<void> {
        const form = readForm();
        const clientErrors = validate(form);

        // Don’t hit the server if the form is obviously invalid
        if (Object.keys(clientErrors).length) {
            applyErrors(clientErrors);

            if (clientErrors.name) fields.name.trigger("focus");
            else if (clientErrors.email) fields.email.trigger("focus");
            else phone.focus();

            return;
        }

        $save.prop("disabled", true);
        banner.hide();

        try {
            const payload = { name: form.name, email: form.email, phone: form.phone };

            if (form.id) {
                await ContactsApi.update(form.id, payload);
            } else {
                await ContactsApi.create(payload);
            }

            modal.hide();
            await refresh();
        } catch (err) {
            console.error(err);
            banner.show(extractErrorMessage(err));
            modal.hide();
        } finally {
            $save.prop("disabled", false);
        }
    }

    async function removeContact(id: ContactId): Promise<void> {
        banner.hide();
        try {
            await ContactsApi.remove(id);
            await refresh();
        } catch (err) {
            console.error(err);
            banner.show(extractErrorMessage(err));
        }
    }

    async function openForEdit(id: ContactId): Promise<void> {
        banner.hide();
        try {
            const c = await ContactsApi.getById(id);
            setForm({
                id: c.id ?? "",
                name: c.name ?? "",
                email: c.email ?? "",
                phone: c.phone ?? "",
            });
            modal.show();
        } catch (err) {
            console.error(err);
            banner.show(extractErrorMessage(err));
        }
    }

    // Initial load
    refresh().catch(console.error);

    $searchForm.on("submit", async (e) => {
        e.preventDefault();
        currentQuery = getVal($searchBox);
        await refresh();
    });

    $addBtn.on("click", () => {
        banner.hide();
        clearForm();
        modal.show();
    });

    $contactForm.on("submit", (e) => {
        e.preventDefault();
        save().catch(console.error);
    });

    // As soon as the user types, remove the inline error for that field
    (["name", "email"] as Array<keyof typeof fields>).forEach((f) => {
        fields[f].on("input", () => clearFieldError(f as Field));
    });

    $modalEl.on("input", "#phoneA, #phoneB, #phoneC", () => clearFieldError("phone"));

    // Double-click a row to edit (shortcut)
    $tbody.on("dblclick", "tr[data-id]", function () {
        const id = String($(this).data("id") || "").trim();
        if (id) openForEdit(id as ContactId);
    });

    // Delegated handlers because tbody gets re-rendered
    $tbody.on("click", ".editBtn", function () {
        const id = String($(this).data("id") || "").trim();
        if (id) openForEdit(id as ContactId);
    });

    $tbody.on("click", ".deleteBtn", function () {
        const id = String($(this).data("id") || "").trim();
        if (id) removeContact(id as ContactId);
    });

    $modalEl.on("shown.bs.modal", () => fields.name.trigger("focus"));

    // Remove focus from any element inside the modal when closing
    $modalEl.on("hide.bs.modal", () => {
      const active = document.activeElement as HTMLElement | null;
      if (active && $modalEl[0].contains(active)) active.blur();
    });

    // Always reset when closing so you don't reopen with old data/errors
    $modalEl.on("hidden.bs.modal", () => clearForm());
});
