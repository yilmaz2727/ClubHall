
    function showTab(tab) {
        const about = document.getElementById("about-section");
        const events = document.getElementById("events-section");
        const createEvent = document.getElementById("createEvent-section");
        const members = document.getElementById("members-section");

        const tabAbout = document.getElementById("tabAbout");
        const tabEvents = document.getElementById("tabEvents");
        const tabCreateEvent = document.getElementById("tabCreateEvent");
        const tabMembers = document.getElementById("tabMembers");

        about.style.display = "none";
        events.style.display = "none";
        createEvent.style.display = "none";
        members.style.display = "none";

        tabAbout.classList.remove("active");
        tabEvents.classList.remove("active");
        tabCreateEvent.classList.remove("active");
        tabMembers.classList.remove("active");

        if (tab === "about")
        {
            about.style.display = "flex";
            tabAbout.classList.add("active");
        }
        else if(tab === "events")
        {
            events.style.display = "grid";
            tabEvents.classList.add("active");
        }
        else if(tab === "createEvent")
        {
            createEvent.style.display = "block";
            tabCreateEvent.classList.add("active");
        }
        else if(tab === "members")
        {
            members.style.display = "block";
            tabMembers.classList.add("active");
        }
    }