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

Web.saveToStorage = (name, obj) => {
    console.log("Saving...");
    localStorage.setItem(name, obj);
};

Web.getStorageItem = (name) => {
    var results = localStorage.getItem(name);
    return JSON.parse(results);
};

Web.getStorageItemAsString = (name) => {
    return localStorage.getItem(name);
};

Web.clearStorage = () => {
    localStorage.clear();
};