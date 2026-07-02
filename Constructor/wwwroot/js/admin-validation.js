const validationBlocks = document.querySelectorAll("[data-admin-validation]");

validationBlocks.forEach((validationBlock) => {
    const hasErrorText = validationBlock.textContent.trim().length > 0;
    validationBlock.classList.toggle("is-visible", hasErrorText);
});
