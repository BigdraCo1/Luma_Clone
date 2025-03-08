///////////////////////////////////////
//             Languages             //
///////////////////////////////////////

/**
 * @type Record<string, string>
 */
const messages = {
    en: "Language set to English",
    th: "เปลี่ยนเป็นภาษาไทยเรียบร้อยแล้ว"
};

/**
 * Set the language and reload the page
 * @param {string} language The language to set
 */
function setLanguage(language) {
    document.cookie = `lang=${language};path=/;max-age=31536000;SameSite=Lax`;
    const url = new URL(window.location.href);
    url.searchParams.set("toast-message", messages[language]);
    url.searchParams.set("toast-type", "success");
    location.replace(url);
}

///////////////////////////////////////
//               Toasts              //
///////////////////////////////////////

const infoIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-info"><circle cx="12" cy="12" r="10"/><path d="M12 16v-4"/><path d="M12 8h.01"/></svg>`;
const successIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-circle-check"><circle cx="12" cy="12" r="10"/><path d="m9 12 2 2 4-4"/></svg>`;
const warningIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-triangle-alert"><path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3"/><path d="M12 9v4"/><path d="M12 17h.01"/></svg>`;
const errorIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-circle-x"><circle cx="12" cy="12" r="10"/><path d="m15 9-6 6"/><path d="m9 9 6 6"/></svg>`;
const closeIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="white" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-x"><path d="M18 6 6 18"/><path d="m6 6 12 12"/></svg>`;

let toastContainerInit = document.querySelector("#toast-container");

if (toastContainerInit === null) {
    toastContainerInit = document.createElement("div");
    toastContainerInit.id = "toast-container";
    document.body.appendChild(toastContainerInit);
}

const body = document.body;
const toastContainer = toastContainerInit;

/**
 * @type {HTMLElement[]}
 */
const toasts = [];

/**
 * @type Record<string, { start?: DOMRect, end?: DOMRect }>
 */
let rects = {};

/**
 * Start recording the initial position of the toasts
 */
function flipStart() {
    rects = {};
    for (const toast of toasts) {
        const rect = toast.getBoundingClientRect();
        if (!rects[toast.id]) {
            rects[toast.id] = {};
        }
        rects[toast.id].start = rect;
    }
}

/**
 * End recording the final position of the toasts
 */
function flipEnd() {
    for (const toast of toasts) {
        const rect = toast.getBoundingClientRect();
        if (!rects[toast.id]) {
            continue;
        }
        rects[toast.id].end = rect;
    }
}

/**
 * Play the flip animation
 */
function flipPlay() {
    for (const toast of toasts) {
        const rect = rects[toast.id];
        if (!rect || !rect.start || !rect.end) {
            continue;
        }
        const dx = rect.start.x - rect.end.x;
        const dy = rect.start.y - rect.end.y;
        toast.animate([{ transform: `translate(${dx}px, ${dy}px)` }, { transform: "none" }], {
            duration: 250,
            easing: "ease"
        });
    }
}

/**
 * Generate a random ID
 * @returns {string} Random ID
 */
function generateId() {
    return Math.random().toString(36).substring(2, 9);
}

/**
 * @this {HTMLDivElement}
 */
function toastOnMouseLeave() {
    this.classList.remove("removal-queued");
    this.removeEventListener("mouseleave", toastOnMouseLeave);
    setTimeout(() => {
        removeToast(this.id);
    }, 1000);
}

/**
 * Remove a toast message
 * @param {string} id ID of the toast to remove
 * @param {boolean} onclick Whether the toast was removed by clicking the close button
 */
function removeToast(id, onclick = false) {
    const toast = document.getElementById(id);
    if (!toast) {
        const toastIndex = toasts.findIndex((t) => t.id === id);
        if (toastIndex !== -1) {
            toasts.splice(toastIndex, 1);
        }
        return;
    }
    if (!toasts.includes(toast)) {
        return;
    }
    if (toast.matches(":hover") && !onclick) {
        if (toast.classList.contains("removal-queued")) {
            return;
        }
        toast.classList.add("removal-queued");
        toast.addEventListener("mouseleave", toastOnMouseLeave);
        return;
    }
    toasts.splice(toasts.indexOf(toast), 1);
    const toastRect = toast.getBoundingClientRect();
    const toastX = toastRect.x;
    const toastY = toastRect.y;
    flipStart();
    toastContainer.removeChild(toast);
    toast.style.position = "fixed";
    toast.style.left = `${toastX}px`;
    toast.style.top = `${toastY}px`;
    body.appendChild(toast);
    flipEnd();
    flipPlay();
    toast.classList.add("removing");
    toast.addEventListener("animationend", () => {
        toast.remove();
    });
}

/**
 * Show a toast message
 * @param {string} message Message to display
 * @param {string?} description Description to display
 * @param {"info" | "success" | "warning" | "error" | undefined} type Type of toast (info, success, warning, error)
 */
function showToast(message, description, type) {
    if (!message) {
        return;
    }
    if (!description) {
        description = "";
    }
    if (!["info", "success", "warning", "error"].includes(type || "")) {
        type = "info";
    }

    const toast = document.createElement("div");
    toast.classList.add("toast", `toast-${type}`);
    const id = generateId();
    toast.id = id;
    const icon =
        type === "info"
            ? infoIcon
            : type === "success"
            ? successIcon
            : type === "warning"
            ? warningIcon
            : type === "error"
            ? errorIcon
            : infoIcon;

    const parser = new DOMParser();
    const typeIconElement = parser.parseFromString(icon, "image/svg+xml").documentElement;
    const closeIconElement = parser.parseFromString(closeIcon, "image/svg+xml").documentElement;

    const header = document.createElement("div");
    header.classList.add("toast-header");
    header.appendChild(typeIconElement);
    const messageSpan = document.createElement("span");
    messageSpan.classList.add("mr-auto", "ml-2");
    messageSpan.textContent = message;
    header.appendChild(messageSpan);
    const closeButton = document.createElement("button");
    closeButton.classList.add("toast-close");
    closeButton.appendChild(closeIconElement);
    closeButton.addEventListener("click", () => {
        removeToast(id, true);
    });
    header.appendChild(closeButton);
    toast.appendChild(header);
    if (description) {
        const descriptionDiv = document.createElement("div");
        descriptionDiv.classList.add("toast-body");
        descriptionDiv.textContent = description;
        toast.appendChild(descriptionDiv);
    }

    flipStart();
    toasts.push(toast);
    toastContainer.appendChild(toast);
    flipEnd();
    flipPlay();
    setTimeout(() => {
        removeToast(id);
    }, 5000);
}

const urlParams = new URLSearchParams(window.location.search);

if (urlParams.has("toast-message") || urlParams.has("toast-description")) {
    let message = urlParams.get("toast-message");
    let description = urlParams.get("toast-description") || "";
    const type = urlParams.get("toast-type") || "info";

    if (!message) {
        message = description;
        description = "";
    }

    if (message) {
        // @ts-expect-error
        showToast(message, description, type);

        const newUrl = new URL(window.location.href);
        newUrl.searchParams.delete("toast-message");
        newUrl.searchParams.delete("toast-description");
        newUrl.searchParams.delete("toast-type");

        window.history.replaceState(null, "", newUrl);
    }
}

///////////////////////////////////////
//               Utils               //
///////////////////////////////////////

/**
 * Encode raw bytes data to base64 data URL
 * @param {Uint8Array} bytes The data to encode
 * @param {string} type The mimetype of the data
 * @returns {Promise<string | ArrayBuffer | null>} Base64 data URL string
 */
async function bytesToBase64DataUrl(bytes, type = "application/octet-stream") {
    return await new Promise((resolve, reject) => {
        const reader = Object.assign(new FileReader(), {
            onload: () => resolve(reader.result),
            onerror: () => reject(reader.error)
        });
        reader.readAsDataURL(new File([bytes], "", { type }));
    });
}




// navbar scroll

window.addEventListener("scroll", function () {
    const navbar = document.querySelector(".navbar");
    if (window.scrollY > 0) {
        navbar.classList.add("scrolled");
    } else {
        navbar.classList.remove("scrolled");
    }
});
