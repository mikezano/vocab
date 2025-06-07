window.animationInterop = {
    onAnimationEnd: (element, dotNetRef, methodName) => {
        if (!element) return;

        const handler = () => {
            dotNetRef.invokeMethodAsync(methodName);
            element.removeEventListener('animationend', handler);
        };

        element.addEventListener('animationend', handler);
    }
};