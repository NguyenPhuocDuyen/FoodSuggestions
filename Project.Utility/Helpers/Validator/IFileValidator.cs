using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Utility.Helpers.Validator
{
    public interface IFileValidator
    {
        bool IsValid(IFormFile file);
    }
}
