namespace medical_appointment_system.Models
{
    public class SidebarItem
    {
        public string Title { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Icon { get; set; }
        public string[] Roles { get; set; }
    }
}