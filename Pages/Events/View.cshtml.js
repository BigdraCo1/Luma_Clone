/** @type {string} */
// @ts-expect-error
const registrationAcceptSuccessText = document.querySelector(
    "#registration-accept-success-text"
)?.textContent;

/** @type {string} */
// @ts-expect-error
const registrationAcceptFailText = document.querySelector(
    "#registration-accept-fail-text"
)?.textContent;

/** @type {string} */
// @ts-expect-error
const registrationRejectSuccessText = document.querySelector(
    "#registration-reject-success-text"
)?.textContent;

/** @type {string} */
// @ts-expect-error
const registrationRejectFailText = document.querySelector(
    "#registration-reject-fail-text"
)?.textContent;

/** @type {HTMLElement} */
// @ts-expect-error
const registrationCardsContainer = document.querySelector("#registration-cards-container");

/** @type {Record<string, { start?: DOMRect, end?: DOMRect }>} */
let rects = {};

function flipStart() {
    rects = {};
    const elements = document.querySelectorAll(".registration-card");
    for (const element of elements) {
        const rect = element.getBoundingClientRect();
        rects[element.id] = { start: rect };
    }
}

function flipEnd() {
    const elements = document.querySelectorAll(".registration-card");
    for (const element of elements) {
        const rect = element.getBoundingClientRect();
        if (!rects[element.id]) {
            continue;
        }
        rects[element.id].end = rect;
    }
}

function flipPlay() {
    const elements = document.querySelectorAll(".registration-card");
    for (const element of elements) {
        const rect = rects[element.id];
        if (!rect || !rect.start || !rect.end) {
            continue;
        }
        const dx = rect.start.x - rect.end.x;
        const dy = rect.start.y - rect.end.y;
        element.animate([{ transform: `translate(${dx}px, ${dy}px)` }, { transform: "none" }], {
            duration: 250,
            easing: "ease"
        });
    }
}

/**
 * @param {string} userId
 */
function removeCard(userId) {
    const card = document.getElementById(`registration-${userId}`);
    if (card === null || card === undefined) {
        return;
    }
    const cardRect = card.getBoundingClientRect();
    const cardX = cardRect.x;
    const cardY = cardRect.y;
    flipStart();
    registrationCardsContainer.removeChild(card);
    card.style.position = "fixed";
    card.style.left = `${cardX}px`;
    card.style.top = `${cardY}px`;
    body.appendChild(card);
    flipEnd();
    flipPlay();
    card.classList.add("removing");
    card.addEventListener("animationend", () => {
        card.remove();
    });
    const cardsLeft = document.querySelectorAll(
        "#registration-cards-container > .registration-card"
    ).length;
    if (cardsLeft === 0) {
        registrationCardsContainer.remove();
    }
}

/**
 * @param {MouseEvent} event
 * @param {string} eventId
 * @param {string} userId
 */
function acceptRegistration(event, eventId, userId) {
    event.preventDefault();
    try {
        ajaxFetch(`/events/accept-registration?eventId=${eventId}&userId=${userId}`, {
            method: "POST"
        });
        showToast(registrationAcceptSuccessText, null, "success");
        removeCard(userId);
    } catch (err) {
        showToast(registrationAcceptFailText, String(err), "error");
    }
}

/**
 * @param {MouseEvent} event
 * @param {string} eventId
 * @param {string} userId
 */
function rejectRegistration(event, eventId, userId) {
    event.preventDefault();
    try {
        ajaxFetch(`/events/reject-registration?eventId=${eventId}&userId=${userId}`, {
            method: "POST"
        });
        showToast(registrationRejectSuccessText, null, "success");
        removeCard(userId);
    } catch (err) {
        showToast(registrationRejectFailText, String(err), "error");
    }
}
