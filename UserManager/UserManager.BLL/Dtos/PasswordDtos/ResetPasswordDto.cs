﻿namespace UserManager.BLL.Dtos.PasswordDtos;

public class ResetPasswordDto
{
    public Guid UserId { get; set; }
    public string Token { get; set; }
    public string Password { get; set; }
}
