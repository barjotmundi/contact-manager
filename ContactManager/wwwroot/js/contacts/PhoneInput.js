export class PhoneInput {
    constructor(container) {
        this.container = container;
        const $root = $(container);
        this.$a = $root.find("#phoneA");
        this.$b = $root.find("#phoneB");
        this.$c = $root.find("#phoneC");
        this.init();
    }
    // Combined formatted value like "(123)-456-7890"
    get value() {
        return PhoneInput.format(this.digits());
    }
    set value(phone) {
        this.setDigits(phone);
    }
    // Digits only (0-10 chars) for validation / API payloads
    get rawDigits() {
        return this.digits();
    }
    clear() {
        this.setDigits("");
    }
    // Focus the first incomplete box so typing feels natural
    focus() {
        var _a, _b;
        if (((_a = this.$a.val()) === null || _a === void 0 ? void 0 : _a.length) !== 3)
            return void this.$a.trigger("focus");
        if (((_b = this.$b.val()) === null || _b === void 0 ? void 0 : _b.length) !== 3)
            return void this.$b.trigger("focus");
        this.$c.trigger("focus");
    }
    init() {
        // Auto-advance to next box once current segment is full
        this.$a.on("input", () => {
            const v = this.sanitize(this.$a, 3);
            if (v.length === 3)
                this.$b.trigger("focus");
        });
        this.$b.on("input", () => {
            const v = this.sanitize(this.$b, 3);
            if (v.length === 3)
                this.$c.trigger("focus");
        });
        this.$c.on("input", () => {
            this.sanitize(this.$c, 4);
        });
        // Backspace at start jumps to previous box (feels like one input)
        this.$b.on("keydown", (e) => {
            if (e.key === "Backspace" && !this.$b.val()) {
                e.preventDefault();
                this.$a.trigger("focus");
            }
        });
        this.$c.on("keydown", (e) => {
            if (e.key === "Backspace" && !this.$c.val()) {
                e.preventDefault();
                this.$b.trigger("focus");
            }
        });
        // Let users paste a full number into any box
        [
            ["a", this.$a],
            ["b", this.$b],
            ["c", this.$c],
        ].forEach(([slot, $el]) => {
            $el.on("paste", (e) => this.handlePaste(slot, e));
        });
    }
    digits() {
        return (PhoneInput.onlyDigits(this.$a.val()) +
            PhoneInput.onlyDigits(this.$b.val()) +
            PhoneInput.onlyDigits(this.$c.val())).slice(0, 10);
    }
    setDigits(phone) {
        const d = PhoneInput.onlyDigits(phone).slice(0, 10);
        this.$a.val(d.slice(0, 3));
        this.$b.val(d.slice(3, 6));
        this.$c.val(d.slice(6, 10));
    }
    sanitize($el, maxLen) {
        const clean = PhoneInput.onlyDigits($el.val()).slice(0, maxLen);
        $el.val(clean);
        return clean;
    }
    handlePaste(slot, e) {
        var _a, _b;
        const text = (_b = (_a = e.originalEvent.clipboardData) === null || _a === void 0 ? void 0 : _a.getData("text")) !== null && _b !== void 0 ? _b : "";
        const d = PhoneInput.onlyDigits(text);
        if (!d)
            return;
        e.preventDefault();
        // If they paste a full phone number, just fill everything
        if (d.length >= 10) {
            this.setDigits(d);
            this.$c.trigger("focus");
            return;
        }
        // Partial paste: insert starting at the current box
        const start = slot === "a" ? 0 : slot === "b" ? 3 : 6;
        const existing = this.digits().padEnd(10, "");
        this.setDigits(existing.slice(0, start) + d + existing.slice(start));
        this.focus();
    }
    static onlyDigits(val) {
        return String(val !== null && val !== void 0 ? val : "").replace(/\D/g, "");
    }
    static format(digits) {
        const d = PhoneInput.onlyDigits(digits).slice(0, 10);
        if (!d)
            return "";
        if (d.length <= 3)
            return `(${d}`;
        if (d.length <= 6)
            return `(${d.slice(0, 3)})-${d.slice(3)}`;
        return `(${d.slice(0, 3)})-${d.slice(3, 6)}-${d.slice(6)}`;
    }
}
//# sourceMappingURL=PhoneInput.js.map