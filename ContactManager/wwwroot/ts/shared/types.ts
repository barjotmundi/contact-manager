export type ContactId = string;

export interface Contact {
    id?: ContactId;
    name: string;
    email: string;
    phone: string;
}

export type ApiResult =
    | { success: true; message?: string }
    | { success: false; message?: string; error?: string }
    | { ok: true; message?: string }
    | { ok: false; message?: string; error?: string }
    | { isSuccess: boolean; message?: string };
