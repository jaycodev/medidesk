namespace Web.Helpers
{
    public class SidebarHelper
    {
        public static List<SidebarItem> GetSidebarItems(string? activeRole)
        {
            if (string.IsNullOrEmpty(activeRole))
                return new List<SidebarItem>();

            activeRole = activeRole.ToLower();

            var allItems = new List<SidebarItem>
            {
                new SidebarItem { Title = "Inicio", Controller = "Appointments", Action = "Home", Icon = "fa-solid fa-house", Roles = new[] { "paciente", "medico", "administrador" } },
                new SidebarItem { Title = "Citas médicas", Controller = "Appointments", Action = "AllAppointments", Icon = "fa-solid fa-notes-medical", Roles = new[] { "administrador" } },
                new SidebarItem { Title = "Reservar cita", Controller = "Appointments", Action = "Reserve", Icon = "fa-solid fa-calendar-check", Roles = new[] { "paciente" } },
                new SidebarItem { Title = "Médicos", Controller = "Doctors", Action = "Index", Icon = "fa-solid fa-user-doctor", Roles = new[] { "administrador" } },
                new SidebarItem { Title = "Pacientes", Controller = "Patients", Action = "Index", Icon = "fa-solid fa-bed-pulse", Roles = new[] { "administrador" } },
                new SidebarItem { Title = "Especialidades", Controller = "Specialties", Action = "Index", Icon = "fa-solid fa-stethoscope", Roles = new[] { "administrador" } },
                new SidebarItem { Title = "Horarios", Controller = "Schedules", Action = "Index", Icon = "fa-solid fa-calendar-week", Roles = new[] { "medico" } },
                new SidebarItem { Title = "Usuarios", Controller = "Users", Action = "Index", Icon = "fa-solid fa-users", Roles = new[] { "administrador" } }
            };

            return allItems
                .Where(item => item.Roles.Any(r => r.ToLower() == activeRole))
                .ToList();
        }
    }
}
