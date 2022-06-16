
## Passion Project - 2022 - ASP.NET

**This project is created for a Hair Salon to manage employee & service information and for booking apointments**

### Setting up the databse:
- Create database locally by running Update-Database command in Package Manager Console
- Tools => Nuget Package Manager => Package Manage Console => type: Update-Database
---
### Models:
1. Employee
2. Service
3. Appointment
---
### Relationships:
- Employee is the person providing the service.
- Service in current context is Hair Cut, Hair colour etc.
- Appointment can be booked by a customer by selecting the service, employee and the date and time of appointment.
- An Employee can provide multiple service.
- A service can be provided by multiple employeed.
---
### Features:
- Authentication
1. Only Admin can perform add, update and delete operations on Employees & Services.
2. String token is passed to API for authentication.

- Employee
1. User can add an Employee (Admin only)
2. User can edit Employee Information (Admin only)
3. User can upload employee picture (Admin only)
4. User can see list of Employees.
5. On edit page admin can add a service to an employee or remove a service from an employee.
6. User can view Employee details along with the appointments list.
7. User can preview image before update.

- Service
1. User can add a Service (Admin only)
2. User can edit a Service (Admin only)
3. User can delete a Service (Admin only)
4. User can see list of services provided.
5. User can view Service details.

- Appointment
1. Guest user can book an appointment.
2. Employee list is populated based on the service selected by the user.
3. Appointment end time is calculated based on the duration saved in the service.
4. User can see up coming appointments on employee details page.
---
### Additional Features:
The application is made responsive to suppport multiple screen sizes.


