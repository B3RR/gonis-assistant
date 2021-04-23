﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gonis.Assistant.Blazor.Errors
{
    public interface IErrorComponent
    {
        void ShowError(string title, string message);

        void HideError();
    }
}
