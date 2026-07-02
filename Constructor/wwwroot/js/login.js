const validationBlock = document.querySelector("[data-login-validation]");

if (validationBlock) {
    const hasErrorText = validationBlock.textContent.trim().length > 0;
    validationBlock.classList.toggle("is-visible", hasErrorText);
}
