using System;
using System.Collections.Generic;
using BuildingAdmin.DataLayer.Models;

namespace BuildingAdmin.Tests.Helper
{
    public static class HelperObjects
    {

        public static UserModel GetUser(UserStatesEnum state = UserStatesEnum.Active){
            return new UserModel(){
                Password = "encodedPassword",
                Salt = "salt",
                State = state,
                FirstName = "test",
                LastName = "test",
                CreatedAt = DateTime.Now,
                Id = HelperConstants.TestEmailAddress,
                Roles = new List<string>()
            };
        }
    }
}