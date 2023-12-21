using System.Data.SqlTypes;
using System.Security.Authentication;
using infrastructure.dataModels;
using infrastructure.models;
using infrastructure.repository;
using Microsoft.Extensions.Logging;
using service.services.Password;

namespace service.services;

public class AccountService
{
    private readonly PasswordHashRepository _passwordHashRepository;
    private readonly UserRepository _userRepository;
    private readonly NotificationRepository _notificationRepository;

    public AccountService(UserRepository userRepository,
        PasswordHashRepository passwordHashRepository,
        NotificationRepository notificationRepository)
    {
        _userRepository = userRepository;
        _passwordHashRepository = passwordHashRepository;
        _notificationRepository = notificationRepository;
    }

    public User Authenticate(LoginModel model)
    {
        var passwordHash =
            _passwordHashRepository.GetById(model.Email); //gets the hash from database and authenticates it  
        if (ReferenceEquals(passwordHash, null)) throw new KeyNotFoundException();

        var hashAlgorithm = PasswordHashAlgorithm.Create(passwordHash.Algorithm);
        var isValid = hashAlgorithm.VerifyHashedPassword(model.Password, passwordHash.Hash, passwordHash.Salt);

        if (!isValid) throw new InvalidCredentialException("Invalid credential!");
        
        var user = _userRepository.GetByEmail(model.Email);
        if (ReferenceEquals(user, null)) throw new KeyNotFoundException();
        return user;

    }

    public User Register(RegisterModel model)
    {
        var user = _userRepository.Create(model, DateTime.Now); //creates the user 
        if (ReferenceEquals(user, null)) throw new SqlTypeException(" Create user");

        var hashAlgorithm = PasswordHashAlgorithm.Create(); //chooses hashing algorithm and hashes password
        var salt = hashAlgorithm.GenerateSalt();
        var hash = hashAlgorithm.HashPassword(model.Password, salt);
        var password = new PasswordHash
        {
            Id = user.Id,
            Hash = hash,
            Salt = salt,
            Algorithm = hashAlgorithm.GetName()
        };

        var isCreated = _passwordHashRepository.Create(password); //stores the password
        if (isCreated == false) throw new SqlTypeException("Could not Create user");

        if (!SetNotificationSettings(user)) { throw new SqlTypeException("Could not set Notification settings"); }
        return user; 
    }

    private bool SetNotificationSettings(User user)
    {
        var userSettings = new NotificationSettingsDto
        {
            UserId = user.Id,
            InviteNotification = false,
            InviteNotificationEmail = false,
            ExpenseNotification = false,
            ExpenseNotificationEmail = true
        };

        bool createdSuccessfully = _notificationRepository.CreateUserNotificationSettings(userSettings);
        Console.Write("you have created:" + createdSuccessfully);
        return createdSuccessfully;
    }
}