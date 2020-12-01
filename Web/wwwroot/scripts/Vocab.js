var Web = Web || {};

Web.setFocus = (element) => {
    element.focus();
};

Web.getDimensions = () => {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

Web.clearRadioButtons = () => {
    document.querySelectorAll("input[type=radio]")
        .forEach(el => el.checked = false);
};

Web.saveToStorage = (translations) => {
    console.log("Saving...");
    localStorage.setItem('translations', translations);
};

Web.getStorageItem = (name) => {
    var results = localStorage.getItem(name);
    return JSON.parse(results);
};

Web.clearStorage = () => {
    localStorage.clear();
};