var Vocab = Vocab || {};
Vocab.clearRadioButtons = function () {
    document.querySelectorAll("input[type=radio]").forEach(el => el.checked = false);
};