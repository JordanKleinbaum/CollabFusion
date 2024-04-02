// script.js
document.addEventListener("DOMContentLoaded", function () {
    const themeToggle = document.querySelector("#theme-toggle");
    const body = document.querySelector("body");

    // Check if the user has a theme preference stored in local storage
    const currentTheme = localStorage.getItem("theme");

    if (currentTheme) {
        body.classList.add(currentTheme);
    }

    // Toggle between light mode and dark mode
    themeToggle.addEventListener("click", function () {
        body.classList.toggle("dark-mode");

        // Save the user's preference in local storage
        const theme = body.classList.contains("dark-mode") ? "dark-mode" : "";
        localStorage.setItem("theme", theme);

        // Change the icon based on the current theme
        updateThemeToggleIcon();
    });

    // Function to update the theme toggle icon
    function updateThemeToggleIcon() {
        const currentTheme = body.classList.contains("dark-mode") ? "🌙" : "☀️";
        themeToggle.textContent = currentTheme;
    }

    // Initial call to set the icon based on the current theme
    updateThemeToggleIcon();
});
