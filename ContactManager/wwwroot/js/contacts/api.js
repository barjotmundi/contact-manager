/// <reference types="jquery" />
export const ContactsApi = {
    async listHtml() {
        return await $.get("/contacts/list");
    },
    async searchHtml(query) {
        return await $.get("/contacts/search", { query });
    },
    async getById(id) {
        return await $.get(`/api/contacts/${encodeURIComponent(id)}`);
    },
    async create(contact) {
        return await $.ajax({
            url: "/api/contacts",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(contact),
        });
    },
    async update(id, contact) {
        return await $.ajax({
            url: `/api/contacts/${encodeURIComponent(id)}`,
            type: "PUT",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(contact),
        });
    },
    async remove(id) {
        return await $.ajax({
            url: `/api/contacts/${encodeURIComponent(id)}`,
            type: "DELETE"
        });
    },
};
//# sourceMappingURL=api.js.map