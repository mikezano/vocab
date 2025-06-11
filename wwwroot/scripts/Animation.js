window.animationInterop = {
    onAnimationEnd: (element, dotNetRef, methodName) => {
        console.log('onAnimationEnd called', element, dotNetRef, methodName);
        if (!element) return;

        const handler = () => {

            element.removeEventListener('animationend', handler);
            dotNetRef.invokeMethodAsync(methodName);
            console.log('Animation ended, handler removed');
        };

        element.addEventListener('animationend', handler);
    }
};