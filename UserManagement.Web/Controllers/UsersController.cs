using System.Diagnostics;
using System.Linq;
using Serilog;
using Serilog.Context;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Web.Models.Users;

namespace UserManagement.WebMS.Controllers;

[Route("users")]
public class UsersController : Controller
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public ViewResult List()
    {
        var items = _userService.GetAll().Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = items.ToList()
        };

        return View(model);
    }

    [HttpGet("active")]
    public ViewResult ActiveUsers()
    {
        var activeUsers = _userService.FilterByActive(isActive: true);
        var activeUserItems = activeUsers.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = activeUserItems.ToList(),
        };

        return View("List", model);
    }

    [HttpGet("nonactive")]
    public ViewResult NonActiveUsers()
    {
        var nonActiveUsers = _userService.FilterByActive(isActive: false);
        var nonActiveUserItems = nonActiveUsers.Select(p => new UserListItemViewModel
        {
            Id = p.Id,
            Forename = p.Forename,
            Surname = p.Surname,
            Email = p.Email,
            IsActive = p.IsActive,
            DateOfBirth = p.DateOfBirth
        });

        var model = new UserListViewModel
        {
            Items = nonActiveUserItems.ToList(),
        };

        return View("List",model);
    }

    // New action to display the user creation form
    [HttpGet("create")]
    public ViewResult Create()
    {
        var model = new UserFormViewModel();
        return View(model);
    }

    // New action to handle the user creation form submission
    [HttpPost("create")]
    public IActionResult CreateUser(UserFormViewModel userViewModel)
    {
        if (ModelState.IsValid)
        {
            // Creating a new User object and assigning it with the data from userViewModel
            var newUser = new User
            {
                Forename = userViewModel.Forename,
                Surname = userViewModel.Surname,
                Email = userViewModel.Email,
                IsActive = userViewModel.IsActive,
                DateOfBirth = userViewModel.DateOfBirth
            };

            LogContext.PushProperty("UserId", newUser.Id);
            Log.Information("User {UserId} created a new user with email {Email}.", newUser.Id, newUser.Email);

            // Add the new user to the database 
            DataContext dataContext = new DataContext();
            dataContext.Create(newUser);

            // Save the changes to the database
            dataContext.SaveChanges();

            // Redirect back to the User list after creation
            return RedirectToAction("List");
        }

       
        return View(userViewModel);
    }

    [HttpGet("details/{id}")]
    public IActionResult Details(long id)
    {
        // Retrieve user details by id 
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);

            if (user == null)
        {
            // Return a 404 Not Found response if the user is not found
            return NotFound(); 
        }

        var model = new UserDetailsViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        // Return the user details view with the model
        return View(model); 
    }
    [HttpGet("edit/{id}")]
    public IActionResult Edit(long id)
    {
        // Retrieve user details by id
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            // Return a 404 Not Found response if the user is not found
            return NotFound(); 
        }

        LogContext.PushProperty("UserId", id);
        Debug.WriteLine($"User ID for editing: {id}");

        // logger action
        Log.Information("User {UserId} is editing the user with ID {UserId}.at Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}", id, user.Id);
       

        var model = new UserEditViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        // Return the user edit view with the model
        return View(model);
    }
    
    [HttpPost("edit/{id}")]
    public IActionResult Edit(long id, UserEditViewModel model)
    {
        if (id != model.Id)
        {
            // Return a 400 response if the IDs don't match
            return NotFound(); 
        }

        if (!ModelState.IsValid)
        {
            // Return to the edit view with validation errors
            return View(model); 
        }

        // Retrieve user details by id
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            return NotFound(); 
        }


        LogContext.PushProperty("UserId", id);

        //log the action
        Log.Information("User {UserId} edited the user with ID {UserId} at Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}.", id, user.Id);
      


        // Update the user's information with the data from the model

        user.Forename = model.Forename;
        user.Surname = model.Surname;
        user.Email = model.Email;
        user.IsActive = model.IsActive;
        user.DateOfBirth = model.DateOfBirth;

        DataContext dataContext = new DataContext();

        dataContext.Update(user);

        // Save the changes to the database

        dataContext.SaveChanges();

        
        
        //  success message
        TempData["Message"] = "User updated successfully";

        // Redirecting back to the user list after editing
        return RedirectToAction("List"); 
    }


    [HttpGet("delete/{id}")]
    public IActionResult Delete(long id)
    {
        // Retrieve user details by id
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);

        
        if (user == null)
        {
           return NotFound();
        }

        var model = new UserDeleteViewModel
        {
            Id = user.Id,
            Forename = user.Forename,
            Surname = user.Surname,
            Email = user.Email,
            IsActive = user.IsActive,
            DateOfBirth = user.DateOfBirth
        };

        return View(model);
    }


    [HttpPost("delete/{id}")]
    public IActionResult DeleteConfirmed(long id)
    {
        // Retrieve user details by id
        var user = _userService.GetAll().FirstOrDefault(u => u.Id == id);

        if (user == null)
        {
            // Return a 404 Not Found response if the user is not found
            return NotFound(); 
        }


        LogContext.PushProperty("UserId", id);
        Log.Information("User {UserId} deleted the user with ID {UserId} at Timestamp: {Timestamp:yyyy-MM-dd HH:mm:ss}.", id, user.Id);
        

        // Perform user deletion 
        _userService.DeleteUser(user);

        TempData["Message"] = "User deleted successfully";

        // Redirect back to the user list after deletion
        return RedirectToAction("List"); 
    }

}
