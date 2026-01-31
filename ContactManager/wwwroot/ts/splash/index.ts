((): void => {
    const splash = document.getElementById("splash") as HTMLElement | null;
    if (!splash) return; // no splash on this page

    const NEXT = "/Contacts";
    const SHOW_MS = 1200; // how long to show the splash
    const FADE_MS = 450;  // match CSS transition time

    let redirected = false;
    const go = (): void => {
        if (redirected) return;
        redirected = true;
        window.location.href = NEXT;
    };

    document.body.classList.add("splash-lock");

    const fade = (): void => {
        splash.classList.add("hide");
    };

    const cleanupAndGo = (): void => {
        document.body.classList.remove("splash-lock");
        go();
    };

    const t1 = window.setTimeout(fade, SHOW_MS);
    const t2 = window.setTimeout(cleanupAndGo, SHOW_MS + FADE_MS);

    splash.addEventListener("click", (): void => {
        // click overrides the auto timers
        window.clearTimeout(t1);
        window.clearTimeout(t2);

        fade();
        window.setTimeout(cleanupAndGo, FADE_MS);
    });
})();
