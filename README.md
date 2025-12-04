# Document Management System (DMS)

A comprehensive, modern Document Management System built with ASP.NET 8 Blazor Server and MudBlazor.

## Features

### 📄 Document Management
- **Upload & Storage**: Upload documents with automatic file organization by date
- **Version Control**: Track document versions with full history
- **Categorization**: Organize documents into customizable categories
- **Tagging**: Add multiple tags to documents for easy filtering
- **Search & Filter**: Advanced search with filters for category, tags, and status
- **Check-in/Check-out**: Prevent concurrent editing conflicts

### 🔐 Security & Access Control
- **Role-Based Access**: Administrator, Manager, User, and Guest roles
- **Document Permissions**: Granular permissions (View, Edit, Delete, Share, Full Control)
- **Audit Trail**: Complete activity logging with IP tracking
- **User Authentication**: ASP.NET Identity integration

### 🔄 Workflow Management
- **Approval Workflows**: Customizable multi-step approval processes
- **Task Assignment**: Assign tasks to users or roles
- **Workflow Templates**: Reusable workflow definitions
- **Status Tracking**: Monitor workflow progress in real-time

### 🔔 Notifications
- **Activity Notifications**: Real-time notifications for document activities
- **Workflow Alerts**: Notifications for workflow assignments and completions
- **Email Support**: Ready for email notification integration

### 📊 Dashboard & Analytics
- **Statistics Dashboard**: Visual overview of document statistics
- **Recent Activity**: Track recent document uploads and changes
- **Quick Actions**: Fast access to common operations
- **Activity Charts**: Visual representation of system usage

### 💬 Collaboration
- **Comments**: Add comments and discussions on documents
- **Sharing**: Share documents with specific users or roles
- **Threaded Replies**: Support for comment threads

### 🎨 Modern UI
- **MudBlazor Components**: Beautiful, responsive Material Design interface
- **Dark Mode**: Toggle between light and dark themes
- **Responsive Design**: Works seamlessly on desktop, tablet, and mobile
- **Intuitive Navigation**: Easy-to-use sidebar navigation

## Technology Stack

- **Framework**: ASP.NET 8.0
- **UI**: Blazor Server with MudBlazor 6.11.2
- **Database**: SQL Server with Entity Framework Core 8.0
- **Authentication**: ASP.NET Identity
- **Validation**: FluentValidation 11.9
- **Image Processing**: SixLabors.ImageSharp 3.1

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- SQL Server (LocalDB or full instance)
- Visual Studio 2022 or VS Code

### Installation

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd DMS-NEW
   ```

2. **Update connection string**

   Edit `appsettings.json` and update the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=DMSDb;Trusted_Connection=True;"
   }
   ```

3. **Run the application**
   ```bash
   dotnet run
   ```

4. **Access the application**

   Open your browser and navigate to `https://localhost:5001`

### Default Admin Credentials
- **Email**: admin@dms.com
- **Password**: Admin@123456

## Project Structure

```
DMS-NEW/
├── Data/                    # Database context and initialization
├── Models/                  # Domain models
│   ├── ApplicationUser.cs
│   ├── Document.cs
│   ├── Category.cs
│   ├── Tag.cs
│   ├── Workflow*.cs
│   └── ...
├── Services/               # Business logic services
│   ├── DocumentService.cs
│   ├── FileStorageService.cs
│   ├── VersionControlService.cs
│   ├── WorkflowService.cs
│   └── ...
├── Validators/             # FluentValidation validators
├── Pages/                  # Blazor pages
│   ├── Index.razor        # Dashboard
│   ├── Documents.razor    # Document listing
│   ├── Upload.razor       # Document upload
│   ├── Categories.razor   # Category management
│   ├── Workflows.razor    # Workflow management
│   └── AuditLog.razor     # Audit trail
├── Shared/                 # Shared components
│   ├── MainLayout.razor
│   └── NavMenu.razor
└── wwwroot/               # Static files
```

## Configuration

### File Storage
Configure file storage settings in `appsettings.json`:

```json
"FileStorage": {
  "RootPath": "DocumentStorage",
  "MaxFileSize": 104857600,
  "AllowedExtensions": [".pdf", ".doc", ".docx", ...]
}
```

### Database
The application uses Entity Framework Core with SQL Server. The database is automatically created and seeded on first run.

## Features in Detail

### Document Upload
- Drag-and-drop file upload
- File size validation (default: 100MB max)
- File type validation
- Automatic metadata extraction
- Preview before upload

### Version Control
- Automatic version creation on document update
- Version history tracking
- Version comparison
- Restore previous versions
- Change description for each version

### Workflow System
- Multi-step approval workflows
- Role-based or user-specific assignment
- Parallel and sequential workflow steps
- Workflow status tracking
- Email notifications (ready to integrate)

### Search & Filter
- Full-text search across title, description, and filename
- Filter by category
- Filter by tags (multiple selection)
- Filter by status
- Filter by date range
- Advanced query builder

### Audit Trail
- Complete activity logging
- User action tracking
- IP address logging
- Timestamp for all actions
- Searchable audit logs

## Security Features

- Password complexity requirements
- Role-based authorization
- Document-level permissions
- Secure file storage
- SQL injection prevention
- XSS protection
- CSRF protection

## API Endpoints

The application uses Blazor Server, so all interactions are handled through SignalR. For REST API integration, consider adding a separate API project.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License.

## Support

For issues and questions:
- Create an issue in the GitHub repository
- Contact the development team

## Roadmap

Future enhancements:
- [ ] Document preview for PDF, Word, Excel
- [ ] Full-text search with Elasticsearch
- [ ] Advanced analytics dashboard
- [ ] Email notification integration
- [ ] Mobile app
- [ ] OCR for scanned documents
- [ ] Document templates
- [ ] Bulk operations
- [ ] Integration with cloud storage (Azure, AWS S3)
- [ ] Advanced reporting

## Acknowledgments

- MudBlazor for the beautiful UI components
- ASP.NET Team for the excellent framework
- Community contributors

---

**Built with ❤️ using ASP.NET 8 and Blazor Server**
