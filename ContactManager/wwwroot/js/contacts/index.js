import { ContactsApi } from "./api.js";
import { PhoneInput } from "./PhoneInput.js";
import { createGlobalErrorBanner, extractErrorMessage } from "../shared/errors.js";
import { configureAntiforgeryForAjax } from "../shared/antiforgery.js";
$(document).ready(() => {
    configureAntiforgeryForAjax();
    const $tbody = $("#contactsTbody");
    // Search is a normal form so Enter works without extra key handlers
    const $searchForm = $("#searchForm");
    const $searchBox = $("#searchBox");
    const $addBtn = $("#addNewBtn");
    const $modalEl = $("#contactModal");
    const modal = new window.bootstrap.Modal($modalEl[0], { backdrop: true, keyboard: true });
    // One modal form handles both create + update
    const $contactForm = $("#contactForm");
    const $id = $("#contactId");
    const $save = $("#saveBtn");
    // Single place to show server / network errors
    const banner = createGlobalErrorBanner();
    const fields = {
        name: $("#name"),
        email: $("#email"),
    };
    // Manages the 3-box phone input and gives back a clean formatted value
    const phone = new PhoneInput($modalEl[0]);
    let currentQuery = "";
    let refreshToken = 0;
    function getVal($el) {
        var _a;
        return String((_a = $el.val()) !== null && _a !== void 0 ? _a : "").trim();
    }
    function clearFieldError(field) {
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
    function setFieldError(field, message) {
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
    function clearValidation() {
        ["name", "email", "phone"].forEach(clearFieldError);
    }
    function applyErrors(errors) {
        clearValidation();
        Object.keys(errors).forEach((k) => {
            const msg = errors[k];
            if (msg)
                setFieldError(k, msg);
        });
    }
    function readForm() {
        return {
            id: getVal($id) || undefined,
            name: getVal(fields.name),
            email: getVal(fields.email),
            phone: phone.value, // "(123)-456-7890"
        };
    }
    function setForm(c) {
        var _a, _b, _c, _d;
        $id.val((_a = c.id) !== null && _a !== void 0 ? _a : "");
        fields.name.val((_b = c.name) !== null && _b !== void 0 ? _b : "");
        fields.email.val((_c = c.email) !== null && _c !== void 0 ? _c : "");
        phone.value = (_d = c.phone) !== null && _d !== void 0 ? _d : "";
        clearValidation();
    }
    function clearForm() {
        setForm({ id: "", name: "", email: "", phone: "" });
    }
    function validate(contact) {
        var _a, _b;
        const errors = {};
        const name = ((_a = contact.name) !== null && _a !== void 0 ? _a : "").trim();
        const email = ((_b = contact.email) !== null && _b !== void 0 ? _b : "").trim();
        const digits = phone.rawDigits; // easier than parsing "(123)-..." back into digits
        if (!name)
            errors.name = "Required";
        if (!email)
            errors.email = "Required";
        if (digits.length === 0) {
            errors.phone = "Required";
        }
        else if (digits.length !== 10) {
            errors.phone = "Invalid format (123)-456-7890";
        }
        if (email) {
            const emailOk = /^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/.test(email);
            if (!emailOk)
                errors.email = "Invalid entry";
        }
        return errors;
    }
    async function refresh() {
        const token = ++refreshToken;
        const q = currentQuery.trim();
        try {
            const html = q ? await ContactsApi.searchHtml(q) : await ContactsApi.listHtml();
            // Prevent older requests from overwriting newer results
            if (token !== refreshToken)
                return;
            $tbody.html(html);
            banner.hide();
        }
        catch (err) {
            if (token !== refreshToken)
                return;
            console.error(err);
            banner.show(extractErrorMessage(err));
        }
    }
    async function save() {
        const form = readForm();
        const clientErrors = validate(form);
        // Don’t hit the server if the form is obviously invalid
        if (Object.keys(clientErrors).length) {
            applyErrors(clientErrors);
            if (clientErrors.name)
                fields.name.trigger("focus");
            else if (clientErrors.email)
                fields.email.trigger("focus");
            else
                phone.focus();
            return;
        }
        $save.prop("disabled", true);
        banner.hide();
        try {
            const payload = { name: form.name, email: form.email, phone: form.phone };
            if (form.id) {
                await ContactsApi.update(form.id, payload);
            }
            else {
                await ContactsApi.create(payload);
            }
            modal.hide();
            await refresh();
        }
        catch (err) {
            console.error(err);
            banner.show(extractErrorMessage(err));
            modal.hide();
        }
        finally {
            $save.prop("disabled", false);
        }
    }
    async function removeContact(id) {
        banner.hide();
        try {
            await ContactsApi.remove(id);
            await refresh();
        }
        catch (err) {
            console.error(err);
            banner.show(extractErrorMessage(err));
        }
    }
    async function openForEdit(id) {
        var _a, _b, _c, _d;
        banner.hide();
        try {
            const c = await ContactsApi.getById(id);
            setForm({
                id: (_a = c.id) !== null && _a !== void 0 ? _a : "",
                name: (_b = c.name) !== null && _b !== void 0 ? _b : "",
                email: (_c = c.email) !== null && _c !== void 0 ? _c : "",
                phone: (_d = c.phone) !== null && _d !== void 0 ? _d : "",
            });
            modal.show();
        }
        catch (err) {
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
        fields.name.trigger("focus");
    });
    $contactForm.on("submit", (e) => {
        e.preventDefault();
        save().catch(console.error);
    });
    // As soon as the user types, remove the inline error for that field
    ["name", "email"].forEach((f) => {
        fields[f].on("input", () => clearFieldError(f));
    });
    $modalEl.on("input", "#phoneA, #phoneB, #phoneC", () => clearFieldError("phone"));
    // Double-click a row to edit (nice shortcut)
    $tbody.on("dblclick", "tr[data-id]", function () {
        const id = String($(this).data("id") || "").trim();
        if (id)
            openForEdit(id);
    });
    // Delegated handlers because tbody gets re-rendered
    $tbody.on("click", ".editBtn", function () {
        const id = String($(this).data("id") || "").trim();
        if (id)
            openForEdit(id);
    });
    $tbody.on("click", ".deleteBtn", function () {
        const id = String($(this).data("id") || "").trim();
        if (id)
            removeContact(id);
    });
    // Always reset when closing so you don't reopen with old data/errors
    $modalEl.on("hidden.bs.modal", () => clearForm());
});
//# sourceMappingURL=index.js.map