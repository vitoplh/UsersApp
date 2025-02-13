namespace UsersApp.Services;

public enum UserServiceResult
{
    Success,
    Failed,
    NotFound,
    AlreadyExists,
    EmailInUse
}