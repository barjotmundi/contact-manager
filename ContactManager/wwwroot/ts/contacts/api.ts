/// <reference types="jquery" />

import type { Contact, ContactId } from "../shared/types.js";

export const ContactsApi = {
    async listHtml(): Promise<string> {
        return await $.get("/contacts/list");
    },

    async searchHtml(query: string): Promise<string> {
        return await $.get("/contacts/search", { query });
    },

    async getById(id: ContactId): Promise<Contact> {
        return await $.get(`/contacts/${encodeURIComponent(id)}`);
    },

    async create(contact: Contact): Promise<any> {
        return await $.ajax({
            url: "/contacts",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(contact),
        });
    },

    async update(id: ContactId, contact: Contact): Promise<any> {
        return await $.ajax({
            url: `/contacts/${encodeURIComponent(id)}`,
            type: "PUT",
            contentType: "application/json",
            dataType: "json",
            data: JSON.stringify(contact),
        });
    },

    async remove(id: ContactId): Promise<any> {
        return await $.ajax({
            url: `/contacts/${encodeURIComponent(id)}`,
            type: "DELETE"
        });
    },
};
