window.animationInterop = {
    onAnimationEnd: (element, dotNetRef, methodName) => {
        if (!element) return;

        const handler = () => {

            element.removeEventListener('animationend', handler);
            dotNetRef.invokeMethodAsync(methodName);
        };

        element.addEventListener('animationend', handler);
    }
};