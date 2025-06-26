document.addEventListener("DOMContentLoaded", function () {
  const calendarEl = document.getElementById("calendar");

  const calendar = new FullCalendar.Calendar(calendarEl, {
    initialView: "dayGridMonth",
    eventDisplay: "block",
    locale: "es",
    firstDay: 1,
    dayMaxEventRows: true,
    dayMaxEvents: 3,
    moreLinkText: function (n) {
      return `+${n} más`;
    },
    titleFormat: { year: "numeric", month: "long" },
    buttonText: {
      today: "Hoy",
      month: "Mes",
      week: "Semana",
      day: "Día",
    },
    headerToolbar: {
      left: "prev,next today",
      center: "title",
      right: "dayGridMonth,dayGridWeek,dayGridDay",
    },
    datesSet: function (info) {
      const calendarTitle = document.querySelector(".fc-toolbar-title");
      if (calendarTitle) {
        const date = info.view.currentStart;
        const meses = [
          "ENERO",
          "FEBRERO",
          "MARZO",
          "ABRIL",
          "MAYO",
          "JUNIO",
          "JULIO",
          "AGOSTO",
          "SETIEMBRE",
          "OCTUBRE",
          "NOVIEMBRE",
          "DICIEMBRE",
        ];
        const mes = meses[date.getMonth()];
        const anio = date.getFullYear();
        calendarTitle.textContent = `${mes} ${anio}`;
      }

      const todayBtn = document.querySelector(".fc-today-button");
      if (todayBtn) {
        todayBtn.classList.add("btn", "btn-primary");
      }

      const prevBtn = document.querySelector(".fc-prev-button");
      const nextBtn = document.querySelector(".fc-next-button");
      if (prevBtn && nextBtn) {
        prevBtn.classList.add("btn", "btn-secondary");
        nextBtn.classList.add("btn", "btn-secondary");
      }

      const viewButtons = document.querySelectorAll(
        ".fc-dayGridMonth-button, .fc-dayGridWeek-button, .fc-dayGridDay-button"
      );

      viewButtons.forEach((btn) => {
        btn.classList.remove("btn-primary", "btn-secondary");
        btn.classList.add("btn", "btn-secondary");

        if (btn.classList.contains("fc-button-active")) {
          btn.classList.remove("btn-secondary");
          btn.classList.add("btn-primary");
        }
      });
    },
    events: window.appointmentEvents || [],
    eventClick: function (info) {
      const appointmentId = info.event.id;
      if (appointmentId) {
        window.location.href = `/appointments/details/${appointmentId}`;
      }
    },
    eventDidMount: function (info) {
      const status = info.event.extendedProps.status;

      const el = info.el;
      if (!el) return;

      el.style.backgroundColor = "";
      el.style.borderColor = "";
      el.style.color = "";
      el.style.cursor = "pointer";

      el.classList.remove(
        "bg-primary-subtle",
        "border-primary-subtle",
        "text-primary-emphasis",
        "bg-warning-subtle",
        "border-warning-subtle",
        "text-warning-emphasis",
        "bg-danger-subtle",
        "border-danger-subtle",
        "text-danger-emphasis",
        "bg-success-subtle",
        "border-success-subtle",
        "text-success-emphasis"
      );

      let bgClass = "",
        borderClass = "",
        textClass = "",
        iconClass = "";

      switch (status) {
        case "confirmada":
          bgClass = "bg-primary-subtle";
          borderClass = "border-primary-subtle";
          textClass = "text-primary-emphasis";
          iconClass = "fa-calendar-check";
          break;
        case "pendiente":
          bgClass = "bg-warning-subtle";
          borderClass = "border-warning-subtle";
          textClass = "text-warning-emphasis";
          iconClass = "fa-hourglass-half";
          break;
        case "cancelada":
          bgClass = "bg-danger-subtle";
          borderClass = "border-danger-subtle";
          textClass = "text-danger-emphasis";
          iconClass = "fa-ban";
          break;
        case "atendida":
          bgClass = "bg-success-subtle";
          borderClass = "border-success-subtle";
          textClass = "text-success-emphasis";
          iconClass = "fa-check";
          break;
        default:
          bgClass = "bg-secondary-subtle";
          borderClass = "border-secondary-subtle";
          textClass = "text-secondary-emphasis";
          iconClass = "fa-circle-question";
      }

      el.classList.add("border", bgClass, borderClass);

      const startDate = new Date(info.event.start);
      const endDate = new Date(info.event.end);

      const format12h = (date) => {
        let h = date.getHours();
        const m = date.getMinutes().toString().padStart(2, "0");
        const suffix = h >= 12 ? "p. m." : "a. m.";
        h = h % 12;
        h = h === 0 ? 12 : h;
        return `${h.toString().padStart(2, "0")}:${m} ${suffix}`;
      };

      const rangeText = `${format12h(startDate)} - ${format12h(endDate)}`;

      const titleEl = el.querySelector(".fc-event-title");
      if (titleEl) {
        titleEl.className = "fc-event-title " + textClass;
        const icon = document.createElement("i");
        icon.className = "fa-solid " + iconClass + " ms-1 me-1";
        titleEl.innerHTML = "";
        titleEl.appendChild(icon);
        titleEl.append(" " + rangeText);
      }
    },
  });

  window.addEventListener("resize", function () {
    if (window.innerWidth < 576) {
      calendar.changeView("dayGridDay");
    } else {
      calendar.changeView("dayGridMonth");
    }
  });

  calendar.render();

  window.dispatchEvent(new Event("resize"));
});
