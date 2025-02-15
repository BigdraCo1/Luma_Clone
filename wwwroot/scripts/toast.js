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

const toastContainer = toastContainerInit;

/**
 * Show a toast message
 * @param {string} message Message to display
 * @param {string} description Description to display
 * @param {string} type Type of toast (info, success, warning, error)
 */
function showToast(message, description, type) {
    const toast = document.createElement("div");
    toast.classList.add("toast", `toast-${type}`);
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
            <button type="button" class="toast-close" onclick="this.parentElement.parentElement.remove()">
                ${closeIcon}
            </button>
        </div>
        <div class="toast-body">
            ${description}
        </div>
    `;
    toastContainer.appendChild(toast);
    setTimeout(() => {
        toast.remove();
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

    const newUrl =
        window.location.protocol +
        "//" +
        window.location.host +
        window.location.pathname +
        "?" +
        urlParams.toString();
    window.history.replaceState({ path: newUrl }, "", newUrl);
}
