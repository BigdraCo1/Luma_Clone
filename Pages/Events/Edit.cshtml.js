/** @type {HTMLInputElement} */
// @ts-expect-error
const fileInput = document.querySelector("#image-file-input");

/** @type {HTMLImageElement} */
// @ts-expect-error
const imagePreviewElement = document.querySelector("#image-preview");

/** @type {HTMLInputElement} */
// @ts-expect-error
const formImageInput = document.querySelector("#form-image-input");

/** @type {HTMLDivElement} */
// @ts-expect-error
const questionsContainer = document.querySelector("#event-registration-questions");

/**
 * @param {MouseEvent} event
 */
function imageSelect(event) {
    event.preventDefault();
    fileInput.click();
}

async function imageUpload() {
    /** @type {File} */
    // @ts-expect-error
    const file = fileInput.files[0];
    const dataUrl = await bytesToBase64DataUrl(new Uint8Array(await file.arrayBuffer()), file.type);
    imagePreviewElement.setAttribute("src", String(dataUrl));
    formImageInput.value = String(dataUrl);
}

if (formImageInput.value) {
    imagePreviewElement.setAttribute("src", formImageInput.value);
}
