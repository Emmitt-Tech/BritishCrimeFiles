// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

document.addEventListener("DOMContentLoaded", function () {
    const timelineCarousel = document.querySelector(".case-timeline-carousel");

    if (!timelineCarousel) {
        return;
    }

    const timelineSection = timelineCarousel.closest("section");
    const timelineRows = Array.from(timelineCarousel.querySelectorAll(".timeline-row"));

    if (!timelineSection || timelineRows.length === 0) {
        return;
    }

    const timelineItems = timelineRows.map(function (row) {
        const date = row.querySelector(".timeline-event-date")?.textContent?.trim() ?? "";
        const title = row.querySelector(".timeline-event-title")?.textContent?.trim() ?? "";
        const description = row.querySelector(".timeline-popup p")?.textContent?.trim() ?? "";

        return { date, title, description };
    });

    const style = document.createElement("style");
    style.textContent = `
        .case-timeline-launch {
            margin-top: 1rem;
            margin-bottom: 1rem;
        }

        .case-timeline-launch-button {
            display: inline-flex;
            align-items: center;
            gap: .5rem;
            padding: .65rem 1rem;
            border: 1px solid #666;
            border-radius: 6px;
            background: #fff;
            color: #222;
            font: inherit;
            font-weight: 600;
            cursor: pointer;
        }

        .case-timeline-launch-button:hover {
            background: #f5f5f5;
        }

        .case-timeline-modal-backdrop {
            display: none;
            position: fixed;
            inset: 0;
            z-index: 2000;
            padding: 4vh 4vw;
            background: rgba(0, 0, 0, .58);
            align-items: center;
            justify-content: center;
        }

        .case-timeline-modal-backdrop.is-open {
            display: flex;
        }

        .case-timeline-modal {
            width: min(1000px, 100%);
            max-height: 92vh;
            display: flex;
            flex-direction: column;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 20px 60px rgba(0, 0, 0, .35);
            overflow: hidden;
        }

        .case-timeline-modal-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 1rem;
            padding: 1.1rem 1.35rem;
            border-bottom: 1px solid #ddd;
        }

        .case-timeline-modal-title {
            margin: 0;
            font-size: 1.4rem;
        }

        .case-timeline-modal-close {
            border: 0;
            background: transparent;
            font-size: 2rem;
            line-height: 1;
            cursor: pointer;
            color: #555;
        }

        .case-timeline-modal-body {
            overflow-y: auto;
            padding: .5rem 1.35rem 1.35rem;
        }

        .case-timeline-table {
            width: 100%;
            border-collapse: collapse;
        }

        .case-timeline-table th,
        .case-timeline-table td {
            padding: .85rem .75rem;
            border-bottom: 1px solid #e2e2e2;
            text-align: left;
            vertical-align: top;
        }

        .case-timeline-table th {
            position: sticky;
            top: 0;
            z-index: 1;
            background: #fff;
            font-size: .85rem;
            text-transform: uppercase;
            letter-spacing: .04em;
            color: #666;
        }

        .case-timeline-table-date {
            width: 160px;
            white-space: nowrap;
            font-weight: 600;
            color: #555;
        }

        .case-timeline-table-event {
            font-weight: 600;
        }

        .case-timeline-detail {
            margin-top: .55rem;
            font-weight: 400;
        }

        .case-timeline-detail summary {
            display: inline-block;
            cursor: pointer;
            color: #555;
            font-weight: 600;
        }

        .case-timeline-detail-text {
            margin-top: .55rem;
            line-height: 1.55;
            color: #333;
        }

        @media (max-width: 650px) {
            .case-timeline-modal-backdrop {
                padding: 0;
            }

            .case-timeline-modal {
                width: 100%;
                height: 100%;
                max-height: 100vh;
                border-radius: 0;
            }

            .case-timeline-table th:first-child,
            .case-timeline-table td:first-child {
                width: 110px;
            }

            .case-timeline-table-date {
                width: 110px;
                white-space: normal;
            }
        }
    `;
    document.head.appendChild(style);

    const launch = document.createElement("div");
    launch.className = "case-timeline-launch";

    const launchButton = document.createElement("button");
    launchButton.type = "button";
    launchButton.className = "case-timeline-launch-button";
    launchButton.textContent = `View timeline (${timelineItems.length})`;
    launchButton.setAttribute("aria-haspopup", "dialog");
    launchButton.setAttribute("aria-controls", "caseTimelineModal");
    launch.appendChild(launchButton);

    timelineCarousel.replaceWith(launch);

    const backdrop = document.createElement("div");
    backdrop.className = "case-timeline-modal-backdrop";
    backdrop.id = "caseTimelineModal";
    backdrop.setAttribute("role", "dialog");
    backdrop.setAttribute("aria-modal", "true");
    backdrop.setAttribute("aria-labelledby", "caseTimelineModalTitle");

    const modal = document.createElement("div");
    modal.className = "case-timeline-modal";

    const header = document.createElement("div");
    header.className = "case-timeline-modal-header";

    const heading = document.createElement("h2");
    heading.className = "case-timeline-modal-title";
    heading.id = "caseTimelineModalTitle";
    heading.textContent = "Case timeline";

    const closeButton = document.createElement("button");
    closeButton.type = "button";
    closeButton.className = "case-timeline-modal-close";
    closeButton.setAttribute("aria-label", "Close timeline");
    closeButton.textContent = "×";

    header.appendChild(heading);
    header.appendChild(closeButton);

    const body = document.createElement("div");
    body.className = "case-timeline-modal-body";

    const table = document.createElement("table");
    table.className = "case-timeline-table";

    const thead = document.createElement("thead");
    const headerRow = document.createElement("tr");
    const dateHeader = document.createElement("th");
    const eventHeader = document.createElement("th");
    dateHeader.textContent = "Date";
    eventHeader.textContent = "Event";
    headerRow.appendChild(dateHeader);
    headerRow.appendChild(eventHeader);
    thead.appendChild(headerRow);

    const tbody = document.createElement("tbody");

    timelineItems.forEach(function (item) {
        const row = document.createElement("tr");

        const dateCell = document.createElement("td");
        dateCell.className = "case-timeline-table-date";
        dateCell.textContent = item.date;

        const eventCell = document.createElement("td");
        const eventTitle = document.createElement("div");
        eventTitle.className = "case-timeline-table-event";
        eventTitle.textContent = item.title;
        eventCell.appendChild(eventTitle);

        if (item.description) {
            const details = document.createElement("details");
            details.className = "case-timeline-detail";

            const summary = document.createElement("summary");
            summary.textContent = "More details";

            const detailText = document.createElement("div");
            detailText.className = "case-timeline-detail-text";
            detailText.textContent = item.description;

            details.appendChild(summary);
            details.appendChild(detailText);
            eventCell.appendChild(details);
        }

        row.appendChild(dateCell);
        row.appendChild(eventCell);
        tbody.appendChild(row);
    });

    table.appendChild(thead);
    table.appendChild(tbody);
    body.appendChild(table);

    modal.appendChild(header);
    modal.appendChild(body);
    backdrop.appendChild(modal);
    document.body.appendChild(backdrop);

    let previouslyFocusedElement = null;

    function openTimeline() {
        previouslyFocusedElement = document.activeElement;
        backdrop.classList.add("is-open");
        document.body.style.overflow = "hidden";
        closeButton.focus();
    }

    function closeTimeline() {
        backdrop.classList.remove("is-open");
        document.body.style.overflow = "";
        previouslyFocusedElement?.focus();
    }

    launchButton.addEventListener("click", openTimeline);
    closeButton.addEventListener("click", closeTimeline);

    backdrop.addEventListener("click", function (event) {
        if (event.target === backdrop) {
            closeTimeline();
        }
    });

    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" && backdrop.classList.contains("is-open")) {
            closeTimeline();
        }
    });
});
