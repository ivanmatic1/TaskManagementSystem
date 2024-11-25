﻿using System;
using System.Collections.Generic;

namespace TaskManagementSystem.Models
{
    public class ProjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string OwnerId { get; set; }
        public string OwnerName { get; set; }
        public List<string> MemberIds { get; set; } = new List<string>(); 
    }

}
