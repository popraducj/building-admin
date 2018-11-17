using System;

namespace BuildingAdmin.DataLayer.Models
{

    public enum EmailTypeEnum {
        Registration,
        EmailConfirmation,
        ForgotPassword
    }

    public enum UserStatesEnum {
        NotConfirmed,
        Blocked,
        Active,
        Deleted
    }
}