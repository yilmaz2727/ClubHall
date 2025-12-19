function showTab(tab) {
    const about = document.getElementById("about-section");
    const events = document.getElementById("events-section");
    const createEvent = document.getElementById("createEvent-section");
    const members = document.getElementById("members-section");

    const tabAbout = document.getElementById("tabAbout");
    const tabEvents = document.getElementById("tabEvents");
    const tabCreateEvent = document.getElementById("tabCreateEvent");
    const tabMembers = document.getElementById("tabMembers");
    if(about) about.style.display = "none";
    if(events) events.style.display = "none";
    if(createEvent) createEvent.style.display = "none";
    if(members) members.style.display = "none";

    if(tabAbout) tabAbout.classList.remove("active");
    if(tabEvents) tabEvents.classList.remove("active");
    if(tabCreateEvent) tabCreateEvent.classList.remove("active");
    if(tabMembers) tabMembers.classList.remove("active");

    if (tab === "about" && about) {
        about.style.display = "block";
        if(tabAbout) tabAbout.classList.add("active");
    }
    else if (tab === "events" && events) {
        events.style.display = "grid"; 
        if(tabEvents) tabEvents.classList.add("active");
    }
    else if (tab === "createEvent" && createEvent) {
        createEvent.style.display = "block";
        if (tabCreateEvent) tabCreateEvent.classList.add("active");
    }
    else if (tab === "members" && members) {
        members.style.display = "block";
        if (tabMembers) tabMembers.classList.add("active");
    }
    else if (tab === "edit" && createEvent) {
        createEvent.style.display = "block";
    }
}