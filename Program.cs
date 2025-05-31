using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using TaskManager.Data;
using TaskManager.Models;

namespace TaskManager
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context = new TaskManagerContext())
            {
                // Ensure database is created
                context.Database.EnsureCreated();
            }

            bool exit = false;
            while (!exit)
            {
                Console.Clear();
                Console.WriteLine("Task Manager");
                Console.WriteLine("------------");
                Console.WriteLine("1. Add Task");
                Console.WriteLine("2. View Tasks");
                Console.WriteLine("3. Update Task");
                Console.WriteLine("4. Mark Task as Complete");
                Console.WriteLine("5. Delete Task"); // New option
                Console.WriteLine("6. View Categories");
                Console.WriteLine("7. Add Category");
                Console.WriteLine("8. View Tasks by Category");
                Console.WriteLine("9. View Calendar (Bonus)"); // Adjusted option number
                Console.WriteLine("0. Exit");
                Console.Write("Select an option: ");

                string? choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AddTask();
                        break;
                    case "2":
                        ViewTasks();
                        break;
                    case "3":
                        UpdateTask();
                        break;
                    case "4":
                        MarkTaskAsComplete();
                        break;
                    case "5": // New case
                        DeleteTask();
                        break;
                    case "6":
                        ViewCategories();
                        break;
                    case "7":
                        AddCategory();
                        break;
                    case "8":
                        ViewTasksByCategory();
                        break;
                    case "9": // Adjusted case number
                        ViewCalendar();
                        break;
                    case "0":
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option. Press any key to continue...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        static void AddTask()
        {
            Console.Clear();
            Console.WriteLine("Add New Task");
            Console.WriteLine("------------");

            using (var context = new TaskManagerContext())
            {
                Console.Write("Title: ");
                string title = Console.ReadLine() ?? string.Empty;

                Console.Write("Description: ");
                string description = Console.ReadLine() ?? string.Empty;

                ViewCategories(false); // Display categories for selection
                Console.Write("Category ID: ");
                if (!int.TryParse(Console.ReadLine(), out int categoryId) || !context.Categories.Any(c => c.Id == categoryId))
                {
                    Console.WriteLine("Invalid Category ID. Task not added.");
                    PressAnyKeyToContinue();
                    return;
                }

                Console.Write("Due Date (YYYY-MM-DD, optional): ");
                DateTime? dueDate = null;
                string? dueDateStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(dueDateStr) && DateTime.TryParse(dueDateStr, out DateTime parsedDate))
                {
                    dueDate = parsedDate;
                }

                Console.Write("Priority (1-High, 2-Medium, 3-Low, optional): ");
                int? priority = null;
                string? priorityStr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(priorityStr) && int.TryParse(priorityStr, out int parsedPriority))
                {
                    priority = parsedPriority;
                }

                var newTask = new Models.Task
                {
                    Title = title,
                    Description = description,
                    CategoryId = categoryId,
                    IsCompleted = false,
                    DueDate = dueDate,
                    Priority = priority
                };

                context.Tasks.Add(newTask);
                context.SaveChanges();
                Console.WriteLine("Task added successfully!");
            }
            PressAnyKeyToContinue();
        }

        static void ViewTasks(bool clearConsole = true)
        {
            if (clearConsole) Console.Clear();
            Console.WriteLine("View Tasks");
            Console.WriteLine("----------");

            using (var context = new TaskManagerContext())
            {
                var tasks = context.Tasks.Include(t => t.Category).OrderBy(t => t.DueDate).ThenBy(t => t.Priority).ToList();
                if (!tasks.Any())
                {
                    Console.WriteLine("No tasks found.");
                }
                else
                {
                    foreach (var task in tasks)
                    {
                        Console.WriteLine($"ID: {task.Id}");
                        Console.WriteLine($"  Title: {task.Title}");
                        Console.WriteLine($"  Description: {task.Description}");
                        Console.WriteLine($"  Category: {task.Category?.Name ?? "N/A"}");
                        Console.WriteLine($"  Completed: {(task.IsCompleted ? "Yes" : "No")}");
                        Console.WriteLine($"  Due Date: {task.DueDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
                        Console.WriteLine($"  Priority: {PriorityHelper.GetPriorityName(task.Priority)}");
                        Console.WriteLine();
                    }
                }
            }
            if (clearConsole) PressAnyKeyToContinue();
        }

        static void UpdateTask()
        {
            Console.Clear();
            ViewTasks(false);
            Console.WriteLine("Update Task");
            Console.WriteLine("-----------");

            Console.Write("Enter ID of task to update: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine("Invalid ID format.");
                PressAnyKeyToContinue();
                return;
            }

            using (var context = new TaskManagerContext())
            {
                var taskToUpdate = context.Tasks.Find(taskId);
                if (taskToUpdate == null)
                {
                    Console.WriteLine("Task not found.");
                    PressAnyKeyToContinue();
                    return;
                }
                else
                {
                    Console.Write($"New Title (current: {taskToUpdate.Title}): ");
                    string? newTitle = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newTitle))
                    {
                        taskToUpdate.Title = newTitle;
                    }

                    Console.Write($"New Description (current: {taskToUpdate.Description}): ");
                    string? newDescription = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newDescription))
                    {
                        taskToUpdate.Description = newDescription;
                    }

                    ViewCategories(false);
                    Console.Write($"New Category ID (current: {taskToUpdate.CategoryId}): ");
                    string? newCategoryIdStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newCategoryIdStr) && int.TryParse(newCategoryIdStr, out int newCategoryId) && context.Categories.Any(c => c.Id == newCategoryId))
                    {
                        taskToUpdate.CategoryId = newCategoryId;
                    }
                    else if (!string.IsNullOrWhiteSpace(newCategoryIdStr))
                    {
                        Console.WriteLine("Invalid Category ID. Category not changed.");
                    }

                    Console.Write($"New Due Date (YYYY-MM-DD, current: {taskToUpdate.DueDate?.ToString("yyyy-MM-dd") ?? "N/A"}): ");
                    string? newDueDateStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newDueDateStr) && DateTime.TryParse(newDueDateStr, out DateTime newDueDate))
                    {
                        taskToUpdate.DueDate = newDueDate;
                    }
                    else if (string.IsNullOrWhiteSpace(newDueDateStr) && taskToUpdate.DueDate.HasValue)
                    {
                         // Allow clearing the due date
                         Console.Write("Clear Due Date? (y/n): ");
                         if((Console.ReadLine()?.Trim().ToLower() ?? "n") == "y") taskToUpdate.DueDate = null;
                    }

                    Console.Write($"New Priority (1-High, 2-Medium, 3-Low, current: {taskToUpdate.Priority?.ToString() ?? "N/A"}): ");
                    string? newPriorityStr = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(newPriorityStr) && int.TryParse(newPriorityStr, out int newPriority))
                    {
                        taskToUpdate.Priority = newPriority;
                    }
                     else if (string.IsNullOrWhiteSpace(newPriorityStr) && taskToUpdate.Priority.HasValue)
                    {
                         // Allow clearing the priority
                         Console.Write("Clear Priority? (y/n): ");
                         if((Console.ReadLine()?.Trim().ToLower() ?? "n") == "y") taskToUpdate.Priority = null;
                    }

                    context.SaveChanges();
                    Console.WriteLine("Task updated successfully!");
                }
            }
            PressAnyKeyToContinue();
        }

        static void MarkTaskAsComplete()
        {
            Console.Clear();
            ViewTasks(false);
            Console.WriteLine("Mark Task as Complete");
            Console.WriteLine("---------------------");

            Console.Write("Enter ID of task to mark as complete: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine("Invalid ID format.");
                PressAnyKeyToContinue();
                return;
            }

            using (var context = new TaskManagerContext())
            {
                var taskToComplete = context.Tasks.Find(taskId);
                if (taskToComplete == null)
                {
                    Console.WriteLine("Task not found.");
                    PressAnyKeyToContinue();
                    return;
                }
                else
                {
                    taskToComplete.IsCompleted = true;
                    context.SaveChanges();
                    Console.WriteLine("Task marked as complete!");
                }
            }
            PressAnyKeyToContinue();
        }

        static void DeleteTask()
        {
            Console.Clear();
            ViewTasks(false);
            Console.WriteLine("Delete Task");
            Console.WriteLine("-----------");

            Console.Write("Enter ID of task to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int taskId))
            {
                Console.WriteLine("Invalid ID format.");
                PressAnyKeyToContinue();
                return;
            }

            using (var context = new TaskManagerContext())
            {
                var taskToDelete = context.Tasks.Find(taskId);
                if (taskToDelete == null)
                {
                    Console.WriteLine("Task not found.");
                }
                else
                {
                    Console.Write($"Are you sure you want to delete task '{taskToDelete.Title}' (ID: {taskToDelete.Id})? (y/n): ");
                    string? confirmation = Console.ReadLine()?.Trim().ToLower();
                    if (confirmation == "y")
                    {
                        context.Tasks.Remove(taskToDelete);
                        context.SaveChanges();
                        Console.WriteLine("Task deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Task deletion cancelled.");
                    }
                }
            }
            PressAnyKeyToContinue();
        }

        static void ViewCategories(bool clearConsole = true)
        {
            if(clearConsole) Console.Clear();
            Console.WriteLine("Categories");
            Console.WriteLine("----------");
            using (var context = new TaskManagerContext())
            {
                var categories = context.Categories.ToList();
                if (!categories.Any())
                {
                    Console.WriteLine("No categories found.");
                }
                else
                {
                    foreach (var category in categories)
                    {
                        Console.WriteLine($"ID: {category.Id} - Name: {category.Name}");
                    }
                }
            }
            if(clearConsole) PressAnyKeyToContinue();
        }

        static void AddCategory()
        {
            Console.Clear();
            Console.WriteLine("Add New Category");
            Console.WriteLine("----------------");
            Console.Write("Category Name: ");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Category name cannot be empty.");
                PressAnyKeyToContinue();
                return;
            }

            using (var context = new TaskManagerContext())
            {
                if (context.Categories.Any(c => c.Name.ToLower() == name.ToLower()))
                {
                    Console.WriteLine("Category with this name already exists.");
                }
                else
                {
                    var newCategory = new Category { Name = name };
                    context.Categories.Add(newCategory);
                    context.SaveChanges();
                    Console.WriteLine("Category added successfully!");
                }
            }
            PressAnyKeyToContinue();
        }

        static void ViewTasksByCategory()
        {
            Console.Clear();
            ViewCategories(false);
            Console.Write("Enter Category ID to view tasks: ");
            if (!int.TryParse(Console.ReadLine(), out int categoryId))
            {
                Console.WriteLine("Invalid Category ID format.");
                PressAnyKeyToContinue();
                return;
            }

            using (var context = new TaskManagerContext())
            {
                var category = context.Categories.Include(c => c.Tasks).ThenInclude(t => t.Category)
                                       .FirstOrDefault(c => c.Id == categoryId);

                if (category == null)
                {                    
                    Console.WriteLine("Category not found.");
                    PressAnyKeyToContinue();
                    return;
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine($"Tasks in Category: {category.Name}");
                    Console.WriteLine("-----------------------------------");
                    if (!category.Tasks.Any())
                    {
                        Console.WriteLine("No tasks found in this category.");
                    }
                    else
                    {
                        foreach (var task in category.Tasks.OrderBy(t => t.DueDate).ThenBy(t => t.Priority))
                        {
                            Console.WriteLine($"ID: {task.Id}");
                            Console.WriteLine($"  Title: {task.Title}");
                            Console.WriteLine($"  Description: {task.Description}");
                            Console.WriteLine($"  Completed: {(task.IsCompleted ? "Yes" : "No")}");
                            Console.WriteLine($"  Due Date: {task.DueDate?.ToString("yyyy-MM-dd") ?? "N/A"}");
                            Console.WriteLine($"  Priority: {PriorityHelper.GetPriorityName(task.Priority)}");
                            Console.WriteLine();
                        }
                    }
                }
            }
            PressAnyKeyToContinue();
        }

        static void ViewCalendar()
        {
            Console.Clear();
            Console.WriteLine("CLI Calendar View (Bonus)");
            Console.WriteLine("-------------------------");

            Console.Write("Enter Year (e.g., 2023): ");
            if (!int.TryParse(Console.ReadLine(), out int year))
            {
                Console.WriteLine("Invalid year format.");
                PressAnyKeyToContinue();
                return;
            }

            Console.Write("Enter Month (1-12): ");
            if (!int.TryParse(Console.ReadLine(), out int month) || month < 1 || month > 12)
            {
                Console.WriteLine("Invalid month format.");
                PressAnyKeyToContinue();
                return;
            }

            DisplayCalendar(year, month);
            DisplayTasksForMonth(year, month);

            PressAnyKeyToContinue();
        }

        static void DisplayCalendar(int year, int month)
        {
            Console.WriteLine($"\n{new DateTime(year, month, 1):MMMM yyyy}");
            Console.WriteLine("Su Mo Tu We Th Fr Sa");

            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            int startingDayOfWeek = (int)firstDayOfMonth.DayOfWeek; // 0 for Sunday, 1 for Monday, ...

            // Print leading spaces
            for (int i = 0; i < startingDayOfWeek; i++)
            {
                Console.Write("   ");
            }

            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                Console.Write($"{day,2} ");
                if ((day + startingDayOfWeek) % 7 == 0)
                {
                    Console.WriteLine();
                }
            }
            Console.WriteLine("\n");
        }

        static void DisplayTasksForMonth(int year, int month)
        {
            Console.WriteLine("Tasks for this month:");
            using (var context = new TaskManagerContext())
            {
                var tasksInMonth = context.Tasks
                    .Include(t => t.Category)
                    .Where(t => t.DueDate.HasValue && t.DueDate.Value.Year == year && t.DueDate.Value.Month == month)
                    .OrderBy(t => t.DueDate)
                    .ToList();

                if (!tasksInMonth.Any())
                {
                    Console.WriteLine("No tasks scheduled for this month.");
                }
                else
                {
                    foreach (var task in tasksInMonth)
                    {
                        Console.WriteLine($"- {task.DueDate?.ToString("dd")}: {task.Title} ({(task.IsCompleted ? "Completed" : "Pending")}, Prio: {PriorityHelper.GetPriorityName(task.Priority)}, Cat: {task.Category?.Name ?? "N/A"})");
                    }
                }
            }
        }

        static void PressAnyKeyToContinue()
        {
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
        }

        public enum PriorityLevel
        {
            High = 1,
            Medium = 2,
            Low = 3
        }
    }

    public static class PriorityHelper
    {
        public static string GetPriorityName(int? priorityValue)
        {
            if (!priorityValue.HasValue) return "N/A";
            
            return ((Program.PriorityLevel)priorityValue.Value).ToString();
        }
    }
}
