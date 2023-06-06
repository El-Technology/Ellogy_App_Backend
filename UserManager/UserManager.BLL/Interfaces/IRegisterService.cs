﻿using UserManager.BLL.Dtos;

namespace UserManager.BLL.Interfaces;

public interface IRegisterService
{
    public Task RegisterUserAsync(UserRegisterRequestDto userRegister);
}