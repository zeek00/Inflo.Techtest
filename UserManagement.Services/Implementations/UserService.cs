using System;
using System.Collections.Generic;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

using System.Linq;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)     
    {
        // filters using the isActive property
        try
        {
            var allUsers = _dataAccess.GetAll<User>();
            var filteredUsers = allUsers.Where(user => user.IsActive == isActive);

            return filteredUsers.ToList();

        }
        catch(Exception ex)
        {
            Console.WriteLine("Error occured while filtering users: " + ex.Message);
            throw;
        }
            
    }

    public void DeleteUser(User user)
    {
        try
        {
            _dataAccess.Delete(user); // Assuming you have a method to delete the user in your data context
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error occurred while deleting the user: " + ex.Message);
            throw;
        }
        
    }


    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();
}
