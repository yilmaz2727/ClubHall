function showTab(tab) {
    const about = document.getElementById("about-section");
    const events = document.getElementById("events-section");
    const createEvent = document.getElementById("createEvent-section");
    const members = document.getElementById("members-section");

    const tabAbout = document.getElementById("tabAbout");
    const tabEvents = document.getElementById("tabEvents");
    const tabCreateEvent = document.getElementById("tabCreateEvent");
    const tabMembers = document.getElementById("tabMembers");

    // Tüm içerikleri sakla
    about.style.display = "none";
    events.style.display = "none";
    createEvent.style.display = "none";
    members.style.display = "none";

    // Aktif class'ını kaldır
    tabAbout.classList.remove("active");
    tabEvents.classList.remove("active");
    if (tabCreateEvent) tabCreateEvent.classList.remove("active");
    if (tabMembers) tabMembers.classList.remove("active");

    // Seçilen tab'ı göster
    if (tab === "about") {
        about.style.display = "block";
        tabAbout.classList.add("active");
    }
    else if (tab === "events") {
        events.style.display = "grid";
        tabEvents.classList.add("active");
    }
    else if (tab === "createEvent") {
        createEvent.style.display = "block";
        if (tabCreateEvent) tabCreateEvent.classList.add("active");
    }
    else if (tab === "members") {
        members.style.display = "block";
        if (tabMembers) tabMembers.classList.add("active");
    }

    document.addEventListener("DOMContentLoaded", function () {
    const activeTab = "@ViewBag.ActiveTab";

    if (activeTab) {
        showTab(activeTab);
    } else {
        showTab("about");
    }
});

}



