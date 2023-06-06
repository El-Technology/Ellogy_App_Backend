﻿// ReSharper disable UnusedAutoPropertyAccessor.Global
#pragma warning disable CS8618
namespace UserManager.BLL.Dtos;

public class UserRegisterRequestDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string Organization { get; set; }
    public string Department { get; set; }
}