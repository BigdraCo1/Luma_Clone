/** @type {HTMLInputElement} */
// @ts-expect-error
const fileInput = document.querySelector("#avatar-file-input");

const updateAvatarSuccessText =
    document.querySelector("#avatar-update-success-text")?.textContent ??
    "Avatar updated successfully";
const updateAvatarFailText =
    document.querySelector("#avatar-update-fail-text")?.textContent ?? "Failed to update avatar:";

/**
 * @param {SubmitEvent} event
 */
function avatarSelect(event) {
    event.preventDefault();
    fileInput.click();
}

async function avatarUpload() {
    /** @type {File} */
    // @ts-expect-error
    const file = fileInput.files[0];
    const dataUrl = await bytesToBase64DataUrl(new Uint8Array(await file.arrayBuffer()), file.type);
    const body = {
        AvatarDataUrl: dataUrl
    };
    try {
        await ajaxFetch("/users/avatar", {
            method: "POST",
            body
        });
        const url = new URL(window.location.href);
        url.searchParams.set("toast-message", updateAvatarSuccessText);
        url.searchParams.set("toast-type", "success");
        location.replace(url);
    } catch (err) {
        showToast(updateAvatarFailText, String(err), "error");
    }
}
