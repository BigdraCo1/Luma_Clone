/** @type {HTMLInputElement} */
// @ts-expect-error
const fileInput = document.querySelector("#avatar-file-input");

function avatarSelect() {
    fileInput.click();
}

async function avatarUpload() {
    /** @type {File} */
    // @ts-expect-error
    const file = fileInput.files[0];
    const dataUrl = await bytesToBase64DataUrl(new Uint8Array(await file.arrayBuffer()), file.type);
    console.log(dataUrl);
}
