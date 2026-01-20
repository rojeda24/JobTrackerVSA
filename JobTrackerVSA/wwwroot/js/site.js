function initializeDateTimeInputWithLocalTime(selector) {
    const now = new Date();
    const offset = now.getTimezoneOffset() * 60000; //Transform to milliseconds
    const localISOTime = new Date(now - offset).toISOString().slice(0, 16);//Excluding seconds and milliseconds

    const input = document.querySelector(selector);
    if (input) {
        input.value = localISOTime;
    }
}