# TaskManager Project Description
## Project Overview
TaskManager is a C# console application that provides a comprehensive task management system with a command-line interface. The application uses Entity Framework Core with SQLite for data persistence, allowing users to manage tasks and categories efficiently.

## Key Features and Functionality
### Task Management
- Create Tasks : Add new tasks with title, description, category, due date, and priority level
- View Tasks : Display all tasks with their details including completion status
- Update Tasks : Modify existing task details including title, description, category, due date, and priority
- Delete Tasks : Remove tasks from the system with confirmation
- Mark as Complete : Change the status of tasks to completed
- Priority Levels : Assign High, Medium, or Low priority to tasks
- Due Dates : Set and manage task deadlines
### Category Management
- Create Categories : Add new categories to organize tasks
- View Categories : Display all available categories
- Task Categorization : Assign tasks to specific categories
- Filter by Category : View tasks belonging to a specific category
### Additional Features
- Calendar View : Display tasks in a monthly calendar format
- Data Persistence : All data is stored in a SQLite database
- Nullable Properties : Support for optional task properties (due date, priority)
### Technical Implementation
- Entity Framework Core : Used for ORM and database operations
- SQLite Database : Lightweight database for storing task and category data
- Model-First Approach : Defined data models with proper relationships
- Nullable Reference Types : Proper handling of null values
- LINQ : Used for querying and filtering tasks
## Architecture
- Models : Task and Category classes defining the data structure
- Data Context : TaskManagerContext for database operations
- Program Logic : Console-based UI and business logic in Program.cs
