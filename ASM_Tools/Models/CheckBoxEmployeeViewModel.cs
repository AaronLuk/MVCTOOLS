﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASM_Tools.Models
{
    public class CheckBoxEmployeeViewModel
    {
        public int Id { get; set; }
        public Tool tool { get; set; }
        public bool Checked { get; set; }
        public string Role { get; set; }
    }
}