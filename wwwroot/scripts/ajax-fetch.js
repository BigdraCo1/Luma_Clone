/**
 * @typedef {object} FetchOptions
 * @property {'GET' | 'POST' | 'PUT' | 'DELETE' | 'PATCH' | 'HEAD' | 'OPTIONS' | 'TRACE' | 'CONNECT'} [method='GET'] - The HTTP method.
 * @property {HeadersInit} [headers] - Request headers.
 * @property {*} [body] - The request body. Can be a string, FormData, Blob, ArrayBuffer, URLSearchParams, JavaScript object (for JSON), etc.
 * @property {'json' | 'text' | 'blob' | 'arrayBuffer' | 'formData'} [responseType='json'] - Expected response type.
 * @property {number} [timeout=0] - Request timeout in milliseconds. 0 means no timeout.
 * @property {AbortSignal} [signal] - AbortSignal to cancel the request.
 * @property {'omit' | 'same-origin' | 'include'} [credentials] - Controls whether credentials (cookies, auth headers) are sent. Defaults to 'omit'.
 */

/**
 * @class AjaxHeaders
 * @implements {Headers}
 * A simplified Headers class to mimic fetch Headers API using plain object internally.
 */
class AjaxHeaders {
    /**
     * @private
     * @type {Record<string, string>}
     */
    _headers = {};

    /**
     * @private
     * @type {string[]}
     */
    _setCookieHeaders = [];

    /**
     * Creates a new Headers object.
     * @param {HeadersInit} [init] - Initial headers.
     * @param {string[]} [setCookieHeaders] - Initial Set-Cookie headers (internal use).
     */
    constructor(init, setCookieHeaders = []) {
        this._setCookieHeaders = [...setCookieHeaders];

        if (init instanceof AjaxHeaders) {
            this._headers = { ...init._headers };
            this._setCookieHeaders = [...init._setCookieHeaders];
        } else if (init instanceof Headers) {
            init.forEach((value, key) => {
                if (key.toLowerCase() === "set-cookie") {
                    this._setCookieHeaders.push(value);
                } else {
                    this._headers[key.toLowerCase()] = value;
                }
            });
        } else if (init && typeof init === "object") {
            for (const key in init) {
                if (Object.hasOwnProperty.call(init, key)) {
                    this._headers[key.toLowerCase()] = String(init[key]);
                }
            }
        }
    }

    /**
     * Adds a new value to the headers.
     * @param {string} name - Header name.
     * @param {string} value - Header value.
     */
    append(name, value) {
        const lowerName = name.toLowerCase();
        if (lowerName === "set-cookie") {
            this._setCookieHeaders.push(value);
        } else if (this._headers[lowerName]) {
            this._headers[lowerName] += ", " + value;
        } else {
            this._headers[lowerName] = value;
        }
    }

    /**
     * Deletes a header.
     * @param {string} name - Header name.
     */
    delete(name) {
        if (name.toLowerCase() === "set-cookie") {
            this._setCookieHeaders = [];
        } else {
            delete this._headers[name.toLowerCase()];
        }
    }

    /**
     * Returns the value of a header.
     * @param {string} name - Header name.
     * @returns {string | null} - Header value or null if not found.
     */
    get(name) {
        if (name.toLowerCase() === "set-cookie") {
            return this._setCookieHeaders.join(", ") || null;
        }
        return this._headers[name.toLowerCase()] || null;
    }

    /**
     * Returns the values of all 'Set-Cookie' headers.
     * @returns {string[]} An array of strings, where each string is a 'Set-Cookie' header value.
     */
    getSetCookie() {
        return [...this._setCookieHeaders];
    }

    /**
     * Checks if a header exists.
     * @param {string} name - Header name.
     * @returns {boolean} - True if header exists, false otherwise.
     */
    has(name) {
        if (name.toLowerCase() === "set-cookie") {
            return this._setCookieHeaders.length > 0;
        }
        return !!this._headers[name.toLowerCase()];
    }

    /**
     * Sets a header value.
     * @param {string} name - Header name.
     * @param {string} value - Header value.
     */
    set(name, value) {
        const lowerName = name.toLowerCase();
        if (lowerName === "set-cookie") {
            this._setCookieHeaders = [value];
        } else {
            this._headers[lowerName] = value;
        }
    }

    /**
     * Iterates over the headers.
     * @param {(value: string, key: string, parent: Headers) => void} callbackfn - Callback function for each header.
     * @param {any} [thisArg] - `this` context for the callback.
     */
    forEach(callbackfn, thisArg) {
        for (const key in this._headers) {
            if (Object.hasOwnProperty.call(this._headers, key)) {
                callbackfn.call(thisArg, this._headers[key], key, this);
            }
        }
        this._setCookieHeaders.forEach((cookie) => {
            callbackfn.call(thisArg, cookie, "set-cookie", this);
        });
    }

    /**
     * Returns an iterator for header entries.
     * @returns {IterableIterator<[string, string]>}
     */
    *entries() {
        for (const key in this._headers) {
            if (Object.hasOwnProperty.call(this._headers, key)) {
                yield [key, this._headers[key]];
            }
        }
        for (const cookie of this._setCookieHeaders) {
            yield ["set-cookie", cookie];
        }
    }

    /**
     * Returns an iterator for header keys.
     * @returns {IterableIterator<string>}
     */
    *keys() {
        for (const key in this._headers) {
            if (Object.hasOwnProperty.call(this._headers, key)) {
                yield key;
            }
        }
        if (this._setCookieHeaders.length > 0) {
            yield "set-cookie";
        }
    }

    /**
     * Returns an iterator for header values.
     * @returns {IterableIterator<string>}
     */
    *values() {
        for (const key in this._headers) {
            if (Object.hasOwnProperty.call(this._headers, key)) {
                yield this._headers[key];
            }
        }
        for (const cookie of this._setCookieHeaders) {
            yield cookie;
        }
    }

    /**
     * Returns an iterator for header entries (same as entries()).
     * @returns {IterableIterator<[string, string]>}
     */
    [Symbol.iterator]() {
        return this.entries();
    }

    /**
     * Returns a plain object representation of the headers.
     * @returns {Record<string, string>}
     */
    toJSON() {
        const jsonHeaders = { ...this._headers };
        if (this._setCookieHeaders.length > 0) {
            // @ts-expect-error
            jsonHeaders["set-cookie"] = this.get("set-cookie");
        }
        return jsonHeaders;
    }
}

/**
 * @typedef {object} AjaxResponse
 * @property {number} status - The HTTP status code of the response.
 * @property {string} statusText - The status text returned by the server.
 * @property {boolean} ok - Boolean indicating whether the response was successful (status in the range 200-299).
 * @property {AjaxHeaders} headers - The response headers.
 * @property {() => Promise<any>} json - Parses the response body as JSON.
 * @property {() => Promise<string>} text - Parses the response body as text.
 * @property {() => Promise<Blob>} blob - Parses the response body as a Blob.
 * @property {() => Promise<ArrayBuffer>} arrayBuffer - Parses the response body as an ArrayBuffer.
 * @property {() => Promise<FormData>} formData - Parses the response body as FormData.
 */

/**
 * Simulates the fetch API using XMLHttpRequest.
 * @param {string} url - The URL to fetch.
 * @param {FetchOptions} [options] - Optional fetch options.
 * @returns {Promise<AjaxResponse>} A Promise that resolves with an AjaxResponse object.
 */
function ajaxFetch(url, options = {}) {
    return new Promise((resolve, reject) => {
        const xhr = new XMLHttpRequest();
        const method = options.method || "GET";
        const headersInit = options.headers;
        let body = options.body;
        const responseType = options.responseType || "json";
        const timeout = options.timeout || 0;
        const signal = options.signal;
        const credentialsOption = options.credentials || "omit";

        xhr.open(method, url, true);

        if (credentialsOption === "include") {
            xhr.withCredentials = true;
        } else if (credentialsOption === "same-origin") {
            xhr.withCredentials = false;
        } else if (credentialsOption === "omit") {
            xhr.withCredentials = false;
        }

        const headers = new AjaxHeaders(headersInit);

        if (
            body &&
            typeof body === "object" &&
            !(body instanceof FormData) &&
            !(body instanceof Blob) &&
            !(body instanceof ArrayBuffer) &&
            !(body instanceof URLSearchParams)
        ) {
            if (!headers.has("Content-Type")) {
                headers.set("Content-Type", "application/json");
            }
            if (headers.get("Content-Type")?.toLowerCase().startsWith("application/json")) {
                body = JSON.stringify(body);
            }
        }

        headers.forEach((value, name) => {
            xhr.setRequestHeader(name, value);
        });

        if (timeout > 0) {
            xhr.timeout = timeout;
            xhr.ontimeout = () => {
                reject(new Error("Request timed out"));
            };
        }

        if (signal) {
            signal.addEventListener("abort", () => {
                xhr.abort();
                reject(new Error("Request aborted by signal"));
            });
        }

        xhr.onload = () => {
            const { parsedHeaders, setCookieHeaders } = parseResponseHeaders(
                xhr.getAllResponseHeaders()
            );
            const response = {
                status: xhr.status,
                statusText: xhr.statusText,
                ok: xhr.status >= 200 && xhr.status < 300,
                headers: new AjaxHeaders(parsedHeaders, setCookieHeaders),
                json: () => parseResponseBody(xhr, responseType, "json", xhr),
                text: () => parseResponseBody(xhr, responseType, "text", xhr),
                blob: () => parseResponseBody(xhr, responseType, "blob", xhr),
                arrayBuffer: () => parseResponseBody(xhr, responseType, "arrayBuffer", xhr),
                formData: () => parseResponseBody(xhr, responseType, "formData", xhr)
            };

            if (response.ok) {
                resolve(response);
            } else {
                reject(
                    new AjaxFetchError(
                        `HTTP error! status: ${response.status} at URL: ${url}`,
                        response,
                        url
                    )
                );
            }
        };

        xhr.onerror = () => {
            reject(new Error(`Network error at URL: ${url}`));
        };

        xhr.onabort = () => {
            reject(new Error(`Request aborted at URL: ${url}`));
        };

        if (body !== undefined) {
            xhr.send(body);
        } else {
            xhr.send();
        }
    });
}

/**
 * @private
 * Parses response headers string into a Headers-like object and extracts Set-Cookie headers.
 * @param {string} headersString - Raw response headers string.
 * @returns {{parsedHeaders: Record<string, string>, setCookieHeaders: string[]}} - Parsed headers object and Set-Cookie headers.
 */
// ... (parseResponseHeaders function - no changes needed) ...
function parseResponseHeaders(headersString) {
    /** @type {Record<string, string>} */
    const parsedHeaders = {};
    /** @type {string[]} */
    const setCookieHeaders = [];
    if (!headersString) {
        return { parsedHeaders, setCookieHeaders };
    }
    const headerPairs = headersString.trim().split("\r\n");
    for (const pair of headerPairs) {
        const index = pair.indexOf(": ");
        if (index > 0) {
            const key = pair.substring(0, index); // Do not lowercase key here, needed for Set-Cookie check
            const value = pair.substring(index + 2);
            if (key.toLowerCase() === "set-cookie") {
                setCookieHeaders.push(value);
            } else {
                parsedHeaders[key.toLowerCase()] = value; // Lowercase key for other headers
            }
        }
    }
    return { parsedHeaders, setCookieHeaders };
}

/**
 * @private
 * Parses the response body based on the specified response type.
 * @param {XMLHttpRequest} xhr - The XMLHttpRequest object.
 * @param {'json' | 'text' | 'blob' | 'arrayBuffer' | 'formData'} responseType - The expected response type.
 * @param {string} method - The parsing method to use ('json', 'text', etc.).
 * @param {XMLHttpRequest} originalXhr - The original XMLHttpRequest object for context in errors.
 * @returns {Promise<any>} - A Promise that resolves with the parsed response body.
 */
async function parseResponseBody(xhr, responseType, method, originalXhr) {
    return new Promise((resolve, reject) => {
        if (xhr.status === 204 || xhr.status === 205) {
            // No Content responses
            resolve(null);
            return;
        }

        const contentType = xhr.getResponseHeader("Content-Type");

        if (method === "json" || responseType === "json") {
            if (contentType && contentType.toLowerCase().startsWith("application/json")) {
                // More robust Content-Type check
                try {
                    resolve(JSON.parse(xhr.responseText));
                } catch (e) {
                    reject(new Error("Failed to parse JSON response body", { cause: e })); // Include cause for better error info
                }
            } else if (method === "json") {
                reject(
                    new Error(
                        `Expected JSON response, but Content-Type is not application/json, but "${contentType}". URL: ${originalXhr.responseURL}`
                    )
                ); // More informative error
            } else {
                // if responseType is json but no json content type, try to parse anyway
                try {
                    resolve(JSON.parse(xhr.responseText));
                } catch (e) {
                    resolve(xhr.responseText); // if parse fails, resolve as text as fallback for responseType = 'json'
                }
            }
        } else if (method === "text" || responseType === "text") {
            resolve(xhr.responseText);
        } else if (method === "blob" || responseType === "blob") {
            if (xhr.responseType === "blob") {
                resolve(xhr.response);
            } else {
                reject(
                    new Error(
                        `XMLHttpRequest responseType must be "blob" for blob response, but is "${xhr.responseType}". URL: ${originalXhr.responseURL}`
                    )
                ); // More informative error
            }
        } else if (method === "arrayBuffer" || responseType === "arrayBuffer") {
            if (xhr.responseType === "arraybuffer") {
                resolve(xhr.response);
            } else {
                reject(
                    new Error(
                        `XMLHttpRequest responseType must be "arraybuffer" for arrayBuffer response, but is "${xhr.responseType}". URL: ${originalXhr.responseURL}`
                    )
                ); // More informative error
            }
        } else if (method === "formData" || responseType === "formData") {
            try {
                const formData = parseTextToFormData(xhr.responseText); // Simple FormData parsing for text response
                resolve(formData);
            } catch (e) {
                reject(new Error("Failed to parse FormData from text response", { cause: e })); // Include cause
            }
        } else {
            resolve(xhr.responseText); // Default to text if responseType is unknown
        }
    });
}

/**
 * @private
 * Simple parser to convert text response to FormData (very basic, for demonstration).
 * Not robust for all FormData formats.
 * @param {string} text - Text response body.
 * @returns {FormData} - FormData object.
 */
// ... (parseTextToFormData function - no changes needed, but remember its limitations) ...
function parseTextToFormData(text) {
    const formData = new FormData();
    const lines = text.split("\n"); // Assumes each line is a key=value pair
    for (const line of lines) {
        const parts = line.split("=");
        if (parts.length === 2) {
            formData.append(parts[0].trim(), parts[1].trim());
        }
    }
    return formData;
}

/**
 * @class AjaxFetchError
 * Custom error class for ajaxFetch, including the response object.
 * @extends {Error}
 */
class AjaxFetchError extends Error {
    /**
     * @param {string} message - Error message.
     * @param {AjaxResponse} response - The AjaxResponse object related to the error.
     * @param {string} url - The URL that was being fetched when the error occurred.
     */
    constructor(message, response, url) {
        super(message);
        this.name = "AjaxFetchError";
        /** @type {AjaxResponse} */
        this.response = response;
        /** @type {string} */
        this.url = url; // Store the URL
        // Maintains proper stack trace for where our error was thrown (only works in V8)
        // @ts-expect-error
        if (typeof Error.captureStackTrace === "function") {
            // Check if it's a function first
            // @ts-expect-error
            Error.captureStackTrace(this, AjaxFetchError);
        }
    }
}

// TODO: Remove
// // Example usage:
// async function testAjaxFetch() {
//     try {
//         const response = await ajaxFetch("https://jsonplaceholder.typicode.com/todos/1", {
//             method: "GET",
//             headers: {
//                 Accept: "application/json",
//                 "X-Custom-Header": "ajax-fetch-demo"
//             },
//             timeout: 5000,
//             responseType: "json", // explicitly set for clarity, though 'json' is default
//             credentials: "include" // Example credentials option
//         });

//         console.log("Status:", response.status);
//         console.log("Status Text:", response.statusText);
//         console.log("OK:", response.ok);
//         console.log("Headers:", response.headers.toJSON());
//         console.log("Set-Cookie Headers:", response.headers.getSetCookie()); // Log Set-Cookie headers
//         const data = await response.json();
//         console.log("Data:", data);

//         const textResponse = await ajaxFetch("https://httpbin.org/get", { responseType: "text" });
//         console.log("Text Response:", await textResponse.text());

//         const formDataResponse = await ajaxFetch("https://httpbin.org/post", {
//             method: "POST",
//             body: new URLSearchParams({ name: "John", age: "30" }), // Example POST body as URLSearchParams
//             headers: { "Content-Type": "application/x-www-form-urlencoded" },
//             responseType: "formData"
//         });
//         const formData = await formDataResponse.formData();
//         console.log("FormData Response:");
//         formData.forEach((value, key) => {
//             console.log(`${key}: ${value}`);
//         });

//         const jsonPostResponse = await ajaxFetch("https://httpbin.org/post", {
//             method: "POST",
//             body: { message: "Hello from Ajax Fetch", data: [1, 2, 3] }, // Example JSON POST body (object)
//             headers: { "Content-Type": "application/json" }, // Explicitly set Content-Type for JSON
//             responseType: "json"
//         });
//         console.log("JSON Post Response:", await jsonPostResponse.json());

//         const errorResponse = await ajaxFetch("https://httpbin.org/status/404");
//         // This line will likely not be reached because of the error
//     } catch (error) {
//         if (error instanceof AjaxFetchError) {
//             console.error("Fetch error:", error.message);
//             console.error("URL:", error.url); // Log the URL from the error
//             console.error("Response Status:", error.response.status);
//             console.error("Response Headers:", error.response.headers.toJSON());
//             // Optionally handle response body from error.response here if needed.
//         } else {
//             console.error("General error:", error.message);
//         }
//     }
// }

// testAjaxFetch();
