/**
 * Synchronizes a hidden input (containing a UTC ISO string) with a visible datetime-local input (displaying local time).
 * 
 * This function performs two main actions:
 * 1. Initialization: Converts the UTC value from the hidden input to the user's local timezone for display.
 *    It also immediately normalizes the hidden input value to ensure a valid UTC string is ready for submission.
 * 2. Change Handling: Listens for input on the visual element, converts the selected local time back to UTC,
 *    and updates the hidden input.
 * 
 * @param {string} hiddenInputId - The ID of the hidden input element storing the UTC value (sent to server).
 * @param {string} visualInputId - The ID of the visible input element used for user interaction (local time).
*/
function syncUtcAndLocalTime(hiddenInputId, visualInputId) {
    const hiddenInput = document.getElementById(hiddenInputId);
    const visualInput = document.getElementById(visualInputId);

    if (!hiddenInput || !visualInput) return;

    // 1. Init: UTC (Server) -> Local (Visual)
    if (hiddenInput.value) {
        const utcDate = new Date(hiddenInput.value);
        if (!isNaN(utcDate.getTime()) && utcDate.getFullYear() > 1) {
             // Get local ISO string (yyyy-MM-ddTHH:mm)
            // Subtract timezone offset to shift "UTC time" to "Local numbers", then slice
            const offset = utcDate.getTimezoneOffset() * 60000;
            const localISOTime = new Date(utcDate.getTime() - offset).toISOString().slice(0, 16);
            visualInput.value = localISOTime;

            // CRITICAL FIX: Ensure hidden input is consistent with the visual input immediately.
            // This handles cases where the server sent a local time string (unspecified kind) 
            // or we just want to guarantee a clean UTC string is sent back.
            // We take the local time displayed and convert it back to a full UTC ISO string.
            const normalizedUtc = new Date(visualInput.value).toISOString();
            hiddenInput.value = normalizedUtc;
        }
        else {
             // If date is empty or invalid (e.g. MinValue), set current local time
            const now = new Date();
            const offset = now.getTimezoneOffset() * 60000;

            //Excluding seconds and milliseconds
            const localISOTime = new Date(now - offset).toISOString().slice(0, 16);
            visualInput.value = localISOTime;
            
            // And update hidden to be valid UTC
            hiddenInput.value = now.toISOString();
        }
    }

    // 2. Change: Local (Visual) -> UTC (Hidden)
    visualInput.addEventListener('input', function () {
        if (!this.value) {
            hiddenInput.value = '';
            return;
        }
        const localDate = new Date(this.value); // Browser creates date in local timezone
        hiddenInput.value = localDate.toISOString(); // Converts to UTC string
    });
}

/**
 * Converts UTC date strings in HTML elements to the user's local time.
 * 
 * Required HTML structure:
 * - Element must have class "local-date".
 * - Element must have attribute "data-utc" containing a valid UTC ISO string (e.g., "2023-10-27T10:00:00Z").
 * - Optional: Attribute "data-format" to specify display format: "date", "time", or default (full date + time).
 * 
 * Example:
 * <span class="local-date" data-utc="2023-10-27T10:00:00Z" data-format="date"></span>
 */
function convertUtcLabelsToLocal() {
    const elements = document.querySelectorAll('.local-date');
    elements.forEach(el => {
        const utcString = el.getAttribute('data-utc');
        if (!utcString) return;

        const date = new Date(utcString);
        if (isNaN(date.getTime())) return;

        const format = el.getAttribute('data-format');
        let options = {};

        if (format === 'date') {
            options = { year: 'numeric', month: 'short', day: '2-digit' };
        } else if (format === 'time') {
            options = { hour: 'numeric', minute: '2-digit' };
        } else {
            options = { year: 'numeric', month: 'short', day: '2-digit', hour: 'numeric', minute: '2-digit' };
        }

        // For time-only, allow more specific control.
        // Modern browsers handle combined options well in toLocaleDateString.
        if (format === 'time') {
            el.textContent = date.toLocaleTimeString(undefined, options);
        } else {
            el.textContent = date.toLocaleDateString(undefined, options);
        }
    });
}
