﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CinemaSchedule.Areas.Identity.Data;

public class User : IdentityUser
{
    public byte[]? ProfilePicture { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string? cinemaID { get; set; }
    public bool CanRead { get; set; } = false;
    public bool CanWrite { get; set; } = false;
}

