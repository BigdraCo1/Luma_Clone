/** @type {string} */
// @ts-expect-error
const displayedUserId = document.querySelector("#displayed-user-id")?.textContent;

/** @type {string} */
// @ts-expect-error
const followSuccessText = document.querySelector("#follow-success-text")?.textContent;

/** @type {string} */
// @ts-expect-error
const followFailText = document.querySelector("#follow-fail-text")?.textContent;

/** @type {string} */
// @ts-expect-error
const unfollowSuccessText = document.querySelector("#unfollow-success-text")?.textContent;

/** @type {string} */
// @ts-expect-error
const unfollowFailText = document.querySelector("#unfollow-fail-text")?.textContent;

/**
 * @param {MouseEvent} event
 */
async function follow(event) {
    event.preventDefault();
    try {
        await ajaxFetch(`/users/follow?id=${displayedUserId}`, {
            method: "POST"
        });
        showToast(followSuccessText, null, "success");
    } catch (err) {
        showToast(followFailText, String(err), "error");
    }
}

/**
 * @param {MouseEvent} event
 */
async function unfollow(event) {
    event.preventDefault();
    try {
        await ajaxFetch(`/users/unfollow?id=${displayedUserId}`, {
            method: "POST"
        });
        showToast(unfollowSuccessText, null, "success");
    } catch (err) {
        showToast(unfollowFailText, String(err), "error");
    }
}
