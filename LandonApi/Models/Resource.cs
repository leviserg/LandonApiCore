﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LandonApi.Models
{
    public abstract class Resource : Link
    {
        [JsonIgnore]
        public Link Selfhref { get; set; }
    }
}
