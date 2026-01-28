/**
 * Synchronizes a hidden input (containing a UTC ISO string) with separate visible Date and Time inputs (displaying local time).
 * 
 * This function performs two main actions:
 * 1. Initialization: Converts the UTC value from the hidden input to the user's local timezone for display.
 *    It splits the local time into date and time parts to populate the visual inputs.
 * 2. Change Handling: Listens for input on the visual elements, combines the selected local date and time back to UTC,
 *    and updates the hidden input.
 * 
 * @param {string} hiddenInputId - The ID of the hidden input element storing the UTC value (sent to server).
 * @param {string} visualDateId - The ID of the visible input element for the date (local time).
 * @param {string} visualTimeId - The ID of the visible input element for the time (local time).
*/
function syncUtcAndLocalTime(hiddenInputId, visualDateId, visualTimeId) {
    const hiddenInput = document.getElementById(hiddenInputId);
    const dateInput = document.getElementById(visualDateId);
    const timeInput = document.getElementById(visualTimeId);

    if (!hiddenInput || !dateInput || !timeInput) return;

    // 1. Init: UTC (Server) -> Local (Visual)
    if (hiddenInput.value) {
        const utcDate = new Date(hiddenInput.value);
        if (!isNaN(utcDate.getTime()) && utcDate.getFullYear() > 1) {
            // Get local time components
            const offset = utcDate.getTimezoneOffset() * 60000;
            const localISOTime = new Date(utcDate.getTime() - offset).toISOString();
            
            // Split into date and time parts
            dateInput.value = localISOTime.slice(0, 10);
            timeInput.value = localISOTime.slice(11, 16);
        } else {
             // Default to now
            const now = new Date();
            const offset = now.getTimezoneOffset() * 60000;
            const localISOTime = new Date(now - offset).toISOString();
            
            dateInput.value = localISOTime.slice(0, 10);
            timeInput.value = localISOTime.slice(11, 16);
            
            hiddenInput.value = now.toISOString();
        }
    }

    // 2. Change: Local (Visual) -> UTC (Hidden)
    function updateHidden() {
        if (!dateInput.value || !timeInput.value) {
            hiddenInput.value = '';
            return;
        }
        
        const localIsoString = `${dateInput.value}T${timeInput.value}`;
        const localDate = new Date(localIsoString);
        
        if (!isNaN(localDate.getTime())) {
            hiddenInput.value = localDate.toISOString();
        }
    }

    dateInput.addEventListener('input', updateHidden);
    timeInput.addEventListener('input', updateHidden);
    
    // Initial sync to ensure hidden value is normalized if it was populated with defaults
    updateHidden(); 
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
