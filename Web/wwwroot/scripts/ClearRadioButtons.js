var Web = Web || {};
Web.clearRadioButtons = function () {
    document.querySelectorAll("input[type=radio]").forEach(el => el.checked = false);
};