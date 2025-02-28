interface AjaxHeaders {
    [key: string]: string;
}

interface AjaxResponse {
    status: number;
    statusText: string;
    ok: boolean;
    headers: AjaxHeaders;
    json: () => Promise<any>;
    text: () => Promise<string>;
    blob: () => Promise<Blob>;
    arrayBuffer: () => Promise<ArrayBuffer>;
    formData: () => Promise<FormData>;
}

interface FetchOptions {
    method?: "GET" | "POST" | "PUT" | "DELETE" | "PATCH" | "HEAD" | "OPTIONS" | "TRACE" | "CONNECT";
    headers?: HeadersInit;
    body?: any;
    responseType?: "json" | "text" | "blob" | "arrayBuffer" | "formData";
    timeout?: number;
    signal?: AbortSignal;
    credentials?: "omit" | "same-origin" | "include";
}

declare function bytesToBase64DataUrl(
    data: Uint8Array,
    type: string
): Promise<string | ArrayBuffer | null>;
declare function ajaxFetch(url: string, options: FetchOptions): Promise<AjaxResponse>;
declare function showToast(
    message: string,
    description: string | undefined,
    type: "info" | "success" | "warning" | "error" | undefined
): void;
