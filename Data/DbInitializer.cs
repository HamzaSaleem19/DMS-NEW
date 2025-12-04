using Microsoft.AspNetCore.Identity;
using DMS.Models;

namespace DMS.Data;

public static class DbInitializer
{
    public static async Task Initialize(ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Ensure database is created
        context.Database.EnsureCreated();

        // Create roles
        string[] roleNames = { "Administrator", "Manager", "User", "Guest" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // Create admin user
        var adminEmail = "admin@dms.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                FirstName = "System",
                LastName = "Administrator",
                Department = "IT",
                Position = "System Administrator",
                EmailConfirmed = true,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(admin, "Admin@123456");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Administrator");
            }
        }

        // Create sample categories
        if (!context.Categories.Any())
        {
            var categories = new[]
            {
                new Category { Name = "General", Description = "General documents", Color = "#2196F3", Icon = "folder" },
                new Category { Name = "Finance", Description = "Financial documents", Color = "#4CAF50", Icon = "attach_money" },
                new Category { Name = "Human Resources", Description = "HR related documents", Color = "#FF9800", Icon = "people" },
                new Category { Name = "Legal", Description = "Legal documents", Color = "#9C27B0", Icon = "gavel" },
                new Category { Name = "Marketing", Description = "Marketing materials", Color = "#E91E63", Icon = "campaign" },
                new Category { Name = "Technical", Description = "Technical documentation", Color = "#00BCD4", Icon = "code" },
                new Category { Name = "Sales", Description = "Sales documents", Color = "#8BC34A", Icon = "trending_up" },
                new Category { Name = "Operations", Description = "Operational documents", Color = "#FF5722", Icon = "settings" }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Create sample tags
        if (!context.Tags.Any())
        {
            var tags = new[]
            {
                new Tag { Name = "Important", Color = "#f44336" },
                new Tag { Name = "Urgent", Color = "#ff9800" },
                new Tag { Name = "Confidential", Color = "#9c27b0" },
                new Tag { Name = "Public", Color = "#4caf50" },
                new Tag { Name = "Draft", Color = "#607d8b" },
                new Tag { Name = "Final", Color = "#2196f3" },
                new Tag { Name = "Archive", Color = "#795548" },
                new Tag { Name = "Review", Color = "#ff5722" }
            };

            context.Tags.AddRange(tags);
            await context.SaveChangesAsync();
        }

        // Create sample workflow template
        if (!context.WorkflowTemplates.Any())
        {
            var workflow = new WorkflowTemplate
            {
                Name = "Document Approval",
                Description = "Standard document approval workflow",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            context.WorkflowTemplates.Add(workflow);
            await context.SaveChangesAsync();

            var managerRole = await roleManager.FindByNameAsync("Manager");
            var adminRole = await roleManager.FindByNameAsync("Administrator");

            if (managerRole != null && adminRole != null)
            {
                var steps = new[]
                {
                    new WorkflowStep
                    {
                        WorkflowTemplateId = workflow.Id,
                        Name = "Manager Review",
                        StepOrder = 1,
                        AssignedToRoleId = managerRole.Id,
                        RequireAllApprovers = false
                    },
                    new WorkflowStep
                    {
                        WorkflowTemplateId = workflow.Id,
                        Name = "Administrator Approval",
                        StepOrder = 2,
                        AssignedToRoleId = adminRole.Id,
                        RequireAllApprovers = false
                    }
                };

                context.WorkflowSteps.AddRange(steps);
                await context.SaveChangesAsync();
            }
        }
    }
}
