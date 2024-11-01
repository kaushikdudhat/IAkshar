using System;
using System.Collections.Generic;

namespace iAkshar.Models;

public partial class Suggestions
{
    public int Id { get; set; }
    public string Note { get; set; }
    public DateTime CreatedOn { get; set; }
    public int CreatedBy { get; set; }
}
