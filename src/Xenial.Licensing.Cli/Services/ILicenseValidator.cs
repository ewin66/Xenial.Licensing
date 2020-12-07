﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace Xenial.Licensing.Cli.Services
{
    public interface ILicenseValidator
    {
        public Task<bool> IsValid(string licString, string? publicKey = null);
    }
}
