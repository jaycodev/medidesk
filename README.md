<h1 align="center">🏥 Medical Appointment System</h1>

<p align="center">
  <img src="https://github.com/jason-vila/medical-appointment-system/blob/main/medical-appointment-system/Content/Img/preview1.png" alt="Preview 1" width="800"/>
</p>

<p align="center">
  <img src="https://github.com/jason-vila/medical-appointment-system/blob/main/medical-appointment-system/Content/Img/preview2.png" alt="Preview 2" width="800"/>
</p>

A modern, responsive, and secure web application for managing medical appointments. Designed for administrators, doctors, and patients with features like light/dark mode, user roles, calendar scheduling, and exportable reports.

## 🚀 Features

- 🔐 Role-based access (Admin, Doctor, Patient)
- 🗓️ Appointment scheduling with calendar
- ☁️ Cloudinary image upload integration
- 📁 Export reports (PDF & Excel)
- 🌙 Light/Dark mode toggle
- 📊 Dashboard with real-time stats
- 📱 Fully responsive interface

## 🔧 Setup Instructions

Follow these steps to run the project locally:

1. **Clone the repository**

   ```bash
   git clone https://github.com/your-username/jason-vila-medical-appointment-system.git
   cd jason-vila-medical-appointment-system/medical-appointment-system
   ```

2. **Copy and edit configuration files**

   ```bash
   cp cloudinary.config.example cloudinary.config
   cp connectionStrings.config.example connectionStrings.config
   ```

   Then fill them with your own values.

3. **Set up the database**

   Use the scripts in the `/database/` folder (`schema.sql`, `data.sql`, `stored_procedures.sql`) to create and seed the database.

4. **Run the project**

   Open `medical-appointment-system.sln` in Visual Studio and run it (requires .NET Framework 4.7.2).

## 📁 Configuration

The app uses two external config files for security:

### `cloudinary.config`

Used to connect to your Cloudinary account (for image uploads):

```xml
<appSettings>
  <add key="CloudinaryCloudName" value="your_cloud_name"/>
  <add key="CloudinaryApiKey" value="your_api_key"/>
  <add key="CloudinaryApiSecret" value="your_api_secret"/>
</appSettings>
```

### `connectionStrings.config`

Used to connect to your local or remote SQL Server database:

```xml
<connectionStrings>
  <add name="DefaultConnection" connectionString="your_connection_string"/>
</connectionStrings>
```

> [!NOTE]
> Both files are ignored via .gitignore. Do not commit sensitive credentials.


## 🛠️ Tech Stack

- **Backend:** ASP.NET MVC (.NET Framework 4.7.2)
- **Frontend:** Razor Views with HTML, CSS, JavaScript (jQuery, DataTables)
- **Database:** SQL Server (or compatible)
- **Cloud Storage:** [Cloudinary](https://cloudinary.com/) for image management
- **Authentication:** ASP.NET Identity with role-based authorization
- **PDF Generation:** [iTextSharp](https://github.com/itext/itextsharp) (`5.5.13.4`)
- **Excel Export:** [ClosedXML](https://github.com/ClosedXML/ClosedXML) (`0.105.0`) and `DocumentFormat.OpenXml` (`3.1.1`)
- **UI Enhancements:** Bootstrap, Modernizr, Light/Dark mode toggle
- **Client-Side Validation:** jQuery Validation + Unobtrusive

💾 Database scripts are located in the `/database/` directory.

## 🧑‍💻 Contributors

<a href="https://github.com/jason-vila/medical-appointment-system/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=jason-vila/medical-appointment-system" />
</a>

## 📄 License

This project is licensed under the [MIT License](./LICENSE).
