const trashIcon = `<svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="rgb(220 38 38)" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-trash-2"><path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" x2="10" y1="11" y2="17"/><line x1="14" x2="14" y1="11" y2="17"/></svg><svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round" class="lucide lucide-trash-2"><path d="M3 6h18"/><path d="M19 6v14c0 1-1 2-2 2H7c-1 0-2-1-2-2V6"/><path d="M8 6V4c0-1 1-2 2-2h4c1 0 2 1 2 2v2"/><line x1="10" x2="10" y1="11" y2="17"/><line x1="14" x2="14" y1="11" y2="17"/></svg>`;

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

/** @type {string} */
const questionCountLimitReachedText =
    document.querySelector("#question-count-limit-reached-text")?.textContent ??
    "You can only add up to 8 questions";

/** @type {string} */
const questionPlaceholder =
    document.querySelector("#question-placeholder")?.textContent ?? "Question";

let currentQuestionCount = 0;

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

/**
 * @param {MouseEvent} event
 */
function addQuestion(event) {
    event.preventDefault();
    if (currentQuestionCount >= 8) {
        showToast(questionCountLimitReachedText, "", "error");
        return;
    }
    currentQuestionCount++;
    const questionListElement = document.createElement("li");
    questionListElement.classList.add("flex-grow");
    const question = document.createElement("div");
    question.classList.add("flex", "items-center", "gap-2", "flex-grow");
    const questionText = document.createElement("input");
    questionText.setAttribute("type", "text");
    questionText.setAttribute("placeholder", questionPlaceholder);
    questionText.setAttribute("name", "Event.Questions");
    questionText.setAttribute("required", "required");
    questionText.classList.add(
        "flex-grow",
        "px-4",
        "py-2",
        "ml-2",
        "max-w-160",
        "text-sm",
        "text-gray-200",
        "transition-all",
        "bg-transparent",
        "border-2",
        "border-gray-400",
        "rounded-xl",
        "focus:border-gray-200"
    );
    question.appendChild(questionText);
    const questionDeleteButton = document.createElement("button");
    questionDeleteButton.innerHTML = trashIcon;
    questionDeleteButton.addEventListener("click", () => {
        currentQuestionCount--;
        questionListElement.remove();
    });
    questionDeleteButton.classList.add(
        "flex",
        "items-center",
        "justify-center",
        "w-10",
        "h-10",
        "p-0",
        "ml-2",
        "bg-transparent",
        "border-none"
    );
    question.appendChild(questionDeleteButton);
    questionListElement.appendChild(question);
    questionsContainer.appendChild(questionListElement);
}

if (formImageInput.value) {
    imagePreviewElement.setAttribute("src", formImageInput.value);
}