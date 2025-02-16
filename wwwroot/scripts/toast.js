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

const toasts = [];

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
            easing: "ease-out",
            fill: "both"
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
        if (toasts.includes(toast)) {
            toasts.splice(toasts.indexOf(toast), 1);
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
 * @param {string?} type Type of toast (info, success, warning, error)
 */
function showToast(message, description, type) {
    if (!message) {
        return;
    }
    if (!description) {
        description = "";
    }
    if (!type) {
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
    toast.innerHTML = `
        <div class="toast-header">
            ${icon}
            <span class="mr-auto ml-2">${message}</span>
            <button type="button" class="toast-close" onclick="removeToast('${id}', true)">
                ${closeIcon}
            </button>
        </div>
    `;
    if (description) {
        toast.innerHTML += `
            <div class="toast-body">
                ${description}
            </div>
        `;
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

if (urlParams.has("message")) {
    const message = urlParams.get("message") || "Error: Toast message not found";
    const description = urlParams.get("description") || "";
    const type = urlParams.get("type") || "info";

    showToast(message, description, type);

    urlParams.delete("message");
    urlParams.delete("description");
    urlParams.delete("type");

    const newParams = urlParams.toString();

    let newUrl = window.location.protocol + "//" + window.location.host + window.location.pathname;

    if (newParams) {
        newUrl += "?" + newParams;
    }

    window.history.replaceState(null, "", newUrl);
}
