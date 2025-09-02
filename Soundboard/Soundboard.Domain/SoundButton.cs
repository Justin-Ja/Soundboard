#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Soundboard.Domain.DataAccess.Implementations;

public class SoundButton : DomainObject
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public string HotKey { get; set; }
    public Guid GridId { get; set; }
    public SoundButtonGridLayout GridLayout { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int? BoundKeyCode { get; set; } 
    public int? BoundKeyModifiers { get; set; }
}

